using System.Net.Http;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions;
using global::Avalonia.Platform.Storage;
using global::Avalonia.Threading;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

/// <summary>Standalone read-only report viewer. Parses document JSON and loads its optional API/images.</summary>
public sealed class ReportPreview : UserControl
{
    public static readonly StyledProperty<string?> DocumentJsonProperty = AvaloniaProperty.Register<ReportPreview, string?>(nameof(DocumentJson));
    public static readonly StyledProperty<ReportDataSource?> DataSourceProperty = AvaloniaProperty.Register<ReportPreview, ReportDataSource?>(nameof(DataSource));
    public static readonly DirectProperty<ReportPreview, ReportDocument?> DocumentProperty = AvaloniaProperty.RegisterDirect<ReportPreview, ReportDocument?>(nameof(Document), c => c.Document);
    public static readonly DirectProperty<ReportPreview, string?> ErrorProperty = AvaloniaProperty.RegisterDirect<ReportPreview, string?>(nameof(Error), c => c.Error);
    public string? DocumentJson { get => GetValue(DocumentJsonProperty); set => SetValue(DocumentJsonProperty, value); }
    public ReportDataSource? DataSource { get => GetValue(DataSourceProperty); set => SetValue(DataSourceProperty, value); }
    public ReportDocument? Document { get; private set; }
    public string? Error { get; private set; }
    public ReportLayout? Layout { get; private set; }
    /// <summary>Optional consumer-owned client for authenticated API and image requests. Not serialized.</summary>
    public HttpClient? HttpClient { get; set; }
    /// <summary>Optional host printer. The PDF exists until the returned task completes; KeepDocument controls its subsequent lifetime.</summary>
    public Func<string, ReportPrintOptions?, CancellationToken, Task<ReportPrintResult>>? PrintHandler { get; set; }
    public int CurrentPageNumber => _page + 1;
    public double Zoom { get => _zoom; set { if (!double.IsFinite(value)) throw new ArgumentOutOfRangeException(nameof(value)); _zoom = Math.Clamp(value, .25, 2); Draw(); } }
    private double _zoom = .75;
    private int _page, _generation;
    private ReportDocument? _renderDocument;
    private string? _requestedJson;
    private bool _isLoading, _restartOnAttach, _isPrinting;
    private CancellationTokenSource? _loading;
    private readonly ReportPageSurface _surface = new() { ShowRulers = false };
    private readonly ScrollViewer _scroll;
    private readonly TextBlock _status = new() { VerticalAlignment = VerticalAlignment.Center };
    private readonly Button _previous = new() { Name = "PreviewPrevious", Content = "上一页" };
    private readonly Button _next = new() { Name = "PreviewNext", Content = "下一页" };
    private readonly Button _print = new() { Name = "PrintButton", Content = "打印" };
    private readonly Button _export = new() { Content = "导出 PDF" };
    public ReportPreview()
    {
        PrintTool.CleanupTemporaryDocuments();
        var toolbar = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Margin = new Thickness(12, 8) };
        _previous.Click += (_, _) => GoToPage(_page); _next.Click += (_, _) => GoToPage(_page + 2);
        var zoom = new ComboBox { ItemsSource = new[] { "50%", "75%", "100%", "125%", "150%" }, SelectedIndex = 1 };
        zoom.SelectionChanged += (_, _) => Zoom = new[] { .5, .75, 1, 1.25, 1.5 }[Math.Max(0, zoom.SelectedIndex)];
        var reload = new Button { Content = "刷新" }; reload.Click += async (_, _) => { if (_requestedJson is { } json) await LoadFromPropertyAsync(json); };
        _print.Click += async (_, _) => { try { _status.Text = (await PrintAsync()).Message; } catch (Exception e) { _status.Text = e.Message; } };
        _export.Click += async (_, _) =>
        {
            try
            {
                var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
                if (storage?.CanSave != true) throw new InvalidOperationException("当前平台不支持文件对话框。");
                using var file = await storage.SaveFilePickerAsync(new() { Title = "导出 PDF", SuggestedFileName = "report.pdf", DefaultExtension = "pdf" });
                if (file is null) return; await using var stream = await file.OpenWriteAsync(); await ExportPdfAsync(stream);
            }
            catch (Exception e) { _status.Text = e.Message; }
        };
        foreach (var control in new Control[] { _previous, _next, zoom, reload, _print, _export, _status }) toolbar.Children.Add(control);
        _scroll = new ScrollViewer { Content = _surface, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        _scroll.Bind(BackgroundProperty, new DynamicResourceExtension("One.Subtle"));
        var toolbarFrame = new Border { Child = toolbar, BorderThickness = new Thickness(0, 0, 0, 1) };
        toolbarFrame.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        var grid = new Grid { RowDefinitions = new RowDefinitions("Auto,*") }; grid.Children.Add(toolbarFrame); Grid.SetRow(_scroll, 1); grid.Children.Add(_scroll); Content = grid; Draw();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DocumentJsonProperty)
        {
            if (string.IsNullOrWhiteSpace(DocumentJson)) Clear();
            else _ = LoadFromPropertyAsync(DocumentJson);
        }
        else if (change.Property == DataSourceProperty && _requestedJson is { } json) _ = LoadFromPropertyAsync(json);
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_restartOnAttach && _requestedJson is { } json) { _restartOnAttach = false; _ = LoadFromPropertyAsync(json); }
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _restartOnAttach = _isLoading && _requestedJson is not null;
        ++_generation; _loading?.Cancel(); _isLoading = false;
        base.OnDetachedFromVisualTree(e);
    }
    // LoadAsync reports errors only for its own generation. An earlier request must never replace
    // the state/status of a later request, even when its handler completes after cancellation.
    private async Task LoadFromPropertyAsync(string json) { try { await LoadAsync(json); } catch (Exception) { } }
    private void Clear()
    {
        ++_generation; _loading?.Cancel(); _requestedJson = null; _restartOnAttach = false; _isLoading = false;
        SetDocument(null); _renderDocument = null; Layout = null; _page = 0; SetError(null);
        _surface.ClearImages(); Draw();
    }
    private void SetDocument(ReportDocument? document)
    { var previous = Document; Document = document; RaisePropertyChanged(DocumentProperty, previous, document); }
    public async Task LoadAsync(string json, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        _loading?.Cancel();
        using var loading = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _loading = loading; loading.CancelAfter(TimeSpan.FromSeconds(30));
        var token = loading.Token; var generation = ++_generation;
        _requestedJson = json; _restartOnAttach = false; _isLoading = true;
        SetDocument(null); _renderDocument = null; Layout = null; _page = 0; SetError(null);
        _surface.ClearImages(); Draw();
        try
        {
            token.ThrowIfCancellationRequested();
            var document = ReportDocument.Load(json);
            var source = DataSource ?? await ReportResources.LoadDataAsync(document, HttpClient, token);
            await ReportResources.LoadImagesAsync(document, HttpClient, token, source); token.ThrowIfCancellationRequested();
            var layout = ReportPaginator.Paginate(document, source);
            var snapshot = ReportPdfExporter.Snapshot(document);
            token.ThrowIfCancellationRequested();
            if (generation != _generation) return;
            _renderDocument = snapshot; Layout = layout; _isLoading = false;
            SetDocument(document); SetError(null); Draw();
        }
        catch (Exception exception)
        {
            if (generation == _generation)
            {
                _isLoading = false; Layout = null; _renderDocument = null; SetDocument(null);
                SetError(exception is OperationCanceledException ? "报表加载已取消或超时。" : exception.Message); Draw();
            }
            throw;
        }
        finally { if (ReferenceEquals(_loading, loading)) _loading = null; }
    }
    public void GoToPage(int page) { _page = Math.Clamp(page - 1, 0, Math.Max(0, (Layout?.Pages.Count ?? 1) - 1)); _scroll.Offset = default; Draw(); }
    private void SetError(string? value) { var old = Error; Error = value; RaisePropertyChanged(ErrorProperty, old, value); if (value is not null) _status.Text = value; }
    private void Draw()
    {
        if (_scroll is null) return;
        _page = Math.Clamp(_page, 0, Math.Max(0, (Layout?.Pages.Count ?? 1) - 1));
        _previous.IsEnabled = Layout is not null && _page > 0; _next.IsEnabled = Layout is not null && _page + 1 < Layout.Pages.Count;
        _print.IsEnabled = Layout is not null && !_isPrinting;
        _export.IsEnabled = Layout is not null;
        _surface.SetDocument(Layout is null ? null : _renderDocument, null); _surface.Update(Layout, true, _page, -1, _zoom, Error);
        if (Error is null) _status.Text = _isLoading ? "正在加载报表…" : Layout is null ? "请加载报表 JSON" : $"第 {_page + 1} / {Layout.Pages.Count} 页";
    }
    public Task ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (_renderDocument is null || Layout is null) throw new InvalidOperationException("报表尚未加载或布局无效。");
        return ReportPdfExporter.ExportSnapshotAsync(_renderDocument, Layout, destination, cancellationToken);
    }
    public async Task<ReportPrintResult> PrintAsync(ReportPrintOptions? options = null, CancellationToken cancellationToken = default)
    {
        Dispatcher.UIThread.VerifyAccess(); cancellationToken.ThrowIfCancellationRequested();
        if (_renderDocument is null || Layout is null) throw new InvalidOperationException("报表尚未加载或布局无效。");
        if (_isPrinting) throw new InvalidOperationException("打印任务正在提交。");
        PrintTool.CleanupTemporaryDocuments();
        var path = PrintTool.CreateTemporaryPdfPath(); var keep = false;
        _isPrinting = true; Draw();
        try
        {
            var settings = new FileStreamOptions { Mode = FileMode.CreateNew, Access = FileAccess.Write, Share = FileShare.None };
            if (!OperatingSystem.IsWindows()) settings.UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite;
            await using (var file = new FileStream(path, settings)) await ExportPdfAsync(file, cancellationToken);
            var result = await (PrintHandler ?? PrintTool.PrintPdfAsync)(path, options, cancellationToken); keep = result.KeepDocument;
            if (keep) PrintTool.ScheduleTemporaryCleanup(path);
            return keep ? result with { Message = result.Message + " 临时文件将在运行 24 小时后或下次使用控件时清理：" + path, DocumentPath = path } : result;
        }
        finally { if (!keep) PrintTool.DeleteTemporaryDocument(path); _isPrinting = false; Draw(); }
    }
}
