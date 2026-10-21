# AttachmentItem

[English](../en/TheOne.Avalonia.Controls.AttachmentItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentItem` · Class · `object`

A theme-independent attachment presentation model, without HTTP or business relationship logic.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L31)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CanOpen`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L72) | `bool` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L73) | `bool` | writable; CLR / n/a | `true` | CanRemove configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`FileName`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L45) | `string` | writable; CLR / n/a | `""` | FileName configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`FileSize`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L51) | `long` | writable; CLR / n/a | `0 / default` | FileSize configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`FileUrl`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L46) | `string?` | writable; CLR / n/a | `null` | FileUrl configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L67) | `bool` | writable; CLR / n/a | `false` | IsUploading configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`SizeText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L56) | `string` | writable; CLR / n/a | `accessor-defined / state` | SizeText configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |
| [`Status`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L74) | `TheOne.Avalonia.Controls.AttachmentStatus` | writable; CLR / n/a | `0 / default` | Business node-state enum; styles derive from state rather than storing state in classes. Values: `Default`, `Uploading`, `Success`, `Error` |
| [`StatusText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L57) | `string` | writable; CLR / n/a | `"待上传"` | Display text for operation status; business logic should use state/results rather than parse this text. |
| [`UploadProgress`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L58) | `double` | writable; CLR / n/a | `0 / default` | UploadProgress configuration on AttachmentItem; type and initial value are listed here. Read/write uses the model's public accessor. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void AttachmentItem.MarkUploading()`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L85) | Starts an upload with unknown progress; no transfer percentage is fabricated. |
| [`void AttachmentItem.MarkUploaded(string fileUrl, long fileSize)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L95) | Records the consumer-provided server result without rewriting its URL. |
| [`void AttachmentItem.MarkFailed(string message)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L109) | Marks failure and permits removing the failed row. |
| [`static AttachmentItem AttachmentItem.CreateUploaded(string fileName, string? fileUrl, long fileSize)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L118) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`static string AttachmentItem.FormatSize(long size)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L128) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`AttachmentItem.AttachmentItem()`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L31) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event PropertyChangedEventHandler? AttachmentItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L43) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.AttachmentStatus`](TheOne.Avalonia.Controls.AttachmentStatus.md)
