# OneIconCatalog

[English](../en/TheOne.Avalonia.Controls.OneIconCatalog.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneIconCatalog.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneIconCatalog` · Class · `object`

The built-in glyphs, in stable enum order. Vector paths are authored here on a 24-unit grid.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`All`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L402) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Controls.OneIconInfo>` | 只读; CLR / n/a | `Array.AsReadOnly(Entries.Select(entry => entry.Info).ToArray())` | Read-only metadata for all glyphs, in stable enum order. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static OneIconInfo OneIconCatalog.Get(OneIconKind kind)`](../../../TheOne.Avalonia/Controls/OneIconCatalog.cs#L405) | Gets metadata for a valid enum value. Throws for undefined values. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.OneIconInfo`](TheOne.Avalonia.Controls.OneIconInfo.md)
- [`TheOne.Avalonia.Controls.OneIconKind`](TheOne.Avalonia.Controls.OneIconKind.md)
