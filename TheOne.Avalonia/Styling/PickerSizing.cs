using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Styling;

/// <summary>Shared row metrics keep the native picker viewport and selection band aligned.</summary>
public sealed class PickerSizing : AvaloniaObject
{
    public static readonly AttachedProperty<double> ItemHeightProperty =
        AvaloniaProperty.RegisterAttached<PickerSizing, Control, double>("ItemHeight", 40, true);

    public static readonly AttachedProperty<double> PopupHeightProperty =
        AvaloniaProperty.RegisterAttached<PickerSizing, Control, double>("PopupHeight", 340, true);

    public static readonly AttachedProperty<double> ViewportHeightProperty =
        AvaloniaProperty.RegisterAttached<PickerSizing, Control, double>("ViewportHeight", 200, true);

    public static double GetViewportHeight(Control control)
    {
        return control.GetValue(ViewportHeightProperty);
    }

    public static void SetViewportHeight(Control control, double value)
    {
        control.SetValue(ViewportHeightProperty, value);
    }

    public static double GetItemHeight(Control control)
    {
        return control.GetValue(ItemHeightProperty);
    }

    public static void SetItemHeight(Control control, double value)
    {
        control.SetValue(ItemHeightProperty, value);
    }

    public static double GetPopupHeight(Control control)
    {
        return control.GetValue(PopupHeightProperty);
    }

    public static void SetPopupHeight(Control control, double value)
    {
        control.SetValue(PopupHeightProperty, value);
    }
}