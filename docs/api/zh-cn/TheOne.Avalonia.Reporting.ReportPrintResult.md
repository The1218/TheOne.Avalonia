# ReportPrintResult

[English](../en/TheOne.Avalonia.Reporting.ReportPrintResult.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportPrintResult.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportPrintResult` · Class · `object`

Submission is accepted by the OS handler/spooler, not confirmation that paper was printed.

[Source / 声明](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DocumentPath`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L11) | `string?` | 可写; CLR / init-only | `null` | Temporary PDF retained for an asynchronous Windows handler, if applicable. |
| [`KeepDocument`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L8) | `bool` | 可写; CLR / init-only | `required constructor argument` | 打印处理结束后是否保留生成文档；由结果接收方遵循释放契约。 |
| [`Message`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L8) | `string` | 可写; CLR / init-only | `required constructor argument` | 可供用户或诊断展示的结果说明；操作结果仍以状态字段为准。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportPrintResult.ReportPrintResult(string Message, bool KeepDocument)`](../../../TheOne.Avalonia/Reporting/PrintTool.cs#L8) | Submission is accepted by the OS handler/spooler, not confirmation that paper was printed. |
