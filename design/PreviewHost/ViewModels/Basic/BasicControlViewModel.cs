using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public class BasicControlViewModel : ReactiveObject
{
    private int _invocations;

    public BasicControlViewModel()
    {
        InvokeCommand =
            ReactiveCommand.Create<object?>(parameter => Status = $"已执行 {++_invocations} 次 · {parameter ?? Text}");
        LastCommand = ReactiveCommand.Create<object?>(_ =>
        {
            SelectedIndex = 999;
            RequestScroll(999);
        });
    }

    public string Text
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "TheOne · 示例文本";

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "修改控件以检查 ViewModel 双向绑定";

    public double Value
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 64;

    public decimal? Number
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 28;

    public bool? Checked
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = false;

    public bool On
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool Expanded
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool Enabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public int SelectedIndex
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public object? SelectedItem
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public DateTimeOffset? Date
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new DateTimeOffset(2026, 10, 13, 0, 0, 0, TimeSpan.FromHours(8));

    public DateTime? CalendarDate
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new DateTime(2026, 10, 13);

    public TimeSpan? Time
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new TimeSpan(14, 30, 0);

    public IReadOnlyList<string> Items
    {
        get => field ??= Enumerable.Range(1, 1000).Select(index => $"工作区 {index:0000}").ToArray();
    } = null!;

    public PreviewTreeNode[] TreeRoots
    {
        get => field ??= Enumerable.Range(1, 1000).Select(index => new PreviewTreeNode($"节点 {index:0000}", "独立模型与回收容器",
            true,
            index == 1
                ? Enumerable.Range(1, 1000).Select(child => new PreviewTreeNode($"子节点 {child:0000}", "可增长子集合"))
                    .ToArray()
                : [])).ToArray();
    } = null!;

    public ICommand InvokeCommand { get; }
    public ICommand LastCommand { get; }

    public event EventHandler<int>? ScrollRequested;

    protected void RequestScroll(int index)
    {
        ScrollRequested?.Invoke(this, index);
    }
}