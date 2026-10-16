using global::Avalonia.Controls;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Shares field ownership and native conversion errors between forms and steps.</summary>
internal static class StepValidationVisuals
{
    internal static Control? Owner(Control control) => control.GetVisualAncestors().OfType<Control>()
        .FirstOrDefault(ancestor => ancestor is Form or VerificationStep);

    internal static string MemberOf(Control control)
    {
        var field = control.GetVisualAncestors().TakeWhile(ancestor => ancestor is not Form && ancestor is not VerificationStep).OfType<StepField>().FirstOrDefault();
        return !string.IsNullOrEmpty(field?.MemberName) ? field.MemberName : control.Name ?? "";
    }

    internal static IEnumerable<StepValidationError> NativeErrors(IEnumerable<Control> controls)
    {
        foreach (var control in controls.Where(DataValidationErrors.GetHasErrors))
            foreach (var error in DataValidationErrors.GetErrors(control)?.Cast<object>() ?? [])
                yield return new(MemberOf(control), error is Exception exception ? exception.GetBaseException().Message : error.ToString() ?? "Invalid input.");
    }
}
