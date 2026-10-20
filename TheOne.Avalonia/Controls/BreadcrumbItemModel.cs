using Avalonia;

namespace TheOne.Avalonia.Controls;

/// <summary>Optional, observable breadcrumb data. Key is caller-owned navigation identity.</summary>
public sealed class BreadcrumbItemModel : AvaloniaObject
{
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<BreadcrumbItemModel, object?>(nameof(Header));
    public static readonly StyledProperty<OneIconKind?> IconProperty = AvaloniaProperty.Register<BreadcrumbItemModel, OneIconKind?>(nameof(Icon),
        validate: value => value is null || Enum.IsDefined(value.Value));
    public static readonly StyledProperty<object?> KeyProperty = AvaloniaProperty.Register<BreadcrumbItemModel, object?>(nameof(Key));
    public static readonly StyledProperty<bool> IsEnabledProperty = AvaloniaProperty.Register<BreadcrumbItemModel, bool>(nameof(IsEnabled), true);

    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public OneIconKind? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public object? Key { get => GetValue(KeyProperty); set => SetValue(KeyProperty, value); }
    public bool IsEnabled { get => GetValue(IsEnabledProperty); set => SetValue(IsEnabledProperty, value); }
}
