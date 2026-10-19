using TheOne.Avalonia.Workflow;

namespace TheOne.Avalonia.Controls;

/// <summary>Display adapter used by the workflow's built-in multi-choice inspector.</summary>
internal sealed class WorkflowChoiceTextAdapter : MultiSelectItemAdapter
{
    public override string GetText(object item) => item is WorkflowChoice choice ? choice.Text : "";
}
