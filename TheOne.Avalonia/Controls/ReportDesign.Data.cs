using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    private readonly ConditionalWeakTable<ReportDocument, SourceSnapshot> _historySources = new();
    private IReadOnlyList<ReportDataNode> _dataRoots = [];
    private string? _treeJson;
    private string? _treeSearch;
    private ReportDataSource? _treeSource;
    private ReportApiRequest? _treeRequest;

    private void RefreshDataTree()
    {
        var rootsChanged = false;
        if (!ReferenceEquals(_treeJson, _apiJson) || !ReferenceEquals(_treeRequest, _apiRequest) ||
            (_apiJson is null && !ReferenceEquals(_treeSource, DataSource)))
        {
            rootsChanged = true;
            _treeJson = _apiJson;
            _treeSource = DataSource;
            _treeRequest = _apiRequest;
            if (_apiJson is not null)
            {
                _dataRoots = ReportDataCatalog.Tree(_apiJson, _apiRequest);
            }
            else if (DataSource is { } source)
            {
                var children = source.Fields.Select(field => new ReportDataNode(field.Label, "",
                    ReportDataNodeKind.Scalar,
                    DataSample(source, field.Key),
                    [], field.Key, false)).ToArray();
                _dataRoots =
                [
                    new ReportDataNode(source.Name, "", ReportDataNodeKind.Collection, source.RowCount + " 条记录",
                        children, isJson: false, expanded: true)
                ];
            }
            else
            {
                _dataRoots = [];
            }
        }

        _treeSource = DataSource;
        var search = FieldSearch?.Text?.Trim() ?? "";
        if (!rootsChanged && search == _treeSearch) return;
        _treeSearch = search;

        ReportDataNode? Filter(ReportDataNode node)
        {
            if (search.Length == 0 || node.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                node.Preview.Contains(search, StringComparison.OrdinalIgnoreCase)) return node;
            var children = node.Children.Select(Filter).OfType<ReportDataNode>().ToArray();
            return children.Length == 0
                ? null
                : new ReportDataNode(node.Name, node.Pointer, node.Kind, node.Preview, children, node.FieldKey,
                    node.IsJson, true, node.TypeLabel);
        }

        var filtered = _dataRoots.Select(Filter).OfType<ReportDataNode>().ToArray();
        if (filtered.SequenceEqual(State.DataNodes)) return;
        ResetDataTreeState();
        State.DataNodes.Clear();
        foreach (var node in filtered) State.DataNodes.Add(node);
    }

    private IEnumerable<string> RootBindingKeys()
    {
        return Document is null
            ? []
            : Document.Elements.Select(element => element.FieldKey)
                .Concat(Document.Columns.Select(column => column.FieldKey)).Where(ReportJsonData.IsRootField);
    }

    private static string DataSample(ReportDataSource source, string key)
    {
        if (source.RowCount == 0) return "无数据";
        try
        {
            var text = Convert.ToString(source.GetValue(0, key), CultureInfo.CurrentCulture) ?? "null";
            return text.Length <= 120 ? text : text[..120] + "…";
        }
        catch (Exception)
        {
            return "读取失败";
        }
    }

    private ReportDocument CaptureHistorySource(ReportDocument snapshot)
    {
        if (!_historySources.TryGetValue(snapshot, out _))
        {
            _historySources.Add(snapshot, CaptureSource());
            RememberHistorySelection(snapshot);
        }
        return snapshot;
    }

    private void UpdateHistorySource()
    {
        if (_lastSnapshot is not { } snapshot) return;
        _historySources.Remove(snapshot);
        _historySources.Add(snapshot, CaptureSource());
    }

    private void RememberApiSource()
    {
        if (_lastSnapshot is { } snapshot) CaptureHistorySource(snapshot);
    }

    private void RestoreApiDataSource(ReportDocument snapshot)
    {
        if (_historySources.TryGetValue(snapshot, out var saved))
        {
            SetCurrentValue(DataSourceProperty, saved.Source);
            _apiRequest = saved.Request;
            _apiJson = saved.Json;
            State.ApiUrl = saved.Request?.Url ?? snapshot.DataSourceUrl;
        }
        if (_apiRequest is not null && _apiRequest.Url == snapshot.DataSourceUrl)
        {
            _apiRequest = ReportApiRequest.FromDocument(snapshot);
        }
        RefreshDataTree();
        UpdateHistorySource();
    }

    private static void AdjustColumnType(ReportColumn column, ReportValueKind? kind)
    {
        if (kind != ReportValueKind.Number)
        {
            column.ShowSum = false;
            column.ConditionEnabled = false;
            if (kind is not null && column.Format is ReportValueFormat.Number or ReportValueFormat.Currency)
                column.Format = ReportValueFormat.Automatic;
        }

        if (kind is not null && kind != ReportValueKind.Date && column.Format == ReportValueFormat.Date)
            column.Format = ReportValueFormat.Automatic;
    }

    /// <summary>Drops an actual JSON node or typed source field at a paper position in millimetres.</summary>
    public async Task DropDataNodeAsync(ReportDataNode node, double x, double y)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (IsPreview || Document is not { } document || node.Kind == ReportDataNodeKind.Object ||
            !double.IsFinite(x) || !double.IsFinite(y)) return;
        var point = new Point(x, y);
        var element = PageSurface.ElementAt(point);
        if (node.Kind == ReportDataNodeKind.Collection)
        {
            if (element?.Kind != ReportElementKind.Table || TopLevel.GetTopLevel(this) is not Window owner) return;
            var apiJson = _apiJson;
            if (node.IsJson && apiJson is null) return;
            var source = node.IsJson
                ? ReportJsonData.CreateSource(apiJson!, node.Pointer, RootBindingKeys())
                : DataSource;
            if (source is null) return;
            var fields = source.Fields.Where(field => !ReportJsonData.IsRootField(field.Key)).ToArray();
            var samples = fields.ToDictionary(field => field.Key, field => DataSample(source, field.Key),
                StringComparer.Ordinal);
            var dialog = new ReportTableFieldsDialog(document, fields, node.Name, samples);
            var mappings = await dialog.ShowDialog<IReadOnlyList<ReportColumnFieldBinding>?>(owner);
            if (mappings is null || !ReferenceEquals(document, Document) ||
                (node.IsJson && !ReferenceEquals(apiJson, _apiJson)) || !document.Elements.Contains(element)) return;
            RememberApiSource();
            var previousBatch = _batch;
            _batch = true;
            try
            {
                if (node.IsJson)
                {
                    ApplyApiRequest(document);
                    document.DataBodyPointer = node.Pointer;
                }

                foreach (var column in document.Columns)
                {
                    var mapping = mappings.FirstOrDefault(binding => ReferenceEquals(binding.Column, column));
                    if (mapping is not null)
                    {
                        var field = fields.First(field => field.Key == mapping.FieldKey);
                        column.FieldKey = mapping.FieldKey;
                        if (column.Header.StartsWith("列 ", StringComparison.Ordinal) &&
                            int.TryParse(column.Header.AsSpan(2), out var number) &&
                            number > 0) column.Header = field.Label;
                    }
                    else
                    {
                        column.FieldKey = "";
                    }

                    AdjustColumnType(column, source.Fields.FirstOrDefault(field => field.Key == column.FieldKey)?.Kind);
                }

                SetCurrentValue(DataSourceProperty, source);
                SetCurrentValue(SelectedElementProperty, element);
                SetCurrentValue(SelectedColumnProperty, document.Columns.FirstOrDefault());
                State.PropertyTab = 1;
                RememberApiSource();
            }
            finally
            {
                _batch = previousBatch;
            }

            DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
            State.ApiStatus = "已绑定集合 " + node.Name + "；" + source.RowCount + " 条数据。";
            return;
        }

        if (!node.IsJson)
        {
            DropField(node.FieldKey, x, y);
            return;
        }

        if (_apiJson is null || element is
                { Kind: ReportElementKind.Line or ReportElementKind.Rectangle or ReportElementKind.PageNumber }) return;
        var bodyPointer = document.DataSourceUrl == State.ApiUrl ? document.DataBodyPointer : "";
        var scalarSource = ReportJsonData.CreateSource(_apiJson, bodyPointer, RootBindingKeys().Append(node.FieldKey));
        if (DataSource is { Name: not "API" } typedSource) scalarSource = typedSource.WithRootValues(scalarSource);
        if (!scalarSource.Fields.Any(field => field.Key == node.FieldKey)) return;
        var value = ReportJsonData.ReadRootValue(_apiJson, node.Pointer);
        var text = Convert.ToString(value, CultureInfo.CurrentCulture) ?? "";
        if (element?.Kind == ReportElementKind.Image && (!Uri.TryCreate(text, UriKind.Absolute, out var imageUri) ||
                                                         imageUri.Scheme is not ("http" or "https"))) return;
        var columnIndex = element?.Kind == ReportElementKind.Table ? PageSurface.ColumnAt(element, point) : -1;
        if (element?.Kind == ReportElementKind.Table && columnIndex < 0) return;
        RememberApiSource();
        var wasBatch = _batch;
        _batch = true;
        try
        {
            element ??= AddElement(ReportElementKind.Text, x, y);
            ApplyApiRequest(document);
            document.DataBodyPointer = bodyPointer;
            SetCurrentValue(DataSourceProperty, scalarSource);
            SetCurrentValue(SelectedElementProperty, element);
            if (element.Kind == ReportElementKind.Table)
            {
                SetCurrentValue(SelectedColumnProperty, document.Columns[columnIndex]);
                document.Columns[columnIndex].FieldKey = node.FieldKey;
                document.Columns[columnIndex].Text = text;
                AdjustColumnType(document.Columns[columnIndex],
                    scalarSource.Fields.First(field => field.Key == node.FieldKey).Kind);
            }
            else
            {
                element.FieldKey = node.FieldKey;
                // QR payloads resolve through FieldKey; keep the user's editable static fallback intact.
                if (element.Kind is not (ReportElementKind.CheckBox or ReportElementKind.QRCode)) element.Text = text;
                if (element.Kind == ReportElementKind.Image) element.ImageUrl = text;
            }

            State.PropertyTab = 1;
            RememberApiSource();
        }
        finally
        {
            _batch = wasBatch;
        }

        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(Document)));
        if (element.Kind == ReportElementKind.Image) await RefreshRemoteImagesAsync();
    }

    private SourceSnapshot CaptureSource() => new(DataSource,
        _apiRequest is { } request ? request with { SelectedDataPointers = request.SelectedDataPointers?.ToArray() } : null,
        _apiJson);

    private sealed record SourceSnapshot(ReportDataSource? Source, ReportApiRequest? Request, string? Json);
}
