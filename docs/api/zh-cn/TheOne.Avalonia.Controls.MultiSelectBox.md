# MultiSelectBox

[English](../en/TheOne.Avalonia.Controls.MultiSelectBox.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectBox.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectBox` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A searchable selection over model items, with bounded tags and native virtualized results.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L15)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

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

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool MultiSelectBox.OpenDropDown()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L74) | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`void MultiSelectBox.CloseDropDown()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.Interaction.cs#L80) | 打开并聚焦搜索，或关闭并回焦字段；打开要求已挂载且可用 |
| [`void MultiSelectBox.RefreshItems()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L111) | Refreshes plain collections and models that do not provide change notifications. |
| [`bool MultiSelectBox.ToggleItem(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L185) | 切换可编辑项；新增选择要求项在数据源中，移除允许已选的外部值；未改变返回 `false` |
| [`bool MultiSelectBox.ClearSelection()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L195) | Removes editable selections, retaining selected disabled items. |
| [`MultiSelectBox.MultiSelectBox()`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L81) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<SelectionChangedEventArgs>? MultiSelectBox.SelectionChanged`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L79) | 冒泡事件，AddedItems / RemovedItems 提供原始模型，批量用户操作合并为一次通知 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> MultiSelectBox.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList?> MultiSelectBox.SelectedItemsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<MultiSelectItemAdapter?> MultiSelectBox.ItemAdapterProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> MultiSelectBox.ItemTemplateProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> MultiSelectBox.SelectedItemTemplateProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.SearchTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.PlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.SearchPlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.ClearTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.OpenTextProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> MultiSelectBox.RemoveTextFormatProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> MultiSelectBox.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> MultiSelectBox.IsDropDownOpenProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> MultiSelectBox.MaxVisibleTagsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> MultiSelectBox.MaxDropDownHeightProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> MultiSelectBox.VisibleItemsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, IReadOnlyList<MultiSelectBoxItem>> MultiSelectBox.DisplayTagsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, int> MultiSelectBox.SelectedCountProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, int> MultiSelectBox.HiddenSelectedCountProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasSelectionProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasHiddenSelectionProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.HasNoResultsProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, bool> MultiSelectBox.CanClearProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L42) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<MultiSelectBox, double> MultiSelectBox.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L43) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<SelectionChangedEventArgs> MultiSelectBox.SelectionChangedEvent`](../../../TheOne.Avalonia/Controls/MultiSelectBox.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.MultiSelectBoxItem`](TheOne.Avalonia.Controls.MultiSelectBoxItem.md)
- [`TheOne.Avalonia.Controls.MultiSelectItemAdapter`](TheOne.Avalonia.Controls.MultiSelectItemAdapter.md)
