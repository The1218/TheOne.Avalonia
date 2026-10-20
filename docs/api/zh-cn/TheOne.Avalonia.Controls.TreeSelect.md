# TreeSelect

[English](../en/TheOne.Avalonia.Controls.TreeSelect.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelect.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelect` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A searchable tree selector with stable keys and virtualized, flattened visible rows.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClear`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L132) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CheckMode`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L35) | `TheOne.Avalonia.Controls.TreeSelectCheckMode` | 可写; OneWay | `TreeSelectCheckMode.Independent` | false / Independent；单选忽略勾选模式 校验：`value => Enum.IsDefined(value)` 可选值：`Independent`, `Cascade` |
| [`ClearText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L67) | `string` | 可写; OneWay | `"清空可编辑选择"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”；空状态及可访问操作文字 |
| [`DisplayTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L97) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L63) | `string` | 可写; OneWay | `"无匹配节点"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”；空状态及可访问操作文字 |
| [`HasHiddenSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L122) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasNoResults`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L127) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasSelection`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L117) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HiddenSelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L112) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsDropDownOpen`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L79) | `bool` | 可写; TwoWay | `false` | false / false；浏览不改变选择 / TwoWay 展开状态 |
| [`IsMultiSelect`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L31) | `bool` | 可写; OneWay | `false` | false / Independent；单选忽略勾选模式 |
| [`IsReadOnly`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L75) | `bool` | 可写; OneWay | `false` | false / false；浏览不改变选择 / TwoWay 展开状态 |
| [`ItemAdapter`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L39) | `TheOne.Avalonia.Controls.TreeSelectItemAdapter?` | 可写; OneWay | `null` | IEnumerable? 根集合 / 可选强类型模型映射 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L142) | `double` | 只读; OneWay | `computed / state` | 单项／滚轮单元的 DIP 高度；主题密度和显式局部值可能覆盖注册初值。 |
| [`ItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L43) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null；列表 / 标签模板 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L15) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | IEnumerable? 根集合 / 可选强类型模型映射 |
| [`MaxDropDownHeight`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L87) | `double` | 可写; OneWay | `320` | 320 DIP，有限且 ≥80；列表视口高度上限 校验：`value => double.IsFinite(value) && value >= 80` |
| [`MaxVisibleTags`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L83) | `int` | 可写; OneWay | `3` | 3，0–100；单选显示一项，多选超出显示 +N，0 仅计数 校验：`value => value is >= 0 and <= 100` |
| [`OpenText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L71) | `string` | 可写; OneWay | `"展开或收起树"` | “无匹配节点” / “清空可编辑选择” / “展开或收起树”；空状态及可访问操作文字 |
| [`PlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L55) | `string` | 可写; OneWay | `"请选择"` | “请选择” / “搜索节点” |
| [`SearchPlaceholderText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L59) | `string` | 可写; OneWay | `"搜索节点"` | “请选择” / “搜索节点” |
| [`SearchText`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L51) | `string` | 可写; TwoWay | `""` | 空字符串，TwoWay；忽略大小写搜索，保留匹配节点的祖先 |
| [`SelectedCount`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L107) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedItem`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L27) | `object?` | 可写; TwoWay | `null` | object?，TwoWay；单项 Key / 原始对象 |
| [`SelectedItemTemplate`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L47) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null；列表 / 标签模板 |
| [`SelectedItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L102) | `System.Collections.Generic.IReadOnlyList<object>` | 只读; OneWay | `computed / state` | 多选模型集合；元素必须来自源集合，变更与清空行为遵循控件的禁选／只读契约。 |
| [`SelectedKey`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L23) | `object?` | 可写; TwoWay | `null` | object?，TwoWay；单项 Key / 原始对象 |
| [`SelectedKeys`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L19) | `System.Collections.IList?` | 可写; TwoWay | `null` | IList?，TwoWay；默认独立可观察集合 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L137) | `string?` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`VisibleItems`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L92) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.TreeSelectItem>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool TreeSelect.OpenDropDown()`](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L105) | 打开多选弹层并聚焦搜索；控件必须已挂载且可用，返回是否打开。 |
| [`void TreeSelect.CloseDropDown()`](../../../TheOne.Avalonia/Controls/TreeSelect.Interaction.cs#L111) | 关闭多选弹层，按交互上下文将焦点返回输入区。 |
| [`void TreeSelect.RefreshItems()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L104) | Re-read model structure and mapping; search alone never rebuilds subscriptions. |
| [`bool TreeSelect.ToggleItem(object item)`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L281) | Toggle a model's selection. Cascade affects enabled leaves only; disabled selections remain locked. |
| [`bool TreeSelect.ClearSelection()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L295) | Clear editable selections; selected disabled nodes remain. |
| [`bool TreeSelect.SetExpanded(object key, bool expanded)`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L310) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`TreeSelect.TreeSelect()`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L34) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<SelectionChangedEventArgs>? TreeSelect.SelectionChanged`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L19) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> TreeSelect.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IList?> TreeSelect.SelectedKeysProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TreeSelect.SelectedKeyProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> TreeSelect.SelectedItemProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> TreeSelect.IsMultiSelectProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TreeSelectCheckMode> TreeSelect.CheckModeProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<TreeSelectItemAdapter?> TreeSelect.ItemAdapterProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> TreeSelect.ItemTemplateProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> TreeSelect.SelectedItemTemplateProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L45) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.SearchTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L49) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.PlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L53) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.SearchPlaceholderTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L57) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L61) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.ClearTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L65) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> TreeSelect.OpenTextProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L69) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> TreeSelect.IsReadOnlyProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L73) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> TreeSelect.IsDropDownOpenProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L77) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> TreeSelect.MaxVisibleTagsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L81) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> TreeSelect.MaxDropDownHeightProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L85) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> TreeSelect.VisibleItemsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L89) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> TreeSelect.DisplayTagsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L94) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, IReadOnlyList<object>> TreeSelect.SelectedItemsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L99) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, int> TreeSelect.SelectedCountProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L104) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, int> TreeSelect.HiddenSelectedCountProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L109) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasSelectionProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L114) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasHiddenSelectionProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L119) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.HasNoResultsProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L124) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, bool> TreeSelect.CanClearProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L129) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, string?> TreeSelect.ValidationErrorProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L134) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<TreeSelect, double> TreeSelect.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/TreeSelect.Properties.cs#L139) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<SelectionChangedEventArgs> TreeSelect.SelectionChangedEvent`](../../../TheOne.Avalonia/Controls/TreeSelect.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.TreeSelectCheckMode`](TheOne.Avalonia.Controls.TreeSelectCheckMode.md)
- [`TheOne.Avalonia.Controls.TreeSelectItem`](TheOne.Avalonia.Controls.TreeSelectItem.md)
- [`TheOne.Avalonia.Controls.TreeSelectItemAdapter`](TheOne.Avalonia.Controls.TreeSelectItemAdapter.md)
