# ReportDesign 报表设计器

[English](../en/report-design.md) · [控件文档](README.md) · [ReportPreview 预览控件](report-preview.md)

`ReportDesign` 是以自由排版为核心的桌面报表设计器，数据源可选，不连接接口也能设计静态报表。加载 `OneTheme` 后可直接使用；编辑器复用 `One.*` 资源与三档尺寸，打印纸张始终为白底，并保持真实比例。

```xml
<one:ReportDesign Document="{Binding Report}"
                  DataSource="{Binding SalesSource}"
                  SaveCommand="{Binding SaveReportCommand}" />
```

视图声明正确的 `x:DataType`；`one` 为 `using:TheOne.Avalonia.Controls`，模型位于 `TheOne.Avalonia.Reporting`。

## 工作台与拖放编辑

顶部报表名称使用原生 `TabStrip`，包含文档标题和内置关闭操作，旁边为保存状态及文档操作，下面为格式工具栏。沿用原生标签项内容布局，移除标签栏外层重复边框与留白。关闭当前唯一文档会清空 `Document`、`DataSource`，禁用保存与 PDF 导出；新建／打开仍可恢复文档。左侧组件／数据栏贯穿工作区高度，设计／预览页签只位于纸张区域顶部。右侧采用“属性／数据”两个页签，几何、内容、字体和颜色在属性页分块排列。组件／数据（`SourceTabs`）、属性／数据（`PropertyTabs`）及设计／预览（`ViewTabs`）统一使用内置 `TabControl` 的 `Classes="card"` 样式，不自定义页签模板，保留原生键盘交互。这些固定功能页显式关闭 `TabFeatures.CanClose` 与 `TabFeatures.CanReorder`，不可关闭或重排。报表名称栏使用 [TabFeatures 关闭能力](collections.md#tabstrip-独立标签导航)。

从左侧拖入文本、表格、图片、线条、矩形或页码。组件按钮采用紧凑的 44 px 最小高度，结构树占据左侧剩余空间。拖动元素可移动，右下角手柄可缩放；表格还可通过外侧右边缘手柄调整整体宽度，保持各列比例。内部列边界用于调整相邻列宽，表头与明细之间的边界改变表头高度，底边改变明细模板行高。列宽保存为相对权重 `WidthWeight`；`TableHeaderHeightMm` 保存表头高度，实际高度还会按文字测量增高。

几何使用 mm，支持 1 mm 吸附、方向键微移（Shift 为 5 mm）、Delete 删除及撤销／重做，一次画布拖动记为一次编辑。工具栏编辑字体、字号、粗体、斜体、下划线、对齐和列表；属性栏与边框／填充下拉框使用内置 `ColorPicker`。字号编辑器及 `FontSizeValue` 使用印刷点 pt；模型 `ReportElement.FontSize` 仍为 DIP，换算为 `pt = DIP × 0.75`，新增元素默认 12 pt（16 DIP）。

字体下拉框通过 Avalonia 的 `FontManager` 读取本机系统字体，不使用固定预设列表。当前文档字体即使未在本机安装也会保留为可选项，绘制时按可用字体回退；保存字体名称不会将字体文件嵌入模板。

点击纸张元素或结构树可选中元素，点击表格模板单元格可编辑该列。结构树按标题、页眉、明细、汇总和页脚组织元素；点击结构项会切回设计模式，并将对应元素、单元格或区域滚动到可见位置。点击子项时保持父节点展开；元素选择只同步树中选中项，不重建层级。重命名或结构编辑需要重建时，保留原有展开状态。当前一个文档支持一个重复明细表格，其他元素可有多个。

## 页面设置与固定区域

“页面设置”打开所属窗口的模态对话框，分组编辑纸张／方向、四边距及固定区域，并提供实时比例纸张预览与就地校验。支持 A4、A3、Letter 和自定义尺寸，继承宿主实际主题与尺寸密度；表单可滚动，主次操作按钮固定在底部。应用前先校验草稿，取消或关闭窗口不修改文档。

`PageWidthMm` / `PageHeightMm` 为物理纸张尺寸；可空的 `MarginLeftMm`、`MarginTopMm`、`MarginRightMm`、`MarginBottomMm` 分别覆盖旧版统一边距 `MarginMm`。

页眉从 `TopMargin` 开始，高度为 `HeaderHeightMm`；页脚从 `PageHeightMm - BottomMargin - FooterHeightMm` 开始。画布绘制这些已知区域边界，标尺旁标签按区域放置，不随表格漂移。设计模式可上下拖动左侧页眉／页脚边界手柄改变区域高度，页脚元素跟随页脚边界移动。分页为页脚区域保留空间，固定元素与表头在每页重复；区域之间必须留有明细空间。A4 保持真实比例，通过滚动查看，不为填满视口拉伸。

## 网络图片与 MVVM JSON 保存

服务器图片使用 `ReportElement.ImageUrl`：在图片属性中输入 HTTP / HTTPS 网络路径并点击“加载网络图片”。下载内容只缓存在运行时，不写入 JSON；模板保存 `ImageUrl`，预览端重新获取。仍可通过 `SetImage` 导入本地 PNG、JPEG、WebP 或 BMP，写入内嵌 `ImageData`。支持等比适应、等比填充裁剪和拉伸；单张上限 8 MiB、1600 万像素，一份报表的网络图片合计上限 32 MiB、3200 万像素。本地图片替换校验失败保留原图；改用新的网络地址后加载失败，设计模式显示加载／失败占位，不继续绘制旧地址缓存。图片提示只在设计模式显示。

`SaveCommand` 类型为 `ICommand`。保存按钮将当前 `ReportDocument` 作为参数并遵循 `CanExecute`；消费者在命令中调用 `document.Save()` 获取 JSON，自行保存至服务器。执行命令本身不代表保存成功，持久化成功后调用 `designer.MarkSaved()` 更新状态。未设置命令时使用系统本地保存对话框；“打开”读取本地模板，“新建”创建空白报表。

```csharp
// 消费者的保存命令处理方法中：
string json = document.Save();
// 此处等待应用自己的仓储或 HTTP 保存操作完成。
designer.MarkSaved(); // 仅在持久化成功后调用

// 后续取回 JSON：
ReportDocument restored = ReportDocument.Load(json);
designer.Document = restored;
```

`ReportDocument.Save()` / 静态 `Load(json)` 封装版本化的 `ReportTemplate` JSON，保存纸张、边距、区域高度、元素、字体、列、绑定、格式、合计、条件、内嵌图片，以及公共 `DataSourceUrl`、`DataBodyPointer`、`DataSourceAuthentication`、`ImageUrl`。不保存数据记录、接口响应、运行时图片缓存、HTTP 客户端及已获取的 token。登录入参属于模板定义，会随 JSON 保存；需要将凭证保留在模板之外的应用，可向独立预览提供消费者持有的已鉴权客户端。

无效模板不会替换当前文档。模板上限 32 MiB、1000 个元素、100 列和一个表格；源生成序列化元数据支持 NativeAOT。`SaveTemplate()` / `LoadTemplate(json)` 继续可用。加载会清理上一份报表的运行时源，然后按保存的接口、方式、入参、鉴权和集合路径自动恢复数据及响应树，无须再次拖字段。需要等待数据及绑定网络图片加载完成（例如导出前），可用 `await LoadTemplateAsync(json, cancellationToken)`。网络／字段错误保留已加载定义和绑定、显示错误，并向显式异步调用方抛出异常；无接口模板仍可静态设计。

```csharp
var picture = designer.AddElement(ReportElementKind.Image, 150, 30);
picture.ImageUrl = "https://reports.example.com/logo.png";
picture.ImageFit = ReportImageFit.Contain;
var json = designer.Document!.Save();
await designer.LoadTemplateAsync(json);
await using var pdf = File.Create("report.pdf");
await designer.ExportPdfAsync(pdf); // UI 线程调用，包含全部页面
```

PDF 复用分页与 Avalonia / Skia 绘制，文字和图形保留为矢量，图片嵌入 PDF；物理 mm 按 72 点／英寸换算，去除标尺、选框和编辑器。无源模板也能导出占位符，非法布局阻止导出。系统字体与字形回退可能影响换行排版。

## 可选 API 数据与绑定

“数据”页签独占整个左侧栏，响应树填满接口操作与搜索框下方的剩余高度。“报表结构”只在“组件”页签内显示，位于紧凑组件栏下方；切换页签保留结构选中状态。切换组件／数据还会保留数据树的展开、选中、滚动位置及搜索条件，不会重复请求接口。源和查询未变时复用筛选后的根节点，避免刷新重建节点而丢失选择。

给已挂载设计器赋值保存的 `Document` 也会自动取数；应用已显式提供数据源时保留应用数据。自动加载过程中赋值 `DataSource` 会接管并取消当前取数。关闭／替换文档取消旧请求，迟到响应不能覆盖新报表。未完成的自动加载在卸载时中断、重新挂载后恢复；已加载数据在缓存导航返回时保留，不再重复请求。自动加载整体限时 30 秒，运行时数据不写入模板 JSON，也不会作为一次定义修改加入撤销栈。

在“数据”页签点击“添加数据源”，对话框支持 HTTP / HTTPS URL、GET / POST / PUT / PATCH / DELETE 和请求参数。参数输入为 JSON 对象（上限 64 KiB），GET 编码到查询字符串并保留 URL 已有参数；其他方式支持 JSON 请求体或 Form 表单，嵌套 Form 值按 JSON 字符串传递。请求超时 20 秒，响应上限 4 MiB。

点击对话框“获取数据源”先查看格式化响应 JSON，成功后才能确认导入树；修改请求设置会使上次结果失效，必须重新获取。取消／关闭不修改已有数据源；确认不会重复发送请求。JSON 文本预览最多显示 20 万字符，导入保留完整响应。导入后按对象／单值／集合展示实际索引与数值预览，当前绑定记录保持原样，直到拖入节点建立绑定。

单值节点可拖入文本或指定表格单元格，填入当前值并保留绑定；拖到空白纸张创建绑定文本。包含 HTTP / HTTPS 图片路径的单值可拖入图片组件，加载并绑定图片。对象只用于组织层级，不作为值拖入。

集合只能拖到已有表格，打开逐列字段映射模态框，显示各列、字段选择与首条数据样例，长列表使用原生滚动。确认后切换明细集合并一次应用全部映射，可作为一次编辑撤销。未选字段的列保留静态 `ReportColumn.Text`、清除旧绑定；未绑定或非数字列关闭求和与数字条件。取消或关闭对话框不改变列和数据源；集合不能拖到文本或空白纸张。

所选接口与集合 JSON Pointer 保存为 `DataSourceUrl`、`DataBodyPointer`。`$root/company`、`$root/orders/0/logo` 等绝对单值键可与集合的相对字段并存。JSON 保存这些绑定键，独立预览重新获取响应后，可同时恢复根单值、明细记录及绑定图片地址。转义 JSON Pointer 保留带点／斜杠属性名和实际数组索引，不与嵌套路径混淆。请求失败保留原设计和数据源。

绑定节点时同时保存 `DataSourceMethod`、`DataSourceParameters`、`DataSourceParameterMode`，独立预览复用相同请求。`FetchApiDataAsync(url, cancellationToken)` 保留 GET 兼容入口；`FetchApiDataAsync(new ReportApiRequest(url, "POST", "{\"page\":1}"), cancellationToken)` 支持配置请求。`ReportResources.FetchJsonAsync(request, client, cancellationToken)` 可使用消费者提供的 `HttpClient`。

对话框按“权限勾选 → 登录与权限请求头 → 数据接口”从上到下排列。分区使用对话框本身的背景，输入框保留原生底色。勾选后显示登录 URL、方式、JSON／Form 入参、响应 token 字段和请求头配置。“获取权限”只调用登录接口，不依赖数据 URL，显示登录响应 JSON 和解析出的请求头值；token 路径错误时保留 JSON 便于检查。单独验证权限不导入数据或启用确认按钮；“获取数据源”仍执行完整的登录／取数流程。

请求头名称支持下拉选择 Authorization、X-API-Key、X-Auth-Token、Token、Access-Token、Auth-Token，也可自由输入；使用原生 `AutoCompleteBox` 和显式下拉按钮实现可编辑选择。token 字段支持 JSON Pointer（如 `/data/token`，包括 `~0`／`~1` 转义）或点分路径（如 `data.token`），值必须为非空字符串或数字。请求头值自动组合“可选前缀 + 字段值”；`Bearer ` 的尾部空格会保留，直接使用 token 时将前缀留空。

```csharp
var request = new ReportApiRequest("https://reports.example.com/rows")
{
    Authentication = new ReportApiAuthentication
    {
        Url = "https://reports.example.com/login", Method = "POST",
        Parameters = "{\"username\":\"example\",\"password\":\"example\"}",
        ParameterMode = ReportApiParameterMode.Json,
        TokenField = "/data/token", HeaderName = "Authorization", HeaderPrefix = "Bearer "
    }
};
await designer.FetchApiDataAsync(request);
```

绑定已获取的节点时，设置写入 `ReportDocument.DataSourceAuthentication`，参与撤销／重做；`null` 表示直接请求。每次按“登录 → 取 token → 请求数据”的顺序执行，登录失败、token 无效或取消时不继续请求数据。权限头只添加到这一次数据请求，不修改共享客户端默认请求头，也不自动传给独立图片请求。对话框先显示解析出的请求头值和数据 JSON，再确认导入。实际 token 不进入模板，独立预览会重新登录。登录和数据响应分别受相同的 4 MiB／20 秒限制。当前编辑器支持一个 token 请求头，不包含交互式 OAuth、刷新 token 流程或多个鉴权头。

## 复选框与条码

组件栏支持拖入 `ReportElementKind.CheckBox` 和 `Barcode`，继承现有移动、缩放、结构定位与撤销行为。复选框 `Text` 为标签，`IsChecked` 为默认状态；绑定字段优先，支持布尔值、1/0、yes/no、是/否，空值视为未勾选，其他值阻止预览／导出，拖入布尔字段不覆盖标签。

条码采用 Code 128B，`Text` 为静态码值或由 `FieldKey` 绑定，`BarcodeShowText` 控制是否显示码值。支持 1–128 个可打印 ASCII 字符，包含校验码、结束码和两侧 10 模块静区；不支持中文、GS1/FNC 或其他条码类型。模块宽度最少 0.75 DIP（约 0.20 mm），尺寸不足会报告错误。条码和复选框直接绘制矢量，进入模板 JSON、独立预览与 PDF；实际打印机分辨率和扫描设备仍需使用者验证。

树展示深度为 16 层，每个数组最多展示前 200 个实际索引，整树约 4000 个节点，超出部分显示截断提示。这些限制只影响展示，集合绑定仍读取完整源行。数据源最多支持 4096 个不同的行字段键，超出元数据上限会明确报错；自动根字段发现也有上限，模板已保存的根绑定按需恢复。

数据搜索框与 TreeView 固定使用原生 Small 尺寸，不跟随宿主密度放大。节点以单行展示属性名及值／数量摘要；长内容省略，悬停可查看名称、JSON Pointer 和值预览。有显示数量上限的响应树使用原生 StackPanel 项面板与单一物理滚动视口，避免嵌套虚拟面板在展开后滚动丢失内容；横向内容限制在侧栏宽度内。树上方隐藏获取／绑定成功提示，获取或绑定失败的信息仍会显示。

`FetchApiDataAsync(url, cancellationToken)` 获取并展示响应树。公开 `ReportDataNode` 提供 `Kind`（`Scalar`／`Object`／`Collection`）、`Pointer`、`FieldKey`、`Preview`、`Children` 和 `IsExpanded`；`DropDataNodeAsync(node, x, y)` 按 mm 坐标执行同样的拖入流程。`ReportJsonData.CreateTree(json)` 创建节点；`CreateSource(json, bodyPointer, rootFieldKeys: ...)` 创建集合源并补充指定的根单值键：

```csharp
var nodes = ReportJsonData.CreateTree(responseJson);
var source = ReportJsonData.CreateSource(responseJson, "/orders", ["$root/company"]);
var collectionNode = nodes[0].Children.Single(item => item.Pointer == "/orders");
await designer.DropDataNodeAsync(collectionNode, 30, 70); // 此位置已有表格
```

`FindBodies(json)` 仍可作为 JSON 数据体工具使用，设计器通过树节点直接拖入。

也可直接提供强类型数据，不经过 HTTP：

```csharp
var report = new ReportDocument { Title = "销售明细报表" };
report.Columns.Add(new ReportColumn { Header = "产品", FieldKey = "Product" });
report.Columns.Add(new ReportColumn
{
    Header = "金额", FieldKey = "Amount", Format = ReportValueFormat.Currency,
    DecimalPlaces = 2, ShowSum = true
});
var source = ReportDataSource.Create("Sales", sales,
    new ReportField<Sale>("Product", "产品", row => row.Product),
    new ReportField<Sale>("Amount", "金额", row => row.Price * row.Quantity, ReportValueKind.Number));
designer.Document = report;
designer.DataSource = source;
```

getter 显式注册，嵌套访问写进 lambda，不扫描反射属性或动态编译表达式。格式支持自动、文本、数字、货币和日期，当前使用 `zh-CN`、货币前缀 `¥`；只有数字字段可求和，空值显示为空。数字条件支持大于、小于或等于阈值时改变文字颜色，不修改值及合计；条件保存在 JSON，设计、预览和 PDF 使用同一结果。

## 预览与更新契约

`ShowPreview()` / `ShowDesign()` 切换模式；`GoToPage(int)` 使用从 1 开始的页码并限制在有效范围；`Zoom` 为 0.35–1.5。`SelectedElement`、`SelectedColumn` 支持双向绑定。独立只读 JSON 查看使用 [ReportPreview](report-preview.md)，可获取接口数据／图片、导出 PDF 及打印。

- 几何使用 mm，按 `96 / 25.4` 转为 DIP；模型字号和最小行高使用 DIP，列权重按表格宽度归一化。
- 表格高度包含表头和一行明细模板。纵向调整改变展开行最小高度，换行还会继续增高。
- 先格式化再测量文字，明细行完整保留在一页。预留页脚区域，末页合计放不下时增加汇总页。
- 空源生成一页，已配置合计为零。缺失字段、错误格式／几何、数字溢出、过窄列及过高单行会清空 `Layout` 并报告错误；修正后恢复。
- 源创建时固定行成员；getter 读取行对象当前值。修改行值后调用 `RefreshReport()`；替换源刷新成员并回到首页。文档／列通知自动刷新，要求在 UI 线程操作。
- `ReportPaginator.Paginate(document, source)` 返回只读文字／几何快照，测量需要初始化 Avalonia。分页同步计算，不为每行／单元格创建 Control。
- 撤销／重做保留文档实例，但替换元素和列实例；直接模型编辑也进入历史，替换文档重置历史。

当前不包含多明细表／多源、分组、交叉表或子报表。合计使用 `ShowSum`，不执行任意表达式；JSON 日期字符串暂按文本处理。编辑器标签使用中文。

## PreviewHost 与专项验证

“扩展控件 → ReportDesign”打开销售示例；“扩展控件 → ReportPreview”以已保存 JSON 和显式 96 行数据源演示独立预览，不依赖外网。两者填满宿主内容区，可按控件名搜索；离开后返回保留设计器状态。

```sh
dotnet run --project design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/reportdesign/artifacts -- --reportdesign
dotnet run --project design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/reportdesign/artifacts -- --verify-reportdesign .preview/reportdesign/screenshots
```

报表专项覆盖布局、绑定、指针编辑、响应树拖入与字段映射、JSON 往返、图片、PDF 及主题绘制。NativeAotSmoke 的 `--require-aot --verify-reportdesign` 运行相同报表专项，验证原生消费。这些检查不代表物理打印输出或 Windows／Linux 实跑已验证。

`--verify-report-authentication` 使用本地接口检查登录／数据请求顺序、可编辑请求头、失败／取消、确认、保存及独立预览。`--verify-report-lifecycle` 在明暗主题与三档密度下检查普通／card 原生页签及缓存的 ReportDesign 页面，以及四种方向的溢出、键盘与关闭操作。两者均接受可选截图目录。在 `--` 前添加 `-p:ReportOnlyPreview=true` 可排除无关 PreviewHost 示例；NativeAotSmoke 支持同一属性与命令，并可加 `--require-aot`。

`--verify-report-tree-scroll` 单独检查响应树的真实滚轮、稳定滚动范围、到底后的实际文字绘制、返回顶部、折叠／展开、搜索、单值指针拖放及请求失败恢复，覆盖明暗主题、三档宿主密度和较窄视口；同样支持上述隔离属性与原生消费命令。

`--verify-report-sidebar` 组合数据树滚动、结构节点定位及反复切换组件／数据页签的检查；可选输出目录包含两种侧栏页签在明暗主题、三档宿主密度下的截图。

`--verify-report-reload` 检查模板重开、根字段／表格最新数据、请求格式与鉴权、图片绑定、取消、迟到响应隔离、缓存导航及 MVVM 文档赋值。数据树滚动专项也覆盖反复切换后的展开、选中、滚动位置和筛选节点身份保留。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ReportDesign

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ReportDesign.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ConditionIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L21) | `int` | 可写; OneWay | `computed / state` | 条件格式编辑器的当前选项索引；与选中列模型同步。 |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L39) | `int` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L23) | `TheOne.Avalonia.Reporting.ReportDataSource?` | 可写; OneWay | `null` | 显式数据来源；由宿主提供符合目标模型契约的数据，不能用显示模板代替数据适配。 |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | 可写; OneWay | `null` | 当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。 |
| [`FontFamilyName`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L30) | `string` | 可写; OneWay | `computed / state` | 报表文本字体族名称；使用目标平台可用字体并检查回退。 |
| [`FontSizeValue`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L25) | `double` | 可写; OneWay | `computed / state` | 报表属性编辑器当前字号值；与选中元素／文档同步。 |
| [`FormatIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L31) | `int` | 可写; OneWay | `computed / state` | 报表数值格式编辑器的当前选项索引。 |
| [`ImageFitIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L39) | `int` | 可写; OneWay | `computed / state` | 报表图片适应模式编辑器的当前选项索引。 |
| [`IsPreview`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L38) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L36) | `TheOne.Avalonia.Reporting.ReportLayout?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SaveCommand`](../../../TheOne.Avalonia/Controls/ReportDesign.Commands.cs#L26) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON. |
| [`SelectedColumn`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L24) | `TheOne.Avalonia.Reporting.ReportColumn?` | 可写; TwoWay | `null` | 当前选中的报表列模型；空值表示未选择。 |
| [`SelectedElement`](../../../TheOne.Avalonia/Controls/ReportDesign.Editing.cs#L19) | `TheOne.Avalonia.Reporting.ReportElement?` | 可写; TwoWay | `null` | 当前选中的报表元素模型；空值表示未选择。 |
| [`SelectedField`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L26) | `TheOne.Avalonia.Reporting.ReportFieldInfo?` | 可写; OneWay | `computed / state` | 当前选中的可用报表字段；用于插入或编辑绑定。 |
| [`SourceFields`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L25) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportFieldInfo>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SumIndex`](../../../TheOne.Avalonia/Controls/ReportDesign.Files.cs#L40) | `int` | 可写; OneWay | `computed / state` | 报表汇总操作编辑器的当前选项索引。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L37) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportDesign.axaml.cs#L40) | `double` | 可写; CLR / n/a | `accessor-defined / state` | 视图缩放倍率，1 表示 100%；只缩放视图，不修改报表毫米尺寸或流程坐标。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ReportDesign`](../../api/zh-cn/TheOne.Avalonia.Controls.ReportDesign.md) | Class | 18 | 17 | 0 |
| [`TheOne.Avalonia.Reporting.PrintTool`](../../api/zh-cn/TheOne.Avalonia.Reporting.PrintTool.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) | Class | 7 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiParameterMode.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportApiRequest`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportApiRequest.md) | Class | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportColumn`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportColumn.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportCondition`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportCondition.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNode`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataNode.md) | Class | 9 | 0 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataNodeKind.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDataSource`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDataSource.md) | Class | 3 | 2 | 0 |
| [`TheOne.Avalonia.Reporting.ReportDocument`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportDocument.md) | Class | 29 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElement`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportElement.md) | Class | 25 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportElementKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportElementKind.md) | Enum | 8 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportField<T>`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportField_T_.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportFieldInfo`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportFieldInfo.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportImageFit`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportImageFit.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonBody`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportJsonBody.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportJsonData`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportJsonData.md) | Class | 0 | 6 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayout`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayout.md) | Class | 19 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutColumn`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutElement`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutElement.md) | Class | 12 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportLayoutRow`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportLayoutRow.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportObservable`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportObservable.md) | Class | 0 | 0 | 1 |
| [`TheOne.Avalonia.Reporting.ReportPage`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPage.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPaginator`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPaginator.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPdfExporter`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPdfExporter.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintOptions`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintOptions.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintPlatform`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintPlatform.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportPrintResult`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportPrintResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportResources`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportResources.md) | Class | 0 | 4 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTemplate`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTemplate.md) | Class | 0 | 3 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextAlignment`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTextAlignment.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportTextListStyle`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportTextListStyle.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueFormat`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportValueFormat.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Reporting.ReportValueKind`](../../api/zh-cn/TheOne.Avalonia.Reporting.ReportValueKind.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
