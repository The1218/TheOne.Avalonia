# ReportApiAuthentication

[English](../en/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportApiAuthentication` · Class · `object`

Login configuration only. Resolved tokens are never stored in the report document.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HeaderName`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L14) | `string` | writable; CLR / init-only | `"Authorization"` | HTTP header carrying resolved authentication, normally Authorization; validated by the loader. |
| [`HeaderPrefix`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L15) | `string` | writable; CLR / init-only | `"Bearer "` | Prefix prepended to the token, normally Bearer plus a space; resolved tokens are not persisted. |
| [`Method`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L10) | `string` | writable; CLR / init-only | `"POST"` | HTTP method such as GET/POST, configured according to the server API. |
| [`ParameterMode`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L12) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | writable; CLR / init-only | `0 / default` | Request-parameter encoding enum; see report API source guidance for JSON/form behavior. Values: `Json`, `Form` |
| [`Parameters`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L11) | `string` | writable; CLR / init-only | `""` | Request-parameter text encoded according to ParameterMode, without inferring business fields. |
| [`TokenField`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L13) | `string` | writable; CLR / init-only | `"/data/token"` | Token path in the login response, as JSON Pointer or dot path; must resolve to a nonempty string/number. |
| [`Url`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L9) | `string` | writable; CLR / init-only | `""` | Absolute request URL, following report HTTP source protocol/loading boundaries. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportApiAuthentication.ReportApiAuthentication()`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
