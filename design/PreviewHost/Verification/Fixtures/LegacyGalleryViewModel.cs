using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class LegacyGalleryViewModel : ReactiveObject, INotifyDataErrorInfo
{
    private int _saveCount;
    public LegacyGalleryViewModel()
    {
        SaveCommand = ReactiveCommand.Create<object?>(_ => Status = $"已保存第 {++_saveCount} 次 · {Email}");
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            Email = "hello@theone.dev";
            Progress = 64;
            Status = "预览数据已重置";
        });
        HelpCommand = ReactiveCommand.Create<object?>(_ => Status = "提示：Tab 切换焦点，空格切换选择，方向键调整数值。");
        MenuCommand = ReactiveCommand.Create<object?>(parameter => Status = $"菜单命令：{parameter}");
        InspectNodeCommand = ReactiveCommand.Create<object?>(_ => Status = $"当前节点：{SelectedTreeNode?.Name ?? "未选择"}");
        RenameNodeCommand = ReactiveCommand.Create<object?>(_ =>
        {
            if (SelectedTreeNode is not { IsEnabled: true } node)
                return;
            node.Name = node.Name.EndsWith(" · 示例") ? node.Name[..^5] : node.Name + " · 示例";
            this.RaisePropertyChanged(nameof(SelectedNodeName));
            Status = $"已重命名演示节点：{node.Name}";
        });
        ExpandTreeCommand = ReactiveCommand.Create<object?>(_ => SetTreeExpansion(true));
        CollapseTreeCommand = ReactiveCommand.Create<object?>(_ => SetTreeExpansion(false));
        SelectedTreeNode = TreeRoots[0].Children[0].Children[0];
        AddTabCommand = ReactiveCommand.Create<object?>(_ => DemoTabs.Add(new DemoTab($"新标签 {++_tabCount}", "标签页来自 ObservableCollection，排序和关闭会同步修改集合。")));
        ResetTabsCommand = ReactiveCommand.Create<object?>(_ => ResetTabs());
        ReferenceTabsCommand = ReactiveCommand.Create<object?>(_ => LoadReferenceTabs());
        ResetTabs();
    }

    public bool NavigationCollapsed
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ObservableCollection<NavEntry> NavigationRoots { get; } = PreviewNavigation.Create();

    public NavEntry? SelectedNavigation
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(NavigationTitle));
        }
    }

    public string NavigationTitle => "控件库 / " + (SelectedNavigation?.Title ?? "全部控件");

    public string Email
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(EmailError));
            this.RaisePropertyChanged(nameof(HasErrors));
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Email)));
        }
    } = "hello@theone.dev";

    public string EmailError => Email.Contains('@') && Email.Contains('.') ? "" : "请输入有效的电子邮箱地址";
    public bool HasErrors => EmailError.Length > 0;

    public IEnumerable GetErrors(string? propertyName) => (propertyName is null or nameof(Email)) && HasErrors ? new[]
    {
        EmailError
    }

    : Array.Empty<string>();
    public double Progress
    {
        get;
        set
        {
            var progress = Math.Clamp(value, 0, 100);
            if (field == progress)
                return;
            this.RaiseAndSetIfChanged(ref field, progress);
        }
    } = 64;

    public bool DemoEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "准备就绪 · 点击、输入或使用 Tab 检查控件";

    private int _tabCount;
    public ObservableCollection<DemoTab> DemoTabs { get; } = [];
    public ICommand AddTabCommand { get; }
    public ICommand ResetTabsCommand { get; }
    public ICommand ReferenceTabsCommand { get; }

    private void LoadReferenceTabs()
    {
        DemoTabs.Clear();
        foreach (var title in new[]
        {
            "Kestrel Sport Sedan",
            "Meridian GT",
            "Longhaul Estate",
            "Trailcut 200",
            "Velocia Coupe",
            "Regent Executive",
            "Nimbus Crossover",
            "Foreman Crew Cab",
            "Dune Runner",
            "Cityhop Compact",
            "Auralis Grand Tourer",
            "Trailhead Wagon"
        }

        )
            DemoTabs.Add(new DemoTab(title, $"{title}：选中标题与内容面板连续连接。使用右侧箭头或滚轮查看溢出标签；支持拖动排序与关闭。"));
    }

    private void ResetTabs()
    {
        DemoTabs.Clear();
        DemoTabs.Add(new DemoTab("欢迎", "固定标签：可排序，不能关闭。", false));
        DemoTabs.Add(new DemoTab("主题", "浅色与深色共享语义色，选中卡片与内容面板使用同一个表面。"));
        DemoTabs.Add(new DemoTab("交互", "拖动标题调整顺序；点击 × 关闭；Ctrl / ⌘ + W 关闭选中项。"));
        _tabCount = 0;
    }

    public string[] Suggestions { get; } = ["Avalonia", "TheOne", "Still", "Skia", "Windows", "macOS", "Linux"];
    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand HelpCommand { get; }
    public ICommand MenuCommand { get; }
    public ICommand InspectNodeCommand { get; }
    public ICommand RenameNodeCommand { get; }
    public ICommand ExpandTreeCommand { get; }
    public ICommand CollapseTreeCommand { get; }
    public PreviewTreeNode[] TreeRoots { get; } = [new("TheOne.Avalonia", "控件库项目", true, new("Themes", "浅色、深色和控件模板", true, new("Colors.axaml", "语义颜色与画刷"), new("TreeViewItem.axaml", "树节点模板"), new("MenuItem.axaml", "菜单项目模板")), new("Controls", "基础控件", true, new("Button", "按钮与命令"), new("TreeView", "树形数据与选择")), new("只读节点", "禁用状态示例", false)), new("PreviewHost", "可操作的预览应用", true, new PreviewTreeNode("ControlGallery.axaml", "控件目录"))];

    public PreviewTreeNode? SelectedTreeNode
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(SelectedNodeName));
            this.RaisePropertyChanged(nameof(SelectedNodeDescription));
        }
    }

    public string SelectedNodeName => SelectedTreeNode?.Name ?? "未选择节点";
    public string SelectedNodeDescription => SelectedTreeNode?.Description ?? "选择节点查看说明";

    public bool ShowTreeDetails
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    private void SetTreeExpansion(bool expanded)
    {
        void Visit(PreviewTreeNode node)
        {
            node.IsExpanded = expanded && node.Children.Length > 0;
            foreach (var child in node.Children)
                Visit(child);
        }

        foreach (var root in TreeRoots)
            Visit(root);
        Status = expanded ? "已展开所有演示节点" : "已折叠所有演示节点";
    }

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
}

public sealed record DemoTab(string Title, string Description, bool CanClose = true);
