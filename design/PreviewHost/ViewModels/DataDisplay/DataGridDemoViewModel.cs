using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Collections;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class DataGridDemoViewModel : ReactiveObject
{
    public DataGridDemoViewModel()
    {
        _ = DataGridAot.PreserveRowType<GridRecord>();
        View = new DataGridCollectionView<GridRecord>(Records);
        View.CollectionChanged += (_, _) => NotifyViewChanged();
        FirstCommand = ReactiveCommand.Create<object?>(_ => Select(0));
        MiddleCommand = ReactiveCommand.Create<object?>(_ => Select(View.Count / 2));
        LastCommand = ReactiveCommand.Create<object?>(_ => Select(View.Count - 1));
        FilterCommand = ReactiveCommand.Create<object?>(_ => ApplyFilter());
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            if (!View.CanFilter)
                return;
            FilterText = "";
            View.Filter = null;
            View.SortDescriptions.Clear();
            Selected = null;
            this.RaisePropertyChanged(nameof(CountLabel));
            ResetRequested?.Invoke();
        });
    }

    public GridRecord[] Records { get; } = CreateRecords();
    public DataGridCollectionView<GridRecord> View { get; }
    public ICommand FirstCommand { get; }
    public ICommand MiddleCommand { get; }
    public ICommand LastCommand { get; }
    public ICommand FilterCommand { get; }
    public ICommand ResetCommand { get; }

    public GridRecord? Selected
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
                return;
            if (field is not null)
                field.PropertyChanged -= SelectedChanged;
            this.RaiseAndSetIfChanged(ref field, value);
            if (field is not null)
                field.PropertyChanged += SelectedChanged;
            this.RaisePropertyChanged(nameof(SelectedLabel));
        }
    }

    public bool ShowSummary
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int FrozenRightCount
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string FilterText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Metrics
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public bool IsReadOnly
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsInteractive
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public string CountLabel => $"{View.Count:N0} / {Records.Length:N0} 行";

    public string SelectedLabel =>
        Selected is { } row ? $"已选择 #{row.Id:00000} · {row.Name} · {row.Warehouse}" : "未选择 · 点击单元格或使用定位按钮";

    private void SelectedChanged(object? sender, PropertyChangedEventArgs args)
    {
        this.RaisePropertyChanged(nameof(SelectedLabel));
    }

    private void Select(int index)
    {
        if (index >= 0 && index < View.Count)
            Selected = (GridRecord)View[index];
    }

    private void ApplyFilter()
    {
        if (!View.CanFilter)
            return;
        var query = FilterText.Trim();
        Selected = null;
        View.Filter = string.IsNullOrEmpty(query)
            ? null
            : item => item is GridRecord row && (row.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                 row.Warehouse.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                 row.Id.ToString().Contains(query));
        this.RaisePropertyChanged(nameof(CountLabel));
    }

    public static GridRecord[] CreateRecords()
    {
        string[] products = ["便携显示器", "无线键盘", "桌面扩展坞", "会议摄像头", "降噪耳机", "智能灯具"];
        string[] warehouses = ["上海仓", "杭州仓", "深圳仓", "北京仓"];
        return Enumerable.Range(1, 10000).Select(id => new GridRecord(id,
            $"{products[(id - 1) % products.Length]} {id:00000}", warehouses[(id - 1) % warehouses.Length], id % 80 + 1,
            99m + id % 1200 + 0.5m, id % 5 != 0,
            id == 6 ? "这是一条较长的备注：列宽按表头和可见内容自适应；达到最大列宽后自动换行，行高随内容增长，同时保留一万条数据的按需显示与回收。" : "标准库存 · 可编辑")).ToArray();
    }

    public event Action? ResetRequested;

    public void NotifyViewChanged()
    {
        this.RaisePropertyChanged(nameof(CountLabel));
    }
}