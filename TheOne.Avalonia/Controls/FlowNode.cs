using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A flow marker with independently templated content and an explicit business state.</summary>
[PseudoClasses(":completed", ":in-progress", ":pending", ":vertical")]
[TemplatePart("PART_Indicator", typeof(Control))]
[TemplatePart("PART_ContentPresenter", typeof(ContentPresenter))]
public class FlowNode : ContentControl
{
    public static readonly StyledProperty<FlowNodeStatus> StatusProperty =
        AvaloniaProperty.Register<FlowNode, FlowNodeStatus>(nameof(Status), validate: Enum.IsDefined);

    public static readonly StyledProperty<Orientation> OrientationProperty =
        FlowStep.OrientationProperty.AddOwner<FlowNode>();

    public static readonly StyledProperty<double> IndicatorSizeProperty =
        AvaloniaProperty.Register<FlowNode, double>(nameof(IndicatorSize), 30,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<double> ContentSpacingProperty =
        AvaloniaProperty.Register<FlowNode, double>(nameof(ContentSpacing), 12,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<IDataTemplate?> DefaultContentTemplateProperty =
        AvaloniaProperty.Register<FlowNode, IDataTemplate?>(nameof(DefaultContentTemplate));

    public static readonly DirectProperty<FlowNode, IDataTemplate?> EffectiveContentTemplateProperty =
        AvaloniaProperty.RegisterDirect<FlowNode, IDataTemplate?>(nameof(EffectiveContentTemplate),
            node => node.EffectiveContentTemplate);

    public static readonly DirectProperty<FlowNode, string> EffectiveStateTextProperty =
        AvaloniaProperty.RegisterDirect<FlowNode, string>(nameof(EffectiveStateText), node => node.EffectiveStateText);

    public static readonly DirectProperty<FlowNode, bool> IsVerticalProperty =
        AvaloniaProperty.RegisterDirect<FlowNode, bool>(nameof(IsVertical), node => node.IsVertical);

    public static readonly DirectProperty<FlowNode, bool> IsCompletedProperty =
        AvaloniaProperty.RegisterDirect<FlowNode, bool>(nameof(IsCompleted), node => node.IsCompleted);

    public static readonly DirectProperty<FlowNode, bool> IsInProgressProperty =
        AvaloniaProperty.RegisterDirect<FlowNode, bool>(nameof(IsInProgress), node => node.IsInProgress);

    private IDataTemplate? _effectiveTemplate;
    private FlowNodeContent? _observedContent;
    private string _stateText = "未完成";
    private bool _vertical, _completed, _inProgress;

    public FlowNode()
    {
        UpdateState();
    }

    internal FlowStep? Owner { get; private set; }
    internal Control? Indicator { get; private set; }

    public FlowNodeStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double IndicatorSize
    {
        get => GetValue(IndicatorSizeProperty);
        set => SetValue(IndicatorSizeProperty, value);
    }

    public double ContentSpacing
    {
        get => GetValue(ContentSpacingProperty);
        set => SetValue(ContentSpacingProperty, value);
    }

    /// <summary>Fallback for FlowNodeContent; OneTheme supplies the default four-field template.</summary>
    public IDataTemplate? DefaultContentTemplate
    {
        get => GetValue(DefaultContentTemplateProperty);
        set => SetValue(DefaultContentTemplateProperty, value);
    }

    public IDataTemplate? EffectiveContentTemplate => _effectiveTemplate;
    public string EffectiveStateText => _stateText;
    public bool IsVertical => _vertical;
    public bool IsCompleted => _completed;
    public bool IsInProgress => _inProgress;
    protected override Type StyleKeyOverride => typeof(FlowNode);

    internal void SetOwner(FlowStep? owner)
    {
        Owner = owner;
        UpdateContentTemplate();
    }

    internal void UpdateContentTemplate()
    {
        var template = ContentTemplate ?? Owner?.ContentTemplate ?? Owner?.ItemTemplate;
        if (template is null && Content is not Control && Content is not null)
            template = this.FindDataTemplate(Content) ?? (Content is FlowNodeContent ? DefaultContentTemplate : null);
        SetAndRaise(EffectiveContentTemplateProperty, ref _effectiveTemplate, template);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        base.OnApplyTemplate(args);
        Indicator = args.NameScope.Find<Control>("PART_Indicator");
        UpdateContentTemplate();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        ObserveContent();
        UpdateState();
        UpdateContentTemplate();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        UnobserveContent();
        base.OnDetachedFromVisualTree(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentProperty)
        {
            ObserveContent();
            UpdateState();
            UpdateContentTemplate();
        }

        if (change.Property == ContentTemplateProperty || change.Property == DefaultContentTemplateProperty)
            UpdateContentTemplate();
        if (change.Property == StatusProperty || change.Property == OrientationProperty)
        {
            UpdateState();
            Owner?.InvalidateConnectors();
        }
    }

    private void ObserveContent()
    {
        UnobserveContent();
        if (!this.IsAttachedToVisualTree() || Content is not FlowNodeContent content) return;
        _observedContent = content;
        content.PropertyChanged += ContentChanged;
    }

    private void UnobserveContent()
    {
        if (_observedContent is { } content) content.PropertyChanged -= ContentChanged;
        _observedContent = null;
    }

    private void ContentChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is null or "" or nameof(FlowNodeContent.State)) UpdateState();
    }

    private void UpdateState()
    {
        SetAndRaise(IsVerticalProperty, ref _vertical, Orientation == Orientation.Vertical);
        SetAndRaise(IsCompletedProperty, ref _completed, Status == FlowNodeStatus.Completed);
        SetAndRaise(IsInProgressProperty, ref _inProgress, Status == FlowNodeStatus.InProgress);
        var state = Content is FlowNodeContent content && !string.IsNullOrWhiteSpace(content.State)
            ? content.State!
            : Status switch { FlowNodeStatus.Completed => "已完成", FlowNodeStatus.InProgress => "进行中", _ => "未完成" };
        SetAndRaise(EffectiveStateTextProperty, ref _stateText, state);
        PseudoClasses.Set(":completed", _completed);
        PseudoClasses.Set(":in-progress", _inProgress);
        PseudoClasses.Set(":pending", Status == FlowNodeStatus.Pending);
        PseudoClasses.Set(":vertical", _vertical);
    }
}