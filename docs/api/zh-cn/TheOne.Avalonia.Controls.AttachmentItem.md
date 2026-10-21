# AttachmentItem

[English](../en/TheOne.Avalonia.Controls.AttachmentItem.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentItem.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentItem` · Class · `object`

A theme-independent attachment presentation model, without HTTP or business relationship logic.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L31)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanOpen`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L72) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanRemove`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L73) | `bool` | 可写; CLR / n/a | `true` | AttachmentItem 的 CanRemove 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`FileName`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L45) | `string` | 可写; CLR / n/a | `""` | AttachmentItem 的 FileName 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`FileSize`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L51) | `long` | 可写; CLR / n/a | `0 / default` | AttachmentItem 的 FileSize 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`FileUrl`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L46) | `string?` | 可写; CLR / n/a | `null` | AttachmentItem 的 FileUrl 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L67) | `bool` | 可写; CLR / n/a | `false` | AttachmentItem 的 IsUploading 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`SizeText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L56) | `string` | 可写; CLR / n/a | `accessor-defined / state` | AttachmentItem 的 SizeText 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Status`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L74) | `TheOne.Avalonia.Controls.AttachmentStatus` | 可写; CLR / n/a | `0 / default` | 节点业务状态枚举；样式由状态派生，不把样式类作为状态存储。 可选值：`Default`, `Uploading`, `Success`, `Error` |
| [`StatusText`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L57) | `string` | 可写; CLR / n/a | `"待上传"` | 当前操作状态的展示文本；业务逻辑使用状态枚举和结果，而不解析此文本。 |
| [`UploadProgress`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L58) | `double` | 可写; CLR / n/a | `0 / default` | AttachmentItem 的 UploadProgress 配置字段；值类型和初值如表，通过此模型的公开属性访问器读写。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void AttachmentItem.MarkUploading()`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L85) | Starts an upload with unknown progress; no transfer percentage is fabricated. |
| [`void AttachmentItem.MarkUploaded(string fileUrl, long fileSize)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L95) | Records the consumer-provided server result without rewriting its URL. |
| [`void AttachmentItem.MarkFailed(string message)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L109) | Marks failure and permits removing the failed row. |
| [`static AttachmentItem AttachmentItem.CreateUploaded(string fileName, string? fileUrl, long fileSize)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L118) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`static string AttachmentItem.FormatSize(long size)`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L128) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`AttachmentItem.AttachmentItem()`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L31) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event PropertyChangedEventHandler? AttachmentItem.PropertyChanged`](../../../TheOne.Avalonia/Controls/AttachmentItem.cs#L43) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.AttachmentStatus`](TheOne.Avalonia.Controls.AttachmentStatus.md)
