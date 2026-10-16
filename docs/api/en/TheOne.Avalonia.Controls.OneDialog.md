# OneDialog

[English](../en/TheOne.Avalonia.Controls.OneDialog.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneDialog.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneDialog` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L119)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L123) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task<OneDialogResult> OneDialog.ShowAsync<TView, TViewModel>(TViewModel viewModel, DialogOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken)) where TView : UserControl, new() where TViewModel : class`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L125) | Creates a typed view lazily on the UI thread; compiled bindings and NativeAOT are supported. |
| [`static Task<OneDialogResult> OneDialog.ShowAsync(string title, string message, OneDialogButtons buttons = OneDialogButtons.Ok, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L136) | Dispatches to the UI thread internally. |
| [`static Task<OneDialogResult> OneDialog.ShowContentAsync(object? content, string? title = null, OneDialogButtons buttons = OneDialogButtons.OkCancel, IDataTemplate? contentTemplate = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L138) | Dispatches to the UI thread internally. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.DialogOptions`](TheOne.Avalonia.Controls.DialogOptions.md)
- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
