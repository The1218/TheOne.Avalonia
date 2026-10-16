using System.Collections;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

// Visible documentation supplements the complete demo without changing its model or bindings.
internal sealed class ControlDetailsView : UserControl
{
    private readonly string _name;
    private readonly Control _demo;
    private readonly bool _pageOwnsObservation;
    private readonly List<(TypeContract Owner, PropertyContract Contract, TextBlock Live)> _live = [];
    private readonly StackPanel _events = new() { Spacing = 4 };
    private IDisposable? _subscription;
    private bool _hasEvents;
    private readonly bool _observesEvents;
    internal AvaloniaObject Target { get; private set; }
    internal string ControlName => _name;
    internal int EventCount => _hasEvents ? _events.Children.Count : 0;
    internal Description PropertyTable { get; } = new() { Name = "AdditionalProperties" };
    internal Description ApiTable { get; } = new() { Name = "PublicApi" };
    internal IReadOnlyList<Description> ModelTables { get; }

    internal ControlDetailsView(string name, Control demo, bool pageOwnsObservation = false)
    {
        _name = name; _demo = demo; _pageOwnsObservation = pageOwnsObservation;
        Target = FindTarget(demo, name);
        var owners = ContractCatalog.Owners(name);
        var body = new StackPanel { Spacing = 24 };
        var styles = ContractCatalog.Current.Styles.Where(s => s.Target != "Control" && ContractCatalog.StyleTargets(name).Contains(s.Target) && !s.Internal)
            .DistinctBy(s => (s.Target, s.Name)).ToArray();
        var styleTable = new Description { Name = "StyleContracts" };
        foreach (var style in styles)
            styleTable.Items.Add(Row(style.Target + "." + style.Name, "Classes=\"" + style.Name + "\"", style.Effect.Length > 0 ? style.Effect : style.Selector));
        if (styles.Length > 0) body.Children.Add(Section(name + " · 附加 Classes", styleTable));
        foreach (var owner in owners)
        foreach (var property in ContractCatalog.ControlProperties(owner))
        {
            var live = Text("");
            var description = new StackPanel { Spacing = 4, Children =
            {
                Text(property.Type + (property.Writable ? " · 可写" : " · 只读"), true), Text(property.Summary),
                Text("默认：" + property.Default + "    绑定：" + property.Binding), live
            } };
            if (property.Choices.Length > 0) description.Children.Add(Text("可选值：" + string.Join(" / ", property.Choices)));
            if (property.Validation.Length > 0) description.Children.Add(Text("校验：" + property.Validation));
            PropertyTable.Items.Add(new DescriptionItem { Header = owner.Name + "." + property.Name, Content = description });
            _live.Add((owner, property, live));
        }
        if (PropertyTable.ItemCount > 0) body.Children.Add(Section(name + " · 额外属性", PropertyTable));
        foreach (var owner in owners)
        {
            foreach (var member in owner.Methods) ApiTable.Items.Add(Row(owner.Name + " · 方法", member.Signature, member.Summary));
            foreach (var member in owner.Events) ApiTable.Items.Add(Row(owner.Name + " · 事件", member.Signature, member.Summary));
            foreach (var member in owner.Fields.Where(f => !f.ValueType.Contains("Property", StringComparison.Ordinal)))
                ApiTable.Items.Add(Row(owner.Name + " · 字段", member.Signature, member.Summary));
        }
        if (ApiTable.ItemCount > 0) body.Children.Add(Section(name + " · 扩展 API / 事件", ApiTable));
        var modelTables = new List<Description>();
        var modelBody = new StackPanel { Spacing = 16 };
        var ownerIds = owners.Select(o => o.Id).ToHashSet();
        foreach (var model in ContractCatalog.Models(owners).Where(m => !ownerIds.Contains(m.Id) && !m.IsStatic))
        {
            var table = new Description();
            table.Items.Add(Row("类型", model.Id, model.Summary));
            foreach (var property in model.Properties)
                table.Items.Add(Row(property.Name, property.Type + (property.Writable ? " · 可写" : " · 只读"), property.Summary,
                    "默认：" + property.Default + (property.Choices.Length > 0 ? "\n可选值：" + string.Join(" / ", property.Choices) : "")));
            foreach (var value in model.Values) table.Items.Add(Row("枚举值", value.Signature, value.Summary));
            foreach (var member in model.Methods.Concat(model.Events).Concat(model.Fields)) table.Items.Add(Row("成员", member.Signature, member.Summary));
            modelBody.Children.Add(new StackPanel { Spacing = 8, Children = { Text(model.Name, true), table } });
            modelTables.Add(table);
        }
        ModelTables = modelTables;
        if (modelTables.Count > 0) body.Children.Add(Section(name + " · Model 内置模型", modelBody));
        _observesEvents = owners.Any(owner => owner.Events.Length > 0);
        if (_observesEvents)
        {
            _events.Children.Add(Text("操作上方示例，观察本库扩展事件。"));
            body.Children.Add(Section(name + " · 扩展事件记录", _events));
        }
        Content = body;
        IsVisible = body.Children.Count > 0;
        AttachedToVisualTree += (_, _) => { if (!_pageOwnsObservation) Observe(); };
        DetachedFromVisualTree += (_, _) => { if (!_pageOwnsObservation) StopObserving(); };
    }
    internal static Control Section(string title, Control content) => PreviewSection.Create(title, content);
    private static DescriptionItem Row(string label, params string[] lines) => new()
    { Header = label, Content = new StackPanel { Spacing = 4, Children = { Text(string.Join("\n", lines.Where(l => l.Length > 0))) } } };
    internal void StopObserving()
    {
        _subscription?.Dispose(); _subscription = null; Target.PropertyChanged -= Changed;
        if (_demo is WindowControlDemoView windows) windows.SampleChanged -= SampleReplaced;
    }
    private void SampleReplaced(object? sender, EventArgs args) => Observe();
    internal void Observe()
    {
        StopObserving(); Target = FindTarget(_demo, _name);
        _subscription = _observesEvents ? ContractAccessors.Watch(Target, Log) : null; Target.PropertyChanged += Changed;
        if (_demo is WindowControlDemoView windows) windows.SampleChanged += SampleReplaced;
        foreach (var entry in _live) Update(entry);
    }
    private AvaloniaObject Resolve(PropertyContract p) => p.Target.EndsWith("DataGridColumn", StringComparison.Ordinal) && Target is DataGrid grid && grid.Columns.Count > 0 ? grid.Columns[0] : Target;
    private void Update((TypeContract Owner, PropertyContract Contract, TextBlock Live) entry)
    {
        var key = entry.Owner.Id + "." + entry.Contract.Name;
        if (!ReferenceEquals(Resolve(entry.Contract), Target))
        {
            entry.Live.Text = "列配置在对应 DataGridColumn 上设置，完整用法中展示了实际列配置。";
            return;
        }
        entry.Live.Text = ContractAccessors.Matches(Resolve(entry.Contract), key)
            ? "当前示例：" + Format(ContractAccessors.Read(Resolve(entry.Contract), key)) : "在对应子项、模型或弹层上设置。";
    }
    private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        foreach (var entry in _live.Where(p => p.Contract.Name == args.Property.Name)) Update(entry);
    }
    private void Log(string value)
    {
        if (!_hasEvents) { _events.Children.Clear(); _hasEvents = true; }
        _events.Children.Insert(0, Text(value));
        while (EventCount > 10) _events.Children.RemoveAt(10);
    }
    internal static AvaloniaObject FindTarget(Control demo, string name)
    {
        if (demo is WindowControlDemoView windows) return windows.Sample;
        var targetName = name == "DataGridTree" ? "DataGrid" : name;
        return demo.GetLogicalDescendants().OfType<Control>().Concat(demo.GetVisualDescendants().OfType<Control>()).Prepend(demo)
            .FirstOrDefault(c => c.GetType().Name == targetName) ?? demo;
    }
    private static string Format(object? value) => value switch
    {
        null => "null", bool b => b ? "true" : "false", string text => text,
        ICollection collection => collection.Count + " 项", IFormattable f => f.ToString(null, CultureInfo.InvariantCulture), _ => value.ToString() ?? ""
    };
    private static TextBlock Text(string text, bool heading = false) => new()
    { Text = text, FontWeight = heading ? FontWeight.SemiBold : FontWeight.Normal, TextWrapping = TextWrapping.Wrap };
}
