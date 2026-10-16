namespace TheOne.Documentation;

internal static class MethodHelp
{
    private static readonly Dictionary<string, (string Zh, string En)> Entries = """
    DeferRefresh|延迟集合刷新并合并批量变更；在 using 范围结束时释放返回的刷新令牌。|Defers collection refresh to batch changes; dispose the returned token at the end of a using scope.
    IsFilterMatch|查询原始节点是否直接命中当前筛选条件。|Checks whether an original node directly matches the current filter.
    IsLeaf|查询原始节点是否为没有子项的叶节点。|Checks whether an original node is a leaf with no children.
    TryGetNodeState|尝试读取原始节点的层级、父级及展开状态；返回 false 表示未找到。|Attempts to read hierarchy/parent/expansion state; false means the node was not found.
    TrySetExpanded|尝试修改原始节点展开状态；返回值表示是否成功，仍遵守树状态限制。|Attempts to change original-node expansion; the result indicates success under tree-state restrictions.
    Dispose|解除订阅并释放实例持有的资源；释放后不再将其作为活动视图使用。|Detaches subscriptions and releases owned resources; do not reuse it as an active view afterwards.
    Render|Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。|Avalonia drawing callback using the supplied DrawingContext; business code normally does not call it directly.
    ExportPng|将当前图表绘制为 PNG；目标尺寸、缩放与输出流参数见签名，调用方负责流生命周期。|Renders the chart to PNG; see size/scale/stream parameters in the signature. The caller owns stream lifetime.
    GetLegendItemBounds|获取指定图例项的当前可绘制边界；布局尚未就绪或索引无效时可能没有结果。|Gets the current drawable bounds of a legend item; unavailable layout/invalid indices may return no result.
    HitTestLegend|按控件局部坐标命中图例项，供业务交互定位。|Hit-tests legend items in control-local coordinates for business interaction.
    GetLegendPageNavigationBounds|获取上一页／下一页图例导航按钮的绘制边界。|Gets rendered bounds of previous/next legend-page navigation.
    PageLegend|按指定增量切换图例页；不修改序列源数据。|Moves legend pages by the supplied delta without modifying series data.
    ResetViewport|重置图表缩放／平移视口，保留原始序列数据。|Resets chart zoom/pan while preserving original series data.
    GetConfiguredTransition|读取内容切换的已配置过渡；是否播放仍由动效开关决定。|Reads the configured content transition; motion settings still determine playback.
    GetSummaryResult|读取指定列的当前统计结果，包括有效数量、汇总值和错误。|Reads current column aggregates, including valid count, result and errors.
    ApplyFilter|应用列筛选条件并返回应用结果；失败信息通过结果／筛选错误状态获取。|Applies a column filter and returns its result; inspect result/filter-error state on failure.
    ClearFilter|清除指定列筛选，不重置其他列的条件。|Clears the specified column filter while preserving other conditions.
    ClearAllFilters|清除表格所有列筛选并刷新可见行。|Clears all column filters and refreshes visible rows.
    Matches|判断输入值是否符合此筛选条件；具体空值和比较规则见条件类型。|Checks a value against this filter; null/comparison semantics follow the condition type.
    Create|通过显式类型参数、模型及委托创建适配器／数据源；签名中的 getter 保留编译类型信息。|Creates an adapter/source from explicit types, models and delegates; typed getters preserve compile-time information.
    Convert|执行该转换器的布局值转换；由模板绑定调用，输入类型与返回类型见签名。|Performs the converter's layout-value conversion for template bindings; see input/result types in the signature.
    FocusFirstError|定位并聚焦第一个校验失败字段；无错误或不可聚焦时按返回结果处理。|Locates/focuses the first failing field; inspect its result when none exists or focus is unavailable.
    ScrollToToday|水平滚动甘特时间轴以定位 Today 日期。|Scrolls the Gantt timeline horizontally to locate Today.
    GetValue|通过显式字段键／类型化 getter 从原始行读取值；不使用反射路径。|Reads an original-row value through an explicit field key/typed getter, without reflection paths.
    ValidateAsync|异步校验并返回成员关联的错误；遵守取消令牌，不将完成等同于通过。|Validates asynchronously and returns member-associated errors; honors cancellation and distinguishes completion from success.
    PickAsync|异步打开文件选择；由宿主文件选择器实现，取消时按返回集合处理。|Asynchronously picks files via the host picker; interpret cancellation through its returned collection.
    Previous|切换到上一张可用预览图片。|Moves to the previous available preview image.
    Next|切换到下一张可用预览图片。|Moves to the next available preview image.
    Reload|重新加载当前图片；原始来源与错误状态遵循图片加载契约。|Reloads the current image under its source/error loading contract.
    FitToWindow|将图片缩放到适合当前视口，不修改源像素。|Fits the image into the viewport without modifying source pixels.
    ActualSize|按实际像素比例显示图片；视口可能出现滚动范围。|Displays the image at actual pixel scale, potentially creating scrollable overflow.
    ZoomIn|放大当前预览视图；遵守控件的缩放范围。|Zooms the current preview in within supported limits.
    ZoomOut|缩小当前预览视图；遵守控件的缩放范围。|Zooms the current preview out within supported limits.
    ToggleFullScreen|切换图片预览的全屏展示状态。|Toggles image-preview fullscreen presentation.
    OpenDropDown|打开多选弹层并聚焦搜索；控件必须已挂载且可用，返回是否打开。|Opens the multiselect popup and focuses search; requires an attached/enabled control and reports whether it opened.
    CloseDropDown|关闭多选弹层，按交互上下文将焦点返回输入区。|Closes the multiselect popup and restores field focus according to interaction context.
    ToggleItem|切换原始模型的选择，遵守禁选及只读规则；返回 false 表示没有变更。|Toggles an original model subject to disabled/read-only rules; false means no change.
    GetText|使用已配置的类型化适配器读取原始模型显示／搜索文本。|Reads original-model display/search text through the configured typed adapter.
    IsEnabled|使用适配器判断原始模型能否参与用户选择。|Uses the adapter to determine whether an original model permits user selection.
    CloseSubmenu|关闭当前级联子菜单并清理其弹出状态，保留业务选择。|Closes the current cascading submenu and clears popup state while retaining selection.
    ToString|返回此搜索结果的展示文本，不作为模型身份或序列化格式。|Returns result display text, not model identity or a serialization format.
    ShowAsync|在指定或解析出的 owner 上显示消息／对话框并异步等待结果；owner、取消及关闭语义见窗口服务指南。|Shows a message/dialog on an explicit or resolved owner and asynchronously awaits its result; see window-service ownership/cancellation/closing rules.
    ShowContentAsync|在对话框宿主中显示自定义内容并等待结果；内容负责其业务命令。|Shows custom dialog content and awaits its result; the content owns business commands.
    Show|在指定或解析出的 owner 上显示通知／Toast；返回句柄和生命周期按该服务契约处理。|Shows a notification/toast on an explicit or resolved owner; handles/lifetime follow the service contract.
    CloseAll|关闭此服务在对应 owner 范围内的全部活动消息。|Closes all active messages in the service's applicable owner scope.
    GoToPage|切换到指定页码；分页器与报表的页码／越界处理分别见其契约。|Navigates to a requested page; paging/report contracts define indexing and range handling.
    GoPrevious|切换到上一页，边界时不产生无效页号。|Moves to the previous page without producing an invalid boundary page.
    GoNext|切换到下一页，边界时不产生无效页号。|Moves to the next page without producing an invalid boundary page.
    GoFirst|切换到第一页。|Navigates to the first page.
    GoLast|切换到最后一页。|Navigates to the last page.
    ShowPreview|将报表设计器切换到预览模式，保留同一文档。|Switches the report designer to preview mode while retaining its document.
    ShowDesign|将报表设计器切换回设计模式。|Switches the report designer back to design mode.
    BindField|将选中报表元素／列绑定到显式注册的数据字段。|Binds a selected report element/column to an explicitly registered data field.
    LoadAsync|异步加载报表 JSON 及相关数据／图片；遵守取消、失败和新旧加载隔离契约。|Loads report JSON and associated data/images asynchronously, respecting cancellation, failures and stale-load isolation.
    ExportPdfAsync|将当前报表布局异步导出为 PDF；输出路径与取消参数见签名。|Asynchronously exports the current report layout to PDF; see output/cancellation parameters.
    PrintAsync|调用配置的打印处理器并等待结果；主库不替宿主选择打印依赖或静默提交策略。|Invokes the configured print handler and awaits its result; the host chooses printing dependencies/submission policy.
    For|为指定模型／属性集合创建类型化步骤校验器。|Creates a typed step validator for the supplied model/member set.
    Close|通过标签页关闭约定关闭目标项，执行可关闭性与命令判断。|Closes a target tab under its closeability/command contract.
    Move|按标签页重排约定移动目标项，保留原始数据对象。|Moves a tab under its reordering contract while retaining original data objects.
    OpenReadAsync|异步打开上传文件的只读流；调用方在使用后释放流。|Asynchronously opens an upload file's readable stream; the caller disposes it after use.
    Reset|重置校验步骤的完成／错误状态；模型值的处理遵循步骤契约。|Resets step completion/error state; model-value handling follows the step contract.
    GoBack|回到前一步，保留步骤模型，按导航规则更新状态。|Moves back one step, retaining models and updating navigation state.
    ToWorld|将画布视口坐标换算为文档世界坐标，计入缩放和平移。|Converts viewport coordinates into document-world coordinates, accounting for zoom/pan.
    CancelGesture|取消当前画布手势并清理拖动／连线预览。|Cancels the current canvas gesture and clears drag/connection previews.
    AddNode|向当前工作流文档加入节点；通过文档历史入口保存可撤销操作。|Adds a workflow node through document history so the edit can be undone.
    SelectNode|选择指定的原始流程节点；与连线选择按编辑器规则协调。|Selects an original workflow node under the editor's connection-selection rules.
    Connect|尝试创建节点端口连接，复用文档连接校验和历史。|Attempts a port connection using document validation/history.
    DeleteSelection|删除当前选中的节点／连线，并参与撤销历史。|Deletes selected nodes/connections through undo history.
    FitToView|调整缩放和平移以容纳文档节点，保留模型坐标。|Adjusts zoom/pan to fit document nodes without changing model coordinates.
    Locate|定位目标节点／校验问题，使其进入当前视口。|Locates a node/validation issue in the current viewport.
    ArrangeNodes|自动整理节点布局，通过文档编辑历史记录坐标变更。|Arranges node layout while recording coordinate changes in document history.
    CopySelection|复制当前选择为编辑器支持的片段，保持序列化边界。|Copies the selection into a supported editor fragment under serialization boundaries.
    PasteSelection|粘贴已复制片段，创建独立标识并记录可撤销编辑。|Pastes a copied fragment with independent identities and undoable edits.
    PrintPdfAsync|将现有 PDF 交给桌面打印适配器并返回结果；平台支持与保留文件规则见打印文档。|Passes an existing PDF to the desktop print adapter and returns its result; see platform/file-retention rules.
    Load|从 JSON 载入类型化业务文档；版本、字段及结构校验见对应序列化契约。|Loads a typed business document from JSON under its version/field/structure validation contract.
    IsRootField|判断字段键是否使用报表根级字段前缀。|Checks whether a field key uses the report root-field prefix.
    ReadRootValue|从 JSON 根按指定路径读取标量字段。|Reads a scalar field from the JSON root using its path.
    FindBodies|枚举 JSON 中可作为报表明细记录源的数组及其路径。|Finds JSON arrays and paths that can serve as report detail-record sources.
    CreateSource|从 JSON 记录数组路径和显式字段配置创建报表数据源。|Creates a report source from a JSON record-array path and explicit field configuration.
    Paginate|校验文档并计算不可变分页布局，供设计预览与打印共享。|Validates the document and computes immutable paginated layout shared by preview/printing.
    ExportAsync|异步输出分页报表 PDF；调用方提供目标并遵守取消与文件生命周期。|Exports a paginated report PDF asynchronously with caller-supplied destination/cancellation/file lifetime.
    LoadDataAsync|按报表 API 配置异步加载记录，不在序列化文档内保存返回记录。|Loads records asynchronously from report API configuration without persisting fetched rows in the template.
    LoadImagesAsync|异步解析报表外部图片，按加载批次维护缓存和失败状态。|Resolves report images asynchronously with batch-owned caching/failure state.
    Save|将业务文档保存为可移植 JSON；不包含运行期凭据、视觉对象或非持久化状态。|Saves portable business-document JSON, excluding runtime credentials, visual objects and transient state.
    Undo|撤销最近一次可撤销的文档编辑。|Undoes the most recent undoable document edit.
    Redo|重做最近撤销的文档编辑。|Redoes the most recently undone document edit.
    ClearHistory|清空撤销与重做记录，保留当前文档数据。|Clears undo/redo history while retaining current document data.
    Remove|删除指定工作流对象及关联关系，通过文档编辑历史执行。|Removes a workflow object/associated relationships through document history.
    MoveNodes|批量更新节点坐标并记录一次可撤销移动。|Updates node coordinates in a batch recorded as an undoable move.
    SetProperty|通过文档修改流程属性的固定值／引用，以保持撤销与通知。|Edits workflow literal/reference properties through the document to preserve undo/notifications.
    CheckConnection|校验候选端口连接，返回失败原因而不创建连线。|Validates a candidate port connection and reports failures without creating it.
    TryConnect|校验并尝试创建连接；返回是否成功及相关结果。|Validates and attempts a connection, returning success and associated results.
    IsReachable|判断流程图中目标节点是否可从起点到达，用于连线与循环判断。|Checks graph reachability for connection/cycle decisions.
    GetReferenceOptions|为当前节点属性列出兼容的上游数据引用选项。|Lists compatible upstream references for the current node property.
    Validate|校验工作流结构、端口与属性引用，返回可定位的问题集合。|Validates workflow structure, ports and references, returning locatable issues.
    Refresh|从底层模型与文档重新同步属性检查器，包含撤销后及上游变化。|Resynchronizes the inspector from its model/document, including undo and upstream changes.
    """.Split('\n', StringSplitOptions.RemoveEmptyEntries).Select(line => line.Trim().Split('|'))
        .ToDictionary(parts => parts[0], parts => (parts[1], parts[2]), StringComparer.Ordinal);

    internal static string? Describe(string name, bool zh) => Entries.TryGetValue(name, out var entry) ? zh ? entry.Zh : entry.En : null;
}
