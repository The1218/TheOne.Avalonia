using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class MenuScrollingPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
            VerifyCase("ContextMenu", theme, size, width,
                size == ControlSize.Normal && theme == ThemeVariant.Light && width == 760, check, output);
        foreach (var kind in new[] { "Menu", "MenuItem", "MenuFlyout" })
            VerifyCase(kind, ThemeVariant.Light, ControlSize.Normal, 760, true, check, output);
        VerifyConstrainedShort(check, output);
    }

    private static void VerifyConstrainedShort(Action<bool, string> check, string? output)
    {
        const string label = "MenuScrolling/ContextMenu/Light/Normal/760/constrained-two-items";
        try
        {
            using var menu = Open("ContextMenu", ThemeVariant.Light, ControlSize.Normal, 760, false, 80);
            var scroll = Scroller(menu);
            var viewport = Presenter(scroll);
            var arrows = Buttons(scroll);
            scroll.ScrollToHome();
            Flush();
            var first = VisibleItems(menu, viewport).FirstOrDefault();
            check(first is not null && PointerCanReach(first),
                label + ": an eighty-pixel popup keeps at least one full menu entry reachable");
            var down = arrows.Single(button => DockPanel.GetDock(button) == Dock.Bottom);
            check(down.IsVisible && down.Bounds.Height is > 0 and <= 20,
                label + ": the constrained menu exposes a compact down button" + Details(scroll, arrows));
            var before = scroll.Offset.Y;
            Click(down);
            check(scroll.Offset.Y > before, label + ": real down-button input advances the constrained two-item menu");
            scroll.ScrollToEnd();
            Flush();
            var last = menu.Items[^1];
            check(FullyInside(last, viewport) && PointerCanReach(last),
                label + ": the last entry is fully visible and reachable at the bottom");
            Capture(menu, last, label + "/bottom", check, output);
            Click(last);
            check(menu.Command.Count == 1 && !menu.IsOpen(),
                label + ": the exposed last entry executes its command and closes the constrained menu");
        }
        catch (Exception error)
        {
            check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message);
        }
        finally
        {
            Flush();
        }
    }

    private static void VerifyCase(string kind, ThemeVariant theme, ControlSize size, int width, bool interact,
        Action<bool, string> check, string? output)
    {
        var label = $"MenuScrolling/{kind}/{theme.Key}/{size}/{width}";
        try
        {
            using (var shortMenu = Open(kind, theme, size, width, false))
            {
                var scroller = Scroller(shortMenu);
                var presenter = Presenter(scroller);
                var buttons = Buttons(scroller);
                check(buttons.Length == 2 && buttons.All(button => !button.IsVisible),
                    label + ": the natural two-item menu shows neither scroll button" + Details(scroller, buttons));
                check(shortMenu.Items.All(item => FullyInside(item, presenter)),
                    label + ": both natural-height menu entries fit completely in the content viewport");
                check(shortMenu.Items.All(item => PointerCanReach(item)),
                    label + ": real pointer movement reaches both entries without an overlaid scroll button");
                Capture(shortMenu, shortMenu.Items[1], label + "/short", check, output);
                if (interact)
                {
                    Click(shortMenu.Items[1]);
                    check(shortMenu.Command.Count == 1 && !shortMenu.IsOpen(),
                        label + ": pointer activation executes the second command and closes the menu");
                    shortMenu.Reopen();
                    Flush();
                    shortMenu.Items[0].Focus();
                    shortMenu.Items[0].IsSelected = true;
                    Press(TopLevel.GetTopLevel(shortMenu.Items[0])!, Key.Down);
                    check(shortMenu.Items[1].IsSelected, label + ": native Down selects the next item");
                    Press(TopLevel.GetTopLevel(shortMenu.Items[1])!, Key.Enter);
                    check(shortMenu.Command.Count == 2 && !shortMenu.IsOpen(),
                        label + ": native Enter executes once and closes the menu");
                }
            }

            using var longMenu = Open(kind, theme, size, width, true);
            var scroll = Scroller(longMenu);
            var viewport = Presenter(scroll);
            var arrows = Buttons(scroll);
            check(
                arrows.Length == 2 && arrows.All(button =>
                    button.MinHeight == 0 && button.MinWidth == 0 && button.Padding == default &&
                    button.Bounds.Height <= 20),
                label + ": internal scrolling buttons have zero minimums/padding and stay at most twenty pixels high" +
                Details(scroll, arrows));
            scroll.ScrollToHome();
            Flush();
            CheckPosition(longMenu, scroll, viewport, arrows, "top", false, true, label, check);
            Capture(longMenu, VisibleItems(longMenu, viewport).FirstOrDefault(), label + "/long-top", check, output);
            if (!interact) return;
            var down = arrows.Single(button => DockPanel.GetDock(button) == Dock.Bottom);
            var up = arrows.Single(button => DockPanel.GetDock(button) == Dock.Top);
            var before = scroll.Offset.Y;
            Click(down);
            check(scroll.Offset.Y > before, label + ": real down-button click advances native scroll offset");
            scroll.Offset = new Vector(0, (scroll.Extent.Height - scroll.Viewport.Height) / 2);
            Flush();
            CheckPosition(longMenu, scroll, viewport, arrows, "middle", true, true, label, check);
            before = scroll.Offset.Y;
            Click(up);
            check(scroll.Offset.Y < before, label + ": real up-button click decreases native scroll offset");
            before = scroll.Offset.Y;
            var root = TopLevel.GetTopLevel(viewport)!;
            var point = viewport.TranslatePoint(new Point(viewport.Bounds.Width / 2, viewport.Bounds.Height / 2), root)!
                .Value;
            root.MouseMove(point);
            root.MouseWheel(point, new Vector(0, -2));
            Flush();
            check(scroll.Offset.Y > before, label + ": native wheel input scrolls the menu content");
            scroll.ScrollToEnd();
            Flush();
            CheckPosition(longMenu, scroll, viewport, arrows, "bottom", true, false, label, check);
            Capture(longMenu, VisibleItems(longMenu, viewport).LastOrDefault(), label + "/long-bottom", check, output);
            var last = VisibleItems(longMenu, viewport).LastOrDefault();
            if (last is not null) Click(last);
            check(last is not null && longMenu.Command.Count == 1 && !longMenu.IsOpen(),
                label + ": a fully exposed scrolled entry remains clickable and executes once");
        }
        catch (Exception error)
        {
            check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message);
        }
        finally
        {
            Flush();
        }
    }

    private static Fixture Open(string kind, ThemeVariant theme, ControlSize size, int width, bool many,
        double? maximum = null)
    {
        var command = new CounterCommand();
        var items = Enumerable.Range(0, many ? 36 : 2).Select(index => new MenuItem
        {
            Header = many ? $"菜单命令 {index + 1:00}" : index == 0 ? "复制" : "查看详情", Command = command
        }).ToArray();
        var anchor = new Button
        {
            Content = "右键查看菜单", HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
        };
        var bar = new Menu { VerticalAlignment = VerticalAlignment.Top };
        var content = new StackPanel();
        content.Children.Add(kind is "Menu" or "MenuItem" ? bar : anchor);
        var border = new Border { Padding = new Thickness(16), Child = content };
        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        var window = new Window
        {
            Width = width, Height = 450, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = border
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        ContextMenu? context = null;
        MenuFlyout? flyout = null;
        MenuItem? top = null;
        MenuItem? outer = null;
        Control host;
        Action reopen;
        Action close;
        Func<bool> isOpen;
        if (kind is "Menu" or "MenuItem")
        {
            top = new MenuItem { Header = "命令" };
            foreach (var item in items) top.Items.Add(item);
            if (kind == "MenuItem")
            {
                outer = new MenuItem { Header = "文件", Items = { top } };
                bar.Items.Add(outer);
            }
            else
            {
                bar.Items.Add(top);
            }

            reopen = () =>
            {
                if (outer is not null) outer.IsSubMenuOpen = true;
                top.IsSubMenuOpen = true;
            };
            close = () =>
            {
                top.IsSubMenuOpen = false;
                if (outer is not null) outer.IsSubMenuOpen = false;
                bar.Close();
            };
            isOpen = () => top.IsSubMenuOpen;
            window.Show();
            Flush();
            reopen();
            Flush();
            host = top.GetVisualDescendants().OfType<Popup>()
                .Single(popup => ReferenceEquals(popup.TemplatedParent, top)).Child!;
        }
        else if (kind == "ContextMenu")
        {
            context = new ContextMenu();
            foreach (var item in items) context.Items.Add(item);
            if (maximum is { } height) context.MaxHeight = height;
            else if (many) context.MaxHeight = 200;
            anchor.ContextMenu = context;
            reopen = () => context.Open(anchor);
            close = context.Close;
            isOpen = () => context.IsOpen;
            window.Show();
            Flush();
            reopen();
            Flush();
            host = context;
        }
        else
        {
            flyout = new MenuFlyout();
            foreach (var item in items) flyout.Items.Add(item);
            reopen = () => flyout.ShowAt(anchor);
            close = flyout.Hide;
            isOpen = () => flyout.IsOpen;
            window.Show();
            Flush();
            reopen();
            Flush();
            host = flyout.Popup.Child!;
        }

        if (maximum is { } maxHeight)
        {
            host.MaxHeight = maxHeight;
            Flush();
        }
        else if (many)
        {
            host.MaxHeight = 200;
            Flush();
        }

        return new Fixture(window, host, items, command, reopen, close, isOpen);
    }

    private static ScrollViewer Scroller(Fixture fixture)
    {
        return fixture.Host.GetVisualDescendants().OfType<ScrollViewer>().First();
    }

    private static ScrollContentPresenter Presenter(ScrollViewer scroll)
    {
        return scroll.GetVisualDescendants().OfType<ScrollContentPresenter>()
            .Single(p => ReferenceEquals(p.TemplatedParent, scroll));
    }

    private static RepeatButton[] Buttons(ScrollViewer scroll)
    {
        return scroll.GetVisualDescendants().OfType<RepeatButton>()
            .Where(button => ReferenceEquals(button.TemplatedParent, scroll)).ToArray();
    }

    private static Rect RelativeBounds(Control item, Visual host)
    {
        return new Rect(item.TranslatePoint(default, host)!.Value, item.Bounds.Size);
    }

    private static bool FullyInside(Control item, Control host)
    {
        return item.IsEffectivelyVisible && item.Bounds.Height > 0 && RelativeBounds(item, host) is var rect &&
               rect.Y >= -.5 && rect.Bottom <= host.Bounds.Height + .5 && rect.X >= -.5 &&
               rect.Right <= host.Bounds.Width + .5;
    }

    private static MenuItem[] VisibleItems(Fixture fixture, Control viewport)
    {
        return fixture.Items.Where(item => FullyInside(item, viewport)).ToArray();
    }

    private static void CheckPosition(Fixture fixture, ScrollViewer scroll, ScrollContentPresenter viewport,
        RepeatButton[] buttons, string position,
        bool up, bool down, string label, Action<bool, string> check)
    {
        check(buttons.Single(button => DockPanel.GetDock(button) == Dock.Top).IsVisible == up &&
              buttons.Single(button => DockPanel.GetDock(button) == Dock.Bottom).IsVisible == down,
            label + $": {position} has the native expected up/down visibility ({up}/{down})" +
            Details(scroll, buttons));
        var content = RelativeBounds(viewport, scroll);
        check(buttons.Where(button => button.IsVisible).All(button => RelativeBounds(button, scroll) is var rect &&
                                                                      (rect.Bottom <= content.Y + .5 ||
                                                                       rect.Y >= content.Bottom - .5)),
            label + $": {position} scroll buttons do not overlap the content viewport");
        var visible = VisibleItems(fixture, viewport);
        check(visible.Length >= 2 && visible.All(PointerCanReach),
            label + $": {position} exposes at least two fully visible items to real pointer input");
    }

    private static bool PointerCanReach(Control item)
    {
        var root = TopLevel.GetTopLevel(item);
        if (root is null || !item.IsEffectivelyVisible || item.Bounds.Height <= 0) return false;
        var point = item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), root);
        if (point is null) return false;
        root.MouseMove(point.Value);
        Flush();
        return item.IsPointerOver;
    }

    private static void Click(Control item)
    {
        var root = TopLevel.GetTopLevel(item)!;
        var point = item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), root)!.Value;
        root.MouseMove(point);
        root.MouseDown(point, MouseButton.Left);
        root.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Press(TopLevel root, Key key)
    {
        root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null);
        root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null);
        Flush();
    }

    private static string Details(ScrollViewer scroll, RepeatButton[] buttons)
    {
        return
            $" [extent={scroll.Extent.Height:F1}, viewport={scroll.Viewport.Height:F1}, offset={scroll.Offset.Y:F1}; " +
            string.Join(", ",
                buttons.Select(button =>
                    $"{DockPanel.GetDock(button)} visible={button.IsVisible} height={button.Bounds.Height:F1} min={button.MinHeight:F1} padding={button.Padding}")) +
            "]";
    }

    private static void Capture(Fixture fixture, MenuItem? item, string label, Action<bool, string> check,
        string? output)
    {
        if (item is null)
        {
            check(false, label + ": no visible row available for actual hover rendering");
            return;
        }

        PointerCanReach(item);
        using var bitmap =
            new RenderTargetBitmap(
                new PixelSize(Math.Max(1, (int)Math.Ceiling(fixture.Host.Bounds.Width)),
                    Math.Max(1, (int)Math.Ceiling(fixture.Host.Bounds.Height))), new Vector(96, 96));
        bitmap.Render(fixture.Host);
        var pixels = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var pin = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), pin.AddrOfPinnedObject(), pixels.Length,
                bitmap.PixelSize.Width * 4);
        }
        finally
        {
            pin.Free();
        }

        var distinct = new HashSet<uint>();
        for (var i = 0; i < pixels.Length; i += 4)
        {
            distinct.Add(BitConverter.ToUInt32(pixels, i));
            if (distinct.Count > 16) break;
        }

        check(distinct.Count > 4, label + ": actual popup rendering contains surface, text and arrow/hover pixels");
        var surface = item.GetVisualDescendants().OfType<Border>().Single(part =>
            part.Name == "PART_LayoutRoot" && ReferenceEquals(part.TemplatedParent, item));
        var row = RelativeBounds(surface, fixture.Host);
        var x = Math.Clamp((int)row.X + 3, 0, bitmap.PixelSize.Width - 1);
        var y = Math.Clamp((int)(row.Y + row.Height / 2), 0, bitmap.PixelSize.Height - 1);
        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        var pixel = bitmap.Format == PixelFormat.Bgra8888
            ? Color.FromArgb(pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset])
            : Color.FromArgb(pixels[offset + 3], pixels[offset], pixels[offset + 1], pixels[offset + 2]);
        check(item.IsPointerOver && surface.Background is ISolidColorBrush brush && pixel == brush.Color,
            label + ": visible menu row receives hover and its actual pixels match the menu hover surface");
        if (output is not null)
            bitmap.Save(Path.Combine(output, label.Replace('/', '-') + ".png"), PngBitmapEncoderOptions.Default);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class Fixture(
        Window window,
        Control host,
        MenuItem[] items,
        CounterCommand command,
        Action reopen,
        Action close,
        Func<bool> isOpen) : IDisposable
    {
        public Control Host { get; } = host;
        public MenuItem[] Items { get; } = items;
        public CounterCommand Command { get; } = command;
        public Action Reopen { get; } = reopen;
        public Func<bool> IsOpen { get; } = isOpen;

        public void Dispose()
        {
            close();
            window.Close();
            Flush();
        }
    }

    private sealed class CounterCommand : ICommand
    {
        public int Count { get; private set; }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Count++;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}