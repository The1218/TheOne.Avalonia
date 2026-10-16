# ReportApiRequest

[English](../en/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportApiRequest.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportApiRequest` · Class · `object`

Portable API configuration. Parameters is a JSON object; GET parameters become query values.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Authentication`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L12) | `TheOne.Avalonia.Reporting.ReportApiAuthentication?` | 可写; CLR / init-only | `null` | 可选登录配置；运行期解析的凭据与报表序列化定义分开管理。 |
| [`Method`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10) | `string` | 可写; CLR / init-only | `"GET"` | HTTP 请求方法，例如 GET 或 POST；应用应按服务端接口配置。 |
| [`ParameterMode`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | 可写; CLR / init-only | `ReportApiParameterMode.Json` | 请求参数编码方式枚举；JSON 与表单处理见报表 API 数据源章节。 可选值：`Json`, `Form` |
| [`Parameters`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10) | `string` | 可写; CLR / init-only | `""` | 请求参数配置文本，按 ParameterMode 编码；不自动推导业务字段。 |
| [`Url`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10) | `string` | 可写; CLR / init-only | `required constructor argument` | 请求的绝对地址；协议与加载边界遵循报表 HTTP 数据源契约。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportApiRequest.ReportApiRequest(string Url, string Method = "GET", string Parameters = "", ReportApiParameterMode ParameterMode = ReportApiParameterMode.Json)`](../../../TheOne.Avalonia/Reporting/ReportApiRequest.cs#L10) | Portable API configuration. Parameters is a JSON object; GET parameters become query values. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiAuthentication`](TheOne.Avalonia.Reporting.ReportApiAuthentication.md)
- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
