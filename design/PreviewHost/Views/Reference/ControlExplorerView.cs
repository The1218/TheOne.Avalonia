using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

internal sealed record ExplorerProperty(TypeContract Owner, PropertyContract Contract)
{
    public string Key => Owner.Id + "." + Contract.Name;
    public string Label => Owner.Name + "." + Contract.Name;
}

public sealed class ControlExplorerView : UserControl
{
    private readonly ComboBox _picker = new() { MinWidth = 220, HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly ContentControl _sampleHost = new() { HorizontalContentAlignment = HorizontalAlignment.Stretch };
    private readonly WrapPanel _styles = new() { Orientation = Orientation.Horizontal };
    private readonly ListBox _properties = new();
    private readonly TextBlock _description = Text("", 14), _live = Text("", 13), _feedback = Text("选择一个属性进行编辑。", 13);
    private readonly TextBox _editor = new() { Name = "PropertyEditor", PlaceholderText = "输入属性值" };
    private readonly ComboBox _values = new() { IsVisible = false };
    private readonly Button _apply = new() { Name = "ApplyProperty", Content = "应用值", Classes = { "primary" } }, _restore = new() { Name = "RestoreProperty", Content = "恢复示例初值" };
    private readonly ListBox _api = new(), _models = new();
    private readonly TextBlock _modelText = Text("", 13);
    private readonly ObservableCollection<string> _events = [];
    private readonly List<ExplorerSample> _variants = [];
    private readonly Dictionary<string, object?> _originalValues = [];
    private ExplorerSample? _sample;
    private IDisposable? _subscription;
    private bool _updating, _refreshQueued;
    internal ComboBox ControlPicker => _picker;
    internal ListBox PropertyPicker => _properties;
    internal AvaloniaObject? SampleTarget => _sample?.Target;
    internal IReadOnlyList<string> EventLog => _events;
    internal int VariantCount => _variants.Count;
    internal void SelectControl(int id) => _picker.SelectedItem = ExplorerSamples.Choices.FirstOrDefault(c => c.Id == id);
    public event EventHandler<int>? ControlRequested;

    public ControlExplorerView()
    {
        var header = new StackPanel { Spacing = 8, Children =
        {
            Text("控件能力实验室", 26), Text("按 Styles、额外属性、API / 事件和内置模型查看；示例独立创建，不修改其他页面。", 14), _picker
        } };
        _picker.ItemsSource = ExplorerSamples.Choices;
        _picker.ItemTemplate = new FuncDataTemplate<ExplorerChoice>((choice, _) => Text(choice is null ? "" : choice.Group + " · " + choice.Name, 14));
        _picker.SelectionChanged += (_, _) => SelectControl();
        _properties.ItemTemplate = new FuncDataTemplate<ExplorerProperty>((property, _) => new StackPanel { Spacing = 3, Children =
        {
            Text(property?.Label ?? "", 13), Text(property?.Contract.Type ?? "", 11)
        } });
        _properties.SelectionChanged += (_, _) => SelectProperty();
        _apply.Click += (_, _) => ApplyValue(_values.IsVisible ? _values.SelectedItem as string ?? "" : _editor.Text ?? "");
        _restore.Click += (_, _) => RestoreValue();
        var reset = new Button { Content = "重建示例（含原绑定）", Margin = new Thickness(0, 0, 8, 8) }; reset.Click += (_, _) => SelectControl();
        var source = new Button { Content = "查看完整业务示例 →", Margin = new Thickness(0, 0, 8, 8) };
        _apply.Margin = _restore.Margin = new Thickness(0, 0, 8, 8);
        source.Click += (_, _) => { if (_picker.SelectedItem is ExplorerChoice { Id: >= 0 } choice) ControlRequested?.Invoke(this, choice.Id); };
        var propertyBody = new Grid { ColumnDefinitions = new ColumnDefinitions("2*,3*"), ColumnSpacing = 16 };
        propertyBody.Children.Add(_properties);
        var edit = new StackPanel { Spacing = 10, Children = { _description, _live, _editor, _values, new WrapPanel { Children = { _apply, _restore } }, _feedback } };
        var editScroll = new ScrollViewer { Content = edit, HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
        Grid.SetColumn(editScroll, 1); propertyBody.Children.Add(editScroll);
        _api.ItemTemplate = new FuncDataTemplate<string>((value, _) => Text(value ?? "", 13));
        _models.ItemTemplate = new FuncDataTemplate<TypeContract>((type, _) => Text(type is null ? "" : type.Id[(type.Namespace.Length + 1)..], 13));
        _models.SelectionChanged += (_, _) => ShowModel();
        var modelBody = new Grid { ColumnDefinitions = new ColumnDefinitions("2*,3*"), ColumnSpacing = 16 };
        modelBody.Children.Add(_models); var modelScroll = new ScrollViewer { Content = _modelText }; Grid.SetColumn(modelScroll, 1); modelBody.Children.Add(modelScroll);
        var events = new ListBox { ItemsSource = _events, MaxHeight = 110 };
        var apiBody = new Grid { RowDefinitions = new RowDefinitions("*,Auto") }; apiBody.Children.Add(_api); Grid.SetRow(events, 1); apiBody.Children.Add(events);
        var tabs = new TabControl { Name = "ReferenceTabs", Items =
        {
            new TabItem { Header = "样式", Content = new ScrollViewer { Content = _styles, HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled } },
            new TabItem { Header = "属性", Content = propertyBody },
            new TabItem { Header = "API / 事件", Content = apiBody },
            new TabItem { Header = "模型", Content = modelBody }
        } };
        var sampleFrame = Frame(new StackPanel { Spacing = 8, Children = { Text("当前可交互示例", 15), _sampleHost } }); sampleFrame.MaxHeight = 380;
        var actions = new WrapPanel { Children = { reset, source } };
        var layout = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"), RowSpacing = 12 };
        layout.Children.Add(header); Grid.SetRow(sampleFrame, 1); layout.Children.Add(sampleFrame);
        Grid.SetRow(actions, 2); layout.Children.Add(actions); Grid.SetRow(tabs, 3); layout.Children.Add(tabs);
        Content = layout;
        SizeChanged += (_, _) =>
        {
            var narrow = Bounds.Width < 640;
            foreach (var pair in new[] { (Body: propertyBody, List: _properties, Detail: editScroll), (Body: modelBody, List: _models, Detail: modelScroll) })
            {
                pair.Body.ColumnDefinitions = new ColumnDefinitions(narrow ? "*" : "2*,3*");
                pair.Body.RowDefinitions = new RowDefinitions(narrow ? "140,*" : "*");
                Grid.SetColumn(pair.Detail, narrow ? 0 : 1); Grid.SetRow(pair.Detail, narrow ? 1 : 0);
            }
            sampleFrame.MaxHeight = Math.Max(110, Math.Min(340, Bounds.Height * .3));
            foreach (var card in _styles.Children.OfType<Border>())
                if (card.Child is StackPanel body) body.Width = StyleCardWidth();
        };
        AttachedToVisualTree += (_, _) => { if (_sample is null) { if (_picker.SelectedIndex < 0) _picker.SelectedIndex = 0; else SelectControl(); } };
        DetachedFromVisualTree += (_, _) => DisposeSamples();
    }

    private void SelectControl()
    {
        if (_picker.SelectedItem is not ExplorerChoice choice) return;
        DisposeSamples(); _originalValues.Clear(); _events.Clear(); _styles.Children.Clear();
        _sample = ExplorerSamples.Create(choice, this); _sampleHost.Content = _sample.View;
        _subscription = ContractAccessors.Watch(_sample.Target, Log);
        _sample.Target.PropertyChanged += SampleChanged;
        var owners = ContractCatalog.Owners(choice.Name);
        var properties = owners.SelectMany(o => o.Properties.Select(p => new ExplorerProperty(o, p)))
            .Where(p => !ContractAccessors.CanRead(p.Key) || ContractAccessors.Matches(ExplorerSamples.Resolve(_sample, p.Contract), p.Key)).ToArray();
        foreach (var property in properties)
            if (ContractAccessors.CanRead(property.Key)) _originalValues[property.Key] = ContractAccessors.Read(ExplorerSamples.Resolve(_sample, property.Contract), property.Key);
        _properties.ItemsSource = properties; _properties.SelectedIndex = properties.Length > 0 ? 0 : -1;
        _api.ItemsSource = owners.SelectMany(o => new[] { o.Id + " : " + o.Base }.Concat(o.Methods.Select(m => m.Signature + "\n" + m.Summary))
            .Concat(o.Events.Select(e => "事件：" + e.Signature + "\n" + e.Summary)).Concat(o.Fields.Select(f => "公开标识符／常量：" + f.Signature))).ToArray();
        _models.ItemsSource = ContractCatalog.Models(owners); _models.SelectedIndex = _models.ItemCount > 0 ? 0 : -1;
        var targets = ContractCatalog.StyleTargets(choice.Name);
        var publicStyles = ContractCatalog.Current.Styles.Where(s => targets.Contains(s.Target) && !s.Internal).DistinctBy(s => s.Name).ToArray();
        foreach (var style in publicStyles)
        {
            var sample = ExplorerSamples.Create(choice, this);
            if (sample.Target is Control styled)
            {
                foreach (var existing in styled.Classes.Where(c => publicStyles.Any(s => s.Name == c)).ToArray()) styled.Classes.Remove(existing);
                styled.Classes.Add(style.Name);
            }
            sample.View.MaxWidth = 340;
            _variants.Add(sample);
            var detail = new Expander { Header = "选择器、状态与资源", Content = Text(style.Effect.Length > 0 ? style.Effect : style.Selector, 11) };
            _styles.Children.Add(Frame(new StackPanel { Width = StyleCardWidth(), Spacing = 8, Children = { Text("Classes=\"" + style.Name + "\"", 14), sample.View, detail } }));
        }
        if (_variants.Count == 0) _styles.Children.Add(Text("此控件没有额外公开外观类，使用默认主题及属性配置。small / normal / large 密度仍适用。", 14));
        _feedback.Text = "每个示例使用独立数据；改动复杂集合、模板和模型时可查看完整业务示例。";
        Log(choice.Name + "：示例已创建");
    }

    private void SelectProperty()
    {
        if (_properties.SelectedItem is not ExplorerProperty property || _sample is null) return;
        _updating = true;
        try
        {
            var p = property.Contract; var value = ContractAccessors.Read(ExplorerSamples.Resolve(_sample, p), property.Key);
            _description.Text = property.Label + "\n" + p.Type + "\n" + p.Summary + "\n注册/模型初值：" + p.Default + "\n绑定：" + p.Binding
                + (p.Inherits ? " · 继承到后代" : "") + (p.Validation.Length > 0 ? "\n校验：" + p.Validation : "");
            _live.Text = "当前值：" + Format(value);
            var choices = p.Choices.Length > 0 ? p.Choices : p.Type is "bool" or "bool?" ? new[] { "false", "true" } : [];
            if (choices.Length > 0 && p.Type.EndsWith('?')) choices = new[] { "" }.Concat(choices).ToArray();
            _values.IsVisible = choices.Length > 0; _editor.IsVisible = choices.Length == 0;
            _values.ItemsSource = choices; _values.SelectedItem = Format(value); _editor.Text = Format(value);
            _apply.IsEnabled = p.Writable && ContractAccessors.CanEdit(property.Key);
            _restore.IsEnabled = p.Writable && _originalValues.ContainsKey(property.Key);
            if (!_apply.IsEnabled) _feedback.Text = p.Writable ? "复杂对象通过内置模型和完整业务示例演示；可恢复本示例初值。" : "这是只读输出，操作示例后可观察其更新。";
        }
        finally { _updating = false; }
    }
    internal bool ApplyValue(string value)
    {
        if (_updating || _sample is null || _properties.SelectedItem is not ExplorerProperty property) return false;
        try
        {
            var changed = ContractAccessors.Set(ExplorerSamples.Resolve(_sample, property.Contract), property.Key, value, Log);
            _feedback.Text = changed ? "已应用到当前示例。" : "该属性需要模型、集合或模板对象，请使用对应业务示例。";
            Log(property.Label + " = " + value); RefreshValue(); return changed;
        }
        catch (Exception error) when (error is ArgumentException or FormatException or OverflowException or InvalidOperationException)
        { _feedback.Text = "值未应用，请按属性类型和校验条件输入：" + error.Message; return false; }
    }
    private void RestoreValue()
    {
        if (_sample is null || _properties.SelectedItem is not ExplorerProperty property || !_originalValues.TryGetValue(property.Key, out var value)) return;
        ContractAccessors.Restore(ExplorerSamples.Resolve(_sample, property.Contract), property.Key, value); SelectProperty();
        _feedback.Text = "已恢复该属性初值；需要恢复原绑定时点击“重建示例”。";
    }
    private void SampleChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (_refreshQueued) return; _refreshQueued = true;
        Dispatcher.UIThread.Post(() => { _refreshQueued = false; RefreshValue(); }, DispatcherPriority.Background);
    }
    private void RefreshValue()
    {
        if (_sample is not null && _properties.SelectedItem is ExplorerProperty property)
            _live.Text = "当前值：" + Format(ContractAccessors.Read(ExplorerSamples.Resolve(_sample, property.Contract), property.Key));
    }
    private void ShowModel()
    {
        _modelText.Text = _models.SelectedItem is TypeContract type ? type.Id + "\n" + type.Summary + "\n\n"
            + string.Join("\n\n", type.Properties.Select(p => p.Name + " : " + p.Type + "\n初值：" + p.Default + "\n" + p.Summary))
            + "\n\n" + string.Join("\n", type.Values.Select(v => v.Signature))
            + "\n\n" + string.Join("\n\n", type.Methods.Select(m => m.Signature + "\n" + m.Summary)) : "这个控件没有额外内置业务模型。";
    }
    private void Log(string text) { _events.Insert(0, text); while (_events.Count > 40) _events.RemoveAt(_events.Count - 1); }
    private double StyleCardWidth() => Bounds.Width > 0 ? Math.Max(120, Math.Min(290, Bounds.Width - 40)) : 290;
    private void DisposeSamples()
    {
        if (_sample is not null) _sample.Target.PropertyChanged -= SampleChanged;
        _subscription?.Dispose(); _subscription = null; _sampleHost.Content = null; _sample?.Dispose(); _sample = null;
        foreach (var sample in _variants) sample.Dispose(); _variants.Clear();
    }
    private static string Format(object? value) => value switch
    {
        null => "", bool b => b ? "true" : "false", string text => text,
        ICollection collection => collection.GetType().Name + " · " + collection.Count + " 项",
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? ""
    };
    private static TextBlock Text(string value, double size) => new() { Text = value, FontSize = size, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap };
    private static Border Frame(Control child)
    {
        var frame = new Border { Child = child, Padding = new Thickness(12), Margin = new Thickness(0, 0, 12, 12), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8) };
        frame.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface")); frame.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line")); return frame;
    }
}
