using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using SkiaSharp;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportDesignFeaturesPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyDefinition(check);
        VerifyDesigner(check, output);
        VerifyDocumentTabs(check, output);
        ReportStructurePreview.Verify(check, output);
        ReportFontsPreview.Verify(check);
        ReportNavigationPreview.Verify(check, output);
        ReportCanvasInteractionPreview.Verify(check, output);
        ReportDialogsPreview.Verify(check, output);
        ReportDataTreePreview.Verify(check, output);
        ReportApiComponentsPreview.Verify(check, output);
        VerifyPreview(check, output);
        VerifyPrintArguments(check);
    }

    private static ReportDocument Definition()
    {
        var document = new ReportDocument { Title = "远程销售报表", HeaderHeightMm = 28, FooterHeightMm = 16 };
        document.Columns.Add(new ReportColumn { Header = "项目", FieldKey = "/item", WidthWeight = 2 });
        document.Columns.Add(new ReportColumn { Header = "金额", FieldKey = "/amount", WidthWeight = 1 });
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.Text, Name = "页眉标题", X = 14, Y = 16, Width = 90, Height = 12, Text = "远程销售报表" });
        document.Elements.Add(new ReportElement
        {
            Kind = ReportElementKind.Table, Name = "表格1", X = 14, Y = 49, Width = 182, Height = 27, FontSize = 16,
            TableHeaderHeightMm = 11
        });
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.PageNumber, Name = "页脚页码", X = 14, Y = 271, Width = 100, Height = 10 });
        return document;
    }

    private static void VerifyDefinition(Action<bool, string> check)
    {
        var document = Definition();
        document.MarginLeftMm = 17;
        document.MarginTopMm = 13;
        document.MarginRightMm = 15;
        document.MarginBottomMm = 12;
        document.DataSourceUrl = "https://example.invalid/report";
        document.DataBodyPointer = "/data";
        document.Elements.Add(new ReportElement
        {
            Kind = ReportElementKind.Image, Name = "远程图片", X = 120, Y = 16, Width = 20, Height = 12,
            ImageUrl = "https://example.invalid/logo.png"
        });
        var restored = ReportDocument.Load(document.Save());
        check(restored.LeftMargin == 17 && restored.TopMargin == 13 && restored.RightMargin == 15 &&
              restored.BottomMargin == 12
              && restored.HeaderHeightMm == 28 && restored.FooterHeightMm == 16 && restored.HeaderBottom == 41 &&
              restored.FooterTop == 269,
            "ReportDesign/features: Document.Save/Load preserves separate margins and fixed header/footer geometry");
        check(restored.DataSourceUrl == document.DataSourceUrl && restored.DataBodyPointer == "/data"
                                                               && restored.Elements.Last().ImageUrl ==
                                                               document.Elements.Last().ImageUrl &&
                                                               restored.Elements[1].TableHeaderHeightMm == 11,
            "ReportDesign/features: portable JSON preserves API/body/image URLs and editable table header height");
        var withoutElements = new ReportDocument { HeaderHeightMm = 23, FooterHeightMm = 19 };
        var layout = ReportPaginator.Paginate(withoutElements);
        check(withoutElements.HeaderBottom == 37 && withoutElements.FooterTop == 264
                                                 && Math.Abs(layout.DetailBottom - withoutElements.FooterTop *
                                                     ReportPaginator.DipsPerMillimetre) < .01,
            "ReportDesign/features: header and footer remain known page regions without any elements");
    }

    private static void VerifyDocumentTabs(Action<bool, string> check, string? output)
    {
        var document = Definition();
        var view = new ReportDesign
        {
            Document = document,
            DataSource =
                ReportDataSource.Create("Rows", new[] { 1 }, new ReportField<int>("value", "值", value => value))
        };
        var window = Open(view);
        try
        {
            var tabs = Part<TabStrip>(view, "ReportNameTabs");
            var header = (TabStripItem)tabs.ContainerFromIndex(0)!;
            var close = header.GetVisualDescendants().OfType<Button>()
                .Single(button => button.Name == "PART_CloseButton");
            check(tabs.GetType() == typeof(TabStrip) && header.GetType() == typeof(TabStripItem)
                                                     && ReferenceEquals(tabs.SelectedItem, document) &&
                                                     close.IsEffectivelyVisible
                                                     && new[] { "SourceTabs", "ViewTabs", "PropertyTabs" }.All(name =>
                                                         Part<TabControl>(view, name).GetType() == typeof(TabControl)),
                "ReportDesign/tabs: the report name uses native TabStrip with closing while all three designer tab groups remain native TabControl");
            document.Title = "订单报表";
            Flush();
            check(header.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == document.Title),
                "ReportDesign/tabs: renaming the document updates its bound native header");
            EventHandler<TabClosingEventArgs> cancel = (_, args) => args.Cancel = true;
            view.AddHandler(TabFeatures.TabClosingEvent, cancel);
            Click(window, close);
            check(ReferenceEquals(view.Document, document) && view.DataSource is not null && tabs.Items.Count == 1,
                "ReportDesign/tabs: the consumer can cancel report closing through the routed TabClosing event");
            view.RemoveHandler(TabFeatures.TabClosingEvent, cancel);
            Capture(window, output, "reportdesign-native-tabs");
            Click(window, close);
            check(view.Document is null && view.DataSource is null && tabs.Items.Count == 0 && !tabs.IsVisible
                  && !Part<Button>(view, "SaveButton").IsEffectivelyEnabled &&
                  !Part<Button>(view, "PreviewButton").IsEffectivelyEnabled
                  && !Part<Button>(view, "ExportPdfButton").IsEffectivelyEnabled,
                "ReportDesign/tabs: closing the report clears document and data, hides its header and disables document actions");
            Click(window, Part<Button>(view, "NewReportButton"));
            check(view.Document is not null && !ReferenceEquals(view.Document, document) && tabs.Items.Count == 1
                  && ReferenceEquals(tabs.SelectedItem, view.Document) && tabs.IsEffectivelyVisible
                  && Part<Button>(view, "SaveButton").IsEffectivelyEnabled &&
                  Part<Button>(view, "PreviewButton").IsEffectivelyEnabled,
                "ReportDesign/tabs: New restores a fresh document, selected report header and document actions after closing");
            header = (TabStripItem)tabs.ContainerFromIndex(0)!;
            check(TabFeatures.Close(header) && view.Document is null,
                "ReportDesign/tabs: the restored report still uses the MVVM close command");
            view.LoadTemplate(document.Save());
            Flush();
            check(
                view.Document?.Title == document.Title && tabs.Items.Count == 1 &&
                ReferenceEquals(tabs.SelectedItem, view.Document),
                "ReportDesign/tabs: loading saved JSON after closing restores the native report tab");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyDesigner(Action<bool, string> check, string? output)
    {
        var view = new ReportDesign { Document = Definition(), Zoom = .8 };
        var window = Open(view);
        var document = view.Document!;
        var command = new SaveProbe();
        view.SaveCommand = command;
        Flush();
        var save = Part<Button>(view, "SaveButton");
        Click(window, save);
        check(command.Executions == 1 && ReferenceEquals(command.Parameter, document),
            "ReportDesign/features: native Save button executes the consumer MVVM command with the current document");
        command.Enabled = false;
        command.Refresh();
        Flush();
        Click(window, save);
        check(!save.IsEffectivelyEnabled && command.Executions == 1,
            "ReportDesign/features: Save button follows consumer CanExecute and cannot invoke a disabled save");

        var sources = Part<TabControl>(view, "SourceTabs");
        var views = Part<TabControl>(view, "ViewTabs");
        var properties = Part<TabControl>(view, "PropertyTabs");
        var scroll = Part<ScrollViewer>(view, "PageScroll");
        var sourceTop = sources.TranslatePoint(default, window)!.Value.Y;
        var viewsTop = views.TranslatePoint(default, window)!.Value.Y;
        var scrollTop = scroll.TranslatePoint(default, window)!.Value.Y;
        check(sourceTop <= viewsTop + 1 && scrollTop > viewsTop + 20 &&
              views.TranslatePoint(default, window)!.Value.X >= sources.Bounds.Width
              && properties.Items.Count <= 2 && window.GetVisualDescendants().OfType<Menu>().Count() == 0,
            "ReportDesign/features: source dock starts at the work area top, native view tabs occupy the right top, and the extra menu strip is removed");

        var table = document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        view.SelectedElement = table;
        Flush();
        var surface = Part<Control>(view, "PageSurface");

        Point Paper(double x, double y)
        {
            return surface.TranslatePoint(new Point(88 + x * ReportPaginator.DipsPerMillimetre * view.Zoom,
                30 + y * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
        }

        var initialX = table.X;
        var initialY = table.Y;
        var initialWidth = table.Width;
        var firstWidth = table.Width * document.Columns[0].WidthWeight / document.Columns.Sum(c => c.WidthWeight);
        Drag(window, Paper(table.X + firstWidth, table.Y + 5), Paper(table.X + firstWidth + 8, table.Y + 5));
        var newWidth = table.Width * document.Columns[0].WidthWeight / document.Columns.Sum(c => c.WidthWeight);
        check(
            Math.Abs(newWidth - firstWidth - 8) < 1 && table.X == initialX && table.Y == initialY &&
            table.Width == initialWidth,
            "ReportDesign/features: dragging an internal cell boundary resizes adjacent columns while retaining table position and total width");
        Click(window, Part<Button>(view, "UndoButton"));
        table = document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        check(
            Math.Abs(table.Width * document.Columns[0].WidthWeight / document.Columns.Sum(c => c.WidthWeight) -
                     firstWidth) < .01,
            "ReportDesign/features: column boundary drag is one undoable gesture");

        view.SelectedElement = table;
        Flush();
        var header = view.Layout!.HeaderHeight / ReportPaginator.DipsPerMillimetre;
        Drag(window, Paper(table.X + 20, table.Y + header), Paper(table.X + 20, table.Y + header + 4));
        check(Math.Abs(table.TableHeaderHeightMm - header - 4) < 1 && table.Y == initialY,
            "ReportDesign/features: dragging the table header bottom changes its height in place");
        Click(window, Part<Button>(view, "UndoButton"));
        table = document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        view.SelectedElement = table;
        Flush();
        var height = table.Height;
        Drag(window, Paper(table.X + 20, table.Y + height), Paper(table.X + 20, table.Y + height + 5));
        check(Math.Abs(table.Height - height - 5) < 1 && table.Y == initialY,
            "ReportDesign/features: dragging the detail row bottom changes row height without moving the table");
        Click(window, Part<Button>(view, "UndoButton"));
        table = document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        check(Math.Abs(table.Height - height) < .01 && Math.Abs(table.TableHeaderHeightMm - 11) < .01,
            "ReportDesign/features: header and detail height gestures restore through undo");

        view.SelectedElement = table;
        Flush();
        Click(window, Part<Button>(view, "FillMenuButton"));
        var picker = Part<ColorPicker>(view, "ToolbarFillEditor");
        picker.Color = Color.Parse("#A6D5E1");
        Flush();
        check(
            table.Fill.Equals("#FFA6D5E1", StringComparison.OrdinalIgnoreCase) ||
            Color.Parse(table.Fill) == picker.Color,
            "ReportDesign/features: the toolbar uses the built-in ColorPicker and updates the selected element");
        Part<Button>(view, "FillMenuButton").Flyout!.Hide();
        Flush();

        var settingsButton = Part<Button>(view, "PageSettingsButton");
        Click(window, settingsButton);
        var dialog = Dialog(window);
        var oldWidth = document.PageWidthMm;
        DialogPart<NumericUpDown>(dialog, "MarginLeft").Value = 22;
        Click(dialog, dialog.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Content, "取消")));
        Flush();
        check(document.LeftMargin == 14 && document.PageWidthMm == oldWidth && !dialog.IsVisible,
            "ReportDesign/features: cancelling page settings leaves the report definition unchanged");
        Click(window, settingsButton);
        dialog = Dialog(window);
        DialogPart<ComboBox>(dialog, "PaperPicker").SelectedIndex = 0;
        DialogPart<ComboBox>(dialog, "PageOrientationPicker").SelectedIndex = 1;
        DialogPart<NumericUpDown>(dialog, "MarginLeft").Value = 18;
        DialogPart<NumericUpDown>(dialog, "HeaderHeight").Value = 25;
        Click(dialog, DialogPart<Button>(dialog, "ApplyPageSettings"));
        Flush();
        check(
            document.PageWidthMm == 297 && document.PageHeightMm == 210 && document.LeftMargin == 18 &&
            document.HeaderHeightMm == 25 && !dialog.IsVisible,
            "ReportDesign/features: page settings applies A4 landscape, individual margins and fixed region heights");
        Capture(window, output, "reportdesign-features");
        window.Close();
    }

    private static void VerifyPreview(Action<bool, string> check, string? output)
    {
        using var endpoint = new ResourceEndpoint();
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var document = Definition();
        document.DataSourceUrl = endpoint.Url + "/data";
        document.DataBodyPointer = "/data";
        document.Elements.Add(new ReportElement
        {
            Kind = ReportElementKind.Image, Name = "网络图片", X = 150, Y = 16, Width = 20, Height = 12,
            ImageUrl = endpoint.Url + "/image"
        });
        document.Elements.Add(new ReportElement
        {
            Kind = ReportElementKind.Image, Name = "复用图片", X = 174, Y = 16, Width = 20, Height = 12,
            ImageUrl = endpoint.Url + "/image"
        });
        var preview = new ReportPreview { HttpClient = client };
        var window = Open(preview);
        Await(preview.LoadAsync(document.Save()));
        Flush();
        check(preview.Error is null && preview.Document?.Title == document.Title &&
              preview.Layout is { Pages.Count: > 1 }
              && preview.Layout.Pages.SelectMany(p => p.Rows).Count() == 80 &&
              preview.Layout.Pages[0].Rows[0].Cells[0] == "项目 1",
            "ReportPreview/features: JSON loads its API body, resolves field bindings and expands 80 rows across pages");
        check(
            endpoint.ImageRequests == 1 && preview.Document!.Elements.Where(e => e.Kind == ReportElementKind.Image)
                .All(e => e.ImageData.Length == 0),
            "ReportPreview/features: duplicate image URLs download once and fetched bytes remain outside portable image definitions");
        var next = Named<Button>(preview, "PreviewNext");
        Click(window, next);
        check(preview.CurrentPageNumber == 2,
            "ReportPreview/features: native next-page button navigates the independent preview control");
        using var pdf = new MemoryStream();
        Await(preview.ExportPdfAsync(pdf));
        check(pdf.Length > 1000 && Encoding.ASCII.GetString(pdf.ToArray(), 0, 4) == "%PDF",
            "ReportPreview/features: remote images and API-backed pages export through the shared PDF renderer");
        check(endpoint.ImageRequests == 1,
            "ReportPreview/features: PDF export uses the loaded image snapshot without another HTTP request");
        Capture(window, output, "reportpreview-remote");

        var unavailable = ReportDocument.Load(document.Save());
        unavailable.DataSourceUrl = endpoint.Url + "/unavailable";
        var requestFailed = false;
        try
        {
            Await(preview.LoadAsync(unavailable.Save()));
        }
        catch (HttpRequestException)
        {
            requestFailed = true;
        }

        check(
            requestFailed && preview.Document is null && preview.Layout is null && preview.Error is { Length: > 0 } &&
            !Named<Button>(preview, "PrintButton").IsEnabled,
            "ReportPreview/features: HTTP failure removes the previous report snapshot and disables stale printing");

        var failed = false;
        try
        {
            Await(preview.LoadAsync("{invalid"));
        }
        catch (JsonException)
        {
            failed = true;
        }
        catch (ArgumentException)
        {
            failed = true;
        }

        check(
            failed && preview.Layout is null && preview.Error is { Length: > 0 } &&
            !Named<Button>(preview, "PrintButton").IsEnabled,
            "ReportPreview/features: invalid JSON clears rendered pages and disables printing with an explicit error");
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        var wasCancelled = false;
        try
        {
            Await(preview.LoadAsync(document.Save(), cancelled.Token));
        }
        catch (OperationCanceledException)
        {
            wasCancelled = true;
        }

        check(wasCancelled && preview.Layout is null && !Named<Button>(preview, "PrintButton").IsEnabled,
            "ReportPreview/features: caller cancellation leaves no printable stale pages");
        Await(preview.LoadAsync(document.Save()));
        Flush();
        check(preview.Layout is { Pages.Count: > 1 } && preview.Error is null && preview.CurrentPageNumber == 1,
            "ReportPreview/features: successful reload recovers from errors and starts on the first page");
        preview.DocumentJson = document.Save();
        preview.DocumentJson = null;
        Flush();
        check(
            preview.Document is null && preview.Layout is null && preview.Error is null &&
            !Named<Button>(preview, "PrintButton").IsEnabled,
            "ReportPreview/features: clearing bound JSON cancels pending loading and clears the document, pages and print state");
        window.Close();
    }

    private static void VerifyPrintArguments(Action<bool, string> check)
    {
        var path = Path.Combine(Path.GetTempPath(), "report with spaces.pdf");
        var windows = PrintTool.CreateStartInfo(path, ReportPrintPlatform.Windows);
        check(windows.UseShellExecute && windows.Verb == "print" && windows.FileName == Path.GetFullPath(path),
            "PrintTool/features: Windows uses the registered PDF print verb without executing a shell command");
        foreach (var platform in new[] { ReportPrintPlatform.MacOS, ReportPrintPlatform.Linux })
        {
            var start = PrintTool.CreateStartInfo(path, platform, new ReportPrintOptions("Office printer; literal", 2));
            check(!start.UseShellExecute && start.ArgumentList.SequenceEqual(new[]
                                         {
                                             "-d", "Office printer; literal", "-n", "2", "--", Path.GetFullPath(path)
                                         })
                                         && start.FileName == (platform == ReportPrintPlatform.MacOS
                                             ? "/usr/bin/lp"
                                             : "lp"),
                $"PrintTool/features: {platform} passes printer, copies and spaced PDF paths as literal CUPS arguments");
        }

        var invalid = false;
        try
        {
            PrintTool.CreateStartInfo(path, ReportPrintPlatform.Linux, new ReportPrintOptions(Copies: 0));
        }
        catch (ArgumentOutOfRangeException)
        {
            invalid = true;
        }

        check(invalid, "PrintTool/features: invalid copy counts are rejected before any process starts");
    }

    private static Window Open(Control control)
    {
        var window = new Window
        {
            Content = control, Width = 1586, Height = 992, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = ThemeVariant.Light
        };
        ControlSizing.SetSize(window, ControlSize.Normal);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Await(Task task)
    {
        var watch = Stopwatch.StartNew();
        while (!task.IsCompleted && watch.Elapsed < TimeSpan.FromSeconds(15))
        {
            Flush();
            Thread.Sleep(2);
        }

        if (!task.IsCompleted) throw new TimeoutException("Report feature verification timed out.");
        task.GetAwaiter().GetResult();
        Flush();
    }

    private static T Part<T>(ReportDesign view, string name) where T : Control
    {
        return view.FindControl<T>(name)!;
    }

    private static T Named<T>(Control root, string name) where T : Control
    {
        return root.GetVisualDescendants().OfType<T>().Single(c => c.Name == name);
    }

    private static T DialogPart<T>(Window dialog, string name) where T : Control
    {
        return Named<T>(dialog, name);
    }

    private static Window Dialog(Window owner)
    {
        return ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows.Single(w =>
            w != owner && w.IsVisible && w.Title == "页面设置");
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

    private static void Drag(Window window, Point from, Point to)
    {
        window.MouseMove(from);
        window.MouseDown(from, MouseButton.Left);
        window.MouseMove(to);
        window.MouseUp(to, MouseButton.Left);
        Flush();
    }

    private static void Capture(Window window, string? output, string name)
    {
        if (output is null) return;
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    internal static byte[] CreateImagePng()
    {
        using var bitmap = new SKBitmap(96, 96);
        using var canvas = new SKCanvas(bitmap);
        canvas.Clear(new SKColor(0, 96, 97));
        using var paint = new SKPaint
            { Color = SKColors.White, IsAntialias = true, Style = SKPaintStyle.Stroke, StrokeWidth = 4 };
        canvas.DrawRect(26, 18, 44, 60, paint);
        canvas.DrawLine(35, 34, 61, 34, paint);
        canvas.DrawLine(35, 46, 61, 46, paint);
        canvas.DrawLine(35, 58, 61, 58, paint);
        canvas.DrawLine(46, 42, 46, 70, paint);
        using var image = SKImage.FromBitmap(bitmap);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }

    private sealed class SaveProbe : ICommand
    {
        public bool Enabled { get; set; } = true;
        public int Executions { get; private set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return Enabled;
        }

        public void Execute(object? parameter)
        {
            Parameter = parameter;
            Executions++;
        }

        public void Refresh()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class ResourceEndpoint : IDisposable
    {
        private readonly TcpListener _server = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stop = new();
        private int _imageRequests;

        internal ResourceEndpoint()
        {
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

        public int ImageRequests => Volatile.Read(ref _imageRequests);
        public string Url { get; }

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
                    using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                    var request = await reader.ReadLineAsync(_stop.Token);
                    string? line;
                    do
                    {
                        line = await reader.ReadLineAsync(_stop.Token);
                    } while (!string.IsNullOrEmpty(line));

                    var image = request?.StartsWith("GET /image ", StringComparison.Ordinal) == true;
                    var unavailable = request?.StartsWith("GET /unavailable ", StringComparison.Ordinal) == true;
                    byte[] bytes;
                    if (image)
                    {
                        Interlocked.Increment(ref _imageRequests);
                        bytes = CreateImagePng();
                    }
                    else
                    {
                        bytes = Encoding.UTF8.GetBytes("{\"data\":[" + string.Join(",",
                                                           Enumerable.Range(1, 80).Select(i =>
                                                               "{\"item\":\"项目 " + i + "\",\"amount\":" + i + "}")) +
                                                       "]}");
                    }

                    var header = Encoding.ASCII.GetBytes("HTTP/1.1 " +
                                                         (unavailable ? "503 Service Unavailable" : "200 OK") +
                                                         "\r\nContent-Type: " +
                                                         (image ? "image/png" : "application/json; charset=utf-8") +
                                                         "\r\nContent-Length: " + bytes.Length +
                                                         "\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header, _stop.Token);
                    await stream.WriteAsync(bytes, _stop.Token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (IOException)
                {
                }
            }
        }
    }
}