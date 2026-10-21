using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public abstract partial class ChartBase
{
    private protected virtual bool IsValidDataPoint(ChartPoint point) =>
        double.IsFinite(point.X) && double.IsFinite(point.Y) && (!IsRadial || point.Y > 0);

    private protected virtual bool IsValidSelectionPoint(int series, int index, ChartPoint point) => IsValidDataPoint(point);

    private protected virtual void OnRefreshChartData() { }

    private protected virtual void OnChartDataObjectSubscribed(AvaloniaObject source) { }
    private protected virtual void OnChartDataObjectUnsubscribed(AvaloniaObject source) { }
    private protected virtual bool ShouldProcessChartDataChange(AvaloniaProperty property) => true;
    private protected virtual int CustomChartAnimationMarkCount => _positions.Count;

    private protected virtual bool SupportsViewportNavigation => !IsRadial && !UsesCustomChartGeometry;

    private protected void SetChartDataPlot(Rect plot) => _cartesianPlot = plot;
    private protected void SetChartNormalizedPosition(int series, int index, Point position) =>
        _normalizedPositions[(series, index)] = position;

    private protected virtual string? DescribeSpecializedPoint(ChartSelection selection) => null;

    private protected virtual (int Series, int Index)? HitTestSpecializedPoint(Point point) => null;

    private protected IBrush GetChartBrush(int series, int point = -1) => SeriesBrush(series, point);

    private protected string FormatChartValue(ChartSeries series, double value)
    {
        try { return value.ToString(series.ValueFormat ?? ValueFormat, System.Globalization.CultureInfo.CurrentCulture); }
        catch (FormatException) { return value.ToString("0.##", System.Globalization.CultureInfo.CurrentCulture); }
    }

    private protected void SetChartMarkPosition(int series, int index, Point center) =>
        _positions[(series, index)] = center;

    private protected void InvalidateChartGeometry()
    {
        _animateDataPending = false;
        FinishDataAnimation();
        ClearPointerHover();
        MarkGeometryDirty();
        UpdateSelection();
    }

    private protected void NotifyChartDataChanged() => DataChanged();
}

/// <summary>Shared direct-drawing infrastructure for non-Cartesian statistical charts.</summary>
public abstract class SpecializedChartBase : ChartBase
{
    private readonly Dictionary<(int Series, int Index), Rect> _markBounds = [];
    private readonly List<(int Series, int Index)> _markOrder = [];

    private protected override bool UsesCustomChartGeometry => true;

    private protected virtual bool UseDefaultMarkHitTesting => true;

    private protected sealed override void PrepareChartGeometry(Rect plot)
    {
        _markBounds.Clear();
        _markOrder.Clear();
        PrepareSpecializedGeometry(plot);
    }

    private protected abstract void PrepareSpecializedGeometry(Rect plot);

    private protected void RegisterMark(int series, int index, Point center, Rect hitBounds)
    {
        if (!double.IsFinite(center.X) || !double.IsFinite(center.Y) ||
            !double.IsFinite(hitBounds.X) || !double.IsFinite(hitBounds.Y) ||
            !double.IsFinite(hitBounds.Width) || !double.IsFinite(hitBounds.Height)) return;
        var key = (series, index);
        if (UseDefaultMarkHitTesting)
        {
            if (!_markBounds.ContainsKey(key)) _markOrder.Add(key);
            _markBounds[key] = hitBounds;
        }
        SetChartMarkPosition(series, index, center);
    }

    private protected virtual (int Series, int Index)? HitTestMark(Point point)
    {
        for (var i = _markOrder.Count - 1; i >= 0; i--)
            if (_markBounds[_markOrder[i]].Contains(point)) return _markOrder[i];
        return null;
    }

    private protected sealed override (int Series, int Index)? HitTestSpecializedPoint(Point point) =>
        double.IsFinite(point.X) && double.IsFinite(point.Y) && new Rect(Bounds.Size).Contains(point)
            ? HitTestMark(point) : null;
}
