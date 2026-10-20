# IUploadFilePicker

[English](../en/TheOne.Avalonia.Controls.IUploadFilePicker.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.IUploadFilePicker.md) · [API index](README.md)

`TheOne.Avalonia.Controls.IUploadFilePicker` · Interface

Optional host picker service. Results transfer to the consumer if accepted; rejected results are disposed. Null preserves selection.

[Source / 声明](../../../TheOne.Avalonia/Controls/UploadFile.cs#L229)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task<UploadFile?> IUploadFilePicker.PickAsync(TopLevel owner, CancellationToken cancellationToken)`](../../../TheOne.Avalonia/Controls/UploadFile.cs#L231) | Asynchronously picks files via the host picker; interpret cancellation through its returned collection. |

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.UploadFile`](TheOne.Avalonia.Controls.UploadFile.md)
