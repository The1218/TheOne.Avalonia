# UploadHttpResult

[English](../en/TheOne.Avalonia.Controls.UploadHttpResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadHttpResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadHttpResult` · Class · `object`

A successful HTTP response. A null URL means the response contained no matching absolute file URL.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L40)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ResponseBody`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L40) | `string` | 可写; CLR / init-only | `required constructor argument` | HTTP 上传响应正文；由宿主按服务端协议解析，不假定固定 JSON 结构。 |
| [`StatusCode`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L40) | `System.Net.HttpStatusCode` | 可写; CLR / init-only | `required constructor argument` | HTTP 响应状态码；与上传成功及返回地址一起验证。 可选值：`Continue`, `SwitchingProtocols`, `Processing`, `EarlyHints`, `OK`, `Created`, `Accepted`, `NonAuthoritativeInformation`, `NoContent`, `ResetContent`, `PartialContent`, `MultiStatus`, `AlreadyReported`, `IMUsed`, `MultipleChoices`, `Ambiguous`, `MovedPermanently`, `Moved`, `Found`, `Redirect`, `SeeOther`, `RedirectMethod`, `NotModified`, `UseProxy`, `Unused`, `TemporaryRedirect`, `RedirectKeepVerb`, `PermanentRedirect`, `BadRequest`, `Unauthorized`, `PaymentRequired`, `Forbidden`, `NotFound`, `MethodNotAllowed`, `NotAcceptable`, `ProxyAuthenticationRequired`, `RequestTimeout`, `Conflict`, `Gone`, `LengthRequired`, `PreconditionFailed`, `RequestEntityTooLarge`, `RequestUriTooLong`, `UnsupportedMediaType`, `RequestedRangeNotSatisfiable`, `ExpectationFailed`, `MisdirectedRequest`, `UnprocessableEntity`, `UnprocessableContent`, `Locked`, `FailedDependency`, `UpgradeRequired`, `PreconditionRequired`, `TooManyRequests`, `RequestHeaderFieldsTooLarge`, `UnavailableForLegalReasons`, `InternalServerError`, `NotImplemented`, `BadGateway`, `ServiceUnavailable`, `GatewayTimeout`, `HttpVersionNotSupported`, `VariantAlsoNegotiates`, `InsufficientStorage`, `LoopDetected`, `NotExtended`, `NetworkAuthenticationRequired` |
| [`SuccessFileUrl`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L40) | `string?` | 可写; CLR / init-only | `required constructor argument` | 成功上传后返回的文件地址；可能为空，需与服务端响应适配。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`UploadHttpResult.UploadHttpResult(HttpStatusCode StatusCode, string ResponseBody, string? SuccessFileUrl)`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L40) | A successful HTTP response. A null URL means the response contained no matching absolute file URL. |
