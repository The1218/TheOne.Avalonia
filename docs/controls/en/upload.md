# Upload

Use [AttachmentList](attachment-list.md) for multiple-file selection and attachment collections. Upload retains its single-file contract.

`TheOne.Avalonia.Controls.Upload` selects one image and renders a thumbnail. It includes default templates, the native file picker and optional single-file HTTP upload. Configure `UploadApi` for the built-in transport, or supply `UploadCommand` to implement the operation in your application.

## Built-in API upload

```xml
<one:Upload SelectedFile="{Binding CoverFile, Mode=TwoWay}"
            UploadApi="{Binding UploadEndpoint, Mode=TwoWay}"
            RequestType="{Binding UploadMethod}"
            SuccessFileUrl="{Binding CoverUrl, Mode=TwoWay}" />
```

`UploadApi` is a two-way string property and must contain an absolute HTTP(S) endpoint. `RequestType` chooses `UploadRequestType.Post` (default), `Put` or `Patch`. Changing the method does not change the request body: the built-in transport sends `multipart/form-data` with these exact field names:

| Field | Content |
| --- | --- |
| `filename` | The selected file name as text. |
| `filedata` | The file stream as binary content, with the file name and optional MIME type. |

The built-in `UploadModel` carries `FileName`, `FileData` (`Stream`) and optional `ContentType`. The wire format sends file bytes, rather than a JSON object or a Base64 string. The endpoint must accept this multipart contract; `RequestType` selects the HTTP verb, not a JSON/form encoding mode. For APIs with different field names, request shapes or response rules, set `UploadCommand` instead.

When calling `UploadHttp.SendAsync` directly, the request reads `FileData` from its current position and disposes that stream when the request finishes; the caller still owns the client. A successful `UploadHttpResult` contains the status code, response text and nullable file URL. Response bodies are limited to 1 MiB; larger responses raise an error. Disabling or detaching the control cancels an active built-in request, and callers can cancel explicitly through `UploadAsync(CancellationToken)`.

Selecting a file never starts a request. The default upload button executes `RequestUploadCommand`, which uses the host `UploadCommand` when present and otherwise starts the built-in `UploadAsync()` operation. Host commands retain their `CanExecute` behavior. The built-in operation reports `IsUploading`, `Progress`, `StatusText` and `ErrorMessage`, disables conflicting actions and leaves the host-owned `IsBusy` value intact. Request failures can be retried. Changing the selected file, endpoint, method, HTTP client or host command cancels the old operation and clears its result, so a stale response cannot overwrite the current file's URL.

On a successful HTTP response, the response body is searched for an absolute HTTP(S) URL whose path ends with an allowed file extension. Nested JSON string values and plain-text URLs are supported; the URL does not need a specific property name such as `url`. Query strings and fragments do not affect the extension check, so `https://cdn.example.com/covers/a.PNG?token=123#preview` can match `.png`. Relative paths, `data:`/`file:` URLs and URLs without a valid file suffix are rejected. `AllowedExtensions` controls the accepted suffixes; when it is empty, the suffix must still contain 1–16 ASCII letters or digits after the dot, without filtering against a built-in list of file types. If several values match, the first valid result is used. `UploadHttp.ExtractFileUrl` exposes the same extraction rule for consuming code.

`SuccessFileUrl` is a two-way string property: a valid response is written back to the ViewModel for later storage or display. A 2xx response without a valid file URL is an explicit upload error and leaves no success URL. This extraction validates URL format and suffix; it does not request the returned URL or verify that the remote resource exists.

The control's optional `HttpClient` property accepts a consumer-owned client. Configure authentication headers, message handlers, timeout or a test transport on that client; the control does not dispose an injected client. The default client sends no application authentication. The PreviewHost **Built-in API upload** example starts with an empty endpoint, uses its own selection and exposes method selection plus a copyable URL result. Entering a real endpoint and pressing upload sends the selected file to that endpoint. Its separate **Host command** example reads bytes locally and sends no network request.

## Host command integration

```xml
<one:Upload SelectedFile="{Binding CoverFile}"
            UploadCommand="{Binding UploadCoverCommand}"
            IsBusy="{Binding IsUploading}"
            Progress="{Binding UploadProgress}"
            StatusText="{Binding UploadStatus}"
            MaxFileSize="10485760" />
```

`SelectedFile` is a two-way `UploadFile` binding. The host command receives that exact immutable selection as its parameter. Read data with `await file.OpenReadAsync(cancellationToken)` and dispose each returned stream. Every call opens a fresh stream; the command must use the supplied parameter rather than looking up a potentially newer selection. No local filesystem path is required.

Create an `UploadFile` with a name, a stream factory, optional byte size and MIME type. `UploadFile.FromStorageFileAsync` adapts an Avalonia `IStorageFile`; `ownsStorageFile: true` transfers that source to the wrapper, while the default false leaves the source caller-owned. The native picker uses true. Accepted picker results transfer to the consumer through `SelectedFile`: the host must dispose old selections after replacement/removal and dispose the current selection when its screen/model is no longer used. Custom picker results that are rejected or arrive after cancellation are disposed by the control. Do not share ownership of the same disposable selection between independent consumers.

`UploadFile.Dispose()` prevents new reads and releases an owned storage source after existing stream leases close. The control releases its own decoded thumbnails and read streams, and never disposes accepted or externally bound file objects. The PreviewHost model demonstrates releasing an old owned selection when the binding changes.

`UploadCommand` takes precedence over `UploadApi`; its `CanExecute` controls the upload action. The host sets `IsBusy`, `Progress` (0–100) and `StatusText` during its operation, and owns any server response mapping or retry policy. Busy or disabled controls reject selection, removal and upload requests. Selecting a file does not start an upload automatically. The PreviewHost host command reads a packaged or selected image stream and reports its byte count; it performs no network request.

## Defaults and customization

| Member | Behavior |
| --- | --- |
| `SelectCommand` / `SelectAsync` | Native single-file picker; cancellation preserves the current selection. |
| `RemoveCommand` | Clears the selection, thumbnail and errors. |
| `RequestUploadCommand` / `UploadAsync` | Checks enabled, busy, preview and validation state, then uses the host command or the built-in upload. |
| `UploadApi` | Endpoint string with two-way binding by default. |
| `RequestType` | HTTP request verb, default `Post`; uses one-way binding by default. |
| `SuccessFileUrl` | Parsed result URL string with two-way binding by default. |
| `HttpClient` | Optional consumer-owned client for the built-in transport. |
| `IsUploading` | Read-only state for an active built-in request. |
| `FilePicker` | Optional `IUploadFilePicker` host service. `PickAsync` returns one `UploadFile` or null on cancellation. |
| `AllowedExtensions` | Comma/semicolon/space separated extensions. Defaults to PNG, JPEG, WebP, BMP and GIF. Empty accepts any extension. Explicitly allowed non-image files use a file icon and can be uploaded without thumbnail decoding. |
| `MaxFileSize` | Default 10 MiB; zero disables the declared-size limit. Thumbnail reads are capped at 64 MiB independently. Actual read size is checked when metadata is unavailable. |
| `PreviewImage`, `HasFile`, `HasPreview`, `IsPreviewLoading`, `IsSelecting`, `ErrorMessage` | Read-only bindable display state. |
| `Title`, `Description`, `SelectText`, `ReplaceText`, `RemoveText`, `UploadText` | Localizable captions. |

The built-in UI follows inherited Small / Normal / Large density and light/dark `One.*` resources. `EmptyTemplate`, `FileTemplate` and `ActionsTemplate` are independently replaceable. All receive the `Upload` instance as their data context, preserving access to selection, preview and commands:

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

Unsupported types and oversized picker results preserve the previous selection and report an error. A malformed or unreadable bound image keeps its file metadata, displays an error and disables the upload action until the source is replaced, removed or successfully reloaded with `ReloadPreview`. Replacing, clearing or detaching cancels pending preview work; stale picker or decoder results cannot overwrite newer data. Reattaching reloads the preserved selection. The first decoded image frame is used for thumbnails; animation playback and multiple files are outside this control's scope.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Upload

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Upload.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ActionsTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L202) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`AllowedExtensions`](../../../TheOne.Avalonia/Controls/Upload.cs#L178) | `string` | writable; OneWay | `".png,.jpg,.jpeg,.webp,.bmp,.gif"` | Comma/semicolon/space separated extensions. Defaults to PNG, JPEG, WebP, BMP and GIF. Empty accepts any extension. Explicitly allowed non-image files use a file icon and can be uploaded without thumbnail decoding. |
| [`Description`](../../../TheOne.Avalonia/Controls/Upload.cs#L148) | `string` | writable; OneWay | `"PNG、JPG、WebP 等图片 · 单文件"` | Localizable captions. |
| [`EmptyTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L190) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Upload.cs#L213) | `string?` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`FilePicker`](../../../TheOne.Avalonia/Controls/Upload.cs#L118) | `TheOne.Avalonia.Controls.IUploadFilePicker?` | writable; OneWay | `null` | Optional `IUploadFilePicker` host service. `PickAsync` returns one `UploadFile` or null on cancellation. |
| [`FileTemplate`](../../../TheOne.Avalonia/Controls/Upload.cs#L196) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HasFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L209) | `bool` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`HasPreview`](../../../TheOne.Avalonia/Controls/Upload.cs#L210) | `bool` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L56) | `System.Net.Http.HttpClient?` | writable; OneWay | `null` | Optional consumer-owned client for the built-in transport. |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/Upload.cs#L124) | `bool` | writable; OneWay | `false` | Upload-in-progress state maintained by the async operation; business submissions should respect it. |
| [`IsPreviewLoading`](../../../TheOne.Avalonia/Controls/Upload.cs#L211) | `bool` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`IsSelecting`](../../../TheOne.Avalonia/Controls/Upload.cs#L212) | `bool` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`IsUploading`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L62) | `bool` | read-only; OneWay | `computed / state` | Read-only state for an active built-in request. |
| [`MaxFileSize`](../../../TheOne.Avalonia/Controls/Upload.cs#L184) | `long` | writable; OneWay | `10 * 1024 * 1024` | Default 10 MiB; zero disables the declared-size limit. Thumbnail reads are capped at 64 MiB independently. Actual read size is checked when metadata is unavailable. Validation: `value => value >= 0` |
| [`PreviewImage`](../../../TheOne.Avalonia/Controls/Upload.cs#L208) | `Avalonia.Media.IImage?` | read-only; OneWay | `computed / state` | Read-only bindable display state. |
| [`Progress`](../../../TheOne.Avalonia/Controls/Upload.cs#L130) | `double` | writable; OneWay | `0 / default` | Progress value with range/unit defined by its upload/Gantt model, distinct from a success flag. Validation: `value => double.IsFinite(value) && value is >= 0 and <= 100` |
| [`RemoveCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L215) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Clears the selection, thumbnail and errors. |
| [`RemoveText`](../../../TheOne.Avalonia/Controls/Upload.cs#L166) | `string` | writable; OneWay | `"移除"` | Localizable captions. |
| [`ReplaceText`](../../../TheOne.Avalonia/Controls/Upload.cs#L160) | `string` | writable; OneWay | `"替换"` | Localizable captions. |
| [`RequestType`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L42) | `TheOne.Avalonia.Controls.UploadRequestType` | writable; OneWay | `0 / default` | HTTP request verb, default `Post`; uses one-way binding by default. Validation: `value => Enum.IsDefined(value)` Values: `Post`, `Put`, `Patch` |
| [`RequestUploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L216) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Checks enabled, busy, preview and validation state, then uses the host command or the built-in upload. |
| [`SelectCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L214) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Native single-file picker; cancellation preserves the current selection. |
| [`SelectText`](../../../TheOne.Avalonia/Controls/Upload.cs#L154) | `string` | writable; OneWay | `"选择图片"` | Localizable captions. |
| [`SelectedFile`](../../../TheOne.Avalonia/Controls/Upload.cs#L106) | `TheOne.Avalonia.Controls.UploadFile?` | writable; TwoWay | `null` | Selected original upload-file model; null clears the current selection. |
| [`StatusText`](../../../TheOne.Avalonia/Controls/Upload.cs#L136) | `string?` | writable; OneWay | `null` | Display text for operation status; business logic should use state/results rather than parse this text. |
| [`SuccessFileUrl`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L49) | `string?` | writable; TwoWay | `null` | Parsed result URL string with two-way binding by default. |
| [`Title`](../../../TheOne.Avalonia/Controls/Upload.cs#L142) | `string` | writable; OneWay | `"上传图片"` | Localizable captions. |
| [`UploadApi`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L36) | `string?` | writable; TwoWay | `null` | Endpoint string with two-way binding by default. |
| [`UploadCommand`](../../../TheOne.Avalonia/Controls/Upload.cs#L112) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`UploadText`](../../../TheOne.Avalonia/Controls/Upload.cs#L172) | `string` | writable; OneWay | `"上传文件"` | Localizable captions. |

### UploadFile

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.UploadFile.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ContentType`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L29) | `string?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`Name`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L27) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Name used for presentation/identity; keep it consistent with templates and keys. |
| [`Size`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L28) | `long?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`SizeText`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L31) | `string` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

| Type | Action signature | Purpose |
| --- | --- | --- |
| [`IUploadFilePicker`](../../api/en/TheOne.Avalonia.Controls.IUploadFilePicker.md) | `Task<UploadFile?> IUploadFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)` | Asynchronously picks files via the host picker; interpret cancellation through its returned collection. |
| [`Upload`](../../api/en/TheOne.Avalonia.Controls.Upload.md) | `Task Upload.UploadAsync(CancellationToken cancellationToken = default(CancellationToken))` | Uploads explicitly. A supplied UploadCommand takes precedence over built-in HTTP. Errors are exposed through ErrorMessage; cancellation and stale responses never publish a URL. |
| [`Upload`](../../api/en/TheOne.Avalonia.Controls.Upload.md) | `Task Upload.SelectAsync(CancellationToken cancellationToken = default(CancellationToken))` | Selects one file through the host service or native storage provider. Cancellation preserves the current file. |
| [`Upload`](../../api/en/TheOne.Avalonia.Controls.Upload.md) | `void Upload.ReloadPreview()` | Reloads the thumbnail from a fresh file stream. The control owns and releases decoded thumbnails. |
| [`UploadFile`](../../api/en/TheOne.Avalonia.Controls.UploadFile.md) | `Task<Stream> UploadFile.OpenReadAsync(CancellationToken cancellationToken = default(CancellationToken))` | Asynchronously opens an upload file's readable stream; the caller disposes it after use. |
| [`UploadFile`](../../api/en/TheOne.Avalonia.Controls.UploadFile.md) | `static Task<UploadFile> UploadFile.FromStorageFileAsync(IStorageFile file, CancellationToken cancellationToken = default(CancellationToken), bool ownsStorageFile = false)` | The storage file must remain usable for as long as this selection is used. |
| [`UploadHttp`](../../api/en/TheOne.Avalonia.Controls.UploadHttp.md) | `static Task<UploadHttpResult> UploadHttp.SendAsync(HttpClient client, string uploadApi, UploadRequestType requestType, UploadModel model, string allowedExtensions, IProgress<double>? progress = null, CancellationToken cancellationToken = default(CancellationToken))` | Sends from the stream's current position and closes it when the request finishes. The caller owns the client. Progress is the file transfer percentage (0–100); an unknown-length stream reports its start and completion. Responses larger than 1 MiB are rejected. Only successful HTTP responses are inspected for file URLs. |
| [`UploadHttp`](../../api/en/TheOne.Avalonia.Controls.UploadHttp.md) | `static string? UploadHttp.ExtractFileUrl(string responseBody, string allowedExtensions)` | Returns the first absolute HTTP(S) file URL matching the extension filter, without contacting that URL. |

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.UploadFile`](../../api/en/TheOne.Avalonia.Controls.UploadFile.md) | 4 |
| [`TheOne.Avalonia.Controls.UploadHttpResult`](../../api/en/TheOne.Avalonia.Controls.UploadHttpResult.md) | 3 |
| [`TheOne.Avalonia.Controls.UploadModel`](../../api/en/TheOne.Avalonia.Controls.UploadModel.md) | 3 |
| [`TheOne.Avalonia.Controls.UploadRequestType`](../../api/en/TheOne.Avalonia.Controls.UploadRequestType.md) | 3 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
