using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NumericUpDownPreview
{
    public static void Verify(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var variant in new[] { "default", "filled", "underlined" })
            Verify(theme, size, width, variant, check, output);
    }

    private static void Verify(ThemeVariant theme, ControlSize size, int width, string variant,
        Action<bool, string> check, string? output)
    {
        var numeric = new NumericUpDown
            { Value = 28, Minimum = 0, Maximum = 100, Increment = .5m, FormatString = "0.0" };
        if (variant != "default") numeric.Classes.Add(variant);
        var after = new Button { Content = "完成", HorizontalAlignment = HorizontalAlignment.Left };
        var host = new Border
            { Padding = new Thickness(20), Child = new StackPanel { Spacing = 12, Children = { numeric, after } } };
        host.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        var window = new Window
        {
            Width = width, Height = 210, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
            Content = host
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        var label = $"NumericUpDown/{theme.Key}/{size}/{width}/{variant}";
        try
        {
            var spinner = Part<ButtonSpinner>(numeric, "PART_Spinner");
            var input = Part<TextBox>(numeric, "PART_TextBox");
            var border = Part<Border>(spinner, "PART_Border");
            var innerBorder = Part<Border>(input, "PART_BorderElement");
            var increase = Part<RepeatButton>(spinner, "PART_IncreaseButton");
            var decrease = Part<RepeatButton>(spinner, "PART_DecreaseButton");
            var buttons = new[] { increase, decrease };
            var underlined = variant == "underlined";
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            check(
                numeric.Bounds.Height == height && numeric.Bounds.Width == width - 40 &&
                buttons.All(b => b.Bounds.Height <= height),
                label + ": field and native spin targets fit the requested density and viewport");
            check(
                innerBorder.BorderThickness == default && (underlined
                    ? border.BorderThickness == new Thickness(0, 0, 0, 1)
                    : border.BorderThickness == new Thickness(1)),
                label + ": the embedded editor has no duplicate outline");
            foreach (var left in new[] { false, true })
            {
                numeric.ButtonSpinnerLocation = left ? Location.Left : Location.Right;
                Flush();
                input.Focus();
                Flush();
                check(
                    buttons.All(b =>
                        underlined
                            ? b.BorderThickness == default
                            : Colour(b.BorderBrush) == Resource(numeric, "One.InputLine")),
                    label + $"/{(left ? "left" : "right")}: spinner dividers stay neutral or absent while focused");
                using var bitmap = Render(numeric);
                var origin = increase.TranslatePoint(default, numeric)!.Value;
                var lineX = (int)(left ? origin.X + increase.Bounds.Width - 1 : origin.X);
                var lineY = (int)(origin.Y + increase.Bounds.Height / 2);
                check(Pixel(bitmap, lineX, lineY) == Resource(numeric, underlined ? "One.Surface" : "One.InputLine"),
                    label +
                    $"/{(left ? "left" : "right")}: actual button-edge pixels have no accent-colored vertical bars");
                check(
                    Pixel(bitmap, bitmap.PixelSize.Width / 2, bitmap.PixelSize.Height - 1) ==
                    Resource(numeric, "One.Focus"),
                    label + ": actual focus pixels remain on the outer border or baseline");
                if (!left)
                {
                    if (underlined)
                        check(Pixel(bitmap, bitmap.PixelSize.Width / 2, 0) == Resource(numeric, "One.Surface"),
                            label + ": underlined chrome has no top border");
                    if (output is not null)
                    {
                        Directory.CreateDirectory(output);
                        bitmap.Save(Path.Combine(output, $"numeric-{theme.Key}-{size}-{width}-{variant}.png"),
                            PngBitmapEncoderOptions.Default);
                    }
                }
            }

            numeric.ButtonSpinnerLocation = Location.Right;
            Flush();
            input.SelectAll();
            window.KeyTextInput("12.5");
            Press(window, Key.Enter);
            check(numeric.Value == 12.5m, label + ": native decimal text input commits without replacing the editor");
            Press(window, Key.Up);
            check(numeric.Value == 13m, label + ": native Up key increments the value");
            Click(decrease);
            check(numeric.Value == 12.5m, label + ": native decrement button changes the value");
            numeric.Value = 100;
            Flush();
            Click(increase);
            check(numeric.Value == 100 && !increase.IsEffectivelyEnabled && decrease.IsEffectivelyEnabled,
                label + ": native limits disable the unavailable direction");
            numeric.Value = 28;
            numeric.IsReadOnly = true;
            Flush();
            Click(increase);
            check(numeric.Value == 28 && input.IsReadOnly, label + ": readonly input does not spin");
            numeric.IsReadOnly = false;
            var previousWidth = input.Bounds.Width;
            numeric.ShowButtonSpinner = false;
            Flush();
            check(
                !Part<StackPanel>(spinner, "PART_SpinnerPanel").IsEffectivelyVisible &&
                input.Bounds.Width > previousWidth, label + ": hiding buttons returns their width to the editor");
            numeric.ShowButtonSpinner = true;
            Flush();
            Click(after);
            DataValidationErrors.SetErrors(numeric, new object[] { new FormatException("Invalid value") });
            Flush();
            check(
                Colour(spinner.BorderBrush) == Resource(numeric, "One.Danger") && buttons.All(b =>
                    underlined
                        ? b.BorderThickness == default
                        : Colour(b.BorderBrush) == Resource(numeric, "One.InputLine")),
                label + ": validation colors the outer chrome without coloring dividers");
            DataValidationErrors.ClearErrors(numeric);
            numeric.IsEnabled = false;
            Flush();
            check(
                !input.IsEffectivelyEnabled && buttons.All(b => !b.IsEffectivelyEnabled) &&
                Colour(border.Background) == Resource(numeric, "One.DisabledSurface"),
                label + ": disabled chrome and native input remain disabled together");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static T Part<T>(TemplatedControl owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>()
            .First(c => c.Name == name && ReferenceEquals(c.TemplatedParent, owner));
    }

    private static Color Colour(IBrush? brush)
    {
        return ((ISolidColorBrush)brush!).Color;
    }

    private static Color Resource(Control control, string key)
    {
        control.TryFindResource(key, control.ActualThemeVariant, out var value);
        return Colour((IBrush)value!);
    }

    private static void Press(TopLevel root, Key key)
    {
        root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null);
        root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null);
        Flush();
    }

    private static void Click(Control control)
    {
        var root = TopLevel.GetTopLevel(control)!;
        var hit = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value;
        root.MouseMove(hit);
        root.MouseDown(hit, MouseButton.Left);
        root.MouseUp(hit, MouseButton.Left);
        Flush();
    }

    private static RenderTargetBitmap Render(Control control)
    {
        var bitmap =
            new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)),
                new Vector(96, 96));
        bitmap.Render(control);
        return bitmap;
    }

    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var pixels = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), handle.AddrOfPinnedObject(), pixels.Length,
                bitmap.PixelSize.Width * 4);
        }
        finally
        {
            handle.Free();
        }

        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Bgra8888
            ? Color.FromArgb(pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset])
            : Color.FromArgb(pixels[offset + 3], pixels[offset], pixels[offset + 1], pixels[offset + 2]);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}