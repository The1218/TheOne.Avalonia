using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A native node card. Header displays the model's Title.</summary>
[PseudoClasses(":has-icon", ":has-title", ":has-content", ":icon-only", ":root", ":selected")]
[TemplatePart("PART_IconPresenter", typeof(ContentPresenter))]
public class RelationGraphNodePresenter : HeaderedContentControl
{
    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, double>(nameof(IconSize), 24, validate: Positive);

    public static readonly StyledProperty<double> IconTileSizeProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, double>(nameof(IconTileSize), 44, validate: Positive);

    public static readonly StyledProperty<double> IconSpacingProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, double>(nameof(IconSpacing), 14, validate: NonNegative);

    public static readonly StyledProperty<double> TextSpacingProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, double>(nameof(TextSpacing), 6, validate: NonNegative);

    public static readonly StyledProperty<double> HeaderFontSizeProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, double>(nameof(HeaderFontSize), 16, validate: Positive);

    public static readonly StyledProperty<IBrush?> ContentForegroundProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, IBrush?>(nameof(ContentForeground));

    public static readonly StyledProperty<IBrush?> IconBackgroundProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, IBrush?>(nameof(IconBackground));

    public static readonly StyledProperty<IBrush?> IconForegroundProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, IBrush?>(nameof(IconForeground));

    public static readonly StyledProperty<bool> IsRootProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, bool>(nameof(IsRoot));

    public static readonly DirectProperty<RelationGraphNodePresenter, bool> IsSelectedProperty =
        AvaloniaProperty.RegisterDirect<RelationGraphNodePresenter, bool>(nameof(IsSelected), node => node.IsSelected);

    internal static readonly StyledProperty<object?> ChildrenSourceValueProperty =
        AvaloniaProperty.Register<RelationGraphNodePresenter, object?>("ChildrenSourceValue");

    private bool _isSelected;

    public RelationGraphNodePresenter()
    {
        UpdateState();
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

    public double IconTileSize
    {
        get => GetValue(IconTileSizeProperty);
        set => SetValue(IconTileSizeProperty, value);
    }

    public double IconSpacing
    {
        get => GetValue(IconSpacingProperty);
        set => SetValue(IconSpacingProperty, value);
    }

    public double TextSpacing
    {
        get => GetValue(TextSpacingProperty);
        set => SetValue(TextSpacingProperty, value);
    }

    public double HeaderFontSize
    {
        get => GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public IBrush? ContentForeground
    {
        get => GetValue(ContentForegroundProperty);
        set => SetValue(ContentForegroundProperty, value);
    }

    public IBrush? IconBackground
    {
        get => GetValue(IconBackgroundProperty);
        set => SetValue(IconBackgroundProperty, value);
    }

    public IBrush? IconForeground
    {
        get => GetValue(IconForegroundProperty);
        set => SetValue(IconForegroundProperty, value);
    }

    public bool IsRoot
    {
        get => GetValue(IsRootProperty);
        set => SetValue(IsRootProperty, value);
    }

    /// <summary>Selection state maintained by the owning RelationGraph.</summary>
    public bool IsSelected => _isSelected;

    protected override Type StyleKeyOverride => typeof(RelationGraphNodePresenter);

    internal void SetSelected(bool selected)
    {
        SetAndRaise(IsSelectedProperty, ref _isSelected, selected);
        PseudoClasses.Set(":selected", selected);
    }

    protected override bool RegisterContentPresenter(ContentPresenter presenter)
    {
        return base.RegisterContentPresenter(presenter) || presenter.Name == "PART_IconPresenter";
    }

    private static bool Positive(double value)
    {
        return double.IsFinite(value) && value > 0;
    }

    private static bool NonNegative(double value)
    {
        return double.IsFinite(value) && value >= 0;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IconProperty)
        {
            if (change.OldValue is ILogical oldChild) LogicalChildren.Remove(oldChild);
            if (change.NewValue is ILogical newChild && !LogicalChildren.Contains(newChild))
                LogicalChildren.Add(newChild);
        }

        if (change.Property == IconProperty || change.Property == HeaderProperty ||
            change.Property == ContentProperty || change.Property == IsRootProperty) UpdateState();
    }

    private void UpdateState()
    {
        PseudoClasses.Set(":has-icon", Icon is not null);
        PseudoClasses.Set(":has-title", Header is not null);
        PseudoClasses.Set(":has-content", Content is not null);
        PseudoClasses.Set(":icon-only", Icon is not null && Header is null && Content is null);
        PseudoClasses.Set(":root", IsRoot);
    }
}