# UploadFile

[English](../en/TheOne.Avalonia.Controls.UploadFile.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.UploadFile.md) · [API index](README.md)

`TheOne.Avalonia.Controls.UploadFile` · Class · `object`

An immutable single-file selection. Every read returns a new stream owned by the caller.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadFile.cs#L7)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L16) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Name`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L14) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`Size`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L15) | `long?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SizeText`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L17) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task<Stream> UploadFile.OpenReadAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L27) | 异步打开上传文件的只读流；调用方在使用后释放流。 |
| [`static Task<UploadFile> UploadFile.FromStorageFileAsync(IStorageFile file, CancellationToken cancellationToken = default(CancellationToken), bool ownsStorageFile = false)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L42) | The storage file must remain usable for as long as this selection is used. |
| [`void UploadFile.Dispose()`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L62) | Rejects new reads and releases an owned storage source after all open streams close. |
| [`UploadFile.UploadFile(string name, Func<CancellationToken, Task<Stream>> openRead, long? size = null, string? contentType = null)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L19) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |
