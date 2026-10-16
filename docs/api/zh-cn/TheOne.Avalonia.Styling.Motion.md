# Motion

[English](../en/TheOne.Avalonia.Styling.Motion.md) · [简体中文](../zh-cn/TheOne.Avalonia.Styling.Motion.md) · [API index](README.md)

`TheOne.Avalonia.Styling.Motion` · Class · `Avalonia.AvaloniaObject`

Set Enabled=False on a subtree to suppress theme motion.

[Source / 声明](../../../TheOne.Avalonia/Styling/Motion.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool Motion.GetEnabled(Control control)`](../../../TheOne.Avalonia/Styling/Motion.cs#L15) | 读取附加属性。向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |
| [`static void Motion.SetEnabled(Control control, bool value)`](../../../TheOne.Avalonia/Styling/Motion.cs#L16) | 设置附加属性。向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |
| [`Motion.Motion()`](../../../TheOne.Avalonia/Styling/Motion.cs#L7) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> Motion.EnabledProperty`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
