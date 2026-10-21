using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Creates a preview section only when its reserved area intersects the actual viewport.</summary>
internal sealed class PreviewDeferredContent : ContentControl
{
    private Func<Control>? _create;
    private double? _estimatedHeight;

    public PreviewDeferredContent(Func<Control> create, double? estimatedHeight = null, bool keyboardEntry = false)
    {
        _create = create;
        _estimatedHeight = estimatedHeight;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        Focusable = keyboardEntry;
        EffectiveViewportChanged += ViewportChanged;
    }

    internal bool IsMaterialized => _create is null;

    internal double? EstimatedHeight
    {
        get => _estimatedHeight;
        set
        {
            if (_estimatedHeight == value) return;
            _estimatedHeight = value;
            if (_create is not null) InvalidateMeasure();
        }
    }

    protected override Type StyleKeyOverride => typeof(ContentControl);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_create is not null && _estimatedHeight is null && change.Property == ControlSizing.SizeProperty)
            InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_create is null) return base.MeasureOverride(availableSize);
        var height = _estimatedHeight ?? (ControlSizing.GetSize(this) switch
        {
            ControlSize.Small => 224, ControlSize.Large => 336, _ => 280
        });
        return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : 400, height);
    }

    private void ViewportChanged(object? sender, EffectiveViewportChangedEventArgs args)
    {
        if (Bounds.Width > 0 && Bounds.Height > 0 && args.EffectiveViewport.Intersects(new Rect(Bounds.Size)))
            Materialize();
    }

    internal void Materialize()
    {
        if (_create is not { } create) return;
        _create = null;
        EffectiveViewportChanged -= ViewportChanged;
        Content = create();
        if (!IsFocused) Focusable = false;
    }

    protected override void OnGotFocus(FocusChangedEventArgs args)
    {
        base.OnGotFocus(args);
        if (!ReferenceEquals(args.NewFocusedElement, this)) return;
        // Uncreated examples must remain reachable through native Tab navigation.
        // Resolve focus synchronously, without delayed callbacks that could steal it later.
        Materialize();
        UpdateLayout();
        var manager = TopLevel.GetTopLevel(this)?.FocusManager;
        IInputElement? Next(IInputElement element, NavigationDirection direction)
        {
            var visited = new HashSet<IInputElement>();
            while (visited.Add(element))
            {
                var candidate = manager?.FindNextElement(direction, new FindNextElementOptions { FocusedElement = element });
                if (candidate is null) return null;
                if (candidate is InputElement { IsEffectivelyEnabled: true, IsEffectivelyVisible: true, Focusable: true })
                    return candidate;
                element = candidate;
            }
            return null;
        }
        var backwards = (args.KeyModifiers & KeyModifiers.Shift) != 0;
        var next = Next(this, NavigationDirection.Next);
        if (backwards)
        {
            if (Inside(next))
            {
                var visited = new HashSet<IInputElement>();
                while (next is not null && visited.Add(next))
                {
                    var following = Next(next, NavigationDirection.Next);
                    if (!Inside(following) || visited.Contains(following!)) break;
                    next = following;
                }
            }
            else next = Next(this, NavigationDirection.Previous);
        }
        if (next is not null && !ReferenceEquals(next, this)) next.Focus(NavigationMethod.Tab, args.KeyModifiers);
        Focusable = false;
    }

    private bool Inside(IInputElement? element) => element is Visual visual && visual.GetVisualAncestors().Contains(this);
}
