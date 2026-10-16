using System.Collections;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.VisualTree;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

internal sealed record PreviewProperty(TypeContract Owner, PropertyContract Contract)
{
    public string Key => Owner.Id + "." + Contract.Name;
    public string Label => Owner.Name + "." + Contract.Name;
}

// Supplement the original complete demo in place. Never reparent or resize its controls.
internal sealed class ControlDetailsView : UserControl
{
    private readonly string _name;
    private readonly Control _demo;
    private readonly ComboBox _properties = new() { Name = "AdditionalProperty", HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly ComboBox _values = new();
    private readonly TextBox _editor = new() { Name = "PropertyEditor", PlaceholderText = "输入该属性的值" };
    private readonly TextBlock _description = Text(""), _live = Text(""), _feedback = Text("");
    private readonly Button _apply = new() { Name = "ApplyProperty", Content = "应用到上方示例", Classes = { "primary" } };
    private readonly Button _restore = new() { Content = "恢复开始编辑时的值" };
    private readonly Dictionary<string, object?> _original = [];
    private readonly StackPanel _events = new() { Spacing = 4 };
    private readonly string[] _styleNames;
    private IDisposable? _subscription;
    private readonly bool _pageOwnsObservation;
    internal AvaloniaObject Target { get; private set; }
    internal ComboBox PropertyPicker => _properties;
    internal Expander PropertySection { get; }
    internal Expander StyleSection { get; }
    internal int EventCount => _events.Children.Count;
    internal string ControlName => _name;

    internal ControlDetailsView(string name, Control demo, bool pageOwnsObservation = false)
    {
        _name = name; _demo = demo; Target = FindTarget(demo, name); _pageOwnsObservation = pageOwnsObservation;
        var owners = ContractCatalog.Owners(name);
        var styles = ContractCatalog.Current.Styles.Where(s => ContractCatalog.StyleTargets(name).Contains(s.Target) && !s.Internal).DistinctBy(s => s.Name).ToArray();
        _styleNames = styles.Select(s => s.Name).ToArray();
        var styleBody = new StackPanel { Spacing = 12 };
        styleBody.Children.Add(Text("保留完整示例。点击样式将其应用到上方的实际控件；状态、尺寸与数据仍由原示例控制。"));
        var choices = new WrapPanel();
        foreach (var style in styles)
        {
            var button = new Button { Content = "Classes=\"" + style.Name + "\"", Margin = new Thickness(0, 0, 8, 8) };
            button.Click += (_, _) => ApplyStyle(style.Name); choices.Children.Add(button);
            styleBody.Children.Add(new Expander { Header = style.Name + " · 选择器、状态与资源", Content = Text(style.Effect.Length > 0 ? style.Effect : style.Selector) });
        }
        styleBody.Children.Insert(1, choices);
        if (styles.Length == 0) styleBody.Children.Add(Text("此控件使用默认主题和属性配置；密度使用顶部的 Small / Normal / Large 切换。"));
        StyleSection = new Expander { Header = name + " · 样式演示", Content = styleBody };

        _properties.ItemTemplate = new FuncDataTemplate<PreviewProperty>((p, _) => Text(p?.Label ?? ""));
        _properties.ItemsSource = owners.SelectMany(o => o.Properties.Select(p => new PreviewProperty(o, p))).ToArray();
        _properties.SelectionChanged += (_, _) => DescribeProperty();
        _apply.Click += (_, _) => ApplyValue(_values.IsVisible ? _values.SelectedItem as string ?? "" : _editor.Text ?? "");
        _restore.Click += (_, _) =>
        {
            if (_properties.SelectedItem is PreviewProperty p && _original.TryGetValue(p.Key, out var value))
            { ContractAccessors.Restore(Resolve(p.Contract), p.Key, value); DescribeProperty(); }
        };
        _apply.Margin = _restore.Margin = new Thickness(0, 0, 8, 8);
        PropertySection = new Expander { Header = name + " · 额外属性演示", Content = new StackPanel { Spacing = 10, Children =
        {
            _properties, _description, _live, _editor, _values,
            new WrapPanel { Children = { _apply, _restore } }, _feedback
        } } };
        var api = new StackPanel { Spacing = 10 };
        foreach (var owner in owners)
        {
            var members = new StackPanel { Spacing = 8 };
            foreach (var member in owner.Methods.Concat(owner.Events).Concat(owner.Fields)) members.Children.Add(Text(member.Signature + "\n" + member.Summary));
            api.Children.Add(new Expander { Header = owner.Id, Content = members });
        }
        var models = new ComboBox { HorizontalAlignment = HorizontalAlignment.Stretch };
        var modelText = Text("");
        models.ItemsSource = ContractCatalog.Models(owners);
        models.ItemTemplate = new FuncDataTemplate<TypeContract>((t, _) => Text(t?.Id ?? ""));
        models.SelectionChanged += (_, _) =>
        {
            if (models.SelectedItem is not TypeContract t) return;
            modelText.Text = t.Summary + "\n\n" + string.Join("\n\n", t.Properties.Select(p => p.Name + " : " + p.Type + "\n" + p.Summary + "\n初值：" + p.Default))
                + "\n\n" + string.Join("\n", t.Values.Select(v => v.Signature))
                + "\n\n" + string.Join("\n\n", t.Methods.Select(m => m.Signature + "\n" + m.Summary));
        };
        models.SelectedIndex = models.ItemCount > 0 ? 0 : -1;
        var body = new StackPanel { Spacing = 12, Children =
        {
            Text(name + " · 样式、属性与接入", 18), StyleSection, PropertySection,
            new Expander { Header = name + " · API 与事件", Content = api },
            new Expander { Header = name + " · 内置模型", Content = new StackPanel { Spacing = 10, Children = { models, modelText } } },
            new Expander { Header = name + " · 当前示例事件", Content = _events }
        } };
        var border = new Border { Padding = new Thickness(16), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Child = body };
        border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line")); Content = border;
        AttachedToVisualTree += (_, _) => { if (!_pageOwnsObservation) Observe(); };
        DetachedFromVisualTree += (_, _) => { if (!_pageOwnsObservation) StopObserving(); };
    }

    internal void StopObserving()
    {
        _subscription?.Dispose(); _subscription = null; Target.PropertyChanged -= Changed;
        if (_demo is WindowControlDemoView windows) windows.SampleChanged -= SampleReplaced;
    }
    private void SampleReplaced(object? sender, EventArgs args) => Observe();
    internal void Observe()
    {
        StopObserving();
        // Popups/template children can become available only after the complete demo attaches.
        var resolved = FindTarget(_demo, _name);
        if (!ReferenceEquals(Target, resolved)) { Target = resolved; _original.Clear(); }
        _subscription = ContractAccessors.Watch(Target, Log); Target.PropertyChanged += Changed;
        if (_demo is WindowControlDemoView windows) windows.SampleChanged += SampleReplaced;
        if (_properties.SelectedIndex < 0 && _properties.ItemCount > 0) _properties.SelectedIndex = 0;
        DescribeProperty();
    }
    private AvaloniaObject Resolve(PropertyContract p) => p.Target.EndsWith("DataGridColumn", StringComparison.Ordinal) && Target is DataGrid grid && grid.Columns.Count > 0 ? grid.Columns[0] : Target;
    private void DescribeProperty()
    {
        if (_properties.SelectedItem is not PreviewProperty p) return;
        var contract = p.Contract;
        var matched = ContractAccessors.Matches(Resolve(contract), p.Key);
        var value = matched ? ContractAccessors.Read(Resolve(contract), p.Key) : null;
        if (matched && !_original.ContainsKey(p.Key)) _original[p.Key] = value;
        _description.Text = p.Label + " : " + contract.Type + "\n" + contract.Summary + "\n注册／模型初值：" + contract.Default + "\n绑定：" + contract.Binding
            + (contract.Validation.Length > 0 ? "\n校验：" + contract.Validation : "");
        _live.Text = matched ? "当前示例值：" + Format(value) : "此成员用于相关子项／弹层或模型，见本页的完整业务示例与内置模型说明。";
        var choices = contract.Choices.Length > 0 ? contract.Choices : contract.Type is "bool" or "bool?" ? new[] { "false", "true" } : [];
        if (choices.Length > 0 && contract.Type.EndsWith('?')) choices = new[] { "" }.Concat(choices).ToArray();
        _values.ItemsSource = choices; _values.SelectedItem = Format(value); _values.IsVisible = choices.Length > 0;
        _editor.IsVisible = choices.Length == 0; _editor.Text = Format(value);
        _apply.IsEnabled = matched && contract.Writable && ContractAccessors.CanEdit(p.Key);
        _restore.IsEnabled = matched && contract.Writable && _original.ContainsKey(p.Key);
        _feedback.Text = _apply.IsEnabled ? "修改只作用于本页原始示例；复杂集合与模板保留原业务示例。" : contract.Writable ? "此类型使用完整示例中的模型／集合配置。" : "只读输出，操作原示例观察其变化。";
    }
    internal bool ApplyValue(string value)
    {
        if (_properties.SelectedItem is not PreviewProperty p) return false;
        try
        {
            var changed = ContractAccessors.Set(Resolve(p.Contract), p.Key, value, Log);
            DescribeProperty(); _feedback.Text = changed ? "已应用到本页原示例。" : "该成员需要对应模型或子项。"; return changed;
        }
        catch (Exception error) when (error is ArgumentException or FormatException or OverflowException or InvalidOperationException)
        { _feedback.Text = "值未应用：" + error.Message; return false; }
    }
    internal void ApplyStyle(string name)
    {
        if (Target is not Control control || !_styleNames.Contains(name)) return;
        foreach (var existing in control.Classes.Where(c => _styleNames.Contains(c)).ToArray()) control.Classes.Remove(existing);
        control.Classes.Add(name); control.BringIntoView();
    }
    private void Changed(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (PropertySection.IsExpanded && _properties.SelectedItem is PreviewProperty p && args.Property.Name == p.Contract.Name && ContractAccessors.Matches(Resolve(p.Contract), p.Key))
            _live.Text = "当前示例值：" + Format(ContractAccessors.Read(Resolve(p.Contract), p.Key));
    }
    private void Log(string value)
    {
        _events.Children.Insert(0, Text(value));
        while (_events.Children.Count > 40) _events.Children.RemoveAt(40);
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
        null => "", bool b => b ? "true" : "false", string text => text,
        ICollection collection => collection.Count + " 项", IFormattable f => f.ToString(null, CultureInfo.InvariantCulture), _ => value.ToString() ?? ""
    };
    private static TextBlock Text(string text, double size = 13) => new() { Text = text, FontSize = size, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap };
}
