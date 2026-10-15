# WorkflowEditor 流程编辑器

[English](../en/workflow-editor.md) · [目录](README.md)

`WorkflowEditor` 绑定 `WorkflowDocument`，编辑节点、命名端口、连线与业务字段引用。控件内置默认模板，宿主提供节点工厂、业务数据、持久化和执行逻辑，并可按需替换模板；控件不内置审批引擎、HTTP 或脚本执行。

## 公共契约

模型位于 `TheOne.Avalonia.Workflow`，与可视控件分离。

| 类型 | 职责 |
| --- | --- |
| WorkflowDocument | 可观察 Nodes / Connections、标题、修订号、编辑事务和撤销重做 |
| WorkflowNodeDefinition | 宿主注册的 TypeId、标题、描述、分类、CreateNode 工厂 |
| WorkflowNode | 稳定 Id / TypeId、标题、说明、逻辑坐标、Width / Shape、注释标记、Ports、Properties、State 和展示 Content |
| WorkflowPort | 节点内稳定 ID、方向、DataType、最大连接数；非正数表示不限 |
| WorkflowConnection | 稳定连线 ID、来源节点/端口 ID、目标节点/端口 ID、标签 |
| WorkflowProperty | Key、标题、DataType、Text/Number/Boolean、必填标记、字符串固定值或 Reference |
| WorkflowDataReference | NodeId + OutputPortId + FieldPath，不依赖显示名称 |
| WorkflowOutputField | 宿主声明的输出字段路径、名称及类型 |

Number / Boolean 固定值以字符串保存，按不变区域性数值和布尔语义校验。DataType 是宿主 token（如 email），不会反射查找 CLR 类型。端口及输出描述按不可变记录使用，变更时替换 Ports 条目。ID 不随重命名和排序变化。

## 消费端接入

```xml
<Grid>
<one:WorkflowEditor Document="{Binding Document, Mode=TwoWay}"
                    NodeDefinitions="{Binding NodeDefinitions}"
                    SelectedNode="{Binding SelectedNode, Mode=TwoWay}"
                    SaveCommand="{Binding SaveCommand}" />
</Grid>
```

消费端声明具体 ViewModel 和正确 x:DataType，保持编译绑定。单个 Document 同时包含节点和边，避免替换多个数据源时的不一致。控件默认在有限父容器中拉伸，不固定宽高或最小高度；推荐放入 Grid / DockPanel 的剩余空间。无界 StackPanel 或外部纵向滚动容器无法提供可铺满的高度。

```csharp
var document = new WorkflowDocument();
var source = new WorkflowNode { TypeId = "request", Title = "提交申请" };
source.Ports.Add(new("out", "申请数据", WorkflowPortDirection.Output,
    "flow", 0, [new("applicant.email", "申请人邮箱", "email")]));
var target = new WorkflowNode { TypeId = "notification", Title = "通知", X = 300 };
target.Ports.Add(new("in", "输入", WorkflowPortDirection.Input));
target.Properties.Add(new() {
    Key = "recipient", Title = "收件人", DataType = "email", IsRequired = true,
    Reference = new(source.Id, "out", "applicant.email")
});
document.AddNode(source);
document.AddNode(target);
document.TryConnect(source.Id, "out", target.Id, "in", out var error);
```

NodeDefinitions 由宿主注册；工厂返回独立新节点、唯一 ID 和匹配 TypeId。节点库支持搜索、按 Category 分组、点击添加及拖入画布，也可调用 AddNode(definition, position)。NodeDefinitionTemplate 接收定义，使用者可提供带图标的库条目。示例将图中的审批流程及图标作为消费端数据提供。

## 业务数据关联

连线只表达图关系，不会自动传递全部数据。GetReferenceOptions(node, property) 按上游可达关系与字段 DataType 筛选声明输出，字段 any 接受所有类型。宿主根据引用三元组查输出表或调用显式编译 getter；FieldPath 不作为反射路径或表达式执行。

示例“解析数据关联”使用宿主输出字典，将提交申请的金额和邮箱分别提供给审批金额、通知收件人。再次解析会读取当前数据源值。

## 模板

- 控件内置完整外框、标题栏、节点库、画布、属性面板、工具栏与缩略图，无需消费端提供模板即可使用。
- HeaderTemplate 接收 WorkflowEditor，默认展示文档标题、校验和保存按钮；可设置为 null 隐藏标题栏，也可用 IDataTemplate 替换。
- NodeTemplate 接收 WorkflowNode，默认展示标题、说明与状态。端口和选择边框保留在模板外。
- NodeDefinitionTemplate 接收 WorkflowNodeDefinition，默认在节点库展示标题与说明。
- ContentTemplate 接收节点 Content，可使用任意具体业务类型和编译绑定。Content 不序列化，加载后由宿主按 TypeId 重建。
- PropertyTemplate 接收选中节点。默认使用内置 TabStrip 展示基础、数据和端口页，支持编辑名称、说明、固定值与上游引用，修改参与文档撤销历史，并遵循 IsReadOnly。SelectedNodeEditor 暴露 WorkflowNodeEditor / WorkflowPropertyEditor，方便自定义模板复用编辑逻辑。业务专用表单可替换此模板，也可隐藏面板后在外部绑定 SelectedNode。模板自行提供滚动容器，默认删除按钮固定在滚动区域外。
- SelectedConnection 提供选中边，宿主可在外部编辑 Label 或业务条件；默认面板显示节点属性。

示例强类型配置行使用 ComboBox 切换固定值/上游引用，通过 Document.SetProperty 和 Edit 参与图编辑的同一撤销历史。加载和复制后，宿主按 TypeId 重建 Content，避免把展示对象写入文件或与原节点共享可变业务对象。

## 编辑与校验

所有模型和控件更新在 UI 线程进行。集合/属性通知刷新视图；替换 Document 解除旧订阅并清空选择。直接宿主数据更新会清除旧历史，需要撤销时使用 Edit(title, redo, undo)，回调应确定且不抛异常。展示 Content 和运行 State 的更新不改变保存修订号或清除历史。

支持节点拖动、网格吸附、框选、Shift 多选、方向键微移（Shift=10）、Delete/Backspace 删除、Ctrl/Cmd+Z / Shift+Z 撤销重做、Ctrl/Cmd+C/V/D 编辑器内复制/粘贴/副本。复制生成新节点与边 ID，重映射内部数据引用，保留外部引用并在失效时报告。剪贴板仅在当前编辑器内使用。

从输出拖到输入连接，或 Tab/Enter 依次激活输出/输入按钮；Escape 取消。空格+拖动或中键平移；滚轮按指针缩放，范围 25%–200%。原生文本输入保留自己的快捷键。

选择/平移工具通过 IsPanMode 切换，对齐命令将所选节点顶部对齐。缩略图支持点击与拖动导航。FitPadding 控制适应画布的边距，示例为运行预览标记保留额外顶部空间。校验按钮打开可点击定位的问题列表。IsAnnotation 节点不计入状态栏执行节点数量。

IsReadOnly 禁止编辑器写入，允许选择和视口操作；自定义配置面板须由宿主禁用写操作，示例已演示。宿主仍可主动更新模型。ShowNodeLibrary / ShowProperties 由宿主控制折叠，适配窄视口。

连接默认拒绝缺失端点、方向/类型不匹配、重复、自连、数量超限及环路。AllowCycles 可显式放开环路，ConnectionValidator 增加宿主连接限制。Validate(allowCycles) 检查结构、字段及引用；具体业务与权限另由宿主校验。没有强制的业务“开始/结束”类型。

## 保存加载

`SaveCommand` 是可绑定的宿主 `ICommand`，默认保存按钮传入当前 `Document`，并遵循 `CanExecute` / `CanExecuteChanged`；未提供命令时按钮禁用。控件本身不弹出文件选择器或写文件。PreviewHost 的 ViewModel 通过异步命令和注入的 `IWorkflowEditorFileService` 完成保存；导出、打开也使用命令。取消或失败不标记已保存，异步保存旧快照期间的新修改仍保留未保存状态。

```csharp
string json = WorkflowSerializer.Save(document);
WorkflowDocument loaded = WorkflowSerializer.Load(json);
```

schema version=1，使用源生成 System.Text.Json 元数据。保存节点、端口、字段、引用、坐标、连线；排除 Content、模板、运行状态、服务、选择、视口和历史。额外数据可用宿主外层 DTO 保存。

不支持的版本、无效 ID/坐标被拒绝；草稿失效引用/端点保留并由 Validate 报告。宿主先解析新文档再替换绑定，示例解析失败保留原流程。

## 示例与验证

PreviewHost：**扩展控件 → WorkflowEditor**。主界面还原设计图的七节点审批流程、备注、图标、端口及状态；属性页签使用内置 `TabStrip`。节点库、工具栏、属性面板及示例操作区默认使用 Small 尺寸（正文与输入文字 12），画布保留宿主尺寸和节点比例。下方折叠区演示默认与自定义模板、强类型 Content、配置面板、引用选择/解析、只读、可编辑 JSON 及原生文件保存/打开。独立数据关联示例展示提交申请 → 审批 → 通知的收件人引用。

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/workflow-editor/artifacts
dotnet .preview/workflow-editor/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-workflow .preview/workflow-editor/render
```

NativeAotSmoke 支持 --verify-workflow。专项验证契约、JSON、编译模板、真实键鼠、明暗、三档尺寸和折叠面板后的窄视口。文件选择器需桌面实跑，headless 覆盖共享 JSON 路径。

初版为直接绘制圆角正交线与拓扑分层排列，不提供完整障碍避让、折点编辑或节点虚拟化。业务规则执行与输出求值由宿主负责。
