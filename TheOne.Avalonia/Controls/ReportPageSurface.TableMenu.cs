using Avalonia;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportPageSurface
{
    internal bool TryTableColumn(Point local, out ReportElement? table, out int index)
    {
        table = null;
        index = -1;
        if (_preview || _document is null || _document.Columns.Count == 0 || PagePoint(local) is not { } mm) return false;
        if (_document.Elements.Count > 0)
        {
            table = ElementAt(mm);
            if (table?.Kind != ReportElementKind.Table) return false;
            index = ColumnAt(table, mm);
        }
        else if (_layout is { } layout)
        {
            var y = (local.Y - TopGutter) / _zoom;
            if (y < layout.TableTop || y > layout.DetailBottom) return false;
            var x = (local.X - LeftGutter) / _zoom - layout.TableX;
            for (var i = 0; i < layout.Columns.Count; i++)
            {
                if (x >= 0 && x < layout.Columns[i].Width) { index = i; break; }
                x -= layout.Columns[i].Width;
            }
        }
        return index >= 0 && index < _document.Columns.Count;
    }

    internal Rect TableColumnRect(ReportElement? table, int index)
    {
        if (_document is null || index < 0 || index >= _document.Columns.Count) return default;
        if (table is not null)
        {
            var weight = _document.Columns.Sum(column => column.WidthWeight);
            var x = table.X + table.Width * _document.Columns.Take(index).Sum(column => column.WidthWeight) / weight;
            return PageRect(new Rect(x, table.Y, table.Width * _document.Columns[index].WidthWeight / weight,
                table.TableHeaderHeightMm));
        }
        return _layout is { } layout && index < layout.Columns.Count
            ? new Rect(LeftGutter + (layout.TableX + layout.Columns.Take(index).Sum(column => column.Width)) * _zoom,
                TopGutter + layout.TableTop * _zoom, layout.Columns[index].Width * _zoom, layout.HeaderHeight * _zoom)
            : default;
    }
}
