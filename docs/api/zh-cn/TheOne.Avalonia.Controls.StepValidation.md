# StepValidation

[English](../en/TheOne.Avalonia.Controls.StepValidation.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.StepValidation.md) · [API index](README.md)

`TheOne.Avalonia.Controls.StepValidation` · Class · `object`

Creates attribute validators for concrete bound model types without dynamic binding or assembly-wide preservation.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L25)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static IStepValidator StepValidation.For<T>(T model, params string[] memberNames) where T : class`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L27) | 为指定模型／属性集合创建类型化步骤校验器。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IStepValidator`](TheOne.Avalonia.Controls.IStepValidator.md)
