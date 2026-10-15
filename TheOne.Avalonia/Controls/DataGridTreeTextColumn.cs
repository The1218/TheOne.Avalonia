using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Shapes;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Media;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Prototype tree text column. Editing uses the original native TextBox and binding.</summary>
public sealed class DataGridTreeTextColumn : DataGridTextColumn
{
    public static readonly StyledProperty<double> IndentProperty =
        AvaloniaProperty.Register<DataGridTreeTextColumn, double>(nameof(Indent), 16, validate: value => double.IsFinite(value) && value >= 0);
    public double Indent { get => GetValue(IndentProperty); set => SetValue(IndentProperty, value); }

    protected override Control GenerateElement(DataGridCell cell, object dataItem) => new DataGridTreeCellPresenter(base.GenerateElement(cell, dataItem), Indent);
    protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
    {
        var editor = base.GenerateEditingElementDirect(cell, dataItem);
        var tree = cell.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault()?.CollectionView as IDataGridTreeCollectionView;
        if (tree is not null && tree.TryGetNodeState(dataItem, out var state)) editor.Margin = new Thickness(state.Depth * Indent + 20, 0, 0, 0);
        return editor;
    }
    protected override void RefreshCellContent(Control element, string propertyName) =>
        base.RefreshCellContent(element is DataGridTreeCellPresenter presenter ? presenter.NativeContent : element, propertyName);
}

/// <summary>Only realized cells subscribe to tree presentation changes; model DataContext is inherited.</summary>
internal sealed class DataGridTreeCellPresenter : Grid
{
    private static readonly Geometry Collapsed = StreamGeometry.Parse("M2,0 L7,5 L2,10 L1,9 L5,5 L1,1 Z");
    private static readonly Geometry Expanded = StreamGeometry.Parse("M0,2 L5,7 L10,2 L9,1 L5,5 L1,1 Z");
    private readonly Button _expander;
    private readonly global::Avalonia.Controls.Shapes.Path _icon = new() { Width = 10, Height = 10, Stretch = Stretch.Uniform };
    private readonly double _indent;
    private IDataGridTreeCollectionView? _tree;
    private DataGrid? _grid;
    public Control NativeContent { get; }
    public DataGridTreeCellPresenter(Control content, double indent)
    {
        NativeContent = content; _indent = indent;
        ColumnDefinitions = new ColumnDefinitions("20,*");
        _expander = new Button { Name = "TreeExpander", Content = _icon, Width = 20, MinWidth = 0, MinHeight = 0,
            Height = 20, Padding = default, BorderThickness = default, Focusable = false, VerticalAlignment = VerticalAlignment.Center };
        _expander.Classes.Add("ghost");
        _icon.Bind(Shape.FillProperty, _expander.GetObservable(Button.ForegroundProperty));
        _expander.AddHandler(PointerPressedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        _expander.AddHandler(PointerReleasedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        _expander.Click += (_, args) =>
        {
            args.Handled = true;
            if (DataContext is { } item && _tree is not null && _tree.TryGetNodeState(item, out var state)) _tree.TrySetExpanded(item, !state.IsExpanded);
        };
        SetColumn(content, 1); Children.Add(_expander); Children.Add(content);
        DataContextChanged += (_, _) => Update();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _grid = this.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
        if (_grid is not null) _grid.PropertyChanged += GridChanged;
        Connect();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        if (_grid is not null) _grid.PropertyChanged -= GridChanged;
        if (_tree is not null) _tree.TreeChanged -= Changed;
        _tree = null; _grid = null;
        base.OnDetachedFromVisualTree(args);
    }
    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args) { if (args.Property == DataGrid.CollectionViewProperty) Connect(); }
    private void Connect()
    {
        if (_tree is not null) _tree.TreeChanged -= Changed;
        _tree = _grid?.CollectionView as IDataGridTreeCollectionView;
        if (_tree is not null) _tree.TreeChanged += Changed;
        Update();
    }
    private void Changed(object? sender, EventArgs args) => Update();
    private void Update()
    {
        var state = default(DataGridTreeNodeState);
        var valid = DataContext is { } item && _tree is not null && _tree.TryGetNodeState(item, out state);
        Margin = new Thickness(valid ? state.Depth * _indent : 0, 0, 0, 0);
        _expander.IsVisible = valid && state.HasChildren;
        _icon.Data = state.IsExpanded ? Expanded : Collapsed;
        AutomationProperties.SetName(_expander, state.IsExpanded ? "Collapse" : "Expand");
    }
}
