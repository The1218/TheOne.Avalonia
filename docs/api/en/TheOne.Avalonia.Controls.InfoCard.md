# InfoCard

[English](../en/TheOne.Avalonia.Controls.InfoCard.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.InfoCard.md) · [API index](README.md)

`TheOne.Avalonia.Controls.InfoCard` · Class · `Avalonia.Controls.Primitives.HeaderedContentControl`

A themed information container with a title, icon, actions, body and footer.

[Source / 声明](../../../TheOne.Avalonia/Controls/InfoCard.cs#L17)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`BoxShadow`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L50) | `Avalonia.Media.BoxShadows` | writable; OneWay | `inherited registration` | Outer shadow, supplied by the shadow and hove theme classes. |
| [`Footer`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L56) | `object?` | writable; OneWay | `null` | Footer content, customized independently of body/header. |
| [`FooterBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L61) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Footer background, default `One.InfoCard.Footer` |
| [`FooterPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L65) | `Avalonia.Thickness` | read-only; OneWay | `computed / state` | Computed footer insets: horizontal Padding and vertical FooterSpacing. |
| [`FooterSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L74) | `double` | writable; OneWay | `12` | Footer vertical padding Validation: `value => double.IsFinite(value) && value >= 0` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderActions`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L54) | `object?` | writable; OneWay | `null` | Action content beside the header, such as business buttons with native command/focus behavior. |
| [`HeaderActionsTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L55) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L59) | `Avalonia.Media.IBrush?` | writable; OneWay | `null` | Header background, default `One.Subtle` |
| [`HeaderContentSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L69) | `double` | writable; OneWay | `10` | 8 ; 10 ; 12 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L67) | `double` | writable; OneWay | `16` | 14 ; 16 ; 18 Validation: `value => double.IsFinite(value) && value > 0` |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L52) | `object?` | writable; OneWay | `null` | Header icon content, customized independently of its text. |
| [`HeaderIconSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L68) | `double` | writable; OneWay | `18` | 16 ; 18 ; 20 Validation: `value => double.IsFinite(value) && value > 0` |
| [`HeaderIconTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L53) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | Native data-template extension point; retain the correct x:DataType and follow the control's template data scope. |
| [`HeaderPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L63) | `Avalonia.Thickness` | read-only; OneWay | `computed / state` | Computed header insets: horizontal Padding and vertical HeaderSpacing. |
| [`HeaderSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L66) | `double` | writable; OneWay | `12` | Header vertical padding Validation: `value => double.IsFinite(value) && value >= 0` |
| [`ShowFooterSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L73) | `bool` | writable; OneWay | `true` | Shows a divider before an existing footer when another region precedes it. |
| [`ShowHeaderSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L71) | `bool` | writable; OneWay | `true` | Shows an edge-to-edge divider between the header and body. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`InfoCard.InfoCard()`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L47) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<BoxShadows> InfoCard.BoxShadowProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> InfoCard.HeaderIconProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L20) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.HeaderIconTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L21) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> InfoCard.HeaderActionsProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L22) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.HeaderActionsTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L23) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<object?> InfoCard.FooterProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L24) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IDataTemplate?> InfoCard.FooterTemplateProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L25) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> InfoCard.HeaderBackgroundProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L26) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<IBrush?> InfoCard.FooterBackgroundProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L27) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<InfoCard, Thickness> InfoCard.HeaderPaddingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L28) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<InfoCard, Thickness> InfoCard.FooterPaddingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L29) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> InfoCard.HeaderSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L30) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> InfoCard.HeaderFontSizeProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L31) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> InfoCard.HeaderIconSizeProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L32) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> InfoCard.HeaderContentSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L33) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> InfoCard.ShowHeaderSeparatorProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L34) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<bool> InfoCard.ShowFooterSeparatorProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L35) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<double> InfoCard.FooterSpacingProperty`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L36) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

Controls accept `small` / `normal` / `large` or inherited `ControlSizing.Size`. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

| Class | Selector | Setters/resources |
| --- | --- | --- |
| `flat` | [`one&#124;InfoCard.flat`](../../../TheOne.Avalonia/Themes/Variants.axaml#L111) | one&#124;InfoCard.flat => BorderThickness=0; HeaderBackground=Transparent; FooterBackground=Transparent; ShowHeaderSeparator=False; ShowFooterSeparator=False; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| `glow` | [`one&#124;InfoCard.glow`](../../../TheOne.Avalonia/Themes/Variants.axaml#L108) | one&#124;InfoCard.glow => HeaderBackground={DynamicResource One.Gradient.Glow}; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| `hove` | [`^.hove:pointerover:not(:disabled)`](../../../TheOne.Avalonia/Themes/Controls/InfoCard.axaml#L92) | ^.hove:pointerover:not(:disabled) => BorderBrush={DynamicResource One.Accent}; Background={DynamicResource One.SurfaceHover}; BoxShadow={DynamicResource One.InfoCard.HoverShadow} |
| `outlined` | [`one&#124;InfoCard.outlined`](../../../TheOne.Avalonia/Themes/Variants.axaml#L216) | one&#124;InfoCard.outlined => BorderBrush={DynamicResource One.Accent}; HeaderBackground={DynamicResource One.AccentSoft}; one&#124;InfoCard.outlined:disabled => BorderBrush={DynamicResource One.Line}; HeaderBackground={DynamicResource One.DisabledSurface} |
| `shadow` | [`^.shadow:not(:disabled)`](../../../TheOne.Avalonia/Themes/Controls/InfoCard.axaml#L91) | ^.shadow:not(:disabled) => BoxShadow={DynamicResource One.InfoCard.Shadow} |
