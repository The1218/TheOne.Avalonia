using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private readonly Dictionary<(int Series, int Index), double> _radialFractions = [];
    private readonly Dictionary<(int Series, int Index), RadialSector> _radialSectorLookup = [];
    private readonly List<RadialSector> _radialSectors = [];
    private readonly Dictionary<(int Series, int Index), RadialDrawCache> _radialDrawCache = [];
    private Pen? _radialSelectionPen;
    private Point _radialCenter;
    private double _radialInnerRadius;
    private double _radialOuterRadius;

    private protected void DrawRadial(DrawingContext? context, Rect plot)
    {
        if (context is null)
        {
            BuildRadialSectors(plot);
            return;
        }

        using var clip = context.PushClip(plot);
        RadialSector? raised = null;
        foreach (var sector in _radialSectors)
            if (IsRaisedSector(sector)) raised = sector;
            else DrawRadialSector(context, sector);
        // The raised slice paints above its neighbours, including its data label.
        if (raised is { } active) DrawRadialSector(context, active);
    }

    private protected void PrepareGaugeGeometry()
    {
        ClearRadialDrawCache();
        _radialSectors.Clear();
        _radialSectorLookup.Clear();
        _radialFractions.Clear();
    }

    private void BuildRadialSectors(Rect plot)
    {
        ClearRadialDrawCache();
        _radialSectors.Clear();
        _radialSectorLookup.Clear();
        _radialFractions.Clear();
        var seriesIndex = FirstVisibleSeries();
        if (seriesIndex < 0 || SeriesSource![seriesIndex].ItemsSource is not { } points) return;
        var maximum = 0d;
        var count = 0;
        foreach (var point in points)
            if (IsValid(point))
            {
                maximum = Math.Max(maximum, point.Y);
                count++;
            }

        if (count == 0) return;
        var total = 0d;
        foreach (var point in points)
            if (IsValid(point))
                total += point.Y / maximum;
        _radialCenter = plot.Center;
        // Reserve the entire possible lift/scale, so hover cannot collide with the legend or title.
        var half = Math.Min(plot.Width, plot.Height) / 2;
        _radialOuterRadius = Math.Max(0,
            (half - SliceHoverOffset - ChartPadding / 2 - SelectionThickness) / SliceHoverScale);
        _radialInnerRadius = HasRadialHole ? _radialOuterRadius * InnerRadius : 0;
        if (_radialOuterRadius <= 0) return;
        var angle = -Math.PI / 2;
        for (var i = 0; i < points.Count; i++)
        {
            if (!IsValid(points[i])) continue;
            var fraction = points[i].Y / maximum / total;
            var sweep = fraction * Math.Tau;
            if (sweep <= 0) continue;
            var gap = count == 1 ? 0 : Math.Min(0.025, sweep * 0.08);
            var sector = new RadialSector(seriesIndex, i, angle + gap / 2, sweep - gap);
            _radialSectors.Add(sector);
            _radialSectorLookup[(seriesIndex, i)] = sector;
            _radialFractions[(seriesIndex, i)] = fraction;
            var markRadius = HasRadialHole ? (_radialOuterRadius + _radialInnerRadius) / 2 : _radialOuterRadius * 0.62;
            _positions[(seriesIndex, i)] = RadialPoint(_radialCenter, markRadius, angle + sweep / 2);
            angle += sweep;
        }
    }

    private bool IsRaisedSector(RadialSector sector)
    {
        return IsEffectivelyEnabled && !IsAnimating && _hoverGeometryVersion == _geometryVersion &&
               _hover == (sector.Series, sector.Index);
    }

    private RadialPresentation PresentSector(RadialSector sector, bool animate)
    {
        var angles = animate
            ? PresentRadialAngles(sector.Series, sector.Index, sector.Start, sector.Sweep)
            : (sector.Start, sector.Sweep);
        var raised = IsRaisedSector(sector);
        var center = raised
            ? RadialPoint(_radialCenter, SliceHoverOffset, angles.Item1 + angles.Item2 / 2)
            : _radialCenter;
        var scale = raised ? SliceHoverScale : 1;
        return new RadialPresentation(center, _radialOuterRadius * scale, _radialInnerRadius * scale, angles.Item1,
            angles.Item2);
    }

    private Point GetRadialPresentedPosition((int Series, int Index) key, Point original)
    {
        if (!_radialSectorLookup.TryGetValue(key, out var sector)) return original;
        var presentation = PresentSector(sector, false);
        var radius = HasRadialHole ? (presentation.Outer + presentation.Inner) / 2 : presentation.Outer * 0.62;
        return RadialPoint(presentation.Center, radius, presentation.Start + presentation.Sweep / 2);
    }

    private void DrawRadialSector(DrawingContext context, RadialSector sector)
    {
        var presentation = PresentSector(sector, true);
        var key = (sector.Series, sector.Index);
        if (!_radialDrawCache.TryGetValue(key, out var cached))
            _radialDrawCache[key] = cached = new RadialDrawCache();
        if (cached.Fill is null || cached.Presentation != presentation)
        {
            var geometry = AnnularSector(presentation.Center, presentation.Outer, presentation.Inner,
                presentation.Start, presentation.Sweep);
            cached.Fill = HasRadialHole && _radialInnerRadius > 0
                ? new CombinedGeometry
                {
                    GeometryCombineMode = GeometryCombineMode.Exclude,
                    Geometry1 = geometry,
                    Geometry2 = new EllipseGeometry(new Rect(_radialCenter.X - _radialInnerRadius,
                        _radialCenter.Y - _radialInnerRadius, _radialInnerRadius * 2, _radialInnerRadius * 2))
                } : geometry;
            cached.Presentation = presentation;
        }
        var selected = Selection is { } selection && selection.SeriesIndex == sector.Series &&
                       selection.PointIndex == sector.Index;
        context.DrawGeometry(SeriesBrush(sector.Series, sector.Index),
            selected ? _radialSelectionPen ??= new Pen(Background, SelectionThickness) : null, cached.Fill);
        var markRadius = HasRadialHole ? (presentation.Outer + presentation.Inner) / 2 : presentation.Outer * 0.62;
        if (ShowDataLabels && presentation.Sweep * markRadius >= FontSize * 3 && presentation.Outer >= FontSize * 3)
        {
            var mark = RadialPoint(presentation.Center, markRadius, presentation.Start + presentation.Sweep / 2);
            var text = cached.Label ??= Text($"{_radialFractions[key]:P0}", Background);
            var label = new Rect(mark.X - text.Width / 2, mark.Y - text.Height / 2, text.Width, text.Height);
            if (SectorContains(presentation, label.TopLeft) && SectorContains(presentation, label.TopRight) &&
                SectorContains(presentation, label.BottomLeft) && SectorContains(presentation, label.BottomRight) &&
                !IntersectsRadialHole(label, presentation.Center, presentation.Inner) &&
                !IntersectsRadialHole(label, _radialCenter, _radialInnerRadius))
                context.DrawText(text, label.TopLeft);
        }
    }

    private void ClearRadialDrawCache()
    {
        _radialDrawCache.Clear();
        _radialSelectionPen = null;
    }

    private static bool IntersectsRadialHole(Rect bounds, Point center, double radius)
    {
        if (radius <= 0) return false;
        var closest = new Point(Math.Clamp(center.X, bounds.Left, bounds.Right),
            Math.Clamp(center.Y, bounds.Top, bounds.Bottom));
        var distance = closest - center;
        return distance.X * distance.X + distance.Y * distance.Y < radius * radius;
    }

    private (int Series, int Index)? HitTestRadial(Point point)
    {
        if (IsGauge || _positions.Count == 0 || !double.IsFinite(point.X) || !double.IsFinite(point.Y)) return null;
        var original = point - _radialCenter;
        // Preserve the entire resting doughnut hole, even under an offset full-circle slice.
        if (HasRadialHole &&
            original.X * original.X + original.Y * original.Y < _radialInnerRadius * _radialInnerRadius) return null;
        if (_hover is { } key && _radialSectorLookup.TryGetValue(key, out var raised) && IsRaisedSector(raised) &&
            SectorContains(PresentSector(raised, false), point)) return key;
        foreach (var sector in _radialSectors)
        {
            // Keeping the resting sector interactive prevents hover from oscillating at the lifted edge.
            var resting = new RadialPresentation(_radialCenter, _radialOuterRadius, _radialInnerRadius, sector.Start,
                sector.Sweep);
            if (SectorContains(resting, point)) return (sector.Series, sector.Index);
        }

        return null;
    }

    private static bool SectorContains(RadialPresentation sector, Point point)
    {
        var offset = point - sector.Center;
        var squared = offset.X * offset.X + offset.Y * offset.Y;
        if (squared > sector.Outer * sector.Outer || squared < sector.Inner * sector.Inner) return false;
        var delta = (Math.Atan2(offset.Y, offset.X) - sector.Start) % Math.Tau;
        if (delta < 0) delta += Math.Tau;
        return delta <= sector.Sweep;
    }

    private protected static Point RadialPoint(Point center, double radius, double angle)
    {
        return new Point(center.X + radius * Math.Cos(angle), center.Y + radius * Math.Sin(angle));
    }

    private protected static StreamGeometry AnnularSector(Point center, double outer, double inner, double start,
        double sweep)
    {
        var geometry = new StreamGeometry();
        using var path = geometry.Open();
        path.BeginFigure(RadialPoint(center, outer, start));
        var half = sweep / 2;
        var outerSize = new Size(outer, outer);
        path.ArcTo(RadialPoint(center, outer, start + half), outerSize, 0, false, SweepDirection.Clockwise);
        path.ArcTo(RadialPoint(center, outer, start + sweep), outerSize, 0, false, SweepDirection.Clockwise);
        if (sweep >= Math.Tau - 1e-12)
        {
            // Separate closed contours avoid a selected full circle/ring gaining a radial seam.
            path.EndFigure(true);
            if (inner > 0)
            {
                var fullInner = new Size(inner, inner);
                path.BeginFigure(RadialPoint(center, inner, start));
                path.ArcTo(RadialPoint(center, inner, start - Math.PI), fullInner, 0, false,
                    SweepDirection.CounterClockwise);
                path.ArcTo(RadialPoint(center, inner, start - Math.Tau), fullInner, 0, false,
                    SweepDirection.CounterClockwise);
                path.EndFigure(true);
            }

            return geometry;
        }

        if (inner > 0)
        {
            path.LineTo(RadialPoint(center, inner, start + sweep));
            var innerSize = new Size(inner, inner);
            path.ArcTo(RadialPoint(center, inner, start + half), innerSize, 0, false, SweepDirection.CounterClockwise);
            path.ArcTo(RadialPoint(center, inner, start), innerSize, 0, false, SweepDirection.CounterClockwise);
        }
        else
        {
            path.LineTo(center);
        }

        path.EndFigure(true);
        return geometry;
    }

    // These angles remain target geometry. Animation never writes presented angles into this cache.
    private readonly record struct RadialSector(int Series, int Index, double Start, double Sweep);

    private readonly record struct RadialPresentation(
        Point Center,
        double Outer,
        double Inner,
        double Start,
        double Sweep);

    private sealed class RadialDrawCache
    {
        public RadialPresentation Presentation { get; set; }
        public Geometry? Fill { get; set; }
        public FormattedText? Label { get; set; }
    }
}
