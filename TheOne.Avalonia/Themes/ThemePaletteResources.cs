using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace TheOne.Avalonia.Themes;

/// <summary>A stable resource provider that publishes a complete cached palette atomically.</summary>
internal sealed class ThemePaletteResources : ResourceProvider
{
    private readonly Dictionary<(object Key, ThemeVariant? Theme), SolidColorBrush> _brushes = [];
    private readonly Dictionary<(object Key, ThemeVariant? Theme), ISolidColorBrush> _defaults = [];
    private ResourceDictionary _current;
    private bool _pending;
    private ResourceDictionary _published;

    internal ThemePaletteResources(ThemeColor color, IReadOnlyList<IResourceProvider> defaults)
    {
        _current = _published = ThemePalettes.Get(color);
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var key in ThemePalettes.SolidKeys())
            for (var i = defaults.Count - 1; i >= 0; i--)
                if (defaults[i].TryGetResource(key, variant, out var value) && value is ISolidColorBrush brush)
                {
                    _defaults[(key, variant)] = brush;
                    break;
                }
    }

    public override bool HasResources => true;

    public override bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        var found = Resolve(_current, key, theme, out value);
        if (!found || value is not ISolidColorBrush solid || !ReferenceEquals(_current, _published)) return found;
        if (!_brushes.TryGetValue((key, theme), out var live))
            _brushes.Add((key, theme), live = new SolidColorBrush { Color = solid.Color, Opacity = solid.Opacity });
        value = live;
        return true;
    }

    private bool Resolve(ResourceDictionary palette, object key, ThemeVariant? theme, out object? value)
    {
        if (palette.TryGetResource(key, theme, out value)) return true;
        for (var variant = theme; variant is not null; variant = variant.InheritVariant)
            if (_defaults.TryGetValue((key, variant), out var brush))
            {
                value = brush;
                return true;
            }

        return false;
    }

    internal void SetColor(ThemeColor color)
    {
        var next = ThemePalettes.Get(color);
        if (ReferenceEquals(next, _current)) return;
        _current = next;
        if (Owner is null)
        {
            UpdateBrushes(next);
            _published = next;
            return;
        }

        if (_pending) return;
        _pending = true;
        Dispatcher.UIThread.Post(Publish, DispatcherPriority.Render);
    }

    private void Publish()
    {
        _pending = false;
        if (ReferenceEquals(_published, _current)) return;
        // Preserve foreground identity: changing a TextBlock's brush property also invalidates
        // text measurement. Mutable colors repaint existing glyphs without rebuilding layouts.
        var next = _current;
        UpdateBrushes(next);
        _published = next;
        if (!ReferenceEquals(next, _current)) return;
        RaiseResourcesChanged();
    }

    private void UpdateBrushes(ResourceDictionary palette)
    {
        foreach (var (key, brush) in _brushes)
            if (Resolve(palette, key.Key, key.Theme, out var value) && value is ISolidColorBrush next)
            {
                brush.Color = next.Color;
                brush.Opacity = next.Opacity;
            }
    }
}