using System.Collections.ObjectModel;
using Avalonia.Media;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class NavEntry(string title, int? category = null, bool isGroup = false) : ReactiveObject
{
    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = title;

    public int? Category { get; } = category;
    public bool IsGroup { get; } = isGroup;
    public bool IsEnabled { get; set; } = true;

    public bool IsExpanded
    {
        get;
        set
        {
            if (field != value) this.RaiseAndSetIfChanged(ref field, value);
        }
    } = isGroup;

    public ObservableCollection<NavEntry> Entries { get; } = [];

    public Geometry Icon { get; init; } = Geometry.Parse(isGroup
        ? "M2,3 L7,3 L9,5 L16,5 L16,15 L2,15 Z"
        : "M3,3 L15,3 L15,15 L3,15 Z M3,7 L15,7 M7,7 L7,15");

    public override string ToString()
    {
        return Title;
    }
}

internal static partial class  PreviewNavigation
{
    public static ObservableCollection<NavEntry> Create()
    {
        NavEntry Group(string name, params (int Id, string Title)[] pages)
        {
            var shape = name switch
            {
                "基础控件" => "M2,2 H7 V7 H2 Z M11,2 H16 V7 H11 Z M2,11 H7 V16 H2 Z M11,11 H16 V16 H11 Z",
                "数据展示" => "M2,3 H16 V15 H2 Z M2,7 H16 M7,3 V15 M12,3 V15",
                "导航与布局" => "M2,4 H16 V14 H2 Z M7,4 V14",
                "图表控件" => "M3,15 V9 H6 V15 M8,15 V5 H11 V15 M13,15 V2 H16 V15 M1,16 H17",
                "扩展控件" => "M6,2 H12 V6 H16 V12 H12 V16 H6 V12 H2 V6 H6 Z",
                _ => "M3,9 L7,13 L15,4"
            };
            var group = new NavEntry(name, isGroup: true)
            {
                Icon = Geometry.Parse(shape)
            };
            foreach (var page in pages)
                group.Entries.Add(new NavEntry(page.Title, page.Id));
            return group;
        }

        return
        [
            new NavEntry("全部控件", 0),
            Group("基础控件", (1, "按钮与操作"), (2, "输入与校验"), (3, "选择与切换"), (4, "数值与进度"), (5, "日期与时间"), (12, "分裂按钮"),
                (13, "标签与可选文本")),
            Group("数据展示", (7, "树与菜单"), (10, "数据表格 DataGrid"), (18, "内容轮播 Carousel")),
            Group("导航与布局", (6, "导航与布局"), (11, "侧栏与分隔"), (17, "独立标签导航"), (20, "内容切换")), Group("图表控件", (23, "图表 Chart")),
            Group("扩展控件", (14, "内容弹层"), (19, "下拉刷新"), (21, "通知与消息"), (22, "窗口 OneWindow"), (24, "信息卡片 InfoCard"),
                (25, "导航菜单 NavMenu")),
            Group("交互验证", (8, "尺寸与交互"), (9, "大数据虚拟化"), (15, "选择控件状态"), (16, "按钮控件状态"))
        ];
    }
}