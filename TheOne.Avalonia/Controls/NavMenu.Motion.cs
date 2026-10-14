using global::Avalonia;
using global::Avalonia.Animation;
using global::Avalonia.Animation.Easings;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Presenters;
using global::Avalonia.Media;
using global::Avalonia.Styling;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

public partial class NavMenu
{
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty = AvaloniaProperty.Register<NavMenu, TimeSpan>(nameof(TransitionDuration), TimeSpan.FromMilliseconds(200), validate: value => value >= TimeSpan.Zero);
    public static readonly DirectProperty<NavMenu, double> TransitionExtentProperty = AvaloniaProperty.RegisterDirect<NavMenu, double>(nameof(TransitionExtent), menu => menu.TransitionExtent);
    private static readonly StyledProperty<double> CompactMotionProgressProperty = AvaloniaProperty.Register<NavMenu, double>("CompactMotionProgress", 1d);
    public TimeSpan TransitionDuration { get => GetValue(TransitionDurationProperty); set => SetValue(TransitionDurationProperty, value); }
    public double TransitionExtent => _transitionExtent;
    private double _transitionExtent = double.PositiveInfinity;
    private CancellationTokenSource? _compactMotion;
    private readonly List<CompactVisual> _compactVisuals = [];
    private int _compactRevision;

    private sealed class CompactVisual(NavMenuItem row, object? item, ContentPresenter icon, ContentPresenter label, Point? position)
    {
        public NavMenuItem Row { get; } = row;
        public object? Item { get; } = item;
        public ContentPresenter Icon { get; } = icon;
        public ContentPresenter Label { get; } = label;
        public Point? Position { get; } = position;
        public double IconStart { get; } = icon.IsVisible ? icon.Opacity : 0;
        public double LabelStart { get; } = label.IsVisible ? label.Opacity : 0;
        public ITransform? OriginalTransform { get; set; }
        public TranslateTransform Offset { get; } = new();
        public double DeltaX { get; set; }
        public bool Prepared { get; set; }
        public void Restore()
        {
            if (!Prepared) return;
            Icon.SetCurrentValue(RenderTransformProperty, OriginalTransform);
            Icon.SetCurrentValue(OpacityProperty, 1d); Label.SetCurrentValue(OpacityProperty, 1d);
            Prepared = false;
        }
    }

    private void CompactMotionPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == BoundsProperty)
            SetAndRaise(TransitionExtentProperty, ref _transitionExtent, Bounds.Height > 0 && double.IsFinite(Bounds.Height) ? Bounds.Height : double.PositiveInfinity);
        if (change.Property == CollapsedWidthProperty || change.Property == ItemHeightProperty || change.Property == IconSizeProperty || change.Property == FontSizeProperty) CancelCompactMotion();
        if (change.Property == CompactMotionProgressProperty) UpdateCompactVisuals();
        if (change.Property == Motion.EnabledProperty && !Motion.GetEnabled(this) || change.Property == TransitionDurationProperty && TransitionDuration == TimeSpan.Zero)
            CancelCompactMotion();
    }
    private void BeginCompactMotion()
    {
        var visuals = new List<CompactVisual>();
        if (this.IsAttachedToVisualTree() && Motion.GetEnabled(this) && TransitionDuration > TimeSpan.Zero && _popupParent is null)
            foreach (var row in GetRealizedContainers().OfType<NavMenuItem>())
            {
                var parts = row.GetVisualDescendants().OfType<ContentPresenter>();
                if (parts.FirstOrDefault(control => control.Name == "NavMenuIcon") is not { } icon || parts.FirstOrDefault(control => control.Name == "NavMenuContent") is not { } label) continue;
                var position = icon.IsVisible ? icon.TranslatePoint(default, this) : null;
                // Only the visible viewport participates; recycled off-screen rows never replay entrance motion.
                if (row.TranslatePoint(default, this) is not { } rowPosition || rowPosition.Y < -row.MinHeight || rowPosition.Y > Bounds.Height) continue;
                visuals.Add(new(row, ItemFromContainer(row), icon, label, position));
            }
        // Capture rendered positions before cancelling, so reversal continues from the current pose.
        CancelCompactMotion();
        if (visuals.Count == 0) return;
        _compactVisuals.AddRange(visuals);
        var revision = _compactRevision;
        Dispatcher.UIThread.Post(() => StartCompactMotion(revision), DispatcherPriority.Loaded);
    }
    private async void StartCompactMotion(int revision)
    {
        if (revision != _compactRevision || !this.IsAttachedToVisualTree() || !Motion.GetEnabled(this)) return;
        UpdateLayout();
        _compactVisuals.RemoveAll(visual => !ReferenceEquals(ItemFromContainer(visual.Row), visual.Item) || !visual.Icon.IsAttachedToVisualTree());
        foreach (var visual in _compactVisuals)
        {
            visual.OriginalTransform = visual.Icon.RenderTransform;
            if (visual.Position is { } old && visual.Icon.TranslatePoint(default, this) is { } current) visual.DeltaX = Math.Clamp(old.X, 0, Math.Max(0, Bounds.Width - visual.Icon.Bounds.Width)) - current.X;
            var group = new TransformGroup();
            if (visual.OriginalTransform is { } original) group.Children.Add(original as Transform ?? new MatrixTransform(original.Value));
            group.Children.Add(visual.Offset);
            visual.Icon.SetCurrentValue(RenderTransformProperty, group);
            visual.Prepared = true;
        }
        if (_compactVisuals.Count == 0) return;
        SetValue(CompactMotionProgressProperty, 0d); UpdateCompactVisuals();
        var cts = _compactMotion = new CancellationTokenSource();
        var animation = new Animation
        {
            Duration = IsCollapsed ? TimeSpan.FromTicks(TransitionDuration.Ticks * 7 / 10) : TransitionDuration,
            Easing = new CubicEaseOut(), FillMode = FillMode.Both,
            Children =
            {
                new KeyFrame { Cue = new Cue(0), Setters = { new Setter(CompactMotionProgressProperty, 0d) } },
                new KeyFrame { Cue = new Cue(1), Setters = { new Setter(CompactMotionProgressProperty, 1d) } }
            }
        };
        try { await animation.RunAsync(this, cts.Token); }
        finally { if (ReferenceEquals(_compactMotion, cts)) CancelCompactMotion(); }
    }
    private void UpdateCompactVisuals()
    {
        var progress = GetValue(CompactMotionProgressProperty);
        foreach (var visual in _compactVisuals.Where(visual => visual.Prepared))
        {
            visual.Offset.X = visual.DeltaX * (1 - progress);
            visual.Icon.SetCurrentValue(OpacityProperty, visual.IconStart + (1 - visual.IconStart) * progress);
            visual.Label.SetCurrentValue(OpacityProperty, visual.LabelStart + (1 - visual.LabelStart) * progress);
        }
    }
    private void RemoveCompactMotion(NavMenuItem row)
    {
        foreach (var visual in _compactVisuals.Where(visual => visual.Row == row).ToArray()) { visual.Restore(); _compactVisuals.Remove(visual); }
    }
    private void CancelCompactMotion()
    {
        _compactRevision++;
        var previous = _compactMotion; _compactMotion = null;
        previous?.Cancel(); previous?.Dispose();
        foreach (var visual in _compactVisuals) visual.Restore();
        _compactVisuals.Clear(); SetValue(CompactMotionProgressProperty, 1d);
    }
}
