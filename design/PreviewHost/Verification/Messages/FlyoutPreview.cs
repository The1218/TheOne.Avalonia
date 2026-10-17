using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class FlyoutPreview
{
    private static (Window Window, FlyoutDemoView Demo) Open(ThemeVariant theme, ControlSize size)
    {
        var demo = new FlyoutDemoView();
        var window = new Window
        {
            Width = 1120, Height = 1080, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        return (window, demo);
    }

    private static T Find<T>(FlyoutDemoView demo, string name) where T : Control
    {
        return demo.FindControl<T>(name)!;
    }

    private static Flyout Flyout(FlyoutDemoView demo, string name)
    {
        return (Flyout)Find<Button>(demo, name).Flyout!;
    }

    private static FlyoutPresenter Presenter(Flyout flyout)
    {
        return (FlyoutPresenter)flyout.Popup.Child!;
    }

    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyDensity(theme, size);
        VerifyInteraction();
        VerifyConsumer();
    }

    private static void VerifyDensity(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var flyout = Flyout(demo, "OpenEditor");
        Click(window, Find<Button>(demo, "OpenEditor"));
        var presenter = Presenter(flyout);
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var padding = size switch { ControlSize.Small => 12, ControlSize.Large => 20, _ => 16 };
        var name = $"Flyout/{theme.Key}/{size}";
        Check(flyout.IsOpen && presenter.Bounds.Height > 0 && presenter.Bounds.Height < presenter.MaxHeight,
            $"{name}: actual native Flyout opens with content-driven height");
        Check(
            presenter.FontSize == font && presenter.Padding == new Thickness(padding) &&
            ControlSizing.GetSize(Find<TextBox>(demo, "WorkspaceName")) == size,
            $"{name}: native popup presenter and its form inherit the three density sizes");
        Check(
            Colour(presenter.Background) == Resource(presenter, "One.Surface") &&
            Colour(presenter.Foreground) == Resource(presenter, "One.Text") &&
            Colour(presenter.BorderBrush) == Resource(presenter, "One.OutlineStrong"),
            $"{name}: native popup uses TheOne semantic surface, text and border colors");
        Check(
            Part<ContentPresenter>(presenter, "PART_ContentPresenter").Content == flyout.Content &&
            Part<Border>(presenter, "LayoutRoot").ClipToBounds &&
            !Part<Border>(presenter, "PART_Outline").IsHitTestVisible,
            $"{name}: native content contract, rounded clipping and overlay outline are retained");
        Check(presenter.CornerRadius == new CornerRadius(8) && presenter.MaxWidth == 640 && presenter.MaxHeight == 480,
            $"{name}: floating content uses bounded theme dimensions and the common popup radius");
        Check(Find<TextBox>(demo, "WorkspaceName").IsFocused, $"{name}: Standard mode focuses the first native input");
        flyout.Hide();
        Click(window, Find<Button>(demo, "OpenEditor"));
        Check(presenter.FontSize == font && presenter.Padding == new Thickness(padding),
            $"{name}: reopening preserves inherited presenter density");
        flyout.Hide();
        Click(window, Find<Button>(demo, "OpenList"));
        var list = Find<ListBox>(demo, "MemberList");
        Check(list.ItemCount == 10_000 && list.GetRealizedContainers().Count() is > 0 and < 80,
            $"{name}: actual popup list virtualizes ten thousand data items in its bounded viewport");
        Click(TopLevel.GetTopLevel(Presenter(Flyout(demo, "OpenList")))!, Find<Button>(demo, "JumpLast"));
        ControlPreview.Flush();
        Check(
            list.SelectedIndex == 9999 && list.ContainerFromIndex(9999) is not null &&
            list.ContainerFromIndex(0) is null && list.GetRealizedContainers().Count() < 80,
            $"{name}: distant selection realizes the final item while recycling the first viewport");
        demo.UpdateMetrics();
        Check(Find<TextBlock>(demo, "ListMetrics").Text!.Contains("10,000"),
            $"{name}: interaction preview reports actual realized containers");
        Flyout(demo, "OpenList").Hide();
        window.Close();
    }

    private static void VerifyInteraction()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        var flyout = Flyout(demo, "OpenEditor");
        var open = Find<Button>(demo, "OpenEditor");
        Click(window, open);
        var presenter = Presenter(flyout);
        var popupHost = TopLevel.GetTopLevel(presenter)!;
        var input = Find<TextBox>(demo, "WorkspaceName");
        input.SelectAll();
        popupHost.KeyTextInput("TheOne design");
        ControlPreview.Flush();
        Click(popupHost, Find<Button>(demo, "ApplyEdit"));
        Check(!flyout.IsOpen && demo.ApplyCount == 1 && demo.AppliedName == "TheOne design",
            "Real popup text input and apply button commit the form and close native Flyout");
        Click(window, open);
        Check(ReferenceEquals(presenter, Presenter(flyout)) && input.Text == "TheOne design",
            "Reopening reuses the native presenter and restores the applied form state");
        popupHost = TopLevel.GetTopLevel(presenter)!;
        input.SelectAll();
        popupHost.KeyTextInput("Discard me");
        ControlPreview.Flush();
        Click(popupHost, Find<Button>(demo, "CancelEdit"));
        Click(window, open);
        Check(input.Text == "TheOne design" && demo.ApplyCount == 1,
            "Cancel leaves committed state unchanged when the popup reopens");
        input.SelectAll();
        popupHost.KeyTextInput("   ");
        ControlPreview.Flush();
        Click(popupHost, Find<Button>(demo, "ApplyEdit"));
        Check(flyout.IsOpen && input.IsFocused && demo.ApplyCount == 1,
            "Empty popup form validation keeps the flyout open and focuses its input");
        popupHost.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
        popupHost.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
        ControlPreview.Flush();
        Check(!flyout.IsOpen, "Native FlyoutPresenter handles Escape from its focused content");
        Click(window, open);
        ClickOutside(window);
        Check(!flyout.IsOpen, "Real outside pointer input light-dismisses the native Flyout");
        Find<CheckBox>(demo, "PreventDismiss").IsChecked = true;
        Click(window, open);
        ClickOutside(window);
        Check(flyout.IsOpen && Find<TextBlock>(demo, "FlyoutStatus").Text!.Contains("阻止"),
            "Native cancellable Closing keeps the popup open after an outside click");
        Click(TopLevel.GetTopLevel(presenter)!, Find<Button>(demo, "CancelEdit"));
        Check(!flyout.IsOpen, "Popup cancel button can explicitly close a flyout whose outside dismissal is blocked");
        Find<CheckBox>(demo, "PreventDismiss").IsChecked = false;
        foreach (var direction in Enumerable.Range(0, 4))
        {
            Find<ComboBox>(demo, "PlacementPicker").SelectedIndex = direction;
            Click(window, open);
            var expected = direction switch
            {
                1 => PlacementMode.Right, 2 => PlacementMode.Top, 3 => PlacementMode.Left, _ => PlacementMode.Bottom
            };
            Check(
                flyout.IsOpen && flyout.Popup.Placement == expected && flyout.Popup.PlacementTarget == open &&
                presenter.Bounds.Width > 0,
                $"Native {expected} placement opens from the real trigger and keeps platform constraint adjustment");
            flyout.Hide();
        }

        Find<ComboBox>(demo, "FlyoutSizePicker").SelectedIndex = 3;
        Click(window, open);
        Check(presenter.FontSize == 16 && input.MinHeight == 44,
            "FlyoutPresenterClasses can independently select large size and propagate it into content");
        flyout.Hide();
        Find<ComboBox>(demo, "FlyoutSizePicker").SelectedIndex = 0;
        Click(window, open);
        Check(presenter.FontSize == 14 && input.MinHeight == 36,
            "Reopening removes the earlier explicit presenter size and restores inherited normal density");
        window.RequestedThemeVariant = ThemeVariant.Dark;
        ControlSizing.SetSize(window, ControlSize.Small);
        ControlPreview.Flush();
        Check(
            flyout.IsOpen && presenter.FontSize == 12 && input.MinHeight == 30 &&
            Colour(presenter.Background) == Color.Parse("#0F0F0F"),
            "An open native popup updates its theme and inherited density without recreating the presenter");
        flyout.Hide();
        Find<ComboBox>(demo, "ShowModePicker").SelectedIndex = 1;
        Click(window, open);
        Check(flyout.ShowMode == FlyoutShowMode.Transient && open.IsFocused && !input.IsFocused,
            "Transient display preserves trigger focus using native Flyout behavior");
        flyout.Hide();
        Find<ComboBox>(demo, "ShowModePicker").SelectedIndex = 0;
        Click(window, Find<Button>(demo, "OpenList"));
        var listFlyout = Flyout(demo, "OpenList");
        var list = Find<ListBox>(demo, "MemberList");
        var host = TopLevel.GetTopLevel(Presenter(listFlyout))!;
        var scroll = list.GetVisualDescendants().OfType<ScrollViewer>().Single();
        var point = list.TranslatePoint(new Point(list.Bounds.Width / 2, list.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point);
        host.MouseWheel(point, new Vector(0, -5));
        ControlPreview.Flush();
        Check(scroll.Offset.Y > 0 && list.GetRealizedContainers().Count() < 80,
            "Real wheel input scrolls the nested virtual list while keeping its realization bounded");
        Click(host, Find<Button>(demo, "JumpLast"));
        host.KeyPress(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
        host.KeyRelease(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
        ControlPreview.Flush();
        Check(list.SelectedIndex == 9998 && list.ContainerFromIndex(9998) is not null,
            "Native keyboard navigation continues to operate at a distant recycled list item");
        Click(host, list.ContainerFromIndex(9999)!);
        Check(list.SelectedIndex == 9999, "Real pointer input selects the final virtualized data item");
        Click(host, Find<Button>(demo, "ConfirmMember"));
        Check(!listFlyout.IsOpen && Find<TextBlock>(demo, "MemberStatus").Text!.Contains("10000"),
            $"Confirming a distant item updates the page and closes its native content flyout (open={listFlyout.IsOpen}, status={Find<TextBlock>(demo, "MemberStatus").Text}, selected={list.SelectedIndex})");
        Click(window, Find<Button>(demo, "OpenList"));
        Check(list.ItemCount == 10_000 && list.SelectedIndex == 9999 && list.GetRealizedContainers().Count() < 80,
            "Reopening a content flyout preserves its model source and selected item with bounded realization");
        listFlyout.Hide();
        Click(window, Find<Button>(demo, "ResetFlyoutDemo"));
        Check(demo.ApplyCount == 0 && demo.AppliedName == "TheOne workspace" && list.SelectedIndex == -1,
            "Real reset control restores both flyout previews");
        window.Close();
    }

    private static void VerifyConsumer()
    {
        var presenter = new FlyoutPresenter
        {
            Classes = { "large" }, Content = "Consumer template", Padding = new Thickness(7), FontSize = 18,
            BorderThickness = new Thickness(2), BorderBrush = Brushes.Red, Background = Brushes.Blue,
            CornerRadius = new CornerRadius(6), MaxHeight = 180,
            ContentTemplate =
                new FuncDataTemplate<string>((value, _) => new TextBlock { Text = value, Name = "CustomFlyoutText" })
        };
        var window = new Window { Width = 420, Height = 260, Content = presenter };
        window.Show();
        ControlPreview.Flush();
        Check(
            presenter.FontSize == 18 &&
            Part<ContentPresenter>(presenter, "PART_ContentPresenter").Margin == new Thickness(7) &&
            presenter.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Name == "CustomFlyoutText"),
            "Native FlyoutPresenter retains local typography, padding and ContentTemplate overrides");
        Check(
            Part<Border>(presenter, "PART_Outline").BorderBrush == Brushes.Red &&
            Part<Border>(presenter, "LayoutRoot").CornerRadius.TopLeft == 6 && presenter.MaxHeight == 180,
            "Consumer outline, radius and bounded-height overrides flow into the native template");
        presenter.ClearValue(TemplatedControl.PaddingProperty);
        window.Resources["One.Flyout.Large.Padding"] = new Thickness(23);
        ControlPreview.Flush();
        Check(presenter.Padding == new Thickness(23),
            "Flyout density resources update existing controls after removing local overrides");
        window.Close();
        var flyoutTheme = new ControlTheme
        {
            TargetType = typeof(FlyoutPresenter),
            BasedOn = (ControlTheme)presenter.FindResource(typeof(FlyoutPresenter))!
        };
        flyoutTheme.Setters.Add(new Setter(TemplatedControl.BorderBrushProperty, Brushes.Red));
        flyoutTheme.Setters.Add(new Setter(TemplatedControl.CornerRadiusProperty, new CornerRadius(4)));
        var nativeFlyout = new Flyout
            { Content = new TextBlock { Text = "Custom native flyout theme" }, FlyoutPresenterTheme = flyoutTheme };
        var trigger = new Button { Content = "Custom theme", Flyout = nativeFlyout };
        window = new Window { Width = 420, Height = 260, Content = trigger };
        window.Show();
        ControlPreview.Flush();
        Click(window, trigger);
        Check(
            ReferenceEquals(Presenter(nativeFlyout).Theme, flyoutTheme) &&
            Presenter(nativeFlyout).BorderBrush == Brushes.Red && Presenter(nativeFlyout).CornerRadius.TopLeft == 4,
            "Native FlyoutPresenterTheme overrides can derive from the public TheOne control theme");
        nativeFlyout.Hide();
        window.Close();
        var longText = new TextBlock
            { Text = string.Join('\n', Enumerable.Repeat("Bounded native content scrolling", 100)) };
        presenter = new FlyoutPresenter
        {
            Content = longText, HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top, MaxHeight = 180
        };
        window = new Window { Width = 420, Height = 260, Content = presenter };
        window.Show();
        ControlPreview.Flush();
        var scroll = Part<ScrollViewer>(presenter, "PART_ScrollViewer");
        Check(presenter.Bounds.Height == 180 && scroll.Extent.Height > scroll.Viewport.Height,
            "Long non-list content uses the bounded native presenter ScrollViewer");
        scroll.ScrollToEnd();
        ControlPreview.Flush();
        Check(scroll.Offset.Y > 0, "Native flyout content can scroll to its end");
        presenter.Padding = new Thickness(0);
        presenter.Content = new Button
        {
            Content = "Edge button", HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch, Background = Brushes.Blue
        };
        ControlPreview.Flush();
        var button = (Button)presenter.Content;
        Hover(window, button);
        ControlPreview.Flush();
        using (var bitmap =
               new RenderTargetBitmap(new PixelSize((int)presenter.Bounds.Width, (int)presenter.Bounds.Height),
                   new Vector(96, 96)))
        {
            bitmap.Render(presenter);
            Check(
                Pixel(bitmap, bitmap.PixelSize.Width - 1, bitmap.PixelSize.Height / 2) ==
                Colour(presenter.BorderBrush) && Pixel(bitmap, bitmap.PixelSize.Width - 1, 0).A == 0,
                "Skia pixels preserve the outer stroke and transparent rounded corner above a hovered edge button");
        }

        window.Close();
    }

    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(14);
        ControlPreview.Flush();
        var card = gallery.FindControl<Border>("FlyoutCard")!;
        Check(card.IsVisible && card.Child is FlyoutDemoView,
            "Flyout catalogue lazily loads its interactive content preview");
        var demo = (FlyoutDemoView)card.Child!;
        Click(window, Find<Button>(demo, "OpenEditor"));
        var flyout = Flyout(demo, "OpenEditor");
        Check(flyout.IsOpen, "Flyout catalogue opens the editor using actual trigger input");
        flyout.Hide();
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0;
        ControlPreview.Flush();
        Check(Find<FlyoutPresenter>(demo, "InlinePresenter").FontSize == 12,
            "Global size selector updates the native inline flyout presenter");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0);
        gallery.Search("FlyoutPresenter");
        ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("TextControlsCard")!.IsVisible,
            "Global FlyoutPresenter search finds its dedicated interaction preview");
        gallery.Search("");
        ControlPreview.Flush();
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size);
            var prefix = $"flyout-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}";
            EnhancementsPreview.Save(window, Path.Combine(output, $"{prefix}.png"));
            Click(window, Find<Button>(demo, "OpenEditor"));
            var flyout = Flyout(demo, "OpenEditor");
            EnhancementsPreview.Save(Presenter(flyout), Path.Combine(output, $"{prefix}-editor.png"));
            flyout.Hide();
            Click(window, Find<Button>(demo, "OpenList"));
            flyout = Flyout(demo, "OpenList");
            Click(TopLevel.GetTopLevel(Presenter(flyout))!, Find<Button>(demo, "JumpLast"));
            demo.UpdateMetrics();
            ControlPreview.Flush();
            EnhancementsPreview.Save(Presenter(flyout), Path.Combine(output, $"{prefix}-virtual-list.png"));
            flyout.Hide();
            window.Close();
        }
    }

    private static void ClickOutside(Window window)
    {
        var point = new Point(2, 2);
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }

    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
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
}