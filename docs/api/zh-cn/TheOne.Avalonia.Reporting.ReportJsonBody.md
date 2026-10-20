# ReportJsonBody

[English](../en/TheOne.Avalonia.Reporting.ReportJsonBody.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportJsonBody.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportJsonBody` · Class · `object`

JSON Pointer identifies the body unambiguously, even when property names contain dots or slashes.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Label`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 显示标签文本；不作为模型身份、字段路径或序列化标识。 |
| [`Pointer`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | JSON 数据的 JSON Pointer 路径；路径结构和根节点含义见数据源章节。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportJsonBody.ReportJsonBody(string Pointer, string Label)`](../../../TheOne.Avalonia/Reporting/ReportJsonData.cs#L7) | JSON Pointer identifies the body unambiguously, even when property names contain dots or slashes. |
