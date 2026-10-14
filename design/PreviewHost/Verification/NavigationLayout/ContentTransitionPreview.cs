using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class ContentTransitionPreview
{
    private static (Window Window, ContentTransitionDemoView Demo, TransitioningContentControl Sample) Open(ThemeVariant theme, ControlSize size, int width = 1040)
    {
        var demo = new ContentTransitionDemoView();
        var window = new Window { Width = width, Height = 970, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = new Border { Padding = new Thickness(24), Child = demo } };
        ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
        return (window, demo, demo.FindControl<TransitioningContentControl>("ContentTransitionSample")!);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) VerifyInteraction(theme, size);
        VerifyConsumer(); VerifyIndependentControls(); VerifyDetach();
    }
    private static void VerifyInteraction(ThemeVariant theme, ControlSize size)
    {
        var (window, demo, sample) = Open(theme, size); var label = $"Content transition/{theme.Key}/{size}";
        var events = new List<TransitionCompletedEventArgs>(); sample.TransitionCompleted += (_, args) => events.Add(args);
        var motion = demo.FindControl<CheckBox>("ContentMotionEnabled")!; var picker = demo.FindControl<ComboBox>("ContentTransitionPicker")!;
        Check(sample.GetType() == typeof(TransitioningContentControl) && Presenters(sample).Length == 2 && Presenters(sample).Count(p => p.Content is not null && p.IsVisible) == 1, $"{label}: native dual-presenter contract starts with one current data template");
        Check(sample.Padding == new Thickness(size switch { ControlSize.Small => 12, ControlSize.Large => 20, _ => 16 }) && sample.FontSize == (size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 }), $"{label}: inherited three-size content padding and typography");
        Check(Colour(sample.Background) == Resource(sample, "One.Surface") && Colour(sample.Foreground) == Resource(sample, "One.Text") && Colour(sample.BorderBrush) == Resource(sample, "One.Line") && Part<Border>(sample, "PART_LayoutRoot").ClipToBounds, $"{label}: semantic surface, text, outline and rounded animation clipping");
        Check(ContentTransitionOptions.GetConfiguredTransition(sample) is CrossFade { Duration.TotalMilliseconds: 200 }, $"{label}: default transition uses the theme duration");
        // Keep the default-duration assertion above, but allow real pointer dispatch
        // enough time to interrupt the probe on a busy renderer or NativeAOT host.
        var initialFade = (CrossFade)ContentTransitionOptions.GetConfiguredTransition(sample)!;
        initialFade.Duration = TimeSpan.FromMilliseconds(800);
        Click(window, demo.FindControl<Button>("NextContentPage")!); EnhancementsPreview.Advance(35);
        var incoming = Current(sample);
        Check(Presenters(sample).Count(p => p.IsVisible) == 2 && incoming.Opacity is > 0 and < 1 && ReferenceEquals(incoming.Content, demo.Pages[1]), $"{label}: real next button plays a native crossfade between the two models");
        Check(!Presenters(sample).Single(p => !ReferenceEquals(p, incoming)).IsHitTestVisible && incoming.IsHitTestVisible, $"{label}: only incoming content accepts input during a transition");
        Click(window, motion); ControlPreview.Flush(); Stable(sample, label);
        Check(!Motion.GetEnabled(sample) && events.Any(args => ReferenceEquals(args.To, demo.Pages[1]) && !args.HasRunToCompletion), $"{label}: disabling motion mid-fade settles current content and reports interruption through the native event (enabled={Motion.GetEnabled(sample)}, checked={motion.IsChecked}, events={string.Join(',', events.Select(e => e.HasRunToCompletion))})");
        initialFade.Duration = TimeSpan.FromMilliseconds(200);
        var still = Pixels(window, sample); EnhancementsPreview.Advance(80);
        Check(still.SequenceEqual(Pixels(window, sample)), $"{label}: actual Skia content frames stay still with motion disabled");
        var draft = Current(sample).GetVisualDescendants().OfType<TextBox>().Single(box => box.Name == "TransitionDraft");
        Click(window, draft); draft.SelectAll(); window.KeyTextInput("保留手工草稿"); ControlPreview.Flush();
        Click(window, Current(sample).GetVisualDescendants().OfType<Button>().Single(button => button.Name == "SaveTransitionDraft"));
        Check(((TransitionForm)demo.Pages[1]).Draft == "保留手工草稿" && demo.ActionCount == 1, $"{label}: current template retains real text input and button actions");
        Click(window, demo.FindControl<Button>("NextContentPage")!); Stable(sample, label); Bounded(demo.CurrentList()!, label);
        Click(window, demo.FindControl<Button>("LastTransitionRecord")!); ControlPreview.Flush();
        Check(demo.CurrentList()!.SelectedIndex == 999 && demo.CurrentList()!.ContainerFromIndex(999) is not null, $"{label}: distant interaction realizes the thousandth record");
        Click(window, demo.FindControl<Button>("PreviousContentPage")!); Stable(sample, label);
        Check(Current(sample).GetVisualDescendants().OfType<TextBox>().Single().Text == "保留手工草稿", $"{label}: switching templates preserves the model-backed draft");
        Click(window, demo.FindControl<Button>("NextContentPage")!); Stable(sample, label); Bounded(demo.CurrentList()!, label);
        Check(demo.CurrentList()!.SelectedIndex == 999, $"{label}: recreated virtual list preserves model-backed selection");
        Click(window, motion); Click(window, demo.FindControl<Button>("PreviousContentPage")!); EnhancementsPreview.Advance(35);
        Check(Current(sample).Opacity is > 0 and < 1 && Motion.GetEnabled(sample), $"{label}: re-enabling motion restores the next transition"); Settle(sample);
        Check(events.Any(args => ReferenceEquals(args.To, demo.Pages[1]) && args.HasRunToCompletion), $"{label}: resumed animation raises a successfully completed native event");
        foreach (var mode in new[] { 1, 2 })
        {
            picker.SelectedIndex = mode; demo.FindControl<CheckBox>("ReverseContentTransition")!.IsChecked = true;
            demo.SelectPage(0); ControlPreview.Flush(); EnhancementsPreview.Advance(25);
            var transform = Current(sample).RenderTransform;
            Check(transform is not null && (mode == 1 ? transform.Value.M31 : transform.Value.M32) < 0 && sample.IsTransitionReversed, $"{label}: reversed {(mode == 1 ? "horizontal" : "vertical")} native slide uses the correct direction");
            Click(window, motion); Stable(sample, label);
            Check(Current(sample).RenderTransform is null, $"{label}: disabling motion mid-slide clears translation immediately");
            Click(window, motion); demo.SelectPage(1); ControlPreview.Flush(); EnhancementsPreview.Advance(30);
            Check(Current(sample).RenderTransform is not null, $"{label}: custom slide configuration resumes after motion is enabled");
            picker.SelectedIndex = 3; ControlPreview.Flush(); Stable(sample, label);
            Check(sample.PageTransition is null, $"{label}: selecting no transition during playback cancels and preserves current content");
        }
        Click(window, demo.FindControl<Button>("ClearTransitionContent")!);
        Check(sample.Content is null && Presenters(sample).All(p => p.Content is null) && Part<Border>(sample, "PART_Outline").BorderThickness == sample.BorderThickness, $"{label}: null content clears both presenters while retaining the fixed outline");
        Click(window, demo.FindControl<Button>("RestoreTransitionContent")!); Stable(sample, label);
        picker.SelectedIndex = 0; demo.SelectPage(0); Settle(sample);
        Click(window, demo.FindControl<Button>("RapidContentSwitch")!); EnhancementsPreview.AdvanceUntil(() => !demo.IsRapidSwitching, 600); Settle(sample);
        Check(ReferenceEquals(sample.Content, demo.Pages[1]) && Presenters(sample).Count(p => p.Content is not null) == 1, $"{label}: rapid interrupted transitions retain the latest model and release old templates");
        var before = events.Count; sample.Content = sample.Content; ControlPreview.Flush();
        Check(events.Count == before, $"{label}: assigning identical content does not duplicate transition events");
        sample.ClearValue(Motion.EnabledProperty); Motion.SetEnabled(window, false); ControlPreview.Flush();
        Check(motion.IsChecked == false && !Motion.GetEnabled(sample), $"{label}: preview switch reflects inherited global motion");
        Click(window, motion); Check(Motion.GetEnabled(sample), $"{label}: local motion override takes precedence over global preference");
        Click(window, demo.FindControl<Button>("ResetContentTransition")!); Stable(sample, label);
        Check(!sample.IsSet(Motion.EnabledProperty) && !Motion.GetEnabled(sample) && motion.IsChecked == false && ContentTransitionOptions.GetConfiguredTransition(sample) is CrossFade, $"{label}: reset restores global motion inheritance and themed transition");
        Motion.SetEnabled(window, true); window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); ControlPreview.Flush();
        Check(motion.IsChecked == true && sample.FontSize == (size == ControlSize.Large ? 12 : 16) && Colour(sample.Background) == Resource(sample, "One.Surface"), $"{label}: live theme, density and global motion updates preserve content");
        Click(window, demo.FindControl<CheckBox>("EnableContentRegion")!);
        Check(!Current(sample).IsEffectivelyEnabled && Colour(sample.Foreground) == Resource(sample, "One.DisabledText") && !demo.FindControl<Button>("NextContentPage")!.IsEnabled, $"{label}: disabled content and navigation use the theme state");
        Click(window, demo.FindControl<CheckBox>("EnableContentRegion")!); window.Width = 680; ControlPreview.Flush();
        Check(sample.Bounds.Width <= 632 && sample.Bounds.Height == 400, $"{label}: compact layout keeps a bounded viewport"); window.Close();
    }
    private sealed class TransitionSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private IPageTransition? _transition = new CrossFade(TimeSpan.FromMilliseconds(100));
        public IPageTransition? Transition { get => _transition; set { _transition = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Transition))); } }
    }
    private static void VerifyConsumer()
    {
        var settings = new TransitionSettings();
        var sample = new TransitioningContentControl { Content = "First", Padding = new Thickness(7), FontSize = 19, Background = Brushes.Yellow, Foreground = Brushes.Red, BorderBrush = Brushes.Blue, BorderThickness = new Thickness(3), CornerRadius = new CornerRadius(9) };
        using var binding = sample.Bind(TransitioningContentControl.PageTransitionProperty, CompiledBinding.Create<TransitionSettings, IPageTransition?>(value => value.Transition, source: settings));
        var window = new Window { Width = 480, Height = 260, Content = sample }; ControlSizing.SetSize(window, ControlSize.Small); window.Show(); ControlPreview.Flush();
        Check(sample.Padding.Left == 7 && sample.FontSize == 19 && Part<Border>(sample, "PART_LayoutRoot").Background == Brushes.Yellow && Part<Border>(sample, "PART_Outline").BorderBrush == Brushes.Blue && sample.CornerRadius == new CornerRadius(9), "Content transition: consumer appearance overrides retain precedence");
        Check(ReferenceEquals(ContentTransitionOptions.GetConfiguredTransition(sample), settings.Transition), "Content transition: motion adapter preserves supplied transition configuration");
        settings.Transition = new PageSlide(TimeSpan.FromMilliseconds(120)); sample.Content = "Second"; ControlPreview.Flush(); EnhancementsPreview.Advance(25);
        Check(ReferenceEquals(ContentTransitionOptions.GetConfiguredTransition(sample), settings.Transition) && Current(sample).RenderTransform is { } forward && forward.Value.M31 > 0, "Content transition: transition binding updates without losing the binding and preserves forward direction");
        Motion.SetEnabled(window, false); ControlPreview.Flush(); Stable(sample, "Consumer");
        settings.Transition = null; sample.Content = "Third"; ControlPreview.Flush();
        Check(sample.PageTransition is null && Equals(Current(sample).Content, "Third"), "Content transition: bound null transition presents new content directly");
        settings.Transition = new CrossFade(TimeSpan.FromMilliseconds(80)); sample.Content = "Fourth"; ControlPreview.Flush(); Stable(sample, "Consumer");
        Check(ReferenceEquals(ContentTransitionOptions.GetConfiguredTransition(sample), settings.Transition), "Content transition: bound transition can be reintroduced while motion is disabled");
        Motion.SetEnabled(window, true); sample.ContentTemplate = new FuncDataTemplate<string>((text, _) => new TextBlock { Text = "Custom " + text }); sample.Content = "Fifth"; Settle(sample);
        Check(Current(sample).GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Custom Fifth"), "Content transition: consumer ContentTemplate renders current model data");
        var shared = new TextBox { Text = "Shared control" }; sample.ContentTemplate = null; sample.Content = shared; Settle(sample);
        sample.Content = "Between"; ControlPreview.Flush(); EnhancementsPreview.Advance(20); sample.Content = shared; Settle(sample);
        Check(ReferenceEquals(Current(sample).Child, shared) && Presenters(sample).Count(p => ReferenceEquals(p.Content, shared)) == 1, "Content transition: rapid return to the same Control preserves native single-parent ownership"); window.Close();
    }
    private static void VerifyDetach()
    {
        var (window, demo, sample) = Open(ThemeVariant.Dark, ControlSize.Large); demo.SelectPage(1); ControlPreview.Flush(); EnhancementsPreview.Advance(30);
        var host = (Border)window.Content!; host.Child = null; ControlPreview.Flush(); EnhancementsPreview.Advance(60);
        Check(Presenters(sample).All(p => p.Opacity == 1 && p.RenderTransform is null), "Content transition: detachment cancels running animation and clears residue");
        host.Child = demo; ControlPreview.Flush(); Stable(sample, "Reattached");
        demo.SelectPage(0); Settle(sample); Check(ReferenceEquals(Current(sample).Content, demo.Pages[0]), "Content transition: reattachment resumes native switching without stale callbacks");
        Click(window, demo.FindControl<Button>("RapidContentSwitch")!); host.Child = null; ControlPreview.Flush();
        Check(!demo.IsRapidSwitching, "Content transition: preview detachment stops rapid-switch timer"); window.Close();
    }
    private static void VerifyIndependentControls()
    {
        var first = new TransitioningContentControl { Content = "Left" };
        var second = new TransitioningContentControl { Content = "Right" }; Grid.SetColumn(second, 1);
        var window = new Window { Width = 600, Height = 260, Content = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("*,*"), Children = { first, second } } };
        window.Show(); ControlPreview.Flush(); second.PageTransition = first.PageTransition;
        Check(ReferenceEquals(ContentTransitionOptions.GetConfiguredTransition(first), ContentTransitionOptions.GetConfiguredTransition(second)), "Content transition: shared transition configuration is preserved across independent controls");
        first.Content = "New left"; second.Content = "New right"; ControlPreview.Flush(); EnhancementsPreview.Advance(30);
        Motion.SetEnabled(first, false); ControlPreview.Flush(); Stable(first, "Independent first");
        Check(Current(second).Opacity is > 0 and < 1 && Motion.GetEnabled(second), "Content transition: disabling one control does not cancel another control sharing its transition");
        Settle(second); window.Close();
    }
    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(20); ControlPreview.Flush(); var card = gallery.FindControl<Border>("ContentTransitionCard")!;
        Check(card.IsVisible && card.Child is ContentTransitionDemoView, "Content transition category lazily loads native interaction preview");
        var demo = (ContentTransitionDemoView)card.Child!; var sample = demo.FindControl<TransitioningContentControl>("ContentTransitionSample")!;
        Click(window, demo.FindControl<Button>("NextContentPage")!); Settle(sample); Check(sample.Content is TransitionForm, "Content transition catalogue supports real page navigation");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 2; window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2; ControlPreview.Flush();
        Check(sample.Padding.Left == 20 && sample.ActualThemeVariant == ThemeVariant.Dark, "Content transition catalogue follows global theme and density");
        window.NavigateToCategory(0); gallery.Search("TransitioningContentControl"); ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("RefreshCard")!.IsVisible, "Content transition search opens dedicated preview without a comparison page"); gallery.Search("");
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo, sample) = Open(theme, size); var name = $"content-transition-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            RefreshPreview.Save(window, Path.Combine(output, name + ".png"));
            demo.SelectPage(1); ControlPreview.Flush(); EnhancementsPreview.Advance(45); RefreshPreview.Save(window, Path.Combine(output, name + "-fading.png"));
            Click(window, demo.FindControl<CheckBox>("ContentMotionEnabled")!); RefreshPreview.Save(window, Path.Combine(output, name + "-motion-off.png"));
            Click(window, demo.FindControl<Button>("LastTransitionRecord")!); RefreshPreview.Save(window, Path.Combine(output, name + "-records.png"));
            Click(window, demo.FindControl<CheckBox>("ContentMotionEnabled")!); demo.FindControl<ComboBox>("ContentTransitionPicker")!.SelectedIndex = 1;
            demo.SelectPage(0); ControlPreview.Flush(); EnhancementsPreview.Advance(45); RefreshPreview.Save(window, Path.Combine(output, name + "-sliding.png")); window.Close();
        }
        var (compact, _, _) = Open(ThemeVariant.Dark, ControlSize.Normal, 680); RefreshPreview.Save(compact, Path.Combine(output, "content-transition-compact.png")); compact.Close();
    }
    private static ContentPresenter[] Presenters(TransitioningContentControl sample) => sample.GetVisualDescendants().OfType<ContentPresenter>().Where(p => p.TemplatedParent == sample && p.Name is "PART_ContentPresenter" or "PART_ContentPresenter2").ToArray();
    private static ContentPresenter Current(TransitioningContentControl sample) => Presenters(sample).Single(p => p.IsVisible && ReferenceEquals(p.Content, sample.Content));
    private static void Settle(TransitioningContentControl sample) { EnhancementsPreview.Advance(280); Stable(sample, "Settled content"); }
    private static void Stable(TransitioningContentControl sample, string label) => Check(Presenters(sample).Count(p => p.IsVisible && p.Content is not null) == 1 && Current(sample).Opacity == 1 && Current(sample).RenderTransform is null && Presenters(sample).Count(p => p.Content is not null) == 1, $"{label}: only current presenter remains with clean opacity and transform");
    private static void Bounded(ListBox list, string label) => Check(list.ItemCount == 1000 && list.ItemsPanelRoot is VirtualizingStackPanel && list.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: bounded native list realizes fewer than 80 of a thousand records");
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    private static byte[] Pixels(Window window, Control control)
    {
        ControlPreview.Flush(); using var bitmap = window.CaptureRenderedFrame()!; var origin = control.TranslatePoint(default, window)!.Value;
        var rect = new PixelRect((int)(origin.X * window.RenderScaling), (int)(origin.Y * window.RenderScaling), (int)(control.Bounds.Width * window.RenderScaling), (int)(control.Bounds.Height * window.RenderScaling));
        var stride = rect.Width * 4; var bytes = new byte[stride * rect.Height]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(rect, pin.AddrOfPinnedObject(), bytes.Length, stride); } finally { pin.Free(); } return bytes;
    }
}
