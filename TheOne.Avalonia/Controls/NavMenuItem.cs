using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Data;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>A recycled native tree container; group rows expand without navigating.</summary>
[PseudoClasses(":group", ":compact", ":active-ancestor", ":cascade")]
public class NavMenuItem : TreeViewItem
{
    public static readonly StyledProperty<bool> IsGroupProperty = AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsGroup));
    public static readonly StyledProperty<bool> IsCompactProperty = AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsCompact));
    public static readonly StyledProperty<bool> IsActiveAncestorProperty = AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsActiveAncestor));
    public static readonly StyledProperty<bool> IsCascadeItemProperty = AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsCascadeItem));
    public bool IsCascadeItem { get => GetValue(IsCascadeItemProperty); set => SetValue(IsCascadeItemProperty, value); }
    public bool IsCompact { get => GetValue(IsCompactProperty); set => SetValue(IsCompactProperty, value); }
    public bool IsActiveAncestor { get => GetValue(IsActiveAncestorProperty); set => SetValue(IsActiveAncestorProperty, value); }
    public bool IsGroup { get => GetValue(IsGroupProperty); set => SetValue(IsGroupProperty, value); }
    protected override Type StyleKeyOverride => typeof(NavMenuItem);
    private ToggleButton? _chevron;
    private Grid? _header;
    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        base.OnApplyTemplate(args);
        _chevron = args.NameScope.Find<ToggleButton>("PART_ExpandCollapseChevron");
        _header = args.NameScope.Find<Grid>("PART_Header");
        UpdateColumns();
        UpdateChevron();
        _reveal = args.NameScope.Find<RevealHost>("TreeContent");
        BindReveal(_reveal);
    }
    private RevealHost? _reveal;
    public NavMenuItem()
    {
        AttachedToVisualTree += (_, _) => BindReveal(_reveal);
        DetachedFromVisualTree += (_, _) => ClearRevealBindings();
    }
    private void ClearRevealBindings()
    {
        foreach (var binding in _revealBindings) binding.Dispose();
        _revealBindings.Clear();
    }
    private readonly List<IDisposable> _revealBindings = [];
    private void BindReveal(RevealHost? reveal)
    {
        ClearRevealBindings();
        if (reveal is null || this.GetVisualAncestors().OfType<NavMenu>().FirstOrDefault() is not { } menu) return;
        _revealBindings.Add(reveal.Bind(RevealHost.DurationProperty, CompiledBinding.Create<NavMenu, TimeSpan>(owner => owner.TransitionDuration, source: menu)));
        _revealBindings.Add(reveal.Bind(RevealHost.TransitionExtentProperty, CompiledBinding.Create<NavMenu, double>(owner => owner.TransitionExtent, source: menu)));
    }
    private void UpdateChevron()
    {
        if (_chevron is { } button)
        { button.SetCurrentValue(WidthProperty, MinHeight); button.SetCurrentValue(HeightProperty, MinHeight); }
    }
    private void UpdateColumns()
    {
        if (_header is not null) _header.ColumnDefinitions = ColumnDefinitions.Parse(IsCascadeItem ? "*,Auto" : "Auto,*");
    }
    protected override void OnKeyDown(KeyEventArgs args)
    {
        // Native TreeViewItem consumes Right/Left before the containing tree sees them.
        // Popup levels navigate adjacent flyouts instead of changing model expansion.
        if (Parent is NavMenu { UsesPopupNavigation: true } menu)
        {
            menu.HandlePopupKey(args);
            if (args.Handled) return;
        }
        base.OnKeyDown(args);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MinHeightProperty) UpdateChevron();
        if (change.Property == IsCascadeItemProperty) { PseudoClasses.Set(":cascade", IsCascadeItem); UpdateColumns(); }
        if (change.Property == IsCompactProperty) PseudoClasses.Set(":compact", IsCompact);
        if (change.Property == IsActiveAncestorProperty) PseudoClasses.Set(":active-ancestor", IsActiveAncestor);
        if (change.Property == IsGroupProperty) PseudoClasses.Set(":group", IsGroup);
    }
}
