using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed record ReportFetchedData(ReportApiRequest Request, string Json);

internal sealed partial class ReportDataSourceDialog : Window
{
    private CancellationTokenSource? _cancellation;

    internal ReportDataSourceDialog(ReportApiRequest initial)
    {
        ReportDialogChrome.Initialize(this, "添加数据源", 1000, 780);
        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,*,Auto") };
        root.Children.Add(ReportDialogChrome.Header("Table", "添加数据源", "配置 API 请求，先检查返回的 JSON，再确认导入设计器。"));
        var body = new Grid
        {
            Name = "ApiDataSourceForm", RowDefinitions = new RowDefinitions("Auto,Auto,Auto"), RowSpacing = 20,
            Margin = new Thickness(24, 16)
        };
        var formScroll = new ScrollViewer
        {
            Name = "ApiDataSourceFormScroll", Content = body,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(formScroll, 1);
        root.Children.Add(formScroll);
        var authentication = CreateAuthentication(initial.Authentication);
        body.Children.Add(_requiresAuthentication);
        Grid.SetRow(authentication, 1);
        body.Children.Add(authentication);
        var dataSection = new Grid
        {
            Name = "ApiDataInterfaceSection", RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto"),
            RowSpacing = 12
        };
        Grid.SetRow(dataSection, 2);
        body.Children.Add(dataSection);
        dataSection.Children.Add(ReportDialogChrome.Text("数据接口", weight: FontWeight.SemiBold));
        var address = new Grid { ColumnDefinitions = new ColumnDefinitions("120,*"), ColumnSpacing = 12 };
        var method = new ComboBox
        {
            Name = "ApiRequestMethod", ItemsSource = new[] { "GET", "POST", "PUT", "PATCH", "DELETE" },
            SelectedItem = initial.Method.ToUpperInvariant(), HorizontalAlignment = HorizontalAlignment.Stretch
        };
        if (method.SelectedIndex < 0) method.SelectedIndex = 0;
        var url = new TextBox
            { Name = "ApiRequestUrl", Text = initial.Url, PlaceholderText = "https://example.com/api/report" };
        address.Children.Add(ReportDialogChrome.Field("请求方式", method));
        var urlField = ReportDialogChrome.Field("API 接口 URL", url);
        Grid.SetColumn(urlField, 1);
        address.Children.Add(urlField);
        Grid.SetRow(address, 1);
        dataSection.Children.Add(address);
        var options = new Grid { ColumnDefinitions = new ColumnDefinitions("180,*,Auto"), ColumnSpacing = 12 };
        Grid.SetRow(options, 2);
        dataSection.Children.Add(options);
        var mode = new ComboBox
        {
            Name = "ApiParameterMode", ItemsSource = new[] { "JSON 请求体", "Form 表单" },
            SelectedIndex = (int)initial.ParameterMode, HorizontalAlignment = HorizontalAlignment.Stretch
        };
        options.Children.Add(mode);
        var hint = ReportDialogChrome.Text("GET 自动转换为 URL 查询参数", 12, true);
        hint.VerticalAlignment = VerticalAlignment.Center;
        hint.TextWrapping = TextWrapping.Wrap;
        Grid.SetColumn(hint, 1);
        options.Children.Add(hint);
        var fetch = ReportDialogChrome.Button("FetchApiRequest", "获取数据源", true);
        Grid.SetColumn(fetch, 2);
        options.Children.Add(fetch);
        var panes = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 16, Height = 240 };
        Grid.SetRow(panes, 3);
        dataSection.Children.Add(panes);
        var parameterPanel = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*"), RowSpacing = 8 };
        parameterPanel.Children.Add(ReportDialogChrome.Text("请求参数", weight: FontWeight.SemiBold));
        var description = ReportDialogChrome.Text("填写 JSON 对象；无参数时留空。支持嵌套 JSON。", 12, true);
        description.TextWrapping = TextWrapping.Wrap;
        Grid.SetRow(description, 1);
        parameterPanel.Children.Add(description);
        var parameters = new TextBox
        {
            Name = "ApiRequestParameters", Text = initial.Parameters,
            PlaceholderText = "{\n  \"page\": 1,\n  \"keyword\": \"\"\n}", AcceptsReturn = true,
            TextWrapping = TextWrapping.NoWrap, FontFamily = new FontFamily("Menlo,Consolas"),
            VerticalContentAlignment = VerticalAlignment.Top
        };
        ScrollViewer.SetVerticalScrollBarVisibility(parameters, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(parameters, ScrollBarVisibility.Auto);
        Grid.SetRow(parameters, 2);
        parameterPanel.Children.Add(parameters);
        panes.Children.Add(parameterPanel);
        var responsePanel = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*"), RowSpacing = 8 };
        Grid.SetColumn(responsePanel, 1);
        panes.Children.Add(responsePanel);
        responsePanel.Children.Add(ReportDialogChrome.Text("响应 JSON", weight: FontWeight.SemiBold));
        var responseHint = ReportDialogChrome.Text("请求成功后可确认导入；取消不会修改已有数据源。", 12, true);
        responseHint.TextWrapping = TextWrapping.Wrap;
        Grid.SetRow(responseHint, 1);
        responsePanel.Children.Add(responseHint);
        var response = new TextBox
        {
            Name = "ApiResponseJson", IsReadOnly = true, AcceptsReturn = true, TextWrapping = TextWrapping.NoWrap,
            PlaceholderText = "等待获取数据…", FontFamily = new FontFamily("Menlo,Consolas"),
            VerticalContentAlignment = VerticalAlignment.Top
        };
        ScrollViewer.SetVerticalScrollBarVisibility(response, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(response, ScrollBarVisibility.Auto);
        Grid.SetRow(response, 2);
        responsePanel.Children.Add(response);
        var status = ReportDialogChrome.Text("请求成功后，点击确认添加。", 12, true);
        status.Name = "ApiRequestStatus";
        status.TextWrapping = TextWrapping.Wrap;
        var cancel = ReportDialogChrome.Button("CancelDataSource", "取消");
        cancel.IsCancel = true;
        cancel.Click += (_, _) => Close(null);
        var confirm = ReportDialogChrome.Button("ConfirmDataSource", "确认添加", true);
        confirm.IsEnabled = false;
        confirm.IsDefault = true;
        var footer = ReportDialogChrome.Footer(status, cancel, confirm);
        Grid.SetRow(footer, 2);
        root.Children.Add(footer);
        Content = root;
        ReportFetchedData? fetched = null;

        ReportApiRequest Request()
        {
            return new ReportApiRequest(url.Text?.Trim() ?? "", method.SelectedItem as string ?? "GET",
                parameters.Text ?? "",
                (ReportApiParameterMode)mode.SelectedIndex) { Authentication = ReadAuthentication() };
        }

        void Invalidate()
        {
            fetched = null;
            confirm.IsEnabled = false;
            status.Text = "请求设置已变更，请获取数据源后确认。";
            mode.IsEnabled = method.SelectedItem as string != "GET";
            hint.Text = method.SelectedItem as string == "GET" ? "参数将编码为 URL 查询参数" :
                mode.SelectedIndex == 1 ? "参数将作为 Form 表单发送" : "参数将作为 JSON 请求体发送";
        }

        url.TextChanged += (_, _) => Invalidate();
        parameters.TextChanged += (_, _) => Invalidate();
        method.SelectionChanged += (_, _) => Invalidate();
        mode.SelectionChanged += (_, _) => Invalidate();
        ObserveAuthentication(Invalidate);
        Invalidate();
        status.Text = "配置请求后，先获取 JSON 进行检查。";

        void Busy(bool busy)
        {
            dataSection.IsEnabled = !busy;
            EnableAuthentication(!busy);
            if (busy) confirm.IsEnabled = false;
        }

        CancellationToken StartRequest()
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
            _cancellation = new CancellationTokenSource();
            fetched = null;
            Busy(true);
            return _cancellation.Token;
        }

        _fetchAuthentication.Click += async (_, _) =>
        {
            var authenticationRequest = ReadAuthentication();
            if (authenticationRequest is null) return;
            var token = StartRequest();
            _headerValue.Text = "";
            _loginResponse.Text = "";
            _loginStatus.Text = status.Text = "正在请求登录接口…";
            try
            {
                authenticationRequest.Validate();
                var json = await ReportResources.FetchJsonAsync(authenticationRequest.LoginRequest,
                    cancellationToken: token);
                if (token.IsCancellationRequested || !IsVisible) return;
                _loginResponse.Text = FormatResponseJson(json);
                _headerValue.Text = authenticationRequest.ResolveHeader(json);
                _loginStatus.Text = status.Text = "权限获取成功，已解析 token。可继续配置并获取数据源。";
            }
            catch (Exception exception) when (exception is ArgumentException or JsonException or HttpRequestException
                                                  or TimeoutException or OperationCanceledException)
            {
                if (!token.IsCancellationRequested && IsVisible)
                    _loginStatus.Text = status.Text = "权限获取失败：" + exception.Message;
            }
            finally
            {
                if (IsVisible) Busy(false);
            }
        };
        fetch.Click += async (_, _) =>
        {
            var request = Request();
            var token = StartRequest();
            if (request.Authentication is not null)
            {
                _headerValue.Text = "";
                _loginResponse.Text = "";
                _loginStatus.Text = "获取数据时将重新验证权限。";
            }

            status.Text = request.Authentication is null ? "正在请求 API…" : "正在登录获取 token…";
            try
            {
                var json = await ReportResources.FetchJsonWithAuthenticationAsync(request, null, token,
                    (loginJson, value) =>
                    {
                        if (!token.IsCancellationRequested && IsVisible)
                        {
                            _loginResponse.Text = FormatResponseJson(loginJson);
                            _headerValue.Text = value;
                            _loginStatus.Text = "权限获取成功，已解析 token。";
                            status.Text = "登录成功，正在请求数据接口…";
                        }
                    });
                if (token.IsCancellationRequested || !IsVisible) return;
                fetched = new ReportFetchedData(request, json);
                response.Text = FormatResponseJson(json);
                status.Text = "获取成功，确认后将完整数据树加载到设计器。";
                confirm.IsEnabled = true;
            }
            catch (Exception exception) when (exception is ArgumentException or JsonException or HttpRequestException
                                                  or TimeoutException or OperationCanceledException)
            {
                if (!token.IsCancellationRequested && IsVisible)
                {
                    status.Text = "获取失败：" + exception.Message;
                    response.Text = "";
                    if (request.Authentication is not null && string.IsNullOrEmpty(_headerValue.Text))
                        _loginStatus.Text = "本次请求未取得有效权限，可单独获取权限进行检查。";
                }
            }
            finally
            {
                if (IsVisible) Busy(false);
            }
        };
        confirm.Click += (_, _) =>
        {
            if (fetched is not null && fetched.Request == Request())
            {
                Result = fetched;
                Close(fetched);
            }
        };
        Closed += (_, _) =>
        {
            _cancellation?.Cancel();
            _cancellation?.Dispose();
        };
    }

    internal ReportFetchedData? Result { get; private set; }

    private static string FormatResponseJson(string json)
    {
        using var parsed = JsonDocument.Parse(json);
        using var buffer = new MemoryStream();
        using (var writer = new Utf8JsonWriter(buffer,
                   new JsonWriterOptions { Indented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
        {
            parsed.RootElement.WriteTo(writer);
        }

        var formatted = Encoding.UTF8.GetString(buffer.ToArray());
        return formatted.Length > 200_000 ? formatted[..200_000] + "\n…（仅截断预览，导入保留完整 JSON）" : formatted;
    }
}