using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public partial class LegacyControlGallery : UserControl
{
    private int _category;
    private string _search = "";
    private DispatcherTimer? _treeReverseTimer;
    private DispatcherTimer? _virtualTimer;
    private TreeView? _hugeTree;

    public LegacyControlGallery()
    {
        InitializeComponent();
        DetachedFromVisualTree += (_, _) => { StopTreeReverse(); _virtualTimer?.Stop(); };
        SizeChanged += (_, args) =>
        {
            var available = Math.Max(0, args.NewSize.Width - 64);
            Cards.ItemWidth = available >= 900 ? available / 2 : available;
        };
    }

    private void OnMotionChanged(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (sender is CheckBox toggle && TopLevel.GetTopLevel(this) is { } topLevel)
            TheOne.Avalonia.Styling.Motion.SetEnabled(topLevel, toggle.IsChecked == true);
    }

    private void OnExpandDirectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is ComboBox { SelectedIndex: >= 0 and <= 3 } picker && MotionExpander is { } expander)
            expander.ExpandDirection = (ExpandDirection)picker.SelectedIndex;
    }

    private void StopTreeReverse()
    {
        _treeReverseTimer?.Stop();
        _treeReverseTimer = null;
    }

    private void OnTreeToggle(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args)
    {
        StopTreeReverse();
        MotionTreeRoot.IsExpanded = !MotionTreeRoot.IsExpanded;
    }

    private void OnTreeReverse(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args)
    {
        StopTreeReverse();
        MotionTreeRoot.IsExpanded = false;
        var step = 0;
        var timer = _treeReverseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(35) };
        timer.Tick += (_, _) =>
        {
            if (!ReferenceEquals(_treeReverseTimer, timer)) return;
            MotionTreeRoot.IsExpanded = ++step % 2 == 1;
            if (step == 3) StopTreeReverse();
        };
        timer.Start();
    }

    public void ResetScroll() => CatalogScroll.Offset = new Vector(0, 0);

    private void OnTreePointerPressed(object? sender, PointerPressedEventArgs args)
    {
        if (!args.GetCurrentPoint(TreeSample).Properties.IsRightButtonPressed || args.Source is not Control source)
            return;
        var container = source as TreeViewItem ?? source.GetVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
        if (container is { IsEffectivelyEnabled: true })
            TreeSample.SelectedItem = TreeSample.ItemFromContainer(container);
    }

    public void SelectCategory(int category)
    {
        _category = category;
        if (category == 9) InitializeVirtualPreview();
        if (category == 10 && DataGridCard.Child is null) DataGridCard.Child = new DataGridDemoView();
        if (category == 11 && LayoutControlsCard.Child is null) LayoutControlsCard.Child = new LayoutDemoView();
        if (category == 12 && SplitButtonsCard.Child is null) SplitButtonsCard.Child = new SplitButtonDemoView();
        if (category == 13 && TextControlsCard.Child is null) TextControlsCard.Child = new TextControlsDemoView();
        if (category == 14 && FlyoutCard.Child is null) FlyoutCard.Child = new FlyoutDemoView();
        if (category == 15 && SelectionControlsCard.Child is null) SelectionControlsCard.Child = new SelectionControlsDemoView();
        if (category == 16 && ButtonControlsCard.Child is null) ButtonControlsCard.Child = new ButtonControlsDemoView();
        if (category == 17 && TabStripCard.Child is null) TabStripCard.Child = new TabStripDemoView();
        if (category == 18 && CarouselCard.Child is null) CarouselCard.Child = new CarouselDemoView();
        if (category == 19 && RefreshCard.Child is null) RefreshCard.Child = new RefreshDemoView();
        if (category == 20 && ContentTransitionCard.Child is null) ContentTransitionCard.Child = new ContentTransitionDemoView();
        if (category == 21 && NotificationCardPreview.Child is null) NotificationCardPreview.Child = new NotificationDemoView();
        if (category == 22 && OneWindowCard.Child is null) OneWindowCard.Child = new OneWindowDemoView();
        if (category == 23 && ChartCard.Child is null) ChartCard.Child = new ChartDemoView();
        if (category == 25 && NavMenuPreviewHost.Child is null) NavMenuPreviewHost.Child = new NavMenuDemoView();
        if (category == 24 && InfoCardPreviewHost.Child is null) InfoCardPreviewHost.Child = new InfoCardDemoView();
        if (category == 9) _virtualTimer?.Start(); else _virtualTimer?.Stop();
        ApplyFilter();
    }

    private void InitializeVirtualPreview()
    {
        if (_hugeTree is not null) return;
        HugeList.ItemsSource = VirtualizationPreview.Rows;
        HugeCombo.ItemsSource = VirtualizationPreview.Rows;
        HugeAuto.ItemsSource = VirtualizationPreview.Rows;
        HugeTabs.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<string>(VirtualizationPreview.Rows);
        HugeMenuTarget.ContextMenu = new ContextMenu { ItemsSource = VirtualizationPreview.Rows, MaxHeight = 300 };
        _hugeTree = VirtualizationPreview.CreateTree();
        HugeTreeHost.Child = _hugeTree;
        _virtualTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _virtualTimer.Tick += (_, _) => UpdateVirtualCounts();
        UpdateVirtualCounts();
    }
    private void UpdateVirtualCounts()
    {
        var branch = _hugeTree?.GetRealizedContainers().OfType<TreeViewItem>().FirstOrDefault(node => node.IsExpanded);
        VirtualCounts.Text = $"列表 {HugeList.GetRealizedContainers().Count()} / 10,000 · 树根 {_hugeTree?.GetRealizedContainers().Count()} / 10,000 · 子节点 {branch?.GetRealizedContainers().Count() ?? 0} / 10,000 · 标签 {HugeTabs.GetRealizedContainers().Count()} / {HugeTabs.ItemCount:N0}";
    }
    private void OnHugeJump(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args)
    {
        HugeList.SelectedIndex = 9999; HugeList.ScrollIntoView(9999);
        if (_hugeTree is { } tree && TheOne.Avalonia.Controls.Virtualization.BringIntoView(tree, 0, 9000) is { } node)
        {
            tree.SelectedItem = ((ItemsControl)node.Parent!).ItemFromContainer(node); node.Focus();
        }
        UpdateVirtualCounts();
    }
    private void OnHugeTabJump(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args) => HugeTabs.SelectedIndex = 9000;

    public void Search(string search)
    {
        _search = search.Trim();
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var visible = 0;
        foreach (var card in Cards.Children.OfType<Border>().Append(DataGridCard).Append(LayoutControlsCard).Append(SplitButtonsCard).Append(TextControlsCard).Append(FlyoutCard).Append(SelectionControlsCard).Append(ButtonControlsCard).Append(TabStripCard).Append(CarouselCard).Append(RefreshCard).Append(ContentTransitionCard).Append(NotificationCardPreview).Append(OneWindowCard).Append(ChartCard).Append(InfoCardPreviewHost).Append(NavMenuPreviewHost))
        {
            var metadata = (card.Tag as string ?? "").Split('|', 2);
            card.IsVisible = (_category == 0 && (metadata[0] is not ("9" or "10" or "11" or "12" or "13" or "14" or "15" or "16" or "17" or "18" or "19" or "20" or "21" or "22" or "23" or "24" or "25") || _search.Length > 0 && metadata[0] is ("10" or "11" or "12" or "13" or "14" or "15" or "16" or "17" or "18" or "19" or "20" or "21" or "22" or "23" or "24" or "25")) || metadata[0] == _category.ToString())
                && (metadata.Length > 1 && metadata[1].Contains(_search, StringComparison.OrdinalIgnoreCase));
            if (card.IsVisible)
            {
                if (card == DataGridCard && card.Child is null) card.Child = new DataGridDemoView();
                if (card == LayoutControlsCard && card.Child is null) card.Child = new LayoutDemoView();
                if (card == SplitButtonsCard && card.Child is null) card.Child = new SplitButtonDemoView();
                if (card == TextControlsCard && card.Child is null) card.Child = new TextControlsDemoView();
                if (card == FlyoutCard && card.Child is null) card.Child = new FlyoutDemoView();
                if (card == SelectionControlsCard && card.Child is null) card.Child = new SelectionControlsDemoView();
                if (card == ButtonControlsCard && card.Child is null) card.Child = new ButtonControlsDemoView();
                if (card == TabStripCard && card.Child is null) card.Child = new TabStripDemoView();
                if (card == CarouselCard && card.Child is null) card.Child = new CarouselDemoView();
                if (card == RefreshCard && card.Child is null) card.Child = new RefreshDemoView();
                if (card == ContentTransitionCard && card.Child is null) card.Child = new ContentTransitionDemoView();
                if (card == NotificationCardPreview && card.Child is null) card.Child = new NotificationDemoView();
                if (card == OneWindowCard && card.Child is null) card.Child = new OneWindowDemoView();
                if (card == ChartCard && card.Child is null) card.Child = new ChartDemoView();
                if (card == NavMenuPreviewHost && card.Child is null) card.Child = new NavMenuDemoView();
                if (card == InfoCardPreviewHost && card.Child is null) card.Child = new InfoCardDemoView();
                visible++;
            }
        }
        NoResults.IsVisible = visible == 0;
        CatalogScroll.Offset = new Vector(0, 0);
    }
}
