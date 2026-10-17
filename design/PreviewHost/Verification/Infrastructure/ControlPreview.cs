using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ControlPreview
{
    public static void Render(string outputDirectory, string[] controls)
    {
        if (controls.Length == 1 && controls[0] == "--all")
        {
            RenderAll(outputDirectory);
            return;
        }

        if (controls.Length == 0)
            throw new ArgumentException(
                "Specify control names (for example DatePicker TimePicker) or explicitly use --all.");
        TargetedControlPreview.Render(Path.GetFullPath(outputDirectory), controls);
    }

    private static void RenderAll(string outputDirectory)
    {
        var output = Path.GetFullPath(outputDirectory);
        Directory.CreateDirectory(output);
        DateTimePickerPreview.Render(output, new[] { "DatePicker", "TimePicker" });
        OneWindowChecks.Render(output);
        NotificationPreview.Render(output);
        ContentTransitionPreview.Render(output);
        RefreshPreview.Render(output);
        CarouselPreview.Render(output);
        TabStripPreview.Render(output);
        ButtonControlsPreview.Render(output);
        SelectionControlsPreview.Render(output);
        ToggleSwitchPreview.Render(output);
        RadioButtonPreview.Render(output);
        FlyoutPreview.Render(output);
        TextControlsPreview.Render(output);
        SplitButtonPreview.Render(output);
        LayoutControlsPreview.Render(output);
        DataGridPreview.Render(output);
        foreach (var (name, width, height) in new[] { ("wide", 1280, 1060), ("compact", 800, 800) })
        foreach (var dark in new[] { false, true })
        {
            var window = Open(width, height);
            window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = dark ? 2 : 1;
            Flush();
            Directory.CreateDirectory(output);
            var path = Path.Combine(output, $"{name}-{(dark ? "dark" : "light")}.png");
            using var bitmap = new RenderTargetBitmap(new PixelSize(width, height), new Vector(96, 96));
            bitmap.Render(window);
            bitmap.Save(path, PngBitmapEncoderOptions.Default);
            Console.WriteLine($"Rendered {path}");
            if (name == "wide")
            {
                NavigationPreview.Render(window, window.FindControl<LegacyControlGallery>("Gallery")!, output, dark);
                EnhancementsPreview.Render(window, window.FindControl<LegacyControlGallery>("Gallery")!, output, dark);
                InteractionRepairsPreview.Render(window, window.FindControl<LegacyControlGallery>("Gallery")!, output,
                    dark);
                window.NavigateToCategory(10);
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"datagrid-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(11);
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"layout-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(12);
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"splitbuttons-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(13);
                window.Height = 1240;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"text-controls-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(14);
                window.Height = 1120;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"flyout-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(15);
                window.Height = 1400;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"selection-comparison-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(16);
                window.Height = 1320;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"button-comparison-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(17);
                window.Height = 1060;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"tabstrip-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(18);
                window.Height = 1140;
                Flush();
                EnhancementsPreview.Save(window,
                    Path.Combine(output, $"carousel-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(19);
                window.Height = 1160;
                Flush();
                RefreshPreview.Save(window, Path.Combine(output, $"refresh-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(20);
                window.Height = 1120;
                Flush();
                RefreshPreview.Save(window,
                    Path.Combine(output, $"content-transition-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(21);
                window.Height = 1280;
                Flush();
                RefreshPreview.Save(window,
                    Path.Combine(output, $"notification-catalogue-{(dark ? "dark" : "light")}.png"));
                window.NavigateToCategory(22);
                window.Height = 1100;
                Flush();
                RefreshPreview.Save(window,
                    Path.Combine(output, $"onewindow-catalogue-{(dark ? "dark" : "light")}.png"));
            }

            window.Close();
        }
    }

    public static void Verify()
    {
        TextAlignmentPreview.Verify(Check);
        DataGridStage1Preview.Verify(Check);
        DataGridRightFrozenPreview.Verify(Check);
        DataGridSummaryPreview.Verify(Check);
        DataGridFilterPreview.Verify(Check);
        PreviewPagesPreview.Verify(Check);
        ChartPreview.Verify(Check);
        ControlVariantsPreview.Verify(Check);
        InfoCardPreview.Verify(Check);
        TagVerificationStepPreview.Verify(Check);
        NavMenuPreview.Verify(Check);
        ChartDemoPreview.Verify(Check);
        DateTimePickerPreview.Verify(Check);
        OneWindowChecks.Verify(Check);
        LoginWindowChecks.Verify(Check);
        NotificationPreview.Verify();
        ContentTransitionPreview.Verify();
        RefreshPreview.Verify();
        CarouselPreview.Verify();
        TabStripPreview.Verify();
        VerifyCoreControls();
    }

    // Shared by the complete suite and focused retries after a template/state change.
    public static void VerifyCoreControls()
    {
        ReferenceChromePreview.Verify();
        ButtonControlsPreview.Verify();
        SelectionControlsPreview.Verify();
        ToggleSwitchPreview.Verify();
        RadioButtonPreview.Verify();
        FlyoutPreview.Verify();
        TextControlsPreview.Verify();
        SplitButtonPreview.Verify();
        LayoutControlsPreview.Verify();
        DataGridBindingPreview.Verify(Check);
        DataGridPreview.Verify();
        InteractionRepairsPreview.Verify();
        var window = Open(1280, 1060);
        var gallery = window.FindControl<LegacyControlGallery>("Gallery")!;
        var save = gallery.FindControl<Button>("SaveButton")!;
        var point = save.TranslatePoint(new Point(save.Bounds.Width / 2, save.Bounds.Height / 2), window)!.Value;
        var presenter = save.GetVisualDescendants().OfType<ContentPresenter>()
            .Single(control => control.Name == "PART_ContentPresenter");

        Check(BrushColor(presenter.Background) == Color.Parse("#006061"),
            "Primary button uses OneTheme, not fallback Fluent");
        window.MouseMove(point);
        Flush();
        Check(BrushColor(presenter.Background) == Color.Parse("#006A6B"), "Pointer hover state");
        window.MouseDown(point, MouseButton.Left);
        Flush();
        Check(BrushColor(presenter.Background) == Color.Parse("#007475"), "Pointer pressed state");
        window.MouseUp(point, MouseButton.Left);
        Flush();
        Check(window.Model.Status.Contains("第 1 次"), "Mouse click executes the bound command");
        window.MouseMove(new Point(1, 1));

        var email = gallery.FindControl<TextBox>("EmailBox")!;
        Check(email.Focus(NavigationMethod.Tab), "TextBox can receive keyboard focus");
        email.SelectAll();
        window.KeyTextInput("invalid-email");
        Flush();
        Check(window.Model.Email == "invalid-email" && window.Model.HasErrors,
            "Text input updates compiled binding and validation");
        Check(DataValidationErrors.GetHasErrors(email), "Validation reaches the real TextBox error state");
        var border = email.GetVisualDescendants().OfType<Border>()
            .Single(control => control.Name == "PART_BorderElement");
        Check(BrushColor(border.BorderBrush) == Color.Parse("#99000D"), "Error border uses semantic danger colour");
        window.Model.Email = "hello@theone.dev";
        Flush();
        Check(!DataValidationErrors.GetHasErrors(email), "Validation clears when input is corrected");

        var check = gallery.FindControl<CheckBox>("CheckSample")!;
        var checkFrame = check.GetVisualDescendants().OfType<Border>().Single(part => part.Name == "PART_Border");
        var mixed = gallery.GetVisualDescendants().OfType<CheckBox>()
            .Single(item => item.IsThreeState && item.IsChecked is null);
        var mixedFrame = mixed.GetVisualDescendants().OfType<Border>().Single(part => part.Name == "PART_Border");
        Check(BrushColor(checkFrame.Background)?.A == 0 && BrushColor(mixedFrame.Background)?.A == 0,
            "Checked and mixed CheckBox labels have no selection background");
        InteractionRepairsPreview.Hover(window, check);
        Check(BrushColor(checkFrame.Background)?.A == 0,
            "Checked CheckBox hover retains a transparent label background");
        window.MouseMove(new Point(1, 1));
        Flush();
        check.Focus(NavigationMethod.Tab);
        window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
        window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
        Flush();
        Check(check.IsChecked == false, "Space toggles CheckBox");

        var slider = gallery.FindControl<Slider>("SliderSample")!;
        slider.Value = 81;
        Flush();
        Check(window.Model.Progress == 81 && gallery.FindControl<ProgressBar>("ProgressSample")!.Value == 81,
            "Slider updates the model and ProgressBar");

        var number = gallery.FindControl<NumericUpDown>("NumberSample")!;
        number.Focus(NavigationMethod.Tab);
        window.KeyPress(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
        window.KeyRelease(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
        Flush();
        Check(number.Value == 4, "NumericUpDown keyboard increment preserves spinner behaviour");

        var picker = gallery.FindControl<ComboBox>("PlatformPicker")!;
        picker.IsDropDownOpen = true;
        Flush();
        Check(picker.IsDropDownOpen, "ComboBox popup opens with the rewritten template");
        picker.SelectedIndex = 2;
        picker.IsDropDownOpen = false;
        Check(picker.SelectedIndex == 2, "ComboBox selection works");

        foreach (var control in new Control[]
                 {
                     gallery.FindControl<DatePicker>("DateSample")!,
                     gallery.FindControl<TimePicker>("TimeSample")!,
                     gallery.FindControl<CalendarDatePicker>("CalendarSample")!
                 })
        {
            var button = control.GetVisualDescendants().OfType<Button>()
                .Single(candidate => candidate.Name is "PART_FlyoutButton" or "PART_Button");
            var popup = control.GetVisualDescendants().OfType<Popup>().Single();
            button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            Flush();
            Check(popup.IsOpen && popup.Child!.Bounds.Height > 0,
                $"{control.GetType().Name} popup and presenter render");
            popup.IsOpen = false;
            Flush();
        }

        foreach (var theme in new[] { 2, 1, 2, 1 })
        {
            window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = theme;
            Flush();
            Check(BrushColor(presenter.Background) == Color.Parse(theme == 2 ? "#63D9DE" : "#006061"),
                "Existing template updates on live theme switch");
            Check(BrushColor(mixedFrame.Background)?.A == 0,
                "Mixed CheckBox label stays transparent on a live theme switch");
        }

        window.Model.DemoEnabled = false;
        Flush();
        Check(!save.IsEffectivelyEnabled && BrushColor(presenter.Foreground) == Color.Parse("#AAB1B1"),
            "Disabled state propagates to the visual template");
        window.Model.DemoEnabled = true;

        gallery.Search("TextBox");
        Flush();
        Check(
            gallery.FindControl<Border>("InputsCard")!.IsVisible &&
            !gallery.FindControl<Border>("ActionsCard")!.IsVisible,
            "Preview search filters the catalogue");
        gallery.Search("");
        gallery.SelectCategory(5);
        Flush();
        Check(
            gallery.FindControl<Border>("DatesCard")!.IsVisible &&
            !gallery.FindControl<Border>("InputsCard")!.IsVisible,
            "Preview navigation filters by category");
        gallery.SelectCategory(0);

        var expander = gallery.FindControl<Expander>("ExpanderSample")!;
        expander.IsExpanded = false;
        Flush();
        Check(!expander.IsExpanded, "Expander state changes");
        var scroll = gallery.FindControl<ScrollViewer>("ScrollSample")!;
        scroll.Offset = new Vector(0, 35);
        Flush();
        Check(scroll.Offset.Y > 0, "Rewritten ScrollViewer retains scrolling");

        var navigation = window.FindControl<NavMenu>("Navigation")!;
        Check(
            window.Model.NavigationRoots.Count(root => root.IsGroup) == 6 &&
            navigation.ItemsPanelRoot is VirtualizingStackPanel,
            "NavMenu/shell: six sections use bounded virtualized sidebar");
        window.NavigateToCategory(25);
        Flush();
        Check(
            window.Model.SelectedNavigation?.Category == 25 && gallery.FindControl<Border>("NavMenuPreviewHost") is
                { IsVisible: true, Child: NavMenuDemoView },
            "NavMenu/shell: grouped navigation loads interactive NavMenu preview");
        window.NavigateToCategory(23);
        Flush();
        Check(window.Model.SelectedNavigation?.Category == 23 && gallery.FindControl<Border>("ChartCard")!.IsVisible,
            "NavMenu/shell: grouped chart page retains its stable category");
        window.Model.NavigationCollapsed = true;
        Flush();
        Check(
            navigation.IsCollapsed && window.FindControl<Grid>("Shell")!.ColumnDefinitions[0].Width.Value ==
            navigation.CollapsedWidth + 16,
            "NavMenu/shell: main sidebar compact toggle uses current density rail width");
        window.NavigateToCategory(25);
        Flush();
        Check(window.Model.SelectedNavigation?.Category == 25 && navigation.IsSubmenuOpen,
            "NavMenu/shell: compact grouped navigation selects popup page through compiled binding");
        window.Model.NavigationCollapsed = false;
        Flush();
        Check(!navigation.IsSubmenuOpen && window.FindControl<Grid>("Shell")!.ColumnDefinitions[0].Width.Value == 256,
            "NavMenu/shell: expanding main sidebar closes popup and restores layout");
        window.NavigateToCategory(0);
        Flush();
        gallery.Search("NavMenu");
        Flush();
        Check(gallery.FindControl<Border>("NavMenuPreviewHost")!.IsVisible,
            "NavMenu/shell: global search discovers navigation preview");
        gallery.Search("");
        Flush();
        NavigationPreview.Verify(window, gallery);
        EnhancementsPreview.Verify(window, gallery);
        VirtualizationPreview.VerifyGallery(window, gallery);
        DataGridPreview.VerifyGallery(window, gallery);
        LayoutControlsPreview.VerifyGallery(window, gallery);
        SplitButtonPreview.VerifyGallery(window, gallery);
        TextControlsPreview.VerifyGallery(window, gallery);
        FlyoutPreview.VerifyGallery(window, gallery);
        TabStripPreview.VerifyGallery(window, gallery);
        CarouselPreview.VerifyGallery(window, gallery);
        RefreshPreview.VerifyGallery(window, gallery);
        ContentTransitionPreview.VerifyGallery(window, gallery);
        NotificationPreview.VerifyGallery(window, gallery);
        window.Close();
        Console.WriteLine("All control interaction checks passed.");
    }

    private static LegacyPreviewWindow Open(int width, int height)
    {
        var window = new LegacyPreviewWindow
            { Width = width, Height = height, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        window.FindControl<LegacyControlGallery>("Gallery")!.ResetScroll();
        Flush();
        return window;
    }

    private static Color? BrushColor(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    internal static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException($"Failed: {message}");
        Console.WriteLine($"PASS: {message}");
    }
}