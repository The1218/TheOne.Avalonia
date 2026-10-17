using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;
using Path = System.IO.Path;

namespace TheOne.DesignPreview;

internal static class ColorPickerPreview
{
    public static void VerifyModelSpacing(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 480, 320 })
        {
            var view = new ColorView { SelectedIndex = 0, Color = Color.Parse("#80006061") };
            var picker = new ColorPicker { SelectedIndex = 0, Color = view.Color };
            var root = new StackPanel { Spacing = 12, Children = { picker, view } };
            ControlSizing.SetSize(root, size);
            var window = Open(new Border { Padding = new Thickness(16), Child = root }, width, 840, theme);
            try
            {
                VerifyModelGroup(view, view, $"inline/{theme}/{size}/{width}");
                if (output != null)
                {
                    using var frame = window.CaptureRenderedFrame()!;
                    Save(frame, output, $"colorpicker-model-{theme}-{size}-{width}.png");
                }

                var button = picker.GetVisualDescendants().OfType<DropDownButton>().Single();
                Click(window, button, .5, .5);
                check(button.Flyout?.IsOpen == true,
                    $"ColorPicker/model: {theme}/{size}/{width} opens with native pointer input");
                if (button.Flyout is Flyout flyout && flyout.Content is Control content)
                {
                    Part<TabControl>(content, "PART_TabControl").SelectedIndex = 0;
                    Flush();
                    VerifyModelGroup(content, picker, $"popup/{theme}/{size}/{width}");
                    if (output != null && TopLevel.GetTopLevel(content) is { } popup)
                    {
                        using var frame = popup.CaptureRenderedFrame()!;
                        Save(frame, output, $"colorpicker-model-popup-{theme}-{size}-{width}.png");
                    }
                }
                else
                {
                    check(false, "ColorPicker/model: native popup content exists");
                }

                button.Flyout?.Hide();
                Flush();
            }
            finally
            {
                window.Close();
                Flush();
            }
        }

        void VerifyModelGroup(Control content, ColorView view, string label)
        {
            WaitFor(() =>
                content.GetVisualDescendants().OfType<Grid>().Any(g => g.Classes.Contains("one-color-components")));
            var hsv = Part<RadioButton>(content, "HsvRadioButton");
            var model = (Grid)hsv.GetVisualParent()!;
            var rgb = model.Children.OfType<RadioButton>().Single(r => r != hsv);
            var gap = hsv.Bounds.Left - rgb.Bounds.Right;
            check(Math.Abs(gap - 4) < .1, $"ColorPicker/model: {label} RGB and HSV have a 4 px gap (actual={gap})");
            check(rgb.Bounds.Width > 0 && hsv.Bounds.Right <= model.Bounds.Width + .1 &&
                  Math.Abs(rgb.Bounds.Height - hsv.Bounds.Height) < .1,
                $"ColorPicker/model: {label} buttons fit their shared model column and align vertically");
            foreach (var radio in new[] { rgb, hsv })
            {
                var presenter = Part<ContentPresenter>(radio, "PART_ContentPresenter");
                check(
                    presenter.Child is { } child && child.DesiredSize.Width <=
                    presenter.Bounds.Width - presenter.Padding.Left - presenter.Padding.Right + .1,
                    $"ColorPicker/model: {label}/{radio.Content} label fits without clipping");
            }

            var initial = view.Color;
            var host = TopLevel.GetTopLevel(content)!;
            var point = hsv.TranslatePoint(new Point(hsv.Bounds.Width / 2, hsv.Bounds.Height / 2), host)!.Value;
            host.MouseDown(point, MouseButton.Left);
            host.MouseUp(point, MouseButton.Left);
            Flush();
            check(view.ColorModel == ColorModel.Hsva && hsv.IsChecked == true && rgb.IsChecked != true,
                $"ColorPicker/model: {label} pointer switches to HSV exclusively");
            rgb.Focus();
            host.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            host.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Flush();
            check(
                view.ColorModel == ColorModel.Rgba && rgb.IsChecked == true && hsv.IsChecked != true &&
                view.Color == initial,
                $"ColorPicker/model: {label} keyboard restores RGB without changing color");
            using var frame = host.CaptureRenderedFrame()!;
            var bytes = new byte[frame.PixelSize.Width * frame.PixelSize.Height * 4];
            var memory = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                frame.CopyPixels(new PixelRect(frame.PixelSize), memory, bytes.Length, frame.PixelSize.Width * 4);
                Marshal.Copy(memory, bytes, 0, bytes.Length);
                var mid = rgb.TranslatePoint(new Point(rgb.Bounds.Width + gap / 2, rgb.Bounds.Height / 2), host)!.Value;
                var surfacePoint = model.TranslatePoint(new Point(model.Bounds.Width / 2, -2), host)!.Value;

                int Pixel(Point p)
                {
                    return BitConverter.ToInt32(bytes, ((int)p.Y * frame.PixelSize.Width + (int)p.X) * 4);
                }

                check(Pixel(mid) == Pixel(surfacePoint),
                    $"ColorPicker/model: {label} gap renders the surrounding surface instead of button chrome");
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }
        }
    }

    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 480, 320 })
            Visuals(theme, size, width, check, output);
        Interaction(check, output);
    }

    private static void Visuals(ThemeVariant theme, ControlSize size, int width, Action<bool, string> check,
        string? output)
    {
        var picker = new ColorPicker { Color = Color.Parse("#80006061"), IsAlphaEnabled = true };
        var view = new ColorView { Color = Color.Parse("#80006061"), IsAlphaEnabled = true, SelectedIndex = 0 };
        var root = new StackPanel { Spacing = 16, Children = { picker, view } };
        ControlSizing.SetSize(root, size);
        var window = Open(new Border { Padding = new Thickness(16), Child = root }, width, 760, theme);
        try
        {
            var label = $"{theme}/{size}/{width}";
            var height = size switch { ControlSize.Small => 30d, ControlSize.Large => 44d, _ => 36d };
            var button = picker.GetVisualDescendants().OfType<DropDownButton>().Single();
            var chrome = Part<Border>(button, "RootBorder");
            check(chrome.CornerRadius == picker.CornerRadius && chrome.BorderThickness == new Thickness(1),
                $"ColorPicker/border: {label} retains a complete rounded outline at rest");
            check(picker.Bounds.Height == height && button.Bounds.Height == height,
                $"ColorPicker/size: {label} matches input density (picker={picker.Bounds.Height}, button={button.Bounds.Height}, min={button.MinHeight}, classes={string.Join(',', button.Classes)})");
            check(picker.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == picker.Color.ToString()),
                $"ColorPicker/swatch: {label} shows the actual selected color");
            var tabs = Part<TabControl>(view, "PART_TabControl");
            WaitFor(() =>
                tabs.Items.Count == 2 &&
                view.GetVisualDescendants().OfType<TextBox>().Any(t => t.Name == "PART_HexTextBox"));
            check(tabs.Items.Count == 2, $"ColorPicker/layout: {label} has only Custom and Palette pages");
            check(tabs.Bounds.Width <= view.Bounds.Width + 1 && view.Bounds.Width <= width - 32 + 1,
                $"ColorPicker/layout: {label} fits the inline view without horizontal clipping");
            var background = Part<Border>(view, "ContentBackgroundBorder");
            view.TryFindResource("One.Surface", theme, out var surface);
            check(BrushColor(background.Background) == BrushColor(surface as IBrush),
                $"ColorPicker/theme: {label} uses One.Surface");
            var spectrum = Part<ColorSpectrum>(view, "ColorSpectrum");
            WaitFor(() =>
                spectrum.GetVisualDescendants().OfType<Rectangle>()
                    .Any(r => r.Name == "PART_SpectrumRectangle" && r.Fill is ImageBrush));
            check(spectrum.Bounds.Width > 60 && spectrum.Bounds.Height > 60,
                $"ColorPicker/spectrum: {label} retains the official native spectrum");
            var hue = Part<ColorSlider>(view, "ColorSpectrumThirdComponentSlider");
            var alpha = Part<ColorSlider>(view, "ColorSpectrumAlphaSlider");
            WaitFor(() =>
                hue.TranslatePoint(default, view)!.Value.Y >=
                spectrum.TranslatePoint(default, view)!.Value.Y + spectrum.Bounds.Height);
            var spectrumTop = spectrum.TranslatePoint(default, view)!.Value;
            var hueTop = hue.TranslatePoint(default, view)!.Value;
            var alphaTop = alpha.TranslatePoint(default, view)!.Value;
            var spectrumGrid = (Grid)spectrum.GetVisualParent()!;
            check(
                hue.Orientation == Orientation.Horizontal && hueTop.Y >= spectrumTop.Y + spectrum.Bounds.Height &&
                alphaTop.Y > hueTop.Y,
                $"ColorPicker/layout: {label} hue and alpha sit below the spectrum without overlap (rows={spectrumGrid.RowDefinitions.Count}, cols={spectrumGrid.ColumnDefinitions.Count}, hueRow={Grid.GetRow(hue)}, alphaRow={Grid.GetRow(alpha)}, rowHeights={string.Join(',', spectrumGrid.RowDefinitions.Select(r => r.ActualHeight))}, spectrum={spectrumTop}, hue={hueTop}, alpha={alphaTop})");
            var hex = Part<TextBox>(view, "PART_HexTextBox");
            var numbers = view.GetVisualDescendants().OfType<NumericUpDown>().ToArray();
            var inputTop = numbers.Select(n => n.TranslatePoint(default, view)!.Value.Y).ToArray();
            check(
                numbers.All(n =>
                    n.GetVisualDescendants().OfType<TextBox>().All(t => t.Bounds.Width <= n.Bounds.Width + 1)),
                $"ColorPicker/fields: {label} native input chrome fits each compact channel (sizes={string.Join(';', numbers.Select(n => $"{n.Name}:{n.Bounds.Size}, min={n.MinWidth}, text={n.GetVisualDescendants().OfType<TextBox>().First().Bounds.Size}"))})");
            check(numbers.Length == 4 && inputTop.Max() - inputTop.Min() < 1 &&
                  inputTop.Min() >= spectrumTop.Y + spectrum.Bounds.Height &&
                  hex.TranslatePoint(default, view)!.Value.Y > inputTop.Max() &&
                  hueTop.Y >= hex.TranslatePoint(default, view)!.Value.Y + hex.Bounds.Height,
                $"ColorPicker/layout: {label} model, four channels and Hex share the spectrum page without overlap");
            check(Math.Abs(hex.Bounds.Height - height) < 1,
                $"ColorPicker/fields: {label} hex input uses the selected density");
            view.TryFindResource("One.Subtle", theme, out var subtle);
            var prefix = Part<Grid>(view, "HexInputGrid").Children.OfType<Border>().Single();
            check(BrushColor(prefix.Background) == BrushColor(subtle as IBrush),
                $"ColorPicker/fields: {label} component labels use themed chrome");
            if (output != null)
            {
                using var components = window.CaptureRenderedFrame()!;
                Save(components, output, $"colorpicker-components-{theme}-{size}-{width}.png");
            }

            view.SelectedIndex = 0;
            Flush();
            using var frame = window.CaptureRenderedFrame()!;
            var bytes = new byte[frame.PixelSize.Width * frame.PixelSize.Height * 4];
            var memory = Marshal.AllocHGlobal(bytes.Length);
            try
            {
                frame.CopyPixels(new PixelRect(frame.PixelSize), memory, bytes.Length, frame.PixelSize.Width * 4);
                Marshal.Copy(memory, bytes, 0, bytes.Length);
                var colors = new HashSet<int>();
                for (var i = 0; i < bytes.Length; i += 4) colors.Add(BitConverter.ToInt32(bytes, i));
                check(colors.Count > 500, $"ColorPicker/pixels: {label} renders actual color gradients and chrome");
            }
            finally
            {
                Marshal.FreeHGlobal(memory);
            }

            if (output != null) Save(frame, output, $"colorpicker-{theme}-{size}-{width}.png");
            if (width == 480)
            {
                var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)!
                    .Value;
                window.MouseMove(point);
                window.MouseDown(point, MouseButton.Left);
                Flush();
                check(
                    chrome.CornerRadius == picker.CornerRadius && chrome.BorderThickness == new Thickness(1) &&
                    BrushColor(chrome.BorderBrush)?.A > 0,
                    $"ColorPicker/border: {label} preserves rounded border while the pointer is held down");
                if (output != null)
                {
                    using var held = window.CaptureRenderedFrame()!;
                    Save(held, output, $"colorpicker-pressed-{theme}-{size}.png");
                }

                window.MouseUp(point, MouseButton.Left);
                Flush();
                check(button.Flyout?.IsOpen == true, $"ColorPicker/popup: {label} opens the official flyout");
                check(chrome.CornerRadius == picker.CornerRadius && BrushColor(chrome.BorderBrush)?.A > 0,
                    $"ColorPicker/border: {label} preserves rounded border while the popup is open");
                if (button.Flyout is Flyout flyout && flyout.Content is Control content)
                {
                    Flush();
                    var contentBackground = Part<Border>(content, "ContentBackgroundBorder");
                    check(BrushColor(contentBackground.Background) == BrushColor(surface as IBrush),
                        $"ColorPicker/popup: {label} follows the owner theme in popup scope");
                    var popupTabs = Part<TabControl>(content, "PART_TabControl");
                    popupTabs.SelectedIndex = 0;
                    Flush();
                    WaitFor(() => popupTabs.Items.Count == 2 && content.GetVisualDescendants().OfType<TextBox>()
                        .Any(t => t.Name == "PART_HexTextBox"));
                    check(
                        popupTabs.Items.Count == 2 &&
                        content.GetVisualDescendants().OfType<NumericUpDown>().Count() == 4,
                        $"ColorPicker/popup: {label} combines spectrum and native fields in one page");
                    check(Math.Abs(Part<TextBox>(content, "PART_HexTextBox").Bounds.Height - height) < 1,
                        $"ColorPicker/popup: {label} follows owner input density");
                    popupTabs.SelectedIndex = 0;
                    Flush();
                    if (output != null && TopLevel.GetTopLevel(content) is { } popup)
                    {
                        using var popupFrame = popup.CaptureRenderedFrame();
                        if (popupFrame != null) Save(popupFrame, output, $"colorpicker-popup-{theme}-{size}.png");
                    }
                }

                button.Flyout?.Hide();
                Flush();
            }
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Interaction(Action<bool, string> check, string? output)
    {
        var demo = new ColorPickerPage { Margin = new Thickness(24) };
        var window = Open(new ScrollViewer { Content = demo }, 920, 1040, ThemeVariant.Light);
        try
        {
            var picker = demo.FindControl<ColorPicker>("ControlSample")!;
            var view = demo.FindControl<ColorView>("ColorViewSample")!;
            check(picker.SelectedIndex == 1 && view.SelectedIndex == 1,
                "ColorPicker/layout: compact palette is the default editing page");
            check(Part<TabControl>(view, "PART_TabControl").Items.Count == 2,
                "ColorPicker/layout: Values is removed before the custom page is first opened");
            if (output != null)
            {
                using var paletteFrame = window.CaptureRenderedFrame()!;
                Save(paletteFrame, output, "colorpicker-palette-demo-Light.png");
            }

            view.SelectedIndex = 0;
            Flush();
            check(picker.Color == demo.Model.SelectedColor && view.Color == picker.Color,
                "ColorPicker/binding: official Color properties initialize through compiled two-way binding");
            demo.Model.SelectedColor = Color.Parse("#804276A5");
            Flush();
            check(picker.Color == demo.Model.SelectedColor && view.Color == picker.Color,
                "ColorPicker/binding: consumer changes synchronize dropdown and inline view");
            var changes = 0;
            view.ColorChanged += (_, _) => changes++;
            var spectrum = Part<ColorSpectrum>(view, "ColorSpectrum");
            WaitFor(() => spectrum.GetVisualDescendants().OfType<Rectangle>().Any(r => r.Fill is ImageBrush));
            var old = view.HsvColor;
            Click(window, spectrum, .65, .35);
            Flush();
            check(
                view.HsvColor != old && changes > 0 && demo.Model.SelectedColor == view.Color &&
                picker.Color == view.Color,
                "ColorPicker/input: native spectrum pointer changes color and updates consumer bindings");
            spectrum.Focus();
            old = spectrum.HsvColor;
            Press(window, Key.Right, PhysicalKey.ArrowRight);
            check(spectrum.HsvColor != old,
                $"ColorPicker/input: spectrum retains keyboard navigation (focusable={spectrum.Focusable}, focused={spectrum.IsFocused}, old={old}, new={spectrum.HsvColor})");
            var hex = Part<TextBox>(view, "PART_HexTextBox");
            hex.Focus();
            hex.SelectAll();
            window.KeyTextInput("336699");
            Press(window, Key.Enter, PhysicalKey.Enter);
            check(
                view.Color.R == 0x33 && view.Color.G == 0x66 && view.Color.B == 0x99 &&
                demo.Model.SelectedColor == view.Color,
                "ColorPicker/hex: native hex input updates selected RGB and compiled binding");
            var invalidBefore = view.Color;
            hex.SelectAll();
            window.KeyTextInput("invalid");
            Press(window, Key.Enter, PhysicalKey.Enter);
            check(view.Color == invalidBefore, "ColorPicker/hex: invalid text does not corrupt the selected color");
            view.ColorModel = ColorModel.Rgba;
            Flush();
            var component = Part<NumericUpDown>(view, "Component1NumericUpDown");
            component.Focus();
            var input = component.GetVisualDescendants().OfType<TextBox>().First();
            input.Focus();
            input.SelectAll();
            window.KeyTextInput("72");
            Press(window, Key.Enter, PhysicalKey.Enter);
            check(view.Color.R == 72 && demo.Model.SelectedColor == view.Color,
                "ColorPicker/components: native numeric input edits red channel");
            var hsv = Part<RadioButton>(view, "HsvRadioButton");
            hsv.Focus();
            Press(window, Key.Space, PhysicalKey.Space);
            check(view.ColorModel == ColorModel.Hsva, "ColorPicker/components: native model button switches to HSV");
            var beforeHue = view.Color;
            input.Focus();
            input.SelectAll();
            window.KeyTextInput("216");
            Press(window, Key.Enter, PhysicalKey.Enter);
            check(
                component.Value == 216 && component.Maximum >= 359 && component.Maximum <= 360 &&
                view.Color != beforeHue && view.Color == demo.Model.SelectedColor,
                $"ColorPicker/components: inline HSV numeric input retains native ranges and two-way binding (value={component.Value}, max={component.Maximum}, before={beforeHue}, after={view.Color}, bound={demo.Model.SelectedColor})");
            view.IsColorSpectrumVisible = false;
            Flush();
            check(
                !spectrum.IsVisible && ((TabItem)Part<TabControl>(view, "PART_TabControl").Items[0]!).IsVisible &&
                hex.Bounds.Height > 0,
                "ColorPicker/visibility: native component-only configuration remains editable on Custom");
            view.IsColorSpectrumVisible = true;
            view.IsColorComponentsVisible = false;
            Flush();
            check(
                !hex.GetVisualAncestors().OfType<Grid>().First(g => g.Classes.Contains("one-color-components"))
                    .IsVisible && spectrum.IsVisible,
                "ColorPicker/visibility: native component flag hides the inline input group");
            view.IsColorComponentsVisible = true;
            Flush();
            view.SelectedIndex = 1;
            Flush();
            var palette = view.GetVisualDescendants().OfType<ListBox>().Single();
            var tiles = palette.GetVisualDescendants().OfType<ListBoxItem>().ToArray();
            check(
                tiles.Length == 50 && tiles.All(tile =>
                    tile.Bounds.Width >= 20 && tile.Bounds.Height == 24 && tile.Padding == default),
                $"ColorPicker/palette: ten hue columns and five shade rows retain compact usable swatches (count={tiles.Length}; sample={string.Join(';', tiles.Take(2).Select(t => $"{t.Bounds.Size} padding={t.Padding}"))})");
            palette.SelectedIndex = 4;
            palette.Focus();
            Press(window, Key.Right, PhysicalKey.ArrowRight);
            check(palette.SelectedItem is Color chosen && chosen == view.Color && picker.Color == view.Color,
                "ColorPicker/palette: native palette selection updates shared color");
            demo.Model.AlphaEnabled = false;
            Flush();
            check(!picker.IsAlphaEnabled && !view.IsAlphaEnabled && view.Color.A == 255,
                "ColorPicker/alpha: disabling alpha follows the official opaque-color behavior");
            demo.Model.AlphaEnabled = true;
            view.SelectedIndex = 0;
            Flush();
            var button = picker.GetVisualDescendants().OfType<DropDownButton>().Single();
            button.Focus();
            Press(window, Key.Space, PhysicalKey.Space);
            check(button.Flyout?.IsOpen == true, "ColorPicker/flyout: native keyboard activation opens the dropdown");
            if (button.Flyout is Flyout flyout && flyout.Content is Control content)
            {
                Part<TabControl>(content, "PART_TabControl").SelectedIndex = 0;
                Flush();
                check(content.GetVisualDescendants().OfType<ColorSpectrum>().Any(),
                    "ColorPicker/flyout: original popup template retains spectrum and inputs");
            }

            Press(window, Key.Escape, PhysicalKey.Escape);
            Flush();
            check(button.Flyout?.IsOpen != true, "ColorPicker/flyout: native Escape closes the popup");
            var disabled = demo.FindControl<ColorPicker>("DisabledSample")!;
            check(!disabled.IsEffectivelyEnabled && disabled.Opacity < 1,
                "ColorPicker/disabled: native disabled state has themed feedback");
            demo.Model.SelectedColor = Color.Parse("#006061");
            Flush();
            if (output != null)
            {
                using var frame = window.CaptureRenderedFrame()!;
                Save(frame, output, "colorpicker-demo-Light.png");
            }

            window.RequestedThemeVariant = ThemeVariant.Dark;
            Flush();
            view.TryFindResource("One.Surface", ThemeVariant.Dark, out var surface);
            check(BrushColor(Part<Border>(view, "TabBackgroundBorder").Background) == BrushColor(surface as IBrush),
                "ColorPicker/theme: existing inline view follows a runtime theme switch");
            if (output != null)
            {
                using var frame = window.CaptureRenderedFrame()!;
                Save(frame, output, "colorpicker-demo-Dark.png");
            }

            view.SelectedIndex = 1;
            Flush();
            if (output != null)
            {
                using var frame = window.CaptureRenderedFrame()!;
                Save(frame, output, "colorpicker-palette-demo-Dark.png");
            }
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>().First(c => c.Name == name);
    }

    private static Color? BrushColor(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Window Open(Control content, int width, int height, ThemeVariant theme)
    {
        var window = new Window { Content = content, Width = width, Height = height, RequestedThemeVariant = theme };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void Click(Window window, Control control, double x, double y)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width * x, control.Bounds.Height * y), window)!
            .Value;
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Press(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        Flush();
    }

    private static void WaitFor(Func<bool> ready)
    {
        var watch = Stopwatch.StartNew();
        while (!ready() && watch.ElapsedMilliseconds < 3000)
        {
            Flush();
            Thread.Sleep(10);
        }

        Flush();
    }

    private static void Save(Bitmap frame, string output, string name)
    {
        Directory.CreateDirectory(output);
        frame.Save(Path.Combine(output, name), PngBitmapEncoderOptions.Default);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}