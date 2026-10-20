# NavPageModel

[English](../en/TheOne.Avalonia.Controls.NavPageModel.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPageModel.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPageModel` · Class · `Avalonia.AvaloniaObject`

Default navigation data. ViewModel is resolved by the host's data templates.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L19) | `System.Collections.ObjectModel.ObservableCollection<TheOne.Avalonia.Controls.NavPageModel>` | read-only; CLR / n/a | `[]` | Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes. |
| [`Icon`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L15) | `object?` | writable; OneWay | `null` | Icon content/vector geometry as defined by the signature; the host owns external bitmap lifetimes. |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L17) | `bool` | writable; OneWay | `true` | IsClosable configuration on NavPageModel; type and initial value are listed here. IsClosableProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsPinned`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L18) | `bool` | writable; OneWay | `false` | IsPinned configuration on NavPageModel; type and initial value are listed here. IsPinnedProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`Title`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L14) | `string` | writable; OneWay | `""` | Display title, distinct from business identity or a localization key. |
| [`ViewModel`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L16) | `object?` | writable; OneWay | `null` | ViewModel configuration on NavPageModel; type and initial value are listed here. ViewModelProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`NavPageModel.NavPageModel()`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L7) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string> NavPageModel.TitleProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L9) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> NavPageModel.IconProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L10) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> NavPageModel.ViewModelProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavPageModel.IsClosableProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L12) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavPageModel.IsPinnedProperty`](../../../TheOne.Avalonia/Controls/NavPageModel.cs#L13) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
