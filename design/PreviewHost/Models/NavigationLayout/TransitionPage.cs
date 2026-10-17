using ReactiveUI;

namespace TheOne.DesignPreview;

public abstract class TransitionPage(string title) : ReactiveObject
{
    public string Title { get; } = title;
}

public sealed class TransitionOverview() : TransitionPage("工作区概览");

public sealed class TransitionForm() : TransitionPage("编辑草稿")
{
    public string Draft
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "完善下一组基础控件";
}

public sealed record TransitionRecord(int Number, string Title);

public sealed class TransitionRecords() : TransitionPage("更新记录")
{
    public IReadOnlyList<TransitionRecord> Rows { get; } = Enumerable.Range(1, 1000)
        .Select(number => new TransitionRecord(number, $"工作区更新 {number:0000}")).ToArray();

    public TransitionRecord? SelectedRow
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}