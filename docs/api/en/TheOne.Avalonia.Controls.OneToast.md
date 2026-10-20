# OneToast

[English](../en/TheOne.Avalonia.Controls.OneToast.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneToast.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneToast` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L154)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L159) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static void OneToast.Show(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L165) | UI thread. |
| [`static void OneToast.Show(INotification notification, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L175) | Shows the notification's message and status icon, ignoring Title, while retaining expiration and click/close callbacks. |
| [`static Task OneToast.ShowAsync(string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L183) | Dispatches to the UI thread internally. |
| [`static void OneToast.CloseAll(Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L189) | UI thread. |
