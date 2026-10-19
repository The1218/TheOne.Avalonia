# ReportApiRequest

[English](../en/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportApiRequest` · Class · `object`

Portable API configuration. Parameters is a JSON object; GET parameters become query values.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Authentication`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L19) | `TheOne.Avalonia.Reporting.ReportApiAuthentication?` | writable; CLR / init-only | `null` | Optional login configuration; resolved runtime credentials are kept separate from the serialized report definition. |
| [`Method`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L15) | `string` | writable; CLR / init-only | `"GET"` | HTTP method such as GET/POST, configured according to the server API. |
| [`ParameterMode`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L17) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | writable; CLR / init-only | `ReportApiParameterMode.Json` | Request-parameter encoding enum; see report API source guidance for JSON/form behavior. Values: `Json`, `Form` |
| [`Parameters`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L16) | `string` | writable; CLR / init-only | `""` | Request-parameter text encoded according to ParameterMode, without inferring business fields. |
| [`SelectedDataPointers`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L22) | `string[]?` | writable; CLR / init-only | `null` | JSON Pointers visible in the designer; null retains all available items. |
| [`SourceName`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L20) | `string` | writable; CLR / init-only | `"接口数据"` | Catalog group label after a successful request, default “接口数据”; not an identity key or request URL. |
| [`Url`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L14) | `string` | writable; CLR / init-only | `required constructor argument` | Absolute request URL, following report HTTP source protocol/loading boundaries. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`ReportApiRequest.ReportApiRequest(string Url, string Method = "GET", string Parameters = "", ReportApiParameterMode ParameterMode = ReportApiParameterMode.Json)`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L13) | Portable API configuration. Parameters is a JSON object; GET parameters become query values. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](TheOne.Avalonia.Reporting.ReportApiAuthentication.md)
- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
