using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportDataSourceDialog
{
    private Border _authenticationPanel = null!;
    private Button _fetchAuthentication = null!;
    private AutoCompleteBox _headerName = null!;
    private ComboBox _loginMethod = null!, _loginMode = null!;
    private TextBox _loginResponse = null!;
    private TextBlock _loginStatus = null!;

    private TextBox _loginUrl = null!,
        _loginParameters = null!,
        _tokenField = null!,
        _headerPrefix = null!,
        _headerValue = null!;

    private CheckBox _requiresAuthentication = null!;

    private Control CreateAuthentication(ReportApiAuthentication? initial)
    {
        var settings = initial ?? new ReportApiAuthentication();
        _requiresAuthentication = new CheckBox
            { Name = "ApiRequiresAuthentication", Content = "需要权限验证（先登录获取 token）", IsChecked = initial is not null };
        var form = new StackPanel { Spacing = 12 };
        var heading = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 12 };
        var title = ReportDialogChrome.Text("登录与权限请求头", weight: FontWeight.SemiBold);
        title.VerticalAlignment = VerticalAlignment.Center;
        heading.Children.Add(title);
        _fetchAuthentication = ReportDialogChrome.Button("FetchApiLoginRequest", "获取权限");
        Add(heading, _fetchAuthentication, 1);
        form.Children.Add(heading);
        _loginMethod = new ComboBox
        {
            Name = "ApiLoginMethod", ItemsSource = new[] { "POST", "GET", "PUT", "PATCH", "DELETE" },
            SelectedItem = settings.Method.ToUpperInvariant(), HorizontalAlignment = HorizontalAlignment.Stretch
        };
        _loginUrl = new TextBox
            { Name = "ApiLoginUrl", Text = settings.Url, PlaceholderText = "https://example.com/api/login" };
        _loginMode = new ComboBox
        {
            Name = "ApiLoginParameterMode", ItemsSource = new[] { "JSON 请求体", "Form 表单" },
            SelectedIndex = (int)settings.ParameterMode, HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var address = new Grid { ColumnDefinitions = new ColumnDefinitions("110,*,160"), ColumnSpacing = 12 };
        Add(address, ReportDialogChrome.Field("登录方式", _loginMethod), 0);
        Add(address, ReportDialogChrome.Field("登录接口 URL", _loginUrl), 1);
        Add(address, ReportDialogChrome.Field("参数格式", _loginMode), 2);
        form.Children.Add(address);
        _loginParameters = new TextBox
        {
            Name = "ApiLoginParameters", Text = settings.Parameters,
            PlaceholderText = "{\"username\": \"\", \"password\": \"\"}", AcceptsReturn = true, Height = 100,
            VerticalContentAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Menlo,Consolas")
        };
        ScrollViewer.SetVerticalScrollBarVisibility(_loginParameters, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(_loginParameters, ScrollBarVisibility.Auto);
        _loginResponse = new TextBox
        {
            Name = "ApiLoginResponseJson", IsReadOnly = true, AcceptsReturn = true, Height = 100,
            TextWrapping = TextWrapping.NoWrap, PlaceholderText = "点击获取权限，检查登录响应和 token 字段",
            VerticalContentAlignment = VerticalAlignment.Top, FontFamily = new FontFamily("Menlo,Consolas")
        };
        ScrollViewer.SetVerticalScrollBarVisibility(_loginResponse, ScrollBarVisibility.Auto);
        ScrollViewer.SetHorizontalScrollBarVisibility(_loginResponse, ScrollBarVisibility.Auto);
        var panes = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 16 };
        Add(panes, ReportDialogChrome.Field("登录入参 · JSON 对象", _loginParameters), 0);
        Add(panes, ReportDialogChrome.Field("登录响应 JSON", _loginResponse), 1);
        form.Children.Add(panes);
        var fields = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*,130"), ColumnSpacing = 12 };
        _tokenField = new TextBox
            { Name = "ApiTokenField", Text = settings.TokenField, PlaceholderText = "/data/token 或 data.token" };
        // Avalonia's editable selection control is AutoCompleteBox. Use its native input,
        // list selection, keyboard handling and popup, with a button to show all suggestions.
        _headerName = new AutoCompleteBox
        {
            Name = "ApiAuthHeaderName", Text = settings.HeaderName,
            ItemsSource = new[] { "Authorization", "X-API-Key", "X-Auth-Token", "Token", "Access-Token", "Auth-Token" },
            MinimumPrefixLength = 0, FilterMode = AutoCompleteFilterMode.None, IsTextCompletionEnabled = false,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var expand = new Button
        {
            Name = "ApiAuthHeaderSuggestions",
            Content = new PathIcon { Data = Geometry.Parse("M 1,3 L 6,8 L 11,3"), Width = 10, Height = 6 },
            Padding = new Thickness(8), MinWidth = 0, MinHeight = 0, Focusable = false,
            Background = Brushes.Transparent, BorderThickness = new Thickness(0)
        };
        expand.Click += (_, _) =>
        {
            _headerName.Focus();
            _headerName.IsDropDownOpen = !_headerName.IsDropDownOpen;
        };
        _headerName.InnerRightContent = expand;
        _headerPrefix = new TextBox
            { Name = "ApiAuthHeaderPrefix", Text = settings.HeaderPrefix, PlaceholderText = "可留空" };
        Add(fields, ReportDialogChrome.Field("响应 token 字段", _tokenField), 0);
        Add(fields, ReportDialogChrome.Field("请求头名称 · 可输入", _headerName), 1);
        Add(fields, ReportDialogChrome.Field("值前缀 · 保留空格", _headerPrefix), 2);
        form.Children.Add(fields);
        _headerValue = new TextBox
            { Name = "ApiAuthHeaderValue", IsReadOnly = true, PlaceholderText = "登录成功后自动填入：前缀 + token 字段值" };
        form.Children.Add(ReportDialogChrome.Field("请求头值 · 自动获取", _headerValue));
        _loginStatus = ReportDialogChrome.Text("可单独获取权限，确认登录响应和 token 字段是否正确。", 12, true);
        _loginStatus.Name = "ApiLoginStatus";
        _loginStatus.TextWrapping = TextWrapping.Wrap;
        form.Children.Add(_loginStatus);
        _authenticationPanel = new Border
            { Name = "ApiAuthenticationPanel", Child = form, IsVisible = initial is not null };
        return _authenticationPanel;
    }

    private static void Add(Grid grid, Control field, int column)
    {
        Grid.SetColumn(field, column);
        grid.Children.Add(field);
    }

    private ReportApiAuthentication? ReadAuthentication()
    {
        return _requiresAuthentication.IsChecked != true
            ? null
            : new ReportApiAuthentication
            {
                Url = _loginUrl.Text?.Trim() ?? "", Method = _loginMethod.SelectedItem as string ?? "POST",
                Parameters = _loginParameters.Text ?? "",
                ParameterMode = (ReportApiParameterMode)_loginMode.SelectedIndex,
                TokenField = _tokenField.Text?.Trim() ?? "", HeaderName = _headerName.Text?.Trim() ?? "",
                HeaderPrefix = _headerPrefix.Text ?? ""
            };
    }

    private void ObserveAuthentication(Action invalidate)
    {
        void Changed(bool loginChanged = false)
        {
            _authenticationPanel.IsVisible = _requiresAuthentication.IsChecked == true;
            _loginMode.IsEnabled = _loginMethod.SelectedItem as string != "GET";
            _headerValue.Text = "";
            if (loginChanged) _loginResponse.Text = "";
            _loginStatus.Text = "权限设置已变更，请重新获取权限验证。";
            invalidate();
        }

        _requiresAuthentication.IsCheckedChanged += (_, _) => Changed();
        foreach (var text in new[] { _loginUrl, _loginParameters }) text.TextChanged += (_, _) => Changed(true);
        foreach (var text in new[] { _tokenField, _headerPrefix }) text.TextChanged += (_, _) => Changed();
        _loginMethod.SelectionChanged += (_, _) => Changed(true);
        _loginMode.SelectionChanged += (_, _) => Changed(true);
        _headerName.PropertyChanged += (_, args) =>
        {
            if (args.Property == AutoCompleteBox.TextProperty) Changed();
        };
        _loginMode.IsEnabled = _loginMethod.SelectedItem as string != "GET";
    }

    private void EnableAuthentication(bool enabled)
    {
        _requiresAuthentication.IsEnabled = enabled;
        _authenticationPanel.IsEnabled = enabled;
    }
}