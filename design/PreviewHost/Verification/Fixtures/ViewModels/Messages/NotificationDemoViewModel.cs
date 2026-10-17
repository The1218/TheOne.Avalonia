using Avalonia.Controls.Notifications;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class NotificationDemoViewModel : ReactiveObject
{
    public bool AutoClose
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool MotionEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public decimal? Seconds
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 2;

    public int PositionIndex
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(Position));
        }
    } = 1;

    public NotificationPosition Position => PositionIndex switch
    {
        0 => NotificationPosition.TopLeft,
        2 => NotificationPosition.TopCenter,
        3 => NotificationPosition.BottomLeft,
        4 => NotificationPosition.BottomRight,
        5 => NotificationPosition.BottomCenter,
        _ => NotificationPosition.TopRight
    };

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "尚未显示通知";

    public string Metrics
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public int ShownCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int ClosedCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int ClickedCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public TimeSpan Duration => AutoClose ? TimeSpan.FromSeconds((double)(Seconds ?? 2)) : TimeSpan.Zero;
}