# WorkflowMiniMap

[English](../en/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowMiniMap.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowMiniMap` · Class · `Avalonia.Controls.Control`

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Editor`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L13) | `TheOne.Avalonia.Controls.WorkflowEditor?` | 可写; OneWay | `null` | 缩略图关联的 WorkflowEditor；用于共享文档和视口，不创建独立工作流状态。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`override void WorkflowMiniMap.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L28) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`WorkflowMiniMap.WorkflowMiniMap()`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L11) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<WorkflowEditor?> WorkflowMiniMap.EditorProperty`](../../../TheOne.Avalonia/Controls/WorkflowMiniMap.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `large` | [`:is(Control).large`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L37) | :is(Control).large => styling:ControlSizing.Size=Large |
| `normal` | [`:is(Control).normal`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L34) | :is(Control).normal => styling:ControlSizing.Size=Normal |
| `small` | [`:is(Control).small`](../../../TheOne.Avalonia/Themes/Sizing.axaml#L31) | :is(Control).small => styling:ControlSizing.Size=Small |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.WorkflowEditor`](TheOne.Avalonia.Controls.WorkflowEditor.md)
