# WorkflowChoiceAdapter

[English](../en/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) · [简体中文](../zh-cn/TheOne.Avalonia.Workflow.WorkflowChoiceAdapter.md) · [API index](README.md)

`TheOne.Avalonia.Workflow.WorkflowChoiceAdapter` · Class · `object`

Compiled host projections for stable user/role keys and display text, without property reflection.

[Source / 声明](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L4)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

No additional CLR/attached properties are declared on this type; see its base for inherited members.

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`abstract string WorkflowChoiceAdapter.GetKey(object item)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L6) | The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries. |
| [`abstract string WorkflowChoiceAdapter.GetText(object item)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L7) | Reads original-model display/search text through the configured typed adapter. |
| [`static WorkflowChoiceAdapter WorkflowChoiceAdapter.Create<T>(Func<T, string> key, Func<T, string> text)`](../../../TheOne.Avalonia/Workflow/WorkflowChoice.cs#L9) | Creates an adapter/source from explicit types, models and delegates; typed getters preserve compile-time information. |
