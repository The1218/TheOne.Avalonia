using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuSelectionPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 900, 420 })
        {
            var roots = Enumerable.Range(0, 3).Select(i => new NavEntry($"Root {i}")).ToArray();
            foreach (var root in roots)
                for (var i = 0; i < 3; i++) root.Entries.Add(new NavEntry($"{root.Title} / {i}"));
            roots[0].Entries[1].Entries.Add(new NavEntry("Deep selected page"));
            var menu = new NavMenu
            {
                ItemsSource = roots, OpenOnHover = true,
                HoverOpenDelay = TimeSpan.FromMilliseconds(1), HoverCloseDelay = TimeSpan.FromSeconds(2),
                ChildrenBinding = CompiledBinding.Create<NavEntry, System.Collections.ObjectModel.ObservableCollection<NavEntry>>(n => n.Entries)
            };
            var window = new Window { Width = width, Height = 600, RequestedThemeVariant = theme,
                Content = new Border { Padding = new Thickness(16), Child = menu, HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left } };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            try
            {
                foreach (var path in new[] { new[] { 0 }, new[] { 0, 0 }, new[] { 0, 1, 0 } })
                {
                    menu.IsCollapsed = false; Flush();
                    check(menu.SelectPath(path), $"NavMenu/selection: selects {string.Join('/', path)} before collapsing"); Flush();
                    var selected = menu.SelectedItem;
                    menu.IsCollapsed = true; Flush();
                    check(((NavMenuItem)menu.ContainerFromIndex(0)!).IsActiveAncestor,
                        "NavMenu/selection: collapsing retains the actual active root");
                    for (var i = 0; i < roots.Length; i++)
                    {
                        var row = (NavMenuItem)menu.ContainerFromIndex(i)!;
                        var header = Part<Border>(row, "PART_LayoutRoot");
                        var point = header.TranslatePoint(new Point(header.Bounds.Width / 2, header.Bounds.Height / 2), window)!.Value;
                        window.MouseMove(new Point(width - 2, 2)); Flush();
                        window.MouseMove(point); Wait();
                        var label = $"{theme}/{size}/{width}/{string.Join('/', path)}/hover-{i}";
                        check(menu.Submenu is not null, $"NavMenu/selection: {label} hover opens submenu");
                        var child = menu.Submenu!;
                        check(ReferenceEquals(menu.SelectedItem, selected), $"NavMenu/selection: {label} hover preserves selected page");
                        foreach (var item in child.GetRealizedContainers().OfType<NavMenuItem>())
                        {
                            var data = child.ItemFromContainer(item);
                            var active = ReferenceEquals(data, selected) || data is NavEntry entry && selected is NavEntry current && entry.Entries.Contains(current);
                            var background = (Part<Border>(item, "PART_LayoutRoot").Background as ISolidColorBrush)?.Color;
                            child.TryFindResource("One.AccentSoft", theme, out var accent);
                            check(item.IsSelected == ReferenceEquals(data, selected) && (!item.IsActiveAncestor || active),
                                $"NavMenu/selection: {label}/{data} has only its own selection state (selected={item.IsSelected}, ancestor={item.IsActiveAncestor})");
                            check((background == (accent as ISolidColorBrush)?.Color) == (item.IsSelected || item.IsActiveAncestor),
                                $"NavMenu/selection: {label}/{data} selected background matches current page (actual={background}, selected={item.IsSelected}, ancestor={item.IsActiveAncestor})");
                            VerifyPixel(Part<Border>(item, "PART_LayoutRoot"), check, $"{label}/{data}");
                        }
                        if (output != null)
                        {
                            Directory.CreateDirectory(output);
                            using var frame = window.CaptureRenderedFrame()!;
                            frame.Save(Path.Combine(output, $"navmenu-selection-{theme}-{size}-{width}-{string.Join('-', path)}-{i}.png"), PngBitmapEncoderOptions.Default);
                        }
                    }
                    menu.CloseSubmenu(); Flush();
                }
                var invocations = 0; menu.ItemInvoked += (_, _) => invocations++;
                Hover((NavMenuItem)menu.ContainerFromIndex(2)!, window);
                var target = (NavMenuItem)menu.Submenu!.ContainerFromIndex(1)!;
                target.Focus();
                window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
                window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null); Flush();
                check(ReferenceEquals(menu.SelectedItem, roots[2].Entries[1]) && invocations == 1 && !menu.IsSubmenuOpen,
                    $"NavMenu/selection: {theme}/{size}/{width} popup keyboard invokes the chosen page exactly once");
                Hover((NavMenuItem)menu.ContainerFromIndex(2)!, window);
                var selectedRow = (NavMenuItem)menu.Submenu!.ContainerFromIndex(1)!;
                check(selectedRow.IsSelected && !((NavMenuItem)menu.Submenu.ContainerFromIndex(0)!).IsActiveAncestor &&
                    !((NavMenuItem)menu.Submenu.ContainerFromIndex(2)!).IsActiveAncestor,
                    $"NavMenu/selection: {theme}/{size}/{width} reopening retains only the selected popup page");
                menu.CloseSubmenu(); Flush();
                Hover((NavMenuItem)menu.ContainerFromIndex(1)!, window);
                check(menu.Submenu!.GetRealizedContainers().OfType<NavMenuItem>().All(row => !row.IsSelected && !row.IsActiveAncestor) && invocations == 1,
                    $"NavMenu/selection: {theme}/{size}/{width} switching to another root clears unrelated highlights without invocation");
            }
            finally { window.Close(); Flush(); }
        }
    }

    private static T Part<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().First(c => c.Name == name && c.TemplatedParent == owner);
    private static void VerifyPixel(Border header, Action<bool, string> check, string label)
    {
        using var frame = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(header.Bounds.Width), (int)Math.Ceiling(header.Bounds.Height)), new Vector(96, 96));
        frame.Render(header);
        var bytes = new byte[frame.PixelSize.Width * frame.PixelSize.Height * 4];
        var memory = System.Runtime.InteropServices.Marshal.AllocHGlobal(bytes.Length);
        try
        {
            frame.CopyPixels(new PixelRect(frame.PixelSize), memory, bytes.Length, frame.PixelSize.Width * 4);
            System.Runtime.InteropServices.Marshal.Copy(memory, bytes, 0, bytes.Length);
            var index = ((frame.PixelSize.Height / 2) * frame.PixelSize.Width + 3) * 4;
            var bgra = frame.Format == PixelFormat.Bgra8888;
            var actual = Color.FromArgb(bytes[index + 3], bytes[index + (bgra ? 2 : 0)], bytes[index + 1], bytes[index + (bgra ? 0 : 2)]);
            var expected = (header.Background as ISolidColorBrush)?.Color ?? Colors.Transparent;
            check(expected.A == 0 ? actual.A == 0 : actual == expected,
                $"NavMenu/selection: {label} rendered row pixels match its own background");
        }
        finally { System.Runtime.InteropServices.Marshal.FreeHGlobal(memory); }
    }
    private static void Hover(NavMenuItem row, Window window)
    {
        window.MouseMove(new Point(window.Bounds.Width - 2, 2)); Flush();
        var header = Part<Border>(row, "PART_LayoutRoot");
        window.MouseMove(header.TranslatePoint(new Point(header.Bounds.Width / 2, header.Bounds.Height / 2), window)!.Value); Wait();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Wait()
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(60) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start();
        try { Dispatcher.UIThread.PushFrame(frame); } finally { timer.Stop(); }
        Flush();
    }
}
