# MultiSelectItemAdapter

[English](../en/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.MultiSelectItemAdapter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.MultiSelectItemAdapter` · Class · `object`

Explicit, trimming-safe text and availability accessors for model-backed choices.

[Source / 声明](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L8)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`abstract string MultiSelectItemAdapter.GetText(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L10) | Reads original-model display/search text through the configured typed adapter. |
| [`virtual bool MultiSelectItemAdapter.IsEnabled(object item)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L12) | Uses the adapter to determine whether an original model permits user selection. |
| [`static MultiSelectItemAdapter MultiSelectItemAdapter.Create<T>(Func<T, string> text, Func<T, bool>? isEnabled = null)`](../../../TheOne.Avalonia/Controls/MultiSelectItemAdapter.cs#L17) | Creates an adapter/source from explicit types, models and delegates; typed getters preserve compile-time information. |
