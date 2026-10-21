# AttachmentPreviewRequestedEventArgs

[English](../en/TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

Forwards the original attachment for host-controlled preview or download.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L278)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L280) | `TheOne.Avalonia.Controls.IAttachmentItem` | read-only; CLR / n/a | `item ?? throw new ArgumentNullException(nameof(item))` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`AttachmentPreviewRequestedEventArgs.AttachmentPreviewRequestedEventArgs(IAttachmentItem item)`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L278) | Forwards the original attachment for host-controlled preview or download. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IAttachmentItem`](TheOne.Avalonia.Controls.IAttachmentItem.md)
