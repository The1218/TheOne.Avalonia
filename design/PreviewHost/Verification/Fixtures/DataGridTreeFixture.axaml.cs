using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DataGridTreeFixture : UserControl
{
    public DataGridTreeFixture()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public DataGridTreeFixtureModel Model { get; } = new();
    public DataGrid Grid => this.FindControl<DataGrid>("TreeGrid")!;
}

public sealed class DataGridTreeFixtureModel : IDisposable, INotifyPropertyChanged
{
    private bool _summaryLeavesOnly;
    private DataGridSummaryScope _summaryScope;

    public DataGridTreeFixtureModel()
    {
        DataGridAot.PreserveRowType<TreePrototypeNode>();
        DataGridAot.PreserveRowType<TreePrototypeDetails>();
        Branch.Children.Add(Leaf);
        Branch.Children.Add(new TreePrototypeNode("交互验证"));
        Root.Children.Add(Branch);
        Root.Children.Add(new TreePrototypeNode("服务端"));
        for (var index = 0; index < 1000; index++)
            Other.Children.Add(new TreePrototypeNode($"设备 {index + 1:0000}") { Quantity = index + 1 });
        Roots = [Root, Other];
        View = new DataGridTreeCollectionView<TreePrototypeNode>(Roots, node => node.Children);
    }

    public TreePrototypeNode Root { get; } = new("研发项目");
    public TreePrototypeNode Branch { get; } = new("客户端");
    public TreePrototypeNode Leaf { get; } = new("界面组件");
    public TreePrototypeNode Other { get; } = new("库存明细 · 1,000 项");
    public ObservableCollection<TreePrototypeNode> Roots { get; }
    public DataGridTreeCollectionView<TreePrototypeNode> View { get; }
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

public sealed class TreePrototypeNode : INotifyPropertyChanged, IEditableObject
{
    private (string Name, int Quantity)? _backup;
    private ObservableCollection<TreePrototypeNode> _children = [];
    private TreePrototypeDetails _details = new();
    private string _name = "";
    private int _quantity;

    public TreePrototypeNode()
    {
    }

    public TreePrototypeNode(string name)
    {
        _name = name;
    }

    public TreePrototypeDetails Details
    {
        get => _details;
        set
        {
            _details = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Details)));
        }
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

    public ObservableCollection<TreePrototypeNode> Children
    {
        get => _children;
        set
        {
            _children = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Children)));
        }
    }

    public int BeginCount { get; private set; }
    public int CancelCount { get; private set; }
    public int CommitCount { get; private set; }

    public void BeginEdit()
    {
        _backup ??= (Name, Quantity);
        BeginCount++;
    }

    public void EndEdit()
    {
        _backup = null;
        CommitCount++;
    }

    public void CancelEdit()
    {
        if (_backup is { } backup)
        {
            Name = backup.Name;
            Quantity = backup.Quantity;
            _backup = null;
        }

        CancelCount++;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public sealed class TreePrototypeDetails : INotifyPropertyChanged
{
    private int _score;

    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Score)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}