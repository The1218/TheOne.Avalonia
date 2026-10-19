using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportDataSourceDialog
{
    private TextBox _loginUrl = null!, _loginParameters = null!, _loginResponse = null!, _tokenField = null!,
        _headerPrefix = null!, _headerValue = null!;
    private ComboBox _loginMethod = null!, _loginMode = null!;
    private AutoCompleteBox _headerName = null!;
    private ListBox _tokenList = null!;
    private string? _loginJson;
    private ReportApiRequest? _loginSnapshot;
    private Task<bool>? _pendingLogin;
    private bool _updatingAuthentication;

    private (Control Login, Control Header) CreateAuthentication(ReportApiAuthentication settings)
    {
        _loginUrl = new TextBox { Name = "ApiLoginUrl", Text = settings.Url, PlaceholderText = "https://example.com/api/login" };
        _loginMethod = Method("ApiLoginMethod", settings.Method);
        _loginMode = Mode("ApiLoginParameterMode", settings.ParameterMode);
        _loginParameters = JsonEditor("ApiLoginParameters", settings.Parameters);
        _loginResponse = JsonEditor("ApiLoginResponseJson", "", true);
        var login = RequestPage("添加权限接口", _loginMethod, _loginUrl, _loginMode, _loginParameters,
            _loginResponse, "调用权限接口", "FetchApiLoginRequest", async () => await FetchLoginAsync(true));
        _headerName = new AutoCompleteBox
        {
            Name = "ApiAuthHeaderName", Text = settings.HeaderName,
            ItemsSource = new[] { "Authorization", "X-API-Key", "X-Auth-Token", "Token", "Access-Token", "Auth-Token" },
            MinimumPrefixLength = 0, FilterMode = AutoCompleteFilterMode.None, IsTextCompletionEnabled = false,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var expand = new Button { Name = "ApiAuthHeaderSuggestions", MinWidth = 0, MinHeight = 0,
            Padding = new Thickness(8), Background = Brushes.Transparent, BorderThickness = new Thickness(0),
            Content = new ReportIcon { Kind = "Chevron", Width = 12, Height = 12 }, Focusable = false };
        expand.Click += (_, _) => _headerName.IsDropDownOpen = !_headerName.IsDropDownOpen;
        _headerName.InnerRightContent = expand;
        _headerPrefix = new TextBox { Name = "ApiAuthHeaderPrefix", Text = settings.HeaderPrefix, PlaceholderText = "可留空；Bearer 后保留空格" };
        _tokenField = new TextBox { Name = "ApiTokenField", Text = settings.TokenField, PlaceholderText = "/data/token 或 data.token" };
        _headerValue = new TextBox { Name = "ApiAuthHeaderValue", IsReadOnly = true, PlaceholderText = "选择 Token 后生成脱敏预览" };
        _tokenList = new ListBox
        {
            Name = "ApiTokenCandidates", MinHeight = 180, MaxHeight = 320,
            ItemTemplate = new FuncDataTemplate<ReportDataNode>((node, _) =>
            {
                var row = new StackPanel { Spacing = 3 };
                row.Children.Add(ReportDialogChrome.Text(node?.Name ?? ""));
                row.Children.Add(ReportDialogChrome.Text((node?.Pointer ?? "") + " · " + node?.TypeLabel + " · ••••••", 12, true));
                return row;
            })
        };
        ScrollViewer.SetIsScrollChainingEnabled(_tokenList, false);
        _tokenList.SelectionChanged += (_, _) =>
        {
            if (!_updatingAuthentication && _tokenList.SelectedItem is ReportDataNode node) _tokenField.Text = node.Pointer;
        };
        var form = new StackPanel { Spacing = 14 };
        form.Children.Add(ReportDialogChrome.Text("设置权限头", 17, weight: FontWeight.SemiBold));
        var columns = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 20 };
        columns.Children.Add(Field("TokenField", "选择 Token 数据 · 字符串或数字", _tokenList));
        var fields = new StackPanel { Spacing = 12 };
        fields.Children.Add(Field("HeaderName", "请求头名称 · 可输入", _headerName));
        fields.Children.Add(Field("HeaderPrefix", "值前缀 · 保留尾部空格", _headerPrefix));
        fields.Children.Add(Field("TokenPath", "Token 路径 · 支持手动输入", _tokenField));
        Add(columns, fields, 1);
        form.Children.Add(columns);
        form.Children.Add(ReportDialogChrome.Field("请求头预览", _headerValue));
        form.Children.Add(ReportDialogChrome.Text("Token 仅显示脱敏预览，不保存到报表模板。", 12, true));
        return (login, Page(form));
    }

    private ReportApiAuthentication? ReadAuthentication() => _requiresAuthentication.IsChecked != true ? null : new()
    {
        Url = _loginUrl.Text?.Trim() ?? "", Method = _loginMethod.SelectedItem as string ?? "POST",
        Parameters = _loginParameters.Text ?? "", ParameterMode = (ReportApiParameterMode)_loginMode.SelectedIndex,
        TokenField = _tokenField.Text?.Trim() ?? "", HeaderName = _headerName.Text?.Trim() ?? "",
        HeaderPrefix = _headerPrefix.Text ?? ""
    };

    private void ObserveAuthentication()
    {
        void Changed(bool login)
        {
            if (_updatingAuthentication) return;
            if (login) ClearAuthenticationResponse();
            InvalidateData();
            UpdateHeaderPreview();
        }
        ObserveText(_loginUrl, () => Changed(true));
        ObserveText(_loginParameters, () => Changed(true));
        _loginMethod.SelectionChanged += (_, _) => { _loginMode.IsEnabled = _loginMethod.SelectedItem as string != "GET"; Changed(true); };
        _loginMode.SelectionChanged += (_, _) => Changed(true);
        ObserveText(_tokenField, () => { Changed(false); SynchronizeTokenSelection(); });
        ObserveText(_headerPrefix, () => Changed(false));
        _headerName.PropertyChanged += (_, e) => { if (e.Property == AutoCompleteBox.TextProperty) Changed(false); };
        _loginMode.IsEnabled = _loginMethod.SelectedItem as string != "GET";
    }

    private void ClearAuthenticationResponse()
    {
        _updatingAuthentication = true;
        try
        {
            _loginJson = null;
            _loginSnapshot = null;
            _loginResponse.Text = "";
            _tokenList.ItemsSource = null;
            _tokenList.SelectedItem = null;
            _headerValue.Text = "";
        }
        finally { _updatingAuthentication = false; }
    }

    private void SynchronizeTokenSelection()
    {
        if (_updatingAuthentication) return;
        var path = _tokenField.Text?.Trim() ?? "";
        if (path.Length > 0 && !path.StartsWith('/'))
            path = "/" + string.Join("/", path.Split('.').Select(part => part.Replace("~", "~0").Replace("/", "~1")));
        _updatingAuthentication = true;
        try { _tokenList.SelectedItem = _tokenList.ItemsSource?.Cast<ReportDataNode>().FirstOrDefault(node => node.Pointer == path); }
        finally { _updatingAuthentication = false; }
    }

    private void UpdateHeaderPreview()
    {
        _headerValue.Text = "";
        if (_loginJson is null || ReadAuthentication() is not { } authentication) return;
        try
        {
            authentication.ResolveHeader(_loginJson);
            _headerValue.Text = authentication.HeaderName + ": " + authentication.HeaderPrefix + "••••••••";
        }
        catch (ArgumentException) { }
    }

    private Task<bool> FetchLoginAsync(bool force = false, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (ReadAuthentication() is null) return Task.FromResult(false);
        if (_pendingLogin is not { IsCompleted: false } || _cancellation?.IsCancellationRequested != false)
        {
            if (!force && _loginJson is not null && _loginSnapshot == ReadAuthentication()?.LoginRequest) return Task.FromResult(true);
            _pendingLogin = FetchLoginCoreAsync();
        }
        return AwaitRequestAsync(_pendingLogin, _cancellation, cancellationToken);
    }

    private async Task<bool> FetchLoginCoreAsync()
    {
        var request = ReadAuthentication()!.LoginRequest;
        InvalidateData(false);
        ClearAuthenticationResponse();
        var operation = StartRequest();
        _status.Text = "正在调用权限接口…";
        try
        {
            var json = await ReportResources.FetchJsonAsync(request, _client, operation.Token);
            operation.Token.ThrowIfCancellationRequested();
            if (_closed || operation.Generation != _generation) return false;
            var preview = FormatResponseJson(json, true);
            var nodes = ReportJsonData.CreateTree(json).SelectMany(Flatten)
                .Where(node => node.Kind == ReportDataNodeKind.Scalar && node.TypeLabel == "值")
                .Where(node => ReportJsonData.ReadRootValue(json, node.Pointer) is string or long or double or decimal).ToArray();
            _loginJson = json;
            _loginSnapshot = request;
            _loginResponse.Text = preview;
            _updatingAuthentication = true;
            try
            {
                _tokenList.ItemsSource = nodes;
                _tokenList.SelectedItem = nodes.FirstOrDefault(node => node.Pointer == _tokenField.Text);
            }
            finally { _updatingAuthentication = false; }
            UpdateHeaderPreview();
            _status.Text = "权限接口调用成功，请继续选择 Token 数据。";
            return true;
        }
        catch (Exception error) when (_closed || IsRequestError(error))
        {
            if (!_closed && operation.Generation == _generation) _status.Text = "权限获取失败：" + error.Message;
            return false;
        }
        finally { if (!_closed && operation.Generation == _generation) SetRequestBusy(false); }

        static IEnumerable<ReportDataNode> Flatten(ReportDataNode node) => new[] { node }.Concat(node.Children.SelectMany(Flatten));
    }

    private async Task<IReadOnlyList<StepValidationError>> ValidateLoginAsync(CancellationToken token)
    {
        if (RequestErrors(ReadAuthentication()!.LoginRequest, "LoginUrl", "LoginParameters") is { Count: > 0 } errors) return errors;
        var task = FetchLoginAsync(cancellationToken: token);
        var generation = _generation;
        var success = await task;
        token.ThrowIfCancellationRequested();
        if (generation != _generation) return [new("LoginResponse", "权限设置已变更，请重新调用接口。")];
        if (success) return [];
        var message = _status.Text ?? "请重新获取权限。";
        _status.Text = "请检查当前步骤。";
        return [new("LoginResponse", message)];
    }

    private Task<IReadOnlyList<StepValidationError>> ValidateHeaderAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        IReadOnlyList<StepValidationError> errors = [];
        var authentication = ReadAuthentication()!;
        if (_loginJson is null || _loginSnapshot != authentication.LoginRequest)
            errors = [new("TokenField", "权限接口已变更，请返回第一步重新调用。")];
        else
        {
            try { authentication.ValidateHeader(authentication.HeaderPrefix + "token"); }
            catch (ArgumentException error) { errors = [new("HeaderName", error.Message)]; }
            if (errors.Count == 0)
            {
                try { authentication.ResolveHeader(_loginJson); UpdateHeaderPreview(); }
                catch (ArgumentException error) { errors = [new("TokenPath", error.Message)]; }
            }
        }
        return Task.FromResult(errors);
    }
}
