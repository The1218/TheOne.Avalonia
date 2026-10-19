# MultiSelectBox 多选框

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

弹层使用独立的有限视口：内容超出时显示原生纵向滚动条；短列表没有滚动条是正常行为。鼠标滚轮、触摸板细粒度纵向／横向／斜向增量，以及滚动手势的惯性和结束阶段，在结果列表、搜索框、空结果及列表边界均停留在弹层内，不带动 Home 等外层页面。保留内部原生滚动；关闭后恢复普通页面滚动，旧弹层产生的尾帧仍被隔离。

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

搜索复用已缓存的条目文字，只重新计算结果投影，不重新枚举 `ItemsSource`／`SelectedItems`、执行适配器 getter 或重建模型订阅；搜索仍是已加载数据上的同步线性筛选。模型通知只更新对应条目的文字与可选状态，集合变更按当前数据维护条目并差量更新模型订阅。若 getter 依赖未发出通知的外部状态，调用 `RefreshItems()` 显式刷新，不能依赖输入搜索文字来刷新模型。

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

专项只覆盖 MultiSelectBox：真实按键与指针、搜索、多选与锁定标签、清空、集合替换、只读／禁用、模板和挂载生命周期、空结果、一万条远距离容器回收、批量已选标签上限，以及明暗 × 三档尺寸 × 宽窄窗口的布局和实际像素。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### MultiSelectBox

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L76) | `bool` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`ClearText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L62) | `string` | 可写; OneWay | `"清空可编辑选择"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L70) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | 只读; OneWay | `computed / state` | 只读模板数据；`MultiSelectBoxItem.Item` 是原始模型，其他属性是文字、可选／选中状态和模板内容 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L61) | `string` | 可写; OneWay | `"无匹配选项"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L74) | `bool` | 只读; OneWay | `computed / state` | 只读，未建立标签的选择数量与是否有溢出 |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L75) | `bool` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L73) | `bool` | 只读; OneWay | `computed / state` | 只读，去重后选择数量与是否非空 |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L72) | `int` | 只读; OneWay | `computed / state` | 只读，未建立标签的选择数量与是否有溢出 |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L66) | `bool` | 可写; TwoWay | `false` | 选择只读及可双向绑定的弹层状态；禁用或卸载会关闭弹层 |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L65) | `bool` | 可写; OneWay | `false` | 选择只读及可双向绑定的弹层状态；禁用或卸载会关闭弹层 |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L55) | `TheOne.Avalonia.Controls.MultiSelectItemAdapter?` | 可写; OneWay | `null` | 显式文本与可选状态 getter；文本同时用于搜索和默认标签 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L77) | `double` | 只读; OneWay | `computed / state` | 只读，无结果、存在可清空项和当前密度的最小输入高度 |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L56) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 分别呈现结果和标签中的原始模型；模板要声明对应 `x:DataType` |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L53) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | `IEnumerable?`，选项模型；不传入预创建控件 |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L68) | `double` | 可写; OneWay | `320` | 默认 `320`，必须为至少 80 的有限数；弹层同时约束到宿主可用范围 校验：`value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L67) | `int` | 可写; OneWay | `3` | 默认 `3`，范围 `0–100`；只为前 N 个选择建立标签，其余显示数量 校验：`value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L63) | `string` | 可写; OneWay | `"展开或收起选项"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L59) | `string` | 可写; OneWay | `"请选择"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`RemoveTextFormat`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L64) | `string` | 可写; OneWay | `"移除 {0}"` | 原生操作按钮的可访问名称；移除格式默认 `移除 {0}`，参数为模型文本 |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L60) | `string` | 可写; OneWay | `"搜索选项"` | 未选择、搜索输入与无匹配提示，可覆盖为宿主语言 |
| [`SearchText`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L58) | `string` | 可写; TwoWay | `""` | 默认空字符串，支持双向绑定；关闭弹层保留查询 |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L71) | `int` | 只读; OneWay | `computed / state` | 只读，去重后选择数量与是否非空 |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 分别呈现结果和标签中的原始模型；模板要声明对应 `x:DataType` |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L54) | `System.Collections.IList?` | 可写; TwoWay | `null` | `IList?`，默认双向绑定；用户操作与外部集合变更同步 |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L69) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.MultiSelectBoxItem>` | 只读; OneWay | `computed / state` | 只读模板数据；`MultiSelectBoxItem.Item` 是原始模型，其他属性是文字、可选／选中状态和模板内容 |

### MultiSelectBoxItem

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L33) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L36) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`DisplayContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L35) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L31) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L32) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 当前项的选择状态；与控件选中模型保持一致，不与无关项共享状态。 |
| [`Item`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L29) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`RemoveLabel`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L34) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagContent`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L37) | `object` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TagTemplate`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L38) | `Avalonia.Controls.Templates.IDataTemplate?` | 只读; CLR / n/a | `accessor-defined / state` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`Text`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L30) | `string` | 只读; CLR / n/a | `""` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |

### MultiSelectItemAdapter

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.OpenDropDown()` | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.CloseDropDown()` | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `void MultiSelectBox.RefreshItems()` | Refreshes plain collections and models that do not provide change notifications. |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ToggleItem(object item)` | 切换可编辑项；新增选择要求项在数据源中，移除允许已选的外部值；未改变返回 `false` |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `bool MultiSelectBox.ClearSelection()` | Removes editable selections, retaining selected disabled items. |
| [`MultiSelectItemAdapter`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `abstract string MultiSelectItemAdapter.GetText(object item)` | 使用已配置的类型化适配器读取原始模型显示／搜索文本。 |
| [`MultiSelectItemAdapter`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) | `virtual bool MultiSelectItemAdapter.IsEnabled(object item)` | 使用适配器判断原始模型能否参与用户选择。 |

### 事件订阅

按需订阅下列通知，并在宿主释放时解除订阅；这里只列事件入口，不列路由事件标识字段。

| 类型 | 事件 | 用途 |
| --- | --- | --- |
| [`MultiSelectBox`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) | `SelectionChanged` | 冒泡事件，AddedItems / RemovedItems 提供原始模型，批量用户操作合并为一次通知 |
| [`MultiSelectBoxItem`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | `PropertyChanged` | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](../../api/zh-cn/TheOne.Avalonia.Controls.MultiSelectBoxItem.md) | 10 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
