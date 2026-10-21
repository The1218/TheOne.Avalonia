using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ControlGallery : UserControl
{
    private readonly Dictionary<int, UserControl> _pages = [];
    private readonly Dictionary<int, Control> _presentations = [];
    private GalleryViewModel? _model;
    private int _scrollRequest;
    private readonly Dictionary<Control, PreviewPageFocus> _focus = [];
    private readonly Dictionary<Control, Vector> _focusOffsets = [];
    private readonly HashSet<Popup> _openPopups = [];
    private IDisposable? _popupSubscription;

    public ControlGallery()
    {
        InitializeComponent();
        DataContextChanged += (_, _) => ObserveModel();
        DetachedFromVisualTree += (_, _) =>
        {
            ClosePopups();
            _popupSubscription?.Dispose();
            _popupSubscription = null;
            Unsubscribe();
        };
        AttachedToVisualTree += (_, _) =>
        {
            _popupSubscription?.Dispose();
            _popupSubscription = Popup.IsOpenProperty.Changed.AddClassHandler<Popup>(PopupChanged);
            ObserveModel();
        };
        GotFocus += (_, args) =>
        {
            if (PageHost.Content is { } page && args.NewFocusedElement is InputElement target && Contains(page, target))
            {
                if (_focus.TryGetValue(page, out var prior))
                {
                    if (prior.Matches(target)) return;
                    prior.Dispose();
                }
                var focus = new PreviewPageFocus(target);
                _focus[page] = focus;
                focus.Track();
            }
        };
        SizeChanged += (_, _) =>
        {
            UpdateViewport();
        };
        PageScroll.PropertyChanged += (_, args) =>
        {
            if (args.Property == ScrollViewer.ViewportProperty) UpdateViewport();
        };
    }

    public UserControl? CurrentView { get; private set; }

    public int CreatedPageCount => _pages.Count;

    private void Unsubscribe()
    {
        if (_model is not null)
        {
            _model.PropertyChanged -= ModelChanged;
            _model.ControlRequested -= ControlRequested;
        }
    }

    private void ObserveModel()
    {
        Unsubscribe();
        _model = DataContext as GalleryViewModel;
        if (_model is not null)
        {
            _model.PropertyChanged += ModelChanged;
            _model.ControlRequested += ControlRequested;
        }

        ShowPage();
    }

    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(GalleryViewModel.CurrentPage)) ShowPage();
    }

    private void ShowPage()
    {
        if (_model?.CurrentPage is not { } descriptor) return;
        if (!_pages.TryGetValue(descriptor.Id, out var page))
        {
            _pages.Add(descriptor.Id, page = PreviewPageCatalog.Create(descriptor.Id));
            if (page is ShowcasePage showcase) showcase.ControlRequested += ControlRequested;
            if (page is HomePage home) home.ControlRequested += (_, id) => _model?.Select(id);
            _presentations.Add(descriptor.Id,
                page is HomePage or ShowcasePage ? page : new ControlPreviewPage(descriptor, page));
        }

        // Editors own a bounded viewport; do not measure their canvas at infinite height.
        var presentation = _presentations[descriptor.Id];
        if (ReferenceEquals(PageHost.Content, presentation)) return;
        var isReportDesigner = page is ReportDesign or ReportPreview;
        var fillsViewport = presentation is ControlPreviewPage { UsesViewport: true };
        PageScroll.VerticalScrollBarVisibility =
            fillsViewport ? ScrollBarVisibility.Disabled : ScrollBarVisibility.Auto;
        PageHost.VerticalAlignment = fillsViewport ? VerticalAlignment.Stretch : VerticalAlignment.Top;
        PageFrame.Padding = new Thickness(isReportDesigner ? 0 : 28);
        CurrentView = page;
        var focused = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement() as InputElement;
        var leavingPageFocus = PageHost.Content is { } previous && Contains(previous, focused);
        if (PageHost.Content is { } current && _focus.TryGetValue(current, out var currentFocus)) currentFocus.Capture();
        if (PageHost.Content is { } leaving) _focusOffsets[leaving] = PageScroll.Offset;
        ClosePopups();
        PageHost.Content = presentation;
        ResetScroll();
        UpdateViewport();
        // Resume only a content-initiated navigation. Menu/search focus always stays with its caller.
        if (leavingPageFocus && _focus.TryGetValue(presentation, out var saved))
        {
            ++_scrollRequest; // Cancel the queued top reset before resuming an editing position.
            if (_focusOffsets.TryGetValue(presentation, out var offset)) PageScroll.Offset = offset;
            saved.Resume(presentation);
        }
        else if (_focus.TryGetValue(presentation, out var inactiveFocus)) inactiveFocus.RestoreSelection(presentation);
    }

    private static bool Contains(Control page, InputElement? element) =>
        element is not null && (ReferenceEquals(page, element) || element.GetVisualAncestors().Contains(page));

    private void PopupChanged(Popup popup, AvaloniaPropertyChangedEventArgs args)
    {
        if (!args.GetNewValue<bool>()) _openPopups.Remove(popup);
        else if (PageHost.Content is { } page && (Contains(page, popup) || Contains(page, popup.PlacementTarget)))
            _openPopups.Add(popup);
    }

    private void ClosePopups()
    {
        foreach (var popup in _openPopups.ToArray()) popup.SetCurrentValue(Popup.IsOpenProperty, false);
        _openPopups.Clear();
    }

    private void UpdateViewport()
    {
        if (PageHost.Content is ControlPreviewPage page) page.SetViewportHeight(PageScroll.Viewport.Height);
    }

    private void ControlRequested(object? sender, int id)
    {
        ScrollToControl(id);
    }

    public UserControl GetControlView(int id)
    {
        return CurrentView is ShowcasePage showcase ? showcase.GetControlView(id) : CurrentView!;
    }

    public void ScrollToControl(int id)
    {
        ScheduleScroll(id);
    }

    private void ScheduleScroll(int? id)
    {
        var request = ++_scrollRequest;

        void Apply()
        {
            if (request != _scrollRequest) return;
            if (id is { } controlId && CurrentView is ShowcasePage showcase)
            {
                // A section destination needs measured coordinates; resetting to the top does not.
                UpdateLayout();
                if (showcase.GetSection(controlId).TranslatePoint(default, PageHost) is { } position)
                    PageScroll.Offset = new Vector(0,
                        Math.Clamp(position.Y + 28, 0, Math.Max(0, PageScroll.Extent.Height - PageScroll.Viewport.Height)));
            }
            else PageScroll.Offset = default;
        }

        Apply();
        // Initial selection/expansion in a later section can request scrolling during attachment.
        // Apply the latest explicit destination after those native requests have completed.
        Dispatcher.UIThread.Post(Apply, DispatcherPriority.Background);
    }

    public void ResetScroll()
    {
        ScheduleScroll(null);
    }
}
