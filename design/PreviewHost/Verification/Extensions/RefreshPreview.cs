using Avalonia;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Rendering.Composition;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class RefreshPreview
{
    private static (Window Window, RefreshDemoView Demo) Open(ThemeVariant theme, ControlSize size, int width = 1040)
    {
        var demo = new RefreshDemoView(); demo.FindControl<CheckBox>("ManualComplete")!.IsChecked = true;
        var window = new Window { Width = width, Height = 980, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo } };
        ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush(); return (window, demo);
    }
    public static void Verify()
    {
        VerifyMotionLifecycle();
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) { VerifyMotion(theme, size); VerifyNative(theme, size); }
        VerifyDirections(); VerifyCustomization(); VerifyDetach();
    }
    private static void VerifyNative(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size); var refresh = demo.FindControl<RefreshContainer>("RefreshSample")!;
        var list = demo.FindControl<ListBox>("RefreshList")!; var visualizer = refresh.Visualizer!;
        var label = $"Refresh/{theme.Key}/{size}";
        var height = size switch { ControlSize.Small => 64, ControlSize.Large => 96, _ => 80 };
        var iconSize = size switch { ControlSize.Small => 18, ControlSize.Large => 26, _ => 22 };
        Check(refresh.GetType() == typeof(RefreshContainer) && visualizer.GetType() == typeof(RefreshVisualizer), $"{label}: retains official container and visualizer types");
        Check(refresh.FontSize == (size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 }) && Math.Abs(visualizer.Bounds.Height - height) < 0.5, $"{label}: inherited density overrides the native fixed default dimension");
        var icon = visualizer.GetVisualDescendants().OfType<PathIcon>().Single(control => control.Name == "PART_Icon");
        Check(icon.Bounds.Width == iconSize && icon.Bounds.Height == iconSize && !visualizer.IsHitTestVisible && !visualizer.IsTabStop, $"{label}: three-size native animated icon does not intercept input or keyboard focus");
        Check(Colour(refresh.Background) == Resource(refresh, "One.Surface") && Colour(visualizer.Background) == Resource(visualizer, "One.AccentSoft") && Colour(visualizer.Foreground) == Resource(visualizer, "One.Accent"), $"{label}: TheOne surface, refresh band and semantic accent");
        Check(State(visualizer) == RefreshVisualizerState.Idle && Part<Grid>(refresh, "PART_RefreshVisualizerPresenter").Children.Contains(visualizer), $"{label}: official template host starts in Idle");
        Bounded(list, label);
        var firstRow = list.GetRealizedContainers().First();
        var cells = firstRow.GetVisualDescendants().OfType<Grid>().Single(grid => grid.ColumnDefinitions.Count == 3).Children.OfType<TextBlock>().ToArray();
        var positions = cells.Select(cell => cell.TranslatePoint(default, firstRow)!.Value.X).ToArray();
        Check(cells.Length == 3 && positions[0] + cells[0].Bounds.Width < positions[1] && positions[1] + cells[1].Bounds.Width < positions[2], $"{label}: row number, title and revision occupy separate non-overlapping columns");
        var shortPull = Hold(window, refresh, height * 0.3); ControlPreview.Flush();
        Check(State(visualizer) == RefreshVisualizerState.Interacting && demo.RequestCount == 0, $"{label}: real short pull enters Interacting without requesting data");
        Release(window, shortPull); Check(State(visualizer) == RefreshVisualizerState.Idle && demo.RequestCount == 0, $"{label}: releasing below the threshold cancels refresh");
        var pending = Hold(window, refresh, height * 1.2);
        Check(State(visualizer) == RefreshVisualizerState.Pending && demo.FindControl<TextBlock>("RefreshStateLabel")!.Text == "释放以刷新", $"{label}: real over-threshold pull enters Pending and updates release guidance");
        Check(ElementComposition.GetElementVisual(visualizer) is { Offset.Y: > 0 }, $"{label}: native compositor exposes the refresh indicator during the pull");
        Release(window, pending);
        Check(State(visualizer) == RefreshVisualizerState.Refreshing && demo.IsRefreshing && demo.RequestCount == 1 && !demo.FindControl<Button>("RequestRefresh")!.IsEnabled, $"{label}: release requests once and retains the native async deferral");
        refresh.RequestRefresh(); ControlPreview.Flush();
        Check(demo.RequestCount == 1 && demo.IsRefreshing && State(visualizer) == RefreshVisualizerState.Refreshing, $"{label}: repeated programmatic requests share the pending demo operation");
        Click(window, demo.FindControl<Button>("CompleteRefresh")!);
        Check(demo.Revision == 1 && !demo.IsRefreshing && State(visualizer) == RefreshVisualizerState.Idle && demo.Rows.Count == 1000 && demo.Rows.All(row => row.Revision == 1), $"{label}: completing all deferrals refreshes a thousand models and returns to Idle");
        Bounded(list, label + "/updated");
        Click(window, demo.FindControl<Button>("LastRefreshRow")!);
        Check(list.SelectedIndex == 999 && list.ContainerFromIndex(999) is not null && ((RefreshRow)list.SelectedItem!).Number == 1000, $"{label}: distant selection materializes the thousandth data model");
        Bounded(list, label + "/last");
        var scroll = list.GetVisualDescendants().OfType<ScrollViewer>().First(); var offset = scroll.Offset.Y;
        var count = demo.RequestCount; var farPull = Hold(window, refresh, height * 1.2); Release(window, farPull);
        Check(demo.RequestCount == count && !demo.IsRefreshing, $"{label}: pulling in the middle of the scroll range does not request refresh");
        var selectedBeforeRefresh = ((RefreshRow)list.SelectedItem!).Number; offset = scroll.Offset.Y;
        Click(window, demo.FindControl<Button>("RequestRefresh")!); Click(window, demo.FindControl<Button>("CompleteRefresh")!);
        Check(demo.Revision == 2 && ((RefreshRow)list.SelectedItem!).Number == selectedBeforeRefresh && Math.Abs(scroll.Offset.Y - offset) < 1, $"{label}: programmatic refresh preserves distant selection and scroll position");
        Check(list.ContainerFromIndex(999)!.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Name == "RefreshRowTitle" && text.Text == demo.Rows[999].Title), $"{label}: recycled row template uses the refreshed current model");
        Click(window, demo.FindControl<Button>("FirstRefreshRow")!); ControlPreview.Flush();
        Click(window, demo.FindControl<CheckBox>("MousePull")!); count = demo.RequestCount;
        var disabledPull = Hold(window, refresh, height * 1.2); Release(window, disabledPull);
        Check(demo.RequestCount == count && State(visualizer) == RefreshVisualizerState.Idle, $"{label}: native IsMouseEnabled=False rejects desktop pull input");
        Click(window, demo.FindControl<CheckBox>("MousePull")!);
        Click(window, demo.FindControl<CheckBox>("EnableRefresh")!);
        var blocked = Hold(window, refresh, height * 1.2); Release(window, blocked);
        Check(demo.RequestCount == count && !refresh.IsEffectivelyEnabled && !list.IsEffectivelyEnabled && !demo.FindControl<Button>("RequestRefresh")!.IsEnabled, $"{label}: disabled refresh region blocks gestures and manual requests");
        Check(Colour(refresh.Foreground) == Resource(refresh, "One.DisabledText"), $"{label}: disabled foreground uses the shared semantic color");
        Click(window, demo.FindControl<CheckBox>("EnableRefresh")!);
        demo.FindControl<CheckBox>("ManualComplete")!.IsChecked = false;
        Click(window, demo.FindControl<CheckBox>("FailNextRefresh")!); Click(window, demo.FindControl<Button>("RequestRefresh")!);
        EnhancementsPreview.AdvanceUntil(() => !demo.IsRefreshing, 1600);
        Check(demo.FailureCount == 1 && demo.Revision == 2 && State(visualizer) == RefreshVisualizerState.Idle && demo.FindControl<TextBlock>("RefreshStatus")!.Text!.Contains("失败"), $"{label}: simulated async failure preserves data and completes the native deferral");
        Click(window, demo.FindControl<Button>("RequestRefresh")!); EnhancementsPreview.AdvanceUntil(() => !demo.IsRefreshing, 1600);
        Check(demo.Revision == 3 && demo.FailureCount == 1 && State(visualizer) == RefreshVisualizerState.Idle, $"{label}: retry automatically completes and restores idle interaction");
        demo.FindControl<CheckBox>("ManualComplete")!.IsChecked = true; Click(window, demo.FindControl<Button>("RequestRefresh")!);
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); ControlPreview.Flush();
        Check(State(visualizer) == RefreshVisualizerState.Refreshing && visualizer.Bounds.Height == (size == ControlSize.Large ? 64 : 96) && Colour(visualizer.Foreground) == Resource(visualizer, "One.Accent"), $"{label}: live theme and density changes keep the pending request and update the native indicator");
        Click(window, demo.FindControl<Button>("CompleteRefresh")!);
        window.Width = 680; ControlPreview.Flush(); Bounded(list, label + "/compact");
        Check(refresh.Bounds.Width <= 632 && refresh.Bounds.Height == 400 && demo.FindControl<TextBlock>("RefreshMetrics")!.Text!.Contains("1,000"), $"{label}: compact preview maintains a bounded viewport and reports actual realized rows");
        Click(window, demo.FindControl<Button>("ResetRefreshDemo")!);
        Check(demo.Revision == 0 && demo.RequestCount == 0 && !demo.IsRefreshing && list.ItemCount == 1000 && State(visualizer) == RefreshVisualizerState.Idle, $"{label}: reset releases pending work and restores the data preview");
        window.Close();
    }
    private static void VerifyMotion(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var refresh = demo.FindControl<RefreshContainer>("RefreshSample")!;
        var visualizer = refresh.Visualizer!; var list = demo.FindControl<ListBox>("RefreshList")!;
        var toggle = demo.FindControl<CheckBox>("RefreshMotionEnabled")!;
        var glyph = (Control)visualizer.Content!;
        var composition = ElementComposition.GetElementVisual(glyph)!;
        var label = $"Refresh motion/{theme.Key}/{size}";
        Motion.SetEnabled(window, false); ControlPreview.Flush();
        Check(!Motion.GetEnabled(visualizer) && toggle.IsChecked == false && composition.ImplicitAnimations is null, $"{label}: inherited Motion.Enabled disables native glyph interpolation and synchronizes the preview switch");
        var shortPull = Hold(window, refresh, visualizer.Bounds.Height * .3);
        Check(State(visualizer) == RefreshVisualizerState.Interacting && demo.FindControl<TextBlock>("RefreshStateLabel")!.Text == "继续下拉", $"{label}: motion off retains short-pull input and guidance");
        Release(window, shortPull); Check(demo.RequestCount == 0 && State(visualizer) == RefreshVisualizerState.Idle, $"{label}: motion off preserves cancel below threshold");
        var held = Hold(window, refresh, visualizer.Bounds.Height * 1.2);
        Check(State(visualizer) == RefreshVisualizerState.Pending && composition.Scale == new Vector3D(1, 1, 1) && composition.RotationAngle == 0 && demo.FindControl<TextBlock>("RefreshStateLabel")!.Text == "释放以刷新", $"{label}: motion off suppresses pending pulse and rotation but retains release guidance");
        Check(ElementComposition.GetElementVisual(visualizer)!.Offset.Y > 0 && ElementComposition.GetElementVisual(visualizer)!.ImplicitAnimations is null, $"{label}: native pull offset follows input without an animated container transition");
        Release(window, held); EnhancementsPreview.Advance(180);
        Check(demo.IsRefreshing && demo.RequestCount == 1 && demo.FindControl<TextBlock>("RefreshStateLabel")!.Text == "正在刷新" && !demo.FindControl<Button>("RequestRefresh")!.IsEnabled, $"{label}: static indicator retains refresh status, deferral and duplicate-request guard");
        var still = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(120);
        Check(still.SequenceEqual(IndicatorPixels(window, refresh)), $"{label}: actual Skia indicator frames remain still with motion off");
        Click(window, toggle); EnhancementsPreview.Advance(60);
        Check(Motion.GetEnabled(visualizer) && composition.ImplicitAnimations is not null && demo.RequestCount == 1 && demo.Revision == 0 && demo.IsRefreshing, $"{label}: local motion override restores animation without a new refresh request");
        var spinning = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(120);
        Check(!spinning.SequenceEqual(IndicatorPixels(window, refresh)), $"{label}: actual Skia rotation resumes during the same pending refresh");
        for (var iteration = 0; iteration < 3; iteration++)
        {
            Click(window, toggle); EnhancementsPreview.Advance(30);
            var frozen = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(75);
            Check(frozen.SequenceEqual(IndicatorPixels(window, refresh)) && demo.RequestCount == 1 && demo.IsRefreshing, $"{label}: repeated switch {iteration + 1} stops the current compositor animation immediately");
            Click(window, toggle); EnhancementsPreview.Advance(30);
        }
        Click(window, toggle); // Complete with motion disabled, then start another pull.
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); ControlPreview.Flush();
        var changedTheOne = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(100);
        Check(changedTheOne.SequenceEqual(IndicatorPixels(window, refresh)) && ElementComposition.GetElementVisual(visualizer)!.Offset.Y == visualizer.Bounds.Height && demo.IsRefreshing && demo.RequestCount == 1 && !Motion.GetEnabled(visualizer) && Colour(visualizer.Foreground) == Resource(visualizer, "One.Accent"), $"{label}: live theme and density changes preserve the visible static indicator and current request");
        Click(window, demo.FindControl<Button>("CompleteRefresh")!); ControlPreview.Flush();
        Check(demo.Revision == 1 && !demo.IsRefreshing && State(visualizer) == RefreshVisualizerState.Idle && ElementComposition.GetElementVisual(visualizer)!.Offset.Y == 0, $"{label}: static completion restores Idle and the list position");
        held = Hold(window, refresh, visualizer.Bounds.Height * 1.2); Release(window, held);
        Check(demo.RequestCount == 2 && demo.IsRefreshing, $"{label}: disabling animation preserves subsequent pull-to-refresh requests");
        Click(window, toggle); Click(window, demo.FindControl<Button>("CompleteRefresh")!);
        Click(window, demo.FindControl<Button>("LastRefreshRow")!); Bounded(list, label);
        Check(list.SelectedIndex == 999 && list.ContainerFromIndex(999) is not null, $"{label}: motion toggles preserve thousand-row virtualization and distant interaction");
        Click(window, demo.FindControl<Button>("ResetRefreshDemo")!);
        Check(!Motion.GetEnabled(refresh) && !refresh.IsSet(Motion.EnabledProperty) && toggle.IsChecked == false, $"{label}: reset removes the local override and restores the global motion preference");
        Motion.SetEnabled(window, true); ControlPreview.Flush();
        Check(Motion.GetEnabled(visualizer) && toggle.IsChecked == true && composition.ImplicitAnimations is not null, $"{label}: re-enabling inherited motion restores native idle transitions");
        window.Close();
    }
    private static void VerifyMotionLifecycle()
    {
        var (window, demo) = Open(ThemeVariant.Dark, ControlSize.Normal);
        var refresh = demo.FindControl<RefreshContainer>("RefreshSample")!; var visualizer = refresh.Visualizer!;
        var original = (Control)visualizer.Content!; var originalVisual = ElementComposition.GetElementVisual(original)!;
        Motion.SetEnabled(refresh, false); ControlPreview.Flush();
        var held = Hold(window, refresh, visualizer.Bounds.Height * 1.2); Release(window, held);
        var custom = new TextBlock { Text = "↻", FontSize = 26 }; visualizer.Content = custom; ControlPreview.Flush();
        var customVisual = ElementComposition.GetElementVisual(custom)!;
        Check(customVisual.ImplicitAnimations is null && originalVisual.ImplicitAnimations is not null && demo.RequestCount == 1 && demo.IsRefreshing, "Refresh motion: replacing custom indicator content while refreshing releases old animation ownership and applies the current motion preference");
        Motion.SetEnabled(visualizer, true); EnhancementsPreview.Advance(50);
        var rotating = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(110);
        Check(customVisual.ImplicitAnimations is not null && !rotating.SequenceEqual(IndicatorPixels(window, refresh)) && !Motion.GetEnabled(refresh) && demo.RequestCount == 1, "Refresh motion: visualizer-local override resumes custom content without changing container preference or issuing a new request");
        visualizer.ClearValue(Motion.EnabledProperty); ControlPreview.Flush();
        var staticCustom = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(100);
        Check(staticCustom.SequenceEqual(IndicatorPixels(window, refresh)) && customVisual.ImplicitAnimations is null, "Refresh motion: clearing visualizer override restores inherited static custom content");
        // All changes occur before a frame is submitted; the last preference must win.
        Motion.SetEnabled(refresh, true); Motion.SetEnabled(refresh, false); Motion.SetEnabled(refresh, true);
        demo.FinishRefresh(); ControlPreview.Flush(); EnhancementsPreview.Advance(120);
        Check(!demo.IsRefreshing && demo.RequestCount == 1 && State(visualizer) == RefreshVisualizerState.Idle, "Refresh motion: rapid changes in one compositor batch followed by completion leave no extra request");
        var idle = IndicatorPixels(window, refresh); EnhancementsPreview.Advance(120);
        Check(idle.SequenceEqual(IndicatorPixels(window, refresh)), "Refresh motion: completing in the same batch as animation resume leaves no continuing spinner");
        Motion.SetEnabled(refresh, false); ControlPreview.Flush();
        var host = (Border)window.Content!; host.Child = null; ControlPreview.Flush();
        Check(customVisual.ImplicitAnimations is not null, "Refresh motion: detachment releases suppressed animation collections");
        host.Child = demo; ControlPreview.Flush();
        Check(ElementComposition.GetElementVisual(custom)!.ImplicitAnimations is null && !demo.IsRefreshing, "Refresh motion: reattachment applies the current preference without retaining a stale request");
        window.Close();
        var standalone = new RefreshVisualizer { Content = new TextBlock { Text = "↻" } };
        window = new Window { Width = 240, Height = 160, Content = standalone }; Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        var standaloneGlyph = ElementComposition.GetElementVisual((Control)standalone.Content!)!;
        Check(standaloneGlyph.ImplicitAnimations is null && State(standalone) == RefreshVisualizerState.Idle, "Refresh motion: standalone visualizer honors inherited motion preference without a refresh provider");
        Motion.SetEnabled(window, true); ControlPreview.Flush();
        Check(standaloneGlyph.ImplicitAnimations is not null, "Refresh motion: standalone visualizer restores native transitions when motion is enabled"); window.Close();
    }
    private static byte[] IndicatorPixels(Window window, RefreshContainer refresh)
    {
        ControlPreview.Flush(); using var bitmap = window.CaptureRenderedFrame()!;
        var center = refresh.TranslatePoint(new Point(refresh.Bounds.Width / 2, refresh.Visualizer!.Bounds.Height / 2), window)!.Value;
        var side = (int)Math.Ceiling(40 * window.RenderScaling); var stride = side * 4;
        var bytes = new byte[side * stride]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect((int)(center.X * window.RenderScaling) - side / 2, (int)(center.Y * window.RenderScaling) - side / 2, side, side), pin.AddrOfPinnedObject(), bytes.Length, stride); }
        finally { pin.Free(); }
        return bytes;
    }
    private static void VerifyDirections()
    {
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var refresh = new RefreshContainer { Content = new ScrollViewer { Content = new Border { Width = 1000, Height = 1000 } } };
            var window = new Window { Width = 500, Height = 300, Content = refresh, WindowDecorations = WindowDecorations.None };
            ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
            var length = size switch { ControlSize.Small => 64, ControlSize.Large => 96, _ => 80 };
            foreach (var direction in Enum.GetValues<PullDirection>())
            {
                refresh.PullDirection = direction; ControlPreview.Flush(); var visualizer = refresh.Visualizer!;
                var horizontal = direction is PullDirection.LeftToRight or PullDirection.RightToLeft;
                Check(Math.Abs((horizontal ? visualizer.Bounds.Width : visualizer.Bounds.Height) - length) < 0.5, $"Refresh/{size}/{direction}: default visualizer keeps the matching density dimension");
                var transform = visualizer.RenderTransform as TranslateTransform;
                Check(transform is not null && Math.Abs(horizontal ? transform.X : transform.Y) == length, $"Refresh/{size}/{direction}: native idle translation matches the styled dimension");
                var deferrals = new List<RefreshCompletionDeferral>();
                EventHandler<RefreshRequestedEventArgs> requested = (_, args) => deferrals.Add(args.GetDeferral());
                refresh.RefreshRequested += requested; Motion.SetEnabled(window, false); refresh.RequestRefresh(); ControlPreview.Flush();
                var indicator = ElementComposition.GetElementVisual(visualizer)!;
                Check(State(visualizer) == RefreshVisualizerState.Refreshing && indicator.Offset.X == -transform!.X && indicator.Offset.Y == -transform.Y && indicator.ImplicitAnimations is null,
                    $"Refresh motion/{size}/{direction}: static refreshing band remains exposed in the native pull direction");
                foreach (var deferral in deferrals) deferral.Complete(); ControlPreview.Flush(); refresh.RefreshRequested -= requested;
                Check(State(visualizer) == RefreshVisualizerState.Idle && indicator.Offset.X == 0 && indicator.Offset.Y == 0,
                    $"Refresh motion/{size}/{direction}: completing static refresh restores native idle offsets");
            }
            window.Close();
        }
    }
    private static void VerifyCustomization()
    {
        var visualizer = new RefreshVisualizer { Height = 132, Foreground = Brushes.Red, Background = Brushes.Blue,
            Content = new TextBlock { Text = "↻", FontSize = 32 } };
        var list = new ListBox { ItemsSource = Enumerable.Range(0, 1000).ToArray() };
        var refresh = new RefreshContainer { Visualizer = visualizer, Content = list, Background = Brushes.Yellow, BorderBrush = Brushes.Red,
            BorderThickness = new Thickness(2), CornerRadius = new CornerRadius(8), Padding = new Thickness(3) };
        var window = new Window { Width = 520, Height = 320, Content = refresh, WindowDecorations = WindowDecorations.None };
        ControlSizing.SetSize(window, ControlSize.Small); window.Show(); ControlPreview.Flush();
        Check(ReferenceEquals(refresh.Visualizer, visualizer) && visualizer.Bounds.Height == 132 && visualizer.Content is TextBlock { Text: "↻", FontSize: 32 }, "Refresh custom visualizer dimension and custom animated content remain intact");
        Check(Part<Border>(refresh, "PART_Outline").BorderBrush == Brushes.Red && refresh.Padding.Left == 3 && refresh.Background == Brushes.Yellow && visualizer.Foreground == Brushes.Red && visualizer.Background == Brushes.Blue, "Refresh consumer surface, outline, padding and indicator colors take precedence");
        var deferrals = new List<RefreshCompletionDeferral>(); refresh.RefreshRequested += (_, args) => deferrals.Add(args.GetDeferral());
        refresh.RequestRefresh(); ControlPreview.Flush();
        Check(deferrals.Count > 0 && State(visualizer) == RefreshVisualizerState.Refreshing, "Refresh custom visualizer preserves the native refresh request and deferral contract");
        foreach (var deferral in deferrals) deferral.Complete(); ControlPreview.Flush(); Check(State(visualizer) == RefreshVisualizerState.Idle, "Refresh custom deferral completion restores Idle");
        Bounded(list, "Custom Refresh"); window.Close();
    }
    private static void VerifyDetach()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        demo.FindControl<CheckBox>("ManualComplete")!.IsChecked = false;
        demo.FindControl<RefreshContainer>("RefreshSample")!.RequestRefresh(); ControlPreview.Flush(); var revision = demo.Revision;
        window.Close(); EnhancementsPreview.Advance(1100);
        Check(!demo.IsRefreshing && demo.Revision == revision, "Refresh preview detachment stops the async timer and balances all deferrals without applying stale data");
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(19); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("RefreshCard")!;
        Check(card.IsVisible && card.Child is RefreshDemoView, "Refresh category lazily loads the pull-to-refresh interaction preview");
        var demo = (RefreshDemoView)card.Child!; demo.FindControl<CheckBox>("ManualComplete")!.IsChecked = true;
        Click(window, demo.FindControl<Button>("RequestRefresh")!); Click(window, demo.FindControl<Button>("CompleteRefresh")!);
        Check(demo.Revision == 1 && demo.Rows.Count == 1000, "Refresh catalogue supports real async request and completion actions");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        Check(demo.FindControl<RefreshContainer>("RefreshSample")!.Visualizer!.Bounds.Height == 64 && demo.ActualThemeVariant == ThemeVariant.Dark, "Refresh catalogue follows global theme and indicator density");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0); gallery.Search("RefreshContainer"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("CarouselCard")!.IsVisible, "Refresh search locates its dedicated interaction preview");
        gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size); var refresh = demo.FindControl<RefreshContainer>("RefreshSample")!;
            var name = $"refresh-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            Save(window, Path.Combine(output, name + ".png"));
            var held = Hold(window, refresh, refresh.Visualizer!.Bounds.Height * 1.15); EnhancementsPreview.Advance(200);
            Save(window, Path.Combine(output, name + "-pending.png")); Release(window, held); EnhancementsPreview.Advance(200);
            Save(window, Path.Combine(output, name + "-refreshing.png"));
            Click(window, demo.FindControl<CheckBox>("RefreshMotionEnabled")!); EnhancementsPreview.Advance(180);
            Save(window, Path.Combine(output, name + "-motion-off.png"));
            Click(window, demo.FindControl<CheckBox>("RefreshMotionEnabled")!); EnhancementsPreview.Advance(120);
            Save(window, Path.Combine(output, name + "-motion-restored.png"));
            Click(window, demo.FindControl<Button>("CompleteRefresh")!); EnhancementsPreview.Advance(180);
            Click(window, demo.FindControl<Button>("LastRefreshRow")!); Save(window, Path.Combine(output, name + "-last.png")); window.Close();
        }
        var (compact, _) = Open(ThemeVariant.Dark, ControlSize.Normal, 680); Save(compact, Path.Combine(output, "refresh-compact.png")); compact.Close();
    }
    internal static void Save(Window window, string path)
    {
        ControlPreview.Flush(); using var bitmap = window.CaptureRenderedFrame() ?? throw new InvalidOperationException("Refresh preview requires a composited Skia frame.");
        bitmap.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
    }
    private static Point Hold(TopLevel window, RefreshContainer refresh, double distance)
    {
        var start = refresh.TranslatePoint(new Point(refresh.Bounds.Width / 2, 44), window)!.Value;
        var end = start + new Vector(0, distance); window.MouseMove(start); window.MouseDown(start, MouseButton.Left);
        for (var step = 1; step <= 8; step++) { window.MouseMove(start + (end - start) * (step / 8d), RawInputModifiers.LeftMouseButton); EnhancementsPreview.Advance(10); }
        ControlPreview.Flush(); return end;
    }
    private static void Release(TopLevel window, Point end) { window.MouseUp(end, MouseButton.Left); ControlPreview.Flush(); }
    private static RefreshVisualizerState State(RefreshVisualizer visualizer) => visualizer.GetValue(RefreshVisualizer.RefreshVisualizerStateProperty);
    private static void Bounded(ListBox list, string label) => Check(list.ItemCount == 1000 && list.ItemsPanelRoot is VirtualizingStackPanel && list.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: native list viewport realizes fewer than 80 of a thousand data rows");
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
