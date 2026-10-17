# NavPage

[English](../en/TheOne.Avalonia.Controls.NavPage.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavPage.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavPage` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

Navigation shell with retained, closable card pages and a collapsible sidebar.

[Usage and examples](../../controls/en/nav-page.md)

[Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L16)

## Properties

| Name | Type | Default | Binding / target |
| --- | --- | --- | --- |
| [ChildrenBinding](../../../TheOne.Avalonia/Controls/NavPage.cs#L57) | `Avalonia.Data.BindingBase?` | `null` | OneWay; writable;  |
| [CollapsedWidth](../../../TheOne.Avalonia/Controls/NavPage.cs#L68) | `double` | `64` | OneWay; writable;  |
| [DefaultHome](../../../TheOne.Avalonia/Controls/NavPage.cs#L49) | `TheOne.Avalonia.Controls.NavPageModel?` | `null` | OneWay; writable;  |
| [ExpandedWidth](../../../TheOne.Avalonia/Controls/NavPage.cs#L67) | `double` | `260` | OneWay; writable;  |
| [FooterTemplate](../../../TheOne.Avalonia/Controls/NavPage.cs#L60) | `Avalonia.Controls.Templates.IDataTemplate?` | `null` | OneWay; writable;  |
| [HeaderIcon](../../../TheOne.Avalonia/Controls/NavPage.cs#L64) | `object?` | `null` | OneWay; writable;  |
| [HeaderTemplate](../../../TheOne.Avalonia/Controls/NavPage.cs#L59) | `Avalonia.Controls.Templates.IDataTemplate?` | `null` | OneWay; writable;  |
| [HeaderTitle](../../../TheOne.Avalonia/Controls/NavPage.cs#L63) | `string` | `"THE ONE"` | OneWay; writable;  |
| [IconBinding](../../../TheOne.Avalonia/Controls/NavPage.cs#L55) | `Avalonia.Data.BindingBase?` | `null` | OneWay; writable;  |
| [IsCollapsed](../../../TheOne.Avalonia/Controls/NavPage.cs#L65) | `bool` | `false` | TwoWay; writable;  |
| [ItemsSource](../../../TheOne.Avalonia/Controls/NavPage.cs#L48) | `System.Collections.IEnumerable?` | `null` | OneWay; writable;  |
| [PageTemplate](../../../TheOne.Avalonia/Controls/NavPage.cs#L58) | `Avalonia.Controls.Templates.IDataTemplate?` | `null` | OneWay; writable;  |
| [RightHeader](../../../TheOne.Avalonia/Controls/NavPage.cs#L61) | `object?` | `null` | OneWay; writable; custom right-side header |
| [RightHeaderTemplate](../../../TheOne.Avalonia/Controls/NavPage.cs#L62) | `Avalonia.Controls.Templates.IDataTemplate?` | `null` | OneWay; writable; template for RightHeader |
| [SelectedPage](../../../TheOne.Avalonia/Controls/NavPage.cs#L70) | `object?` | `computed / state` | OneWay; read-only;  |
| [ShowItemIcon](../../../TheOne.Avalonia/Controls/NavPage.cs#L66) | `bool` | `true` | OneWay; writable;  |
| [TitleBinding](../../../TheOne.Avalonia/Controls/NavPage.cs#L53) | `Avalonia.Data.BindingBase?` | `null` | OneWay; writable;  |
| [TransitionDuration](../../../TheOne.Avalonia/Controls/NavPage.cs#L69) | `System.TimeSpan` | `TimeSpan.FromMilliseconds(200)` | OneWay; writable;  |
| [ViewModelBinding](../../../TheOne.Avalonia/Controls/NavPage.cs#L51) | `Avalonia.Data.BindingBase?` | `null` | OneWay; writable;  |

## Methods and constructors

| Signature | Source |
| --- | --- |
| `bool NavPage.OpenPage(object source)` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L233) |
| `bool NavPage.ClosePage(object source)` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L279) |
| `int NavPage.CloseAllPages()` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L280) |
| `NavPage.NavPage()` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L16) |

## Property identifiers and fields

| Signature | Source |
| --- | --- |
| `static readonly StyledProperty<IEnumerable?> NavPage.ItemsSourceProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L18) |
| `static readonly StyledProperty<NavPageModel?> NavPage.DefaultHomeProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L19) |
| `static readonly StyledProperty<BindingBase?> NavPage.ViewModelBindingProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L20) |
| `static readonly StyledProperty<BindingBase?> NavPage.TitleBindingProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L21) |
| `static readonly StyledProperty<BindingBase?> NavPage.IconBindingProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L22) |
| `static readonly StyledProperty<BindingBase?> NavPage.ChildrenBindingProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L23) |
| `static readonly StyledProperty<IDataTemplate?> NavPage.PageTemplateProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L24) |
| `static readonly StyledProperty<IDataTemplate?> NavPage.HeaderTemplateProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L25) |
| `static readonly StyledProperty<IDataTemplate?> NavPage.FooterTemplateProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L26) |
| `static readonly StyledProperty<object?> NavPage.RightHeaderProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L27) |
| `static readonly StyledProperty<IDataTemplate?> NavPage.RightHeaderTemplateProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L28) |
| `static readonly StyledProperty<string> NavPage.HeaderTitleProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L29) |
| `static readonly StyledProperty<object?> NavPage.HeaderIconProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L30) |
| `static readonly StyledProperty<bool> NavPage.IsCollapsedProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L31) |
| `static readonly StyledProperty<bool> NavPage.ShowItemIconProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L32) |
| `static readonly StyledProperty<double> NavPage.ExpandedWidthProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L33) |
| `static readonly StyledProperty<double> NavPage.CollapsedWidthProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L34) |
| `static readonly StyledProperty<TimeSpan> NavPage.TransitionDurationProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L35) |
| `static readonly DirectProperty<NavPage, object?> NavPage.SelectedPageProperty` | [Source](../../../TheOne.Avalonia/Controls/NavPage.cs#L36) |
