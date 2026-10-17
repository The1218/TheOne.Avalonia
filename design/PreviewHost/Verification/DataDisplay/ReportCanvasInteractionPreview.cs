using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportCanvasInteractionPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        var document = new ReportDocument();
        document.Columns.Add(new ReportColumn { Header = "金额", FieldKey = "Amount", ShowSum = true });
        document.Columns.Add(new ReportColumn { Header = "项目" });
        document.Columns.Add(new ReportColumn { Header = "备注" });
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.Text, Name = "公司名称", Text = "公司", X = 18, Y = 22, Width = 80, Height = 10 });
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.Table, Name = "明细表", X = 18, Y = 90, Width = 132, Height = 28, FontSize = 16 });
        document.Elements.Add(new ReportElement
            { Kind = ReportElementKind.PageNumber, Name = "页脚页码", X = 18, Y = 272, Width = 80, Height = 10 });
        var view = new ReportDesign
        {
            Document = document,
            DataSource = ReportDataSource.Create("Rows", Enumerable.Range(1, 80),
                new ReportField<int>("Amount", "金额", value => value, ReportValueKind.Number))
        };
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
            check(view.Layout is not null && view.ValidationError is null,
                $"ReportDesign/canvas: the numeric-source interaction fixture produces a valid layout ({view.ValidationError ?? "valid"})");
            var surface = view.FindControl<Control>("PageSurface")!;
            var scroll = view.FindControl<ScrollViewer>("PageScroll")!;
            var undo = view.FindControl<Button>("UndoButton")!;

            ReportElement Table()
            {
                return document.Elements.Single(element => element.Kind == ReportElementKind.Table);
            }

            Point Paper(double x, double y)
            {
                return surface.TranslatePoint(new Point(88 + x * ReportPaginator.DipsPerMillimetre * view.Zoom,
                    30 + y * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
            }

            void Undo()
            {
                Click(window, undo);
                check(!undo.IsEffectivelyEnabled,
                    "ReportDesign/canvas: a completed pointer gesture creates exactly one undo entry");
            }

            void SelectTable()
            {
                view.SelectedElement = Table();
                Flush();
                scroll.Offset = default;
                Flush();
            }

            SelectTable();
            var table = Table();
            var weights = document.Columns.Select(column => column.WidthWeight).ToArray();
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height / 2),
                Paper(table.X + table.Width + 18, table.Y + table.Height / 2));
            check(
                Math.Abs(table.Width - 150) < .1 && Math.Abs(table.Height - 28) < .1 &&
                document.Columns.Select(column => column.WidthWeight).SequenceEqual(weights),
                "ReportDesign/canvas: the visible right table grip changes total width while retaining row height and column proportions");
            Undo();
            table = Table();
            check(Math.Abs(table.Width - 132) < .1 && ReferenceEquals(view.Document, document),
                "ReportDesign/canvas: undo restores whole-table width in the consumer document");

            SelectTable();
            table = Table();
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height),
                Paper(table.X + table.Width + 12, table.Y + table.Height + 7));
            check(Math.Abs(table.Width - 144) < .1 && Math.Abs(table.Height - 35) < .1,
                "ReportDesign/canvas: the distinct bottom-right table grip changes both dimensions");
            Undo();

            SelectTable();
            table = Table();
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height), Paper(205, 280));
            check(
                table.X + table.Width <= document.PageWidthMm - document.RightMargin + .1 &&
                table.Y + table.Height <= document.FooterTop + .1,
                "ReportDesign/canvas: whole-table resizing respects the right margin and footer region");
            Undo();

            SelectTable();
            table = Table();
            var firstWidth = table.Width * document.Columns[0].WidthWeight /
                             document.Columns.Sum(column => column.WidthWeight);
            Drag(window, Paper(table.X + firstWidth, table.Y + 5), Paper(table.X + firstWidth + 6, table.Y + 5));
            check(document.Columns[0].WidthWeight > document.Columns[1].WidthWeight && Math.Abs(table.Width - 132) < .1,
                $"ReportDesign/canvas: an internal separator continues to resize adjacent columns without changing total width (weights {string.Join(", ", document.Columns.Select(column => column.WidthWeight))}, table {table.X}/{table.Y}/{table.Width}/{table.Height}, first width {firstWidth}, layout {view.ValidationError ?? "valid"})");
            Undo();

            SelectTable();
            table = Table();
            var header = view.Layout!.HeaderHeight / ReportPaginator.DipsPerMillimetre;
            Drag(window, Paper(table.X + 10, table.Y + header), Paper(table.X + 10, table.Y + header + 3));
            check(Math.Abs(table.TableHeaderHeightMm - header - 3) < 1 && Math.Abs(table.Height - 28) < .1,
                "ReportDesign/canvas: the internal header boundary retains independent row-height editing");
            Undo();

            SelectTable();
            table = Table();
            Drag(window, Paper(table.X + 10, table.Y + table.Height), Paper(table.X + 10, table.Y + table.Height + 5));
            check(Math.Abs(table.Height - 33) < .1 && Math.Abs(table.Width - 132) < .1,
                "ReportDesign/canvas: the lower row boundary retains independent detail-height editing");
            Undo();

            scroll.Offset = default;
            Flush();
            var headerHeight = document.HeaderHeightMm;
            Drag(window, Paper(-2, document.HeaderBottom), Paper(-2, document.HeaderBottom + 8));
            check(Math.Abs(document.HeaderHeightMm - headerHeight - 8) < .1 && Math.Abs(document.TopMargin - 14) < .1,
                "ReportDesign/canvas: the visible header boundary grip edits header height without changing page margins");
            Undo();

            scroll.Offset = default;
            Flush();
            Drag(window, Paper(-2, document.HeaderBottom), Paper(-2, Table().Y + 30));
            check(document.HeaderHeightMm >= 4 && document.HeaderBottom <= Table().Y + .1,
                "ReportDesign/canvas: header resizing preserves minimum height and leaves the table inside detail");
            Undo();

            scroll.Offset = new Vector(0, Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height));
            Flush();
            var footerHeight = document.FooterHeightMm;
            var footerOffset = document.Elements.Single(element => element.Kind == ReportElementKind.PageNumber).Y -
                               document.FooterTop;
            Drag(window, Paper(-2, document.FooterTop), Paper(-2, document.FooterTop - 7));
            var movedFooter = document.Elements.Single(element => element.Kind == ReportElementKind.PageNumber);
            check(Math.Abs(document.FooterHeightMm - footerHeight - 7) < .1 && Math.Abs(document.BottomMargin - 14) < .1
                                                                            && Math.Abs(movedFooter.Y -
                                                                                document.FooterTop - footerOffset) < .1,
                "ReportDesign/canvas: the visible footer boundary grip edits footer height without changing the bottom margin");
            view.ShowPreview();
            Flush();
            var pageCount = view.Layout!.Pages.Count;

            bool FooterInk(int page)
            {
                view.GoToPage(page);
                Flush();
                scroll.Offset = new Vector(0, Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height));
                Flush();
                var position = Paper(movedFooter.X, movedFooter.Y);
                using var frame = window.CaptureRenderedFrame()!;
                return HasInk(frame,
                    new Rect(position,
                        new Size(movedFooter.Width * ReportPaginator.DipsPerMillimetre * view.Zoom,
                            movedFooter.Height * ReportPaginator.DipsPerMillimetre * view.Zoom)));
            }

            check(pageCount > 1 && movedFooter.Y >= document.FooterTop && FooterInk(1) && FooterInk(pageCount),
                "ReportDesign/canvas: footer elements keep their band-relative offset and render on both first and last paginated pages");
            view.ShowDesign();
            Flush();
            Undo();

            scroll.Offset = new Vector(0, Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height));
            Flush();
            IPointer? capturedPointer = null;
            EventHandler<PointerPressedEventArgs> observe = (_, args) => capturedPointer = args.Pointer;
            surface.AddHandler(InputElement.PointerPressedEvent, observe, RoutingStrategies.Tunnel, true);
            var start = Paper(-2, document.FooterTop);
            var end = Paper(-2, document.FooterTop - 3);
            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(end);
            capturedPointer?.Capture(null);
            window.MouseUp(end, MouseButton.Left);
            Flush();
            surface.RemoveHandler(InputElement.PointerPressedEvent, observe);
            check(capturedPointer is not null && Math.Abs(document.FooterHeightMm - footerHeight - 3) < .1,
                "ReportDesign/canvas: losing native pointer capture finishes a band gesture with consistent footer geometry");
            Undo();

            var tree = view.FindControl<TreeView>("StructureTree")!;
            view.Zoom = 1.3;
            Flush();
            SetExpanded(window, Node(tree, "页脚"), true);
            scroll.Offset = default;
            view.ShowPreview();
            Flush();
            Click(window, Header(Node(tree, "页脚页码")));
            var footer = document.Elements.Single(element => element.Kind == ReportElementKind.PageNumber);

            bool Visible(ReportElement element)
            {
                var point = surface.TranslatePoint(new Point(
                    88 + element.X * ReportPaginator.DipsPerMillimetre * view.Zoom,
                    30 + element.Y * ReportPaginator.DipsPerMillimetre * view.Zoom), scroll)!.Value;
                return point.Y >= -1 && point.Y + element.Height * ReportPaginator.DipsPerMillimetre * view.Zoom <=
                    scroll.Viewport.Height + 1;
            }

            check(
                !view.IsPreview && ReferenceEquals(view.SelectedElement, footer) && Visible(footer) &&
                scroll.Offset.Y > 0,
                "ReportDesign/canvas: a structure child exits preview and locates the exact element inside the design viewport");
            view.ShowPreview();
            scroll.Offset = default;
            Flush();
            Click(window, Header(Node(tree, "页脚页码")));
            check(!view.IsPreview && ReferenceEquals(view.SelectedElement, footer) && Visible(footer),
                "ReportDesign/canvas: clicking the already-selected structure child locates it again");

            SetExpanded(window, Node(tree, "报表汇总"), true);
            view.ShowPreview();
            Flush();
            Click(window, Header(Node(tree, "金额 · 求和")));
            check(
                !view.IsPreview && ReferenceEquals(view.SelectedElement, Table()) &&
                ReferenceEquals(view.SelectedColumn, document.Columns[0]),
                "ReportDesign/canvas: a summary node selects its current document table and exact column after undo replaced the element objects");
            Click(window, Header(Node(tree, "明细表")));
            check(ReferenceEquals(view.SelectedElement, Table()) && view.SelectedColumn is null,
                "ReportDesign/canvas: the table structure node selects the complete table instead of a stale cell");

            var validSource = view.DataSource;
            view.DataSource = ReportDataSource.Create("Wrong kind", Enumerable.Range(1, 80),
                new ReportField<int>("Amount", "金额", value => value));
            view.Zoom = .72;
            SelectTable();
            table = Table();
            var originalX = table.X;
            var originalWidth = table.Width;
            firstWidth = table.Width * document.Columns[0].WidthWeight /
                         document.Columns.Sum(column => column.WidthWeight);
            check(view.Layout is null && view.ValidationError is not null,
                "ReportDesign/canvas: a mismatched summary data type retains its business validation error");
            Drag(window, Paper(table.X + firstWidth, table.Y + 5), Paper(table.X + firstWidth + 5, table.Y + 5));
            check(document.Columns[0].WidthWeight > document.Columns[1].WidthWeight &&
                  Math.Abs(table.X - originalX) < .1
                  && Math.Abs(table.Width - originalWidth) < .1 && view.ValidationError is not null,
                "ReportDesign/canvas: invalid source data does not turn an internal column resize into table movement");
            Undo();
            view.DataSource = validSource;
            Flush();

            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, "reportdesign-table-and-band-handles.png"),
                    PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
        }

        VerifyProportionalResize(check);
    }

    private static void VerifyProportionalResize(Action<bool, string> check)
    {
        var document = new ReportDocument();
        foreach (var (title, weight) in new[] { ("一", 1d), ("二", 1d), ("宽列", 16d), ("四", 1d) })
            document.Columns.Add(new ReportColumn { Header = title, WidthWeight = weight });
        var table = new ReportElement
            { Kind = ReportElementKind.Table, Name = "不等宽表格", X = 18, Y = 90, Width = 178, Height = 28, FontSize = 16 };
        document.Elements.Add(table);
        var view = new ReportDesign { Document = document, SelectedElement = table };
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
            var surface = view.FindControl<Control>("PageSurface")!;
            var scroll = view.FindControl<ScrollViewer>("PageScroll")!;

            Point Paper(double x, double y)
            {
                return surface.TranslatePoint(new Point(88 + x * ReportPaginator.DipsPerMillimetre * view.Zoom,
                    30 + y * ReportPaginator.DipsPerMillimetre * view.Zoom), window)!.Value;
            }

            var weights = document.Columns.Select(column => column.WidthWeight).ToArray();
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height / 2),
                Paper(table.X + 36, table.Y + table.Height / 2));
            check(view.ValidationError is null && view.Layout!.Columns.All(column => column.Width >= 32)
                                               && document.Columns.Select(column => column.WidthWeight)
                                                   .SequenceEqual(weights) && table.Width > 150,
                "ReportDesign/canvas: whole-table shrinking retains unequal column weights and every minimum printable column width");
            var total = document.Columns.Sum(column => column.WidthWeight);
            var firstWidth = table.Width * document.Columns[0].WidthWeight / total;
            var pairWidth = table.Width * (document.Columns[0].WidthWeight + document.Columns[1].WidthWeight) / total;
            Drag(window, Paper(table.X + firstWidth, table.Y + 5), Paper(table.X + firstWidth + 6, table.Y + 5));
            check(
                pairWidth < 18 && document.Columns.All(column => column.WidthWeight > 0) &&
                view.ValidationError is null,
                "ReportDesign/canvas: resizing a narrow adjacent pair never creates negative column weights");

            var wrapped = new ReportDocument();
            foreach (var title in new[] { "长表头自动换行", "另一列自动换行", "第三列自动换行" })
                wrapped.Columns.Add(new ReportColumn { Header = title });
            table = new ReportElement
            {
                Kind = ReportElementKind.Table, Name = "换行表格", X = 18, Y = 90, Width = 132, Height = 28, FontSize = 16
            };
            wrapped.Elements.Add(table);
            view.Document = wrapped;
            view.SelectedElement = table;
            Flush();
            scroll.Offset = default;
            Flush();
            var bodyHeight = table.Height - view.Layout!.HeaderHeight / ReportPaginator.DipsPerMillimetre;
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height / 2),
                Paper(table.X + 75, table.Y + table.Height / 2));
            check(view.ValidationError is null && view.Layout!.HeaderHeight / ReportPaginator.DipsPerMillimetre > 11
                                               && Math.Abs(table.Height -
                                                           view.Layout.HeaderHeight /
                                                           ReportPaginator.DipsPerMillimetre - bodyHeight) < .1,
                "ReportDesign/canvas: shrinking width measures wrapped headers and retains the actual detail-template height");
            Drag(window, Paper(table.X + table.Width, table.Y + table.Height),
                Paper(table.X + table.Width, table.Y + 5));
            check(
                Math.Abs(table.Height - view.Layout!.HeaderHeight / ReportPaginator.DipsPerMillimetre -
                         28 / ReportPaginator.DipsPerMillimetre) < .1,
                "ReportDesign/canvas: minimum table geometry matches the visible 28-DIP detail row and its resize handle");

            var movedDocument = new ReportDocument();
            movedDocument.Columns.Add(new ReportColumn { Header = "数值" });
            movedDocument.Columns.Add(new ReportColumn { Header = "说明" });
            var moving = new ReportElement
            {
                Kind = ReportElementKind.Text, Name = "跨区说明", Text = "移动到明细", X = 18, Y = 22, Width = 60, Height = 10
            };
            movedDocument.Elements.Add(moving);
            movedDocument.Elements.Add(new ReportElement
                { Kind = ReportElementKind.Table, Name = "分组表格", X = 18, Y = 90, Width = 132, Height = 28 });
            view.Document = movedDocument;
            view.SelectedElement = moving;
            Flush();
            scroll.Offset = default;
            Flush();
            Drag(window, Paper(moving.X + 4, moving.Y + 4), Paper(moving.X + 4, movedDocument.HeaderBottom + 24));
            var tree = view.FindControl<TreeView>("StructureTree")!;

            string ParentName(string child)
            {
                return Header(Node(tree, child).GetVisualAncestors().OfType<TreeViewItem>().First())
                    .GetVisualDescendants().OfType<TextBlock>().First().Text!;
            }

            check(
                moving.Y >= movedDocument.HeaderBottom && ParentName(moving.Name) == "明细" &&
                Node(tree, "页眉").Items.Count == 0,
                "ReportDesign/canvas: one real pointer move across the header boundary updates the element's actual structure group");
            var undo = view.FindControl<Button>("UndoButton")!;
            Click(window, undo);
            SetExpanded(window, Node(tree, "页眉"), true);
            check(
                !undo.IsEffectivelyEnabled &&
                Math.Abs(movedDocument.Elements.Single(element => element.Name == moving.Name).Y - 22) < .1 &&
                ParentName(moving.Name) == "页眉",
                "ReportDesign/canvas: one undo restores both cross-region element geometry and its structure group");

            var sumColumn = movedDocument.Columns[0];
            sumColumn.ShowSum = true;
            Flush();
            SetExpanded(window, Node(tree, "报表汇总"), true);
            check(Node(tree, "报表汇总").Items.Count == 1 && ParentName("数值 · 求和") == "报表汇总",
                "ReportDesign/canvas: enabling column sums immediately adds its native summary-tree child");
            sumColumn.Header = "更新后的数值";
            Flush();
            check(Node(tree, "报表汇总").IsExpanded && ParentName("更新后的数值 · 求和") == "报表汇总",
                "ReportDesign/canvas: renaming a summed column immediately updates its tree label and retains expansion");
            sumColumn.ShowSum = false;
            Flush();
            check(Node(tree, "报表汇总").Items.Count == 0,
                "ReportDesign/canvas: disabling sums immediately removes the corresponding summary-tree child");
        }
        finally
        {
            window.Close();
        }
    }

    private static ContentPresenter Header(TreeViewItem item)
    {
        return item.GetVisualDescendants().OfType<ContentPresenter>()
            .First(part => part.Name == "PART_HeaderPresenter");
    }

    private static TreeViewItem Node(TreeView tree, string name)
    {
        return tree.GetVisualDescendants().OfType<TreeViewItem>()
            .Single(item => Header(item).GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == name));
    }

    private static void SetExpanded(Window window, TreeViewItem item, bool expanded)
    {
        if (item.IsExpanded != expanded)
            Click(window,
                item.GetVisualDescendants().OfType<ToggleButton>()
                    .First(part => part.Name == "PART_ExpandCollapseChevron"));
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

    private static void Drag(Window window, Point start, Point end)
    {
        window.MouseMove(start);
        window.MouseDown(start, MouseButton.Left);
        window.MouseMove(new Point((start.X + end.X) / 2, (start.Y + end.Y) / 2));
        window.MouseMove(end);
        window.MouseUp(end, MouseButton.Left);
        Flush();
    }

    private static bool HasInk(Bitmap bitmap, Rect bounds)
    {
        var scale = bitmap.Dpi.X / 96;
        var x = Math.Max(0, (int)(bounds.X * scale));
        var y = Math.Max(0, (int)(bounds.Y * scale));
        var width = Math.Min(bitmap.PixelSize.Width - x, (int)(bounds.Width * scale));
        var height = Math.Min(bitmap.PixelSize.Height - y, (int)(bounds.Height * scale));
        if (width <= 0 || height <= 0) return false;
        var size = width * height * 4;
        var address = Marshal.AllocHGlobal(size);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, width, height), address, size, width * 4);
            var rgba = bitmap.Format == PixelFormat.Rgba8888;
            var count = 0;
            for (var i = 0; i < width * height; i++)
            {
                var offset = i * 4;
                var red = Marshal.ReadByte(address, offset + (rgba ? 0 : 2));
                var green = Marshal.ReadByte(address, offset + 1);
                var blue = Marshal.ReadByte(address, offset + (rgba ? 2 : 0));
                if (red < 120 && green < 120 && blue < 120 && ++count > 8) return true;
            }

            return false;
        }
        finally
        {
            Marshal.FreeHGlobal(address);
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}