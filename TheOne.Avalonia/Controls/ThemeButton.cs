using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public enum ThemeButtonScope { Window, Application }

/// <summary>Switches the current effective light/dark theme without overriding a binding.</summary>
public class ThemeButton : ToggleButton
{
    private bool _updatingState;
    public static readonly StyledProperty<ThemeButtonScope> ScopeProperty =
        AvaloniaProperty.Register<ThemeButton, ThemeButtonScope>(nameof(Scope));
    public ThemeButtonScope Scope { get => GetValue(ScopeProperty); set => SetValue(ScopeProperty, value); }
    protected override Type StyleKeyOverride => typeof(ThemeButton);
    public ThemeButton()
    {
        AttachedToVisualTree += (_, _) => UpdateState();
        ActualThemeVariantChanged += (_, _) => UpdateState();
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ThemeVariantScope.ActualThemeVariantProperty) UpdateState();
        if (change.Property == IsCheckedProperty && !_updatingState && IsChecked is { } dark)
            ApplyTheme(dark ? ThemeVariant.Dark : ThemeVariant.Light);
    }
    private void UpdateState()
    {
        var dark = ActualThemeVariant == ThemeVariant.Dark;
        _updatingState = true;
        try { SetCurrentValue(IsCheckedProperty, dark); }
        finally { _updatingState = false; }
        PseudoClasses.Set(":dark", dark);
        var label = dark ? "切换浅色主题" : "切换深色主题";
        global::Avalonia.Automation.AutomationProperties.SetName(this, label);
        ToolTip.SetTip(this, label);
    }
    private void ApplyTheme(ThemeVariant next)
    {
        if (Scope == ThemeButtonScope.Application && Application.Current is { } app)
            app.SetCurrentValue(Application.RequestedThemeVariantProperty, next);
        else if (TopLevel.GetTopLevel(this) is Window window)
            window.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, next);
    }
}
