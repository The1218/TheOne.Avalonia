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

internal static class UnderlinedInputPreview
{
    public static void Verify(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var kind in new[] { "TextBox", "ComboBox", "EditableComboBox", "AutoCompleteBox" })
            Verify(theme, size, width, kind, check, output);
    }

    private static void Verify(ThemeVariant theme, ControlSize size, int width, string kind, Action<bool, string> check,
        string? output)
    {
        TemplatedControl input = kind switch
        {
            "TextBox" => new TextBox { Text = "示例文本" },
            "AutoCompleteBox" => new AutoCompleteBox
                { Text = "Alpha", ItemsSource = new[] { "Alpha", "Beta", "Bravo" } },
            _ => new ComboBox
            {
                IsEditable = kind == "EditableComboBox", ItemsSource = new[] { "Alpha", "Beta", "Bravo" },
                SelectedIndex = 0, HorizontalAlignment = HorizontalAlignment.Stretch
            }
        };
        input.Classes.Add("underlined");
        var after = new Button { Content = "完成", HorizontalAlignment = HorizontalAlignment.Left };
        var host = new Border
            { Padding = new Thickness(20), Child = new StackPanel { Spacing = 12, Children = { input, after } } };
        host.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        var window = new Window
        {
            Width = width, Height = 240, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
            Content = host
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var label = $"Underlined/{kind}/{theme.Key}/{size}/{width}";
        try
        {
            var editor = input as TextBox ?? (kind == "AutoCompleteBox" ? Part<TextBox>(input, "PART_TextBox") :
                kind == "EditableComboBox" ? Part<TextBox>(input, "PART_EditableTextBox") : null);
            var border = input is ComboBox
                ? Part<Border>(input, "Background")
                : Part<Border>(editor!, "PART_BorderElement");
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            after.Focus();
            window.MouseMove(default);
            Flush();
            check(
                input.Bounds.Width == width - 40 && input.Bounds.Height >= height && input.Bounds.Height <= height + 2,
                label + ": native density fits the field without a nested editor adding height");
            CheckChrome(input, border, 1, "One.InputLine", check, label + "/rest");
            var point = input.TranslatePoint(new Point(input.Bounds.Width / 2, input.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            Flush();
            CheckChrome(input, border, 1, "One.InputLine", check, label + "/hover");
            (editor as Control ?? input).Focus(NavigationMethod.Tab);
            Flush();
            CheckChrome(input, border, 2, "One.Focus", check, label + "/focus");
            if (kind == "EditableComboBox")
            {
                var inner = Part<Border>(editor!, "PART_BorderElement");
                check(inner.BorderThickness == default && inner.CornerRadius == default,
                    label + ": editable ComboBox focus has no second rounded outline");
            }

            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var bitmap = Render(input);
                bitmap.Save(Path.Combine(output, $"underlined-{kind}-{theme.Key}-{size}-{width}.png"),
                    PngBitmapEncoderOptions.Default);
            }

            if (editor is not null)
            {
                editor.SelectAll();
                window.KeyTextInput("Beta");
                Flush();
                check(editor.Text == "Beta", label + ": native text input is preserved");
            }

            if (input is ComboBox combo)
            {
                input.Focus(NavigationMethod.Tab);
                Press(window, Key.F4);
                check(combo.IsDropDownOpen, label + ": native keyboard opens the dropdown");
                var popup = Part<Popup>(combo, "PART_Popup");
                Press(TopLevel.GetTopLevel(popup.Child!)!, Key.Escape);
                check(!combo.IsDropDownOpen, label + ": native Escape closes the dropdown");
                (editor as Control ?? input).Focus(NavigationMethod.Tab);
                Flush();
            }

            DataValidationErrors.SetErrors(input, new object[] { new FormatException("Invalid value") });
            Flush();
            CheckChrome(input, border, 2, "One.Danger", check, label + "/error");
            DataValidationErrors.ClearErrors(input);
            after.Focus();
            input.IsEnabled = false;
            Flush();
            CheckChrome(input, border, 1, "One.InputLine", check, label + "/disabled");
            check(!input.IsEffectivelyEnabled && (editor is null || !editor.IsEffectivelyEnabled),
                label + ": disabled state still blocks native input");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void CheckChrome(Control input, Border border, double thickness, string brush,
        Action<bool, string> check, string label)
    {
        check(border.BorderThickness == new Thickness(0, 0, 0, thickness) && border.CornerRadius == default,
            label + ": only the expected square-corner baseline is drawn");
        using var bitmap = Render(input);
        var expected = Resource(input, brush);
        var origin = border.TranslatePoint(default, input)!.Value;
        var middle = (int)(origin.X + border.Bounds.Width / 2);
        var bottom = (int)(origin.Y + border.Bounds.Height - 1);
        check(Pixel(bitmap, middle, bottom) == expected && Pixel(bitmap, middle, (int)origin.Y) != expected
                                                        && Pixel(bitmap, (int)origin.X,
                                                            (int)(origin.Y + border.Bounds.Height / 2)) != expected,
            label +
            $": actual baseline pixels have the correct color without top or side borders (bottom={Pixel(bitmap, middle, bottom)}, expected={expected})");
    }

    private static T Part<T>(TemplatedControl owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>()
            .First(c => c.Name == name && ReferenceEquals(c.TemplatedParent, owner));
    }

    private static Color Resource(Control control, string key)
    {
        control.TryFindResource(key, control.ActualThemeVariant, out var value);
        return ((ISolidColorBrush)value!).Color;
    }

    private static void Press(TopLevel root, Key key)
    {
        root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null);
        root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null);
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