# ReportDataNode

[English](../en/TheOne.Avalonia.Reporting.ReportDataNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Reporting.ReportDataNode.md) · [API index](README.md)

`TheOne.Avalonia.Reporting.ReportDataNode` · Class · `TheOne.Avalonia.Reporting.ReportObservable`

[继承的额外成员: ReportObservable](TheOne.Avalonia.Reporting.ReportObservable.md)

A JSON response node, or a field from a consumer supplied typed source.

[Source / 声明](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L6)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L17) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Reporting.ReportDataNode>` | 只读; CLR / n/a | `accessor-defined / state` | 子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。 |
| [`FieldKey`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L18) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 显式注册的数据字段键；不是反射属性路径，必须与数据源字段一致。 |
| [`IsExpanded`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L20) | `bool` | 可写; CLR / n/a | `false` | 当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。 |
| [`IsJson`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L19) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Kind`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L14) | `TheOne.Avalonia.Reporting.ReportDataNodeKind` | 只读; CLR / n/a | `accessor-defined / state` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Object`, `Scalar`, `Collection` |
| [`Name`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L12) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`Pointer`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L13) | `string` | 只读; CLR / n/a | `accessor-defined / state` | JSON 数据的 JSON Pointer 路径；路径结构和根节点含义见数据源章节。 |
| [`Preview`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L16) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`TypeLabel`](../../../TheOne.Avalonia/Reporting/ReportDataNode.cs#L15) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Reporting.ReportDataNodeKind`](TheOne.Avalonia.Reporting.ReportDataNodeKind.md)
