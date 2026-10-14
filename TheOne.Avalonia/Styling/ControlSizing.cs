using global::Avalonia;
using global::Avalonia.Controls;

namespace TheOne.Avalonia.Styling;

public enum ControlSize { Small, Normal, Large }

/// <summary>Inheritable density: set on an individual control or on a whole form.</summary>
public sealed class ControlSizing : AvaloniaObject
{
    public static readonly AttachedProperty<ControlSize> SizeProperty =
        AvaloniaProperty.RegisterAttached<ControlSizing, Control, ControlSize>("Size", ControlSize.Normal, inherits: true);

    static ControlSizing()
    {
        SizeProperty.Changed.AddClassHandler<Control>((control, _) => Apply(control));
        // Default Normal does not raise Size.Changed; parent attachment initializes it.
        StyledElement.ParentProperty.Changed.AddClassHandler<Control>((control, _) => Apply(control));
        // Flyout reapplies FlyoutPresenterClasses before each opening and removes other
        // classes, including our inherited density marker. Restore it after attachment.
        Control.LoadedEvent.AddClassHandler<FlyoutPresenter>((control, _) => Apply(control));
    }

    public static ControlSize GetSize(Control control) => control.GetValue(SizeProperty);
    public static void SetSize(Control control, ControlSize value) => control.SetValue(SizeProperty, value);

    private static void Apply(Control control)
    {
        var size = GetSize(control);
        control.Classes.Set("one-small", size == ControlSize.Small);
        control.Classes.Set("one-normal", size == ControlSize.Normal);
        control.Classes.Set("one-large", size == ControlSize.Large);
    }
}
