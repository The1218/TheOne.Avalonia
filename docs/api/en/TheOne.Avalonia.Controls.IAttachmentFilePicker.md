# IAttachmentFilePicker

[English](../en/TheOne.Avalonia.Controls.IAttachmentFilePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IAttachmentFilePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IAttachmentFilePicker` · Interface

Provides a multi-file selection. Ownership of returned files transfers to AttachmentList.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L272)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task<IReadOnlyList<UploadFile>> IAttachmentFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L274) | Asynchronously picks files via the host picker; interpret cancellation through its returned collection. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
