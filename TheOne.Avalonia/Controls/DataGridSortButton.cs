using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using Path = Avalonia.Controls.Shapes.Path;

namespace TheOne.Avalonia.Controls;

/// <summary>Keyboard-accessible sort entry using the native column's edit and sorting pipeline.</summary>
[PseudoClasses(":ascending", ":descending", ":unsorted")]
public sealed class DataGridSortButton : Button
{
    private static readonly Geometry Unsorted =
        StreamGeometry.Parse("M1,4 L5,0 L9,4 L8,5 L5,2 L2,5 Z M1,8 L5,12 L9,8 L8,7 L5,10 L2,7 Z");

    private static readonly Geometry Ascending = StreamGeometry.Parse("M0,7 L5,2 L10,7 L9,8 L5,4 L1,8 Z");
    private static readonly Geometry Descending = StreamGeometry.Parse("M0,3 L5,8 L10,3 L9,2 L5,6 L1,2 Z");

    public static readonly DirectProperty<DataGridSortButton, ListSortDirection?> DirectionProperty =
        AvaloniaProperty.RegisterDirect<DataGridSortButton, ListSortDirection?>(nameof(Direction),
            button => button.Direction);

    public static readonly DirectProperty<DataGridSortButton, int> SortPriorityProperty =
        AvaloniaProperty.RegisterDirect<DataGridSortButton, int>(nameof(SortPriority), button => button.SortPriority);

    private readonly Path _icon = new() { Name = "SortIcon", Stretch = Stretch.Uniform };

    private readonly TextBlock _priority = new()
    {
        Name = "SortPriority", VerticalAlignment = VerticalAlignment.Center, FontWeight = FontWeight.SemiBold,
        IsVisible = false
    };

    private DataGridColumn? _column;
    private IDisposable? _foreground;
    private string? _foregroundKey;
    private DataGrid? _grid;
    private DataGridColumnHeader? _header;
    private KeyModifiers _modifiers;
    private DataGridSortDescriptionCollection? _sorts;

    public DataGridSortButton()
    {
        Classes.Add("ghost");
        MinHeight = MinWidth = 0;
        Padding = BorderThickness = default;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 2, Children = { _icon, _priority } };
        IsVisible = false;
        _icon.Bind(Shape.FillProperty, this.GetObservable(ForegroundProperty));
        _priority.Bind(TextBlock.ForegroundProperty, this.GetObservable(ForegroundProperty));
        AddHandler(PointerPressedEvent, (_, args) =>
        {
            _modifiers = args.KeyModifiers;
            args.Handled = true;
        }, RoutingStrategies.Bubble, true);
        AddHandler(PointerReleasedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        AddHandler(KeyUpEvent, (_, args) =>
        {
            if (args.Key is Key.Enter or Key.Space) args.Handled = true;
        }, RoutingStrategies.Bubble, true);
        UpdateSize();
    }

    public ListSortDirection? Direction
    {
        get;
        private set => SetAndRaise(DirectionProperty, ref field, value);
    }

    public int SortPriority
    {
        get;
        private set => SetAndRaise(SortPriorityProperty, ref field, value);
    }

    protected override Type StyleKeyOverride => typeof(Button);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _grid = this.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
        _header = this.GetVisualAncestors().OfType<DataGridColumnHeader>().FirstOrDefault();
        _column = DataGridColumn.GetColumnContainingElement(this);
        if (_grid is not null) _grid.PropertyChanged += OwnerChanged;
        if (_column is not null)
        {
            _column.PropertyChanged += OwnerChanged;
            _column.CanUserSortChanged += SortAvailabilityChanged;
            if (_column is DataGridBoundColumn bound) bound.ModelBindingChanged += SortAvailabilityChanged;
        }

        if (_header is not null) _header.Classes.CollectionChanged += HeaderChanged;
        Update();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        if (_grid is not null) _grid.PropertyChanged -= OwnerChanged;
        if (_column is not null)
        {
            _column.PropertyChanged -= OwnerChanged;
            _column.CanUserSortChanged -= SortAvailabilityChanged;
            if (_column is DataGridBoundColumn bound) bound.ModelBindingChanged -= SortAvailabilityChanged;
        }

        if (_header is not null) _header.Classes.CollectionChanged -= HeaderChanged;
        if (_sorts is not null) _sorts.CollectionChanged -= HeaderChanged;
        _sorts = null;
        _foreground?.Dispose();
        _foreground = null;
        _foregroundKey = null;
        _grid = null;
        _column = null;
        _header = null;
        base.OnDetachedFromVisualTree(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ControlSizing.SizeProperty) UpdateSize();
    }

    protected override void OnKeyDown(KeyEventArgs args)
    {
        _modifiers = args.KeyModifiers;
        base.OnKeyDown(args);
    }

    protected override void OnClick()
    {
        if (IsEffectivelyEnabled) _column?.CycleSort(_modifiers);
        _modifiers = KeyModifiers.None;
        base.OnClick();
    }

    private void UpdateSize()
    {
        var size = ControlSizing.GetSize(this);
        Height = size switch { ControlSize.Small => 20, ControlSize.Large => 28, _ => 24 };
        MinWidth = Height + (_priority.IsVisible
            ? size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 }
            : 0);
        Width = _priority.IsVisible ? double.NaN : Height;
        _priority.FontSize = size switch { ControlSize.Small => 10, ControlSize.Large => 12, _ => 11 };
        _icon.Width = _icon.Height = size switch { ControlSize.Small => 10, ControlSize.Large => 14, _ => 12 };
    }

    private void SortAvailabilityChanged(object? sender, EventArgs args)
    {
        Update();
    }

    private void OwnerChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        Update();
    }

    private void HeaderChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        Update();
    }

    private void Update()
    {
        IsVisible = _header?.Classes.Contains(":dragIndicator") != true && _grid is not null && _column is not null &&
                    _grid.Columns.Contains(_column);
        IsEnabled = IsVisible && _grid!.CanUserSortColumns && _column!.CanUserSort;
        var sorts = _grid?.CollectionView?.SortDescriptions;
        if (!ReferenceEquals(sorts, _sorts))
        {
            if (_sorts is not null) _sorts.CollectionChanged -= HeaderChanged;
            _sorts = sorts;
            if (_sorts is not null) _sorts.CollectionChanged += HeaderChanged;
        }

        Direction = _column?.SortDirection;
        SortPriority = _column?.SortPriority ?? 0;
        _priority.Text = SortPriority.ToString(CultureInfo.InvariantCulture);
        _priority.IsVisible = SortPriority > 0 && _sorts?.Count > 1;
        UpdateSize();
        _icon.Data = Direction switch
        {
            ListSortDirection.Ascending => Ascending, ListSortDirection.Descending => Descending, _ => Unsorted
        };
        PseudoClasses.Set(":ascending", Direction == ListSortDirection.Ascending);
        PseudoClasses.Set(":descending", Direction == ListSortDirection.Descending);
        PseudoClasses.Set(":unsorted", Direction is null);
        var key = !IsEnabled ? "One.DisabledText" : Direction is null ? "One.Muted" : "One.Accent";
        if (_foregroundKey != key)
        {
            _foregroundKey = key;
            _foreground?.Dispose();
            _foreground = Bind(ForegroundProperty, new DynamicResourceExtension(key));
        }

        var state = Direction switch
        {
            ListSortDirection.Ascending => "升序", ListSortDirection.Descending => "降序", _ => "未排序"
        };
        var priority = _priority.IsVisible ? $" · 优先级 {SortPriority}" : "";
        AutomationProperties.SetName(this, $"排序 {_column?.Header} · {state}{priority}");
        ToolTip.SetTip(this, $"{state}{priority} · 未排序 → 升序 → 降序 → 未排序；Shift 多列排序，Ctrl／⌘ 清除排序");
    }
}