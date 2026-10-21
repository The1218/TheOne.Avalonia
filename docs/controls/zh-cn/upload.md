# Upload 上传

一次选择多个附件并展示文件集合请使用 [AttachmentList 附件列表](attachment-list.md)。本控件保持单文件契约。

`TheOne.Avalonia.Controls.Upload` 选择一张图片并显示缩略图，内置默认模板、系统文件选择器和可选的单文件 HTTP 上传。配置 `UploadApi` 可使用内置上传，也可提供 `UploadCommand` 接入业务上传逻辑。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### Upload

![Upload — 单图选择的空态与宿主命令入口；截图没有发出网络上传。](../images/Upload-light.png)

单图选择的空态与宿主命令入口；截图没有发出网络上传。

[查看 Dark 对照](../images/Upload-dark.png)

<!-- visual-guide:end -->

## 内置 API 上传

```xml
<one:Upload SelectedFile="{Binding CoverFile, Mode=TwoWay}"
            UploadApi="{Binding UploadEndpoint, Mode=TwoWay}"
            RequestType="{Binding UploadMethod}"
            SuccessFileUrl="{Binding CoverUrl, Mode=TwoWay}" />
```

`UploadApi` 是默认双向绑定的字符串属性，应填写 HTTP(S) 绝对地址。`RequestType` 支持 `UploadRequestType.Post`（默认）、`Put` 和 `Patch`。修改请求方式不会改变请求体格式：内置上传采用 `multipart/form-data`，字段名固定为：

| 字段 | 内容 |
| --- | --- |
| `filename` | 文件名文本。 |
| `filedata` | 文件流的二进制内容，附带文件名和可选 MIME 类型。 |

内置 `UploadModel` 包含 `FileName`、`FileData`（`Stream`）和可选的 `ContentType`。发送的是文件字节，并非 JSON 对象或 Base64 字符串。服务端接口需要接受这套 multipart 契约；`RequestType` 设置 HTTP 方法，不切换 JSON/表单编码方式。字段名、请求结构或响应规则不同的接口可通过 `UploadCommand` 接入。

直接调用 `UploadHttp.SendAsync` 时，请求从 `FileData` 的当前位置读取，并在请求结束时释放该流；客户端仍归调用方所有。成功结果 `UploadHttpResult` 提供状态码、响应文本和可空的文件 URL。响应体上限为 1 MiB，超过上限会报错。

选择文件不会自动发请求。默认上传按钮执行 `RequestUploadCommand`：有宿主 `UploadCommand` 时优先执行该命令，否则调用内置 `UploadAsync()`。宿主命令仍遵循 `CanExecute`。内置上传通过 `IsUploading`、`Progress`、`StatusText` 和 `ErrorMessage` 提供状态，执行中禁用冲突操作，不改写宿主的 `IsBusy`。失败后可重试。文件、地址、请求方式、HTTP 客户端或宿主命令变化时，会取消旧请求并清空结果，过期响应不能写入新文件的 URL。禁用或移出可视树也会取消正在执行的请求；调用方可通过 `UploadAsync(CancellationToken)` 主动取消。

HTTP 请求成功后，从响应体中查找路径以允许文件后缀结尾的 HTTP(S) 绝对 URL，支持嵌套 JSON 字符串值和纯文本 URL，不限定 `url` 等字段名。查询参数与片段不影响后缀判断，例如 `https://cdn.example.com/covers/a.PNG?token=123#preview` 可匹配 `.png`。相对路径、`data:`/`file:` 地址及没有有效文件后缀的 URL 均不接受。接受后缀由 `AllowedExtensions` 控制；其为空时，点号后的后缀仍须为 1–16 个 ASCII 字母或数字，不对照内置文件类型列表筛选。存在多个合法结果时取第一个。消费端也可通过 `UploadHttp.ExtractFileUrl` 使用同一解析规则。

`SuccessFileUrl` 为双向绑定的字符串属性，合法响应会回写 ViewModel，供后续保存或展示。仅 HTTP 2xx 不代表上传完成：响应没有合法文件 URL 时会明确报错，不产生成功地址。这里校验 URL 格式和文件后缀，不访问结果 URL，也不验证远程资源是否真实存在。

可通过 `HttpClient` 属性注入消费端拥有的客户端，配置认证请求头、消息处理器、超时或测试传输；控件不会释放注入的客户端。默认客户端不携带应用认证。PreviewHost 的“内置 API 上传”示例默认接口为空，使用独立的文件选择，支持切换请求方式和复制结果 URL。填写真实接口并点击上传后，才会把文件发送到该接口；另一个“宿主命令”示例仅在本地读取文件字节。

## 宿主命令接入

```xml
<one:Upload SelectedFile="{Binding CoverFile}"
            UploadCommand="{Binding UploadCoverCommand}"
            IsBusy="{Binding IsUploading}"
            Progress="{Binding UploadProgress}"
            StatusText="{Binding UploadStatus}"
            MaxFileSize="10485760" />
```

`SelectedFile` 默认双向绑定，类型为不可变的 `UploadFile`。`UploadCommand` 接收选中时的同一个文件对象作为参数。通过 `await file.OpenReadAsync(cancellationToken)` 获取内容，并释放返回的流；每次调用都会打开一个新流。业务命令应使用收到的参数，不要在异步操作中重新读取可能已经变化的选中项。文件不需要拥有本地路径。

`UploadFile` 的构造参数为名称、打开流的委托，以及可选字节大小、MIME 类型。`UploadFile.FromStorageFileAsync` 可适配 Avalonia 的 `IStorageFile`；`ownsStorageFile: true` 将源文件所有权交给包装对象，默认 false 则仍归调用方所有。系统选择器采用 true。接受后的选择结果通过 `SelectedFile` 转交消费端：宿主应在替换、移除后释放旧选中项，并在页面或模型结束使用时释放当前项。自定义选择器中被拒绝或取消后迟到的结果由控件释放。独立消费端不应共享同一对象的释放所有权。

`UploadFile.Dispose()` 阻止新读取，等现有流租约关闭后再释放其拥有的存储文件。控件只释放自己解码的缩略图和读取流，不释放已接受或外部绑定的文件对象。PreviewHost 模型演示在选中项绑定变化后释放旧文件。

`UploadCommand` 优先于 `UploadApi`，其 `CanExecute` 控制上传按钮。宿主通过 `IsBusy`、`Progress`（0–100）和 `StatusText` 提供执行状态，并负责服务端结果映射和重试策略。执行中或禁用时，不允许选择、替换、移除、重复提交。选择图片不会自动上传。PreviewHost 的宿主命令示例只读取内置图片或选中文件的流并显示字节数，不请求网络。

## 默认行为与自定义

| 成员 | 行为 |
| --- | --- |
| `SelectCommand` / `SelectAsync` | 系统单文件选择器，取消后保留原选中项。 |
| `RemoveCommand` | 清除文件、缩略图和错误。 |
| `RequestUploadCommand` / `UploadAsync` | 检查启用、忙碌、预览和校验状态后，执行宿主命令或内置上传。 |
| `UploadApi` | 上传地址字符串，默认双向绑定。 |
| `RequestType` | HTTP 请求方法，默认为 `Post`，默认单向绑定。 |
| `SuccessFileUrl` | 解析后的结果 URL 字符串，默认双向绑定。 |
| `HttpClient` | 内置上传可使用的消费端客户端。 |
| `IsUploading` | 内置请求执行中的只读状态。 |
| `FilePicker` | 可选的 `IUploadFilePicker` 服务。`PickAsync` 返回一个 `UploadFile`，取消则返回 null。 |
| `AllowedExtensions` | 以逗号、分号或空格分隔的扩展名，默认为 PNG、JPEG、WebP、BMP、GIF。空字符串接受任意扩展名。显式允许的非图片文件使用文件图标占位，仍可上传。 |
| `MaxFileSize` | 默认 10 MiB，零关闭声明大小限制。预览读取另有 64 MiB 上限；缺少大小元数据时仍校验实际读取字节。 |
| `PreviewImage`、`HasFile`、`HasPreview`、`IsPreviewLoading`、`IsSelecting`、`ErrorMessage` | 可绑定的只读展示状态。 |
| `Title`、`Description`、`SelectText`、`ReplaceText`、`RemoveText`、`UploadText` | 可本地化文案。 |

默认界面继承 Small / Normal / Large 密度，复用 Light / Dark 的 `One.*` 资源。可分别替换 `EmptyTemplate`、`FileTemplate`、`ActionsTemplate`。三个模板的数据上下文均为 `Upload`，可直接绑定文件、预览和命令：

```xml
<one:Upload.FileTemplate>
  <DataTemplate x:DataType="one:Upload">
    <StackPanel Spacing="8">
      <Image Source="{Binding PreviewImage}" Height="120" Stretch="Uniform" />
      <TextBlock Text="{Binding SelectedFile.Name}" />
    </StackPanel>
  </DataTemplate>
</one:Upload.FileTemplate>
```

选择器返回不支持类型或超限文件时，保留旧文件并报告错误。直接绑定损坏或不可读图片时，保留文件信息，显示错误并禁用上传；可替换、移除或调用 `ReloadPreview` 重试。替换、清除、移出可视树会取消旧预览，过期的文件选择和解码结果不会覆盖新数据。重新加入可视树会加载保留的文件。缩略图采用解码后的首帧，不负责动画播放或多文件列表。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### Upload

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ActionsTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L202) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`AllowedExtensions`](../../../TheOne.Avalonia/Controls/Upload.cs#L178) | `string` | 可写; OneWay | `".png,.jpg,.jpeg,.webp,.bmp,.gif"` | 以逗号、分号或空格分隔的扩展名，默认为 PNG、JPEG、WebP、BMP、GIF。空字符串接受任意扩展名。显式允许的非图片文件使用文件图标占位，仍可上传。 |
| [`Description`](../../../TheOne.Avalonia/Controls/Upload.cs#L148) | `string` | 可写; OneWay | `"PNG、JPG、WebP 等图片 · 单文件"` | 可本地化文案。 |
| [`EmptyTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L190) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Upload.cs#L213) | `string?` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`FilePicker`](../../../TheOne.Avalonia/Controls/Upload.cs#L118) | `TheOne.Avalonia.Controls.IUploadFilePicker?` | 可写; OneWay | `null` | 可选的 `IUploadFilePicker` 服务。`PickAsync` 返回一个 `UploadFile`，取消则返回 null。 |
| [`FileTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L196) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HasFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L209) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`HasPreview`](../../../TheOne.Avalonia/Controls/Upload.cs#L210) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L56) | `System.Net.Http.HttpClient?` | 可写; OneWay | `null` | 内置上传可使用的消费端客户端。 |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Upload.cs#L124) | `bool` | 可写; OneWay | `false` | 上传处理中的状态；由异步上传过程维护，业务提交应尊重此状态。 |
| [`IsPreviewLoading`](../../../TheOne.Avalonia/Controls/Upload.cs#L211) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`IsSelecting`](../../../TheOne.Avalonia/Controls/Upload.cs#L212) | `bool` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L62) | `bool` | 只读; OneWay | `computed / state` | 内置请求执行中的只读状态。 |
| [`MaxFileSize`](../../../TheOne.Avalonia/Controls/Upload.cs#L184) | `long` | 可写; OneWay | `10 * 1024 * 1024` | 默认 10 MiB，零关闭声明大小限制。预览读取另有 64 MiB 上限；缺少大小元数据时仍校验实际读取字节。 校验：`value => value >= 0` |
| [`PreviewImage`](../../../TheOne.Avalonia/Controls/Upload.cs#L208) | `Avalonia.Media.IImage?` | 只读; OneWay | `computed / state` | 可绑定的只读展示状态。 |
| [`Progress`](../../../TheOne.Avalonia/Controls/Upload.cs#L130) | `double` | 可写; OneWay | `0 / default` | 完成进度数值；范围／单位由所属上传或甘特模型契约定义，不用作成功标志。 校验：`value => double.IsFinite(value) && value is >= 0 and <= 100` |
| [`RemoveCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L215) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 清除文件、缩略图和错误。 |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/Upload.cs#L166) | `string` | 可写; OneWay | `"移除"` | 可本地化文案。 |
| [`ReplaceText`](../../../TheOne.Avalonia/Controls/Upload.cs#L160) | `string` | 可写; OneWay | `"替换"` | 可本地化文案。 |
| [`RequestType`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L42) | `TheOne.Avalonia.Controls.UploadRequestType` | 可写; OneWay | `0 / default` | HTTP 请求方法，默认为 `Post`，默认单向绑定。 校验：`value => Enum.IsDefined(value)` 可选值：`Post`, `Put`, `Patch` |
| [`RequestUploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L216) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 检查启用、忙碌、预览和校验状态后，执行宿主命令或内置上传。 |
| [`SelectCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L214) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 系统单文件选择器，取消后保留原选中项。 |
| [`SelectText`](../../../TheOne.Avalonia/Controls/Upload.cs#L154) | `string` | 可写; OneWay | `"选择图片"` | 可本地化文案。 |
| [`SelectedFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L106) | `TheOne.Avalonia.Controls.UploadFile?` | 可写; TwoWay | `null` | 当前选中的原始上传文件模型；空值取消当前项选择。 |
| [`StatusText`](../../../TheOne.Avalonia/Controls/Upload.cs#L136) | `string?` | 可写; OneWay | `null` | 当前操作状态的展示文本；业务逻辑使用状态枚举和结果，而不解析此文本。 |
| [`SuccessFileUrl`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L49) | `string?` | 可写; TwoWay | `null` | 解析后的结果 URL 字符串，默认双向绑定。 |
| [`Title`](../../../TheOne.Avalonia/Controls/Upload.cs#L142) | `string` | 可写; OneWay | `"上传图片"` | 可本地化文案。 |
| [`UploadApi`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L36) | `string?` | 可写; TwoWay | `null` | 上传地址字符串，默认双向绑定。 |
| [`UploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L112) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`UploadText`](../../../TheOne.Avalonia/Controls/Upload.cs#L172) | `string` | 可写; OneWay | `"上传文件"` | 可本地化文案。 |

### UploadFile

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L29) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Name`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L27) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`Size`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L28) | `long?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SizeText`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L31) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`IUploadFilePicker`](../../api/zh-cn/TheOne.Avalonia.Controls.IUploadFilePicker.md) | `Task<UploadFile?> IUploadFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)` | 异步打开文件选择；由宿主文件选择器实现，取消时按返回集合处理。 |
| [`Upload`](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md) | `Task Upload.UploadAsync(CancellationToken cancellationToken = default(CancellationToken))` | Uploads explicitly. A supplied UploadCommand takes precedence over built-in HTTP. Errors are exposed through ErrorMessage; cancellation and stale responses never publish a URL. |
| [`Upload`](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md) | `Task Upload.SelectAsync(CancellationToken cancellationToken = default(CancellationToken))` | Selects one file through the host service or native storage provider. Cancellation preserves the current file. |
| [`Upload`](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md) | `void Upload.ReloadPreview()` | Reloads the thumbnail from a fresh file stream. The control owns and releases decoded thumbnails. |
| [`UploadFile`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md) | `Task<Stream> UploadFile.OpenReadAsync(CancellationToken cancellationToken = default(CancellationToken))` | 异步打开上传文件的只读流；调用方在使用后释放流。 |
| [`UploadFile`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md) | `static Task<UploadFile> UploadFile.FromStorageFileAsync(IStorageFile file, CancellationToken cancellationToken = default(CancellationToken), bool ownsStorageFile = false)` | The storage file must remain usable for as long as this selection is used. |
| [`UploadHttp`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadHttp.md) | `static Task<UploadHttpResult> UploadHttp.SendAsync(HttpClient client, string uploadApi, UploadRequestType requestType, UploadModel model, string allowedExtensions, IProgress<double>? progress = null, CancellationToken cancellationToken = default(CancellationToken))` | Sends from the stream's current position and closes it when the request finishes. The caller owns the client. Progress is the file transfer percentage (0–100); an unknown-length stream reports its start and completion. Responses larger than 1 MiB are rejected. Only successful HTTP responses are inspected for file URLs. |
| [`UploadHttp`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadHttp.md) | `static string? UploadHttp.ExtractFileUrl(string responseBody, string allowedExtensions)` | Returns the first absolute HTTP(S) file URL matching the extension filter, without contacting that URL. |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.UploadFile`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md) | 4 |
| [`TheOne.Avalonia.Controls.UploadHttpResult`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadHttpResult.md) | 3 |
| [`TheOne.Avalonia.Controls.UploadModel`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadModel.md) | 3 |
| [`TheOne.Avalonia.Controls.UploadRequestType`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadRequestType.md) | 3 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
