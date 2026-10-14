using System.Collections.Specialized;
using System.Diagnostics;
using global::Avalonia;
using global::Avalonia.Animation.Easings;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Data;
using global::Avalonia.Input;
using global::Avalonia.Media;
using global::Avalonia.Media.Imaging;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public sealed partial class TabFeatures
{
    // Preview the insertion gap without changing the consumer's collection on every pointer move.
    // Only realized headers participate; the independent drag image survives container recycling.
    private sealed class DragState(TabControl tabs)
    {
        private static readonly CubicEaseOut Easing = new();
        private readonly Dictionary<TabItem, HeaderVisual> _headers = [];
        private readonly Stopwatch _clock = new();
        private TabItem? _pressed;
        private IPointer? _pointer;
        private Point _start, _position;
        private Rect _origin, _viewport;
        private int _from, _to;
        private double _span;
        private bool _dragging, _settling, _committing;
        private ScrollViewer? _scroll;
        private AdornerLayer? _layer;
        private Canvas? _overlay;
        private Border? _preview;
        private TranslateTransform? _previewOffset;
        private RenderTargetBitmap? _bitmap;
        private DispatcherTimer? _timer;
        private Vector _settleFrom, _settleTo;
        private double _settleStarted;
        private bool Vertical => tabs.TabStripPlacement is Dock.Left or Dock.Right;
        private double Axis(Point value) => Vertical ? value.Y : value.X;
        private double Length(Size value) => Vertical ? value.Height : value.Width;
        private Vector Along(double value) => Vertical ? new Vector(0, value) : new Vector(value, 0);
        private double Duration => Motion.GetEnabled(tabs)
            ? (tabs.TryFindResource("One.Motion.Fast", out var value) && value is TimeSpan time ? Math.Max(0, time.TotalMilliseconds) : 120)
            : 0;

        public void Pressed(object? sender, PointerPressedEventArgs args)
        {
            if (!GetCanReorder(tabs) || MutableItems(tabs) is null || !args.GetCurrentPoint(tabs).Properties.IsLeftButtonPressed || args.Source is not Control source) return;
            var chain = source.GetVisualAncestors().Prepend(source).ToArray();
            if (chain.OfType<Button>().Any()) return;
            var item = chain.OfType<TabItem>().FirstOrDefault();
            if (item is null || Owner(item) != tabs || !item.IsEffectivelyEnabled) return;
            Clear();
            _pressed = item; _start = _position = args.GetPosition(tabs); _pointer = args.Pointer;
        }

        public void Moved(object? sender, PointerEventArgs args)
        {
            if (_pointer != args.Pointer || _settling) return;
            // The down/capture/up sequence owns this gesture; native/coalesced moves can omit button flags.
            _position = args.GetPosition(tabs);
            if (!_dragging)
            {
                if (_pressed is null || new Vector(_position.X - _start.X, _position.Y - _start.Y).Length < 6 || !Begin()) return;
                args.Pointer.Capture(tabs);
            }
            UpdatePreview();
            args.Handled = true;
        }

        private bool Begin()
        {
            if (_pressed is null || AdornerLayer.GetAdornerLayer(tabs) is not { } layer) return false;
            tabs.UpdateLayout();
            _from = _to = tabs.IndexFromContainer(_pressed);
            if (_from < 0 || NaturalBounds(_pressed) is not { } origin || origin.Width <= 0 || origin.Height <= 0) return false;
            _origin = origin;
            _span = Length(origin.Size) + (Vertical ? _pressed.Margin.Top + _pressed.Margin.Bottom : _pressed.Margin.Left + _pressed.Margin.Right);
            _scroll = _pressed.GetVisualAncestors().OfType<ScrollViewer>().FirstOrDefault();
            _viewport = _scroll is not null && _scroll.TranslatePoint(default, tabs) is { } start
                ? new Rect(start, _scroll.Bounds.Size) : origin;
            var scale = TopLevel.GetTopLevel(tabs)?.RenderScaling ?? 1;
            _bitmap = new RenderTargetBitmap(new PixelSize(Math.Max(1, (int)Math.Ceiling(origin.Width * scale)), Math.Max(1, (int)Math.Ceiling(origin.Height * scale))), new Vector(96 * scale, 96 * scale));
            _bitmap.Render(_pressed);
            _previewOffset = new TranslateTransform();
            _preview = new Border
            {
                Name = "PART_TabDragPreview", Width = origin.Width, Height = origin.Height,
                RenderTransform = _previewOffset, IsHitTestVisible = false,
                Child = new DragImage(_bitmap)
            };
            Canvas.SetLeft(_preview, origin.X); Canvas.SetTop(_preview, origin.Y);
            _overlay = new Canvas { Name = "PART_TabDragLayer", IsHitTestVisible = false, Clip = new RectangleGeometry(_viewport) };
            _overlay.Children.Add(_preview);
            AdornerLayer.SetAdornedElement(_overlay, tabs);
            _layer = layer; layer.Children.Add(_overlay);
            _dragging = true; _clock.Restart();
            _timer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(16) };
            _timer.Tick += Tick; _timer.Start();
            return true;
        }

        // TranslatePoint on the header itself includes its preview transform. Hit testing uses
        // layout coordinates instead, so a displaced neighbour cannot cause repeated swapping.
        private Rect? NaturalBounds(TabItem item) => item.GetVisualParent()?.TranslatePoint(item.Bounds.Position, tabs) is { } position ? new Rect(position, item.Bounds.Size) : null;
        private HeaderVisual Header(TabItem item)
        {
            if (!_headers.TryGetValue(item, out var visual)) _headers[item] = visual = new HeaderVisual(item);
            return visual;
        }
        public void Prepare(TabItem item)
        {
            if (!_dragging) return;
            var index = tabs.IndexFromContainer(item);
            var visual = Header(item);
            visual.Hide(index == _from);
            item.Classes.Set("dragging", index == _from);
            item.Classes.Set("drop-target", index == _to && _to != _from);
            var shift = index > _from && index <= _to ? -_span : index < _from && index >= _to ? _span : 0;
            visual.MoveTo(Along(shift), _clock.Elapsed.TotalMilliseconds, Duration);
        }
        public void Recycle(TabItem item)
        {
            if (_headers.Remove(item, out var visual)) visual.Dispose();
            if (_pressed == item) _pressed = null;
        }
        private void UpdatePreview()
        {
            if (!_dragging || _previewOffset is null) return;
            var offset = Axis(_position) - Axis(_start);
            var minimum = Axis(_viewport.Position) - Axis(_origin.Position);
            var maximum = minimum + Math.Max(0, Length(_viewport.Size) - Length(_origin.Size));
            offset = Math.Clamp(offset, minimum, maximum);
            var translation = Along(offset);
            _previewOffset.X = translation.X; _previewOffset.Y = translation.Y;
            // Insertion follows the pointer, independently of the clipped preview's width.
            // A selected header (including its close button) can be wider than the first slot.
            var insertion = Axis(_position);
            _to = _from;
            foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>())
            {
                var index = tabs.IndexFromContainer(item);
                if (index == _from || !item.IsEffectivelyEnabled || NaturalBounds(item) is not { } bounds) continue;
                if (index > _from && insertion > Axis(bounds.Center)) _to = Math.Max(_to, index);
                if (index < _from && insertion < Axis(bounds.Center)) _to = Math.Min(_to, index);
            }
            foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>()) Prepare(item);
        }

        private void Tick(object? sender, EventArgs args)
        {
            if (!tabs.IsAttachedToVisualTree() || !tabs.IsEffectivelyEnabled || !GetCanReorder(tabs)) { Clear(); return; }
            var now = _clock.Elapsed.TotalMilliseconds;
            if (_dragging)
            {
                ScrollAtEdge();
                UpdatePreview();
            }
            foreach (var visual in _headers.Values) visual.Tick(now, Duration);
            if (!_settling || _previewOffset is null) return;
            var progress = Duration <= 0 ? 1 : Math.Clamp((now - _settleStarted) / Duration, 0, 1);
            var offset = _settleFrom + (_settleTo - _settleFrom) * Easing.Ease(progress);
            _previewOffset.X = offset.X; _previewOffset.Y = offset.Y;
            if (progress >= 1) Clear();
        }

        private void ScrollAtEdge()
        {
            if (_scroll is null || !_viewport.Inflate(12).Contains(_position)) return;
            var position = Axis(_position) - Axis(_viewport.Position);
            var length = Length(_viewport.Size);
            var edge = Math.Min(40, length / 4);
            var speed = position < edge ? -Math.Clamp((edge - position) / edge, 0, 1) : position > length - edge ? Math.Clamp((position - length + edge) / edge, 0, 1) : 0;
            if (speed == 0) return;
            var offset = Vertical ? _scroll.Offset.Y : _scroll.Offset.X;
            var maximum = Math.Max(0, Length(_scroll.Extent) - Length(_scroll.Viewport));
            var next = Math.Clamp(offset + speed * 12, 0, maximum);
            if (Math.Abs(next - offset) < 0.01) return;
            _scroll.Offset = Along(next);
            tabs.UpdateLayout();
        }

        public void Released(object? sender, PointerReleasedEventArgs args)
        {
            if (_pointer != args.Pointer) return;
            if (!_dragging) { Clear(); return; }
            _position = args.GetPosition(tabs); UpdatePreview();
            args.Handled = true;
            if (!_viewport.Inflate(12).Contains(_position)) { Clear(); return; }
            var positions = tabs.GetRealizedContainers().OfType<TabItem>()
                .Select(item => (Index: tabs.IndexFromContainer(item), Bounds: NaturalBounds(item), Offset: _headers.TryGetValue(item, out var visual) ? visual.Offset : default))
                .Where(entry => entry.Bounds.HasValue).ToArray();
            _dragging = false; _settling = true; _committing = true;
            try
            {
                ResetHeaders();
                if (_from != _to && !Move(tabs, _from, _to)) { Clear(); return; }
                tabs.ScrollIntoView(_to); tabs.UpdateLayout();
                if (tabs.ContainerFromIndex(_to) is not TabItem target || NaturalBounds(target) is not { } destination || _previewOffset is null) { Clear(); return; }
                Header(target).Hide(true);
                // FLIP only the realized headers: keep their screen position across the single
                // collection mutation, then finish the small remaining distance with easing.
                foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>())
                {
                    var index = tabs.IndexFromContainer(item);
                    if (index == _to || NaturalBounds(item) is not { } bounds) continue;
                    var previous = index;
                    if (_to > _from && index >= _from && index < _to) previous++;
                    if (_to < _from && index > _to && index <= _from) previous--;
                    var old = positions.FirstOrDefault(entry => entry.Index == previous);
                    if (old.Bounds is not { } oldBounds) continue;
                    var visual = Header(item);
                    visual.MoveTo(oldBounds.Position + old.Offset - bounds.Position, _clock.Elapsed.TotalMilliseconds, 0);
                    visual.MoveTo(default, _clock.Elapsed.TotalMilliseconds, Duration);
                }
                _settleFrom = new Vector(_previewOffset.X, _previewOffset.Y);
                _settleTo = destination.Position - _origin.Position;
                _settleStarted = _clock.Elapsed.TotalMilliseconds;
                _preview!.Width = destination.Width; _preview.Height = destination.Height;
                if (Duration <= 0) Clear();
            }
            finally { _committing = false; ReleasePointer(); }
        }

        public void KeyDown(object? sender, KeyEventArgs args)
        {
            if (args.Key == Key.Escape && (_dragging || _pressed is not null)) { Clear(); args.Handled = true; return; }
            if ((args.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) == 0) return;
            if (args.Key == Key.W && tabs.ContainerFromIndex(tabs.SelectedIndex) is TabItem item) args.Handled = Close(item);
            if ((args.KeyModifiers & KeyModifiers.Shift) != 0 && args.Key is Key.Left or Key.Right)
                args.Handled = Move(tabs, tabs.SelectedIndex, tabs.SelectedIndex + (args.Key == Key.Left ? -1 : 1));
        }
        public void CaptureLost(object? sender, PointerCaptureLostEventArgs args) { if (ReferenceEquals(args.Source, tabs) && !_settling) Clear(); }
        public void ItemsChanged(object? sender, NotifyCollectionChangedEventArgs args) { if (!_committing) Clear(); }
        public void OwnerChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
        {
            if (args.Property == Motion.EnabledProperty && !Motion.GetEnabled(tabs))
            {
                if (_settling) Clear();
                else foreach (var visual in _headers.Values) visual.Tick(_clock.Elapsed.TotalMilliseconds, 0);
            }
            if (args.Property == TabControl.TabStripPlacementProperty || args.Property == ItemsControl.ItemsSourceProperty || (args.Property == InputElement.IsEnabledProperty && !tabs.IsEffectivelyEnabled)
                || (args.Property == Visual.BoundsProperty && args.GetOldValue<Rect>().Size != args.GetNewValue<Rect>().Size)) Clear();
        }
        private void ReleasePointer()
        {
            var pointer = _pointer; _pointer = null;
            if (pointer?.Captured == tabs) pointer.Capture(null);
        }
        private void ResetHeaders() { foreach (var visual in _headers.Values) visual.Dispose(); _headers.Clear(); }
        public void Clear()
        {
            _dragging = _settling = false;
            _timer?.Stop(); if (_timer is not null) _timer.Tick -= Tick; _timer = null;
            ResetHeaders(); _pressed = null;
            if (_overlay is not null) { _layer?.Children.Remove(_overlay); AdornerLayer.SetAdornedElement(_overlay, null); }
            _overlay = null; _layer = null; _preview = null; _previewOffset = null;
            _bitmap?.Dispose(); _bitmap = null; _scroll = null; _clock.Stop();
            ReleasePointer();
        }

        private sealed class HeaderVisual : IDisposable
        {
            private readonly TabItem _item;
            private readonly TranslateTransform _translation = new();
            private readonly IDisposable? _transform;
            private IDisposable? _hidden;
            private Vector _from, _target;
            private double _started;
            public Vector Offset => new(_translation.X, _translation.Y);
            public HeaderVisual(TabItem item)
            {
                _item = item;
                var group = new TransformGroup();
                if (item.RenderTransform is { } existing) group.Children.Add(existing as Transform ?? new MatrixTransform(existing.Value));
                group.Children.Add(_translation);
                _transform = item.SetValue(Visual.RenderTransformProperty, group, BindingPriority.Animation);
            }
            public void Hide(bool hide)
            {
                if (hide) _hidden ??= _item.SetValue(Visual.OpacityProperty, 0d, BindingPriority.Animation);
                else { _hidden?.Dispose(); _hidden = null; }
            }
            public void MoveTo(Vector target, double now, double duration)
            {
                if (target == _target && duration > 0) return;
                Tick(now, duration);
                _from = Offset; _target = target; _started = now;
                if (duration <= 0) Tick(now, 0);
            }
            public void Tick(double now, double duration)
            {
                var progress = duration <= 0 ? 1 : Math.Clamp((now - _started) / duration, 0, 1);
                var offset = _from + (_target - _from) * Easing.Ease(progress);
                _translation.X = offset.X; _translation.Y = offset.Y;
            }
            public void Dispose() { _hidden?.Dispose(); _transform?.Dispose(); _item.Classes.Remove("dragging"); _item.Classes.Remove("drop-target"); }
        }

        private sealed class DragImage(RenderTargetBitmap bitmap) : Control
        {
            protected override Size MeasureOverride(Size availableSize) => bitmap.Size;
            public override void Render(DrawingContext context)
            {
                base.Render(context);
                // Bitmap source rectangles are pixels, not DIPs. Using bitmap.Size here
                // crops a high-DPI render target and enlarges the remaining top-left area.
                context.DrawImage(bitmap, new Rect(0, 0, bitmap.PixelSize.Width, bitmap.PixelSize.Height), new Rect(Bounds.Size));
            }
        }
    }
}
