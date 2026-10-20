# Pagination

[English](../en/TheOne.Avalonia.Controls.Pagination.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Pagination.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Pagination` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

One-based pagination with bounded page links and native navigation / input controls.

[Source / 声明](../../../TheOne.Avalonia/Controls/Pagination.cs#L26)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/Pagination.cs#L322) | `bool` | 只读; OneWay | `computed / state` | 只读，前后导航是否有目标页 |
| [`CanGoPrevious`](../../../TheOne.Avalonia/Controls/Pagination.cs#L316) | `bool` | 只读; OneWay | `computed / state` | 只读，前后导航是否有目标页 |
| [`CompactBreakpoint`](../../../TheOne.Avalonia/Controls/Pagination.cs#L196) | `double` | 可写; OneWay | `640` | 640；内容区在此宽度以下，或实际数字导航无法完整放下时自动紧凑；0 关闭自动切换 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/Pagination.cs#L172) | `long` | 可写; TwoWay | `1` | 从 1 开始的 long 页码，默认 1，默认双向绑定 校验：`value => value >= 1` |
| [`CurrentPageText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L370) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L286) | `string` | 可写; OneWay | `"暂无记录"` | 没有可显示数据时的提示文本；由应用覆盖以进行本地化。 |
| [`FirstItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L304) | `long` | 只读; OneWay | `computed / state` | 只读，当前页记录范围，从 1 开始；空数据均为 0 |
| [`HasJumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L346) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasPages`](../../../TheOne.Avalonia/Controls/Pagination.cs#L328) | `bool` | 只读; OneWay | `computed / state` | 只读，是否存在记录页 |
| [`HasSummary`](../../../TheOne.Avalonia/Controls/Pagination.cs#L340) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/Pagination.cs#L334) | `bool` | 只读; OneWay | `computed / state` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/Pagination.cs#L208) | `double` | 可写; OneWay | `36` | Small / Normal / Large 为 30 / 36 / 44，导航与输入的最小高度 校验：`value => double.IsFinite(value) && value > 0` |
| [`JumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L352) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`JumpPrefixText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L274) | `string` | 可写; OneWay | `"跳至"` | 页码跳转框前的本地化提示。 |
| [`JumpText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L268) | `string` | 可写; OneWay | `"跳转"` | 执行页码跳转的按钮文本。 |
| [`LastItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L310) | `long` | 只读; OneWay | `computed / state` | 只读，当前页记录范围，从 1 开始；空数据均为 0 |
| [`Layout`](../../../TheOne.Avalonia/Controls/Pagination.cs#L190) | `TheOne.Avalonia.Controls.PaginationLayout` | 可写; OneWay | `0 / default` | Standard / Compact，默认 Standard 校验：`Enum.IsDefined` 可选值：`Standard`, `Compact` |
| [`NextText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L262) | `string` | 可写; OneWay | `"下一页"` | 下一页按钮的可访问／显示文本。 |
| [`PageChangedCommand`](../../../TheOne.Avalonia/Controls/Pagination.cs#L292) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`PageCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L298) | `long` | 只读; OneWay | `computed / state` | 只读，总页数；空数据为 0 |
| [`PageItems`](../../../TheOne.Avalonia/Controls/Pagination.cs#L376) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageItem>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PageSize`](../../../TheOne.Avalonia/Controls/Pagination.cs#L178) | `int` | 可写; TwoWay | `20` | 正整数，每页条数，默认 20，默认双向绑定 校验：`value => value > 0` |
| [`PageSizeOptions`](../../../TheOne.Avalonia/Controls/Pagination.cs#L184) | `System.Collections.Generic.IEnumerable<int>?` | 可写; OneWay | `DefaultSizes` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`PageSizeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L250) | `string` | 可写; OneWay | `"{0} 条 / 页"` | 每页条数选择的格式文本；占位参数与分页文档一致。 |
| [`PageSizes`](../../../TheOne.Avalonia/Controls/Pagination.cs#L382) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageSize>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PageUnitText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L280) | `string` | 可写; OneWay | `"页"` | 页码后面的本地化单位文本。 |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L256) | `string` | 可写; OneWay | `"上一页"` | 上一页按钮的可访问／显示文本。 |
| [`RangeText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L364) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`RangeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L244) | `string` | 可写; OneWay | `"第 {0:N0}–{1:N0} 条，共 {2:N0} 条"` | 当前数据范围的格式文本；不更改分页计算。 |
| [`ShowJump`](../../../TheOne.Avalonia/Controls/Pagination.cs#L232) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowPageSizeSelector`](../../../TheOne.Avalonia/Controls/Pagination.cs#L226) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowRange`](../../../TheOne.Avalonia/Controls/Pagination.cs#L220) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowTotal`](../../../TheOne.Avalonia/Controls/Pagination.cs#L214) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`SiblingCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L202) | `int` | 可写; OneWay | `2` | 2，当前页两侧邻居数量，允许 0–5 校验：`value => value is >= 0 and <= 5` |
| [`TotalCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L166) | `long` | 可写; OneWay | `0 / default` | 非负 long，总记录数，默认 0 校验：`value => value >= 0` |
| [`TotalText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L358) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TotalTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L238) | `string` | 可写; OneWay | `"共 {0:N0} 条"` | 总条数的格式文本；仅负责展示。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool Pagination.GoToPage(long page)`](../../../TheOne.Avalonia/Controls/Pagination.cs#L603) | 切换到指定页码；分页器与报表的页码／越界处理分别见其契约。 |
| [`bool Pagination.GoPrevious()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L611) | 切换到上一页，边界时不产生无效页号。 |
| [`bool Pagination.GoNext()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L616) | 切换到下一页，边界时不产生无效页号。 |
| [`bool Pagination.GoFirst()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L621) | 切换到第一页。 |
| [`bool Pagination.GoLast()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L626) | 切换到最后一页。 |
| [`Pagination.Pagination()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L160) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<PaginationPageChangedEventArgs>? Pagination.PageChanged`](../../../TheOne.Avalonia/Controls/Pagination.cs#L390) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<long> Pagination.TotalCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<long> Pagination.CurrentPageProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> Pagination.PageSizeProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IEnumerable<int>?> Pagination.PageSizeOptionsProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<PaginationLayout> Pagination.LayoutProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Pagination.CompactBreakpointProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> Pagination.SiblingCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L51) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Pagination.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L54) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Pagination.ShowTotalProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L58) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Pagination.ShowRangeProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L61) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Pagination.ShowPageSizeSelectorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L64) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Pagination.ShowJumpProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L67) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.TotalTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L70) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.RangeTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L73) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.PageSizeTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L76) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.PreviousTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L79) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.NextTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L82) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.JumpTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L85) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.JumpPrefixTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L88) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.PageUnitTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L91) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Pagination.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L94) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> Pagination.PageChangedCommandProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L97) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, long> Pagination.PageCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L100) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, long> Pagination.FirstItemProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L103) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, long> Pagination.LastItemProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L106) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.CanGoPreviousProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L109) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.CanGoNextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L112) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasPagesProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L115) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.IsCompactProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L118) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasSummaryProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L121) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasJumpErrorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L124) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, string> Pagination.JumpErrorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L127) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, string> Pagination.TotalTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L130) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, string> Pagination.RangeTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L133) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, string> Pagination.CurrentPageTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L136) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageItem>> Pagination.PageItemsProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L140) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageSize>> Pagination.PageSizesProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L144) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<PaginationPageChangedEventArgs> Pagination.PageChangedEvent`](../../../TheOne.Avalonia/Controls/Pagination.cs#L148) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.PaginationLayout`](TheOne.Avalonia.Controls.PaginationLayout.md)
- [`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs`](TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md)
- [`TheOne.Avalonia.Controls.PaginationPageItem`](TheOne.Avalonia.Controls.PaginationPageItem.md)
- [`TheOne.Avalonia.Controls.PaginationPageSize`](TheOne.Avalonia.Controls.PaginationPageSize.md)
