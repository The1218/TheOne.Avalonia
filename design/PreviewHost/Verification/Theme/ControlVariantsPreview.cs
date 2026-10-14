using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ControlVariantsPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) Verify(theme, size, check);
    }

    private static Window Open(ControlVariantsFixture page, ThemeVariant theme, ControlSize size)
    {
        var window = new Window
        {
            Width = 1120, Height = 2000, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme,
            Content = new ScrollViewer { Content = new Border { Padding = new Thickness(24), Child = page } }
        };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false);
        window.Show(); Flush(); window.MouseMove(new Point(1, 1)); Flush();
        return window;
    }

    private static void Verify(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var page = new ControlVariantsFixture(); var window = Open(page, theme, size);
        var label = $"Theme/{theme.Key}/{size}";
        NovaPalettePreview.Verify(page, theme, label, check);
        T Named<T>(string name) where T : Control => page.FindControl<T>(name)!;
        var button = Named<Button>("ThemeGradientButton"); var presenter = Part<ContentPresenter>(button, "PART_ContentPresenter");
        check(Colour(Resource(page, "One.Accent")) == Color.Parse(theme == ThemeVariant.Light ? "#006061" : "#63D9DE") &&
              Colour(Resource(page, "One.Info")) != Colour(Resource(page, "One.Accent")), $"{label}: Theme accent and separate information color apply globally");
        check(Gradient(presenter.Background, Resource(page, "One.Gradient.Brand")) && Colour(presenter.Foreground) == Colour(Resource(page, "One.OnAccent")), $"{label}: gradient action has semantic text and native presenter");
        var point = Center(button); window.MouseMove(point); Flush();
        check(Gradient(presenter.Background, Resource(page, "One.Gradient.BrandHover")), $"{label}: pointer hover updates gradient");
        window.MouseDown(point, MouseButton.Left); Flush();
        check(Gradient(presenter.Background, Resource(page, "One.Gradient.BrandPressed")), $"{label}: pressed gradient remains distinct");
        window.MouseUp(point, MouseButton.Left); Flush();
        check(page.Model.Status.StartsWith("已执行 1 次") && Named<TextBlock>("ThemeCommandStatus").Text == page.Model.Status, $"{label}: real pointer click executes compiled command once");
        Click(Named<IconButton>("ThemeIconButton"));
        check(page.Model.Status.StartsWith("已执行 2 次"), $"{label}: IconButton inherits gradient without changing command behavior");
        var split = Named<SplitButton>("ThemeSplitButton");
        Click(Part<Button>(split, "PART_PrimaryButton"));
        Click(Part<Button>(split, "PART_SecondaryButton"));
        check(page.Model.Status.StartsWith("已执行 3 次") && split.Flyout!.IsOpen, $"{label}: native split primary command and secondary flyout work independently");
        split.Flyout!.Hide(); window.MouseMove(new Point(1, 1)); Flush();
        var soft = Part<ContentPresenter>(Named<Button>("ThemeSoftButton"), "PART_ContentPresenter");
        check(Colour(soft.Background) == Colour(Resource(page, "One.AccentSoft")) && Colour(soft.Foreground) == Colour(Resource(page, "One.Accent")), $"{label}: soft action uses quiet semantic colors");
        var toggle = Named<ToggleButton>("ThemeSoftToggle"); Click(toggle);
        var togglePresenter = Part<ContentPresenter>(toggle, "PART_ContentPresenter");
        check(toggle.IsChecked == true && Colour(togglePresenter.Foreground) == Colour(Resource(page, "One.Accent")), $"{label}: selected soft toggle keeps readable foreground during hover");
        window.MouseMove(new Point(1, 1)); Flush();
        check(Colour(togglePresenter.Background) == Colour(Resource(page, "One.Accent")) && Colour(togglePresenter.Foreground) == Colour(Resource(page, "One.OnAccent")), $"{label}: selected soft toggle remains distinct after pointer exit");
        var toggleSplit = Named<ToggleSplitButton>("ThemeToggleSplit");
        Click(Part<Button>(toggleSplit, "PART_PrimaryButton")); Click(Part<Button>(toggleSplit, "PART_SecondaryButton"));
        check(toggleSplit.IsChecked && toggleSplit.Flyout!.IsOpen, $"{label}: gradient toggle split keeps native checked state and flyout");
        toggleSplit.Flyout!.Hide(); window.MouseMove(new Point(1, 1)); Flush();
        var disabled = Part<ContentPresenter>(Named<Button>("ThemeDisabledButton"), "PART_ContentPresenter");
        check(Colour(disabled.Background) == Colour(Resource(page, "One.DisabledSurface")) && Colour(disabled.Foreground) == Colour(Resource(page, "One.DisabledText")), $"{label}: disabled gradient becomes neutral");
        var input = Named<TextBox>("ThemeFilledInput"); input.Text = "changed"; Flush();
        check(page.Model.Text == "changed" && Colour(input.Background) == Colour(Resource(page, "One.Subtle")), $"{label}: filled input keeps compiled two-way binding");
        var combo = Named<ComboBox>("ThemeFilledCombo"); Click(combo); Flush();
        check(combo.IsDropDownOpen && combo.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: filled combo preserves its native virtualized popup");
        combo.IsDropDownOpen = false;
        var number = Named<NumericUpDown>("ThemeFilledNumber"); number.Value = 42;
        var auto = Named<AutoCompleteBox>("ThemeFilledAutoComplete"); auto.Text = "workspace"; Flush();
        check(page.Model.Number == 42 && page.Model.Text == "workspace" && input.Text == "workspace" && Colour(auto.Background) == Colour(Resource(page, "One.Subtle")), $"{label}: numeric and auto-complete filled inputs retain compiled two-way updates");
        var error = Named<TextBox>("ThemeErrorInput");
        check(Colour(Part<Border>(error, "PART_BorderElement").BorderBrush) == Colour(Resource(page, "One.Danger")), $"{label}: filled appearance preserves validation border");
        check(Gradient(Named<ProgressBar>("ThemeGradientProgress").Foreground, Resource(page, "One.Gradient.Brand")), $"{label}: progress uses readable brand gradient");
        var glow = Named<InfoCard>("ThemeGlowCard"); var flat = Named<InfoCard>("ThemeFlatCard");
        check(Gradient(glow.HeaderBackground, Resource(page, "One.Gradient.Glow")) && flat.BorderThickness == new Thickness(0) && !flat.ShowHeaderSeparator && !flat.ShowFooterSeparator, $"{label}: card appearances preserve content slots with distinct chrome");
        var chart = Named<LineChart>("ThemeChart");
        Color?[] Palette() => new[] { chart.AccentBrush, chart.SecondaryBrush, chart.TertiaryBrush, chart.QuaternaryBrush, chart.FifthBrush, chart.SixthBrush }.Select(Colour).ToArray();
        check(chart.SeriesSource?.Count == 6 && Palette().Distinct().Count() == 6 && Palette().All(color => color.HasValue), $"{label}: six compiled series use six distinct semantic paints");
        var oldPalette = Palette();
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; Flush();
        check(!oldPalette.SequenceEqual(Palette()) && Gradient(presenter.Background, Resource(page, "One.Gradient.Brand")) && Gradient(glow.HeaderBackground, Resource(page, "One.Gradient.Glow")), $"{label}: existing charts, buttons and cards update during live theme switching");
        check(Colour(Named<Tag>("ThemeInformationTag").Foreground) == Colour(Resource(page, "One.Info")), $"{label}: information tag follows information rather than brand color");
        chart.FifthBrush = Brushes.Red; chart.SixthBrush = Brushes.Blue;
        using (var image = new MemoryStream())
        {
            chart.ExportPng(image, new Size(420, 260));
            check(image.Length > 500 && chart.FifthBrush == Brushes.Red && chart.SixthBrush == Brushes.Blue, $"{label}: export retains explicit fifth and sixth series overrides");
        }
        var grid = Named<DataGrid>("ThemeGrid");
        check(grid.BorderThickness == new Thickness(0) && grid.Columns.Count == 2 && grid.GetVisualDescendants().OfType<DataGridRow>().Any(), $"{label}: borderless grid retains native rows and compiled columns");
        var list = Named<ListBox>("ThemeList"); list.BringIntoView(); Flush();
        check(list.GetVisualDescendants().OfType<ListBoxItem>().Count() is > 0 and < 40, $"{label}: bounded thousand-item list remains virtualized");
        list.SelectedIndex = 999; list.ScrollIntoView(999); Flush();
        check(page.Model.SelectedIndex == 999 && list.ContainerFromIndex(999) is ListBoxItem { IsSelected: true } && list.GetVisualDescendants().OfType<ListBoxItem>().Count() < 40, $"{label}: distant selection updates VM with bounded recycled containers");
        list.SelectedIndex = 0; list.ScrollIntoView(0); Flush();
        check(list.ContainerFromIndex(0) is ListBoxItem { IsSelected: true } && list.ContainerFromIndex(999) is null, $"{label}: returning to first item recycles distant selected container");
        VerifyAdditional(page, window, label, check);
        window.Close();
    }

    private static void VerifyAdditional(ControlVariantsFixture page, Window window, string label, Action<bool, string> check)
    {
        T Named<T>(string name) where T : Control => page.FindControl<T>(name)!;
        var text = Named<TextBox>("UnderlinedText"); Click(text);
        var frame = Part<Border>(text, "PART_BorderElement");
        check(frame.BorderThickness == new Thickness(0, 0, 0, 2) && Colour(frame.BorderBrush) == Colour(Resource(page, "One.Focus")), $"{label}: underlined input retains a native focus line");
        text.Classes.Add("error"); Flush();
        check(Colour(frame.BorderBrush) == Colour(Resource(page, "One.Danger")), $"{label}: underlined input preserves validation color");
        text.Classes.Remove("error"); text.SelectAll(); window.KeyTextInput("underlined input"); Flush();
        check(page.Model.Text == "underlined input", $"{label}: real text entry preserves compiled two-way binding in underlined inputs");
        text.IsEnabled = false; Flush();
        check(Colour(text.Foreground) == Colour(Resource(page, "One.DisabledText")) && Colour(frame.Background) == Colour(Resource(page, "One.DisabledSurface")), $"{label}: underlined input retains native disabled text and surface");
        text.IsEnabled = true;
        var combo = Named<ComboBox>("UnderlinedCombo"); Click(combo);
        check(combo.IsDropDownOpen && combo.GetRealizedContainers().Count() is > 0 and < 80, $"{label}: underlined combo retains bounded native popup containers");
        combo.IsDropDownOpen = false;
        var number = Named<NumericUpDown>("UnderlinedNumber"); Click(Part<TextBox>(number, "PART_TextBox"));
        check(number.BorderThickness == new Thickness(0, 0, 0, 2), $"{label}: underlined spinner retains focus on the native editor");
        var auto = Named<AutoCompleteBox>("UnderlinedAutoComplete"); var autoText = Part<TextBox>(auto, "PART_TextBox");
        Click(autoText); autoText.SelectAll(); window.KeyTextInput("auto-complete input"); Flush();
        check(page.Model.Text == "auto-complete input" && Part<Border>(autoText, "PART_BorderElement").BorderThickness == new Thickness(0, 0, 0, 2), $"{label}: underlined auto-complete retains native editor and compiled updates");
        var date = Named<DatePicker>("FilledDate"); var time = Named<TimePicker>("FilledTime"); var calendar = Named<CalendarDatePicker>("FilledCalendar");
        check(Colour(date.Background) == Colour(Resource(page, "One.Subtle")) && Colour(time.Background) == Colour(Resource(page, "One.Subtle")) && Colour(calendar.Background) == Colour(Resource(page, "One.Subtle")), $"{label}: all filled date/time controls consume shared library colors");
        foreach (var picker in new Control[] { date, time })
        {
            Click(Part<Button>(picker, "PART_FlyoutButton"));
            var popup = Part<Popup>(picker, "PART_Popup");
            check(popup.IsOpen, $"{label}/{picker.GetType().Name}: filled appearance preserves native flyout opening");
            popup.IsOpen = false; picker.IsEnabled = false; Flush();
            var disabled = Part<ContentPresenter>(Part<Button>(picker, "PART_FlyoutButton"), "PART_ContentPresenter");
            check(Colour(disabled.Foreground) == Colour(Resource(page, "One.DisabledText")) && Colour(disabled.Background) == Colour(Resource(page, "One.DisabledSurface")), $"{label}/{picker.GetType().Name}: disabled filled picker uses neutral native chrome");
            picker.IsEnabled = true; Flush();
        }
        Click(Part<Button>(calendar, "PART_Button"));
        check(calendar.IsDropDownOpen, $"{label}: filled calendar preserves its native calendar popup");
        calendar.IsDropDownOpen = false;
        var card = Named<InfoCard>("OutlinedCard");
        check(Colour(card.BorderBrush) == Colour(Resource(page, "One.Accent")) && Colour(card.HeaderBackground) == Colour(Resource(page, "One.AccentSoft")), $"{label}: outlined cards use the library accent without replacing content presenters");
        card.IsEnabled = false; Flush();
        check(Colour(card.BorderBrush) == Colour(Resource(page, "One.Line")) && Colour(card.HeaderBackground) == Colour(Resource(page, "One.DisabledSurface")), $"{label}: disabled outlined card returns to neutral chrome");
        card.IsEnabled = true;
        var tabs = Named<TabControl>("PillsTabs"); tabs.BringIntoView(); Flush();
        var first = (TabItem)tabs.ContainerFromIndex(0)!;
        check(first.CornerRadius.TopLeft == 999 && !Part<Border>(first, "PART_SelectedPipe").IsVisible && Colour(Part<Border>(first, "PART_LayoutRoot").Background) == Colour(Resource(page, "One.Accent")) && Colour(Part<ContentPresenter>(first, "PART_ContentPresenter").Foreground) == Colour(Resource(page, "One.OnAccent")), $"{label}: pill tabs retain selected state with readable native headers");
        Click(first); tabs.IsEnabled = false; Flush();
        check(Colour(Part<Border>(first, "PART_LayoutRoot").Background) == Colour(Resource(page, "One.DisabledSurface")) && Colour(Part<ContentPresenter>(first, "PART_ContentPresenter").Foreground) == Colour(Resource(page, "One.DisabledText")), $"{label}: selected hovered pill headers become neutral when disabled");
        tabs.IsEnabled = true;
        tabs.SelectedIndex = 999; tabs.ScrollIntoView(999); Flush();
        check(page.Model.SelectedIndex == 999 && tabs.ContainerFromIndex(999) is TabItem { IsSelected: true } && tabs.GetRealizedContainers().Count() < 40, $"{label}: pill tabs virtualize and select the distant thousandth model");
        tabs.SelectedIndex = 0; tabs.ScrollIntoView(0); Flush();
        var list = Named<ListBox>("ThemeList"); list.BringIntoView(); Flush();
        var item = (ListBoxItem)list.ContainerFromIndex(0)!;
        check(item.Margin == new Thickness(0, 3) && Colour(Part<ContentPresenter>(item, "PART_ContentPresenter").BorderBrush) == Colour(Resource(page, "One.Accent")), $"{label}: recycled card list items keep the selection border");
        list.IsEnabled = false; Flush();
        check(Colour(Part<ContentPresenter>(item, "PART_ContentPresenter").Foreground) == Colour(Resource(page, "One.DisabledText")), $"{label}: selected card list items retain native disabled text");
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var window = Open(new ControlVariantsFixture(), theme, size);
            using var frame = window.CaptureRenderedFrame()!;
            var path = Path.Combine(output, $"control-variants-{theme.Key}-{size}.png");
            frame.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}"); window.Close();
        }
    }

    private static T Part<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().Single(part => part.Name == name && part.TemplatedParent == control);
    private static IBrush? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? value as IBrush : null;
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static bool Gradient(IBrush? brush, IBrush? expected) => brush is ILinearGradientBrush a && expected is ILinearGradientBrush b && a.GradientStops.Select(stop => stop.Color).SequenceEqual(b.GradientStops.Select(stop => stop.Color));
    private static Point Center(Control control) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), TopLevel.GetTopLevel(control)!)!.Value;
    private static void Click(Control control)
    {
        control.BringIntoView(); Flush(); var host = TopLevel.GetTopLevel(control)!; var point = Center(control);
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
