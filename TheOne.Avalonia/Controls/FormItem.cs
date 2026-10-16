using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Layout;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A labelled field with caller-owned native input and shared step validation messages.</summary>
public class FormItem : StepField
{
    public static readonly StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<FormItem, string?>(nameof(Label));
    public static readonly StyledProperty<string?> HelpTextProperty = AvaloniaProperty.Register<FormItem, string?>(nameof(HelpText));
    public static readonly StyledProperty<bool> IsRequiredProperty = AvaloniaProperty.Register<FormItem, bool>(nameof(IsRequired));
    public static readonly StyledProperty<Orientation> LabelOrientationProperty = AvaloniaProperty.Register<FormItem, Orientation>(nameof(LabelOrientation), Orientation.Vertical, inherits: true, validate: Enum.IsDefined);
    public static readonly StyledProperty<double> LabelWidthProperty = AvaloniaProperty.Register<FormItem, double>(nameof(LabelWidth), 120, inherits: true, validate: value => double.IsFinite(value) && value > 0);
    public static readonly DirectProperty<FormItem, Dock> LabelDockProperty = AvaloniaProperty.RegisterDirect<FormItem, Dock>(nameof(LabelDock), item => item.LabelDock);
    public static readonly DirectProperty<FormItem, double> EffectiveLabelWidthProperty = AvaloniaProperty.RegisterDirect<FormItem, double>(nameof(EffectiveLabelWidth), item => item.EffectiveLabelWidth);
    public static readonly DirectProperty<FormItem, bool> HasLabelProperty = AvaloniaProperty.RegisterDirect<FormItem, bool>(nameof(HasLabel), item => item.HasLabel);
    public static readonly DirectProperty<FormItem, bool> HasHelpTextProperty = AvaloniaProperty.RegisterDirect<FormItem, bool>(nameof(HasHelpText), item => item.HasHelpText);
    private Dock _labelDock = Dock.Top;
    private double _effectiveLabelWidth = double.NaN;
    private bool _hasLabel, _hasHelpText;
    private Label? _label;
    private ContentPresenter? _presenter;
    private Control? _labelledInput;
    public string? Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    public string? HelpText { get => GetValue(HelpTextProperty); set => SetValue(HelpTextProperty, value); }
    /// <summary>Visual required marker; validation rules remain owned by the registered validator.</summary>
    public bool IsRequired { get => GetValue(IsRequiredProperty); set => SetValue(IsRequiredProperty, value); }
    public Orientation LabelOrientation { get => GetValue(LabelOrientationProperty); set => SetValue(LabelOrientationProperty, value); }
    public double LabelWidth { get => GetValue(LabelWidthProperty); set => SetValue(LabelWidthProperty, value); }
    public Dock LabelDock => _labelDock;
    public double EffectiveLabelWidth => _effectiveLabelWidth;
    public bool HasLabel => _hasLabel;
    public bool HasHelpText => _hasHelpText;
    protected override Type StyleKeyOverride => typeof(FormItem);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        RemoveLabel(); base.OnApplyTemplate(args);
        _label = args.NameScope.Find<Label>("PART_Label");
        _presenter = args.NameScope.Find<ContentPresenter>("PART_ContentPresenter"); QueueLabel();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    { base.OnAttachedToVisualTree(args); QueueLabel(); }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    { RemoveLabel(); base.OnDetachedFromVisualTree(args); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LabelProperty || change.Property == HelpTextProperty || change.Property == LabelOrientationProperty || change.Property == LabelWidthProperty)
        {
            SetAndRaise(HasLabelProperty, ref _hasLabel, !string.IsNullOrEmpty(Label));
            SetAndRaise(HasHelpTextProperty, ref _hasHelpText, !string.IsNullOrEmpty(HelpText));
            SetAndRaise(LabelDockProperty, ref _labelDock, LabelOrientation == Orientation.Horizontal ? Dock.Left : Dock.Top);
            SetAndRaise(EffectiveLabelWidthProperty, ref _effectiveLabelWidth, LabelOrientation == Orientation.Horizontal ? LabelWidth : double.NaN);
            if (change.Property == LabelProperty) QueueLabel();
        }
        if (change.Property == ContentProperty || change.Property == ContentTemplateProperty || change.Property == MemberNameProperty)
        {
            if (StepValidationVisuals.Owner(this) is Form form) form.ClearValidation();
            QueueLabel();
        }
    }
    private void QueueLabel() => Dispatcher.UIThread.Post(() =>
    {
        if (!this.IsAttachedToVisualTree() || _label is null) return;
        RemoveLabel();
        if (!HasLabel) return;
        var input = _presenter?.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.Focusable
            && ReferenceEquals(control.GetVisualAncestors().OfType<FormItem>().FirstOrDefault(), this));
        _label.Target = input;
        if (input is not null && AutomationProperties.GetLabeledBy(input) is null)
        { AutomationProperties.SetLabeledBy(input, _label); _labelledInput = input; }
    }, DispatcherPriority.Loaded);
    private void RemoveLabel()
    {
        if (_labelledInput is { } input && ReferenceEquals(AutomationProperties.GetLabeledBy(input), _label)) input.ClearValue(AutomationProperties.LabeledByProperty);
        _labelledInput = null;
        if (_label is not null) _label.Target = null;
    }
}
