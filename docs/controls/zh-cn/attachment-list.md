# AttachmentList 附件列表

`AttachmentList` 提供一次选择多个文件、附件空态、状态、进度、打开和移除入口。它不发送 HTTP 请求、不删除服务端文件，也不维护业务附件关系。单文件选择继续使用 [Upload](upload.md)。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### AttachmentList

![AttachmentList — 已完成、上传中和失败三种文件状态；进度只是模型数据，不伪造网络传输。](../images/AttachmentList-light.png)

已完成、上传中和失败三种文件状态；进度只是模型数据，不伪造网络传输。

[查看 Dark 对照](../images/AttachmentList-dark.png)

<!-- visual-guide:end -->

## 接入

```xml
<one:AttachmentList Title="申请附件"
    Files="{CompiledBinding Attachments}"
    UploadCommand="{CompiledBinding UploadAttachmentsCommand}"
    RemoveCommand="{CompiledBinding RemoveAttachmentCommand}"
    IsBusy="{CompiledBinding IsLoading}"
    PreviewRequested="OnPreviewRequested" />
```

`Files` 为 `IEnumerable<IAttachmentItem>?`，通常绑定 `ObservableCollection<AttachmentItem>`。集合变化刷新空态，文件项属性变化刷新显示和命令门禁。已有业务模型可实现 `IAttachmentItem`；打开事件和移除命令始终收到原对象，控件不复制或修改集合。

## 公共契约

| 成员 | 说明 |
| --- | --- |
| `Title`、`Description`、`HintText` | 标题、说明和提示文字 |
| `UploadButtonText`、`EmptyText` | 多选按钮和空态文案 |
| `IsUploadVisible`、`IsRemoveVisible` | 分别控制选择和移除入口；只读列表可将两者设为 `false` |
| `IsBusy` | 禁止发起选择、打开或移除，并取消尚未接受的多选结果 |
| `UploadCommand` | `ICommand`，参数为 `IReadOnlyList<UploadFile>`，一次交付完整多选批次 |
| `RemoveCommand` | `ICommand`，参数为原 `IAttachmentItem`；由宿主移除关联或集合项 |
| `PreviewRequested` | 路由事件，`AttachmentPreviewRequestedEventArgs.Item` 为原对象，由宿主预览或下载 |
| `FilePicker` | 可选 `IAttachmentFilePicker`，`Task<IReadOnlyList<UploadFile>> PickAsync(TopLevel, CancellationToken)` |
| `SelectFilesAsync(CancellationToken = default)` | 调用原生多选或可注入选择器；取消不改变附件集合 |
| `HasFiles`、`IsSelecting`、`ErrorMessage` | 只读空态、选择中和选择错误信息 |
| `SelectCommand`、`RequestRemoveCommand`、`RequestPreviewCommand` | 模板使用的受控操作；移除和打开会检查原对象仍在集合中 |

`IAttachmentItem : INotifyPropertyChanged` 提供只读 `FileName`、`FileUrl`、`FileSize`、`SizeText`、`StatusText`、`UploadProgress`、`IsUploading`、`CanOpen`、`CanRemove`、`Status`。`AttachmentStatus` 为 `Default / Uploading / Success / Error`。

`AttachmentItem` 为可写的通用显示模型，提供 `MarkUploading()`、`MarkUploaded(string fileUrl, long fileSize)`、`MarkFailed(string message)`、`CreateUploaded(string fileName, string? fileUrl, long fileSize)` 和 `FormatSize(long size)`。原始资源地址不被改写；URL 的业务有效性由宿主判断。上传开始不伪造传输百分比，零进度显示不确定进度，已知进度限定为 0–100 的有限数值。

## 文件归属与取消

1. 原生选择固定使用 `AllowMultiple=true`，直接创建拥有 `IStorageFile` 的 `UploadFile`，不依赖本地文件路径。
2. 选择结果经过当前视图、启用状态、忙碌状态及命令检查。取消、命令拒绝、视图卸载或过期结果会释放整批文件。DataContext、上传命令、Files 来源替换或集合成员变化都会使未接受的选择过期。
3. `UploadCommand.CanExecute(batch)` 接受且 `Execute(batch)` 正常返回后，文件所有权转移至宿主。异步命令应在自己的 `finally` 中逐个 `Dispose()`；控件卸载不会释放已转交的文件。
4. 每次读取使用 `await using var stream = await file.OpenReadAsync(token)`；保存业务版本和文件关联，丢弃离页后的迟到响应。
5. 选择器取消不修改 `Files`。原生选择窗口能否立即关闭由平台决定；迟到结果不会执行上传。
6. 自定义选择器须释放没有作为结果返回的文件；已返回文件交由控件管理。控件不承诺撤回宿主已经发出的 HTTP 请求。

## 业务上传与预览

宿主负责认证、multipart 字段、返回 DTO、资源 ID、逐文件状态、保存门禁及关联删除。不要把 `UploadFile` 当成已存远端附件，也不要把本地路径保存为服务器 URL。

```csharp
private async Task UploadAttachments(IReadOnlyList<UploadFile> files)
{
    try
    {
        foreach (var file in files)
        {
            var item = new AttachmentItem { FileName = file.Name, FileSize = file.Size ?? 0 };
            Attachments.Add(item);
            item.MarkUploading();
            try
            {
                await using var stream = await file.OpenReadAsync();
                var result = await uploadService.UploadAsync(stream, file.Name);
                item.MarkUploaded(result.Url, result.Size);
            }
            catch (Exception error) { item.MarkFailed(error.Message); }
        }
    }
    finally { foreach (var file in files) file.Dispose(); }
}
```

`uploadService` 是宿主提供的业务服务。失败项仍留在集合，宿主决定重试、移除或阻止保存。`PreviewRequested` 可路由至 [ImagePreview](image-preview.md)、文件下载或系统打开。受保护资源通过宿主服务读取；传入的位图由宿主释放。

## 主题与验证

模板仅使用 `One.*` 语义资源，支持 Light / Dark 和继承的 `ControlSizing.Size` 三档尺寸。文件名换行、操作区可换行，适合窄容器。新增能力只需验证多选、取消、过期结果、所有权、集合/项目通知及这一个控件的主题和尺寸；无需运行整库回归。
