# ReportLayoutRow

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutRow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutRow.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutRow` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Cells`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `System.Collections.Generic.IReadOnlyList<string>` | 可写; CLR / init-only | `required constructor argument` | 一行按列顺序排列的最终文本；不再从此快照反查原始数据。 |
| [`Foregrounds`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L10) | `System.Collections.Generic.IReadOnlyList<string>` | 可写; CLR / init-only | `[]` | 按单元格顺序排列的条件前景色；与 Cells 索引一致。 |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `double` | 可写; CLR / init-only | `required constructor argument` | 高度配置；保留内容、字号及数据坐标契约，固定值可能覆盖主题密度。 |
| [`SourceIndex`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | `int` | 可写; CLR / init-only | `required constructor argument` | 布局行对应的原始数据零基索引，用于定位源记录。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportLayoutRow.ReportLayoutRow(int SourceIndex, IReadOnlyList<string> Cells, double Height)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
