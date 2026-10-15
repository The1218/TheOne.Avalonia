using System.Collections;
using System.Collections.Specialized;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Data;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A single/multiple image viewer for HTTP(S), local paths and caller-owned Bitmap instances.</summary>
[PseudoClasses(":multiple", ":loading", ":error", ":has-image", ":compact", ":fullscreen")]
[TemplatePart("PART_Viewport", typeof(ImagePreviewSurface))]
public partial class ImagePreview : TemplatedControl
{
    public static readonly StyledProperty<object?> SourceProperty = AvaloniaProperty.Register<ImagePreview, object?>(nameof(Source), null);
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<ImagePreview, IEnumerable?>(nameof(ItemsSource), null);
    public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<ImagePreview, int>(nameof(SelectedIndex), 0, defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<ImagePreview, string?>(nameof(Title), null);
    public static readonly StyledProperty<bool> ShowHeaderProperty = AvaloniaProperty.Register<ImagePreview, bool>(nameof(ShowHeader), true);
    public static readonly StyledProperty<bool> ShowCloseButtonProperty = AvaloniaProperty.Register<ImagePreview, bool>(nameof(ShowCloseButton), false);
    public static readonly StyledProperty<bool> IsFitToWindowProperty = AvaloniaProperty.Register<ImagePreview, bool>(nameof(IsFitToWindow), true, defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<double> ZoomFactorProperty = AvaloniaProperty.Register<ImagePreview, double>(nameof(ZoomFactor), 1, defaultBindingMode: BindingMode.TwoWay, validate: value => double.IsFinite(value) && value is >= .001 and <= 64);
    public static readonly StyledProperty<double> ChromeHeightProperty = AvaloniaProperty.Register<ImagePreview, double>(nameof(ChromeHeight), 44, validate: value => double.IsFinite(value) && value > 0);
    public static readonly DirectProperty<ImagePreview, int> ImageCountProperty = AvaloniaProperty.RegisterDirect<ImagePreview, int>(nameof(ImageCount), control => control.ImageCount);
    public static readonly DirectProperty<ImagePreview, bool> IsLoadingProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(IsLoading), control => control.IsLoading);
    public static readonly DirectProperty<ImagePreview, bool> HasImageProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(HasImage), control => control.HasImage);
    public static readonly DirectProperty<ImagePreview, bool> HasErrorProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(HasError), control => control.HasError);
    public static readonly DirectProperty<ImagePreview, bool> IsMultipleProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(IsMultiple), control => control.IsMultiple);
    public static readonly DirectProperty<ImagePreview, bool> CanPreviousProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(CanPrevious), control => control.CanPrevious);
    public static readonly DirectProperty<ImagePreview, bool> CanNextProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(CanNext), control => control.CanNext);
    public static readonly DirectProperty<ImagePreview, bool> IsFullScreenProperty = AvaloniaProperty.RegisterDirect<ImagePreview, bool>(nameof(IsFullScreen), control => control.IsFullScreen);
    public static readonly DirectProperty<ImagePreview, string?> ErrorMessageProperty = AvaloniaProperty.RegisterDirect<ImagePreview, string?>(nameof(ErrorMessage), control => control.ErrorMessage);
    public static readonly DirectProperty<ImagePreview, string> DisplayTitleProperty = AvaloniaProperty.RegisterDirect<ImagePreview, string>(nameof(DisplayTitle), control => control.DisplayTitle);
    public static readonly DirectProperty<ImagePreview, string> PageTextProperty = AvaloniaProperty.RegisterDirect<ImagePreview, string>(nameof(PageText), control => control.PageText);
    public static readonly DirectProperty<ImagePreview, string> ZoomTextProperty = AvaloniaProperty.RegisterDirect<ImagePreview, string>(nameof(ZoomText), control => control.ZoomText);
    public static readonly RoutedEvent<RoutedEventArgs> CloseRequestedEvent = RoutedEvent.Register<ImagePreview, RoutedEventArgs>(nameof(CloseRequested), RoutingStrategies.Bubble);
    public object? Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
    public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    public string? Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public bool ShowHeader { get => GetValue(ShowHeaderProperty); set => SetValue(ShowHeaderProperty, value); }
    public bool ShowCloseButton { get => GetValue(ShowCloseButtonProperty); set => SetValue(ShowCloseButtonProperty, value); }
    public bool IsFitToWindow { get => GetValue(IsFitToWindowProperty); set => SetValue(IsFitToWindowProperty, value); }
    public double ZoomFactor { get => GetValue(ZoomFactorProperty); set => SetValue(ZoomFactorProperty, value); }
    public double ChromeHeight { get => GetValue(ChromeHeightProperty); set => SetValue(ChromeHeightProperty, value); }
    public int ImageCount { get; private set => SetAndRaise(ImageCountProperty, ref field, value); } = 0;
    public bool IsLoading { get; private set => SetAndRaise(IsLoadingProperty, ref field, value); } = false;
    public bool HasImage { get; private set => SetAndRaise(HasImageProperty, ref field, value); } = false;
    public bool HasError { get; private set => SetAndRaise(HasErrorProperty, ref field, value); } = false;
    public bool IsMultiple { get; private set => SetAndRaise(IsMultipleProperty, ref field, value); } = false;
    public bool CanPrevious { get; private set => SetAndRaise(CanPreviousProperty, ref field, value); } = false;
    public bool CanNext { get; private set => SetAndRaise(CanNextProperty, ref field, value); } = false;
    public bool IsFullScreen { get; private set => SetAndRaise(IsFullScreenProperty, ref field, value); } = false;
    public string? ErrorMessage { get; private set => SetAndRaise(ErrorMessageProperty, ref field, value); } = null;
    public string DisplayTitle { get; private set => SetAndRaise(DisplayTitleProperty, ref field, value); } = "图片预览";
    public string PageText { get; private set => SetAndRaise(PageTextProperty, ref field, value); } = "";
    public string ZoomText { get; private set => SetAndRaise(ZoomTextProperty, ref field, value); } = "适应窗口";
    public event EventHandler<RoutedEventArgs>? CloseRequested { add => AddHandler(CloseRequestedEvent, value); remove => RemoveHandler(CloseRequestedEvent, value); }
    /// <summary>The currently decoded image. Images created from paths are owned by the viewer.</summary>
    public IImage? CurrentImage => _image;
    private readonly List<object?> _items = [];
    private INotifyCollectionChanged? _collection;
    private ImagePreviewSurface? _surface;
    private readonly List<Button> _buttons = [];
    private bool _attached, _updating, _settingZoom;
    private object? _selectedSource;
    private IImage? _image;
    private Window? _fullScreenWindow;
    private ImagePreview? _fullScreenOrigin;

    protected override Type StyleKeyOverride => typeof(ImagePreview);
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (_surface is not null) { _surface.CancelPan(); _surface.Owner = null; }
        foreach (var button in _buttons) button.Click -= ButtonClick;
        _buttons.Clear();
        base.OnApplyTemplate(e);
        _surface = e.NameScope.Find<ImagePreviewSurface>("PART_Viewport");
        if (_surface is not null) { _surface.Owner = this; _surface.InvalidateVisual(); }
        foreach (var name in new[] { "PART_Previous", "PART_Next", "PART_ZoomOut", "PART_ZoomIn", "PART_Fit", "PART_Actual", "PART_FullScreen", "PART_Close", "PART_Retry" })
            if (e.NameScope.Find<Button>(name) is { } button) { _buttons.Add(button); button.Click += ButtonClick; }
        UpdateView();
    }
    private void ButtonClick(object? sender, RoutedEventArgs e)
    {
        switch ((sender as Control)?.Name)
        {
            case "PART_Previous": Previous(); break;
            case "PART_Next": Next(); break;
            case "PART_ZoomOut": ZoomOut(); break;
            case "PART_ZoomIn": ZoomIn(); break;
            case "PART_Fit": FitToWindow(); break;
            case "PART_Actual": ActualSize(); break;
            case "PART_FullScreen": ToggleFullScreen(); break;
            case "PART_Close": RequestClose(); break;
            case "PART_Retry": Reload(); break;
        }
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e); _attached = true; ObserveCollection(); RefreshItems();
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false; ObserveCollection(); CancelLoad(); ReleaseImage(); _surface?.CancelPan();
        _fullScreenWindow?.Close(); _selectedSource = null; IsLoading = false;
        base.OnDetachedFromVisualTree(e);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty || change.Property == SourceProperty)
        {
            ObserveCollection(); if (_attached) RefreshItems();
        }
        else if (change.Property == SelectedIndexProperty && !_updating && _attached) SelectCurrent();
        else if (change.Property == TitleProperty) UpdateView();
        else if (change.Property == ZoomFactorProperty || change.Property == IsFitToWindowProperty)
        {
            if (!_settingZoom)
            {
                if (change.Property == ZoomFactorProperty) SetCurrentValue(IsFitToWindowProperty, false);
                if (IsFitToWindow) { _surface?.ResetPan(); _surface?.UpdateFit(); }
                _surface?.ConstrainPan(); UpdateView();
            }
        }
        else if (change.Property == BoundsProperty)
        {
            PseudoClasses.Set(":compact", Bounds.Width < 380);
        }
        else if (change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled) _surface?.CancelPan();
    }
    private void ObserveCollection()
    {
        var next = _attached ? ItemsSource as INotifyCollectionChanged : null;
        if (ReferenceEquals(next, _collection)) return;
        if (_collection is not null) _collection.CollectionChanged -= CollectionChanged;
        _collection = next;
        if (_collection is not null) _collection.CollectionChanged += CollectionChanged;
    }
    private void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => RefreshItems(true);
    private void RefreshItems(bool preserve = false)
    {
        var previous = _selectedSource;
        _items.Clear();
        if (ItemsSource is { } items) { foreach (var item in items) _items.Add(item); }
        else if (Source is { } source) _items.Add(source);
        ImageCount = _items.Count;
        var index = SelectedIndex;
        if (preserve && previous is not null)
        {
            var found = _items.FindIndex(item => ReferenceEquals(item, previous));
            if (found >= 0) index = found;
        }
        _updating = true;
        try { SetCurrentValue(SelectedIndexProperty, _items.Count == 0 ? -1 : Math.Clamp(index, 0, _items.Count - 1)); }
        finally { _updating = false; }
        SelectCurrent();
    }
    private void SelectCurrent()
    {
        if (_updating) return;
        var index = _items.Count == 0 ? -1 : Math.Clamp(SelectedIndex, 0, _items.Count - 1);
        _updating = true;
        try { SetCurrentValue(SelectedIndexProperty, index); }
        finally { _updating = false; }
        var source = index < 0 ? null : _items[index];
        if (!ReferenceEquals(_selectedSource, source) || (_image is null && !IsLoading && !HasError))
        { _selectedSource = source; StartLoad(source); }
        UpdateView();
    }
    private void UpdateView()
    {
        HasImage = _image is not null; HasError = ErrorMessage is not null;
        IsMultiple = _items.Count > 1; CanPrevious = SelectedIndex > 0; CanNext = SelectedIndex >= 0 && SelectedIndex < _items.Count - 1;
        PageText = IsMultiple ? $"{SelectedIndex + 1} / {_items.Count}" : "";
        DisplayTitle = Title ?? SourceName(_selectedSource);
        ZoomText = IsFitToWindow ? "适应窗口" : ZoomFactor.ToString("P0");
        PseudoClasses.Set(":multiple", IsMultiple); PseudoClasses.Set(":loading", IsLoading);
        PseudoClasses.Set(":error", HasError); PseudoClasses.Set(":has-image", HasImage);
        _surface?.InvalidateVisual();
    }
    private static string SourceName(object? source)
    {
        var text = source switch { Uri { IsAbsoluteUri: true } uri => uri.IsFile ? uri.LocalPath : uri.AbsolutePath, Uri uri => uri.OriginalString, string value => value, _ => "" };
        if (Uri.TryCreate(text, UriKind.Absolute, out var address) && address.Scheme is "http" or "https") text = address.AbsolutePath;
        return string.IsNullOrWhiteSpace(text) ? "图片预览" : Path.GetFileName(text.Split('?')[0]);
    }
    public void Previous() { if (IsEffectivelyEnabled && CanPrevious) SetCurrentValue(SelectedIndexProperty, SelectedIndex - 1); }
    public void Next() { if (IsEffectivelyEnabled && CanNext) SetCurrentValue(SelectedIndexProperty, SelectedIndex + 1); }
    public void Reload() { if (_attached) StartLoad(_selectedSource); }
    public void FitToWindow()
    {
        SetCurrentValue(IsFitToWindowProperty, true); _surface?.ResetPan(); _surface?.UpdateFit(); UpdateView();
    }
    public void ActualSize() => ZoomAt(1, _surface?.Center ?? default);
    public void ZoomIn() => ZoomAt(ZoomFactor * 1.25, _surface?.Center ?? default);
    public void ZoomOut() => ZoomAt(ZoomFactor / 1.25, _surface?.Center ?? default);
    internal void SetFitZoom(double value)
    {
        _settingZoom = true;
        try { SetCurrentValue(ZoomFactorProperty, Math.Clamp(value, .001, 64)); }
        finally { _settingZoom = false; }
        UpdateView();
    }
    internal void ZoomAt(double factor, Point point)
    {
        if (!HasImage || !IsEffectivelyEnabled) return;
        var old = ZoomFactor; factor = Math.Clamp(factor, .001, 64);
        _settingZoom = true;
        try { SetCurrentValue(IsFitToWindowProperty, false); SetCurrentValue(ZoomFactorProperty, factor); }
        finally { _settingZoom = false; }
        _surface?.ZoomAround(point, old, factor); UpdateView();
    }
    private void RequestClose()
    {
        if (_fullScreenOrigin is not null) _fullScreenOrigin._fullScreenWindow?.Close();
        else RaiseEvent(new RoutedEventArgs(CloseRequestedEvent));
    }
    public void ToggleFullScreen()
    {
        if (!IsEffectivelyEnabled) return;
        if (_fullScreenOrigin is not null) { _fullScreenOrigin._fullScreenWindow?.Close(); return; }
        if (_fullScreenWindow is not null) { _fullScreenWindow.Close(); return; }
        if (TopLevel.GetTopLevel(this) is not Window owner) return;
        var preview = new ImagePreview { ShowCloseButton = true, _fullScreenOrigin = this };
        IDisposable[] bindings =
        [
            preview.Bind(SourceProperty, CompiledBinding.Create<ImagePreview, object?>(view => view.Source, source: this)),
            preview.Bind(ItemsSourceProperty, CompiledBinding.Create<ImagePreview, IEnumerable?>(view => view.ItemsSource, source: this)),
            preview.Bind(TitleProperty, CompiledBinding.Create<ImagePreview, string?>(view => view.Title, source: this)),
            preview.Bind(SelectedIndexProperty, CompiledBinding.Create<ImagePreview, int>(view => view.SelectedIndex, source: this, mode: BindingMode.TwoWay))
        ];
        Styling.ControlSizing.SetSize(preview, Styling.ControlSizing.GetSize(this));
        var window = new Window { Content = preview, WindowState = WindowState.FullScreen, WindowDecorations = WindowDecorations.None, Title = DisplayTitle, RequestedThemeVariant = ActualThemeVariant };
        _fullScreenWindow = window; IsFullScreen = true; preview.IsFullScreen = true;
        PseudoClasses.Set(":fullscreen", true); preview.PseudoClasses.Set(":fullscreen", true);
        window.Closed += (_, _) =>
        {
            foreach (var binding in bindings) binding.Dispose();
            _fullScreenWindow = null; IsFullScreen = false; PseudoClasses.Set(":fullscreen", false); Focus();
        };
        window.Show(owner); preview.Focus();
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled || !IsEffectivelyEnabled || e.KeyModifiers is not (KeyModifiers.None or KeyModifiers.Shift)) return;
        switch (e.Key)
        {
            case Key.Left: if (!IsMultiple) return; Previous(); break;
            case Key.Right: if (!IsMultiple) return; Next(); break;
            case Key.Add: case Key.OemPlus: ZoomIn(); break;
            case Key.Subtract: case Key.OemMinus: ZoomOut(); break;
            case Key.Escape: if (_fullScreenOrigin is null && !ShowCloseButton) return; RequestClose(); break;
            default: return;
        }
        e.Handled = true;
    }
}
