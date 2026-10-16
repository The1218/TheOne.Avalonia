# UploadHttp

[English](../en/TheOne.Avalonia.Controls.UploadHttp.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadHttp.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadHttp` · Class · `object`

Streams one file as multipart/form-data, using the fields filename and filedata.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L35)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static Task<UploadHttpResult> UploadHttp.SendAsync(HttpClient client, string uploadApi, UploadRequestType requestType, UploadModel model, string allowedExtensions, IProgress<double>? progress = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L44) | Sends from the stream's current position and closes it when the request finishes. The caller owns the client. Progress is the file transfer percentage (0–100); an unknown-length stream reports its start and completion. Responses larger than 1 MiB are rejected. Only successful HTTP responses are inspected for file URLs. |
| [`static string? UploadHttp.ExtractFileUrl(string responseBody, string allowedExtensions)`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L75) | Returns the first absolute HTTP(S) file URL matching the extension filter, without contacting that URL. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.UploadHttpResult`](TheOne.Avalonia.Controls.UploadHttpResult.md)
- [`TheOne.Avalonia.Controls.UploadModel`](TheOne.Avalonia.Controls.UploadModel.md)
- [`TheOne.Avalonia.Controls.UploadRequestType`](TheOne.Avalonia.Controls.UploadRequestType.md)
