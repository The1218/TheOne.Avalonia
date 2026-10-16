using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Data;
using global::Avalonia.Layout;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Styling;

/// <summary>Scopes chrome and density to the official picker's detached flyout tree.</summary>
internal static class ColorPickerStyling
{
    static ColorPickerStyling()
    {
        Control.LoadedEvent.AddClassHandler<DropDownButton>((button, _) => Apply(button));
        Control.LoadedEvent.AddClassHandler<TabControl>((tabs, _) =>
        {
            if (tabs.Name == "PART_TabControl" && tabs.TemplatedParent is ColorView &&
                tabs.Items.OfType<TabItem>().FirstOrDefault()?.Content is Grid grid && grid.Children.OfType<ColorSpectrum>().FirstOrDefault() is { } spectrum)
                Dispatcher.UIThread.Post(() => ApplyLayout(spectrum, tabs));
        });
        Control.BoundsProperty.Changed.AddClassHandler<ColorSpectrum>((spectrum, _) =>
        {
            if (spectrum.TemplatedParent is ColorView && spectrum.GetVisualParent() is Grid)
                Dispatcher.UIThread.Post(() => ApplyLayout(spectrum));
        });
        Control.IsVisibleProperty.Changed.AddClassHandler<ColorSpectrum>((spectrum, _) =>
        {
            if (spectrum.TemplatedParent is ColorView && spectrum.GetVisualParent() is Grid)
                Dispatcher.UIThread.Post(() => ApplyLayout(spectrum));
        });
        ControlSizing.SizeProperty.Changed.AddClassHandler<ColorPicker>((picker, _) =>
        {
            foreach (var button in picker.GetVisualDescendants().OfType<DropDownButton>()) Apply(button);
        });
    }

    internal static void Initialize() { }

    private static void ApplyLayout(ColorSpectrum spectrum, TabControl? tabs = null)
    {
        // Keep the original template instances and name scope, including native Hex handlers.
        if (spectrum.TemplatedParent is not ColorView || spectrum.GetVisualParent() is not Grid grid ||
            !grid.Children.OfType<ColorSlider>().Any(slider => slider.Name == "ColorSpectrumThirdComponentSlider")) return;
        if (grid.ColumnDefinitions.Count == 3)
        {
            grid.ColumnDefinitions.Clear();
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            grid.RowDefinitions.Clear();
            grid.RowDefinitions.Add(new RowDefinition(new GridLength(200)));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            foreach (var slider in grid.Children.OfType<ColorSlider>())
                slider.SetCurrentValue(Grid.RowProperty, slider.Name == "ColorSpectrumThirdComponentSlider" ? 2 : 3);
        }
        tabs ??= spectrum.GetVisualAncestors().OfType<TabControl>().FirstOrDefault();
        if (tabs?.Items.Count == 3 && tabs.Items[2] is TabItem values && values.Content is Grid components)
        {
            values.Content = null;
            ArrangeComponents(components);
            grid.Children.Add(components);
            components.SetCurrentValue(Grid.RowProperty, 1);
            components.Bind(Visual.IsVisibleProperty, new TemplateBinding { Property = ColorView.IsColorComponentsVisibleProperty });
            tabs.Items.Remove(values);
        }
        if (grid.ColumnDefinitions.Count == 1 && grid.RowDefinitions.Count == 4 && (!spectrum.IsVisible || spectrum.Bounds.Width > 0))
            grid.RowDefinitions[0].Height = new GridLength(spectrum.IsVisible ? spectrum.Bounds.Width : 0);
    }

    private static void ArrangeComponents(Grid components)
    {
        components.Classes.Add("one-color-components");
        components.Margin = new Thickness(0, 10, 0, 0);
        var header = components.Children.OfType<Grid>().First();
        var model = header.Children.OfType<Grid>().First();
        model.ColumnSpacing = 4;
        var hex = header.Children.OfType<Grid>().Single(g => g.Name == "HexInputGrid");
        header.Children.Remove(model); header.Children.Remove(hex); components.Children.Remove(header);
        var labels = components.Children.OfType<Border>().OrderBy(Grid.GetRow).ToArray();
        var inputs = components.Children.OfType<NumericUpDown>().OrderBy(Grid.GetRow).ToArray();
        components.ColumnDefinitions.Clear();
        foreach (var column in new ColumnDefinitions("80,6,*,6,*,6,*,6,*")) components.ColumnDefinitions.Add(column);
        components.RowDefinitions.Clear();
        foreach (var row in new RowDefinitions("16,4,Auto,8,Auto")) components.RowDefinitions.Add(row);
        foreach (var slider in components.Children.OfType<ColorSlider>()) slider.SetValue(Visual.IsVisibleProperty, false);
        for (var i = 0; i < inputs.Length; i++)
        {
            var label = labels[i];
            label.Classes.Add("one-color-component-label");
            label.SetValue(TemplatedControl.WidthProperty, double.NaN);
            label.SetValue(TemplatedControl.HeightProperty, 16d);
            label.SetValue(Border.BorderThicknessProperty, default(Thickness));
            label.SetValue(Border.CornerRadiusProperty, default(CornerRadius));
            label.SetCurrentValue(Grid.RowProperty, 0); label.SetCurrentValue(Grid.ColumnProperty, 2 + i * 2);
            var input = inputs[i];
            // Replace only template geometry; keep Value, ranges and editing bindings untouched.
            input.SetValue(TemplatedControl.WidthProperty, double.NaN);
            input.SetValue(Layoutable.MinWidthProperty, 0d);
            input.SetValue(TemplatedControl.PaddingProperty, new Thickness(4, 0));
            input.SetValue(TemplatedControl.CornerRadiusProperty, new CornerRadius(5));
            input.SetValue(Layoutable.MarginProperty, default(Thickness));
            input.SetValue(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            input.SetCurrentValue(Grid.RowProperty, 2); input.SetCurrentValue(Grid.ColumnProperty, 2 + i * 2);
        }
        var modelLabel = new TextBlock { Text = "Model", HorizontalAlignment = HorizontalAlignment.Center };
        modelLabel.Classes.Add("one-color-component-label");
        modelLabel.Bind(Visual.IsVisibleProperty, model.GetObservable(Visual.IsVisibleProperty));
        components.Children.Add(modelLabel); components.Children.Add(model); components.Children.Add(hex);
        model.SetCurrentValue(Grid.RowProperty, 2); model.SetCurrentValue(Grid.ColumnProperty, 0);
        model.SetCurrentValue(Grid.ColumnSpanProperty, 1);
        hex.SetCurrentValue(Grid.RowProperty, 4); hex.SetCurrentValue(Grid.ColumnProperty, 0);
        hex.SetCurrentValue(Grid.ColumnSpanProperty, 9);
    }

    private static void Apply(DropDownButton button)
    {
        if (button.TemplatedParent is not ColorPicker owner || button.Flyout is not Flyout flyout || !flyout.FlyoutPresenterClasses.Contains("nopadding")) return;
        var size = ControlSizing.GetSize(owner);
        ApplyClasses(flyout.FlyoutPresenterClasses, size);
        if (flyout.Content is Control content)
        {
            ControlSizing.SetSize(content, size);
            var presenter = content.GetVisualAncestors().OfType<FlyoutPresenter>().FirstOrDefault();
            if (presenter != null) ApplyClasses(presenter.Classes, size);
        }
    }

    private static void ApplyClasses(IList<string> classes, ControlSize size)
    {
        if (!classes.Contains("one-colorpicker")) classes.Add("one-colorpicker");
        foreach (var name in new[] { "one-colorpicker-small", "one-colorpicker-normal", "one-colorpicker-large" }) classes.Remove(name);
        classes.Add(size switch { ControlSize.Small => "one-colorpicker-small", ControlSize.Large => "one-colorpicker-large", _ => "one-colorpicker-normal" });
    }
}
