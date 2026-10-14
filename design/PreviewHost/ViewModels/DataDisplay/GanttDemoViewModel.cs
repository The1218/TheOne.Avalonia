using System.Collections.ObjectModel;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class GanttDemoViewModel : ReactiveObject
{
    public GanttDemoViewModel()
    {
        Filter();
        SelectedTask = Tasks.First(t => t.Id == "timeline");
    }

    public ObservableCollection<GanttTask> Tasks { get; } = CreateTasks();

    public IEnumerable<GanttTask> Items
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    public IReadOnlyList<string> Owners { get; } = ["全部负责人", "林悦", "陈舟", "周宁", "苏禾", "许安"];

    public string SearchText
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            Filter();
        }
    } = "";

    public string SelectedOwner
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            Filter();
        }
    } = "全部负责人";

    public GanttTask? SelectedTask
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public double DayWidth
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 48;

    public DateOnly ViewStart
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(2026, 10, 5);

    public DateOnly ViewEnd
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = new(2026, 10, 26);

    public DateOnly Today { get; } = new(2026, 10, 14);

    public bool LargeData
    {
        get;
        set
        {
            this.RaiseAndSetIfChanged(ref field, value);
            while (Tasks.Count > 10) Tasks.RemoveAt(Tasks.Count - 1);
            if (value)
            {
                Tasks.Add(new GanttTask { Id = "backlog", Title = "后续迭代", Owner = "研发组" });
                for (var i = 0; i < 1500; i++)
                    Tasks.Add(new GanttTask
                    {
                        Id = $"extra-{i}", ParentId = "backlog", Title = $"迭代任务 {i + 1:N0}", Owner = "周宁",
                        Start = new DateOnly(2026, 10, 5).AddDays(i % 17),
                        EndExclusive = new DateOnly(2026, 10, 8).AddDays(i % 17), Progress = i % 11 / 10.0
                    });
            }

            Filter();
        }
    }

    private void Filter()
    {
        if (string.IsNullOrWhiteSpace(SearchText) && SelectedOwner == "全部负责人")
        {
            Items = Tasks;
            return;
        }

        var included = new HashSet<string>(StringComparer.Ordinal);
        var lookup = Tasks.ToDictionary(t => t.Id);
        var children = Tasks.ToLookup(t => t.ParentId);
        foreach (var task in Tasks.Where(t =>
                     (string.IsNullOrWhiteSpace(SearchText) ||
                      t.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) &&
                     (SelectedOwner == "全部负责人" || t.Owner == SelectedOwner)))
        {
            var stack = new Stack<GanttTask>();
            stack.Push(task);
            while (stack.TryPop(out var descendant))
            {
                included.Add(descendant.Id);
                foreach (var child in children[descendant.Id]) stack.Push(child);
            }

            var parentId = task.ParentId;
            while (parentId is not null && lookup.TryGetValue(parentId, out var parent))
            {
                included.Add(parent.Id);
                parentId = parent.ParentId;
            }
        }

        Items = Tasks.Where(t => included.Contains(t.Id)).ToArray();
    }

    public static ObservableCollection<GanttTask> CreateTasks()
    {
        return
        [
            new GanttTask { Id = "design", Title = "需求与设计", Owner = "产品组" },
            new GanttTask
            {
                Id = "requirements", ParentId = "design", Title = "需求梳理", Owner = "林悦",
                Start = new DateOnly(2026, 10, 5),
                EndExclusive = new DateOnly(2026, 10, 8), Progress = 1
            },
            new GanttTask
            {
                Id = "interaction", ParentId = "design", Title = "交互设计", Owner = "陈舟",
                Start = new DateOnly(2026, 10, 8),
                EndExclusive = new DateOnly(2026, 10, 13), Progress = 1
            },
            new GanttTask { Id = "development", Title = "控件开发", Owner = "研发组" },
            new GanttTask
            {
                Id = "timeline", ParentId = "development", Title = "时间轴绘制", Owner = "周宁",
                Start = new DateOnly(2026, 10, 12),
                EndExclusive = new DateOnly(2026, 10, 17), Progress = .6
            },
            new GanttTask
            {
                Id = "data", ParentId = "development", Title = "数据与交互", Owner = "苏禾",
                Start = new DateOnly(2026, 10, 12),
                EndExclusive = new DateOnly(2026, 10, 20), Progress = .35
            },
            new GanttTask
            {
                Id = "integration", ParentId = "development", Title = "集成联调", Owner = "周宁",
                Start = new DateOnly(2026, 10, 19),
                EndExclusive = new DateOnly(2026, 10, 22)
            },
            new GanttTask { Id = "delivery", Title = "验证与交付", Owner = "质量组" },
            new GanttTask
            {
                Id = "verification", ParentId = "delivery", Title = "回归验证", Owner = "许安",
                Start = new DateOnly(2026, 10, 22),
                EndExclusive = new DateOnly(2026, 10, 24)
            },
            new GanttTask
            {
                Id = "acceptance", ParentId = "delivery", Title = "版本验收", Owner = "林悦",
                Start = new DateOnly(2026, 10, 23),
                IsMilestone = true
            }
        ];
    }
}