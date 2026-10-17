# NavPageModel

[English](../en/TheOne.Avalonia.Controls.NavPageModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPageModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPageModel` · Class · `Avalonia.AvaloniaObject`

Default navigation data. ViewModel is resolved by the host's data templates.

[用法与示例](../../controls/zh-cn/nav-page.md)

[Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7)

## Properties

| Name | Type | Default | Binding / target |
| --- | --- | --- | --- |
| [Children](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L19) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.NavPageModel>` | `[]` | CLR / n/a; read-only;  |
| [Icon](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L15) | `object?` | `null` | OneWay; writable;  |
| [IsClosable](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L17) | `bool` | `true` | OneWay; writable;  |
| [IsPinned](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L18) | `bool` | `false` | OneWay; writable;  |
| [Title](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L14) | `string` | `""` | OneWay; writable;  |
| [ViewModel](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L16) | `object?` | `null` | OneWay; writable;  |

## Methods and constructors

| Signature | Source |
| --- | --- |
| `NavPageModel.NavPageModel()` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7) |

## Property identifiers and fields

| Signature | Source |
| --- | --- |
| `static readonly StyledProperty<string> NavPageModel.TitleProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L9) |
| `static readonly StyledProperty<object?> NavPageModel.IconProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L10) |
| `static readonly StyledProperty<object?> NavPageModel.ViewModelProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L11) |
| `static readonly StyledProperty<bool> NavPageModel.IsClosableProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L12) |
| `static readonly StyledProperty<bool> NavPageModel.IsPinnedProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L13) |
