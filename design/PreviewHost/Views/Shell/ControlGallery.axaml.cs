using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public partial class ControlGallery : UserControl
{
    private GalleryViewModel? _model;
    private readonly Dictionary<int, UserControl> _pages = [];
    private int _scrollRequest;
    public ControlGallery()
    {
        InitializeComponent(); DataContextChanged += (_,_) => ObserveModel(); DetachedFromVisualTree += (_,_) => Unsubscribe(); AttachedToVisualTree += (_,_) => ObserveModel();
        SizeChanged += (_, _) => SourcePanel.SetViewportHeight(Bounds.Height);
        ExplorerButton.Click += (_, _) =>
        {
            var descriptor = _model?.CurrentPage;
            var id = descriptor?.ControlIds?.FirstOrDefault() ?? descriptor?.Id ?? 1;
            _model?.Select(PreviewPageCatalog.Reference.Id);
            if (CurrentView is ControlExplorerView explorer) explorer.SelectControl(id == 0 ? 1 : id);
        };
    }
    public UserControl? CurrentView => PageHost.Content as UserControl;
    public int CreatedPageCount => _pages.Count;
    private void Unsubscribe() { if(_model is not null) { _model.PropertyChanged -= ModelChanged; _model.ControlRequested -= ControlRequested; } }
    private void ObserveModel() { Unsubscribe(); _model=DataContext as GalleryViewModel; if(_model is not null) { _model.PropertyChanged += ModelChanged; _model.ControlRequested += ControlRequested; } ShowPage(); }
    private void ModelChanged(object? sender,PropertyChangedEventArgs args) { if(args.PropertyName==nameof(GalleryViewModel.CurrentPage)) ShowPage(); }
    private void ShowPage()
    {
        if(_model?.CurrentPage is not { } descriptor) return;
        if(!_pages.TryGetValue(descriptor.Id,out var page))
        {
            _pages.Add(descriptor.Id,page=PreviewPageCatalog.Create(descriptor.Id));
            if(page is ShowcasePage showcase) showcase.ControlRequested += ControlRequested;
            if(page is HomePage home) home.ControlRequested += (_, id) => _model?.Select(id);
            if(page is ControlExplorerView explorer) explorer.ControlRequested += (_, id) => _model?.Select(id);
        }
        // Editors own a bounded viewport; do not measure their canvas at infinite height.
        var isReportDesigner=page is TheOne.Avalonia.Controls.ReportDesign or TheOne.Avalonia.Controls.ReportPreview;
        var fillsViewport=isReportDesigner || page is WorkflowEditorDemoView or ControlExplorerView;
        PageScroll.VerticalScrollBarVisibility=fillsViewport ? global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled : global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
        PageHost.VerticalContentAlignment=fillsViewport ? global::Avalonia.Layout.VerticalAlignment.Stretch : global::Avalonia.Layout.VerticalAlignment.Top;
        PageFrame.Padding=new Thickness(isReportDesigner ? 0 : 28);
        PageHost.Content=page; ResetScroll();
        ExplorerButton.IsVisible = page is not ControlExplorerView;
        SourcePanel.SetViews(page is ShowcasePage grouped ? grouped.SourceViews : new[] { page });
    }
    private void ControlRequested(object? sender,int id) => ScrollToControl(id);
    public UserControl GetControlView(int id) => CurrentView is ShowcasePage showcase ? showcase.GetControlView(id) : CurrentView!;
    public void ScrollToControl(int id)
    {
        ScheduleScroll(id);
    }
    private void ScheduleScroll(int? id)
    {
        var request=++_scrollRequest;
        void Apply()
        {
            if(request!=_scrollRequest) return;
            UpdateLayout();
            if(id is { } controlId && CurrentView is ShowcasePage showcase && showcase.GetSection(controlId).TranslatePoint(default,PageHost) is { } position)
                PageScroll.Offset=new Vector(0,Math.Clamp(position.Y+28,0,Math.Max(0,PageScroll.Extent.Height-PageScroll.Viewport.Height)));
            else PageScroll.Offset=default;
        }
        Apply();
        // Initial selection/expansion in a later section can request scrolling during attachment.
        // Apply the latest explicit destination after those native requests have completed.
        Dispatcher.UIThread.Post(Apply,DispatcherPriority.Background);
    }
    public void ResetScroll() => ScheduleScroll(null);
}
