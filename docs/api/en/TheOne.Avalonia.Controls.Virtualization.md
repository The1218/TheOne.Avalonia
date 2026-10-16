# Virtualization

[English](../en/TheOne.Avalonia.Controls.Virtualization.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Virtualization.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Virtualization` · Class · `Avalonia.AvaloniaObject`

Viewport-aware selection and navigation for native tab and hierarchical controls.

[Source / 声明](../../../TheOne.Avalonia/Controls/Virtualization.cs#L13)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | `bool` | writable; OneWay; attached | `false` | Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`static bool Virtualization.GetEnabled(ItemsControl control)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L33) | Reads the attached property. Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls. |
| [`static void Virtualization.SetEnabled(ItemsControl control, bool value)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L34) | Sets the attached property. Enables supported virtualization; still requires a bounded viewport and model source rather than precreated controls. |
| [`static TreeViewItem? Virtualization.BringIntoView(TreeView tree, params int[] path)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L55) | Materializes only the ancestor path and target of a large tree. |
| [`static void Virtualization.SelectAll(TreeView tree)`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L75) | Selects all nodes exposed by expanded branches without creating their containers. |
| [`Virtualization.Virtualization()`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L13) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly AttachedProperty<bool> Virtualization.EnabledProperty`](../../../TheOne.Avalonia/Controls/Virtualization.cs#L15) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
