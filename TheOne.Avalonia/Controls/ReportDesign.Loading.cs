using System.Globalization;
using Avalonia.Threading;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    private bool _committingDocumentData;
    private Task _documentDataTask = Task.CompletedTask;
    private ReportDocument? _loadingDataDocument;
    private bool _pendingDocumentData;

    private void ResetDocumentData(bool clearApiSource)
    {
        ++_fetchGeneration;
        _fetchCancellation?.Cancel();
        _imageCancellation?.Cancel();
        State.IsFetching = false;
        State.ApiStatus = "";
        _loadingDataDocument = null;
        _documentDataTask = Task.CompletedTask;
        _pendingDocumentData = Document?.DataSourceUrl.Length > 0;
        _apiJson = null;
        _apiRequest = null;
        _fetchUpdating = true;
        try
        {
            State.SelectedBody = null;
            State.Bodies.Clear();
            State.ResponsePreview = "";
        }
        finally
        {
            _fetchUpdating = false;
        }

        ResetDataTreeState();
        if (clearApiSource) SetCurrentValue(DataSourceProperty, null);
        _treeJson = null;
        _treeSource = null;
        _treeSearch = null;
        _dataRoots = [];
        State.DataNodes.Clear();
        FieldSearch.Text = "";
        FilterFields();
    }

    private Task StartDocumentDataLoad(CancellationToken cancellationToken = default)
    {
        if (Document is not { DataSourceUrl.Length: > 0 } document || DataSource is not null)
        {
            _pendingDocumentData = false;
            return _documentDataTask = Task.CompletedTask;
        }

        if (ReferenceEquals(_loadingDataDocument, document) && State.IsFetching) return _documentDataTask;
        _pendingDocumentData = false;
        _documentDataTask = LoadDocumentDataAsync(document, cancellationToken);
        _ = ObserveDocumentDataLoadAsync(_documentDataTask);
        return _documentDataTask;
    }

    private static async Task ObserveDocumentDataLoadAsync(Task task)
    {
        // Automatic property/attach loading reports errors in the editor. Explicit
        // LoadTemplateAsync callers also receive the original exception from the task.
        try
        {
            await task;
        }
        catch (Exception)
        {
        }
    }

    private async Task LoadDocumentDataAsync(ReportDocument document, CancellationToken cancellationToken)
    {
        _fetchCancellation?.Cancel();
        _fetchCancellation?.Dispose();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _fetchCancellation = cancellation;
        cancellation.CancelAfter(TimeSpan.FromSeconds(30));
        var token = cancellation.Token;
        var generation = ++_fetchGeneration;
        var request = ReportApiRequest.FromDocument(document);
        var bodyPointer = document.DataBodyPointer;
        _loadingDataDocument = document;
        State.IsFetching = true;
        State.ApiStatus = "正在恢复报表数据…";
        try
        {
            var json = await ReportResources.FetchJsonAsync(request, cancellationToken: token);
            token.ThrowIfCancellationRequested();
            if (!IsCurrent()) return;
            var source = ReportJsonData.CreateSource(json, bodyPointer, RootBindingKeys());
            var images = document.Elements.Where(element => element.Kind == ReportElementKind.Image).ToArray();
            var imageDocument = new ReportDocument();
            foreach (var image in Copy(document).Elements.Where(element => element.Kind == ReportElementKind.Image))
                imageDocument.Elements.Add(image);
            await ReportResources.LoadImagesAsync(imageDocument, null, token, source);
            token.ThrowIfCancellationRequested();
            if (!IsCurrent()) return;
            source = ReportJsonData.CreateSource(json, bodyPointer, RootBindingKeys());
            // Validate current bindings before committing the runtime source. Definition
            // edits made while HTTP was in flight remain in the current document.
            ReportPaginator.Paginate(document, source);
            var previousBatch = _batch;
            _batch = true;
            _committingDocumentData = true;
            try
            {
                for (var index = 0; index < images.Length; index++)
                {
                    var original = images[index];
                    var loaded = imageDocument.Elements[index];
                    var url = original.FieldKey.Length > 0
                        ? Convert.ToString(source.GetValue(0, original.FieldKey), CultureInfo.InvariantCulture)
                        : original.ImageUrl;
                    if (document.Elements.Contains(original) && original.FieldKey == loaded.FieldKey &&
                        loaded.ResolvedImageUrl.Length > 0 && loaded.ResolvedImageUrl == url)
                    {
                        original.ImageUrl = loaded.ImageUrl;
                        original.ResolvedImageUrl = loaded.ResolvedImageUrl;
                        original.ResolvedImageData = loaded.ResolvedImageData;
                    }
                }

                ImportApiResponse(request, json);
                SetCurrentValue(DataSourceProperty, source);
            }
            finally
            {
                _batch = previousBatch;
                _committingDocumentData = false;
            }

            _lastSnapshot = CaptureHistorySource(Copy(document));
            Rebuild();
            State.ApiStatus = "报表数据已恢复。";
        }
        catch (Exception exception)
        {
            if (generation == _fetchGeneration && ReferenceEquals(Document, document))
            {
                State.ApiStatus = exception is OperationCanceledException
                    ? "恢复报表数据已取消或超时。"
                    : "恢复报表数据失败：" + exception.Message;
                State.HasApiError = true;
                State.Status = State.ApiStatus;
            }

            throw;
        }
        finally
        {
            if (ReferenceEquals(_fetchCancellation, cancellation)) _fetchCancellation = null;
            if (generation == _fetchGeneration)
            {
                State.IsFetching = false;
                _loadingDataDocument = null;
            }
        }

        bool IsCurrent()
        {
            return generation == _fetchGeneration && ReferenceEquals(Document, document)
                                                  && request == ReportApiRequest.FromDocument(document) &&
                                                  bodyPointer == document.DataBodyPointer;
        }
    }

    /// <summary>Loads a saved definition and awaits its optional API data and bound images.</summary>
    public async Task LoadTemplateAsync(string json, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        cancellationToken.ThrowIfCancellationRequested();
        LoadTemplate(json);
        // LoadTemplate can start automatically in an attached view; link explicit caller
        // cancellation to that same operation rather than issuing a second login/fetch.
        var task = _pendingDocumentData ? StartDocumentDataLoad(cancellationToken) : _documentDataTask;
        var operation = _fetchCancellation;
        using var registration = cancellationToken.Register(() =>
        {
            if (ReferenceEquals(operation, _fetchCancellation)) operation?.Cancel();
        });
        await task;
        cancellationToken.ThrowIfCancellationRequested();
    }
}