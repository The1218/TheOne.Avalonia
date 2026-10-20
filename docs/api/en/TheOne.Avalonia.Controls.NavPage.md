# NavPage

[English](../en/TheOne.Avalonia.Controls.NavPage.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPage.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPage` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Navigation shell with retained, closable card pages and a collapsible sidebar.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavPage.cs#L16)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ChildrenBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L57) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | Native binding for menu children; use concrete compiled model bindings rather than reflection paths. |
| [`CollapsedWidth`](../../../TheOne.Avalonia/Controls/NavPage.cs#L68) | `double` | writable; OneWay | `64` | CollapsedWidth configuration on NavPage; type and initial value are listed here. CollapsedWidthProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `v => v > 0 && double.IsFinite(v)` |
| [`DefaultHome`](../../../TheOne.Avalonia/Controls/NavPage.cs#L49) | `TheOne.Avalonia.Controls.NavPageModel?` | writable; OneWay | `null` | DefaultHome configuration on NavPage; type and initial value are listed here. DefaultHomeProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`ExpandedWidth`](../../../TheOne.Avalonia/Controls/NavPage.cs#L67) | `double` | writable; OneWay | `260` | ExpandedWidth configuration on NavPage; type and initial value are listed here. ExpandedWidthProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. Validation: `v => v > 0 && double.IsFinite(v)` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L60) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/NavPage.cs#L64) | `object?` | writable; OneWay | `null` | Header icon content, customized independently of its text. |
| [`HeaderTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L59) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderTitle`](../../../TheOne.Avalonia/Controls/NavPage.cs#L63) | `string` | writable; OneWay | `"THE ONE"` | HeaderTitle configuration on NavPage; type and initial value are listed here. HeaderTitleProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IconBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L55) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | IconBinding configuration on NavPage; type and initial value are listed here. IconBindingProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`IsCollapsed`](../../../TheOne.Avalonia/Controls/NavPage.cs#L65) | `bool` | writable; TwoWay | `false` | Collapses the menu to an icon rail; children use hover popups while current selection is retained. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/NavPage.cs#L48) | `System.Collections.IEnumerable?` | writable; OneWay | `null` | Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport. |
| [`PageTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L58) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`RightHeader`](../../../TheOne.Avalonia/Controls/NavPage.cs#L61) | `object?` | writable; OneWay | `null` | RightHeader configuration on NavPage; type and initial value are listed here. RightHeaderProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`RightHeaderTemplate`](../../../TheOne.Avalonia/Controls/NavPage.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`SelectedPage`](../../../TheOne.Avalonia/Controls/NavPage.cs#L70) | `object?` | read-only; OneWay | `computed / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`ShowItemIcon`](../../../TheOne.Avalonia/Controls/NavPage.cs#L66) | `bool` | writable; OneWay | `true` | ShowItemIcon configuration on NavPage; type and initial value are listed here. ShowItemIconProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`TitleBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L53) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | TitleBinding configuration on NavPage; type and initial value are listed here. TitleBindingProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |
| [`TransitionDuration`](../../../TheOne.Avalonia/Controls/NavPage.cs#L69) | `System.TimeSpan` | writable; OneWay | `TimeSpan.FromMilliseconds(200)` | Expansion/transition duration; no animation runs when Motion.Enabled=False. Validation: `v => v >= TimeSpan.Zero` |
| [`ViewModelBinding`](../../../TheOne.Avalonia/Controls/NavPage.cs#L51) | `Avalonia.Data.BindingBase?` | writable; OneWay | `null` | ViewModelBinding configuration on NavPage; type and initial value are listed here. ViewModelBindingProperty participates in Avalonia property notifications. See the usage contract and linked declaration/accessor for its context. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`bool NavPage.OpenPage(object source)`](../../../TheOne.Avalonia/Controls/NavPage.cs#L233) | Opens once per model identity and activates the retained page. Null view models are ignored. |
| [`bool NavPage.ClosePage(object source)`](../../../TheOne.Avalonia/Controls/NavPage.cs#L279) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`int NavPage.CloseAllPages()`](../../../TheOne.Avalonia/Controls/NavPage.cs#L280) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`NavPage.NavPage()`](../../../TheOne.Avalonia/Controls/NavPage.cs#L16) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<IEnumerable?> NavPage.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<NavPageModel?> NavPage.DefaultHomeProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavPage.ViewModelBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavPage.TitleBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavPage.IconBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<BindingBase?> NavPage.ChildrenBindingProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.PageTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.HeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.FooterTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> NavPage.RightHeaderProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> NavPage.RightHeaderTemplateProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<string> NavPage.HeaderTitleProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> NavPage.HeaderIconProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavPage.IsCollapsedProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> NavPage.ShowItemIconProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavPage.ExpandedWidthProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> NavPage.CollapsedWidthProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<TimeSpan> NavPage.TransitionDurationProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<NavPage, object?> NavPage.SelectedPageProperty`](../../../TheOne.Avalonia/Controls/NavPage.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Controls.NavPageModel`](TheOne.Avalonia.Controls.NavPageModel.md)
