# ReportLayoutElement

[English](../en/TheOne.Avalonia.Reporting.ReportLayoutElement.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportLayoutElement.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportLayoutElement` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Bold`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `bool` | 可写; CLR / init-only | `required constructor argument` | 文本使用粗体／半粗体绘制；换行与布局仍按实际字体度量。 |
| [`Fill`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | 可写; CLR / init-only | `required constructor argument` | 报表元素填充颜色字符串；Transparent 表示透明，须为有效 Avalonia 颜色。 |
| [`FontSize`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | 可写; CLR / init-only | `required constructor argument` | 报表文本字号，单位 DIP；文档和单元素范围校验由分页器处理。 |
| [`Foreground`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | 可写; CLR / init-only | `required constructor argument` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`Height`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | 可写; CLR / init-only | `required constructor argument` | 高度配置；保留内容、字号及数据坐标契约，固定值可能覆盖主题密度。 |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `TheOne.Avalonia.Reporting.ReportElementKind` | 可写; CLR / init-only | `required constructor argument` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Text`, `Table`, `Line`, `Rectangle`, `PageNumber`, `Image`, `CheckBox`, `Barcode` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | 可写; CLR / init-only | `required constructor argument` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`ShowBorder`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `bool` | 可写; CLR / init-only | `required constructor argument` | 显示报表元素边缘线；不改变元素几何尺寸。 |
| [`Text`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `string` | 可写; CLR / init-only | `required constructor argument` | 文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。 |
| [`Width`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | 可写; CLR / init-only | `required constructor argument` | 宽度配置；物理单位或布局单位遵循所属模型说明，不能据此改变数据坐标含义。 |
| [`X`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | 可写; CLR / init-only | `required constructor argument` | 模型横向坐标／数值；按所属图表、流程或报表坐标系统解释。 |
| [`Y`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | `double` | 可写; CLR / init-only | `required constructor argument` | 模型纵向坐标／数值；按所属图表、流程或报表坐标系统解释。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ReportLayoutElement.ReportLayoutElement(string Name, ReportElementKind Kind, double X, double Y, double Width, double Height, string Text, double FontSize, bool Bold, string Foreground, string Fill, bool ShowBorder)`](../../../TheOne.Avalonia/Reporting/ReportPaginator.cs#L12) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportElementKind`](TheOne.Avalonia.Reporting.ReportElementKind.md)
