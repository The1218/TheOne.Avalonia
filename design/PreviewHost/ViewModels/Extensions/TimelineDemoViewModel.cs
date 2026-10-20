using System.Collections.ObjectModel;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class TimelineDemoViewModel : ReactiveObject
{
    public ObservableCollection<TimelineItemModel> Events { get; } = [];
    public TimelineMode[] Modes { get; } = Enum.GetValues<TimelineMode>();
    public TimelineMode Mode { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool Reverse { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public TimelineDemoViewModel() => Reset();
    public void Reset()
    {
        Events.Clear();
        Events.Add(new() { Key = "submitted", Header = "提交申请", Content = "采购申请已提交，等待审批。", Timestamp = "09:00", Status = TimelineStatus.Success });
        Events.Add(new() { Key = "review", Header = "预算复核", Content = "补充成本中心后重新提交。记录可包含多行说明，随内容自然增高。", Timestamp = "09:15", Status = TimelineStatus.Warning });
        Events.Add(new() { Key = "pending", Header = "等待主管审批", Content = "由宿主更新业务状态；控件不执行审批。", Timestamp = "09:30", Status = TimelineStatus.Pending });
    }
    public void Add() => Events.Add(new() { Key = Guid.NewGuid().ToString("N"), Header = "新增事件", Content = "ObservableCollection 增量追加。", Timestamp = DateTime.Now.ToString("HH:mm:ss"), Status = TimelineStatus.Default });
}
