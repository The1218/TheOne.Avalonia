using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class FlyoutDemoViewModel : ReactiveObject
{
    public IReadOnlyList<string> Rows { get; } = Enumerable.Range(1, 10000).Select(index => $"成员 {index:D5} · Still workspace").ToArray();
    public int Placement { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public int Size { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public int ShowMode { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public int SelectedIndex { get; set => this.RaiseAndSetIfChanged(ref field, value); } = -1;
    public bool PreventDismiss { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool Notify { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "TheOne workspace";
    public string AppliedName { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "TheOne workspace";
    public string Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "尚未应用更改";
    public string Metrics { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "打开弹层以查看当前主题与尺寸。";
    public string ListMetrics { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "等待列表打开";
    public string MemberStatus { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "尚未选择成员";
    public int ApplyCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }

    public bool Apply()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Status = "名称不能为空";
            return false;
        }

        AppliedName = Name.Trim();
        Status = $"已应用 {++ApplyCount} 次 · {AppliedName} · {(Notify ? "通知成员" : "不通知")}";
        return true;
    }

    public void Reset()
    {
        PreventDismiss = false;
        Placement = Size = ShowMode = 0;
        AppliedName = Name = "TheOne workspace";
        ApplyCount = 0;
        Notify = true;
        SelectedIndex = -1;
        Status = "尚未应用更改";
        MemberStatus = "尚未选择成员";
    }
}
