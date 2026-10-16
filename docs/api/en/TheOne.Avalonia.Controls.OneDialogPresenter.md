# OneDialogPresenter

[English](../en/TheOne.Avalonia.Controls.OneDialogPresenter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneDialogPresenter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneDialogPresenter` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Buttons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L12) | `TheOne.Avalonia.Controls.OneDialogButtons` | writable; OneWay | `0 / default` | Dialog button configuration/flags, interpreted according to the options type and close-result contract. Values: `Ok`, `OkCancel`, `YesNo`, `YesNoCancel` |
| [`CloseOnEscape`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L20) | `bool` | writable; OneWay | `true` | Allows Escape to close a dialog, following its cancellation/result semantics. |
| [`ShowButtons`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L18) | `bool` | writable; OneWay | `true` | Shows the default button area; does not control buttons in custom content. |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L16) | `bool` | writable; OneWay | `true` | Shows the close button; the host and associated commands retain control over closing. |
| [`ShowTitle`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L14) | `bool` | writable; OneWay | `true` | Shows the dialog title area while retaining its configured title value. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`OneDialogPresenter.OneDialogPresenter()`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L24) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<OneDialogButtons> OneDialogPresenter.ButtonsProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowTitleProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneDialogPresenter.ShowButtonsProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> OneDialogPresenter.CloseOnEscapeProperty`](../../../TheOne.Avalonia/Controls/OneDialogPresenter.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
