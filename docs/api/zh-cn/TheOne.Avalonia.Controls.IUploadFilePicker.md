# IUploadFilePicker

[English](../en/TheOne.Avalonia.Controls.IUploadFilePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IUploadFilePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IUploadFilePicker` · Interface

Optional host picker service. Results transfer to the consumer if accepted; rejected results are disposed. Null preserves selection.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadFile.cs#L106)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task<UploadFile?> IUploadFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L108) | 异步打开文件选择；由宿主文件选择器实现，取消时按返回集合处理。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
