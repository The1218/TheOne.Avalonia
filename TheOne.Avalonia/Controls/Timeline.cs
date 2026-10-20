using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace TheOne.Avalonia.Controls;

/// <summary>The side occupied by event content; Left places the rail on the left.</summary>
public enum TimelineMode { Left, Right, Alternate }

/// <summary>Semantic event state. Pending is a static hollow marker, not a progress animation.</summary>
public enum TimelineStatus { Default, Success, Warning, Error, Pending }

/// <summary>A naturally sized vertical event history. Collection order is caller-owned.</summary>
public class Timeline : ItemsControl
{
    public static readonly StyledProperty<TimelineMode> ModeProperty =
        AvaloniaProperty.Register<Timeline, TimelineMode>(nameof(Mode), validate: Enum.IsDefined);
    public static readonly StyledProperty<bool> ReverseProperty =
        AvaloniaProperty.Register<Timeline, bool>(nameof(Reverse), inherits: true);
    public static readonly StyledProperty<double> ItemSpacingProperty =
        AvaloniaProperty.Register<Timeline, double>(nameof(ItemSpacing), 16, inherits: true,
            validate: value => double.IsFinite(value) && value >= 0);

    private readonly Dictionary<Control, List<IDisposable>> _bindings = [];

    public Timeline() => ItemsView.CollectionChanged += (_, _) => RefreshPositions();

    /// <summary>Left/right rail placement, or alternating content around a centered rail.</summary>
    public TimelineMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    /// <summary>Reverses visual order without mutating Items or ItemsSource.</summary>
    public bool Reverse { get => GetValue(ReverseProperty); set => SetValue(ReverseProperty, value); }
    /// <summary>Space after each event, excluding the visually last event.</summary>
    public double ItemSpacing { get => GetValue(ItemSpacingProperty); set => SetValue(ItemSpacingProperty, value); }
    protected override Type StyleKeyOverride => typeof(Timeline);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) => new TimelineItem();
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) => NeedsContainer<TimelineItem>(item, out recycleKey);

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        ClearBindings(container);
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is not TimelineItem row) return;
        if (!ReferenceEquals(row, item))
        {
            // Native ItemsControl puts ItemTemplate on a HeaderedContentControl's header.
            // Timeline deliberately reserves ItemTemplate for the event body.
            row.ClearValue(HeaderedContentControl.HeaderProperty);
            row.ClearValue(HeaderedContentControl.HeaderTemplateProperty);
            row.ClearValue(ContentControl.ContentTemplateProperty);
            if (ItemTemplate is not null) row.SetCurrentValue(ContentControl.ContentTemplateProperty, ItemTemplate);
            row.ClearValue(TimelineItem.TimestampProperty);
            row.ClearValue(TimelineItem.DotProperty);
            row.ClearValue(TimelineItem.StatusProperty);
            if (item is TimelineItemModel model)
            {
                var bindings = _bindings[container] =
                [
                    row.Bind(HeaderedContentControl.HeaderProperty, CompiledBinding.Create<TimelineItemModel, object?>(m => m.Header, model)),
                    row.Bind(TimelineItem.TimestampProperty, CompiledBinding.Create<TimelineItemModel, object?>(m => m.Timestamp, model)),
                    row.Bind(TimelineItem.DotProperty, CompiledBinding.Create<TimelineItemModel, object?>(m => m.Dot, model)),
                    row.Bind(TimelineItem.StatusProperty, CompiledBinding.Create<TimelineItemModel, TimelineStatus>(m => m.Status, model))
                ];
                // ItemTemplate, when supplied, receives the original model as in native ItemsControl.
                if (ItemTemplate is null)
                    bindings.Add(row.Bind(ContentControl.ContentProperty, CompiledBinding.Create<TimelineItemModel, object?>(m => m.Content, model)));
            }
        }
        Configure(row, index);
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        RefreshPositions();
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        if (container is TimelineItem row) Configure(row, newIndex);
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        ClearBindings(container);
        if (container is TimelineItem row) row.Configure(TimelineMode.Left, false, true);
        base.ClearContainerForItemOverride(container);
    }

    private void ClearBindings(Control container)
    {
        if (_bindings.Remove(container, out var bindings)) foreach (var binding in bindings) binding.Dispose();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty || change.Property == ReverseProperty) RefreshPositions();
    }

    private void Configure(TimelineItem row, int index)
    {
        var visualIndex = Reverse ? ItemCount - 1 - index : index;
        row.Configure(Mode, visualIndex % 2 != 0, visualIndex == ItemCount - 1);
    }

    private void RefreshPositions()
    {
        foreach (var row in GetRealizedContainers().OfType<TimelineItem>()) Configure(row, IndexFromContainer(row));
    }
}

/// <summary>Default Timeline panel. Reverses arrangement without copying or resetting the data source.</summary>
public sealed class TimelinePanel : Panel
{
    static TimelinePanel() => AffectsArrange<TimelinePanel>(Timeline.ReverseProperty);
    protected override Size MeasureOverride(Size availableSize)
    {
        double width = 0, height = 0;
        foreach (var child in Children)
        {
            child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            width = Math.Max(width, child.DesiredSize.Width);
            height += child.DesiredSize.Height;
        }
        return new Size(width, height);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var reverse = GetValue(Timeline.ReverseProperty);
        double y = 0;
        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[reverse ? Children.Count - 1 - i : i];
            child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
            y += child.DesiredSize.Height;
        }
        return finalSize;
    }
}
