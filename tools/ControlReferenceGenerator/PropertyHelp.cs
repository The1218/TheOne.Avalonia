namespace TheOne.Documentation;

// Shared, authored semantics for members whose declarations have no XML summary.
internal static class PropertyHelp
{
    private static readonly Dictionary<string, (string Zh, string En)> Entries = Parse("""
    ReportDocument.DataSourceName|设计器左侧数据目录的来源名称，默认“接口数据”；随报表模板保存，不改变接口地址或字段键。|Source label in the designer's data catalog, default “接口数据”; saved with the template without changing the API URL or field keys.
    ReportApiRequest.SourceName|请求成功后用于数据目录分组的来源名称；默认“接口数据”，不作为身份标识或请求 URL。|Catalog group label after a successful request, default “接口数据”; not an identity key or request URL.
    ReportDataNode.IconKind|只读图标名称：集合为 Table、对象为 Folder、单值为 Text；由节点 Kind 计算，不参与字段绑定。|Read-only icon name derived from Kind: Table for collections, Folder for objects and Text for scalar values; independent of field binding.
    WorkflowEditor.ChoiceItemsSource|多选字段的默认候选数据源，支持双向绑定和集合通知；使用 ChoiceItemAdapter 提供稳定 Key 与显示文字，审批人或角色数据由宿主加载。|Default choice catalog for multi-select fields; supports two-way binding and collection notifications. ChoiceItemAdapter supplies stable keys and labels; the host loads people or roles.
    WorkflowEditor.ChoiceItemAdapter|使用 WorkflowChoiceAdapter.Create<T> 指定候选项 Key 和显示文字的编译委托；不使用反射属性路径。未设置时数据项必须为 WorkflowChoice。|Typed key and label projections from WorkflowChoiceAdapter.Create<T>, without reflection member paths. When omitted, records must be WorkflowChoice instances.
    WorkflowProperty.SelectedKeys|多选字段保存的稳定 Key 集合，序列化为 JSON 数组；选项对象与显示文字不保存。直接修改用于模型绑定；用户编辑通过 WorkflowDocument.SetSelectedKeys 记录撤销。|Stable selected keys serialized as a JSON array; records and labels are not persisted. Direct changes support model binding; use WorkflowDocument.SetSelectedKeys for undoable user edits.
    WorkflowPropertyEditor.Choices|当前字段的候选投影，按 Key 保持标识；仅显示用，持久状态为 Property.SelectedKeys。|Current choice projections identified by key, for display only; Property.SelectedKeys owns persisted selection.
    WorkflowPropertyEditor.SelectedChoices|绑定 MultiSelectBox.SelectedItems 的双向选择集合；添加和移除会更新 SelectedKeys，并通过文档记录撤销。|Two-way collection for MultiSelectBox.SelectedItems; additions and removals update SelectedKeys through the document undo history.
    WorkflowPropertyEditor.ChoiceError|候选数据 Key 为空、重复或类型不兼容时的错误文字；不会清除已保存的选择。|Error text for empty or duplicate catalog keys or incompatible records; persisted selections are retained.
    WorkflowPropertyEditor.HasMissingChoices|已保存的 Key 暂时不在候选数据源中时为 true；保留可移除标签，数据恢复后重新关联。|True when persisted keys are temporarily absent from the catalog; removable tags remain and reconnect when records return.
    WorkflowChoice.IsAvailable|该 Key 当前是否存在于宿主候选数据源；不代表业务审批权限。|Whether this key currently exists in the host catalog; it does not indicate approval authorization.
    QRCode.Text|二维码内容，原样保留，使用带 ECI 标记的 UTF-8；空值、无效 Unicode 或超出当前等级容量时清除旧图形并设置 ValidationError。|Payload preserved exactly and encoded as UTF-8 with ECI; empty, invalid Unicode or over-capacity input clears the old symbol and sets ValidationError.
    QRCode.ErrorCorrection|纠错等级 Low、Medium、Quartile、High，默认 Medium；使用指定等级，不自动提升。|Low, Medium, Quartile or High error correction, default Medium; the exact requested level is used without automatic boosting.
    QRCode.QuietZone|四周静区宽度，单位为模块，范围 4–32，默认 4；非正方形空间中居中绘制正方形二维码。|Quiet border on every side in modules, range 4–32, default 4; non-square bounds center a square symbol.
    QRCode.ValidationError|只读编码或尺寸错误；有效时为 null。含静区的每个模块至少 1 DIP，非法状态不显示旧二维码。|Read-only encoding or size error, null when valid. Each module including the quiet zone needs at least 1 DIP; invalid states never display a stale symbol.
    QRCode.Foreground|二维码深色模块画刷，默认固定黑色；自定义时应保持与背景的扫描对比度。|Dark-module brush, fixed black by default; preserve scanning contrast when customizing.
    QRCode.Background|浅色模块及静区画刷，默认固定白色，不随明暗主题反色。|Light-module and quiet-zone brush, fixed white by default and never inverted by brightness themes.
    ReportElement.QRCodeErrorCorrection|二维码元素的纠错等级，默认 Medium；随模板、撤销和重做保存。|Error correction for a QR report element, default Medium; retained by templates and undo/redo.
    ReportElement.QRCodeQuietZone|二维码元素四周的静区模块数，范围 4–32，默认 4；设计、预览和 PDF 共用。|Quiet-zone modules around a QR report element, range 4–32, default 4; shared by design, preview and PDF.
    Description.LabelWidth|标签列的 DIP 宽度；向后代继承，可由 DescriptionItem 单项覆盖，窄布局切换为上下排列。|Label-column width in DIPs, inherited by descendants and overridable per DescriptionItem; narrow layouts place the label above the content.
    Description.StackWidth|切换上下排列的可用宽度阈值，单位 DIP；宽度小于该值时标签在上、内容在下。向后代继承，可由单项覆盖。|Available-width threshold in DIPs for stacked labels and content; below this width the label appears above its content. Inherited by descendants and overridable per item.
    DescriptionItem.LabelWidth|此说明项的标签列 DIP 宽度；默认继承 Description.LabelWidth，局部值仅覆盖本项。|Label-column width in DIPs for this row; inherits Description.LabelWidth by default, while a local value overrides this item only.
    DescriptionItem.StackWidth|此说明项切换上下排列的可用宽度阈值，单位 DIP；默认继承 Description.StackWidth，局部值仅覆盖本项。|Available-width threshold in DIPs for stacking this row; inherits Description.StackWidth by default, while a local value overrides this item only.
    IsLoading|显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。|Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes.
    Orientation|横向或纵向排列；切换后同时更新节点、连线及内容布局，不改变业务顺序。|Horizontal/vertical arrangement; updates node, connector and content layout without changing business order.
    Placement|装饰或内容的位置枚举；按枚举值选择边缘，不使用字符串坐标。|Placement enum selecting a decoration/content edge rather than string coordinates.
    BorderBrush|边缘描边画刷；使用动态主题资源以保持明暗切换。|Border stroke brush; use dynamic theme resources to follow brightness changes.
    CornerRadius|外观圆角，单位为 Avalonia DIP；尺寸和背景裁剪仍由所属模板决定。|Corner radius in Avalonia DIPs; sizing and background clipping remain template responsibilities.
    FontFamily|文本绘制使用的字体族；字体回退和实际度量由 Avalonia 文本系统处理。|Font family for text rendering; Avalonia handles fallback and actual text metrics.
    AnimationDuration|数据过渡的持续时间；是否启用动画还受控件开关和 Motion.Enabled 约束。|Duration of data transitions, also gated by the control animation switch and Motion.Enabled.
    IsAnimationEnabled|启用该图表的数据过渡；全局 Motion.Enabled=False 时仍关闭动画。|Enables chart data transitions; global Motion.Enabled=False still disables them.
    ChartBase.Minimum|主数值轴下界；空值由数据自动推导，显式范围必须与数据及 Maximum 协调。|Primary value-axis minimum; null derives it from data. Explicit ranges must be consistent with the data and Maximum.
    ChartBase.Maximum|主数值轴上界；空值使用自动范围，不通过裁剪更改数据本身。|Primary value-axis maximum; null uses automatic bounds without changing the underlying data.
    InnerRadius|径向图内半径比例；控制中空区域，保留扇区数值和比例含义。|Inner-radius ratio for radial charts, controlling the hole without changing sector values/proportions.
    SamplingMode|密集序列的采样算法；可选策略见枚举，采样影响绘制点数，不替换原始数据。|Sampling strategy for dense series; see enum choices. It reduces rendered points without replacing source data.
    SamplingThreshold|启用采样的点数阈值；结合视口与 SamplingMode 调整大数据绘制成本。|Point threshold for sampling, combined with the viewport and SamplingMode to bound rendering work.
    SecondaryUnit|第二数值轴的单位后缀，与主轴单位独立。|Unit suffix for the secondary value axis, independent of the primary unit.
    SecondaryValueFormat|第二数值轴的数值格式字符串；使用符合 .NET 数值格式的值。|Numeric format string for the secondary value axis, using .NET numeric formatting.
    SecondaryYAxisTitle|第二数值轴标题；不改变序列使用哪一条数值轴。|Secondary value-axis title; it does not reassign series to that axis.
    SecondaryYMinimum|第二数值轴下界；空值自动计算，与 SecondaryYMaximum 配合使用。|Secondary value-axis minimum; null is automatic, paired with SecondaryYMaximum.
    SecondaryYMaximum|第二数值轴上界；空值自动计算，与 SecondaryYMinimum 配合使用。|Secondary value-axis maximum; null is automatic, paired with SecondaryYMinimum.
    ShowSecondaryAxis|显示第二数值轴；序列轴归属仍由序列模型配置。|Shows the secondary value axis; series models still determine axis assignment.
    YAxisTitle|主数值轴标题，参与实际文本度量与轴区域布局。|Primary value-axis title, included in text measurement and axis layout.
    ConnectorThickness|连接线描边宽度，单位 DIP；不改变节点的业务状态。|Connector stroke width in DIPs, independent of business node state.
    ViewportWidth|宿主提供给流程布局的可用宽度；用于节点排布，通常由模板维护。|Available width supplied to flow layout, normally maintained by the template.
    DayWidth|甘特时间轴每一天的 DIP 宽度；改变横向缩放，不修改任务日期。|DIP width of one day on the Gantt timeline; changes horizontal scale without modifying task dates.
    EmptyText|没有可显示数据时的提示文本；由应用覆盖以进行本地化。|Message when no data can be displayed; applications override it for localization.
    HeaderHeight|标题区高度；图表使用 DIP，报表布局使用其布局坐标，勿与毫米源模型混用。|Header height in chart DIPs or report layout coordinates; do not confuse it with millimetre source fields.
    OwnerHeader|甘特左侧负责人列的标题文本。|Header text for the Gantt owner column.
    ProgressHeader|甘特左侧完成进度列的标题文本。|Header text for the Gantt progress column.
    RowHeight|每一行的 DIP 高度；与视口和回收／直接绘制布局一起决定可见行数。|Row height in DIPs; combines with the viewport and rendering/recycling layout to determine visible rows.
    SelectedTask|当前选中的原始甘特任务；空值清除选择，保持模型对象身份。|Selected original Gantt task; null clears selection while preserving model identity.
    TaskHeader|甘特左侧任务名称列的标题文本。|Header text for the Gantt task-name column.
    TaskListWidth|甘特左侧任务信息区的 DIP 宽度；剩余区域分配给时间轴。|DIP width of the Gantt task-information pane; the timeline uses the remaining space.
    Today|时间轴上“今天”标记的日期；可显式设置以演示或固定业务日期。|Date used for the timeline's today marker; set explicitly for demos or a fixed business date.
    ViewStart|甘特可见日期范围起点；与终点共同定义时间轴，不修改任务模型。|Start of the Gantt date range; defines the timeline with its end without modifying tasks.
    Icon|图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。|Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes.
    IconPlacement|图标相对文本的位置枚举；左右或上下布局由模板处理。|Icon position relative to text; the template arranges horizontal/vertical placement.
    IconSize|图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。|Icon drawing size in DIPs, independent of font size and button hit area.
    IconSpacing|图标与相邻文本／内容的 DIP 间距。|DIP spacing between the icon and adjacent text/content.
    Spacing|子内容之间的 DIP 间距；不代替容器 Padding 或子项 Margin。|DIP gap between children, separate from container Padding and item Margin.
    ChromeHeight|图片预览工具栏／装饰区高度，单位 DIP。|Height of the image-preview chrome/tool area in DIPs.
    IsFitToWindow|启用适合视口的图片缩放；手动缩放与此状态的关系见图片预览交互约定。|Fits the image into the viewport; see image-preview interaction rules for manual zoom behavior.
    SelectedIndex|当前显示项的零基索引；越界处理遵循所属控件的选择契约。|Zero-based displayed-item index; range handling follows the owning control's selection contract.
    ShowCloseButton|显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。|Shows the close button; the host and associated commands retain control over closing.
    ShowHeader|显示标题区域；不删除模型中的标题内容。|Shows the header area without deleting the model's title content.
    ZoomFactor|图片显示缩放倍率；改变视图，不改写源图片像素。|Image display scale factor; changes the view rather than source pixels.
    HeaderActions|标题区右侧的操作内容；可放置业务按钮，保持命令和焦点行为。|Action content beside the header, such as business buttons with native command/focus behavior.
    HeaderIcon|标题区图标内容，与标题文本分开定制。|Header icon content, customized independently of its text.
    LoadingIcon|加载状态图标；为空时沿用默认指示器。|Loading-state icon; null retains the default indicator.
    LoadingIconSize|加载图标的 DIP 尺寸；不控制覆盖层的面积。|Loading icon size in DIPs, independent of overlay area.
    LoadingText|加载中的说明文本，可本地化；不代表任务完成结果。|Localizable loading message, distinct from the task's completion result.
    OverlayOpacity|覆盖层不透明度；范围及有限数约束见校验列。|Overlay opacity; see validation for range/finite-number constraints.
    IsActive|指示器是否运行动画；关闭时保留控件布局，全局动效开关仍生效。|Whether the indicator animates; disabling preserves layout and the global motion switch still applies.
    ChildrenBinding|读取菜单子集合的原生绑定；须使用具体模型的编译绑定，不用反射路径。|Native binding for menu children; use concrete compiled model bindings rather than reflection paths.
    ExpandedBinding|菜单项展开状态的绑定；绑定到稳定模型可跨回收保留展开。|Menu expansion binding; stable model state preserves expansion across recycling.
    FilterPlaceholder|菜单内置筛选输入的占位提示文本。|Placeholder text for the menu's built-in filter input.
    Indent|每一级层次增加的 DIP 缩进；不改变父子关系。|Additional DIP indentation per hierarchy level, without changing parent/child relationships.
    IsCollapsed|切换菜单为紧凑图标栏；子菜单通过悬浮弹层展示，保留当前选择。|Collapses the menu to an icon rail; children use hover popups while current selection is retained.
    IsGroupBinding|读取菜单项是否为分组的绑定；分组与普通可导航项行为不同。|Binding for whether an item is a group; groups differ from ordinary navigable items.
    IsItemEnabledBinding|读取单项可用性的绑定；禁用项不响应用户导航选择。|Binding for item availability; disabled items do not respond to user navigation selection.
    ItemHeight|单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。|Item/wheel-cell height in DIPs; theme density or explicit values may override registration defaults.
    SearchOwner|搜索结果关联的菜单实例；供筛选弹层定位并激活原始菜单项。|Menu instance associated with search results, used to locate and activate original items.
    SearchText|当前搜索文本；搜索与选中状态独立，不用查询字符串充当模型主键。|Current search text, independent of selection and model identity.
    SearchTextBinding|读取单项搜索文本的编译绑定；展示模板仍可独立定制。|Compiled binding that reads item search text; display templates remain independently customizable.
    ShowFilter|显示菜单内置筛选框；关闭后仍可由宿主提供独立搜索入口。|Shows the menu's built-in filter; the host may provide a separate search UI when hidden.
    TransitionDuration|展开／切换过渡时长；Motion.Enabled=False 时不执行动画。|Expansion/transition duration; no animation runs when Motion.Enabled=False.
    IsActiveAncestor|当前项是否为选中项的祖先；由菜单维护，不能用作普通子项的选中状态。|Whether this item is an ancestor of the selected item; menu-owned and distinct from child selection.
    IsCascadeItem|当前容器是否属于级联弹出菜单；由模板／菜单维护。|Whether this container belongs to a cascading popup, maintained by the menu/template.
    IsCompact|启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。|Enables compact layout; the template determines visible fields without altering business data.
    IsGroup|此菜单项是否为分组；不等同于当前项是否选中。|Whether this menu item is a group, distinct from selected state.
    HighlightQuery|需在源文本中高亮的查询；只改变绘制，不修改 SourceText。|Query highlighted within source text; affects rendering without changing SourceText.
    SourceText|高亮前的完整源文本；不要传入格式化的富文本标记。|Full unhighlighted source text, without rich-text markup.
    Buttons|对话框按钮配置集合／标志；由选项类型和关闭结果契约解释。|Dialog button configuration/flags, interpreted according to the options type and close-result contract.
    CloseOnEscape|允许 Esc 关闭对话框；结果与取消语义按对话框选项处理。|Allows Escape to close a dialog, following its cancellation/result semantics.
    ShowButtons|显示默认按钮区；自定义内容中的按钮不受此开关控制。|Shows the default button area; does not control buttons in custom content.
    ShowTitle|显示对话框标题区；标题值仍保留在选项中。|Shows the dialog title area while retaining its configured title value.
    LeftContent|窗口标题栏左侧扩展内容，位于库提供的标题栏结构内。|Additional content on the left side of the library window title bar.
    RightContent|窗口标题栏右侧扩展内容；避免覆盖原生窗口操作区域。|Additional right title-bar content; avoid covering window-operation hit areas.
    ShowThemeButton|显示窗口内置明暗切换按钮；配色资源仍由 ThemeColorPicker 或 OneTheme 控制。|Shows the built-in brightness toggle; palette resources remain controlled by ThemeColorPicker/OneTheme.
    ShowTitleBar|显示库提供的标题栏区域；窗口生命周期与 owner 不因此变化。|Shows the library title bar without changing window lifetime or ownership.
    ThemeButtonScope|窗口内置主题按钮的切换作用域；使用枚举区分应用与窗口。|Theme-toggle scope for the built-in button, distinguishing application and window via enum.
    TitleBarHeight|标题栏区域的 DIP 高度；改变时保留标题与窗口按钮点击空间。|Title-bar height in DIPs; retain sufficient title and window-button hit areas.
    Scope|主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。|Scope of Light/Dark switching; does not change the ThemeColor palette enum.
    SelectedColor|SelectedThemeColor 的兼容别名；两者表示同一颜色枚举，优先绑定 SelectedThemeColor。|Compatibility alias of SelectedThemeColor; both represent the same palette enum. Prefer binding SelectedThemeColor.
    JumpPrefixText|页码跳转框前的本地化提示。|Localized text before the page-jump input.
    JumpText|执行页码跳转的按钮文本。|Text on the page-jump action button.
    NextText|下一页按钮的可访问／显示文本。|Accessible/display text for the next-page button.
    PreviousText|上一页按钮的可访问／显示文本。|Accessible/display text for the previous-page button.
    PageSizeTextFormat|每页条数选择的格式文本；占位参数与分页文档一致。|Format text for page-size choices, using placeholders defined in the pagination guide.
    PageUnitText|页码后面的本地化单位文本。|Localized unit displayed after a page number.
    RangeTextFormat|当前数据范围的格式文本；不更改分页计算。|Format text for the current item range, without changing pagination calculations.
    TotalTextFormat|总条数的格式文本；仅负责展示。|Display-only format text for the total item count.
    ContentSource|读取关系节点正文的类型化字段访问器；不使用字符串反射路径。|Typed accessor for relation-node body content, without reflection paths.
    IconSource|读取关系节点图标的类型化字段访问器。|Typed accessor for relation-node icons.
    TitleSource|读取关系节点标题的类型化字段访问器。|Typed accessor for relation-node titles.
    RootNode|关系图的根模型；子节点通过模型树或访问器获取。|Root model for a relation graph; children come from its tree/accessor contract.
    HeaderFontSize|节点标题字号，单位 DIP，按实际文本度量参与布局。|Node-title font size in DIPs, included in actual text measurement.
    IconTileSize|图标承载底板的 DIP 尺寸，独立于图标自身尺寸。|Icon-tile size in DIPs, independent of icon size.
    IsRoot|此节点是否为根节点；由关系图生成器维护。|Whether this is the root node, maintained by the graph presenter.
    TextSpacing|标题与正文之间的 DIP 间距。|DIP spacing between title and body text.
    ConditionIndex|条件格式编辑器的当前选项索引；与选中列模型同步。|Current conditional-format editor option index, synchronized with the selected column.
    FontFamilyName|报表文本字体族名称；使用目标平台可用字体并检查回退。|Report text font-family name; use fonts available on the target platform and check fallback.
    FontSizeValue|报表属性编辑器当前字号值；与选中元素／文档同步。|Current font-size editor value, synchronized with the selected report element/document.
    FormatIndex|报表数值格式编辑器的当前选项索引。|Current option index in the report numeric-format editor.
    ImageFitIndex|报表图片适应模式编辑器的当前选项索引。|Current option index in the report image-fit editor.
    SelectedColumn|当前选中的报表列模型；空值表示未选择。|Selected report column model; null means no selection.
    SelectedElement|当前选中的报表元素模型；空值表示未选择。|Selected report element model; null means no selection.
    SelectedField|当前选中的可用报表字段；用于插入或编辑绑定。|Selected available report field, used for insertion/binding edits.
    SumIndex|报表汇总操作编辑器的当前选项索引。|Current option index in the report summary-operation editor.
    Zoom|视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。|View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates.
    DocumentJson|报表文档 JSON；读取／替换失败和数据刷新行为按报表加载契约处理。|Report document JSON; loading failures/replacement/data refresh follow the report loading contract.
    IsOpen|展开或显示内容；收起保留内容对象，过渡遵循 Motion.Enabled。|Expands/shows content; collapsing retains the content object and transitions follow Motion.Enabled.
    Direction|展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。|Reveal/port direction enum; choices are defined by its concrete enum type.
    MemberName|对应模型成员的准确名称；用于将校验错误映射到字段，不是显示标签。|Exact model member name used to map validation errors to a field, separate from its display label.
    IsBusy|上传处理中的状态；由异步上传过程维护，业务提交应尊重此状态。|Upload-in-progress state maintained by the async operation; business submissions should respect it.
    Progress|完成进度数值；范围／单位由所属上传或甘特模型契约定义，不用作成功标志。|Progress value with range/unit defined by its upload/Gantt model, distinct from a success flag.
    SelectedFile|当前选中的原始上传文件模型；空值取消当前项选择。|Selected original upload-file model; null clears the current selection.
    StatusText|当前操作状态的展示文本；业务逻辑使用状态枚举和结果，而不解析此文本。|Display text for operation status; business logic should use state/results rather than parse this text.
    CompletedCommandParameter|校验通过并完成步骤时传给 CompletedCommand 的业务参数。|Business argument passed to CompletedCommand when validation succeeds and the step completes.
    AllowCycles|允许连接形成环；关闭时由编辑器连接校验阻止环路，运行器仍需处理业务循环语义。|Allows cyclic connections; otherwise editor validation rejects cycles. Runners still own business-loop semantics.
    FitPadding|缩放适配内容时保留的 DIP 留白。|DIP padding reserved when fitting content to the viewport.
    IsPanMode|将指针拖动用于平移画布；选择和编辑手势按流程编辑器规则处理。|Uses pointer dragging to pan the canvas, following workflow selection/edit gesture rules.
    Pan|画布平移向量；只改变视口，不改写节点模型坐标。|Canvas pan vector, affecting the viewport without rewriting node coordinates.
    SelectedConnection|当前选中的连线模型；与节点选择的互斥规则见编辑器文档。|Selected connection model, following the editor's node/connection selection rules.
    SelectedNode|当前选中的节点模型；空值清除节点选择。|Selected node model; null clears node selection.
    ShowNodeLibrary|显示可添加节点的定义库面板。|Shows the library panel of node definitions available to add.
    ShowProperties|显示所选对象的属性编辑面板。|Shows the selected object's property editor panel.
    ShowRuntimePreview|显示运行时预览面板；实际业务执行由宿主运行器负责。|Shows runtime preview; the host runner owns actual business execution.
    ShowValidation|显示文档校验结果面板；关闭面板不等于绕过校验。|Shows document validation results; hiding the panel does not bypass validation.
    SnapToGrid|节点移动时吸附到网格；不修改已有节点，直到进行相应编辑。|Snaps moved nodes to the grid; existing coordinates remain until edited.
    Editor|缩略图关联的 WorkflowEditor；用于共享文档和视口，不创建独立工作流状态。|WorkflowEditor associated with the minimap, sharing document/viewport rather than creating independent workflow state.
    OpenOnClick|点击日期输入区域时打开原生选择弹层；不替换文本输入和键盘行为。|Opens the native date popup when clicking the input, retaining text and keyboard behavior.
    IsMouseSwipeEnabled|允许鼠标拖动轮播翻页；触摸和原生导航行为仍遵循 Carousel 契约。|Allows mouse dragging to swipe the carousel, while touch/native navigation follow Carousel semantics.
    PointIndex|选中数据点在所属序列中的零基索引。|Zero-based index of the selected point within its series.
    SeriesIndex|选中序列在源集合中的零基索引。|Zero-based index of the selected series in the source collection.
    Point|命中的原始图表数据点，供选择事件和业务详情读取。|Original hit-tested chart point for selection events/business details.
    Series|命中的原始图表序列模型。|Original hit-tested chart-series model.
    IsVisible|是否参与该序列的绘制和可见数据范围计算。|Whether the series participates in rendering and visible-data range calculation.
    UseMotionSettings|使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。|Uses shared motion configuration without forcing animation when globally/locally disabled.
    FrozenRightColumnCount|右侧冻结列数；与左侧冻结区域共同占用可用宽度。|Number of frozen columns on the right, sharing available width with left-frozen columns.
    ShowColumnFilters|启用表格列筛选入口；具体列还必须允许对应筛选能力。|Enables column-filter entry points; each column must also permit filtering.
    Error|操作失败的结构化／文本信息；与成功标志一起检查，不忽略失败。|Structured/text failure information; inspect it together with the success flag.
    Succeeded|本次操作是否成功；失败时读取 Error，不把空结果等同于成功。|Whether the operation succeeded; read Error on failure rather than treating empty output as success.
    ValidCount|统计中有效参与计算的值数量；不等同于源行总数。|Count of valid values included in the aggregate, distinct from total source rows.
    CloseOnOverlayClick|允许点击遮罩关闭弹窗；取消／返回结果遵循对话框选项。|Allows overlay clicks to close the dialog, following cancellation/result options.
    MotionEnabled|该弹窗的动效开关；同时遵守继承的 Motion.Enabled。|Dialog-specific motion switch, also respecting inherited Motion.Enabled.
    DialogOptions.Size|对话框尺寸枚举；采用库提供的尺寸档位。|Dialog size enum using the library's supported size presets.
    ControlSizing.Size|Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。|Shared Small/Normal/Large density inherited by descendants; explicit local values/classes can override it.
    Motion.Enabled|向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。|Inherited motion switch; disabling goes to final layout/state while preserving native input.
    Time|节点内容的时间显示文本；不参与节点执行或步骤排序。|Displayed node time text, independent of execution and step ordering.
    Status|节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。|Business node-state enum; styles derive from state rather than storing state in classes.
    Start|甘特任务起始日期，含当日；范围终点采用 EndExclusive。|Inclusive Gantt task start date, paired with EndExclusive.
    EndExclusive|甘特任务结束日期，不包含当日；里程碑和汇总任务按甘特契约解释。|Exclusive Gantt task end date; milestone/summary behavior follows the Gantt contract.
    IsMilestone|将任务显示为里程碑；不将其当作普通持续区间条。|Displays the task as a milestone rather than an ordinary duration bar.
    IsSummary|当前项是否为汇总任务；由树形任务计算结果维护。|Whether the item is a summary task, maintained by hierarchical task calculation.
    Owner|任务负责人文本，与控件弹窗的窗口 owner 是不同概念。|Task-owner text, distinct from popup/window ownership.
    ParentId|父任务的稳定标识；空值表示根级，不能形成循环。|Stable parent-task identifier; null denotes a root, and cycles are invalid.
    Task|命中的原始任务对象；供选择事件与业务详情使用。|Original hit-tested task object for selection events/business details.
    Fraction|仪表分段占比；配合其他分段定义绘制范围，不以像素宽度代替数值。|Gauge segment fraction defining its range with other segments, independent of pixel widths.
    CancelText|取消按钮的本地化文本；不改变取消返回值。|Localized cancel-button text without changing its result value.
    ConfirmText|确认按钮的本地化文本；不自动执行宿主业务操作。|Localized confirmation text; it does not execute host business logic itself.
    IsDangerous|将确认操作显示为危险语义样式；授权和执行仍由业务决定。|Applies danger semantics to confirmation; the business layer owns authorization/execution.
    EnterDuration|通知入场持续时间；遵守共享动效开关。|Notification entrance duration, respecting shared motion settings.
    ExitDuration|通知退场持续时间；遵守共享动效开关。|Notification exit duration, respecting shared motion settings.
    IsCurrent|是否为分页器当前页；由分页状态派生。|Whether this is the current pagination page, derived from paging state.
    Page|页码值；按分页器文档使用从 1 开始的页号。|One-based page number following the pagination contract.
    IndicatorSize|刷新指示器的 DIP 尺寸；不改变触发刷新所需的拉动距离。|Refresh-indicator size in DIPs, independent of the pull threshold.
    UseThemeVisualizerSizing|让刷新指示器跟随共享主题密度；显式定制时可关闭。|Makes the refresh visualizer follow shared theme density; disable for explicit sizing.
    Validator|步骤使用的校验器实例；返回错误关联成员名，复用 VerificationStep 校验契约。|Step validator instance whose errors identify members, following VerificationStep validation contracts.
    Message|可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。|Result message for users/diagnostics; status fields still determine the operation outcome.
    ResponseBody|HTTP 上传响应正文；由宿主按服务端协议解析，不假定固定 JSON 结构。|HTTP upload response body, parsed by the host according to its server protocol rather than an assumed JSON shape.
    StatusCode|HTTP 响应状态码；与上传成功及返回地址一起验证。|HTTP response status code, checked alongside upload success and returned URL.
    SuccessFileUrl|成功上传后返回的文件地址；可能为空，需与服务端响应适配。|Returned file URL after upload; may be empty and requires server-response adaptation.
    Virtualization.Enabled|启用库支持的虚拟化方案；仍需有限视口和模型数据源，不预建全部控件。|Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls.
    PickerScroll.Enabled|日期／时间选择滚轮的交互开关；不替换原生提交、取消和焦点行为。|Interaction switch for picker wheels, retaining native commit/cancel/focus behavior.
    HighlightClip|滚轮高亮区域的裁剪配置；通常由选择器模板维护。|Clipping configuration for the wheel highlight, normally maintained by picker templates.
    PopupHeight|选择弹层的 DIP 高度；与 ItemHeight 和 ViewportHeight 协调。|Picker-popup height in DIPs, coordinated with ItemHeight and ViewportHeight.
    ViewportHeight|选择滚轮可见区域的 DIP 高度；不等于数据项数量。|Visible picker-wheel height in DIPs, distinct from item count.
    HeaderName|携带鉴权结果的 HTTP 请求头名称；默认 Authorization，由加载器校验合法性。|HTTP header carrying resolved authentication, normally Authorization; validated by the loader.
    HeaderPrefix|拼接在 token 前的请求头前缀；默认 Bearer 后有空格，不保存解析后的 token。|Prefix prepended to the token, normally Bearer plus a space; resolved tokens are not persisted.
    Method|HTTP 请求方法，例如 GET 或 POST；应用应按服务端接口配置。|HTTP method such as GET/POST, configured according to the server API.
    ParameterMode|请求参数编码方式枚举；JSON 与表单处理见报表 API 数据源章节。|Request-parameter encoding enum; see report API source guidance for JSON/form behavior.
    Parameters|请求参数配置文本，按 ParameterMode 编码；不自动推导业务字段。|Request-parameter text encoded according to ParameterMode, without inferring business fields.
    TokenField|登录响应中 token 的路径；支持 JSON Pointer 或点分路径，必须得到非空字符串或数字。|Token path in the login response, as JSON Pointer or dot path; must resolve to a nonempty string/number.
    Url|请求的绝对地址；协议与加载边界遵循报表 HTTP 数据源契约。|Absolute request URL, following report HTTP source protocol/loading boundaries.
    Authentication|可选登录配置；运行期解析的凭据与报表序列化定义分开管理。|Optional login configuration; resolved runtime credentials are kept separate from the serialized report definition.
    Condition|报表条件格式的比较运算枚举，与 ConditionValue 一起使用。|Report conditional-format comparison enum, paired with ConditionValue.
    ConditionColor|命中列条件格式时使用的颜色字符串；分页器校验颜色合法性。|Color string used when a column condition matches, validated by the paginator.
    ConditionEnabled|启用此列的条件格式；关闭时保留条件配置但不应用颜色。|Enables conditional formatting for the column, retaining configuration when disabled.
    ConditionValue|报表条件比较的十进制阈值，与 Condition 运算符共同使用。|Decimal threshold compared using the report Condition operator.
    DecimalPlaces|数值展示的小数位数；仅格式化，不更改源数据精度。|Displayed decimal places, formatting without changing source precision.
    FieldKey|显式注册的数据字段键；不是反射属性路径，必须与数据源字段一致。|Explicitly registered field key rather than a reflection path; must match a source field.
    Format|字段显示格式枚举；自动、文本、数值、货币或日期，不改变源值。|Field display-format enum: automatic, text, numeric, currency or date, without changing source values.
    ShowSum|在汇总区域显示此列数值合计；非数值字段按报表规则处理。|Shows the column's numeric total in the summary, following report rules for nonnumeric fields.
    UseThousandsSeparator|数值格式启用千位分隔，仅影响展示文本。|Enables thousands separators in numeric display text only.
    WidthWeight|表格列的相对宽度权重；实际宽度按可用表格宽度分配。|Relative table-column width weight, allocated from available table width.
    DataBodyPointer|HTTP JSON 响应中记录数组的 JSON Pointer；空字符串表示响应根。|JSON Pointer to the record array in an HTTP JSON response; empty means the response root.
    DataSourceAuthentication|数据源请求使用的登录配置；不在文档内保存运行期 token。|Login configuration for source requests, without persisting runtime tokens in the document.
    DataSourceMethod|报表数据源的 HTTP 方法，默认 GET。|Report-source HTTP method, GET by default.
    DataSourceParameterMode|报表数据源参数的编码方式枚举。|Parameter-encoding enum for the report source.
    DataSourceParameters|报表数据源的参数配置文本，与编码方式共同使用。|Report-source parameter text, interpreted with its encoding mode.
    DataSourceUrl|报表远程 JSON 数据源地址；替换与刷新按报表加载契约处理。|Remote report JSON source URL, following report replacement/refresh loading rules.
    FontSize|报表文本字号，单位 DIP；文档和单元素范围校验由分页器处理。|Report font size in DIPs, with document/element range validation performed by the paginator.
    MinimumRowHeight|报表明细最小行高，单位 DIP；换行内容可使实际行高增加。|Minimum report-detail row height in DIPs; wrapped content may increase actual height.
    Subtitle|报表副标题文本，与主标题独立显示。|Report subtitle displayed independently of the main title.
    BarcodeShowText|条码下方是否显示原始文本；不改变编码内容。|Whether to show source text below a barcode, without changing encoded content.
    Bold|文本使用粗体／半粗体绘制；换行与布局仍按实际字体度量。|Renders text bold/semibold while wrapping/layout still use actual font metrics.
    Fill|报表元素填充颜色字符串；Transparent 表示透明，须为有效 Avalonia 颜色。|Report element fill-color string; Transparent is transparent and values must be valid Avalonia colors.
    ImageFit|图片适应方式枚举：Contain 保持完整、Cover 裁切覆盖、Fill 拉伸。|Image-fit enum: Contain preserves the whole image, Cover crops to fill, and Fill stretches.
    ImageName|嵌入图片的显示名称；图片数据仍由 ImageData 保存。|Display name for an embedded image whose bytes remain in ImageData.
    ImageUrl|远程图片地址；加载结果和原始配置分离，按报表图片加载约定处理。|Remote image URL, with loaded results separated from source configuration under report loading rules.
    IsChecked|报表复选框元素的勾选状态。|Checked state of a report checkbox element.
    Italic|文本斜体显示；目标字体不支持时由字体系统处理回退。|Italic text styling, subject to font-system fallback when unsupported.
    Kind|所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。|Kind enum for the owning field/property/element; see its concrete enum for choices.
    ListStyle|报表文本行的列表前缀：无、圆点或复选框样式。|Report text-line list prefix: none, bullet or checkbox style.
    ShowBorder|显示报表元素边缘线；不改变元素几何尺寸。|Shows a report element border without changing its geometry.
    TextAlignment|报表文本对齐枚举，包括左右、居中和两端对齐。|Report text-alignment enum, including left, right, center and justified.
    Underline|在报表文本上绘制下划线，不切换输入控件的 underlined 类。|Underlines report text, distinct from an input control's underlined class.
    Getter|显式类型化的字段读取委托；从原始行读取业务值，不执行字符串反射查找。|Explicit typed field getter that reads original rows without string-based reflection lookup.
    Pointer|JSON 数据的 JSON Pointer 路径；路径结构和根节点含义见数据源章节。|JSON Pointer path into source JSON; see source documentation for path/root semantics.
    DetailBottom|报表布局中明细区的底部坐标，单位 DIP；供分页与绘制共享。|Bottom coordinate of the report detail region in DIPs, shared by pagination/rendering.
    Margin|分页布局的默认边距，单位 DIP；与源模型的 MarginMm 区分。|Default paginated-layout margin in DIPs, distinct from source-model MarginMm.
    PageHeight|分页后纸张高度，单位 DIP，已经由毫米转换。|Paginated page height in DIPs, already converted from millimetres.
    PageWidth|分页后纸张宽度，单位 DIP，已经由毫米转换。|Paginated page width in DIPs, already converted from millimetres.
    Summary|按列对齐的汇总展示文本集合；由分页器从数据源计算。|Column-aligned summary display texts calculated by the paginator from source data.
    SummaryHeight|分页汇总区域高度，单位 DIP。|Height of the paginated summary region in DIPs.
    TableTop|分页表格区域顶部坐标，单位 DIP。|Top coordinate of the paginated table region in DIPs.
    TableWidth|分页表格的实际宽度，单位 DIP。|Actual paginated table width in DIPs.
    TableX|分页表格的横向起始坐标，单位 DIP。|Horizontal origin of the paginated table in DIPs.
    IsNumeric|此布局列是否按数值字段处理；影响对齐与汇总格式。|Whether the layout column is numeric, affecting alignment/summary formatting.
    Cells|一行按列顺序排列的最终文本；不再从此快照反查原始数据。|Final cell texts in column order; this snapshot is not a reverse lookup of original data.
    Foregrounds|按单元格顺序排列的条件前景色；与 Cells 索引一致。|Conditional foreground colors in cell order, aligned with Cells indices.
    SourceIndex|布局行对应的原始数据零基索引，用于定位源记录。|Zero-based source-data index for locating the original record.
    Number|分页结果中的页号；用于页码绘制。|Page number in the pagination result, used for page-number rendering.
    Rows|此页包含的原子明细行集合；不在打印阶段重新拆分行。|Atomic detail rows on this page, without splitting rows again during printing.
    ShowSummary|此页是否绘制汇总区域，通常由分页器对末页设置。|Whether this page renders the summary, normally assigned to the final page by the paginator.
    Copies|打印份数；由打印处理器校验并提交至选定打印机。|Copy count, validated/submitted by the print handler to the selected printer.
    PrinterName|选定打印机的名称；为空时由具体打印处理器决定默认行为。|Selected printer name; an empty value delegates default behavior to the print handler.
    KeepDocument|打印处理结束后是否保留生成文档；由结果接收方遵循释放契约。|Whether to retain the generated document after printing, following the receiver's disposal contract.
    SourceNodeId|连线起点节点的稳定 Id，须与文档节点集合对应。|Stable source-node Id of a connection, matching the document node collection.
    SourcePortId|连线起点的输出端口 Id，须属于 SourceNodeId 对应节点。|Source output-port Id, belonging to the SourceNodeId node.
    TargetNodeId|连线终点节点的稳定 Id，须与文档节点集合对应。|Stable target-node Id of a connection, matching the document node collection.
    TargetPortId|连线终点的输入端口 Id，须属于 TargetNodeId 对应节点。|Target input-port Id, belonging to the TargetNodeId node.
    FieldPath|上游输出字段的稳定路径键；由宿主定义，不解析 CLR 反射路径。|Stable upstream output-field path key defined by the host, without CLR reflection lookup.
    NodeId|引用或校验问题关联的节点 Id；可空类型表示没有定位到特定节点。|Node Id associated with a reference/validation issue; nullable values can indicate no specific node.
    OutputPortId|数据引用的上游输出端口 Id。|Upstream output-port Id used by a data reference.
    IsAnnotation|该节点仅为注释展示，不作为可执行业务步骤。|Marks an annotation node rather than an executable business step.
    Shape|节点外观枚举，Card、Pill 或 Note；不改变节点的 TypeId。|Node appearance enum, Card/Pill/Note, independent of TypeId.
    TypeId|宿主节点定义的稳定类型键；加载后用它恢复定义和非序列化内容，不进行 CLR 类型查找。|Stable host node-definition key used to restore definitions/nonserialized content after loading, without CLR type lookup.
    Category|节点库的分类名称；用于组织定义列表。|Category name organizing node definitions in the library.
    CreateNode|创建新节点模型的宿主工厂委托；每次返回独立实例并保留类型键。|Host factory creating independent node models with their type keys.
    DataType|宿主定义的类型键，例如 flow、text、number；用于端口／引用兼容判断，不是 CLR 类型名。|Host-defined type key such as flow/text/number for port/reference compatibility, not a CLR type name.
    Path|输出字段的稳定路径键；与数据引用中的 FieldPath 一致。|Stable output-field path key matching FieldPath in data references.
    Fields|端口公开的输出字段元数据；允许宿主描述可引用的数据结构。|Output-field metadata exposed by a port to describe referenceable data.
    MaxConnections|端口最大连线数；具体允许值及连接约束由流程文档校验处理。|Maximum connections for a port; workflow document validation enforces supported values/connection rules.
    IsRequired|字段／流程属性是否必填；校验器按所属模型契约生成错误。|Whether a field/workflow property is required; validation follows its owning model contract.
    Reference|指向上游输出的结构化引用；空值表示使用固定值，不保存解析后的运行数据。|Structured upstream-output reference; null uses the literal value, without persisting resolved runtime data.
    Literal|当前流程属性的固定值文本；通过文档编辑入口修改，以参与撤销历史。|Literal text of a workflow property, edited through the document to participate in undo history.
    SelectedOption|属性检查器选中的固定值／上游引用选项；切换通过文档历史更新 Reference。|Selected literal/upstream-reference option; switching updates Reference through document history.
    ConnectionId|校验问题关联的连线 Id；空值表示问题不定位到具体连线。|Connection Id associated with a validation issue; null means no specific connection.
    PageSize|每页行数；树形视图不支持分页，此成员遵循该视图的限制。|Rows per page; hierarchical views do not support paging and enforce their own restrictions.
    Depth|节点深度，根级从 0 开始；用于计算缩进。|Node depth starting at zero for roots, used to calculate indentation.
    HasChildren|节点是否存在子项；用于显示展开入口，不代表当前已展开。|Whether a node has children, controlling expansion affordances independently of current expansion.
    IsFilterMatch|当前节点是否直接匹配树筛选条件；祖先保留与直接命中分开。|Whether the node directly matches the tree filter, distinct from retaining ancestors.
    IsForcedExpanded|筛选是否强制展开当前节点以呈现匹配后代；与用户保存的展开状态分开。|Whether filtering forces expansion to reveal matching descendants, distinct from saved user expansion.
    Parent|树节点的直接父对象；根节点为空，保持原始模型身份。|Direct parent model of a tree node; null for roots, preserving original identity.
    Columns|报表列定义或分页列快照集合；前者可编辑，快照用于绘制，元素类型见签名。|Report column definitions or paginated snapshots; definitions are editable while snapshots serve rendering, as defined by their element types.
    Elements|报表自由元素定义或分页元素快照；源模型使用毫米，布局快照使用 DIP。|Free report element definitions or paginated snapshots; source geometry uses millimetres and layout snapshots use DIPs.
    Pages|分页器生成的页面快照集合；预览和打印共用该布局。|Page snapshots produced by the paginator, sharing layout between preview and printing.
    """);

    internal static string? Describe(TypeContract owner, PropertyContract property, bool zh)
    {
        if (Entries.TryGetValue(owner.Name + "." + property.Name, out var exact) || Entries.TryGetValue(property.Name, out exact)) return zh ? exact.Zh : exact.En;
        if (property.Name.EndsWith("Mm", StringComparison.Ordinal))
            return zh ? $"{property.Name}：纸张／元素几何尺寸，单位毫米；可空边距回退到 MarginMm。分页时转换为 DIP，不预先乘缩放倍率。" : $"{property.Name}: page/element geometry in millimetres. Nullable margins fall back to MarginMm. Pagination converts to DIPs; do not preapply view zoom.";
        if (property.Type.Contains("Brush", StringComparison.Ordinal))
        {
            var role = BrushRoles.GetValueOrDefault(property.Name);
            if (role != default) return zh ? role.Zh + "画刷；动态主题使用 One.* 资源，显式局部值优先。" : role.En + " brush; use One.* dynamic resources for themes. Explicit local values take precedence.";
        }
        return null;
    }
    private static readonly Dictionary<string, (string Zh, string En)> BrushRoles = Parse("""
    AccentBrush|主序列／强调色|Primary-series/accent
    SecondaryBrush|第二序列|Second-series
    TertiaryBrush|第三序列|Third-series
    QuaternaryBrush|第四序列|Fourth-series
    FifthBrush|第五序列|Fifth-series
    SixthBrush|第六序列|Sixth-series
    GridBrush|绘图区网格线|Plot grid-line
    MutedBrush|次要文字与辅助标记|Muted text/helper-mark
    CompletedConnectorBrush|已完成步骤连接线|Completed-step connector
    ConnectorBrush|未完成步骤连接线|Pending-step connector
    HeaderBrush|甘特表头背景|Gantt header background
    OnTaskBrush|任务条上方的对比文字|Contrasting text on task bars
    ProgressBrush|甘特已完成进度区|Gantt completed-progress area
    SelectionBrush|当前选中任务的强调背景|Selected-task highlight
    TaskBrush|甘特任务条底色|Gantt task-bar background
    WeekendBrush|时间轴周末区域|Timeline weekend region
    AlternateCheckerBrush|透明棋盘第二格|Alternate transparency checker
    CheckerBrush|透明棋盘第一格|Primary transparency checker
    OverlayBrush|加载覆盖层|Loading overlay
    ContentForeground|节点正文文字|Node body text
    IconBackground|图标底板|Icon-tile background
    IconForeground|图标前景|Icon foreground
    Brush|当前序列／分段|Current series/segment
    """);
    private static Dictionary<string, (string Zh, string En)> Parse(string text) => text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(line => line.Trim().Split('|')).ToDictionary(parts => parts[0], parts => (parts[1], parts[2]), StringComparer.Ordinal);
}
