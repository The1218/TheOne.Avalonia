# WorkflowCanvas

[English](../en/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.WorkflowCanvas.md) · [API index](README.md)

`TheOne.Avalonia.Controls.WorkflowCanvas` · Class · `Avalonia.Controls.Control`

WorkflowEditor template part. Graph coordinates are independent of zoom and pan.

[Source / 声明](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Point WorkflowCanvas.ToWorld(Point point)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L71) | 将画布视口坐标换算为文档世界坐标，计入缩放和平移。 |
| [`override void WorkflowCanvas.Render(DrawingContext context)`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L133) | Avalonia 绘制回调；使用提供的 DrawingContext 绘制当前状态，业务代码通常不直接调用。 |
| [`void WorkflowCanvas.CancelGesture()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L276) | 取消当前画布手势并清理拖动／连线预览。 |
| [`WorkflowCanvas.WorkflowCanvas()`](../../../TheOne.Avalonia/Controls/WorkflowCanvas.cs#L31) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
