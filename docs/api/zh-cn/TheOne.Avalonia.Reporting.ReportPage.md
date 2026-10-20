# ReportPage

[English](../en/TheOne.Avalonia.Reporting.ReportPage.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPage.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPage` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Number`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32) | `int` | 可写; CLR / init-only | `required constructor argument` | 分页结果中的页号；用于页码绘制。 |
| [`Rows`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportLayoutRow>` | 可写; CLR / init-only | `required constructor argument` | 此页包含的原子明细行集合；不在打印阶段重新拆分行。 |
| [`ShowSummary`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32) | `bool` | 可写; CLR / init-only | `required constructor argument` | 此页是否绘制汇总区域，通常由分页器对末页设置。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportPage.ReportPage(int Number, IReadOnlyList<ReportLayoutRow> Rows, bool ShowSummary)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L32) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportLayoutRow`](TheOne.Avalonia.Reporting.ReportLayoutRow.md)
