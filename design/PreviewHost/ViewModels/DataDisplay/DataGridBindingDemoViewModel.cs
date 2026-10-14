using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Collections;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class DataGridBindingDemoViewModel : ReactiveObject
{
    public DataGridBindingDemoViewModel()
    {
        _ = DataGridAot.PreserveRowType<GridRecord>();
        ObservableRows = new ObservableCollection<GridRecord>(ListRows);
        TypedView = new DataGridCollectionView<GridRecord>(ListRows);
        ObservableRows.CollectionChanged += (_, _) => this.RaisePropertyChanged(nameof(SourceLabel));
        UseListCommand = ReactiveCommand.Create<object?>(_ => SelectSource(false));
        UseObservableCommand = ReactiveCommand.Create<object?>(_ => SelectSource(true));
        AddCommand = ReactiveCommand.Create<object?>(_ =>
        {
            if (CanAdd)
                ObservableRows.Add(new GridRecord(ObservableRows.Count + 1, "新增设备", "上海仓", 1, 99m, true, "动态增加"));
        });
        InspectCommand = ReactiveCommand.Create<object?>(row =>
        {
            if (row is GridRecord record)
                ActionLabel = $"查看 #{record.Id:0000} · {record.Name}";
        });
    }

    public List<GridRecord> ListRows { get; } = Enumerable.Range(1, 1000).Select(id =>
        new GridRecord(id, $"设备 {id:0000}", "上海仓", id % 40 + 1, 99m + id, id % 3 != 0, "集合绑定示例")).ToList();

    public ObservableCollection<GridRecord> ObservableRows { get; }
    public DataGridCollectionView<GridRecord> TypedView { get; }

    public GridRecord? Selected
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public IEnumerable<GridRecord> Rows => CanAdd ? ObservableRows : ListRows;

    public string SourceLabel =>
        $"{(CanAdd ? "ObservableCollection<GridRecord>" : "List<GridRecord>")} · {Rows.Count():N0} 项 · 列和单元格模板自动推断行类型";

    public string ActionLabel
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "点击单元格中的查看按钮";

    public bool CanAdd { get; private set; }

    public ICommand UseListCommand { get; }
    public ICommand UseObservableCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand InspectCommand { get; }

    private void SelectSource(bool observable)
    {
        if (CanAdd == observable)
            return;
        CanAdd = observable;
        this.RaisePropertyChanged(nameof(Rows));
        this.RaisePropertyChanged(nameof(SourceLabel));
        this.RaisePropertyChanged(nameof(CanAdd));
    }
}