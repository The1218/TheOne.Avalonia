# OneWindow

[English](../en/TheOne.Avalonia.Controls.OneWindow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneWindow.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneWindow` · Class · `Avalonia.Controls.Window`

A native Window with a customizable title bar and automatically managed message hosts.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneWindow.cs#L12)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L39) | `TheOne.Avalonia.Controls.OneWindow?` | read-only; CLR / n/a | `accessor-defined / state` | UI thread. |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L38) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L30) | `object?` | writable; OneWay | `null` | Additional content on the left side of the library window title bar. |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L32) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L31) | `object?` | writable; OneWay | `null` | Additional right title-bar content; avoid covering window-operation hit areas. |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L34) | `bool` | writable; OneWay | `true` | Shows the built-in brightness toggle; palette resources remain controlled by ThemeColorPicker/OneTheme. |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L36) | `bool` | writable; OneWay | `true` | Shows the library title bar without changing window lifetime or ownership. |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L35) | `TheOne.Avalonia.Controls.ThemeButtonScope` | writable; OneWay | `0 / default` | Theme-toggle scope for the built-in button, distinguishing application and window via enum. Values: `Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L37) | `double` | writable; OneWay | `48` | Title-bar height in DIPs; retain sufficient title and window-button hit areas. Validation: `value => double.IsFinite(value) && value >= 0` |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`OneWindow.OneWindow()`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L41) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> OneWindow.LeftContentProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> OneWindow.RightContentProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> OneWindow.LeftContentTemplateProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> OneWindow.RightContentTemplateProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneWindow.ShowThemeButtonProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ThemeButtonScope> OneWindow.ThemeButtonScopeProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneWindow.ShowTitleBarProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> OneWindow.TitleBarHeightProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<OneWindow, bool> OneWindow.IsDialogOpenProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.ThemeButtonScope`](TheOne.Avalonia.Controls.ThemeButtonScope.md)
