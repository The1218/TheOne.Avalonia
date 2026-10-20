# Breadcrumb

[English](../en/TheOne.Avalonia.Controls.Breadcrumb.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Breadcrumb.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Breadcrumb` · Class · `Avalonia.Controls.ItemsControl`

A caller-owned navigation path. The final item is the noninteractive current page.

[Source / 声明](../../../TheOne.Avalonia/Controls/Breadcrumb.Input.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentItem`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L43) | `object?` | read-only; OneWay | `computed / state` | null ; Read-only final item: `breadcrumb.CurrentItem`. |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L42) | `System.Windows.Input.ICommand?` | writable; OneWay | `null` | null ; `ItemInvokedCommand="{CompiledBinding NavigateCommand}"`; receives the original item, not its Key or index. |
| [`Separator`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L39) | `object?` | writable; OneWay | `"/"` | `"/"` ; `Separator="›"` replaces it; `Separator="{x:Null}"` hides it. Use repeatable values; create visual instances through SeparatorTemplate. |
| [`SeparatorTemplate`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L40) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Separate noninteractive separator presentation. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Breadcrumb.Breadcrumb()`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L37) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Events

| Complete signature/value | Contract |
| --- | --- |
| [`event EventHandler<BreadcrumbItemInvokedEventArgs>? Breadcrumb.ItemInvoked`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L44) | State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<object?> Breadcrumb.SeparatorProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> Breadcrumb.SeparatorTemplateProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ICommand?> Breadcrumb.ItemInvokedCommandProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<Breadcrumb, object?> Breadcrumb.CurrentItemProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly RoutedEvent<BreadcrumbItemInvokedEventArgs> Breadcrumb.ItemInvokedEvent`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.BreadcrumbItem`](TheOne.Avalonia.Controls.BreadcrumbItem.md)
- [`TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs`](TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md)
- [`TheOne.Avalonia.Controls.BreadcrumbItemModel`](TheOne.Avalonia.Controls.BreadcrumbItemModel.md)
