# ImageUpload

`TheOne.Avalonia.Controls.ImageUpload` is a single-image card with a dashed border, an image area and a size hint. It derives from [Upload](upload.md) and reuses its file selection, validation and two-way `SelectedFile` contract.

```xml
<one:ImageUpload SelectedFile="{Binding CoverFile, Mode=TwoWay}"
                 Mode="Upload" MaxFileSize="3145728" />
<one:ImageUpload SelectedFile="{Binding CoverFile}" Mode="Preview"
                 HintText="Cover image" />
```

| Member | Behavior |
| --- | --- |
| `Mode` | `ImageUploadMode.Upload` by default: select, replace, view and delete. `Preview` permits viewing only, including when commands are called directly. Switching modes cancels a pending picker or built-in upload. The host can still update the bound image. |
| `SelectedFile` | Inherited two-way `UploadFile` selection. Use a fresh readable stream factory for each read; paths are not required. |
| `MaxFileSize` | Default 3 MiB (3145728 bytes). The actual read is checked even without size metadata. Zero removes the configured limit; the inherited 64 MiB read cap still applies. |
| `AllowedExtensions` | Inherited default PNG, JPEG, WebP, BMP and GIF. Non-image selections are rejected, and decoding errors are displayed. |
| `ImageHeight` | Image-area height: Small 200, Normal 240, Large 280 DIP. Can be overridden independently. |
| `ImageStretch` | `Uniform` by default, preserving the entire image with letterboxing. |
| `HintText` / `DisplayHint` | Optional caption override; otherwise displays the current size limit or a preview caption. An empty string suppresses the caption text. |
| `SelectText`, `ReplaceText`, `ViewText`, `RemoveText` | Localizable action captions. “Edit” means selecting a replacement image, rather than cropping or painting. Cancelling preserves the current image. |
| `ViewCommand` / `View()` | Opens [ImagePreview](image-preview.md) with the original decoded image for zoom, pan and actual-size viewing. Close or Escape dismisses the popup. The popup hides the desktop full-screen action. |

Hover or keyboard focus reveals the toolbar. Preview mode hides edit/delete actions and blocks selection, removal and upload commands. `IsEnabled="False"` disables every action, including viewing. Light/dark themes and inherited `ControlSizing.Size` are supported. Empty preview mode shows a non-editable placeholder.

Selecting an image does not send a network request. Submit the bound file from your own form, or explicitly call the inherited `RequestUploadCommand` / `UploadAsync` after configuring `UploadCommand` or `UploadApi`; the compact card does not include a separate submit button. The inherited `EmptyTemplate`, `FileTemplate` and `ActionsTemplate` belong to Upload's default template and are not used by this card; replace `Template` to customize its structure.

The control owns and releases decoded images. The consumer owns accepted `UploadFile` selections and must dispose them after replacement/removal and when its screen/model is released, as described in [Upload](upload.md#host-command-integration). Do not dispose a shared file while another control still uses it.

PreviewHost: **ImageUpload** under extension controls.

<!-- visual-guide:start -->
## Appearance and example states

[Capture scope and visual index](visual-index.md). These are actual source-rendered examples; use this page’s usage notes for interaction and binding contracts.

### ImageUpload

![ImageUpload — Single-image empty, populated and preview modes; selection/replacement still hand bound files to the host.](../images/ImageUpload-light.png)

Single-image empty, populated and preview modes; selection/replacement still hand bound files to the host.

[View Dark comparison](../images/ImageUpload-dark.png)

<!-- visual-guide:end -->
