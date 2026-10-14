using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.InteractionRepairsPreview;

namespace TheOne.DesignPreview;

// Bare official controls exercise defaults, independently of the opt-in visual variants.
internal static class ReferenceChromePreview
{
    private sealed record Sample(Window Window, TextBox Text, TextBox ReadOnly, Button Button,
        ListBox List, ComboBox Combo, CheckBox Check, Calendar Calendar, FlyoutPresenter Flyout, ToolTip Tip, LineChart Chart, NavMenu Nav);
    private static Sample Open(ThemeVariant theme, ControlSize size)
    {
        var text = new TextBox { PlaceholderText = "输入内容", Width = 260 };
        var readOnly = new TextBox { Text = "只读内容", IsReadOnly = true, Width = 260 };
        var button = new Button { Content = "普通按钮" };
        var list = new ListBox { ItemsSource = new[] { "默认条目", "选中条目", "其他条目" }, SelectedIndex = 1, Width = 260, Height = 140 };
        var combo = new ComboBox { ItemsSource = new[] { "下拉选项", "其他选项" }, SelectedIndex = 0, Width = 260 };
        var check = new CheckBox { Content = "启用功能" };
        var calendar = new Calendar { SelectedDate = new DateTime(2026, 10, 14) };
        var flyout = new FlyoutPresenter { Content = "弹层内容 / Popup", Width = 260 };
        var tip = new ToolTip { Content = "提示 / Tooltip", Opacity = 1 };
        var chart = new LineChart { Width = 340, Height = 220, Title = "图表专用主题色", SeriesSource = new[] { new ChartSeries { Name = "趋势", ItemsSource = new[] { new ChartPoint(0, 2), new ChartPoint(1, 5), new ChartPoint(2, 3) } } } };
        var nav = new NavMenu { ItemsSource = new[] { "概览", "设置" }, Width = 260, Height = 140 };
        var left = new StackPanel { Spacing = 16, Children = { button, text, readOnly, combo, check, new RadioButton { Content = "选择选项", IsChecked = true }, new ToggleSwitch { OnContent = "已开启", IsChecked = true }, list, nav } };
        var right = new StackPanel { Spacing = 16, Children = { calendar, flyout, tip, chart } };
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 32, Children = { left, right } }; Grid.SetColumn(right, 1);
        var window = new Window { Width = 820, Height = 980, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new StackPanel { Margin = new Thickness(28), Spacing = 24, Children = { new TextBlock { Text = $"Nova · {theme.Key} · {size}", FontSize = 22 }, grid } } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        return new(window, text, readOnly, button, list, combo, check, calendar, flyout, tip, chart, nav);
    }
    public static void Verify()
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var s = Open(theme, size); var label = $"ReferenceChrome/{theme.Key}/{size}";
            NovaPalettePreview.Verify(s.Window, theme, label, Check);
            var chrome = Part<Border>(s.Text, "PART_BorderElement");
            Check(chrome.CornerRadius == new CornerRadius(6) && chrome.BorderThickness == new Thickness(1) && Colour(chrome.BorderBrush)?.A == 0 && Colour(chrome.Background) == Resource(s.Text, "One.Subtle"), $"{label}: native editor uses a six-pixel radius and neutral fill without an idle outline");
            Check(Colour(s.Text.PlaceholderForeground) == Resource(s.Text, "One.Placeholder"), $"{label}: placeholder uses the original tertiary text colour");
            Hover(s.Window, s.Text);
            Check(Colour(chrome.Background) == Resource(s.Text, "One.NeutralHover") && Colour(chrome.BorderBrush)?.A == 0, $"{label}: editor hover retains the neutral reference chrome");
            s.Text.Focus(NavigationMethod.Tab); ControlPreview.Flush();
            Check(chrome.BorderThickness == new Thickness(1) && Colour(chrome.BorderBrush) == Resource(s.Text, "One.Focus") && Colour(chrome.Background) == Resource(s.Text, "One.Subtle"), $"{label}: focused editor has a one-pixel focus outline");
            Hover(s.Window, s.ReadOnly); s.ReadOnly.Focus(); ControlPreview.Flush();
            Check(Colour(Part<Border>(s.ReadOnly, "PART_BorderElement").Background) == Resource(s.ReadOnly, "One.Subtle") && Colour(Part<Border>(s.ReadOnly, "PART_BorderElement").BorderBrush)?.A == 0, $"{label}: read-only editor retains its neutral fill on hover and focus");
            Check(Part<ContentPresenter>(s.Button, "PART_ContentPresenter").CornerRadius == new CornerRadius(6) && s.List.CornerRadius == new CornerRadius(6) && s.List.BorderThickness == new Thickness(0), $"{label}: native button and list frame use reference metrics");
            var selected = (ListBoxItem)s.List.ContainerFromIndex(1)!;
            Check(selected.CornerRadius == new CornerRadius(size == ControlSize.Small ? 4 : 6) && Colour(Part<ContentPresenter>(selected, "PART_ContentPresenter").Background) == Resource(selected, "One.AccentSoft"), $"{label}: list item density and selected fill match the reference");
            Check(Part<Border>(s.Check, "NormalRectangle").CornerRadius == new CornerRadius(3), $"{label}: checkbox has a three-pixel glyph radius");
            Check(s.Calendar.CornerRadius == new CornerRadius(8) && s.Flyout.CornerRadius == new CornerRadius(8) && Colour(s.Flyout.BorderBrush) == Resource(s.Flyout, "One.OutlineStrong"), $"{label}: calendar and popup use eight-pixel corners and the strong popup outline");
            Check(s.Tip.CornerRadius == new CornerRadius(4) && Colour(s.Tip.Foreground) == Resource(s.Tip, "One.Muted"), $"{label}: tooltip uses four-pixel corners and secondary text");
            Check(Colour(s.Chart.Background) == Resource(s.Chart, "One.Chart.Background") && Colour(s.Chart.Foreground) == Resource(s.Chart, "One.Chart.Text") && Colour(s.Chart.GridBrush) == Resource(s.Chart, "One.Chart.Grid"), $"{label}: chart uses its own reference background, text and grid roles");
            var navItem = (NavMenuItem)s.Nav.ContainerFromIndex(0)!;
            var navChrome = Part<Border>(navItem, "PART_LayoutRoot");
            Hover(s.Window, navChrome);
            Check(Colour(navChrome.Background) == Resource(navItem, "One.Subtle"),
                $"{label}: navigation hover uses reference secondary fill (actual {Colour(navChrome.Background)}, expected {Resource(navItem, "One.Subtle")})");
            s.Combo.IsDropDownOpen = true; ControlPreview.Flush();
            var popup = Part<Popup>(s.Combo, "PART_Popup");
            Check(popup.IsOpen && popup.Child is Border { CornerRadius.TopLeft: 8 } border && Colour(border.BorderBrush) == Resource(s.Combo, "One.OutlineStrong"), $"{label}: native combo popup uses reference corners and outline");
            var comboItem = (ComboBoxItem)s.Combo.ContainerFromIndex(0)!;
            Check(Colour(Part<ContentPresenter>(comboItem, "PART_ContentPresenter").Foreground) == Resource(comboItem, "One.Text"), $"{label}: selected native combo item retains primary text colour");
            s.Combo.IsDropDownOpen = false; s.Text.CornerRadius = new CornerRadius(11); s.Text.BorderBrush = Brushes.Red; ControlPreview.Flush();
            s.Text.Focus(); ControlPreview.Flush();
            Check(chrome.CornerRadius == new CornerRadius(11), $"{label}: explicit consumer radius survives native focus state");
            s.Window.Close();
        }
    }
    public static void Render(string output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var s = Open(theme, size);
            EnhancementsPreview.Save(s.Window, Path.Combine(output, $"reference-chrome-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}.png"));
            s.Window.Close();
        }
    }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
}
