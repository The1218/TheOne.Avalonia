using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridTreeDemoView : UserControl
{
    public DataGridTreeDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public DataGridTreeDemoViewModel Model { get; } = new();
}

public sealed class DataGridTreeDemoViewModel : IDisposable, INotifyPropertyChanged
{
    private bool _summaryLeavesOnly;
    private DataGridSummaryScope _summaryScope;

    public DataGridTreeDemoViewModel()
    {
        DataGridAot.PreserveRowType<DataGridTreeDemoNode>();
        Branch.Children.Add(Leaf);
        Branch.Children.Add(new DataGridTreeDemoNode("交互验证"));
        Root.Children.Add(Branch);
        Root.Children.Add(new DataGridTreeDemoNode("服务端"));
        for (var index = 0; index < 1000; index++)
            Other.Children.Add(new DataGridTreeDemoNode($"设备 {index + 1:0000}") { Quantity = index + 1 });
        Roots = [Root, Other];
        View = new DataGridTreeCollectionView<DataGridTreeDemoNode>(Roots, node => node.Children);
    }

    public DataGridTreeDemoNode Root { get; } = new("研发项目");
    public DataGridTreeDemoNode Branch { get; } = new("客户端");
    public DataGridTreeDemoNode Leaf { get; } = new("界面组件");
    public DataGridTreeDemoNode Other { get; } = new("库存明细 · 1,000 项");
    public ObservableCollection<DataGridTreeDemoNode> Roots { get; }
    public DataGridTreeCollectionView<DataGridTreeDemoNode> View { get; }
    public DataGridSummaryScope[] SummaryScopes { get; } = Enum.GetValues<DataGridSummaryScope>();

    public DataGridSummaryScope SummaryScope
    {
        get => _summaryScope;
        set
        {
            _summaryScope = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SummaryScope)));
        }
    }

    public bool SummaryLeavesOnly
    {
        get => _summaryLeavesOnly;
        set
        {
            _summaryLeavesOnly = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SummaryLeavesOnly)));
        }
    }

    public void Dispose()
    {
        View.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class DataGridTreeDemoNode : INotifyPropertyChanged, IEditableObject
{
    private (string Name, int Quantity)? _backup;
    private ObservableCollection<DataGridTreeDemoNode> _children = [];
    private string _name = "";
    private int _quantity;

    public DataGridTreeDemoNode()
    {
    }

    public DataGridTreeDemoNode(string name)
    {
        _name = name;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new DataValidationException("名称不能为空。");
            if (_name == value) return;
            _name = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity == value) return;
            _quantity = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quantity)));
        }
    }

    public ObservableCollection<DataGridTreeDemoNode> Children
    {
        get => _children;
        set
        {
            _children = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
        }
    }

    public void BeginEdit()
    {
        _backup ??= (Name, Quantity);
    }

    public void EndEdit()
    {
        _backup = null;
    }

    public void CancelEdit()
    {
        if (_backup is { } backup)
        {
            Name = backup.Name;
            Quantity = backup.Quantity;
            _backup = null;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
