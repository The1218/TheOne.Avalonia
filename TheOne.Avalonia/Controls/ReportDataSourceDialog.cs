using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed record ReportFetchedData(ReportApiRequest Request, string Json);

internal sealed partial class ReportDataSourceDialog : Window
{
    private readonly VerificationStep _steps;
    private readonly CheckBox _requiresAuthentication;
    private readonly TextBlock _status = ReportDialogChrome.Text("配置接口后调用验证，再继续下一步。", 12, true);
    private readonly HttpClient _client;
    private readonly bool _ownsClient;
    private readonly CompleteCommand _complete;
    private readonly TextBox _url, _parameters, _response;
    private readonly ComboBox _method, _mode;
    private readonly StepItem _loginStep, _headerStep, _dataStep, _selectionStep;
    private readonly ReportApiRequest _initial;
    private CancellationTokenSource? _cancellation;
    private ReportFetchedData? _fetched;
    private Task<bool>? _pendingData;
    private Button? _previous, _next;
    private Control? _requestContent;
    private bool _closed;
    private int _generation;
    private bool _restoreInitialSelection = true;

    internal ReportDataSourceDialog(ReportApiRequest initial, HttpClient? client = null)
    {
        _initial = initial;
        _client = client ?? new HttpClient();
        _ownsClient = client is null;
        _complete = new CompleteCommand(this);
        ReportDialogChrome.Initialize(this, "添加数据源", 1000, 820);
        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,*") };
        _requiresAuthentication = new CheckBox
        {
            Name = "ApiRequiresAuthentication", Content = "需要权限校验", IsChecked = initial.Authentication is not null,
            Margin = new Thickness(12, 0, 0, 0), VerticalAlignment = VerticalAlignment.Center
        };
        var header = ReportDialogChrome.Header("Table", "添加数据源", "配置接口并选择需要导入的数据。");
        if (header.Child is Grid headerGrid)
        {
            headerGrid.ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto");
            Add(headerGrid, _requiresAuthentication, 2);
        }
        root.Children.Add(header);
        var authentication = CreateAuthentication(initial.Authentication ?? new ReportApiAuthentication());
        _method = Method("ApiRequestMethod", initial.Method);
        _url = new TextBox { Name = "ApiRequestUrl", Text = initial.Url, PlaceholderText = "https://example.com/api/report" };
        _mode = Mode("ApiParameterMode", initial.ParameterMode);
        _parameters = JsonEditor("ApiRequestParameters", initial.Parameters);
        _response = JsonEditor("ApiResponseJson", "", true);
        var data = RequestPage("设置数据接口", _method, _url, _mode, _parameters, _response,
            "获取数据", "FetchApiRequest", async () => await FetchDataAsync(true));
        var selection = CreateSelection();
        _loginStep = Step("权限接口", authentication.Login, ValidateLoginAsync);
        _headerStep = Step("权限头与 Token", authentication.Header, ValidateHeaderAsync);
        _dataStep = Step("数据接口", data, ValidateDataAsync);
        _selectionStep = Step("选择数据", selection, ValidateSelectionAsync);
        _steps = new VerificationStep
        {
            Name = "ApiDataSourceSteps", CompleteText = "确认添加", CompletedCommand = _complete,
            BorderThickness = new Thickness(0), MinHeight = 0, MaxHeight = double.PositiveInfinity,
            Margin = new Thickness(4, 0, 4, 0), HorizontalAlignment = HorizontalAlignment.Stretch
        };
        foreach (var state in new[] { ":information", ":success" })
            _steps.Styles.Add(new Style(selector => selector.OfType<Tag>().Class("step-indicator").Class(state))
            {
                Setters =
                {
                    new Setter(BackgroundProperty, new DynamicResourceExtension("One.Accent")),
                    new Setter(ForegroundProperty, new DynamicResourceExtension("One.OnAccent"))
                }
            });
        _steps.TemplateApplied += (_, e) =>
        {
            if (e.NameScope.Find<ListBox>("PART_Steps") is { } headers) headers.Height = 80;
            if (e.NameScope.Find<ContentPresenter>("PART_ContentPresenter") is { } presenter) presenter.ClipToBounds = true;
            _previous = e.NameScope.Find<Button>("PART_PreviousButton");
            _next = e.NameScope.Find<Button>("PART_NextButton");
            if (_next?.Parent is not Grid footer || _previous is null) return;
            footer.ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto,Auto");
            Grid.SetColumn(_previous, 2);
            Grid.SetColumn(_next, 3);
            _next.IsDefault = true;
            var cancel = ReportDialogChrome.Button("CancelDataSource", "取消");
            cancel.IsCancel = true;
            cancel.Click += (_, _) => Close(null);
            Add(footer, cancel, 1);
            var separator = new Border { Height = 1, VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, -12, 0, 0), IsHitTestVisible = false };
            separator.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Line"));
            Grid.SetColumnSpan(separator, 4);
            footer.Children.Add(separator);
            _status.Name = "ApiRequestStatus";
            _status.TextWrapping = TextWrapping.Wrap;
            _status.VerticalAlignment = VerticalAlignment.Center;
            var status = new StackPanel { Spacing = 4, Margin = new Thickness(0, 0, 12, 0) };
            if (_status.Parent is Panel previousStatus) previousStatus.Children.Remove(_status);
            status.Children.Add(_status);
            if (e.NameScope.Find<ProgressBar>("PART_Progress") is { } progress)
            {
                footer.Children.Remove(progress);
                status.Children.Add(progress);
            }
            footer.Children.Add(status);
        };
        _steps.PropertyChanged += (_, e) =>
        {
            if (e.Property == VerificationStep.IsBusyProperty)
            {
                // VerificationStep can stop awaiting a cancelled validator before HTTP unwinds.
                if (!_steps.IsBusy && _requestContent is not null) CancelRequest();
                _requiresAuthentication.IsEnabled = !_steps.IsBusy && _requestContent is null;
            }
            if (e.Property == VerificationStep.CurrentContentProperty)
            {
                if (_requestContent is not null && !ReferenceEquals(_requestContent, _steps.CurrentContent)) CancelRequest();
                if (ReferenceEquals(_steps.CurrentContent, selection)) SelectionChanged();
            }
        };
        Grid.SetRow(_steps, 1);
        root.Children.Add(_steps);
        Content = root;
        RebuildSteps();
        _requiresAuthentication.IsCheckedChanged += (_, _) =>
        {
            InvalidateData();
            ClearAuthenticationResponse();
            RebuildSteps();
        };
        ObserveText(_url, () => InvalidateData());
        ObserveText(_parameters, () => InvalidateData());
        _method.SelectionChanged += (_, _) => { _mode.IsEnabled = _method.SelectedItem as string != "GET"; InvalidateData(); };
        _mode.SelectionChanged += (_, _) => InvalidateData();
        _mode.IsEnabled = _method.SelectedItem as string != "GET";
        ObserveAuthentication();
        Closed += (_, _) =>
        {
            _closed = true;
            InvalidateData(false);
            ClearAuthenticationResponse();
            if (_ownsClient) _client.Dispose();
        };
    }

    internal ReportFetchedData? Result { get; private set; }

    private void RebuildSteps() => _steps.ItemsSource = _requiresAuthentication.IsChecked == true
        ? [_loginStep, _headerStep, _dataStep, _selectionStep] : [_dataStep, _selectionStep];

    private static StepItem Step(string title, Control content,
        Func<CancellationToken, Task<IReadOnlyList<StepValidationError>>> validate) =>
        new() { Title = title, Content = content, Validator = new Validator(validate) };

    private sealed class Validator(Func<CancellationToken, Task<IReadOnlyList<StepValidationError>>> validate) : IStepValidator
    {
        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken token) => validate(token);
    }

    private sealed class CompleteCommand(ReportDataSourceDialog owner) : ICommand
    {
        public bool CanExecute(object? parameter) => owner._fetched is not null &&
            !string.IsNullOrWhiteSpace(owner._sourceName.Text) && owner._choices.Any(choice => choice.IsSelected);
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter) || owner._fetched is not { } fetched || fetched.Request != owner.ReadRequest()) return;
            owner.Result = fetched with { Request = fetched.Request with
            {
                SourceName = owner._sourceName.Text!.Trim(),
                SelectedDataPointers = owner._choices.Where(choice => choice.IsSelected).Select(choice => choice.Node.Pointer).ToArray()
            } };
            owner.Close(owner.Result);
        }
        public event EventHandler? CanExecuteChanged;
        internal void Changed() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private ReportApiRequest ReadRequest() => new(_url.Text?.Trim() ?? "", _method.SelectedItem as string ?? "GET",
        _parameters.Text ?? "", (ReportApiParameterMode)_mode.SelectedIndex) { Authentication = ReadAuthentication() };

    private static void ObserveText(TextBox editor, Action changed) => editor.PropertyChanged += (_, e) =>
    {
        // TextChanged is deferred; a late event can otherwise invalidate a newer HTTP operation.
        if (e.Property == TextBox.TextProperty) changed();
    };

    private void InvalidateData(bool configurationChanged = true)
    {
        CancelRequest();
        if (configurationChanged) _restoreInitialSelection = false;
        _fetched = null;
        _response.Text = "";
        _choices.Clear();
        _selectionList.ItemsSource = null;
        _selectionList.SelectedItem = null;
        _search.Text = "";
        _status.Text = "设置已变更，请重新调用接口验证。";
        _complete.Changed();
    }

    private void CancelRequest()
    {
        ++_generation;
        _cancellation?.Cancel();
        _cancellation?.Dispose();
        _cancellation = null;
        _pendingLogin = null;
        _pendingData = null;
        if (_requestContent is not null) SetRequestBusy(false);
    }

    private (CancellationToken Token, int Generation) StartRequest()
    {
        CancelRequest();
        _cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        SetRequestBusy(true);
        return (_cancellation.Token, _generation);
    }

    private void SetRequestBusy(bool busy)
    {
        _requiresAuthentication.IsEnabled = !busy && !_steps.IsBusy;
        if (busy) _requestContent = _steps.CurrentContent as Control;
        if (_requestContent is { } content) content.IsEnabled = !busy;
        if (!busy) _requestContent = null;
        _previous?.SetCurrentValue(IsEnabledProperty, !busy && _steps.CanGoBack);
        _next?.SetCurrentValue(IsEnabledProperty, !busy && _steps.CanGoNext);
    }

    private Task<bool> FetchDataAsync(bool force = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (_pendingData is not { IsCompleted: false } || _cancellation?.IsCancellationRequested != false)
        {
            if (!force && _fetched is { } fetched && fetched.Request == ReadRequest()) return Task.FromResult(true);
            _pendingData = FetchDataCoreAsync();
        }
        return AwaitRequestAsync(_pendingData, _cancellation, cancellationToken);
    }

    private static async Task<bool> AwaitRequestAsync(Task<bool> task, CancellationTokenSource? operation,
        CancellationToken cancellationToken)
    {
        // Capture this operation; cancellation of an older validator must never cancel a retry.
        try { return await task.WaitAsync(cancellationToken); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancel before validation unwinds. A separate token callback can be disposed by
            // WaitAsync's cancellation continuation before that callback ever runs.
            try { operation?.Cancel(); }
            catch (ObjectDisposedException) { }
            throw;
        }
    }

    private async Task<bool> FetchDataCoreAsync()
    {
        var request = ReadRequest();
        InvalidateData(false);
        var operation = StartRequest();
        _status.Text = "正在调用数据接口…";
        try
        {
            if (request.Authentication is not null && (_loginJson is null || _loginSnapshot != request.Authentication.LoginRequest))
                throw new ArgumentException("权限接口已变更，请返回第一步重新调用。");
            var header = request.Authentication?.ResolveHeader(_loginJson!);
            var json = await ReportResources.FetchJsonWithHeaderAsync(request, header, _client, operation.Token);
            operation.Token.ThrowIfCancellationRequested();
            if (_closed || operation.Generation != _generation) return false;
            var items = ReportDataCatalog.Items(json);
            _fetched = new ReportFetchedData(request, json);
            _response.Text = FormatResponseJson(json);
            PopulateSelection(items);
            _status.Text = "获取成功，可继续选择数据项。";
            return true;
        }
        catch (Exception error) when (_closed || IsRequestError(error))
        {
            if (!_closed && operation.Generation == _generation)
            {
                _response.Text = "";
                _status.Text = "获取失败：" + error.Message;
            }
            return false;
        }
        finally
        {
            if (!_closed && operation.Generation == _generation) SetRequestBusy(false);
            _complete.Changed();
        }
    }

    private async Task<IReadOnlyList<StepValidationError>> ValidateDataAsync(CancellationToken token)
    {
        if (RequestErrors(ReadRequest(), "DataUrl", "DataParameters") is { Count: > 0 } errors) return errors;
        var task = FetchDataAsync(cancellationToken: token);
        var generation = _generation;
        var success = await task;
        token.ThrowIfCancellationRequested();
        if (generation != _generation) return [new("DataResponse", "设置已变更，请重新获取数据。")];
        if (success) return [];
        var message = _status.Text ?? "请重新获取数据。";
        _status.Text = "请检查当前步骤。";
        return [new("DataResponse", message)];
    }

    private Task<IReadOnlyList<StepValidationError>> ValidateSelectionAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        IReadOnlyList<StepValidationError> errors = _fetched is null || _fetched.Request != ReadRequest()
            ? [new("Selection", "接口设置已变更，请返回数据接口步骤重新获取。")]
            : string.IsNullOrWhiteSpace(_sourceName.Text) ? [new("SourceName", "请输入数据源名称。")]
            : !_choices.Any(choice => choice.IsSelected) ? [new("Selection", "请至少勾选一个数据项。")]
            : [];
        return Task.FromResult(errors);
    }

    private static IReadOnlyList<StepValidationError> RequestErrors(ReportApiRequest request, string urlMember, string parameterMember)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            return [new(urlMember, "请输入有效的 HTTP / HTTPS 接口地址。")];
        try { using var message = (request with { Authentication = null }).CreateMessage(); return []; }
        catch (Exception error) when (IsRequestError(error)) { return [new(parameterMember, error.Message)]; }
    }

    private static bool IsRequestError(Exception error) => error is ArgumentException or JsonException or HttpRequestException
        or TimeoutException or OperationCanceledException;

    private static ComboBox Method(string name, string selected)
    {
        var box = new ComboBox { Name = name, ItemsSource = new[] { "GET", "POST", "PUT", "PATCH", "DELETE" },
            SelectedItem = selected.ToUpperInvariant(), HorizontalAlignment = HorizontalAlignment.Stretch };
        if (box.SelectedIndex < 0) box.SelectedIndex = 0;
        return box;
    }
    private static ComboBox Mode(string name, ReportApiParameterMode selected) => new()
    {
        Name = name, ItemsSource = new[] { "JSON 请求体", "Form 表单" }, SelectedIndex = (int)selected,
        HorizontalAlignment = HorizontalAlignment.Stretch
    };
    private static TextBox JsonEditor(string name, string text, bool readOnly = false)
    {
        var box = new TextBox { Name = name, Text = text, IsReadOnly = readOnly, AcceptsReturn = true,
            FontFamily = new FontFamily("Menlo,Consolas"), TextWrapping = TextWrapping.NoWrap,
            MinHeight = 160, MaxHeight = 280, VerticalContentAlignment = VerticalAlignment.Top,
            PlaceholderText = readOnly ? "调用成功后显示响应…" : "{\"page\":1}" };
        ScrollViewer.SetVerticalScrollBarVisibility(box, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(box, ScrollBarVisibility.Auto);
        ScrollViewer.SetIsScrollChainingEnabled(box, false);
        return box;
    }

    private static StepField Field(string member, string label, Control editor) =>
        new() { MemberName = member, Content = ReportDialogChrome.Field(label, editor) };

    private static ScrollViewer Page(Control content) => new()
    {
        Content = content, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
        VerticalScrollBarVisibility = ScrollBarVisibility.Auto, IsScrollChainingEnabled = false
    };

    private static Control RequestPage(string title, ComboBox method, TextBox url, ComboBox mode,
        TextBox parameters, TextBox response, string action, string actionName, Func<Task> fetch)
    {
        var login = actionName == "FetchApiLoginRequest";
        var member = login ? "Login" : "Data";
        var form = new StackPanel { Spacing = 14 };
        form.Children.Add(ReportDialogChrome.Text(title, 17, weight: FontWeight.SemiBold));
        var address = new Grid { ColumnDefinitions = new ColumnDefinitions("130,*"), ColumnSpacing = 12 };
        address.Children.Add(ReportDialogChrome.Field("请求方式", method));
        Add(address, Field(member + "Url", login ? "登录接口 URL" : "API 接口 URL", url), 1);
        form.Children.Add(address);
        mode.MaxWidth = 220;
        mode.HorizontalAlignment = HorizontalAlignment.Left;
        form.Children.Add(ReportDialogChrome.Field("参数格式 · GET 自动编码为查询参数", mode));
        var panes = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 16 };
        panes.Children.Add(Field(member + "Parameters", "请求参数 · JSON 对象", parameters));
        Add(panes, Field(member + "Response", "响应预览", response), 1);
        form.Children.Add(panes);
        var button = ReportDialogChrome.Button(actionName, action, true);
        button.HorizontalAlignment = HorizontalAlignment.Left;
        button.Click += async (_, _) => await fetch();
        form.Children.Add(button);
        return Page(form);
    }

    private static void Add(Grid grid, Control control, int column) { Grid.SetColumn(control, column); grid.Children.Add(control); }

    private static string FormatResponseJson(string json, bool maskStrings = false)
    {
        using var parsed = JsonDocument.Parse(json);
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer,
                   new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
            Write(parsed.RootElement, writer);
        var formatted = Encoding.UTF8.GetString(buffer.ToArray());
        return formatted.Length > 200_000 ? formatted[..200_000] + "\n…（仅截断预览，导入保留完整 JSON）" : formatted;

        void Write(JsonElement value, Utf8JsonWriter writer)
        {
            if (!maskStrings) { value.WriteTo(writer); return; }
            if (value.ValueKind == JsonValueKind.String) writer.WriteStringValue("••••••••");
            else if (value.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
                foreach (var property in value.EnumerateObject()) { writer.WritePropertyName(property.Name); Write(property.Value, writer); }
                writer.WriteEndObject();
            }
            else if (value.ValueKind == JsonValueKind.Array)
            {
                writer.WriteStartArray(); foreach (var child in value.EnumerateArray()) Write(child, writer); writer.WriteEndArray();
            }
            else value.WriteTo(writer);
        }
    }
}
