using Avalonia;

namespace TheOne.Avalonia.Controls;

/// <summary>Optional observable event data; Key remains caller-owned identity. Timestamp is displayed, never sorted or formatted implicitly.</summary>
public sealed class TimelineItemModel : AvaloniaObject
{
    public static readonly StyledProperty<object?> KeyProperty = AvaloniaProperty.Register<TimelineItemModel, object?>(nameof(Key));
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<TimelineItemModel, object?>(nameof(Header));
    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<TimelineItemModel, object?>(nameof(Content));
    public static readonly StyledProperty<object?> TimestampProperty = AvaloniaProperty.Register<TimelineItemModel, object?>(nameof(Timestamp));
    public static readonly StyledProperty<object?> DotProperty = AvaloniaProperty.Register<TimelineItemModel, object?>(nameof(Dot));
    public static readonly StyledProperty<TimelineStatus> StatusProperty = AvaloniaProperty.Register<TimelineItemModel, TimelineStatus>(nameof(Status), validate: Enum.IsDefined);
    public object? Key { get => GetValue(KeyProperty); set => SetValue(KeyProperty, value); }
    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public object? Content { get => GetValue(ContentProperty); set => SetValue(ContentProperty, value); }
    public object? Timestamp { get => GetValue(TimestampProperty); set => SetValue(TimestampProperty, value); }
    public object? Dot { get => GetValue(DotProperty); set => SetValue(DotProperty, value); }
    public TimelineStatus Status { get => GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
}
