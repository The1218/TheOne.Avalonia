using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed record RefreshRow(int Number, string Title, int Revision);

public sealed class RefreshDemoViewModel : ReactiveObject
{
    public IReadOnlyList<RefreshRow> Rows
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = CreateRows(0);

    public int SelectedIndex
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = -1;

    public bool Enabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool MousePull
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool Manual
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool Fail
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool MotionEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "尚未刷新";

    public string Metrics
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string State
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "等待下拉";

    public int RequestCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int Revision
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int FailureCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public static RefreshRow[] CreateRows(int revision)
    {
        return Enumerable.Range(1, 1000).Select(number => new RefreshRow(number, $"工作区记录 {number:0000}", revision))
            .ToArray();
    }
}