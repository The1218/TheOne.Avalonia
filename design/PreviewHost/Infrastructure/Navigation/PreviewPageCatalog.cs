using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed record PreviewPageDescriptor(int Id, string Title, string Group, IReadOnlyList<int>? ControlIds = null);

internal static class PreviewPageCatalog
{
    private static readonly PreviewPageDescriptor[] CombinedPages =
    [
        new(400, "按钮", "基础控件", [1, 101, 16, 12, 130, 305]),
        new(401, "输入 Input", "基础控件", [2, 4, 111]),
        new(402, "选择 Select", "基础控件", [110, 3, 15, 106])
    ];

    public static PreviewPageDescriptor Home { get; } = new(0, "Home 首页", "开始使用");

    public static IReadOnlyList<PreviewPageDescriptor> Controls { get; } =
    [
        new(1, "Button", "基础控件"),
        new(305, "IconButton", "基础控件"),
        new(331, "OneIcon", "基础控件"),
        new(332, "Breadcrumb", "导航与布局"),
        new(306, "LoadingContainer", "扩展控件"),
        new(308, "RelationGraph", "数据展示"),
        new(309, "GanttChart", "数据展示"),
        new(310, "ImagePreview", "数据展示"),
        new(315, "WorkflowEditor", "扩展控件"),
        new(317, "ColorPicker", "基础控件"),
        new(318, "ReportDesign", "扩展控件"),
        new(319, "ReportPreview", "扩展控件"),
        new(320, "MessageBox", "消息与对话框"),
        new(321, "Upload", "扩展控件"),
        new(322, "Form", "扩展控件"),
        new(323, "MultiSelectBox", "扩展控件"),
        new(330, "TreeSelect", "扩展控件"),
        new(324, "ThemeColorPicker", "基础控件"),
        new(325, "ThemeButton", "基础控件"),
        new(22, "OneWindow", "消息与对话框"),
        new(326, "LoginWindow", "消息与对话框"),
        new(327, "Description", "数据展示"),
        new(334, "Timeline", "数据展示"),
        new(328, "NavPage", "扩展控件"),
        new(329, "QRCode", "数据展示"),
        new(101, "ToggleButton", "基础控件"),
        new(16, "RepeatButton", "基础控件"),
        new(2, "TextBox", "基础控件"),
        new(3, "CheckBox", "基础控件"),
        new(15, "RadioButton", "基础控件"),
        new(106, "ToggleSwitch", "基础控件"),
        new(4, "NumericUpDown", "基础控件"),
        new(108, "Slider", "基础控件"),
        new(109, "ProgressBar", "基础控件"),
        new(110, "ComboBox", "基础控件"),
        new(111, "AutoCompleteBox", "基础控件"),
        new(5, "CalendarDatePicker", "基础控件"),
        new(333, "DateTimePicker", "扩展控件"),
        new(113, "Calendar", "基础控件"),
        new(114, "DatePicker", "基础控件"),
        new(115, "TimePicker", "基础控件"),
        new(13, "Label", "基础控件"),
        new(117, "SelectableTextBlock", "基础控件"),
        new(118, "ListBox", "数据展示"),
        new(9, "ItemsControl", "数据展示"),
        new(6, "TabControl", "导航与布局"),
        new(7, "TreeView", "数据展示"),
        new(122, "Menu", "导航与布局"),
        new(123, "ContextMenu", "导航与布局"),
        new(8, "Expander", "导航与布局"),
        new(125, "ScrollViewer", "导航与布局"),
        new(126, "ScrollBar", "导航与布局"),
        new(127, "Separator", "导航与布局"),
        new(128, "ToolTip", "导航与布局"),
        new(12, "SplitButton", "导航与布局"),
        new(130, "ToggleSplitButton", "导航与布局"),
        new(11, "SplitView", "导航与布局"),
        new(132, "GridSplitter", "导航与布局"),
        new(10, "DataGrid", "数据展示"),
        new(313, "DataGridTree", "数据展示"),
        new(14, "FlyoutPresenter", "扩展控件"),
        new(17, "TabStrip", "导航与布局"),
        new(18, "Carousel", "数据展示"),
        new(19, "RefreshContainer", "扩展控件"),
        new(20, "TransitioningContentControl", "导航与布局"),
        new(24, "InfoCard", "扩展控件"),
        new(25, "NavMenu", "扩展控件"),
        new(26, "Tag", "扩展控件"),
        new(311, "Avatar", "扩展控件"),
        new(27, "VerificationStep", "扩展控件"),
        new(307, "FlowStep", "扩展控件"),
        new(312, "Pagination", "扩展控件"),
        new(300, "Toast", "消息与对话框"),
        new(301, "Dialog", "消息与对话框"),
        new(302, "OneNotification", "消息与对话框"),
        new(23, "LineChart", "图表控件"),
        new(201, "AreaChart", "图表控件"),
        new(202, "BarChart", "图表控件"),
        new(203, "HorizontalBarChart", "图表控件"),
        new(204, "ScatterChart", "图表控件"),
        new(205, "PieChart", "图表控件"),
        new(206, "DoughnutChart", "图表控件"),
        new(207, "GaugeChart", "图表控件"),
        new(208, "StackedBarChart", "图表控件"),
        new(209, "StackedHorizontalBarChart", "图表控件"),
        new(210, "StackedAreaChart", "图表控件"),
        new(211, "PercentStackedBarChart", "图表控件"),
        new(212, "PercentStackedHorizontalBarChart", "图表控件"),
        new(213, "PercentStackedAreaChart", "图表控件")
    ];

    public static IReadOnlyList<PreviewPageDescriptor> Pages { get; } = new[] { Home }.Concat(CombinedPages)
        .Concat(Controls.Where(control => !CombinedPages.Any(page => page.ControlIds!.Contains(control.Id)))).ToArray();

    public static IReadOnlyList<PreviewPageDescriptor> SearchEntries { get; } = new[] { Home }.Concat(CombinedPages)
        .Concat(Controls.Select(control => control with { Group = SearchGroup(control.Id) })).ToArray();

    private static string SearchGroup(int id)
    {
        var page = Find(id);
        return page.ControlIds is null ? page.Group : $"{page.Group} / {page.Title}";
    }

    public static PreviewPageDescriptor Find(int id)
    {
        return Pages.First(page => page.Id == id || page.ControlIds?.Contains(id) == true);
    }

    public static UserControl Create(int id)
    {
        var page = Find(id);
        return page.ControlIds is null ? CreateControl(page.Id) : new ShowcasePage(page);
    }

    internal static UserControl CreateControl(int id)
    {
        return id switch
        {
            0 => new HomePage(),
            324 => new ThemeControlDemoView(false),
            325 => new ThemeControlDemoView(true),
            22 => new WindowControlDemoView(false),
            326 => new WindowControlDemoView(true),
            327 => new DescriptionDemoView(),
            334 => new TimelineDemoView(),
            328 => new NavPageDemoView(),
            329 => new QRCodeDemoView(),
            1 => new ButtonPage(),
            305 => new IconButtonPage(),
            331 => new OneIconDemoView(),
            332 => new BreadcrumbDemoView(),
            306 => new LoadingContainerPage(),
            308 => new RelationGraphDemoView(),
            309 => new GanttDemoView(),
            310 => new ImagePreviewDemoView(),
            315 => new WorkflowEditorDemoView(),
            317 => new ColorPickerPage(),
            318 => ReportDesignDemo.CreateDesignerSample(),
            319 => PreviewPrinting.CreatePreviewSample(),
            320 => new MessageBoxDemoView(),
            321 => new UploadDemoView(),
            322 => new FormDemoView(),
            323 => new MultiSelectBoxDemoView(),
            330 => new TreeSelectDemoView(),
            101 => new ToggleButtonPage(),
            16 => new RepeatButtonPage(),
            2 => new TextBoxPage(),
            3 => new CheckBoxPage(),
            15 => new RadioButtonPage(),
            106 => new ToggleSwitchPage(),
            4 => new NumericUpDownPage(),
            108 => new SliderPage(),
            109 => new ProgressBarPage(),
            110 => new ComboBoxPage(),
            111 => new AutoCompleteBoxPage(),
            5 => new CalendarDatePickerPage(),
            333 => new DateTimePickerDemoView(),
            113 => new CalendarPage(),
            114 => new DatePickerPage(),
            115 => new TimePickerPage(),
            13 => new LabelPage(),
            117 => new SelectableTextBlockPage(),
            118 => new ListBoxPage(),
            9 => new ItemsControlPage(),
            6 => new TabControlPage(),
            7 => new TreeViewPage(),
            122 => new MenuPage(),
            123 => new ContextMenuPage(),
            8 => new ExpanderPage(),
            125 => new ScrollViewerPage(),
            126 => new ScrollBarPage(),
            127 => new SeparatorPage(),
            128 => new ToolTipPage(),
            12 => new SplitButtonPage(),
            130 => new ToggleSplitButtonPage(),
            11 => new SplitViewPage(),
            132 => new GridSplitterPage(),
            10 => new DataGridDemoView { Height = 660 },
            313 => new DataGridTreeDemoView { Height = 660 },
            14 => new FlyoutDemoView(),
            17 => new TabStripDemoView(),
            18 => new CarouselDemoView(),
            19 => new RefreshDemoView(),
            20 => new ContentTransitionDemoView(),
            24 => new InfoCardDemoView(),
            26 => new TagDemoView(),
            311 => new AvatarDemoView(),
            27 => new VerificationStepDemoView(),
            307 => new FlowStepDemoView(),
            312 => new PaginationDemoView(),
            25 => new NavMenuDemoView(),
            300 => new ToastPage(),
            301 => new DialogPage(),
            302 => new OneNotificationPage(),
            23 => new ChartShowcaseView(ChartKind.Line),
            201 => new ChartShowcaseView(ChartKind.Area),
            202 => new ChartShowcaseView(ChartKind.Bar),
            203 => new ChartShowcaseView(ChartKind.HorizontalBar),
            204 => new ChartShowcaseView(ChartKind.Scatter),
            205 => new ChartShowcaseView(ChartKind.Pie),
            206 => new ChartShowcaseView(ChartKind.Doughnut),
            207 => new ChartShowcaseView(ChartKind.Gauge),
            208 => new ChartShowcaseView(ChartKind.StackedBar),
            209 => new ChartShowcaseView(ChartKind.StackedHorizontalBar),
            210 => new ChartShowcaseView(ChartKind.StackedArea),
            211 => new ChartShowcaseView(ChartKind.PercentStackedBar),
            212 => new ChartShowcaseView(ChartKind.PercentStackedHorizontalBar),
            213 => new ChartShowcaseView(ChartKind.PercentStackedArea),
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };
    }
}
