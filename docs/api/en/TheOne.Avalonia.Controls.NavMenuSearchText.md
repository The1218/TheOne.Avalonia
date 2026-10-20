# NavMenuSearchText

[English](../en/TheOne.Avalonia.Controls.NavMenuSearchText.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.NavMenuSearchText.md) · [API index](README.md)

`TheOne.Avalonia.Controls.NavMenuSearchText` · Class · `Avalonia.Controls.TextBlock`

Search-aware text for NavMenu content templates. SourceText remains bindable when inlines are rebuilt.

[Source / 声明](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L9)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`HighlightQuery`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L14) | `string?` | writable; OneWay; attached; inherits | `null` | Query highlighted within source text; affects rendering without changing SourceText. |
| [`SourceText`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L17) | `string?` | writable; OneWay | `null` | Full unhighlighted source text, without rich-text markup. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static string? NavMenuSearchText.GetHighlightQuery(Control control)`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L25) | Reads the attached property. Query highlighted within source text; affects rendering without changing SourceText. |
| [`static void NavMenuSearchText.SetHighlightQuery(Control control, string? value)`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L30) | Sets the attached property. Query highlighted within source text; affects rendering without changing SourceText. |
| [`NavMenuSearchText.NavMenuSearchText()`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L9) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> NavMenuSearchText.SourceTextProperty`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L11) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly AttachedProperty<string?> NavMenuSearchText.HighlightQueryProperty`](../../../TheOne.Avalonia/Controls/NavMenuSearchText.cs#L14) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.
