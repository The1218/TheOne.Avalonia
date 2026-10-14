using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class FlowApprovalContent : ReactiveObject
{
    public FlowApprovalContent()
    {
        DetailsCommand = ReactiveCommand.Create<object?>(_ =>
        {
            DetailsCount++;
            Details = $"已查看申请详情 · {DetailsCount} 次";
        });
    }

    public string Title => "负责人审批";
    public string Time => "10-14 10:00";
    public string Approver => "陈明 · 部门负责人";
    public string Text => "申请资料已核验，等待审批意见。";
    public string FileName => "采购申请.pdf";

    public string Details
    {
        get;
        set
        {
            if (field == value) return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(HasDetails));
        }
    } = "";

    public bool HasDetails => !string.IsNullOrEmpty(Details);
    public ICommand DetailsCommand { get; }
    public int DetailsCount { get; private set; }
}

public sealed class FlowStepDemoViewModel : ReactiveObject
{
    private bool _longText;

    public FlowStepDemoViewModel()
    {
        ResetCommand = ReactiveCommand.Create<object?>(_ => Reset());
        AdvanceCommand = ReactiveCommand.Create<object?>(_ =>
        {
            var current = Nodes.FirstOrDefault(node => node.Status == FlowNodeStatus.InProgress);
            if (current is not null) current.Status = FlowNodeStatus.Completed;
            var pending = Nodes.FirstOrDefault(node => node.Status == FlowNodeStatus.Pending);
            if (pending is not null) pending.Status = FlowNodeStatus.InProgress;
            Status = pending is null ? "流程已全部完成" : "进行中节点已完成，后续节点开始处理";
        });
        ToggleTextCommand = ReactiveCommand.Create<object?>(_ =>
        {
            _longText = !_longText;
            if (Nodes.FirstOrDefault()?.Content is FlowNodeContent content)
                content.Text = _longText ? "申请资料已提交。这是一段较长的节点说明，用于检查窄视口下的自动换行、节点自然高度和连接线位置。补充材料可在审批完成之前上传。" : "申请资料已提交";
        });
        AddCommand = ReactiveCommand.Create<object?>(_ => Nodes.Add(new FlowNodeItem
            { Content = new FlowNodeContent { Title = "归档记录", Text = "流程结束后归档" } }));
        RemoveCommand = ReactiveCommand.Create<object?>(_ =>
        {
            if (Nodes.Count > 0) Nodes.RemoveAt(Nodes.Count - 1);
        });
        Reset();
    }

    public ObservableCollection<FlowNodeItem> Nodes { get; } = [];
    public FlowApprovalContent Approval { get; } = new();
    public ICommand ResetCommand { get; }
    public ICommand AdvanceCommand { get; }
    public ICommand ToggleTextCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "业务状态驱动节点 · 修改集合与内容后自动更新";

    public void Reset()
    {
        Nodes.Clear();
        _longText = false;
        Nodes.Add(Item("提交申请", "10-14 09:20", "申请资料已提交", FlowNodeStatus.Completed));
        Nodes.Add(Item("资料审核", "10-14 09:45", "资料完整，审核通过", FlowNodeStatus.Completed));
        Nodes.Add(Item("负责人审批", "10-14 10:00", "等待负责人确认", FlowNodeStatus.InProgress));
        Nodes.Add(Item("流程结束", null, "审批通过后自动结束", FlowNodeStatus.Pending));
        Status = "业务状态驱动节点 · 修改集合与内容后自动更新";
    }

    private static FlowNodeItem Item(string title, string? time, string text, FlowNodeStatus status)
    {
        return new FlowNodeItem
            { Status = status, Content = new FlowNodeContent { Title = title, Time = time, Text = text } };
    }
}