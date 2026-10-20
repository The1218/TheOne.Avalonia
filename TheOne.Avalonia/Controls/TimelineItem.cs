using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>An event with a title, body, timestamp and semantic or custom marker.</summary>
[PseudoClasses(":last", ":custom-dot", ":success", ":warning", ":error", ":pending", ":has-header")]
public class TimelineItem : HeaderedContentControl
{
    public static readonly StyledProperty<object?> TimestampProperty = AvaloniaProperty.Register<TimelineItem, object?>(nameof(Timestamp));
    public static readonly StyledProperty<IDataTemplate?> TimestampTemplateProperty = AvaloniaProperty.Register<TimelineItem, IDataTemplate?>(nameof(TimestampTemplate));
    public static readonly StyledProperty<object?> DotProperty = AvaloniaProperty.Register<TimelineItem, object?>(nameof(Dot));
    public static readonly StyledProperty<IDataTemplate?> DotTemplateProperty = AvaloniaProperty.Register<TimelineItem, IDataTemplate?>(nameof(DotTemplate));
    public static readonly StyledProperty<TimelineStatus> StatusProperty = AvaloniaProperty.Register<TimelineItem, TimelineStatus>(nameof(Status), validate: Enum.IsDefined);
    public static readonly StyledProperty<double> ItemSpacingProperty = Timeline.ItemSpacingProperty.AddOwner<TimelineItem>();

    private Grid? _layout;
    private Control? _body, _timestamp;
    private TimelineMode _mode;
    private bool _odd, _last = true;

    public TimelineItem() => UpdateState();
    public object? Timestamp { get => GetValue(TimestampProperty); set => SetValue(TimestampProperty, value); }
    public IDataTemplate? TimestampTemplate { get => GetValue(TimestampTemplateProperty); set => SetValue(TimestampTemplateProperty, value); }
    public object? Dot { get => GetValue(DotProperty); set => SetValue(DotProperty, value); }
    public IDataTemplate? DotTemplate { get => GetValue(DotTemplateProperty); set => SetValue(DotTemplateProperty, value); }
    public TimelineStatus Status { get => GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
    public double ItemSpacing { get => GetValue(ItemSpacingProperty); set => SetValue(ItemSpacingProperty, value); }
    protected override Type StyleKeyOverride => typeof(TimelineItem);

    internal void Configure(TimelineMode mode, bool odd, bool last)
    {
        _mode = mode; _odd = odd; _last = last;
        PseudoClasses.Set(":last", last);
        UpdateLayoutParts();
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _layout = e.NameScope.Find<Grid>("PART_Layout");
        _body = e.NameScope.Find<Control>("PART_Body");
        _timestamp = e.NameScope.Find<Control>("PART_Timestamp");
        UpdateState();
        UpdateLayoutParts();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StatusProperty || change.Property == DotProperty || change.Property == HeaderProperty) UpdateState();
        if (change.Property == ItemSpacingProperty || change.Property == TimestampProperty) UpdateLayoutParts();
    }
    private void UpdateState()
    {
        PseudoClasses.Set(":custom-dot", Dot is not null);
        PseudoClasses.Set(":has-header", Header is not null);
        PseudoClasses.Set(":success", Status == TimelineStatus.Success);
        PseudoClasses.Set(":warning", Status == TimelineStatus.Warning);
        PseudoClasses.Set(":error", Status == TimelineStatus.Error);
        PseudoClasses.Set(":pending", Status == TimelineStatus.Pending);
    }
    private void UpdateLayoutParts()
    {
        if (_layout is null || _body is null || _timestamp is null) return;
        var alternate = _mode == TimelineMode.Alternate;
        var contentLeft = _mode == TimelineMode.Right || alternate && _odd;
        _layout.ColumnDefinitions[0].Width = contentLeft || alternate ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        _layout.ColumnDefinitions[2].Width = !contentLeft || alternate ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
        var column = contentLeft ? 0 : 2;
        Grid.SetColumn(_body, column);
        Grid.SetRow(_body, alternate ? 0 : 1);
        Grid.SetRowSpan(_body, alternate ? 2 : 1);
        _body.Margin = new Thickness(contentLeft ? 0 : 8, 0, contentLeft ? 8 : 0, _last ? 0 : ItemSpacing);
        _body.SetValue(TextBlock.TextAlignmentProperty, contentLeft ? TextAlignment.Right : TextAlignment.Left);
        Grid.SetColumn(_timestamp, alternate ? 2 - column : column);
        Grid.SetRowSpan(_timestamp, alternate ? 2 : 1);
        _timestamp.Margin = alternate ? new Thickness(contentLeft ? 8 : 0, 0, contentLeft ? 0 : 8, _last ? 0 : ItemSpacing) : new Thickness(contentLeft ? 0 : 8, 0, contentLeft ? 8 : 0, Timestamp is null ? 0 : 4);
        _timestamp.SetValue(TextBlock.TextAlignmentProperty, alternate ? (contentLeft ? TextAlignment.Left : TextAlignment.Right) : (contentLeft ? TextAlignment.Right : TextAlignment.Left));
        _timestamp.IsVisible = Timestamp is not null;
        _timestamp.VerticalAlignment = VerticalAlignment.Top;
    }
}
