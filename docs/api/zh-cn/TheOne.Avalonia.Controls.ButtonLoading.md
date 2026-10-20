# ButtonLoading

[English](../en/TheOne.Avalonia.Controls.ButtonLoading.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ButtonLoading` · Class · `Avalonia.AvaloniaObject`

Loading state for official Button controls; the temporary override preserves IsEnabled bindings.

[Source / 声明](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool ButtonLoading.GetIsLoading(Button button)`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L23) | 读取附加属性。显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`static void ButtonLoading.SetIsLoading(Button button, bool value)`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L28) | 设置附加属性。显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`ButtonLoading.ButtonLoading()`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L10) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> ButtonLoading.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
