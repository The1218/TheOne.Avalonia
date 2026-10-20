# UploadHttp

[English](../en/TheOne.Avalonia.Controls.UploadHttp.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadHttp.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadHttp` · Class · `object`

Streams one file as multipart/form-data, using the fields filename and filedata.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L43)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task<UploadHttpResult> UploadHttp.SendAsync(HttpClient client, string uploadApi, UploadRequestType requestType, UploadModel model, string allowedExtensions, IProgress<double>? progress = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L52) | Sends from the stream's current position and closes it when the request finishes. The caller owns the client. Progress is the file transfer percentage (0–100); an unknown-length stream reports its start and completion. Responses larger than 1 MiB are rejected. Only successful HTTP responses are inspected for file URLs. |
| [`static string? UploadHttp.ExtractFileUrl(string responseBody, string allowedExtensions)`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L87) | Returns the first absolute HTTP(S) file URL matching the extension filter, without contacting that URL. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.UploadHttpResult`](TheOne.Avalonia.Controls.UploadHttpResult.md)
- [`TheOne.Avalonia.Controls.UploadModel`](TheOne.Avalonia.Controls.UploadModel.md)
- [`TheOne.Avalonia.Controls.UploadRequestType`](TheOne.Avalonia.Controls.UploadRequestType.md)
