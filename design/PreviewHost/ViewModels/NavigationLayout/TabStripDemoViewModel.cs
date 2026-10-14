using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class TabStripDemoViewModel : ReactiveObject
{
    public IReadOnlyList<string> Rows { get; } = Enumerable.Range(1, 10000).Select(index => $"工作区 {index:00000}").ToArray();

    public int Workflow
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(WorkflowStatus));
        }
    }

    public int SelectedIndex
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(SelectedTitle));
        }
    } = -1;

    public bool Enabled { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public decimal? Target { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 9001;
    public string Metrics { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public string WorkflowStatus => Workflow == 1 ? "动态：查看最近操作记录" : "概览：查看工作区摘要";
    public string SelectedTitle => SelectedIndex >= 0 && SelectedIndex < Rows.Count ? Rows[SelectedIndex] : "尚未选择";
    public ICommand JumpCommand { get; }
    public ICommand LastCommand { get; }
    public ICommand ResetCommand { get; }

    public event EventHandler<int>? ScrollRequested;
    public TabStripDemoViewModel()
    {
        JumpCommand = ReactiveCommand.Create<object?>(_ => Select((int)(Target ?? 1) - 1));
        LastCommand = ReactiveCommand.Create<object?>(_ => Select(Rows.Count - 1));
        ResetCommand = ReactiveCommand.Create<object?>(_ => Select(0));
    }

    private void Select(int index)
    {
        SelectedIndex = Math.Clamp(index, 0, Rows.Count - 1);
        ScrollRequested?.Invoke(this, SelectedIndex);
    }
}
