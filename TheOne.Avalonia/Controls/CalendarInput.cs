using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Click-to-open calendar with native date parsing and an Escape path back to the editor.</summary>
public sealed class CalendarInput : AvaloniaObject
{
    public static readonly AttachedProperty<bool> OpenOnClickProperty =
        AvaloniaProperty.RegisterAttached<CalendarInput, CalendarDatePicker, bool>("OpenOnClick");
    private static readonly AttachedProperty<DateTime?> OpenedDateProperty = AvaloniaProperty.RegisterAttached<CalendarInput, CalendarDatePicker, DateTime?>("OpenedDate");
    private static readonly AttachedProperty<Control[]?> PartsProperty = AvaloniaProperty.RegisterAttached<CalendarInput, CalendarDatePicker, Control[]?>("Parts");
    static CalendarInput()
    {
        OpenOnClickProperty.Changed.AddClassHandler<CalendarDatePicker>((picker, change) =>
        {
            picker.RemoveHandler(InputElement.PointerReleasedEvent, OnReleased);
            picker.CalendarOpened -= OnOpened;
            picker.TemplateApplied -= OnTemplate;
            ClearParts(picker);
            if (change.GetNewValue<bool>())
            {
                picker.AddHandler(InputElement.PointerReleasedEvent, OnReleased, RoutingStrategies.Bubble, true);
                picker.CalendarOpened += OnOpened;
                picker.TemplateApplied += OnTemplate;
                HookParts(picker, picker.GetVisualDescendants().OfType<Control>().Where(part => part.TemplatedParent == picker && part.Name is "PART_TextBox" or "PART_Calendar").ToArray());
            }
        });
    }
    public static bool GetOpenOnClick(CalendarDatePicker picker) => picker.GetValue(OpenOnClickProperty);
    public static void SetOpenOnClick(CalendarDatePicker picker, bool value) => picker.SetValue(OpenOnClickProperty, value);
    private static void OnOpened(object? sender, EventArgs args)
    {
        if (sender is CalendarDatePicker picker)
        {
            picker.SetValue(OpenedDateProperty, picker.SelectedDate);
        }
    }
    private static void OnTemplate(object? sender, TemplateAppliedEventArgs args)
    {
        if (sender is CalendarDatePicker picker)
            HookParts(picker, new Control?[] { args.NameScope.Find<TextBox>("PART_TextBox"), args.NameScope.Find<Calendar>("PART_Calendar") }.OfType<Control>().ToArray());
    }
    private static void ClearParts(CalendarDatePicker picker)
    {
        foreach (var part in picker.GetValue(PartsProperty) ?? []) part.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        picker.SetValue(PartsProperty, null);
    }
    private static void HookParts(CalendarDatePicker picker, Control[] parts)
    {
        ClearParts(picker);
        picker.SetValue(PartsProperty, parts);
        foreach (var part in parts) part.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel, true);
    }
    private static void OnKeyDown(object? sender, KeyEventArgs args)
    {
        if (sender is not Control { TemplatedParent: CalendarDatePicker picker } || !picker.IsDropDownOpen) return;
        if (args.Key == Key.Escape)
        {
            args.Handled = true;
            picker.SetCurrentValue(CalendarDatePicker.SelectedDateProperty, picker.GetValue(OpenedDateProperty));
            picker.SetCurrentValue(CalendarDatePicker.IsDropDownOpenProperty, false);
            Dispatcher.UIThread.Post(() => picker.GetValue(PartsProperty)?.OfType<TextBox>().FirstOrDefault()?.Focus());
        }
        else if (args.Key == Key.Enter && sender is TextBox)
        {
            // Let the native TextBox handler commit/validate first.
            Dispatcher.UIThread.Post(() => picker.SetCurrentValue(CalendarDatePicker.IsDropDownOpenProperty, false));
        }
    }
    private static void OnReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (sender is not CalendarDatePicker picker || !picker.IsEffectivelyEnabled || args.InitialPressMouseButton != MouseButton.Left || args.Source is not Control source) return;
        var editor = source as TextBox ?? source.GetVisualAncestors().OfType<TextBox>().FirstOrDefault();
        if (editor?.Name != "PART_TextBox" || editor.TemplatedParent != picker) return;
        Dispatcher.UIThread.Post(() =>
        {
            if (picker.IsAttachedToVisualTree() && picker.IsEffectivelyEnabled && GetOpenOnClick(picker))
                picker.SetCurrentValue(CalendarDatePicker.IsDropDownOpenProperty, true);
        });
    }
}
