using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class TabDragPreview
{
    public static void VerifyEndDrop(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var directItems in new[] { true, false })
        foreach (var motion in new[] { true, false })
        foreach (var firstToEnd in new[] { true, false })
        foreach (var layout in new[] { "stretched", "auto", "auto-variable" })
        foreach (var farBlank in new[] { false, true })
            VerifyEndDropCase(directItems, motion, firstToEnd, layout, farBlank, check, output);
        foreach (var motion in new[] { true, false })
        foreach (var firstToEnd in new[] { true, false })
            VerifyEndDropCase(true, motion, firstToEnd, "auto-variable", true, check, output, true);
        VerifyOwnContainerMoves(check, output);
    }

    private static void VerifyEndDropCase(bool directItems, bool motion, bool firstToEnd, string layout, bool farBlank,
        Action<bool, string> check, string? output, bool ownItemSource = false)
    {
        var tabs = new TabControl { SelectedIndex = 0 };
        tabs.Classes.Add("card");
        TabFeatures.SetCanClose(tabs, true);
        TabFeatures.SetCanReorder(tabs, true);
        Motion.SetEnabled(tabs, motion);
        Virtualization.SetEnabled(tabs, true);
        var variableHeight = layout == "auto-variable";
        var models = new ObservableCollection<EndDropModel>
        {
            new("需求", "需求内容", variableHeight ? 64 : 100),
            new("开发", "开发内容", variableHeight ? 260 : 100),
            new("验收", "验收内容", variableHeight ? 140 : 100)
        };
        ObservableCollection<TabItem>? ownItems = null;
        if (directItems)
        {
            ownItems = new ObservableCollection<TabItem>(models.Select(model => new TabItem
            {
                Header = model.Title,
                Content = new TextBlock
                {
                    Name = "EndDropSelectedContent", Text = model.Content, MinHeight = model.ContentHeight,
                    Margin = new Thickness(20)
                }
            }));
            if (ownItemSource) tabs.ItemsSource = ownItems;
            else
                foreach (var item in ownItems)
                    tabs.Items.Add(item);
        }
        else
        {
            tabs.ItemsSource = models;
            tabs.ItemTemplate = new FuncDataTemplate<EndDropModel>((model, _) => new TextBlock { Text = model?.Title });
            tabs.ContentTemplate = new FuncDataTemplate<EndDropModel>((model, _) =>
                new TextBlock
                {
                    Name = "EndDropSelectedContent", Text = model?.Content, MinHeight = model?.ContentHeight ?? 0,
                    Margin = new Thickness(20)
                });
        }

        Control host;
        if (layout == "stretched") host = new Border { Padding = new Thickness(24), Child = tabs };
        else
            host = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                Content = new Border
                {
                    Padding = new Thickness(24), Child = new StackPanel
                    {
                        Spacing = 16, Children =
                        {
                            new TextBlock { Text = "TabControl 项目扩展", FontSize = 22 },
                            PreviewSection.Create("TabControl · TabFeatures 关闭与重排", new StackPanel
                            {
                                Spacing = 12, Children =
                                {
                                    new TextBlock
                                    {
                                        Text = "Classes=\"card\" · CanClose / CanReorder",
                                        TextWrapping = TextWrapping.Wrap
                                    },
                                    tabs
                                }
                            }),
                            new TextBlock { Text = "说明区域位于自动高度的控件之后。" }
                        }
                    }
                }
            };
        var window = new Window
        {
            Title = "Tab end-drop regression", Width = 860, Height = layout == "stretched" ? 420 : 720,
            RequestedThemeVariant = ThemeVariant.Light,
            Content = host
        };
        window.Show();
        ControlPreview.Flush();
        var label =
            $"TabDrag/end-drop/{(ownItemSource ? "ObservableTabItems" : directItems ? "TabItems" : "ItemsSource")}/motion-{motion}/{layout}/{(farBlank ? "far-blank" : "near-blank")}/{(firstToEnd ? "first-to-end-blank" : "last-to-own-end-blank")}";
        try
        {
            var before = tabs.Items.Cast<object>().ToArray();
            var sourceIndex = firstToEnd ? 0 : 2;
            var source = before[sourceIndex];
            tabs.SelectedIndex = sourceIndex;
            ControlPreview.Flush();
            var initialHeaders = tabs.GetRealizedContainers().OfType<TabItem>()
                .ToDictionary(item => item, item => item.RenderTransform);
            var sourceHeader = Item(tabs, sourceIndex);
            var scroll = HeaderScroll(tabs);
            var last = Item(tabs, 2);
            var lastRight = last.TranslatePoint(new Point(last.Bounds.Width, last.Bounds.Height / 2), window)!.Value;
            var viewportRight = scroll.TranslatePoint(new Point(scroll.Bounds.Width, scroll.Bounds.Height / 2), window)!
                .Value.X;
            var blank = new Point(farBlank ? viewportRight - 24 : Math.Min(lastRight.X + 72, viewportRight - 24),
                lastRight.Y);
            check(tabs.Items.Count == 3 && tabs.GetRealizedContainers().Count() == 3 &&
                  blank.X > lastRight.X + (farBlank ? 240 : 20) &&
                  Virtualization.GetEnabled(tabs) &&
                  (layout == "stretched" || tabs.Bounds.Height < window.Bounds.Height - 100),
                label +
                ": three real headers leave the requested trailing release area in the virtualized stretched or natural-height fixture");
            SaveEndDrop(window, output, label, "before");
            var start = Center(sourceHeader, window);
            Down(window, start);
            Move(window, start + new Vector(24, 0));
            check(Ghost(window) is not null && sourceHeader.Opacity == 0,
                label + ": real pointer movement starts a lifted card header drag");
            var trailing = last.TranslatePoint(new Point(last.Bounds.Width - 3, last.Bounds.Height / 2), window)!.Value;
            Move(window, trailing);
            if (motion) Pump(80);
            Move(window, blank);
            if (motion) Pump(160);
            Up(window, blank);
            Pump(240);
            SaveEndDrop(window, output, label, "after");

            var expected = firstToEnd ? new[] { before[1], before[2], before[0] } : before;
            var actual = tabs.Items.Cast<object>().ToArray();
            check(actual.Length == 3 && actual.Zip(expected).All(pair => ReferenceEquals(pair.First, pair.Second)),
                label + ": release keeps every item identity and commits only the requested final position");
            if (ownItemSource)
                check(
                    ReferenceEquals(tabs.ItemsSource, ownItems) && ownItems!.Count == 3 && ownItems.Zip(expected)
                        .All(pair => ReferenceEquals(pair.First, pair.Second)),
                    label + ": the explicit ObservableCollection<TabItem> source retains its identity and final order");
            check(tabs.GetRealizedContainers().Count() == 3 && Clean(window, tabs) &&
                  tabs.GetRealizedContainers().OfType<TabItem>().All(item => item.Opacity == 1 &&
                                                                             (!initialHeaders.TryGetValue(item,
                                                                                  out var transform) ||
                                                                              ReferenceEquals(item.RenderTransform,
                                                                                  transform))),
                label + ": every header restores opacity, its original transform and temporary state after release");
            var expectedContent =
                directItems ? ((TextBlock)((TabItem)source).Content!).Text : ((EndDropModel)source).Content;
            check(ReferenceEquals(tabs.SelectedItem, source) && tabs.SelectedIndex == 2 &&
                  tabs.GetVisualDescendants().OfType<TextBlock>().Any(text =>
                      text.Name == "EndDropSelectedContent" && text.Text == expectedContent &&
                      text.IsEffectivelyVisible && text.Bounds.Width > 0 && text.Bounds.Height > 0),
                label + ": the moved or unchanged selected item retains its visible original content");
            check(tabs.GetRealizedContainers().OfType<TabItem>().All(item => item.IsAttachedToVisualTree() &&
                                                                             item.GetVisualAncestors().Contains(tabs) &&
                                                                             item.Bounds.Width > 0 &&
                                                                             item.Bounds.Height > 0 &&
                                                                             item.GetVisualDescendants()
                                                                                 .OfType<TextBlock>().Any(text =>
                                                                                     text.Text is "需求" or "开发"
                                                                                         or "验收" &&
                                                                                     text.IsEffectivelyVisible &&
                                                                                     text.Bounds.Width > 0 &&
                                                                                     text.Bounds.Height > 0)),
                label + ": all three card titles remain present and visibly laid out after the end-area drop");
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }
    }

    private static void VerifyOwnContainerMoves(Action<bool, string> check, string? output)
    {
        foreach (var appearance in new[] { "card", "pills", "" })
        foreach (var placement in new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right })
            VerifyOwnContainerMoveCase(appearance, placement, ThemeVariant.Light, ControlSize.Normal, 860, check, null);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyOwnContainerMoveCase("card", Dock.Top, theme, size, 360, check, output);
    }

    private static void VerifyOwnContainerMoveCase(string appearance, Dock placement, ThemeVariant theme,
        ControlSize size, int width,
        Action<bool, string> check, string? output)
    {
        var original = new[]
        {
            new TabItem { Header = "需求", Content = new TextBlock { Text = "需求内容", Margin = new Thickness(16) } },
            new TabItem { Header = "开发", Content = new TextBlock { Text = "开发内容", Margin = new Thickness(16) } },
            new TabItem { Header = "验收", Content = new TextBlock { Text = "验收内容", Margin = new Thickness(16) } }
        };
        var tabs = new TabControl { SelectedIndex = 0, TabStripPlacement = placement };
        if (appearance.Length > 0) tabs.Classes.Add(appearance);
        foreach (var item in original) tabs.Items.Add(item);
        TabFeatures.SetCanClose(tabs, true);
        TabFeatures.SetCanReorder(tabs, true);
        Virtualization.SetEnabled(tabs, true);
        Motion.SetEnabled(tabs, false);
        var window = new Window
        {
            Width = width, Height = 420, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = tabs }
        };
        ControlSizing.SetSize(window, size);
        window.Show();
        ControlPreview.Flush();
        var label =
            $"TabDrag/own-container/{(appearance.Length > 0 ? appearance : "default")}/{placement}/{theme.Key}/{size}/{width}";
        try
        {
            if (output is not null && size == ControlSize.Normal) SaveEndDrop(window, output, label, "before");
            check(TabFeatures.Move(tabs, 0, 2), label + ": the public Move API accepts an own-container reorder");
            ControlPreview.Flush();
            check(ReferenceEquals(tabs.Items[2], original[0]) && ReferenceEquals(tabs.SelectedItem, original[0]) &&
                  tabs.SelectedIndex == 2 &&
                  MountedOwnHeaders(tabs) && OwnContentVisible(original[0], window),
                label + ": Move remounts every original header and preserves the selected content");
            Item(tabs, 2).Focus();
            ControlPreview.Flush();
            window.KeyPress(Key.Left, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.ArrowLeft, null);
            window.KeyRelease(Key.Left, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.ArrowLeft,
                null);
            ControlPreview.Flush();
            check(ReferenceEquals(tabs.Items[0], original[1]) && ReferenceEquals(tabs.Items[1], original[0]) &&
                  ReferenceEquals(tabs.Items[2], original[2]) &&
                  ReferenceEquals(tabs.SelectedItem, original[0]) && tabs.SelectedIndex == 1 &&
                  MountedOwnHeaders(tabs) && OwnContentVisible(original[0], window),
                label +
                ": real Ctrl+Shift+Left restores order, visual attachment and selection after a preceding Move");
            if (width == 360)
            {
                using var frame = window.CaptureRenderedFrame();
                check(
                    frame is not null && PreviewRenderedPixels.UniqueColors(frame) > 24 && tabs.GetRealizedContainers()
                        .OfType<TabItem>().All(item => ControlSizing.GetSize(item) == size),
                    label + ": real narrow themed headers render in the inherited density after both reorder paths");
                if (output is not null && size == ControlSize.Normal) SaveEndDrop(window, output, label, "after");
            }

            if (appearance == "card")
            {
                var close = Item(tabs, 1).GetVisualDescendants().OfType<Button>()
                    .Single(button => button.Name == "PART_CloseButton");
                var point = Center(close, window);
                Down(window, point);
                Up(window, point);
                check(tabs.Items.Count == 2 && !tabs.Items.Contains(original[0]) &&
                      ReferenceEquals(tabs.Items[0], original[1]) && ReferenceEquals(tabs.Items[1], original[2]) &&
                      MountedOwnHeaders(tabs) && tabs.SelectedItem is TabItem selected &&
                      OwnContentVisible(selected, window),
                    label + ": the real close button remains usable on a reordered own card header");
            }
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }
    }

    private static bool MountedOwnHeaders(TabControl tabs)
    {
        return tabs.GetRealizedContainers().Count() == tabs.Items.Count &&
               tabs.Items.Cast<TabItem>().All(item =>
                   ReferenceEquals(tabs.ContainerFromIndex(tabs.Items.IndexOf(item)), item) &&
                   item.IsAttachedToVisualTree() && item.GetVisualAncestors().Contains(tabs) && item.Opacity == 1 &&
                   (item.RenderTransform is null || item.RenderTransform.Value == Matrix.Identity) &&
                   item.Bounds.Width > 0 && item.Bounds.Height > 0);
    }

    private static bool OwnContentVisible(TabItem item, Window window)
    {
        return item.Content is Control content && content.IsAttachedToVisualTree() &&
               ReferenceEquals(TopLevel.GetTopLevel(content), window) && content.IsEffectivelyVisible &&
               content.Bounds.Width > 0 && content.Bounds.Height > 0;
    }

    private static void SaveEndDrop(Window window, string? output, string label, string phase)
    {
        if (output is null) return;
        using var frame = window.CaptureRenderedFrame();
        frame?.Save(Path.Combine(output, label.Replace('/', '-') + "-" + phase + ".png"),
            PngBitmapEncoderOptions.Default);
    }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, tabs, items) = Open(theme, size);
            var label = $"TabDrag/{theme.Key}/{size}";
            var original = items[0];
            var first = Item(tabs, 0);
            var initial = first.RenderTransform;
            var start = Center(first, window);
            IPointer? pointer = null;

            void Observe(object? sender, PointerEventArgs args)
            {
                pointer = args.Pointer;
            }

            tabs.AddHandler(InputElement.PointerMovedEvent, Observe, RoutingStrategies.Tunnel, true);
            Down(window, start);
            Move(window, start + new Vector(3, 0));
            check(Ghost(window) is null && items[0] == original, label + ": click jitter does not initiate a drag");
            window.MouseMove(start + new Vector(24, 0));
            ControlPreview.Flush();
            check(
                Ghost(window) is { } ghost && Math.Abs(((TranslateTransform)ghost.RenderTransform!).X - 24) < 1 &&
                first.Opacity == 0,
                label + ": lifted header follows the pointer one-to-one without duplicating the source");
            check(pointer?.Captured == tabs && tabs.SelectedItem?.Equals(original) == true,
                label + ": native/coalesced moves without repeated button flags retain capture and selection");
            var second = Item(tabs, 1);
            var third = Item(tabs, 2);
            var target = Center(third, window) + new Vector(9, 0);
            Move(window, target);
            Pump(38);
            var shift = second.RenderTransform!.Value.M31;
            check(shift < -1 && shift > -first.Bounds.Width - 1,
                label + ": neighbours ease into the insertion gap while collection stays unchanged");
            check(items[0] == original && items.Count == 1000,
                label + ": preview does not repeatedly mutate the model collection");
            for (var i = 0; i < 6; i++) Move(window, target);
            Pump(150);
            check(
                third.Classes.Contains("drop-target") &&
                Math.Abs(second.RenderTransform!.Value.M31 + first.Bounds.Width) < 1,
                label + ": stationary pointer keeps a stable insertion gap without oscillation");
            Up(window, target);
            check(items[2] == original && tabs.SelectedItem?.Equals(original) == true && pointer?.Captured is null,
                label + ": release commits one reorder and preserves the selected model");
            check(Ghost(window) is not null, label + ": release keeps a short settling preview instead of jumping");
            Pump(200);
            check(Clean(window, tabs) && ReferenceEquals(first.RenderTransform, initial),
                label + ": settling restores transforms, opacity and temporary visuals");
            check(tabs.GetRealizedContainers().Count() < 40,
                label + ": thousand-item headers remain virtualized after reorder");

            var narrowHeader = new Style(selector => selector.OfType<TabItem>())
                { Setters = { new Setter(Control.WidthProperty, 100d) } };
            var wideSelected = new Style(selector => selector.OfType<TabItem>().Class(":selected"))
                { Setters = { new Setter(Control.WidthProperty, 180d) } };
            tabs.Styles.Add(narrowHeader);
            tabs.Styles.Add(wideSelected);
            ControlPreview.Flush();
            check(Item(tabs, 2).Bounds.Width > Item(tabs, 0).Bounds.Width + 40,
                label + ": regression uses a selected header wider than the first target");
            for (var round = 0; round < 3; round++)
            {
                start = Center(Item(tabs, items.IndexOf(original)), window);
                Down(window, start);
                target = Item(tabs, 0).TranslatePoint(new Point(3, first.Bounds.Height / 2), window)!.Value;
                Move(window, target);
                Up(window, target);
                Pump(180);
                check(items[0] == original && tabs.SelectedIndex == 0 && Clean(window, tabs),
                    label + $": consecutive drag {round + 1} returns the wider selected header to first position");
                start = Center(Item(tabs, 0), window);
                Down(window, start);
                target = Center(Item(tabs, 2), window) + new Vector(9, 0);
                Move(window, target);
                Up(window, target);
                check(items[2] == original, label + $": consecutive drag {round + 1} moves away again");
                // The next down must also clean up an unfinished settling frame.
            }

            Pump(180);
            tabs.Styles.Remove(wideSelected);
            tabs.Styles.Remove(narrowHeader);
            ControlPreview.Flush();

            first = Item(tabs, 0);
            first.Opacity = 0.8;
            var custom = new TranslateTransform(1, 0);
            first.RenderTransform = custom;
            var before = items.ToArray();
            start = Center(first, window);
            Down(window, start);
            Move(window, Center(Item(tabs, 2), window));
            Press(window, Key.Escape, PhysicalKey.Escape);
            Pump(180);
            check(
                items.SequenceEqual(before) && Clean(window, tabs, first) && first.Opacity == 0.8 &&
                ReferenceEquals(first.RenderTransform, custom) && pointer?.Captured is null,
                label + ": Escape restores consumer opacity/transform and cannot commit later");
            first.ClearValue(Visual.OpacityProperty);
            first.ClearValue(Visual.RenderTransformProperty);

            start = Center(Item(tabs, 0), window);
            Down(window, start);
            Move(window, start + new Vector(48, 0));
            pointer!.Capture(null);
            Pump(180);
            check(Clean(window, tabs) && items.SequenceEqual(before),
                label + ": lost capture cancels with no late mutation");
            Up(window, start);

            start = Center(Item(tabs, 0), window);
            Down(window, start);
            Move(window, Center(Item(tabs, 2), window));
            TabFeatures.SetCanReorder(tabs, false);
            Up(window, start);
            Pump(180);
            check(Clean(window, tabs) && items.SequenceEqual(before),
                label + ": disabling reorder cancels an active gesture");
            TabFeatures.SetCanReorder(tabs, true);

            start = Center(Item(tabs, 0), window);
            Down(window, start);
            Move(window, Center(Item(tabs, 2), window));
            items.RemoveAt(7);
            var externallyChanged = items.ToArray();
            Up(window, start);
            Pump(180);
            check(Clean(window, tabs) && items.SequenceEqual(externallyChanged),
                label + ": external collection edits cancel the stale insertion target");

            start = Center(Item(tabs, 0), window);
            original = items[0];
            Down(window, start);
            target = Center(Item(tabs, 2), window) + new Vector(9, 0);
            Move(window, target);
            Motion.SetEnabled(tabs, false);
            ControlPreview.Flush();
            check(Math.Abs(Item(tabs, 1).RenderTransform!.Value.M31 + Item(tabs, 0).Bounds.Width) < 1,
                label + ": reduced motion uses the same live gap without interpolation");
            Up(window, target);
            check(items[2] == original && Clean(window, tabs),
                label + ": reduced motion commits and cleans up immediately");
            Motion.SetEnabled(tabs, true);

            var outsideBefore = items.ToArray();
            start = Center(Item(tabs, 0), window);
            Down(window, start);
            Move(window, start + new Vector(45, 0));
            Up(window, start + new Vector(45, 100));
            Pump(160);
            check(Clean(window, tabs) && items.SequenceEqual(outsideBefore),
                label + ": release outside the header cancels instead of committing a stale target");

            var scroll = HeaderScroll(tabs);
            tabs.SelectedIndex = 0;
            tabs.ScrollIntoView(0);
            ControlPreview.Flush();
            start = Center(Item(tabs, 0), window);
            Down(window, start);
            var edge = scroll.TranslatePoint(new Point(scroll.Bounds.Width - 2, scroll.Bounds.Height / 2), window)!
                .Value;
            Move(window, edge);
            Pump(650);
            check(scroll.Offset.X > 180 && Ghost(window) is not null && tabs.GetRealizedContainers().Count() < 40,
                label + ": stationary edge drag scrolls and recycles without losing the floating header");
            check(
                tabs.GetRealizedContainers().OfType<TabItem>().All(item =>
                    tabs.IndexFromContainer(item) == 0 || (item.Opacity == 1 && !item.Classes.Contains("dragging"))),
                label + ": recycled containers do not inherit hidden source state");
            Press(window, Key.Escape, PhysicalKey.Escape);
            var offset = scroll.Offset.X;
            Pump(120);
            check(Clean(window, tabs) && Math.Abs(scroll.Offset.X - offset) < 1,
                label + ": cancel stops edge scrolling and removes the independent drag image");

            tabs.SelectedIndex = items.Count - 1;
            tabs.ScrollIntoView(tabs.SelectedIndex);
            ControlPreview.Flush();
            scroll.Offset = new Vector(Math.Max(0, scroll.Extent.Width - scroll.Viewport.Width), 0);
            ControlPreview.Flush();
            // The native panel estimates unmeasured widths when jumping far. Pick an actual
            // visible distant header, then interact through its measured screen coordinates.
            var lastIndex = tabs.GetRealizedContainers().OfType<TabItem>().Select(tabs.IndexFromContainer).Order()
                .Skip(3).First();
            tabs.SelectedIndex = lastIndex;
            ControlPreview.Flush();
            check(lastIndex > 900, label + ": far-scroll verification uses a realized model beyond index 900");
            var distant = items[lastIndex];
            start = Center(Item(tabs, lastIndex), window);
            Down(window, start);
            target = Center(Item(tabs, lastIndex - 1), window) - new Vector(9, 0);
            Move(window, target);
            Up(window, target);
            Pump(200);
            check(
                items[lastIndex - 1] == distant && tabs.SelectedIndex == lastIndex - 1 &&
                tabs.GetRealizedContainers().Count() < 40 && Clean(window, tabs),
                label + ": distant headers reorder and retain selection after recycling");

            start = Center(Item(tabs, lastIndex - 1), window);
            Down(window, start);
            Move(window, start - new Vector(35, 0));
            window.Content = null;
            Up(window, start);
            Pump(180);
            check(
                Ghost(window) is null && tabs.GetRealizedContainers().OfType<TabItem>()
                    .All(item => item.Opacity == 1 && !item.Classes.Contains("dragging")),
                label + ": unloading cancels timers, capture and temporary header state");
            tabs.RemoveHandler(InputElement.PointerMovedEvent, Observe);
            window.Close();
            ControlPreview.Flush();
        }

        VerifyOtherHeaders(check);
        VerifyScaling(check);
    }

    private static void VerifyScaling(Action<bool, string> check)
    {
        foreach (var scale in new[] { 1d, 1.25, 1.5, 2, 3 })
        {
            var (window, tabs, items) = Open(ThemeVariant.Light, ControlSize.Normal);
            window.SetRenderScaling(scale);
            ControlPreview.Flush();
            var original = items[0];
            for (var round = 0; round < 3; round++)
            {
                var source = Item(tabs, items.IndexOf(original));
                var start = Center(source, window);
                Down(window, start);
                var pixels = new PixelSize((int)Math.Ceiling(860 * scale), (int)Math.Ceiling(420 * scale));
                var dpi = new Vector(96 * scale, 96 * scale);
                using var reference = new RenderTargetBitmap(pixels, dpi);
                reference.Render(window);
                var origin = source.TranslatePoint(default, window)!.Value;
                Move(window, start + new Vector(24, 0));
                using var dragged = new RenderTargetBitmap(pixels, dpi);
                dragged.Render(window);
                var matching = 0;
                var count = 0;
                for (var y = (int)(6 * scale); y < (source.Bounds.Height - 6) * scale; y++)
                for (var x = (int)(15 * scale); x < (source.Bounds.Width - 15) * scale; x++)
                {
                    var a = Pixel(reference, (int)Math.Round(origin.X * scale) + x,
                        (int)Math.Round(origin.Y * scale) + y);
                    var b = Pixel(dragged, (int)Math.Round(origin.X * scale) + (int)Math.Round(24 * scale) + x,
                        (int)Math.Round(origin.Y * scale) + y);
                    if (Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B) < 36) matching++;
                    count++;
                }

                check((double)matching / count > .97,
                    $"TabDrag/{scale}x/{round + 1}: repeated drag retains original text scale and complete image ({matching}/{count} pixels match)");
                var destination = round % 2 == 0 ? 2 : 0;
                var target = destination == 0
                    ? Item(tabs, 0).TranslatePoint(new Point(3, source.Bounds.Height / 2), window)!.Value
                    : Center(Item(tabs, 2), window) + new Vector(9, 0);
                Move(window, target);
                Up(window, target);
                Pump(180);
                check(items[destination] == original && tabs.SelectedIndex == destination && Clean(window, tabs),
                    $"TabDrag/{scale}x/{round + 1}: scaled drag commits the requested first/third position and clears the preview");
            }

            window.Close();
            ControlPreview.Flush();
        }
    }

    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
    {
        var bytes = new byte[4];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), handle.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            handle.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromRgb(bytes[0], bytes[1], bytes[2])
            : Color.FromRgb(bytes[2], bytes[1], bytes[0]);
    }

    private static void VerifyOtherHeaders(Action<bool, string> check)
    {
        foreach (var placement in new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right })
        foreach (var appearance in new[] { "card", "pills", "" })
        {
            var (window, tabs, items) = Open(ThemeVariant.Light, ControlSize.Normal, appearance, placement);
            Motion.SetEnabled(tabs, false);
            var source = items[0];
            var first = Item(tabs, 0);
            var start = Center(first, window);
            Down(window, start);
            var target = Center(Item(tabs, 2), window) +
                         (placement is Dock.Left or Dock.Right ? new Vector(0, 8) : new Vector(8, 0));
            Move(window, target);
            Up(window, target);
            check(items[2] == source && tabs.SelectedItem?.Equals(source) == true && Clean(window, tabs),
                $"TabDrag/{appearance}/{placement}: native header placement supports pointer reorder");
            var selected = Item(tabs, 2);
            selected.Focus();
            window.KeyPress(Key.Left, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.ArrowLeft, null);
            window.KeyRelease(Key.Left, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.ArrowLeft,
                null);
            ControlPreview.Flush();
            check(items[1] == source && tabs.SelectedIndex == 1,
                $"TabDrag/{appearance}/{placement}: native keyboard reorder remains available");
            if (appearance == "card")
            {
                selected = Item(tabs, 1);
                var close = selected.GetVisualDescendants().OfType<Button>()
                    .Single(button => button.Name == "PART_CloseButton");
                var point = Center(close, window);
                Down(window, point);
                Up(window, point);
                check(!items.Contains(source) && Ghost(window) is null,
                    $"TabDrag/{appearance}/{placement}: close button never starts a header drag");
            }

            window.Close();
            ControlPreview.Flush();
        }

        var (fixedWindow, fixedTabs, _) = Open(ThemeVariant.Light, ControlSize.Normal);
        fixedTabs.ItemsSource = new[] { "Fixed A", "Fixed B", "Fixed C" };
        ControlPreview.Flush();
        var a = Center(Item(fixedTabs, 0), fixedWindow);
        Down(fixedWindow, a);
        Move(fixedWindow, Center(Item(fixedTabs, 2), fixedWindow));
        Up(fixedWindow, a);
        check(Ghost(fixedWindow) is null && fixedTabs.Items[0]?.Equals("Fixed A") == true,
            "TabDrag/fixed: immutable sources do not start a drag");
        fixedWindow.Close();
        ControlPreview.Flush();
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var scale in new[] { 1d, 2d })
        {
            var (window, tabs, _) = Open(theme, size);
            window.SetRenderScaling(scale);
            ControlPreview.Flush();
            Save("before");
            var start = Center(Item(tabs, 0), window);
            Down(window, start);
            var target = Center(Item(tabs, 2), window) + new Vector(14, 0);
            Move(window, target);
            Pump(160);
            Save("drag");
            Up(window, target);
            Pump(200);
            Save("after");
            window.Close();
            ControlPreview.Flush();

            void Save(string phase)
            {
                using var bitmap = new RenderTargetBitmap(new PixelSize((int)(860 * scale), (int)(420 * scale)),
                    new Vector(96 * scale, 96 * scale));
                bitmap.Render(window);
                bitmap.Save(Path.Combine(output, $"tab-drag-{theme.Key}-{size}-{scale}x-{phase}.png"),
                    PngBitmapEncoderOptions.Default);
            }
        }
    }

    private static (Window Window, TabControl Tabs, ObservableCollection<string> Items) Open(ThemeVariant theme,
        ControlSize size, string appearance = "card", Dock placement = Dock.Top)
    {
        var items = new ObservableCollection<string>(Enumerable.Range(1, 1000).Select(index => $"工作区 {index:0000}"));
        items[4] = "不同宽度的工作区";
        var tabs = new TabControl
        {
            ItemsSource = items, SelectedIndex = 0, TabStripPlacement = placement,
            ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text }),
            ContentTemplate = new FuncDataTemplate<string>((text, _) => new StackPanel
            {
                Margin = new Thickness(24), Spacing = 12,
                Children =
                {
                    new TextBlock { Text = text, FontSize = 22 }, new TextBlock { Text = "拖动标题排序 · 相邻标题平滑让位 · Esc 取消" }
                }
            })
        };
        if (appearance.Length > 0) tabs.Classes.Add(appearance);
        TabFeatures.SetCanReorder(tabs, true);
        var window = new Window
        {
            Title = "Tab drag verification", Width = 860, Height = 420, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = tabs }
        };
        ControlSizing.SetSize(window, size);
        window.Show();
        ControlPreview.Flush();
        return (window, tabs, items);
    }

    private static TabItem Item(TabControl tabs, int index)
    {
        return (TabItem)(tabs.ContainerFromIndex(index) ??
                         throw new InvalidOperationException($"Header {index} is not realized"));
    }

    private static ScrollViewer HeaderScroll(TabControl tabs)
    {
        return Item(tabs, tabs.SelectedIndex).GetVisualAncestors().OfType<ScrollViewer>().First();
    }

    private static Control? Ghost(Window window)
    {
        return window.GetVisualDescendants().OfType<Control>()
            .SingleOrDefault(control => control.Name == "PART_TabDragPreview");
    }

    private static bool Clean(Window window, TabControl tabs, TabItem? custom = null)
    {
        return Ghost(window) is null && tabs.GetRealizedContainers().OfType<TabItem>().All(item =>
            !item.Classes.Contains("dragging") && !item.Classes.Contains("drop-target") &&
            (item == custom || item.Opacity == 1));
    }

    private static Point Center(Control control, Window window)
    {
        return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
    }

    private static void Down(Window window, Point point)
    {
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static void Move(Window window, Point point)
    {
        window.MouseMove(point, RawInputModifiers.LeftMouseButton);
        ControlPreview.Flush();
    }

    private static void Up(Window window, Point point)
    {
        window.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static void Press(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        ControlPreview.Flush();
    }

    private static void Pump(int milliseconds)
    {
        var frame = new DispatcherFrame();
        DispatcherTimer.RunOnce(() => frame.Continue = false, TimeSpan.FromMilliseconds(milliseconds));
        Dispatcher.UIThread.PushFrame(frame);
        ControlPreview.Flush();
    }

    private sealed record EndDropModel(string Title, string Content, double ContentHeight);
}