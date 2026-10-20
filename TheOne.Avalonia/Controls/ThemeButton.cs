using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public enum ThemeButtonScope
{
    Window,
    Application
}

/// <summary>Switches Light/Dark, optionally including Default (inherited/system theme), without replacing bindings.</summary>
public class ThemeButton : ToggleButton
{
    public static readonly StyledProperty<ThemeButtonScope> ScopeProperty =
        AvaloniaProperty.Register<ThemeButton, ThemeButtonScope>(nameof(Scope));

    private bool _updatingState;
    private AvaloniaObject? _themeTarget;

    public ThemeButton()
    {
        AttachedToVisualTree += (_, _) => ObserveThemeTarget();
        DetachedFromVisualTree += (_, _) => StopObservingThemeTarget();
    }

    public ThemeButtonScope Scope
    {
        get => GetValue(ScopeProperty);
        set => SetValue(ScopeProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(ThemeButton);

    protected override void Toggle()
    {
        // Default -> Dark -> Light -> Default. Keep native pointer/keyboard/command handling.
        SetCurrentValue(IsCheckedProperty, IsThreeState
            ? IsChecked switch { null => true, true => false, false => null }
            : IsChecked != true);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ScopeProperty && _themeTarget is not null) ObserveThemeTarget();
        if (change.Property == ThemeVariantScope.ActualThemeVariantProperty || change.Property == IsThreeStateProperty)
            UpdateState();
        if (change.Property == IsCheckedProperty && !_updatingState)
        {
            ApplyTheme(IsChecked switch
            {
                true => ThemeVariant.Dark,
                false => ThemeVariant.Light,
                null => ThemeVariant.Default
            });
            UpdateState();
        }
    }

    private AvaloniaObject? ThemeTarget => Scope == ThemeButtonScope.Application
        ? Application.Current
        : TopLevel.GetTopLevel(this) as Window;

    private void ObserveThemeTarget()
    {
        StopObservingThemeTarget();
        _themeTarget = ThemeTarget;
        if (_themeTarget is not null) _themeTarget.PropertyChanged += ThemeTargetChanged;
        UpdateState();
    }

    private void StopObservingThemeTarget()
    {
        if (_themeTarget is not null) _themeTarget.PropertyChanged -= ThemeTargetChanged;
        _themeTarget = null;
    }

    private void ThemeTargetChanged(object? sender, AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == Application.RequestedThemeVariantProperty ||
            change.Property == ThemeVariantScope.RequestedThemeVariantProperty ||
            change.Property == Application.ActualThemeVariantProperty ||
            change.Property == ThemeVariantScope.ActualThemeVariantProperty)
            UpdateState();
    }

    private void UpdateState()
    {
        var target = ThemeTarget;
        var requested = target is Application app ? app.RequestedThemeVariant
            : (target as Window)?.RequestedThemeVariant;
        var actual = target is Application application ? application.ActualThemeVariant
            : (target as Window)?.ActualThemeVariant ?? ActualThemeVariant;
        var dark = actual == ThemeVariant.Dark;
        var automatic = IsThreeState && (requested is null || requested == ThemeVariant.Default);
        _updatingState = true;
        try
        {
            SetCurrentValue(IsCheckedProperty, automatic ? null : dark);
        }
        finally
        {
            _updatingState = false;
        }

        PseudoClasses.Set(":dark", dark);
        PseudoClasses.Set(":default", automatic);
        var label = automatic ? "当前跟随系统／上级主题，切换深色主题"
            : dark ? "切换浅色主题" : IsThreeState ? "跟随系统／上级主题" : "切换深色主题";
        AutomationProperties.SetName(this, label);
        ToolTip.SetTip(this, label);
    }

    private void ApplyTheme(ThemeVariant next)
    {
        if (ThemeTarget is Application app)
            app.SetCurrentValue(Application.RequestedThemeVariantProperty, next);
        else if (ThemeTarget is Window window)
            window.SetCurrentValue(ThemeVariantScope.RequestedThemeVariantProperty, next);
    }
}
