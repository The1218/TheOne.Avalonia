# RefreshOptions

[English](../en/TheOne.Avalonia.Controls.RefreshOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RefreshOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RefreshOptions` · Class · `Avalonia.AvaloniaObject`

Density for the native refresh indicator, including the container-created visualizer.

[Source / 声明](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L10)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`IndicatorSize`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | `double` | writable; OneWay; attached; inherits | `22` | Refresh-indicator size in DIPs, independent of the pull threshold. |
| [`UseThemeVisualizerSizing`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | `bool` | writable; OneWay; attached | `false` | Makes the refresh visualizer follow shared theme density; disable for explicit sizing. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static double RefreshOptions.GetIndicatorSize(Control control)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L23) | Reads the attached property. Refresh-indicator size in DIPs, independent of the pull threshold. |
| [`static void RefreshOptions.SetIndicatorSize(Control control, double value)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L24) | Sets the attached property. Refresh-indicator size in DIPs, independent of the pull threshold. |
| [`static bool RefreshOptions.GetUseThemeVisualizerSizing(RefreshContainer container)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L25) | Reads the attached property. Makes the refresh visualizer follow shared theme density; disable for explicit sizing. |
| [`static void RefreshOptions.SetUseThemeVisualizerSizing(RefreshContainer container, bool value)`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L26) | Sets the attached property. Makes the refresh visualizer follow shared theme density; disable for explicit sizing. |
| [`RefreshOptions.RefreshOptions()`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L10) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<double> RefreshOptions.IndicatorSizeProperty`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<bool> RefreshOptions.UseThemeVisualizerSizingProperty`](../../../TheOne.Avalonia/Controls/RefreshOptions.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
