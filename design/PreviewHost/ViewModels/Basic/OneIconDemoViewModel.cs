using System.Collections.ObjectModel;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed record OneIconDemoGroup(string Title, IReadOnlyList<OneIconInfo> Icons);
public sealed record OneIconDemoRow(string? Title, IReadOnlyList<OneIconInfo> Icons)
{
    public bool HasTitle => Title is not null;
}
public sealed record OneIconCategoryChoice(OneIconCategory? Category, string Title);

public sealed class OneIconDemoViewModel : ReactiveObject
{
    public IReadOnlyList<OneIconInfo> Icons => OneIconCatalog.All;
    public string CatalogSummary => $"{Categories.Count - 1} 类、{Icons.Count} 个矢量图标。点击图标查看尺寸、变体及控件搭配；颜色继承宿主，三档尺寸见下方对照。";
    public IReadOnlyList<OneIconVariant> Variants { get; } = Enum.GetValues<OneIconVariant>();
    public IReadOnlyList<double> Sizes { get; } = new double[] { 16, 20, 24, 32 };
    public IReadOnlyList<OneIconCategoryChoice> Categories { get; } =
    [
        new(null, "全部图标"), new(OneIconCategory.Actions, "常用操作"), new(OneIconCategory.Navigation, "方向导航"),
        new(OneIconCategory.Files, "文件内容"), new(OneIconCategory.People, "用户权限"), new(OneIconCategory.Feedback, "消息反馈"),
        new(OneIconCategory.Data, "数据图表"), new(OneIconCategory.DateTime, "日期时间"), new(OneIconCategory.Layout, "界面布局"),
        new(OneIconCategory.Devices, "系统设备"), new(OneIconCategory.Development, "开发网络"),
        new(OneIconCategory.Workflow, "流程审批"), new(OneIconCategory.Design, "编辑设计"),
        new(OneIconCategory.Finance, "金融财务"), new(OneIconCategory.Industry, "工业制造"),
        new(OneIconCategory.Medical, "医疗健康"), new(OneIconCategory.Infrastructure, "基础设施"),
        new(OneIconCategory.Transport, "交通物流"), new(OneIconCategory.Commerce, "商业购物"),
        new(OneIconCategory.Education, "教育学习"), new(OneIconCategory.Weather, "天气环境")
    ];
    public ObservableCollection<OneIconDemoGroup> Groups { get; } = [];
    private int _columns;
    public IReadOnlyList<OneIconDemoRow> Rows
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];
    public string SearchText
    {
        get;
        set
        {
            value ??= "";
            if (field == value) return;
            this.RaiseAndSetIfChanged(ref field, value);
            Filter();
        }
    } = "";
    public OneIconCategoryChoice? SelectedCategory
    {
        get;
        set
        {
            if (Equals(field, value)) return;
            this.RaiseAndSetIfChanged(ref field, value);
            Filter();
        }
    }
    public OneIconInfo SelectedIcon
    {
        get;
        set { this.RaiseAndSetIfChanged(ref field, value); this.RaisePropertyChanged(nameof(VariantHint)); }
    } = OneIconCatalog.Get(OneIconKind.Search);
    public OneIconVariant Variant
    {
        get;
        set { this.RaiseAndSetIfChanged(ref field, value); this.RaisePropertyChanged(nameof(VariantHint)); }
    }
    public double PreviewSize { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 20;
    public string ResultsText { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public bool IsEmpty { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public string VariantHint => SelectedIcon.HasFilledVariant ? "此图标提供独立实心轮廓。" : "此图标未提供实心轮廓，Filled 保持线性。";

    public OneIconDemoViewModel() { SelectedCategory = Categories[0]; }

    internal void SetGalleryWidth(double width)
    {
        if (!double.IsFinite(width) || width <= 0) return;
        var columns = Math.Max(1, (int)(width / 104));
        if (_columns == columns) return;
        _columns = columns;
        BuildRows();
    }

    private void BuildRows()
    {
        if (_columns == 0) return;
        var rows = new List<OneIconDemoRow>();
        foreach (var group in Groups)
        for (var i = 0; i < group.Icons.Count; i += _columns)
            rows.Add(new OneIconDemoRow(i == 0 ? group.Title : null, group.Icons.Skip(i).Take(_columns).ToArray()));
        Rows = rows;
    }

    private void Filter()
    {
        var query = SearchText.Trim();
        var matches = Icons.Where(icon => (SelectedCategory?.Category is null || icon.Category == SelectedCategory.Category) &&
            (icon.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || icon.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase))).ToArray();
        Groups.Clear();
        foreach (var group in matches.GroupBy(icon => icon.Category))
            Groups.Add(new OneIconDemoGroup(Categories.First(category => category.Category == group.Key).Title, group.ToArray()));
        ResultsText = $"{matches.Length} / {Icons.Count} 个图标 · {Groups.Count} 个分类";
        IsEmpty = matches.Length == 0;
        BuildRows();
    }
}
