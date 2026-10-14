using System.Runtime.CompilerServices;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Media;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Styling;

/// <summary>Finishes fractional wheel offsets without replacing native picker selection or recycling.</summary>
public sealed class PickerScroll : AvaloniaObject
{
    public static readonly AttachedProperty<bool> EnabledProperty = AvaloniaProperty.RegisterAttached<PickerScroll, DateTimePickerPanel, bool>("Enabled");
    public static readonly AttachedProperty<Geometry> HighlightClipProperty = AvaloniaProperty.RegisterAttached<PickerScroll, ListBoxItem, Geometry>("HighlightClip", new RectangleGeometry(new Rect()));
    private static readonly ConditionalWeakTable<DateTimePickerPanel, State> States = new();
    private static readonly ConditionalWeakTable<ScrollContentPresenter, State> Sources = new();
    public static bool GetEnabled(DateTimePickerPanel panel) => panel.GetValue(EnabledProperty);
    public static void SetEnabled(DateTimePickerPanel panel, bool value) => panel.SetValue(EnabledProperty, value);
    public static Geometry GetHighlightClip(ListBoxItem item) => item.GetValue(HighlightClipProperty);
    public static void SetHighlightClip(ListBoxItem item, Geometry value) => item.SetValue(HighlightClipProperty, value);
    static PickerScroll()
    {
        // Gesture events bubble only. Class handlers run before the native panel's
        // instance completion handler, including clamping short-list overscroll.
        InputElement.ScrollGestureEvent.AddClassHandler<ScrollContentPresenter>((source, args) => { if (Sources.TryGetValue(source, out var state)) state.Gesture(source, args); }, handledEventsToo: true);
        InputElement.ScrollGestureEndedEvent.AddClassHandler<ScrollContentPresenter>((source, args) => { if (Sources.TryGetValue(source, out var state)) state.GestureEnded(source, args); }, handledEventsToo: true);
        EnabledProperty.Changed.AddClassHandler<DateTimePickerPanel>((panel, change) =>
        {
            if (change.GetNewValue<bool>()) States.GetValue(panel, control => new State(control)).Enable();
            else if (States.TryGetValue(panel, out var state)) { state.Disable(); States.Remove(panel); }
        });
    }
    private static readonly ConditionalWeakTable<PickerPresenterBase, RestoreWork> Restores = new();
    private sealed class RestoreWork(PickerPresenterBase presenter)
    {
        private bool _queued;
        public void Queue()
        {
            if (_queued) return;
            _queued = true;
            Dispatcher.UIThread.Post(() =>
            {
                _queued = false;
                if (!presenter.IsAttachedToVisualTree()) return;
                var panels = presenter.GetVisualDescendants().OfType<DateTimePickerPanel>().ToArray();
                foreach (var panel in panels) if (States.TryGetValue(panel, out var state)) { state._idle.Stop(); state._snapping = true; }
                try
                {
                    // Assigning the same Date/Time does not reinitialize a cached native presenter.
                    // Restore in dependency order so leap-year and month day bounds are correct.
                    if (presenter is DatePickerPresenter date)
                    {
                        Set(DateTimePickerPanelType.Year, date.Date.Year);
                        Set(DateTimePickerPanelType.Month, date.Date.Month);
                        Set(DateTimePickerPanelType.Day, date.Date.Day);
                    }
                    else if (presenter is TimePickerPresenter time)
                    {
                        var hour = time.Time.Hours;
                        if (time.ClockIdentifier == "12HourClock") hour = hour == 0 ? 12 : hour > 12 ? hour - 12 : hour;
                        Set(DateTimePickerPanelType.Hour, hour); Set(DateTimePickerPanelType.Minute, time.Time.Minutes);
                        Set(DateTimePickerPanelType.Second, time.Time.Seconds); Set(DateTimePickerPanelType.TimePeriod, time.Time.Hours >= 12 ? 1 : 0);
                    }
                }
                finally { foreach (var panel in panels) if (States.TryGetValue(panel, out var state)) state._snapping = false; }
                void Set(DateTimePickerPanelType kind, int value)
                {
                    var panel = panels.FirstOrDefault(control => control.PanelType == kind);
                    if (panel is not null && value >= panel.MinimumValue && value <= panel.MaximumValue) panel.SelectedValue = value;
                }
            }, DispatcherPriority.Loaded);
        }
    }
    private sealed class State(DateTimePickerPanel panel)
    {
        internal readonly DispatcherTimer _idle = new() { Interval = TimeSpan.FromMilliseconds(140) };
        private bool _enabled, _attached, _gesture;
        internal bool _snapping;
        private ScrollContentPresenter? _source;
        private PickerPresenterBase? _presenter;
        public void Enable()
        {
            if (_enabled) return;
            _enabled = true; _idle.Tick += Idle;
            panel.AttachedToVisualTree += Attached; panel.DetachedFromVisualTree += Detached;
            if (panel.IsAttachedToVisualTree()) Attach();
        }
        public void Disable()
        {
            Detach(); _idle.Tick -= Idle;
            panel.AttachedToVisualTree -= Attached; panel.DetachedFromVisualTree -= Detached; _enabled = false;
        }
        private void Attached(object? sender, VisualTreeAttachmentEventArgs args) => Attach();
        private void Detached(object? sender, VisualTreeAttachmentEventArgs args) => Detach();
        private void Attach()
        {
            if (_attached) return;
            _attached = true;
            panel.SelectionChanged += Activity; panel.LayoutUpdated += Layout;
            _source = panel.GetVisualParent() as ScrollContentPresenter;
            if (_source is not null) { Sources.Remove(_source); Sources.Add(_source, this); }
            _presenter = panel.GetVisualAncestors().OfType<PickerPresenterBase>().FirstOrDefault();
            _presenter?.AddHandler(InputElement.PointerPressedEvent, BeforeConfirm, RoutingStrategies.Tunnel, true);
            _presenter?.AddHandler(InputElement.KeyDownEvent, BeforeKey, RoutingStrategies.Tunnel, true);
            if (_presenter is not null) Restores.GetValue(_presenter, control => new RestoreWork(control)).Queue();
            RefreshClips();
        }
        private void Detach()
        {
            _idle.Stop(); _gesture = false;
            if (!_attached) return;
            panel.SelectionChanged -= Activity; panel.LayoutUpdated -= Layout;
            if (_source is not null && Sources.TryGetValue(_source, out var state) && ReferenceEquals(state, this)) Sources.Remove(_source);
            _presenter?.RemoveHandler(InputElement.PointerPressedEvent, BeforeConfirm);
            _presenter?.RemoveHandler(InputElement.KeyDownEvent, BeforeKey);
            _source = null; _presenter = null; _attached = false;
        }
        private void Activity(object? sender, EventArgs args)
        {
            if (_snapping) return;
            _idle.Stop();
            if (!_gesture) _idle.Start();
        }
        internal void Gesture(object? sender, ScrollGestureEventArgs args) { _gesture = true; _idle.Stop(); }
        internal void GestureEnded(object? sender, ScrollGestureEndedEventArgs args) { _gesture = false; Snap(); }
        private void Idle(object? sender, EventArgs args) { _idle.Stop(); Snap(); }
        private void BeforeKey(object? sender, KeyEventArgs args) { if (args.Key == Key.Enter) Snap(); }
        private void BeforeConfirm(object? sender, PointerPressedEventArgs args)
        {
            if (args.Source is Visual source && args.GetCurrentPoint(panel).Properties.IsLeftButtonPressed &&
                source.GetVisualAncestors().Prepend(source).Any(visual => visual is Button { Name: "PART_AcceptButton" })) Snap();
        }
        private void Snap()
        {
            _idle.Stop();
            if (!_attached || !double.IsFinite(panel.ItemHeight) || panel.ItemHeight <= 0 || panel.Children.Count == 0) return;
            var y = Math.Round(panel.Offset.Y / panel.ItemHeight, MidpointRounding.AwayFromZero) * panel.ItemHeight;
            if (!panel.ShouldLoop)
            {
                var last = Math.Max(0, Math.Ceiling((panel.MaximumValue - panel.MinimumValue + 1d) / panel.Increment) - 1);
                y = Math.Clamp(y, 0, last * panel.ItemHeight);
            }
            if (Math.Abs(y - panel.Offset.Y) < 0.001) return;
            _snapping = true;
            try { panel.Offset = panel.Offset.WithY(y); }
            finally { _snapping = false; }
        }
        private void Layout(object? sender, EventArgs args) => RefreshClips();
        private void RefreshClips()
        {
            // Paint the accent text only where each recycled row intersects the fixed selection band.
            // Native IsSelected changes by whole indexes, while trackpads move by fractional pixels.
            var top = (panel.Bounds.Height - panel.ItemHeight) / 2;
            var bottom = top + panel.ItemHeight;
            foreach (var item in panel.Children.OfType<ListBoxItem>())
            {
                var start = Math.Clamp(top - item.Bounds.Y, 0, item.Bounds.Height);
                var end = Math.Clamp(bottom - item.Bounds.Y, 0, item.Bounds.Height);
                var rect = new Rect(0, start, item.Bounds.Width, Math.Max(0, end - start));
                if (item.GetValue(HighlightClipProperty) is RectangleGeometry previous && previous.Rect == rect) continue;
                item.SetValue(HighlightClipProperty, new RectangleGeometry(rect));
            }
        }
    }
}
