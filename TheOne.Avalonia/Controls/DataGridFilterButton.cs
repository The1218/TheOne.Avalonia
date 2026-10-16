using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Shapes;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions;
using global::Avalonia.Media;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Native-header filter entry. The owning column is obtained through DataGrid's public API.</summary>
public sealed class DataGridFilterButton : Button
{
    private readonly global::Avalonia.Controls.Shapes.Path _icon = new() { Data = StreamGeometry.Parse("M0,0 H12 L7,6 V11 L5,10 V6 Z"), Stretch = Stretch.Uniform };
    private DataGrid? _grid;
    private DataGridColumn? _column;
    private DataGridColumnHeader? _header;
    private IDisposable? _foreground;
    private string? _foregroundKey;
    private Flyout? _flyout;
    private DataGridFilterEditor? _editor;
    protected override Type StyleKeyOverride => typeof(Button);
    public DataGridFilterButton()
    {
        Classes.Add("ghost"); MinHeight = MinWidth = 0; Padding = default; BorderThickness = default;
        HorizontalAlignment = HorizontalAlignment.Center; VerticalAlignment = VerticalAlignment.Center;
        IsVisible = false; Content = _icon;
        _icon.Bind(Shape.FillProperty, this.GetObservable(ForegroundProperty));
        AddHandler(PointerPressedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        AddHandler(PointerReleasedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        AddHandler(KeyUpEvent, (_, args) => { if (args.Key is Key.Enter or Key.Space) args.Handled = true; }, RoutingStrategies.Bubble, true);
        UpdateSize();
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _grid = this.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
        _header = this.GetVisualAncestors().OfType<DataGridColumnHeader>().FirstOrDefault();
        _column = DataGridColumn.GetColumnContainingElement(this);
        if (_grid is not null) _grid.PropertyChanged += GridChanged;
        if (_column is not null)
        {
            _column.PropertyChanged += ColumnChanged;
            if (_column is DataGridBoundColumn bound) bound.ModelBindingChanged += ModelBindingChanged;
            if (_grid is not null && _grid.Columns.Contains(_column)) DataGridColumnOptions.ConnectOwner(_column, _grid);
        }
        Update();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        _flyout?.Hide();
        if (_grid is not null) _grid.PropertyChanged -= GridChanged;
        if (_column is not null)
        {
            _column.PropertyChanged -= ColumnChanged;
            if (_column is DataGridBoundColumn bound) bound.ModelBindingChanged -= ModelBindingChanged;
        }
        _header?.Classes.Set("one-filter-header", false);
        _grid = null; _column = null; _header = null;
        base.OnDetachedFromVisualTree(args);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ControlSizing.SizeProperty) UpdateSize();
    }
    private void UpdateSize()
    {
        var size = ControlSizing.GetSize(this);
        Width = Height = size switch { ControlSize.Small => 20, ControlSize.Large => 28, _ => 24 };
        if (_icon is not null) _icon.Width = _icon.Height = size switch { ControlSize.Small => 10, ControlSize.Large => 14, _ => 12 };
        _editor?.UpdateSize();
    }
    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGridFeatures.ShowColumnFiltersProperty || args.Property == DataGrid.CollectionViewProperty) Update();
        if (args.Property == DataGrid.ItemsSourceProperty) _flyout?.Hide();
    }
    private void ModelBindingChanged(object? sender, EventArgs args) { _flyout?.Hide(); Update(); }
    private void ColumnChanged(object? sender, AvaloniaPropertyChangedEventArgs args) => Update();
    private void Update()
    {
        IsVisible = _header?.Classes.Contains(":dragIndicator") != true && _grid is not null && _column is not null && _grid.Columns.Contains(_column) && DataGridFeatures.GetShowColumnFilters(_grid) && DataGridColumnOptions.EffectiveKind(_column) != DataGridFilterKind.None;
        _header?.Classes.Set("one-filter-header", IsVisible);
        IsEnabled = IsVisible && DataGridColumnOptions.EffectiveKind(_column!) != DataGridFilterKind.None && _grid!.CollectionView is DataGridCollectionView { CanFilter: true };
        var active = _column is not null && DataGridColumnOptions.GetFilterCondition(_column) is { IsEmpty: false };
        var foregroundKey = !IsEnabled ? "One.DisabledText" : active ? "One.Accent" : "One.Muted";
        if (_foregroundKey != foregroundKey)
        {
            _foregroundKey = foregroundKey; _foreground?.Dispose();
            _foreground = this.Bind(ForegroundProperty, new DynamicResourceExtension(foregroundKey));
        }
        AutomationProperties.SetName(this, _column?.Header is string title ? "筛选 " + title : "筛选当前列");
        ToolTip.SetTip(this, _column is not null && DataGridColumnOptions.GetFilterError(_column) is { } error ? error :
            !IsEnabled ? "需要可读取的文本／数值实体属性，以及支持筛选的数据视图。" : active ? "已筛选 · 点击修改或清除条件" : "筛选此列");
        if (!IsVisible || !IsEnabled) _flyout?.Hide();
    }
    protected override void OnClick()
    {
        if (_grid is null || _column is null || !IsEnabled) return;
        if (_flyout?.IsOpen == true) { base.OnClick(); return; }
        _editor = new DataGridFilterEditor(this, _grid, _column, () => _flyout?.Hide());
        _flyout = new Flyout { Content = _editor.Content, Placement = PlacementMode.BottomEdgeAlignedRight };
        _flyout.Closed += (_, _) =>
        {
            _editor = null;
            if (_flyout is not null) _flyout.Content = null;
            Dispatcher.UIThread.Post(() => { if (this.IsAttachedToVisualTree() && IsEffectivelyVisible && IsEffectivelyEnabled) Focus(); }, DispatcherPriority.Background);
        };
        Flyout = _flyout; base.OnClick(); _editor?.FocusInput();
    }
}
