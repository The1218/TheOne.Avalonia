# Pagination

[English](../en/TheOne.Avalonia.Controls.Pagination.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Pagination.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Pagination` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

One-based pagination with bounded page links and native navigation / input controls.

[Source / 声明](../../../TheOne.Avalonia/Controls/Pagination.cs#L26)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanGoNext`](../../../TheOne.Avalonia/Controls/Pagination.cs#L103) | `bool` | read-only; OneWay | `computed / state` | Read-only availability of directional targets |
| [`CanGoPrevious`](../../../TheOne.Avalonia/Controls/Pagination.cs#L102) | `bool` | read-only; OneWay | `computed / state` | Read-only availability of directional targets |
| [`CompactBreakpoint`](../../../TheOne.Avalonia/Controls/Pagination.cs#L82) | `double` | writable; OneWay | `640` | 640; Standard becomes compact below this content width or when the measured navigation cannot fit; 0 disables automatic switching Validation: `value => double.IsFinite(value) && value >= 0` |
| [`CurrentPage`](../../../TheOne.Avalonia/Controls/Pagination.cs#L78) | `long` | writable; TwoWay | `1` | One-based long page number; default 1; two-way by default Validation: `value => value >= 1` |
| [`CurrentPageText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L111) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L97) | `string` | writable; OneWay | `"暂无记录"` | Message when no data can be displayed; applications override it for localization. |
| [`FirstItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L100) | `long` | read-only; OneWay | `computed / state` | Read-only, one-based record range; both 0 for empty results |
| [`HasJumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L107) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasPages`](../../../TheOne.Avalonia/Controls/Pagination.cs#L104) | `bool` | read-only; OneWay | `computed / state` | Read-only indication of non-empty record pages |
| [`HasSummary`](../../../TheOne.Avalonia/Controls/Pagination.cs#L106) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsCompact`](../../../TheOne.Avalonia/Controls/Pagination.cs#L105) | `bool` | read-only; OneWay | `computed / state` | Enables compact layout; the template determines visible fields without altering business data. |
| [`ItemHeight`](../../../TheOne.Avalonia/Controls/Pagination.cs#L84) | `double` | writable; OneWay | `36` | Minimum navigation / input height: 30 / 36 / 44 for Small / Normal / Large Validation: `value => double.IsFinite(value) && value > 0` |
| [`JumpError`](../../../TheOne.Avalonia/Controls/Pagination.cs#L108) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`JumpPrefixText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L95) | `string` | writable; OneWay | `"跳至"` | Localized text before the page-jump input. |
| [`JumpText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L94) | `string` | writable; OneWay | `"跳转"` | Text on the page-jump action button. |
| [`LastItem`](../../../TheOne.Avalonia/Controls/Pagination.cs#L101) | `long` | read-only; OneWay | `computed / state` | Read-only, one-based record range; both 0 for empty results |
| [`Layout`](../../../TheOne.Avalonia/Controls/Pagination.cs#L81) | `TheOne.Avalonia.Controls.PaginationLayout` | writable; OneWay | `0 / default` | Standard / Compact; default Standard Validation: `Enum.IsDefined` Values: `Standard`, `Compact` |
| [`NextText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L93) | `string` | writable; OneWay | `"下一页"` | Accessible/display text for the next-page button. |
| [`PageChangedCommand`](../../../TheOne.Avalonia/Controls/Pagination.cs#L98) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`PageCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L99) | `long` | read-only; OneWay | `computed / state` | Read-only count of pages; 0 for empty results |
| [`PageItems`](../../../TheOne.Avalonia/Controls/Pagination.cs#L112) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageItem>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PageSize`](../../../TheOne.Avalonia/Controls/Pagination.cs#L79) | `int` | writable; TwoWay | `20` | Positive integer; default 20; two-way by default Validation: `value => value > 0` |
| [`PageSizeOptions`](../../../TheOne.Avalonia/Controls/Pagination.cs#L80) | `System.Collections.Generic.IEnumerable<int>?` | writable; OneWay | `DefaultSizes` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`PageSizeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L91) | `string` | writable; OneWay | `"{0} 条 / 页"` | Format text for page-size choices, using placeholders defined in the pagination guide. |
| [`PageSizes`](../../../TheOne.Avalonia/Controls/Pagination.cs#L113) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.PaginationPageSize>` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PageUnitText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L96) | `string` | writable; OneWay | `"页"` | Localized unit displayed after a page number. |
| [`PreviousText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L92) | `string` | writable; OneWay | `"上一页"` | Accessible/display text for the previous-page button. |
| [`RangeText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L110) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`RangeTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L90) | `string` | writable; OneWay | `"第 {0:N0}–{1:N0} 条，共 {2:N0} 条"` | Format text for the current item range, without changing pagination calculations. |
| [`ShowJump`](../../../TheOne.Avalonia/Controls/Pagination.cs#L88) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowPageSizeSelector`](../../../TheOne.Avalonia/Controls/Pagination.cs#L87) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowRange`](../../../TheOne.Avalonia/Controls/Pagination.cs#L86) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`ShowTotal`](../../../TheOne.Avalonia/Controls/Pagination.cs#L85) | `bool` | writable; OneWay | `true` | All true; Compact always hides the range and jump controls |
| [`SiblingCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L83) | `int` | writable; OneWay | `2` | 2 neighboring pages per side; allowed range 0–5 Validation: `value => value is >= 0 and <= 5` |
| [`TotalCount`](../../../TheOne.Avalonia/Controls/Pagination.cs#L77) | `long` | writable; OneWay | `0` | Non-negative long record count; default 0 Validation: `value => value >= 0` |
| [`TotalText`](../../../TheOne.Avalonia/Controls/Pagination.cs#L109) | `string` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`TotalTextFormat`](../../../TheOne.Avalonia/Controls/Pagination.cs#L89) | `string` | writable; OneWay | `"共 {0:N0} 条"` | Display-only format text for the total item count. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool Pagination.GoToPage(long page)`](../../../TheOne.Avalonia/Controls/Pagination.cs#L246) | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`bool Pagination.GoPrevious()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L252) | Moves to the previous page without producing an invalid boundary page. |
| [`bool Pagination.GoNext()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L253) | Moves to the next page without producing an invalid boundary page. |
| [`bool Pagination.GoFirst()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L254) | Navigates to the first page. |
| [`bool Pagination.GoLast()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L255) | Navigates to the last page. |
| [`Pagination.Pagination()`](../../../TheOne.Avalonia/Controls/Pagination.cs#L76) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<PaginationPageChangedEventArgs>? Pagination.PageChanged`](../../../TheOne.Avalonia/Controls/Pagination.cs#L114) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<long> Pagination.TotalCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<long> Pagination.CurrentPageProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> Pagination.PageSizeProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable<int>?> Pagination.PageSizeOptionsProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<PaginationLayout> Pagination.LayoutProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Pagination.CompactBreakpointProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<int> Pagination.SiblingCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Pagination.ItemHeightProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Pagination.ShowTotalProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L37) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Pagination.ShowRangeProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Pagination.ShowPageSizeSelectorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L39) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Pagination.ShowJumpProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.TotalTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.RangeTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L42) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.PageSizeTextFormatProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L43) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.PreviousTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.NextTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L45) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.JumpTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L46) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.JumpPrefixTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.PageUnitTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L48) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Pagination.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L49) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> Pagination.PageChangedCommandProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, long> Pagination.PageCountProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L51) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, long> Pagination.FirstItemProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L52) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, long> Pagination.LastItemProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.CanGoPreviousProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L54) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.CanGoNextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L55) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasPagesProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L56) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.IsCompactProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L57) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasSummaryProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L58) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, bool> Pagination.HasJumpErrorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L59) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, string> Pagination.JumpErrorProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L60) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, string> Pagination.TotalTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L61) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, string> Pagination.RangeTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, string> Pagination.CurrentPageTextProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L63) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageItem>> Pagination.PageItemsProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L64) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageSize>> Pagination.PageSizesProperty`](../../../TheOne.Avalonia/Controls/Pagination.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<PaginationPageChangedEventArgs> Pagination.PageChangedEvent`](../../../TheOne.Avalonia/Controls/Pagination.cs#L66) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.PaginationLayout`](TheOne.Avalonia.Controls.PaginationLayout.md)
- [`TheOne.Avalonia.Controls.PaginationPageChangedEventArgs`](TheOne.Avalonia.Controls.PaginationPageChangedEventArgs.md)
- [`TheOne.Avalonia.Controls.PaginationPageItem`](TheOne.Avalonia.Controls.PaginationPageItem.md)
- [`TheOne.Avalonia.Controls.PaginationPageSize`](TheOne.Avalonia.Controls.PaginationPageSize.md)
