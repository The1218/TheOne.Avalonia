using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuFlingPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var frames = new List<string> { "pass,step,before,after,extent,ms,input,allocation,prepared,cleared,rows" };
        var window = new MainWindow { Width = 1080, Height = 650 };
        window.Model.MotionEnabled = true;
        window.Show();
        ThemeAppearancePreview.Flush();
        var menu = window.FindControl<NavMenu>("Navigation")!;
        var scroll = menu.GetVisualDescendants().OfType<ScrollViewer>().Single(s => s.Name == "PART_MenuScrollViewer");
        var point = scroll.TranslatePoint(new Point(100, scroll.Bounds.Height / 2), window)!.Value;
        var prepared = 0;
        var cleared = 0;

        void Track(object? sender, ContainerPreparedEventArgs args)
        {
            prepared++;
            if (args.Container is NavMenuItem row)
            {
                row.ContainerPrepared -= Track;
                row.ContainerPrepared += Track;
                row.ContainerClearing -= Clear;
                row.ContainerClearing += Clear;
            }
        }

        void Clear(object? sender, ContainerClearingEventArgs args)
        {
            cleared++;
        }

        menu.ContainerPrepared += Track;
        menu.ContainerClearing += Clear;
        foreach (var row in menu.GetRealizedTreeContainers().OfType<NavMenuItem>())
        {
            row.ContainerPrepared += Track;
            row.ContainerClearing += Clear;
        }

        try
        {
            for (var pass = 0; pass < 6; pass++)
            {
                window.MouseWheel(point, new Vector(0, -1000));
                ThemeAppearancePreview.Flush();
                check(scroll.Offset.Y > 0, $"NavMenu/fling/{pass}: starts at the bottom");
                for (var step = 0; step < 60 && scroll.Offset.Y > 0; step++)
                {
                    prepared = cleared = 0;
                    var before = scroll.Offset.Y;
                    var allocation = GC.GetAllocatedBytesForCurrentThread();
                    var timer = Stopwatch.StartNew();
                    window.MouseWheel(point, new Vector(0, 5));
                    var input = timer.Elapsed.TotalMilliseconds;
                    ThemeAppearancePreview.Flush();
                    frames.Add(FormattableString.Invariant(
                        $"{pass},{step},{before},{scroll.Offset.Y},{scroll.Extent.Height},{timer.Elapsed.TotalMilliseconds},{input},{GC.GetAllocatedBytesForCurrentThread() - allocation},{prepared},{cleared},{menu.GetRealizedTreeContainers().Count()}"));
                }

                check(scroll.Offset.Y == 0, $"NavMenu/fling/{pass}: native wheel returns to the top");
            }
        }
        finally
        {
            window.Close();
        }

        if (output is not null) File.WriteAllLines(output, frames);
        VerifyGallery(check, output is null ? null : Path.GetDirectoryName(Path.GetFullPath(output)));
        VerifyBoundedRoots(check);
    }

    private static void VerifyGallery(Action<bool, string> check, string? output)
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var motion in new[] { false, true })
        {
            var window = new MainWindow { Width = 1080, Height = 650 };
            window.Model.Size = size;
            window.Model.Theme = variant;
            window.Model.MotionEnabled = motion;
            window.Show();
            ThemeAppearancePreview.Flush();
            try
            {
                var menu = window.FindControl<NavMenu>("Navigation")!;
                menu.TryFindResource("One.Surface", variant, out var background);
                menu.Background = (IBrush)background!;
                ThemeAppearancePreview.Flush();
                var scroll = menu.GetVisualDescendants().OfType<ScrollViewer>()
                    .Single(s => s.Name == "PART_MenuScrollViewer");
                var roots = menu.GetRealizedContainers().ToArray();
                var selected = menu.SelectedItem;
                var label = $"NavMenu/fling/{variant}/{size}/motion={motion}";
                check(menu.ItemsPanelRoot is StackPanel && roots.Length == menu.ItemCount,
                    label + ": small hierarchical roots retain their containers");
                foreach (var width in new[] { 232d, 160d })
                {
                    menu.Width = width;
                    ThemeAppearancePreview.Flush();
                    var point = scroll.TranslatePoint(new Point(width / 2, scroll.Bounds.Height / 2), window)!.Value;
                    foreach (var delta in new[] { 1d, 5d, 20d })
                    {
                        // Several wheel events let nested panels settle their estimated extents.
                        for (var i = 0; i < 4; i++)
                        {
                            window.MouseWheel(point, new Vector(0, -1000));
                            ThemeAppearancePreview.Flush();
                        }

                        check(Math.Abs(scroll.Offset.Y + scroll.Viewport.Height - scroll.Extent.Height) < 1,
                            label + $"/{width}/{delta}: starts at the actual bottom");
                        var last = scroll.Offset.Y;
                        var extent = scroll.Extent.Height;
                        var monotonic = true;
                        var stableExtent = true;
                        for (var i = 0; i < 250 && scroll.Offset.Y > 0; i++)
                        {
                            window.MouseWheel(point, new Vector(0, delta));
                            ThemeAppearancePreview.Flush();
                            monotonic &= scroll.Offset.Y <= last;
                            stableExtent &= Math.Abs(scroll.Extent.Height - extent) < 1;
                            last = scroll.Offset.Y;
                        }

                        check(scroll.Offset.Y == 0 && monotonic,
                            label +
                            $"/{width}/{delta}: wheel reversal crosses the middle and reaches the top without jumping backwards");
                        check(stableExtent,
                            label + $"/{width}/{delta}: total scroll height stays stable across group boundaries");
                        check(
                            Enumerable.Range(0, menu.ItemCount)
                                .All(i => ReferenceEquals(menu.ContainerFromIndex(i), roots[i])),
                            label + $"/{width}/{delta}: expanded root branches are never recycled by wheel input");
                        check(
                            ReferenceEquals(menu.SelectedItem, selected) &&
                            window.Model.NavigationRoots.Skip(1).All(group => group.IsExpanded),
                            label + $"/{width}/{delta}: scrolling preserves page selection and model expansion");
                    }

                    var first = (NavMenuItem)menu.ContainerFromIndex(0)!;
                    var surface = first.GetVisualDescendants().OfType<Border>().First(c => c.Name == "PART_LayoutRoot");
                    check(
                        surface.Bounds.X >= 4 && surface.Bounds.Width <= width - 8 &&
                        surface.Bounds.Height == menu.ItemHeight,
                        label + $"/{width}: selected row retains its inset and density");
                    if (!motion)
                    {
                        using var bitmap =
                            new RenderTargetBitmap(new PixelSize((int)width, (int)Math.Ceiling(menu.Bounds.Height)));
                        bitmap.Render(menu);
                        var bytes = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
                        var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                        try
                        {
                            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), pinned.AddrOfPinnedObject(),
                                bytes.Length, bitmap.PixelSize.Width * 4);
                        }
                        finally
                        {
                            pinned.Free();
                        }

                        var y = (int)surface.TranslatePoint(new Point(0, surface.Bounds.Height / 2), menu)!.Value.Y;

                        Color Pixel(int x)
                        {
                            var offset = (y * bitmap.PixelSize.Width + x) * 4;
                            var bgra = bitmap.Format == PixelFormat.Bgra8888;
                            return Color.FromArgb(bytes[offset + 3], bytes[offset + (bgra ? 2 : 0)], bytes[offset + 1],
                                bytes[offset + (bgra ? 0 : 2)]);
                        }

                        check(
                            Pixel(0) == ((ISolidColorBrush)menu.Background!).Color &&
                            Pixel(8) == ((ISolidColorBrush)surface.Background!).Color,
                            label +
                            $"/{width}: rendered selected background and outer inset pixels retain their theme colors");
                        if (output is not null)
                            bitmap.Save(Path.Combine(output, $"NavMenu-fling-{variant}-{size}-{width}.png"),
                                PngBitmapEncoderOptions.Default);
                    }
                }

                var header = (NavMenuItem)menu.ContainerFromIndex(1)!;
                var toggle = header.GetVisualDescendants().OfType<ToggleButton>()
                    .First(b => b.Name == "PART_ExpandCollapseChevron");
                var togglePoint =
                    toggle.TranslatePoint(new Point(toggle.Bounds.Width / 2, toggle.Bounds.Height / 2), window)!.Value;
                window.MouseDown(togglePoint, MouseButton.Left);
                window.MouseUp(togglePoint, MouseButton.Left);
                ThemeAppearancePreview.Flush();
                check(!window.Model.NavigationRoots[1].IsExpanded, label + ": pointer still collapses retained root");
                header.Focus();
                ThemeAppearancePreview.Press(window, Key.Right);
                check(window.Model.NavigationRoots[1].IsExpanded, label + ": native Right restores expansion");
            }
            finally
            {
                window.Close();
            }
        }
    }

    private static void VerifyBoundedRoots(Action<bool, string> check)
    {
        var roots = new ObservableCollection<NavEntry> { new("Large branch", isGroup: true) };
        for (var i = 0; i < 10000; i++) roots[0].Entries.Add(new NavEntry($"Page {i}"));
        var menu = new NavMenu
        {
            ItemsSource = roots,
            ChildrenBinding = CompiledBinding.Create<NavEntry, ObservableCollection<NavEntry>>(n => n.Entries),
            ExpandedBinding = CompiledBinding.Create<NavEntry, bool>(n => n.IsExpanded, mode: BindingMode.TwoWay)
        };
        var window = new Window { Width = 232, Height = 400, Content = menu };
        Motion.SetEnabled(window, false);
        window.Show();
        ThemeAppearancePreview.Flush();
        try
        {
            var root = (NavMenuItem)menu.ContainerFromIndex(0)!;
            check(menu.ItemsPanelRoot is StackPanel && root.ItemsPanelRoot is VirtualizingStackPanel,
                "NavMenu/fling/bounded: small root collection retains native child virtualization");
            check(menu.SelectPath(0, 9999), "NavMenu/fling/bounded: distant child navigation still succeeds");
            ThemeAppearancePreview.Flush();
            check(
                root.GetRealizedContainers().Count() < 80 && root.ContainerFromIndex(0) is null &&
                root.ContainerFromIndex(9999) is not null,
                "NavMenu/fling/bounded: 10,000 children only realize the viewport and bounded cache");
            for (var i = 1; i < 33; i++) roots.Add(new NavEntry($"Root {i}"));
            ThemeAppearancePreview.Flush();
            check(menu.ItemsPanelRoot is VirtualizingStackPanel,
                "NavMenu/fling/bounded: large root collection returns to native virtualization");
            while (roots.Count > 1) roots.RemoveAt(roots.Count - 1);
            ThemeAppearancePreview.Flush();
            check(menu.ItemsPanelRoot is StackPanel && menu.SelectPath(0, 0),
                "NavMenu/fling/bounded: removing roots restores stable branches and navigation");
            var custom = new FuncTemplate<Panel?>(() => new VirtualizingStackPanel { CacheLength = .5 });
            menu.ItemsPanel = custom;
            ThemeAppearancePreview.Flush();
            check(
                ReferenceEquals(menu.ItemsPanel, custom) &&
                menu.ItemsPanelRoot is VirtualizingStackPanel { CacheLength: .5 },
                "NavMenu/fling/bounded: explicit consumer ItemsPanel takes precedence");
        }
        finally
        {
            window.Close();
        }
    }
}