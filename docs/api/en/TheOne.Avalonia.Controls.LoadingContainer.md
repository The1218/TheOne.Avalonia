# LoadingContainer

[English](../en/TheOne.Avalonia.Controls.LoadingContainer.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.LoadingContainer.md) · [API index](README.md)

`TheOne.Avalonia.Controls.LoadingContainer` · Class · `Avalonia.Controls.ContentControl`

Hosts content with a modal, theme-aware loading overlay without replacing its DataContext or layout.

[Source / 声明](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L11)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HasCustomLoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L95) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HasLoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L107) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L53) | `bool` | writable; OneWay | `false` | Shows busy feedback; buttons retain size and prevent repeated actions, while containers show a loading layer. The host resets it when work finishes. |
| [`LoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L77) | `object?` | writable; OneWay | `null` | Loading-state icon; null retains the default indicator. |
| [`LoadingIconSize`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L89) | `double` | writable; OneWay | `28` | Loading icon size in DIPs, independent of overlay area. Validation: `value => double.IsFinite(value) && value > 0` |
| [`LoadingIconTemplate`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L83) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`LoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L59) | `string?` | writable; OneWay | `"正在加载…"` | Localizable loading message, distinct from the task's completion result. |
| [`OverlayBrush`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L71) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Loading overlay brush; use One.* dynamic resources for themes. Explicit local values take precedence. |
| [`OverlayOpacity`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L65) | `double` | writable; OneWay | `.65` | Overlay opacity; see validation for range/finite-number constraints. Validation: `value => double.IsFinite(value) && value is >= 0 and <= 1` |
| [`ShowDefaultLoadingIndicator`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L101) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`LoadingContainer.LoadingContainer()`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L48) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<bool> LoadingContainer.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> LoadingContainer.LoadingTextProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> LoadingContainer.OverlayOpacityProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> LoadingContainer.OverlayBrushProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> LoadingContainer.LoadingIconProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> LoadingContainer.LoadingIconTemplateProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> LoadingContainer.LoadingIconSizeProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.HasCustomLoadingIconProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.ShowDefaultLoadingIndicatorProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L40) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<LoadingContainer, bool> LoadingContainer.HasLoadingTextProperty`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
