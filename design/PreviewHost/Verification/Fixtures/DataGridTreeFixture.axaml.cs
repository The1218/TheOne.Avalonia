using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridTreeFixture : UserControl
{
    public DataGridTreeFixtureModel Model { get; } = new();
    public DataGrid Grid => this.FindControl<DataGrid>("TreeGrid")!;
    public DataGridTreeFixture() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}

public sealed class DataGridTreeFixtureModel : IDisposable
{
    public TreePrototypeNode Root { get; } = new("研发项目");
    public TreePrototypeNode Branch { get; } = new("客户端");
    public TreePrototypeNode Leaf { get; } = new("界面组件");
    public TreePrototypeNode Other { get; } = new("库存明细 · 1,000 项");
    public ObservableCollection<TreePrototypeNode> Roots { get; }
    public DataGridTreeCollectionView<TreePrototypeNode> View { get; }
    public DataGridTreeFixtureModel()
    {
        DataGridAot.PreserveRowType<TreePrototypeNode>();
        Branch.Children.Add(Leaf); Branch.Children.Add(new("交互验证"));
        Root.Children.Add(Branch); Root.Children.Add(new("服务端"));
        for (int index = 0; index < 1000; index++) Other.Children.Add(new($"设备 {index + 1:0000}") { Quantity = index + 1 });
        Roots = [Root, Other];
        View = new(Roots, node => node.Children);
    }
    public void Dispose() => View.Dispose();
}

public sealed class TreePrototypeNode : INotifyPropertyChanged, IEditableObject
{
    private string _name = "";
    private int _quantity;
    private ObservableCollection<TreePrototypeNode> _children = [];
    private (string Name, int Quantity)? _backup;
    public TreePrototypeNode() { }
    public TreePrototypeNode(string name) => _name = name;
    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DataValidationException("名称不能为空。");
            if (_name == value) return;
            _name = value; PropertyChanged?.Invoke(this, new(nameof(Name)));
        }
    }
    public int Quantity
    {
        get => _quantity;
        set { if (_quantity == value) return; _quantity = value; PropertyChanged?.Invoke(this, new(nameof(Quantity))); }
    }
    public ObservableCollection<TreePrototypeNode> Children
    {
        get => _children;
        set { _children = value; PropertyChanged?.Invoke(this, new(nameof(Children))); }
    }
    public int BeginCount { get; private set; }
    public int CancelCount { get; private set; }
    public int CommitCount { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;
    public void BeginEdit() { _backup ??= (Name, Quantity); BeginCount++; }
    public void EndEdit() { _backup = null; CommitCount++; }
    public void CancelEdit()
    {
        if (_backup is { } backup) { Name = backup.Name; Quantity = backup.Quantity; _backup = null; }
        CancelCount++;
    }
}
