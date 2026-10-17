using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

/// <summary>The calculated display values for a selected leaf, milestone or summary task.</summary>
public sealed record GanttSelection(
    GanttTask Task,
    DateOnly Start,
    DateOnly EndExclusive,
    double Progress,
    bool IsSummary)
{
    public DateOnly DisplayEnd => EndExclusive > Start ? EndExclusive.AddDays(-1) : Start;
}

/// <summary>An all-day task. Non-milestone leaf tasks use [Start, EndExclusive).</summary>
public class GanttTask : INotifyPropertyChanged
{
    public string Id
    {
        get;
        set => Set(ref field, value);
    } = Guid.NewGuid().ToString("N");

    public string? ParentId
    {
        get;
        set => Set(ref field, value);
    }

    public string Title
    {
        get;
        set => Set(ref field, value);
    } = "";

    public string? Owner
    {
        get;
        set => Set(ref field, value);
    }

    public DateOnly Start
    {
        get;
        set => Set(ref field, value);
    } = DateOnly.FromDateTime(DateTime.Today);

    public DateOnly EndExclusive
    {
        get;
        set => Set(ref field, value);
    } = DateOnly.FromDateTime(DateTime.Today).AddDays(1);

    /// <summary>Completion fraction in [0, 1]. Summary values are computed from descendants.</summary>
    public double Progress
    {
        get;
        set => Set(ref field, value);
    }

    public bool IsMilestone
    {
        get;
        set => Set(ref field, value);
    }

    public bool IsExpanded
    {
        get;
        set => Set(ref field, value);
    } = true;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}