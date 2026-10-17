using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;

namespace TheOne.Avalonia.Controls;

/// <summary>A native Button with a separate bindable icon region. Geometry, controls and templated data are supported.</summary>
[PseudoClasses(":has-icon", ":icon-only")]
public class IconButton : Button
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<IconButton, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<IconButton, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<IconButton, double>(nameof(IconSize), 15,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<double> IconSpacingProperty =
        AvaloniaProperty.Register<IconButton, double>(nameof(IconSpacing), 8,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<IconPlacement> IconPlacementProperty =
        AvaloniaProperty.Register<IconButton, IconPlacement>(nameof(IconPlacement),
            validate: value => Enum.IsDefined(value));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        ButtonLoading.IsLoadingProperty.AddOwner<IconButton>();

    public static readonly DirectProperty<IconButton, bool> ShowIconProperty =
        AvaloniaProperty.RegisterDirect<IconButton, bool>(nameof(ShowIcon), control => control.ShowIcon);

    public static readonly DirectProperty<IconButton, bool> HasIconProperty =
        AvaloniaProperty.RegisterDirect<IconButton, bool>(nameof(HasIcon), control => control.HasIcon);

    public static readonly DirectProperty<IconButton, bool> HasLabelProperty =
        AvaloniaProperty.RegisterDirect<IconButton, bool>(nameof(HasLabel), control => control.HasLabel);

    public IconButton()
    {
        UpdateState();
    }

    public bool ShowIcon
    {
        get;
        private set => SetAndRaise(ShowIconProperty, ref field, value);
    }

    public IconPlacement IconPlacement
    {
        get => GetValue(IconPlacementProperty);
        set => SetValue(IconPlacementProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public double IconSpacing
    {
        get => GetValue(IconSpacingProperty);
        set => SetValue(IconSpacingProperty, value);
    }

    public bool HasIcon
    {
        get;
        private set => SetAndRaise(HasIconProperty, ref field, value);
    }

    public bool HasLabel
    {
        get;
        private set => SetAndRaise(HasLabelProperty, ref field, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty || change.Property == ContentProperty ||
            change.Property == IsLoadingProperty) UpdateState();
    }

    private void UpdateState()
    {
        HasIcon = Icon is not null;
        HasLabel = Content is not null && Content is not string { Length: 0 };
        ShowIcon = HasIcon && !IsLoading;
        PseudoClasses.Set(":has-icon", HasIcon);
        PseudoClasses.Set(":icon-only", !HasLabel);
    }
}