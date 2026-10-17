using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.GestureRecognizers;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>Enables mouse input on the native carousel swipe recognizer, independently of touch and pen.</summary>
public sealed class CarouselOptions : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsMouseSwipeEnabledProperty =
        AvaloniaProperty.RegisterAttached<CarouselOptions, Carousel, bool>("IsMouseSwipeEnabled");

    static CarouselOptions()
    {
        IsMouseSwipeEnabledProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        // These native properties recreate the recognizer; apply after the recreation.
        Carousel.IsSwipeEnabledProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Carousel.PageTransitionProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Carousel.ViewportFractionProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Carousel.WrapSelectionProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Carousel.TemplateProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Carousel.ItemsPanelProperty.Changed.AddClassHandler<Carousel>((carousel, _) => QueueApply(carousel));
        Control.LoadedEvent.AddClassHandler<Carousel>((carousel, _) => Apply(carousel));
    }

    public static bool GetIsMouseSwipeEnabled(Carousel carousel)
    {
        return carousel.GetValue(IsMouseSwipeEnabledProperty);
    }

    public static void SetIsMouseSwipeEnabled(Carousel carousel, bool value)
    {
        carousel.SetValue(IsMouseSwipeEnabledProperty, value);
    }

    private static void QueueApply(Carousel carousel)
    {
        Dispatcher.UIThread.Post(() => Apply(carousel), DispatcherPriority.Loaded);
    }

    private static void Apply(Carousel carousel)
    {
        if (carousel.ItemsPanelRoot is not VirtualizingCarouselPanel panel) return;
        foreach (var recognizer in panel.GestureRecognizers.OfType<SwipeGestureRecognizer>())
            recognizer.IsMouseEnabled = GetIsMouseSwipeEnabled(carousel);
    }
}