# IAttachmentItem

[English](../en/TheOne.Avalonia.Controls.IAttachmentItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IAttachmentItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IAttachmentItem` · Interface

Read-only presentation contract. Lists forward the original item to consumer commands.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanOpen`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L25) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L26) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`FileName`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L18) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`FileSize`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L20) | `long` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`FileUrl`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L19) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L24) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SizeText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L21) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Status`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L27) | `TheOne.Avalonia.Controls.AttachmentStatus` | 只读; CLR / n/a | `accessor-defined / state` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 可选值：`Default`, `Uploading`, `Success`, `Error` |
| [`StatusText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L22) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 当前操作状态的展示文本；业务逻辑使用状态枚举和结果，而不解析此文本。 |
| [`UploadProgress`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L23) | `double` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.AttachmentStatus`](TheOne.Avalonia.Controls.AttachmentStatus.md)
