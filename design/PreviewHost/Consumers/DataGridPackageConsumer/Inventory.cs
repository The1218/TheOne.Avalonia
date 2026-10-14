using System.Collections.ObjectModel;
using System.ComponentModel;
using Avalonia.Collections;
using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DataGridPackageConsumer;

public sealed class StockRow(int id) : INotifyPropertyChanged
{
    public int Id { get; } = id;
    public string Name => Id % 2 == 0 ? "设备 / Equipment" : "配件 / Accessory";
    public string Category => Id % 3 == 0 ? "A" : "B";
    public decimal Price => 100m + Id / 10m;
    private int _quantity = id % 20;
    public int Quantity { get => _quantity; set { if (_quantity == value) return; _quantity = value; PropertyChanged?.Invoke(this, new(nameof(Quantity))); } }
    public event PropertyChangedEventHandler? PropertyChanged;
}
public sealed class Inventory
{
    public ObservableCollection<StockRow> Source { get; } = new(Enumerable.Range(1, 1000).Select(id => new StockRow(id)));
    public DataGridCollectionView<StockRow> View { get; }
    public Inventory() { _ = DataGridAot.PreserveRowType<StockRow>(); View = new(Source); }
}
public partial class ConsumerView : UserControl
{
    public ConsumerView() => InitializeComponent();
}
