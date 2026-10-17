using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using SkiaSharp;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportApiComponentsPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        using var endpoint = new Endpoint();
        VerifyRequests(check, endpoint);
        var document = new ReportDocument();
        var view = new ReportDesign { Document = document };
        var window = new Window
        {
            Content = view, Width = 1586, Height = 992, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = ThemeVariant.Light
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, ControlSize.Normal);
        window.Show();
        Flush();
        try
        {
            Click(window, Part<TabItem>(view, "DataTabButton"));
            var before = document.Save();

            Window OpenDialog()
            {
                Click(window, Part<Button>(view, "AddDataSourceButton"));
                var opened = window.OwnedWindows.Single(dialog => dialog.Title == "添加数据源");
                Motion.SetEnabled(opened, false);
                return opened;
            }

            void Fill(Window dialog)
            {
                Part<ComboBox>(dialog, "ApiRequestMethod").SelectedItem = "POST";
                Part<TextBox>(dialog, "ApiRequestUrl").Text = endpoint.Url;
                Part<TextBox>(dialog, "ApiRequestParameters").Text = "{\"page\":2,\"keyword\":\"咖啡\"}";
                Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
                Until(() => Part<Button>(dialog, "ConfirmDataSource").IsEnabled);
            }

            var dialog = OpenDialog();
            check(!Part<Button>(dialog, "ConfirmDataSource").IsEnabled,
                "Report API/dialog: confirmation is disabled before a successful response");
            Fill(dialog);
            check(
                Part<TextBox>(dialog, "ApiResponseJson").Text!.Contains("approved") &&
                Part<TreeView>(view, "DataTree").Items.Count == 0 && before == document.Save(),
                "Report API/dialog: fetching shows formatted JSON inside the dialog without importing or changing the report");
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
            {
                dialog.RequestedThemeVariant = theme;
                ControlSizing.SetSize(dialog, size);
                Flush();
                var action = Part<Button>(dialog, "ConfirmDataSource");
                var position = action.TranslatePoint(default, dialog)!.Value;
                check(
                    position.X >= 0 && position.Y >= 0 &&
                    position.X + action.Bounds.Width <= dialog.ClientSize.Width + 1 &&
                    position.Y + action.Bounds.Height <= dialog.ClientSize.Height + 1,
                    $"Report API/dialog/{theme}/{size}: native confirmation remains fully visible");
                Save(dialog, output, $"report-api-dialog-{theme}-{size}");
            }

            dialog.Width = 680;
            dialog.Height = 620;
            Flush();
            Save(dialog, output, "report-api-dialog-narrow");
            var paramsBox = Part<TextBox>(dialog, "ApiRequestParameters");
            paramsBox.Text = "{\"page\":3}";
            Flush();
            check(!Part<Button>(dialog, "ConfirmDataSource").IsEnabled,
                "Report API/dialog: changing request parameters invalidates the previous response confirmation");
            Click(dialog, Part<Button>(dialog, "CancelDataSource"));
            check(before == document.Save() && Part<TreeView>(view, "DataTree").Items.Count == 0,
                "Report API/dialog: cancel discards the pending source and keeps the report unchanged");
            dialog = OpenDialog();
            Fill(dialog);
            var requests = endpoint.Requests.Count;
            Click(dialog, Part<Button>(dialog, "ConfirmDataSource"));
            check(
                Part<TreeView>(view, "DataTree").Items.Count > 0 && endpoint.Requests.Count == requests &&
                view.DataSource is null,
                "Report API/dialog: confirm imports the fetched tree without making another request or replacing bound rows");
            var tree = Part<TreeView>(view, "DataTree");
            var roots = tree.Items.OfType<ReportDataNode>().ToArray();
            dialog = OpenDialog();
            Part<TextBox>(dialog, "ApiRequestUrl").Text = endpoint.Url + "/invalid";
            Click(dialog, Part<Button>(dialog, "FetchApiRequest"));
            Until(() => Part<TextBlock>(dialog, "ApiRequestStatus").Text!.Contains("获取失败"));
            check(
                !Part<Button>(dialog, "ConfirmDataSource").IsEnabled &&
                tree.Items.OfType<ReportDataNode>().SequenceEqual(roots),
                "Report API/dialog: invalid JSON cannot be confirmed and leaves the imported tree intact");
            Click(dialog, Part<Button>(dialog, "CancelDataSource"));

            Click(window, Part<TabItem>(view, "ComponentsTabButton"));
            var surface = Part<Control>(view, "PageSurface");

            Point Paper(double x, double y)
            {
                return surface.TranslatePoint(
                    new Point(88 + x * ReportPaginator.DipsPerMillimetre * view.Zoom,
                        30 + y * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
            }

            void DragPalette(string name, double x, double y)
            {
                var control = Part<Button>(view, name);
                var start = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
                    window)!.Value;
                var end = Paper(x, y);
                window.MouseMove(start);
                window.MouseDown(start, MouseButton.Left);
                window.MouseMove(start + new Vector(12, 4), RawInputModifiers.LeftMouseButton);
                window.MouseMove(end, RawInputModifiers.LeftMouseButton);
                window.MouseUp(end, MouseButton.Left);
                Flush();
            }

            DragPalette("CheckBoxPalette", 18, 65);
            DragPalette("BarcodePalette", 18, 100);
            var checkbox = document.Elements.Single(element => element.Kind == ReportElementKind.CheckBox);
            var barcode = document.Elements.Single(element => element.Kind == ReportElementKind.Barcode);
            check(Math.Abs(checkbox.Y - 65) < 1 && Math.Abs(barcode.Y - 100) < 1 && view.ValidationError is null,
                "Report components: real palette drags create independently positioned checkbox and barcode elements");
            view.SelectedElement = checkbox;
            Flush();
            Click(window, Part<CheckBox>(view, "ElementCheckedEditor"));
            check(checkbox.IsChecked, "Report checkbox: the native inspector edits its default checked state");
            checkbox.Text = "已审核";

            IEnumerable<ReportDataNode> All(ReportDataNode node)
            {
                return new[] { node }.Concat(node.Children.SelectMany(All));
            }

            var nodes = roots.SelectMany(All).ToArray();
            Await(view.DropDataNodeAsync(nodes.Single(node => node.Pointer == "/approved"), checkbox.X + 2,
                checkbox.Y + 2));
            Await(view.DropDataNodeAsync(nodes.Single(node => node.Pointer == "/code"), barcode.X + 2, barcode.Y + 2));
            check(
                checkbox.Text == "已审核" && checkbox.FieldKey == "$root/approved" && view.Layout!.Elements
                    .Single(element => element.Kind == ReportElementKind.CheckBox).Text is "True" or "是",
                "Report checkbox: a dropped boolean binds the checked value while preserving the caption");
            check(
                barcode.Text == "AB12" && barcode.FieldKey == "$root/code" && document.DataSourceMethod == "POST" &&
                document.DataSourceParameters.Contains("咖啡"),
                "Report barcode/API: a scalar binds the code and the configured method and parameters persist with the report");
            var json = document.Save();
            var restored = ReportDocument.Load(json);
            check(restored.Elements.Any(element => element.Kind == ReportElementKind.CheckBox && element.IsChecked) &&
                  restored.Elements.Any(element => element.Kind == ReportElementKind.Barcode && element.BarcodeShowText)
                  && restored.DataSourceMethod == "POST" &&
                  restored.DataSourceParameters == document.DataSourceParameters,
                "Report components/template: checkbox, barcode, request method and parameters survive JSON save/load");
            view.SelectedElement = barcode;
            Part<TabControl>(view, "PropertyTabs").SelectedIndex = 0;
            Flush();
            Click(window, Part<CheckBox>(view, "BarcodeTextToggle"));
            check(!barcode.BarcodeShowText, "Report barcode: the native inspector can hide the human-readable code");
            Click(window, Part<CheckBox>(view, "BarcodeTextToggle"));
            view.ShowPreview();
            Flush();
            using (var frame = window.CaptureRenderedFrame()!)
            {
                var origin = Paper(barcode.X, barcode.Y);
                var scale = frame.Dpi.X / 96;
                using var stream = new MemoryStream();
                frame.Save(stream, PngBitmapEncoderOptions.Default);
                using var bitmap = SKBitmap.Decode(stream.ToArray());
                var y = (int)((origin.Y + 10 * view.Zoom) * scale);
                var startX = (int)((origin.X + 1) * scale);
                var endX = (int)((origin.X + barcode.Width * ReportPaginator.DipsPerMillimetre * view.Zoom - 1) *
                                 scale);
                var transitions = 0;
                var prior = false;
                for (var x = startX; x < endX; x++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    var dark = pixel.Red < 100 && pixel.Green < 100 && pixel.Blue < 100;
                    if (dark != prior) transitions++;
                    prior = dark;
                }

                check(transitions >= 36,
                    "Report barcode/pixels: preview contains real black/white bars, not a barcode font or placeholder");
                // Independent AB12 vector: Start B (104), 33,34,17,18, checksum 19, Stop (106).
                const string expectedRuns = "2112141113231311231232212232112211322331112";
                var unit = (barcode.Width * ReportPaginator.DipsPerMillimetre - 4) /
                           (expectedRuns.Sum(run => run - '0') + 20);
                var module = 10;
                var matches = true;
                for (var run = 0; run < expectedRuns.Length; run++)
                for (var offset = 0; offset < expectedRuns[run] - '0'; offset++, module++)
                {
                    var x = (int)((origin.X + (2 + (module + .5) * unit) * view.Zoom) * scale);
                    var pixel = bitmap.GetPixel(x, y);
                    var dark = pixel.Red < 100 && pixel.Green < 100 && pixel.Blue < 100;
                    matches &= dark == (run % 2 == 0);
                }

                check(matches,
                    "Report barcode/pixels: actual modules match the known AB12 start/data/checksum/stop pattern");
                var boxOrigin = Paper(checkbox.X, checkbox.Y);
                var boxSize = Math.Max(14, checkbox.FontSize);
                var tick = bitmap.GetPixel((int)((boxOrigin.X + (2 + boxSize * .43) * view.Zoom) * scale),
                    (int)((boxOrigin.Y +
                           ((checkbox.Height * ReportPaginator.DipsPerMillimetre - boxSize) / 2 + boxSize * .73) *
                           view.Zoom) * scale));
                check(tick.Red < 120 && tick.Green < 120 && tick.Blue < 120,
                    "Report checkbox/pixels: a bound true value draws a real check mark");
            }

            Save(window, output, "report-checkbox-barcode-preview");
            using var pdf = new MemoryStream();
            Await(ReportPdfExporter.ExportAsync(document, view.DataSource, pdf));
            check(Encoding.ASCII.GetString(pdf.ToArray(), 0, 4) == "%PDF",
                "Report components/PDF: the shared vector drawing path exports checkbox and barcode");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                File.WriteAllBytes(Path.Combine(output, "report-checkbox-barcode.pdf"), pdf.ToArray());
            }

            var preview = new ReportPreview();
            var previewWindow = new Window { Content = preview, Width = 1000, Height = 800 };
            previewWindow.Show();
            Flush();
            try
            {
                Await(preview.LoadAsync(json));
                check(preview.Layout is not null && endpoint.Requests.Last().Method == "POST" &&
                      endpoint.Requests.Last().Body.Contains("咖啡")
                      && preview.Layout.Elements.Any(element =>
                          element.Kind == ReportElementKind.Barcode && element.Text == "AB12"),
                    "Report components/independent preview: saved JSON repeats the POST parameters and resolves component fields");
                Save(previewWindow, output, "report-checkbox-barcode-independent");
            }
            finally
            {
                previewWindow.Close();
            }

            var invalid = ReportDocument.Load(json);
            invalid.Elements.Single(element => element.Kind == ReportElementKind.Barcode).FieldKey = "";
            invalid.Elements.Single(element => element.Kind == ReportElementKind.Barcode).Text = "不支持中文条码";
            var rejected = false;
            try
            {
                invalid.Save();
            }
            catch (ArgumentException)
            {
                rejected = true;
            }

            check(rejected,
                "Report barcode: unsupported characters fail validation instead of exporting an invalid symbol");
            var flags = new ReportDocument();
            flags.Elements.Add(
                new ReportElement { Kind = ReportElementKind.CheckBox, FieldKey = "flag", Text = "审核状态" });
            var invalidFlag = ReportDataSource.Create("Flags", new[] { "unknown" },
                new ReportField<string>("flag", "状态", value => value));
            rejected = false;
            try
            {
                ReportPaginator.Paginate(flags, invalidFlag);
            }
            catch (ArgumentException)
            {
                rejected = true;
            }

            check(rejected,
                "Report checkbox: unsupported field values fail validation instead of silently printing a default check state");
            restored.DataSourceMethod = "PUT";
            restored.DataSourceParameters = "{\"page\":9}";
            Await(view.LoadTemplateAsync(restored.Save()));
            Click(window, Part<TabItem>(view, "DataTabButton"));
            dialog = OpenDialog();
            check(
                Part<ComboBox>(dialog, "ApiRequestMethod").SelectedItem as string == "PUT" &&
                Part<TextBox>(dialog, "ApiRequestParameters").Text == "{\"page\":9}",
                "Report API/template: loading a report prefills its saved request instead of a previously fetched request");
            Click(dialog, Part<Button>(dialog, "CancelDataSource"));
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyRequests(Action<bool, string> check, Endpoint endpoint)
    {
        Await(ReportResources.FetchJsonAsync(new ReportApiRequest(endpoint.Url + "?existing=1", "GET",
            "{\"name\":\"咖啡 & 茶\",\"page\":2}")));
        var get = endpoint.Requests.Last();
        check(
            get.Method == "GET" && get.Path.Contains("existing=1&name=") &&
            Uri.UnescapeDataString(get.Path).Contains("咖啡 & 茶") && get.Body.Length == 0,
            "Report API/requests: GET preserves existing query values and correctly escapes Unicode and reserved characters");
        foreach (var method in new[] { "POST", "PUT", "PATCH", "DELETE" })
        {
            Await(ReportResources.FetchJsonAsync(new ReportApiRequest(endpoint.Url, method,
                "{\"filter\":{\"approved\":true},\"page\":2}")));
            var request = endpoint.Requests.Last();
            using var body = JsonDocument.Parse(request.Body);
            check(
                request.Method == method && request.ContentType.StartsWith("application/json") &&
                body.RootElement.GetProperty("filter").GetProperty("approved").GetBoolean(),
                $"Report API/requests: {method} sends a JSON body with nested parameter values");
        }

        Await(ReportResources.FetchJsonAsync(new ReportApiRequest(endpoint.Url, "POST",
            "{\"name\":\"a & b\",\"page\":2}", ReportApiParameterMode.Form)));
        var form = endpoint.Requests.Last();
        check(
            form.ContentType.StartsWith("application/x-www-form-urlencoded") && form.Body.Contains("name=a+%26+b") &&
            form.Body.Contains("page=2"), "Report API/requests: form parameters use standard URL encoding");
        var count = endpoint.Requests.Count;
        var rejected = false;
        try
        {
            Await(ReportResources.FetchJsonAsync(new ReportApiRequest(endpoint.Url, "POST", "[]")));
        }
        catch (ArgumentException)
        {
            rejected = true;
        }

        check(rejected && endpoint.Requests.Count == count,
            "Report API/requests: invalid parameter shapes are rejected before HTTP is sent");
    }

    private static T Part<T>(Control root, string name) where T : Control
    {
        return root.GetVisualDescendants().Prepend(root).OfType<T>().FirstOrDefault(control => control.Name == name) ??
               throw new InvalidOperationException("Missing " + name);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Until(Func<bool> ready)
    {
        var end = DateTime.UtcNow.AddSeconds(12);
        while (!ready() && DateTime.UtcNow < end)
        {
            Thread.Sleep(10);
            Flush();
        }

        if (!ready()) throw new InvalidOperationException("Report API condition timed out.");
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
        Flush();
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    internal sealed record Request(
        string Method,
        string Path,
        string Body,
        string ContentType,
        IReadOnlyDictionary<string, string> Headers);

    internal sealed class Endpoint : IDisposable
    {
        private readonly TcpListener _server = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stop = new();

        internal Endpoint(string? json = null)
        {
            Json = json ?? "{\"approved\":true,\"code\":\"AB12\",\"rows\":[{\"name\":\"咖啡\"}]}";
            _server.Start();
            Url = "http://127.0.0.1:" + ((IPEndPoint)_server.LocalEndpoint).Port;
            _ = Task.Run(async () =>
            {
                try
                {
                    while (!_stop.IsCancellationRequested)
                    {
                        var client = await _server.AcceptTcpClientAsync(_stop.Token);
                        _ = Respond(client);
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (SocketException) when (_stop.IsCancellationRequested)
                {
                }
            });
        }

        internal string Json { get; set; }
        internal Func<Request, CancellationToken, Task<string>>? ResponseFactory { get; set; }
        internal ConcurrentQueue<Request> Requests { get; } = new();
        internal string Url { get; }

        public void Dispose()
        {
            _stop.Cancel();
            _server.Stop();
            _stop.Dispose();
        }

        private async Task Respond(TcpClient client)
        {
            using (client)
            {
                try
                {
                    using var stream = client.GetStream();
                    var header = new List<byte>();
                    var single = new byte[1];
                    while (await stream.ReadAsync(single, _stop.Token) > 0)
                    {
                        header.Add(single[0]);
                        if (header.Count >= 4 && header.TakeLast(4).SequenceEqual(new byte[] { 13, 10, 13, 10 })) break;
                    }

                    var lines = Encoding.ASCII.GetString(header.ToArray()).Split("\r\n");
                    var first = lines[0].Split(' ');
                    var length = lines.FirstOrDefault(line =>
                        line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase));
                    var body = new byte[length is null ? 0 : int.Parse(length.Split(':')[1].Trim())];
                    if (body.Length > 0) await stream.ReadExactlyAsync(body, _stop.Token);
                    var contentType =
                        lines.FirstOrDefault(line =>
                                line.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))?.Split(':', 2)[1]
                            .Trim() ?? "";
                    var headers = lines.Skip(1).Where(line => line.Contains(':')).Select(line => line.Split(':', 2))
                        .ToDictionary(parts => parts[0], parts => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);
                    var received = new Request(first[0], first[1], Encoding.UTF8.GetString(body), contentType, headers);
                    Requests.Enqueue(received);
                    var responseJson = first[1].StartsWith("/login", StringComparison.Ordinal)
                        ? "{\"data\":{\"token\":\"session-" + Requests.Count + "\"}}"
                        : first[1].Contains("/invalid")
                            ? "not JSON"
                            : ResponseFactory is { } factory
                                ? await factory(received, _stop.Token)
                                : Json;
                    var bytes = Encoding.UTF8.GetBytes(responseJson);
                    var response = Encoding.ASCII.GetBytes("HTTP/1.1 " +
                                                           (first[1].StartsWith("/denied", StringComparison.Ordinal)
                                                               ? "401 Unauthorized"
                                                               : "200 OK") +
                                                           "\r\nContent-Type: application/json; charset=utf-8\r\nContent-Length: " +
                                                           bytes.Length + "\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(response, _stop.Token);
                    await stream.WriteAsync(bytes, _stop.Token);
                }
                catch (IOException)
                {
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}