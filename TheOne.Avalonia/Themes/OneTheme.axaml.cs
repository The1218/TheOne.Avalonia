using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Themes;

/// <summary>TheOne theme for Avalonia's built-in controls, including light and dark resources.</summary>
public class OneTheme : Styles
{
    public static readonly StyledProperty<ThemeColor> ColorThemeProperty =
        AvaloniaProperty.Register<OneTheme, ThemeColor>(nameof(ColorTheme), validate: value => Enum.IsDefined(value));

    private readonly ThemePaletteResources? _colorResources;

    public OneTheme()
    {
        _ = ControlSizing.SizeProperty;
        _ = Motion.EnabledProperty;
        ColorPickerStyling.Initialize();
        PopupScrollBoundary.Initialize();
        MenuFlyoutItems.Initialize();
        AvaloniaXamlLoader.Load(this);
        _colorResources = new ThemePaletteResources(ColorTheme, Resources.MergedDictionaries.ToArray());
        Resources.MergedDictionaries.Add(_colorResources);
    }

    /// <summary>Color palette; changing this does not change the requested Light / Dark variant.</summary>
    public ThemeColor ColorTheme
    {
        get => GetValue(ColorThemeProperty);
        set => SetValue(ColorThemeProperty, value);
    }

    public static IReadOnlyList<ThemeColor> ColorThemes { get; } = Array.AsReadOnly(Enum.GetValues<ThemeColor>());

    internal static OneTheme? ApplicationTheme => Application.Current?.Styles.OfType<OneTheme>().FirstOrDefault();

    /// <summary>Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings.</summary>
    public static void SetColor(ThemeColor color)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!Enum.IsDefined(color)) throw new ArgumentOutOfRangeException(nameof(color));
        var theme = ApplicationTheme ??
                    throw new InvalidOperationException(
                        "Register OneTheme in Application.Styles before switching its color palette.");
        theme.SetCurrentValue(ColorThemeProperty, color);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ColorThemeProperty) _colorResources?.SetColor(ColorTheme);
        base.OnPropertyChanged(change);
    }
}
