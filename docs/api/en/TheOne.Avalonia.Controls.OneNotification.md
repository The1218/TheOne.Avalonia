# OneNotification

[English](../en/TheOne.Avalonia.Controls.OneNotification.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneNotification.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneNotification` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L115)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L120) | `Avalonia.Controls.Window?` | writable; CLR / n/a | `accessor-defined / state` | The native relationship established by `child.Show(parent)` / `child.ShowDialog<T>(parent)`, affecting platform window lifetime. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static void OneNotification.Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L130) | Shows on the UI thread. The owner is optional; the registered default owner or most recently activated visible themed window is used. |
| [`static void OneNotification.Show(INotification notification, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L136) | UI thread. |
| [`static Task OneNotification.ShowAsync(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null, Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L142) | Dispatches to the UI thread internally. |
| [`static void OneNotification.CloseAll(Window? owner = null)`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L148) | UI thread. |
