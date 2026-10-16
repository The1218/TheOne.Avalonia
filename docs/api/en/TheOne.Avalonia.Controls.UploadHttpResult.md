# UploadHttpResult

[English](../en/TheOne.Avalonia.Controls.UploadHttpResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadHttpResult.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadHttpResult` · Class · `object`

A successful HTTP response. A null URL means the response contained no matching absolute file URL.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L32)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ResponseBody`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L32) | `string` | writable; CLR / init-only | `required constructor argument` | HTTP upload response body, parsed by the host according to its server protocol rather than an assumed JSON shape. |
| [`StatusCode`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L32) | `System.Net.HttpStatusCode` | writable; CLR / init-only | `required constructor argument` | HTTP response status code, checked alongside upload success and returned URL. Values: `Continue`, `SwitchingProtocols`, `Processing`, `EarlyHints`, `OK`, `Created`, `Accepted`, `NonAuthoritativeInformation`, `NoContent`, `ResetContent`, `PartialContent`, `MultiStatus`, `AlreadyReported`, `IMUsed`, `MultipleChoices`, `Ambiguous`, `MovedPermanently`, `Moved`, `Found`, `Redirect`, `SeeOther`, `RedirectMethod`, `NotModified`, `UseProxy`, `Unused`, `TemporaryRedirect`, `RedirectKeepVerb`, `PermanentRedirect`, `BadRequest`, `Unauthorized`, `PaymentRequired`, `Forbidden`, `NotFound`, `MethodNotAllowed`, `NotAcceptable`, `ProxyAuthenticationRequired`, `RequestTimeout`, `Conflict`, `Gone`, `LengthRequired`, `PreconditionFailed`, `RequestEntityTooLarge`, `RequestUriTooLong`, `UnsupportedMediaType`, `RequestedRangeNotSatisfiable`, `ExpectationFailed`, `MisdirectedRequest`, `UnprocessableEntity`, `UnprocessableContent`, `Locked`, `FailedDependency`, `UpgradeRequired`, `PreconditionRequired`, `TooManyRequests`, `RequestHeaderFieldsTooLarge`, `UnavailableForLegalReasons`, `InternalServerError`, `NotImplemented`, `BadGateway`, `ServiceUnavailable`, `GatewayTimeout`, `HttpVersionNotSupported`, `VariantAlsoNegotiates`, `InsufficientStorage`, `LoopDetected`, `NotExtended`, `NetworkAuthenticationRequired` |
| [`SuccessFileUrl`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L32) | `string?` | writable; CLR / init-only | `required constructor argument` | Returned file URL after upload; may be empty and requires server-response adaptation. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`UploadHttpResult.UploadHttpResult(HttpStatusCode StatusCode, string ResponseBody, string? SuccessFileUrl)`](../../../TheOne.Avalonia/Controls/UploadRequest.cs#L32) | A successful HTTP response. A null URL means the response contained no matching absolute file URL. |
