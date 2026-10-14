using TheOne.Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;

namespace TheOne.DesignPreview;

public sealed record ControlVariantsRow(string Name, string Category, string State);

public sealed class ControlVariantsFixtureModel : ReactiveObject
{
    public IReadOnlyList<ControlVariantsRow> Rows { get; } =
    [
        new("Button", "操作", "默认 / soft / gradient"),
        new("TextBox", "输入", "默认 / filled / 校验"),
        new("DataGrid", "数据", "默认 / borderless"),
        new("Tag", "反馈", "Soft / Outline / Solid")
    ];
    public IList<ChartSeries> Series { get; } = Enumerable.Range(0, 6).Select(index => new ChartSeries
    {
        Name = $"系列 {index + 1}",
        ItemsSource = Enumerable.Range(0, 5).Select(point => new ChartPoint(point, 8 + index * 5 + ((point + index) % 3) * 3)).ToArray()
    }).ToArray();

    public DateTimeOffset? Date { get; set => this.RaiseAndSetIfChanged(ref field, value); } = DateTimeOffset.Now;
    public DateTime? CalendarDate { get; set => this.RaiseAndSetIfChanged(ref field, value); } = DateTime.Today;
    public TimeSpan? Time { get; set => this.RaiseAndSetIfChanged(ref field, value); } = new TimeSpan(14, 30, 0);
    private int _invocations;
    public string Text { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "TheOne workspace";
    public decimal? Number { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 28;
    public int SelectedIndex { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public string Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "点击操作以检查命令绑定";
    public IReadOnlyList<string> Items { get; } = Enumerable.Range(1, 1000).Select(index => $"工作区 {index:0000}").ToArray();
    public ICommand InvokeCommand { get; }
    public ControlVariantsFixtureModel()
    {
        DataGridAot.PreserveRowType<ControlVariantsRow>();
        InvokeCommand = ReactiveCommand.Create<object?>(parameter => Status = $"已执行 {++_invocations} 次 · {parameter ?? "操作"}");
    }
}
