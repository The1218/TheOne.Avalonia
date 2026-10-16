# WorkflowNode

[English](../en/TheOne.Avalonia.Workflow.WorkflowNode.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowNode.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowNode` · Class · `TheOne.Avalonia.Workflow.WorkflowObservable`

[继承的额外成员: WorkflowObservable](TheOne.Avalonia.Workflow.WorkflowObservable.md)

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L47)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L67) | `object?` | 可写; CLR / n/a | `null` | Optional host presentation data, never serialized. Restore it from TypeId after loading. |
| [`Description`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L61) | `string` | 可写; CLR / n/a | `""` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`Id`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L55) | `string` | 可写; CLR / init-only | `Guid.NewGuid().ToString("N")` | 模型稳定标识；持久化／引用使用，避免在同一文档中重复。 |
| [`IsAnnotation`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L58) | `bool` | 可写; CLR / init-only | `false` | 该节点仅为注释展示，不作为可执行业务步骤。 |
| [`Ports`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L68) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowPort>` | 只读; CLR / n/a | `[]` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`Properties`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L69) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Workflow.WorkflowProperty>` | 只读; CLR / n/a | `[]` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`Shape`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L57) | `TheOne.Avalonia.Workflow.WorkflowNodeShape` | 可写; CLR / init-only | `0 / default` | 节点外观枚举，Card、Pill 或 Note；不改变节点的 TypeId。 可选值：`Card`, `Pill`, `Note` |
| [`State`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L64) | `TheOne.Avalonia.Workflow.WorkflowNodeState` | 可写; CLR / n/a | `0 / default` | 模型的状态枚举；可选值及持久化含义见对应枚举和所属控件契约。 可选值：`Pending`, `Running`, `Completed`, `Failed`, `Skipped` |
| [`StateText`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L65) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Title`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L60) | `string` | 可写; CLR / n/a | `""` | 显示标题；不等于业务主键或本地化资源键。 |
| [`TypeId`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L56) | `string` | 可写; CLR / init-only | `"task"` | 宿主节点定义的稳定类型键；加载后用它恢复定义和非序列化内容，不进行 CLR 类型查找。 |
| [`Width`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L59) | `double` | 可写; CLR / n/a | `180` | 宽度配置；物理单位或布局单位遵循所属模型说明，不能据此改变数据坐标含义。 |
| [`X`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L62) | `double` | 可写; CLR / n/a | `0 / default` | 模型横向坐标／数值；按所属图表、流程或报表坐标系统解释。 |
| [`Y`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L63) | `double` | 可写; CLR / n/a | `0 / default` | 模型纵向坐标／数值；按所属图表、流程或报表坐标系统解释。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`WorkflowNode.WorkflowNode()`](../../../TheOne.Avalonia/Workflow/WorkflowModels.cs#L70) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Workflow.WorkflowNodeShape`](TheOne.Avalonia.Workflow.WorkflowNodeShape.md)
- [`TheOne.Avalonia.Workflow.WorkflowNodeState`](TheOne.Avalonia.Workflow.WorkflowNodeState.md)
- [`TheOne.Avalonia.Workflow.WorkflowPort`](TheOne.Avalonia.Workflow.WorkflowPort.md)
- [`TheOne.Avalonia.Workflow.WorkflowProperty`](TheOne.Avalonia.Workflow.WorkflowProperty.md)
