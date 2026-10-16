# ReportPrintOptions

[English](../en/TheOne.Avalonia.Reporting.ReportPrintOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPrintOptions.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPrintOptions` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Copies`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L6) | `int` | 可写; CLR / init-only | `1` | 打印份数；由打印处理器校验并提交至选定打印机。 |
| [`PrinterName`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L6) | `string?` | 可写; CLR / init-only | `null` | 选定打印机的名称；为空时由具体打印处理器决定默认行为。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportPrintOptions.ReportPrintOptions(string? PrinterName = null, int Copies = 1)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L6) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
