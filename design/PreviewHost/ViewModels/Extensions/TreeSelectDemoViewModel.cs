using System.Collections;
using System.Collections.ObjectModel;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class TreeSelectDepartment
{
    public string Id { get; init; } = "";
    public string Name { get; init; } = "";
    public bool Enabled { get; init; } = true;
    public ObservableCollection<TreeSelectDepartment> Teams { get; } = [];
}

public sealed class TreeSelectDemoViewModel : ReactiveObject
{
    public ObservableCollection<TreeSelectNode> Nodes { get; } =
    [
        new() { Key = "product", Text = "产品中心", Children =
        [
            new() { Key = "design", Text = "设计组" },
            new() { Key = "research", Text = "用户研究" }
        ] },
        new() { Key = "engineering", Text = "研发中心", Children =
        [
            new() { Key = "frontend", Text = "前端组" },
            new() { Key = "backend", Text = "后端组" },
            new() { Key = "locked", Text = "历史团队（禁选）", IsEnabled = false }
        ] }
    ];
    public object? SelectedKey { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "design";
    public IList IndependentKeys { get; set => this.RaiseAndSetIfChanged(ref field, value); } = new ObservableCollection<object> { "frontend", "locked" };
    public IList CascadeKeys { get; set => this.RaiseAndSetIfChanged(ref field, value); } = new ObservableCollection<object> { "design" };
    public IList CustomKeys { get; set => this.RaiseAndSetIfChanged(ref field, value); } = new ObservableCollection<object>();
    public ObservableCollection<TreeSelectDepartment> Departments { get; } =
    [
        new() { Id = "finance", Name = "财务部", Teams =
        { new() { Id = "accounting", Name = "会计组" }, new() { Id = "audit", Name = "审计组" } } }
    ];
    public TreeSelectItemAdapter Adapter { get; } = TreeSelectItemAdapter.Create<TreeSelectDepartment>(
        department => department.Id, department => department.Name, department => department.Teams,
        department => department.Enabled);
    public ObservableCollection<TreeSelectNode> LargeNodes { get; } = [];
    public void LoadLargeTree()
    {
        if (LargeNodes.Count > 0) return;
        var parent = new TreeSelectNode { Key = "large-root", Text = "一万个节点" };
        for (var i = 0; i < 10000; i++) parent.Children!.Add(new TreeSelectNode { Key = "item-" + i, Text = $"节点 {i:00000}" });
        LargeNodes.Add(parent);
    }
}
