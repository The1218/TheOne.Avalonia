using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace TheOne.DesignPreview.Reference;

internal sealed record ContractSet(TypeContract[] Types, StyleContract[] Styles);
internal sealed record TypeContract(string Id, string Name, string Namespace, string Kind, bool IsControl, bool IsStatic, string Base,
    string Summary, string Source, PropertyContract[] Properties, MemberContract[] Methods, MemberContract[] Events, MemberContract[] Values, string[] Related, MemberContract[] Fields);
internal sealed record PropertyContract(string Name, string Type, string Summary, string Default, string Binding, string Validation,
    bool Writable, bool Attached, string Identifier, string Source, bool Inherits, bool Static, string[] Choices, string Target);
internal sealed record MemberContract(string Signature, string Summary, string Source, string ValueType = "");
internal sealed record StyleContract(string Target, string Name, string Selector, string Effect, bool Internal, string Source);

[JsonSerializable(typeof(ContractSet))]
internal partial class ContractJsonContext : JsonSerializerContext;

internal static class ContractCatalog
{
    // Native appearance aliases do not add a new control capability.
    private static readonly HashSet<string> NativeAppearance = ["Background", "Foreground", "FontFamily", "FontSize", "FontWeight", "Padding", "BorderBrush", "BorderThickness", "CornerRadius"];
    private static readonly Lazy<ContractSet> Data = new(() =>
    {
        using var stream = typeof(ContractCatalog).Assembly.GetManifestResourceStream("PreviewHost.Reference.ControlContracts.json")
            ?? throw new InvalidOperationException("Embedded control contracts are missing. Run ControlReferenceGenerator.");
        return JsonSerializer.Deserialize(stream, ContractJsonContext.Default.ContractSet)
            ?? throw new InvalidOperationException("Embedded control contracts are empty.");
    });
    internal static ContractSet Current => Data.Value;
    internal static IEnumerable<PropertyContract> ControlProperties(TypeContract owner) =>
        owner.Properties.Where(property => !owner.IsControl || !NativeAppearance.Contains(property.Name));
    internal static TypeContract? Find(string name) => Current.Types.FirstOrDefault(t => t.Name == (name switch
    { "Toast" => "OneToast", "Dialog" => "OneDialog", "MessageBox" => "OneMessageBox", _ => name }));
    internal static IReadOnlyList<TypeContract> Owners(string name)
    {
        var names = new HashSet<string> { name };
        for (var type = Find(name); type is not null; type = Find(type.Base.Split('.').Last()))
        { names.Add(type.Name); if (type.Base == type.Id) break; }
        foreach (var extra in name switch
        {
            "Button" or "RepeatButton" or "ToggleButton" or "IconButton" => new[] { "ButtonLoading" },
            "Calendar" or "CalendarDatePicker" or "DatePicker" or "TimePicker" => new[] { "CalendarInput", "CalendarOptions", "PickerSizing", "PickerScroll" },
            "DataGrid" or "DataGridTree" => new[] { "DataGridFeatures", "DataGridColumnOptions", "DataGridTreeFeatures", "DataGridColumn", "DataGridRowMetadata", "DataGridCollectionView" },
            "TabControl" or "TabStrip" => new[] { "TabFeatures", "Virtualization" },
            "TreeView" => new[] { "Virtualization" },
            "Carousel" => new[] { "CarouselOptions" },
            "RefreshContainer" or "RefreshVisualizer" => new[] { "RefreshOptions", "RefreshMotion" },
            "NotificationCard" => new[] { "NotificationMotion" },
            "TransitioningContentControl" => new[] { "ContentTransitionOptions" },
            _ => Array.Empty<string>()
        }) names.Add(extra);
        return Current.Types.Where(t => names.Contains(t.Name)).ToArray();
    }
    internal static IReadOnlyList<string> StyleTargets(string name)
    {
        var names = new HashSet<string> { name };
        for (var type = Find(name); type is not null; type = Find(type.Base.Split('.').Last()))
        { names.Add(type.Name); names.Add(type.Base.Split('.').Last()); }
        if (name is "RepeatButton" or "ToggleButton") names.Add("Button");
        if (name == "ToggleSplitButton") names.Add("SplitButton");
        return names.ToArray();
    }
    internal static IReadOnlyList<TypeContract> Models(IEnumerable<TypeContract> owners)
    {
        var ids = owners.Select(t => t.Id).ToHashSet();
        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var type in Current.Types.Where(t => ids.Contains(t.Id)).ToArray())
            foreach (var id in type.Related) changed |= ids.Add(id);
        }
        return Current.Types.Where(t => ids.Contains(t.Id) && !t.IsControl && t.Name is not "ControlSizing" and not "ControlSize" and not "Motion").ToArray();
    }
}

internal sealed class ExplorerCommand(Action action) : ICommand
{
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => action();
    public event EventHandler? CanExecuteChanged { add { } remove { } }
}
internal sealed class ExplorerSubscription(Action dispose) : IDisposable
{
    private Action? _dispose = dispose;
    public void Dispose() => Interlocked.Exchange(ref _dispose, null)?.Invoke();
}
