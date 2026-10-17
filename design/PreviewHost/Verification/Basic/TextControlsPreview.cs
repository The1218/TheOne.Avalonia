using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

internal static class TextControlsPreview
{
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyDensity(theme, size);
        VerifyInteraction();
        VerifyConsumer();
    }

    private static (Window Window, TextControlsDemoView Demo) Open(ThemeVariant theme, ControlSize size,
        int width = 1120)
    {
        var demo = new TextControlsDemoView();
        var window = new Window
        {
            Width = width, Height = 1080, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = demo }
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        return (window, demo);
    }

    private static void VerifyDensity(ThemeVariant theme, ControlSize size)
    {
        var (window, demo) = Open(theme, size);
        var label = demo.FindControl<Label>("NameLabel")!;
        var input = demo.FindControl<TextBox>("NameInput")!;
        var text = demo.FindControl<SelectableTextBlock>("SelectionSample")!;
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        var lineHeight = size switch { ControlSize.Small => 18, ControlSize.Large => 24, _ => 21 };
        var name = $"Text/{theme.Key}/{size}";
        Check(
            label.FontSize == font && label.Bounds.Height == height && input.Bounds.Height == height &&
            label.VerticalContentAlignment == VerticalAlignment.Center,
            $"{name}: native labels align with form fields at the correct density");
        Check(
            Part<ContentPresenter>(label, "PART_ContentPresenter").RecognizesAccessKey && label.Target == input &&
            !KeyboardNavigation.GetIsTabStop(label),
            $"{name}: native Label target and access-key presenter are retained without another tab stop");
        Check(
            Colour(label.Foreground) == Resource(label, "One.Text") &&
            Colour(demo.FindControl<Label>("RoleLabel")!.Foreground) == Resource(label, "One.Muted"),
            $"{name}: default and muted labels use semantic theme colors");
        Check(
            text.FontSize == font && text.LineHeight == lineHeight && double.IsNaN(text.Height) &&
            text.TextLayout.TextLines.Count >= 3 && text.Bounds.Height >= lineHeight * 3,
            $"{name}: selectable paragraphs size from their native multiline layout and line-height tokens");
        Check(
            Colour(text.Foreground) == Resource(text, "One.Text") &&
            Colour(text.SelectionBrush) == Resource(text, "One.TextSelection") && text.SelectionForegroundBrush is null,
            $"{name}: selection uses a Theme highlight while retaining native text formatting");
        Check(
            text.Focusable && text.FocusAdorner is not null && text.ContextFlyout is MenuFlyout &&
            string.Equals(text.Cursor?.ToString(), "IBeam", StringComparison.OrdinalIgnoreCase),
            $"{name}: native selectable text retains focus, pointer selection and themed context menu");
        Click(window, label);
        Check(input.IsFocused, $"{name}: real label pointer activation focuses its associated input");
        text.Focus();
        text.SelectAll();
        ControlPreview.Flush();
        Check(
            text.SelectedText == demo.SampleText && text.CanCopy &&
            demo.FindControl<TextBlock>("SelectionMetrics")!.Text!.Contains($"{demo.SampleText.Length} 个字符"),
            $"{name}: native all-selection updates copy eligibility and live preview metrics");
        using (var bitmap =
               new RenderTargetBitmap(
                   new PixelSize((int)Math.Ceiling(text.Bounds.Width), (int)Math.Ceiling(text.Bounds.Height)),
                   new Vector(96, 96)))
        {
            bitmap.Render(text);
            var range = text.TextLayout.HitTestTextRange(0, 5).First();
            Check(Pixel(bitmap, (int)range.X + 2, (int)range.Bottom - 1) == Colour(text.SelectionBrush),
                $"{name}: actual Skia pixels use the selected-text highlight brush");
        }

        var rich = demo.FindControl<SelectableTextBlock>("RichSample")!;
        rich.Focus();
        rich.SelectAll();
        ControlPreview.Flush();
        Check(
            rich.SelectedText == rich.Inlines!.Text && rich.SelectionForegroundBrush is null && rich.TextLayout
                .TextLines.SelectMany(line => line.TextRuns).Any(run => run.Properties?.FontRenderingEmSize == 20),
            $"{name}: rich-text selection retains the explicitly sized native inline run");
        Check(
            Colour(demo.FindControl<SelectableTextBlock>("DisabledText")!.Foreground) ==
            Resource(text, "One.DisabledText"), $"{name}: disabled text uses the shared disabled foreground");
        window.Close();
    }

    private static void VerifyInteraction()
    {
        var (window, demo) = Open(ThemeVariant.Light, ControlSize.Normal);
        var nameInput = demo.FindControl<TextBox>("NameInput")!;
        var role = demo.FindControl<ComboBox>("RoleInput")!;
        var text = demo.FindControl<SelectableTextBlock>("SelectionSample")!;
        role.Focus();
        Press(window, Key.N, PhysicalKey.N, RawInputModifiers.Alt, "n");
        Check(nameInput.IsFocused, "Alt + N activates the native Label access key and focuses its target");
        Press(window, Key.R, PhysicalKey.R, RawInputModifiers.Alt, "r");
        Check(role.IsFocused, "Alt + R activates a different native Label target");
        var from = TextPoint(text, 0, window);
        var to = TextPoint(text, 5, window);
        window.MouseMove(from);
        window.MouseDown(from, MouseButton.Left);
        window.MouseMove(to, RawInputModifiers.LeftMouseButton);
        window.MouseUp(to, MouseButton.Left);
        ControlPreview.Flush();
        Check(text.IsFocused && text.SelectedText == "Theme" && text.CanCopy,
            "Real pointer dragging selects exactly the native text range");
        var word = TextPoint(text, 8, window);
        ClickAt(window, word);
        ClickAt(window, word);
        Check(text.SelectedText.Trim() == "makes", "Native double-click selects a word using text-layout hit testing");
        ClickAt(window, word);
        Check(text.SelectedText == demo.SampleText, "Native triple-click selects the complete paragraph");
        var copyGesture = Application.Current!.PlatformSettings!.HotkeyConfiguration.Copy.First();
        PressGesture(window, copyGesture);
        Check(demo.CopyCount == 1 && demo.LastCopiedText == demo.SampleText && ReadClipboard(window) == demo.SampleText,
            "Platform copy shortcut writes the selected text to the Headless clipboard and raises the native copy event");
        text.ClearSelection();
        ControlPreview.Flush();
        Check(!text.CanCopy && text.SelectedText == "", "Native ClearSelection resets selection and copy availability");
        PressGesture(window, Application.Current!.PlatformSettings!.HotkeyConfiguration.SelectAll.First());
        Check(text.SelectedText == demo.SampleText, "Platform select-all shortcut selects the native paragraph");
        Click(window, demo.FindControl<Button>("CopyText")!);
        Check(demo.CopyCount == 2 && text.SelectedText == demo.SampleText && ReadClipboard(window) == demo.SampleText,
            "Preview copy button preserves text focus and executes native clipboard copying");
        text.SelectionStart = 0;
        text.SelectionEnd = 5;
        ControlPreview.Flush();
        var point = TextPoint(text, 2, window);
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Right);
        window.MouseUp(point, MouseButton.Right);
        ControlPreview.Flush();
        var flyout = (PopupFlyoutBase)text.ContextFlyout!;
        var menu = (MenuFlyoutPresenter)flyout.Popup.Child!;
        Check(flyout.IsOpen && text.SelectedText == "Theme" && menu.GetRealizedContainers().Count() == 1,
            "Native text context menu preserves the selected range and uses the themed menu presenter");
        var copy = menu.GetRealizedContainers().OfType<MenuItem>().Single();
        Check(copy.IsEffectivelyEnabled && ControlSizing.GetSize(copy) == ControlSize.Normal && copy.Header is not null,
            "Context copy command follows native CanCopy and localized header resources");
        Click(TopLevel.GetTopLevel(menu)!, copy);
        Check(
            demo.CopyCount == 3 && demo.LastCopiedText == "Theme" && ReadClipboard(window) == "Theme" && !flyout.IsOpen,
            "Real context-copy click copies the selected range and closes the native flyout");
        text.Focus();
        text.ClearSelection();
        point = TextPoint(text, 25, window);
        window.MouseDown(point, MouseButton.Right);
        window.MouseUp(point, MouseButton.Right);
        ControlPreview.Flush();
        Check(!menu.GetRealizedContainers().OfType<MenuItem>().Single().IsEffectivelyEnabled,
            "Native context copy command is disabled when there is no selection");
        flyout.Hide();
        text.Focus();
        text.SelectAll();
        nameInput.Focus();
        ControlPreview.Flush();
        Check(!text.CanCopy && text.SelectedText == "",
            "Moving focus to another input preserves native selection-clearing behavior");
        Click(window, demo.FindControl<Button>("SelectTextAll")!);
        Check(text.SelectedText == demo.SampleText,
            "Actual preview all-selection button focuses and selects the paragraph");
        var rangeStart = text.SelectionStart;
        var rangeEnd = text.SelectionEnd;
        window.RequestedThemeVariant = ThemeVariant.Dark;
        ControlSizing.SetSize(window, ControlSize.Large);
        ControlPreview.Flush();
        Check(
            text.SelectionStart == rangeStart && text.SelectionEnd == rangeEnd && text.FontSize == 16 &&
            text.LineHeight == 24 && Colour(text.Foreground) == Color.Parse("#F2F2F2"),
            "Live theme and density changes preserve a valid native selected range");
        Click(window, demo.FindControl<CheckBox>("WrapText")!);
        ControlPreview.Flush();
        Check(text.TextWrapping == TextWrapping.NoWrap, "Actual wrapping checkbox changes native paragraph wrapping");
        Click(window, demo.FindControl<CheckBox>("TextDemoEnabled")!);
        Check(!text.IsEffectivelyEnabled && !demo.FindControl<Label>("NameLabel")!.IsEffectivelyEnabled,
            "Preview disabling affects labels and selectable text");
        var count = demo.CopyCount;
        Click(window, demo.FindControl<Button>("SelectTextAll")!);
        Click(window, demo.FindControl<Button>("CopyText")!);
        Check(demo.CopyCount == count && !text.IsFocused,
            "Disabled text cannot be selected or copied through the preview controls");
        Click(window, demo.FindControl<Button>("ResetTextDemo")!);
        Check(
            text.IsEffectivelyEnabled && text.TextWrapping == TextWrapping.Wrap && demo.CopyCount == 0 && !text.CanCopy,
            "Actual reset button restores the text interaction state");
        window.Close();
    }

    private static void VerifyConsumer()
    {
        var field = new TextBox { Text = "Consumer" };
        var label = new Label
        {
            Content = "_Consumer", Target = field, Classes = { "small" }, Padding = new Thickness(10, 2), FontSize = 18,
            BorderThickness = new Thickness(1), BorderBrush = Brushes.Red, CornerRadius = new CornerRadius(5)
        };
        var text = new SelectableTextBlock
        {
            Text = "Local style and native layout", Classes = { "large" }, FontSize = 17, LineHeight = 29,
            Padding = new Thickness(7), TextWrapping = TextWrapping.NoWrap, SelectionBrush = Brushes.Red,
            SelectionForegroundBrush = Brushes.White
        };
        var panel = new StackPanel { Spacing = 16 };
        panel.Children.Add(label);
        panel.Children.Add(field);
        panel.Children.Add(text);
        var window = new Window { Width = 460, Height = 260, Content = panel };
        window.Show();
        ControlPreview.Flush();
        Check(
            label.FontSize == 18 &&
            Part<ContentPresenter>(label, "PART_ContentPresenter").Padding == new Thickness(10, 2) &&
            Part<ContentPresenter>(label, "PART_ContentPresenter").CornerRadius.TopLeft == 5,
            "Plain native Label retains explicit font, padding, border and corner overrides");
        Check(
            text.FontSize == 17 && text.LineHeight == 29 && text.Padding == new Thickness(7) &&
            text.SelectionBrush == Brushes.Red && text.SelectionForegroundBrush == Brushes.White,
            "Plain native SelectableTextBlock retains local typography and selection overrides");
        text.ClearValue(TextBlock.LineHeightProperty);
        window.Resources["One.Text.Large.LineHeight"] = 35d;
        ControlPreview.Flush();
        Check(text.LineHeight == 35,
            "Consumer line-height resources update an existing selectable paragraph after removing its local override");
        window.Close();
    }

    public static void VerifyGallery(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(13);
        ControlPreview.Flush();
        var card = gallery.FindControl<Border>("TextControlsCard")!;
        Check(card.IsVisible && card.Child is TextControlsDemoView,
            "Text catalogue loads Label and SelectableTextBlock interaction previews on demand");
        var demo = (TextControlsDemoView)card.Child!;
        Click(window, demo.FindControl<Label>("NameLabel")!);
        Check(demo.FindControl<TextBox>("NameInput")!.IsFocused,
            "Catalogue label click activates the associated input through real pointer input");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 0;
        ControlPreview.Flush();
        Check(
            demo.FindControl<Label>("NameLabel")!.MinHeight == 30 &&
            demo.FindControl<SelectableTextBlock>("SelectionSample")!.FontSize == 12,
            "Global size selector updates native labels and selectable paragraphs");
        window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = 1;
        window.NavigateToCategory(0);
        gallery.Search("SelectableTextBlock");
        ControlPreview.Flush();
        Check(card.IsVisible && !gallery.FindControl<Border>("SplitButtonsCard")!.IsVisible,
            "Global SelectableTextBlock search finds the text interaction preview");
        gallery.Search("");
        ControlPreview.Flush();
    }

    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, demo) = Open(theme, size);
            var name = theme.Key!.ToString()!.ToLowerInvariant();
            var text = demo.FindControl<SelectableTextBlock>("SelectionSample")!;
            text.Focus();
            text.SelectionStart = 0;
            text.SelectionEnd = 37;
            ControlPreview.Flush();
            EnhancementsPreview.Save(window,
                Path.Combine(output, $"text-controls-{name}-{size.ToString().ToLowerInvariant()}.png"));
            if (size == ControlSize.Normal)
            {
                var rich = demo.FindControl<SelectableTextBlock>("RichSample")!;
                rich.Focus();
                rich.SelectAll();
                ControlPreview.Flush();
                EnhancementsPreview.Save(rich, Path.Combine(output, $"text-controls-{name}-rich-selection.png"));
                text.Focus();
                text.SelectAll();
                ControlPreview.Flush();
                var point = TextPoint(text, 2, window);
                window.MouseDown(point, MouseButton.Right);
                window.MouseUp(point, MouseButton.Right);
                ControlPreview.Flush();
                var flyout = (PopupFlyoutBase)text.ContextFlyout!;
                EnhancementsPreview.Save(flyout.Popup.Child!,
                    Path.Combine(output, $"text-controls-{name}-copy-menu.png"));
                flyout.Hide();
            }

            window.Close();
        }
    }

    private static Point TextPoint(SelectableTextBlock text, int index, TopLevel host)
    {
        var rect = text.TextLayout.HitTestTextPosition(index);
        return text.TranslatePoint(
            new Point(rect.X + text.Padding.Left + 0.2, rect.Y + text.Padding.Top + rect.Height / 2), host)!.Value;
    }

    private static void ClickAt(TopLevel host, Point point)
    {
        host.MouseMove(point);
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static void Press(TopLevel host, Key key, PhysicalKey physical,
        RawInputModifiers modifiers = RawInputModifiers.None, string? symbol = null)
    {
        host.KeyPress(key, modifiers, physical, symbol);
        host.KeyRelease(key, modifiers, physical, symbol);
        ControlPreview.Flush();
    }

    private static void PressGesture(TopLevel host, KeyGesture gesture)
    {
        var modifiers = RawInputModifiers.None;
        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Control)) modifiers |= RawInputModifiers.Control;
        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Meta)) modifiers |= RawInputModifiers.Meta;
        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Shift)) modifiers |= RawInputModifiers.Shift;
        if (gesture.KeyModifiers.HasFlag(KeyModifiers.Alt)) modifiers |= RawInputModifiers.Alt;
        Press(host, gesture.Key, gesture.Key == Key.A ? PhysicalKey.A : PhysicalKey.C, modifiers,
            gesture.Key.ToString().ToLowerInvariant());
    }

    private static string? ReadClipboard(TopLevel window)
    {
        var task = window.Clipboard!.TryGetTextAsync();
        if (!task.IsCompleted) EnhancementsPreview.AdvanceUntil(() => task.IsCompleted, 1000);
        return task.GetAwaiter().GetResult();
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