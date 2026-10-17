using System.Collections;
using System.ComponentModel;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class GridRecord(
    int id,
    string name,
    string warehouse,
    int quantity,
    decimal price,
    bool enabled,
    string notes)
    : ReactiveObject, IEditableObject, INotifyDataErrorInfo
{
    private (string Name, string Warehouse, int Quantity, decimal Price, bool Enabled, string Notes)? _snapshot;
    public int Id { get; } = id;

    public string Name
    {
        get;
        set
        {
            value ??= "";
            if (field == value) return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(HasErrors));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Name)));
        }
    } = name;

    public string Warehouse
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value ?? "");
    } = warehouse;

    public int Quantity
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = quantity;

    public decimal Price
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = price;

    public bool Enabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = enabled;

    public string Notes
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value ?? "");
    } = notes;

    public void BeginEdit()
    {
        _snapshot ??= (Name, Warehouse, Quantity, Price, Enabled, Notes);
    }

    public void EndEdit()
    {
        _snapshot = null;
    }

    public void CancelEdit()
    {
        if (_snapshot is not { } old) return;
        _snapshot = null;
        Name = old.Name;
        Warehouse = old.Warehouse;
        Quantity = old.Quantity;
        Price = old.Price;
        Enabled = old.Enabled;
        Notes = old.Notes;
    }

    public bool HasErrors => string.IsNullOrWhiteSpace(Name);

    public IEnumerable GetErrors(string? propertyName)
    {
        return HasErrors && propertyName is null or "" or nameof(Name) ? new[] { "设备名称不能为空" } : Array.Empty<string>();
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
}