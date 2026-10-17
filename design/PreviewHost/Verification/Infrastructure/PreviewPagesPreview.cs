using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using ReactiveUI;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class PreviewPagesPreview
{
    public static void VerifyReportDesignPage(Action<bool, string> check, string? output = null)
    {
        var descriptor = PreviewPageCatalog.Find(318);
        check(
            descriptor.Title == "ReportDesign" && descriptor.Group == "扩展控件" &&
            PreviewPageCatalog.Pages.Count(p => p.Id == 318) == 1,
            "PreviewHost/ReportDesign: unique extension navigation entry");
        var window = new MainWindow { Width = 1920, Height = 1200, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        try
        {
            window.NavigateToCategory(318);
            ControlPreview.Flush();
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            var designer = (ReportDesign)gallery.GetControlView(318);
            var paperScroll = designer.FindControl<ScrollViewer>("PageScroll")!;
            var hostScroll = gallery.FindControl<ScrollViewer>("PageScroll")!;
            check(
                gallery.CurrentView == designer && window.Model.CurrentPage == descriptor &&
                designer.Document!.Elements.Count > 0 && designer.DataSource is not null,
                "PreviewHost/ReportDesign: navigation opens the editable sales report sample");
            check(hostScroll.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled &&
                  gallery.FindControl<Border>("PageFrame")!.Padding == new Thickness(0)
                  && designer.Bounds.Height > 600 && paperScroll.Bounds.Height > 300 &&
                  designer.Bounds.Height <= gallery.Bounds.Height,
                "PreviewHost/ReportDesign: the designer fills a bounded viewport and owns paper scrolling");
            designer.Document!.Title = "保留报表编辑";
            var preview = designer.FindControl<TabItem>("PreviewTab")!;
            var point = preview.TranslatePoint(new Point(preview.Bounds.Width / 2, preview.Bounds.Height / 2), window)!
                .Value;
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            ControlPreview.Flush();
            check(designer.IsPreview && designer.Layout!.Pages.Count > 1,
                "PreviewHost/ReportDesign: the embedded native preview tab opens paginated details");
            window.NavigateToCategory(1);
            ControlPreview.Flush();
            check(
                hostScroll.VerticalScrollBarVisibility == ScrollBarVisibility.Auto &&
                gallery.FindControl<Border>("PageFrame")!.Padding == new Thickness(28),
                "PreviewHost/ReportDesign: leaving the designer restores the standard gallery layout");
            window.Model.Search = "ReportDesign";
            ControlPreview.Flush();
            window.Model.SelectedSearchResult = window.Model.SearchResults.Single(p => p.Title == "ReportDesign");
            ControlPreview.Flush();
            check(
                gallery.CurrentView == designer && designer.Document.Title == "保留报表编辑" && designer.IsPreview &&
                window.Model.Search == "",
                "PreviewHost/ReportDesign: search returns to the cached document and preview state");
            designer.ShowDesign();
            ControlPreview.Flush();
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame();
                frame?.Save(Path.Combine(output, "reportdesign-previewhost.png"), PngBitmapEncoderOptions.Default);
            }

            window.NavigateToCategory(319);
            ControlPreview.Flush();
            var reader = (ReportPreview)gallery.GetControlView(319);
            check(
                PreviewPageCatalog.Find(319).Title == "ReportPreview" &&
                reader.Layout?.Pages.SelectMany(p => p.Rows).Count() == 96,
                "PreviewHost/ReportPreview: the independent viewer entry loads document JSON and its supplied sample source");
            check(
                reader.Bounds.Height > 600 && reader.Bounds.Height <= gallery.Bounds.Height &&
                hostScroll.VerticalScrollBarVisibility == ScrollBarVisibility.Disabled,
                "PreviewHost/ReportPreview: the independent viewer owns a bounded page viewport");
            if (output is not null)
            {
                using var frame = window.CaptureRenderedFrame();
                frame?.Save(Path.Combine(output, "reportpreview-previewhost.png"), PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }
    }

    public static void VerifyWorkflowPage(Action<bool, string> check)
    {
        var descriptor = PreviewPageCatalog.Find(315);
        check(
            descriptor.Title == "WorkflowEditor" && descriptor.Group == "扩展控件" &&
            PreviewPageCatalog.Pages.Count(p => p.Id == 315) == 1,
            "PreviewHost/WorkflowEditor: unique extension navigation entry");
        var window = new MainWindow { Width = 1600, Height = 1100, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        try
        {
            window.NavigateToCategory(315);
            ControlPreview.Flush();
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            var demo = (WorkflowEditorDemoView)gallery.GetControlView(315);
            check(gallery.CurrentView == demo && demo.Model.Document.Nodes.Count(n => !n.IsAnnotation) == 7,
                "PreviewHost/WorkflowEditor: navigation arranges the complete seven-node design sample");
            var editor = demo.FindControl<WorkflowEditor>("Editor")!;
            var height = editor.Bounds.Height;
            window.Height = 850;
            ControlPreview.Flush();
            check(
                editor.Bounds.Height < height && editor.Bounds.Height > 500 &&
                gallery.FindControl<ScrollViewer>("PageScroll")!.VerticalScrollBarVisibility ==
                ScrollBarVisibility.Disabled,
                "PreviewHost/WorkflowEditor: gallery supplies a finite viewport and editor fills it on resize");
            var node = demo.Model.Document.Nodes.Single(n => n.Id == "approval_01");
            node.Title = "保留配置";
            window.NavigateToCategory(1);
            window.Model.Search = "WorkflowEditor";
            ControlPreview.Flush();
            window.Model.SelectedSearchResult = window.Model.SearchResults.Single(p => p.Title == "WorkflowEditor");
            ControlPreview.Flush();
            check(
                gallery.CurrentView == demo &&
                demo.Model.Document.Nodes.Single(n => n.Id == "approval_01").Title == "保留配置",
                "PreviewHost/WorkflowEditor: searching restores the cached view and consumer data");
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }
    }

    public static void VerifyDataGridTreePage(Action<bool, string> check, string? output = null)
    {
        var descriptor = PreviewPageCatalog.Find(313);
        check(descriptor.Title == "DataGridTree" && descriptor.Group == "数据展示"
                                                 && PreviewPageCatalog.Pages.Count(page => page.Id == 313) == 1,
            "PreviewHost/DataGridTree: tree grid has its own data display navigation entry");
        var window = new MainWindow { Width = 1280, Height = 1000, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        DataGridTreeFixture? demo = null;
        try
        {
            window.NavigateToCategory(313);
            ControlPreview.Flush();
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            demo = (DataGridTreeFixture)gallery.GetControlView(313);
            check(gallery.CurrentView == demo && window.Model.CurrentPage == descriptor
                                              && demo.Grid.Bounds.Height > 0 &&
                                              ReferenceEquals(demo.Grid.CollectionView, demo.Model.View),
                "PreviewHost/DataGridTree: normal navigation arranges the existing tree fixture and native grid");
            check(DataGridFeatures.GetShowColumnFilters(demo.Grid) && DataGridFeatures.GetShowSummaryRow(demo.Grid),
                "PreviewHost/DataGridTree: page exposes phase two filtering and summaries");
            demo.Model.View.TrySetExpanded(demo.Model.Root, true);
            demo.Grid.SelectedItem = demo.Model.Branch;
            ControlPreview.Flush();
            window.NavigateToCategory(10);
            window.Model.Search = "DataGridTree";
            ControlPreview.Flush();
            window.Model.SelectedSearchResult = window.Model.SearchResults.Single(page => page.Title == "DataGridTree");
            ControlPreview.Flush();
            check(gallery.CurrentView == demo && demo.Model.View.Count == 4
                                              && ReferenceEquals(demo.Grid.SelectedItem, demo.Model.Branch) &&
                                              window.Model.Search == "",
                "PreviewHost/DataGridTree: search returns to the cached page with expansion and selection intact");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame();
                frame?.Save(Path.Combine(output, "datagrid-tree-page.png"), PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
            if (demo is not null)
            {
                demo.Grid.ItemsSource = null;
                demo.Model.Dispose();
            }

            ControlPreview.Flush();
        }
    }

    public static void VerifyPaginationPage(Action<bool, string> check)
    {
        var descriptor = PreviewPageCatalog.Find(312);
        check(descriptor.Title == "Pagination" && descriptor.Group == "扩展控件",
            "PreviewHost/Pagination: catalogue and extension navigation identify the pagination page");
        var window = new MainWindow { Width = 1280, Height = 1000, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        try
        {
            window.NavigateToCategory(312);
            ControlPreview.Flush();
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            var demo = (PaginationDemoView)gallery.GetControlView(312);
            check(
                gallery.CurrentView == demo && window.Model.CurrentPage == descriptor &&
                demo.FindControl<Pagination>("StandardPagination")!.PageCount == 64,
                "PreviewHost/Pagination: navigation loads the typed demo and actual pagination controls");
            demo.FindControl<Pagination>("StandardPagination")!.GoToPage(10);
            ControlPreview.Flush();
            window.NavigateToCategory(1);
            window.Model.Search = "Pagination";
            ControlPreview.Flush();
            window.Model.SelectedSearchResult = window.Model.SearchResults.Single(page => page.Title == "Pagination");
            ControlPreview.Flush();
            check(gallery.CurrentView == demo && demo.Model.CurrentPage == 10 && window.Model.Search == "",
                "PreviewHost/Pagination: searching returns to the cached page without resetting its shared page state");
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }
    }

    public static void Verify(Action<bool, string> check)
    {
        Console.WriteLine($"Runtime dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");
        VerifyReactiveContracts(check);
        PreviewViewModelContracts.Verify(check);
        PreviewCommandContracts.Verify(check);
        var window = new MainWindow { Width = 1280, Height = 920, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        var gallery = window.FindControl<ControlGallery>("Gallery")!;
        check(window.Model.NavigationRoots.Count ==
              PreviewPageCatalog.Pages.Select(page => page.Group).Distinct().Count()
              && window.Model.NavigationRoots.Sum(group => group.Entries.Count) +
              window.Model.NavigationRoots.Count(root => root.Category is not null) == PreviewPageCatalog.Pages.Count,
            "PreviewHost/pages: standalone Home and grouped pages each have one navigation entry");
        check(gallery.CreatedPageCount == 1, "PreviewHost/pages: opening shell creates only the active page");
        foreach (var page in PreviewPageCatalog.Pages)
        {
            window.NavigateToCategory(page.Id);
            ControlPreview.Flush();
            var view = gallery.CurrentView ?? throw new InvalidOperationException("Missing active page.");
            check(view is not LegacyControlGallery && view.DataContext is not Control,
                $"PreviewHost/pages/{page.Title}: separate typed view and presentation data");
            check(window.Model.CurrentPage == page,
                $"PreviewHost/pages/{page.Title}: navigation selects its descriptor");
            if (page.ControlIds is not null)
                check(gallery.FindControl<ScrollViewer>("PageScroll")!.Offset.Y == 0,
                    $"PreviewHost/groups/{page.Title}: initial selection in later sections cannot move the page away from its heading");
            if (page.Group == "图表控件")
                foreach (var id in page.ControlIds!)
                {
                    var section = (ChartShowcaseView)gallery.GetControlView(id);
                    var title = PreviewPageCatalog.Controls.First(control => control.Id == id).Title;
                    var variantCount = section.Kind == ChartKind.Gauge ? Enum.GetValues<GaugeStyle>().Length : 3;
                    check(
                        section.Samples.Count == variantCount &&
                        section.Samples.All(sample => sample.GetType().Name == title),
                        $"PreviewHost/charts/{title}: all simultaneous variants retain the native control type");
                    check(
                        section.Samples.Select(sample => sample.Title).Distinct().Count() == variantCount &&
                        !section.Samples.Any(sample => sample.GetVisualDescendants().Any()),
                        $"PreviewHost/charts/{title}: distinct presentations draw data without per-point controls");
                    var editor = section.OpenEditor();
                    ControlPreview.Flush();
                    editor.Model.Reset();
                    ControlPreview.Flush();
                    check(editor.ChartSample.GetType().Name == title && editor.Model.IsFixedKind,
                        $"PreviewHost/charts/{title}: the lazily opened editor keeps its fixed type after resetting data");
                    section.CloseEditor();
                    ControlPreview.Flush();
                }
        }

        check(gallery.CreatedPageCount == PreviewPageCatalog.Pages.Count,
            "PreviewHost/pages: previously visited pages retain their individual state");
        window.NavigateToCategory(2);
        ControlPreview.Flush();
        var textPage = (TextBoxPage)gallery.GetControlView(2);
        var text = textPage.FindControl<TextBox>("ControlSample")!;
        text.Text = "来自控件的编辑";
        ControlPreview.Flush();
        check(textPage.Model.Text == text.Text, "PreviewHost/mvvm: TextBox writes its separate view model");
        textPage.Model.Text = "来自模型的更新";
        ControlPreview.Flush();
        check(text.Text == textPage.Model.Text, "PreviewHost/mvvm: view model updates TextBox");
        window.NavigateToCategory(6);
        ControlPreview.Flush();
        var tabPage = (TabControlPage)gallery.GetControlView(6);
        var tabModel = (TabControlPageViewModel)tabPage.Model;
        var tabs = tabPage.FindControl<TabControl>("ControlSample")!;
        tabModel.LastTabCommand.Execute(null);
        ControlPreview.Flush();
        check(tabs.SelectedIndex == 999 && tabs.GetRealizedContainers().Count() < 80,
            "PreviewHost/TabControl: thousand mutable models retain bounded header virtualization");
        var moved = tabModel.Tabs[999];
        check(TabFeatures.Move(tabs, 999, 0) && tabModel.Tabs[0] == moved,
            "PreviewHost/TabControl: reorder updates the view model collection");
        tabs.ScrollIntoView(0);
        ControlPreview.Flush();
        check(tabs.ContainerFromIndex(0) is TabItem item && TabFeatures.Close(item) && tabModel.Tabs.Count == 999,
            "PreviewHost/TabControl: closing updates the view model collection");
        tabModel.AddTabCommand.Execute(null);
        ControlPreview.Flush();
        check(tabModel.Tabs.Count == 1000 && tabs.SelectedIndex == 999,
            "PreviewHost/TabControl: add command selects the new distant item");
        window.NavigateToCategory(3);
        ControlPreview.Flush();
        var choice = (CheckBoxPage)gallery.GetControlView(3);
        choice.FindControl<CheckBox>("ControlSample")!.IsChecked = true;
        ControlPreview.Flush();
        check(choice.Model.Checked == true, "PreviewHost/mvvm: CheckBox state is two-way");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            window.Model.Theme = theme;
            window.Model.Size = size;
            window.NavigateToCategory(1);
            ControlPreview.Flush();
            var button = ((ButtonPage)gallery.GetControlView(1)).FindControl<Button>("ControlSample")!;
            check(button.ActualThemeVariant == theme && ControlSizing.GetSize(button) == size,
                $"PreviewHost/shell/{theme.Key}/{size}: global settings reach the grouped page");
        }

        check(PreviewPageCatalog.Pages.Count < PreviewPageCatalog.Controls.Count * 0.65,
            "PreviewHost/groups: related controls reduce navigation destinations");
        check(PreviewPageCatalog.Find(1) == PreviewPageCatalog.Find(130) && PreviewPageCatalog.Find(1).Title == "按钮",
            "PreviewHost/groups: all five button controls share one destination");
        check(
            PreviewPageCatalog.Find(13) == PreviewPageCatalog.Find(24) &&
            PreviewPageCatalog.Find(10).ControlIds is null,
            "PreviewHost/groups: data display is combined and DataGrid remains independent");
        foreach (var id in new[] { 118, 9, 7, 18, 11 })
        {
            window.NavigateToCategory(id);
            ControlPreview.Flush();
            var view = gallery.GetControlView(id);
            if (view is BasicControlPage basic)
            {
                basic.Model.LastCommand.Execute(null);
                ControlPreview.Flush();
            }
            else if (view is CarouselDemoView carousel)
            {
                carousel.Model.LastCommand.Execute(null);
                ControlPreview.Flush();
            }

            foreach (var items in view.GetVisualDescendants().OfType<ItemsControl>().Where(items =>
                         items is ListBox or TreeView or Carousel || items.GetType() == typeof(ItemsControl)))
            {
                check(items.ItemCount == 1000 && items.GetRealizedContainers().Count() is > 0 and < 80,
                    $"PreviewHost/groups/{id}/{items.Name}: both styles retain a bounded thousand-item viewport");
                check(items.ContainerFromIndex(999) is not null,
                    $"PreviewHost/groups/{id}/{items.Name}: far item is realized without creating preceding rows");
                if (items is TreeView tree)
                {
                    var child = Virtualization.BringIntoView(tree, 0, 999);
                    ControlPreview.Flush();
                    var parent = (TreeViewItem)tree.ContainerFromIndex(0)!;
                    check(
                        child is not null && parent.ItemCount == 1000 &&
                        parent.GetRealizedContainers().Count() is > 0 and < 80,
                        $"PreviewHost/groups/{id}/{items.Name}: the thousand-child collection also recycles and realizes its last item");
                    tree.SetCurrentValue(TreeView.SelectedItemProperty, parent.ItemFromContainer(child!));
                    ControlPreview.Flush();
                    check(
                        ((TreeViewPage)view).Model.SelectedItem ==
                        ((TreeViewPage)view).Model.TreeRoots[0].Children[999],
                        $"PreviewHost/groups/{id}/{items.Name}: distant child selection updates the shared model");
                }
            }
        }

        window.NavigateToCategory(11);
        ControlPreview.Flush();
        var splitPage = (SplitViewPage)gallery.GetControlView(11);
        var paneList = splitPage.FindControl<ListBox>("DetailedSample")!;
        splitPage.Model.Expanded = false;
        ControlPreview.Flush();
        check(paneList.SelectedIndex == 999 && paneList.GetRealizedContainers().Count() is > 0 and < 80
                                            && !paneList.GetVisualDescendants().OfType<TextBlock>()
                                                .Any(text => text.IsEffectivelyVisible),
            "PreviewHost/SplitView: compact pane hides labels and preserves distant selection and bounded recycling");
        splitPage.Model.Expanded = true;
        ControlPreview.Flush();
        check(
            paneList.ContainerFromIndex(999) is not null && paneList.GetVisualDescendants().OfType<TextBlock>()
                .Any(text => text.IsEffectivelyVisible),
            "PreviewHost/SplitView: reopening restores labels and the selected thousandth row");
        check(!PreviewPageCatalog.Controls.Any(control => control.Title == "RefreshVisualizer"),
            "PreviewHost: RefreshVisualizer has no standalone catalogue page");
        window.NavigateToCategory(1);
        ControlPreview.Flush();
        var cachedButton = gallery.GetControlView(1);
        var pageCount = gallery.CreatedPageCount;
        window.Model.Search = "ToggleButton";
        ControlPreview.Flush();
        window.Model.SelectedSearchResult = window.Model.SearchResults.Single(page => page.Title == "ToggleButton");
        ControlPreview.Flush();
        check(
            gallery.GetControlView(1) == cachedButton && gallery.CreatedPageCount == pageCount &&
            gallery.FindControl<ScrollViewer>("PageScroll")!.Offset.Y > 0,
            "PreviewHost/search: a member search locates its section in the cached group, including the current page");
        window.Model.Search = "ToggleButton";
        ControlPreview.Flush();
        window.Model.SelectedSearchResult = window.Model.SearchResults.Single(page => page.Title == "ToggleButton");
        ControlPreview.Flush();
        check(window.Model.Search.Length == 0,
            "PreviewHost/search: repeated selection of the same control dismisses search");
        check(PreviewPageCatalog.Pages.Count(page => page.Group == "图表控件") == 6,
            "PreviewHost/charts: fourteen chart types share six family destinations");
        window.Model.Search = "PercentStackedAreaChart";
        ControlPreview.Flush();
        window.Model.SelectedSearchResult =
            window.Model.SearchResults.Single(page => page.Title == "PercentStackedAreaChart");
        ControlPreview.Flush();
        check(window.Model.CurrentPage?.Id == 412 && gallery.FindControl<ScrollViewer>("PageScroll")!.Offset.Y > 0,
            "PreviewHost/charts/search: a specific type resolves and scrolls to its family section");
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var descriptor in PreviewPageCatalog.Controls.Where(control => control.Group == "图表控件"))
        {
            window.Model.Theme = theme;
            window.Model.Size = size;
            window.NavigateToCategory(descriptor.Id);
            ControlPreview.Flush();
            var section = (ChartShowcaseView)gallery.GetControlView(descriptor.Id);
            if (section.Kind == ChartKind.Gauge)
                check(section.Samples.All(sample =>
                        sample.Bounds.Width == (size == ControlSize.Small ? 240 : size == ControlSize.Large ? 360 : 300)
                        && sample.Bounds.Height ==
                        (size == ControlSize.Small ? 224 : size == ControlSize.Large ? 336 : 280)
                        && sample.FontSize == (size == ControlSize.Small ? 10 : size == ControlSize.Large ? 12 : 11)),
                    $"PreviewHost/Gauge/{theme.Key}/{size}: actual viewport and typography change with inherited density");
            check(
                section.Samples.All(sample =>
                    sample.ActualThemeVariant == theme && ControlSizing.GetSize(sample) == size &&
                    sample.Bounds.Width > 0 && sample.Bounds.Height > 0),
                $"PreviewHost/charts/{descriptor.Title}/{theme.Key}/{size}: every variant inherits live theme and density with a measured viewport");
        }

        window.NavigateToCategory(23);
        ControlPreview.Flush();
        var line = (ChartShowcaseView)gallery.GetControlView(23);
        var lineEditor = line.OpenEditor();
        ControlPreview.Flush();
        lineEditor.Model.LoadThousandPoints();
        ControlPreview.Flush();
        lineEditor.Model.SelectedSeries = 0;
        lineEditor.Model.SelectedPoint = 999;
        lineEditor.Model.SelectedSeries = 0;
        lineEditor.Model.SelectedPoint = 999;
        lineEditor.ChartSample.BringDataPointIntoView(0, 999);
        ControlPreview.Flush();
        check(
            lineEditor.ChartSample.Selection?.PointIndex == 999 && !lineEditor.ChartSample.GetVisualDescendants().Any(),
            "PreviewHost/charts: grouped editor selects a distant thousandth point without item controls");
        window.NavigateToCategory(205);
        window.NavigateToCategory(23);
        ControlPreview.Flush();
        check(
            ReferenceEquals(line, gallery.GetControlView(23)) && lineEditor.Model.Series[0].ItemsSource?.Count == 1000,
            "PreviewHost/charts: leaving and revisiting a family retains its editor data");
        lineEditor.FindControl<Button>("RapidChartUpdate")!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        check(lineEditor.IsRapidUpdating, "PreviewHost/charts: the expanded editor can start real update actions");
        line.CloseEditor();
        ControlPreview.Flush();
        check(!lineEditor.IsRapidUpdating,
            "PreviewHost/charts: collapsing the editor stops its background update sequence");
        window.Model.Search = "Toast";
        ControlPreview.Flush();
        check(window.Model.SearchResults.Any(page => page.Title == "Toast"),
            "PreviewHost/search: independent message pages are searchable");
        window.Model.Search = "";
        window.Width = 700;
        ControlPreview.Flush();
        check(window.Model.EffectiveNavigationCollapsed && window.FindControl<NavMenu>("Navigation")!.IsVisible,
            "PreviewHost/compact: narrow shell retains icon navigation");
        window.Close();
        ControlPreview.Flush();
    }

    private static void VerifyReactiveContracts(Action<bool, string> check)
    {
        var model = new BasicControlViewModel();
        var before = model.Text;
        var changing = 0;
        var changed = 0;
        ((INotifyPropertyChanging)model).PropertyChanging += (_, args) =>
        {
            if (args.PropertyName == nameof(model.Text) && model.Text == before) changing++;
        };
        model.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(model.Text)) changed++;
        };
        model.Text = "ReactiveUI 编译绑定";
        model.Text = "ReactiveUI 编译绑定";
        check(model is ReactiveObject && changing == 1 && changed == 1,
            "PreviewHost/reactive: VM changes use ReactiveObject notifications and suppress equal values");
        object? received = null;
        using var command = ReactiveCommand.Create<object?>(parameter => received = parameter);
        ((ICommand)command).Execute("原始命令参数");
        ControlPreview.Flush();
        check(((ICommand)command).CanExecute(null) && Equals(received, "原始命令参数"),
            "PreviewHost/reactive: ReactiveCommand-backed ICommand preserves execution and parameters");
    }

    public static void Render(string output)
    {
        RenderPages(output, [400, 405], "previewhost");
    }

    public static void RenderCharts(string output)
    {
        RenderPages(output, [409, 410, 411, 412, 413, 414], "chartgallery");
    }

    private static void RenderPages(string output, int[] ids, string prefix)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 700, 1280 })
        foreach (var id in ids)
        {
            var window = new MainWindow { Width = width, Height = 1000, WindowDecorations = WindowDecorations.None };
            window.Model.Theme = theme;
            window.Model.Size = size;
            window.Model.MotionEnabled = false;
            window.Show();
            window.NavigateToCategory(id);
            ControlPreview.Flush();
            using var image = window.CaptureRenderedFrame()!;
            image.Save(Path.Combine(output, $"{prefix}-{id}-{theme.Key}-{size}-{width}.png"),
                PngBitmapEncoderOptions.Default);
            window.Close();
        }
    }
}