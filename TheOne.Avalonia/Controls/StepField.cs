using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Places a step member's validation messages directly below its caller-owned input.</summary>
public class StepField : ContentControl
{
    public static readonly StyledProperty<string> MemberNameProperty = AvaloniaProperty.Register<StepField, string>(nameof(MemberName), "");
    public static readonly DirectProperty<StepField, IReadOnlyList<StepValidationError>> ErrorsProperty = AvaloniaProperty.RegisterDirect<StepField, IReadOnlyList<StepValidationError>>(nameof(Errors), field => field.Errors);
    public static readonly DirectProperty<StepField, bool> HasErrorsProperty = AvaloniaProperty.RegisterDirect<StepField, bool>(nameof(HasErrors), field => field.HasErrors);
    private IReadOnlyList<StepValidationError> _errors = [];
    private bool _hasErrors;
    public string MemberName { get => GetValue(MemberNameProperty); set => SetValue(MemberNameProperty, value); }
    public IReadOnlyList<StepValidationError> Errors => _errors;
    public bool HasErrors => _hasErrors;
    protected override Type StyleKeyOverride => typeof(StepField);
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        this.GetVisualAncestors().OfType<VerificationStep>().FirstOrDefault()?.RefreshFieldErrors();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        SetErrors([]);
        base.OnDetachedFromVisualTree(args);
    }
    internal void SetErrors(IReadOnlyList<StepValidationError> errors)
    {
        SetAndRaise(ErrorsProperty, ref _errors, errors);
        SetAndRaise(HasErrorsProperty, ref _hasErrors, errors.Count > 0);
    }
}
