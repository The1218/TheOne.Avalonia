using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

/// <summary>A single-table report workbench with typed field binding and paginated A4 preview.</summary>
public partial class ReportDesign : UserControl
{
    public static readonly StyledProperty<ReportDocument?> DocumentProperty =
        AvaloniaProperty.Register<ReportDesign, ReportDocument?>(nameof(Document));

    public static readonly StyledProperty<ReportDataSource?> DataSourceProperty =
        AvaloniaProperty.Register<ReportDesign, ReportDataSource?>(nameof(DataSource));

    public static readonly StyledProperty<ReportColumn?> SelectedColumnProperty =
        AvaloniaProperty.Register<ReportDesign, ReportColumn?>(nameof(SelectedColumn),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly DirectProperty<ReportDesign, IReadOnlyList<ReportFieldInfo>> SourceFieldsProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, IReadOnlyList<ReportFieldInfo>>(nameof(SourceFields),
            c => c.SourceFields);

    public static readonly DirectProperty<ReportDesign, ReportFieldInfo?> SelectedFieldProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, ReportFieldInfo?>(nameof(SelectedField), c => c.SelectedField,
            (c, v) => c.SelectedField = v);

    public static readonly DirectProperty<ReportDesign, int> FormatIndexProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, int>(nameof(FormatIndex), c => c.FormatIndex,
            (c, v) => c.FormatIndex = v);

    private readonly HashSet<ReportColumn> _columns = [];
    private readonly bool _ready;
    private readonly Stack<ReportDocument> _undo = [], _redo = [];
    private bool _attached, _syncing, _batch, _restoring;
    private ReportDocument? _lastSnapshot;

    private ReportDocument? _observed;
    private int _pageIndex;

    public ReportDesign()
    {
        CloseDocumentCommand = new CloseReportCommand(this);
        InitializeComponent();
        InitializeDataTreeState();
        StructureTree.AddHandler(PointerReleasedEvent, OnStructurePointerReleased, RoutingStrategies.Bubble, true);
        InitializeDesigner();
        _ready = true;
        State.PropertyChanged += StateChanged;
        PageSurface.ColumnSelected += index =>
        {
            if (Document is { } document && index >= 0 && index < document.Columns.Count)
                SetCurrentValue(SelectedColumnProperty, document.Columns[index]);
        };
        SetCurrentValue(DocumentProperty, new ReportDocument());
    }

    public ReportDocument? Document
    {
        get => GetValue(DocumentProperty);
        set => SetValue(DocumentProperty, value);
    }

    public ReportDataSource? DataSource
    {
        get => GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }

    public ReportColumn? SelectedColumn
    {
        get => GetValue(SelectedColumnProperty);
        set => SetValue(SelectedColumnProperty, value);
    }

    public IReadOnlyList<ReportFieldInfo> SourceFields => DataSource?.Fields ?? [];

    public ReportFieldInfo? SelectedField
    {
        get => SourceFields.FirstOrDefault(f =>
            f.Key == (SelectedElement is { Kind: not ReportElementKind.Table } element
                ? element.FieldKey
                : SelectedColumn?.FieldKey));
        set
        {
            if (!_syncing && value is not null) BindField(value.Key);
        }
    }

    public int FormatIndex
    {
        get => (int)(SelectedColumn?.Format ?? ReportValueFormat.Automatic);
        set
        {
            if (!_syncing && SelectedColumn is not null && value >= 0 && value <= 4)
                SelectedColumn.Format = (ReportValueFormat)value;
        }
    }

    public ReportLayout? Layout { get; private set; }
    public string? ValidationError { get; private set; }
    public bool IsPreview => State.IsPreview;
    public int CurrentPageNumber => _pageIndex + 1;

    public double Zoom
    {
        get => State.Zoom;
        set
        {
            if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value));
            State.Zoom = Math.Clamp(value, .35, 1.5);
        }
    }

    internal ReportDesignState State { get; } = new();

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        Observe();
        Rebuild();
        if (_pendingDocumentData) StartDocumentDataLoad();
        else if (Document?.Elements.Any(e => e.ImageUrl.Length > 0) == true) _ = RefreshRemoteImagesAsync();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (ReferenceEquals(_loadingDataDocument, Document) && State.IsFetching) _pendingDocumentData = true;
        _fetchGeneration++;
        _fetchCancellation?.Cancel();
        _fetchCancellation?.Dispose();
        _fetchCancellation = null;
        State.IsFetching = false;
        _imageCancellation?.Cancel();
        _imageCancellation?.Dispose();
        _imageCancellation = null;
        Unobserve();
        _attached = false;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (!_ready) return;
        if (change.Property == DocumentProperty)
        {
            var clearApiSource = _apiJson is not null && DataSource?.Name == "API";
            Unobserve();
            _historySources.Clear();
            State.Document = Document ?? new ReportDocument();
            State.OpenDocuments.Clear();
            if (Document is { } current) State.OpenDocuments.Add(current);
            State.HasDocument = Document is not null;
            State.SaveStatus = Document is null ? "" : "未保存";
            ((CloseReportCommand)CloseDocumentCommand).Refresh();
            State.ApiUrl = State.Document.DataSourceUrl;
            ResetDocumentData(clearApiSource);
            if (_attached) Observe();
            if (!_restoring)
            {
                _undo.Clear();
                _redo.Clear();
            }

            _lastSnapshot = CaptureHistorySource(Copy(State.Document));
            SetCurrentValue(SelectedElementProperty, null);
            SetCurrentValue(SelectedColumnProperty, State.Document.Columns.LastOrDefault());
            Rebuild();
            if (_attached && _pendingDocumentData) StartDocumentDataLoad();
            else if (_attached && Document?.Elements.Any(e => e.ImageUrl.Length > 0) == true)
                _ = RefreshRemoteImagesAsync();
        }
        else if (change.Property == DataSourceProperty)
        {
            if (!_committingDocumentData && DataSource is not null && _loadingDataDocument is not null)
            {
                ++_fetchGeneration;
                _fetchCancellation?.Cancel();
                State.IsFetching = false;
                _loadingDataDocument = null;
                _pendingDocumentData = false;
            }

            if (!_batch && !_restoring) UpdateHistorySource();
            RaisePropertyChanged(SourceFieldsProperty, Array.Empty<ReportFieldInfo>(), SourceFields);
            FilterFields();
            _pageIndex = 0;
            Rebuild();
        }
        else if (change.Property == SelectedElementProperty)
        {
            State.SelectedElement = SelectedElement;
            if (SelectedElement?.Kind == ReportElementKind.Image) State.PropertyTab = 0;
            if (SelectedElement is { Kind: ReportElementKind.Table } && SelectedColumn is null)
                SetCurrentValue(SelectedColumnProperty, Document?.Columns.FirstOrDefault());
            if (SelectedElement is { Kind: not ReportElementKind.Table }) SetCurrentValue(SelectedColumnProperty, null);
            UpdateInspector();
            Redraw();
            SynchronizeStructureSelection();
        }
        else if (change.Property == SelectedColumnProperty)
        {
            if (SelectedColumn is not null && Document?.Columns.Contains(SelectedColumn) != true)
            {
                SetCurrentValue(SelectedColumnProperty, null);
                return;
            }

            State.SelectedColumn = SelectedColumn;
            UpdateInspector();
            Redraw();
        }
    }

    private void Observe()
    {
        _observed = Document;
        if (_observed is null) return;
        _observed.PropertyChanged += DefinitionChanged;
        _observed.Columns.CollectionChanged += ColumnsChanged;
        _observed.Elements.CollectionChanged += ElementsChanged;
        ObserveElements();
        ObserveColumns();
        _lastSnapshot = CaptureHistorySource(Copy(_observed));
    }

    private void Unobserve()
    {
        if (_observed is not null)
        {
            _observed.PropertyChanged -= DefinitionChanged;
            _observed.Columns.CollectionChanged -= ColumnsChanged;
        }

        UnobserveElements();
        if (_observed is not null) _observed.Elements.CollectionChanged -= ElementsChanged;
        foreach (var column in _columns) column.PropertyChanged -= DefinitionChanged;
        _columns.Clear();
        _observed = null;
    }

    private void ObserveColumns()
    {
        if (_observed is null) return;
        foreach (var removed in _columns.Where(c => !_observed.Columns.Contains(c)).ToArray())
        {
            removed.PropertyChanged -= DefinitionChanged;
            _columns.Remove(removed);
        }

        foreach (var column in _observed.Columns)
            if (_columns.Add(column))
                column.PropertyChanged += DefinitionChanged;
    }

    private void ColumnsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ObserveColumns();
        if (SelectedColumn is null || Document?.Columns.Contains(SelectedColumn) != true)
            SetCurrentValue(SelectedColumnProperty, Document?.Columns.LastOrDefault());
        DefinitionChanged(sender, new PropertyChangedEventArgs(nameof(ReportDocument.Columns)));
    }

    private void DefinitionChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_batch) return;
        State.SaveStatus = "未保存";
        if (!_restoring && _lastSnapshot is not null)
        {
            _undo.Push(CaptureHistorySource(_lastSnapshot));
            _redo.Clear();
        }

        _lastSnapshot = Document is null ? null : CaptureHistorySource(Copy(Document));
        if (e.PropertyName is nameof(ReportElement.Name) or nameof(ReportElement.Y) or nameof(ReportColumn.Header)
            or nameof(ReportColumn.ShowSum)
            or nameof(SelectedElement) or nameof(ReportDocument.Elements) or nameof(ReportDocument.Columns)
            or nameof(Document)) RefreshStructure();
        Rebuild();
        if (e.PropertyName == nameof(ReportElement.ImageUrl)) _ = RefreshRemoteImagesAsync();
    }

    private void StateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReportDesignState.IsDataTab))
        {
            if (State.IsDataTab) RestoreDataTreeState();
            else CaptureDataTreeState();
        }

        if (e.PropertyName == nameof(ReportDesignState.SelectedColumn) &&
            !ReferenceEquals(SelectedColumn, State.SelectedColumn))
            SetCurrentValue(SelectedColumnProperty, State.SelectedColumn);
        if (e.PropertyName == nameof(ReportDesignState.SelectedElement) &&
            !ReferenceEquals(SelectedElement, State.SelectedElement))
            SetCurrentValue(SelectedElementProperty, State.SelectedElement);
        if (e.PropertyName == nameof(ReportDesignState.SelectedBody) && !_fetchUpdating) SelectApiBody();
        if (e.PropertyName == nameof(ReportDesignState.Snap)) PageSurface.Snap = State.Snap;
        if (e.PropertyName == nameof(ReportDesignState.Zoom)) Redraw();
        if (e.PropertyName == nameof(ReportDesignState.IsPreview)) Rebuild();
    }

    /// <summary>Re-evaluates registered getters and repaginates. Source row membership is a creation-time snapshot.</summary>
    public void RefreshReport()
    {
        Rebuild();
    }

    public void ShowPreview()
    {
        if (State.IsPreview) Rebuild();
        else State.IsPreview = true;
    }

    public void ShowDesign()
    {
        if (!State.IsPreview) Rebuild();
        else State.IsPreview = false;
    }

    public void GoToPage(int pageNumber)
    {
        _pageIndex = Math.Clamp(pageNumber - 1, 0, Math.Max(0, (Layout?.Pages.Count ?? 1) - 1));
        Redraw();
        PageScroll.Offset = default;
    }

    public void BindField(string fieldKey)
    {
        if (DataSource is null) return;
        if (SelectedElement is { Kind: not ReportElementKind.Table } element)
        {
            if (element.Kind is not (ReportElementKind.Text or ReportElementKind.CheckBox
                or ReportElementKind.Barcode)) return;
            if (!DataSource.Fields.Any(f => f.Key == fieldKey)) throw new ArgumentException("字段不存在。", nameof(fieldKey));
            element.FieldKey = fieldKey;
            State.PropertyTab = 1;
            return;
        }

        if (SelectedColumn is null) return;
        var field = DataSource.Fields.FirstOrDefault(f => f.Key == fieldKey) ??
                    throw new ArgumentException($"Unknown report field: {fieldKey}", nameof(fieldKey));
        if (SelectedColumn.FieldKey == field.Key) return;
        var previousBatch = _batch;
        _batch = true;
        try
        {
            SelectedColumn.FieldKey = field.Key;
            SelectedColumn.Header = field.Label;
            SelectedColumn.Format = ReportValueFormat.Automatic;
            if (field.Kind != ReportValueKind.Number)
            {
                SelectedColumn.ShowSum = false;
                SelectedColumn.ConditionEnabled = false;
            }

            if (field.Kind == ReportValueKind.Number) SelectedColumn.DecimalPlaces = 2;
        }
        finally
        {
            _batch = previousBatch;
        }

        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(SelectedField)));
    }

    private void Rebuild()
    {
        if (!_ready) return;
        State.SourceLabel = DataSource is null ? "未连接数据源" : $"{DataSource.Name} · {DataSource.RowCount:N0} 条记录";
        try
        {
            if (Document is null) throw new ArgumentException("请创建报表。");
            Layout = ReportPaginator.Paginate(Document, DataSource);
            ValidationError = null;
            State.Status = State.IsPreview
                ? $"{DataSource?.RowCount ?? 0} 条数据 · {Layout.Pages.Count} 页"
                : $"{Document.Elements.Count} 个元素 · 拖动排版 / 字段拖放绑定 · " +
                  (DataSource is null ? "未连接数据源（可选）" : State.SourceLabel);
        }
        catch (ArgumentException exception)
        {
            Layout = null;
            ValidationError = exception.Message;
            State.Status = exception.Message;
        }

        _pageIndex = Math.Clamp(_pageIndex, 0, Math.Max(0, (Layout?.Pages.Count ?? 1) - 1));
        State.CanUndo = _undo.Count > 0;
        State.CanRedo = _redo.Count > 0;
        State.PaperLabel = Document is null ? "未设置纸张" :
            Document.PageWidthMm == 210 && Document.PageHeightMm == 297 ? "A4 · 纵向    │    单位：mm" :
            Document.PageWidthMm == 297 && Document.PageHeightMm == 210 ? "A4 · 横向    │    单位：mm" :
            $"{Document.PageWidthMm:0} × {Document.PageHeightMm:0} mm";
        UpdateInspector();
        Redraw();
        if (State.Structure.Count == 0 || !ReferenceEquals(_structureDocument, Document)
                                       || (Document is { } document &&
                                           (_structureHeaderBottom != document.HeaderBottom ||
                                            _structureFooterTop != document.FooterTop))) RefreshStructure();
    }

    private void UpdateInspector()
    {
        _syncing = true;
        try
        {
            State.SelectedColumn = SelectedColumn;
            State.SourceName = DataSource?.Name ?? "未连接";
            RaisePropertyChanged(ImageFitIndexProperty, -1, ImageFitIndex);
            RaisePropertyChanged(SumIndexProperty, -1, SumIndex);
            RaisePropertyChanged(ConditionIndexProperty, -1, ConditionIndex);
            RaisePropertyChanged(FontFamilyNameProperty, "", FontFamilyName);
            if (!State.FontSizes.Contains(FontSizeValue))
                State.FontSizes.Insert(State.FontSizes.TakeWhile(size => size < FontSizeValue).Count(), FontSizeValue);
            RaisePropertyChanged(FontSizeValueProperty, -1, FontSizeValue);
            RaisePropertyChanged(SelectedFieldProperty, null, SelectedField);
            RaisePropertyChanged(FormatIndexProperty, -1, FormatIndex);
            State.BindingStatus = SelectedField is null ? "尚未绑定有效字段" : $"{DataSource?.Name}.{SelectedField.Key}";
            State.FormatSample = SelectedColumn is null || SelectedField is null || DataSource?.RowCount is not > 0
                ? "无示例数据"
                : ReportPaginator.FormatValue(DataSource.GetValue(0, SelectedField.Key), SelectedColumn,
                    SelectedField.Kind);
        }
        catch (Exception)
        {
            State.FormatSample = "当前字段无法使用此格式";
        }
        finally
        {
            _syncing = false;
        }
    }

    private void Redraw()
    {
        if (!_ready) return;
        var count = Layout?.Pages.Count ?? 0;
        State.PageStatus = count == 0 ? "尚无可预览页面" :
            State.IsPreview ? $"第 {_pageIndex + 1} / {count} 页" : $"设计模板 · {count} 页预览";
        State.CanPrevious = State.IsPreview && _pageIndex > 0;
        State.CanNext = State.IsPreview && _pageIndex + 1 < count;
        PageSurface.SetDocument(Document, SelectedElement);
        PageSurface.SourceName = DataSource?.Name ?? "";
        PageSurface.Update(Layout, State.IsPreview, _pageIndex,
            Document is null ? -1 : Document.Columns.IndexOf(SelectedColumn!), State.Zoom, ValidationError);
    }

    private void FilterFields()
    {
        var search = FieldSearch?.Text?.Trim() ?? "";
        State.Fields.Clear();
        foreach (var field in SourceFields.Where(f =>
                     f.Key.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                     f.Label.Contains(search, StringComparison.OrdinalIgnoreCase))) State.Fields.Add(field);
        RefreshDataTree();
    }

    private void OnSearch(object? sender, TextChangedEventArgs e)
    {
        if (_ready) FilterFields();
    }

    private void OnBindField(object? sender, RoutedEventArgs e)
    {
        if (State.PendingField is not null && !IsPreview) BindField(State.PendingField.Key);
    }

    private void OnFieldDoubleTapped(object? sender, TappedEventArgs e)
    {
        OnBindField(sender, e);
    }

    private void OnColumnSelection(object? sender, SelectionChangedEventArgs e)
    {
        if (_ready) UpdateInspector();
    }

    private void OnDesign(object? sender, RoutedEventArgs e)
    {
        ShowDesign();
    }

    private void OnPreview(object? sender, RoutedEventArgs e)
    {
        ShowPreview();
    }

    private void OnPreviousPage(object? sender, RoutedEventArgs e)
    {
        GoToPage(CurrentPageNumber - 1);
    }

    private void OnNextPage(object? sender, RoutedEventArgs e)
    {
        GoToPage(CurrentPageNumber + 1);
    }

    private void OnFitPage(object? sender, RoutedEventArgs e)
    {
        if (Layout is not null)
            Zoom = Math.Min((PageScroll.Viewport.Width - 120) / Layout.PageWidth,
                (PageScroll.Viewport.Height - 60) / Layout.PageHeight);
    }

    private void OnAddColumn(object? sender, RoutedEventArgs e)
    {
        if (Document is null || IsPreview) return;
        var field = State.PendingField ?? DataSource?.Fields.FirstOrDefault();
        var column = new ReportColumn { Header = field?.Label ?? "新列", FieldKey = field?.Key ?? "" };
        Document.Columns.Add(column);
        SetCurrentValue(SelectedColumnProperty, column);
    }

    private void OnRemoveColumn(object? sender, RoutedEventArgs e)
    {
        if (!IsPreview && SelectedColumn is { } column) Document?.Columns.Remove(column);
    }

    private void OnUndo(object? sender, RoutedEventArgs e)
    {
        Restore(_undo, _redo);
    }

    private void OnRedo(object? sender, RoutedEventArgs e)
    {
        Restore(_redo, _undo);
    }

    private void Restore(Stack<ReportDocument> from, Stack<ReportDocument> to)
    {
        if (from.Count == 0 || Document is null) return;
        var index = Document.Columns.IndexOf(SelectedColumn!);
        var elementIndex = Document.Elements.IndexOf(SelectedElement!);
        to.Push(CaptureHistorySource(Copy(Document)));
        var restored = from.Pop();
        _restoring = true;
        _batch = true;
        try
        {
            // Keep the consumer's document instance, including an existing compiled binding to it.
            Document.Title = restored.Title;
            Document.Subtitle = restored.Subtitle;
            Document.Description = restored.Description;
            Document.Footer = restored.Footer;
            Document.PageWidthMm = restored.PageWidthMm;
            Document.PageHeightMm = restored.PageHeightMm;
            Document.MarginMm = restored.MarginMm;
            Document.MarginLeftMm = restored.MarginLeftMm;
            Document.MarginTopMm = restored.MarginTopMm;
            Document.MarginRightMm = restored.MarginRightMm;
            Document.MarginBottomMm = restored.MarginBottomMm;
            Document.HeaderHeightMm = restored.HeaderHeightMm;
            Document.FooterHeightMm = restored.FooterHeightMm;
            Document.DataSourceUrl = restored.DataSourceUrl;
            Document.DataBodyPointer = restored.DataBodyPointer;
            Document.DataSourceMethod = restored.DataSourceMethod;
            Document.DataSourceParameters = restored.DataSourceParameters;
            Document.DataSourceParameterMode = restored.DataSourceParameterMode;
            Document.DataSourceAuthentication = restored.DataSourceAuthentication;
            Document.FontSize = restored.FontSize;
            Document.MinimumRowHeight = restored.MinimumRowHeight;
            Document.Columns.Clear();
            foreach (var column in restored.Columns) Document.Columns.Add(column);
            Document.Elements.Clear();
            foreach (var element in restored.Elements) Document.Elements.Add(element);
            _lastSnapshot = Copy(Document);
        }
        finally
        {
            _restoring = false;
            _batch = false;
        }

        SetCurrentValue(SelectedElementProperty, Document.Elements.ElementAtOrDefault(Math.Max(0, elementIndex)));
        SetCurrentValue(SelectedColumnProperty, Document.Columns.ElementAtOrDefault(Math.Max(0, index)));
        RestoreApiDataSource(restored);
        RefreshStructure();
        Rebuild();
        if (Document.Elements.Any(e =>
                e.ImageUrl.Length > 0 && (e.ResolvedImageUrl != e.ImageUrl || e.ResolvedImageData.Length == 0)))
            _ = RefreshRemoteImagesAsync();
    }

    internal static ReportDocument Copy(ReportDocument source)
    {
        var copy = new ReportDocument
        {
            Title = source.Title, Subtitle = source.Subtitle, Description = source.Description, Footer = source.Footer,
            PageWidthMm = source.PageWidthMm, PageHeightMm = source.PageHeightMm, MarginMm = source.MarginMm,
            FontSize = source.FontSize, MinimumRowHeight = source.MinimumRowHeight,
            MarginLeftMm = source.MarginLeftMm, MarginTopMm = source.MarginTopMm, MarginRightMm = source.MarginRightMm,
            MarginBottomMm = source.MarginBottomMm,
            HeaderHeightMm = source.HeaderHeightMm, FooterHeightMm = source.FooterHeightMm,
            DataSourceUrl = source.DataSourceUrl, DataBodyPointer = source.DataBodyPointer,
            DataSourceMethod = source.DataSourceMethod, DataSourceParameters = source.DataSourceParameters,
            DataSourceParameterMode = source.DataSourceParameterMode,
            DataSourceAuthentication = source.DataSourceAuthentication
        };
        foreach (var c in source.Columns)
            copy.Columns.Add(new ReportColumn
            {
                Header = c.Header, FieldKey = c.FieldKey, Text = c.Text, WidthWeight = c.WidthWeight, Format = c.Format,
                DecimalPlaces = c.DecimalPlaces, UseThousandsSeparator = c.UseThousandsSeparator, ShowSum = c.ShowSum,
                ConditionEnabled = c.ConditionEnabled, Condition = c.Condition, ConditionValue = c.ConditionValue,
                ConditionColor = c.ConditionColor
            });
        foreach (var e in source.Elements)
            copy.Elements.Add(new ReportElement
            {
                Name = e.Name, Kind = e.Kind, X = e.X, Y = e.Y, Width = e.Width, Height = e.Height, Text = e.Text,
                FieldKey = e.FieldKey, FontSize = e.FontSize, Bold = e.Bold, Foreground = e.Foreground, Fill = e.Fill,
                ShowBorder = e.ShowBorder,
                FontFamily = e.FontFamily, Italic = e.Italic, Underline = e.Underline, TextAlignment = e.TextAlignment,
                ListStyle = e.ListStyle, ImageData = e.ImageData, ImageName = e.ImageName, ImageFit = e.ImageFit,
                ImageUrl = e.ImageUrl, ResolvedImageUrl = e.ResolvedImageUrl, ResolvedImageData = e.ResolvedImageData,
                TableHeaderHeightMm = e.TableHeaderHeightMm,
                IsChecked = e.IsChecked, BarcodeShowText = e.BarcodeShowText
            });
        return copy;
    }
}