// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Avalonia.Automation.Peers;
using Avalonia.Collections;
using Avalonia.Controls.Automation.Peers;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Utilities;
using Avalonia.VisualTree;

namespace Avalonia.Controls;

/// <summary>
///     Displays data in a customizable grid.
/// </summary>
[TemplatePart(DATAGRID_elementBottomRightCornerHeaderName, typeof(Visual))]
[TemplatePart(DATAGRID_elementColumnHeadersPresenterName, typeof(DataGridColumnHeadersPresenter))]
[TemplatePart(DATAGRID_elementFrozenColumnScrollBarSpacerName, typeof(Control))]
[TemplatePart(DATAGRID_elementHorizontalScrollbarName, typeof(ScrollBar))]
[TemplatePart(DATAGRID_elementRowsPresenterName, typeof(DataGridRowsPresenter))]
[TemplatePart(DATAGRID_elementTopLeftCornerHeaderName, typeof(ContentControl))]
[TemplatePart(DATAGRID_elementTopRightCornerHeaderName, typeof(ContentControl))]
[TemplatePart(DATAGRID_elementVerticalScrollbarName, typeof(ScrollBar))]
[PseudoClasses(":invalid", ":empty-rows", ":empty-columns")]
#if !DATAGRID_INTERNAL
public
#endif
    partial class DataGrid : TemplatedControl
{
    private const string DATAGRID_elementRowsPresenterName = "PART_RowsPresenter";
    private const string DATAGRID_elementColumnHeadersPresenterName = "PART_ColumnHeadersPresenter";
    private const string DATAGRID_elementFrozenColumnScrollBarSpacerName = "PART_FrozenColumnScrollBarSpacer";
    private const string DATAGRID_elementHorizontalScrollbarName = "PART_HorizontalScrollbar";
    private const string DATAGRID_elementTopLeftCornerHeaderName = "PART_TopLeftCornerHeader";
    private const string DATAGRID_elementTopRightCornerHeaderName = "PART_TopRightCornerHeader";
    private const string DATAGRID_elementBottomRightCornerHeaderName = "PART_BottomRightCorner";
    private const string DATAGRID_elementVerticalScrollbarName = "PART_VerticalScrollbar";
    internal const bool DATAGRID_defaultCanUserReorderColumns = true;
    internal const bool DATAGRID_defaultCanUserResizeColumns = true;
    internal const bool DATAGRID_defaultCanUserSortColumns = true;

    /// <summary>
    ///     The default order to use for columns when there is no <see cref="DisplayAttribute.Order" />
    ///     value available for the property.
    /// </summary>
    /// <remarks>
    ///     The value of 10,000 comes from the DataAnnotations spec, allowing
    ///     some properties to be ordered at the beginning and some at the end.
    /// </remarks>
    private const int DATAGRID_defaultColumnDisplayOrder = 10000;

    private const double DATAGRID_horizontalGridLinesThickness = 1;
    private const double DATAGRID_minimumRowHeaderWidth = 4;
    private const double DATAGRID_minimumColumnHeaderHeight = 4;
    internal const double DATAGRID_maximumStarColumnWidth = 10000;
    internal const double DATAGRID_minimumStarColumnWidth = 0.001;
    private const double DATAGRID_mouseWheelDelta = 50.0;
    private const double DATAGRID_maxHeadersThickness = 32768;

    private const double DATAGRID_defaultRowHeight = 22;
    internal const double DATAGRID_defaultRowGroupSublevelIndent = 20;
    private const double DATAGRID_defaultMinColumnWidth = 20;
    private const double DATAGRID_defaultMaxColumnWidth = double.PositiveInfinity;

    /// <summary>
    ///     Identifies the CanUserReorderColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserReorderColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserReorderColumns));

    /// <summary>
    ///     Identifies the CanUserResizeColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserResizeColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserResizeColumns));

    /// <summary>
    ///     Identifies the CanUserSortColumns dependency property.
    /// </summary>
    public static readonly StyledProperty<bool> CanUserSortColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(CanUserSortColumns), true);

    /// <summary>
    ///     Identifies the ColumnHeaderHeight dependency property.
    /// </summary>
    public static readonly StyledProperty<double> ColumnHeaderHeightProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(ColumnHeaderHeight),
            double.NaN,
            validate: IsValidColumnHeaderHeight);

    /// <summary>
    ///     Identifies the ColumnWidth dependency property.
    /// </summary>
    public static readonly StyledProperty<DataGridLength> ColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, DataGridLength>(nameof(ColumnWidth), DataGridLength.Auto);

    /// <summary>
    ///     Identifies the <see cref="RowTheme" /> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> RowThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(RowTheme));

    /// <summary>
    ///     Identifies the <see cref="CellTheme" /> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> CellThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(CellTheme));

    /// <summary>
    ///     Identifies the <see cref="ColumnHeaderTheme" /> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> ColumnHeaderThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(ColumnHeaderTheme));

    /// <summary>
    ///     Identifies the <see cref="RowGroupTheme" /> dependency property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme> RowGroupThemeProperty =
        AvaloniaProperty.Register<DataGrid, ControlTheme>(nameof(RowGroupTheme));

    public static readonly StyledProperty<int> FrozenColumnCountProperty =
        AvaloniaProperty.Register<DataGrid, int>(
            nameof(FrozenColumnCount),
            validate: ValidateFrozenColumnCount,
            coerce: (owner, value) => ((DataGrid)owner).ValidateLeftFrozenCount(value));

    public static readonly StyledProperty<DataGridGridLinesVisibility> GridLinesVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, DataGridGridLinesVisibility>(nameof(GridLinesVisibility));

    public static readonly StyledProperty<DataGridHeadersVisibility> HeadersVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, DataGridHeadersVisibility>(nameof(HeadersVisibility));

    public static readonly StyledProperty<IBrush> HorizontalGridLinesBrushProperty =
        AvaloniaProperty.Register<DataGrid, IBrush>(nameof(HorizontalGridLinesBrush));

    public static readonly StyledProperty<ScrollBarVisibility> HorizontalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>(nameof(HorizontalScrollBarVisibility));

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(IsReadOnly));

    public static readonly StyledProperty<bool> AreRowGroupHeadersFrozenProperty =
        AvaloniaProperty.Register<DataGrid, bool>(
            nameof(AreRowGroupHeadersFrozen),
            true);

    /// <summary>
    ///     Defines the <see cref="IsScrollInertiaEnabled" /> property.
    /// </summary>
    public static readonly AttachedProperty<bool> IsScrollInertiaEnabledProperty =
        ScrollViewer.IsScrollInertiaEnabledProperty.AddOwner<DataGrid>();

    public static readonly DirectProperty<DataGrid, bool> IsValidProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, bool>(
            nameof(IsValid),
            o => o.IsValid);

    public static readonly StyledProperty<double> MaxColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(MaxColumnWidth),
            DATAGRID_defaultMaxColumnWidth,
            validate: IsValidColumnWidth);

    public static readonly StyledProperty<double> MinColumnWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(MinColumnWidth),
            DATAGRID_defaultMinColumnWidth,
            validate: IsValidMinColumnWidth);

    public static readonly StyledProperty<IBrush> RowBackgroundProperty =
        AvaloniaProperty.Register<DataGrid, IBrush>(nameof(RowBackground));

    public static readonly StyledProperty<double> RowHeightProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(RowHeight),
            double.NaN,
            validate: IsValidRowHeight);

    public static readonly StyledProperty<double> RowHeaderWidthProperty =
        AvaloniaProperty.Register<DataGrid, double>(
            nameof(RowHeaderWidth),
            double.NaN,
            validate: IsValidRowHeaderWidth);

    public static readonly StyledProperty<DataGridSelectionMode> SelectionModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridSelectionMode>(nameof(SelectionMode));

    public static readonly StyledProperty<IBrush> VerticalGridLinesBrushProperty =
        AvaloniaProperty.Register<DataGrid, IBrush>(nameof(VerticalGridLinesBrush));

    public static readonly StyledProperty<ScrollBarVisibility> VerticalScrollBarVisibilityProperty =
        AvaloniaProperty.Register<DataGrid, ScrollBarVisibility>(nameof(VerticalScrollBarVisibility));

    public static readonly StyledProperty<ITemplate<Control>> DropLocationIndicatorTemplateProperty =
        AvaloniaProperty.Register<DataGrid, ITemplate<Control>>(nameof(DropLocationIndicatorTemplate));

    public static readonly DirectProperty<DataGrid, int> SelectedIndexProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, int>(
            nameof(SelectedIndex),
            o => o.SelectedIndex,
            (o, v) => o.SelectedIndex = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<DataGrid, object> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, object>(
            nameof(SelectedItem),
            o => o.SelectedItem,
            (o, v) => o.SelectedItem = v,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<DataGridClipboardCopyMode> ClipboardCopyModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridClipboardCopyMode>(
            nameof(ClipboardCopyMode),
            DataGridClipboardCopyMode.ExcludeHeader);

    public static readonly StyledProperty<bool> AutoGenerateColumnsProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(AutoGenerateColumns));

    /// <summary>
    ///     Identifies the ItemsSource property.
    /// </summary>
    public static readonly StyledProperty<IEnumerable> ItemsSourceProperty =
        AvaloniaProperty.Register<DataGrid, IEnumerable>(nameof(ItemsSource));

    public static readonly StyledProperty<bool> AreRowDetailsFrozenProperty =
        AvaloniaProperty.Register<DataGrid, bool>(nameof(AreRowDetailsFrozen));

    public static readonly StyledProperty<IDataTemplate> RowDetailsTemplateProperty =
        AvaloniaProperty.Register<DataGrid, IDataTemplate>(nameof(RowDetailsTemplate));

    public static readonly StyledProperty<DataGridRowDetailsVisibilityMode> RowDetailsVisibilityModeProperty =
        AvaloniaProperty.Register<DataGrid, DataGridRowDetailsVisibilityMode>(nameof(RowDetailsVisibilityMode));


    public static readonly DirectProperty<DataGrid, IDataGridCollectionView> CollectionViewProperty =
        AvaloniaProperty.RegisterDirect<DataGrid, IDataGridCollectionView>(nameof(CollectionView),
            o => o.CollectionView);

    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<DataGrid, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    private readonly IndexToValueTable<bool> _collapsedSlotsTable;
    private readonly List<DataGridRow> _loadedRows;

    // prevents reentry into the VerticalScroll event handler
    private readonly Queue<Action> _lostFocusActions;
    private readonly DataGridSelectedItemsCollection _selectedItems;
    private readonly IndexToValueTable<bool> _showDetailsTable;
    private bool _areHandlersSuspended;
    private byte _autoGeneratingColumnOperationCount;
    private bool _autoSizingColumns;

    private Visual _bottomRightCorner;
    private Control _clickedElement;
    private ContentControl _clipboardContentControl;

    // used to store the current column during a Reset
    private int _desiredCurrentColumnIndex;

    // this is a workaround only for the scenarios where we need it, it is not all encompassing nor always updated
    private RoutedEventArgs _editingEventArgs;
    private bool _executingLostFocusActions;
    private bool _flushCurrentCellChanged;
    private bool _focusEditingControl;
    private Visual _focusedObject;
    private DataGridRow _focusedRow;
    private Control _frozenColumnScrollBarSpacer;

    // the sum of the widths in pixels of the scrolling columns preceding
    // the first displayed scrolling column
    private double _horizontalOffset;
    private byte _horizontalScrollChangesIgnored;
    private bool _ignoreNextScrollBarsLayout;

    private bool _isValid = true;

    // Nth row of rows 0..N that make up the RowHeightEstimate
    private int _lastEstimatedRow;
    private bool _makeFirstDisplayedCellCurrentCellPending;
    private bool _measured;
    private int? _mouseOverRowIndex; // -1 is used for the 'new row'

    // the number of pixels of the firstDisplayedScrollingCol which are not displayed
    private int _noCurrentCellChangeCount;
    private int _noSelectionChangeCount;
    private DataGridColumn _previousCurrentColumn;
    private object _previousCurrentItem;
    private double[] _rowGroupHeightsByLevel;
    private double _rowHeaderDesiredWidth;
    private DataGridRowsPresenter _rowsPresenter;
    private Size? _rowsPresenterAvailableSize;
    private bool _scrollingByHeight;

    private int _selectedIndex = -1;
    private object _selectedItem;
    private bool _successfullyUpdatedSelection;
    private bool _temporarilyResetCurrentCell;

    private ContentControl _topLeftCornerHeader;

    private INotifyCollectionChanged _topLevelGroup;
    private ContentControl _topRightCornerHeader;
    private object _uneditedValue; // Represents the original current cell value at the time it enters editing mode.

    // An approximation of the sum of the heights in pixels of the scrolling rows preceding
    // the first displayed scrolling row.  Since the scrolled off rows are discarded, the grid
    // does not know their actual height. The heights used for the approximation are the ones
    // set as the rows were scrolled off.
    private double _verticalOffset;
    private byte _verticalScrollChangesIgnored;

    static DataGrid()
    {
        AffectsMeasure<DataGrid>(
            ColumnHeaderHeightProperty,
            HorizontalScrollBarVisibilityProperty,
            VerticalScrollBarVisibilityProperty);

        ItemsSourceProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnItemsSourcePropertyChanged(e));
        CanUserResizeColumnsProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnCanUserResizeColumnsChanged(e));
        ColumnWidthProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnColumnWidthChanged(e));
        FrozenRightColumnCountProperty.Changed.AddClassHandler<DataGrid>((grid, _) => grid.ProcessFrozenColumnCount());
        FrozenColumnCountProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnFrozenColumnCountChanged(e));
        GridLinesVisibilityProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnGridLinesVisibilityChanged(e));
        HeadersVisibilityProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnHeadersVisibilityChanged(e));
        HorizontalGridLinesBrushProperty.Changed.AddClassHandler<DataGrid>((x, e) =>
            x.OnHorizontalGridLinesBrushChanged(e));
        IsReadOnlyProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnIsReadOnlyChanged(e));
        MaxColumnWidthProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnMaxColumnWidthChanged(e));
        MinColumnWidthProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnMinColumnWidthChanged(e));
        RowHeightProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnRowHeightChanged(e));
        RowHeaderWidthProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnRowHeaderWidthChanged(e));
        SelectionModeProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnSelectionModeChanged(e));
        VerticalGridLinesBrushProperty.Changed.AddClassHandler<DataGrid>((x, e) =>
            x.OnVerticalGridLinesBrushChanged(e));
        SelectedIndexProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnSelectedIndexChanged(e));
        SelectedItemProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnSelectedItemChanged(e));
        IsEnabledProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.DataGrid_IsEnabledChanged(e));
        AreRowGroupHeadersFrozenProperty.Changed.AddClassHandler<DataGrid>((x, e) =>
            x.OnAreRowGroupHeadersFrozenChanged(e));
        RowDetailsTemplateProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnRowDetailsTemplateChanged(e));
        RowDetailsVisibilityModeProperty.Changed.AddClassHandler<DataGrid>((x, e) =>
            x.OnRowDetailsVisibilityModeChanged(e));
        AutoGenerateColumnsProperty.Changed.AddClassHandler<DataGrid>((x, e) => x.OnAutoGenerateColumnsChanged(e));

        FocusableProperty.OverrideDefaultValue<DataGrid>(true);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Avalonia.Controls.DataGrid" /> class.
    /// </summary>
    public DataGrid()
    {
        KeyDown += DataGrid_KeyDown;
        KeyUp += DataGrid_KeyUp;

        //TODO: Check if override works
        GotFocus += DataGrid_GotFocus;
        LostFocus += DataGrid_LostFocus;

        _loadedRows = new List<DataGridRow>();
        _lostFocusActions = new Queue<Action>();
        _selectedItems = new DataGridSelectedItemsCollection(this);
        RowGroupHeadersTable = new IndexToValueTable<DataGridRowGroupInfo>();

        DisplayData = new DataGridDisplayData(this);
        ColumnsInternal = CreateColumnsInstance();
        ColumnsInternal.CollectionChanged += ColumnsInternal_CollectionChanged;

        RowHeightEstimate = DATAGRID_defaultRowHeight;
        RowDetailsHeightEstimate = 0;
        _rowHeaderDesiredWidth = 0;

        DataConnection = new DataGridDataConnection(this);
        _showDetailsTable = new IndexToValueTable<bool>();
        _collapsedSlotsTable = new IndexToValueTable<bool>();

        AnchorSlot = -1;
        _lastEstimatedRow = -1;
        EditingColumnIndex = -1;
        _mouseOverRowIndex = null;
        CurrentCellCoordinates = new DataGridCellCoordinates(-1, -1);

        RowGroupHeaderHeightEstimate = DATAGRID_defaultRowHeight;

        UpdatePseudoClasses();
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the user can change
    ///     the column display order by dragging column headers with the mouse.
    /// </summary>
    public bool CanUserReorderColumns
    {
        get => GetValue(CanUserReorderColumnsProperty);
        set => SetValue(CanUserReorderColumnsProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the user can adjust column widths using the mouse.
    /// </summary>
    public bool CanUserResizeColumns
    {
        get => GetValue(CanUserResizeColumnsProperty);
        set => SetValue(CanUserResizeColumnsProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the user can sort columns by clicking the column header.
    /// </summary>
    public bool CanUserSortColumns
    {
        get => GetValue(CanUserSortColumnsProperty);
        set => SetValue(CanUserSortColumnsProperty, value);
    }

    /// <summary>
    ///     Gets or sets the height of the column headers row.
    /// </summary>
    public double ColumnHeaderHeight
    {
        get => GetValue(ColumnHeaderHeightProperty);
        set => SetValue(ColumnHeaderHeightProperty, value);
    }

    /// <summary>
    ///     Gets or sets the theme applied to all rows.
    /// </summary>
    public ControlTheme RowTheme
    {
        get => GetValue(RowThemeProperty);
        set => SetValue(RowThemeProperty, value);
    }

    /// <summary>
    ///     Gets or sets the theme applied to all cells.
    /// </summary>
    public ControlTheme CellTheme
    {
        get => GetValue(CellThemeProperty);
        set => SetValue(CellThemeProperty, value);
    }

    /// <summary>
    ///     Gets or sets the theme applied to all column headers.
    /// </summary>
    public ControlTheme ColumnHeaderTheme
    {
        get => GetValue(ColumnHeaderThemeProperty);
        set => SetValue(ColumnHeaderThemeProperty, value);
    }

    /// <summary>
    ///     Gets or sets the theme applied to all row groups.
    /// </summary>
    public ControlTheme RowGroupTheme
    {
        get => GetValue(RowGroupThemeProperty);
        set => SetValue(RowGroupThemeProperty, value);
    }

    /// <summary>
    ///     Gets or sets the standard width or automatic sizing mode of columns in the control.
    /// </summary>
    public DataGridLength ColumnWidth
    {
        get => GetValue(ColumnWidthProperty);
        set => SetValue(ColumnWidthProperty, value);
    }

    /// <summary>
    ///     Gets or sets the number of columns that the user cannot scroll horizontally.
    /// </summary>
    public int FrozenColumnCount
    {
        get => GetValue(FrozenColumnCountProperty);
        set => SetValue(FrozenColumnCountProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates which grid lines separating inner cells are shown.
    /// </summary>
    public DataGridGridLinesVisibility GridLinesVisibility
    {
        get => GetValue(GridLinesVisibilityProperty);
        set => SetValue(GridLinesVisibilityProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates the visibility of row and column headers.
    /// </summary>
    public DataGridHeadersVisibility HeadersVisibility
    {
        get => GetValue(HeadersVisibilityProperty);
        set => SetValue(HeadersVisibilityProperty, value);
    }

    /// <summary>
    ///     Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating rows.
    /// </summary>
    public IBrush HorizontalGridLinesBrush
    {
        get => GetValue(HorizontalGridLinesBrushProperty);
        set => SetValue(HorizontalGridLinesBrushProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates how the horizontal scroll bar is displayed.
    /// </summary>
    public ScrollBarVisibility HorizontalScrollBarVisibility
    {
        get => GetValue(HorizontalScrollBarVisibilityProperty);
        set => SetValue(HorizontalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the user can edit the values in the control.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the row group header sections
    ///     remain fixed at the width of the display area or can scroll horizontally.
    /// </summary>
    public bool AreRowGroupHeadersFrozen
    {
        get => GetValue(AreRowGroupHeadersFrozenProperty);
        set => SetValue(AreRowGroupHeadersFrozenProperty, value);
    }

    /// <summary>
    ///     Gets or sets whether scroll gestures should include inertia in their behavior and value.
    /// </summary>
    public bool IsScrollInertiaEnabled
    {
        get => GetValue(IsScrollInertiaEnabledProperty);
        set => SetValue(IsScrollInertiaEnabledProperty, value);
    }

    public bool IsValid
    {
        get => _isValid;
        internal set
        {
            SetAndRaise(IsValidProperty, ref _isValid, value);
            PseudoClasses.Set(":invalid", !value);
        }
    }

    /// <summary>
    ///     Gets or sets the maximum width of columns in the <see cref="T:Avalonia.Controls.DataGrid" /> .
    /// </summary>
    public double MaxColumnWidth
    {
        get => GetValue(MaxColumnWidthProperty);
        set => SetValue(MaxColumnWidthProperty, value);
    }

    /// <summary>
    ///     Gets or sets the minimum width of columns in the <see cref="T:Avalonia.Controls.DataGrid" />.
    /// </summary>
    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    /// <summary>
    ///     Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint row backgrounds.
    /// </summary>
    public IBrush RowBackground
    {
        get => GetValue(RowBackgroundProperty);
        set => SetValue(RowBackgroundProperty, value);
    }

    /// <summary>
    ///     Gets or sets the standard height of rows in the control.
    /// </summary>
    public double RowHeight
    {
        get => GetValue(RowHeightProperty);
        set => SetValue(RowHeightProperty, value);
    }

    /// <summary>
    ///     Gets or sets the width of the row header column.
    /// </summary>
    public double RowHeaderWidth
    {
        get => GetValue(RowHeaderWidthProperty);
        set => SetValue(RowHeaderWidthProperty, value);
    }

    /// <summary>
    ///     Gets or sets the selection behavior of the data grid.
    /// </summary>
    public DataGridSelectionMode SelectionMode
    {
        get => GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    /// <summary>
    ///     Gets or sets the <see cref="T:System.Windows.Media.Brush" /> that is used to paint grid lines separating columns.
    /// </summary>
    public IBrush VerticalGridLinesBrush
    {
        get => GetValue(VerticalGridLinesBrushProperty);
        set => SetValue(VerticalGridLinesBrushProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates how the vertical scroll bar is displayed.
    /// </summary>
    public ScrollBarVisibility VerticalScrollBarVisibility
    {
        get => GetValue(VerticalScrollBarVisibilityProperty);
        set => SetValue(VerticalScrollBarVisibilityProperty, value);
    }

    /// <summary>
    ///     Gets or sets the template that is used when rendering the column headers.
    /// </summary>
    public ITemplate<Control> DropLocationIndicatorTemplate
    {
        get => GetValue(DropLocationIndicatorTemplateProperty);
        set => SetValue(DropLocationIndicatorTemplateProperty, value);
    }

    /// <summary>
    ///     Gets or sets the index of the current selection.
    /// </summary>
    /// <returns>
    ///     The index of the current selection, or -1 if the selection is empty.
    /// </returns>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetAndRaise(SelectedIndexProperty, ref _selectedIndex, value);
    }

    /// <summary>
    ///     Gets or sets the data item corresponding to the selected row.
    /// </summary>
    public object SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
    }

    /// <summary>
    ///     The property which determines how DataGrid content is copied to the Clipboard.
    /// </summary>
    public DataGridClipboardCopyMode ClipboardCopyMode
    {
        get => GetValue(ClipboardCopyModeProperty);
        set => SetValue(ClipboardCopyModeProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether columns are created
    ///     automatically when the <see cref="P:Avalonia.Controls.DataGrid.ItemsSource" /> property is set.
    /// </summary>
    public bool AutoGenerateColumns
    {
        get => GetValue(AutoGenerateColumnsProperty);
        set => SetValue(AutoGenerateColumnsProperty, value);
    }

    /// <summary>
    ///     Gets or sets a collection that is used to generate the content of the control.
    /// </summary>
    public IEnumerable ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates whether the row details sections remain
    ///     fixed at the width of the display area or can scroll horizontally.
    /// </summary>
    public bool AreRowDetailsFrozen
    {
        get => GetValue(AreRowDetailsFrozenProperty);
        set => SetValue(AreRowDetailsFrozenProperty, value);
    }

    /// <summary>
    ///     Gets or sets the template that is used to display the content of the details section of rows.
    /// </summary>
    public IDataTemplate RowDetailsTemplate
    {
        get => GetValue(RowDetailsTemplateProperty);
        set => SetValue(RowDetailsTemplateProperty, value);
    }

    /// <summary>
    ///     Gets or sets a value that indicates when the details sections of rows are displayed.
    /// </summary>
    public DataGridRowDetailsVisibilityMode RowDetailsVisibilityMode
    {
        get => GetValue(RowDetailsVisibilityModeProperty);
        set => SetValue(RowDetailsVisibilityModeProperty, value);
    }

    /// <summary>
    ///     Gets current <see cref="IDataGridCollectionView" />.
    /// </summary>
    public IDataGridCollectionView CollectionView =>
        DataConnection.CollectionView;

    /// <summary>
    ///     Gets a collection that contains all the columns in the control.
    /// </summary>
    public ObservableCollection<DataGridColumn> Columns =>
        // we use a backing field here because the field's type
        // is a subclass of the property's
        ColumnsInternal;

    /// <summary>
    ///     Gets or sets the column that contains the current cell.
    /// </summary>
    public DataGridColumn CurrentColumn
    {
        get
        {
            if (CurrentColumnIndex == -1) return null;
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            return ColumnsItemsInternal[CurrentColumnIndex];
        }
        set
        {
            var dataGridColumn = value;
            if (dataGridColumn == null) throw DataGridError.DataGrid.ValueCannotBeSetToNull("value", "CurrentColumn");
            if (CurrentColumn != dataGridColumn)
            {
                if (dataGridColumn.OwningGrid != this)
                    // Provided column does not belong to this DataGrid
                    throw DataGridError.DataGrid.ColumnNotInThisDataGrid();
                if (!dataGridColumn.IsVisible)
                    // CurrentColumn cannot be set to an invisible column
                    throw DataGridError.DataGrid.ColumnCannotBeCollapsed();
                if (CurrentSlot == -1)
                    // There is no current row so the current column cannot be set
                    throw DataGridError.DataGrid.NoCurrentRow();
                var beginEdit = EditingColumnIndex != -1;

                //exitEditingMode, keepFocus, raiseEvents
                if (!EndCellEdit(DataGridEditAction.Commit, true, ContainsFocus, true))
                    // Edited value couldn't be committed or aborted
                    return;

                UpdateSelectionAndCurrency(dataGridColumn.Index, CurrentSlot, DataGridSelectionAction.None,
                    false); //scrollIntoView
                Debug.Assert(_successfullyUpdatedSelection);

                if (beginEdit &&
                    EditingColumnIndex == -1 &&
                    CurrentSlot != -1 &&
                    CurrentColumnIndex != -1 &&
                    CurrentColumnIndex == dataGridColumn.Index &&
                    dataGridColumn.OwningGrid == this &&
                    !GetColumnEffectiveReadOnlyState(dataGridColumn))
                    // Returning to editing mode since the grid was in that mode prior to the EndCellEdit call above.
                    BeginCellEdit(new RoutedEventArgs());
            }
        }
    }

    /// <summary>
    ///     Gets a list that contains the data items corresponding to the selected rows.
    /// </summary>
    public IList SelectedItems => _selectedItems;

    internal DataGridColumnCollection ColumnsInternal { get; }

    internal int AnchorSlot { get; private set; }

    internal double ActualRowHeaderWidth
    {
        get
        {
            if (!AreRowHeadersVisible) return 0;

            return !double.IsNaN(RowHeaderWidth) ? RowHeaderWidth : RowHeadersDesiredWidth;
        }
    }

    internal double ActualRowsPresenterHeight
    {
        get
        {
            if (_rowsPresenter != null) return _rowsPresenter.Bounds.Height;
            return 0;
        }
    }

    internal bool AreColumnHeadersVisible =>
        (HeadersVisibility & DataGridHeadersVisibility.Column) == DataGridHeadersVisibility.Column;

    internal bool AreRowHeadersVisible =>
        (HeadersVisibility & DataGridHeadersVisibility.Row) == DataGridHeadersVisibility.Row;

    /// <summary>
    ///     Indicates whether or not at least one auto-sizing column is waiting for all the rows
    ///     to be measured before its final width is determined.
    /// </summary>
    internal bool AutoSizingColumns
    {
        get => _autoSizingColumns;
        set
        {
            if (_autoSizingColumns && !value && ColumnsInternal != null)
            {
                var adjustment = CellsWidth - ColumnsInternal.VisibleEdgedColumnsWidth;
                AdjustColumnWidths(0, adjustment, false);
                foreach (var column in ColumnsInternal.GetVisibleColumns())
                    column.IsInitialDesiredWidthDetermined = true;
                ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
                ComputeScrollBarsLayout();
                InvalidateColumnHeadersMeasure();
                InvalidateRowsMeasure(true);
            }

            _autoSizingColumns = value;
        }
    }

    internal double AvailableSlotElementRoom { get; set; }

    internal double CellsEstimatedHeight => RowsPresenterAvailableSize?.Height ?? 0;

    // Width currently available for cells this value is smaller.  This width is reduced by the existence of RowHeaders
    // or a vertical scrollbar.  Layout is asynchronous so changes to the RowHeaders or the vertical scrollbar are
    // not reflected immediately
    internal double CellsWidth
    {
        get
        {
            var rowsWidth = double.PositiveInfinity;
            if (RowsPresenterAvailableSize.HasValue)
                rowsWidth = Math.Max(0, RowsPresenterAvailableSize.Value.Width - ActualRowHeaderWidth);
            return double.IsPositiveInfinity(rowsWidth) ? ColumnsInternal.VisibleEdgedColumnsWidth : rowsWidth;
        }
    }

    internal DataGridColumnHeadersPresenter ColumnHeaders { get; private set; }

    internal List<DataGridColumn> ColumnsItemsInternal => ColumnsInternal.ItemsInternal;

    internal bool ContainsFocus { get; private set; }

    internal int CurrentColumnIndex
    {
        get => CurrentCellCoordinates.ColumnIndex;

        private set => CurrentCellCoordinates.ColumnIndex = value;
    }

    internal int CurrentSlot
    {
        get => CurrentCellCoordinates.Slot;

        private set => CurrentCellCoordinates.Slot = value;
    }

    internal DataGridDataConnection DataConnection { get; }

    internal DataGridDisplayData DisplayData { get; }

    internal int EditingColumnIndex { get; private set; }

    internal DataGridRow EditingRow { get; private set; }

    internal double FirstDisplayedScrollingColumnHiddenWidth { get; private set; }

    // When the RowsPresenter's width increases, the HorizontalOffset will be incorrect until
    // the scrollbar's layout is recalculated, which doesn't occur until after the cells are measured.
    // This property exists to account for this scenario, and avoid collapsing the incorrect cells.
    internal double HorizontalAdjustment { get; private set; }

    internal static double HorizontalGridLinesThickness => DATAGRID_horizontalGridLinesThickness;

    // the sum of the widths in pixels of the scrolling columns preceding
    // the first displayed scrolling column
    internal double HorizontalOffset
    {
        get => _horizontalOffset;
        set
        {
            if (value < 0) value = 0;
            var widthNotVisible = Math.Max(0, ColumnsInternal.VisibleEdgedColumnsWidth - CellsWidth);
            if (value > widthNotVisible) value = widthNotVisible;
            if (value == _horizontalOffset) return;

            if (HorizontalScrollBar != null && value != HorizontalScrollBar.Value) HorizontalScrollBar.Value = value;
            _horizontalOffset = value;

            DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
            // update the lastTotallyDisplayedScrollingCol
            ComputeDisplayedColumns();
        }
    }

    internal ScrollBar HorizontalScrollBar { get; private set; }

    internal IndexToValueTable<DataGridRowGroupInfo> RowGroupHeadersTable { get; }

    internal bool LoadingOrUnloadingRow { get; private set; }

    internal bool InDisplayIndexAdjustments { get; set; }

    internal int? MouseOverRowIndex
    {
        get => _mouseOverRowIndex;
        set
        {
            if (_mouseOverRowIndex != value)
            {
                DataGridRow oldMouseOverRow = null;
                if (_mouseOverRowIndex.HasValue)
                {
                    var oldSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                    if (IsSlotVisible(oldSlot))
                        oldMouseOverRow = DisplayData.GetDisplayedElement(oldSlot) as DataGridRow;
                }

                _mouseOverRowIndex = value;

                // State for the old row needs to be applied after setting the new value
                if (oldMouseOverRow != null) oldMouseOverRow.ApplyState();

                if (_mouseOverRowIndex.HasValue)
                {
                    var newSlot = SlotFromRowIndex(_mouseOverRowIndex.Value);
                    if (IsSlotVisible(newSlot))
                    {
                        var newMouseOverRow = DisplayData.GetDisplayedElement(newSlot) as DataGridRow;
                        Debug.Assert(newMouseOverRow != null);
                        if (newMouseOverRow != null) newMouseOverRow.ApplyState();
                    }
                }
            }
        }
    }

    internal double NegVerticalOffset { get; private set; }

    internal int NoCurrentCellChangeCount
    {
        get => _noCurrentCellChangeCount;
        set
        {
            _noCurrentCellChangeCount = value;
            if (value == 0) FlushCurrentCellChanged();
        }
    }

    internal double RowDetailsHeightEstimate { get; private set; }

    internal double RowHeadersDesiredWidth
    {
        get => _rowHeaderDesiredWidth;
        set
        {
            // We only auto grow
            if (_rowHeaderDesiredWidth < value)
            {
                var oldActualRowHeaderWidth = ActualRowHeaderWidth;
                _rowHeaderDesiredWidth = value;
                if (oldActualRowHeaderWidth != ActualRowHeaderWidth) EnsureRowHeaderWidth();
            }
        }
    }

    internal double RowGroupHeaderHeightEstimate { get; }

    internal double RowHeightEstimate { get; private set; }

    internal Size? RowsPresenterAvailableSize
    {
        get => _rowsPresenterAvailableSize;
        set
        {
            if (_rowsPresenterAvailableSize.HasValue && value.HasValue &&
                value.Value.Width > RowsPresenterAvailableSize.Value.Width)
            {
                // When the available cells width increases, the horizontal offset can be incorrect.
                // Store away an adjustment to use during the CellsPresenter's measure, so that the
                // ShouldDisplayCell method correctly determines if a cell will be in view.
                //
                //     |   h. offset   |       new available cells width          |
                //     |-------------->|----------------------------------------->|
                //      __________________________________________________        |
                //     |           |           |             |            |       |
                //     |  column0  |  column1  |   column2   |  column3   |<----->|
                //     |           |           |             |            |  adj. |
                //
                var adjustment = _horizontalOffset + value.Value.Width - ColumnsInternal.VisibleEdgedColumnsWidth;
                HorizontalAdjustment = Math.Min(HorizontalOffset, Math.Max(0, adjustment));
            }
            else
            {
                HorizontalAdjustment = 0;
            }

            _rowsPresenterAvailableSize = value;
        }
    }

    internal double[] RowGroupSublevelIndents { get; private set; }

    // This flag indicates whether selection has actually changed during a selection operation,
    // and exists to ensure that FlushSelectionChanged doesn't unnecessarily raise SelectionChanged.
    internal bool SelectionHasChanged { get; set; }

    internal int SlotCount { get; private set; }

    /// <summary>
    ///     Indicates whether or not to use star-sizing logic.  If the DataGrid has infinite available space,
    ///     then star sizing doesn't make sense.  In this case, all star columns grow to a predefined size of
    ///     10,000 pixels in order to show the developer that star columns shouldn't be used.
    /// </summary>
    internal bool UsesStarSizing
    {
        get
        {
            if (ColumnsInternal != null)
                return ColumnsInternal.VisibleStarColumnCount > 0 &&
                       (!RowsPresenterAvailableSize.HasValue ||
                        !double.IsPositiveInfinity(RowsPresenterAvailableSize.Value.Width));
            return false;
        }
    }

    internal ScrollBar VerticalScrollBar { get; private set; }

    internal int VisibleSlotCount { get; set; }

    /// <summary>
    ///     Gets the data item bound to the row that contains the current cell.
    /// </summary>
    protected object CurrentItem
    {
        get
        {
            if (CurrentSlot == -1 || ItemsSource == null || RowGroupHeadersTable.Contains(CurrentSlot)) return null;
            return DataConnection.GetDataItem(RowIndexFromSlot(CurrentSlot));
        }
    }

    private DataGridCellCoordinates CurrentCellCoordinates { get; }

    private int FirstDisplayedNonFillerColumnIndex
    {
        get
        {
            var column = ColumnsInternal.FirstVisibleNonFillerColumn;
            if (column != null)
            {
                if (column.IsFrozen) return column.Index;

                if (DisplayData.FirstDisplayedScrollingCol >= column.Index)
                    return DisplayData.FirstDisplayedScrollingCol;

                return column.Index;
            }

            return -1;
        }
    }

    private bool IsHorizontalScrollBarOverCells => ColumnHeaders != null && Grid.GetColumnSpan(ColumnHeaders) == 2;

    private bool IsVerticalScrollBarOverCells => _rowsPresenter != null && Grid.GetRowSpan(_rowsPresenter) == 2;

    private int NoSelectionChangeCount
    {
        get => _noSelectionChangeCount;
        set
        {
            _noSelectionChangeCount = value;
            if (value == 0) FlushSelectionChanged();
        }
    }

    /// <summary>
    ///     This is an empty content control that's used during the DataGrid's copy procedure
    ///     to determine the value of a ClipboardContentBinding for a particular column and item.
    /// </summary>
    internal ContentControl ClipboardContentControl
    {
        get
        {
            if (_clipboardContentControl == null) _clipboardContentControl = new ContentControl();
            return _clipboardContentControl;
        }
    }

    public event EventHandler<ScrollEventArgs> HorizontalScroll;
    public event EventHandler<ScrollEventArgs> VerticalScroll;

    private static bool IsValidColumnHeaderHeight(double value)
    {
        return double.IsNaN(value) ||
               (value >= DATAGRID_minimumColumnHeaderHeight && value <= DATAGRID_maxHeadersThickness);
    }

    private static bool ValidateFrozenColumnCount(int value)
    {
        return value >= 0;
    }

    private void OnAreRowGroupHeadersFrozenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = (bool)e.NewValue;
        ProcessFrozenColumnCount();

        // Update elements in the RowGroupHeader that were previously frozen
        if (value)
            if (_rowsPresenter != null)
                foreach (var element in _rowsPresenter.Children)
                    if (element is DataGridRowGroupHeader groupHeader)
                        groupHeader.ClearFrozenStates();
    }

    private static bool IsValidColumnWidth(double value)
    {
        return !double.IsNaN(value) && value > 0;
    }

    private static bool IsValidMinColumnWidth(double value)
    {
        return !double.IsNaN(value) && !double.IsPositiveInfinity(value) && value >= 0;
    }

    private static bool IsValidRowHeight(double value)
    {
        return double.IsNaN(value) ||
               (value >= DataGridRow.DATAGRIDROW_minimumHeight &&
                value <= DataGridRow.DATAGRIDROW_maximumHeight);
    }

    private static bool IsValidRowHeaderWidth(double value)
    {
        return double.IsNaN(value) ||
               (value >= DATAGRID_minimumRowHeaderWidth &&
                value <= DATAGRID_maxHeadersThickness);
    }

    private void OnAutoGenerateColumnsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = (bool)e.NewValue;
        if (value)
            InitializeElements(false);
        else
            RemoveAutoGeneratedColumns();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new DataGridAutomationPeer(this);
    }

    private void SetValueNoCallback<T>(AvaloniaProperty<T> property, T value,
        BindingPriority priority = BindingPriority.LocalValue)
    {
        _areHandlersSuspended = true;
        try
        {
            SetValue(property, value, priority);
        }
        finally
        {
            _areHandlersSuspended = false;
        }
    }

    private void OnRowDetailsVisibilityModeChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdateRowDetailsVisibilityMode((DataGridRowDetailsVisibilityMode)e.NewValue);
    }

    private void OnRowDetailsTemplateChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Update the RowDetails templates if necessary
        if (_rowsPresenter != null)
            foreach (var row in GetAllRows())
                if (GetRowDetailsVisibility(row.Index))
                    // DetailsPreferredHeight is initialized when the DetailsElement's size changes.
                    row.ApplyDetailsTemplate(false);

        UpdateRowDetailsHeightEstimate();
        InvalidateMeasure();
    }

    /// <summary>
    ///     ItemsSourceProperty property changed handler.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    private void OnItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            Debug.Assert(DataConnection != null);

            var oldCollectionView = DataConnection.CollectionView;

            var oldValue = (IEnumerable)e.OldValue;
            var newItemsSource = (IEnumerable)e.NewValue;

            if (LoadingOrUnloadingRow)
            {
                SetValueNoCallback(ItemsSourceProperty, oldValue);
                throw DataGridError.DataGrid.CannotChangeItemsWhenLoadingRows();
            }

            // Try to commit edit on the old DataSource, but force a cancel if it fails
            if (!CommitEdit()) CancelEdit(DataGridEditingUnit.Row, false);

            DataConnection.UnWireEvents(DataConnection.DataSource);
            DataConnection.ClearDataProperties();
            ClearRowGroupHeadersTable();

            // The old selected indexes are no longer relevant. There's a perf benefit from
            // updating the selected indexes with a null DataSource, because we know that all
            // of the previously selected indexes have been removed from selection
            DataConnection.DataSource = null;
            _selectedItems.UpdateIndexes();
            CoerceSelectedItem();

            // Wrap an IEnumerable in an ICollectionView if it's not already one
            var setDefaultSelection = false;
            if (newItemsSource is IDataGridCollectionView newCollectionView)
                setDefaultSelection = true;
            else
                newCollectionView = newItemsSource is not null
                    ? DataGridDataConnection.CreateView(newItemsSource)
                    : default;

            DataConnection.DataSource = newCollectionView;

            if (oldCollectionView != DataConnection.CollectionView)
                RaisePropertyChanged(CollectionViewProperty,
                    oldCollectionView,
                    newCollectionView);

            if (DataConnection.DataSource != null)
            {
                // Setup the column headers
                if (DataConnection.DataType != null)
                    foreach (var column in ColumnsInternal.GetDisplayedColumns())
                        if (column is DataGridBoundColumn boundColumn)
                            boundColumn.SetHeaderFromBinding();

                DataConnection.WireEvents(DataConnection.DataSource);
            }

            // Wait for the current cell to be set before we raise any SelectionChanged events
            _makeFirstDisplayedCellCurrentCellPending = true;

            // Clear out the old rows and remove the generated columns
            ClearRows(false); //recycle
            RemoveAutoGeneratedColumns();

            // Set the SlotCount (from the data count and number of row group headers) before we make the default selection
            PopulateRowGroupHeadersTable();
            SelectedItem = null;
            if (DataConnection.CollectionView != null && setDefaultSelection)
                SelectedItem = DataConnection.CollectionView.CurrentItem;

            // Treat this like the DataGrid has never been measured because all calculations at
            // this point are invalid until the next layout cycle.  For instance, the ItemsSource
            // can be set when the DataGrid is not part of the visual tree
            _measured = false;
            InvalidateMeasure();

            UpdatePseudoClasses();
        }
    }

    private void ColumnsInternal_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add
            || e.Action == NotifyCollectionChangedAction.Remove
            || e.Action == NotifyCollectionChangedAction.Reset)
            UpdatePseudoClasses();
    }

    internal void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":empty-columns", !ColumnsInternal.GetVisibleColumns().Any());
        PseudoClasses.Set(":empty-rows", !DataConnection.Any());
    }

    private void OnSelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            var index = (int)e.NewValue;

            // GetDataItem returns null if index is >= Count, we do not check newValue
            // against Count here to avoid enumerating through an Enumerable twice
            // Setting SelectedItem coerces the finally value of the SelectedIndex
            var newSelectedItem = index < 0 ? null : DataConnection.GetDataItem(index);
            SelectedItem = newSelectedItem;
            if (SelectedItem != newSelectedItem) SetValueNoCallback(SelectedIndexProperty, (int)e.OldValue);
        }
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            var rowIndex = e.NewValue == null ? -1 : DataConnection.IndexOf(e.NewValue);
            if (rowIndex == -1)
            {
                // If the Item is null or it's not found, clear the Selection
                if (!CommitEdit(DataGridEditingUnit.Row, true))
                {
                    // Edited value couldn't be committed or aborted
                    SetValueNoCallback(SelectedItemProperty, e.OldValue);
                    return;
                }

                // Clear all row selections
                ClearRowSelection(true);

                if (DataConnection.CollectionView != null) DataConnection.CollectionView.MoveCurrentTo(null);
            }
            else
            {
                var slot = SlotFromRowIndex(rowIndex);
                if (slot != CurrentSlot)
                {
                    if (!CommitEdit(DataGridEditingUnit.Row, true))
                    {
                        // Edited value couldn't be committed or aborted
                        SetValueNoCallback(SelectedItemProperty, e.OldValue);
                        return;
                    }

                    if (slot >= SlotCount || slot < -1)
                        if (DataConnection.CollectionView != null)
                            DataConnection.CollectionView.MoveCurrentToPosition(rowIndex);
                }

                var oldSelectedIndex = SelectedIndex;
                SetValueNoCallback(SelectedIndexProperty, rowIndex);
                try
                {
                    _noSelectionChangeCount++;
                    var columnIndex = CurrentColumnIndex;

                    if (columnIndex == -1) columnIndex = FirstDisplayedNonFillerColumnIndex;
                    if (IsSlotOutOfSelectionBounds(slot))
                    {
                        ClearRowSelection(slot, true);
                        return;
                    }

                    UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, false);
                }
                finally
                {
                    NoSelectionChangeCount--;
                }

                if (!_successfullyUpdatedSelection)
                {
                    SetValueNoCallback(SelectedIndexProperty, oldSelectedIndex);
                    SetValueNoCallback(SelectedItemProperty, e.OldValue);
                }
            }
        }
    }

    private void OnVerticalGridLinesBrushChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (_rowsPresenter != null)
            foreach (var row in GetAllRows())
                row.EnsureGridLines();
    }

    private void OnSelectionModeChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended) ClearRowSelection(true);
    }

    private void OnRowHeaderWidthChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended) EnsureRowHeaderWidth();
    }

    private void OnRowHeightChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            InvalidateRowHeightEstimate();
            // Re-measure all the rows due to the Height change
            InvalidateRowsMeasure(true);
            // DataGrid needs to update the layout information and the ScrollBars
            InvalidateMeasure();
        }
    }

    private void OnMinColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            var oldValue = (double)e.OldValue;
            foreach (var column in ColumnsInternal.GetDisplayedColumns())
                OnColumnMinWidthChanged(column, Math.Max(column.MinWidth, oldValue));
        }
    }

    private void OnMaxColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            var oldValue = (double)e.OldValue;
            foreach (var column in ColumnsInternal.GetDisplayedColumns())
                OnColumnMaxWidthChanged(column, Math.Min(column.MaxWidth, oldValue));
        }
    }

    private void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended)
        {
            var value = (bool)e.NewValue;
            if (value && !CommitEdit(DataGridEditingUnit.Row, true)) CancelEdit(DataGridEditingUnit.Row, false);
        }
    }

    private void OnHorizontalGridLinesBrushChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (!_areHandlersSuspended && _rowsPresenter != null)
            foreach (var row in GetAllRows())
                row.EnsureGridLines();
    }

    private void OnHeadersVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldValue = (DataGridHeadersVisibility)e.OldValue;
        var newValue = (DataGridHeadersVisibility)e.NewValue;

        bool hasFlags(DataGridHeadersVisibility value, DataGridHeadersVisibility flags)
        {
            return (value & flags) == flags;
        }

        var newValueCols = hasFlags(newValue, DataGridHeadersVisibility.Column);
        var newValueRows = hasFlags(newValue, DataGridHeadersVisibility.Row);
        var oldValueCols = hasFlags(oldValue, DataGridHeadersVisibility.Column);
        var oldValueRows = hasFlags(oldValue, DataGridHeadersVisibility.Row);

        // Columns
        if (newValueCols != oldValueCols)
            if (ColumnHeaders != null)
            {
                EnsureColumnHeadersVisibility();
                if (!newValueCols)
                    ColumnHeaders.Measure(default);
                else
                    EnsureVerticalGridLines();
                InvalidateMeasure();
            }

        // Rows
        if (newValueRows != oldValueRows)
            if (_rowsPresenter != null)
            {
                foreach (var element in _rowsPresenter.Children)
                    if (element is DataGridRow row)
                    {
                        row.EnsureHeaderStyleAndVisibility(null);
                        if (newValueRows)
                        {
                            row.ApplyState();
                            row.EnsureHeaderVisibility();
                        }
                    }
                    else if (element is DataGridRowGroupHeader rowGroupHeader)
                    {
                        rowGroupHeader.EnsureHeaderVisibility();
                    }

                InvalidateRowHeightEstimate();
                InvalidateRowsMeasure(true);
            }

        if (_topLeftCornerHeader != null)
        {
            _topLeftCornerHeader.IsVisible = newValueRows && newValueCols;
            if (_topLeftCornerHeader.IsVisible) _topLeftCornerHeader.Measure(default);
        }
    }

    private void OnGridLinesVisibilityChanged(AvaloniaPropertyChangedEventArgs e)
    {
        foreach (var row in GetAllRows())
        {
            row.EnsureGridLines();
            row.InvalidateHorizontalArrange();
        }
    }

    private void OnFrozenColumnCountChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ProcessFrozenColumnCount();
    }

    private void ProcessFrozenColumnCount()
    {
        CorrectColumnFrozenStates();
        ComputeScrollBarsLayout();

        InvalidateColumnHeadersArrange();
        InvalidateCellsArrange();
    }

    private void OnColumnWidthChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var value = (DataGridLength)e.NewValue;

        foreach (var column in ColumnsInternal.GetDisplayedColumns())
            if (column.InheritsWidth)
                column.SetWidthInternalNoCallback(value);

        EnsureHorizontalLayout();
    }

    private void OnCanUserResizeColumnsChanged(AvaloniaPropertyChangedEventArgs e)
    {
        EnsureHorizontalLayout();
    }

    /// <summary>
    ///     Occurs one time for each public, non-static property in the bound data type when the
    ///     <see cref="P:Avalonia.Controls.DataGrid.ItemsSource" /> property is changed and the
    ///     <see cref="P:Avalonia.Controls.DataGrid.AutoGenerateColumns" /> property is true.
    /// </summary>
    public event EventHandler<DataGridAutoGeneratingColumnEventArgs> AutoGeneratingColumn;

    /// <summary>
    ///     Occurs before a cell or row enters editing mode.
    /// </summary>
    public event EventHandler<DataGridBeginningEditEventArgs> BeginningEdit;

    /// <summary>
    ///     Occurs after cell editing has ended.
    /// </summary>
    public event EventHandler<DataGridCellEditEndedEventArgs> CellEditEnded;

    /// <summary>
    ///     Occurs immediately before cell editing has ended.
    /// </summary>
    public event EventHandler<DataGridCellEditEndingEventArgs> CellEditEnding;

    /// <summary>
    ///     Occurs when cell is mouse-pressed.
    /// </summary>
    public event EventHandler<DataGridCellPointerPressedEventArgs> CellPointerPressed;

    /// <summary>
    ///     Occurs when the <see cref="P:Avalonia.Controls.DataGridColumn.DisplayIndex" />
    ///     property of a column changes.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs> ColumnDisplayIndexChanged;

    /// <summary>
    ///     Raised when column reordering ends, to allow subscribers to clean up.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs> ColumnReordered;

    /// <summary>
    ///     Raised when starting a column reordering action.  Subscribers to this event can
    ///     set tooltip and caret UIElements, constrain tooltip position, indicate that
    ///     a preview should be shown, or cancel reordering.
    /// </summary>
    public event EventHandler<DataGridColumnReorderingEventArgs> ColumnReordering;

    /// <summary>
    ///     Occurs when a different cell becomes the current cell.
    /// </summary>
    public event EventHandler<EventArgs> CurrentCellChanged;

    /// <summary>
    ///     Occurs after a <see cref="T:Avalonia.Controls.DataGridRow" />
    ///     is instantiated, so that you can customize it before it is used.
    /// </summary>
    public event EventHandler<DataGridRowEventArgs> LoadingRow;

    /// <summary>
    ///     Occurs when a cell in a <see cref="T:Avalonia.Controls.DataGridTemplateColumn" /> enters editing mode.
    /// </summary>
    public event EventHandler<DataGridPreparingCellForEditEventArgs> PreparingCellForEdit;

    /// <summary>
    ///     Occurs when the row has been successfully committed or cancelled.
    /// </summary>
    public event EventHandler<DataGridRowEditEndedEventArgs> RowEditEnded;

    /// <summary>
    ///     Occurs immediately before the row has been successfully committed or cancelled.
    /// </summary>
    public event EventHandler<DataGridRowEditEndingEventArgs> RowEditEnding;

    /// <summary>
    ///     Occurs when the <see cref="P:Avalonia.Controls.DataGrid.SelectedItem" /> or
    ///     <see cref="P:Avalonia.Controls.DataGrid.SelectedItems" /> property value changes.
    /// </summary>
    public event EventHandler<SelectionChangedEventArgs> SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    /// <summary>
    ///     Occurs when the <see cref="DataGridColumn" /> sorting request is triggered.
    /// </summary>
    public event EventHandler<DataGridColumnEventArgs> Sorting;

    /// <summary>
    ///     Occurs when a <see cref="T:Avalonia.Controls.DataGridRow" />
    ///     object becomes available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowEventArgs> UnloadingRow;

    /// <summary>
    ///     Occurs when a new row details template is applied to a row, so that you can customize
    ///     the details section before it is used.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs> LoadingRowDetails;

    /// <summary>
    ///     Occurs when the <see cref="P:Avalonia.Controls.DataGrid.RowDetailsVisibilityMode" />
    ///     property value changes.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs> RowDetailsVisibilityChanged;

    /// <summary>
    ///     Occurs when a row details element becomes available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowDetailsEventArgs> UnloadingRowDetails;

    /// <summary>
    ///     Enters editing mode for the current cell and current row (if they're not already in editing mode).
    /// </summary>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool BeginEdit()
    {
        return BeginEdit(null);
    }

    /// <summary>
    ///     Enters editing mode for the current cell and current row (if they're not already in editing mode).
    /// </summary>
    /// <param name="editingEventArgs">
    ///     Provides information about the user gesture that caused the call to BeginEdit. Can be
    ///     null.
    /// </param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool BeginEdit(RoutedEventArgs editingEventArgs)
    {
        if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot)) return false;

        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot);

        if (GetColumnEffectiveReadOnlyState(CurrentColumn))
            // Current column is read-only
            return false;
        return BeginCellEdit(editingEventArgs);
    }

    /// <summary>
    ///     Cancels editing mode and restores the original value.
    /// </summary>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CancelEdit()
    {
        return CancelEdit(DataGridEditingUnit.Row);
    }

    /// <summary>
    ///     Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CancelEdit(DataGridEditingUnit editingUnit)
    {
        return CancelEdit(editingUnit, true);
    }

    /// <summary>
    ///     Commits editing mode and pushes changes to the backend.
    /// </summary>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CommitEdit()
    {
        return CommitEdit(DataGridEditingUnit.Row, true);
    }

    /// <summary>
    ///     Commits editing mode for the specified DataGridEditingUnit and pushes changes to the backend.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to commit edit for a Cell or Row.</param>
    /// <param name="exitEditingMode">Editing mode is left if True.</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    public bool CommitEdit(DataGridEditingUnit editingUnit, bool exitEditingMode)
    {
        if (!EndCellEdit(
                DataGridEditAction.Commit,
                editingUnit == DataGridEditingUnit.Cell ? exitEditingMode : true,
                ContainsFocus,
                true))
            return false;
        if (editingUnit == DataGridEditingUnit.Row) return EndRowEdit(DataGridEditAction.Commit, exitEditingMode, true);
        return true;
    }

    /// <summary>
    ///     Scrolls the specified item or RowGroupHeader and/or column into view.
    ///     If item is not null: scrolls the row representing the item into view;
    ///     If column is not null: scrolls the column into view;
    ///     If both item and column are null, the method returns without scrolling.
    /// </summary>
    /// <param name="item">an item from the DataGrid's items source or a CollectionViewGroup from the collection view</param>
    /// <param name="column">a column from the DataGrid's columns collection</param>
    public void ScrollIntoView(object item, DataGridColumn column)
    {
        if ((column == null && (item == null || FirstDisplayedNonFillerColumnIndex == -1))
            || (column != null && column.OwningGrid != this))
            // no-op
            return;
        if (item == null)
        {
            // scroll column into view
            ScrollSlotIntoView(
                column.Index,
                DisplayData.FirstScrollingSlot,
                false,
                true);
        }
        else
        {
            var slot = -1;
            DataGridRowGroupInfo rowGroupInfo = null;
            if (item is DataGridCollectionViewGroup collectionViewGroup)
            {
                rowGroupInfo = RowGroupInfoFromCollectionViewGroup(collectionViewGroup);
                if (rowGroupInfo == null)
                {
                    Debug.Assert(false);
                    return;
                }

                slot = rowGroupInfo.Slot;
            }
            else
            {
                // the row index will be set to -1 if the item is null or not in the list
                var rowIndex = DataConnection.IndexOf(item);
                if (rowIndex == -1) return;
                slot = SlotFromRowIndex(rowIndex);
            }

            var columnIndex = column == null ? FirstDisplayedNonFillerColumnIndex : column.Index;

            if (_collapsedSlotsTable.Contains(slot))
            {
                // We need to expand all parent RowGroups so that the slot is visible
                if (rowGroupInfo != null)
                {
                    ExpandRowGroupParentChain(rowGroupInfo.Level - 1, rowGroupInfo.Slot);
                }
                else
                {
                    rowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(slot));
                    Debug.Assert(rowGroupInfo != null);
                    if (rowGroupInfo != null) ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
                }

                // Update Scrollbar and display information
                NegVerticalOffset = 0;
                SetVerticalOffset(0);
                ResetDisplayedRows();
                DisplayData.FirstScrollingSlot = 0;
                ComputeScrollBarsLayout();
            }

            ScrollSlotIntoView(
                columnIndex, slot,
                true,
                true);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (DataConnection.DataSource != null && !DataConnection.EventsWired)
        {
            DataConnection.WireEvents(DataConnection.DataSource);
            InitializeElements(true /*recycleRows*/);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        // When wired to INotifyCollectionChanged, the DataGrid will be cleaned up by GC
        if (DataConnection.DataSource != null && DataConnection.EventsWired)
            DataConnection.UnWireEvents(DataConnection.DataSource);
    }

    /// <summary>
    ///     Arranges the content of the <see cref="T:Avalonia.Controls.DataGridRow" />.
    /// </summary>
    /// <param name="finalSize">
    ///     The final area within the parent that this element should use to arrange itself and its children.
    /// </param>
    /// <returns>
    ///     The actual size used by the <see cref="T:Avalonia.Controls.DataGridRow" />.
    /// </returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if (_makeFirstDisplayedCellCurrentCellPending) MakeFirstDisplayedCellCurrentCell();

        if (Bounds.Width != finalSize.Width)
        {
            // If our final width has changed, we might need to update the filler
            InvalidateColumnHeadersArrange();
            InvalidateCellsArrange();
        }

        return base.ArrangeOverride(finalSize);
    }

    /// <summary>
    ///     Measures the children of a <see cref="T:Avalonia.Controls.DataGridRow" /> to prepare for
    ///     arranging them during the
    ///     <see cref="M:Avalonia.Controls.DataGridRow.ArrangeOverride(System.Windows.Size)" /> pass.
    /// </summary>
    /// <returns>
    ///     The size that the <see cref="T:Avalonia.Controls.DataGridRow" /> determines it needs during layout, based on its
    ///     calculations of child object allocated sizes.
    /// </returns>
    /// <param name="availableSize">
    ///     The available size that this element can give to child elements. Indicates an upper limit that
    ///     child elements should not exceed.
    /// </param>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Delay layout until after the initial measure to avoid invalid calculations when the
        // DataGrid is not part of the visual tree
        if (!_measured)
        {
            _measured = true;

            // We don't need to clear the rows because it was already done when the ItemsSource changed
            RefreshRowsAndColumns(false);

            //// Update our estimates now that the DataGrid has all of the information necessary
            UpdateRowDetailsHeightEstimate();

            // Update frozen columns to account for columns added prior to loading or autogenerated columns
            if (FrozenColumnCountWithFiller > 0) ProcessFrozenColumnCount();
        }

        Size desiredSize;
        // This is a shortcut to skip layout if we don't have any columns
        if (ColumnsInternal.VisibleEdgedColumnsWidth == 0)
        {
            if (HorizontalScrollBar != null && HorizontalScrollBar.IsVisible) HorizontalScrollBar.IsVisible = false;
            if (VerticalScrollBar != null && VerticalScrollBar.IsVisible) VerticalScrollBar.IsVisible = false;
            desiredSize = base.MeasureOverride(availableSize);
        }
        else
        {
            if (_rowsPresenter != null) _rowsPresenter.InvalidateMeasure();

            InvalidateColumnHeadersMeasure();

            desiredSize = base.MeasureOverride(availableSize);

            ComputeScrollBarsLayout();
        }

        return desiredSize;
    }

    /// <summary>
    ///     Raises the BeginningEdit event.
    /// </summary>
    protected virtual void OnBeginningEdit(DataGridBeginningEditEventArgs e)
    {
        BeginningEdit?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the CellEditEnded event.
    /// </summary>
    protected virtual void OnCellEditEnded(DataGridCellEditEndedEventArgs e)
    {
        CellEditEnded?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the CellEditEnding event.
    /// </summary>
    protected virtual void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
    {
        CellEditEnding?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the CellPointerPressed event.
    /// </summary>
    internal virtual void OnCellPointerPressed(DataGridCellPointerPressedEventArgs e)
    {
        CellPointerPressed?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the CurrentCellChanged event.
    /// </summary>
    protected virtual void OnCurrentCellChanged(EventArgs e)
    {
        CurrentCellChanged?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the LoadingRow event for row preparation.
    /// </summary>
    protected virtual void OnLoadingRow(DataGridRowEventArgs e)
    {
        var handler = LoadingRow;
        if (handler != null)
        {
            Debug.Assert(!_loadedRows.Contains(e.Row));
            _loadedRows.Add(e.Row);
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
            Debug.Assert(_loadedRows.Contains(e.Row));
            _loadedRows.Remove(e.Row);
        }
    }

    /// <summary>
    ///     Scrolls the DataGrid according to the direction of the delta.
    /// </summary>
    /// <param name="e">PointerWheelEventArgs</param>
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        var delta = e.Delta;

        // KeyModifiers.Shift should scroll in horizontal direction. This does not work on every platform.
        // If Shift-Key is pressed and X is close to 0 we swap the Vector.
        if (e.KeyModifiers == KeyModifiers.Shift && MathUtilities.IsZero(delta.X)) delta = new Vector(delta.Y, delta.X);

        if (UpdateScroll(delta * DATAGRID_mouseWheelDelta))
            e.Handled = true;
        else
            e.Handled = e.Handled || !ScrollViewer.GetIsScrollChainingEnabled(this);
    }

    internal bool UpdateScroll(Vector delta)
    {
        if (IsEnabled && DisplayData.NumDisplayedScrollingElements > 0)
        {
            var handled = false;
            var ignoreInvalidate = false;
            var scrollHeight = 0d;

            // Vertical scroll handling
            if (delta.Y > 0)
            {
                scrollHeight = Math.Max(-_verticalOffset, -delta.Y);
            }
            else if (delta.Y < 0)
            {
                if (VerticalScrollBar != null && VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                {
                    scrollHeight = Math.Min(Math.Max(0, VerticalScrollBar.Maximum - _verticalOffset), -delta.Y);
                }
                else
                {
                    var maximum = EdgedRowsHeightCalculated - CellsEstimatedHeight;
                    scrollHeight = Math.Min(Math.Max(0, maximum - _verticalOffset), -delta.Y);
                }
            }

            if (Math.Abs(scrollHeight) > LayoutHelper.LayoutEpsilon)
            {
                DisplayData.PendingVerticalScrollHeight = scrollHeight;
                handled = true;

                var eventType = scrollHeight > 0 ? ScrollEventType.SmallIncrement : ScrollEventType.SmallDecrement;
                VerticalScroll?.Invoke(this, new ScrollEventArgs(eventType, scrollHeight));
            }

            // Horizontal scroll handling
            if (delta.X != 0)
            {
                var horizontalOffset = HorizontalOffset - delta.X;
                var widthNotVisible = Math.Max(0, ColumnsInternal.VisibleEdgedColumnsWidth - CellsWidth);

                if (horizontalOffset < 0) horizontalOffset = 0;
                if (horizontalOffset > widthNotVisible) horizontalOffset = widthNotVisible;

                if (UpdateHorizontalOffset(horizontalOffset))
                {
                    // We don't need to invalidate once again after UpdateHorizontalOffset.
                    ignoreInvalidate = true;
                    handled = true;

                    var eventType = horizontalOffset > 0
                        ? ScrollEventType.SmallIncrement
                        : ScrollEventType.SmallDecrement;
                    HorizontalScroll?.Invoke(this, new ScrollEventArgs(eventType, horizontalOffset));
                }
            }

            if (handled)
            {
                if (!ignoreInvalidate) InvalidateRowsMeasure(false);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Raises the PreparingCellForEdit event.
    /// </summary>
    protected virtual void OnPreparingCellForEdit(DataGridPreparingCellForEditEventArgs e)
    {
        PreparingCellForEdit?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the RowEditEnded event.
    /// </summary>
    protected virtual void OnRowEditEnded(DataGridRowEditEndedEventArgs e)
    {
        RowEditEnded?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the RowEditEnding event.
    /// </summary>
    protected virtual void OnRowEditEnding(DataGridRowEditEndingEventArgs e)
    {
        RowEditEnding?.Invoke(this, e);
    }

    /// <summary>
    ///     Raises the SelectionChanged event and clears the _selectionChanged.
    ///     This event won't get raised again until after _selectionChanged is set back to true.
    /// </summary>
    protected virtual void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        RaiseEvent(e);
    }

    /// <summary>
    ///     Raises the UnloadingRow event for row recycling.
    /// </summary>
    protected virtual void OnUnloadingRow(DataGridRowEventArgs e)
    {
        var handler = UnloadingRow;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    ///     Builds the visual tree for the column header when a new template is applied.
    /// </summary>
    //TODO Validation UI
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        // The template has changed, so we need to refresh the visuals
        _measured = false;

        if (ColumnHeaders != null)
            // If we're applying a new template, we want to remove the old column headers first
            ColumnHeaders.Children.Clear();

        ColumnHeaders = e.NameScope.Find<DataGridColumnHeadersPresenter>(DATAGRID_elementColumnHeadersPresenterName);

        if (ColumnHeaders != null)
        {
            if (ColumnsInternal.FillerColumn != null) ColumnsInternal.FillerColumn.IsRepresented = false;
            ColumnHeaders.OwningGrid = this;

            // Columns were added before our Template was applied, add the ColumnHeaders now
            var sortedInternal = new List<DataGridColumn>(ColumnsItemsInternal);
            sortedInternal.Sort(new DisplayIndexComparer());
            foreach (var column in sortedInternal) InsertDisplayedColumnHeader(column);
        }

        if (_rowsPresenter != null)
            // If we're applying a new template, we want to remove the old rows first
            UnloadElements(false);

        _rowsPresenter = e.NameScope.Find<DataGridRowsPresenter>(DATAGRID_elementRowsPresenterName);

        if (_rowsPresenter != null)
        {
            _rowsPresenter.OwningGrid = this;
            InvalidateRowHeightEstimate();
            UpdateRowDetailsHeightEstimate();
        }

        _frozenColumnScrollBarSpacer = e.NameScope.Find<Control>(DATAGRID_elementFrozenColumnScrollBarSpacerName);
        _rightFrozenScrollBarMargin = null;

        if (HorizontalScrollBar != null) HorizontalScrollBar.Scroll -= HorizontalScrollBar_Scroll;

        HorizontalScrollBar = e.NameScope.Find<ScrollBar>(DATAGRID_elementHorizontalScrollbarName);

        if (HorizontalScrollBar != null)
        {
            HorizontalScrollBar.IsTabStop = false;
            HorizontalScrollBar.Maximum = 0.0;
            HorizontalScrollBar.Orientation = Orientation.Horizontal;
            HorizontalScrollBar.IsVisible = false;
            HorizontalScrollBar.Scroll += HorizontalScrollBar_Scroll;
        }

        if (VerticalScrollBar != null) VerticalScrollBar.Scroll -= VerticalScrollBar_Scroll;

        VerticalScrollBar = e.NameScope.Find<ScrollBar>(DATAGRID_elementVerticalScrollbarName);

        if (VerticalScrollBar != null)
        {
            VerticalScrollBar.IsTabStop = false;
            VerticalScrollBar.Maximum = 0.0;
            VerticalScrollBar.Orientation = Orientation.Vertical;
            VerticalScrollBar.IsVisible = false;
            VerticalScrollBar.Scroll += VerticalScrollBar_Scroll;
        }

        _topLeftCornerHeader = e.NameScope.Find<ContentControl>(DATAGRID_elementTopLeftCornerHeaderName);
        EnsureTopLeftCornerHeader(); // EnsureTopLeftCornerHeader checks for a null _topLeftCornerHeader;
        _topRightCornerHeader = e.NameScope.Find<ContentControl>(DATAGRID_elementTopRightCornerHeaderName);
        _bottomRightCorner = e.NameScope.Find<Visual>(DATAGRID_elementBottomRightCornerHeaderName);
    }

    /// <summary>
    ///     Cancels editing mode for the specified DataGridEditingUnit and restores its original value.
    /// </summary>
    /// <param name="editingUnit">Specifies whether to cancel edit for a Cell or Row.</param>
    /// <param name="raiseEvents">Specifies whether or not to raise editing events</param>
    /// <returns>True if operation was successful. False otherwise.</returns>
    internal bool CancelEdit(DataGridEditingUnit editingUnit, bool raiseEvents)
    {
        if (!EndCellEdit(
                DataGridEditAction.Cancel,
                true,
                ContainsFocus,
                raiseEvents))
            return false;

        if (editingUnit == DataGridEditingUnit.Row) return EndRowEdit(DataGridEditAction.Cancel, true, raiseEvents);

        return true;
    }

    /// <summary>
    ///     call when: selection changes or SelectedItems object changes
    /// </summary>
    internal void CoerceSelectedItem()
    {
        object selectedItem = null;

        if (SelectionMode == DataGridSelectionMode.Extended &&
            CurrentSlot != -1 &&
            _selectedItems.ContainsSlot(CurrentSlot))
            selectedItem = CurrentItem;
        else if (_selectedItems.Count > 0) selectedItem = _selectedItems[0];

        SetValueNoCallback(SelectedItemProperty, selectedItem);

        // Update the SelectedIndex
        var newIndex = -1;

        if (selectedItem != null) newIndex = DataConnection.IndexOf(selectedItem);

        SetValueNoCallback(SelectedIndexProperty, newIndex);
    }

    internal static DataGridCell GetOwningCell(Control element)
    {
        Debug.Assert(element != null);
        var cell = element as DataGridCell;
        while (element != null && cell == null)
        {
            element = element.Parent as Control;
            cell = element as DataGridCell;
        }

        return cell;
    }

    internal IEnumerable<object> GetSelectionInclusive(int startRowIndex, int endRowIndex)
    {
        var endSlot = SlotFromRowIndex(endRowIndex);
        foreach (var slot in _selectedItems.GetSlots(SlotFromRowIndex(startRowIndex)))
        {
            if (slot > endSlot) break;
            yield return DataConnection.GetDataItem(RowIndexFromSlot(slot));
        }
    }

    internal void InitializeElements(bool recycleRows)
    {
        try
        {
            _noCurrentCellChangeCount++;

            // The underlying collection has changed and our editing row (if there is one)
            // is no longer relevant, so we should force a cancel edit.
            CancelEdit(DataGridEditingUnit.Row, false);

            // We want to persist selection throughout a reset, so store away the selected items
            var selectedItemsCache = new List<object>(_selectedItems.SelectedItemsCache);

            var collapsedGroupsCache = RowGroupHeadersTable
                .Where(g => !g.Value.IsVisible)
                .Select(g => g.Value.CollectionViewGroup.Key)
                .ToArray();

            if (recycleRows)
                RefreshRows(recycleRows, true);
            else
                RefreshRowsAndColumns(true);

            // collapse previously collapsed groups
            foreach (var g in collapsedGroupsCache)
            {
                var item = RowGroupHeadersTable.FirstOrDefault(t =>
                    t.Value.CollectionViewGroup.Parent.GroupBy.KeysMatch(t.Value.CollectionViewGroup.Key, g));
                if (item != null) EnsureRowGroupVisibility(item.Value, false, false);
            }

            // Re-select the old items
            _selectedItems.SelectedItemsCache = selectedItemsCache;
            CoerceSelectedItem();
            if (RowDetailsVisibilityMode != DataGridRowDetailsVisibilityMode.Collapsed)
                UpdateRowDetailsVisibilityMode(RowDetailsVisibilityMode);

            // The currently displayed rows may have incorrect visual states because of the selection change
            ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
        }
        finally
        {
            NoCurrentCellChangeCount--;
        }
    }

    internal bool IsDoubleClickRecordsClickOnCall(Control element)
    {
        if (_clickedElement == element)
        {
            _clickedElement = null;
            return true;
        }

        _clickedElement = element;
        return false;
    }

    // Returns the item or the CollectionViewGroup that is used as the DataContext for a given slot.
    // If the DataContext is an item, rowIndex is set to the index of the item within the collection
    internal object ItemFromSlot(int slot, ref int rowIndex)
    {
        if (RowGroupHeadersTable.Contains(slot)) return RowGroupHeadersTable.GetValueAt(slot)?.CollectionViewGroup;

        rowIndex = RowIndexFromSlot(slot);
        return DataConnection.GetDataItem(rowIndex);
    }

    internal bool ProcessDownKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessDownKeyInternal(shift, ctrl);
    }

    internal bool ProcessEndKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessEndKey(shift, ctrl);
    }

    internal bool ProcessEnterKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessEnterKey(shift, ctrl);
    }

    internal bool ProcessHomeKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessHomeKey(shift, ctrl);
    }

    internal void ProcessHorizontalScroll(ScrollEventType scrollEventType)
    {
        if (_horizontalScrollChangesIgnored > 0) return;

        // If the user scrolls with the buttons, we need to update the new value of the scroll bar since we delay
        // this calculation.  If they scroll in another other way, the scroll bar's correct value has already been set
        double scrollBarValueDifference = 0;
        if (scrollEventType == ScrollEventType.SmallIncrement)
            scrollBarValueDifference = GetHorizontalSmallScrollIncrease();
        else if (scrollEventType == ScrollEventType.SmallDecrement)
            scrollBarValueDifference = -GetHorizontalSmallScrollDecrease();
        _horizontalScrollChangesIgnored++;
        try
        {
            if (scrollBarValueDifference != 0)
            {
                Debug.Assert(_horizontalOffset + scrollBarValueDifference >= 0);
                HorizontalScrollBar.Value = _horizontalOffset + scrollBarValueDifference;
            }

            UpdateHorizontalOffset(HorizontalScrollBar.Value);
        }
        finally
        {
            _horizontalScrollChangesIgnored--;
        }
    }

    internal bool ProcessLeftKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessLeftKey(shift, ctrl);
    }

    internal bool ProcessNextKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessNextKey(shift, ctrl);
    }

    internal bool ProcessPriorKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessPriorKey(shift, ctrl);
    }

    internal bool ProcessRightKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessRightKey(shift, ctrl);
    }

    /// <summary>
    ///     Selects items and updates currency based on parameters
    /// </summary>
    /// <param name="columnIndex">column index to make current</param>
    /// <param name="item">data item or CollectionViewGroup to make current</param>
    /// <param name="backupSlot">slot to use in case the item is no longer valid</param>
    /// <param name="action">selection action to perform</param>
    /// <param name="scrollIntoView">whether or not the new current item should be scrolled into view</param>
    internal void ProcessSelectionAndCurrency(int columnIndex, object item, int backupSlot,
        DataGridSelectionAction action, bool scrollIntoView)
    {
        _noSelectionChangeCount++;
        _noCurrentCellChangeCount++;
        try
        {
            var slot = -1;
            if (item is DataGridCollectionViewGroup group)
            {
                var groupInfo = RowGroupInfoFromCollectionViewGroup(group);
                if (groupInfo != null) slot = groupInfo.Slot;
            }
            else
            {
                slot = SlotFromRowIndex(DataConnection.IndexOf(item));
            }

            if (slot == -1) slot = backupSlot;
            if (slot < 0 || slot > SlotCount) return;

            switch (action)
            {
                case DataGridSelectionAction.AddCurrentToSelection:
                    SetRowSelection(slot, true, true);
                    break;
                case DataGridSelectionAction.RemoveCurrentFromSelection:
                    SetRowSelection(slot, false, false);
                    break;
                case DataGridSelectionAction.SelectFromAnchorToCurrent:
                    if (SelectionMode == DataGridSelectionMode.Extended && AnchorSlot != -1)
                    {
                        var anchorSlot = AnchorSlot;
                        if (slot <= anchorSlot)
                            SetRowsSelection(slot, anchorSlot);
                        else
                            SetRowsSelection(anchorSlot, slot);
                    }
                    else
                    {
                        goto case DataGridSelectionAction.SelectCurrent;
                    }

                    break;
                case DataGridSelectionAction.SelectCurrent:
                    ClearRowSelection(slot, true);
                    break;
                case DataGridSelectionAction.None:
                    break;
            }

            if (CurrentSlot != slot || (CurrentColumnIndex != columnIndex && columnIndex != -1))
            {
                if (columnIndex == -1)
                {
                    if (CurrentColumnIndex != -1)
                    {
                        columnIndex = CurrentColumnIndex;
                    }
                    else
                    {
                        var firstVisibleColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
                        if (firstVisibleColumn != null) columnIndex = firstVisibleColumn.Index;
                    }
                }

                if (columnIndex != -1)
                    if (!SetCurrentCellCore(
                            columnIndex, slot,
                            true,
                            SlotFromRowIndex(SelectedIndex) != slot)
                        || (scrollIntoView &&
                            !ScrollSlotIntoView(
                                columnIndex, slot,
                                true,
                                false)))
                        return;
            }

            _successfullyUpdatedSelection = true;
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }
    }

    internal bool ProcessUpKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessUpKey(shift, ctrl);
    }

    //internal void ProcessVerticalScroll(double oldValue, double newValue)
    internal void ProcessVerticalScroll(ScrollEventType scrollEventType)
    {
        if (_verticalScrollChangesIgnored > 0) return;
        Debug.Assert(MathUtilities.LessThanOrClose(VerticalScrollBar.Value, VerticalScrollBar.Maximum));

        _verticalScrollChangesIgnored++;
        try
        {
            Debug.Assert(VerticalScrollBar != null);
            if (scrollEventType == ScrollEventType.SmallIncrement)
            {
                DisplayData.PendingVerticalScrollHeight = GetVerticalSmallScrollIncrease();
                var newVerticalOffset = _verticalOffset + DisplayData.PendingVerticalScrollHeight;
                if (newVerticalOffset > VerticalScrollBar.Maximum)
                    DisplayData.PendingVerticalScrollHeight -= newVerticalOffset - VerticalScrollBar.Maximum;
            }
            else if (scrollEventType == ScrollEventType.SmallDecrement)
            {
                if (MathUtilities.GreaterThan(NegVerticalOffset, 0))
                {
                    DisplayData.PendingVerticalScrollHeight -= NegVerticalOffset;
                }
                else
                {
                    var previousScrollingSlot = GetPreviousVisibleSlot(DisplayData.FirstScrollingSlot);
                    if (previousScrollingSlot >= 0) ScrollSlotIntoView(previousScrollingSlot, false);
                    return;
                }
            }
            else
            {
                DisplayData.PendingVerticalScrollHeight = VerticalScrollBar.Value - _verticalOffset;
            }

            if (!MathUtilities.IsZero(DisplayData.PendingVerticalScrollHeight))
                // Invalidate so the scroll happens on idle
                InvalidateRowsMeasure(false);
        }
        finally
        {
            _verticalScrollChangesIgnored--;
        }
    }

    internal void RefreshRowsAndColumns(bool clearRows)
    {
        if (_measured)
        {
            try
            {
                _noCurrentCellChangeCount++;

                if (clearRows)
                {
                    ClearRows(false);
                    ClearRowGroupHeadersTable();
                    PopulateRowGroupHeadersTable();
                }

                if (AutoGenerateColumns)
                    //Column auto-generation refreshes the rows too
                    AutoGenerateColumnsPrivate();
                foreach (var column in ColumnsItemsInternal)
                    //We don't need to refresh the state of AutoGenerated column headers because they're up-to-date
                    if (!column.IsAutoGenerated && column.HasHeaderCell)
                        column.HeaderCell.UpdatePseudoClasses();

                RefreshRows(false, false);

                if (Columns.Count > 0 && CurrentColumnIndex == -1)
                {
                    MakeFirstDisplayedCellCurrentCell();
                }
                else
                {
                    _makeFirstDisplayedCellCurrentCellPending = false;
                    _desiredCurrentColumnIndex = -1;
                    FlushCurrentCellChanged();
                }
            }
            finally
            {
                NoCurrentCellChangeCount--;
            }
        }
        else
        {
            if (clearRows) ClearRows(false);
            ClearRowGroupHeadersTable();
            PopulateRowGroupHeadersTable();
        }
    }

    internal bool ScrollSlotIntoView(int columnIndex, int slot, bool forCurrentCellChange, bool forceHorizontalScroll)
    {
        Debug.Assert(columnIndex >= 0 && columnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(DisplayData.FirstDisplayedScrollingCol >= -1 &&
                     DisplayData.FirstDisplayedScrollingCol < ColumnsItemsInternal.Count);
        Debug.Assert(DisplayData.LastTotallyDisplayedScrollingCol >= -1 &&
                     DisplayData.LastTotallyDisplayedScrollingCol < ColumnsItemsInternal.Count);
        Debug.Assert(!IsSlotOutOfBounds(slot));
        Debug.Assert(DisplayData.FirstScrollingSlot >= -1 && DisplayData.FirstScrollingSlot < SlotCount);
        Debug.Assert(ColumnsItemsInternal[columnIndex].IsVisible);

        if (CurrentColumnIndex >= 0 &&
            (CurrentColumnIndex != columnIndex || CurrentSlot != slot))
            if (!CommitEditForOperation(columnIndex, slot, forCurrentCellChange) ||
                IsInnerCellOutOfBounds(columnIndex, slot))
                return false;

        var oldHorizontalOffset = HorizontalOffset;

        //scroll horizontally unless we're on a RowGroupHeader and we're not forcing horizontal scrolling
        if ((forceHorizontalScroll || slot != -1)
            && !ScrollColumnIntoView(columnIndex))
            return false;

        //scroll vertically
        if (!ScrollSlotIntoView(slot, oldHorizontalOffset != HorizontalOffset)) return false;

        return true;
    }

    // Convenient overload that commits the current edit.
    internal bool SetCurrentCellCore(int columnIndex, int slot)
    {
        return SetCurrentCellCore(columnIndex, slot, true, true);
    }

    internal bool UpdateHorizontalOffset(double newValue)
    {
        if (HorizontalOffset != newValue)
        {
            HorizontalOffset = newValue;

            InvalidateColumnHeadersMeasure();
            InvalidateRowsMeasure(true);
            return true;
        }

        return false;
    }

    internal bool UpdateSelectionAndCurrency(int columnIndex, int slot, DataGridSelectionAction action,
        bool scrollIntoView)
    {
        _successfullyUpdatedSelection = false;

        _noSelectionChangeCount++;
        _noCurrentCellChangeCount++;
        try
        {
            if (ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                columnIndex == ColumnsInternal.RowGroupSpacerColumn.Index)
                columnIndex = -1;
            if (IsSlotOutOfSelectionBounds(slot) || (columnIndex != -1 && IsColumnOutOfBounds(columnIndex)))
                return false;

            var newCurrentPosition = -1;
            var item = ItemFromSlot(slot, ref newCurrentPosition);

            if (EditingRow != null && slot != EditingRow.Slot && !CommitEdit(DataGridEditingUnit.Row, true))
                return false;

            if (DataConnection.CollectionView != null &&
                DataConnection.CollectionView.CurrentPosition != newCurrentPosition)
                DataConnection.MoveCurrentTo(item, slot, columnIndex, action, scrollIntoView);
            else
                ProcessSelectionAndCurrency(columnIndex, item, slot, action, scrollIntoView);
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    internal void UpdateStateOnCurrentChanged(object currentItem, int currentPosition)
    {
        if (currentItem == CurrentItem && currentItem == SelectedItem && currentPosition == SelectedIndex)
            // The DataGrid's CurrentItem is already up-to-date, so we don't need to do anything
            return;

        var columnIndex = CurrentColumnIndex;
        if (columnIndex == -1)
        {
            if (IsColumnOutOfBounds(_desiredCurrentColumnIndex) ||
                (ColumnsInternal.RowGroupSpacerColumn.IsRepresented &&
                 _desiredCurrentColumnIndex == ColumnsInternal.RowGroupSpacerColumn.Index))
                columnIndex = FirstDisplayedNonFillerColumnIndex;
            else
                columnIndex = _desiredCurrentColumnIndex;
        }

        _desiredCurrentColumnIndex = -1;

        try
        {
            _noSelectionChangeCount++;
            _noCurrentCellChangeCount++;

            if (!CommitEdit()) CancelEdit(DataGridEditingUnit.Row, false);

            ClearRowSelection(true);
            if (currentItem == null)
            {
                SetCurrentCellCore(-1, -1);
            }
            else
            {
                var slot = SlotFromRowIndex(currentPosition);
                ProcessSelectionAndCurrency(columnIndex, currentItem, slot, DataGridSelectionAction.SelectCurrent,
                    false);
            }
        }
        finally
        {
            NoCurrentCellChangeCount--;
            NoSelectionChangeCount--;
        }
    }

    //TODO: Ensure right button is checked for
    internal bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex,
        int slot, bool allowEdit)
    {
        KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out var ctrl, out var shift);
        return UpdateStateOnMouseRightButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
    }

    //TODO: Ensure left button is checked for
    internal bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex,
        int slot, bool allowEdit)
    {
        KeyboardHelper.GetMetaKeyState(this, pointerPressedEventArgs.KeyModifiers, out var ctrl, out var shift);
        return UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl);
    }

    internal void UpdateVerticalScrollBar()
    {
        if (VerticalScrollBar != null && VerticalScrollBar.IsVisible)
        {
            var cellsHeight = CellsEstimatedHeight;
            var edgedRowsHeightCalculated = EdgedRowsHeightCalculated;
            UpdateVerticalScrollBar(
                edgedRowsHeightCalculated > cellsHeight,
                VerticalScrollBarVisibility == ScrollBarVisibility.Visible,
                edgedRowsHeightCalculated,
                cellsHeight);
        }
    }

    /// <summary>
    ///     If the editing element has focus, this method will set focus to the DataGrid itself
    ///     in order to force the element to lose focus.  It will then wait for the editing element's
    ///     LostFocus event, at which point it will perform the specified action.
    ///     NOTE: It is important to understand that the specified action will be performed when the editing
    ///     element loses focus only if this method returns true.  If it returns false, then the action
    ///     will not be performed later on, and should instead be performed by the caller, if necessary.
    /// </summary>
    /// <param name="action">Action to perform after the editing element loses focus</param>
    /// <returns>True if the editing element had focus and the action was cached away; false otherwise</returns>
    //TODO TabStop
    internal bool WaitForLostFocus(Action action)
    {
        if (EditingRow != null && EditingColumnIndex != -1 && !_executingLostFocusActions)
        {
            var editingColumn = ColumnsItemsInternal[EditingColumnIndex];
            var editingElement = editingColumn.GetCellContent(EditingRow);
            if (editingElement != null && editingElement.ContainsChild(_focusedObject))
            {
                Debug.Assert(_lostFocusActions != null);
                _lostFocusActions.Enqueue(action);
                editingElement.LostFocus += EditingElement_LostFocus;
                //IsTabStop = true;
                Focus();
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Raises the LoadingRowDetails for row details preparation
    /// </summary>
    protected virtual void OnLoadingRowDetails(DataGridRowDetailsEventArgs e)
    {
        var handler = LoadingRowDetails;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    ///     Raises the UnloadingRowDetails event
    /// </summary>
    protected virtual void OnUnloadingRowDetails(DataGridRowDetailsEventArgs e)
    {
        var handler = UnloadingRowDetails;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    internal void OnRowDetailsChanged()
    {
        if (!_scrollingByHeight)
            // Update layout when RowDetails are expanded or collapsed, just updating the vertical scroll bar is not enough
            // since rows could be added or removed
            InvalidateMeasure();
    }

    private void UpdateRowDetailsVisibilityMode(DataGridRowDetailsVisibilityMode newDetailsMode)
    {
        var itemCount = DataConnection.Count;
        if (_rowsPresenter != null && itemCount > 0)
        {
            var newDetailsVisibility = false;
            switch (newDetailsMode)
            {
                case DataGridRowDetailsVisibilityMode.Visible:
                    newDetailsVisibility = true;
                    _showDetailsTable.AddValues(0, itemCount, true);
                    break;
                case DataGridRowDetailsVisibilityMode.Collapsed:
                    newDetailsVisibility = false;
                    _showDetailsTable.AddValues(0, itemCount, false);
                    break;
                case DataGridRowDetailsVisibilityMode.VisibleWhenSelected:
                    _showDetailsTable.Clear();
                    break;
            }

            var updated = false;
            foreach (var row in GetAllRows())
                if (row.IsVisible)
                {
                    if (newDetailsMode == DataGridRowDetailsVisibilityMode.VisibleWhenSelected)
                        // For VisibleWhenSelected, we need to calculate the value for each individual row
                        newDetailsVisibility = _selectedItems.ContainsSlot(row.Slot);
                    if (row.AreDetailsVisible != newDetailsVisibility)
                    {
                        updated = true;

                        row.SetDetailsVisibilityInternal(newDetailsVisibility, true, false);
                    }
                }

            if (updated)
            {
                UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsEstimatedHeight);
                InvalidateRowsMeasure(false);
            }
        }
    }

    private void AddNewCellPrivate(DataGridRow row, DataGridColumn column)
    {
        var newCell = new DataGridCell();
        PopulateCellContent(
            false,
            column,
            row,
            newCell);
        if (row.OwningGrid != null)
        {
            newCell.OwningColumn = column;
            newCell.IsVisible = column.IsVisible;
            if (row.OwningGrid.CellTheme is { } cellTheme)
                newCell.SetValue(ThemeProperty, cellTheme, BindingPriority.Template);
        }

        row.Cells.Insert(column.Index, newCell);
    }

    private bool BeginCellEdit(RoutedEventArgs editingEventArgs)
    {
        if (CurrentColumnIndex == -1 || !GetRowSelection(CurrentSlot)) return false;

        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot);
        Debug.Assert(!GetColumnEffectiveReadOnlyState(CurrentColumn));
        Debug.Assert(CurrentColumn.IsVisible);

        if (EditingColumnIndex != -1)
        {
            // Current cell is already in edit mode
            Debug.Assert(EditingColumnIndex == CurrentColumnIndex);
            return true;
        }

        // Get or generate the editing row if it doesn't exist
        var dataGridRow = EditingRow;
        if (dataGridRow == null)
        {
            if (IsSlotVisible(CurrentSlot))
            {
                dataGridRow = DisplayData.GetDisplayedElement(CurrentSlot) as DataGridRow;
                Debug.Assert(dataGridRow != null);
            }
            else
            {
                dataGridRow = GenerateRow(RowIndexFromSlot(CurrentSlot), CurrentSlot);
            }
        }

        Debug.Assert(dataGridRow != null);

        // Cache these to see if they change later
        var currentRowIndex = CurrentSlot;
        var currentColumnIndex = CurrentColumnIndex;

        // Raise the BeginningEdit event
        var dataGridCell = dataGridRow.Cells[CurrentColumnIndex];
        var e = new DataGridBeginningEditEventArgs(CurrentColumn, dataGridRow, editingEventArgs);
        OnBeginningEdit(e);
        if (e.Cancel
            || currentRowIndex != CurrentSlot
            || currentColumnIndex != CurrentColumnIndex
            || !GetRowSelection(CurrentSlot)
            || (EditingRow == null && !BeginRowEdit(dataGridRow)))
            // If either BeginningEdit was canceled, currency/selection was changed in the event handler,
            // or we failed opening the row for edit, then we can no longer continue BeginCellEdit
            return false;
        Debug.Assert(EditingRow != null);
        Debug.Assert(EditingRow.Slot == CurrentSlot);

        // Finally, we can prepare the cell for editing
        EditingColumnIndex = CurrentColumnIndex;
        _editingEventArgs = editingEventArgs;
        EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
        PopulateCellContent(
            true,
            CurrentColumn,
            dataGridRow,
            dataGridCell);
        return true;
    }

    //TODO Validation
    private bool BeginRowEdit(DataGridRow dataGridRow)
    {
        Debug.Assert(EditingRow == null);
        Debug.Assert(dataGridRow != null);

        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);

        if (DataConnection.BeginEdit(dataGridRow.DataContext))
        {
            EditingRow = dataGridRow;
            GenerateEditingElements();
            return true;
        }

        return false;
    }

    private bool CancelRowEdit(bool exitEditingMode)
    {
        if (EditingRow == null) return true;
        Debug.Assert(EditingRow != null && EditingRow.Index >= -1);
        Debug.Assert(EditingRow.Slot < SlotCount);
        Debug.Assert(CurrentColumn != null);

        var dataItem = EditingRow.DataContext;
        if (!DataConnection.CancelEdit(dataItem)) return false;
        foreach (var column in Columns)
        {
            if (!exitEditingMode && column.Index == EditingColumnIndex && column is DataGridBoundColumn) continue;
            PopulateCellContent(
                !exitEditingMode && column.Index == EditingColumnIndex,
                column,
                EditingRow,
                EditingRow.Cells[column.Index]);
        }

        return true;
    }

    private bool CommitEditForOperation(int columnIndex, int slot, bool forCurrentCellChange)
    {
        if (forCurrentCellChange)
        {
            if (!EndCellEdit(DataGridEditAction.Commit, true, true, true)) return false;
            if (CurrentSlot != slot &&
                !EndRowEdit(DataGridEditAction.Commit, true, true))
                return false;
        }

        if (IsColumnOutOfBounds(columnIndex)) return false;
        if (slot >= SlotCount)
        {
            // Current cell was reset because the commit deleted row(s).
            // Since the user wants to change the current cell, we don't
            // want to end up with no current cell. We pick the last row
            // in the grid which may be the 'new row'.
            var lastSlot = LastVisibleSlot;
            if (forCurrentCellChange &&
                CurrentColumnIndex == -1 &&
                lastSlot != -1)
                SetAndSelectCurrentCell(columnIndex, lastSlot, false);
            // Interrupt operation because it has become invalid.
            return false;
        }

        return true;
    }

    //TODO Validation
    private bool CommitRowEdit(bool exitEditingMode)
    {
        if (EditingRow == null) return true;
        Debug.Assert(EditingRow != null && EditingRow.Index >= -1);
        Debug.Assert(EditingRow.Slot < SlotCount);

        //if (!ValidateEditingRow(scrollIntoView: true, wireEvents: false))
        if (!EditingRow.IsValid) return false;

        DataConnection.EndEdit(EditingRow.DataContext);

        if (!exitEditingMode) DataConnection.BeginEdit(EditingRow.DataContext);
        return true;
    }

    private void CompleteCellsCollection(DataGridRow dataGridRow)
    {
        Debug.Assert(dataGridRow != null);
        var cellsInCollection = dataGridRow.Cells.Count;
        if (ColumnsItemsInternal.Count > cellsInCollection)
            for (var columnIndex = cellsInCollection; columnIndex < ColumnsItemsInternal.Count; columnIndex++)
                AddNewCellPrivate(dataGridRow, ColumnsItemsInternal[columnIndex]);
    }

    private void ComputeScrollBarsLayout()
    {
        if (_ignoreNextScrollBarsLayout) _ignoreNextScrollBarsLayout = false;
        //
        var isHorizontalScrollBarOverCells = IsHorizontalScrollBarOverCells;
        var isVerticalScrollBarOverCells = IsVerticalScrollBarOverCells;

        var cellsWidth = CellsWidth;
        var cellsHeight = CellsEstimatedHeight;

        var allowHorizScrollbar = false;
        var forceHorizScrollbar = false;
        double horizScrollBarHeight = 0;
        if (HorizontalScrollBar != null)
        {
            forceHorizScrollbar = HorizontalScrollBarVisibility == ScrollBarVisibility.Visible;
            allowHorizScrollbar = forceHorizScrollbar || (ColumnsInternal.VisibleColumnCount > 0 &&
                                                          HorizontalScrollBarVisibility !=
                                                          ScrollBarVisibility.Disabled &&
                                                          HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden);
            // Compensate if the horizontal scrollbar is already taking up space
            if (!forceHorizScrollbar && HorizontalScrollBar.IsVisible)
                if (!isHorizontalScrollBarOverCells)
                    cellsHeight += HorizontalScrollBar.DesiredSize.Height;

            if (!isHorizontalScrollBarOverCells)
                horizScrollBarHeight = HorizontalScrollBar.Height + HorizontalScrollBar.Margin.Top +
                                       HorizontalScrollBar.Margin.Bottom;
        }

        var allowVertScrollbar = false;
        var forceVertScrollbar = false;
        double vertScrollBarWidth = 0;
        if (VerticalScrollBar != null)
        {
            forceVertScrollbar = VerticalScrollBarVisibility == ScrollBarVisibility.Visible;
            allowVertScrollbar = forceVertScrollbar || (ColumnsItemsInternal.Count > 0 &&
                                                        VerticalScrollBarVisibility != ScrollBarVisibility.Disabled &&
                                                        VerticalScrollBarVisibility != ScrollBarVisibility.Hidden);
            // Compensate if the vertical scrollbar is already taking up space
            if (!forceVertScrollbar && VerticalScrollBar.IsVisible)
                if (!isVerticalScrollBarOverCells)
                    cellsWidth += VerticalScrollBar.DesiredSize.Width;

            if (!isVerticalScrollBarOverCells)
                vertScrollBarWidth = VerticalScrollBar.Width + VerticalScrollBar.Margin.Left +
                                     VerticalScrollBar.Margin.Right;
        }

        // Now cellsWidth is the width potentially available for displaying data cells.
        // Now cellsHeight is the height potentially available for displaying data cells.

        var needHorizScrollbar = false;
        var needVertScrollbar = false;

        var totalVisibleWidth = ColumnsInternal.VisibleEdgedColumnsWidth;
        var totalVisibleFrozenWidth = ColumnsInternal.GetVisibleFrozenEdgedColumnsWidth();

        UpdateDisplayedRows(DisplayData.FirstScrollingSlot, CellsEstimatedHeight);
        var totalVisibleHeight = EdgedRowsHeightCalculated;

        if (!forceHorizScrollbar && !forceVertScrollbar)
        {
            var needHorizScrollbarWithoutVertScrollbar = false;

            if (allowHorizScrollbar &&
                MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight))
            {
                var oldDataHeight = cellsHeight;
                cellsHeight -= horizScrollBarHeight;
                Debug.Assert(cellsHeight >= 0);
                needHorizScrollbarWithoutVertScrollbar = needHorizScrollbar = true;

                if (vertScrollBarWidth > 0 &&
                    allowVertScrollbar &&
                    (MathUtilities.LessThanOrClose(totalVisibleWidth - cellsWidth, vertScrollBarWidth) ||
                     MathUtilities.LessThanOrClose(cellsWidth - totalVisibleFrozenWidth, vertScrollBarWidth)))
                {
                    // Would we still need a horizontal scrollbar without the vertical one?
                    UpdateDisplayedRows(DisplayData.FirstScrollingSlot, cellsHeight);
                    if (DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                        needHorizScrollbar =
                            MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth - vertScrollBarWidth);
                }

                if (!needHorizScrollbar)
                    // Restore old data height because turns out a horizontal scroll bar wouldn't make sense
                    cellsHeight = oldDataHeight;
            }

            // Store the current FirstScrollingSlot because removing the horizontal scrollbar could scroll
            // the DataGrid up; however, if we realize later that we need to keep the horizontal scrollbar
            // then we should use the first slot stored here which is not scrolled.
            var firstScrollingSlot = DisplayData.FirstScrollingSlot;

            UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
            if (allowVertScrollbar &&
                MathUtilities.GreaterThan(cellsHeight, 0) &&
                MathUtilities.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
            {
                cellsWidth -= vertScrollBarWidth;
                Debug.Assert(cellsWidth >= 0);
                needVertScrollbar = true;
            }

            DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();

            // we compute the number of visible columns only after we set up the vertical scroll bar.
            ComputeDisplayedColumns();

            if ((vertScrollBarWidth > 0 || horizScrollBarHeight > 0) &&
                allowHorizScrollbar &&
                needVertScrollbar && !needHorizScrollbar &&
                MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth) &&
                MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight))
            {
                cellsWidth += vertScrollBarWidth;
                cellsHeight -= horizScrollBarHeight;
                Debug.Assert(cellsHeight >= 0);
                needVertScrollbar = false;

                UpdateDisplayedRows(firstScrollingSlot, cellsHeight);
                if (cellsHeight > 0 &&
                    vertScrollBarWidth <= cellsWidth &&
                    DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }

                if (needVertScrollbar)
                    needHorizScrollbar = true;
                else
                    needHorizScrollbar = needHorizScrollbarWithoutVertScrollbar;
            }
        }
        else if (forceHorizScrollbar && !forceVertScrollbar)
        {
            if (allowVertScrollbar)
            {
                if (cellsHeight > 0 &&
                    MathUtilities.LessThanOrClose(vertScrollBarWidth, cellsWidth) &&
                    DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount)
                {
                    cellsWidth -= vertScrollBarWidth;
                    Debug.Assert(cellsWidth >= 0);
                    needVertScrollbar = true;
                }

                DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
            }

            needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
        }
        else if (!forceHorizScrollbar && forceVertScrollbar)
        {
            if (allowHorizScrollbar)
            {
                if (cellsWidth > 0 &&
                    MathUtilities.LessThanOrClose(horizScrollBarHeight, cellsHeight) &&
                    MathUtilities.GreaterThan(totalVisibleWidth, cellsWidth) &&
                    MathUtilities.LessThan(totalVisibleFrozenWidth, cellsWidth))
                {
                    cellsHeight -= horizScrollBarHeight;
                    Debug.Assert(cellsHeight >= 0);
                    needHorizScrollbar = true;
                    UpdateDisplayedRows(DisplayData.FirstScrollingSlot, cellsHeight);
                }

                DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
                ComputeDisplayedColumns();
            }

            needVertScrollbar = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
        }
        else
        {
            Debug.Assert(forceHorizScrollbar && forceVertScrollbar);
            Debug.Assert(allowHorizScrollbar && allowVertScrollbar);
            DisplayData.FirstDisplayedScrollingCol = ComputeFirstVisibleScrollingColumn();
            ComputeDisplayedColumns();
            needVertScrollbar = DisplayData.NumTotallyDisplayedScrollingElements != VisibleSlotCount;
            needHorizScrollbar = totalVisibleWidth > cellsWidth && totalVisibleFrozenWidth < cellsWidth;
        }

        UpdateHorizontalScrollBar(needHorizScrollbar, forceHorizScrollbar, totalVisibleWidth, totalVisibleFrozenWidth,
            cellsWidth);
        UpdateVerticalScrollBar(needVertScrollbar, forceVertScrollbar, totalVisibleHeight, cellsHeight);

        if (_topRightCornerHeader != null)
        {
            // Show the TopRightHeaderCell based on vertical ScrollBar visibility
            if (AreColumnHeadersVisible &&
                VerticalScrollBar != null && VerticalScrollBar.IsVisible)
                _topRightCornerHeader.IsVisible = true;
            else
                _topRightCornerHeader.IsVisible = false;
        }

        if (_bottomRightCorner != null)
            // Show the BottomRightCorner when both scrollbars are visible.
            _bottomRightCorner.IsVisible =
                HorizontalScrollBar != null && HorizontalScrollBar.IsVisible &&
                VerticalScrollBar != null && VerticalScrollBar.IsVisible;

        DisplayData.FullyRecycleElements();
    }

    /// <summary>
    ///     Handles the current editing element's LostFocus event by performing any actions that
    ///     were cached by the WaitForLostFocus method.
    /// </summary>
    /// <param name="sender">Editing element</param>
    /// <param name="e">RoutedEventArgs</param>
    private void EditingElement_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is Control editingElement)
        {
            editingElement.LostFocus -= EditingElement_LostFocus;
            if (EditingRow != null && EditingColumnIndex != -1) FocusEditingCell(true);
            Debug.Assert(_lostFocusActions != null);
            try
            {
                _executingLostFocusActions = true;
                while (_lostFocusActions.Count > 0) _lostFocusActions.Dequeue()();
            }
            finally
            {
                _executingLostFocusActions = false;
            }
        }
    }

    // Makes sure horizontal layout is updated to reflect any changes that affect it
    private void EnsureHorizontalLayout()
    {
        ColumnsInternal.EnsureVisibleEdgedColumnsWidth();
        InvalidateColumnHeadersMeasure();
        InvalidateRowsMeasure(true);
        InvalidateMeasure();
    }

    private void EnsureRowHeaderWidth()
    {
        if (AreRowHeadersVisible)
        {
            if (AreColumnHeadersVisible) EnsureTopLeftCornerHeader();

            if (_rowsPresenter != null)
            {
                var updated = false;

                foreach (var element in _rowsPresenter.Children)
                    if (element is DataGridRow row)
                    {
                        // If the RowHeader resulted in a different width the last time it was measured, we need
                        // to re-measure it
                        if (row.HeaderCell != null && row.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
                        {
                            row.HeaderCell.InvalidateMeasure();
                            updated = true;
                        }
                    }
                    else if (element is DataGridRowGroupHeader groupHeader && groupHeader.HeaderCell != null &&
                             groupHeader.HeaderCell.DesiredSize.Width != ActualRowHeaderWidth)
                    {
                        groupHeader.HeaderCell.InvalidateMeasure();
                        updated = true;
                    }

                if (updated)
                    // We need to update the width of the horizontal scrollbar if the rowHeaders' width actually changed
                    InvalidateMeasure();
            }
        }
    }

    private void EnsureRowsPresenterVisibility()
    {
        if (_rowsPresenter != null)
            // RowCount doesn't need to be considered, doing so might cause extra Visibility changes
            _rowsPresenter.IsVisible = ColumnsInternal.FirstVisibleNonFillerColumn != null;
    }

    private void EnsureTopLeftCornerHeader()
    {
        if (_topLeftCornerHeader != null)
        {
            _topLeftCornerHeader.IsVisible = HeadersVisibility == DataGridHeadersVisibility.All;

            if (_topLeftCornerHeader.IsVisible)
            {
                if (!double.IsNaN(RowHeaderWidth))
                    // RowHeaderWidth is set explicitly so we should use that
                    _topLeftCornerHeader.Width = RowHeaderWidth;
                else if (VisibleSlotCount > 0)
                    // RowHeaders AutoSize and we have at least 1 row so take the desired width
                    _topLeftCornerHeader.Width = RowHeadersDesiredWidth;
            }
        }
    }

    private void InvalidateCellsArrange()
    {
        foreach (var row in GetAllRows()) row.InvalidateHorizontalArrange();
    }

    private void InvalidateColumnHeadersArrange()
    {
        if (ColumnHeaders != null) ColumnHeaders.InvalidateArrange();
    }

    private void InvalidateColumnHeadersMeasure()
    {
        if (ColumnHeaders != null)
        {
            EnsureColumnHeadersVisibility();
            ColumnHeaders.InvalidateMeasure();
        }
    }

    private void InvalidateRowsArrange()
    {
        if (_rowsPresenter != null) _rowsPresenter.InvalidateArrange();
    }

    private void InvalidateRowsMeasure(bool invalidateIndividualElements)
    {
        if (_rowsPresenter != null)
        {
            _rowsPresenter.InvalidateMeasure();

            if (invalidateIndividualElements)
                foreach (var element in _rowsPresenter.Children)
                    element.InvalidateMeasure();
        }
    }

    //TODO: Make override?
    private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
    {
        if (!ContainsFocus)
        {
            ContainsFocus = true;
            ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
            if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot))
                if (DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow row)
                    row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
        }

        // Keep track of which row contains the newly focused element
        DataGridRow focusedRow = null;
        var focusedElement = e.Source as Visual;
        _focusedObject = focusedElement;
        while (focusedElement != null)
        {
            focusedRow = focusedElement as DataGridRow;
            if (focusedRow != null && focusedRow.OwningGrid == this && _focusedRow != focusedRow)
            {
                ResetFocusedRow();
                _focusedRow = focusedRow.IsVisible ? focusedRow : null;
                break;
            }

            focusedElement = focusedElement.GetVisualParent();
        }
    }

    //TODO: Check
    private void DataGrid_IsEnabledChanged(AvaloniaPropertyChangedEventArgs e)
    {
    }

    private void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (!e.Handled) e.Handled = ProcessDataGridKey(e);
    }

    private void DataGrid_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab && CurrentColumnIndex != -1 && e.Source == this)
        {
            var success =
                ScrollSlotIntoView(
                    CurrentColumnIndex, CurrentSlot,
                    false,
                    true);
            Debug.Assert(success);
            if (CurrentColumnIndex != -1 && SelectedItem == null) SetRowSelection(CurrentSlot, true, true);
        }
    }

    //TODO: Make override?
    private void DataGrid_LostFocus(object sender, RoutedEventArgs e)
    {
        _focusedObject = null;
        if (ContainsFocus)
        {
            var focusedObject = TopLevel.GetTopLevel(this)?.FocusManager.GetFocusedElement() as Visual;
            var root = focusedObject?.GetVisualRoot();
            var isPopup = root != null && root != this.GetVisualRoot();
            // Returning focus from an editor to the grid itself stays inside the grid.
            var focusLeftDataGrid = !ReferenceEquals(focusedObject, this) && !this.IsVisualAncestorOf(focusedObject);

            DataGridColumn editingColumn = null;

            if (EditingRow != null && EditingColumnIndex != -1)
                editingColumn = ColumnsItemsInternal[EditingColumnIndex];

            if (focusLeftDataGrid && editingColumn is not DataGridTemplateColumn && !isPopup)
            {
                ContainsFocus = false;
                if (EditingRow != null) CommitEdit(DataGridEditingUnit.Row, true);
                ResetFocusedRow();
                ApplyDisplayedRowsState(DisplayData.FirstScrollingSlot, DisplayData.LastScrollingSlot);
                if (CurrentColumnIndex != -1 && IsSlotVisible(CurrentSlot))
                    if (DisplayData.GetDisplayedElement(CurrentSlot) is DataGridRow row)
                        row.Cells[CurrentColumnIndex].UpdatePseudoClasses();
            }
            else if (focusLeftDataGrid)
            {
                if (focusedObject is Control focusedElement)
                    focusedElement.LostFocus += ExternalEditingElement_LostFocus;
            }
        }
    }

    private void EditingElement_Initialized(object sender, EventArgs e)
    {
        var element = sender as Control;
        if (element != null) element.Initialized -= EditingElement_Initialized;
        PreparingCellForEditPrivate(element);
    }

    //TODO Validation
    //TODO Binding
    //TODO TabStop
    private bool EndCellEdit(DataGridEditAction editAction, bool exitEditingMode, bool keepFocus, bool raiseEvents)
    {
        if (EditingColumnIndex == -1) return true;

        var editingRow = EditingRow;
        if (editingRow is null) return true;

        Debug.Assert(EditingColumnIndex >= 0);
        Debug.Assert(EditingColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(EditingColumnIndex == CurrentColumnIndex);

        // Cache these to see if they change later
        var currentSlot = CurrentSlot;
        var currentColumnIndex = CurrentColumnIndex;

        // We're ready to start ending, so raise the event
        var editingCell = editingRow.Cells[EditingColumnIndex];
        var editingElement = editingCell.Content as Control;
        if (editingElement == null) return false;
        if (raiseEvents)
        {
            var e = new DataGridCellEditEndingEventArgs(CurrentColumn, editingRow, editingElement, editAction);
            OnCellEditEnding(e);
            if (e.Cancel)
                // CellEditEnding has been cancelled
                return false;

            // Ensure that the current cell wasn't changed in the user's CellEditEnding handler
            if (EditingColumnIndex == -1 ||
                currentSlot != CurrentSlot ||
                currentColumnIndex != CurrentColumnIndex)
                return true;
            Debug.Assert(EditingRow != null);
            Debug.Assert(EditingRow.Slot == currentSlot);
            Debug.Assert(EditingColumnIndex != -1);
            Debug.Assert(EditingColumnIndex == CurrentColumnIndex);
        }

        // If we're canceling, let the editing column repopulate its old value if it wants
        if (editAction == DataGridEditAction.Cancel)
        {
            CurrentColumn.CancelCellEditInternal(editingElement, _uneditedValue);

            // Ensure that the current cell wasn't changed in the user column's CancelCellEdit
            if (EditingColumnIndex == -1 ||
                currentSlot != CurrentSlot ||
                currentColumnIndex != CurrentColumnIndex)
                return true;
            Debug.Assert(EditingRow != null);
            Debug.Assert(EditingRow.Slot == currentSlot);
            Debug.Assert(EditingColumnIndex != -1);
            Debug.Assert(EditingColumnIndex == CurrentColumnIndex);
        }

        // If we're committing, explicitly update the source but watch out for any validation errors
        if (editAction == DataGridEditAction.Commit)
        {
            void SetValidationStatus(BindingExpressionBase binding)
            {
                if (!DataValidationErrors.GetHasErrors(editingElement))
                {
                    ResetValidationStatus();
                }
                else
                {
                    if (editingRow != null)
                    {
                        if (editingCell.IsValid)
                        {
                            editingCell.IsValid = false;
                            editingCell.UpdatePseudoClasses();
                        }

                        if (editingRow.IsValid)
                        {
                            editingRow.IsValid = false;
                            editingRow.ApplyState();
                        }
                    }
                }
            }

            var editBinding = CurrentColumn?.CellEditBinding;
            if (editBinding != null && DataValidationErrors.GetHasErrors(editingElement))
            {
                editBinding.UpdateSource();
                SetValidationStatus(editBinding);
                ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, false, true);
                return false;
            }
        }

        ResetValidationStatus();

        if (exitEditingMode)
        {
            CurrentColumn.EndCellEditInternal();
            EditingColumnIndex = -1;
            editingCell.UpdatePseudoClasses();

            //IsTabStop = true;
            if (keepFocus && editingElement.ContainsFocusedElement()) Focus();

            PopulateCellContent(
                !exitEditingMode,
                CurrentColumn,
                editingRow,
                editingCell);

            editingRow.InvalidateDesiredHeight();
            var column = editingCell.OwningColumn;
            if (column.Width.IsSizeToCells || column.Width.IsAuto)
            {
                // Invalidate desired width and force recalculation
                column.SetWidthDesiredValue(0);
                editingRow.OwningGrid.AutoSizeColumn(column, editingCell.DesiredSize.Width);
            }
        }

        // We're done, so raise the CellEditEnded event
        if (raiseEvents) OnCellEditEnded(new DataGridCellEditEndedEventArgs(CurrentColumn, editingRow, editAction));

        // There's a chance that somebody reopened this cell for edit within the CellEditEnded handler,
        // so we should return false if we were supposed to exit editing mode, but we didn't
        return !(exitEditingMode && currentColumnIndex == EditingColumnIndex);
    }

    //TODO Validation
    private bool EndRowEdit(DataGridEditAction editAction, bool exitEditingMode, bool raiseEvents)
    {
        if (EditingRow == null || DataConnection.CommittingEdit) return true;
        if (EditingColumnIndex != -1 || (editAction == DataGridEditAction.Cancel && raiseEvents &&
                                         !((DataConnection.EditableCollectionView != null &&
                                            DataConnection.EditableCollectionView.CanCancelEdit) ||
                                           EditingRow.DataContext is IEditableObject)))
            // Ending the row edit will fail immediately under the following conditions:
            // 1. We haven't ended the cell edit yet.
            // 2. We're trying to cancel edit when the underlying DataType is not an IEditableObject,
            //    because we have no way to properly restore the old value.  We will only allow this to occur
            //    if raiseEvents == false, which means we're internally forcing a cancel.
            return false;
        var editingRow = EditingRow;

        if (raiseEvents)
        {
            var e = new DataGridRowEditEndingEventArgs(EditingRow, editAction);
            OnRowEditEnding(e);
            if (e.Cancel)
                // RowEditEnding has been cancelled
                return false;

            // Editing states might have been changed in the RowEditEnding handlers
            if (EditingColumnIndex != -1) return false;
            if (editingRow != EditingRow) return true;
        }

        // Call the appropriate commit or cancel methods
        if (editAction == DataGridEditAction.Commit)
        {
            if (!CommitRowEdit(exitEditingMode)) return false;
        }
        else
        {
            if (!CancelRowEdit(exitEditingMode) && raiseEvents)
                // We failed to cancel edit so we should abort unless we're forcing a cancel
                return false;
        }

        ResetValidationStatus();

        // Update the previously edited row's state
        if (exitEditingMode && editingRow == EditingRow)
        {
            RemoveEditingElements();
            ResetEditingRow();
        }

        // Raise the RowEditEnded event
        if (raiseEvents) OnRowEditEnded(new DataGridRowEditEndedEventArgs(editingRow, editAction));

        return true;
    }

    private void EnsureColumnHeadersVisibility()
    {
        if (ColumnHeaders != null) ColumnHeaders.IsVisible = AreColumnHeadersVisible;
    }

    private void EnsureVerticalGridLines()
    {
        if (AreColumnHeadersVisible)
        {
            double totalColumnsWidth = 0;
            foreach (var column in ColumnsInternal)
            {
                totalColumnsWidth += column.ActualWidth;

                column.HeaderCell.AreSeparatorsVisible =
                    column != ColumnsInternal.LastVisibleColumn || totalColumnsWidth < CellsWidth;
            }
        }

        foreach (var row in GetAllRows()) row.EnsureGridLines();
    }

    /// <summary>
    ///     Exits editing mode without trying to commit or revert the editing, and
    ///     without repopulating the edited row's cell.
    /// </summary>
    //TODO TabStop
    private void ExitEdit(bool keepFocus)
    {
        if (EditingRow == null || DataConnection.CommittingEdit)
        {
            Debug.Assert(EditingColumnIndex == -1);
            return;
        }

        if (EditingColumnIndex != -1)
        {
            Debug.Assert(EditingColumnIndex >= 0);
            Debug.Assert(EditingColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(EditingColumnIndex == CurrentColumnIndex);
            Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);

            EditingColumnIndex = -1;
            EditingRow.Cells[CurrentColumnIndex].UpdatePseudoClasses();
        }

        //IsTabStop = true;
        if (IsSlotVisible(EditingRow.Slot)) EditingRow.ApplyState();
        ResetEditingRow();
        if (keepFocus) Focus();
    }

    private void ExternalEditingElement_LostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is Control element)
        {
            element.LostFocus -= ExternalEditingElement_LostFocus;
            DataGrid_LostFocus(sender, e);
        }
    }

    private void FlushCurrentCellChanged()
    {
        if (_makeFirstDisplayedCellCurrentCellPending) return;
        if (SelectionHasChanged)
        {
            // selection is changing, don't raise CurrentCellChanged until it's done
            _flushCurrentCellChanged = true;
            FlushSelectionChanged();
            return;
        }

        // We don't want to expand all intermediate currency positions, so we only expand
        // the last current item before we flush the event
        if (_collapsedSlotsTable.Contains(CurrentSlot))
        {
            var rowGroupInfo = RowGroupHeadersTable.GetValueAt(RowGroupHeadersTable.GetPreviousIndex(CurrentSlot));
            Debug.Assert(rowGroupInfo != null);
            if (rowGroupInfo != null) ExpandRowGroupParentChain(rowGroupInfo.Level, rowGroupInfo.Slot);
        }

        if (CurrentColumn != _previousCurrentColumn
            || CurrentItem != _previousCurrentItem)
        {
            CoerceSelectedItem();
            _previousCurrentColumn = CurrentColumn;
            _previousCurrentItem = CurrentItem;

            OnCurrentCellChanged(EventArgs.Empty);
        }

        _flushCurrentCellChanged = false;
    }

    private void FlushSelectionChanged()
    {
        if (SelectionHasChanged && _noSelectionChangeCount == 0 && !_makeFirstDisplayedCellCurrentCellPending)
        {
            CoerceSelectedItem();
            if (NoCurrentCellChangeCount != 0)
                // current cell is changing, don't raise SelectionChanged until it's done
                return;
            SelectionHasChanged = false;

            if (_flushCurrentCellChanged) FlushCurrentCellChanged();

            var e = _selectedItems.GetSelectionChangedEventArgs();
            if (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0) OnSelectionChanged(e);
        }
    }

    //TODO TabStop
    private bool FocusEditingCell(bool setFocus)
    {
        Debug.Assert(CurrentColumnIndex >= 0);
        Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(CurrentSlot >= -1);
        Debug.Assert(CurrentSlot < SlotCount);
        Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);
        Debug.Assert(EditingColumnIndex != -1);

        //IsTabStop = false;
        _focusEditingControl = false;

        var success = false;
        var dataGridCell = EditingRow.Cells[EditingColumnIndex];
        if (setFocus)
        {
            if (dataGridCell.ContainsFocusedElement())
            {
                success = true;
            }
            else
            {
                dataGridCell.Focus();
                success = dataGridCell.ContainsFocusedElement();
            }

            _focusEditingControl = !success;
        }

        return success;
    }

    // Calculates the amount to scroll for the ScrollLeft button
    // This is a method rather than a property to emphasize a calculation
    private double GetHorizontalSmallScrollDecrease()
    {
        // If the first column is covered up, scroll to the start of it when the user clicks the left button
        if (FirstDisplayedScrollingColumnHiddenWidth > 0) return FirstDisplayedScrollingColumnHiddenWidth;

        if (IsColumnOutOfBounds(DisplayData.FirstDisplayedScrollingCol)) return 0;

        // The entire first column is displayed, show the entire previous column when the user clicks
        // the left button
        var previousColumn = ColumnsInternal.GetPreviousVisibleScrollingColumn(
            ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]);
        if (previousColumn != null) return GetEdgedColumnWidth(previousColumn);

        // There's no previous column so don't move
        return 0;
    }

    // Calculates the amount to scroll for the ScrollRight button
    // This is a method rather than a property to emphasize a calculation
    private double GetHorizontalSmallScrollIncrease()
    {
        if (!IsColumnOutOfBounds(DisplayData.FirstDisplayedScrollingCol))
            return GetEdgedColumnWidth(ColumnsItemsInternal[DisplayData.FirstDisplayedScrollingCol]) -
                   FirstDisplayedScrollingColumnHiddenWidth;
        return 0;
    }

    // Calculates the amount the ScrollDown button should scroll
    // This is a method rather than a property to emphasize that calculations are taking place
    private double GetVerticalSmallScrollIncrease()
    {
        if (DisplayData.FirstScrollingSlot >= 0)
            return GetExactSlotElementHeight(DisplayData.FirstScrollingSlot) - NegVerticalOffset;
        return 0;
    }

    private void HorizontalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        ProcessHorizontalScroll(e.ScrollEventType);
        HorizontalScroll?.Invoke(sender, e);
    }

    private bool IsColumnOutOfBounds(int columnIndex)
    {
        return columnIndex >= ColumnsItemsInternal.Count || columnIndex < 0;
    }

    private bool IsInnerCellOutOfBounds(int columnIndex, int slot)
    {
        return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfBounds(slot);
    }

    private bool IsInnerCellOutOfSelectionBounds(int columnIndex, int slot)
    {
        return IsColumnOutOfBounds(columnIndex) || IsSlotOutOfSelectionBounds(slot);
    }

    private bool IsSlotOutOfBounds(int slot)
    {
        return slot >= SlotCount || slot < -1 || _collapsedSlotsTable.Contains(slot);
    }

    private bool IsSlotOutOfSelectionBounds(int slot)
    {
        if (RowGroupHeadersTable.Contains(slot))
        {
            Debug.Assert(slot >= 0 && slot < SlotCount);
            return false;
        }

        var rowIndex = RowIndexFromSlot(slot);
        return rowIndex < 0 || rowIndex >= DataConnection.Count;
    }

    private void MakeFirstDisplayedCellCurrentCell()
    {
        if (CurrentColumnIndex != -1)
        {
            _makeFirstDisplayedCellCurrentCellPending = false;
            _desiredCurrentColumnIndex = -1;
            FlushCurrentCellChanged();
            return;
        }

        if (SlotCount != SlotFromRowIndex(DataConnection.Count))
        {
            _makeFirstDisplayedCellCurrentCellPending = true;
            return;
        }

        // No current cell, therefore no selection either - try to set the current cell to the
        // ItemsSource's ICollectionView.CurrentItem if it exists, otherwise use the first displayed cell.
        var slot = 0;
        if (DataConnection.CollectionView != null)
        {
            if (DataConnection.CollectionView.IsCurrentBeforeFirst ||
                DataConnection.CollectionView.IsCurrentAfterLast)
                slot = RowGroupHeadersTable.Contains(0) ? 0 : -1;
            else
                slot = SlotFromRowIndex(DataConnection.CollectionView.CurrentPosition);
        }
        else
        {
            if (SelectedIndex == -1)
            {
                // Try to default to the first row
                slot = SlotFromRowIndex(0);
                if (!IsSlotVisible(slot)) slot = -1;
            }
            else
            {
                slot = SlotFromRowIndex(SelectedIndex);
            }
        }

        var columnIndex = FirstDisplayedNonFillerColumnIndex;
        if (_desiredCurrentColumnIndex >= 0 && _desiredCurrentColumnIndex < ColumnsItemsInternal.Count)
            columnIndex = _desiredCurrentColumnIndex;

        SetAndSelectCurrentCell(columnIndex, slot, false);
        AnchorSlot = slot;
        _makeFirstDisplayedCellCurrentCellPending = false;
        _desiredCurrentColumnIndex = -1;
        FlushCurrentCellChanged();
    }

    private void PopulateCellContent(bool isCellEdited,
        DataGridColumn dataGridColumn,
        DataGridRow dataGridRow,
        DataGridCell dataGridCell)
    {
        Debug.Assert(dataGridColumn != null);
        Debug.Assert(dataGridRow != null);
        Debug.Assert(dataGridCell != null);

        Control element = null;
        var dataGridBoundColumn = dataGridColumn as DataGridBoundColumn;
        if (isCellEdited)
        {
            // Generate EditingElement and apply column style if available
            element = dataGridColumn.GenerateEditingElementInternal(dataGridCell, dataGridRow.DataContext);
            if (element != null)
            {
                dataGridCell.Content = element;
                if (element.IsInitialized)
                    PreparingCellForEditPrivate(element);
                else
                    // Subscribe to the new element's events
                    element.Initialized += EditingElement_Initialized;
            }
        }
        else
        {
            // Generate Element and apply column style if available
            element = dataGridColumn.GenerateElementInternal(dataGridCell, dataGridRow.DataContext);
            dataGridCell.Content = element;
        }
    }

    private void PreparingCellForEditPrivate(Control editingElement)
    {
        if (EditingColumnIndex == -1 ||
            CurrentColumnIndex == -1 ||
            EditingRow.Cells[CurrentColumnIndex].Content != editingElement)
            // The current cell has changed since the call to BeginCellEdit, so the fact
            // that this element has loaded is no longer relevant
            return;

        Debug.Assert(EditingRow != null);
        Debug.Assert(EditingColumnIndex >= 0);
        Debug.Assert(EditingColumnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(EditingColumnIndex == CurrentColumnIndex);
        Debug.Assert(EditingRow != null && EditingRow.Slot == CurrentSlot);

        FocusEditingCell(ContainsFocus || _focusEditingControl);

        // Prepare the cell for editing and raise the PreparingCellForEdit event for all columns
        var dataGridColumn = CurrentColumn;
        _uneditedValue = dataGridColumn.PrepareCellForEditInternal(editingElement, _editingEventArgs);
        OnPreparingCellForEdit(new DataGridPreparingCellForEditEventArgs(dataGridColumn, EditingRow, _editingEventArgs,
            editingElement));
    }

    private bool ProcessAKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift, out var alt);

        if (ctrl && !shift && !alt && SelectionMode == DataGridSelectionMode.Extended)
        {
            SelectAll();
            return true;
        }

        return false;
    }

    //TODO TabStop
    //TODO FlowDirection
    private bool ProcessDataGridKey(KeyEventArgs e)
    {
        var focusDataGrid = false;
        switch (e.Key)
        {
            case Key.Tab:
                return ProcessTabKey(e);

            case Key.Up:
                focusDataGrid = ProcessUpKey(e);
                break;

            case Key.Down:
                focusDataGrid = ProcessDownKey(e);
                break;

            case Key.PageDown:
                focusDataGrid = ProcessNextKey(e);
                break;

            case Key.PageUp:
                focusDataGrid = ProcessPriorKey(e);
                break;

            case Key.Left:
                focusDataGrid = ProcessLeftKey(e);
                break;

            case Key.Right:
                focusDataGrid = ProcessRightKey(e);
                break;

            case Key.F2:
                return ProcessF2Key(e);

            case Key.Home:
                focusDataGrid = ProcessHomeKey(e);
                break;

            case Key.End:
                focusDataGrid = ProcessEndKey(e);
                break;

            case Key.Enter:
                focusDataGrid = ProcessEnterKey(e);
                break;

            case Key.Escape:
                return ProcessEscapeKey();

            case Key.A:
                return ProcessAKey(e);

            case Key.C:
                return ProcessCopyKey(e.KeyModifiers);

            case Key.Insert:
                return ProcessCopyKey(e.KeyModifiers);
        }

        if (focusDataGrid) Focus();
        return focusDataGrid;
    }

    private bool ProcessDownKeyInternal(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var lastSlot = LastVisibleSlot;
        if (firstVisibleColumnIndex == -1 || lastSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessDownKeyInternal(shift, ctrl))) return true;

        var nextSlot = -1;
        if (CurrentSlot != -1)
        {
            nextSlot = GetNextVisibleSlot(CurrentSlot);
            if (nextSlot >= SlotCount) nextSlot = -1;
        }

        _noSelectionChangeCount++;
        try
        {
            int desiredSlot;
            int columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = FirstVisibleSlot;
                columnIndex = firstVisibleColumnIndex;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else if (ctrl)
            {
                if (shift)
                {
                    // Both Ctrl and Shift
                    desiredSlot = lastSlot;
                    columnIndex = CurrentColumnIndex;
                    action = SelectionMode == DataGridSelectionMode.Extended
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    // Ctrl without Shift
                    desiredSlot = lastSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
            }
            else
            {
                if (nextSlot == -1) return true;
                if (shift)
                {
                    // Shift without Ctrl
                    desiredSlot = nextSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else
                {
                    // Neither Ctrl nor Shift
                    desiredSlot = nextSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
            }

            UpdateSelectionAndCurrency(columnIndex, desiredSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessEndKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.LastVisibleColumn;
        var lastVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var firstVisibleSlot = FirstVisibleSlot;
        var lastVisibleSlot = LastVisibleSlot;
        if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessEndKey(shift, ctrl))) return true;

        _noSelectionChangeCount++;
        try
        {
            if (!ctrl)
            {
                return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                var action = shift && SelectionMode == DataGridSelectionMode.Extended
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;

                UpdateSelectionAndCurrency(lastVisibleColumnIndex, lastVisibleSlot, action, true);
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessEnterKey(bool shift, bool ctrl)
    {
        var oldCurrentSlot = CurrentSlot;

        if (!ctrl)
        {
            // If Enter was used by a TextBox, we shouldn't handle the key
            if (TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() is TextBox focusedTextBox
                && focusedTextBox.AcceptsReturn)
                return false;

            if (WaitForLostFocus(() => ProcessEnterKey(shift, ctrl))) return true;

            // Enter behaves like down arrow - it commits the potential editing and goes down one cell.
            if (!ProcessDownKeyInternal(false, ctrl)) return false;
        }
        else if (WaitForLostFocus(() => ProcessEnterKey(shift, ctrl)))
        {
            return true;
        }

        // Try to commit the potential editing
        if (oldCurrentSlot == CurrentSlot &&
            EndCellEdit(DataGridEditAction.Commit, true, true, true) &&
            EditingRow != null)
        {
            EndRowEdit(DataGridEditAction.Commit, true, true);
            ScrollIntoView(CurrentItem, CurrentColumn);
        }

        return true;
    }

    private bool ProcessEscapeKey()
    {
        if (WaitForLostFocus(() => ProcessEscapeKey())) return true;

        if (EditingColumnIndex != -1)
        {
            // Revert the potential cell editing and exit cell editing.
            EndCellEdit(DataGridEditAction.Cancel, true, true, true);
            return true;
        }

        if (EditingRow != null)
        {
            // Revert the potential row editing and exit row editing.
            EndRowEdit(DataGridEditAction.Cancel, true, true);
            return true;
        }

        return false;
    }

    private bool ProcessF2Key(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);

        if (!shift && !ctrl &&
            EditingColumnIndex == -1 && CurrentColumnIndex != -1 && GetRowSelection(CurrentSlot) &&
            !GetColumnEffectiveReadOnlyState(CurrentColumn))
        {
            if (ScrollSlotIntoView(CurrentColumnIndex, CurrentSlot, false, true)) BeginCellEdit(e);
            return true;
        }

        return false;
    }

    private bool ProcessHomeKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var firstVisibleSlot = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessHomeKey(shift, ctrl))) return true;

        _noSelectionChangeCount++;
        try
        {
            if (!ctrl)
            {
                return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                var action = shift && SelectionMode == DataGridSelectionMode.Extended
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;

                UpdateSelectionAndCurrency(firstVisibleColumnIndex, firstVisibleSlot, action, true);
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessLeftKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var firstVisibleSlot = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessLeftKey(shift, ctrl))) return true;

        var previousVisibleColumnIndex = -1;
        if (CurrentColumnIndex != -1)
        {
            dataGridColumn =
                ColumnsInternal.GetPreviousVisibleNonFillerColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            if (dataGridColumn != null) previousVisibleColumnIndex = dataGridColumn.Index;
        }

        _noSelectionChangeCount++;
        try
        {
            if (ctrl)
            {
                return ProcessLeftMost(firstVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                if (RowGroupHeadersTable.Contains(CurrentSlot))
                {
                    CollapseRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot).CollectionViewGroup, false);
                }
                else if (CurrentColumnIndex == -1)
                {
                    UpdateSelectionAndCurrency(
                        firstVisibleColumnIndex,
                        firstVisibleSlot,
                        DataGridSelectionAction.SelectCurrent,
                        true);
                }
                else
                {
                    if (previousVisibleColumnIndex == -1) return true;

                    UpdateSelectionAndCurrency(
                        previousVisibleColumnIndex,
                        CurrentSlot,
                        DataGridSelectionAction.None,
                        true);
                }
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    // Ctrl Left <==> Home
    private bool ProcessLeftMost(int firstVisibleColumnIndex, int firstVisibleSlot)
    {
        _noSelectionChangeCount++;
        try
        {
            int desiredSlot;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = firstVisibleSlot;
                action = DataGridSelectionAction.SelectCurrent;
                Debug.Assert(_selectedItems.Count == 0);
            }
            else
            {
                desiredSlot = CurrentSlot;
                action = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(firstVisibleColumnIndex, desiredSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessNextKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        if (firstVisibleColumnIndex == -1 || DisplayData.FirstScrollingSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessNextKey(shift, ctrl))) return true;

        var nextPageSlot = CurrentSlot == -1 ? DisplayData.FirstScrollingSlot : CurrentSlot;
        Debug.Assert(nextPageSlot != -1);
        var slot = GetNextVisibleSlot(nextPageSlot);

        var scrollCount = DisplayData.NumTotallyDisplayedScrollingElements;
        while (scrollCount > 0 && slot < SlotCount)
        {
            nextPageSlot = slot;
            scrollCount--;
            slot = GetNextVisibleSlot(slot);
        }

        _noSelectionChangeCount++;
        try
        {
            DataGridSelectionAction action;
            int columnIndex;
            if (CurrentColumnIndex == -1)
            {
                columnIndex = firstVisibleColumnIndex;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                columnIndex = CurrentColumnIndex;
                action = shift && SelectionMode == DataGridSelectionMode.Extended
                    ? action = DataGridSelectionAction.SelectFromAnchorToCurrent
                    : action = DataGridSelectionAction.SelectCurrent;
            }

            UpdateSelectionAndCurrency(columnIndex, nextPageSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessPriorKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        if (firstVisibleColumnIndex == -1 || DisplayData.FirstScrollingSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessPriorKey(shift, ctrl))) return true;

        var previousPageSlot = CurrentSlot == -1 ? DisplayData.FirstScrollingSlot : CurrentSlot;
        Debug.Assert(previousPageSlot != -1);

        var scrollCount = DisplayData.NumTotallyDisplayedScrollingElements;
        var slot = GetPreviousVisibleSlot(previousPageSlot);
        while (scrollCount > 0 && slot != -1)
        {
            previousPageSlot = slot;
            scrollCount--;
            slot = GetPreviousVisibleSlot(slot);
        }

        Debug.Assert(previousPageSlot != -1);

        _noSelectionChangeCount++;
        try
        {
            int columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                columnIndex = firstVisibleColumnIndex;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                columnIndex = CurrentColumnIndex;
                action = shift && SelectionMode == DataGridSelectionMode.Extended
                    ? DataGridSelectionAction.SelectFromAnchorToCurrent
                    : DataGridSelectionAction.SelectCurrent;
            }

            UpdateSelectionAndCurrency(columnIndex, previousPageSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessRightKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.LastVisibleColumn;
        var lastVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var firstVisibleSlot = FirstVisibleSlot;
        if (lastVisibleColumnIndex == -1 || firstVisibleSlot == -1) return false;

        if (WaitForLostFocus(delegate { ProcessRightKey(shift, ctrl); }))
            return true;

        var nextVisibleColumnIndex = -1;
        if (CurrentColumnIndex != -1)
        {
            dataGridColumn = ColumnsInternal.GetNextVisibleColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            if (dataGridColumn != null) nextVisibleColumnIndex = dataGridColumn.Index;
        }

        _noSelectionChangeCount++;
        try
        {
            if (ctrl)
            {
                return ProcessRightMost(lastVisibleColumnIndex, firstVisibleSlot);
            }
            else
            {
                if (RowGroupHeadersTable.Contains(CurrentSlot))
                {
                    ExpandRowGroup(RowGroupHeadersTable.GetValueAt(CurrentSlot).CollectionViewGroup, false);
                }
                else if (CurrentColumnIndex == -1)
                {
                    var firstVisibleColumnIndex = ColumnsInternal.FirstVisibleColumn == null
                        ? -1
                        : ColumnsInternal.FirstVisibleColumn.Index;

                    UpdateSelectionAndCurrency(
                        firstVisibleColumnIndex,
                        firstVisibleSlot,
                        DataGridSelectionAction.SelectCurrent,
                        true);
                }
                else
                {
                    if (nextVisibleColumnIndex == -1) return true;

                    UpdateSelectionAndCurrency(
                        nextVisibleColumnIndex,
                        CurrentSlot,
                        DataGridSelectionAction.None,
                        true);
                }
            }
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    // Ctrl Right <==> End
    private bool ProcessRightMost(int lastVisibleColumnIndex, int firstVisibleSlot)
    {
        _noSelectionChangeCount++;
        try
        {
            int desiredSlot;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                desiredSlot = firstVisibleSlot;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                desiredSlot = CurrentSlot;
                action = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(lastVisibleColumnIndex, desiredSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private bool ProcessTabKey(KeyEventArgs e)
    {
        KeyboardHelper.GetMetaKeyState(this, e.KeyModifiers, out var ctrl, out var shift);
        return ProcessTabKey(e, shift, ctrl);
    }

    private bool ProcessTabKey(KeyEventArgs e, bool shift, bool ctrl)
    {
        if (ctrl || EditingColumnIndex == -1 || IsReadOnly)
            //Go to the next/previous control on the page when
            // - Ctrl key is used
            // - Potential current cell is not edited, or the datagrid is read-only.
            return false;

        // Try to locate a writable cell before/after the current cell
        Debug.Assert(CurrentColumnIndex != -1);
        Debug.Assert(CurrentSlot != -1);

        int neighborVisibleWritableColumnIndex, neighborSlot;
        DataGridColumn dataGridColumn;
        if (shift)
        {
            dataGridColumn = ColumnsInternal.GetPreviousVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            neighborSlot = GetPreviousVisibleSlot(CurrentSlot);
            if (EditingRow != null)
                while (neighborSlot != -1 && RowGroupHeadersTable.Contains(neighborSlot))
                    neighborSlot = GetPreviousVisibleSlot(neighborSlot);
        }
        else
        {
            dataGridColumn = ColumnsInternal.GetNextVisibleWritableColumn(ColumnsItemsInternal[CurrentColumnIndex]);
            neighborSlot = GetNextVisibleSlot(CurrentSlot);
            if (EditingRow != null)
                while (neighborSlot < SlotCount && RowGroupHeadersTable.Contains(neighborSlot))
                    neighborSlot = GetNextVisibleSlot(neighborSlot);
        }

        neighborVisibleWritableColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;

        if (neighborVisibleWritableColumnIndex == -1 && (neighborSlot == -1 || neighborSlot >= SlotCount))
            // There is no previous/next row and no previous/next writable cell on the current row
            return false;

        if (WaitForLostFocus(() => ProcessTabKey(e, shift, ctrl))) return true;

        int targetSlot = -1, targetColumnIndex = -1;

        _noSelectionChangeCount++;
        try
        {
            if (neighborVisibleWritableColumnIndex == -1)
            {
                targetSlot = neighborSlot;
                if (shift)
                {
                    Debug.Assert(ColumnsInternal.LastVisibleWritableColumn != null);
                    targetColumnIndex = ColumnsInternal.LastVisibleWritableColumn.Index;
                }
                else
                {
                    Debug.Assert(ColumnsInternal.FirstVisibleWritableColumn != null);
                    targetColumnIndex = ColumnsInternal.FirstVisibleWritableColumn.Index;
                }
            }
            else
            {
                targetSlot = CurrentSlot;
                targetColumnIndex = neighborVisibleWritableColumnIndex;
            }

            DataGridSelectionAction action;
            if (targetSlot != CurrentSlot || SelectionMode == DataGridSelectionMode.Extended)
            {
                if (IsSlotOutOfBounds(targetSlot)) return true;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else
            {
                action = DataGridSelectionAction.None;
            }

            UpdateSelectionAndCurrency(targetColumnIndex, targetSlot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        if (_successfullyUpdatedSelection && !RowGroupHeadersTable.Contains(targetSlot)) BeginCellEdit(e);

        // Return true to say we handled the key event even if the operation was unsuccessful. If we don't
        // say we handled this event, the framework will continue to process the tab key and change focus.
        return true;
    }

    private bool ProcessUpKey(bool shift, bool ctrl)
    {
        var dataGridColumn = ColumnsInternal.FirstVisibleNonFillerColumn;
        var firstVisibleColumnIndex = dataGridColumn == null ? -1 : dataGridColumn.Index;
        var firstVisibleSlot = FirstVisibleSlot;
        if (firstVisibleColumnIndex == -1 || firstVisibleSlot == -1) return false;

        if (WaitForLostFocus(() => ProcessUpKey(shift, ctrl))) return true;

        var previousVisibleSlot = CurrentSlot != -1 ? GetPreviousVisibleSlot(CurrentSlot) : -1;

        _noSelectionChangeCount++;

        try
        {
            int slot;
            int columnIndex;
            DataGridSelectionAction action;
            if (CurrentColumnIndex == -1)
            {
                slot = firstVisibleSlot;
                columnIndex = firstVisibleColumnIndex;
                action = DataGridSelectionAction.SelectCurrent;
            }
            else if (ctrl)
            {
                if (shift)
                {
                    // Both Ctrl and Shift
                    slot = firstVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action = SelectionMode == DataGridSelectionMode.Extended
                        ? DataGridSelectionAction.SelectFromAnchorToCurrent
                        : DataGridSelectionAction.SelectCurrent;
                }
                else
                {
                    // Ctrl without Shift
                    slot = firstVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
            }
            else
            {
                if (previousVisibleSlot == -1) return true;
                if (shift)
                {
                    // Shift without Ctrl
                    slot = previousVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectFromAnchorToCurrent;
                }
                else
                {
                    // Neither Shift nor Ctrl
                    slot = previousVisibleSlot;
                    columnIndex = CurrentColumnIndex;
                    action = DataGridSelectionAction.SelectCurrent;
                }
            }

            UpdateSelectionAndCurrency(columnIndex, slot, action, true);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return _successfullyUpdatedSelection;
    }

    private void RemoveDisplayedColumnHeader(DataGridColumn dataGridColumn)
    {
        if (ColumnHeaders != null) ColumnHeaders.Children.Remove(dataGridColumn.HeaderCell);
    }

    private void RemoveDisplayedColumnHeaders()
    {
        if (ColumnHeaders != null) ColumnHeaders.Children.Clear();
        ColumnsInternal.FillerColumn.IsRepresented = false;
    }

    private bool ResetCurrentCellCore()
    {
        return CurrentColumnIndex == -1 || SetCurrentCellCore(-1, -1);
    }

    private void ResetEditingRow()
    {
        if (EditingRow != null
            && EditingRow != _focusedRow
            && !IsSlotVisible(EditingRow.Slot))
        {
            // Unload the old editing row if it's off screen
            EditingRow.Clip = null;
            UnloadRow(EditingRow);
            DisplayData.FullyRecycleElements();
        }

        EditingRow = null;
    }

    private void ResetFocusedRow()
    {
        if (_focusedRow != null
            && _focusedRow != EditingRow
            && !IsSlotVisible(_focusedRow.Slot))
        {
            // Unload the old focused row if it's off screen
            _focusedRow.Clip = null;
            UnloadRow(_focusedRow);
            DisplayData.FullyRecycleElements();
        }

        _focusedRow = null;
    }

    public void SelectAll()
    {
        SetRowsSelection(0, SlotCount - 1);
    }

    private void SetAndSelectCurrentCell(int columnIndex,
        int slot,
        bool forceCurrentCellSelection)
    {
        var action = forceCurrentCellSelection ? DataGridSelectionAction.SelectCurrent : DataGridSelectionAction.None;
        UpdateSelectionAndCurrency(columnIndex, slot, action, false);
    }

    // columnIndex = 2, rowIndex = -1 --> current cell belongs to the 'new row'.
    // columnIndex = 2, rowIndex = 2 --> current cell is an inner cell
    // columnIndex = -1, rowIndex = -1 --> current cell is reset
    // columnIndex = -1, rowIndex = 2 --> Unexpected
    private bool SetCurrentCellCore(int columnIndex, int slot, bool commitEdit, bool endRowEdit)
    {
        Debug.Assert(columnIndex < ColumnsItemsInternal.Count);
        Debug.Assert(slot < SlotCount);
        Debug.Assert(columnIndex == -1 || ColumnsItemsInternal[columnIndex].IsVisible);
        Debug.Assert(!(columnIndex > -1 && slot == -1));

        if (columnIndex == CurrentColumnIndex &&
            slot == CurrentSlot)
        {
            Debug.Assert(DataConnection != null);
            Debug.Assert(EditingColumnIndex == -1 || EditingColumnIndex == CurrentColumnIndex);
            Debug.Assert(EditingRow == null || EditingRow.Slot == CurrentSlot || DataConnection.CommittingEdit);
            return true;
        }

        Control oldDisplayedElement = null;
        var oldCurrentCell = new DataGridCellCoordinates(CurrentCellCoordinates);

        object newCurrentItem = null;
        if (!RowGroupHeadersTable.Contains(slot))
        {
            var rowIndex = RowIndexFromSlot(slot);
            if (rowIndex >= 0 && rowIndex < DataConnection.Count) newCurrentItem = DataConnection.GetDataItem(rowIndex);
        }

        if (CurrentColumnIndex > -1)
        {
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(CurrentSlot < SlotCount);

            if (!IsInnerCellOutOfBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot) &&
                IsSlotVisible(oldCurrentCell.Slot))
                oldDisplayedElement = DisplayData.GetDisplayedElement(oldCurrentCell.Slot);

            if (!RowGroupHeadersTable.Contains(oldCurrentCell.Slot) && !_temporarilyResetCurrentCell)
            {
                var keepFocus = ContainsFocus;
                if (commitEdit)
                {
                    if (!EndCellEdit(DataGridEditAction.Commit, true, keepFocus, true)) return false;
                    // Resetting the current cell: setting it to (-1, -1) is not considered setting it out of bounds
                    if ((columnIndex != -1 && slot != -1 && IsInnerCellOutOfSelectionBounds(columnIndex, slot)) ||
                        IsInnerCellOutOfSelectionBounds(oldCurrentCell.ColumnIndex, oldCurrentCell.Slot))
                        return false;

                    if (endRowEdit && !EndRowEdit(DataGridEditAction.Commit, true, true)) return false;
                }
                else
                {
                    CancelEdit(DataGridEditingUnit.Row, false);
                    ExitEdit(keepFocus);
                }
            }
        }

        if (newCurrentItem != null) slot = SlotFromRowIndex(DataConnection.IndexOf(newCurrentItem));
        if (slot == -1 && columnIndex != -1) return false;
        CurrentColumnIndex = columnIndex;
        CurrentSlot = slot;

        if (_temporarilyResetCurrentCell)
            if (columnIndex != -1)
                _temporarilyResetCurrentCell = false;

        if (!_temporarilyResetCurrentCell && EditingColumnIndex != -1) EditingColumnIndex = columnIndex;

        if (oldDisplayedElement != null)
        {
            if (oldDisplayedElement is DataGridRow row)
                // Don't reset the state of the current cell if we're editing it because that would put it in an invalid state
                UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex,
                    !(_temporarilyResetCurrentCell && row.IsEditing &&
                      EditingColumnIndex == oldCurrentCell.ColumnIndex));
            else
                UpdateCurrentState(oldDisplayedElement, oldCurrentCell.ColumnIndex, false);
        }

        if (CurrentColumnIndex > -1)
        {
            Debug.Assert(CurrentSlot > -1);
            Debug.Assert(CurrentColumnIndex < ColumnsItemsInternal.Count);
            Debug.Assert(CurrentSlot < SlotCount);
            if (IsSlotVisible(CurrentSlot))
                UpdateCurrentState(DisplayData.GetDisplayedElement(CurrentSlot), CurrentColumnIndex, true);
        }

        return true;
    }

    private void SetVerticalOffset(double newVerticalOffset)
    {
        _verticalOffset = newVerticalOffset;
        if (VerticalScrollBar != null && !MathUtilities.AreClose(newVerticalOffset, VerticalScrollBar.Value))
            VerticalScrollBar.Value = _verticalOffset;
    }

    private void UpdateCurrentState(Control displayedElement, int columnIndex, bool applyCellState)
    {
        if (displayedElement is DataGridRow row)
        {
            if (AreRowHeadersVisible) row.ApplyHeaderStatus();
            var cell = row.Cells[columnIndex];
            if (applyCellState) cell.UpdatePseudoClasses();
        }
        else if (displayedElement is DataGridRowGroupHeader groupHeader)
        {
            groupHeader.UpdatePseudoClasses();
            if (AreRowHeadersVisible) groupHeader.ApplyHeaderStatus();
        }
    }

    private void UpdateHorizontalScrollBar(bool needHorizScrollbar, bool forceHorizScrollbar, double totalVisibleWidth,
        double totalVisibleFrozenWidth, double cellsWidth)
    {
        if (HorizontalScrollBar != null)
        {
            UpdateRightFrozenScrollBarMargin();
            if (needHorizScrollbar || forceHorizScrollbar)
            {
                //          viewportSize
                //        v---v
                //|<|_____|###|>|
                //  ^     ^
                //  min   max

                // we want to make the relative size of the thumb reflect the relative size of the viewing area
                // viewportSize / (max + viewportSize) = cellsWidth / max
                // -> viewportSize = max * cellsWidth / (max - cellsWidth)

                // always zero
                HorizontalScrollBar.Minimum = 0;
                if (needHorizScrollbar)
                {
                    // maximum travel distance -- not the total width
                    HorizontalScrollBar.Maximum = totalVisibleWidth - cellsWidth;
                    Debug.Assert(totalVisibleFrozenWidth >= 0);
                    if (_frozenColumnScrollBarSpacer != null)
                        _frozenColumnScrollBarSpacer.Width =
                            FrozenRightColumnCount > 0 ? LeftFrozenWidth : totalVisibleFrozenWidth;
                    Debug.Assert(HorizontalScrollBar.Maximum >= 0);

                    // width of the scrollable viewing area
                    var viewPortSize = Math.Max(0, cellsWidth - totalVisibleFrozenWidth);
                    HorizontalScrollBar.ViewportSize = viewPortSize;
                    HorizontalScrollBar.LargeChange = viewPortSize;
                    // The ScrollBar should be in sync with HorizontalOffset at this point.  There's a resize case
                    // where the ScrollBar will coerce an old value here, but we don't want that
                    if (HorizontalScrollBar.Value != _horizontalOffset) HorizontalScrollBar.Value = _horizontalOffset;
                    HorizontalScrollBar.IsEnabled = true;
                }
                else
                {
                    HorizontalScrollBar.Maximum = 0;
                    HorizontalScrollBar.ViewportSize = 0;
                    HorizontalScrollBar.IsEnabled = false;
                }

                if (!HorizontalScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    _ignoreNextScrollBarsLayout = true;
                    // which no processing is needed.
                    HorizontalScrollBar.IsVisible = true;
                    if (HorizontalScrollBar.DesiredSize.Height == 0)
                        // We need to know the height for the rest of layout to work correctly so measure it now
                        HorizontalScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                }
            }
            else
            {
                HorizontalScrollBar.Maximum = 0;
                if (HorizontalScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    HorizontalScrollBar.IsVisible = false;
                    _ignoreNextScrollBarsLayout = true;
                }
            }
        }
    }

    private void UpdateVerticalScrollBar(bool needVertScrollbar, bool forceVertScrollbar, double totalVisibleHeight,
        double cellsHeight)
    {
        if (VerticalScrollBar != null)
        {
            if (needVertScrollbar || forceVertScrollbar)
            {
                //          viewportSize
                //        v---v
                //|<|_____|###|>|
                //  ^     ^
                //  min   max

                // we want to make the relative size of the thumb reflect the relative size of the viewing area
                // viewportSize / (max + viewportSize) = cellsWidth / max
                // -> viewportSize = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                // ->              = max * cellsHeight / (totalVisibleHeight - cellsHeight)
                // ->              = max * cellsHeight / max
                // ->              = cellsHeight

                // always zero
                VerticalScrollBar.Minimum = 0;
                if (needVertScrollbar && !double.IsInfinity(cellsHeight))
                {
                    // maximum travel distance -- not the total height
                    VerticalScrollBar.Maximum = totalVisibleHeight - cellsHeight;
                    Debug.Assert(VerticalScrollBar.Maximum >= 0);

                    // total height of the display area
                    VerticalScrollBar.ViewportSize = cellsHeight;
                    VerticalScrollBar.LargeChange = cellsHeight;
                    VerticalScrollBar.IsEnabled = true;
                }
                else
                {
                    VerticalScrollBar.Maximum = 0;
                    VerticalScrollBar.ViewportSize = 0;
                    VerticalScrollBar.IsEnabled = false;
                }

                if (!VerticalScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    VerticalScrollBar.IsVisible = true;
                    if (VerticalScrollBar.DesiredSize.Width == 0)
                        // We need to know the width for the rest of layout to work correctly so measure it now
                        VerticalScrollBar.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    _ignoreNextScrollBarsLayout = true;
                }
            }
            else
            {
                VerticalScrollBar.Maximum = 0;
                if (VerticalScrollBar.IsVisible)
                {
                    // This will trigger a call to this method via Cells_SizeChanged for
                    // which no processing is needed.
                    VerticalScrollBar.IsVisible = false;
                    _ignoreNextScrollBarsLayout = true;
                }
            }
        }
    }

    private void VerticalScrollBar_Scroll(object sender, ScrollEventArgs e)
    {
        ProcessVerticalScroll(e.ScrollEventType);
        VerticalScroll?.Invoke(sender, e);
    }

    //TODO: Ensure right button is checked for
    private bool UpdateStateOnMouseRightButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex,
        int slot, bool allowEdit, bool shift, bool ctrl)
    {
        Debug.Assert(slot >= 0);

        if (shift || ctrl) return true;
        if (IsSlotOutOfBounds(slot)) return true;
        if (GetRowSelection(slot)) return true;
        // Unselect everything except the row that was clicked on
        _noSelectionChangeCount++;
        try
        {
            UpdateSelectionAndCurrency(columnIndex, slot, DataGridSelectionAction.SelectCurrent, false);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        return true;
    }

    //TODO: Ensure left button is checked for
    private bool UpdateStateOnMouseLeftButtonDown(PointerPressedEventArgs pointerPressedEventArgs, int columnIndex,
        int slot, bool allowEdit, bool shift, bool ctrl)
    {
        bool beginEdit;

        Debug.Assert(slot >= 0);

        // Before changing selection, check if the current cell needs to be committed, and
        // check if the current row needs to be committed. If any of those two operations are required and fail,
        // do not change selection, and do not change current cell.

        var wasInEdit = EditingColumnIndex != -1;

        if (IsSlotOutOfBounds(slot)) return true;

        if (wasInEdit && (columnIndex != EditingColumnIndex || slot != CurrentSlot) &&
            WaitForLostFocus(() =>
                UpdateStateOnMouseLeftButtonDown(pointerPressedEventArgs, columnIndex, slot, allowEdit, shift, ctrl)))
            return true;

        try
        {
            _noSelectionChangeCount++;

            beginEdit = allowEdit &&
                        CurrentSlot == slot &&
                        columnIndex != -1 &&
                        (wasInEdit || CurrentColumnIndex == columnIndex) &&
                        !GetColumnEffectiveReadOnlyState(ColumnsItemsInternal[columnIndex]);

            DataGridSelectionAction action;
            if (SelectionMode == DataGridSelectionMode.Extended && shift)
            {
                // Shift select multiple rows
                action = DataGridSelectionAction.SelectFromAnchorToCurrent;
            }
            else if (GetRowSelection(slot)) // Unselecting single row or Selecting a previously multi-selected row
            {
                if (!ctrl && SelectionMode == DataGridSelectionMode.Extended && _selectedItems.Count != 0)
                    // Unselect everything except the row that was clicked on
                    action = DataGridSelectionAction.SelectCurrent;
                else if (ctrl && EditingRow == null)
                    action = DataGridSelectionAction.RemoveCurrentFromSelection;
                else
                    action = DataGridSelectionAction.None;
            }
            else // Selecting a single row or multi-selecting with Ctrl
            {
                if (SelectionMode == DataGridSelectionMode.Single || !ctrl)
                    // Unselect the currently selected rows except the new selected row
                    action = DataGridSelectionAction.SelectCurrent;
                else
                    action = DataGridSelectionAction.AddCurrentToSelection;
            }

            UpdateSelectionAndCurrency(columnIndex, slot, action, false);
        }
        finally
        {
            NoSelectionChangeCount--;
        }

        if (_successfullyUpdatedSelection && beginEdit && BeginCellEdit(pointerPressedEventArgs))
            FocusEditingCell(true);

        return true;
    }

    /// <summary>
    ///     Returns the Group at the indicated level or null if the item is not in the ItemsSource
    /// </summary>
    /// <param name="item">item</param>
    /// <param name="groupLevel">groupLevel</param>
    /// <returns>The group the given item falls under or null if the item is not in the ItemsSource</returns>
    public DataGridCollectionViewGroup GetGroupFromItem(object item, int groupLevel)
    {
        var itemIndex = DataConnection.IndexOf(item);
        if (itemIndex == -1) return null;
        var groupHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(SlotFromRowIndex(itemIndex));
        var rowGroupInfo = RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
        while (rowGroupInfo != null && rowGroupInfo.Level != groupLevel)
        {
            groupHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(rowGroupInfo.Slot);
            rowGroupInfo = RowGroupHeadersTable.GetValueAt(groupHeaderSlot);
        }

        return rowGroupInfo?.CollectionViewGroup;
    }

    /// <summary>
    ///     Raises the LoadingRowGroup event
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected virtual void OnLoadingRowGroup(DataGridRowGroupHeaderEventArgs e)
    {
        var handler = LoadingRowGroup;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    ///     Raises the UnLoadingRowGroup event
    /// </summary>
    /// <param name="e">EventArgs</param>
    protected virtual void OnUnloadingRowGroup(DataGridRowGroupHeaderEventArgs e)
    {
        var handler = UnloadingRowGroup;
        if (handler != null)
        {
            LoadingOrUnloadingRow = true;
            handler(this, e);
            LoadingOrUnloadingRow = false;
        }
    }

    /// <summary>
    ///     Occurs before a DataGridRowGroupHeader header is used.
    /// </summary>
    public event EventHandler<DataGridRowGroupHeaderEventArgs> LoadingRowGroup;

    /// <summary>
    ///     Occurs when the DataGridRowGroupHeader is available for reuse.
    /// </summary>
    public event EventHandler<DataGridRowGroupHeaderEventArgs> UnloadingRowGroup;

    // Recursively expands parent RowGroupHeaders from the top down
    private void ExpandRowGroupParentChain(int level, int slot)
    {
        if (level < 0) return;
        var previousHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(slot + 1);
        DataGridRowGroupInfo rowGroupInfo = null;
        while (previousHeaderSlot >= 0)
        {
            rowGroupInfo = RowGroupHeadersTable.GetValueAt(previousHeaderSlot);
            Debug.Assert(rowGroupInfo != null);
            if (level == rowGroupInfo.Level)
            {
                if (_collapsedSlotsTable.Contains(rowGroupInfo.Slot))
                    // Keep going up the chain
                    ExpandRowGroupParentChain(level - 1, rowGroupInfo.Slot - 1);
                if (!rowGroupInfo.IsVisible) EnsureRowGroupVisibility(rowGroupInfo, true, false);
                return;
            }

            previousHeaderSlot = RowGroupHeadersTable.GetPreviousIndex(previousHeaderSlot);
        }
    }

    /// <summary>
    ///     This event is raised by OnCopyingRowClipboardContent method after the default row content is prepared.
    ///     Event listeners can modify or add to the row clipboard content.
    /// </summary>
    public event EventHandler<DataGridRowClipboardEventArgs> CopyingRowClipboardContent;

    /// <summary>
    ///     This method raises the CopyingRowClipboardContent event.
    /// </summary>
    /// <param name="e">Contains the necessary information for generating the row clipboard content.</param>
    protected virtual void OnCopyingRowClipboardContent(DataGridRowClipboardEventArgs e)
    {
        CopyingRowClipboardContent?.Invoke(this, e);
    }

    /// <summary>
    ///     This method formats a row (specified by a DataGridRowClipboardEventArgs) into
    ///     a single string to be added to the Clipboard when the DataGrid is copying its contents.
    /// </summary>
    /// <param name="e">DataGridRowClipboardEventArgs</param>
    /// <returns>The formatted string.</returns>
    private string FormatClipboardContent(DataGridRowClipboardEventArgs e)
    {
        var text = new StringBuilder();
        var clipboardRowContent = e.ClipboardRowContent;
        var numberOfItem = clipboardRowContent.Count;
        for (var cellIndex = 0; cellIndex < numberOfItem; cellIndex++)
        {
            var cellContent = clipboardRowContent[cellIndex].Content?.ToString();
            cellContent = cellContent?.Replace("\"", "\"\"");
            text.Append($"\"{cellContent}\"");
            if (cellIndex < numberOfItem - 1)
            {
                text.Append('\t');
            }
            else
            {
                text.Append('\r');
                text.Append('\n');
            }
        }

        return text.ToString();
    }

    /// <summary>
    ///     Handles the case where a 'Copy' key ('C' or 'Insert') has been pressed.  If pressed in combination with
    ///     the control key, and the necessary prerequisites are met, the DataGrid will copy its contents
    ///     to the Clipboard as text.
    /// </summary>
    /// <returns>Whether or not the DataGrid handled the key press.</returns>
    private bool ProcessCopyKey(KeyModifiers modifiers)
    {
        KeyboardHelper.GetMetaKeyState(this, modifiers, out var ctrl, out var shift, out var alt);

        if (ctrl && !shift && !alt && ClipboardCopyMode != DataGridClipboardCopyMode.None && SelectedItems.Count > 0)
        {
            var textBuilder = new StringBuilder();

            if (ClipboardCopyMode == DataGridClipboardCopyMode.IncludeHeader)
            {
                var headerArgs = new DataGridRowClipboardEventArgs(null, true);
                foreach (var column in ColumnsInternal.GetVisibleColumns())
                    headerArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(null, column, column.Header));
                OnCopyingRowClipboardContent(headerArgs);
                textBuilder.Append(FormatClipboardContent(headerArgs));
            }

            for (var index = 0; index < SelectedItems.Count; index++)
            {
                var item = SelectedItems[index];
                var itemArgs = new DataGridRowClipboardEventArgs(item, false);
                foreach (var column in ColumnsInternal.GetVisibleColumns())
                {
                    var content = column.GetCellValue(item, column.ClipboardContentBinding);
                    itemArgs.ClipboardRowContent.Add(new DataGridClipboardCellContent(item, column, content));
                }

                OnCopyingRowClipboardContent(itemArgs);
                textBuilder.Append(FormatClipboardContent(itemArgs));
            }

            var text = textBuilder.ToString();

            if (!string.IsNullOrEmpty(text))
            {
                CopyToClipboard(text);
                return true;
            }
        }

        return false;
    }

    private async void CopyToClipboard(string text)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;

        if (clipboard != null)
            await clipboard.SetTextAsync(text);
    }

    //TODO Validation UI
    private void ResetValidationStatus()
    {
        // Clear the invalid status of the Cell, Row and DataGrid
        if (EditingRow != null)
        {
            EditingRow.IsValid = true;
            if (EditingRow.Index != -1)
            {
                foreach (DataGridCell cell in EditingRow.Cells)
                    if (!cell.IsValid)
                    {
                        cell.IsValid = true;
                        cell.UpdatePseudoClasses();
                    }

                EditingRow.ApplyState();
            }
        }

        IsValid = true;
    }

    /// <summary>
    ///     Raises the AutoGeneratingColumn event.
    /// </summary>
    protected virtual void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
    {
        AutoGeneratingColumn?.Invoke(this, e);
    }

    /// <summary>
    ///     Comparator class so we can sort list by the display index
    /// </summary>
    public class DisplayIndexComparer : IComparer<DataGridColumn>
    {
        int IComparer<DataGridColumn>.Compare(DataGridColumn x, DataGridColumn y)
        {
            return x.DisplayIndexWithFiller < y.DisplayIndexWithFiller ? -1 : 1;
        }
    }
}
