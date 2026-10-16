# Upload

[English](../en/TheOne.Avalonia.Controls.Upload.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Upload.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Upload` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Single-file selection and preview, with optional HTTP upload or a host command.

[Source / 声明](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L10)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ActionsTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L59) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`AllowedExtensions`](../../../TheOne.Avalonia/Controls/Upload.cs#L55) | `string` | 可写; OneWay | `".png,.jpg,.jpeg,.webp,.bmp,.gif"` | 以逗号、分号或空格分隔的扩展名，默认为 PNG、JPEG、WebP、BMP、GIF。空字符串接受任意扩展名。显式允许的非图片文件使用文件图标占位，仍可上传。 |
| [`Description`](../../../TheOne.Avalonia/Controls/Upload.cs#L50) | `string` | 可写; OneWay | `"PNG、JPG、WebP 等图片 · 单文件"` | 可本地化文案。 |
| [`EmptyTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Upload.cs#L65) | `string?` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`FilePicker`](../../../TheOne.Avalonia/Controls/Upload.cs#L45) | `TheOne.Avalonia.Controls.IUploadFilePicker?` | 可写; OneWay | `null` | 可选的 `IUploadFilePicker` 服务。`PickAsync` 返回一个 `UploadFile`，取消则返回 null。 |
| [`FileTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L58) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HasFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L61) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`HasPreview`](../../../TheOne.Avalonia/Controls/Upload.cs#L62) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L24) | `System.Net.Http.HttpClient?` | 可写; OneWay | `null` | 内置上传可使用的消费端客户端。 |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Upload.cs#L46) | `bool` | 可写; OneWay | `false` | 上传处理中的状态；由异步上传过程维护，业务提交应尊重此状态。 |
| [`IsPreviewLoading`](../../../TheOne.Avalonia/Controls/Upload.cs#L63) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`IsSelecting`](../../../TheOne.Avalonia/Controls/Upload.cs#L64) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L25) | `bool` | 只读; OneWay | `computed / state` | 内置请求执行中的只读状态。 |
| [`MaxFileSize`](../../../TheOne.Avalonia/Controls/Upload.cs#L56) | `long` | 可写; OneWay | `10 * 1024 * 1024` | 默认 10 MiB，零关闭声明大小限制。预览读取另有 64 MiB 上限；缺少大小元数据时仍校验实际读取字节。 校验：`value => value >= 0` |
| [`PreviewImage`](../../../TheOne.Avalonia/Controls/Upload.cs#L60) | `Avalonia.Media.IImage?` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`Progress`](../../../TheOne.Avalonia/Controls/Upload.cs#L47) | `double` | 可写; OneWay | `0` | 完成进度数值；范围／单位由所属上传或甘特模型契约定义，不用作成功标志。 校验：`value => double.IsFinite(value) && value is >= 0 and <= 100` |
| [`RemoveCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L67) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 清除文件、缩略图和错误。 |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/Upload.cs#L53) | `string` | 可写; OneWay | `"移除"` | 可本地化文案。 |
| [`ReplaceText`](../../../TheOne.Avalonia/Controls/Upload.cs#L52) | `string` | 可写; OneWay | `"替换"` | 可本地化文案。 |
| [`RequestType`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L20) | `TheOne.Avalonia.Controls.UploadRequestType` | 可写; OneWay | `UploadRequestType.Post` | HTTP 请求方法，默认为 `Post`，默认单向绑定。 校验：`value => Enum.IsDefined(value)` 可选值：`Post`, `Put`, `Patch` |
| [`RequestUploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L68) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 检查启用、忙碌、预览和校验状态后，执行宿主命令或内置上传。 |
| [`SelectCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L66) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 系统单文件选择器，取消后保留原选中项。 |
| [`SelectText`](../../../TheOne.Avalonia/Controls/Upload.cs#L51) | `string` | 可写; OneWay | `"选择图片"` | 可本地化文案。 |
| [`SelectedFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L43) | `TheOne.Avalonia.Controls.UploadFile?` | 可写; TwoWay | `null` | 当前选中的原始上传文件模型；空值取消当前项选择。 |
| [`StatusText`](../../../TheOne.Avalonia/Controls/Upload.cs#L48) | `string?` | 可写; OneWay | `null` | 当前操作状态的展示文本；业务逻辑使用状态枚举和结果，而不解析此文本。 |
| [`SuccessFileUrl`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L22) | `string?` | 可写; TwoWay | `null` | 解析后的结果 URL 字符串，默认双向绑定。 |
| [`Title`](../../../TheOne.Avalonia/Controls/Upload.cs#L49) | `string` | 可写; OneWay | `"上传图片"` | 可本地化文案。 |
| [`UploadApi`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L19) | `string?` | 可写; TwoWay | `null` | 上传地址字符串，默认双向绑定。 |
| [`UploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L44) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`UploadText`](../../../TheOne.Avalonia/Controls/Upload.cs#L54) | `string` | 可写; OneWay | `"上传文件"` | 可本地化文案。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task Upload.UploadAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L59) | Uploads explicitly. A supplied UploadCommand takes precedence over built-in HTTP. Errors are exposed through ErrorMessage; cancellation and stale responses never publish a URL. |
| [`Task Upload.SelectAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Upload.cs#L151) | Selects one file through the host service or native storage provider. Cancellation preserves the current file. |
| [`void Upload.ReloadPreview()`](../../../TheOne.Avalonia/Controls/Upload.cs#L179) | Reloads the thumbnail from a fresh file stream. The control owns and releases decoded thumbnails. |
| [`Upload.Upload()`](../../../TheOne.Avalonia/Controls/Upload.cs#L77) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string?> Upload.UploadApiProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<UploadRequestType> Upload.RequestTypeProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L13) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> Upload.SuccessFileUrlProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<HttpClient?> Upload.HttpClientProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, bool> Upload.IsUploadingProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<UploadFile?> Upload.SelectedFileProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> Upload.UploadCommandProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IUploadFilePicker?> Upload.FilePickerProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> Upload.IsBusyProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> Upload.ProgressProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> Upload.StatusTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.TitleProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.DescriptionProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.SelectTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.ReplaceTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.RemoveTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.UploadTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> Upload.AllowedExtensionsProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<long> Upload.MaxFileSizeProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> Upload.EmptyTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> Upload.FileTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> Upload.ActionsTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, IImage?> Upload.PreviewImageProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, bool> Upload.HasFileProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, bool> Upload.HasPreviewProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, bool> Upload.IsPreviewLoadingProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, bool> Upload.IsSelectingProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Upload, string?> Upload.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

所有控件可使用 `small` / `normal` / `large` 或继承的 `ControlSizing.Size`。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.IUploadFilePicker`](TheOne.Avalonia.Controls.IUploadFilePicker.md)
- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
- [`TheOne.Avalonia.Controls.UploadHttp`](TheOne.Avalonia.Controls.UploadHttp.md)
- [`TheOne.Avalonia.Controls.UploadHttpResult`](TheOne.Avalonia.Controls.UploadHttpResult.md)
- [`TheOne.Avalonia.Controls.UploadModel`](TheOne.Avalonia.Controls.UploadModel.md)
- [`TheOne.Avalonia.Controls.UploadRequestType`](TheOne.Avalonia.Controls.UploadRequestType.md)
