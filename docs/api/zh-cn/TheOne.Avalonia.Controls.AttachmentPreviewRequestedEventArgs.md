# AttachmentPreviewRequestedEventArgs

[English](../en/TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs` · Class · `Avalonia.Interactivity.RoutedEventArgs`

Forwards the original attachment for host-controlled preview or download.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L278)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Item`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L280) | `TheOne.Avalonia.Controls.IAttachmentItem` | 只读; CLR / n/a | `item ?? throw new ArgumentNullException(nameof(item))` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`AttachmentPreviewRequestedEventArgs.AttachmentPreviewRequestedEventArgs(IAttachmentItem item)`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L278) | Forwards the original attachment for host-controlled preview or download. |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IAttachmentItem`](TheOne.Avalonia.Controls.IAttachmentItem.md)
