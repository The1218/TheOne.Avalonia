using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportPageSurface
{
    private const double MinimumDesignerRowHeight = 28;
    // Keep a small rounding allowance above the paginator's 32-DIP column limit.
    private const double MinimumColumnWidth = 32.01 / ReportPaginator.DipsPerMillimetre;
    private ReportDocument? _document;
    private ReportElement? _element;
    private Point? _moveOrigin, _dropHint;
    private Rect _original;
    private bool _resizeWidth, _resizeHeight, _moved;
    private int _columnBoundary = -1, _rowBoundary = -1, _bandBoundary = -1;
    private double[] _columnWidths = [];
    private (ReportElement Element, double Y)[] _footerElements = [];
    private double _headerHeight, _columnWeightTotal, _bandHeight, _footerTop;
    internal bool Snap { get; set; } = true;
    internal string SourceName { get; set; } = "";
    internal event Action<ReportElement?>? ElementSelected;
    internal event Action? GestureStarting, GestureFinished, DeleteRequested;
    internal void SetDocument(ReportDocument? document, ReportElement? selected) { _document = document; _element = selected; PruneImages(); }
    internal Rect PageRect(Rect millimetres)
    {
        var scale = _zoom * ReportPaginator.DipsPerMillimetre;
        return new Rect(LeftGutter + millimetres.X * scale, TopGutter + millimetres.Y * scale, millimetres.Width * scale, millimetres.Height * scale);
    }
    internal Point? PagePoint(Point point)
    {
        if (_document is null) return null;
        var p = new Point((point.X - LeftGutter) / _zoom / ReportPaginator.DipsPerMillimetre, (point.Y - TopGutter) / _zoom / ReportPaginator.DipsPerMillimetre);
        return p.X >= 0 && p.Y >= 0 && p.X <= _document.PageWidthMm && p.Y <= _document.PageHeightMm ? p : null;
    }
    internal void ShowDropHint(Point? point) { _dropHint = point; InvalidateVisual(); }
    internal ReportElement? ElementAt(Point mm) => _document?.Elements.LastOrDefault(e => new Rect(e.X, e.Y, e.Width, Math.Max(2, e.Height)).Contains(mm));
    internal int ColumnAt(ReportElement table, Point mm)
    {
        if (_document is null) return -1;
        var x = mm.X - table.X; var weight = _document.Columns.Sum(c => c.WidthWeight);
        for (var i = 0; i < _document.Columns.Count; i++)
        {
            var width = table.Width * _document.Columns[i].WidthWeight / weight;
            if (x >= 0 && x < width) return i;
            x -= width;
        }
        return -1;
    }
    private void RenderDesigner(DrawingContext context)
    {
        var document = _document!; var d = ReportPaginator.DipsPerMillimetre;
        var width = document.PageWidthMm * d; var height = document.PageHeightMm * d;
        if (!double.IsFinite(width) || !double.IsFinite(height) || width <= 0 || height <= 0) return;
        var paper = new Rect(PaperOffsetX, PaperOffsetY, width * _zoom, height * _zoom);
        if (!IsPrinting) context.DrawRectangle(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), null, paper.Translate(new Vector(3, 4)));
        context.DrawRectangle(Brushes.White, IsPrinting ? null : new Pen(PaperLine, 1), paper);
        var layout = _layout;
        if (layout is null && !_preview) { try { layout = ReportPaginator.Paginate(document); } catch (ArgumentException) { } }
        if (layout is not null && !IsPrinting && ShowRulers) DrawRulers(context, layout);
        using (context.PushTransform(Matrix.CreateScale(_zoom, _zoom) * Matrix.CreateTranslation(PaperOffsetX, PaperOffsetY)))
        using (context.PushClip(new Rect(new Size(width, height))))
        {
            if (!_preview)
            {
                var left = document.LeftMargin * d; var right = width - document.RightMargin * d;
                var guide = new Pen(PrintAccent, .7 / _zoom, DashStyle.Dash);
                foreach (var y in new[] { document.TopMargin, document.HeaderBottom, document.FooterTop, document.PageHeightMm - document.BottomMargin })
                    context.DrawLine(guide, new Point(0, y * d), new Point(width, y * d));
                context.DrawRectangle(null, new Pen(PaperLine, .6, DashStyle.Dash), new Rect(left, document.TopMargin * d, Math.Max(1, right - left), Math.Max(1, height - (document.TopMargin + document.BottomMargin) * d)));
            }
            var elementIndex = -1;
            foreach (var element in document.Elements)
            {
                elementIndex++;
                if (!double.IsFinite(element.X) || !double.IsFinite(element.Y) || !double.IsFinite(element.Width) || !double.IsFinite(element.Height) || element.Width <= 0 || element.Height <= 0) continue;
                var area = new Rect(element.X * d, element.Y * d, element.Width * d, element.Height * d);
                if (element.Kind == ReportElementKind.Table)
                {
                    if (layout is null || layout.Columns.Count == 0) continue;
                    if (!_preview && document.Columns.Count == layout.Columns.Count)
                    {
                        var total = document.Columns.Sum(c => c.WidthWeight);
                        var columns = Array.AsReadOnly(layout.Columns.Select((column, i) => column with { Width = element.Width * d * document.Columns[i].WidthWeight / total }).ToArray());
                        var header = TableHeaderHeight(element) * d;
                        layout = layout with { Columns = columns, HeaderHeight = header, TableTop = area.Y, TableWidth = area.Width };
                    }
                    var tableLayout = layout with { TableX = area.X, TableTop = area.Y };
                    var x = area.X; var y = area.Y;
                    void Cells(IReadOnlyList<string> values, double rowHeight, bool header, IReadOnlyList<string>? colors = null, bool template = false)
                    {
                        x = area.X;
                        for (var c = 0; c < layout.Columns.Count; c++)
                        {
                            var cell = new Rect(x, y, layout.Columns[c].Width, rowHeight);
                            IBrush headerFill = Color.TryParse(element.Fill, out var tableFill) && tableFill.A > 0 ? new SolidColorBrush(tableFill) : PrintAccent;
                            IBrush headerInk = Color.TryParse(element.Foreground, out var tableInk) ? new SolidColorBrush(tableInk) : Brushes.White;
                            context.DrawRectangle(header ? headerFill : Brushes.White, element.ShowBorder ? new Pen(PaperLine, .6) : null, cell);
                            var cellInk = colors is not null && c < colors.Count && Color.TryParse(colors[c], out var cellColor) ? new SolidColorBrush(cellColor) : Ink;
                            ElementText(context, values[c], cell.Deflate(new Thickness(7, 5)), header ? headerInk : cellInk, element, header, !header && layout.Columns[c].IsNumeric, header, template ? Math.Min(element.FontSize, 13) : null);
                            x += layout.Columns[c].Width;
                        }
                        y += rowHeight;
                    }
                    Cells(document.Columns.Select(c => c.Header).ToArray(), layout.HeaderHeight, true);
                    if (_preview)
                    {
                        var page = layout.Pages[Math.Clamp(_pageIndex, 0, layout.Pages.Count - 1)];
                        foreach (var row in page.Rows) Cells(row.Cells, row.Height, false, row.Foregrounds);
                        if (page.ShowSummary) DrawSummary(context, tableLayout with { Margin = area.X }, layout.Summary, y + 8);
                    }
                    else
                    {
                        Cells(document.Columns.Select(c => c.FieldKey.Length > 0 ? "{" + c.FieldKey + "}" : c.Text).ToArray(), Math.Max(MinimumDesignerRowHeight, area.Height - layout.HeaderHeight), false, template: true);
                        if (layout.Pages.SelectMany(p => p.Rows).Any(r => r.Cells.Any(t => t.Length > 0)))
                        {
                            y += 14; Text(context, "示例数据", new Rect(area.X + 8, y, area.Width - 16, 20), PrintAccent, 10); y += 22;
                            foreach (var row in layout.Pages.SelectMany(p => p.Rows).Take(3))
                            { var sampleHeight = Math.Min(32, row.Height); if (y + sampleHeight + layout.SummaryHeight > layout.DetailBottom) break; Cells(row.Cells, sampleHeight, false, row.Foregrounds); }
                        }
                        if (document.Columns.Any(c => c.ShowSum))
                            DrawSummary(context, tableLayout, document.Columns.Select(c => c.ShowSum ? "Sum(" + (SourceName.Length > 0 ? SourceName + "." : "") + c.FieldKey + ")" : "").ToArray(), y + 16, template: true);
                    }
                }
                else
                {
                    var brush = Color.TryParse(element.Foreground, out var color) ? new SolidColorBrush(color) : Ink;
                    IBrush fill = Color.TryParse(element.Fill, out var fillColor) ? new SolidColorBrush(fillColor) : Brushes.Transparent;
                    if (element.Kind == ReportElementKind.Line) context.DrawLine(new Pen(brush, 1), area.TopLeft, area.TopRight);
                    else
                    {
                        context.DrawRectangle(fill, element.ShowBorder || element.Kind == ReportElementKind.Rectangle ? new Pen(brush, 1) : null, area);
                        var text = element.Text;
                        if (element.FieldKey.Length > 0) text = layout?.Elements.ElementAtOrDefault(elementIndex)?.Text ?? "{" + element.FieldKey + "}";
                        if (element.Kind == ReportElementKind.PageNumber)
                            text = _preview ? $"第 {_pageIndex + 1} 页 / 共 {layout?.Pages.Count ?? 1} 页" : "第 {PageNumber} 页 / 共 {TotalPages} 页";
                        if (element.Kind == ReportElementKind.Image) DrawImage(context, element, area);
                        else if (element.Kind == ReportElementKind.CheckBox) DrawCheckBox(context, element, text, area, brush);
                        else if (element.Kind == ReportElementKind.Barcode) DrawBarcode(context, element, text, area, brush);
                        else if (element.Kind != ReportElementKind.Rectangle) ElementText(context, text, area.Deflate(2), brush, element);
                    }
                }
                if (!_preview && ReferenceEquals(element, _element))
                {
                    if (element.Kind == ReportElementKind.Table)
                        context.DrawRectangle(null, new Pen(PrintAccent, .8 / _zoom, DashStyle.Dash), area);
                    var selection = area;
                    if (element.Kind == ReportElementKind.Table && layout is not null && _selected >= 0 && _selected < layout.Columns.Count)
                        selection = new Rect(area.X + layout.Columns.Take(_selected).Sum(c => c.Width), area.Y + layout.HeaderHeight, layout.Columns[_selected].Width, Math.Max(MinimumDesignerRowHeight, area.Height - layout.HeaderHeight));
                    context.DrawRectangle(null, new Pen(PrintAccent, 1.5 / _zoom), selection);
                    foreach (var point in new[] { selection.TopLeft, selection.TopRight, selection.BottomLeft, selection.BottomRight })
                        context.DrawRectangle(Brushes.White, new Pen(PrintAccent, 1 / _zoom), new Rect(point - new Vector(3 / _zoom, 3 / _zoom), new Size(6 / _zoom, 6 / _zoom)));
                    if (element.Kind == ReportElementKind.Table)
                    {
                        foreach (var point in new[] { new Point(area.Right, area.Center.Y), area.BottomRight })
                            context.DrawRectangle(PrintAccent, new Pen(Brushes.White, 1 / _zoom), new Rect(point - new Vector(4 / _zoom, 4 / _zoom), new Size(8 / _zoom, 8 / _zoom)));
                    }
                }
            }
            if (!_preview && document.Elements.Count == 0)
                Text(context, "从左侧拖入组件，开始设计报表", new Rect(document.MarginMm * d, height / 2 - 18, width - document.MarginMm * d * 2, 36), MutedInk, 15);
            if (_dropHint is { } hint && !_preview)
                context.DrawRectangle(new SolidColorBrush(Color.FromArgb(40, 0, 96, 97)), new Pen(PrintAccent, 1, DashStyle.Dash), new Rect(hint.X * d, hint.Y * d, 50 * d, 12 * d));
            if (_preview && _error is not null) Text(context, _error, new Rect(document.MarginMm * d, 20, width - document.MarginMm * d * 2, 60), Brushes.DarkRed, 12);
        }
        if (!_preview)
        {
            Band(context, "页眉", (document.TopMargin + document.HeaderHeightMm / 2) * d);
            Band(context, "明细", (document.HeaderBottom + 8) * d);
            Band(context, "页脚", (document.FooterTop + document.FooterHeightMm / 2) * d);
            foreach (var y in new[] { document.HeaderBottom, document.FooterTop })
            {
                var grip = new Rect(LeftGutter - 14, TopGutter + y * d * _zoom - 5, 20, 10);
                context.DrawRectangle(Brushes.White, new Pen(PrintAccent, 1), grip, 2, 2);
                context.DrawLine(new Pen(PrintAccent, 1), new Point(grip.X + 5, grip.Center.Y - 1.5), new Point(grip.Right - 5, grip.Center.Y - 1.5));
                context.DrawLine(new Pen(PrintAccent, 1), new Point(grip.X + 5, grip.Center.Y + 1.5), new Point(grip.Right - 5, grip.Center.Y + 1.5));
            }
            if (_element is { Kind: ReportElementKind.Table } table && layout is not null && _selected >= 0 && _selected < document.Columns.Count && document.Columns[_selected].FieldKey is { Length: > 0 } key)
            {
                var label = "ƒx  " + (SourceName.Length > 0 ? SourceName + "." : "") + key;
                var labelWidth = Math.Clamp(ReportPaginator.MeasureText(label, 300, 13).Width + 20, 120, 240);
                var tag = new Rect(LeftGutter + (table.X * d + layout.Columns.Take(_selected).Sum(c => c.Width) + 22) * _zoom,
                    TopGutter + (table.Y * d + layout.HeaderHeight - 18) * _zoom, labelWidth * _zoom, 30 * _zoom);
                context.DrawRectangle(new SolidColorBrush(Color.FromArgb(20, 0, 0, 0)), null, tag.Translate(new Vector(2, 2)), 5, 5);
                context.DrawRectangle(new SolidColorBrush(Color.Parse("#EFF7FF")), new Pen(new SolidColorBrush(Color.Parse("#C7D9E6")), 1), tag, 5, 5);
                Text(context, label, tag.Deflate(new Thickness(8, 2)), new SolidColorBrush(Color.Parse("#006595")), 13 * _zoom);
            }
        }
    }
    private void DesignerPressed(PointerPressedEventArgs e)
    {
        if (_preview || _document is null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        var local = e.GetPosition(this);
        if (BandBoundary(local) is var band && band >= 0)
        {
            _bandBoundary = band;
            _bandHeight = band == 0 ? _document.HeaderHeightMm : _document.FooterHeightMm;
            _footerTop = _document.FooterTop;
            _footerElements = band == 1 ? _document.Elements.Where(item => item.Y >= _footerTop).Select(item => (item, item.Y)).ToArray() : [];
            _moveOrigin = new Point((local.X - LeftGutter) / _zoom / ReportPaginator.DipsPerMillimetre, (local.Y - TopGutter) / _zoom / ReportPaginator.DipsPerMillimetre);
            _moved = false; e.Pointer.Capture(this); Focus(); e.Handled = true;
            return;
        }
        if (PagePoint(local) is not { } point) return;
        var tolerance = 6 / _zoom / ReportPaginator.DipsPerMillimetre;
        var element = _element is { } selected && (selected.Kind == ReportElementKind.Table
            && new Rect(selected.X, selected.Y, selected.Width, selected.Height).Inflate(tolerance).Contains(point) || OuterResize(selected, point).Width) ? selected : ElementAt(point);
        ElementSelected?.Invoke(element);
        if (element is null) return;
        _element = element;
        if (element.Kind == ReportElementKind.Table) ColumnSelected?.Invoke(ColumnAt(element, point));
        _moveOrigin = point; _original = new Rect(element.X, element.Y, element.Width, element.Height);
        (_resizeWidth, _resizeHeight) = OuterResize(element, point);
        (_columnBoundary, _rowBoundary) = TableBoundary(element, point);
        if (element.Kind == ReportElementKind.Table && _document is not null)
        {
            var weight = _document.Columns.Sum(c => c.WidthWeight);
            _columnWeightTotal = weight;
            _columnWidths = _document.Columns.Select(c => element.Width * c.WidthWeight / weight).ToArray();
            _headerHeight = TableHeaderHeight(element);
        }
        if (_resizeWidth || _resizeHeight) { _columnBoundary = -1; _rowBoundary = -1; }
        _bandBoundary = -1;
        _moved = false; e.Pointer.Capture(this); Focus(); e.Handled = true;
    }
    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (_moveOrigin is null && !_preview)
        {
            var local = e.GetPosition(this); var band = BandBoundary(local);
            var hoverPoint = PagePoint(local);
            var outer = hoverPoint is { } at && _element is { } selected ? OuterResize(selected, at) : (false, false);
            var hit = hoverPoint is { } boundaryPoint && _element is { } current ? TableBoundary(current, boundaryPoint) : (-1, -1);
            Cursor = new Cursor(band >= 0 ? StandardCursorType.SizeNorthSouth : outer.Item1 && outer.Item2 ? StandardCursorType.BottomRightCorner
                : outer.Item1 || hit.Item1 >= 0 ? StandardCursorType.SizeWestEast : outer.Item2 || hit.Item2 >= 0 ? StandardCursorType.SizeNorthSouth : StandardCursorType.Arrow);
            ToolTip.SetTip(this, band == 0 ? "拖动调整页眉高度" : band == 1 ? "拖动调整页脚高度" : outer.Item1 ? "拖动调整整个表格尺寸" : null);
        }
        if (_moveOrigin is not { } origin || _document is null) return;
        var p = e.GetPosition(this); var d = ReportPaginator.DipsPerMillimetre;
        var point = new Point((p.X - LeftGutter) / _zoom / d, (p.Y - TopGutter) / _zoom / d);
        var delta = point - origin;
        if (!_moved && delta.X * delta.X + delta.Y * delta.Y < .36) return;
        if (!_moved) { GestureStarting?.Invoke(); _moved = true; }
        double SnapValue(double value) => Snap ? Math.Round(value) : value;
        if (_bandBoundary >= 0)
        {
            var maxHeaderBottom = _document.PageHeightMm - _document.BottomMargin - _document.FooterHeightMm - 20;
            var minFooterTop = _document.HeaderBottom + 20;
            foreach (var table in _document.Elements.Where(element => element.Kind == ReportElementKind.Table))
            {
                maxHeaderBottom = Math.Min(maxHeaderBottom, table.Y);
                minFooterTop = Math.Max(minFooterTop, table.Y + table.Height);
            }
            if (_bandBoundary == 0)
                _document.HeaderHeightMm = Math.Clamp(SnapValue(_bandHeight + delta.Y), 4, Math.Max(4, maxHeaderBottom - _document.TopMargin));
            else
            {
                var minimum = Math.Max(4, _footerElements.Select(item => item.Y - _footerTop + item.Element.Height).DefaultIfEmpty(0).Max());
                _document.FooterHeightMm = Math.Clamp(SnapValue(_bandHeight - delta.Y), minimum, Math.Max(minimum, _document.PageHeightMm - _document.BottomMargin - minFooterTop));
                var offset = _document.FooterTop - _footerTop;
                foreach (var item in _footerElements) item.Element.Y = item.Y + offset;
            }
        }
        else if (_element is null) return;
        else if (_columnBoundary >= 0 && _columnBoundary + 1 < _columnWidths.Length)
        {
            var i = _columnBoundary; var total = _columnWidths[i] + _columnWidths[i + 1];
            var minimum = Math.Min(9, total / 2);
            var first = Math.Clamp(SnapValue(_columnWidths[i] + delta.X), minimum, total - minimum);
            // Preserve the original weight scale, the pair's total and the table width.
            for (var c = 0; c < _columnWidths.Length; c++)
                _document.Columns[c].WidthWeight = (c == i ? first : c == i + 1 ? total - first : _columnWidths[c]) * _columnWeightTotal / _original.Width;
        }
        else if (_rowBoundary == 0)
            _element.TableHeaderHeightMm = Math.Clamp(SnapValue(_headerHeight + delta.Y), 4, Math.Max(4, _element.Height - MinimumDesignerRowHeight / d));
        else if (_rowBoundary == 1)
            _element.Height = Math.Clamp(SnapValue(_original.Height + delta.Y), _headerHeight + MinimumDesignerRowHeight / d,
                Math.Max(_headerHeight + MinimumDesignerRowHeight / d, _document.FooterTop - _element.Y));
        else if (_resizeWidth || _resizeHeight)
        {
            var table = _element.Kind == ReportElementKind.Table;
            var minimumWidth = table ? 18d : 8d;
            if (table && _document.Columns.Count > 0 && _document.Columns.All(column => double.IsFinite(column.WidthWeight) && column.WidthWeight > 0))
                minimumWidth = Math.Max(minimumWidth, MinimumColumnWidth * _columnWeightTotal / _document.Columns.Min(column => column.WidthWeight));
            var maximumHeight = (table ? _document.FooterTop : _document.PageHeightMm) - _element.Y;
            if (_resizeWidth)
            {
                var width = Math.Clamp(SnapValue(_original.Width + delta.X), minimumWidth,
                    Math.Max(minimumWidth, _document.PageWidthMm - (table ? _document.RightMargin : 0) - _element.X));
                if (table && width < _original.Width && TableHeaderHeight(_element, width) + MinimumDesignerRowHeight / d > maximumHeight)
                {
                    var lower = width; var upper = _original.Width;
                    for (var i = 0; i < 12; i++)
                    {
                        var middle = (lower + upper) / 2;
                        if (TableHeaderHeight(_element, middle) + MinimumDesignerRowHeight / d <= maximumHeight) upper = middle;
                        else lower = middle;
                    }
                    width = upper;
                }
                _element.Width = width;
            }
            var headerHeight = table ? TableHeaderHeight(_element) : 0;
            var minimumHeight = table ? headerHeight + MinimumDesignerRowHeight / d : 2;
            var desiredHeight = _resizeHeight ? SnapValue(_original.Height + delta.Y) : _original.Height + headerHeight - _headerHeight;
            if (_resizeHeight || table && _resizeWidth) _element.Height = Math.Clamp(desiredHeight, minimumHeight, Math.Max(minimumHeight, maximumHeight));
        }
        else
        {
            _element.X = Math.Clamp(SnapValue(_original.X + delta.X), 0, Math.Max(0, _document.PageWidthMm - _element.Width));
            var top = _element.Kind == ReportElementKind.Table ? _document.HeaderBottom : 0;
            var bottom = _element.Kind == ReportElementKind.Table ? _document.FooterTop : _document.PageHeightMm;
            _element.Y = Math.Clamp(SnapValue(_original.Y + delta.Y), top, Math.Max(top, bottom - _element.Height));
        }
        InvalidateVisual(); e.Handled = true;
    }
    private (bool Width, bool Height) OuterResize(ReportElement element, Point point)
    {
        var tolerance = 6 / _zoom / ReportPaginator.DipsPerMillimetre;
        var right = Math.Abs(point.X - element.X - element.Width) <= tolerance;
        var bottom = Math.Abs(point.Y - element.Y - element.Height) <= tolerance;
        if (right && bottom) return (true, true);
        return (element.Kind == ReportElementKind.Table && right && point.Y >= element.Y && point.Y <= element.Y + element.Height, false);
    }
    private double TableHeaderHeight(ReportElement element, double? width = null)
    {
        var height = element.TableHeaderHeightMm;
        if (_document is null || _document.Columns.Count == 0) return height;
        var total = _document.Columns.Sum(column => column.WidthWeight);
        if (!double.IsFinite(total) || total <= 0) return height;
        foreach (var column in _document.Columns)
        {
            var cellWidth = (width ?? element.Width) * ReportPaginator.DipsPerMillimetre * column.WidthWeight / total;
            height = Math.Max(height, (ReportPaginator.MeasureElementText(column.Header, cellWidth - 16, element, bold: true).Height + 16) / ReportPaginator.DipsPerMillimetre);
        }
        return height;
    }
    private int BandBoundary(Point local)
    {
        if (_preview || _document is null || local.X < LeftGutter - 18 || local.X > LeftGutter + _document.PageWidthMm * ReportPaginator.DipsPerMillimetre * _zoom) return -1;
        var y = (local.Y - TopGutter) / _zoom / ReportPaginator.DipsPerMillimetre;
        var x = (local.X - LeftGutter) / _zoom / ReportPaginator.DipsPerMillimetre;
        if (x >= _document.LeftMargin && x <= _document.PageWidthMm - _document.RightMargin && ElementAt(new Point(x, y)) is not null) return -1;
        var tolerance = 5 / _zoom / ReportPaginator.DipsPerMillimetre;
        return Math.Abs(y - _document.HeaderBottom) <= tolerance ? 0 : Math.Abs(y - _document.FooterTop) <= tolerance ? 1 : -1;
    }
    private (int Column, int Row) TableBoundary(ReportElement element, Point point)
    {
        if (element.Kind != ReportElementKind.Table || _document is null || point.Y < element.Y - 1 || point.Y > element.Y + element.Height + 1.5) return (-1, -1);
        var tolerance = 5 / _zoom / ReportPaginator.DipsPerMillimetre;
        var x = element.X; var weight = _document.Columns.Sum(c => c.WidthWeight);
        if (!double.IsFinite(weight) || weight <= 0 || _document.Columns.Any(column => !double.IsFinite(column.WidthWeight) || column.WidthWeight <= 0)) return (-1, -1);
        for (var i = 0; i < _document.Columns.Count - 1; i++)
        {
            x += element.Width * _document.Columns[i].WidthWeight / weight;
            if (Math.Abs(point.X - x) <= tolerance) return (i, -1);
        }
        if (point.X < element.X || point.X > element.X + element.Width) return (-1, -1);
        if (Math.Abs(point.Y - element.Y - TableHeaderHeight(element)) <= tolerance) return (-1, 0);
        return Math.Abs(point.Y - element.Y - element.Height) <= tolerance ? (-1, 1) : (-1, -1);
    }
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (_moveOrigin is null) return;
        _moveOrigin = null; _bandBoundary = -1; e.Pointer.Capture(null);
        if (_moved) GestureFinished?.Invoke(); _moved = false; _footerElements = []; e.Handled = true;
    }
    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e); _moveOrigin = null; _bandBoundary = -1;
        if (_moved) GestureFinished?.Invoke(); _moved = false; _footerElements = [];
    }
    private bool DesignerKey(KeyEventArgs e)
    {
        if (_document is null || _document.Elements.Count == 0 || _preview || _element is null) return false;
        if (e.Key is Key.Delete or Key.Back) { DeleteRequested?.Invoke(); e.Handled = true; return true; }
        if (e.Key is not (Key.Left or Key.Right or Key.Up or Key.Down)) return false;
        GestureStarting?.Invoke();
        var step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 5 : 1;
        _element.X = Math.Clamp(_element.X + (e.Key == Key.Left ? -step : e.Key == Key.Right ? step : 0), 0, Math.Max(0, _document.PageWidthMm - _element.Width));
        _element.Y = Math.Clamp(_element.Y + (e.Key == Key.Up ? -step : e.Key == Key.Down ? step : 0), 0, Math.Max(0, _document.PageHeightMm - _element.Height));
        GestureFinished?.Invoke(); e.Handled = true; return true;
    }
}
