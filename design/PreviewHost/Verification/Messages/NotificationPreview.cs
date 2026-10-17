using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class NotificationPreview
{
    private static (Window Window, NotificationDemoView Demo) Open(ThemeVariant theme, ControlSize size,
        int width = 1040)
    {
        var demo = new NotificationDemoView();
        var window = new Window
        {
            Width = width, Height = 1160, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo }
        };
        ControlSizing.SetSize(window, size);
        window.Show();
        ControlPreview.Flush();
        return (window, demo);
    }

    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyInteraction(theme, size);
        VerifyConsumer();
        VerifyOverlay();
        VerifyBoundedDelivery();
        VerifyDetach();
    }

    private static void VerifyInteraction(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var label = $"Notification/{theme.Key}/{size}";
        var toggle = demo.FindControl<CheckBox>("NotificationMotionEnabled")!;
        foreach (var type in Enum.GetValues<NotificationType>())
        {
            var model = demo.ShowNotification(type);
            ControlPreview.Flush();
            var card = demo.Cards.Single();
            var surface = Part<Panel>(card, "PART_AnimationHost");
            EnhancementsPreview.Advance(35);
            Check(
                card.GetType() == typeof(NotificationCard) && card.NotificationType == type &&
                ReferenceEquals(card.Content, model),
                $"{label}/{type}: manager creates the native card and preserves notification content/type");
            Check(surface.Opacity is > 0 and < 1 && !card.IsClosing && !card.IsClosed,
                $"{label}/{type}: native display lifecycle has a real intermediate opacity");
            var padding = size switch { ControlSize.Small => 8, ControlSize.Large => 12, _ => 10 };
            var badge = size switch { ControlSize.Small => 20, ControlSize.Large => 28, _ => 24 };
            Check(
                card.Padding == new Thickness(padding) &&
                card.FontSize == size switch { ControlSize.Small => 11, ControlSize.Large => 13, _ => 12 } &&
                Part<Border>(card, "PART_StatusBadge").Bounds.Width == badge &&
                Part<Button>(card, "PART_CloseButton").Bounds.Width == badge,
                $"{label}/{type}: inherited density controls padding, typography, icon badge and close button");
            var key = type switch
            {
                NotificationType.Success => "One.Success", NotificationType.Warning => "One.Warning",
                NotificationType.Error => "One.Danger", _ => "One.Info"
            };
            Check(
                Colour(Part<Panel>(card, "PART_HeaderBar").Background) == Resource(card, key + "Fill") &&
                Colour(Part<PathIcon>(card, "PART_StatusIcon").Foreground) == Resource(card, key) &&
                Colour(card.Background) == Resource(card, "One.Surface") &&
                Colour(card.Foreground) == Resource(card, "One.Text"),
                $"{label}/{type}: semantic type accents and common TheOne surface/text");
            var title = card.GetVisualDescendants().OfType<TextBlock>()
                .Single(text => text.Name == "PART_NotificationTitle");
            var message = card.GetVisualDescendants().OfType<TextBlock>()
                .Single(text => text.Name == "PART_NotificationMessage");
            Check(
                title.Text == model.Title && message.Text == model.Message && title.FontSize == card.FontSize &&
                message.FontSize == card.FontSize && Colour(message.Foreground) == Resource(card, "One.Text"),
                $"{label}/{type}: card-owned notification template renders title and message with inherited typography and foreground");
            Motion.SetEnabled(card, false);
            ControlPreview.Flush();
            Check(surface.Opacity == 1 && surface.RenderTransform is null && !card.IsClosed,
                $"{label}/{type}: disabling motion during display immediately settles without closing");
            var still = Pixels(window, card);
            EnhancementsPreview.Advance(80);
            Check(still.SequenceEqual(Pixels(window, card)),
                $"{label}/{type}: actual Skia card frames remain static after motion is disabled");
            var closed = demo.ClosedCount;
            var events = 0;
            card.NotificationClosed += (_, _) => events++;
            Click(window, Part<Button>(card, "PART_CloseButton"));
            Check(
                card.IsClosing && card.IsClosed && events == 1 && demo.ClosedCount == closed + 1 &&
                demo.Cards.Length == 0,
                $"{label}/{type}: real close-button click completes native close and removes the card exactly once with motion disabled");
            card.Close();
            ControlPreview.Flush();
            Check(events == 1, $"{label}/{type}: repeated close requests do not repeat the native closed event");
        }

        demo.ShowNotification(NotificationType.Success);
        ControlPreview.Flush();
        EnhancementsPreview.Advance(35);
        var interrupted = demo.Cards.Single();
        var interruptedHost = Part<Panel>(interrupted, "PART_AnimationHost");
        var initialOpacity = interruptedHost.Opacity;
        var initialY = interruptedHost.RenderTransform!.Value.M32;
        var interruptedEvents = 0;
        interrupted.NotificationClosed += (_, _) => interruptedEvents++;
        interrupted.Close();
        ControlPreview.Flush();
        Check(
            interruptedHost.Opacity <= initialOpacity + .05 &&
            interruptedHost.RenderTransform!.Value.M32 <= initialY + .5 && !interrupted.IsClosed,
            $"{label}: closing during entry continues from the current pose without a flash or position jump");
        EnhancementsPreview.AdvanceUntil(() => interrupted.IsClosed, 500);
        Check(interrupted.IsClosed && interruptedEvents == 1 && demo.Cards.Length == 0,
            $"{label}: reversing entry into exit releases the card and completes its native event exactly once");
        demo.ShowNotification(NotificationType.Information);
        ControlPreview.Flush();
        EnhancementsPreview.Advance(30);
        var entering = demo.Cards.Single();
        var host = Part<Panel>(entering, "PART_AnimationHost");
        Click(window, toggle);
        Check(host.Opacity == 1 && host.RenderTransform is null && !Motion.GetEnabled(entering) && !entering.IsClosing,
            $"{label}: manager-local motion switch interrupts all inherited display animations");
        Click(window, toggle);
        EnhancementsPreview.Advance(40);
        Check(host.Opacity == 1 && host.RenderTransform is null && Motion.GetEnabled(entering),
            $"{label}: re-enabling motion does not replay a displayed card");
        entering.Close();
        ControlPreview.Flush();
        EnhancementsPreview.Advance(35);
        Check(entering.IsClosing && !entering.IsClosed && host.Opacity is > 0 and < 1 && !entering.IsHitTestVisible,
            $"{label}: closing plays a short exit animation and rejects repeated pointer input");
        var before = demo.ClosedCount;
        Click(window, toggle);
        ControlPreview.Flush();
        Check(entering.IsClosed && demo.Cards.Length == 0 && demo.ClosedCount == before + 1,
            $"{label}: switching motion off during exit immediately finishes native removal and onClose callback");
        demo.ShowNotification(NotificationType.Success, TimeSpan.FromMilliseconds(180));
        ControlPreview.Flush();
        var automatic = demo.Cards.Single();
        EnhancementsPreview.Advance(70);
        Check(!automatic.IsClosing && Part<Panel>(automatic, "PART_AnimationHost").Opacity == 1,
            $"{label}: motion-disabled automatic notification remains until its expiration");
        EnhancementsPreview.AdvanceUntil(() => automatic.IsClosed, 600);
        Check(automatic.IsClosed && demo.Cards.Length == 0,
            $"{label}: native expiration still auto-closes while motion is disabled");
        Click(window, toggle);
        demo.FindControl<CheckBox>("AutoCloseNotifications")!.IsChecked = true;
        demo.FindControl<NumericUpDown>("NotificationSeconds")!.Value = .5m;
        Click(window, demo.FindControl<Button>("ShowWarningNotification")!);
        var timed = demo.Cards.Single();
        EnhancementsPreview.AdvanceUntil(() => timed.IsClosing, 800);
        Check(timed.IsClosing, $"{label}: real automatic-close option uses the selected native expiration");
        EnhancementsPreview.AdvanceUntil(() => timed.IsClosed, 500);
        Check(timed.IsClosed && demo.Cards.Length == 0,
            $"{label}: animated auto-close completes the same native lifecycle");
        demo.FindControl<CheckBox>("AutoCloseNotifications")!.IsChecked = false;
        demo.ShowNotification(NotificationType.Error);
        ControlPreview.Flush();
        EnhancementsPreview.Advance(240);
        var clicked = demo.Cards.Single();
        var clickedBefore = demo.ClickedCount;
        Click(window, Part<Border>(clicked, "PART_LayoutRoot"));
        EnhancementsPreview.AdvanceUntil(() => clicked.IsClosed, 500);
        Check(clicked.IsClosed && demo.ClickedCount == clickedBefore + 1,
            $"{label}: clicking notification body preserves the native onClick/close behavior");
        Motion.SetEnabled(demo.Manager, false);
        Click(window, demo.FindControl<Button>("ShowAllNotifications")!);
        Check(demo.Cards.Length == 4 && demo.Cards.Select(card => card.NotificationType).Distinct().Count() == 4,
            $"{label}: four-type preview stays within the live notification limit");
        before = demo.ClosedCount;
        Click(window, demo.FindControl<Button>("CloseAllNotifications")!);
        Check(demo.Cards.Length == 0 && demo.ClosedCount == before + 4,
            $"{label}: no-motion CloseAll safely removes native items after collection enumeration");
        demo.ShowNotification(NotificationType.Information);
        ControlPreview.Flush();
        var visible = demo.Cards.Single();
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large);
        ControlPreview.Flush();
        Check(
            visible.Padding.Left == (size == ControlSize.Large ? 8 : 12) &&
            Colour(visible.Background) == Resource(visible, "One.Surface") &&
            Colour(Part<PathIcon>(visible, "PART_StatusIcon").Foreground) == Resource(visible, "One.Info"),
            $"{label}: live theme and density update an existing notification");
        demo.Manager.ClearValue(Motion.EnabledProperty);
        Motion.SetEnabled(window, false);
        ControlPreview.Flush();
        Check(toggle.IsChecked == false && !Motion.GetEnabled(visible),
            $"{label}: preview switch follows inherited global motion settings");
        Click(window, demo.FindControl<Button>("ResetNotifications")!);
        ControlPreview.Flush();
        Check(
            demo.Cards.Length == 0 && !demo.Manager.IsSet(Motion.EnabledProperty) && !Motion.GetEnabled(demo.Manager) &&
            demo.ShownCount == 0 && demo.ClosedCount == 0,
            $"{label}: reset removes notifications and restores global preference without stale callbacks");
        Motion.SetEnabled(window, true);
        window.Width = 680;
        ControlPreview.Flush();
        demo.ShowNotification(NotificationType.Success);
        ControlPreview.Flush();
        var compact = demo.Cards.Single();
        Check(
            compact.Bounds.Width == (size == ControlSize.Large ? 280 : 360) &&
            demo.FindControl<Border>("NotificationPreviewArea")!.Bounds.Width <= 632,
            $"{label}: compact viewport fits the native notification card");
        Motion.SetEnabled(demo.Manager, false);
        demo.Manager.CloseAll();
        ControlPreview.Flush();
        window.Close();
    }

    private static void VerifyConsumer()
    {
        var content = new TextBox { Text = "自定义通知内容" };
        var card = new NotificationCard
        {
            Content = content, Width = 440, Padding = new Thickness(9), FontSize = 19, Foreground = Brushes.Red,
            Background = Brushes.Yellow, BorderBrush = Brushes.Blue, BorderThickness = new Thickness(3),
            CornerRadius = new CornerRadius(9)
        };
        var window = new Window { Width = 500, Height = 280, Content = card };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        Check(
            ReferenceEquals(Part<ContentPresenter>(card, "PART_ContentPresenter").Child, content) &&
            card.Padding.Left == 9 && card.FontSize == 19 &&
            Part<Border>(card, "PART_LayoutRoot").Background == Brushes.Yellow &&
            Part<Border>(card, "PART_Outline").BorderBrush == Brushes.Blue,
            "Notification: consumer content, size, padding, font, surface and border overrides retain precedence");
        var close = 0;
        card.NotificationClosed += (_, _) => close++;
        card.Close();
        ControlPreview.Flush();
        Check(card.IsClosing && card.IsClosed && close == 1 && !card.IsVisible,
            "Notification: standalone native card closes and raises its event without a manager");
        window.Close();
        card = new NotificationCard
        {
            Content = new Notification("", "只有正文", NotificationType.Warning, TimeSpan.Zero),
            ContentTemplate =
                new FuncDataTemplate<INotification>((model, _) => new TextBlock { Text = "Custom " + model!.Message })
        };
        window = new Window { Width = 500, Height = 240, Content = card };
        window.Show();
        ControlPreview.Flush();
        Check(card.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Custom 只有正文"),
            "Notification: explicit consumer ContentTemplate overrides the built-in notification template");
        NotificationMotion.SetEnterDuration(card, TimeSpan.Zero);
        NotificationMotion.SetExitDuration(card, TimeSpan.Zero);
        card.Close();
        ControlPreview.Flush();
        Check(card.IsClosed, "Notification: zero-duration animations close through the native event lifecycle");
        window.Close();
    }

    private static void VerifyOverlay()
    {
        var window = new Window { Width = 720, Height = 540, Content = new TextBlock { Text = "宿主窗口" } };
        ControlSizing.SetSize(window, ControlSize.Small);
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        var manager = new WindowNotificationManager(window) { MaxItems = 2 };
        ControlPreview.Flush();
        manager.Show(new Notification("窗口通知", "使用原生 AdornerLayer", NotificationType.Success, TimeSpan.Zero));
        ControlPreview.Flush();
        var card = manager.GetVisualDescendants().OfType<NotificationCard>().Single();
        Check(card.FontSize == 11 && !Motion.GetEnabled(card) && Part<Panel>(card, "PART_AnimationHost").Opacity == 1,
            "Notification: native window-overlay manager inherits window density and motion settings");
        foreach (var position in Enum.GetValues<NotificationPosition>())
        {
            manager.Position = position;
            ControlPreview.Flush();
            Check(card.Bounds.Width == 280 && card.IsEffectivelyVisible,
                $"Notification/{position}: native positional layout keeps the card visible");
        }

        manager.Close(card.Content!);
        ControlPreview.Flush();
        Check(card.IsClosed && !manager.GetVisualDescendants().OfType<NotificationCard>().Any(),
            "Notification: native Close(content) safely removes overlay content with motion disabled");
        window.Close();
    }

    private static void VerifyBoundedDelivery()
    {
        var (window, demo) = Open(ThemeVariant.Dark, ControlSize.Normal);
        Motion.SetEnabled(demo.Manager, false);
        var bounded = true;
        for (var i = 0; i < 1000; i++)
        {
            demo.ShowNotification((NotificationType)(i % 4));
            ControlPreview.Flush();
            bounded &= demo.Cards.Length <= 4 && demo.Cards.All(card => !card.IsClosing);
        }

        Check(bounded && demo.ShownCount == 1000 && demo.ClosedCount == 996,
            "Notification: a thousand sequential deliveries keep at most four live native cards and release evicted controls");
        demo.Manager.CloseAll();
        ControlPreview.Flush();
        Check(demo.Cards.Length == 0 && demo.ClosedCount == 1000,
            "Notification: bounded delivery completes every native close callback exactly once");
        window.Close();
    }

    private static void VerifyDetach()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        demo.ShowNotification(NotificationType.Information, TimeSpan.FromMilliseconds(180));
        ControlPreview.Flush();
        var card = demo.Cards.Single();
        window.Close();
        EnhancementsPreview.Advance(400);
        Check(
            demo.Cards.Length == 0 && demo.ClosedCount == 0 &&
            !Part<Panel>(card, "PART_AnimationHost").IsAnimating(Visual.OpacityProperty),
            "Notification: detachment releases manager children, stops motion and ignores stale preview callbacks");
    }

    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(21);
        ControlPreview.Flush();
        var card = gallery.FindControl<Border>("NotificationCardPreview")!;
        Check(card.IsVisible && card.Child is NotificationDemoView,
            "Notification category lazily loads the native interactive preview");
        var demo = (NotificationDemoView)card.Child!;
        Click(window, demo.FindControl<Button>("ShowSuccessNotification")!);
        Check(demo.Cards.Length == 1 && demo.Cards[0].NotificationType == NotificationType.Success,
            "Notification catalogue supports native notification delivery");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0;
        window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2;
        ControlPreview.Flush();
        Check(demo.Cards[0].FontSize == 11 && demo.Cards[0].ActualThemeVariant == ThemeVariant.Dark,
            "Notification catalogue follows global theme and density");
        Motion.SetEnabled(demo.Manager, false);
        demo.Manager.CloseAll();
        ControlPreview.Flush();
        window.NavigateToCategory(0);
        gallery.Search("NotificationCard");
        ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("ContentTransitionCard")!.IsVisible,
            "Notification search opens the interaction page without a comparison matrix");
        gallery.Search("");
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size);
            var name = $"notification-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            Click(window, demo.FindControl<Button>("ShowAllNotifications")!);
            EnhancementsPreview.Advance(240);
            RefreshPreview.Save(window, Path.Combine(output, name + "-types.png"));
            var last = demo.Cards.Last();
            last.Close();
            ControlPreview.Flush();
            EnhancementsPreview.Advance(65);
            RefreshPreview.Save(window, Path.Combine(output, name + "-closing.png"));
            Motion.SetEnabled(demo.Manager, false);
            demo.Manager.CloseAll();
            ControlPreview.Flush();
            demo.ShowNotification(NotificationType.Information);
            ControlPreview.Flush();
            RefreshPreview.Save(window, Path.Combine(output, name + "-motion-off.png"));
            demo.Manager.CloseAll();
            ControlPreview.Flush();
            Motion.SetEnabled(demo.Manager, true);
            demo.ShowNotification(NotificationType.Success);
            ControlPreview.Flush();
            EnhancementsPreview.Advance(55);
            RefreshPreview.Save(window, Path.Combine(output, name + "-opening.png"));
            window.Close();
        }

        var (compact, demoCompact) = Open(ThemeVariant.Dark, ControlSize.Normal, 680);
        Motion.SetEnabled(demoCompact.Manager, false);
        demoCompact.ShowNotification(NotificationType.Warning);
        ControlPreview.Flush();
        RefreshPreview.Save(compact, Path.Combine(output, "notification-compact.png"));
        compact.Close();
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }

    private static byte[] Pixels(Window window, Control control)
    {
        ControlPreview.Flush();
        using var bitmap = window.CaptureRenderedFrame()!;
        var origin = control.TranslatePoint(default, window)!.Value;
        var rect = new PixelRect((int)(origin.X * window.RenderScaling), (int)(origin.Y * window.RenderScaling),
            (int)(control.Bounds.Width * window.RenderScaling), (int)(control.Bounds.Height * window.RenderScaling));
        var stride = rect.Width * 4;
        var bytes = new byte[stride * rect.Height];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(rect, pin.AddrOfPinnedObject(), bytes.Length, stride);
        }
        finally
        {
            pin.Free();
        }

        return bytes;
    }
}