using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportDataTreePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyJson(check);
        using var endpoint = new Endpoint();
        var document = new ReportDocument();
        var title = new ReportElement { Kind = ReportElementKind.Text, Name = "根标题", Text = "原标题", X = 14, Y = 15, Width = 100, Height = 12 };
        var indexed = new ReportElement { Kind = ReportElementKind.Text, Name = "索引单值", X = 14, Y = 30, Width = 100, Height = 12 };
        var image = new ReportElement { Kind = ReportElementKind.Image, Name = "绑定图片", X = 145, Y = 15, Width = 20, Height = 20 };
        var table = new ReportElement { Kind = ReportElementKind.Table, Name = "明细", X = 14, Y = 60, Width = 182, Height = 24, Fill = "#006061" };
        foreach (var element in new[] { title, indexed, image, table }) document.Elements.Add(element);
        document.Columns.Add(new() { Header = "旧字段", FieldKey = "legacy" });
        document.Columns.Add(new() { Header = "金额（含税）", Format = ReportValueFormat.Currency, WidthWeight = 2, DecimalPlaces = 3, UseThousandsSeparator = false });
        document.Columns.Add(new() { Header = "固定列", Text = "固定内容" });
        var typed = ReportDataSource.Create("Typed rows", new[] { "旧A", "旧B", "旧C" }, new ReportField<string>("legacy", "旧字段", value => value));
        var view = new ReportDesign { Document = document, DataSource = typed };
        var window = Open(view);
        try
        {
            Click(window, Part<TabItem>(view, "DataTabButton"));
            Await(view.FetchApiDataAsync(endpoint.Url + "/json"));
            var tree = Part<TreeView>(view, "DataTree");
            var nodes = tree.Items.OfType<ReportDataNode>().SelectMany(All).ToArray();
            ReportDataNode Node(string pointer) => nodes.Single(node => node.Pointer == pointer);
            var rows = Node("/data/rows");
            check(tree.Items.Count == 1 && nodes.Any(node => node.Kind == ReportDataNodeKind.Object)
                && rows.Kind == ReportDataNodeKind.Collection && Node("/meta/title").Kind == ReportDataNodeKind.Scalar,
                "ReportDesign/data-tree: a successful GET immediately exposes actual object, collection and scalar nodes");
            check(ReferenceEquals(view.DataSource, typed), "ReportDesign/data-tree: fetching the response tree keeps the currently bound typed source until a binding is committed");
            Part<TextBox>(view, "FieldSearch").Text = "amount"; Flush();
            check(tree.Items.OfType<ReportDataNode>().SelectMany(All).Any(node => node.Name == "amount")
                && tree.Items.OfType<ReportDataNode>().Single().Name == "$",
                "ReportDesign/data-tree: search retains the matching scalar and its JSON ancestors");
            Part<TextBox>(view, "FieldSearch").Text = ""; Flush();
            Await(view.DropDataNodeAsync(Node("/meta/title"), 18, 18));
            check(title.Text == "订单清单" && title.FieldKey == "$root/meta/title" && view.DataSource?.RowCount == 3
                && view.Layout?.Elements.First(element => element.Name == title.Name).Text == "订单清单",
                "ReportDesign/data-tree: a root scalar fills and binds a text element while preserving existing typed rows");
            var before = document.Save(); var beforeSource = view.DataSource;
            Await(view.DropDataNodeAsync(rows, 18, 18));
            check(document.Save() == before && ReferenceEquals(beforeSource, view.DataSource) && Dialog(window) is null,
                "ReportDesign/data-tree: dropping a collection on a text element does nothing");
            var cancelled = view.DropDataNodeAsync(rows, 18, 74); Flush();
            var dialog = Dialog(window) ?? throw new InvalidOperationException("Dropping the collection did not open its field mapping dialog.");
            check(document.Save() == before && ReferenceEquals(view.DataSource, beforeSource),
                "ReportDesign/data-tree: opening the collection mapping dialog does not mutate columns, body or source");
            Click(dialog, Named<Button>(dialog, "CancelTableFields")); Await(cancelled);
            check(document.Save() == before && ReferenceEquals(view.DataSource, beforeSource),
                "ReportDesign/data-tree: cancelling collection configuration preserves the document and original source");
            var configured = view.DropDataNodeAsync(rows, 18, 74); Flush();
            dialog = Dialog(window) ?? throw new InvalidOperationException("The collection field mapping dialog did not reopen.");
            Named<ComboBox>(dialog, "TableFieldPicker0").SelectedIndex = 1;
            Named<ComboBox>(dialog, "TableFieldPicker1").SelectedIndex = 2;
            Flush(); Capture(dialog, output, "reportdata-field-mapping");
            Click(dialog, Named<Button>(dialog, "ApplyTableFields")); Await(configured);
            check(document.DataBodyPointer == "/data/rows" && view.DataSource?.RowCount == 2
                && document.Columns[0].FieldKey == "/product/name" && document.Columns[1].FieldKey == "/amount"
                && document.Columns[2].FieldKey == "" && document.Columns[2].Text == "固定内容",
                "ReportDesign/data-tree: confirming maps each table column to the complete selected collection and preserves unbound column content");
            check(view.ValidationError is null && view.Layout!.Pages.SelectMany(page => page.Rows).Select(row => row.Cells[0]).SequenceEqual(new[] { "茶", "咖啡" })
                && view.Layout.Elements.First(element => element.Name == title.Name).Text == "订单清单",
                "ReportDesign/data-tree: table rows and root-bound text resolve together after collection binding");
            check(document.Columns[0].Header == "旧字段" && document.Columns[1].Header == "金额（含税）"
                && document.Columns[1].Format == ReportValueFormat.Currency && document.Columns[1].WidthWeight == 2
                && document.Columns[1].DecimalPlaces == 3 && !document.Columns[1].UseThousandsSeparator,
                "ReportDesign/data-tree: mapping preserves custom column titles, compatible currency formatting and designed column widths");
            Click(window, Part<Button>(view, "UndoButton"));
            check(document.DataBodyPointer == "" && document.Columns[0].FieldKey == "legacy" && ReferenceEquals(view.DataSource, beforeSource)
                && document.Elements.Single(element => element.Name == title.Name).FieldKey == "$root/meta/title",
                "ReportDesign/data-tree: one Undo restores columns, the previous data body and its original typed source without losing the earlier root binding");
            Click(window, Part<Button>(view, "RedoButton"));
            check(document.DataBodyPointer == "/data/rows" && view.DataSource?.RowCount == 2 && document.Columns[0].FieldKey == "/product/name",
                "ReportDesign/data-tree: Redo restores the collection and column bindings together");
            title = document.Elements.Single(element => element.Name == "根标题");
            image = document.Elements.Single(element => element.Name == "绑定图片");
            indexed = document.Elements.Single(element => element.Name == "索引单值");
            Await(view.DropDataNodeAsync(Node("/data/rows/1/product/name"), 18, 33));
            check(indexed.Text == "咖啡" && indexed.FieldKey == "$root/data/rows/1/product/name" && view.DataSource?.RowCount == 2,
                "ReportDesign/data-tree: dragging a real array-index scalar binds that absolute value without replacing table rows");
            Await(view.DropDataNodeAsync(Node("/data/rows/1/amount"), 90, 74));
            check(document.Columns[1].Format == ReportValueFormat.Currency && document.Columns[1].Header == "金额（含税）"
                && document.Columns[1].WidthWeight == 2 && view.Layout!.Pages.SelectMany(page => page.Rows).All(row => row.Cells[1] == "¥ 24.000"),
                "ReportDesign/data-tree: binding a numeric scalar preserves compatible currency formatting and custom table styling");
            Await(view.DropDataNodeAsync(Node("/meta/title"), 160, 74));
            check(document.Columns[2].FieldKey == "$root/meta/title" && document.Columns[2].Text == "订单清单"
                && view.Layout!.Pages.SelectMany(page => page.Rows).All(row => row.Cells[2] == "订单清单"),
                "ReportDesign/data-tree: a scalar dropped into a table cell fills and binds the selected column");
            Await(view.DropDataNodeAsync(Node("/meta/logo"), 150, 20));
            check(image.FieldKey == "$root/meta/logo" && image.ImageUrl == endpoint.Url + "/image" && endpoint.ImageRequests == 1,
                "ReportDesign/data-tree: an HTTP image scalar binds the URL and loads the server image");
            using (var decoded = new Bitmap(new MemoryStream(ReportDesignFeaturesPreview.CreateImagePng(), false)))
            using (var frame = window.CaptureRenderedFrame()!)
            {
                var surface = Part<Control>(view, "PageSurface");
                var logoPoint = surface.TranslatePoint(new Point(88 + (image.X + 3) * ReportPaginator.DipsPerMillimetre * view.Zoom,
                    30 + (image.Y + 3) * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
                check(decoded.PixelSize == new PixelSize(96, 96) && HasTealPixel(frame, logoPoint),
                    "ReportDesign/data-tree: the downloaded PNG fully decodes and its actual teal pixels are drawn in the image element");
            }
            before = document.Save();
            Await(view.DropDataNodeAsync(Node("/meta/title"), 150, 20));
            Await(view.DropDataNodeAsync(Node("/meta"), 18, 18));
            check(document.Save() == before, "ReportDesign/data-tree: non-URL image values and object nodes leave the report unchanged");
            Capture(window, output, "reportdata-bound-designer");
            foreach (var branch in tree.Items.OfType<ReportDataNode>().SelectMany(All).Where(node => node.Pointer is "" or "/meta" or "/data")) branch.IsExpanded = true;
            Flush(); Capture(window, output, "reportdata-tree-expanded");
            var json = document.Save(); endpoint.Refreshed = true;
            var preview = new ReportPreview(); var previewWindow = Open(preview);
            try
            {
                Await(preview.LoadAsync(json));
                check(preview.Error is null && preview.Layout!.Elements.Single(element => element.Name == title.Name).Text == "实时订单"
                    && preview.Layout.Elements.Single(element => element.Name == indexed.Name).Text == "实时咖啡"
                    && preview.Layout.Pages.SelectMany(page => page.Rows).All(row => row.Cells[2] == "实时订单"),
                    "ReportDesign/data-tree: independent JSON preview refetches the selected collection and absolute root/index scalar bindings");
                check(preview.Document!.Elements.Single(element => element.Name == image.Name).ImageUrl == endpoint.Url + "/image-v2" && endpoint.ImageRequests == 2,
                    "ReportDesign/data-tree: independent preview resolves the current API image URL instead of the URL saved with the template");
                Capture(previewWindow, output, "reportdata-independent-preview");
            }
            finally { previewWindow.Close(); Flush(); }
            var priorSource = view.DataSource; var priorDefinition = document.Save();
            Await(view.FetchApiDataAsync(endpoint.Url + "/json"));
            var refreshedTitle = tree.Items.OfType<ReportDataNode>().SelectMany(All).Single(node => node.Pointer == "/meta/title");
            Await(view.DropDataNodeAsync(refreshedTitle, 18, 18));
            var refreshedSource = view.DataSource;
            check(!ReferenceEquals(priorSource, refreshedSource) && document.DataBodyPointer == "/data/rows"
                && Equals(refreshedSource!.GetValue(0, "$root/meta/title"), "实时订单"),
                "ReportDesign/data-tree: a same-URL and same-body refresh creates a distinct data snapshot");
            Click(window, Part<Button>(view, "UndoButton"));
            check(ReferenceEquals(view.DataSource, priorSource) && document.Save() == priorDefinition,
                "ReportDesign/data-tree: Undo restores the exact earlier source snapshot even when URL and body are identical");
            Click(window, Part<Button>(view, "RedoButton"));
            check(ReferenceEquals(view.DataSource, refreshedSource)
                && document.Elements.Single(element => element.Name == "根标题").Text == "实时订单",
                "ReportDesign/data-tree: Redo restores the newer source snapshot and its scalar value");
            var freshDocument = new ReportDocument();
            var freshText = new ReportElement { Name = "新报表文本", Kind = ReportElementKind.Text, Text = "新文档原文", X = 14, Y = 14, Width = 100, Height = 12 };
            freshDocument.Elements.Add(freshText); view.Document = freshDocument;
            var freshSource = ReportDataSource.Create("新报表数据", new[] { "新数据" }, new ReportField<string>("value", "值", value => value));
            view.DataSource = freshSource; freshText.Text = "新文档编辑"; Flush();
            Click(window, Part<Button>(view, "UndoButton"));
            check(ReferenceEquals(view.Document, freshDocument) && ReferenceEquals(view.DataSource, freshSource)
                && freshDocument.Elements.Single().Text == "新文档原文",
                "ReportDesign/data-tree: switching documents isolates source history so a static edit never restores an older report's source");
            var corrupt = new ReportDocument(); corrupt.Elements.Add(new() { Kind = ReportElementKind.Image, X = 14, Y = 14, Width = 20, Height = 20, ImageUrl = endpoint.Url + "/corrupt-image" });
            var rejected = false;
            try { Await(ReportResources.LoadImagesAsync(corrupt)); } catch (ArgumentException) { rejected = true; }
            check(rejected, "ReportDesign/data-tree: a response with a valid PNG header but corrupt pixel data is rejected before the image is cached as loaded");
        }
        finally { window.Close(); Flush(); }
    }

    private static void VerifyJson(Action<bool, string> check)
    {
        const string json = "{\"title\":\"标题\",\"a/b\":{\"~name\":7},\"rows\":[{\"value\":1},{\"value\":2}],\"empty\":[]}";
        Func<string, string, ReportDataSource> legacyCreate = ReportJsonData.CreateSource;
        Func<ReportDocument, HttpClient?, CancellationToken, Task> legacyImages = ReportResources.LoadImagesAsync;
        Await(legacyImages(new ReportDocument(), null, CancellationToken.None));
        check(legacyCreate(json, "/rows").RowCount == 2,
            "ReportDesign/data-tree: the original two-argument JSON source and three-argument image-loading API signatures remain callable");
        var source = ReportJsonData.CreateSource(json, "/rows", ["$root/rows/1/value"]);
        check(source.RowCount == 2 && Equals(source.GetValue(1, "/value"), 2m)
            && Equals(source.GetValue(0, "$root/title"), "标题") && Equals(source.GetValue(0, "$root/a~1b/~0name"), 7m)
            && Equals(source.GetValue(1, "$root/rows/1/value"), 2m),
            "ReportDesign/data-tree: collection row fields coexist with escaped JSON Pointer root/index values");
        var empty = ReportJsonData.CreateSource(json, "/empty");
        var document = new ReportDocument(); document.Elements.Add(new() { Name = "标题", Kind = ReportElementKind.Text, X = 14, Y = 15, Width = 100, Height = 12, FieldKey = "$root/title" });
        check(empty.RowCount == 0 && Equals(empty.GetValue(0, "$root/title"), "标题")
            && ReportPaginator.Paginate(document, empty).Elements.Single().Text == "标题",
            "ReportDesign/data-tree: an empty collection keeps zero real rows while a root scalar still resolves in the page header");
        var primitive = ReportJsonData.CreateSource("[10,20]", "");
        check(primitive.RowCount == 2 && Equals(primitive.GetValue(1, "$"), 20m)
            && ReportJsonData.CreateSource("{\"name\":\"单对象\"}", "").GetValue(0, "/name") as string == "单对象",
            "ReportDesign/data-tree: root primitive collections and a root single object remain valid report bodies");
        var large = "{\"rows\":[" + string.Join(",", Enumerable.Range(0, 1200).Select(index => "{\"id\":" + index + "}")) + "]}";
        var tree = ReportJsonData.CreateTree(large).SelectMany(All).ToArray();
        check(tree.Length <= 4002 && tree.Single(node => node.Pointer == "/rows").Children.Count == 201
            && ReportJsonData.CreateSource(large, "/rows").RowCount == 1200,
            "ReportDesign/data-tree: display nodes are bounded while dragging the collection preserves all 1,200 source rows");
    }

    private static IEnumerable<ReportDataNode> All(ReportDataNode node) => new[] { node }.Concat(node.Children.SelectMany(All));
    private static Window Open(Control control)
    {
        var window = new Window { Content = control, Width = 1586, Height = 992, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = ThemeVariant.Light };
        ControlSizing.SetSize(window, ControlSize.Normal); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static Window? Dialog(Window owner) => ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.SingleOrDefault(window => window != owner && window.IsVisible && window.Title == "配置表格字段");
    private static T Part<T>(ReportDesign view, string name) where T : Control => view.FindControl<T>(name)!;
    private static T Named<T>(Control root, string name) where T : Control => root.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Await(Task task)
    {
        var watch = Stopwatch.StartNew();
        while (!task.IsCompleted && watch.Elapsed < TimeSpan.FromSeconds(15)) { Flush(); Thread.Sleep(2); }
        if (!task.IsCompleted) throw new TimeoutException("Report data-tree verification timed out.");
        task.GetAwaiter().GetResult(); Flush();
    }
    private static void Capture(Window window, string? output, string name)
    {
        if (output is null) return; Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!; frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
    private static bool HasTealPixel(Bitmap frame, Point point)
    {
        var x = (int)(point.X * frame.Dpi.X / 96); var y = (int)(point.Y * frame.Dpi.Y / 96);
        if (x < 0 || y < 0 || x >= frame.PixelSize.Width || y >= frame.PixelSize.Height) return false;
        var memory = System.Runtime.InteropServices.Marshal.AllocHGlobal(4);
        try
        {
            frame.CopyPixels(new PixelRect(x, y, 1, 1), memory, 4, 4);
            var rgba = frame.Format == PixelFormat.Rgba8888;
            var red = System.Runtime.InteropServices.Marshal.ReadByte(memory, rgba ? 0 : 2);
            var green = System.Runtime.InteropServices.Marshal.ReadByte(memory, 1);
            var blue = System.Runtime.InteropServices.Marshal.ReadByte(memory, rgba ? 2 : 0);
            return red < 15 && Math.Abs(green - 96) < 15 && Math.Abs(blue - 97) < 15;
        }
        finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(memory); }
    }
    internal sealed class Endpoint : IDisposable
    {
        private readonly TcpListener _server = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stop = new();
        private int _imageRequests;
        internal int ImageRequests => Volatile.Read(ref _imageRequests);
        internal bool Refreshed { get; set; }
        internal string Url { get; }
        internal Endpoint()
        {
            _server.Start(); Url = "http://127.0.0.1:" + ((IPEndPoint)_server.LocalEndpoint).Port;
            _ = Task.Run(async () =>
            {
                try { while (!_stop.IsCancellationRequested) { var client = await _server.AcceptTcpClientAsync(_stop.Token); _ = Respond(client); } }
                catch (OperationCanceledException) { } catch (SocketException) when (_stop.IsCancellationRequested) { }
            });
        }
        private async Task Respond(TcpClient client)
        {
            using (client)
            {
                try
                {
                    using var stream = client.GetStream(); using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                    var request = await reader.ReadLineAsync(_stop.Token); string? line;
                    do { line = await reader.ReadLineAsync(_stop.Token); } while (!string.IsNullOrEmpty(line));
                    var corrupt = request?.StartsWith("GET /corrupt-image", StringComparison.Ordinal) == true;
                    var image = corrupt || request?.StartsWith("GET /image", StringComparison.Ordinal) == true;
                    byte[] body;
                    if (image)
                    {
                        Interlocked.Increment(ref _imageRequests);
                        body = corrupt ? Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aS1sAAAAASUVORK5CYII=") : ReportDesignFeaturesPreview.CreateImagePng();
                    }
                    else body = Encoding.UTF8.GetBytes("{\"meta\":{\"title\":\"" + (Refreshed ? "实时订单" : "订单清单") + "\",\"logo\":\"" + Url + (Refreshed ? "/image-v2" : "/image")
                        + "\"},\"data\":{\"rows\":[{\"product\":{\"name\":\"茶\"},\"amount\":12.5},{\"product\":{\"name\":\"" + (Refreshed ? "实时咖啡" : "咖啡") + "\"},\"amount\":24}]},\"empty\":[]}");
                    var header = Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: " + (image ? "image/png" : "application/json; charset=utf-8") + "\r\nContent-Length: " + body.Length + "\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header, _stop.Token); await stream.WriteAsync(body, _stop.Token);
                }
                catch (OperationCanceledException) { } catch (IOException) { }
            }
        }
        public void Dispose() { _stop.Cancel(); _server.Stop(); _stop.Dispose(); }
    }
}
