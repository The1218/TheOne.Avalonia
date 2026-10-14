using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class RelationUnit : ReactiveObject
{
    public string? Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string? Description
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public Geometry? Symbol
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ObservableCollection<RelationUnit>? Branches
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];
}

public sealed record RelationGraphSlotSample(string Label, RelationGraphNode Node);

public sealed class RelationGraphDemoViewModel : ReactiveObject
{
    public RelationGraphDemoViewModel()
    {
        AddChildCommand = ReactiveCommand.Create(() =>
            Root.Children!.Add(new RelationGraphNode { Icon = CheckIcon, Title = "新增节点", Content = "集合变化自动更新" }));
        RemoveChildCommand = ReactiveCommand.Create(() =>
        {
            if (Root.Children!.Count > 3) Root.Children.RemoveAt(Root.Children.Count - 1);
        });
    }

    public static Geometry CenterIcon { get; } = StreamGeometry.Parse(
        "M2,2 L9,2 L9,9 L2,9 Z M13,2 L20,2 L20,9 L13,9 Z M2,13 L9,13 L9,20 L2,20 Z M13,13 L20,13 L20,20 L13,20 Z");

    public static Geometry DesignIcon { get; } =
        StreamGeometry.Parse("M3,16 L3,20 L7,20 L20,7 L16,3 Z M13,6 L17,10 L15,12 L11,8 Z");

    public static Geometry CodeIcon { get; } =
        StreamGeometry.Parse("M7,4 L0,11 L7,18 L9,16 L4,11 L9,6 Z M15,4 L13,6 L18,11 L13,16 L15,18 L22,11 Z");

    public static Geometry CheckIcon { get; } = StreamGeometry.Parse("M0,11 L3,8 L8,13 L18,3 L21,6 L8,19 Z");

    public RelationGraphNode Root { get; } = CreateTree();

    public RelationUnit CustomRoot { get; } = new()
    {
        Name = "业务模型", Description = "Branches → 子节点", Symbol = CenterIcon,
        Branches =
        [
            new RelationUnit { Name = "组织单元", Description = "Name → 标题", Symbol = DesignIcon },
            new RelationUnit { Name = "项目单元", Description = "Description → 内容", Symbol = CodeIcon }
        ]
    };

    public IReadOnlyList<Orientation> Orientations { get; } = Enum.GetValues<Orientation>();

    public Orientation Direction
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = Orientation.Horizontal;

    public double CardWidth
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 260;

    public double SiblingSpacing
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 36;

    public double DepthSpacing
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 80;

    public bool ShowIcon
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            Root.Icon = value ? CenterIcon : null;
        }
    } = true;

    public bool ShowTitle
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            Root.Title = value ? "研发中心" : null;
        }
    } = true;

    public bool ShowContent
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            Root.Content = value ? "产品与技术协同" : null;
        }
    } = true;

    public ICommand AddChildCommand { get; }
    public ICommand RemoveChildCommand { get; }

    public IReadOnlyList<RelationGraphSlotSample> SlotSamples { get; } =
    [
        new("完整节点", new RelationGraphNode { Icon = CenterIcon, Title = "研发中心", Content = "产品与技术协同" }),
        new("Icon = null", new RelationGraphNode { Title = "控件开发", Content = "原生行为与性能" }),
        new("Title = null", new RelationGraphNode { Icon = CodeIcon, Content = "内容竖向居中" }),
        new("Content = null", new RelationGraphNode { Icon = DesignIcon, Title = "标题竖向居中" }),
        new("仅标题", new RelationGraphNode { Title = "标题占据整个文本区域" }),
        new("仅内容", new RelationGraphNode { Content = "内容占据整个文本区域" }),
        new("仅图标", new RelationGraphNode { Icon = CheckIcon }),
        new("全部为 null", new RelationGraphNode())
    ];

    public static RelationGraphNode CreateTree()
    {
        return new RelationGraphNode
        {
            Icon = CenterIcon, Title = "研发中心", Content = "产品与技术协同",
            Children = new ObservableCollection<RelationGraphNode>
            {
                new()
                {
                    Icon = DesignIcon, Title = "产品设计", Content = "体验与视觉规范", Children =
                        new ObservableCollection<RelationGraphNode>
                        {
                            new() { Icon = DesignIcon, Title = "交互设计", Content = "流程与交互规范" },
                            new() { Icon = DesignIcon, Title = "视觉设计", Content = "主题与组件规范" }
                        }
                },
                new()
                {
                    Icon = CodeIcon, Title = "应用开发", Content = "桌面与跨平台应用", Children =
                        new ObservableCollection<RelationGraphNode>
                        {
                            new() { Icon = CodeIcon, Title = "控件开发", Content = "原生行为与性能" }
                        }
                },
                new()
                {
                    Icon = CheckIcon, Title = "质量保障", Content = "测试与发布验证", Children =
                        new ObservableCollection<RelationGraphNode>
                        {
                            new() { Icon = CheckIcon, Title = "自动化测试", Content = "功能与绘制验证" }
                        }
                }
            }
        };
    }
}