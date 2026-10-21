# IAttachmentItem

[English](../en/TheOne.Avalonia.Controls.IAttachmentItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IAttachmentItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IAttachmentItem` · Interface

Read-only presentation contract. Lists forward the original item to consumer commands.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanOpen`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L25) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L26) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`FileName`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L18) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`FileSize`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L20) | `long` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`FileUrl`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L19) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L24) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SizeText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L21) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Status`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L27) | `TheOne.Avalonia.Controls.AttachmentStatus` | read-only; CLR / n/a | `accessor-defined / state` | Business node-state enum; styles derive from state rather than storing state in classes. Values: `Default`, `Uploading`, `Success`, `Error` |
| [`StatusText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L22) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Display text for operation status; business logic should use state/results rather than parse this text. |
| [`UploadProgress`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L23) | `double` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.AttachmentStatus`](TheOne.Avalonia.Controls.AttachmentStatus.md)
