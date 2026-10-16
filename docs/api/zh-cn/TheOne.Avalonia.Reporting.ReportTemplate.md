# ReportTemplate

[English](../en/TheOne.Avalonia.Reporting.ReportTemplate.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportTemplate.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportTemplate` · Class · `object`

Versioned, portable report templates. Data rows and API responses are never serialized.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static ReportDocument ReportTemplate.CreateSalesDetail()`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L13) | A source-independent starting template matching the sales report design. |
| [`static string ReportTemplate.Save(ReportDocument document)`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L33) | 将业务文档保存为可移植 JSON；不包含运行期凭据、视觉对象或非持久化状态。 |
| [`static ReportDocument ReportTemplate.Load(string json)`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L40) | 从 JSON 载入类型化业务文档；版本、字段及结构校验见对应序列化契约。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`const int ReportTemplate.MaximumBytes`](../../../TheOne.Avalonia/Reporting/ReportTemplate.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
