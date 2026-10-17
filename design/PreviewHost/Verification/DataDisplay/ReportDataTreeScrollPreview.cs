using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

internal static class ReportDataTreeScrollPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var rows = string.Join(",",
            Enumerable.Range(0, 14).Select(index =>
                "{\"Id\":" + index + ",\"Name\":\"部门 " + index +
                "\",\"LongPropertyNameForNarrowTree\":\"较长的属性值，用于检查内容是否溢出滚动区域\"}"));
        using var endpoint =
            new ReportApiComponentsPreview.Endpoint(
                "{\"Code\":200,\"Message\":\"Success\",\"BusinessCode\":1,\"Data\":[" + rows + "]}");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
            VerifyCase(check, output, endpoint, theme, size);
    }

    private static void VerifyCase(Action<bool, string> check, string? output,
        ReportApiComponentsPreview.Endpoint endpoint, ThemeVariant theme, ControlSize size)
    {
        var designer = new ReportDesign { Document = new ReportDocument() };
        var text = new ReportElement
            { Kind = ReportElementKind.Text, Name = "绑定目标", X = 14, Y = 15, Width = 60, Height = 12 };
        designer.Document.Elements.Add(text);
        var window = new Window
        {
            Content = designer, Width = size == ControlSize.Large ? 1000 : 1400, Height = 900,
            RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        try
        {
            Click(window, designer.FindControl<TabItem>("DataTabButton")!);
            var loading = designer.FetchApiDataAsync(endpoint.Url);
            var end = DateTime.UtcNow.AddSeconds(10);
            while (!loading.IsCompleted && DateTime.UtcNow < end)
            {
                Thread.Sleep(10);
                Flush();
            }

            loading.GetAwaiter().GetResult();
            Flush();
            var status = designer.FindControl<TextBlock>("ApiSourceStatus")!;
            var tree = designer.FindControl<TreeView>("DataTree")!;

            IEnumerable<ReportDataNode> All(ReportDataNode node)
            {
                return new[] { node }.Concat(node.Children.SelectMany(All));
            }

            foreach (var node in tree.Items.OfType<ReportDataNode>().SelectMany(All)) node.IsExpanded = true;
            Flush();
            var scroll = tree.GetVisualDescendants().OfType<ScrollViewer>().First();
            var extent = scroll.Extent;
            var point = tree.TranslatePoint(new Point(tree.Bounds.Width / 2, tree.Bounds.Height / 2), window)!.Value;
            for (var i = 0; i < 25; i++)
            {
                window.MouseWheel(point, new Vector(0, -3));
                Flush();
            }

            check(scroll.Offset.Y > 0 && Math.Abs(scroll.Offset.Y + scroll.Viewport.Height - scroll.Extent.Height) < 1,
                $"Report data tree/{theme}/{size}: real wheel reaches the end of expanded JSON nodes");
            check(
                Math.Abs(scroll.Extent.Height - extent.Height) < 1 && scroll.Extent.Width <= scroll.Viewport.Width + 1,
                $"Report data tree/{theme}/{size}: scroll extent is stable without horizontal overflow");
            using (var frame = window.CaptureRenderedFrame()!)
            {
                using var bytes = new MemoryStream();
                frame.Save(bytes, PngBitmapEncoderOptions.Default);
                using var pixels = SKBitmap.Decode(bytes.ToArray());
                var origin = tree.TranslatePoint(default, window)!.Value;
                var scale = frame.Dpi.X / 96;
                var ink = 0;
                for (var y = (int)((origin.Y + 2) * scale); y < (origin.Y + tree.Bounds.Height - 2) * scale; y++)
                for (var x = (int)((origin.X + 3) * scale); x < (origin.X + tree.Bounds.Width - 20) * scale; x++)
                {
                    var color = pixels.GetPixel(x, y);
                    if (theme == ThemeVariant.Light
                            ? color.Red < 140 && color.Green < 140 && color.Blue < 140
                            : color.Red > 150 && color.Green > 150 && color.Blue > 150) ink++;
                }

                check(ink > 30,
                    $"Report data tree/{theme}/{size}: scrolled viewport actually draws node text instead of becoming blank");
            }

            Capture(window, output, $"tree-bottom-{theme}-{size}");
            for (var i = 0; i < 25; i++)
            {
                window.MouseWheel(point, new Vector(0, 3));
                Flush();
            }

            var selected = tree.Items.OfType<ReportDataNode>().SelectMany(All).Single(node => node.Pointer == "/Code");
            var selectedItem = tree.GetVisualDescendants().OfType<TreeViewItem>()
                .Single(item => ReferenceEquals(item.DataContext, selected));
            Click(window, selectedItem);
            // Preserve the viewport even when the selected node has scrolled out of view.
            for (var i = 0; i < 25; i++)
            {
                window.MouseWheel(point, new Vector(0, -3));
                Flush();
            }

            var offset = scroll.Offset;
            var expanded = tree.Items.OfType<ReportDataNode>().SelectMany(All)
                .ToDictionary(node => node, node => node.IsExpanded);
            var requests = endpoint.Requests.Count;
            for (var cycle = 0; cycle < 3; cycle++)
            {
                Click(window, designer.FindControl<TabItem>("ComponentsTabButton")!);
                Click(window, designer.FindControl<TabItem>("DataTabButton")!);
                scroll = tree.GetVisualDescendants().OfType<ScrollViewer>().First();
                check(ReferenceEquals(tree.SelectedItem, selected) && expanded.All(pair =>
                                                                       pair.Key.IsExpanded == pair.Value)
                                                                   && Math.Abs(scroll.Offset.Y - offset.Y) < .5 &&
                                                                   endpoint.Requests.Count == requests,
                    $"Report data tree/{theme}/{size}: tab cycle {cycle} retains expansion, selection and scroll without fetching again (offset {scroll.Offset.Y}/{offset.Y})");
            }

            for (var i = 0; i < 25; i++)
            {
                window.MouseWheel(point, new Vector(0, 3));
                Flush();
            }

            check(scroll.Offset.Y < .5,
                $"Report data tree/{theme}/{size}: wheel returns to the root without a stuck offset");
            var root = tree.Items.OfType<ReportDataNode>().Single();
            var data = root.Children.Single(node => node.Name == "Data");
            var branch = tree.GetVisualDescendants().OfType<TreeViewItem>()
                .Single(item => ReferenceEquals(item.DataContext, data));
            var chevron = branch.GetVisualDescendants().OfType<ToggleButton>().First(button =>
                button.Name == "PART_ExpandCollapseChevron" && ReferenceEquals(button.TemplatedParent, branch));
            Click(window, chevron);
            check(!data.IsExpanded && scroll.Extent.Height < extent.Height && scroll.Offset.Y == 0,
                $"Report data tree/{theme}/{size}: native collapse updates the node model and shrinks the scroll range");
            Click(window, chevron);
            check(data.IsExpanded && Math.Abs(scroll.Extent.Height - extent.Height) < 1,
                $"Report data tree/{theme}/{size}: re-expanding restores the same scroll range");
            var search = designer.FindControl<TextBox>("FieldSearch")!;
            search.Text = "部门 13";
            Flush();
            check(
                tree.Items.OfType<ReportDataNode>().SelectMany(All).Any(node => node.Preview == "部门 13") &&
                scroll.Offset.Y == 0,
                $"Report data tree/{theme}/{size}: filtering after scrolling retains matching nodes and visible ancestors");
            var filteredRoot = tree.Items[0];
            tree.SelectedItem = tree.Items.OfType<ReportDataNode>().SelectMany(All)
                .Single(node => node.Preview == "部门 13");
            var filteredSelection = tree.SelectedItem;
            Click(window, designer.FindControl<TabItem>("ComponentsTabButton")!);
            designer.RefreshReport();
            Click(window, designer.FindControl<TabItem>("DataTabButton")!);
            check(
                ReferenceEquals(tree.Items[0], filteredRoot) && ReferenceEquals(tree.SelectedItem, filteredSelection) &&
                search.Text == "部门 13",
                $"Report data tree/{theme}/{size}: a filtered tree retains its nodes, selection and search across tabs");
            search.Text = "";
            Flush();
            var scalar = tree.GetVisualDescendants().OfType<TreeViewItem>()
                .Single(item => item.DataContext is ReportDataNode { Pointer: "/Code" });
            var start = scalar.TranslatePoint(new Point(85, scalar.Bounds.Height / 2), window)!.Value;
            var surface = designer.FindControl<Control>("PageSurface")!;
            var target =
                surface.TranslatePoint(
                    new Point(88 + 18 * ReportPaginator.DipsPerMillimetre * designer.Zoom,
                        30 + 18 * ReportPaginator.DipsPerMillimetre * designer.Zoom), window)!.Value;
            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start + new Vector(12, 4), RawInputModifiers.LeftMouseButton);
            window.MouseMove(target, RawInputModifiers.LeftMouseButton);
            window.MouseUp(target, MouseButton.Left);
            Flush();
            check(text.Text == "200" && text.FieldKey == "$root/Code",
                $"Report data tree/{theme}/{size}: real dragging still fills and binds the scalar after scroll/filter operations");
            Capture(window, output, $"tree-top-{theme}-{size}");
            var roots = tree.Items.OfType<ReportDataNode>().ToArray();
            Await(designer.FetchApiDataAsync(endpoint.Url + "/invalid"));
            check(status.IsVisible && tree.Items.OfType<ReportDataNode>().SequenceEqual(roots),
                $"Report data tree/{theme}/{size}: failed fetch still displays an error and retains the loaded tree");
            Await(designer.FetchApiDataAsync(endpoint.Url));
            check(!status.IsVisible,
                $"Report data tree/{theme}/{size}: successful fetch removes the previous error without showing success text");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Await(Task task)
    {
        var end = DateTime.UtcNow.AddSeconds(10);
        while (!task.IsCompleted && DateTime.UtcNow < end)
        {
            Thread.Sleep(10);
            Flush();
        }

        if (!task.IsCompleted) throw new TimeoutException("Data tree API did not complete.");
        task.GetAwaiter().GetResult();
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

    private static void Capture(Window window, string? output, string name)
    {
        if (output is null) return;
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
}