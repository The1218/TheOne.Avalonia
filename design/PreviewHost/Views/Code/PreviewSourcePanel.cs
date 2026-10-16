using Avalonia.Controls;
using Avalonia.Layout;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed record PreviewSourceFile(string Name, string Text, CodeType Language)
{
    public override string ToString() => Name;
}

/// <summary>Loads embedded source only when the user opens the source panel.</summary>
public sealed class PreviewSourcePanel : UserControl
{
    private readonly Expander _expander = new() { Header = "示例代码 · AXAML / C#", HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly ComboBox _files = new() { HorizontalAlignment = HorizontalAlignment.Stretch, MaxDropDownHeight = 280 };
    private readonly PreviewCodeBlock _code = new();
    private readonly StackPanel _body = new() { Spacing = 10 };
    private Control[] _views = [];
    private bool _includeStyleExamples;
    public PreviewSourcePanel()
    {
        Content = _expander;
        _body.Children.Add(_files); _body.Children.Add(_code);
        _expander.PropertyChanged += (_, args) =>
        {
            if (args.Property == Expander.IsExpandedProperty && _expander.IsExpanded && _expander.Content is null) LoadSources();
        };
        _files.SelectionChanged += (_, _) =>
        {
            if (_files.SelectedItem is PreviewSourceFile source)
            { _code.CodeType = source.Language; _code.Text = source.Text; _code.Editor.Header = source.Name; }
        };
    }
    internal void SetViews(IEnumerable<Control> views, bool includeStyleExamples = false)
    {
        _views = views.ToArray();
        _includeStyleExamples = includeStyleExamples;
        _expander.IsExpanded = false;
        _expander.Content = null;
        _files.ItemsSource = null;
        _code.Text = "";
    }
    internal void SetViewportHeight(double height) => _code.Editor.MaxHeight = Math.Max(96, height * .45);
    private void LoadSources()
    {
        var sources = ReadSources(_views, _includeStyleExamples);
        _files.ItemsSource = sources;
        _expander.Content = _body;
        if (sources.Count > 0) _files.SelectedIndex = 0;
    }
    internal static IReadOnlyList<PreviewSourceFile> ReadSources(IEnumerable<Control> views, bool includeStyleExamples = false)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        if (includeStyleExamples) names.Add(nameof(ControlStyleShowcase));
        foreach (var view in views)
        {
            names.Add(view.GetType().Name);
            if (view.DataContext is { } model) names.Add(model.GetType().Name);
            if (view is ReportDesign or ReportPreview) { names.Add("ReportDesignDemo"); names.Add("PreviewPrinting"); }
        }
        var assembly = typeof(App).Assembly;
        return assembly.GetManifestResourceNames()
            .Where(resource => names.Any(name => resource.EndsWith($".{name}.axaml", StringComparison.Ordinal)
                || resource.EndsWith($".{name}.axaml.cs", StringComparison.Ordinal) || resource.EndsWith($".{name}.cs", StringComparison.Ordinal)))
            .OrderBy(resource => resource.EndsWith(".axaml", StringComparison.Ordinal) ? 0 : 1)
            .ThenBy(resource => resource, StringComparer.Ordinal)
            .Select(resource =>
            {
                using var stream = assembly.GetManifestResourceStream(resource)!;
                using var reader = new StreamReader(stream);
                var name = resource[(resource.IndexOf('.') + 1)..];
                return new PreviewSourceFile(name, reader.ReadToEnd(), resource.EndsWith(".cs", StringComparison.Ordinal) ? CodeType.CSharp : CodeType.Axaml);
            }).ToArray();
    }
}
