using System.Net;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ReportAuthenticationPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        using var server = new ReportApiComponentsPreview.Endpoint();
        var auth = new ReportApiAuthentication
            { Url = server.Url + "/login", Parameters = "{\"username\":\"设计师\",\"password\":\"example\"}" };
        var request = new ReportApiRequest(server.Url + "/rows", "POST", "{\"page\":1}") { Authentication = auth };
        using var client = new HttpClient();
        Await(ReportResources.FetchJsonAsync(request, client));
        var requests = server.Requests.ToArray();
        check(requests.Length == 2 && requests[0].Path == "/login" && requests[0].Method == "POST" &&
              requests[0].Body.Contains("设计师")
              && requests[1].Headers["Authorization"] == "Bearer session-1" && requests[1].Body.Contains("page"),
            "Report authentication: login JSON precedes the data request and resolves the nested token into Authorization");
        check(!client.DefaultRequestHeaders.Any(), "Report authentication: shared HttpClient headers are unchanged");
        Await(ReportResources.FetchJsonAsync(
            request with
            {
                Authentication = auth with
                {
                    HeaderName = "X-Custom-Token", HeaderPrefix = "", TokenField = "data.token",
                    ParameterMode = ReportApiParameterMode.Form
                }
            }, client));
        requests = server.Requests.ToArray();
        check(
            requests[^2].ContentType.StartsWith("application/x-www-form-urlencoded") &&
            requests[^2].Body.Contains("password=example") && requests[^1].Headers["X-Custom-Token"] == "session-3" &&
            !requests[^1].Headers.ContainsKey("Authorization"),
            "Report authentication: form login, dotted token path and custom raw-token header work independently");
        var count = server.Requests.Count;
        Await(ReportResources.FetchJsonAsync(request with { Authentication = null }, client));
        check(
            server.Requests.Count == count + 1 && !server.Requests.Last().Headers.ContainsKey("Authorization") &&
            !server.Requests.Last().Headers.ContainsKey("X-Custom-Token"),
            "Report authentication: unchecked configuration makes exactly one direct data request without a stale token");
        foreach (var invalid in new[]
                 {
                     auth with { TokenField = "/missing" }, auth with { TokenField = "/data" },
                     auth with { Url = server.Url + "/denied" }, auth with { Url = server.Url + "/invalid" }
                 })
        {
            count = server.Requests.Count;
            var rejected = false;
            try
            {
                Await(ReportResources.FetchJsonAsync(request with { Authentication = invalid }, client));
            }
            catch (Exception error) when (error is ArgumentException or HttpRequestException or JsonException)
            {
                rejected = true;
            }

            check(rejected && server.Requests.Count == count + 1 && server.Requests.Last().Path != "/rows",
                "Report authentication: failed login or invalid token stops before the data endpoint (" +
                invalid.TokenField + ", " + new Uri(invalid.Url).AbsolutePath + ")");
        }

        foreach (var invalid in new[]
                 {
                     auth with { HeaderName = "Bad Header" }, auth with { HeaderPrefix = "Bearer\r\nInjected: " },
                     auth with { TokenField = "" }, auth with { Url = "file:///login" }
                 })
        {
            count = server.Requests.Count;
            var rejected = false;
            try
            {
                Await(ReportResources.FetchJsonAsync(request with { Authentication = invalid }, client));
            }
            catch (ArgumentException)
            {
                rejected = true;
            }

            check(rejected && server.Requests.Count == count,
                "Report authentication: invalid settings fail before either HTTP request");
        }

        using (var cancellation = new CancellationTokenSource())
        using (var handler = new CancelAfterLogin(cancellation))
        using (var cancelClient = new HttpClient(handler))
        {
            var canceled = false;
            try
            {
                Await(ReportResources.FetchJsonAsync(request, cancelClient, cancellation.Token));
            }
            catch (OperationCanceledException)
            {
                canceled = true;
            }

            check(canceled && handler.Calls == 1,
                "Report authentication: cancellation after login prevents the data request");
        }

        VerifyDialog(check, server, auth, output);
    }

    private static void VerifyDialog(Action<bool, string> check, ReportApiComponentsPreview.Endpoint server,
        ReportApiAuthentication auth, string? output)
    {
        var document = new ReportDocument();
        document.Elements.Add(new ReportElement
            { Name = "状态", Kind = ReportElementKind.Text, X = 15, Y = 60, Width = 60, Height = 10 });
        var designer = new ReportDesign { Document = document };
        var owner = new Window
            { Content = designer, Width = 1586, Height = 992, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(owner, false);
        owner.Show();
        Flush();
        try
        {
            Click(owner, Part<TabItem>(designer, "DataTabButton"));
            Click(owner, Part<Button>(designer, "AddDataSourceButton"));
            var dialog = owner.OwnedWindows.Single(window => window.Title == "添加数据源");
            Motion.SetEnabled(dialog, false);
            check(!Part<Border>(dialog, "ApiAuthenticationPanel").IsVisible,
                "Report authentication/dialog: login region starts hidden for a direct source");
            Click(dialog, Part<CheckBox>(dialog, "ApiRequiresAuthentication"));
            check(Part<Border>(dialog, "ApiAuthenticationPanel").IsVisible,
                "Report authentication/dialog: checkbox expands the login configuration");
            Part<TextBox>(dialog, "ApiLoginUrl").Text = auth.Url;
            Part<TextBox>(dialog, "ApiLoginParameters").Text = auth.Parameters;
            var header = Part<AutoCompleteBox>(dialog, "ApiAuthHeaderName");
            Click(dialog, Part<Button>(dialog, "ApiAuthHeaderSuggestions"));
            check(header.IsDropDownOpen,
                "Report authentication/dialog: editable native header selector opens the suggestions list");
            header.IsDropDownOpen = false;
            var input = header.GetVisualDescendants().OfType<TextBox>().Single();
            input.Focus();
            input.SelectAll();
            dialog.KeyTextInput("X-Workspace-Token");
            Flush();
            Part<TextBox>(dialog, "ApiAuthHeaderPrefix").Text = "";
            check(header.Text == "X-Workspace-Token",
                "Report authentication/dialog: real typing accepts a header beyond the common suggestions");
            dialog.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            dialog.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Flush();
            var before = document.Save();
            var count = server.Requests.Count;
            Click(dialog, Part<Button>(dialog, "FetchApiLoginRequest"));
            Until(() => Part<TextBlock>(dialog, "ApiLoginStatus").Text!.Contains("权限获取成功"));
            check(server.Requests.Count == count + 1 && server.Requests.Last().Path == "/login" &&
                  Part<TextBox>(dialog, "ApiRequestUrl").Text == ""
                  && Part<TextBox>(dialog, "ApiLoginResponseJson").Text!.Contains("token") &&
                  Part<TextBox>(dialog, "ApiAuthHeaderValue").Text!.StartsWith("session-"),
                "Report authentication/dialog: standalone login works before filling a data URL and displays response JSON and the resolved header");
            check(
                !Part<Button>(dialog, "ConfirmDataSource").IsEnabled && designer.DataSource is null &&
                document.Save() == before,
                "Report authentication/dialog: testing login cannot confirm or import a data source");
            var testedHeader = Part<TextBox>(dialog, "ApiAuthHeaderValue").Text;
            Part<TextBox>(dialog, "ApiRequestUrl").Text = server.Url + "/rows";
            Flush();
            check(Part<TextBox>(dialog, "ApiAuthHeaderValue").Text == testedHeader,
                "Report authentication/dialog: configuring the data endpoint keeps the tested token visible");
            count = server.Requests.Count;
            Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
            Until(() => Part<Button>(dialog, "ConfirmDataSource").IsEnabled ||
                        Part<TextBlock>(dialog, "ApiRequestStatus").Text!.Contains("获取失败"));
            check(Part<Button>(dialog, "ConfirmDataSource").IsEnabled,
                "Report authentication/dialog status: " + Part<TextBlock>(dialog, "ApiRequestStatus").Text);
            check(server.Requests.Count == count + 2 &&
                  server.Requests.Last().Headers.ContainsKey("X-Workspace-Token") &&
                  Part<TextBox>(dialog, "ApiAuthHeaderValue").Text!.StartsWith("session-")
                  && Part<TextBox>(dialog, "ApiResponseJson").Text!.Contains("approved") && document.Save() == before,
                "Report authentication/dialog: successful login fills the header value and previews JSON before importing");
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
            {
                dialog.RequestedThemeVariant = theme;
                ControlSizing.SetSize(dialog, size);
                Flush();
                Part<ScrollViewer>(dialog, "ApiDataSourceFormScroll").Offset = default;
                Flush();
                var button = Part<Button>(dialog, "ConfirmDataSource");
                var position = button.TranslatePoint(default, dialog)!.Value;
                check(position.Y >= 0 && position.Y + button.Bounds.Height <= dialog.ClientSize.Height + 1,
                    $"Report authentication/dialog/{theme}/{size}: fixed footer remains visible");
                Save(dialog, output, $"report-auth-{theme}-{size}");
            }

            dialog.Width = 680;
            dialog.Height = 620;
            Flush();
            var scroll = Part<ScrollViewer>(dialog, "ApiDataSourceFormScroll");
            check(scroll.Extent.Height > scroll.Viewport.Height,
                "Report authentication/dialog: narrow viewport scrolls the form while retaining footer actions");
            scroll.Offset = default;
            Flush();
            Save(dialog, output, "report-auth-narrow");
            var tokenField = Part<TextBox>(dialog, "ApiTokenField");
            tokenField.Text = "/missing";
            Flush();
            check(
                !Part<Button>(dialog, "ConfirmDataSource").IsEnabled &&
                Part<TextBox>(dialog, "ApiAuthHeaderValue").Text == "",
                "Report authentication/dialog: editing authentication invalidates confirmation and clears the resolved header");
            count = server.Requests.Count;
            Click(dialog, Part<Button>(dialog, "FetchApiLoginRequest"));
            Until(() => Part<TextBlock>(dialog, "ApiLoginStatus").Text!.Contains("权限获取失败"));
            check(server.Requests.Count == count + 1 && server.Requests.Last().Path == "/login" &&
                  Part<TextBox>(dialog, "ApiLoginResponseJson").Text!.Contains("token")
                  && Part<TextBox>(dialog, "ApiAuthHeaderValue").Text == "" &&
                  !Part<Button>(dialog, "ConfirmDataSource").IsEnabled,
                "Report authentication/dialog: incorrect token path retains login JSON for inspection without calling the data endpoint");
            Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
            Until(() => Part<TextBlock>(dialog, "ApiRequestStatus").Text!.Contains("获取失败"));
            check(!Part<Button>(dialog, "ConfirmDataSource").IsEnabled,
                "Report authentication/dialog: missing token reports a recoverable error and disables confirmation");
            tokenField.Text = "/data/token";
            Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
            Until(() => Part<Button>(dialog, "ConfirmDataSource").IsEnabled);
            count = server.Requests.Count;
            Click(dialog, Part<Button>(dialog, "ConfirmDataSource"));
            check(server.Requests.Count == count && designer.DataSource is null,
                "Report authentication/dialog: confirm imports without logging in again or changing existing bindings");
            var tree = Part<TreeView>(designer, "DataTree");

            IEnumerable<ReportDataNode> All(ReportDataNode node)
            {
                return new[] { node }.Concat(node.Children.SelectMany(All));
            }

            var approved = tree.Items.OfType<ReportDataNode>().SelectMany(All)
                .Single(node => node.Pointer == "/approved");
            Await(designer.DropDataNodeAsync(approved, 17, 62));
            var json = document.Save();
            var restored = ReportDocument.Load(json);
            check(restored.DataSourceAuthentication?.HeaderName == "X-Workspace-Token" &&
                  restored.DataSourceAuthentication.Parameters == auth.Parameters
                  && !json.Contains("session-"),
                "Report authentication/template: login settings persist through source-generated JSON; fetched tokens stay out");
            Click(owner, Part<Button>(designer, "UndoButton"));
            check(document.DataSourceAuthentication is null,
                "Report authentication/history: undo removes the source authentication alongside its field binding");
            Click(owner, Part<Button>(designer, "RedoButton"));
            check(document.DataSourceAuthentication?.HeaderName == "X-Workspace-Token",
                "Report authentication/history: redo restores authentication with the source");
            var preview = new ReportPreview();
            var previewWindow = new Window { Content = preview, Width = 1000, Height = 800 };
            previewWindow.Show();
            try
            {
                count = server.Requests.Count;
                Await(preview.LoadAsync(json));
                check(
                    preview.Layout is not null && server.Requests.Count == count + 2 &&
                    server.Requests.Last().Headers["X-Workspace-Token"] == "session-" + (count + 1),
                    "Report authentication/independent preview: saved JSON logs in again and uses a fresh token");
            }
            finally
            {
                previewWindow.Close();
            }

            Await(designer.LoadTemplateAsync(json));
            Click(owner, Part<TabItem>(designer, "DataTabButton"));
            Click(owner, Part<Button>(designer, "AddDataSourceButton"));
            dialog = owner.OwnedWindows.Single(window => window.Title == "添加数据源");
            check(
                Part<CheckBox>(dialog, "ApiRequiresAuthentication").IsChecked == true &&
                Part<TextBox>(dialog, "ApiLoginUrl").Text == auth.Url &&
                Part<AutoCompleteBox>(dialog, "ApiAuthHeaderName").Text == "X-Workspace-Token",
                "Report authentication/template: loading prefills authentication in the source dialog");
            Click(dialog, Part<CheckBox>(dialog, "ApiRequiresAuthentication"));
            count = server.Requests.Count;
            Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
            Until(() => Part<Button>(dialog, "ConfirmDataSource").IsEnabled);
            check(
                server.Requests.Count == count + 1 && !server.Requests.Last().Headers.ContainsKey("X-Workspace-Token"),
                "Report authentication/dialog: unchecking authentication skips the login and header");
            Click(dialog, Part<Button>(dialog, "CancelDataSource"));
            check(designer.Document!.Save() == json,
                "Report authentication/dialog: cancel retains the saved authenticated source");
        }
        finally
        {
            owner.Close();
            Flush();
        }
    }

    private static T Part<T>(Control root, string name) where T : Control
    {
        return root.GetVisualDescendants().Prepend(root).OfType<T>().First(control => control.Name == name);
    }

    private static void Flush()
    {
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void Until(Func<bool> ready)
    {
        var end = DateTime.UtcNow.AddSeconds(12);
        while (!ready() && DateTime.UtcNow < end)
        {
            Thread.Sleep(10);
            Flush();
        }

        if (!ready()) throw new InvalidOperationException("Authentication condition timed out.");
    }

    private static void Await(Task task)
    {
        Until(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
        Flush();
    }

    private static void Click(Window window, Control control)
    {
        control.BringIntoView();
        Flush();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Save(Window window, string? output, string name)
    {
        if (output is null) return;
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    private sealed class CancelAfterLogin(CancellationTokenSource cancellation) : HttpMessageHandler
    {
        internal int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
        {
            Calls++;
            cancellation.Cancel();
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent("{\"data\":{\"token\":\"temporary\"}}") });
        }
    }
}
