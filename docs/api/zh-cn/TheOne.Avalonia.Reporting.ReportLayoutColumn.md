# ReportLayoutColumn

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutColumn.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutColumn` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 显式注册的数据字段键；不是反射属性路径，必须与数据源字段一致。 |
| [`Header`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `string` | 可写; CLR / init-only | `required constructor argument` | 标题区内容，可为文本或模型；自定义显示由 HeaderTemplate 负责。 |
| [`IsNumeric`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `bool` | 可写; CLR / init-only | `required constructor argument` | 此布局列是否按数值字段处理；影响对齐与汇总格式。 |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | `double` | 可写; CLR / init-only | `required constructor argument` | 宽度配置；物理单位或布局单位遵循所属模型说明，不能据此改变数据坐标含义。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportLayoutColumn.ReportLayoutColumn(string Header, string FieldKey, double Width, bool IsNumeric)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
