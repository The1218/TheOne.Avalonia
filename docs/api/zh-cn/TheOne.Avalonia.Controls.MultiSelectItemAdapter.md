# MultiSelectItemAdapter

[English](../en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectItemAdapter` · Class · `object`

Explicit, trimming-safe text and availability accessors for model-backed choices.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`abstract string MultiSelectItemAdapter.GetText(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L10) | 使用已配置的类型化适配器读取原始模型显示／搜索文本。 |
| [`virtual bool MultiSelectItemAdapter.IsEnabled(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L12) | 使用适配器判断原始模型能否参与用户选择。 |
| [`static MultiSelectItemAdapter MultiSelectItemAdapter.Create<T>(Func<T, string> text, Func<T, bool>? isEnabled = null)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L17) | 通过显式类型参数、模型及委托创建适配器／数据源；签名中的 getter 保留编译类型信息。 |
