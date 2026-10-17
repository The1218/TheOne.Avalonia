using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using TheOne.Avalonia.Controls;

namespace TheOne.Avalonia.Styling;

/// <summary>Contains unconsumed wheel and gesture input at native popup boundaries.</summary>
internal static class PopupScrollBoundary
{
    static PopupScrollBoundary()
    {
        Popup.ChildProperty.Changed.AddClassHandler<Popup>((popup, change) =>
        {
            if (change.GetOldValue<Control?>() is { } previous) Detach(previous);
            Update(popup);
        });
        Popup.IsOpenProperty.Changed.AddClassHandler<Popup>((popup, _) => Update(popup));
        // Menu templates may be created after IsSubMenuOpen is already true.
        StyledElement.TemplatedParentProperty.Changed.AddClassHandler<Popup>((popup, _) => Update(popup));
    }

    private static void Update(Popup popup)
    {
        if (popup.Child is not { } child) return;
        Detach(child);
        // Keep the cached child's boundary while closed: queued inertia/end events
        // can still originate there. Replacing the child releases every handler.
        if (popup.TemplatedParent is AutoCompleteBox or ComboBox or CalendarDatePicker or DatePicker or TimePicker or MenuItem or Menu or NavMenu)
        {
            child.AddHandler(InputElement.PointerWheelChangedEvent, ContainWheel, RoutingStrategies.Bubble);
            child.AddHandler(InputElement.ScrollGestureEvent, ContainGesture, RoutingStrategies.Bubble);
            child.AddHandler(InputElement.ScrollGestureInertiaStartingEvent, ContainPhase, RoutingStrategies.Bubble);
            child.AddHandler(InputElement.ScrollGestureEndedEvent, ContainPhase, RoutingStrategies.Bubble);
        }
    }

    private static void Detach(Control child)
    {
        child.RemoveHandler(InputElement.PointerWheelChangedEvent, ContainWheel);
        child.RemoveHandler(InputElement.ScrollGestureEvent, ContainGesture);
        child.RemoveHandler(InputElement.ScrollGestureInertiaStartingEvent, ContainPhase);
        child.RemoveHandler(InputElement.ScrollGestureEndedEvent, ContainPhase);
    }

    private static void ContainWheel(object? sender, PointerWheelEventArgs args)
    {
        // Descendant lists and picker columns handle native scrolling first.
        // Short lists, exhausted ranges and popup chrome stop before the owner.
        args.Handled = true;
    }

    private static void ContainGesture(object? sender, ScrollGestureEventArgs args)
    {
        // Only unhandled deltas reach this fallback. Let native descendants keep
        // scrolling and finish their inertia/snap state before stopping at the edge.
        args.Handled = true;
        args.ShouldEndScrollGesture = true;
    }

    private static void ContainPhase(object? sender, RoutedEventArgs args)
    {
        args.Handled = true;
    }

    internal static void Initialize()
    {
    }
}
