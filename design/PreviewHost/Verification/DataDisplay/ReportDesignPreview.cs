using Avalonia;
using Avalonia.Controls;
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

internal static class ReportDesignPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        VerifyPagination(check);
        VerifyInteraction(check, output);
        VerifyFiles(check, output);
        VerifyChrome(check);
        ReportDesignFeaturesPreview.Verify(check, output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var density in Enum.GetValues<ControlSize>()) VerifyRendering(check, theme, density, output);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void VerifyChrome(Action<bool, string> check)
    {
        var view = ReportDesignDemo.CreateDesignerSample(); var window = Open(view, width: 1586); window.Height = 992; Flush();
        var table = view.Document!.Elements.Single(e => e.Kind == ReportElementKind.Table); view.SelectedElement = table; Flush();
        var design = Part<TabItem>(view, "DesignTab"); design.Focus(); window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null); window.KeyRelease(Key.Right, RawInputModifiers.None, PhysicalKey.ArrowRight, null); Flush();
        check(view.IsPreview && Part<TabItem>(view, "PreviewTab").IsSelected, "ReportDesign/tabs: native keyboard navigation enters paginated preview");
        Click(window, design);
        check(!view.IsPreview, "ReportDesign/tabs: native design tab restores the editable canvas");
        Click(window, Part<TabItem>(view, "DataTabButton"));
        check(Part<TabControl>(view, "SourceTabs").SelectedIndex == 1 && Part<Button>(view, "AddDataSourceButton").IsEffectivelyVisible, "ReportDesign/tabs: native source tab displays Add Data Source");
        Click(window, Part<TabItem>(view, "DataPropertiesTab"));
        check(Part<TabControl>(view, "PropertyTabs").SelectedIndex == 1 && Part<ComboBox>(view, "FormatPicker").IsEffectivelyVisible, "ReportDesign/tabs: native property tab displays its binding controls");
        var family = Part<ComboBox>(view, "FontFamilyPicker"); var size = Part<ComboBox>(view, "FontEditor");
        check(Math.Abs(family.TranslatePoint(default, window)!.Value.X - 390) < 2 && Math.Abs(size.TranslatePoint(default, window)!.Value.X - 547) < 2 && Math.Abs(Part<Button>(view, "BorderMenuButton").TranslatePoint(default, window)!.Value.X - 1160) < 2, "ReportDesign/toolbar: font and border groups occupy the reference positions at 1586 pixels");
        check(Convert.ToDouble(size.SelectedItem) == 12 && table.FontSize == 16, "ReportDesign/typography: 12 point UI font corresponds to 16 DIPs on the real A4 page");
        size.SelectedItem = 18d; Flush();
        check(table.FontSize == 24 && view.FontSizeValue == 18, "ReportDesign/typography: font dropdown updates the selected element in print points");
        var border = Part<Button>(view, "BorderMenuButton"); var hadBorder = table.ShowBorder; Click(window, border);
        check(border.Flyout!.IsOpen, "ReportDesign/toolbar: border dropdown opens through native pointer input");
        Click(window, Part<CheckBox>(view, "BorderToggle"));
        check(table.ShowBorder != hadBorder, "ReportDesign/toolbar: border dropdown applies its setting to the selected table");
        border.Flyout.Hide(); var fill = Part<Button>(view, "FillMenuButton"); Click(window, fill);
        Part<ColorPicker>(view, "ToolbarFillEditor").Color = Color.Parse("#DDF3F2"); Flush();
        check(fill.Flyout!.IsOpen && Color.Parse(table.Fill) == Color.Parse("#DDF3F2"), "ReportDesign/toolbar: built-in ColorPicker edits the selected element through a compiled binding");
        fill.Flyout.Hide(); Flush();
        var tree = Part<TreeView>(view, "StructureTree");
        check(tree.SelectedItem is not null && tree.GetVisualDescendants().OfType<TreeViewItem>().Any(i => i.IsSelected && ReferenceEquals(i.DataContext, tree.SelectedItem)), "ReportDesign/structure: rebuilding the tree retains the selected nested element");
        var selectedNode = tree.GetVisualDescendants().OfType<TreeViewItem>().Single(i => i.IsSelected);
        var row = selectedNode.GetVisualDescendants().OfType<Border>().First(b => b.Name == "PART_LayoutRoot");
        check(row.Bounds.Height <= 28.1, $"ReportDesign/structure: compact rows match the reference spacing (row {row.Bounds.Height}, item minimum {selectedNode.MinHeight}, row minimum {row.MinHeight}, height {row.Height})");
        window.Close();
    }
    private static Window Open(ReportDesign designer, ThemeVariant? theme = null, ControlSize density = ControlSize.Normal, double width = 1600)
    {
        var window = new Window { Content = designer, Width = width, Height = 1050, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme ?? ThemeVariant.Light };
        ControlSizing.SetSize(window, density); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static T Part<T>(ReportDesign view, string name) where T : Control => view.FindControl<T>(name)!;
    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Save(Window window, string? output, string name)
    {
        if (output is null) return;
        using var image = window.CaptureRenderedFrame()!; image.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
    private static void VerifyPagination(Action<bool, string> check)
    {
        var view = ReportDesignDemo.Create();
        var layout = ReportPaginator.Paginate(view.Document!, view.DataSource!);
        check(layout.Pages.Count > 1 && layout.Pages.SelectMany(p => p.Rows).Select(r => r.SourceIndex).SequenceEqual(Enumerable.Range(0, 96)), "ReportDesign: 96 detail rows expand once in source order across multiple pages");
        check(layout.Pages.SelectMany(p => p.Rows).First().Cells[6] == "¥ 4,999.00", "ReportDesign: typed computed Amount getter feeds the formatted cell");
        var expected = Enumerable.Range(0, 96).Sum(i => (decimal)view.DataSource!.GetValue(i, "Amount")!);
        check(layout.Summary[6] == "¥ " + expected.ToString("N2", System.Globalization.CultureInfo.GetCultureInfo("zh-CN")) && layout.Pages.Count(p => p.ShowSummary) == 1 && layout.Pages[^1].ShowSummary,
            "ReportDesign: grand total includes all source rows once and appears only on the last page");
        check(layout.Pages.All(p => layout.TableTop + layout.HeaderHeight + p.Rows.Sum(r => r.Height) + (p.ShowSummary ? layout.SummaryHeight + 8 : 0) <= layout.DetailBottom + .001),
            "ReportDesign: repeated headers, complete rows and summary stay above the footer");
        var empty = ReportPaginator.Paginate(view.Document!, ReportDesignDemo.Source(0));
        check(empty.Pages.Count == 1 && empty.Pages[0].Rows.Count == 0 && empty.Summary[6] == "¥ 0.00", "ReportDesign: empty sources produce one valid empty page and zero total");
        var simple = new ReportDocument(); simple.Columns.Add(new() { Header = "数量", FieldKey = "Quantity", ShowSum = false });
        var first = ReportPaginator.Paginate(simple, ReportDesignDemo.Source(2));
        var capacity = first.DetailBottom - first.TableTop - first.HeaderHeight;
        simple.MinimumRowHeight = capacity / 2;
        var exact = ReportPaginator.Paginate(simple, ReportDesignDemo.Source(2));
        check(exact.Pages.Count == 1 && exact.Pages[0].Rows.Count == 2, "ReportDesign: exact-height rows do not create a trailing blank page");
        simple.Columns[0].ShowSum = true;
        exact = ReportPaginator.Paginate(simple, ReportDesignDemo.Source(2));
        check(exact.Pages.Count == 2 && exact.Pages[1].Rows.Count == 0 && exact.Pages[1].ShowSummary, "ReportDesign: overflowing total receives a final summary page");
        var wrapDocument = new ReportDocument { MinimumRowHeight = 20 }; wrapDocument.Columns.Add(new() { Header = "长文本", FieldKey = "Text" });
        var wrapped = ReportPaginator.Paginate(wrapDocument, ReportDataSource.Create("Text", new[] { new string('长', 140) }, new ReportField<string>("Text", "长文本", s => s)));
        check(wrapped.Pages[0].Rows[0].Height > 40, "ReportDesign: row heights use measured wrapped text");
        bool Reject(Action action) { try { action(); return false; } catch (ArgumentException) { return true; } }
        check(Reject(() => ReportPaginator.Paginate(wrapDocument, ReportDataSource.Create("Text", new[] { new string('长', 20000) }, new ReportField<string>("Text", "长文本", s => s)))), "ReportDesign: an oversized atomic row reports a clear layout error");
        simple.Columns[0].FieldKey = "Missing";
        check(Reject(() => ReportPaginator.Paginate(simple, ReportDesignDemo.Source(0))), "ReportDesign: an unknown field is rejected even for an empty source");
        check(Reject(() => ReportDataSource.Create("Duplicate", Array.Empty<int>(), new ReportField<int>("X", "X", i => i), new("X", "X", i => i))), "ReportDesign: duplicate field registrations are rejected");
        simple.Columns[0].FieldKey = "Customer";
        check(Reject(() => ReportPaginator.Paginate(simple, ReportDesignDemo.Source(2))), "ReportDesign: text columns cannot silently become numeric totals");
        simple.Columns[0].ShowSum = false; simple.Columns[0].ConditionEnabled = true;
        check(Reject(() => ReportPaginator.Paginate(simple, ReportDesignDemo.Source(2))), "ReportDesign: conditional numeric styles reject text fields");
    }
    private static void VerifyInteraction(Action<bool, string> check, string? output)
    {
        var view = new ReportDesign(); var window = Open(view);
        check(view.DataSource is null && view.Layout?.Pages.Count == 1 && view.ValidationError is null, "ReportDesign/design: blank paper is editable without a data source");
        var surface = Part<Control>(view, "PageSurface");
        Point Paper(double x, double y) => surface.TranslatePoint(new Point(88 + x * ReportPaginator.DipsPerMillimetre * view.Zoom, 30 + y * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
        void Drag(Control source, Point target)
        {
            var point = source.TranslatePoint(new Point(source.Bounds.Width / 2, source.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point); window.MouseDown(point, MouseButton.Left);
            window.MouseMove(new Point(point.X + 12, point.Y + 4), RawInputModifiers.LeftMouseButton);
            window.MouseMove(target, RawInputModifiers.LeftMouseButton); window.MouseUp(target, MouseButton.Left); Flush();
        }
        Drag(Part<Button>(view, "TextPalette"), Paper(25, 25));
        check(view.Document!.Elements.Count == 1 && view.Document.Elements[0].Kind == ReportElementKind.Text, $"ReportDesign/drag: palette text drops onto an unconnected page ({view.Document!.Elements[0].X:0.0}, {view.Document.Elements[0].Y:0.0})");
        var text = view.Document.Elements[0];
        var origin = Paper(text.X + 5, text.Y + 5); var target = Paper(text.X + 25, text.Y + 15);
        window.MouseDown(origin, MouseButton.Left); window.MouseMove(target, RawInputModifiers.LeftMouseButton); window.MouseUp(target, MouseButton.Left); Flush();
        check(Math.Abs(text.X - 45) < 1 && Math.Abs(text.Y - 35) < 1, $"ReportDesign/drag: selected element moves in page coordinates (actual {text.X:0.0}, {text.Y:0.0})");
        var oldWidth = text.Width; var oldHeight = text.Height;
        origin = Paper(text.X + text.Width - .5, text.Y + text.Height - .5); target = Paper(text.X + text.Width + 12, text.Y + text.Height + 5);
        window.MouseDown(origin, MouseButton.Left); window.MouseMove(target, RawInputModifiers.LeftMouseButton); window.MouseUp(target, MouseButton.Left); Flush();
        check(text.Width > oldWidth + 10 && text.Height > oldHeight + 4, "ReportDesign/resize: native pointer dragging the corner resizes the element");
        var document = view.Document; Click(window, Part<Button>(view, "UndoButton"));
        check(ReferenceEquals(document, view.Document) && Math.Abs(view.Document.Elements[0].Width - oldWidth) < .01, "ReportDesign/undo: resize is one undo operation and keeps the document instance");
        Drag(Part<Button>(view, "TablePalette"), Paper(14, 65));
        check(view.Document.Elements.Any(e => e.Kind == ReportElementKind.Table) && view.Document.Columns.Count == 3 && view.Layout is not null, "ReportDesign/drag: an unbound static table can be designed before fetching data");
        var line = view.AddElement(ReportElementKind.Line, 14, 55); var page = view.AddElement(ReportElementKind.PageNumber, 140, 274); page.Width = 56;
        view.ShowPreview(); Flush();
        check(view.Layout?.Pages.Count == 1 && view.DataSource is null, "ReportDesign/preview: static elements and unbound cells preview without API data");
        var templateTable = view.Document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        check(Math.Abs(view.Layout!.Pages[0].Rows[0].Height + view.Layout.HeaderHeight - templateTable.Height * ReportPaginator.DipsPerMillimetre) < .01,
            "ReportDesign/layout: table template height determines the preview row height");
        Save(window, output, "reportdesign-static-preview"); view.ShowDesign();
        Click(window, Part<TabItem>(view, "DataTabButton"));
        using var endpoint = new JsonEndpoint();
        var fetch = view.FetchApiDataAsync(endpoint.Url);
        while (!fetch.IsCompleted) { Thread.Sleep(10); Flush(); }
        fetch.GetAwaiter().GetResult();
        var timeout = DateTime.UtcNow.AddSeconds(10);
        var tree = Part<TreeView>(view, "DataTree");
        while (tree.Items.Count == 0 && DateTime.UtcNow < timeout) { Thread.Sleep(10); Flush(); }
        IEnumerable<ReportDataNode> Descendants(ReportDataNode node) => new[] { node }.Concat(node.Children.SelectMany(Descendants));
        var nodes = tree.Items.OfType<ReportDataNode>().SelectMany(Descendants).ToArray();
        check(view.DataSource is null && nodes.Any(node => node.Pointer == "/data/items"),
            "ReportDesign/API: GET success immediately displays the response tree while retaining the current binding source until a node is dropped");
        var nested = nodes.Single(node => node.Pointer == "/data/items/0/customer/name");
        var staticText = view.Document.Elements.First(e => e.Kind == ReportElementKind.Text);
        view.DropDataNodeAsync(nested, staticText.X + 4, staticText.Y + 4).GetAwaiter().GetResult(); Flush();
        check(staticText.FieldKey == "$root/data/items/0/customer/name",
            "ReportDesign/binding: a nested scalar node binds to the target text element with an absolute response pointer");
        Save(window, output, "reportdesign-api-design");
        view.ShowPreview(); Flush();
        check(view.Layout!.Elements.First(e => e.Name == staticText.Name).Text == "客户 A", "ReportDesign/API: preview resolves a scalar inside a nested collection by its saved pointer");
        Save(window, output, "reportdesign-api-preview");
        var source = view.DataSource;
        var failed = view.FetchApiDataAsync("not-an-api"); failed.GetAwaiter().GetResult();
        check(ReferenceEquals(source, view.DataSource), "ReportDesign/API: invalid endpoints preserve the current document and data");
        window.Close();
    }
    private sealed class JsonEndpoint : IDisposable
    {
        private readonly System.Net.Sockets.TcpListener _server = new(System.Net.IPAddress.Loopback, 0);
        internal string Url { get; }
        internal JsonEndpoint()
        {
            _server.Start(); Url = "http://127.0.0.1:" + ((System.Net.IPEndPoint)_server.LocalEndpoint).Port + "/report";
            _ = Task.Run(async () =>
            {
                try
                {
                    using var connection = await _server.AcceptTcpClientAsync(); await using var stream = connection.GetStream();
                    using var reader = new StreamReader(stream, System.Text.Encoding.ASCII, false, 1024, true);
                    while (await reader.ReadLineAsync() is { Length: > 0 }) { }
                    var rows = string.Join(',', Enumerable.Range(0, 80).Select(i => "{\"amount\":" + (i + 1) * 10 + ",\"customer\":{\"name\":\"客户 A\"}}"));
                    var body = System.Text.Encoding.UTF8.GetBytes("{\"success\":true,\"data\":{\"items\":[" + rows + "]}}");
                    var header = System.Text.Encoding.ASCII.GetBytes("HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: " + body.Length + "\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header); await stream.WriteAsync(body);
                }
                catch (System.Net.Sockets.SocketException) { }
                catch (ObjectDisposedException) { }
            });
        }
        public void Dispose() => _server.Stop();
    }
    private static void VerifyRendering(Action<bool, string> check, ThemeVariant theme, ControlSize density, string? output)
    {
        var view = ReportDesignDemo.CreateDesignerSample(); var window = Open(view, theme, density); var label = $"ReportDesign/{theme.Key}/{density}";
        check(new[] { "SourceTabs", "ViewTabs", "PropertyTabs" }.All(name =>
        {
            var tabs = Part<TabControl>(view, name);
            return tabs.Classes.Contains("card") && !TabFeatures.GetCanClose(tabs) && !TabFeatures.GetCanReorder(tabs)
                && tabs.GetVisualDescendants().OfType<CardTabStrip>().Any()
                && tabs.GetRealizedContainers().OfType<TabItem>().All(item => item.GetVisualDescendants().OfType<CardTabChrome>().Any());
        }), label + ": all designer tabs use the built-in card theme and card header chrome");
        var reportTabs = Part<global::Avalonia.Controls.Primitives.TabStrip>(view, "ReportNameTabs");
        var reportHeader = (global::Avalonia.Controls.Primitives.TabStripItem)reportTabs.ContainerFromIndex(0)!;
        var reportHeaderPosition = reportHeader.TranslatePoint(default, reportTabs)!.Value;
        check(reportTabs.Padding == default && reportTabs.BorderThickness == default && reportHeader.Margin == default
            && Math.Abs(reportHeaderPosition.X) < 1 && Math.Abs(reportTabs.Bounds.Width - reportHeader.Bounds.Width) < 1,
            label + ": the report-name strip has no duplicate border or left/right gutter");
        check(view.Layout is not null && view.ValidationError is null && Part<ComboBox>(view, "BindingPicker").SelectedItem is ReportFieldInfo { Key: "Amount" }, label + ": typed bindings and selected inspector initialize");
        check(view.GetVisualDescendants().OfType<NumericUpDown>().Where(n => n.Bounds.Width > 0).All(numeric =>
        {
            var text = numeric.GetVisualDescendants().OfType<TextBox>().First();
            var measured = new FormattedText(text.Text ?? "", System.Globalization.CultureInfo.CurrentCulture, text.FlowDirection,
                new Typeface(text.FontFamily, text.FontStyle, text.FontWeight), text.FontSize, Brushes.Black);
            return measured.Width <= text.Bounds.Width - text.Padding.Left - text.Padding.Right + 1;
        }), label + ": numeric values remain readable across densities");
        Save(window, output, $"reportdesign-{theme.Key}-{density}-design-wide");
        using (var frame = window.CaptureRenderedFrame()!)
        {
            var surface = Part<Control>(view, "PageSurface"); var layout = view.Layout!;
            var point = surface.TranslatePoint(new Point(88 + (layout.TableX + 12) * view.Zoom, 30 + (layout.TableTop + 3) * view.Zoom), window)!.Value;
            check(HasColor(frame, point, Color.Parse("#006061")), label + ": actual pixels contain the printed teal table header");
            point = surface.TranslatePoint(new Point(90 + layout.Margin * view.Zoom, 30 + (layout.DetailBottom - 30) * view.Zoom), window)!.Value;
            check(HasColor(frame, point, Colors.White), label + ": paper stays white independently of editor theme");
        }
        view.ShowPreview(); Flush(); Save(window, output, $"reportdesign-{theme.Key}-{density}-preview-wide");
        window.Width = 1024; Flush();
        check(Part<ScrollViewer>(view, "PageScroll").Extent.Width > Part<ScrollViewer>(view, "PageScroll").Viewport.Width && Part<ScrollViewer>(view, "PageScroll").Viewport.Width > 200,
            label + ": narrow layout retains native canvas scrolling and both docks");
        Save(window, output, $"reportdesign-{theme.Key}-{density}-preview-narrow"); window.Close();
    }
    private static void VerifyFiles(Action<bool, string> check, string? output)
    {
        var view = ReportDesignDemo.CreateDesignerSample(); var window = Open(view, width: 1586); window.Height = 992;
        var source = view.DataSource;
        var surfaceControl = Part<Control>(view, "PageSurface"); var palette = Part<Button>(view, "ImagePalette");
        var start = palette.TranslatePoint(new Point(palette.Bounds.Width / 2, palette.Bounds.Height / 2), window)!.Value;
        var end = surfaceControl.TranslatePoint(new Point(88 + 140 * ReportPaginator.DipsPerMillimetre * view.Zoom, 30 + 38 * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
        window.MouseDown(start, MouseButton.Left); window.MouseMove(start + new Vector(12, 4), RawInputModifiers.LeftMouseButton); window.MouseMove(end, RawInputModifiers.LeftMouseButton); window.MouseUp(end, MouseButton.Left); Flush();
        var image = view.Document!.Elements.Single(e => e.Kind == ReportElementKind.Image); image.Width = 28; image.Height = 8;
        check(Math.Abs(image.X - 140) < 1 && Math.Abs(image.Y - 38) < 1, $"ReportDesign/image: native pointer dragging inserts an image at the intended page position ({image.X}, {image.Y})");
        using var bitmap = new SkiaSharp.SKBitmap(40, 20); bitmap.Erase(SkiaSharp.SKColors.Coral);
        using var encoded = bitmap.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        view.SetImage(image, encoded.ToArray(), "sample.png");
        image.ImageFit = ReportImageFit.Contain;
        var title = view.Document!.Elements.First(e => e.Name == "报表标题"); view.SelectedElement = title; Flush();
        Click(window, Part<global::Avalonia.Controls.Primitives.ToggleButton>(view, "ItalicButton")); Click(window, Part<global::Avalonia.Controls.Primitives.ToggleButton>(view, "UnderlineButton"));
        Click(window, view.GetVisualDescendants().OfType<Button>().Single(b => Equals(b.Tag, "Center")));
        check(title.Italic && title.Underline && title.TextAlignment == ReportTextAlignment.Center, "ReportDesign/style: native toolbar clicks edit italic, underline and text alignment");
        var previousFont = title.FontFamily;
        var picker = Part<ComboBox>(view, "FontFamilyPicker");
        var selectedFont = picker.Items.Cast<string>().First(font => font != previousFont);
        picker.SelectedItem = selectedFont; Flush();
        check(title.FontFamily == selectedFont, "ReportDesign/style: the system font picker updates the selected report element");
        Click(window, Part<Button>(view, "UndoButton")); title = view.Document!.Elements.First(e => e.Name == "报表标题");
        check(title.FontFamily == previousFont && title.Italic && title.Underline, "ReportDesign/style: undo restores font selection and preserves the other text styles");
        var amount = view.Document.Columns[6]; amount.ConditionValue = 1000; amount.ConditionEnabled = true;
        var rows = view.Layout!.Pages.SelectMany(p => p.Rows).ToArray();
        check(rows[0].Foregrounds[6] == "#C43C3C" && rows[1].Foregrounds[6] == "" && view.Layout.Summary[6] == "¥ 249,434.00", "ReportDesign/style: numeric conditions highlight matching rows without changing totals or pagination data");
        var json = view.SaveTemplate();
        check(json.Contains("ImageData") && !json.Contains("RowCount") && !json.Contains("ApiUrl") && !json.Contains("ResponsePreview"), "ReportDesign/files: template embeds images and excludes runtime source rows and API responses");
        view.LoadTemplate(json); Flush();
        var restored = view.Document!; var restoredImage = restored.Elements.Single(e => e.Kind == ReportElementKind.Image);
        check(restoredImage.ImageData == image.ImageData && restoredImage.ImageName == "sample.png" && restored.Elements.First(e => e.Name == "报表标题") is { Italic: true, Underline: true, TextAlignment: ReportTextAlignment.Center } && restored.Columns[6] is { FieldKey: "Amount", Format: ReportValueFormat.Currency, ShowSum: true, ConditionEnabled: true, ConditionValue: 1000 },
            "ReportDesign/files: source-generated template roundtrip preserves geometry, text styles, bindings, totals and image content");
        check(view.DataSource is null && view.ValidationError is null && view.Layout?.Pages.Count == 1, "ReportDesign/files: portable template opens and previews without connecting a source");
        bool Reject(string value) { try { view.LoadTemplate(value); return false; } catch (Exception exception) when (exception is ArgumentException or System.Text.Json.JsonException) { return ReferenceEquals(restored, view.Document); } }
        check(Reject(json.Replace("\"Version\": 1", "\"Version\": 99")) && Reject("{broken") && Reject(json.Replace("\"X\": 14", "\"X\": -1")), "ReportDesign/files: unsupported, malformed and invalid templates preserve the current document");
        var oldImage = restoredImage.ImageData;
        try { view.SetImage(restoredImage, new byte[] { 1, 2, 3 }); } catch (ArgumentException) { }
        check(restoredImage.ImageData == oldImage, "ReportDesign/files: corrupt image replacement leaves the existing image intact");
        view.SelectedElement = restoredImage; Flush();
        check(Part<ComboBox>(view, "ImageFitPicker").SelectedIndex == 0 && Part<Button>(view, "ChooseImageButton").IsEffectivelyVisible, "ReportDesign/image: selected images expose import and aspect ratio controls");
        Part<ComboBox>(view, "ImageFitPicker").SelectedIndex = 1; Flush();
        check(restoredImage.ImageFit == ReportImageFit.Cover, "ReportDesign/image: compiled inspector binding changes the image cover mode");
        Part<ComboBox>(view, "ImageFitPicker").SelectedIndex = 0; Flush();
        using (var frame = window.CaptureRenderedFrame()!)
        {
            var surface = Part<Control>(view, "PageSurface"); var point = surface.TranslatePoint(new Point(88 + (restoredImage.X + restoredImage.Width / 2) * ReportPaginator.DipsPerMillimetre * view.Zoom, 30 + (restoredImage.Y + restoredImage.Height / 2) * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
            check(HasColor(frame, point, Color.Parse("#FF7F50")), "ReportDesign/image: embedded image is rendered as actual canvas pixels");
        }
        Save(window, output, "reportdesign-image-properties");
        view.DataSource = source; view.ShowPreview(); Flush();
        Save(window, output, "reportdesign-file-preview-page1");
        using var pdf = new MemoryStream(); var task = view.ExportPdfAsync(pdf);
        var timeout = DateTime.UtcNow.AddSeconds(20);
        while (!task.IsCompleted && DateTime.UtcNow < timeout) { Flush(); Thread.Sleep(1); }
        task.GetAwaiter().GetResult();
        check(pdf.Length > 1000 && System.Text.Encoding.ASCII.GetString(pdf.ToArray(), 0, 5) == "%PDF-" && view.Layout!.Pages.Count > 1, "ReportDesign/pdf: all paginated report pages export through the native vector drawing pipeline");
        if (output is not null) { File.WriteAllBytes(Path.Combine(output, "reportdesign-multipage.pdf"), pdf.ToArray()); File.WriteAllText(Path.Combine(output, "portable.reportdesign.json"), json); File.WriteAllText(Path.Combine(output, "expected-pdf-pages.txt"), view.Layout!.Pages.Count.ToString()); }
        view.GoToPage(view.Layout!.Pages.Count); Flush(); Save(window, output, "reportdesign-file-preview-last");
        view.ShowDesign(); restored.Elements.Remove(restoredImage); restored.Columns[6].ConditionEnabled = false; view.SelectedElement = restored.Elements.Single(e => e.Kind == ReportElementKind.Table); view.SelectedColumn = restored.Columns[6];
        Click(window, Part<TabItem>(view, "DataPropertiesTab"));
        title = restored.Elements.First(e => e.Name == "报表标题"); title.Italic = false; title.Underline = false; title.TextAlignment = ReportTextAlignment.Left;
        view.Zoom = .9; Flush(); Save(window, output, "reportdesign-reference-1586x992");
        restored.Columns[6].ConditionEnabled = true; view.BindField("Customer");
        check(!restored.Columns[6].ConditionEnabled && !restored.Columns[6].ShowSum && view.ValidationError is null, "ReportDesign/binding: rebinding a numeric column to text clears numeric-only conditions and totals");
        window.Close();
    }
    private static bool HasColor(Bitmap bitmap, Point point, Color color)
    {
        var scale = bitmap.Dpi.X / 96; var x = (int)(point.X * scale); var y = (int)(point.Y * scale);
        if (x < 0 || y < 0 || x >= bitmap.PixelSize.Width || y >= bitmap.PixelSize.Height) return false;
        using var memory = new PixelBuffer(bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4);
        bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), memory.Address, memory.Size, bitmap.PixelSize.Width * 4);
        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        var rgba = bitmap.Format == PixelFormat.Rgba8888;
        var r = System.Runtime.InteropServices.Marshal.ReadByte(memory.Address, offset + (rgba ? 0 : 2)); var g = System.Runtime.InteropServices.Marshal.ReadByte(memory.Address, offset + 1); var b = System.Runtime.InteropServices.Marshal.ReadByte(memory.Address, offset + (rgba ? 2 : 0));
        return Math.Abs(r - color.R) < 20 && Math.Abs(g - color.G) < 20 && Math.Abs(b - color.B) < 20;
    }
    private sealed class PixelBuffer(int size) : IDisposable
    {
        public int Size { get; } = size;
        public IntPtr Address { get; } = System.Runtime.InteropServices.Marshal.AllocHGlobal(size);
        public void Dispose() => System.Runtime.InteropServices.Marshal.FreeHGlobal(Address);
    }
}
