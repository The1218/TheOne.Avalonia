using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ReportNavigationPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in new[] { ControlSize.Small, ControlSize.Normal, ControlSize.Large })
        {
            var element = new ReportElement
                { Kind = ReportElementKind.Text, Text = "设计与预览切换", X = 18, Y = 22, Width = 100, Height = 12 };
            var document = new ReportDocument();
            document.Elements.Add(element);
            var view = new ReportDesign { Document = document, SelectedElement = element };
            var window = new Window
            {
                Content = view, Width = 1586, Height = 992, RequestedThemeVariant = theme,
                WindowDecorations = WindowDecorations.None
            };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show();
            Flush();
            try
            {
                var tabs = view.FindControl<TabControl>("ViewTabs")!;
                var design = view.FindControl<TabItem>("DesignTab")!;
                var preview = view.FindControl<TabItem>("PreviewTab")!;
                for (var repeat = 0; repeat < 4; repeat++)
                {
                    Click(window, preview);
                    check(view.IsPreview && tabs.SelectedIndex == 1 && preview.IsSelected && !design.IsSelected,
                        $"Report navigation/{theme}/{size}: pointer enters preview, pass {repeat + 1}");
                    Click(window, design);
                    check(
                        !view.IsPreview && tabs.SelectedIndex == 0 && design.IsSelected && !preview.IsSelected &&
                        ReferenceEquals(view.SelectedElement, element),
                        $"Report navigation/{theme}/{size}: pointer returns to design without structure navigation, pass {repeat + 1}");
                }

                Click(window, view.FindControl<Button>("PreviewButton")!);
                check(view.IsPreview && tabs.SelectedIndex == 1,
                    $"Report navigation/{theme}/{size}: toolbar preview synchronizes the native tab selection");
                Click(window, design);
                check(!view.IsPreview && tabs.SelectedIndex == 0,
                    $"Report navigation/{theme}/{size}: design tab exits toolbar preview");
                view.ShowPreview();
                Flush();
                view.ShowDesign();
                Flush();
                check(!view.IsPreview && tabs.SelectedIndex == 0,
                    $"Report navigation/{theme}/{size}: programmatic mode changes synchronize selection");
                design.Focus();
                PressKey(window, Key.Right, PhysicalKey.ArrowRight);
                check(view.IsPreview && preview.IsSelected,
                    $"Report navigation/{theme}/{size}: native right-arrow selects preview");
                preview.Focus();
                PressKey(window, Key.Left, PhysicalKey.ArrowLeft);
                check(!view.IsPreview && design.IsSelected,
                    $"Report navigation/{theme}/{size}: native left-arrow selects design");

                var picker = view.FindControl<ComboBox>("FontFamilyPicker")!;
                // Controlled alternating short/long groups expose virtualized auto-width changes reliably on every host.
                var names = Enumerable.Range(0, 180).Select(index => index % 30 < 15
                    ? $"A{index:000}"
                    : $"Very long installed font family name with a descriptive suffix {index:000}").ToArray();
                picker.ItemsSource = names;
                picker.SelectedItem = names[0];
                Flush();
                Click(window, picker);
                Flush();
                var popup = picker.GetVisualDescendants().OfType<Popup>().Single(part => part.Name == "PART_Popup");
                var child = popup.Child!;
                var host = TopLevel.GetTopLevel(child)!;
                var scroll = child.GetVisualDescendants().OfType<ScrollViewer>().First();
                var width = child.Bounds.Width;
                var minimum = width;
                var maximum = width;
                var oldOffset = scroll.Offset.Y;
                var point = scroll.TranslatePoint(new Point(scroll.Bounds.Width / 2, scroll.Bounds.Height / 2), host)!
                    .Value;
                for (var index = 0; index < 18; index++)
                {
                    host.MouseWheel(point, new Vector(0, -5));
                    Flush();
                    minimum = Math.Min(minimum, child.Bounds.Width);
                    maximum = Math.Max(maximum, child.Bounds.Width);
                }

                check(
                    popup.IsOpen && scroll.Offset.Y > oldOffset && maximum - minimum < .5 && Math.Abs(width - 320) < 2,
                    $"Report fonts/popup/{theme}/{size}: real wheel scrolling through unequal font names keeps the dropdown width stable");
                Save(host, output, $"report-font-popup-{theme}-{size}");
                var scrollOrigin = scroll.TranslatePoint(default, host)!.Value;
                var item = child.GetVisualDescendants().OfType<ComboBoxItem>().First(container =>
                    container.TranslatePoint(default, host) is { } position && position.Y >= scrollOrigin.Y + 1
                                                                            && position.Y + container.Bounds.Height <
                                                                            scrollOrigin.Y + scroll.Viewport.Height -
                                                                            1);
                var name = (string)item.Content!;
                Click(host, item);
                check(!picker.IsDropDownOpen && picker.SelectedItem as string == name && element.FontFamily == name,
                    $"Report fonts/popup/{theme}/{size}: native item selection still updates the element after scrolling (open={picker.IsDropDownOpen}, expected={name}, selected={picker.SelectedItem}, element={element.FontFamily}, item={item.Bounds}, host={host.ClientSize})");
                Click(window, picker);
                Flush();
                check(Math.Abs(popup.Child!.Bounds.Width - width) < .5,
                    $"Report fonts/popup/{theme}/{size}: reopening preserves the same dropdown width");
                picker.IsDropDownOpen = false;
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void PressKey(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        Flush();
    }

    private static void Click(TopLevel host, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point);
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Save(TopLevel host, string? output, string name)
    {
        if (output is null) return;
        Directory.CreateDirectory(output);
        Flush();
        using var frame = host.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }
}