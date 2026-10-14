using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Controls.Templates;
using global::Avalonia.LogicalTree;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A themed information container with a title, icon, actions, body and footer.</summary>
[PseudoClasses(":has-header", ":has-title", ":has-icon", ":has-actions", ":has-body", ":has-footer")]
[TemplatePart("PART_HeaderIconPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_HeaderActionsPresenter", typeof(ContentPresenter))]
[TemplatePart("PART_FooterPresenter", typeof(ContentPresenter))]
public class InfoCard : HeaderedContentControl
{
    public static readonly StyledProperty<object?> HeaderIconProperty = AvaloniaProperty.Register<InfoCard, object?>(nameof(HeaderIcon));
    public static readonly StyledProperty<IDataTemplate?> HeaderIconTemplateProperty = AvaloniaProperty.Register<InfoCard, IDataTemplate?>(nameof(HeaderIconTemplate));
    public static readonly StyledProperty<object?> HeaderActionsProperty = AvaloniaProperty.Register<InfoCard, object?>(nameof(HeaderActions));
    public static readonly StyledProperty<IDataTemplate?> HeaderActionsTemplateProperty = AvaloniaProperty.Register<InfoCard, IDataTemplate?>(nameof(HeaderActionsTemplate));
    public static readonly StyledProperty<object?> FooterProperty = AvaloniaProperty.Register<InfoCard, object?>(nameof(Footer));
    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty = AvaloniaProperty.Register<InfoCard, IDataTemplate?>(nameof(FooterTemplate));
    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty = AvaloniaProperty.Register<InfoCard, IBrush?>(nameof(HeaderBackground));
    public static readonly StyledProperty<IBrush?> FooterBackgroundProperty = AvaloniaProperty.Register<InfoCard, IBrush?>(nameof(FooterBackground));
    public static readonly DirectProperty<InfoCard, Thickness> HeaderPaddingProperty = AvaloniaProperty.RegisterDirect<InfoCard, Thickness>(nameof(HeaderPadding), card => card.HeaderPadding);
    public static readonly DirectProperty<InfoCard, Thickness> FooterPaddingProperty = AvaloniaProperty.RegisterDirect<InfoCard, Thickness>(nameof(FooterPadding), card => card.FooterPadding);
    public static readonly StyledProperty<double> HeaderSpacingProperty = AvaloniaProperty.Register<InfoCard, double>(nameof(HeaderSpacing), 12, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<double> HeaderFontSizeProperty = AvaloniaProperty.Register<InfoCard, double>(nameof(HeaderFontSize), 16, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> HeaderIconSizeProperty = AvaloniaProperty.Register<InfoCard, double>(nameof(HeaderIconSize), 18, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<double> HeaderContentSpacingProperty = AvaloniaProperty.Register<InfoCard, double>(nameof(HeaderContentSpacing), 10, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<bool> ShowHeaderSeparatorProperty = AvaloniaProperty.Register<InfoCard, bool>(nameof(ShowHeaderSeparator), true);
    public static readonly StyledProperty<bool> ShowFooterSeparatorProperty = AvaloniaProperty.Register<InfoCard, bool>(nameof(ShowFooterSeparator), true);
    public static readonly StyledProperty<double> FooterSpacingProperty = AvaloniaProperty.Register<InfoCard, double>(nameof(FooterSpacing), 12, validate: value => double.IsFinite(value) && value >= 0);
    private Thickness _headerPadding;
    private Thickness _footerPadding;

    static InfoCard()
    {
        HeaderIconProperty.Changed.AddClassHandler<InfoCard>((card, change) => card.SlotChanged(change));
        HeaderActionsProperty.Changed.AddClassHandler<InfoCard>((card, change) => card.SlotChanged(change));
        FooterProperty.Changed.AddClassHandler<InfoCard>((card, change) => card.SlotChanged(change));
    }

    public InfoCard() => UpdateState();

    public object? HeaderIcon { get => GetValue(HeaderIconProperty); set => SetValue(HeaderIconProperty, value); }
    public IDataTemplate? HeaderIconTemplate { get => GetValue(HeaderIconTemplateProperty); set => SetValue(HeaderIconTemplateProperty, value); }
    public object? HeaderActions { get => GetValue(HeaderActionsProperty); set => SetValue(HeaderActionsProperty, value); }
    public IDataTemplate? HeaderActionsTemplate { get => GetValue(HeaderActionsTemplateProperty); set => SetValue(HeaderActionsTemplateProperty, value); }
    public object? Footer { get => GetValue(FooterProperty); set => SetValue(FooterProperty, value); }
    public IDataTemplate? FooterTemplate { get => GetValue(FooterTemplateProperty); set => SetValue(FooterTemplateProperty, value); }
    /// <summary>Background of the full-width header region.</summary>
    public IBrush? HeaderBackground { get => GetValue(HeaderBackgroundProperty); set => SetValue(HeaderBackgroundProperty, value); }
    /// <summary>Background of the full-width footer region; Background controls the body.</summary>
    public IBrush? FooterBackground { get => GetValue(FooterBackgroundProperty); set => SetValue(FooterBackgroundProperty, value); }
    /// <summary>Computed header insets: horizontal Padding and vertical HeaderSpacing.</summary>
    public Thickness HeaderPadding => _headerPadding;
    /// <summary>Computed footer insets: horizontal Padding and vertical FooterSpacing.</summary>
    public Thickness FooterPadding => _footerPadding;
    public double HeaderSpacing { get => GetValue(HeaderSpacingProperty); set => SetValue(HeaderSpacingProperty, value); }
    public double HeaderFontSize { get => GetValue(HeaderFontSizeProperty); set => SetValue(HeaderFontSizeProperty, value); }
    public double HeaderIconSize { get => GetValue(HeaderIconSizeProperty); set => SetValue(HeaderIconSizeProperty, value); }
    public double HeaderContentSpacing { get => GetValue(HeaderContentSpacingProperty); set => SetValue(HeaderContentSpacingProperty, value); }
    /// <summary>Shows an edge-to-edge divider between the header and body.</summary>
    public bool ShowHeaderSeparator { get => GetValue(ShowHeaderSeparatorProperty); set => SetValue(ShowHeaderSeparatorProperty, value); }
    /// <summary>Shows a divider before an existing footer when another region precedes it.</summary>
    public bool ShowFooterSeparator { get => GetValue(ShowFooterSeparatorProperty); set => SetValue(ShowFooterSeparatorProperty, value); }
    public double FooterSpacing { get => GetValue(FooterSpacingProperty); set => SetValue(FooterSpacingProperty, value); }
    protected override Type StyleKeyOverride => typeof(InfoCard);

    protected override bool RegisterContentPresenter(ContentPresenter presenter) =>
        base.RegisterContentPresenter(presenter) || presenter.Name is "PART_HeaderIconPresenter" or "PART_HeaderActionsPresenter" or "PART_FooterPresenter";

    private void SlotChanged(AvaloniaPropertyChangedEventArgs change)
    {
        // Keep caller-owned controls on the card's logical tree, as native Header/Content do.
        // A replacement ControlTemplate can then attach the same controls to new presenters.
        if (change.OldValue is ILogical oldChild) LogicalChildren.Remove(oldChild);
        if (change.NewValue is ILogical newChild && !LogicalChildren.Contains(newChild)) LogicalChildren.Add(newChild);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeaderProperty || change.Property == HeaderTemplateProperty ||
            change.Property == HeaderIconProperty || change.Property == HeaderIconTemplateProperty ||
            change.Property == HeaderActionsProperty || change.Property == HeaderActionsTemplateProperty ||
            change.Property == FooterProperty || change.Property == FooterTemplateProperty ||
            change.Property == ContentProperty || change.Property == ContentTemplateProperty ||
            change.Property == PaddingProperty || change.Property == HeaderSpacingProperty || change.Property == FooterSpacingProperty)
        {
            UpdateState();
        }
    }

    private void UpdateState()
    {
        SetAndRaise(HeaderPaddingProperty, ref _headerPadding, new Thickness(Padding.Left, HeaderSpacing, Padding.Right, HeaderSpacing));
        SetAndRaise(FooterPaddingProperty, ref _footerPadding, new Thickness(Padding.Left, FooterSpacing, Padding.Right, FooterSpacing));
        var title = HasContent(Header, HeaderTemplate);
        var icon = HasContent(HeaderIcon, HeaderIconTemplate);
        var actions = HasContent(HeaderActions, HeaderActionsTemplate);
        PseudoClasses.Set(":has-title", title);
        PseudoClasses.Set(":has-icon", icon);
        PseudoClasses.Set(":has-actions", actions);
        PseudoClasses.Set(":has-header", title || icon || actions);
        PseudoClasses.Set(":has-body", Content is not null || ContentTemplate is not null);
        PseudoClasses.Set(":has-footer", HasContent(Footer, FooterTemplate));
    }

    private static bool HasContent(object? content, IDataTemplate? template) =>
        template is not null || (content is not null && (content is not string text || !string.IsNullOrWhiteSpace(text)));
}
