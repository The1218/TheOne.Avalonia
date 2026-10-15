# Pagination

[简体中文](../zh-cn/pagination.md) · **English**

`Pagination` uses native Button, ComboBox and TextBox controls for navigation. With `OneTheme`, it supports Standard / Compact, Light / Dark and Small / Normal / Large. It manages page numbers and ranges, but does not fetch or filter business data. Request data through two-way bindings, PageChanged or PageChangedCommand.

```xml
<!-- Declare x:DataType="vm:YourViewModel" on the view root; one is using:TheOne.Avalonia.Controls. -->
<one:Pagination TotalCount="{Binding TotalCount}"
                PageSize="{Binding PageSize, Mode=TwoWay}"
                CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                PageChangedCommand="{Binding LoadPageCommand}" />
```

## Page numbers and bounds

| Property | Contract |
| --- | --- |
| TotalCount | Non-negative long record count; default 0 |
| PageSize | Positive integer; default 20; two-way by default |
| CurrentPage | One-based long page number; default 1; two-way by default |
| PageCount | Read-only count of pages; 0 for empty results |
| FirstItem / LastItem | Read-only, one-based record range; both 0 for empty results |
| CanGoPrevious / CanGoNext | Read-only availability of directional targets |
| HasPages | Read-only indication of non-empty record pages |

Changing PageSize resets CurrentPage to 1. Reducing TotalCount clamps an obsolete page to the new last page. Increasing the total does not restore an old out-of-range request. During initialization, provide TotalCount and PageSize before CurrentPage. Empty results retain CurrentPage=1 as the business request origin, display `0 / 0`, omit numbered links and disable navigation / jumping. The default empty range label is “暂无记录”.

GoToPage(long), GoPrevious(), GoNext(), GoFirst() and GoLast() report whether navigation actually changed the page. Invalid, repeated, empty or disabled requests return false. Direct CurrentPage assignments must be positive; out-of-range values are clamped to the last page. Update UI properties / collections and invoke navigation methods on the UI thread.

## Layout and omitted pages

Standard includes totals, page-size selection, numbered navigation, jumping and the item range. Compact shows previous, current / total pages and next, with totals / page size below. Narrow widths automatically use Compact; set Layout=Compact to request it explicitly.

| Property | Default and purpose |
| --- | --- |
| Layout | Standard / Compact; default Standard |
| CompactBreakpoint | 640; Standard becomes compact below this content width or when the measured navigation cannot fit; 0 disables automatic switching |
| SiblingCount | 2 neighboring pages per side; allowed range 0–5 |
| ShowTotal / ShowRange / ShowPageSizeSelector / ShowJump | All true; Compact always hides the range and jump controls |
| ItemHeight | Minimum navigation / input height: 30 / 36 / 44 for Small / Normal / Large |

Links retain first / last pages and current-page neighbors. A non-interactive ellipsis represents omitted pages; a gap of one missing page displays that page instead. Page 8 of 64 shows `1 … 6 7 8 9 10 … 64`. Link count depends on SiblingCount, not the total pages; Int64 totals do not create one control per page. Standard groups wrap when space is insufficient; compact groups stack. Padding preserves control insets.

## Page size and jumping

PageSizeOptions accepts IEnumerable<int> and defaults to 10, 20, 50, 100. All values must be positive; duplicates are removed. A custom active PageSize is appended when it is absent from the choices. ObservableCollection changes refresh choices; collection subscriptions are removed when detached and refreshed on reattachment.

Jump input commits on Enter or the jump button, not during typing. Invalid text or a page beyond PageCount preserves the current page and displays an error. Editing clears the error; Escape restores the current page. Native controls retain focus, keyboard input, disabled states and theme feedback. Ellipses are not tab targets.

## Notifications and data loading

PageChanged fires once after an actual page or size change has been normalized. It supplies OldPage, CurrentPage, OldPageSize and PageSize. A page-size change produces one notification containing the new page and size together. A total change with the same current page does not notify. Initialization, direct assignments and boundary clamping can also notify.

PageChangedCommand receives the same PaginationPageChangedEventArgs, with CanExecute checked before execution. It is a notification rather than a navigation gate. Bind IsEnabled to disable interaction during loading. Applications manage cancellation, errors and asynchronous result ordering.

```csharp
if (parameter is PaginationPageChangedEventArgs request)
{
    // Fetch business data using request.CurrentPage and request.PageSize.
}
```

## Text, theme and templates

Configure TotalTextFormat, RangeTextFormat, PageSizeTextFormat, PreviousText, NextText, JumpText, JumpPrefixText, PageUnitText and EmptyText. Formats use the current culture and receive the total, first / last / total, and page size respectively. Page numbers and compact counts follow the current culture. Override the default Chinese captions for localization.

The theme reuses One.Surface, One.Text, One.Muted, One.Line, One.Accent, One.OnAccent and native input-state resources. Preserve PART_PreviousButton, PART_NextButton, PART_Pages, PART_PageSizes, PART_JumpInput and PART_JumpButton for their interactions. Auxiliary theme keys are One.PaginationButton and One.PaginationPages. A custom control theme can change layout; omitting a part removes only its corresponding UI interaction.

## Examples and verification

PreviewHost's “扩展控件 → Pagination” page demonstrates shared Standard / Compact bindings, first / last pages, empty results and disabled controls. Actions restore and reduce the record count.

```sh
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-pagination .preview/pagination/render
dotnet run --project design/PreviewHost --no-build -- --render-controls .preview/pagination/render Pagination
```

Checks use actual pointer / keyboard input and Skia pixels for themes, sizes, wide / narrow layout, bounds, jumping, atomic notifications, focus, template reapplication, compiled bindings and Int64 extremes. NativeAotSmoke supports --verify-pagination --require-aot; publish and execute a real native binary.
