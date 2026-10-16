# IStepValidator

[English](../en/TheOne.Avalonia.Controls.IStepValidator.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IStepValidator.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IStepValidator` · Interface

Validates one step. Implement this for asynchronous or service-backed validation.

[Source / 声明](../../../TheOne.Avalonia/Controls/StepValidation.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task<IReadOnlyList<StepValidationError>> IStepValidator.ValidateAsync(CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/StepValidation.cs#L18) | 校验模型规则与原生输入转换错误；失败、取消、未挂载、禁用或忙碌时返回 `false` |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.StepValidationError`](TheOne.Avalonia.Controls.StepValidationError.md)
