# WorkflowEditor 流程编辑器

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

[English](../en/workflow-editor.md) · [目录](README.md)

`WorkflowEditor` 绑定 `WorkflowDocument`，编辑节点、命名端口、连线与业务字段引用。控件内置默认模板，宿主提供节点工厂、业务数据、持久化和执行逻辑，并可按需替换模板；控件不内置审批引擎、HTTP 或脚本执行。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### WorkflowEditor

![WorkflowEditor — 左侧节点库、中央画布、右侧属性与小地图；审批示例使用业务模型和稳定身份。](../images/WorkflowEditor-light.png)

左侧节点库、中央画布、右侧属性与小地图；审批示例使用业务模型和稳定身份。

[查看 Dark 对照](../images/WorkflowEditor-dark.png)

<!-- visual-guide:end -->

## 公共契约

### 宿主连线工厂与节点保护

`ConnectionFactory` 为可选 `Func<WorkflowNode, WorkflowPort, WorkflowNode, WorkflowPort, WorkflowConnection?>`，参数依次是源节点、输出端口、目标节点、输入端口。工厂应只创建候选对象，不修改文档；可预置 `Label` 和 `Properties`，返回 `null` 拒绝创建。候选端点必须与用户请求一致，身份不能为空且不得重复；库仍检查方向、类型、数量、重复边和环路。`WorkflowDocument.TryConnect(WorkflowConnection, out string?, bool allowCycles = false)` 将已校验候选及其元数据一次性加入撤销历史。不要在 `DocumentEdited` 回调中直接补字段，这种外部变更会清空历史。

`NodeCreationValidator: Func<WorkflowNode,string?>?` 检查工具栏／节点库添加和粘贴候选，返回非 `null` 消息表示拒绝。粘贴只加入获准节点及两端都获准的连线，对被过滤节点的字段引用保留原身份。`NodeDeletionValidator` 使用同一签名，删除选择时保留被拒绝节点、允许删除其余选择；删除关联连线仍由文档统一处理。两项策略只约束编辑器入口，不是业务安全边界；宿主仍须验证提交数据，不应直接修改集合绕过编辑入口。默认不限制任何节点类型，库不认识开始／结束或审批业务标记。

`ConnectionPropertyTemplate: IDataTemplate?` 默认 `null`。提供模板且选中连线时，原有属性栏显示该模板，数据上下文为原 `WorkflowConnection`，标题为“连线属性”；选中节点仍使用 `PropertyTemplate`，上下文为原 `WorkflowNode`。宿主字段的 setter 应调用 `SetConnectionProperty` 或 `Document.Edit` 保持撤销一致，不需另建画布或固定属性栏。

序列化加载只接受具有显式 camelCase `nodes` 和 `connections` 键的有效 v1／v2 快照；`{}`、缺键、`null` 集合和不兼容版本均抛出 `JsonException`，显式空数组仍可表示空草稿。历史应用自己的 graphJson 格式必须先识别并转换，不能当作库 v1 快照直接读取。

模型位于 `TheOne.Avalonia.Workflow`，与可视控件分离。

| 类型 | 职责 |
| --- | --- |
| WorkflowDocument | 可观察 Nodes / Connections、标题、修订号、编辑事务和撤销重做 |
| WorkflowNodeDefinition | 宿主注册的 TypeId、标题、描述、分类、CreateNode 工厂 |
| WorkflowNode | 稳定 Id / TypeId、标题、说明、逻辑坐标、Width / Shape、注释标记、Ports、Properties、State 和展示 Content |
| WorkflowPort | 节点内稳定 ID、方向、DataType、最大连接数；非正数表示不限 |
| WorkflowConnection | 稳定连线 ID、来源节点/端口 ID、目标节点/端口 ID、标签 |
| WorkflowProperty | Key、标题、DataType、Text/Number/Boolean/MultiSelect、必填标记、固定值／Reference 或 SelectedKeys |
| WorkflowChoiceAdapter / WorkflowChoice | 编译期 Key／文本 getter，以及运行时选项展示模型 |
| WorkflowDataReference | NodeId + OutputPortId + FieldPath，不依赖显示名称 |
| WorkflowOutputField | 宿主声明的输出字段路径、名称及类型 |

Number / Boolean 固定值以字符串保存，按不变区域性数值和布尔语义校验。DataType 是宿主 token（如 email），不会反射查找 CLR 类型。端口及输出描述按不可变记录使用，变更时替换 Ports 条目。ID 不随重命名和排序变化。

## 消费端接入

```xml
<Grid>
<one:WorkflowEditor Document="{CompiledBinding Document, Mode=TwoWay}"
                    NodeDefinitions="{CompiledBinding NodeDefinitions}"
                    SelectedNode="{CompiledBinding SelectedNode, Mode=TwoWay}"
                    SaveCommand="{CompiledBinding SaveCommand}" />
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

## 审批人绑定与多选字段

审批人及其他身份列表使用 `WorkflowValueKind.MultiSelect`。将宿主数据绑定到 `WorkflowEditor.ChoiceItemsSource`，通过 `ChoiceItemAdapter` 的编译 getter 指定每项 Key 和显示文本的来源属性，不反射字符串成员路径。显示名称可以重复，Key 必须非空且在数据源中唯一。建议使用 `user:42`、`role:finance` 等稳定 Key 区分人员和角色，后续执行端按 Key 查询真实身份。选项及已选标签内置默认人员图标，无须提供图标属性。

```csharp
public sealed record ApprovalTarget(string Key, string DisplayName);
// 放在具备属性通知的 ViewModel 中；可整体替换，ObservableCollection 也支持增删。
private IEnumerable<ApprovalTarget> _approverItems = new ObservableCollection<ApprovalTarget>
{
    new("user:42", "张敏"), new("role:finance", "财务审批组")
};
public IEnumerable<ApprovalTarget> ApproverItems
{
    get => _approverItems;
    set => Set(ref _approverItems, value); // ViewModel 继承 WorkflowObservable，或使用自己的通知基类
}
public WorkflowChoiceAdapter ApproverAdapter { get; } =
    WorkflowChoiceAdapter.Create<ApprovalTarget>(item => item.Key, item => item.DisplayName);

var field = new WorkflowProperty
{
    Key = "approvers", Title = "审批人", DataType = "principal",
    Kind = WorkflowValueKind.MultiSelect, IsRequired = true
};
approvalNode.Properties.Add(field);
document.SetSelectedKeys(field, ["user:42", "role:finance"]); // 一次可撤销编辑
```

```xml
<one:WorkflowEditor Document="{CompiledBinding Document, Mode=TwoWay}"
    ChoiceItemsSource="{CompiledBinding ApproverItems, Mode=TwoWay}"
    ChoiceItemAdapter="{CompiledBinding ApproverAdapter}" />
```

默认属性栏在“数据”页显示多选框。界面选择写回 `WorkflowProperty.SelectedKeys`，宿主修改该可观察集合也会同步界面。需要保留撤销记录时，使用 `document.SetSelectedKeys(field, keys)`；直接修改集合沿用已有的“外部直接修改模型会重置历史”规则。候选项增删、整体替换及可通知模型的名称变更只更新展示，不修改流程或清空历史；整体替换数据源时需通知源属性。选中 Key 存在各节点自己的属性中，切换节点不会共用一份审批人选择。

自定义属性模板可复用 `WorkflowPropertyEditor`：调用 `ConfigureChoices(source, adapter)`，将 `MultiSelectBox` 绑定到 `Choices` 和 `SelectedChoices`（`Mode=TwoWay`）。`SelectedChoices` 内是含 `Key`、`Text`、`IsAvailable` 的 `WorkflowChoice` 模型。独立创建的字段编辑器不用后应释放；WorkflowEditor 会释放自己持有的编辑器。演示页审批人栏使用此方式，复用内置文本适配器和默认人员模板。

PreviewHost 的基础审批人栏、规则页业务字段和关联示例均复用同一字段编辑器行为：多选字段隐藏固定值／引用输入，显示 `ChoiceError` 与缺失 Key 提示，每个多选框显式设置 `MaxVisibleTags="1"`，折叠标签不影响实际选择。自定义模板独立于默认模板，修改一处实例的控件属性不会自动应用到其他实例。只读时禁用字段写入，保留属性页签和滚动浏览；节点名称直接双向绑定 `SelectedNodeEditor.Title`，即时写入文档并随撤销／重做刷新，不另存失焦提交草稿。字段集合替换及切换节点时释放旧编辑器，按当前属性重新绑定。审批方式、超时范围和单位等为演示业务配置，仍由宿主管理。

数据源过滤、移除或替换后找不到的已选 Key 不会静默删除，仍保留并显示为待解析项，可移除；对应项重新出现时恢复名称。候选 Key 为空或重复时显示 `ChoiceError`，不猜测身份；必填多选为空时校验失败。搜索保留未显示的选择，只读模式阻止界面编辑。JSON 以显式 `selectedKeys` 字符串数组保存；加载、撤销／重做、复制／粘贴保留准确 Key，不保存显示文本、数据源对象或委托。执行端自行按 Key 查询人员／角色，控件不执行审批规则。旧 Text/Number/Boolean 枚举值保持不变，旧版库不能读取新增的 `MultiSelect = 3`。演示会将旧单值 `roleId` 迁移成单项选择；其他业务数据迁移由宿主明确处理。

## 业务数据关联

连线只表达图关系，不会自动传递全部数据。GetReferenceOptions(node, property) 按上游可达关系与字段 DataType 筛选声明输出，字段 any 接受所有类型。宿主根据引用三元组查输出表或调用显式编译 getter；FieldPath 不作为反射路径或表达式执行。

示例“解析数据关联”使用宿主输出字典，将提交申请的金额和邮箱分别提供给审批金额、通知收件人。再次解析会读取当前数据源值。

## 模板

`WorkflowCanvas` 与 `WorkflowMiniMap` 是 WorkflowEditor 的绘制／交互模板辅助控件。普通业务接入使用 WorkflowEditor 及下列模板扩展点，不把这两个辅助控件当作独立编辑器替换目标。完整替换模板时保留 `PART_Canvas`，缩略图的 `Editor` 绑定到当前 WorkflowEditor；具体结构以同版本 [WorkflowEditor.axaml](../../../TheOne.Avalonia/Themes/Controls/WorkflowEditor.axaml) 为准。

- 控件内置完整外框、标题栏、节点库、画布、属性面板、工具栏与缩略图，无需消费端提供模板即可使用。
- HeaderTemplate 接收 WorkflowEditor，默认展示文档标题、校验和保存按钮；可设置为 null 隐藏标题栏，也可用 IDataTemplate 替换。null 时 Presenter 使用显式空模板，避免将编辑器插入自己的视觉树；窗口显示后也支持在 null 与自定义模板间切换。
- NodeTemplate 接收 WorkflowNode，默认展示标题、说明与状态。端口和选择边框保留在模板外。
- NodeDefinitionTemplate 接收 WorkflowNodeDefinition，默认在节点库展示标题与说明。
- ContentTemplate 接收节点 Content，可使用任意具体业务类型和编译绑定。Content 不序列化，加载后由宿主按 TypeId 重建。
- PropertyTemplate 接收选中节点。默认使用内置 TabStrip 展示基础、数据和端口页，支持编辑名称、说明、固定值与上游引用，修改参与文档撤销历史，并遵循 IsReadOnly。SelectedNodeEditor 暴露 WorkflowNodeEditor / WorkflowPropertyEditor，方便自定义模板复用编辑逻辑。业务专用表单可替换此模板，也可隐藏面板后在外部绑定 SelectedNode。模板自行提供滚动容器，默认删除按钮固定在滚动区域外。
- SelectedConnection 提供选中边，宿主可在外部编辑 Label 或业务条件；默认面板显示节点属性。

节点库和属性栏与画布之间各有原生 `GridSplitter`，可用鼠标拖动，分隔条获取焦点后也可用左右方向键调整。左右初始宽度分别为 195、296 DIP，最大宽度为 420、560 DIP；正常宽度下最小值为 140、280 DIP，空间允许时画布保留至少 160 DIP。窄窗口下边栏按比例收缩，窗口恢复后还原用户偏好宽度。折叠时同时释放边栏与分隔条空间，重新展开保留本控件实例的调整值；宽度不写入工作流 JSON。只读模式允许调整布局，不修改文档修订号、选择或属性编辑器。节点库按钮采用更紧凑的留白，演示图标为 18 DIP、图标底板为 24 DIP。完整替换控件模板时，若要保留此行为，应保留 `PART_PanelGrid` 的“左栏、分隔条、画布、分隔条、右栏”五列结构及两个原生分隔条；自定义节点和属性内容模板不需要更改。

节点与连线选择互斥，外部设置 `SelectedNode` / `SelectedConnection` 时同样生效。选中连线会清空所有选中节点及节点属性面板；选中节点会清除连线选择。不属于当前文档的模型统一归为空，即使其 ID 与文档中的模型相同也不会串用选择状态。

示例强类型配置行使用 ComboBox 切换固定值/上游引用，通过 Document.SetProperty 和 Edit 参与图编辑的同一撤销历史。加载和复制后，宿主按 TypeId 重建 Content，避免把展示对象写入文件或与原节点共享可变业务对象。

## 编辑与校验

所有模型和控件更新在 UI 线程进行。集合/属性通知刷新视图；替换 Document 解除旧订阅并清空选择。直接宿主数据更新会清除旧历史，需要撤销时使用 Edit(title, redo, undo)，回调应确定且不抛异常。展示 Content 和运行 State 的更新不改变保存修订号或清除历史。

支持节点拖动、网格吸附、框选、Shift 多选、方向键微移（Shift=10）、Delete/Backspace 删除、Ctrl/Cmd+Z / Shift+Z 撤销重做、Ctrl/Cmd+C/V/D 编辑器内复制/粘贴/副本。复制生成新节点与边 ID，重映射内部数据引用，保留外部引用并在失效时报告。剪贴板仅在当前编辑器内使用。

从输出拖到输入连接，或 Tab/Enter 依次激活输出/输入按钮；Escape 取消。空格+拖动或中键平移；滚轮按指针缩放，范围 25%–200%。原生文本输入保留自己的快捷键。

选择/平移工具通过 IsPanMode 切换，对齐命令将所选节点顶部对齐。缩略图支持点击与拖动导航。FitPadding 控制适应画布的边距，示例为运行预览标记保留额外顶部空间。校验按钮打开可点击定位的问题列表。IsAnnotation 节点不计入状态栏执行节点数量。

IsReadOnly 禁止编辑器写入，允许选择和视口操作；自定义配置面板须由宿主禁用写操作，示例已演示。宿主仍可主动更新模型。ShowNodeLibrary / ShowProperties 由宿主控制折叠，适配窄视口。节点库内的折叠按钮隐藏面板后，画布工具栏仍保留“节点库”切换按钮，支持鼠标和键盘重新打开；按钮通过双向绑定更新 ShowNodeLibrary，宿主绑定不会被替换。

文档内部按节点 ID 查找端点，并按连线建立邻接、端口计数与环路索引；仅集合结构变化使对应索引失效，修改标题、坐标、运行状态和连线标签不重建结构索引。节点与端点 ID 是 init-only，变更 ID 时替换模型对象。Reset、Replace、Move 和替换 Document 都重新对齐当前数据；无效的重复 ID 仍按集合中的第一项解析并报告校验错误。节点删除检测和选中状态匹配使用集合查找，画布与缩略图复用相同文档索引。

## 画布缓存与离屏节点

画布缓存全图节点尺寸、端口端点与连线路径，平移和缩放复用图坐标结果；节点尺寸、端口、坐标及连线变化更新对应结果。离屏连线按实际路径范围裁剪，两个端点都离屏但穿过视口的连线仍绘制并可选择。FitToView、缩略图和数据校验始终基于完整文档，不以当前挂载视图代替模型。

首次布局仍真实测量全部节点，取得换行高度和端口位置；主要优化后续平移、重绘和默认节点的稳态占用，不承诺降低首次测量成本。默认离屏视图最多保留 64 项冷缓存，超过后释放；返回时仅按同一模型复用，不把旧控件重新绑定给另一个节点。自定义模板和编辑内容以状态完整为优先，保留数量不受该上限约束。模型、度量和连线缓存仍随完整文档规模增长，这不是业务数据虚拟化。纯平移复用连线几何与标签；缩放重建相应几何，保持圆角和文字的屏幕尺寸，字号、字体及密度变化会重新测量节点。

默认无状态节点离开视口及预留区域后可卸载，返回时按当前模型复用或重建视图；选中状态属于模型，离屏不会清空选择。自定义 NodeTemplate / ContentTemplate 可能保存本地草稿、光标位置或其他控件状态，画布保留其视图身份；正在编辑、拥有焦点、参与指针捕获或打开上下文菜单的节点也不会在交互中被卸载。节点被删除、Replace 为新对象或切换 Document 时释放旧视图，避免同 ID 新对象串用草稿或选择。显式更换模板会重建模板内容，本地草稿不跨模板替换保留。

这些行为不增加公共开关或更改绑定 API。自定义模板的状态保留优先于卸载收益，大量有状态节点仍需要宿主管理业务数据和编辑范围。

连接默认拒绝缺失端点、方向/类型不匹配、重复、自连、数量超限及环路。AllowCycles 可显式放开环路，ConnectionValidator 增加宿主连接限制。Validate(allowCycles) 检查结构、字段及引用；具体业务与权限另由宿主校验。没有强制的业务“开始/结束”类型。

## 保存加载

`SaveCommand` 是可绑定的宿主 `ICommand`，默认保存按钮传入当前 `Document`，并遵循 `CanExecute` / `CanExecuteChanged`；未提供命令时按钮禁用。控件本身不弹出文件选择器或写文件。PreviewHost 的 ViewModel 通过异步命令和注入的 `IWorkflowEditorFileService` 完成保存；导出、打开也使用命令。取消或失败不标记已保存，异步保存旧快照期间的新修改仍保留未保存状态。

```csharp
string json = WorkflowSerializer.Save(document);
WorkflowDocument loaded = WorkflowSerializer.Load(json);
```

当前保存 schema version=2（`WorkflowSerializer.CurrentVersion`），使用源生成 System.Text.Json 元数据。读取同时兼容 version=1，旧连线得到空的 Properties；version=2 必须有连线 Properties 数组。旧版本读取器不能读取新格式，宿主应协调升级，不能通过改写 version 值降级。保存节点、端口、字段、引用、坐标、连线及其业务字段；排除 Content、模板、运行状态、服务、选择、视口和历史。额外的文档元数据可用宿主外层 DTO 保存。

`WorkflowConnection.Properties` 复用 `WorkflowProperty`，支持固定值、多选和上游字段引用。分支类型、条件、顺序等由宿主定义稳定 Key，Label 只用于显示。连线引用以目标节点的上游范围校验；业务执行仍由宿主负责。使用 `document.SetConnectionProperty(connection, property, value, reference)` 修改既有字段，可保持撤销、重做、Changed 和 Revision。多选使用 `SetSelectedKeys`；字段增删通过 `Edit` 组合为一个事务。直接修改模型仍按既有规则重置历史。

```csharp
var branch = new WorkflowProperty { Key = "routeKind", Title = "分支类型", Value = "approved" };
connection.Properties.Add(branch);
document.SetConnectionProperty(connection, branch, "rejected");
```

连线字段在复制粘贴时深复制，内部引用随新节点 ID 重映射；外部引用保留原身份。Validate 检查 Key、字段类型、必填、多选、数值及引用，错误携带 ConnectionId；不替宿主判断审批表达式。加载拒绝重复字段 Key、未知 Kind 和非法引用身份，不丢弃字段来修复输入。

不支持的版本、无效 ID/坐标被拒绝；草稿失效引用/端点保留并由 Validate 报告。宿主先解析新文档再替换绑定，示例解析失败保留原流程。

## 示例与验证

PreviewHost：**扩展控件 → WorkflowEditor**。主界面还原设计图的七节点审批流程、备注、图标、端口及状态；属性页签使用内置 `TabStrip`。节点库、工具栏、属性面板及示例操作区默认使用 Small 尺寸（正文与输入文字 12），画布保留宿主尺寸和节点比例。下方折叠区演示默认与自定义模板、强类型 Content、配置面板、引用选择/解析、只读、可编辑 JSON 及原生文件保存/打开。独立数据关联示例展示提交申请 → 审批 → 通知的收件人引用。

连线采用直接绘制圆角正交线与拓扑分层排列，支持离屏裁剪和安全节点视图回收，不提供完整障碍避让或折点编辑。业务规则执行与输出求值由宿主负责。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### WorkflowCanvas

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### WorkflowEditor

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AddNodeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L356) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`AlignCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L365) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`AllowCycles`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L324) | `bool` | 可写; OneWay | `false` | 允许连接形成环；关闭时由编辑器连接校验阻止环路，运行器仍需处理业务循环语义。 |
| [`ArrangeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L364) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ChoiceItemAdapter`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L41) | `TheOne.Avalonia.Workflow.WorkflowChoiceAdapter?` | 可写; OneWay | `null` | 使用 WorkflowChoiceAdapter.Create<T> 指定候选项 Key 和显示文字的编译委托；不使用反射属性路径。未设置时数据项必须为 WorkflowChoice。 |
| [`ChoiceItemsSource`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L35) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | 多选字段的默认候选数据源，支持双向绑定和集合通知；使用 ChoiceItemAdapter 提供稳定 Key 与显示文字，审批人或角色数据由宿主加载。 |
| [`ConnectionFactory`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L374) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowConnection?>?` | 可写; CLR / n/a | `null` | Creates connection metadata before its single history edit. Endpoints must match the requested ports; null rejects creation. |
| [`ConnectionPropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L249) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Optional connection inspector; its data context is the original SelectedConnection. |
| [`ConnectionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L371) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, string?>?` | 可写; CLR / n/a | `null` | Host can reject business-specific connections in addition to structural checks. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L236) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DeleteCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L359) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`Document`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L206) | `TheOne.Avalonia.Workflow.WorkflowDocument?` | 可写; TwoWay | `null` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`FitCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L360) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`FitPadding`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L348) | `Avalonia.Thickness` | 可写; OneWay | `new Thickness(32)` | 缩放适配内容时保留的 DIP 留白。 校验：`p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0)` |
| [`GraphSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L303) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L262) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Header content receives this editor. The default exposes document title, validation and SaveCommand. |
| [`IsPanMode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L283) | `bool` | 可写; OneWay | `false` | 将指针拖动用于平移画布；选择和编辑手势按流程编辑器规则处理。 |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L306) | `bool` | 可写; OneWay | `false` | 是否阻止编辑；展示、选择与导航是否仍可用由各控件的只读契约决定。 |
| [`LibraryGroups`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L301) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowNodeGroup>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`LocateIssueCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L368) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`NodeCreationValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L377) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, string?>?` | 可写; CLR / n/a | `null` | Optional host policy for adding and pasting nodes. A non-null message rejects that candidate. |
| [`NodeDefinitionTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L255) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`NodeDefinitions`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L212) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Workflow.WorkflowNodeDefinition>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`NodeDeletionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L380) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, string?>?` | 可写; CLR / n/a | `null` | Optional host policy for deletion. Rejected nodes are retained while other selected nodes may be removed. |
| [`NodeTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L230) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Pan`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L342) | `Avalonia.Vector` | 可写; TwoWay | `new Vector(24, 24)` | 画布平移向量；只改变视口，不改写节点模型坐标。 校验：`x => double.IsFinite(x.X) && double.IsFinite(x.Y)` |
| [`PropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L242) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RedoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L358) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L269) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Host-owned persistence command. The default save button passes Document as its parameter. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L277) | `string` | 可写; OneWay | `""` | 当前搜索文本；搜索与选中状态独立，不用查询字符串充当模型主键。 |
| [`SelectedConnection`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L224) | `TheOne.Avalonia.Workflow.WorkflowConnection?` | 可写; TwoWay | `null` | 当前选中的连线模型；与节点选择的互斥规则见编辑器文档。 |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L218) | `TheOne.Avalonia.Workflow.WorkflowNode?` | 可写; TwoWay | `null` | 当前选中的节点模型；空值清除节点选择。 |
| [`SelectedNodeEditor`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L275) | `TheOne.Avalonia.Workflow.WorkflowNodeEditor?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedNodes`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L204) | `System.Collections.ObjectModel.ReadOnlyObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowNodeLibrary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L312) | `bool` | 可写; OneWay | `true` | 显示可添加节点的定义库面板。 |
| [`ShowProperties`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L318) | `bool` | 可写; OneWay | `true` | 显示所选对象的属性编辑面板。 |
| [`ShowRuntimePreview`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L289) | `bool` | 可写; OneWay | `false` | 显示运行时预览面板；实际业务执行由宿主运行器负责。 |
| [`ShowValidation`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L295) | `bool` | 可写; OneWay | `false` | 显示文档校验结果面板；关闭面板不等于绕过校验。 |
| [`SnapToGrid`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L330) | `bool` | 可写; OneWay | `true` | 节点移动时吸附到网格；不修改已有节点，直到进行相应编辑。 |
| [`Status`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L354) | `string` | 只读; OneWay | `computed / state` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 |
| [`ToggleLibraryCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L366) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`TogglePropertiesCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L367) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`UndoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L357) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ValidateCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L363) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ValidationIssues`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L302) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowValidationIssue>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ValidationSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L304) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Zoom`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L336) | `double` | 可写; TwoWay | `1` | 视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。 校验：`x => double.IsFinite(x) && x >= .25 && x <= 2` |
| [`ZoomInCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L361) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ZoomOutCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L362) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L355) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### WorkflowMiniMap

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowMiniMap.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Editor`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L24) | `TheOne.Avalonia.Controls.WorkflowEditor?` | 可写; OneWay | `null` | 缩略图关联的 WorkflowEditor；用于共享文档和视口，不创建独立工作流状态。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`WorkflowCanvas`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md) | `Point WorkflowCanvas.ToWorld(Point point)` | 将画布视口坐标换算为文档世界坐标，计入缩放和平移。 |
| [`WorkflowCanvas`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md) | `void WorkflowCanvas.CancelGesture()` | 取消当前画布手势并清理拖动／连线预览。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `WorkflowNode? WorkflowEditor.AddNode(WorkflowNodeDefinition definition, Point position)` | 向当前工作流文档加入节点；通过文档历史入口保存可撤销操作。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.SelectNode(WorkflowNode? node, bool additive = false)` | 选择指定的原始流程节点；与连线选择按编辑器规则协调。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `bool WorkflowEditor.Connect(string sourceNode, string sourcePort, string targetNode, string targetPort)` | 尝试创建节点端口连接，复用文档连接校验和历史。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.DeleteSelection()` | 删除当前选中的节点／连线，并参与撤销历史。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.FitToView()` | 调整缩放和平移以容纳文档节点，保留模型坐标。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.Locate(string? nodeId)` | 定位目标节点／校验问题，使其进入当前视口。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.ArrangeNodes()` | 自动整理节点布局，通过文档编辑历史记录坐标变更。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.CopySelection()` | 复制当前选择为编辑器支持的片段，保持序列化边界。 |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `void WorkflowEditor.PasteSelection()` | 粘贴已复制片段，创建独立标识并记录可撤销编辑。 |
| [`WorkflowChoiceAdapter`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) | `abstract string WorkflowChoiceAdapter.GetKey(object item)` |  |
| [`WorkflowChoiceAdapter`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) | `abstract string WorkflowChoiceAdapter.GetText(object item)` | 使用已配置的类型化适配器读取原始模型显示／搜索文本。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.Edit(string title, Action redo, Action undo)` | Apply a host edit to the same history as graph edits. Actions must be deterministic and not throw. |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.Undo()` | 撤销最近一次可撤销的文档编辑。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.Redo()` | 重做最近撤销的文档编辑。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.ClearHistory()` | 清空撤销与重做记录，保留当前文档数据。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.AddNode(WorkflowNode node)` | 向当前工作流文档加入节点；通过文档历史入口保存可撤销操作。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.Remove(IEnumerable<WorkflowNode> nodes, WorkflowConnection? connection = null)` | 删除指定工作流对象及关联关系，通过文档编辑历史执行。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.MoveNodes(IEnumerable<(WorkflowNode Node, double X, double Y)> positions)` | 批量更新节点坐标并记录一次可撤销移动。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.SetProperty(WorkflowProperty property, string value, WorkflowDataReference? reference)` | 通过文档修改流程属性的固定值／引用，以保持撤销与通知。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.SetConnectionProperty(WorkflowConnection connection, WorkflowProperty property, string value, WorkflowDataReference? reference = null)` | 修改本图中指定连线的固定值或引用，保留原字段的撤销记录；不允许编辑其他文档的字段。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `void WorkflowDocument.SetSelectedKeys(WorkflowProperty property, IEnumerable<string> keys)` | One undoable multi-selection edit, preserving stable keys and their order. |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `string? WorkflowDocument.CheckConnection(string sourceNode, string sourcePort, string targetNode, string targetPort, bool allowCycles = false)` | 校验候选端口连接，返回失败原因而不创建连线。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `bool WorkflowDocument.TryConnect(string sourceNode, string sourcePort, string targetNode, string targetPort, out string? error, bool allowCycles = false)` | 校验并尝试创建连接；返回是否成功及相关结果。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `bool WorkflowDocument.TryConnect(WorkflowConnection connection, out string? error, bool allowCycles = false)` | Adds a host-created connection in one undoable edit after validating identity and topology. |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `bool WorkflowDocument.IsReachable(string from, string to)` | 判断流程图中目标节点是否可从起点到达，用于连线与循环判断。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `IReadOnlyList<WorkflowReferenceOption> WorkflowDocument.GetReferenceOptions(WorkflowNode target, WorkflowProperty property)` | 为当前节点属性列出兼容的上游数据引用选项。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `IReadOnlyList<WorkflowValidationIssue> WorkflowDocument.Validate(bool allowCycles = false)` | 校验工作流结构、端口与属性引用，返回可定位的问题集合。 |
| [`WorkflowNodeEditor`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) | `void WorkflowNodeEditor.Refresh()` | Refresh after document edits, including undo and upstream metadata changes. |
| [`WorkflowPropertyEditor`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) | `void WorkflowPropertyEditor.Refresh()` | 从底层模型与文档重新同步属性检查器，包含撤销后及上游变化。 |
| [`WorkflowPropertyEditor`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) | `void WorkflowPropertyEditor.ConfigureChoices(IEnumerable? source, WorkflowChoiceAdapter? adapter)` | Configure host records with typed key/text projections. Collection and label notifications are observed. |
| [`WorkflowSerializer`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowSerializer.md) | `static string WorkflowSerializer.Save(WorkflowDocument document)` | 以当前格式保存完整设计，不保存视觉对象、运行状态或撤销历史。 |
| [`WorkflowSerializer`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowSerializer.md) | `static WorkflowDocument WorkflowSerializer.Load(string json)` | 读取版本 1 或 2；非法身份和字段抛出 JsonException，失效图引用保留给 Validate 报告。 |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`WorkflowEditor`](../../api/zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) | `DocumentEdited` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | `Changed` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |
| [`WorkflowObservable`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowObservable.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Workflow.WorkflowChoice`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowChoice.md) | 3 |
| [`TheOne.Avalonia.Workflow.WorkflowConnection`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowConnection.md) | 7 |
| [`TheOne.Avalonia.Workflow.WorkflowDataReference`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDataReference.md) | 3 |
| [`TheOne.Avalonia.Workflow.WorkflowDocument`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowDocument.md) | 6 |
| [`TheOne.Avalonia.Workflow.WorkflowNode`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNode.md) | 14 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md) | 6 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeEditor`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeEditor.md) | 5 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeGroup`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeGroup.md) | 2 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeShape`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeShape.md) | 3 |
| [`TheOne.Avalonia.Workflow.WorkflowNodeState`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowNodeState.md) | 5 |
| [`TheOne.Avalonia.Workflow.WorkflowOutputField`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowOutputField.md) | 3 |
| [`TheOne.Avalonia.Workflow.WorkflowPort`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowPort.md) | 6 |
| [`TheOne.Avalonia.Workflow.WorkflowPortDirection`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowPortDirection.md) | 2 |
| [`TheOne.Avalonia.Workflow.WorkflowProperty`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowProperty.md) | 9 |
| [`TheOne.Avalonia.Workflow.WorkflowPropertyEditor`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowPropertyEditor.md) | 13 |
| [`TheOne.Avalonia.Workflow.WorkflowReferenceOption`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowReferenceOption.md) | 2 |
| [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowValidationIssue.md) | 3 |
| [`TheOne.Avalonia.Workflow.WorkflowValueKind`](../../api/zh-cn/TheOne.Avalonia.Workflow.WorkflowValueKind.md) | 4 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
