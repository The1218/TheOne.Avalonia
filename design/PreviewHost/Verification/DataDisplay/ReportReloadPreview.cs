using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportReloadPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        using var server =
            new ReportApiComponentsPreview.Endpoint(
                "{\"title\":\"新标题\",\"rows\":[{\"name\":\"新版 A\"},{\"name\":\"新版 B\"}]}");
        var document = new ReportDocument
        {
            DataSourceUrl = server.Url + "/rows", DataBodyPointer = "/rows", DataSourceMethod = "POST",
            DataSourceParameters = "{\"page\":2}"
        };
        document.Columns.Add(new ReportColumn { Header = "名称", FieldKey = "/name" });
        document.Elements.Add(new ReportElement
            { Name = "标题", Text = "旧标题", FieldKey = "$root/title", X = 14, Y = 15, Width = 80, Height = 12 });
        document.Elements.Add(new ReportElement
            { Name = "明细", Kind = ReportElementKind.Table, X = 14, Y = 60, Width = 182, Height = 24 });
        var json = document.Save();
        var view = new ReportDesign();
        var host = new ContentControl { Content = view };
        var window = new Window
            { Content = host, Width = 1400, Height = 900, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        try
        {
            view.LoadTemplate(json);
            Until(() => view.DataSource?.RowCount == 2);
            check(
                server.Requests.Count == 1 && server.Requests.Last().Method == "POST" &&
                server.Requests.Last().Body.Contains("page"),
                "Report reload: legacy synchronous LoadTemplate automatically fetches saved method and parameters once");
            check(
                view.Layout!.Elements.Single(element => element.Name == "标题").Text == "新标题" && view.Layout.Pages
                    .SelectMany(page => page.Rows).Select(row => row.Cells[0]).SequenceEqual(new[] { "新版 A", "新版 B" }),
                "Report reload: root scalar and collection-relative table fields restore together without dropping nodes again");
            check(view.SaveTemplate() == json && !view.FindControl<Button>("UndoButton")!.IsEnabled,
                "Report reload: runtime data restoration leaves saved definition and undo history unchanged");
            var tree = view.FindControl<TreeView>("DataTree")!;
            check(tree.Items.Count == 1, "Report reload: fetched JSON repopulates the data tree");
            Click(window,
                view.FindControl<TabStrip>("ReportNameTabs")!.GetRealizedContainers().OfType<Control>().Single()
                    .GetVisualDescendants().OfType<Button>().First(button => button.Name == "PART_CloseButton"));
            check(view.Document is null && view.DataSource is null && tree.Items.Count == 0,
                "Report reload: closing clears the current document, source and data tree");
            server.Json = "{\"title\":\"重开标题\",\"rows\":[{\"name\":\"重新获取\"}]}";
            Await(view.LoadTemplateAsync(json));
            check(
                view.DataSource?.RowCount == 1 &&
                view.Layout!.Elements.Single(element => element.Name == "标题").Text == "重开标题" &&
                view.Layout.Pages.SelectMany(page => page.Rows).Single().Cells[0] == "重新获取",
                "Report reload: close/reopen saved JSON obtains fresh values and restores element bindings");
            var previous = view.Document;
            var rejected = false;
            try
            {
                view.LoadTemplate("invalid JSON");
            }
            catch (JsonException)
            {
                rejected = true;
            }

            check(rejected && ReferenceEquals(previous, view.Document),
                "Report reload: invalid template validation retains the existing document");
            var form = ReportDocument.Load(json);
            form.DataSourceParameterMode = ReportApiParameterMode.Form;
            Await(view.LoadTemplateAsync(form.Save()));
            check(
                server.Requests.Last().ContentType.StartsWith("application/x-www-form-urlencoded") &&
                server.Requests.Last().Body == "page=2",
                "Report reload: saved Form parameter mode is applied when reopening");

            var auth = ReportDocument.Load(json);
            auth.DataSourceAuthentication = new ReportApiAuthentication
            {
                Url = server.Url + "/login", Parameters = "{\"username\":\"example\"}", HeaderName = "X-Auth-Token",
                HeaderPrefix = ""
            };
            var count = server.Requests.Count;
            Await(view.LoadTemplateAsync(auth.Save()));
            check(
                server.Requests.Count == count + 2 && server.Requests.ToArray()[^2].Path == "/login" &&
                server.Requests.Last().Headers["X-Auth-Token"] == "session-" + (count + 1),
                "Report reload: saved authentication logs in before data fetching and restores bindings with a fresh token");

            var invalid = ReportDocument.Load(json);
            invalid.DataSourceUrl = server.Url + "/invalid";
            var failed = false;
            try
            {
                Await(view.LoadTemplateAsync(invalid.Save()));
            }
            catch (JsonException)
            {
                failed = true;
            }

            check(
                failed && view.Document?.Elements.Count == 2 && view.Document.Columns[0].FieldKey == "/name" &&
                view.DataSource is null && view.FindControl<TextBlock>("ApiSourceStatus")!.IsVisible,
                "Report reload: HTTP/JSON failures keep the loaded definition and bindings while displaying an error");
            Await(view.LoadTemplateAsync(json));
            check(view.DataSource is not null && !view.FindControl<TextBlock>("ApiSourceStatus")!.IsVisible,
                "Report reload: retrying a saved template recovers the source and clears its error");

            var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            server.ResponseFactory = async (request, token) =>
            {
                if (request.Path == "/slow") await gate.Task.WaitAsync(token);
                return server.Json;
            };
            var slow = ReportDocument.Load(json);
            slow.DataSourceUrl = server.Url + "/slow";
            var pending = view.LoadTemplateAsync(slow.Save());
            Until(() => server.Requests.Last().Path == "/slow");
            var replacement = new ReportDocument { Title = "后打开的报表" };
            view.LoadTemplate(replacement.Save());
            gate.SetResult();
            ObserveCancellation(pending);
            check(view.Document?.Title == replacement.Title && view.DataSource is null && tree.Items.Count == 0,
                "Report reload: a delayed old response cannot overwrite a newer document");

            gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            using (var cancellation = new CancellationTokenSource())
            {
                count = server.Requests.Count;
                pending = view.LoadTemplateAsync(slow.Save(), cancellation.Token);
                Until(() => server.Requests.Count > count);
                cancellation.Cancel();
                ObserveCancellation(pending);
                gate.SetResult();
                check(view.DataSource is null && view.Document?.DataSourceUrl == slow.DataSourceUrl,
                    "Report reload: caller cancellation leaves the saved definition without committing partial data");
            }

            gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            count = server.Requests.Count;
            pending = view.LoadTemplateAsync(slow.Save());
            Until(() => server.Requests.Count > count);
            var supplied = ReportDataSource.Create("Application", new[] { "应用数据" },
                new ReportField<string>("/name", "名称", value => value),
                new ReportField<string>("$root/title", "标题", value => value));
            view.DataSource = supplied;
            gate.SetResult();
            ObserveCancellation(pending);
            check(ReferenceEquals(view.DataSource, supplied),
                "Report reload: a source supplied by the consumer while fetching takes precedence over the pending response");

            gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            count = server.Requests.Count;
            pending = view.LoadTemplateAsync(slow.Save());
            Until(() => server.Requests.Count > count);
            host.Content = new TextBlock();
            Flush();
            ObserveCancellation(pending);
            host.Content = view;
            Flush();
            Until(() => server.Requests.Count >= count + 2);
            gate.SetResult();
            Until(() => view.DataSource is not null);
            check(
                view.Document?.DataSourceUrl == slow.DataSourceUrl &&
                view.Layout!.Elements.Single(element => element.Name == "标题").Text == "重开标题",
                "Report reload: an interrupted automatic load restarts on reattachment and restores data");

            gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            count = server.Requests.Count;
            pending = view.LoadTemplateAsync(slow.Save());
            Until(() => server.Requests.Count > count);
            Click(window,
                view.FindControl<TabStrip>("ReportNameTabs")!.GetRealizedContainers().OfType<Control>().Single()
                    .GetVisualDescendants().OfType<Button>().First(button => button.Name == "PART_CloseButton"));
            gate.SetResult();
            ObserveCancellation(pending);
            check(view.Document is null && view.DataSource is null && tree.Items.Count == 0,
                "Report reload: closing during loading cancels the request and prevents a late response reopening the report");
            server.ResponseFactory = null;
            view.Document = ReportDocument.Load(json);
            Until(() => view.DataSource is not null);
            check(view.Layout!.Elements.Single(element => element.Name == "标题").Text == "重开标题",
                "Report reload: direct MVVM Document assignment also restores optional API data");
            var priorSource = view.DataSource;
            count = server.Requests.Count;
            view.Document = ReportDocument.Load(json);
            Until(() => view.DataSource is not null && !ReferenceEquals(view.DataSource, priorSource));
            check(server.Requests.Count == count + 1,
                "Report reload: replacing a document through MVVM replaces its old API source with a fresh fetch");
            count = server.Requests.Count;
            host.Content = new TextBlock();
            Flush();
            host.Content = view;
            Flush();
            check(server.Requests.Count == count && view.DataSource is not null,
                "Report reload: navigating away and back keeps an already loaded source without refetching");
            VerifyImages(check, view);
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, "report-reloaded.png"), PngBitmapEncoderOptions.Default);
            }

            count = server.Requests.Count;
            Await(view.LoadTemplateAsync(new ReportDocument().Save()));
            check(view.DataSource is null && server.Requests.Count == count,
                "Report reload: source-free templates remain static and issue no API request");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyImages(Action<bool, string> check, ReportDesign view)
    {
        using var images = new ReportDataTreePreview.Endpoint { Refreshed = true };
        var document = new ReportDocument { DataSourceUrl = images.Url + "/json", DataBodyPointer = "/data/rows" };
        document.Elements.Add(new ReportElement
        {
            Kind = ReportElementKind.Image, Name = "绑定图片", FieldKey = "$root/meta/logo",
            ImageUrl = images.Url + "/image", X = 14, Y = 15, Width = 20, Height = 20
        });
        Await(view.LoadTemplateAsync(document.Save()));
        check(
            view.Document!.Elements[0].ImageUrl == images.Url + "/image-v2" && images.ImageRequests == 1 &&
            view.ValidationError is null,
            "Report reload: bound server images resolve the current source URL and load before completion");
    }

    private static void ObserveCancellation(Task task)
    {
        try
        {
            Await(task);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static void Await(Task task)
    {
        Until(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
        Flush();
    }

    private static void Until(Func<bool> ready)
    {
        var end = DateTime.UtcNow.AddSeconds(12);
        while (!ready() && DateTime.UtcNow < end)
        {
            Thread.Sleep(10);
            Flush();
        }

        if (!ready()) throw new TimeoutException("Report reload did not complete.");
        Flush();
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

    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }
}