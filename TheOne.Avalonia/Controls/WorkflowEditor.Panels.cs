using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

public partial class WorkflowEditor
{
    private Grid? _panelGrid;
    private double _libraryWidth = 195, _propertiesWidth = 296;
    private bool _updatingPanelLayout;

    private void AttachPanelLayout(Grid? grid)
    {
        if (_panelGrid is { ColumnDefinitions.Count: 5 } old)
        {
            old.SizeChanged -= PanelSizeChanged;
            old.ColumnDefinitions[0].PropertyChanged -= PanelWidthChanged;
            old.ColumnDefinitions[4].PropertyChanged -= PanelWidthChanged;
        }
        _panelGrid = grid;
        if (grid is not { ColumnDefinitions.Count: 5 }) return;
        grid.SizeChanged += PanelSizeChanged;
        grid.ColumnDefinitions[0].PropertyChanged += PanelWidthChanged;
        grid.ColumnDefinitions[4].PropertyChanged += PanelWidthChanged;
        UpdatePanelLayout();
    }

    private void PanelSizeChanged(object? sender, SizeChangedEventArgs e) => UpdatePanelLayout();

    private void PanelWidthChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_updatingPanelLayout || e.Property != ColumnDefinition.WidthProperty || _panelGrid is null) return;
        if (sender == _panelGrid.ColumnDefinitions[0] && ShowNodeLibrary)
            _libraryWidth = _panelGrid.ColumnDefinitions[0].Width.Value;
        if (sender == _panelGrid.ColumnDefinitions[4] && ShowProperties)
            _propertiesWidth = _panelGrid.ColumnDefinitions[4].Width.Value;
    }

    private void UpdatePanelLayout()
    {
        if (_panelGrid is not { ColumnDefinitions.Count: 5 } grid || _updatingPanelLayout) return;
        _updatingPanelLayout = true;
        try
        {
            var columns = grid.ColumnDefinitions;
            var gaps = (ShowNodeLibrary ? 5 : 0) + (ShowProperties ? 5 : 0);
            var available = Math.Max(0, grid.Bounds.Width - gaps);
            var canvasMin = Math.Min(160, available);
            var sideBudget = Math.Max(0, available - canvasMin);
            var preferredLeft = ShowNodeLibrary ? _libraryWidth : 0;
            var preferredRight = ShowProperties ? _propertiesWidth : 0;
            var preferredTotal = preferredLeft + preferredRight;
            var scale = preferredTotal > sideBudget && preferredTotal > 0 ? sideBudget / preferredTotal : 1;
            var left = preferredLeft * scale;
            var right = preferredRight * scale;
            columns[2].MinWidth = canvasMin;
            SetSide(columns[0], left, ShowNodeLibrary ? Math.Min(140, left) : 0, 420);
            SetSide(columns[4], right, ShowProperties ? Math.Min(280, right) : 0, 560);
            columns[1].Width = new GridLength(ShowNodeLibrary ? 5 : 0);
            columns[3].Width = new GridLength(ShowProperties ? 5 : 0);
        }
        finally { _updatingPanelLayout = false; }

        static void SetSide(ColumnDefinition column, double width, double minimum, double maximum)
        {
            column.MinWidth = minimum;
            column.MaxWidth = maximum;
            column.Width = new GridLength(width);
        }
    }
}
