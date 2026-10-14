using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;
using ReactiveUI;

namespace TheOne.DesignPreview;

public enum NavMenuDemoAction
{
    Open,
    Close,
    Select,
    EdgeWindow
}

public sealed record NavMenuDemoRequest(NavMenuDemoAction Action, int[] Path);
public sealed class NavMenuDemoViewModel : ReactiveObject
{
    public bool ShowFilter { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public string Search { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public ICommand ToggleBranchCommand { get; }
    public ICommand ToggleNestedBranchCommand { get; }
    public ICommand ReverseSidebarCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand SearchDeepCommand { get; }
    public ICommand SearchAllChildrenCommand { get; }

    private int _reverseVersion;
    public bool OpenOnHover { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public bool IsCollapsed { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public ObservableCollection<NavEntry> Roots { get; } = [];
    public int Invocations { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public NavEntry? Selected { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public string Metrics { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "按视口创建容器";
    public string InvocationStatus { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "点击菜单或使用 Enter 调用";
    public decimal? EdgeWidth { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 420;
    public bool EdgeRight { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public ICommand OpenCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand JumpRootCommand { get; }
    public ICommand JumpChildCommand { get; }
    public ICommand JumpDeepCommand { get; }
    public ICommand CollapseCommand { get; }
    public ICommand InvokeCommand { get; }
    public ICommand EdgeCommand { get; }

    public event EventHandler<NavMenuDemoRequest>? Requested;
    private void Request(NavMenuDemoAction action, params int[] path) => Requested?.Invoke(this, new(action, path));
    public NavMenuDemoViewModel()
    {
        ToggleBranchCommand = ReactiveCommand.Create<object?>(_ => Roots[0].IsExpanded = !Roots[0].IsExpanded);
        ToggleNestedBranchCommand = ReactiveCommand.Create<object?>(_ =>
        {
            Roots[0].IsExpanded = true;
            Roots[0].Entries[0].IsExpanded = !Roots[0].Entries[0].IsExpanded;
        });
        ReverseSidebarCommand = ReactiveCommand.CreateFromTask<object?>(async _ =>
        {
            var version = ++_reverseVersion;
            IsCollapsed = !IsCollapsed;
            await Task.Delay(70);
            if (version == _reverseVersion)
                IsCollapsed = !IsCollapsed;
        });
        ClearSearchCommand = ReactiveCommand.Create<object?>(_ => Search = "");
        SearchDeepCommand = ReactiveCommand.Create<object?>(_ => Search = "三级页面 1000");
        SearchAllChildrenCommand = ReactiveCommand.Create<object?>(_ => Search = "三级页面");
        OpenCommand = ReactiveCommand.Create<object?>(_ =>
        {
            IsCollapsed = true;
            Request(NavMenuDemoAction.Open, 0);
        });
        CloseCommand = ReactiveCommand.Create<object?>(_ => Request(NavMenuDemoAction.Close));
        JumpRootCommand = ReactiveCommand.Create<object?>(_ => Request(NavMenuDemoAction.Select, 999));
        JumpChildCommand = ReactiveCommand.Create<object?>(_ => Request(NavMenuDemoAction.Select, 0, 999));
        JumpDeepCommand = ReactiveCommand.Create<object?>(_ => Request(NavMenuDemoAction.Select, 0, 0, 999));
        EdgeCommand = ReactiveCommand.Create<object?>(_ => Request(NavMenuDemoAction.EdgeWindow));
        CollapseCommand = ReactiveCommand.Create<object?>(_ =>
        {
            foreach (var root in Roots)
            {
                root.IsExpanded = false;
                foreach (var child in root.Entries)
                    child.IsExpanded = false;
            }
        });
        InvokeCommand = ReactiveCommand.Create<object?>(item =>
        {
            if (item is NavEntry entry)
                InvocationStatus = $"已调用：{entry.Title}（{++Invocations} 次）";
        });
        for (var i = 0; i < 1000; i++)
            Roots.Add(new NavEntry(i == 0 ? "产品管理" : $"根菜单 {i + 1:0000}"));
        for (var i = 0; i < 1000; i++)
            Roots[0].Entries.Add(new NavEntry(i == 0 ? "产品分类" : $"二级菜单 {i + 1:0000}"));
        for (var i = 0; i < 1000; i++)
            Roots[0].Entries[0].Entries.Add(new NavEntry($"三级页面 {i + 1:0000}"));
        Roots[1].Title = "系统设置";
        Roots[0].Entries[1].Title = "标签管理";
        for (var i = 0; i < 1000; i++)
            Roots[1].Entries.Add(new NavEntry($"设置页面 {i + 1:0000}"));
        for (var i = 0; i < 1000; i++)
            Roots[0].Entries[1].Entries.Add(new NavEntry($"标签页面 {i + 1:0000}"));
        Roots[0].IsExpanded = true;
        Roots[0].Entries[8].IsEnabled = false;
    }
}
