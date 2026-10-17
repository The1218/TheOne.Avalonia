using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
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

internal static class ReportDialogsPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyPageSettings(check, output);
        VerifyTableFields(check, output);
    }

    private static void VerifyPageSettings(Action<bool, string> check, string? output)
    {
        var document = new ReportDocument { Title = "页面设置专项" };
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.Text, Name = "页脚文字", X = 14, Y = 270, Width = 160, Height = 10, Text = "固定页脚" });
        var designer = new ReportDesign { Document = document };
        var owner = Open(designer);
        owner.RequestedThemeVariant = ThemeVariant.Dark;
        ControlSizing.SetSize(owner, ControlSize.Large);
        Flush();
        var initial = document.Save();
        try
        {
            void Show()
            {
                Click(owner, designer.FindControl<Button>("PageSettingsButton")!);
            }

            Show();
            var dialog = Dialog(owner, "页面设置");
            check(dialog.ActualThemeVariant == ThemeVariant.Dark && ControlSizing.GetSize(dialog) == ControlSize.Large,
                "Report dialogs/page: the owned dialog inherits the host's actual dark theme and large density");
            var sheet = Named<Border>(dialog, "PageSettingsPaperPreview");
            check(Math.Abs(sheet.Width / sheet.Height - 210d / 297) < .001,
                "Report dialogs/page: the live paper preview preserves A4's true aspect ratio");
            dialog.RequestedThemeVariant = ThemeVariant.Light;
            ControlSizing.SetSize(dialog, ControlSize.Normal);
            Flush();
            var formScroll = Named<ScrollViewer>(dialog, "PageSettingsFormScroll");
            check(formScroll.Offset.Y == 0 && new[] { "HeaderHeight", "FooterHeight" }.All(name =>
                {
                    var input = Named<NumericUpDown>(dialog, name);
                    var position = input.TranslatePoint(default, formScroll)!.Value;
                    return position.Y >= 0 && position.Y + input.Bounds.Height <= formScroll.Bounds.Height + 1;
                }),
                "Report dialogs/page: both fixed-region inputs are fully visible without scrolling in the default normal-size dialog");
            CaptureVariants(dialog, check, output, "report-page-settings", "ApplyPageSettings", "CancelPageSettings");
            Named<NumericUpDown>(dialog, "PaperWidth").Value = 300;
            Named<NumericUpDown>(dialog, "PaperHeight").Value = 200;
            Flush();
            check(
                Named<ComboBox>(dialog, "PaperPicker").SelectedIndex == 3 &&
                Named<ComboBox>(dialog, "PageOrientationPicker").SelectedIndex == 1,
                "Report dialogs/page: manual landscape dimensions select custom paper and synchronize orientation");
            Named<NumericUpDown>(dialog, "PaperWidth").Value = 180;
            Named<NumericUpDown>(dialog, "PaperHeight").Value = 300;
            Flush();
            check(Named<ComboBox>(dialog, "PageOrientationPicker").SelectedIndex == 0,
                "Report dialogs/page: manual portrait dimensions keep the orientation picker consistent");
            Named<NumericUpDown>(dialog, "MarginLeft").Value = 220;
            Flush();
            check(
                !Named<Button>(dialog, "ApplyPageSettings").IsEnabled &&
                Named<TextBlock>(dialog, "PageSettingsValidation").Text!.Contains("页边距"),
                "Report dialogs/page: invalid margins show inline validation and block applying");
            check(document.Save() == initial,
                "Report dialogs/page: changing draft inputs never mutates the report before confirmation");
            Click(dialog, Named<Button>(dialog, "CancelPageSettings"));
            check(!dialog.IsVisible && document.Save() == initial,
                "Report dialogs/page: the native Cancel button discards draft changes");

            Show();
            dialog = Dialog(owner, "页面设置");
            Named<NumericUpDown>(dialog, "HeaderHeight").Value = 40;
            dialog.Close();
            Flush();
            check(document.Save() == initial,
                "Report dialogs/page: closing the window discards pending values");

            Show();
            dialog = Dialog(owner, "页面设置");
            Named<ComboBox>(dialog, "PaperPicker").SelectedIndex = 0;
            Named<ComboBox>(dialog, "PageOrientationPicker").SelectedIndex = 1;
            Named<NumericUpDown>(dialog, "MarginLeft").Value = 18;
            Named<NumericUpDown>(dialog, "HeaderHeight").Value = 25;
            Flush();
            check(Named<TextBlock>(dialog, "PaperDimensions").Text == "297 × 210 mm",
                "Report dialogs/page: paper and orientation update the physical-size preview");
            Click(dialog, Named<Button>(dialog, "ApplyPageSettings"));
            check(!dialog.IsVisible && document.PageWidthMm == 297 && document.PageHeightMm == 210
                  && document.LeftMargin == 18 && document.HeaderHeightMm == 25 && document.Elements[0].Y == 183,
                "Report dialogs/page: confirmation applies one validated draft and keeps footer content in its fixed region");
            Click(owner, designer.FindControl<Button>("UndoButton")!);
            check(document.Save() == initial,
                "Report dialogs/page: all confirmed page settings and footer movement undo together");
        }
        finally
        {
            CloseDialogs(owner);
            owner.Close();
            Flush();
        }
    }

    private static void VerifyTableFields(Action<bool, string> check, string? output)
    {
        using var endpoint = new JsonEndpoint();
        var document = new ReportDocument { Title = "集合字段配置专项" };
        document.Columns.Add(new ReportColumn { Header = "产品", FieldKey = "/old" });
        document.Columns.Add(new ReportColumn { Header = "数量" });
        document.Columns.Add(new ReportColumn { Header = "备注", Text = "保留文字" });
        var table = new ReportElement
            { Kind = ReportElementKind.Table, Name = "明细表格", X = 14, Y = 55, Width = 180, Height = 24, FontSize = 16 };
        document.Elements.Add(table);
        var originalSource = ReportDataSource.Create("原数据", new[] { "原产品" },
            new ReportField<string>("/old", "原产品", value => value));
        var designer = new ReportDesign { Document = document, DataSource = originalSource, Zoom = .75 };
        var owner = Open(designer);
        owner.RequestedThemeVariant = ThemeVariant.Dark;
        ControlSizing.SetSize(owner, ControlSize.Large);
        Flush();
        try
        {
            Click(owner, designer.FindControl<TabItem>("DataTabButton")!);
            Await(designer.FetchApiDataAsync(endpoint.Url));
            var dataTree = designer.FindControl<TreeView>("DataTree")!;

            void Expand(IEnumerable<ReportDataNode> nodes)
            {
                foreach (var node in nodes)
                {
                    node.IsExpanded = true;
                    Expand(node.Children);
                }
            }

            Expand(dataTree.Items.OfType<ReportDataNode>());
            Flush();

            void DropCollection()
            {
                var node = dataTree.GetVisualDescendants().OfType<TreeViewItem>()
                    .Single(item => item.DataContext is ReportDataNode
                    {
                        Kind: ReportDataNodeKind.Collection, Pointer: "/orders"
                    });
                var header = node.GetVisualDescendants().OfType<ContentPresenter>()
                    .First(control => control.Name == "PART_HeaderPresenter");
                header.BringIntoView();
                Flush();
                var start = header.TranslatePoint(new Point(header.Bounds.Width * .6, header.Bounds.Height / 2), owner)!
                    .Value;
                var surface = designer.FindControl<Control>("PageSurface")!;
                var target = surface.TranslatePoint(new Point(
                    88 + (table.X + 20) * ReportPaginator.DipsPerMillimetre * designer.Zoom,
                    30 + (table.Y + 16) * ReportPaginator.DipsPerMillimetre * designer.Zoom), owner)!.Value;
                owner.MouseMove(start);
                owner.MouseDown(start, MouseButton.Left);
                owner.MouseMove(start + new Vector(12, 4), RawInputModifiers.LeftMouseButton);
                owner.MouseMove(target, RawInputModifiers.LeftMouseButton);
                owner.MouseUp(target, MouseButton.Left);
                Flush();
            }

            var initial = document.Save();
            DropCollection();
            var dialog = Dialog(owner, "配置表格字段");
            check(dialog.ActualThemeVariant == ThemeVariant.Dark && ControlSizing.GetSize(dialog) == ControlSize.Large,
                "Report dialogs/fields: the mapping window inherits the host's actual theme and density");
            check(dialog.IsVisible && Named<TextBlock>(dialog, "TableFieldsCollection").Text!.Contains("orders")
                                   && dialog.GetVisualDescendants().OfType<ComboBox>().Count(control =>
                                       control.Name?.StartsWith("TableFieldPicker", StringComparison.Ordinal) ==
                                       true) == 3,
                "Report dialogs/fields: dragging an actual collection opens one native field picker for each existing column");
            check(Named<Button>(dialog, "ApplyTableFields").IsEnabled && document.Save() == initial,
                "Report dialogs/fields: columns may stay unbound and no mapping changes the report before confirmation");
            SelectIndex(dialog, Named<ComboBox>(dialog, "TableFieldPicker0"), 1);
            CaptureVariants(dialog, check, output, "report-table-fields", "ApplyTableFields", "CancelTableFields");
            Click(dialog, Named<Button>(dialog, "CancelTableFields"));
            check(document.Save() == initial && ReferenceEquals(designer.DataSource, originalSource),
                "Report dialogs/fields: Cancel preserves all column definitions and the original data source");

            DropCollection();
            dialog = Dialog(owner, "配置表格字段");
            SelectIndex(dialog, Named<ComboBox>(dialog, "TableFieldPicker0"), 1);
            dialog.Close();
            Flush();
            check(document.Save() == initial && ReferenceEquals(designer.DataSource, originalSource),
                "Report dialogs/fields: closing the window also discards mappings and source changes");

            DropCollection();
            dialog = Dialog(owner, "配置表格字段");
            SelectIndex(dialog, Named<ComboBox>(dialog, "TableFieldPicker0"), 1);
            SelectIndex(dialog, Named<ComboBox>(dialog, "TableFieldPicker1"), 2);
            check(
                Named<Button>(dialog, "ApplyTableFields").IsEnabled &&
                Named<TextBlock>(dialog, "TableFieldsStatus").Text!.Contains("2"),
                "Report dialogs/fields: native keyboard field selection updates the pending mapping summary");
            Click(dialog, Named<Button>(dialog, "ApplyTableFields"));
            check(!dialog.IsVisible && document.DataBodyPointer == "/orders" && designer.DataSource?.RowCount == 2
                  && document.Columns[0].FieldKey == "/product" && document.Columns[1].FieldKey == "/quantity"
                  && document.Columns[2].FieldKey.Length == 0 && document.Columns[2].Text == "保留文字",
                "Report dialogs/fields: confirming binds selected columns and keeps skipped static cells unchanged");
            Click(owner, designer.FindControl<Button>("UndoButton")!);
            check(document.Save() == initial && ReferenceEquals(designer.DataSource, originalSource),
                "Report dialogs/fields: confirmed source selection and all column mappings undo as one edit");
        }
        finally
        {
            CloseDialogs(owner);
            owner.Close();
            Flush();
        }
    }

    private static Window Open(Control content)
    {
        var window = new Window
        {
            Content = content, Width = 1586, Height = 992, RequestedThemeVariant = ThemeVariant.Light,
            WindowDecorations = WindowDecorations.None
        };
        ControlSizing.SetSize(window, ControlSize.Normal);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void CaptureVariants(Window dialog, Action<bool, string> check, string? output, string prefix,
        string applyName, string cancelName)
    {
        var width = dialog.Width;
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
        {
            dialog.RequestedThemeVariant = theme;
            ControlSizing.SetSize(dialog, size);
            Motion.SetEnabled(dialog, false);
            Flush();
            var apply = Named<Button>(dialog, applyName);
            var cancel = Named<Button>(dialog, cancelName);
            var applyPosition = apply.TranslatePoint(default, dialog)!.Value;
            var cancelPosition = cancel.TranslatePoint(default, dialog)!.Value;
            check(applyPosition.X > cancelPosition.X && applyPosition.Y >= 0 &&
                  applyPosition.Y + apply.Bounds.Height <= dialog.ClientSize.Height + 1
                  && cancelPosition.X >= 0 && cancel.Bounds.Width >= 90,
                $"Report dialogs/{prefix}/{theme.Key}/{size}: native primary and secondary actions fit inside the dialog");
            Capture(dialog, output, $"{prefix}-{theme.Key}-{size}");
        }

        dialog.Width = 680;
        Flush();
        var narrowApply = Named<Button>(dialog, applyName);
        var narrowPosition = narrowApply.TranslatePoint(default, dialog)!.Value;
        check(narrowPosition.X + narrowApply.Bounds.Width <= dialog.ClientSize.Width + 1,
            $"Report dialogs/{prefix}: actions remain reachable at the minimum width");
        var inputs = prefix == "report-page-settings"
            ? new[] { "PaperPicker", "FooterHeight" }
            : new[] { "TableFieldPicker0", "TableFieldPicker2" };
        foreach (var name in inputs)
        {
            var input = Named<Control>(dialog, name);
            input.BringIntoView();
            Flush();
            var scroll = input.GetVisualAncestors().OfType<ScrollViewer>().First();
            var position = input.TranslatePoint(default, scroll)!.Value;
            check(position.Y >= -1 && position.Y + input.Bounds.Height <= scroll.Bounds.Height + 1
                                   && input.Bounds.Width > 50 &&
                                   scroll.VerticalScrollBarVisibility == ScrollBarVisibility.Auto,
                $"Report dialogs/{prefix}: {name} stays reachable through native scrolling in the narrow dark/large layout");
        }

        Capture(dialog, output, prefix + "-narrow");
        dialog.Width = width;
        dialog.RequestedThemeVariant = ThemeVariant.Light;
        ControlSizing.SetSize(dialog, ControlSize.Normal);
        Flush();
    }

    private static void SelectIndex(Window dialog, ComboBox picker, int index)
    {
        void Press(Key key, PhysicalKey physicalKey)
        {
            dialog.KeyPress(key, RawInputModifiers.None, physicalKey, null);
            dialog.KeyRelease(key, RawInputModifiers.None, physicalKey, null);
        }

        picker.Focus();
        Press(Key.Home, PhysicalKey.Home);
        for (var count = 0; count < index; count++) Press(Key.Down, PhysicalKey.ArrowDown);
        Flush();
    }

    private static T Named<T>(Control root, string name) where T : Control
    {
        return root.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
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

    private static Window Dialog(Window owner, string title)
    {
        Window? dialog = null;
        WaitUntil(() =>
            (dialog = Windows().FirstOrDefault(window => window != owner && window.IsVisible && window.Title == title))
            is not null);
        return dialog!;
    }

    private static IEnumerable<Window> Windows()
    {
        return ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).Windows;
    }

    private static void CloseDialogs(Window owner)
    {
        foreach (var window in Windows().Where(window => window != owner && window.Owner == owner).ToArray())
            window.Close();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void WaitUntil(Func<bool> condition)
    {
        var timer = Stopwatch.StartNew();
        while (!condition() && timer.Elapsed < TimeSpan.FromSeconds(15))
        {
            Flush();
            Thread.Sleep(2);
        }

        if (!condition()) throw new TimeoutException("Report dialog verification timed out.");
    }

    private static void Await(Task task)
    {
        WaitUntil(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
        Flush();
    }

    private static void Capture(Window window, string? output, string name)
    {
        if (output is null) return;
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    private sealed class JsonEndpoint : IDisposable
    {
        private readonly TcpListener _server = new(IPAddress.Loopback, 0);

        internal JsonEndpoint()
        {
            _server.Start();
            Url = "http://127.0.0.1:" + ((IPEndPoint)_server.LocalEndpoint).Port + "/report";
            _ = Task.Run(async () =>
            {
                try
                {
                    using var connection = await _server.AcceptTcpClientAsync();
                    await using var stream = connection.GetStream();
                    using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                    while (await reader.ReadLineAsync() is { Length: > 0 })
                    {
                    }

                    var body = Encoding.UTF8.GetBytes(
                        "{\"company\":\"示例公司\",\"orders\":[{\"product\":\"产品 A\",\"quantity\":3,\"amount\":15},{\"product\":\"产品 B\",\"quantity\":2,\"amount\":20}]}");
                    var header = Encoding.ASCII.GetBytes(
                        "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: " + body.Length +
                        "\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header);
                    await stream.WriteAsync(body);
                }
                catch (SocketException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
            });
        }

        internal string Url { get; }

        public void Dispose()
        {
            _server.Stop();
        }
    }
}