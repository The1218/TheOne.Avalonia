# WorkflowEditor

[English](../en/TheOne.Avalonia.Controls.WorkflowEditor.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowEditor.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowEditor` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Template-based graph editing. Persistence, execution and business validation belong to the host.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L18)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`AddNodeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L95) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`AlignCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L104) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`AllowCycles`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L88) | `bool` | 可写; OneWay | `false` | 允许连接形成环；关闭时由编辑器连接校验阻止环路，运行器仍需处理业务循环语义。 |
| [`ArrangeCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L103) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ConnectionValidator`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L109) | `System.Func<TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, TheOne.Avalonia.Workflow.WorkflowNode, TheOne.Avalonia.Workflow.WorkflowPort, string?>?` | 可写; CLR / n/a | `null` | Host can reject business-specific connections in addition to structural checks. |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L69) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DeleteCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L98) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`Document`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L64) | `TheOne.Avalonia.Workflow.WorkflowDocument?` | 可写; TwoWay | `null` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`FitCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L99) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`FitPadding`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L92) | `Avalonia.Thickness` | 可写; OneWay | `new Thickness(32)` | 缩放适配内容时保留的 DIP 留白。 校验：`p => new[] { p.Left, p.Top, p.Right, p.Bottom }.All(x => double.IsFinite(x) && x >= 0)` |
| [`GraphSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L83) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L73) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | Header content receives this editor. The default exposes document title, validation and SaveCommand. |
| [`IsPanMode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L78) | `bool` | 可写; OneWay | `false` | 将指针拖动用于平移画布；选择和编辑手势按流程编辑器规则处理。 |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L85) | `bool` | 可写; OneWay | `false` | 是否阻止编辑；展示、选择与导航是否仍可用由各控件的只读契约决定。 |
| [`LibraryGroups`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L81) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowNodeGroup>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`LocateIssueCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L107) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`NodeDefinitionTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L71) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`NodeDefinitions`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L65) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Workflow.WorkflowNodeDefinition>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`NodeTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L68) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Pan`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L91) | `Avalonia.Vector` | 可写; TwoWay | `new Vector(24, 24)` | 画布平移向量；只改变视口，不改写节点模型坐标。 校验：`x => double.IsFinite(x.X) && double.IsFinite(x.Y)` |
| [`PropertyTemplate`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L70) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RedoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L97) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L75) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Host-owned persistence command. The default save button passes Document as its parameter. |
| [`SearchText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L77) | `string` | 可写; OneWay | `""` | 当前搜索文本；搜索与选中状态独立，不用查询字符串充当模型主键。 |
| [`SelectedConnection`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L67) | `TheOne.Avalonia.Workflow.WorkflowConnection?` | 可写; TwoWay | `null` | 当前选中的连线模型；与节点选择的互斥规则见编辑器文档。 |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L66) | `TheOne.Avalonia.Workflow.WorkflowNode?` | 可写; TwoWay | `null` | 当前选中的节点模型；空值清除节点选择。 |
| [`SelectedNodeEditor`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L76) | `TheOne.Avalonia.Workflow.WorkflowNodeEditor?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedNodes`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L63) | `System.Collections.ObjectModel.ReadOnlyObservableCollection<TheOne.Avalonia.Workflow.WorkflowNode>` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`ShowNodeLibrary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L86) | `bool` | 可写; OneWay | `true` | 显示可添加节点的定义库面板。 |
| [`ShowProperties`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L87) | `bool` | 可写; OneWay | `true` | 显示所选对象的属性编辑面板。 |
| [`ShowRuntimePreview`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L79) | `bool` | 可写; OneWay | `false` | 显示运行时预览面板；实际业务执行由宿主运行器负责。 |
| [`ShowValidation`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L80) | `bool` | 可写; OneWay | `false` | 显示文档校验结果面板；关闭面板不等于绕过校验。 |
| [`SnapToGrid`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L89) | `bool` | 可写; OneWay | `true` | 节点移动时吸附到网格；不修改已有节点，直到进行相应编辑。 |
| [`Status`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L93) | `string` | 只读; OneWay | `computed / state` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 |
| [`ToggleLibraryCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L105) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`TogglePropertiesCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L106) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`UndoCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L96) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ValidateCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L102) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ValidationIssues`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L82) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Workflow.WorkflowValidationIssue>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ValidationSummary`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L84) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Zoom`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L90) | `double` | 可写; TwoWay | `1` | 视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。 校验：`x => double.IsFinite(x) && x >= .25 && x <= 2` |
| [`ZoomInCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L100) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ZoomOutCommand`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L101) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L94) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowNode? WorkflowEditor.AddNode(WorkflowNodeDefinition definition, Point position)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L219) | 向当前工作流文档加入节点；通过文档历史入口保存可撤销操作。 |
| [`void WorkflowEditor.SelectNode(WorkflowNode? node, bool additive = false)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L227) | 选择指定的原始流程节点；与连线选择按编辑器规则协调。 |
| [`bool WorkflowEditor.Connect(string sourceNode, string sourcePort, string targetNode, string targetPort)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L240) | 尝试创建节点端口连接，复用文档连接校验和历史。 |
| [`void WorkflowEditor.DeleteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L252) | 删除当前选中的节点／连线，并参与撤销历史。 |
| [`void WorkflowEditor.FitToView()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L253) | 调整缩放和平移以容纳文档节点，保留模型坐标。 |
| [`void WorkflowEditor.Locate(string? nodeId)`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L261) | 定位目标节点／校验问题，使其进入当前视口。 |
| [`void WorkflowEditor.ArrangeNodes()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L266) | 自动整理节点布局，通过文档编辑历史记录坐标变更。 |
| [`void WorkflowEditor.CopySelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L276) | 复制当前选择为编辑器支持的片段，保持序列化边界。 |
| [`void WorkflowEditor.PasteSelection()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L285) | 粘贴已复制片段，创建独立标识并记录可撤销编辑。 |
| [`WorkflowEditor.WorkflowEditor()`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L111) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler? WorkflowEditor.DocumentEdited`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L110) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<WorkflowDocument?> WorkflowEditor.DocumentProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IEnumerable<WorkflowNodeDefinition>?> WorkflowEditor.NodeDefinitionsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<WorkflowNode?> WorkflowEditor.SelectedNodeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<WorkflowConnection?> WorkflowEditor.SelectedConnectionProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.ContentTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.PropertyTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.NodeDefinitionTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> WorkflowEditor.HeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> WorkflowEditor.SaveCommandProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, WorkflowNodeEditor?> WorkflowEditor.SelectedNodeEditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> WorkflowEditor.SearchTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsPanModeProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowRuntimePreviewProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowValidationProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowNodeGroup>> WorkflowEditor.LibraryGroupsProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.GraphSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ValidationSummaryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, IReadOnlyList<WorkflowValidationIssue>> WorkflowEditor.ValidationIssuesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowNodeLibraryProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.ShowPropertiesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L46) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.AllowCyclesProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> WorkflowEditor.SnapToGridProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L48) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> WorkflowEditor.ZoomProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L49) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Vector> WorkflowEditor.PanProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<Thickness> WorkflowEditor.FitPaddingProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L51) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.StatusProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L52) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<WorkflowEditor, string> WorkflowEditor.ZoomTextProperty`](../../../TheOne.Avalonia/Controls/WorkflowEditor.cs#L53) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowConnection`](TheOne.Avalonia.Workflow.WorkflowConnection.md)
- [`TheOne.Avalonia.Workflow.WorkflowDocument`](TheOne.Avalonia.Workflow.WorkflowDocument.md)
- [`TheOne.Avalonia.Workflow.WorkflowNode`](TheOne.Avalonia.Workflow.WorkflowNode.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeDefinition`](TheOne.Avalonia.Workflow.WorkflowNodeDefinition.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeEditor`](TheOne.Avalonia.Workflow.WorkflowNodeEditor.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeGroup`](TheOne.Avalonia.Workflow.WorkflowNodeGroup.md)
- [`TheOne.Avalonia.Workflow.WorkflowPort`](TheOne.Avalonia.Workflow.WorkflowPort.md)
- [`TheOne.Avalonia.Workflow.WorkflowValidationIssue`](TheOne.Avalonia.Workflow.WorkflowValidationIssue.md)
