using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class PaginationPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var layout in Enum.GetValues<PaginationLayout>())
            VerifyAppearance(theme, size, layout, check);
        VerifyNavigation(check);
        VerifyInputs(check);
        VerifyBounds(check);
        VerifyDemo(check);
        if (output is not null) Render(output);
    }

    private static Pagination Sample()
    {
        return new Pagination { TotalCount = 1280, CurrentPage = 8, VerticalAlignment = VerticalAlignment.Top };
    }

    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal,
        double width = 1200, double height = 440)
    {
        var window = new Window
        {
            Width = width, Height = height, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme ?? ThemeVariant.Light,
            Content = new Border { Padding = new Thickness(24), Child = content }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static T Part<T>(Control control, string name) where T : Control
    {
        return control.GetVisualDescendants().OfType<T>()
            .Single(child => child.Name == name && child.TemplatedParent == control);
    }

    private static Button PageButton(Pagination pagination, long page)
    {
        return Part<ItemsControl>(pagination, "PART_Pages").GetVisualDescendants().OfType<Button>()
            .Single(button => button.DataContext is PaginationPageItem item && item.Page == page);
    }

    private static void VerifyAppearance(ThemeVariant theme, ControlSize size, PaginationLayout layout,
        Action<bool, string> check)
    {
        var pager = Sample();
        pager.Layout = layout;
        var window = Open(pager, theme, size, layout == PaginationLayout.Compact ? 420 : 1200);
        try
        {
            var label = $"Pagination/{theme.Key}/{size}/{layout}";
            check(
                pager.PageCount == 64 && pager.FirstItem == 141 && pager.LastItem == 160 &&
                pager.CurrentPageText == "8 / 64", $"{label}: count, page and item range agree");
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            check(
                pager.ItemHeight == height && Part<Button>(pager, "PART_PreviousButton").Bounds.Height == height &&
                Part<ComboBox>(pager, "PART_PageSizes").Bounds.Height == height,
                $"{label}: navigation and page-size selector inherit density (item={pager.ItemHeight}, previous={Part<Button>(pager, "PART_PreviousButton").Bounds.Height}, selector={Part<ComboBox>(pager, "PART_PageSizes").Bounds.Height})");
            check(
                pager.IsCompact == (layout == PaginationLayout.Compact) &&
                Part<ItemsControl>(pager, "PART_Pages").IsEffectivelyVisible != pager.IsCompact &&
                Part<TextBlock>(pager, "PART_CurrentPage").IsEffectivelyVisible == pager.IsCompact,
                $"{label}: exactly one navigation presentation is visible");
            check(
                !pager.IsCompact || (!Part<Control>(pager, "PART_Jump").IsEffectivelyVisible &&
                                     !Part<TextBlock>(pager, "PART_Range").IsEffectivelyVisible),
                $"{label}: compact mode omits jump and item range");
            check(
                pager.PageItems.Select(item => item.Page).SequenceEqual(new long[] { 1, 0, 6, 7, 8, 9, 10, 0, 64 }) &&
                pager.PageItems.Count(item => item.IsCurrent) == 1,
                $"{label}: bounded links preserve boundaries, siblings and ellipses");
            if (!pager.IsCompact)
            {
                var links = Part<ItemsControl>(pager, "PART_Pages");
                var buttons = links.GetVisualDescendants().OfType<Button>().Where(button => button.IsEffectivelyVisible)
                    .ToArray();
                check(
                    links.ItemsPanelRoot is StackPanel { Orientation: Orientation.Horizontal } && buttons.Length == 7 &&
                    buttons.All(button =>
                        Math.Abs(button.TranslatePoint(default, links)!.Value.Y -
                                 buttons[0].TranslatePoint(default, links)!.Value.Y) < 1),
                    $"{label}: every numeric page is visible on one horizontal row");
                var button = PageButton(pager, 8);
                var presenter = Part<ContentPresenter>(button, "PART_ContentPresenter");
                pager.TryFindResource("One.Accent", pager.ActualThemeVariant, out var accent);
                check((presenter.Background as ISolidColorBrush)?.Color == (accent as ISolidColorBrush)?.Color,
                    $"{label}: current page uses the live accent brush");
                using var bitmap =
                    new RenderTargetBitmap(new PixelSize((int)button.Bounds.Width, (int)button.Bounds.Height),
                        new Vector(96, 96));
                bitmap.Render(button);
                check(Pixel(bitmap, 5, (int)button.Bounds.Height / 2) == ((ISolidColorBrush)accent!).Color,
                    $"{label}: actual selected-page pixels match the theme");
            }

            check(
                Part<Button>(pager, "PART_PreviousButton").IsEffectivelyEnabled &&
                Part<Button>(pager, "PART_NextButton").IsEffectivelyEnabled,
                $"{label}: a middle page enables both directions");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyNavigation(Action<bool, string> check)
    {
        var pager = Sample();
        var window = Open(pager);
        var notifications = new List<PaginationPageChangedEventArgs>();
        pager.PageChanged += (_, args) => notifications.Add(args);
        try
        {
            Click(Part<Button>(pager, "PART_NextButton"));
            check(pager.CurrentPage == 9 && notifications.Count == 1,
                "Pagination/navigation: pointer next changes the page and notifies once");
            Click(Part<Button>(pager, "PART_PreviousButton"));
            Click(PageButton(pager, 10));
            check(pager.CurrentPage == 10 && notifications.Count == 3,
                "Pagination/navigation: native page buttons and previous navigate without duplicate callbacks");
            var target = PageButton(pager, 11);
            target.Focus();
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Flush();
            check(pager.CurrentPage == 11 && PageButton(pager, 11).IsFocused,
                "Pagination/navigation: Enter activates a page and preserves focus after links move");
            pager.GoFirst();
            Flush();
            var count = notifications.Count;
            Click(Part<Button>(pager, "PART_PreviousButton"));
            check(
                pager.CurrentPage == 1 && !Part<Button>(pager, "PART_PreviousButton").IsEffectivelyEnabled &&
                notifications.Count == count,
                "Pagination/navigation: the first page disables previous without changing state");
            pager.GoLast();
            Flush();
            count = notifications.Count;
            Click(Part<Button>(pager, "PART_NextButton"));
            check(
                pager.CurrentPage == 64 && !Part<Button>(pager, "PART_NextButton").IsEffectivelyEnabled &&
                notifications.Count == count,
                "Pagination/navigation: the last page disables next without changing state");
            check(!pager.GoToPage(0) && !pager.GoToPage(65) && !pager.GoToPage(64),
                "Pagination/navigation: invalid and repeated requests produce no navigation");
            pager.IsEnabled = false;
            Flush();
            check(
                !pager.GoFirst() && !Part<Button>(pager, "PART_PreviousButton").IsEffectivelyEnabled &&
                !Part<TextBox>(pager, "PART_JumpInput").IsEffectivelyEnabled,
                "Pagination/navigation: disabling the control disables native input and API navigation");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyInputs(Action<bool, string> check)
    {
        var pager = Sample();
        var window = Open(pager);
        var changes = new List<PaginationPageChangedEventArgs>();
        pager.PageChanged += (_, args) => changes.Add(args);
        try
        {
            var sizes = Part<ComboBox>(pager, "PART_PageSizes");
            sizes.Focus();
            window.KeyPress(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
            window.KeyRelease(Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
            Flush();
            check(
                pager.PageSize == 50 && pager.CurrentPage == 1 && pager.PageCount == 26 && changes.Count == 1 &&
                changes[0].OldPage == 8 && changes[0].CurrentPage == 1 && changes[0].PageSize == 50,
                "Pagination/size: keyboard selection resets to page one with one normalized notification");
            var input = Part<TextBox>(pager, "PART_JumpInput");
            input.Focus();
            input.SelectAll();
            window.KeyTextInput("12");
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            Flush();
            check(pager.CurrentPage == 12 && !pager.HasJumpError && input.IsFocused,
                "Pagination/jump: native text input and Enter commit a valid page without stealing focus");
            input.Text = "27";
            Click(Part<Button>(pager, "PART_JumpButton"));
            check(
                pager.CurrentPage == 12 && pager.HasJumpError &&
                Part<TextBlock>(pager, "PART_JumpError").IsEffectivelyVisible,
                "Pagination/jump: an out-of-range page preserves the current page and displays an error");
            input.Text = "abc";
            Click(Part<Button>(pager, "PART_JumpButton"));
            check(pager.HasJumpError && pager.CurrentPage == 12,
                "Pagination/jump: invalid text never becomes a page request");
            input.Focus();
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            Flush();
            check(!pager.HasJumpError && input.Text == "12",
                "Pagination/jump: Escape restores the current page and clears the error");
            input.Text = "26";
            Click(Part<Button>(pager, "PART_JumpButton"));
            check(pager.CurrentPage == 26 && pager.LastItem == 1280 && !pager.CanGoNext,
                "Pagination/jump: the last partial page has a correct range and disabled next");
            var options = new ObservableCollection<int> { 10, 20 };
            pager.PageSizeOptions = options;
            options.Add(75);
            Flush();
            check(
                pager.PageSizes.Any(option => option.Value == 75) && pager.PageSizes.Any(option => option.Value == 50),
                "Pagination/size: observable options refresh and preserve a custom active page size");
            var original = pager.Template;
            pager.Template = new FuncControlTemplate<Pagination>((_, _) => new Border());
            Flush();
            pager.Template = original;
            Flush();
            var before = changes.Count;
            Click(Part<Button>(pager, "PART_PreviousButton"));
            check(pager.CurrentPage == 25 && changes.Count == before + 1,
                "Pagination/template: replacing and restoring a template does not duplicate handlers");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyBounds(Action<bool, string> check)
    {
        var pager = Sample();
        var window = Open(pager);
        try
        {
            pager.TotalCount = 45;
            Flush();
            check(pager.CurrentPage == 3 && pager.PageCount == 3 && pager.FirstItem == 41 && pager.LastItem == 45,
                "Pagination/bounds: shrinking totals clamps the current page and partial range");
            pager.TotalCount = 1280;
            Flush();
            check(pager.CurrentPage == 3,
                "Pagination/bounds: growing totals does not restore an obsolete page request");
            pager.TotalCount = 0;
            Flush();
            check(
                pager.CurrentPage == 1 && pager.PageCount == 0 && pager.FirstItem == 0 && pager.LastItem == 0 &&
                pager.PageItems.Count == 0 && !pager.CanGoPrevious && !pager.CanGoNext &&
                !Part<Control>(pager, "PART_Jump").IsEffectivelyEnabled,
                "Pagination/bounds: an empty result has no fake page or enabled navigation");
            pager.TotalCount = 1;
            Flush();
            check(pager.PageCount == 1 && pager.PageItems.Single().Page == 1 && pager.LastItem == 1 && !pager.CanGoNext,
                "Pagination/bounds: a single record uses one page");
            pager.TotalCount = long.MaxValue;
            pager.PageSize = 1;
            pager.GoLast();
            Flush();
            check(
                pager.PageCount == long.MaxValue && pager.FirstItem == long.MaxValue &&
                pager.LastItem == long.MaxValue && pager.PageItems.Count <= 9,
                "Pagination/bounds: Int64 totals and final-page navigation do not overflow or create unbounded links");
            pager.PageSize = int.MaxValue;
            pager.GoLast();
            Flush();
            check(pager.LastItem == long.MaxValue && pager.FirstItem > 0,
                "Pagination/bounds: large page sizes keep the final range within Int64");
            pager.PageSize = 1;
            pager.CurrentPage = long.MaxValue / 2;
            window.Width = 850;
            Flush();
            check(pager.IsCompact && !Part<ItemsControl>(pager, "PART_Pages").IsEffectivelyVisible,
                $"Pagination/layout: actual long page labels trigger compact mode before navigation overlaps (compact={pager.IsCompact}, nav={Part<Grid>(pager, "PART_Navigation").DesiredSize.Width}, links={Part<ItemsControl>(pager, "PART_Pages").DesiredSize.Width}, width={pager.Bounds.Width}, size={pager.PageSize}, page={pager.CurrentPage}, count={pager.PageCount}, texts={string.Join("|", pager.PageItems.Select(item => item.Text))})");
            pager.TotalCount = 1280;
            pager.PageSize = 20;
            pager.CurrentPage = 8;
            window.Width = 360;
            Flush();
            check(
                pager.IsCompact && Part<Button>(pager, "PART_NextButton").Bounds.Width > 0 &&
                !Part<ItemsControl>(pager, "PART_Pages").IsEffectivelyVisible,
                "Pagination/layout: standard mode automatically becomes compact in a narrow viewport");
            var navigation = Part<Grid>(pager, "PART_Navigation");
            var summary = Part<Grid>(pager, "PART_Summary");
            check(
                summary.TranslatePoint(default, pager)!.Value.Y >=
                navigation.TranslatePoint(default, pager)!.Value.Y + navigation.Bounds.Height &&
                summary.Bounds.Width <= pager.Bounds.Width,
                "Pagination/layout: compact summary sits below navigation without overflowing");
            window.Width = 1200;
            Flush();
            check(!pager.IsCompact && Part<Control>(pager, "PART_Jump").IsEffectivelyVisible,
                $"Pagination/layout: widening restores standard controls (compact={pager.IsCompact}, width={pager.Bounds.Width}, window={window.ClientSize.Width}, jump={Part<Control>(pager, "PART_Jump").IsVisible}, page={pager.CurrentPage}, count={pager.PageCount}, nav={Part<Grid>(pager, "PART_Navigation").DesiredSize.Width})");
            pager.ItemHeight = 52;
            Flush();
            check(
                Part<Button>(pager, "PART_PreviousButton").Bounds.Height >= 52 &&
                Part<ComboBox>(pager, "PART_PageSizes").Bounds.Height >= 52,
                "Pagination/layout: explicit minimum item height overrides inherited density");
            pager.ShowTotal = pager.ShowPageSizeSelector = false;
            Flush();
            check(!summary.IsEffectivelyVisible,
                "Pagination/layout: hiding both summary fields reclaims the summary group");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyDemo(Action<bool, string> check)
    {
        var demo = new PaginationDemoView();
        var window = Open(demo, height: 1200);
        try
        {
            var standard = demo.FindControl<Pagination>("StandardPagination")!;
            var compact = demo.FindControl<Pagination>("CompactPagination")!;
            var before = demo.Model.NotificationCount;
            Click(Part<Button>(standard, "PART_NextButton"));
            check(demo.Model.CurrentPage == 9 && compact.CurrentPage == 9 && demo.Model.NotificationCount == before + 1,
                "Pagination/demo: compiled two-way bindings synchronize layouts and invoke the typed command");
            var sizes = Part<ComboBox>(compact, "PART_PageSizes");
            sizes.SelectedItem = compact.PageSizes.Single(option => option.Value == 100);
            Flush();
            check(demo.Model.PageSize == 100 && demo.Model.CurrentPage == 1 && standard.PageCount == 13,
                "Pagination/demo: page size changes flow through the shared view model");
            demo.Model.TotalCount = 0;
            Flush();
            check(standard.PageCount == 0 && compact.CurrentPageText == "0 / 0",
                "Pagination/demo: empty shared data updates both presentations");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1240, 380 })
        {
            var demo = new PaginationDemoView();
            var window = Open(demo, theme, size, width, 1600);
            try
            {
                window.Height = Math.Ceiling(demo.DesiredSize.Height) + 48;
                Flush();
                using var frame = window.CaptureRenderedFrame()!;
                var path = Path.Combine(output, $"pagination-{theme.Key}-{size}-{width}.png");
                frame.Save(path, PngBitmapEncoderOptions.Default);
                Console.WriteLine($"Rendered {path}");
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    private static void Press(Window window, Key key)
    {
        var physical = key switch
        {
            Key.Enter => PhysicalKey.Enter, Key.Down => PhysicalKey.ArrowDown, _ => PhysicalKey.Escape
        };
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
    }

    private static void Click(Control control)
    {
        control.BringIntoView();
        Flush();
        var window = (Window)TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            pin.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}