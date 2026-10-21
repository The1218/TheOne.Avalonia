# IAttachmentFilePicker

[English](../en/TheOne.Avalonia.Controls.IAttachmentFilePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IAttachmentFilePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IAttachmentFilePicker` · Interface

Provides a multi-file selection. Ownership of returned files transfers to AttachmentList.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L272)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task<IReadOnlyList<UploadFile>> IAttachmentFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L274) | 异步打开文件选择；由宿主文件选择器实现，取消时按返回集合处理。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
