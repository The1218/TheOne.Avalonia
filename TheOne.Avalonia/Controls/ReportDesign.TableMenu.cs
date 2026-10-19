using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    private readonly ConditionalWeakTable<ReportDocument, HistorySelection> _historySelections = new();

    private sealed record HistorySelection(int Column, int Element);

    private void RememberHistorySelection(ReportDocument snapshot)
    {
        _historySelections.Remove(snapshot);
        _historySelections.Add(snapshot, new HistorySelection(
            Document?.Columns.IndexOf(SelectedColumn!) ?? -1,
            Document?.Elements.IndexOf(SelectedElement!) ?? -1));
    }

    private static bool CanInsertColumn(ReportDocument document, double weight)
    {
        if (document.Columns.Count >= 100 || !double.IsFinite(weight) || weight <= 0) return false;
        var width = (document.Elements.FirstOrDefault(item => item.Kind == ReportElementKind.Table)?.Width ??
            document.PageWidthMm - document.LeftMargin - document.RightMargin) * ReportPaginator.DipsPerMillimetre;
        var total = document.Columns.Sum(item => item.WidthWeight) + weight;
        return double.IsFinite(width) && double.IsFinite(total) && total > 0 && width * weight / total >= 32 &&
            document.Columns.All(item => double.IsFinite(item.WidthWeight) && item.WidthWeight > 0 &&
                width * item.WidthWeight / total >= 32);
    }

    private void RemoveColumn(ReportColumn column)
    {
        if (IsPreview || Document is not { } document) return;
        if (document.Columns.Count <= 1)
        {
            State.Status = "表格至少保留一列。";
            return;
        }
        var position = document.Columns.IndexOf(column);
        if (position < 0) return;
        var adjacent = document.Columns[position == document.Columns.Count - 1 ? position - 1 : position + 1];
        ChangeColumns(() => document.Columns.Remove(column), adjacent);
    }

    private void OnTableContextRequested(object? sender, ContextRequestedEventArgs e)
    {
        CloseTableContextMenu();
        if (IsPreview || Document is not { } document || document.Columns.Count == 0) return;
        ReportElement? table;
        int index;
        Rect anchor;
        if (e.TryGetPosition(PageSurface, out var local))
        {
            if (!PageSurface.TryTableColumn(local, out table, out index)) return;
            anchor = new Rect(local, new Size(1, 1));
        }
        else
        {
            table = SelectedElement?.Kind == ReportElementKind.Table ? SelectedElement : null;
            if (table is null && document.Elements.Count > 0) return;
            if (table is not null && !document.Elements.Contains(table)) return;
            index = document.Columns.IndexOf(SelectedColumn!);
            if (index < 0) return;
            anchor = PageSurface.TableColumnRect(table, index);
        }
        if (!double.IsFinite(anchor.X) || !double.IsFinite(anchor.Y) ||
            !double.IsFinite(anchor.Width) || !double.IsFinite(anchor.Height)) return;
        var column = document.Columns[index];
        if (table is not null) SetCurrentValue(SelectedElementProperty, table);
        SetCurrentValue(SelectedColumnProperty, column);
        PageSurface.Focus();
        var left = new MenuItem { Name = "InsertTableColumnLeft", Header = "左侧插入列", Icon = new ReportIcon { Kind = "Plus" } };
        var right = new MenuItem { Name = "InsertTableColumnRight", Header = "右侧插入列", Icon = new ReportIcon { Kind = "Plus" } };
        var delete = new MenuItem { Name = "DeleteTableColumn", Header = "删除列", Icon = new ReportIcon { Kind = "Close" } };
        bool Current() => !IsPreview && ReferenceEquals(document, Document) && document.Columns.Contains(column) &&
            (table is null ? document.Elements.Count == 0 : document.Elements.Contains(table) && table.Kind == ReportElementKind.Table);
        bool CanInsert()
        {
            return Current() && CanInsertColumn(document, column.WidthWeight);
        }
        void Insert(bool after)
        {
            if (!CanInsert()) return;
            var added = new ReportColumn { Header = "新列", WidthWeight = column.WidthWeight };
            ChangeColumns(() => document.Columns.Insert(document.Columns.IndexOf(column) + (after ? 1 : 0), added), added);
        }
        void Remove()
        {
            if (Current()) RemoveColumn(column);
        }
        left.IsEnabled = right.IsEnabled = CanInsert();
        delete.IsEnabled = Current() && document.Columns.Count > 1;
        ToolTip.SetTip(left, "插入空白列；最多 100 列，每列至少保留 32 DIP。宽度不足时请先调整表格或列宽。");
        ToolTip.SetTip(right, ToolTip.GetTip(left));
        ToolTip.SetTip(delete, "删除当前列；表格至少保留一列。支持撤销。");
        left.Click += (_, _) => Insert(false);
        right.Click += (_, _) => Insert(true);
        delete.Click += (_, _) => Remove();
        var menu = new ContextMenu
        {
            ItemsSource = new object[] { left, right, new Separator(), delete },
            Placement = PlacementMode.AnchorAndGravity, PlacementRect = anchor,
            PlacementAnchor = PopupAnchor.BottomLeft, PlacementGravity = PopupGravity.BottomRight
        };
        PageSurface.ContextMenu = menu;
        e.Handled = true;
        menu.Open(PageSurface);
    }

    private void CloseTableContextMenu()
    {
        PageSurface.ContextMenu?.Close();
        PageSurface.ContextMenu = null;
    }

    private void ChangeColumns(Action change, ReportColumn selected)
    {
        // Capture selection against the old definition before collection indexes change.
        if (_lastSnapshot is { } snapshot) RememberHistorySelection(snapshot);
        var previousBatch = _batch;
        _batch = true;
        try
        {
            change();
            SetCurrentValue(SelectedColumnProperty, selected);
        }
        finally { _batch = previousBatch; }
        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(ReportDocument.Columns)));
        PageSurface.Focus();
    }
}
