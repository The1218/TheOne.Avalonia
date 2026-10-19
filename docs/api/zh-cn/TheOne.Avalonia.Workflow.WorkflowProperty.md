# WorkflowProperty

[English](../en/TheOne.Avalonia.Workflow.WorkflowProperty.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowProperty.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowProperty` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

A literal or an upstream reference. DataType is a host-defined token, e.g. text, number, email.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L73)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`DataType`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L86) | `string` | 可写; CLR / init-only | `"text"` | 宿主定义的类型键，例如 flow、text、number；用于端口／引用兼容判断，不是 CLR 类型名。 |
| [`DisplayValue`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L108) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsRequired`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L88) | `bool` | 可写; CLR / init-only | `false` | 字段／流程属性是否必填；校验器按所属模型契约生成错误。 |
| [`Key`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L84) | `string` | 可写; CLR / init-only | `""` | 字段／参数键，用于关联业务值；与显示名称分开维护。 |
| [`Kind`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L87) | `TheOne.Avalonia.Workflow.WorkflowValueKind` | 可写; CLR / init-only | `0 / default` | 所属字段、属性或元素的种类枚举；具体可选值见该枚举定义。 可选值：`Text`, `Number`, `Boolean`, `MultiSelect` |
| [`Reference`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L99) | `TheOne.Avalonia.Workflow.WorkflowDataReference?` | 可写; CLR / n/a | `null` | 指向上游输出的结构化引用；空值表示使用固定值，不保存解析后的运行数据。 |
| [`SelectedKeys`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L81) | `System.Collections.ObjectModel.ObservableCollection<string>` | 只读; CLR / n/a | `[]` | Stable host identities for MultiSelect fields; display labels and runtime objects are not saved. |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L85) | `string` | 可写; CLR / init-only | `""` | 显示标题；不等于业务主键或本地化资源键。 |
| [`Value`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L90) | `string` | 可写; CLR / n/a | `""` | 当前值；单位、范围及空值含义以所属模型和校验条件为准。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowProperty.WorkflowProperty()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L75) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowDataReference`](TheOne.Avalonia.Workflow.WorkflowDataReference.md)
- [`TheOne.Avalonia.Workflow.WorkflowValueKind`](TheOne.Avalonia.Workflow.WorkflowValueKind.md)
