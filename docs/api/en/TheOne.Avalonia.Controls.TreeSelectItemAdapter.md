# TreeSelectItemAdapter

[English](../en/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TreeSelectItemAdapter` · Class · `object`

Explicit, trimming-safe mapping from application models to tree identities and children.

[Source / 声明](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L6)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`abstract object TreeSelectItemAdapter.GetKey(object item)`](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L8) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`abstract string TreeSelectItemAdapter.GetText(object item)`](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L9) | Reads original-model display/search text through the configured typed adapter. |
| [`abstract IEnumerable? TreeSelectItemAdapter.GetChildren(object item)`](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L10) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`virtual bool TreeSelectItemAdapter.IsEnabled(object item)`](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L11) | Uses the adapter to determine whether an original model permits user selection. |
| [`static TreeSelectItemAdapter TreeSelectItemAdapter.Create<T>(Func<T, object> key, Func<T, string> text, Func<T, IEnumerable?> children, Func<T, bool>? isEnabled = null)`](../../../TheOne.Avalonia/Controls/TreeSelectItemAdapter.cs#L13) | Creates an adapter/source from explicit types, models and delegates; typed getters preserve compile-time information. |
