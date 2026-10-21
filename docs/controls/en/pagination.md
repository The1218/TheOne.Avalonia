# Pagination

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

[简体中文](../zh-cn/pagination.md) · **English**

`Pagination` uses native Button, ComboBox and TextBox controls for navigation. With `OneTheme`, it supports Standard / Compact, Light / Dark and Small / Normal / Large. It manages page numbers and ranges, but does not fetch or filter business data. Request data through two-way bindings, PageChanged or PageChangedCommand.

```xml
<!-- Declare x:DataType="vm:YourViewModel" on the view root; one is using:TheOne.Avalonia.Controls. -->
<one:Pagination TotalCount="{Binding TotalCount}"
                PageSize="{Binding PageSize, Mode=TwoWay}"
                CurrentPage="{Binding CurrentPage, Mode=TwoWay}"
                PageChangedCommand="{Binding LoadPageCommand}" />
```

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### Pagination

![Pagination — Pages, ellipses, page size and jumping; page indices start at one and hosts perform loading.](../images/Pagination-light.png)

Pages, ellipses, page size and jumping; page indices start at one and hosts perform loading.

[View Dark comparison](../images/Pagination-dark.png)

<!-- visual-guide:end -->

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

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Pagination

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Pagination.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/Pagination.cs#L322) | `bool` | read-only; OneWay | `computed / state` | Read-only availability of directional targets |
| [`CanGoPrevious`](../../../TheOne.Avalonia/Controls/Pagination.cs#L316) | `bool` | read-only; OneWay | `computed / state` | Read-only availability of directional targets |
| [`CompactBreakpoint`](../../../TheOne.Avalonia/Controls/Pagination.cs#L196) | `double` | writable; OneWay | `640` | 640; Standard becomes compact below this content width or when the measured navigation cannot fit; 0 disables automatic switching Validation: `value => double.IsFinite(value) && value >= 0` |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/Pagination.cs#L172) | `long` | writable; TwoWay | `1` | One-based long page number; default 1; two-way by default Validation: `value => value >= 1` |
| [`CurrentPageText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L370) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L286) | `string` | writable; OneWay | `"暂无记录"` | Message when no data can be displayed; applications override it for localization. |
| [`FirstItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L304) | `long` | read-only; OneWay | `computed / state` | Read-only, one-based record range; both 0 for empty results |
| [`HasJumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L346) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasPages`](../../../TheOne.Avalonia/Controls/Pagination.cs#L328) | `bool` | read-only; OneWay | `computed / state` | Read-only indication of non-empty record pages |
| [`HasSummary`](../../../TheOne.Avalonia/Controls/Pagination.cs#L340) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/Pagination.cs#L334) | `bool` | read-only; OneWay | `computed / state` | Enables compact layout; the template determines visible fields without altering business data. |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/Pagination.cs#L208) | `double` | writable; OneWay | `36` | Minimum navigation / input height: 30 / 36 / 44 for Small / Normal / Large Validation: `value => double.IsFinite(value) && value > 0` |
| [`JumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L352) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`JumpPrefixText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L274) | `string` | writable; OneWay | `"跳至"` | Localized text before the page-jump input. |
| [`JumpText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L268) | `string` | writable; OneWay | `"跳转"` | Text on the page-jump action button. |
| [`LastItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L310) | `long` | read-only; OneWay | `computed / state` | Read-only, one-based record range; both 0 for empty results |
| [`Layout`](../../../TheOne.Avalonia/Controls/Pagination.cs#L190) | `TheOne.Avalonia.Controls.PaginationLayout` | writable; OneWay | `0 / default` | Standard / Compact; default Standard Validation: `Enum.IsDefined` Values: `Standard`, `Compact` |
| [`NextText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L262) | `string` | writable; OneWay | `"下一页"` | Accessible/display text for the next-page button. |
| [`PageChangedCommand`](../../../TheOne.Avalonia/Controls/Pagination.cs#L292) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`PageCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L298) | `long` | read-only; OneWay | `computed / state` | Read-only count of pages; 0 for empty results |
| [`PageItems`](../../../TheOne.Avalonia/Controls/Pagination.cs#L376) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PageSize`](../../../TheOne.Avalonia/Controls/Pagination.cs#L178) | `int` | writable; TwoWay | `20` | Positive integer; default 20; two-way by default Validation: `value => value > 0` |
| [`PageSizeOptions`](../../../TheOne.Avalonia/Controls/Pagination.cs#L184) | `System.Collections.Generic.IEnumerable<int>?` | writable; OneWay | `DefaultSizes` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`PageSizeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L250) | `string` | writable; OneWay | `"{0} 条 / 页"` | Format text for page-size choices, using placeholders defined in the pagination guide. |
| [`PageSizes`](../../../TheOne.Avalonia/Controls/Pagination.cs#L382) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageSize>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PageUnitText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L280) | `string` | writable; OneWay | `"页"` | Localized unit displayed after a page number. |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L256) | `string` | writable; OneWay | `"上一页"` | Accessible/display text for the previous-page button. |
| [`RangeText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L364) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`RangeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L244) | `string` | writable; OneWay | `"第 {0:N0}–{1:N0} 条，共 {2:N0} 条"` | Format text for the current item range, without changing pagination calculations. |
| [`ShowJump`](../../../TheOne.Avalonia/Controls/Pagination.cs#L232) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowPageSizeSelector`](../../../TheOne.Avalonia/Controls/Pagination.cs#L226) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowRange`](../../../TheOne.Avalonia/Controls/Pagination.cs#L220) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowTotal`](../../../TheOne.Avalonia/Controls/Pagination.cs#L214) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`SiblingCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L202) | `int` | writable; OneWay | `2` | 2 neighboring pages per side; allowed range 0–5 Validation: `value => value is >= 0 and <= 5` |
| [`TotalCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L166) | `long` | writable; OneWay | `0 / default` | Non-negative long record count; default 0 Validation: `value => value >= 0` |
| [`TotalText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L358) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TotalTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L238) | `string` | writable; OneWay | `"共 {0:N0} 条"` | Display-only format text for the total item count. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `bool Pagination.GoToPage(long page)` | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `bool Pagination.GoPrevious()` | Moves to the previous page without producing an invalid boundary page. |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `bool Pagination.GoNext()` | Moves to the next page without producing an invalid boundary page. |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `bool Pagination.GoFirst()` | Navigates to the first page. |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `bool Pagination.GoLast()` | Navigates to the last page. |

### Event subscriptions

Subscribe when needed and detach when the host is disposed. These are event entry points, not routed-event identifier fields.

| Type | Event | Purpose |
| --- | --- | --- |
| [`Pagination`](../../api/en/TheOne.Avalonia.Controls.Pagination.md) | `PageChanged` | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.PaginationLayout`](../../api/en/TheOne.Avalonia.Controls.PaginationLayout.md) | 2 |
| [`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs`](../../api/en/TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md) | 4 |
| [`TheOne.Avalonia.Controls.PaginationPageItem`](../../api/en/TheOne.Avalonia.Controls.PaginationPageItem.md) | 6 |
| [`TheOne.Avalonia.Controls.PaginationPageSize`](../../api/en/TheOne.Avalonia.Controls.PaginationPageSize.md) | 2 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
