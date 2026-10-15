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
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-pagination .preview/pagination/render
dotnet run --project design/PreviewHost --no-build -- --render-controls .preview/pagination/render Pagination
```

专项使用真实指针／键盘输入和 Skia 绘制像素，检查明暗主题、三档尺寸、宽窄布局、边界、跳页、原子通知、焦点、模板重新应用、编译绑定和 Int64 极值。独立 NativeAotSmoke 支持 `--verify-pagination --require-aot`，需要发布并执行真正的原生程序。
