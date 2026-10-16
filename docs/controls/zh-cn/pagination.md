# Pagination 分页控件

**简体中文** · [English](../en/pagination.md)

`Pagination` 使用原生 Button、ComboBox、TextBox 展示分页导航。加载 `OneTheme` 后支持 Standard／Compact、Light／Dark 和 Small／Normal／Large。控件负责页码与范围，不加载或筛选业务数据；通过双向绑定、PageChanged 事件或 PageChangedCommand 请求数据。

```xml
<!-- 根视图声明 x:DataType="vm:YourViewModel"，one 为 using:TheOne.Avalonia.Controls。 -->
<one:Pagination TotalCount="{Binding TotalCount}"
                PageSize="{Binding PageSize, Mode=TwoWay}"
                CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                PageChangedCommand="{Binding LoadPageCommand}" />
```

## 页码与边界

| 属性 | 契约 |
| --- | --- |
| TotalCount | 非负 long，总记录数，默认 0 |
| PageSize | 正整数，每页条数，默认 20，默认双向绑定 |
| CurrentPage | 从 1 开始的 long 页码，默认 1，默认双向绑定 |
| PageCount | 只读，总页数；空数据为 0 |
| FirstItem / LastItem | 只读，当前页记录范围，从 1 开始；空数据均为 0 |
| CanGoPrevious / CanGoNext | 只读，前后导航是否有目标页 |
| HasPages | 只读，是否存在记录页 |

设置 PageSize 时回到第 1 页；TotalCount 减少时修正越界 CurrentPage。总数恢复增长后不会恢复旧的越界页码。初始化建议先提供 TotalCount、PageSize，再设置 CurrentPage。空数据的 CurrentPage 保持 1 作为业务请求起点，UI 显示 `0 / 0`，没有数字页码，前后导航与跳页禁用；默认范围文本为“暂无记录”。

`GoToPage(long)`、`GoPrevious()`、`GoNext()`、`GoFirst()`、`GoLast()` 返回是否实际改变页码。无效、重复、空数据或整体禁用时返回 false。直接设置 CurrentPage 必须是正数；越界值会修正到末页。UI 属性、集合变化与导航方法在 UI 线程执行。

## 布局与页码省略

Standard 包含总条数、每页条数选择、数字导航、跳页和记录范围。Compact 仅保留上一页、`当前页 / 总页数`、下一页，以及下方的总条数／每页条数。窄宽度默认自动采用 Compact；设置 `Layout="Compact"` 可固定使用紧凑布局。

| 属性 | 默认值与用途 |
| --- | --- |
| Layout | Standard / Compact，默认 Standard |
| CompactBreakpoint | 640；内容区在此宽度以下，或实际数字导航无法完整放下时自动紧凑；0 关闭自动切换 |
| SiblingCount | 2，当前页两侧邻居数量，允许 0–5 |
| ShowTotal / ShowRange / ShowPageSizeSelector / ShowJump | 默认均为 true；Compact 始终隐藏范围与跳页 |
| ItemHeight | Small / Normal / Large 为 30 / 36 / 44，导航与输入的最小高度 |

数字导航保留首页、末页和当前页邻居，省略部分用不可点击的 `…` 表示；只有一个缺失页时直接展示该页。以第 8 页、64 页为例：`1 … 6 7 8 9 10 … 64`。条目数量取决于 SiblingCount，与总页数无关；支持 Int64 总数而不按总页数创建控件。横向可用空间不足时标准信息组换行，紧凑信息组上下排列。Padding 保留控件内边距。

## 每页条数与跳页

PageSizeOptions 接受 `IEnumerable<int>`，默认 10、20、50、100。每项必须为正数；重复项去重。如果当前 PageSize 不在候选值中，选择器会补入当前值。ObservableCollection 的增删会更新候选项；脱离视觉树后解除集合订阅，重新附加时刷新。

跳页输入通过 Enter 或“跳转”提交，不在输入中途改变页码。无效文本或超过 PageCount 时保留原页码并显示错误；重新编辑清除错误，Escape 恢复当前页码。按钮、下拉框和输入框保留原生焦点、键盘、禁用与主题反馈；省略号不是焦点目标。

## 通知与加载数据

PageChanged 在页码或 PageSize 实际变化并完成归一化后触发一次，包含 OldPage、CurrentPage、OldPageSize、PageSize。修改每页条数产生一份同时包含新页码与新条数的通知；总数变化未改变页码时不触发通知。初始化、直接赋值和边界修正也可能触发通知。

PageChangedCommand 接收同一个 `PaginationPageChangedEventArgs`，执行前检查 CanExecute。它是变化通知，不阻止导航；加载期间需要禁用操作时，绑定 Pagination.IsEnabled。业务负责请求取消、错误处理和异步结果顺序。

```csharp
if (parameter is PaginationPageChangedEventArgs request)
{
    // 使用 request.CurrentPage 和 request.PageSize 请求业务数据。
}
```

## 文本、主题与模板

可以设置 TotalTextFormat、RangeTextFormat、PageSizeTextFormat，以及 PreviousText、NextText、JumpText、JumpPrefixText、PageUnitText、EmptyText。格式使用当前文化，格式串分别接收总条数、起止记录与总条数、每页条数。页码和紧凑计数遵循当前文化；显式替换文本可用于本地化。

默认主题复用 One.Surface、One.Text、One.Muted、One.Line、One.Accent、One.OnAccent 和原生输入状态资源。保留 PART_PreviousButton、PART_NextButton、PART_Pages、PART_PageSizes、PART_JumpInput、PART_JumpButton 以保留对应交互。辅助主题键为 One.PaginationButton、One.PaginationPages。显式控件主题可以重排外观；缺少某个部件仅使对应 UI 操作不可用。

## 示例与验证

PreviewHost 的“扩展控件 → Pagination”展示标准／紧凑共享绑定、首页／末页、空数据与整体禁用，并支持恢复和缩减记录数。

```sh
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/pagination/artifacts
dotnet .preview/pagination/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-pagination .preview/pagination/render
dotnet .preview/pagination/artifacts/bin/PreviewHost/release/PreviewHost.dll --render-controls .preview/pagination/render Pagination
```

专项使用真实指针／键盘输入和 Skia 绘制像素，检查明暗主题、三档尺寸、宽窄布局、边界、跳页、原子通知、焦点、模板重新应用、编译绑定和 Int64 极值。独立 NativeAotSmoke 支持 `--verify-pagination --require-aot`，需要发布并执行真正的原生程序。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### Pagination

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Pagination.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/Pagination.cs#L103) | `bool` | 只读; OneWay | `computed / state` | 只读，前后导航是否有目标页 |
| [`CanGoPrevious`](../../../TheOne.Avalonia/Controls/Pagination.cs#L102) | `bool` | 只读; OneWay | `computed / state` | 只读，前后导航是否有目标页 |
| [`CompactBreakpoint`](../../../TheOne.Avalonia/Controls/Pagination.cs#L82) | `double` | 可写; OneWay | `640` | 640；内容区在此宽度以下，或实际数字导航无法完整放下时自动紧凑；0 关闭自动切换 校验：`value => double.IsFinite(value) && value >= 0` |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/Pagination.cs#L78) | `long` | 可写; TwoWay | `1` | 从 1 开始的 long 页码，默认 1，默认双向绑定 校验：`value => value >= 1` |
| [`CurrentPageText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L111) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L97) | `string` | 可写; OneWay | `"暂无记录"` | 没有可显示数据时的提示文本；由应用覆盖以进行本地化。 |
| [`FirstItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L100) | `long` | 只读; OneWay | `computed / state` | 只读，当前页记录范围，从 1 开始；空数据均为 0 |
| [`HasJumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L107) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasPages`](../../../TheOne.Avalonia/Controls/Pagination.cs#L104) | `bool` | 只读; OneWay | `computed / state` | 只读，是否存在记录页 |
| [`HasSummary`](../../../TheOne.Avalonia/Controls/Pagination.cs#L106) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/Pagination.cs#L105) | `bool` | 只读; OneWay | `computed / state` | 启用紧凑布局；具体显示字段由所属模板决定，不改变业务数据。 |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/Pagination.cs#L84) | `double` | 可写; OneWay | `36` | Small / Normal / Large 为 30 / 36 / 44，导航与输入的最小高度 校验：`value => double.IsFinite(value) && value > 0` |
| [`JumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L108) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`JumpPrefixText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L95) | `string` | 可写; OneWay | `"跳至"` | 页码跳转框前的本地化提示。 |
| [`JumpText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L94) | `string` | 可写; OneWay | `"跳转"` | 执行页码跳转的按钮文本。 |
| [`LastItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L101) | `long` | 只读; OneWay | `computed / state` | 只读，当前页记录范围，从 1 开始；空数据均为 0 |
| [`Layout`](../../../TheOne.Avalonia/Controls/Pagination.cs#L81) | `TheOne.Avalonia.Controls.PaginationLayout` | 可写; OneWay | `0 / default` | Standard / Compact，默认 Standard 校验：`Enum.IsDefined` 可选值：`Standard`, `Compact` |
| [`NextText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L93) | `string` | 可写; OneWay | `"下一页"` | 下一页按钮的可访问／显示文本。 |
| [`PageChangedCommand`](../../../TheOne.Avalonia/Controls/Pagination.cs#L98) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`PageCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L99) | `long` | 只读; OneWay | `computed / state` | 只读，总页数；空数据为 0 |
| [`PageItems`](../../../TheOne.Avalonia/Controls/Pagination.cs#L112) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageItem>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PageSize`](../../../TheOne.Avalonia/Controls/Pagination.cs#L79) | `int` | 可写; TwoWay | `20` | 正整数，每页条数，默认 20，默认双向绑定 校验：`value => value > 0` |
| [`PageSizeOptions`](../../../TheOne.Avalonia/Controls/Pagination.cs#L80) | `System.Collections.Generic.IEnumerable<int>?` | 可写; OneWay | `DefaultSizes` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`PageSizeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L91) | `string` | 可写; OneWay | `"{0} 条 / 页"` | 每页条数选择的格式文本；占位参数与分页文档一致。 |
| [`PageSizes`](../../../TheOne.Avalonia/Controls/Pagination.cs#L113) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageSize>` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`PageUnitText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L96) | `string` | 可写; OneWay | `"页"` | 页码后面的本地化单位文本。 |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L92) | `string` | 可写; OneWay | `"上一页"` | 上一页按钮的可访问／显示文本。 |
| [`RangeText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L110) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`RangeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L90) | `string` | 可写; OneWay | `"第 {0:N0}–{1:N0} 条，共 {2:N0} 条"` | 当前数据范围的格式文本；不更改分页计算。 |
| [`ShowJump`](../../../TheOne.Avalonia/Controls/Pagination.cs#L88) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowPageSizeSelector`](../../../TheOne.Avalonia/Controls/Pagination.cs#L87) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowRange`](../../../TheOne.Avalonia/Controls/Pagination.cs#L86) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`ShowTotal`](../../../TheOne.Avalonia/Controls/Pagination.cs#L85) | `bool` | 可写; OneWay | `true` | 默认均为 true；Compact 始终隐藏范围与跳页 |
| [`SiblingCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L83) | `int` | 可写; OneWay | `2` | 2，当前页两侧邻居数量，允许 0–5 校验：`value => value is >= 0 and <= 5` |
| [`TotalCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L77) | `long` | 可写; OneWay | `0` | 非负 long，总记录数，默认 0 校验：`value => value >= 0` |
| [`TotalText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L109) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TotalTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L89) | `string` | 可写; OneWay | `"共 {0:N0} 条"` | 总条数的格式文本；仅负责展示。 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Pagination | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=16; ItemHeight=44 |
| Pagination | `one-normal` | 模板自动类；不手动绑定 | ^.one-normal => FontSize=14; ItemHeight=36 |
| Pagination | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=12; ItemHeight=30 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Pagination`](../../api/zh-cn/TheOne.Avalonia.Controls.Pagination.md) | Class | 37 | 6 | 1 |
| [`TheOne.Avalonia.Controls.PaginationLayout`](../../api/zh-cn/TheOne.Avalonia.Controls.PaginationLayout.md) | Enum | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs`](../../api/zh-cn/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) | Class | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PaginationPageItem`](../../api/zh-cn/TheOne.Avalonia.Controls.PaginationPageItem.md) | Class | 6 | 1 | 0 |
| [`TheOne.Avalonia.Controls.PaginationPageSize`](../../api/zh-cn/TheOne.Avalonia.Controls.PaginationPageSize.md) | Class | 2 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
