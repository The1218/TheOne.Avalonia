# Upload

[English](../en/TheOne.Avalonia.Controls.Upload.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Upload.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Upload` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Single-file selection and preview, with optional HTTP upload or a host command.

[Source / 声明](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

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

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task Upload.UploadAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L111) | Uploads explicitly. A supplied UploadCommand takes precedence over built-in HTTP. Errors are exposed through ErrorMessage; cancellation and stale responses never publish a URL. |
| [`Task Upload.SelectAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/Upload.cs#L386) | Selects one file through the host service or native storage provider. Cancellation preserves the current file. |
| [`void Upload.ReloadPreview()`](../../../TheOne.Avalonia/Controls/Upload.cs#L452) | Reloads the thumbnail from a fresh file stream. The control owns and releases decoded thumbnails. |
| [`Upload.Upload()`](../../../TheOne.Avalonia/Controls/Upload.cs#L97) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> Upload.UploadApiProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<UploadRequestType> Upload.RequestTypeProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> Upload.SuccessFileUrlProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<HttpClient?> Upload.HttpClientProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, bool> Upload.IsUploadingProperty`](../../../TheOne.Avalonia/Controls/Upload.Http.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<UploadFile?> Upload.SelectedFileProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> Upload.UploadCommandProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IUploadFilePicker?> Upload.FilePickerProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> Upload.IsBusyProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> Upload.ProgressProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> Upload.StatusTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.TitleProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L38) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.DescriptionProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L41) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.SelectTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L44) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.ReplaceTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L47) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.RemoveTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L50) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.UploadTextProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L53) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> Upload.AllowedExtensionsProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L56) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<long> Upload.MaxFileSizeProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L59) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> Upload.EmptyTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L62) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> Upload.FileTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L65) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> Upload.ActionsTemplateProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L68) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, IImage?> Upload.PreviewImageProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L71) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, bool> Upload.HasFileProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L74) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, bool> Upload.HasPreviewProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L77) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, bool> Upload.IsPreviewLoadingProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L80) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, bool> Upload.IsSelectingProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L83) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Upload, string?> Upload.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/Upload.cs#L86) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.IUploadFilePicker`](TheOne.Avalonia.Controls.IUploadFilePicker.md)
- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
- [`TheOne.Avalonia.Controls.UploadHttp`](TheOne.Avalonia.Controls.UploadHttp.md)
- [`TheOne.Avalonia.Controls.UploadHttpResult`](TheOne.Avalonia.Controls.UploadHttpResult.md)
- [`TheOne.Avalonia.Controls.UploadModel`](TheOne.Avalonia.Controls.UploadModel.md)
- [`TheOne.Avalonia.Controls.UploadRequestType`](TheOne.Avalonia.Controls.UploadRequestType.md)
