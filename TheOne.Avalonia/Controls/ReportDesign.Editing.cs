using System.Collections.Specialized;
using System.ComponentModel;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    public static readonly StyledProperty<ReportElement?> SelectedElementProperty =
        AvaloniaProperty.Register<ReportDesign, ReportElement?>(nameof(SelectedElement),
            defaultBindingMode: BindingMode.TwoWay);

    private readonly HashSet<ReportElement> _elements = [];
    private string? _apiJson;
    private ReportApiRequest? _apiRequest;
    private object? _dragPayload;
    private Point _dragStart;
    private bool _dragging, _fetchUpdating;
    private CancellationTokenSource? _fetchCancellation;
    private long _fetchGeneration;

    public ReportElement? SelectedElement
    {
        get => GetValue(SelectedElementProperty);
        set => SetValue(SelectedElementProperty, value);
    }

    private void InitializeDesigner()
    {
        PageSurface.ElementSelected += element => SetCurrentValue(SelectedElementProperty, element);
        PageSurface.GestureStarting += () => _batch = true;
        PageSurface.GestureFinished += () =>
        {
            _batch = false;
            DefinitionChanged(this, new PropertyChangedEventArgs(nameof(SelectedElement)));
        };
        PageSurface.DeleteRequested += () => OnDeleteElement(this, new RoutedEventArgs());
        AddHandler(PointerPressedEvent, PalettePressed, RoutingStrategies.Tunnel, true);
        AddHandler(PointerMovedEvent, PaletteMoved, RoutingStrategies.Tunnel, true);
        AddHandler(PointerReleasedEvent, PaletteReleased, RoutingStrategies.Tunnel, true);
        AddHandler(PointerCaptureLostEvent, (_, e) =>
        {
            if (ReferenceEquals(e.Source, this) && _dragging)
            {
                _dragPayload = null;
                _dragging = false;
                PageSurface.ShowDropHint(null);
            }
        }, RoutingStrategies.Bubble);
    }

    private void ObserveElements()
    {
        if (_observed is null) return;
        foreach (var removed in _elements.Where(e => !_observed.Elements.Contains(e)).ToArray())
        {
            removed.PropertyChanged -= DefinitionChanged;
            _elements.Remove(removed);
        }

        foreach (var element in _observed.Elements)
            if (_elements.Add(element))
                element.PropertyChanged += DefinitionChanged;
    }

    private void UnobserveElements()
    {
        foreach (var element in _elements) element.PropertyChanged -= DefinitionChanged;
        _elements.Clear();
    }

    private void ElementsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ObserveElements();
        if (SelectedElement is not null && Document?.Elements.Contains(SelectedElement) != true)
            SetCurrentValue(SelectedElementProperty, null);
        DefinitionChanged(sender, new PropertyChangedEventArgs(nameof(ReportDocument.Elements)));
    }

    /// <summary>Adds an unbound component at a page position in millimetres. A report supports one repeating table.</summary>
    public ReportElement AddElement(ReportElementKind kind, double x, double y)
    {
        if (Document is null) SetCurrentValue(DocumentProperty, new ReportDocument());
        if (!double.IsFinite(x) || !double.IsFinite(y)) throw new ArgumentOutOfRangeException(nameof(x));
        if (kind == ReportElementKind.Table && Document!.Elements.FirstOrDefault(e => e.Kind == kind) is { } existing)
        {
            SetCurrentValue(SelectedElementProperty, existing);
            return existing;
        }

        var title = kind switch
        {
            ReportElementKind.Table => "表格", ReportElementKind.Line => "线条", ReportElementKind.Rectangle => "矩形",
            ReportElementKind.PageNumber => "页码", ReportElementKind.Image => "图片",
            ReportElementKind.CheckBox => "复选框", ReportElementKind.Barcode => "条码", _ => "文本"
        };
        var width = kind == ReportElementKind.Table
            ?
            Document!.PageWidthMm - Document.LeftMargin - Document.RightMargin
            : kind == ReportElementKind.Line
                ? 80
                : 60;
        var height = kind == ReportElementKind.Table ? 24 :
            kind == ReportElementKind.Line ? 2 :
            kind == ReportElementKind.Image ? 40 :
            kind == ReportElementKind.Barcode ? 22 : 12;
        var element = new ReportElement
        {
            Name = title + (Document!.Elements.Count + 1), Kind = kind,
            X = Math.Clamp(x, 0, Math.Max(0, Document.PageWidthMm - width)),
            Y = Math.Clamp(y, 0, Math.Max(0, Document.PageHeightMm - height)), Width = width, Height = height,
            Text = kind == ReportElementKind.Text ? "请输入文本" :
                kind == ReportElementKind.Barcode ? "12345678" :
                kind == ReportElementKind.CheckBox ? "复选框" : "",
            FontSize = 16, Foreground = kind == ReportElementKind.Table ? "#FFFFFF" : "#212222",
            Fill = kind == ReportElementKind.Table ? "#006061" : "Transparent",
            ShowBorder = kind is ReportElementKind.Rectangle or ReportElementKind.Table
        };
        if (kind == ReportElementKind.Table)
            element.Y = Math.Clamp(element.Y, Document.HeaderBottom,
                Math.Max(Document.HeaderBottom, Document.FooterTop - height));
        if (kind == ReportElementKind.Barcode) element.TextAlignment = ReportTextAlignment.Center;
        var previousBatch = _batch;
        _batch = true;
        try
        {
            if (kind == ReportElementKind.Table && Document.Columns.Count == 0)
                for (var i = 0; i < 3; i++)
                    Document.Columns.Add(new ReportColumn { Header = "列 " + (i + 1), Text = "" });
            Document.Elements.Add(element);
        }
        finally
        {
            _batch = previousBatch;
        }

        SetCurrentValue(SelectedElementProperty, element);
        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
        return element;
    }

    private void PalettePressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsPreview || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed ||
            e.Source is not Visual visual) return;
        var ancestors = visual.GetVisualAncestors().Prepend(visual).ToArray();
        var button = ancestors.OfType<Button>()
            .FirstOrDefault(b => b.Name?.EndsWith("Palette", StringComparison.Ordinal) == true);
        var field = ancestors.OfType<ListBoxItem>().FirstOrDefault(i =>
            i.DataContext is ReportFieldInfo &&
            i.GetVisualAncestors().OfType<ListBox>().Any(list => list.Name == "FieldList"));
        var node = ancestors.OfType<TreeViewItem>().FirstOrDefault(i =>
            i.DataContext is ReportDataNode &&
            i.GetVisualAncestors().OfType<TreeView>().Any(tree => tree.Name == "DataTree"));
        _dragPayload = ancestors.OfType<ToggleButton>().Any()
            ? null
            : node?.DataContext ?? field?.DataContext ?? button?.Tag;
        _dragStart = e.GetPosition(this);
        _dragging = false;
    }

    private void PaletteMoved(object? sender, PointerEventArgs e)
    {
        if (_dragPayload is null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        var delta = e.GetPosition(this) - _dragStart;
        if (!_dragging && delta.X * delta.X + delta.Y * delta.Y < 25) return;
        _dragging = true;
        e.Pointer.Capture(this);
        PageSurface.ShowDropHint(PageSurface.PagePoint(e.GetPosition(PageSurface)));
        e.Handled = true;
    }

    private async void PaletteReleased(object? sender, PointerReleasedEventArgs e)
    {
        var payload = _dragPayload;
        var dragging = _dragging;
        _dragPayload = null;
        _dragging = false;
        if (!dragging || payload is null) return;
        var point = PageSurface.PagePoint(e.GetPosition(PageSurface));
        e.Pointer.Capture(null);
        PageSurface.ShowDropHint(null);
        e.Handled = true;
        if (point is null) return;
        if (payload is string kind && Enum.TryParse<ReportElementKind>(kind, out var elementKind))
            AddElement(elementKind, point.Value.X, point.Value.Y);
        else if (payload is ReportFieldInfo field) DropField(field.Key, point.Value.X, point.Value.Y);
        else if (payload is ReportDataNode node)
            try
            {
                await DropDataNodeAsync(node, point.Value.X, point.Value.Y);
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
            {
                State.ApiStatus = "无法绑定此节点：" + exception.Message;
                State.HasApiError = true;
            }
    }

    /// <summary>Binds a dropped field to the hit table detail cell or text element; empty paper creates a bound text element.</summary>
    public void DropField(string fieldKey, double x, double y)
    {
        if (DataSource?.Fields.Any(f => f.Key == fieldKey) != true)
            throw new ArgumentException("字段不存在。", nameof(fieldKey));
        var point = new Point(x, y);
        var element = PageSurface.ElementAt(point);
        if (element is
            {
                Kind: ReportElementKind.Line or ReportElementKind.Rectangle or ReportElementKind.PageNumber
            }) return;
        var previousBatch = _batch;
        _batch = true;
        try
        {
            element ??= AddElement(ReportElementKind.Text, x, y);
            SetCurrentValue(SelectedElementProperty, element);
            if (element.Kind == ReportElementKind.Table && Document is not null)
            {
                var index = PageSurface.ColumnAt(element, point);
                if (index >= 0) SetCurrentValue(SelectedColumnProperty, Document.Columns[index]);
            }

            BindField(fieldKey);
            State.PropertyTab = 1;
        }
        finally
        {
            _batch = previousBatch;
        }

        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
    }

    /// <summary>GET a JSON endpoint and expose its actual object, scalar and collection nodes for binding.</summary>
    public Task FetchApiDataAsync(string url, CancellationToken cancellationToken = default)
    {
        return FetchApiDataAsync(new ReportApiRequest(url), cancellationToken);
    }

    /// <summary>Fetches a configured API request and imports its response tree without changing existing bindings.</summary>
    public async Task FetchApiDataAsync(ReportApiRequest request, CancellationToken cancellationToken = default)
    {
        _pendingDocumentData = false;
        _loadingDataDocument = null;
        _fetchCancellation?.Cancel();
        _fetchCancellation?.Dispose();
        _fetchCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _fetchCancellation.CancelAfter(TimeSpan.FromSeconds(20));
        var token = _fetchCancellation.Token;
        var generation = ++_fetchGeneration;
        State.IsFetching = true;
        State.ApiStatus = "正在获取接口响应…";
        try
        {
            var json = await ReportResources.FetchJsonAsync(request, cancellationToken: token);
            if (generation != _fetchGeneration) return;
            ImportApiResponse(request, json);
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException
                                              or OperationCanceledException or ArgumentException or TimeoutException)
        {
            if (generation == _fetchGeneration)
            {
                State.ApiStatus = exception is OperationCanceledException
                    ? "请求取消或超过 20 秒；原设计和已绑定数据保留。"
                    : "获取失败：" + exception.Message;
                State.HasApiError = true;
            }
        }
        finally
        {
            if (generation == _fetchGeneration) State.IsFetching = false;
        }
    }

    private void ImportApiResponse(ReportApiRequest request, string json)
    {
        var bodies = ReportJsonData.FindBodies(json);
        State.ApiUrl = request.Url;
        _apiRequest = request;
        _apiJson = json;
        _fetchUpdating = true;
        try
        {
            State.SelectedBody = null;
            State.Bodies.Clear();
            foreach (var body in bodies) State.Bodies.Add(body);
            State.ResponsePreview = json.Length > 4000 ? json[..4000] + "…" : json;
        }
        finally
        {
            _fetchUpdating = false;
        }

        RefreshDataTree();
        State.ApiStatus = "已添加数据源；拖入单值填充，拖入集合配置表格。";
    }

    private void ApplyApiRequest(ReportDocument document)
    {
        var request = _apiRequest ?? new ReportApiRequest(State.ApiUrl);
        document.DataSourceUrl = request.Url;
        document.DataSourceMethod = request.Method;
        document.DataSourceParameters = request.Parameters;
        document.DataSourceParameterMode = request.ParameterMode;
        document.DataSourceAuthentication = request.Authentication;
    }

    private void SelectApiBody()
    {
        if (_apiJson is null || State.SelectedBody is null) return;
        try
        {
            SetCurrentValue(DataSourceProperty, ReportJsonData.CreateSource(_apiJson, State.SelectedBody.Pointer));
            if (Document is not null)
            {
                ApplyApiRequest(Document);
                Document.DataBodyPointer = State.SelectedBody.Pointer;
            }

            State.ApiStatus = $"已选择 {State.SelectedBody.Label}，可拖动字段到报表。";
        }
        catch (ArgumentException exception)
        {
            State.ApiStatus = exception.Message;
            State.HasApiError = true;
        }
    }

    private async void OnFetchData(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this) is not Window owner) return;
        var document = Document;
        var dialog = new ReportDataSourceDialog(_apiRequest ??
                                                (document is null
                                                    ? new ReportApiRequest(State.ApiUrl)
                                                    : ReportApiRequest.FromDocument(document)));
        if (await dialog.ShowDialog<ReportFetchedData?>(owner) is { } fetched && ReferenceEquals(document, Document))
        {
            _fetchCancellation?.Cancel();
            ++_fetchGeneration;
            State.IsFetching = false;
            ImportApiResponse(fetched.Request, fetched.Json);
        }
    }

    private void OnDataTab(object? sender, RoutedEventArgs e)
    {
        State.IsDataTab = true;
    }

    private void OnComponentsTab(object? sender, RoutedEventArgs e)
    {
        State.IsDataTab = false;
    }

    private void OnPropertyTab(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { Tag: string value } && int.TryParse(value, out var index)) State.PropertyTab = index;
    }

    private async void OnPageSettings(object? sender, RoutedEventArgs e)
    {
        if (Document is not { } document || TopLevel.GetTopLevel(this) is not Window owner) return;
        var dialog = new ReportPageSettingsDialog(document);
        if (await dialog.ShowDialog<ReportDocument?>(owner) is not { } settings ||
            !ReferenceEquals(document, Document)) return;
        _batch = true;
        try
        {
            var footerDelta = settings.FooterTop - document.FooterTop;
            var footerTop = document.FooterTop;
            foreach (var element in document.Elements.Where(e => e.Kind != ReportElementKind.Table && e.Y >= footerTop))
                element.Y += footerDelta;
            document.PageWidthMm = settings.PageWidthMm;
            document.PageHeightMm = settings.PageHeightMm;
            document.MarginLeftMm = settings.LeftMargin;
            document.MarginRightMm = settings.RightMargin;
            document.MarginTopMm = settings.TopMargin;
            document.MarginBottomMm = settings.BottomMargin;
            document.HeaderHeightMm = settings.HeaderHeightMm;
            document.FooterHeightMm = settings.FooterHeightMm;
        }
        finally
        {
            _batch = false;
        }

        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
    }

    private void OnInsert(object? sender, RoutedEventArgs e)
    {
        if (!IsPreview && sender is Control { Tag: string kind } &&
            Enum.TryParse<ReportElementKind>(kind, out var parsed))
            AddElement(parsed, Document?.MarginMm ?? 14, 40 + (Document?.Elements.Count ?? 0) * 8);
    }

    private void OnDeleteElement(object? sender, RoutedEventArgs e)
    {
        if (IsPreview || SelectedElement is null || Document is null) return;
        var selected = SelectedElement;
        _batch = true;
        try
        {
            Document.Elements.Remove(selected);
            if (selected.Kind == ReportElementKind.Table) Document.Columns.Clear();
        }
        finally
        {
            _batch = false;
        }

        SetCurrentValue(SelectedElementProperty, null);
        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
    }

    private void OnAlignLeft(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is not null && Document is not null) SelectedElement.X = Document.MarginMm;
    }

    private void OnAlignCenter(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is not null && Document is not null)
            SelectedElement.X = (Document.PageWidthMm - SelectedElement.Width) / 2;
    }

    private void OnClearBinding(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is { Kind: not ReportElementKind.Table } element)
        {
            element.FieldKey = "";
        }
        else if (SelectedColumn is not null)
        {
            _batch = true;
            SelectedColumn.FieldKey = "";
            SelectedColumn.ShowSum = false;
            SelectedColumn.ConditionEnabled = false;
            _batch = false;
            DefinitionChanged(this, new PropertyChangedEventArgs(nameof(SelectedField)));
        }
    }

    private void OnDisconnect(object? sender, RoutedEventArgs e)
    {
        SetCurrentValue(DataSourceProperty, null);
    }

    private void OnNew(object? sender, RoutedEventArgs e)
    {
        ShowDesign();
        SetCurrentValue(DocumentProperty, new ReportDocument());
        SetCurrentValue(SelectedElementProperty, null);
    }

    private void OnExample(object? sender, RoutedEventArgs e)
    {
        ShowDesign();
        SetCurrentValue(DocumentProperty, ReportTemplate.CreateSalesDetail());
        SetCurrentValue(SelectedElementProperty, Document!.Elements.Single(e => e.Kind == ReportElementKind.Table));
        SetCurrentValue(SelectedColumnProperty, Document.Columns[^1]);
        State.PropertyTab = 1;
    }
}