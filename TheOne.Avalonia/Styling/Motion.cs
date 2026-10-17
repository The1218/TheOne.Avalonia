using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Styling;

/// <summary>Set Enabled=False on a subtree to suppress theme motion.</summary>
public sealed class Motion : AvaloniaObject
{
    public static readonly AttachedProperty<bool> EnabledProperty =
        AvaloniaProperty.RegisterAttached<Motion, Control, bool>("Enabled", true, true);

    static Motion()
    {
        EnabledProperty.Changed.AddClassHandler<Control>((control, _) => Apply(control));
    }

    public static bool GetEnabled(Control control)
    {
        return control.GetValue(EnabledProperty);
    }

    public static void SetEnabled(Control control, bool value)
    {
        control.SetValue(EnabledProperty, value);
    }

    private static void Apply(Control control)
    {
        control.Classes.Set("one-no-motion", !GetEnabled(control));
    }
}