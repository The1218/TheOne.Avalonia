using global::Avalonia.Markup.Xaml;
using global::Avalonia.Styling;

namespace TheOne.Avalonia.Themes;

/// <summary>TheOne theme for Avalonia's built-in controls, including light and dark resources.</summary>
public partial class OneTheme : Styles
{
    public OneTheme()
    {
        _ = Styling.ControlSizing.SizeProperty;
        _ = Styling.Motion.EnabledProperty;
        Styling.ColorPickerStyling.Initialize();
        AvaloniaXamlLoader.Load(this);
    }
}
