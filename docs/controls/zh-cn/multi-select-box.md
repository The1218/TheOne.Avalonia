# MultiSelectBox 多选框

`MultiSelectBox` 提供本地搜索、多选标签、清空和原生虚拟化结果列表。选择绑定模型对象，搜索只改变结果投影；隐藏的已选项不会因搜索而丢失。加载 `OneTheme` 后即可使用。

## 编译绑定接入

```csharp
public ObservableCollection<Choice> Choices { get; } = [];
public ObservableCollection<Choice> Selected { get; } = [];
public MultiSelectItemAdapter Adapter { get; } =
    MultiSelectItemAdapter.Create<Choice>(item => item.Name, item => item.Available);
```

```xml
<one:MultiSelectBox ItemsSource="{Binding Choices}"
                    SelectedItems="{Binding Selected, Mode=TwoWay}"
                    ItemAdapter="{Binding Adapter}"
                    PlaceholderText="选择适用项"
                    AutomationProperties.Name="适用项" />
```

在视图声明 `xmlns:one="using:TheOne.Avalonia.Controls"` 与具体 `x:DataType`，保持编译绑定。适配器使用显式泛型 getter，供搜索、标签和禁选判断共同使用；不会反射解析属性路径，也不需要额外元数据注册。没有适配器时使用 `ToString()`，所有项均可选。

## 选择与禁选语义

- `SelectedItems` 绑定非泛型 `IList` 契约，`ObservableCollection<T>` 可直接使用。控件修改同一集合，不用内部集合替换宿主绑定；替换集合及集合通知也会同步。
- 每个控件默认创建独立的可变选择集合。显式绑定 `null`、只读或定长集合时仅展示选择，不尝试修改。`IsReadOnly=True` 同样阻止选择、移除与清空，但仍可搜索和浏览。
- 禁选项不能被用户选中或移除。外部预先选中的禁选项仍显示为锁定标签；`ClearSelection` 只移除可编辑项，保留锁定值。
- 过滤与空搜索结果都保留已有选择。结果区 Ctrl / ⌘ + A 只添加当前筛选结果中已加载且可选的项，不移除隐藏选择；搜索输入框中的该快捷键仍是文本全选。
- 模型按 `Equals` / `GetHashCode` 区分，相等重复项合并展示、选择；`null` 项忽略。模型的相等与哈希语义在使用期间应保持稳定。
- 数据源删除某项时，不主动改写外部选择集合；该项仍可显示为标签并按可编辑状态移除。清除重复的已选值会移除集合中所有相等值。
- 本版是已加载模型上的同步本地筛选，按当前区域设置忽略大小写并去掉查询首尾空白；不包含远程查询、服务分页或数据虚拟化。

## 公共属性与方法

| API | 行为 |
| --- | --- |
| `ItemsSource` | `IEnumerable?`，选项模型；不传入预创建控件 |
| `SelectedItems` | `IList?`，默认双向绑定；用户操作与外部集合变更同步 |
| `ItemAdapter` | 显式文本与可选状态 getter；文本同时用于搜索和默认标签 |
| `ItemTemplate` / `SelectedItemTemplate` | 分别呈现结果和标签中的原始模型；模板要声明对应 `x:DataType` |
| `SearchText` | 默认空字符串，支持双向绑定；关闭弹层保留查询 |
| `PlaceholderText` / `SearchPlaceholderText` / `EmptyText` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| `ClearText` / `OpenText` / `RemoveTextFormat` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| `MaxVisibleTags` | 默认 `3`，范围 `0–100`；只为前 N 个选择建立标签，其余显示数量 |
| `MaxDropDownHeight` | 默认 `320`，必须为至少 80 的有限数；弹层同时约束到宿主可用范围 |
| `IsReadOnly` / `IsDropDownOpen` | 选择只读及可双向绑定的弹层状态；禁用或卸载会关闭弹层 |
| `SelectedCount` / `HasSelection` | 只读，去重后选择数量与是否非空 |
| `HiddenSelectedCount` / `HasHiddenSelection` | 只读，未建立标签的选择数量与是否有溢出 |
| `VisibleItems` / `DisplayTags` | 只读模板数据；`MultiSelectBoxItem.Item` 是原始模型，其他属性是文字、可选／选中状态和模板内容 |
| `HasNoResults` / `CanClear` / `ItemHeight` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| `ToggleItem(object)` | 切换可编辑项；新增选择要求项在数据源中，移除允许已选的外部值；未改变返回 `false` |
| `ClearSelection()` | 批量移除可编辑选择，保留禁选值；未改变返回 `false` |
| `RefreshItems()` | 为普通集合、无通知模型或改变了外部状态的 getter 手动刷新 |
| `OpenDropDown()` / `CloseDropDown()` | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| `SelectionChanged` | 冒泡事件，AddedItems / RemovedItems 提供原始模型，批量用户操作合并为一次通知 |

提供 `INotifyCollectionChanged` 和模型 `INotifyPropertyChanged` 可自动同步，通知应在 UI 线程发出；卸载解除订阅，重新挂载读取最新数据。无通知的变更调用 `RefreshItems()`。自定义模板只改变显示内容，搜索与可选判断仍由适配器决定。

## 键盘与焦点

| 位置与按键 | 行为 |
| --- | --- |
| 字段聚焦，Enter / Space / Down | 展开并聚焦搜索框 |
| 字段聚焦，F4 / Alt + Down | 展开或收起 |
| 搜索框，Down / Up | 移动到可用结果，跳过禁选项 |
| 结果区，Up / Down / Home / End | 移动焦点，不改变已有多选 |
| 搜索框，Enter | 切换活动结果；尚无活动结果时使用首个可用项 |
| 结果区，Enter / Space | 切换当前聚焦项，保留其他选择 |
| 结果区，Ctrl / ⌘ + A | 添加当前筛选范围内的可选项 |
| 弹层中，Esc | 关闭并回焦字段 |
| 弹层中，Tab / Shift + Tab | 关闭并按原生 Tab 顺序导航 |
| 字段或空搜索框，Backspace | 移除最后一个可编辑选择，跳过锁定值 |

标签移除和清空按钮保留原生键盘与焦点支持。组合输入期间由原生文本框处理按键。点击外部关闭弹层时不会夺回新焦点。字段提供 ComboBox 自动化角色和展开／收起能力；原生列表保留行选择自动化。读屏器的目标平台表现需单独实测。

## 主题与规模

颜色复用 `One.*` 资源：输入边框使用可见的 `One.InputLine`，已选标签使用 `One.AccentSoft` 背景和 `One.Text` 文字。Small / Normal / Large 单行输入和默认结果项高度为 30 / 36 / 44；标签换行或自定义内容可增加实际高度。结果项之间保留 4px 间隔。弹层显式同步打开期间的明暗主题与密度，宽度不超过宿主客户区。

结果使用有界原生 `ListBox` 和 `VirtualizingStackPanel`；只为可见行建立 UI。标签按 `MaxVisibleTags` 建立有限数量的控件。数据模型、筛选索引及通知订阅仍对应全部已加载数据，UI 虚拟化不减少业务数据量。不要用外层无限 `ScrollViewer` 替换结果模板的有限视口。

## 预览与专项验证

PreviewHost：**扩展控件 → MultiSelectBox**，包含一万条模型、预选禁选项及编译绑定。

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/multiselect/artifacts
dotnet .preview/multiselect/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-multiselect .preview/multiselect/screenshots
```

专项只覆盖 MultiSelectBox：真实按键与指针、搜索、多选与锁定标签、清空、集合替换、只读／禁用、模板和挂载生命周期、空结果、一万条远距离容器回收、批量已选标签上限，以及明暗 × 三档尺寸 × 宽窄窗口的布局和实际像素。

`NativeAotSmoke` 复用同一专项；发布后执行原生程序的 `--require-aot --verify-multiselect`，验证 getter、编译模板和绑定。托管验证或跨 RID 构建不代表其他平台的原生运行通过。
