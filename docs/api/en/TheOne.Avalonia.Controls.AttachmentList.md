# AttachmentList

[English](../en/TheOne.Avalonia.Controls.AttachmentList.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentList.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentList` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A multi-file picker and attachment presenter. The consumer owns accepted selections and business commands.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Description`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L57) | `string?` | writable; OneWay | `null` | Descriptive text supplied by the host; omitted values keep their initial state. |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L59) | `string` | writable; OneWay | `"暂无附件"` | Message when no data can be displayed; applications override it for localization. |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L72) | `string?` | read-only; OneWay | `computed / state` | Current failure/validation message; an error display is not evidence of successful completion. |
| [`FilePicker`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L69) | `TheOne.Avalonia.Controls.IAttachmentFilePicker?` | writable; OneWay | `null` | FilePicker configuration on AttachmentList; type and initial value are listed here. FilePickerProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Files`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L64) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.IAttachmentItem>?` | writable; OneWay | `null` | Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced. |
| [`HasFiles`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L70) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`HintText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L60) | `string?` | writable; OneWay | `"可一次选择多个文件。"` | HintText configuration on AttachmentList; type and initial value are listed here. HintTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L63) | `bool` | writable; OneWay | `false` | Upload-in-progress state maintained by the async operation; business submissions should respect it. |
| [`IsRemoveVisible`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L62) | `bool` | writable; OneWay | `true` | IsRemoveVisible configuration on AttachmentList; type and initial value are listed here. IsRemoveVisibleProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsSelecting`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L71) | `bool` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`IsUploadVisible`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L61) | `bool` | writable; OneWay | `true` | IsUploadVisible configuration on AttachmentList; type and initial value are listed here. IsUploadVisibleProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`RemoveCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L68) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Receives the original item. The control never mutates Files or deletes a server file. |
| [`RequestPreviewCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L75) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`RequestRemoveCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L74) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`SelectCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L73) | `System.Windows.Input.ICommand` | read-only; CLR / n/a | `accessor-defined / state` | Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument. |
| [`Title`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L56) | `string` | writable; OneWay | `"附件上传"` | Display title, distinct from business identity or a localization key. |
| [`UploadButtonText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L58) | `string` | writable; OneWay | `"上传附件"` | UploadButtonText configuration on AttachmentList; type and initial value are listed here. UploadButtonTextProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`UploadCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L66) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | Receives one IReadOnlyList<UploadFile> batch. Accepted files must be disposed by the command owner. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task AttachmentList.SelectFilesAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L87) | Selects a true multi-file batch. Cancellation preserves Files. Only accepted commands receive ownership. |
| [`AttachmentList.AttachmentList()`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L43) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<AttachmentPreviewRequestedEventArgs>? AttachmentList.PreviewRequested`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L76) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string> AttachmentList.TitleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> AttachmentList.DescriptionProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> AttachmentList.UploadButtonTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> AttachmentList.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string?> AttachmentList.HintTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> AttachmentList.IsUploadVisibleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> AttachmentList.IsRemoveVisibleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> AttachmentList.IsBusyProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IEnumerable<IAttachmentItem>?> AttachmentList.FilesProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> AttachmentList.UploadCommandProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> AttachmentList.RemoveCommandProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IAttachmentFilePicker?> AttachmentList.FilePickerProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<AttachmentList, bool> AttachmentList.HasFilesProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<AttachmentList, bool> AttachmentList.IsSelectingProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<AttachmentList, string?> AttachmentList.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<AttachmentPreviewRequestedEventArgs> AttachmentList.PreviewRequestedEvent`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs`](TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md)
- [`TheOne.Avalonia.Controls.IAttachmentFilePicker`](TheOne.Avalonia.Controls.IAttachmentFilePicker.md)
- [`TheOne.Avalonia.Controls.IAttachmentItem`](TheOne.Avalonia.Controls.IAttachmentItem.md)
