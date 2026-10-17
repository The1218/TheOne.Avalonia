using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;

namespace TheOne.Avalonia.Controls;

public enum TagStatus
{
    Default,
    Information,
    Success,
    Warning,
    Error,
    Processing
}

public enum TagVariant
{
    Soft,
    Outline,
    Solid
}

/// <summary>A compact status label with optional caller-owned icon and content.</summary>
public class Tag : ContentControl
{
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<Tag, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<Tag, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<TagStatus> StatusProperty =
        AvaloniaProperty.Register<Tag, TagStatus>(nameof(Status), validate: Enum.IsDefined);

    public static readonly StyledProperty<TagVariant> VariantProperty =
        AvaloniaProperty.Register<Tag, TagVariant>(nameof(Variant), validate: Enum.IsDefined);

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<Tag, double>(nameof(IconSize), 13,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<Tag, double>(nameof(Spacing), 6,
            validate: value => double.IsFinite(value) && value >= 0);

    public Tag()
    {
        UpdateState();
    }

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
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

    public TagStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public TagVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Tag);

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        return base.RegisterContentPresenter(presenter) || presenter.Name == "PART_IconPresenter";
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty)
        {
            if (change.OldValue is ILogical oldIcon) LogicalChildren.Remove(oldIcon);
            if (change.NewValue is ILogical newIcon && !LogicalChildren.Contains(newIcon)) LogicalChildren.Add(newIcon);
        }

        if (change.Property == StatusProperty || change.Property == VariantProperty ||
            change.Property == IconProperty || change.Property == IconTemplateProperty ||
            change.Property == ContentProperty || change.Property == ContentTemplateProperty) UpdateState();
    }

    private void UpdateState()
    {
        foreach (var status in Enum.GetValues<TagStatus>())
            PseudoClasses.Set(":" + status.ToString().ToLowerInvariant(), Status == status);
        foreach (var variant in Enum.GetValues<TagVariant>())
            PseudoClasses.Set(":" + variant.ToString().ToLowerInvariant(), Variant == variant);
        PseudoClasses.Set(":has-content",
            ContentTemplate is not null ||
            (Content is not null && (Content is not string text || !string.IsNullOrEmpty(text))));
        PseudoClasses.Set(":has-icon", Icon is not null || IconTemplate is not null);
    }
}