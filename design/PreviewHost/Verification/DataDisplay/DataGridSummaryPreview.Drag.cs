using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridSummaryPreview
{
    private static void VerifyDragFeedback(Window window, DataGrid grid, Action<bool, string> check)
    {
        var column = grid.Columns[7]; var neighbor = grid.Columns[6]; var index = column.DisplayIndex;
        DataGridColumnHeader Header(DataGridColumn c) => grid.GetVisualDescendants().OfType<DataGridColumnHeader>().Single(h => Equals(h.Content, c.Header) && !h.Classes.Contains(":dragIndicator"));
        DataGridColumnHeader? Ghost() => grid.GetVisualDescendants().OfType<DataGridColumnHeader>().SingleOrDefault(h => h.Classes.Contains(":dragIndicator"));
        var start = Header(column).TranslatePoint(new Point(30, 14), window)!.Value;
        void Begin()
        {
            window.MouseMove(start); window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start - new Vector(12, 0)); Flush();
        }
        Begin();
        var ghost = Ghost();
        check(ghost is { IsHitTestVisible: false } && ghost.Opacity > 0.9 && Header(column).Opacity < 0.5,
            "DataGrid/drag: card ghost stays readable and source dims without capturing input");
        check(ghost is not null && Math.Abs(ghost.Bounds.X - Header(column).Bounds.X + 12) < 1,
            "DataGrid/drag: first threshold movement retains original grab offset");
        check(ghost is not null && ghost.GetVisualDescendants().OfType<DataGridFilterButton>().All(b => !b.IsVisible) &&
            ghost.GetVisualDescendants().OfType<DataGridSortButton>().All(b => !b.IsVisible), "DataGrid/drag: ghost omits duplicate sort and filter actions");
        check(grid.GetVisualDescendants().OfType<DataGridDropIndicator>().Single() is { IsHitTestVisible: false }, "DataGrid/drag: capped insertion caret remains a native overlay");
        Motion.SetEnabled(window, true);
        var end = Header(neighbor).TranslatePoint(new Point(15, 14), window)!.Value;
        window.MouseMove(end); Flush();
        check(grid.GetVisualDescendants().OfType<DataGridDropIndicator>().Single().RenderTransform is TranslateTransform,
            "DataGrid/drag: moving target uses cancellable caret motion");
        Motion.SetEnabled(window, false); Flush();
        window.MouseUp(end, MouseButton.Left); Flush();
        check(column.DisplayIndex == index - 1 && column.IsFrozen && neighbor.IsFrozen && Ghost() is null && Header(column).Opacity == 1,
            "DataGrid/drag: drop commits within right frozen region and removes feedback");
        column.DisplayIndex = index; Flush();
        grid.Focus(); Begin(); window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null); window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null); Flush();
        window.MouseUp(start, MouseButton.Left); Flush();
        check(column.DisplayIndex == index && Ghost() is null && !Header(column).Classes.Contains(":dragging"), "DataGrid/drag: Escape cancels without reordering or leaving a stale overlay");
        Begin(); window.MouseMove(new Point(-20, -20)); window.MouseUp(new Point(-20, -20), MouseButton.Left); Flush();
        check(column.DisplayIndex == index && Ghost() is null, "DataGrid/drag: releasing outside the header cancels and releases capture");
        Begin(); var edge = new Point(window.Width + 30, start.Y); window.MouseMove(edge); Flush();
        var caret = grid.GetVisualDescendants().OfType<DataGridDropIndicator>().Single();
        check(caret.Bounds.Right <= Header(column).Bounds.Right + 0.5 && caret.Clip is RectangleGeometry { Rect.Width: > 0 },
            "DataGrid/drag: final insertion boundary keeps a visible caret inside its frozen region");
        window.MouseUp(new Point(-20, -20), MouseButton.Left); Flush();
        Begin(); window.Content = null; Flush(); window.Content = grid; Flush();
        check(column.DisplayIndex == index && Ghost() is null && Header(column).Opacity == 1,
            "DataGrid/drag: detaching during drag cleans native capture and feedback");
    }
}
