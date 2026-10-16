# ReportApiAuthentication

[English](../en/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportApiAuthentication.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportApiAuthentication` · Class · `object`

Login configuration only. Resolved tokens are never stored in the report document.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HeaderName`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L14) | `string` | 可写; CLR / init-only | `"Authorization"` | 携带鉴权结果的 HTTP 请求头名称；默认 Authorization，由加载器校验合法性。 |
| [`HeaderPrefix`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L15) | `string` | 可写; CLR / init-only | `"Bearer "` | 拼接在 token 前的请求头前缀；默认 Bearer 后有空格，不保存解析后的 token。 |
| [`Method`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L10) | `string` | 可写; CLR / init-only | `"POST"` | HTTP 请求方法，例如 GET 或 POST；应用应按服务端接口配置。 |
| [`ParameterMode`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L12) | `TheOne.Avalonia.Reporting.ReportApiParameterMode` | 可写; CLR / init-only | `0 / default` | 请求参数编码方式枚举；JSON 与表单处理见报表 API 数据源章节。 可选值：`Json`, `Form` |
| [`Parameters`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L11) | `string` | 可写; CLR / init-only | `""` | 请求参数配置文本，按 ParameterMode 编码；不自动推导业务字段。 |
| [`TokenField`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L13) | `string` | 可写; CLR / init-only | `"/data/token"` | 登录响应中 token 的路径；支持 JSON Pointer 或点分路径，必须得到非空字符串或数字。 |
| [`Url`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L9) | `string` | 可写; CLR / init-only | `""` | 请求的绝对地址；协议与加载边界遵循报表 HTTP 数据源契约。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportApiAuthentication.ReportApiAuthentication()`](../../../TheOne.Avalonia/Reporting/ReportApiAuthentication.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportApiParameterMode`](TheOne.Avalonia.Reporting.ReportApiParameterMode.md)
