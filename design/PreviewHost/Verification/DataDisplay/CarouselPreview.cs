using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Controls;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class CarouselPreview
{
    private static (Window Window, CarouselDemoView Demo) Open(ThemeVariant theme, ControlSize size, int width = 1120)
    {
        var demo = new CarouselDemoView();
        demo.FindControl<CheckBox>("AnimatePages")!.IsChecked = false;
        var window = new Window { Width = width, Height = 1240, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo } };
        ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush(); return (window, demo);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyNative(theme, size);
        VerifyOverrides(); VerifyCollectionChanges();
    }
    private static void VerifyNative(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size); var carousel = demo.FindControl<Carousel>("CarouselSample")!;
        var label = $"Carousel/{theme.Key}/{size}";
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var padding = size switch { ControlSize.Small => 12, ControlSize.Large => 20, _ => 16 };
        var minimum = size switch { ControlSize.Small => 180, ControlSize.Large => 260, _ => 220 };
        Check(carousel.GetType() == typeof(Carousel) && carousel.ItemsPanelRoot is VirtualizingCarouselPanel, $"{label}: retains the official virtualizing carousel panel");
        Check(carousel.ItemCount == 1000 && carousel.FontSize == font && carousel.Padding.Left == padding && carousel.MinHeight == minimum, $"{label}: thousand-model source and inherited three-size metrics");
        Check(Colour(carousel.Background) == Resource(carousel, "One.Surface") && Colour(carousel.Foreground) == Resource(carousel, "One.Text") && carousel.CornerRadius.TopLeft == 12, $"{label}: Still surface, text and rounded border");
        Check(carousel.PageTransition is null && carousel.SelectedIndex == 0 && carousel.ContainerFromIndex(999) is null, $"{label}: reduced motion and AlwaysSelected avoid eagerly creating the final page");
        VerifyCurrent(carousel, demo.Slides[0], label); Bounded(carousel, label);
        var original = carousel.ContainerFromIndex(0)!;
        Click(window, demo.FindControl<Button>("NextPage")!);
        Check(carousel.SelectedIndex == 1 && ReferenceEquals(original, carousel.ContainerFromIndex(1)), $"{label}: next-page click reuses the native container");
        VerifyCurrent(carousel, demo.Slides[1], label + "/next");
        Click(window, demo.FindControl<Button>("PreviousPage")!);
        Check(carousel.SelectedIndex == 0 && !demo.FindControl<Button>("PreviousPage")!.IsEnabled, $"{label}: previous-page click respects the first boundary");
        Click(window, demo.FindControl<Button>("JumpPage")!);
        Check(carousel.SelectedIndex == 900, $"{label}: editable target jump materializes page 901");
        VerifyCurrent(carousel, demo.Slides[900], label + "/jump");
        Click(window, demo.FindControl<Button>("LastPage")!);
        Check(carousel.SelectedIndex == 999 && !demo.FindControl<Button>("NextPage")!.IsEnabled, $"{label}: distant last-page jump and final boundary");
        VerifyCurrent(carousel, demo.Slides[999], label + "/last"); Bounded(carousel, label + "/last");
        var action = Action(carousel); Click(window, action);
        Check(demo.ActionCount == 1 && demo.FindControl<TextBlock>("CarouselStatus")!.Text!.Contains("1000"), $"{label}: recycled page action uses the current model");
        carousel.Focus(); Press(window, Key.Left, PhysicalKey.ArrowLeft);
        Check(carousel.SelectedIndex == 998, $"{label}: native arrow navigation reaches an unrealized adjacent page");
        Press(window, Key.Home, PhysicalKey.Home); Check(carousel.SelectedIndex == 0, $"{label}: native Home returns to the first model");
        Press(window, Key.End, PhysicalKey.End); Check(carousel.SelectedIndex == 999, $"{label}: native End realizes the thousandth model");
        Click(window, demo.FindControl<CheckBox>("WrapPages")!);
        Click(window, demo.FindControl<Button>("NextPage")!); Check(carousel.SelectedIndex == 0, $"{label}: WrapSelection cycles forward across the last boundary");
        Click(window, demo.FindControl<Button>("PreviousPage")!); Check(carousel.SelectedIndex == 999, $"{label}: WrapSelection cycles backward across the first boundary");
        carousel.SelectedIndex = 400; ControlPreview.Flush();
        Swipe(window, carousel, true);
        Check(carousel.SelectedIndex == 401 && !carousel.IsSwiping, $"{label}: actual pointer swipe commits native selection");
        CarouselOptions.SetIsMouseSwipeEnabled(carousel, false); ControlPreview.Flush();
        Check(carousel.IsSwipeEnabled && !carousel.ItemsPanelRoot!.GestureRecognizers.OfType<SwipeGestureRecognizer>().Single().IsMouseEnabled, $"{label}: separate mouse option leaves native touch and pen swipe enabled");
        Swipe(window, carousel, true); Check(carousel.SelectedIndex == 401, $"{label}: consumer mouse-input override rejects real drag navigation");
        CarouselOptions.SetIsMouseSwipeEnabled(carousel, true); ControlPreview.Flush();
        Click(window, demo.FindControl<CheckBox>("EnableSwipe")!); Swipe(window, carousel, false);
        Check(carousel.SelectedIndex == 401, $"{label}: disabling native swipe prevents drag navigation");
        Click(window, demo.FindControl<CheckBox>("EnableSwipe")!);
        demo.FindControl<ComboBox>("ViewportPicker")!.SelectedIndex = 1; ControlPreview.Flush();
        Check(carousel.ViewportFraction == 0.72 && carousel.GetRealizedContainers().Count() is > 1 and <= 4, $"{label}: fractional viewport realizes only the current and neighboring pages");
        carousel.SelectedIndex = 900; ControlPreview.Flush(); VerifyCurrent(carousel, demo.Slides[900], label + "/fraction");
        Bounded(carousel, label + "/fraction");
        Swipe(window, carousel, true);
        Check(carousel.SelectedIndex == 901, $"{label}: adjacent-page mode retains native swipe navigation");
        demo.FindControl<ComboBox>("ViewportPicker")!.SelectedIndex = 0; ControlPreview.Flush();
        Click(window, demo.FindControl<CheckBox>("AnimatePages")!);
        Check(carousel.PageTransition is PageSlide { Duration.TotalMilliseconds: 200, Orientation: PageSlide.SlideAxis.Horizontal }, $"{label}: motion restores the native short horizontal page transition");
        carousel.SelectedIndex = 902; ControlPreview.Flush(); EnhancementsPreview.Advance(30);
        Check(carousel.ItemsPanelRoot!.Children.Count(control => control.IsVisible) == 2 && carousel.ItemsPanelRoot.Children.Any(control => control.RenderTransform is { } transform && Math.Abs(transform.Value.M31) > 0.5), $"{label}: native intermediate animation frame moves only the two participating pages");
        Settle(carousel);
        for (var index = 0; index < 12; index++)
        {
            carousel.SelectedIndex = index % 2 == 0 ? 901 : 999;
            ControlPreview.Flush(); EnhancementsPreview.Advance(15);
            Check(carousel.ItemsPanelRoot!.Children.Count <= 8 && carousel.GetRealizedContainers().Count() == 1, $"{label}: interrupted transition {index + 1} keeps a bounded page pool");
        }
        Settle(carousel); VerifyCurrent(carousel, demo.Slides[999], label + "/rapid");
        Check(carousel.ItemsPanelRoot!.Children.Count(control => control.IsVisible) == 1, $"{label}: rapid transitions settle to one visible page without stale content");
        Click(window, demo.FindControl<Button>("ReversePages")!);
        EnhancementsPreview.AdvanceUntil(() => !demo.IsReversing, 1200); Settle(carousel);
        Check(carousel.SelectedIndex == 0 && !demo.IsReversing, $"{label}: preview rapid-reversal action settles to its final target");
        Click(window, demo.FindControl<CheckBox>("AnimatePages")!); ControlPreview.Flush();
        Check(carousel.PageTransition is null, $"{label}: motion toggle removes further theme page transitions");
        Click(window, demo.FindControl<CheckBox>("EnableCarousel")!); var selected = carousel.SelectedIndex;
        Click(window, Action(carousel));
        Check(!carousel.IsEffectivelyEnabled && !demo.FindControl<Button>("NextPage")!.IsEnabled && carousel.SelectedIndex == selected && demo.ActionCount == 1, $"{label}: disabled carousel and page content reject pointer actions");
        Check(Colour(carousel.Foreground) == Resource(carousel, "One.DisabledText"), $"{label}: disabled text uses the shared semantic color");
        Click(window, demo.FindControl<Button>("ResetCarousel")!);
        Check(carousel.IsEnabled && carousel.SelectedIndex == 0 && demo.ActionCount == 0 && !carousel.WrapSelection && carousel.ViewportFraction == 1, $"{label}: reset restores native interaction and preview state");
        carousel.SelectedIndex = 777; ControlPreview.Flush();
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); ControlPreview.Flush();
        Check(carousel.SelectedIndex == 777 && carousel.FontSize == (size == ControlSize.Large ? 12 : 16) && Colour(carousel.Background) == Resource(carousel, "One.Surface"), $"{label}: live theme and density changes preserve selection and update resources");
        VerifyCurrent(carousel, demo.Slides[777], label + "/updated");
        var description = carousel.ContainerFromIndex(777)!.GetVisualDescendants().OfType<TextBlock>().Single(text => text.Name == "SlideDescription");
        Check(description.FontSize == carousel.FontSize, $"{label}: page body inherits the selected density");
        window.Width = 680; ControlPreview.Flush(); Bounded(carousel, label + "/compact");
        Check(carousel.Bounds.Width <= 632 && double.IsFinite(carousel.ItemsPanelRoot!.Bounds.Width) && double.IsFinite(carousel.ItemsPanelRoot.Bounds.Height), $"{label}: compact layout keeps a finite virtual viewport");
        demo.UpdateMetrics(); Check(demo.FindControl<TextBlock>("CarouselMetrics")!.Text!.Contains("1,000") && demo.CreatedContainerCount <= 8, $"{label}: preview reports actual realization and bounded cumulative creation");
        window.Close();
    }
    private static void VerifyOverrides()
    {
        var carousel = new Carousel { ItemsSource = Enumerable.Range(0, 1000).ToArray(), Height = 230, MinHeight = 0, FontSize = 19, Padding = new Thickness(0),
            CornerRadius = new CornerRadius(12), BorderThickness = new Thickness(2), BorderBrush = Brushes.Lime, Background = Brushes.Blue,
            ItemTemplate = new FuncDataTemplate<int>((number, _) => new Border { Background = Brushes.Red, Child = new TextBlock { Text = "Custom " + number } }) };
        var window = new Window { Width = 520, Height = 280, WindowDecorations = WindowDecorations.None, Content = carousel };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        carousel.Classes.Add("small"); ControlPreview.Flush();
        Check(carousel.FontSize == 19 && carousel.Padding.Left == 0 && carousel.MinHeight == 0 && carousel.CornerRadius.TopLeft == 12 && Part<Border>(carousel, "PART_Outline").BorderBrush == Brushes.Lime, "Carousel consumer properties take precedence over theme density");
        Check(carousel.ContainerFromIndex(0)!.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Custom 0"), "Carousel retains a custom ItemTemplate with a thousand data models");
        var transition = new CrossFade(TimeSpan.FromMilliseconds(30)); carousel.PageTransition = transition; ControlPreview.Flush();
        Check(ReferenceEquals(carousel.PageTransition, transition), "Carousel explicit consumer PageTransition remains effective");
        carousel.SelectedIndex = 999; ControlPreview.Flush(); Settle(carousel); Bounded(carousel, "Custom Carousel");
        Check(carousel.ContainerFromIndex(999)!.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Custom 999"), "Carousel custom template and transition update distant recycled content");
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)carousel.Bounds.Width, (int)carousel.Bounds.Height), new Vector(96, 96)); bitmap.Render(carousel);
        Check(Pixel(bitmap, 0, 0).A == 0 && Pixel(bitmap, bitmap.PixelSize.Width / 2, 0) == Colors.Lime, "Carousel Skia clipping preserves transparent corners and the complete outline around opaque page content");
        window.Close();
    }
    private static void VerifyCollectionChanges()
    {
        var source = new ObservableCollection<string>(Enumerable.Range(1, 1000).Select(number => $"Page {number}"));
        var carousel = new Carousel { ItemsSource = source, PageTransition = null, ItemTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = text }) };
        var window = new Window { Width = 640, Height = 280, Content = carousel, WindowDecorations = WindowDecorations.None };
        window.Show(); carousel.SelectedIndex = 900; ControlPreview.Flush(); var selected = carousel.SelectedItem;
        source.Insert(0, "Inserted"); ControlPreview.Flush();
        Check(Equals(carousel.SelectedItem, selected) && carousel.SelectedIndex == 901 && carousel.ContainerFromIndex(901) is not null, "Carousel collection insertion preserves the selected distant model");
        source.RemoveAt(0); ControlPreview.Flush();
        Check(carousel.SelectedIndex == 900 && Equals(carousel.SelectedItem, selected), "Carousel collection removal restores the selected model index");
        source.RemoveAt(carousel.SelectedIndex); ControlPreview.Flush();
        Check(carousel.SelectedIndex >= 0 && !Equals(carousel.SelectedItem, selected) && carousel.ContainerFromIndex(carousel.SelectedIndex) is not null, "Carousel AlwaysSelected materializes a valid fallback after removing the active model");
        Bounded(carousel, "Mutable Carousel"); source.Clear(); ControlPreview.Flush();
        Check(carousel.SelectedIndex == -1 && !carousel.GetRealizedContainers().Any(), "Carousel empty source clears visible pages and native selection");
        foreach (var number in Enumerable.Range(1, 1000)) source.Add($"Reload {number}"); ControlPreview.Flush();
        Check(carousel.SelectedIndex == 0 && carousel.GetRealizedContainers().Count() == 1, "Carousel data reload realizes one page from a thousand fresh models");
        window.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(18); ControlPreview.Flush();
        var card = gallery.FindControl<Border>("CarouselCard")!;
        Check(card.IsVisible && card.Child is CarouselDemoView, "Carousel category lazily loads its interactive preview");
        var demo = (CarouselDemoView)card.Child!;
        var lastButton = demo.FindControl<Button>("LastPage")!;
        lastButton.BringIntoView(); ControlPreview.Flush();
        Click(window, lastButton);
        var carousel = demo.FindControl<Carousel>("CarouselSample")!; Settle(carousel);
        Check(carousel.SelectedIndex == 999 && carousel.ItemCount == 1000, "Carousel catalogue last-page jump uses a thousand-item model source");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        Check(carousel.FontSize == 12 && carousel.ActualThemeVariant == ThemeVariant.Dark && carousel.SelectedIndex == 999, "Carousel catalogue follows global theme and density without resetting its page");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0); gallery.Search("Carousel"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("ButtonControlsCard")!.IsVisible && !gallery.FindControl<Border>("TabStripCard")!.IsVisible, "Carousel search locates its interaction preview independently");
        gallery.Search(""); ControlPreview.Flush();
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size); var name = $"carousel-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            EnhancementsPreview.Save(window, Path.Combine(output, name + ".png"));
            Click(window, demo.FindControl<Button>("LastPage")!); EnhancementsPreview.Save(window, Path.Combine(output, name + "-last.png"));
            demo.FindControl<ComboBox>("ViewportPicker")!.SelectedIndex = 1;
            demo.FindControl<Carousel>("CarouselSample")!.SelectedIndex = 500; ControlPreview.Flush();
            EnhancementsPreview.Save(window, Path.Combine(output, name + "-neighbors.png"));
            if (size == ControlSize.Normal)
            {
                demo.FindControl<ComboBox>("ViewportPicker")!.SelectedIndex = 0;
                demo.FindControl<CheckBox>("AnimatePages")!.IsChecked = true; ControlPreview.Flush();
                demo.FindControl<Carousel>("CarouselSample")!.Next(); ControlPreview.Flush(); EnhancementsPreview.Advance(50);
                EnhancementsPreview.Save(window, Path.Combine(output, name + "-transition.png"));
            }
            window.Close();
        }
        var (compact, _) = Open(ThemeVariant.Dark, ControlSize.Normal, 680); EnhancementsPreview.Save(compact, Path.Combine(output, "carousel-compact.png")); compact.Close();
    }
    private static void VerifyCurrent(Carousel carousel, CarouselSlide slide, string label)
    {
        var current = carousel.ContainerFromIndex(carousel.SelectedIndex);
        Check(current is not null && Equals(carousel.ItemFromContainer(current), slide) && current.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Name == "SlideTitle" && text.Text == slide.Title), $"{label}: realized page displays the current data model");
    }
    private static void Bounded(Carousel carousel, string label) => Check(carousel.GetRealizedContainers().Count() is > 0 and <= 4 && carousel.ItemsPanelRoot!.Children.Count <= 8, $"{label}: active pages and cached containers stay bounded for a thousand models");
    private static Button Action(Carousel carousel) => carousel.ContainerFromIndex(carousel.SelectedIndex)!.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "SlideAction");
    private static void Settle(Carousel carousel) => EnhancementsPreview.AdvanceUntil(() => carousel.ItemsPanelRoot!.Children.Count(control => control.IsVisible) == carousel.GetRealizedContainers().Count() && !carousel.IsSwiping, 1500);
    private static void Swipe(TopLevel window, Carousel carousel, bool left)
    {
        var start = carousel.TranslatePoint(new Point(carousel.Bounds.Width * (left ? 0.8 : 0.2), carousel.Bounds.Height / 2), window)!.Value;
        var end = carousel.TranslatePoint(new Point(carousel.Bounds.Width * (left ? 0.2 : 0.8), carousel.Bounds.Height / 2), window)!.Value;
        window.MouseMove(start); window.MouseDown(start, MouseButton.Left);
        for (var step = 1; step <= 8; step++) { window.MouseMove(start + (end - start) * (step / 8d), RawInputModifiers.LeftMouseButton); EnhancementsPreview.Advance(10); }
        window.MouseUp(end, MouseButton.Left); EnhancementsPreview.AdvanceUntil(() => !carousel.IsSwiping, 1500); ControlPreview.Flush();
    }
    private static void Press(TopLevel host, Key key, PhysicalKey physical) { host.KeyPress(key, RawInputModifiers.None, physical, null); host.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush(); }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
    {
        var bytes = new byte[4]; var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pinned.AddrOfPinnedObject(), 4, 4); } finally { pinned.Free(); }
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]) : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }
}
