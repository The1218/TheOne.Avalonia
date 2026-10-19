using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

/// <summary>One drawing surface per visible page, independent of the number of source rows or cells.</summary>
internal sealed partial class ReportPageSurface : Control
{
    internal const double LeftGutter = 88, TopGutter = 30;
    private static readonly IBrush Ink = new SolidColorBrush(Color.Parse("#212222"));
    private static readonly IBrush MutedInk = new SolidColorBrush(Color.Parse("#657171"));
    private static readonly IBrush PrintAccent = new SolidColorBrush(Color.Parse("#006061"));
    private static readonly IBrush PaperLine = new SolidColorBrush(Color.Parse("#D2D5D5"));
    private static readonly IBrush Stripe = new SolidColorBrush(Color.Parse("#F3F7F7"));
    private string? _error;
    private ReportLayout? _layout;
    private int _pageIndex, _selected;
    private bool _preview;
    private double _zoom = .72;

    public ReportPageSurface()
    {
        Focusable = true;
        // Focusing the page must not scroll the whole oversized drawing surface into view mid-drag.
        AddHandler(RequestBringIntoViewEvent, (_, e) =>
        {
            if (ReferenceEquals(e.Source, this)) e.Handled = true;
        });
        AutomationProperties.SetName(this, "报表纸张；左右方向键选择表格列");
    }

    internal bool IsPrinting { get; set; }
    internal bool ShowRulers { get; set; } = true;
    private double PaperOffsetX => IsPrinting ? 0 : LeftGutter;
    private double PaperOffsetY => IsPrinting ? 0 : TopGutter;
    internal event Action<int>? ColumnSelected;

    internal void Update(ReportLayout? layout, bool preview, int pageIndex, int selected, double zoom, string? error)
    {
        _layout = layout;
        _preview = preview;
        _pageIndex = pageIndex;
        _selected = selected;
        _zoom = zoom;
        _error = error;
        Width = PaperOffsetX + (layout?.PageWidth ?? 794) * zoom + (IsPrinting ? 0 : 24);
        Height = PaperOffsetY + (layout?.PageHeight ?? 1123) * zoom + (IsPrinting ? 0 : 24);
        InvalidateVisual();
    }

    private IBrush UiBrush(string key, IBrush fallback)
    {
        return this.TryFindResource(key, ActualThemeVariant, out var value) && value is IBrush brush ? brush : fallback;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_document is not null && (_document.Elements.Count > 0 || _document.Columns.Count == 0))
        {
            RenderDesigner(context);
            return;
        }

        if (_layout is not { } layout)
        {
            Text(context, _error ?? "连接数据源后开始设计",
                new Rect(LeftGutter, 80, Math.Max(200, Bounds.Width - LeftGutter - 20), 160), UiBrush("One.Text", Ink),
                14);
            return;
        }

        var paperRect = new Rect(PaperOffsetX, PaperOffsetY, layout.PageWidth * _zoom, layout.PageHeight * _zoom);
        if (!IsPrinting)
            context.DrawRectangle(new SolidColorBrush(Color.FromArgb(18, 0, 0, 0)), null,
                paperRect.Translate(new Vector(3, 4)));
        context.DrawRectangle(Brushes.White, IsPrinting ? null : new Pen(PaperLine), paperRect);
        if (!IsPrinting && ShowRulers) DrawRulers(context, layout);
        using (context.PushTransform(Matrix.CreateScale(_zoom, _zoom) *
                                     Matrix.CreateTranslation(PaperOffsetX, PaperOffsetY)))
        using (context.PushClip(new Rect(new Size(layout.PageWidth, layout.PageHeight))))
        {
            var margin = layout.Margin;
            context.DrawLine(new Pen(PrintAccent, 2), new Point(margin, margin),
                new Point(layout.PageWidth - margin, margin));
            var title = ReportPaginator.MeasureText(layout.Title, layout.ContentWidth, 27, Ink, true);
            context.DrawText(title, new Point(margin, margin + 18));
            var subtitleY = margin + 22 + title.Height;
            var subtitle = ReportPaginator.MeasureText(layout.Subtitle, layout.ContentWidth, 11, MutedInk);
            context.DrawText(subtitle, new Point(margin, subtitleY));
            Text(context, layout.Description,
                new Rect(margin, subtitleY + subtitle.Height + 6, layout.ContentWidth,
                    layout.TableTop - subtitleY - subtitle.Height - 10), MutedInk, 11);
            var y = layout.TableTop;
            DrawCells(context, layout, layout.Columns.Select(c => c.Header).ToArray(), y, layout.HeaderHeight, true);
            y += layout.HeaderHeight;
            var page = layout.Pages[_pageIndex];
            if (_preview)
            {
                foreach (var row in page.Rows)
                {
                    DrawCells(context, layout, row.Cells, y, row.Height, false, row.SourceIndex % 2 == 0,
                        row.Foregrounds);
                    y += row.Height;
                }

                if (page.Rows.Count == 0)
                {
                    Text(context, "暂无明细数据", new Rect(margin, y + 8, layout.ContentWidth, 24), MutedInk,
                        layout.FontSize);
                    y += 36;
                }

                if (page.ShowSummary) DrawSummary(context, layout, layout.Summary, y + 8);
            }
            else
            {
                var template = layout.Columns.Select((c, i) => c.FieldKey.Length == 0
                    ? _document!.Columns[i].Text : "{" + c.FieldKey + "}").ToArray();
                var templateHeight = Math.Max(36,
                    layout.Columns.Select((c, i) =>
                        ReportPaginator.MeasureText(template[i], c.Width - 16, layout.FontSize).Height + 14).Max());
                DrawCells(context, layout, template, y, templateHeight, false, true);
                DrawSelection(context, layout, y, templateHeight);
                y += templateHeight + 18;
                Text(context, "示例数据 · 实际展开请切换预览", new Rect(margin, y, layout.ContentWidth, 20), MutedInk, 10);
                y += 22;
                foreach (var row in layout.Pages.SelectMany(p => p.Rows).Take(3))
                {
                    if (y + row.Height + layout.SummaryHeight + 30 > layout.DetailBottom) break;
                    DrawCells(context, layout, row.Cells, y, row.Height, false, row.SourceIndex % 2 == 0,
                        row.Foregrounds);
                    y += row.Height;
                }

                if (layout.Summary.Any(s => s.Length > 0))
                    DrawSummary(context, layout,
                        layout.Columns.Select((c, i) => layout.Summary[i].Length > 0 ? "Sum(" + c.FieldKey + ")" : "")
                            .ToArray(), y + 18);
            }

            var footerY = layout.PageHeight - margin - 24;
            context.DrawLine(new Pen(PrintAccent), new Point(margin, footerY),
                new Point(layout.PageWidth - margin, footerY));
            Text(context, layout.Footer, new Rect(margin, footerY + 7, layout.ContentWidth * .55, 18), MutedInk, 10);
            Text(context,
                _preview ? $"第 {page.Number} 页 / 共 {layout.Pages.Count} 页" : "第 {PageNumber} 页 / 共 {TotalPages} 页",
                new Rect(margin + layout.ContentWidth * .55, footerY + 7, layout.ContentWidth * .45, 18), MutedInk, 10,
                right: true);
        }

        if (!_preview)
        {
            Band(context, "报表标题", layout.Margin + 35);
            Band(context, "页眉", layout.TableTop + layout.HeaderHeight / 2);
            Band(context, "明细模板", layout.TableTop + layout.HeaderHeight + 18);
            Band(context, "页脚", layout.PageHeight - layout.Margin - 20);
        }
    }

    private void DrawRulers(DrawingContext context, ReportLayout layout)
    {
        var brush = UiBrush("One.Muted", MutedInk);
        var pen = new Pen(UiBrush("One.Line", PaperLine));
        for (var mm = 0; mm <= layout.PageWidth / ReportPaginator.DipsPerMillimetre; mm += 5)
        {
            var x = LeftGutter + mm * ReportPaginator.DipsPerMillimetre * _zoom;
            context.DrawLine(pen, new Point(x, TopGutter - (mm % 20 == 0 ? 9 : 4)), new Point(x, TopGutter - 1));
            if (mm % 20 == 0) Text(context, mm.ToString(), new Rect(x, 0, 32, 16), brush, 9);
        }

        for (var mm = 0; mm <= layout.PageHeight / ReportPaginator.DipsPerMillimetre; mm += 10)
        {
            var y = TopGutter + mm * ReportPaginator.DipsPerMillimetre * _zoom;
            context.DrawLine(pen, new Point(LeftGutter - 5, y), new Point(LeftGutter - 1, y));
        }
    }

    private void Band(DrawingContext context, string label, double y)
    {
        var brush = UiBrush("One.Muted", MutedInk);
        Text(context, label, new Rect(0, TopGutter + y * _zoom - 8, 70, 20), brush, 10, right: true);
        context.DrawLine(new Pen(UiBrush("One.Line", PaperLine)), new Point(74, TopGutter + y * _zoom),
            new Point(LeftGutter - 5, TopGutter + y * _zoom));
    }

    private static void DrawCells(DrawingContext context, ReportLayout layout, IReadOnlyList<string> cells, double y,
        double height, bool header, bool stripe = false, IReadOnlyList<string>? colors = null)
    {
        var x = layout.Margin;
        for (var c = 0; c < layout.Columns.Count; c++)
        {
            var column = layout.Columns[c];
            var rect = new Rect(x, y, column.Width, height);
            context.DrawRectangle(header ? PrintAccent : stripe ? Stripe : Brushes.White, new Pen(PaperLine, .5), rect);
            var cellInk = colors is not null && c < colors.Count && Color.TryParse(colors[c], out var color)
                ? new SolidColorBrush(color)
                : Ink;
            Text(context, cells[c], rect.Deflate(new Thickness(8, 6)), header ? Brushes.White : cellInk,
                layout.FontSize, header, !header && column.IsNumeric);
            x += column.Width;
        }
    }

    private static void DrawSummary(DrawingContext context, ReportLayout layout, IReadOnlyList<string> cells, double y,
        bool template = false)
    {
        context.DrawLine(new Pen(PaperLine), new Point(layout.TableX, y),
            new Point(layout.TableX + layout.ContentWidth, y));
        var x = layout.TableX;
        for (var i = 0; i < layout.Columns.Count; i++)
        {
            var expression = template && cells[i].StartsWith("Sum(", StringComparison.Ordinal);
            Text(context, cells[i],
                new Rect(expression ? layout.TableX + 80 : x + 8, y + 8,
                    expression ? x + layout.Columns[i].Width - layout.TableX - 88 : layout.Columns[i].Width - 16,
                    layout.SummaryHeight - 8), Ink, expression ? 13 : layout.FontSize, !expression,
                layout.Columns[i].IsNumeric);
            x += layout.Columns[i].Width;
        }

        if (cells[0].Length == 0)
            Text(context, "合计", new Rect(layout.TableX + 8, y + 8, layout.Columns[0].Width - 16, 24), Ink,
                layout.FontSize, true);
    }

    private void DrawSelection(DrawingContext context, ReportLayout layout, double y, double height)
    {
        if (_selected < 0 || _selected >= layout.Columns.Count) return;
        var x = layout.Margin + layout.Columns.Take(_selected).Sum(c => c.Width);
        var rect = new Rect(x, y, layout.Columns[_selected].Width, height);
        context.DrawRectangle(null, new Pen(PrintAccent, 1.8), rect);
        foreach (var point in new[] { rect.TopLeft, rect.TopRight, rect.BottomLeft, rect.BottomRight })
            context.DrawRectangle(Brushes.White, new Pen(PrintAccent),
                new Rect(point - new Vector(2.5, 2.5), new Size(5, 5)));
    }

    private static void Text(DrawingContext context, string value, Rect area, IBrush brush, double size,
        bool bold = false, bool right = false)
    {
        if (area.Width < 1 || area.Height < 1 || value.Length == 0) return;
        var text = ReportPaginator.MeasureText(value, area.Width, size, brush, bold);
        using (context.PushClip(area))
        {
            context.DrawText(text,
                new Point(right ? area.Right - text.Width : area.X,
                    area.Y + Math.Max(0, (area.Height - text.Height) / 2)));
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_document is not null && (_document.Elements.Count > 0 || _document.Columns.Count == 0))
        {
            DesignerPressed(e);
            return;
        }

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed || _preview || _layout is not { } layout) return;
        var p = e.GetPosition(this);
        var y = (p.Y - TopGutter) / _zoom;
        if (y < layout.TableTop || y > layout.TableTop + layout.HeaderHeight + 100) return;
        var x = (p.X - LeftGutter) / _zoom - layout.Margin;
        for (var c = 0; c < layout.Columns.Count; c++)
        {
            if (x >= 0 && x < layout.Columns[c].Width)
            {
                ColumnSelected?.Invoke(c);
                Focus();
                e.Handled = true;
                return;
            }

            x -= layout.Columns[c].Width;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (DesignerKey(e)) return;
        if (_preview || _layout is null || e.Key is not (Key.Left or Key.Right)) return;
        ColumnSelected?.Invoke(Math.Clamp(_selected + (e.Key == Key.Right ? 1 : -1), 0, _layout.Columns.Count - 1));
        e.Handled = true;
    }
}
