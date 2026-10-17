using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;

namespace TheOne.Avalonia.Controls;

/// <summary>A linear, non-selecting flow display with native content containers.</summary>
public class FlowStep : ItemsControl
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<FlowStep, Orientation>(nameof(Orientation), Orientation.Horizontal, true,
            validate: Enum.IsDefined);

    public static readonly StyledProperty<double> NodeSpacingProperty =
        AvaloniaProperty.Register<FlowStep, double>(nameof(NodeSpacing), 24, true,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<double> MinNodeWidthProperty =
        AvaloniaProperty.Register<FlowStep, double>(nameof(MinNodeWidth), 180, true,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<FlowStep, IDataTemplate?>(nameof(ContentTemplate));

    private readonly Dictionary<Control, List<IDisposable>> _bindings = [];
    private FlowStepConnectors? _connectors;

    public FlowStep()
    {
        ContainerPrepared += (_, args) =>
        {
            if (args.Container is FlowNode node) node.SetOwner(this);
        };
        ContainerClearing += (_, args) =>
        {
            DisposeBindings(args.Container);
            if (args.Container is FlowNode node) node.SetOwner(null);
        };
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public double NodeSpacing
    {
        get => GetValue(NodeSpacingProperty);
        set => SetValue(NodeSpacingProperty, value);
    }

    public double MinNodeWidth
    {
        get => GetValue(MinNodeWidthProperty);
        set => SetValue(MinNodeWidthProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(FlowStep);

    internal void InvalidateConnectors()
    {
        _connectors?.InvalidateVisual();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        base.OnApplyTemplate(args);
        _connectors = args.NameScope.Find<FlowStepConnectors>("PART_Connectors");
        if (_connectors is not null) _connectors.Owner = this;
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new FlowNode();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<FlowNode>(item, out recycleKey);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        DisposeBindings(container);
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is not FlowNode node) return;
        node.SetOwner(this);
        if (item is not FlowNodeItem model) return;
        _bindings[container] =
        [
            node.Bind(FlowNode.StatusProperty,
                CompiledBinding.Create<FlowNodeItem, FlowNodeStatus>(value => value.Status, model)),
            node.Bind(ContentControl.ContentProperty,
                CompiledBinding.Create<FlowNodeItem, object?>(value => value.Content, model)),
            node.Bind(ContentControl.ContentTemplateProperty,
                CompiledBinding.Create<FlowNodeItem, IDataTemplate?>(value => value.ContentTemplate, model))
        ];
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        DisposeBindings(container);
        if (container is FlowNode node) node.SetOwner(null);
        base.ClearContainerForItemOverride(container);
    }

    private void DisposeBindings(Control container)
    {
        if (_bindings.Remove(container, out var bindings))
            foreach (var binding in bindings)
                binding.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentTemplateProperty || change.Property == ItemTemplateProperty)
            foreach (var node in GetRealizedContainers().OfType<FlowNode>())
                node.UpdateContentTemplate();
    }
}