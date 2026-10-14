using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Input;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    public static readonly StyledProperty<bool> IsLegendInteractiveProperty = AvaloniaProperty.Register<ChartBase, bool>(nameof(IsLegendInteractive), true);
    public bool IsLegendInteractive { get => GetValue(IsLegendInteractiveProperty); set => SetValue(IsLegendInteractiveProperty, value); }
    public int LegendPageIndex { get { BuildGeometry(null); return _geometryDirty ? 0 : _legendPage; } }
    public int LegendPageCount { get { BuildGeometry(null); return _geometryDirty ? 0 : _legendPages.Count; } }
    private readonly record struct LegendItem(int Index, int Series, string Name, Rect Bounds);
    private readonly List<List<LegendItem>> _legendPages = [];
    private readonly Dictionary<int, int> _legendItemPages = [];
    private int _legendPage;
    private int _legendFocus;
    private bool _legendKeyboard;
    private Rect? _legendPrevious;
    private Rect? _legendNext;
    private IPointer? _legendPointer;
    private int? _legendPressed;
    private Point _legendPressPosition;
    private bool _legendDragged;
    private ChartSeries? _legendPressedSeries;
    private bool _suppressSelectionReveal;

    private void BuildLegendLayout()
    {
        _legendPages.Clear(); _legendItemPages.Clear(); _legendPrevious = _legendNext = null;
        if (!ShowLegend || IsGauge || SeriesSource is not { } source || Bounds.Width < 70 || Bounds.Height < FontSize + 20) { _legendPage = 0; return; }
        var radial = IsRadial; var series = FirstVisibleSeries();
        var entries = new List<(int Index, int Series, string Name, double Width)>();
        var count = radial ? series < 0 ? 0 : source[series].ItemsSource!.Count : source.Count;
        for (var i = 0; i < count; i++)
        {
            if (radial && !IsValid(source[series].ItemsSource![i])) continue;
            var name = radial ? source[series].ItemsSource![i].Label ?? $"{i + 1}" : source[i].Name ?? "";
            entries.Add((i, radial ? series : i, name, Math.Min(Text(name, MutedBrush).Width, 120) + 30));
        }
        if (entries.Count == 0) { _legendPage = 0; return; }
        var paged = entries.Sum(entry => entry.Width) > Bounds.Width - 24;
        var left = paged ? 42 : 12; var right = Bounds.Width - (paged ? 42 : 12);
        if (right <= left) { _legendPage = 0; return; }
        var y = Bounds.Height - FontSize - 18; var height = FontSize + 14;
        if (paged)
        {
            _legendPrevious = new Rect(12, y, 24, height);
            _legendNext = new Rect(Bounds.Width - 36, y, 24, height);
        }
        var page = new List<LegendItem>(); var x = (double)left;
        foreach (var entry in entries)
        {
            var width = Math.Min(entry.Width, right - left);
            if (page.Count > 0 && x + width > right) { _legendPages.Add(page); page = []; x = left; }
            var item = new LegendItem(entry.Index, entry.Series, entry.Name, new Rect(x, y, width, height));
            page.Add(item); _legendItemPages[entry.Index] = _legendPages.Count; x += width;
        }
        if (page.Count > 0) _legendPages.Add(page);
        _legendPage = Math.Clamp(_legendPage, 0, _legendPages.Count - 1);
        _legendFocus = Math.Clamp(_legendFocus, 0, Math.Max(0, source.Count - 1));
    }
    public Rect? GetLegendItemBounds(int seriesIndex)
    {
        BuildGeometry(null);
        if (_geometryDirty || IsRadial || _legendPage >= _legendPages.Count) return null;
        foreach (var item in _legendPages[_legendPage]) if (item.Index == seriesIndex) return item.Bounds;
        return null;
    }
    public int? HitTestLegend(Point point)
    {
        BuildGeometry(null);
        if (_geometryDirty || IsRadial || _legendPage >= _legendPages.Count) return null;
        foreach (var item in _legendPages[_legendPage]) if (item.Bounds.Contains(point)) return item.Index;
        return null;
    }
    public Rect? GetLegendPageNavigationBounds(bool next) { BuildGeometry(null); return _geometryDirty ? null : next ? _legendNext : _legendPrevious; }
    public bool PageLegend(int delta)
    {
        BuildGeometry(null);
        if (_geometryDirty || _legendPages.Count == 0) return false;
        var page = (int)Math.Clamp((long)_legendPage + delta, 0, _legendPages.Count - 1);
        if (page == _legendPage) return false;
        _legendPage = page; CancelLegendPress(); ClearPointerHover(); InvalidateVisual(); return true;
    }
    /// <summary>Toggle model visibility, retaining the series' binding. Independent of input/legend flags.</summary>
    public bool ToggleSeriesVisibility(int seriesIndex)
    {
        if (SeriesSource is not { } source || seriesIndex < 0 || seriesIndex >= source.Count) return false;
        var previous = _suppressSelectionReveal; _suppressSelectionReveal = true;
        try
        {
            source[seriesIndex].SetCurrentValue(ChartSeries.IsVisibleProperty, !source[seriesIndex].IsVisible);
            if (!_observing) { MarkGeometryDirty(); ClearPointerHover(); UpdateSelection(); }
        }
        finally { _suppressSelectionReveal = previous; }
        return true;
    }
    private int? HitLegendTarget(Point point)
    {
        BuildGeometry(null);
        if (_geometryDirty || !ShowLegend || _legendPages.Count == 0) return null;
        if (_legendPrevious?.Contains(point) == true && _legendPage > 0) return -1;
        if (_legendNext?.Contains(point) == true && _legendPage + 1 < _legendPages.Count) return -2;
        if (IsRadial || !IsLegendInteractive) return null;
        return HitTestLegend(point);
    }
    private bool BeginLegendPress(PointerPressedEventArgs e)
    {
        if (!IsEffectivelyEnabled || HitLegendTarget(e.GetPosition(this)) is not { } target) return false;
        var source = SeriesSource; var version = _geometryVersion;
        var series = target >= 0 && source is not null && target < source.Count ? source[target] : null;
        CancelPan(); CancelLegendPress(); ClearPointerHover();
        e.Handled = true;
        if (!IsEffectivelyEnabled || version != _geometryVersion || _geometryDirty || !ReferenceEquals(source, SeriesSource) ||
            target >= 0 && (source is null || target >= source.Count || !ReferenceEquals(series, source[target]))) return true;
        _legendPointer = e.Pointer; _legendPressed = target; _legendPressPosition = e.GetPosition(this); _legendDragged = false;
        _legendPressedSeries = series;
        e.Pointer.Capture(this); e.Handled = true; return true;
    }
    private bool MoveLegendPress(PointerEventArgs e)
    {
        if (_legendPointer != e.Pointer) return false;
        var delta = e.GetPosition(this) - _legendPressPosition;
        if (delta.X * delta.X + delta.Y * delta.Y >= 16) _legendDragged = true;
        ClearPointerHover(); e.Handled = true; return true;
    }
    private bool EndLegendPress(PointerReleasedEventArgs e)
    {
        if (_legendPointer != e.Pointer || e.InitialPressMouseButton != MouseButton.Left) return false;
        var target = _legendPressed; var valid = !_legendDragged && IsEffectivelyEnabled && target == HitLegendTarget(e.GetPosition(this));
        if (target >= 0) valid &= SeriesSource is { } source && target.Value < source.Count && ReferenceEquals(_legendPressedSeries, source[target.Value]);
        CancelLegendPress();
        if (valid && target is { } index)
        {
            if (index == -1) PageLegend(-1); else if (index == -2) PageLegend(1);
            else if (!IsRadial && IsLegendInteractive) { _legendFocus = index; ToggleSeriesVisibility(index); }
        }
        e.Handled = true; return true;
    }
    private void CancelLegendPress()
    {
        var pointer = _legendPointer; _legendPointer = null; _legendPressed = null; _legendDragged = false; _legendPressedSeries = null;
        if (pointer?.Captured == this) pointer.Capture(null);
    }
    private bool HandleLegendKey(KeyEventArgs e)
    {
        if (!IsEffectivelyEnabled || IsRadial || !ShowLegend || !IsLegendInteractive || SeriesSource is not { Count: > 0 } source) return false;
        if (e.Key == Key.F6)
        {
            CancelPan(); CancelLegendPress();
            _legendKeyboard = !_legendKeyboard; ClearPointerHover();
            if (_legendKeyboard) RevealLegendFocus(); else RestoreSelectionAutomation();
            InvalidateVisual(); return true;
        }
        if (!_legendKeyboard) return false;
        switch (e.Key)
        {
            case Key.Escape: _legendKeyboard = false; RestoreSelectionAutomation(); InvalidateVisual(); return true;
            case Key.Left: case Key.Up: _legendFocus = Math.Max(0, _legendFocus - 1); break;
            case Key.Right: case Key.Down: _legendFocus = Math.Min(source.Count - 1, _legendFocus + 1); break;
            case Key.Home: _legendFocus = 0; break;
            case Key.End: _legendFocus = source.Count - 1; break;
            case Key.Space: case Key.Enter: ToggleSeriesVisibility(_legendFocus); break;
            default: return false;
        }
        RevealLegendFocus(); InvalidateVisual(); return true;
    }
    private void RevealLegendFocus()
    {
        BuildGeometry(null);
        if (_legendItemPages.TryGetValue(_legendFocus, out var page)) _legendPage = page;
        if (SeriesSource is { } source && _legendFocus < source.Count)
            AutomationProperties.SetItemStatus(this, $"图例 {source[_legendFocus].Name}: {(source[_legendFocus].IsVisible ? "显示" : "隐藏")}; Space / Enter 切换");
    }
    private void RestoreSelectionAutomation() => AutomationProperties.SetItemStatus(this, Selection is { } selection ? Describe(selection) : "");
    private void DrawLegend(DrawingContext context)
    {
        if (_geometryDirty || _legendPage >= _legendPages.Count || SeriesSource is not { } source) return;
        foreach (var item in _legendPages[_legendPage])
        {
            if (item.Series < 0 || item.Series >= source.Count) continue;
            var visible = IsRadial || source[item.Series].IsVisible;
            var dot = new Point(item.Bounds.Left + 8, item.Bounds.Center.Y);
            context.DrawEllipse(visible ? SeriesBrush(item.Series, IsRadial ? item.Index : -1) : null, visible ? null : new Pen(MutedBrush, 1.5), dot, 4, 4);
            using (context.PushClip(new Rect(item.Bounds.Left + 18, item.Bounds.Top, Math.Max(0, item.Bounds.Width - 24), item.Bounds.Height)))
                DrawText(context, item.Name, new Point(item.Bounds.Left + 18, item.Bounds.Top + 6), MutedBrush);
            if (IsFocused && _legendKeyboard && item.Index == _legendFocus && !IsRadial)
                context.DrawRectangle(null, new Pen(AccentBrush, 1.5), item.Bounds.Deflate(1), 3, 3);
        }
        if (_legendPrevious is { } previous) DrawText(context, "‹", new Point(previous.Center.X, previous.Top + 3), _legendPage > 0 ? Foreground : MutedBrush, centered: true);
        if (_legendNext is { } next) DrawText(context, "›", new Point(next.Center.X, next.Top + 3), _legendPage + 1 < _legendPages.Count ? Foreground : MutedBrush, centered: true);
    }
}
