# Upload 上传

`TheOne.Avalonia.Controls.Upload` 选择一张图片并显示缩略图，内置默认模板、系统文件选择器和可选的单文件 HTTP 上传。配置 `UploadApi` 可使用内置上传，也可提供 `UploadCommand` 接入业务上传逻辑。

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
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

### Upload

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md)

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

### UploadFile

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L16) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Name`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L14) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。 |
| [`Size`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L15) | `long?` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SizeText`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L17) | `string` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Upload | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=16; Padding=20 |
| Upload | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=12; Padding=12 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.IUploadFilePicker`](../../api/zh-cn/TheOne.Avalonia.Controls.IUploadFilePicker.md) | Interface | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.Upload`](../../api/zh-cn/TheOne.Avalonia.Controls.Upload.md) | Class | 31 | 4 | 0 |
| [`TheOne.Avalonia.Controls.UploadFile`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadFile.md) | Class | 4 | 4 | 0 |
| [`TheOne.Avalonia.Controls.UploadHttp`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadHttp.md) | Class | 0 | 2 | 0 |
| [`TheOne.Avalonia.Controls.UploadHttpResult`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadHttpResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.UploadModel`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadModel.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.UploadRequestType`](../../api/zh-cn/TheOne.Avalonia.Controls.UploadRequestType.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
