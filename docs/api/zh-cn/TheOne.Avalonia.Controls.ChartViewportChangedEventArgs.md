# ChartViewportChangedEventArgs

[English](../en/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ChartViewportChangedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ChartViewportChangedEventArgs` · Class · `System.EventArgs`

[Source / 声明](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`NewViewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L19) | `Avalonia.Rect` | 只读; CLR / n/a | `newViewport` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`OldViewport`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L18) | `Avalonia.Rect` | 只读; CLR / n/a | `oldViewport` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`ChartViewportChangedEventArgs.ChartViewportChangedEventArgs(Rect oldViewport, Rect newViewport)`](../../../TheOne.Avalonia/Controls/ChartBase.Viewport.cs#L16) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
