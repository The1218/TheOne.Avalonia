# UploadModel

[English](../en/TheOne.Avalonia.Controls.UploadModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadModel` · Class · `object`

A single upload payload. Sending it transfers ownership of to the request.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L17) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`FileData`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L16) | `System.IO.Stream` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`FileName`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L15) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`UploadModel.UploadModel(string fileName, Stream fileData, string? contentType = null)`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L19) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |
