# DataGridColumnOptions

[English](../en/TheOne.Avalonia.Controls.DataGridColumnOptions.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridColumnOptions` · Class · `Avalonia.AvaloniaObject`

Explicit model fields and filter conditions for the official DataGrid column types.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FilterCondition`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L21) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | 可写; TwoWay; 附加属性 | `null` | DataGridFilterCondition?／null ; 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| [`FilterEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | `bool` | 可写; OneWay; 附加属性 | `true` | bool／true ; 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| [`FilterError`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L25) | `string?` | 只读; OneWay; 附加属性 | `null` | string?／null ; 阶段 2 的只读查询属性；条件／字段错误说明，使用 GetFilterError 查询 |
| [`FilterKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | `TheOne.Avalonia.Controls.DataGridFilterKind` | 可写; OneWay; 附加属性 | `0 / default` | DataGridFilterKind／Auto ; `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 可选值：`Auto`, `Text`, `Number`, `None` |
| [`SummaryEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L28) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 显式启用该列，限数值或可空数值属性 |
| [`SummaryFormat`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L35) | `string?` | 可写; OneWay; 附加属性 | `null` | string?／null ; .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |
| [`SummaryKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L31) | `TheOne.Avalonia.Controls.DataGridSummaryKind` | 可写; OneWay; 附加属性 | `0 / default` | DataGridSummaryKind／Sum ; Sum、Average、Min、Max、Count，每列一个 校验：`Enum.IsDefined` 可选值：`Sum`, `Average`, `Min`, `Max`, `Count` |
| [`ValueAccessor`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | `TheOne.Avalonia.Controls.IDataGridValueAccessor?` | 可写; OneWay; 附加属性 | `null` | IDataGridValueAccessor?／null ; 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool DataGridColumnOptions.GetSummaryEnabled(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L50) | 读取附加属性。bool／false ; 显式启用该列，限数值或可空数值属性 |
| [`static void DataGridColumnOptions.SetSummaryEnabled(DataGridColumn column, bool value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L55) | 设置附加属性。bool／false ; 显式启用该列，限数值或可空数值属性 |
| [`static DataGridSummaryKind DataGridColumnOptions.GetSummaryKind(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L60) | 读取附加属性。DataGridSummaryKind／Sum ; Sum、Average、Min、Max、Count，每列一个 |
| [`static void DataGridColumnOptions.SetSummaryKind(DataGridColumn column, DataGridSummaryKind value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L65) | 设置附加属性。DataGridSummaryKind／Sum ; Sum、Average、Min、Max、Count，每列一个 |
| [`static string? DataGridColumnOptions.GetSummaryFormat(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L70) | 读取附加属性。string?／null ; .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |
| [`static void DataGridColumnOptions.SetSummaryFormat(DataGridColumn column, string? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L75) | 设置附加属性。string?／null ; .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |
| [`static IDataGridValueAccessor? DataGridColumnOptions.GetValueAccessor(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L87) | 读取附加属性。IDataGridValueAccessor?／null ; 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |
| [`static void DataGridColumnOptions.SetValueAccessor(DataGridColumn column, IDataGridValueAccessor? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L92) | 设置附加属性。IDataGridValueAccessor?／null ; 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |
| [`static bool DataGridColumnOptions.GetFilterEnabled(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L97) | 读取附加属性。bool／true ; 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| [`static void DataGridColumnOptions.SetFilterEnabled(DataGridColumn column, bool value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L102) | 设置附加属性。bool／true ; 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| [`static DataGridFilterKind DataGridColumnOptions.GetFilterKind(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L107) | 读取附加属性。DataGridFilterKind／Auto ; `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 |
| [`static void DataGridColumnOptions.SetFilterKind(DataGridColumn column, DataGridFilterKind value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L112) | 设置附加属性。DataGridFilterKind／Auto ; `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 |
| [`static DataGridFilterCondition? DataGridColumnOptions.GetFilterCondition(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L117) | 读取附加属性。DataGridFilterCondition?／null ; 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| [`static void DataGridColumnOptions.SetFilterCondition(DataGridColumn column, DataGridFilterCondition? value)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L122) | 设置附加属性。DataGridFilterCondition?／null ; 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| [`static string? DataGridColumnOptions.GetFilterError(DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L127) | 读取附加属性。string?／null ; 阶段 2 的只读查询属性；条件／字段错误说明，使用 GetFilterError 查询 |
| [`DataGridColumnOptions.DataGridColumnOptions()`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L9) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<IDataGridValueAccessor?> DataGridColumnOptions.ValueAccessorProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> DataGridColumnOptions.FilterEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<DataGridFilterKind> DataGridColumnOptions.FilterKindProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<DataGridFilterCondition?> DataGridColumnOptions.FilterConditionProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<string?> DataGridColumnOptions.FilterErrorProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> DataGridColumnOptions.SummaryEnabledProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<DataGridSummaryKind> DataGridColumnOptions.SummaryKindProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<string?> DataGridColumnOptions.SummaryFormatProperty`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
- [`TheOne.Avalonia.Controls.DataGridFilterKind`](TheOne.Avalonia.Controls.DataGridFilterKind.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryKind`](TheOne.Avalonia.Controls.DataGridSummaryKind.md)
- [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](TheOne.Avalonia.Controls.IDataGridValueAccessor.md)
