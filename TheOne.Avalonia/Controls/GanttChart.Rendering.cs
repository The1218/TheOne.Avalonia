using System.Globalization;
using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Media;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class GanttChart
{
    private void Text(DrawingContext context, string value, Rect area, IBrush? brush = null, bool bold = false, bool center = false)
    {
        if (area.Width < 1 || area.Height < 1 || string.IsNullOrEmpty(value)) return;
        var text = new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection,
            new Typeface(FontFamily, FontStyle, bold ? FontWeight.SemiBold : FontWeight), FontSize, brush ?? Foreground ?? Brushes.Black)
        { MaxTextWidth = area.Width, MaxTextHeight = area.Height, Trimming = TextTrimming.CharacterEllipsis };
        using (context.PushClip(area)) context.DrawText(text, new Point(center ? area.X + Math.Max(0, (area.Width - text.Width) / 2) : area.X, area.Y + Math.Max(0, (area.Height - text.Height) / 2)));
    }
    private (double Pane, double NameEnd, bool Details) Columns(double width)
    {
        var pane = PaneWidth(width); var details = pane >= 350;
        return (pane, details ? pane - 148 : pane, details);
    }
    private (int First, int Last) VisibleDays(double width)
    {
        var count = Math.Max(0, ViewEnd.DayNumber - ViewStart.DayNumber);
        return (Math.Clamp((int)Math.Floor(_offset / DayWidth), 0, count),
            Math.Clamp((int)Math.Ceiling((_offset + Math.Max(0, width - PaneWidth(width))) / DayWidth), 0, count));
    }
    private void Grid(DrawingContext context, double width, double height)
    {
        if (ViewEnd <= ViewStart) return;
        var pane = PaneWidth(width); var days = VisibleDays(width); var pen = new Pen(GridBrush, 1);
        using (context.PushClip(new Rect(pane, 0, Math.Max(0, width - pane), height)))
        {
            for (var day = days.First; day < days.Last; day++)
            {
                var date = ViewStart.AddDays(day); var x = TimelineX(date, width);
                if (date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    context.DrawRectangle(WeekendBrush, null, new Rect(x, 0, DayWidth, height));
                if (DayWidth >= 20 || date.DayOfWeek == DayOfWeek.Monday)
                    context.DrawLine(pen, new Point(x, 0), new Point(x, height));
            }
        }
    }
    private void TodayLine(DrawingContext context, double width, double height)
    {
        if (Today < ViewStart || Today >= ViewEnd) return;
        var x = TimelineX(Today, width) + DayWidth / 2;
        if (x >= PaneWidth(width) && x <= width) context.DrawLine(new Pen(ProgressBrush, 1.5), new Point(x, 0), new Point(x, height));
    }
    internal void DrawHeader(DrawingContext context, Size size)
    {
        var width = _rowWidth > 0 && _rows.Count > 0 ? Math.Min(_rowWidth, size.Width) : size.Width;
        var (pane, nameEnd, details) = Columns(width);
        context.DrawRectangle(HeaderBrush, null, new Rect(size));
        var pen = new Pen(GridBrush, 1); var half = size.Height / 2;
        Text(context, "#", new Rect(0, 0, 36, size.Height), MutedBrush, center: true);
        Text(context, TaskHeader, new Rect(48, 0, Math.Max(0, nameEnd - 56), size.Height), bold: true);
        if (details)
        {
            Text(context, OwnerHeader, new Rect(nameEnd + 8, 0, 68, size.Height), bold: true);
            Text(context, ProgressHeader, new Rect(pane - 68, 0, 60, size.Height), bold: true);
        }
        if (ViewEnd > ViewStart)
        {
            var days = VisibleDays(width);
            using (context.PushClip(new Rect(pane, 0, Math.Max(0, width - pane), size.Height)))
            {
                Grid(context, width, size.Height);
                var step = DayWidth >= 32 ? 1 : DayWidth >= 10 ? 7 : 28;
                for (var day = days.First; day < days.Last; day++)
                {
                    var date = ViewStart.AddDays(day); var x = TimelineX(date, width);
                    if (step == 1 || step == 7 && date.DayOfWeek == DayOfWeek.Monday || step == 28 && date.Day == 1)
                    {
                        var next = step == 28 ? Math.Min(ViewEnd.DayNumber, date.DayNumber + DateTime.DaysInMonth(date.Year, date.Month)) : Math.Min(ViewEnd.DayNumber, date.DayNumber + step);
                        Text(context, step == 1 ? date.ToString("dd", CultureInfo.CurrentCulture) : date.ToString("MM/dd", CultureInfo.CurrentCulture),
                            new Rect(x + 2, half, Math.Max(0, (next - date.DayNumber) * DayWidth - 4), half), center: true);
                    }
                    // Month bands follow civil calendar boundaries, including partial months.
                    if (day == days.First || date.Day == 1)
                    {
                        var monthStart = new DateOnly(date.Year, date.Month, 1);
                        var first = Math.Max(ViewStart.DayNumber, monthStart.DayNumber);
                        var end = Math.Min(ViewEnd.DayNumber, monthStart.DayNumber + DateTime.DaysInMonth(date.Year, date.Month));
                        var startX = pane + (first - ViewStart.DayNumber) * DayWidth - _offset;
                        context.DrawLine(pen, new Point(startX, 0), new Point(startX, half));
                        var labelLeft = Math.Max(pane, startX); var labelRight = Math.Min(width, startX + (end - first) * DayWidth);
                        Text(context, date.ToString("yyyy · MMMM", CultureInfo.CurrentCulture), new Rect(labelLeft + 4, 0, Math.Max(0, labelRight - labelLeft - 8), half), center: true);
                    }
                }
                context.DrawLine(pen, new Point(pane, half), new Point(width, half));
                TodayLine(context, width, size.Height);
                if (Today >= ViewStart && Today < ViewEnd && DayWidth >= 32)
                {
                    var x = TimelineX(Today, width);
                    context.DrawRectangle(ProgressBrush, null, new Rect(x, half + 2, DayWidth, Math.Max(0, half - 4)), 3, 3);
                    Text(context, Today.ToString("dd", CultureInfo.CurrentCulture), new Rect(x, half, DayWidth, half), OnTaskBrush, center: true);
                }
            }
        }
        context.DrawLine(pen, new Point(pane, 0), new Point(pane, size.Height));
        context.DrawLine(pen, new Point(0, size.Height - .5), new Point(size.Width, size.Height - .5));
    }
    internal void DrawRow(DrawingContext context, Row row, Size size)
    {
        var (pane, nameEnd, details) = Columns(size.Width); var height = size.Height;
        if (ReferenceEquals(SelectedTask, row.Task)) context.DrawRectangle(SelectionBrush, null, new Rect(size));
        var pen = new Pen(GridBrush, 1);
        using (context.PushClip(new Rect(0, 0, pane, height)))
        {
            Text(context, (row.Index + 1).ToString(CultureInfo.CurrentCulture), new Rect(0, 0, 36, height), MutedBrush, center: true);
            var indent = Math.Min(row.Depth * 16.0, Math.Max(0, nameEnd - 128));
            Text(context, row.Task.Title, new Rect(76 + indent, 0, Math.Max(0, nameEnd - 84 - indent), height), bold: row.Children.Count > 0);
            if (details)
            {
                Text(context, row.Task.Owner ?? "—", new Rect(nameEnd + 8, 0, 68, height), MutedBrush);
                Text(context, row.Task.IsMilestone ? "—" : row.Progress.ToString("P0", CultureInfo.CurrentCulture), new Rect(pane - 68, 0, 60, height), MutedBrush);
            }
            if (ReferenceEquals(SelectedTask, row.Task)) context.DrawRectangle(ProgressBrush, null, new Rect(0, 0, 3, height));
        }
        var axisEnd = Math.Min(size.Width, pane + Math.Max(0, ViewEnd.DayNumber - ViewStart.DayNumber) * DayWidth - _offset);
        using (context.PushClip(new Rect(pane, 0, Math.Max(0, axisEnd - pane), height)))
        {
            Grid(context, size.Width, height);
            var x = TimelineX(row.Start, size.Width); var end = TimelineX(row.End, size.Width);
            var h = Math.Min(26, height * .56); var y = (height - h) / 2;
            if (row.Children.Count > 0)
            {
                context.DrawLine(new Pen(ProgressBrush, 4), new Point(x, height / 2), new Point(end, height / 2));
                context.DrawLine(new Pen(ProgressBrush, 2), new Point(x, height / 2), new Point(x, height / 2 + 6));
                context.DrawLine(new Pen(ProgressBrush, 2), new Point(end, height / 2), new Point(end, height / 2 + 6));
            }
            else if (row.Task.IsMilestone && row.Start >= ViewStart && row.Start < ViewEnd)
            {
                x += DayWidth / 2; var r = Math.Min(9, height / 4);
                var diamond = new StreamGeometry();
                using (var g = diamond.Open()) { g.BeginFigure(new Point(x, height / 2 - r), true); g.LineTo(new Point(x + r, height / 2)); g.LineTo(new Point(x, height / 2 + r)); g.LineTo(new Point(x - r, height / 2)); g.EndFigure(true); }
                context.DrawGeometry(ProgressBrush, null, diamond);
                Text(context, row.Task.Title, new Rect(x + r + 8, 0, Math.Max(0, size.Width - x - r - 16), height));
            }
            else if (end > pane && x < size.Width)
            {
                var rect = new Rect(x + 2, y, Math.Max(1, end - x - 4), h);
                context.DrawRectangle(row.Progress >= 1 ? ProgressBrush : TaskBrush, new Pen(ProgressBrush, 1), rect, 4, 4);
                if (row.Progress > 0 && row.Progress < 1)
                    using (context.PushClip(new Rect(rect.X, rect.Y, rect.Width * row.Progress, rect.Height))) context.DrawRectangle(ProgressBrush, null, rect, 4, 4);
                // Text always uses the available bar region, rather than growing the date geometry.
                if (rect.Width >= 56)
                {
                    var label = row.Progress > 0 ? $"{row.Task.Title} · {row.Progress:P0}" : row.Task.Title;
                    var labelArea = new Rect(rect.X + 8, rect.Y, Math.Max(0, rect.Width - 16), rect.Height);
                    Text(context, label, labelArea, row.Progress >= 1 ? OnTaskBrush : Foreground);
                    if (row.Progress > 0 && row.Progress < 1)
                        using (context.PushClip(new Rect(rect.X, rect.Y, rect.Width * row.Progress, rect.Height))) Text(context, label, labelArea, OnTaskBrush);
                }
                if (ReferenceEquals(SelectedTask, row.Task)) context.DrawRectangle(null, new Pen(ProgressBrush, 2), rect.Inflate(2), 5, 5);
            }
            TodayLine(context, size.Width, height);
        }
        context.DrawLine(pen, new Point(pane, 0), new Point(pane, height));
        context.DrawLine(pen, new Point(0, height - .5), new Point(size.Width, height - .5));
    }

    internal sealed class GanttRowPresenter : Control
    {
        private readonly GanttChart _owner;
        private readonly Row _row;
        private readonly Button? _expand;
        public GanttRowPresenter(GanttChart owner, Row row)
        {
            _owner = owner; _row = row; ClipToBounds = true;
            ToolTip.SetTip(this, $"{row.Task.Title}\n{row.Start:yyyy/MM/dd} — {(row.Task.IsMilestone ? row.Start : row.End.AddDays(-1)):yyyy/MM/dd}\n{row.Progress:P0}");
            if (row.Children.Count > 0)
            {
                _expand = new Button { Width = 24, Height = 24, MinHeight = 0, Padding = default, BorderThickness = default, Focusable = false,
                    Background = Brushes.Transparent, Content = new PathIcon { Width = 10, Height = 10, Foreground = owner.Foreground,
                        Data = StreamGeometry.Parse(row.Task.IsExpanded ? "M0,2 L10,2 L5,8 Z" : "M2,0 L8,5 L2,10 Z") } };
                AutomationProperties.SetName(_expand, $"{(row.Task.IsExpanded ? "Collapse" : "Expand")} {row.Task.Title}");
                _expand.Click += (_, e) => { row.Task.IsExpanded = !row.Task.IsExpanded; e.Handled = true; };
                VisualChildren.Add(_expand); LogicalChildren.Add(_expand);
            }
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (this.FindAncestorOfType<ListBoxItem>() is { } item) AutomationProperties.SetName(item, $"{_row.Task.Title}, {_row.Task.Owner}, {_row.Start:yyyy/MM/dd}, {_row.Progress:P0}");
        }
        internal void RefreshTheme() { if (_expand?.Content is PathIcon icon) icon.Foreground = _owner.Foreground; }
        protected override Size MeasureOverride(Size availableSize)
        {
            _expand?.Measure(new Size(24, 24));
            return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : _owner.TaskListWidth + 400, _owner.RowHeight);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            var columns = _owner.Columns(finalSize.Width);
            var indent = Math.Min(_row.Depth * 16.0, Math.Max(0, columns.NameEnd - 128));
            _expand?.Arrange(new Rect(44 + indent, Math.Max(0, (finalSize.Height - 24) / 2), 24, 24));
            _owner.SetRowWidth(finalSize.Width);
            return finalSize;
        }
        public override void Render(DrawingContext context) { base.Render(context); _owner.DrawRow(context, _row, Bounds.Size); }
    }
}

/// <summary>Template part for GanttChart's frozen calendar and task header.</summary>
public sealed class GanttHeaderPresenter : Control
{
    internal GanttChart? Owner { get; set; }
    public override void Render(DrawingContext context) { base.Render(context); Owner?.DrawHeader(context, Bounds.Size); }
}
