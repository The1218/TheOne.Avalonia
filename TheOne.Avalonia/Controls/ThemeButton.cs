using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Interactivity;
using global::Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public enum ThemeButtonScope { Window, Application }

/// <summary>Switches the current effective light/dark theme without overriding a binding.</summary>
public class ThemeButton : Button
{
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
    }
    private void UpdateState()
    {
        var dark = ActualThemeVariant == ThemeVariant.Dark;
        PseudoClasses.Set(":dark", dark);
        var label = dark ? "切换浅色主题" : "切换深色主题";
        global::Avalonia.Automation.AutomationProperties.SetName(this, label);
        ToolTip.SetTip(this, label);
    }
    protected override void OnClick()
    {
        var next = ActualThemeVariant == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        if (Scope == ThemeButtonScope.Application && Application.Current is { } app)
            app.SetCurrentValue(Application.RequestedThemeVariantProperty, next);
        else if (TopLevel.GetTopLevel(this) is Window window)
            window.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, next);
        base.OnClick();
    }
}
