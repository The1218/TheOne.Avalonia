# UploadFile

[English](../en/TheOne.Avalonia.Controls.UploadFile.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadFile.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadFile` · Class · `object`

An immutable single-file selection. Every read returns a new stream owned by the caller.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadFile.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L29) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Name`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L27) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`Size`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L28) | `long?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SizeText`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L31) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`void UploadFile.Dispose()`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L36) | Rejects new reads and releases an owned storage source after all open streams close. |
| [`Task<Stream> UploadFile.OpenReadAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L53) | Asynchronously opens an upload file's readable stream; the caller disposes it after use. |
| [`static Task<UploadFile> UploadFile.FromStorageFileAsync(IStorageFile file, CancellationToken cancellationToken = default(CancellationToken), bool ownsStorageFile = false)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L87) | The storage file must remain usable for as long as this selection is used. |
| [`UploadFile.UploadFile(string name, Func<CancellationToken, Task<Stream>> openRead, long? size = null, string? contentType = null)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L15) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
