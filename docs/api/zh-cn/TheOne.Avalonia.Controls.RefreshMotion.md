# RefreshMotion

[English](../en/TheOne.Avalonia.Controls.RefreshMotion.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.RefreshMotion.md) · [API index](README.md)

`TheOne.Avalonia.Controls.RefreshMotion` · Class · `Avalonia.AvaloniaObject`

Applies Motion.Enabled to the native refresh compositor without changing refresh state or input.

[Source / 声明](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L18)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`UseMotionSettings`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L20) | `bool` | 可写; OneWay; 附加属性 | `false` | 使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool RefreshMotion.GetUseMotionSettings(RefreshVisualizer control)`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L31) | 读取附加属性。使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |
| [`static void RefreshMotion.SetUseMotionSettings(RefreshVisualizer control, bool value)`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L36) | 设置附加属性。使用库的共享动效配置；全局关闭或目标控件关闭时不强制播放。 |
| [`RefreshMotion.RefreshMotion()`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L18) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> RefreshMotion.UseMotionSettingsProperty`](../../../TheOne.Avalonia/Controls/RefreshMotion.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
