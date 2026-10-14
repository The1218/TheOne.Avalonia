using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Explicit, AOT-safe factories render only requested controls. Never falls back to a full export.</summary>
internal static class TargetedControlPreview
{
    private static readonly Dictionary<string, Func<Control>> Samples = new(StringComparer.OrdinalIgnoreCase)
    {
        ["LineChart"] = () => ChartPreview.CreateSample(ChartKind.Line),
        ["AreaChart"] = () => ChartPreview.CreateSample(ChartKind.Area),
        ["BarChart"] = () => ChartPreview.CreateSample(ChartKind.Bar),
        ["HorizontalBarChart"] = () => ChartPreview.CreateSample(ChartKind.HorizontalBar),
        ["ScatterChart"] = () => ChartPreview.CreateSample(ChartKind.Scatter),
        ["PieChart"] = () => ChartPreview.CreateSample(ChartKind.Pie),
        ["DoughnutChart"] = () => ChartPreview.CreateSample(ChartKind.Doughnut),
        ["GaugeChart"] = () => ChartPreview.CreateSample(ChartKind.Gauge),
        ["StackedBarChart"] = () => ChartPreview.CreateSample(ChartKind.StackedBar),
        ["StackedHorizontalBarChart"] = () => ChartPreview.CreateSample(ChartKind.StackedHorizontalBar),
        ["StackedAreaChart"] = () => ChartPreview.CreateSample(ChartKind.StackedArea),
        ["PercentStackedBarChart"] = () => ChartPreview.CreateSample(ChartKind.PercentStackedBar),
        ["PercentStackedHorizontalBarChart"] = () => ChartPreview.CreateSample(ChartKind.PercentStackedHorizontalBar),
        ["PercentStackedAreaChart"] = () => ChartPreview.CreateSample(ChartKind.PercentStackedArea),
        ["DualYAxisChart"] = ChartPreview.CreateDualAxisSample,
        ["LargeSamplingChart"] = ChartPreview.CreateLargeSample,
        ["CrosshairChart"] = ChartPreview.CreateCrosshairSample,
        ["SharedTooltipChart"] = ChartPreview.CreateSharedTooltipSample,
        ["InteractiveLegendChart"] = ChartPreview.CreateInteractiveLegendSample,
        ["SmoothLineChart"] = ChartPreview.CreateSmoothSample,
        ["LabeledLineChart"] = ChartPreview.CreateLabeledLineSample,
        ["AnimatedChart"] = ChartPreview.CreateAnimatedSample,
        ["PieHoverChart"] = () => ChartPreview.CreateShapeHoverSample(ChartKind.Pie),
        ["DoughnutHoverChart"] = () => ChartPreview.CreateShapeHoverSample(ChartKind.Doughnut),
        ["BarHoverChart"] = () => ChartPreview.CreateShapeHoverSample(ChartKind.Bar),
        ["NeedleGaugeChart"] = ChartPreview.CreateNeedleGaugeSample,
        ["PrecisionGaugeChart"] = () => new GaugeChart
        {
            Title = "小数刻度 · 0–1", Minimum = 0, Maximum = 1, Value = 0.71,
            ShowGaugeNeedle = true, ShowGaugeTicks = true,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Left
        },
        ["ZoomedLineChart"] = () => new LineChart
        {
            Height = 300, Title = "缩放后仍显示穿过视口的线段", Viewport = new Rect(0.25, 0.25, 0.5, 0.5),
            SeriesSource = new[] { new ChartSeries { Name = "趋势", ItemsSource = new[] { new ChartPoint(0, 0), new ChartPoint(100, 100) } } }
        },
        ["PannedBarChart"] = () => { var chart = ChartPreview.CreateSample(ChartKind.Bar); chart.Viewport = new Rect(0.4, 0, 0.5, 1); return chart; },
        [nameof(Button)] = () => new Button { Content = "继续操作" },
        [nameof(ToggleButton)] = () => new ToggleButton { Content = "切换", IsChecked = true },
        [nameof(RepeatButton)] = () => new RepeatButton { Content = "持续操作" },
        [nameof(CheckBox)] = () => new CheckBox { Content = "启用功能", IsChecked = true },
        [nameof(RadioButton)] = () => new RadioButton { Content = "当前选项", IsChecked = true },
        [nameof(ToggleSwitch)] = () => new ToggleSwitch { Content = "开启功能", IsChecked = true },
        [nameof(TextBox)] = () => new TextBox { Text = "输入内容", Width = 280 },
        [nameof(NumericUpDown)] = () => new NumericUpDown { Value = 42, Width = 200 },
        [nameof(Slider)] = () => new Slider { Value = 40, Width = 280 },
        [nameof(ProgressBar)] = () => new ProgressBar { Value = 65, Width = 280 },
        [nameof(ComboBox)] = () => new ComboBox { ItemsSource = new[] { "第一项", "第二项", "第三项" }, SelectedIndex = 1, Width = 240 },
        [nameof(ComboBoxItem)] = () => new ComboBoxItem { Content = "下拉条目", Width = 240 },
        [nameof(ListBoxItem)] = () => new ListBoxItem { Content = "列表条目", Width = 240 },
        [nameof(AutoCompleteBox)] = () => new AutoCompleteBox { ItemsSource = new[] { "Avalonia", "TheOne" }, Text = "TheOne", Width = 240 },
        [nameof(CalendarDatePicker)] = () => new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 13), Width = 240 },
        [nameof(Calendar)] = () => new Calendar { SelectedDate = new DateTime(2026, 10, 13) },
        [nameof(ListBox)] = () => new ListBox { ItemsSource = Rows(), SelectedIndex = 2, Width = 280, Height = 200 },
        [nameof(ItemsControl)] = () => new ItemsControl { ItemsSource = Rows(), Width = 280, Height = 200 },
        [nameof(TreeView)] = () => new TreeView { Width = 280, Height = 200, Items = { new TreeViewItem { Header = "工作区", IsExpanded = true, Items = { new TreeViewItem { Header = "主题" }, new TreeViewItem { Header = "控件" } } } } },
        [nameof(TabControl)] = () => new TabControl { Width = 340, Height = 160, Items = { new TabItem { Header = "概览", Content = "当前标签内容" }, new TabItem { Header = "设置" } } },
        [nameof(Expander)] = () => new Expander { Header = "更多信息", Content = "展开内容", IsExpanded = true, Width = 280 },
        [nameof(SplitView)] = () => new SplitViewPage(),
        [nameof(GridSplitter)] = () => new GridSplitter { Width = 6, Height = 160 },
        [nameof(SplitButton)] = () => new SplitButton { Content = "保存", Flyout = new Flyout { Content = "更多操作" } },
        [nameof(ToggleSplitButton)] = () => new ToggleSplitButton { Content = "启用", IsChecked = true, Flyout = new Flyout { Content = "更多操作" } },
        [nameof(Label)] = () => new Label { Content = "字段标题" },
        [nameof(TextBlock)] = () => new TextBlock { Text = "主题文字" },
        [nameof(SelectableTextBlock)] = () => new SelectableTextBlock { Text = "可以选择并复制的文字", SelectionStart = 0, SelectionEnd = 4 },
        [nameof(Carousel)] = () => new Carousel { ItemsSource = Rows(), SelectedIndex = 3, Width = 300, Height = 180 },
        [nameof(TransitioningContentControl)] = () => new TransitioningContentControl { Content = "当前内容", Width = 300, Height = 160 },
        [nameof(NotificationCard)] = () => new NotificationCard { Content = new Notification("操作成功", "已保存更改", NotificationType.Success, TimeSpan.Zero), Width = 320 },
        [nameof(RefreshVisualizer)] = () => new RefreshVisualizer(),
        [nameof(RefreshContainer)] = () => new RefreshContainer { Width = 300, Height = 200, Content = new ListBox { ItemsSource = Rows() } },
        [nameof(FlyoutPresenter)] = () => new FlyoutPresenter { Content = "浮层内容" },
        [nameof(ThemeButton)] = () => new ThemeButton(),
        [nameof(OneDialogPresenter)] = () => new OneDialogPresenter { Header = "确认", Content = "是否继续操作？", Buttons = OneDialogButtons.OkCancel },
        [nameof(Menu)] = () => new Menu { Items = { new MenuItem { Header = "文件" }, new MenuItem { Header = "编辑" } } },
        [nameof(ContextMenu)] = () => new ContextMenu { Items = { new MenuItem { Header = "复制" }, new MenuItem { Header = "粘贴" } } },
        [nameof(MenuItem)] = () => new MenuItem { Header = "打开工作区" },
    };
    private static string[] Rows() => Enumerable.Range(1, 1000).Select(index => $"数据项 {index}").ToArray();
    public static void Render(string output, string[] requested)
    {
        var controls = requested.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        foreach (var name in controls)
            if (!name.Equals("PreviewNavigation", StringComparison.OrdinalIgnoreCase) && !new[] { "LoadingContainer", "ButtonLoading", "IconButtonLoading" }.Contains(name, StringComparer.OrdinalIgnoreCase) && !name.Equals("TabDrag", StringComparison.OrdinalIgnoreCase) && !name.Equals("CardTabs", StringComparison.OrdinalIgnoreCase) && !Samples.ContainsKey(name) && !new[] { "IconButton", "ButtonVariants", "Toast", "Dialog", "Notification" }.Contains(name, StringComparer.OrdinalIgnoreCase) && !name.Equals("Tag", StringComparison.OrdinalIgnoreCase) && !name.Equals("Step", StringComparison.OrdinalIgnoreCase) && !name.Equals("DataGridBinding", StringComparison.OrdinalIgnoreCase) && !name.Equals("DataGridSummary", StringComparison.OrdinalIgnoreCase) && !name.Equals("DataGridRightFrozen", StringComparison.OrdinalIgnoreCase) && !name.Equals("DataGridFilters", StringComparison.OrdinalIgnoreCase) && !name.Equals("DataGridStage1", StringComparison.OrdinalIgnoreCase) && !name.Equals("DatePicker", StringComparison.OrdinalIgnoreCase) && !name.Equals("TimePicker", StringComparison.OrdinalIgnoreCase) && !name.Equals("ChartGallery", StringComparison.OrdinalIgnoreCase) && !name.Equals("PreviewHost", StringComparison.OrdinalIgnoreCase) && !name.Equals("NavMenu", StringComparison.OrdinalIgnoreCase) && !name.Equals("ReferenceChrome", StringComparison.OrdinalIgnoreCase) && !name.Equals("ControlVariants", StringComparison.OrdinalIgnoreCase) && !name.Equals("InfoCard", StringComparison.OrdinalIgnoreCase) && !name.Equals("LoginWindow", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"No isolated renderer is registered for '{name}'. Register its factory rather than exporting all controls.");
        Directory.CreateDirectory(output);
        if (controls.Contains("PreviewNavigation", StringComparer.OrdinalIgnoreCase)) NavigationChromePreview.Render(output);
        foreach (var name in controls.Where(name => new[] { "LoadingContainer", "ButtonLoading", "IconButtonLoading" }.Contains(name, StringComparer.OrdinalIgnoreCase))) LoadingPreview.Render(output, name);
        foreach (var name in controls.Where(name => new[] { "IconButton", "ButtonVariants", "Toast", "Dialog", "Notification" }.Contains(name, StringComparer.OrdinalIgnoreCase))) ButtonsMessagesPreview.Render(output, name);
        if (controls.Contains("DataGridBinding", StringComparer.OrdinalIgnoreCase)) DataGridBindingPreview.Render(output);
        if (controls.Contains("DataGridSummary", StringComparer.OrdinalIgnoreCase)) DataGridSummaryPreview.Render(output);
        if (controls.Contains("DataGridRightFrozen", StringComparer.OrdinalIgnoreCase)) DataGridRightFrozenPreview.Render(output);
        if (controls.Contains("DataGridFilters", StringComparer.OrdinalIgnoreCase)) DataGridFilterPreview.Render(output);
        if (controls.Contains("DataGridStage1", StringComparer.OrdinalIgnoreCase)) DataGridStage1Preview.Render(output);
        if (controls.Contains("ChartGallery", StringComparer.OrdinalIgnoreCase)) PreviewPagesPreview.RenderCharts(output);
        if (controls.Contains("PreviewHost", StringComparer.OrdinalIgnoreCase)) PreviewPagesPreview.Render(output);
        if (controls.Contains("NavMenu", StringComparer.OrdinalIgnoreCase)) { NavMenuPreview.Render(output); NavMenuEdgePreview.Render(output); NavMenuSearchPreview.Render(output); NavMenuMotionPreview.Render(output); NavMenuFilterPreview.Render(output); }
        TagStepPreview.Render(output, controls.Contains("Tag", StringComparer.OrdinalIgnoreCase), controls.Contains("Step", StringComparer.OrdinalIgnoreCase));
        if (controls.Contains("TabDrag", StringComparer.OrdinalIgnoreCase)) TabDragPreview.Render(output);
        if (controls.Contains("CardTabs", StringComparer.OrdinalIgnoreCase))
            foreach (var dark in new[] { false, true })
            foreach (var size in Enum.GetValues<ControlSize>()) CardTabPreview.Render(output, dark, size);
        if (controls.Contains("ReferenceChrome", StringComparer.OrdinalIgnoreCase)) ReferenceChromePreview.Render(output);
        if (controls.Contains("ControlVariants", StringComparer.OrdinalIgnoreCase)) ControlVariantsPreview.Render(output);
        if (controls.Contains("InfoCard", StringComparer.OrdinalIgnoreCase)) InfoCardPreview.Render(output);
        if (controls.Contains("LoginWindow", StringComparer.OrdinalIgnoreCase)) LoginWindowChecks.Render(output);
        DateTimePickerPreview.Render(output, controls.Where(name => name.Equals("DatePicker", StringComparison.OrdinalIgnoreCase) || name.Equals("TimePicker", StringComparison.OrdinalIgnoreCase)).Select(name => name.Equals("DatePicker", StringComparison.OrdinalIgnoreCase) ? "DatePicker" : "TimePicker").ToArray());
        foreach (var name in controls.Where(Samples.ContainsKey))
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var sample = Samples[name]();
            var window = new Window { Width = sample is ChartBase ? 600 : 480, Height = sample is SplitViewPage ? 880 : sample is ChartBase ? 440 : 360, RequestedThemeVariant = theme,
                Content = sample is SplitViewPage ? new Border { Margin = new Thickness(28), Child = sample }
                    : new StackPanel { Margin = new Thickness(28), Spacing = 24, Children = { new TextBlock { Text = name, FontSize = 22 }, sample } } };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
            if (sample is ChartBase natural && double.IsNaN(natural.Height)) { window.Height = natural.Bounds.Height + 120; ControlPreview.Flush(); }
            if (sample is ChartBase animated && name.Equals("AnimatedChart", StringComparison.OrdinalIgnoreCase))
            {
                Motion.SetEnabled(window, true); animated.SeriesSource![0].ItemsSource![2].Y = 100; ControlPreview.Flush(); ChartPreview.AdvanceAnimation(220);
            }
            if (sample is ChartBase shape && (name.Equals("PieHoverChart", StringComparison.OrdinalIgnoreCase) || name.Equals("DoughnutHoverChart", StringComparison.OrdinalIgnoreCase) || name.Equals("BarHoverChart", StringComparison.OrdinalIgnoreCase)) && shape.GetDataPointPosition(0, 2) is { } shapePoint)
            {
                window.MouseMove(shape.TranslatePoint(shapePoint, window)!.Value); ControlPreview.Flush();
            }
            if (sample is ChartBase chart && (name.Equals("CrosshairChart", StringComparison.OrdinalIgnoreCase) || name.Equals("SharedTooltipChart", StringComparison.OrdinalIgnoreCase)) && chart.GetDataPointPosition(0, 2) is { } hover)
            {
                if (name.Equals("SharedTooltipChart", StringComparison.OrdinalIgnoreCase)) { ToolTip.SetShouldUseOverlayLayer(chart, true); ToolTip.SetShowDelay(chart, 0); }
                window.MouseMove(chart.TranslatePoint(hover, window)!.Value); ControlPreview.Flush();
                if (name.Equals("SharedTooltipChart", StringComparison.OrdinalIgnoreCase)) { ToolTip.SetIsOpen(chart, true); ControlPreview.Flush(); }
            }
            using var bitmap = window.CaptureRenderedFrame()!;
            var path = Path.Combine(output, $"{name.ToLowerInvariant()}-{theme.Key}-{size}.png"); bitmap.Save(path, PngBitmapEncoderOptions.Default);
            Console.WriteLine($"Rendered {path}");
            if (sample is SplitViewPage splitPage)
            {
                splitPage.Model.Expanded = false; ControlPreview.Flush();
                using var closed = window.CaptureRenderedFrame()!;
                closed.Save(Path.Combine(output, $"splitview-{theme.Key}-{size}-closed.png"), PngBitmapEncoderOptions.Default);
            }
            window.Close(); ControlPreview.Flush();
        }
    }
}
