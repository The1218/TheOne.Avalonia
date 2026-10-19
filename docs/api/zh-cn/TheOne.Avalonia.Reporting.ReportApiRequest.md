# ReportApiRequest

[English](../en/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportApiRequest` · Class · `object`

Portable API configuration. Parameters is a JSON object; GET parameters become query values.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Authentication`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L19) | `TheOne.Avalonia.Reporting.ReportApiAuthentication?` | 可写; CLR / init-only | `null` | 可选登录配置；运行期解析的凭据与报表序列化定义分开管理。 |
| [`Method`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L15) | `string` | 可写; CLR / init-only | `"GET"` | HTTP 请求方法，例如 GET 或 POST；应用应按服务端接口配置。 |
| [`ParameterMode`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L17) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | 可写; CLR / init-only | `ReportApiParameterMode.Json` | 请求参数编码方式枚举；JSON 与表单处理见报表 API 数据源章节。 可选值：`Json`, `Form` |
| [`Parameters`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L16) | `string` | 可写; CLR / init-only | `""` | 请求参数配置文本，按 ParameterMode 编码；不自动推导业务字段。 |
| [`SelectedDataPointers`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L22) | `string[]?` | 可写; CLR / init-only | `null` | JSON Pointers visible in the designer; null retains all available items. |
| [`SourceName`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L20) | `string` | 可写; CLR / init-only | `"接口数据"` | 请求成功后用于数据目录分组的来源名称；默认“接口数据”，不作为身份标识或请求 URL。 |
| [`Url`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L14) | `string` | 可写; CLR / init-only | `required constructor argument` | 请求的绝对地址；协议与加载边界遵循报表 HTTP 数据源契约。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportApiRequest.ReportApiRequest(string Url, string Method = "GET", string Parameters = "", ReportApiParameterMode ParameterMode = ReportApiParameterMode.Json)`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L13) | Portable API configuration. Parameters is a JSON object; GET parameters become query values. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](TheOne.Avalonia.Reporting.ReportApiAuthentication.md)
- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
