using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveUI;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class LoadingPreview
{
    public static void Verify(Action<bool, string> check)
    {
        check(PreviewPageCatalog.Create(306) is LoadingContainerPage,
            "LoadingContainer has an independent catalogue page");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var icons = new IconButtonPage();
            var buttons = new ButtonPage();
            var page = new LoadingContainerPage();
            var window = Open(new StackPanel { Spacing = 16, Children = { icons, buttons, page } }, theme, size);
            var label = $"Loading/{theme.Key}/{size}";
            var iconModel = (IconButtonPageViewModel)icons.Model;
            foreach (var position in Enum.GetValues<IconPlacement>())
            {
                var button = icons.FindControl<IconButton>("Placement" + position)!;
                var glyph = Part<ContentPresenter>(button, "PART_IconPresenter");
                var text = Part<ContentPresenter>(button, "PART_LabelPresenter");
                var point = glyph.TranslatePoint(new Point(), button)!.Value;
                var textPoint = text.TranslatePoint(new Point(), button)!.Value;
                check(
                    position switch
                    {
                        IconPlacement.Left => point.X < textPoint.X, IconPlacement.Right => point.X > textPoint.X,
                        IconPlacement.Top => point.Y < textPoint.Y, _ => point.Y > textPoint.Y
                    }, label + ": " + position + " icon placement is physically arranged");
            }

            var target = icons.FindControl<IconButton>("PlacementLeft")!;
            iconModel.IsLoading = true;
            ControlPreview.Flush();
            check(
                target.IsLoading && !target.IsEffectivelyEnabled &&
                !Part<ContentPresenter>(target, "PART_IconPresenter").IsVisible &&
                Part<LoadingIndicator>(target, "PART_LoadingIndicator").IsVisible,
                label + ": IconButton loading replaces icon and disables input");
            var spinner = Part<LoadingIndicator>(target, "PART_LoadingIndicator");
            check(spinner.IsSpinning,
                label +
                $": spinner starts while motion is enabled (active={spinner.IsActive}, visible={spinner.IsVisible}/{spinner.IsEffectivelyVisible}, motion={Motion.GetEnabled(spinner)}, attached={spinner.IsAttachedToVisualTree()})");
            Pump(140);
            check(((RotateTransform)spinner.RenderTransform!).Angle > 0,
                label + ": enabled animation advances the actual rotation");
            iconModel.MotionEnabled = false;
            ControlPreview.Flush();
            check(!spinner.IsSpinning && ((RotateTransform)spinner.RenderTransform!).Angle == 0 && spinner.IsVisible,
                label + ": motion off retains a static loading glyph");
            iconModel.MotionEnabled = true;
            ControlPreview.Flush();
            check(spinner.IsSpinning, label + ": motion re-enable resumes during loading");
            iconModel.IsLoading = false;
            ControlPreview.Flush();
            check(target.IsEnabled && !spinner.IsSpinning,
                label + ": IconButton restores enabled state and stops hidden animation");
            var buttonModel = (ButtonPageViewModel)buttons.Model;
            var native = buttons.FindControl<Button>("LoadingSample")!;
            var count = 0;
            native.Click += (_, _) => count++;
            native.Command = ReactiveCommand.Create<object?>(_ => buttonModel.IsLoading = true);
            native.Focus();
            Press(window, Key.Enter, PhysicalKey.Enter);
            ControlPreview.Flush();
            check(buttonModel.Enabled, label + ": loading preserves the two-way enabled source value");
            check(count == 1 && ButtonLoading.GetIsLoading(native) && !native.IsEffectivelyEnabled,
                label + ": keyboard command enters native Button loading");
            Click(native);
            Press(window, Key.Enter, PhysicalKey.Enter);
            Press(window, Key.Space, PhysicalKey.Space);
            ControlPreview.Flush();
            check(count == 1, label + ": loading blocks repeated pointer and keyboard clicks");
            buttonModel.Enabled = false;
            buttonModel.IsLoading = false;
            ControlPreview.Flush();
            check(!native.IsEnabled, label + ": existing disabled binding survives loading");
            buttonModel.Enabled = true;
            ControlPreview.Flush();
            check(native.IsEnabled, label + ": IsEnabled binding still updates after loading");
            native.Focus();
            Press(window, Key.Enter, PhysicalKey.Enter);
            ControlPreview.Flush();
            check(count == 2, label + ": input resumes after loading");
            var container = page.FindControl<LoadingContainer>("LoadingSample")!;
            var action = page.FindControl<Button>("ContentAction")!;
            var input = page.FindControl<TextBox>("ContentInput")!;
            input.Text = "Retained text";
            input.Focus();
            page.Model.IsLoading = true;
            ControlPreview.Flush();
            var overlay = Part<Border>(container, "PART_LoadingOverlay");
            var containerSpinner = Part<LoadingIndicator>(container, "PART_LoadingIndicator");
            check(
                overlay.IsVisible && !action.IsEffectivelyEnabled && containerSpinner.Width ==
                (size == ControlSize.Small ? 22 : size == ControlSize.Large ? 34 : 28),
                label + ": container blocks content with a sized overlay");
            var mask = Part<Border>(container, "PART_OverlayBackground");
            check(Math.Abs(mask.Opacity - .65) < .001 && overlay.Opacity == 1,
                label + ": only the mask background uses default transparency");
            var clicks = 0;
            action.Click += (_, _) => clicks++;
            foreach (var opacity in new[] { 0d, .25, .65, 1d })
            {
                page.Model.OverlayOpacity = opacity;
                ControlPreview.Flush();
                Click(action);
                action.Focus();
                Press(window, Key.Enter, PhysicalKey.Enter);
                Click(input);
                window.KeyTextInput("Blocked");
                ControlPreview.Flush();
                check(mask.Opacity == opacity && clicks == 0 && input.Text == "Retained text",
                    label + $": opacity {opacity} blocks pointer, keyboard and editing");
            }

            var slider = page.FindControl<Slider>("OverlayOpacitySlider")!;
            slider.Value = .45;
            ControlPreview.Flush();
            check(page.Model.OverlayOpacity == .45 && mask.Opacity == .45,
                label + ": overlay opacity supports compiled two-way preview binding");
            container.OverlayBrush = Brushes.Crimson;
            ControlPreview.Flush();
            check(Equals(mask.Background, Brushes.Crimson), label + ": consumer mask brush overrides theme");
            container.ClearValue(LoadingContainer.OverlayBrushProperty);
            page.Model.UseCustomIcon = true;
            ControlPreview.Flush();
            var iconPresenter = Part<ContentPresenter>(container, "PART_LoadingIconPresenter");
            check(
                iconPresenter.IsVisible && iconPresenter.Child is PathIcon { Data: not null } &&
                !containerSpinner.IsVisible && !containerSpinner.IsSpinning,
                label + ": bound geometry replaces and stops the default indicator");
            page.Model.UseMaterialIcon = true;
            ControlPreview.Flush();
            check(
                iconPresenter.GetVisualDescendants().OfType<MaterialIcon>().Single().Kind ==
                MaterialIconKind.CloudDownload,
                label + ": implicit compiled Material template renders bound icon data");
            container.LoadingIconTemplate = icons.FindControl<IconButton>("MaterialIcon")!.IconTemplate;
            ControlPreview.Flush();
            check(
                iconPresenter.GetVisualDescendants().OfType<MaterialIcon>().Single().Kind ==
                MaterialIconKind.CloudDownload, label + ": explicit compiled LoadingIconTemplate is honored");
            container.LoadingIconTemplate = null;
            var customControl = new PathIcon { Data = Geometry.Parse("M0,0 H16 V16 H0 Z") };
            container.SetCurrentValue(LoadingContainer.LoadingIconProperty, customControl);
            container.LoadingIconSize = 40;
            ControlPreview.Flush();
            check(iconPresenter.Child == customControl && iconPresenter.Width == 40,
                label + ": control icons and local icon size are supported");
            container.ClearValue(LoadingContainer.LoadingIconSizeProperty);
            page.Model.LoadingText = "";
            ControlPreview.Flush();
            check(!Part<TextBlock>(container, "PART_LoadingText").IsEffectivelyVisible,
                label + ": empty text removes the caption and its spacing");
            page.Model.UseCustomIcon = false;
            ControlPreview.Flush();
            check(containerSpinner.IsVisible && containerSpinner.IsSpinning && !iconPresenter.IsVisible,
                label + ": clearing custom icon restores motion-aware default indicator");
            page.Model.LoadingText = "Custom progress";
            page.Model.MotionEnabled = false;
            ControlPreview.Flush();
            check(
                !containerSpinner.IsSpinning && overlay.GetVisualDescendants().OfType<TextBlock>()
                    .Any(text => text.Text == "Custom progress"),
                label + ": compiled loading text updates without animation");
            page.Model.MotionEnabled = true;
            ControlPreview.Flush();
            check(containerSpinner.IsSpinning, label + ": container resumes animation without losing loading state");
            page.Model.IsLoading = false;
            ControlPreview.Flush();
            check(
                !overlay.IsVisible && action.IsEffectivelyEnabled && input.Text == "Retained text" &&
                page.Model.Text == "Retained text" && !containerSpinner.IsSpinning,
                label + ": content state and two-way binding survive loading");
            page.Model.IsLoading = true;
            ControlPreview.Flush();
            window.Close();
            ControlPreview.Flush();
            check(!containerSpinner.IsSpinning, label + ": detached indicators stop animations");
        }

        var asyncPage = new LoadingContainerPage();
        var asyncWindow = Open(asyncPage, ThemeVariant.Light, ControlSize.Normal);
        Click(asyncPage.FindControl<Button>("LoadAction")!);
        Click(asyncPage.FindControl<Button>("LoadAction")!);
        check(asyncPage.Model.IsLoading && asyncPage.Model.OperationCount == 1,
            "Loading demo starts one asynchronous operation for repeated clicks");
        var frame = new DispatcherFrame();
        DispatcherTimer.RunOnce(() => frame.Continue = false, TimeSpan.FromMilliseconds(3200));
        Dispatcher.UIThread.PushFrame(frame);
        ControlPreview.Flush();
        check(!asyncPage.Model.IsLoading && asyncPage.Model.OperationCount == 1,
            "Loading demo completes and restores content after async execution");
        asyncWindow.Close();
        ControlPreview.Flush();
        Console.WriteLine("Runtime dynamic code supported: " + RuntimeFeature.IsDynamicCodeSupported);
    }

    public static void Render(string output, string name)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            Control view = name switch
            {
                "LoadingContainer" => new LoadingContainerPage(), "ButtonLoading" => new ButtonPage(),
                _ => new IconButtonPage()
            };
            var model = (LoadingDemoViewModel)view.DataContext!;
            model.IsLoading = true;
            model.MotionEnabled = false;
            var window = Open(view, theme, size);
            ControlPreview.Flush();
            using var bitmap = new RenderTargetBitmap(new PixelSize(1000, 1000));
            bitmap.Render(window);
            bitmap.Save(Path.Combine(output, $"{name}-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
            window.Close();
            ControlPreview.Flush();
        }
    }

    private static Window Open(Control content, ThemeVariant theme, ControlSize size)
    {
        var window = new Window
        {
            Width = 1000, Height = 1000, RequestedThemeVariant = theme,
            Content = new ScrollViewer { Content = new Border { Padding = new Thickness(24), Child = content } }
        };
        ControlSizing.SetSize(window, size);
        window.Show();
        ControlPreview.Flush();
        return window;
    }

    private static T Part<T>(Control control, string name) where T : Control
    {
        return control.GetVisualDescendants().OfType<T>().Single(part =>
            part.Name == name && (control is not LoadingContainer || part.TemplatedParent == control));
    }

    private static void Pump(int milliseconds)
    {
        var frame = new DispatcherFrame();
        DispatcherTimer.RunOnce(() => frame.Continue = false, TimeSpan.FromMilliseconds(milliseconds));
        Dispatcher.UIThread.PushFrame(frame);
        ControlPreview.Flush();
    }

    private static void Press(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        ControlPreview.Flush();
    }

    private static void Click(Control control)
    {
        var window = (Window)TopLevel.GetTopLevel(control)!;
        control.BringIntoView();
        ControlPreview.Flush();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
            .Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }
}