using System.Diagnostics;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    // The tooltip and hit test change immediately. Only the paint for the old and new marks blends.
    private readonly Dictionary<(int Series, int Index), HoverMark> _hoverMarks = [];
    private readonly List<(int Series, int Index)> _finishedHoverMarks = [];
    private long _hoverStarted;
    private double _hoverDuration;
    private bool _hoverFramePending;
    private long _hoverFrameGeneration;
    private Action<TimeSpan>? _hoverFrame;

    private void SetHoverMarks(IReadOnlyList<ChartSelection> entries)
    {
        var count = Math.Min(12, entries.Count);
        if (_hoverMarks.Count == 0 && count == 0) return;
        var animate = _preparedAnimationMarks <= 6000 && CanAnimatePresentation();
        // Drop earlier exits on rapid sweeps. Keep at most one outgoing and one incoming group.
        foreach (var (key, mark) in _hoverMarks)
        {
            var stays = false;
            for (var i = 0; i < count; i++)
            {
                if (key != (entries[i].SeriesIndex, entries[i].PointIndex)) continue;
                stays = true;
                break;
            }
            if (!stays && (!animate || mark.Target == 0)) _finishedHoverMarks.Add(key);
            mark.From = mark.Amount;
            mark.Target = stays ? 1 : 0;
        }
        RemoveFinishedHoverMarks();

        for (var i = 0; i < count; i++)
        {
            var key = (entries[i].SeriesIndex, entries[i].PointIndex);
            if (!_hoverMarks.TryGetValue(key, out var mark)) _hoverMarks[key] = mark = new HoverMark();
            mark.Target = 1;
            if (!animate) mark.Amount = 1;
        }

        if (!animate || _hoverMarks.Count == 0)
        {
            _hoverDuration = 0;
            return;
        }
        _hoverStarted = Stopwatch.GetTimestamp();
        _hoverDuration = Math.Min(AnimationDuration.TotalMilliseconds, count == 0 ? 95 : 140);
        RequestHoverFrame();
    }

    private void RequestHoverFrame()
    {
        if (_hoverFramePending || TopLevel.GetTopLevel(this) is not { } top) return;
        _hoverFramePending = true;
        if (_hoverFrame is null)
        {
            var generation = _hoverFrameGeneration;
            _hoverFrame = _ =>
            {
                if (generation == _hoverFrameGeneration) AdvanceHoverFrame();
            };
        }
        top.RequestAnimationFrame(_hoverFrame);
    }

    private void AdvanceHoverFrame()
    {
        _hoverFramePending = false;
        if (_hoverMarks.Count == 0) return;
        var progress = CanAnimatePresentation() && _hoverDuration > 0
            ? Math.Min(1, Stopwatch.GetElapsedTime(_hoverStarted).TotalMilliseconds / _hoverDuration) : 1;
        var eased = 1 - Math.Pow(1 - progress, 3);
        foreach (var (key, mark) in _hoverMarks)
        {
            mark.Amount = Mix(mark.From, mark.Target, eased);
            if (progress == 1 && mark.Target == 0) _finishedHoverMarks.Add(key);
        }
        RemoveFinishedHoverMarks();
        InvalidateVisual();
        if (progress < 1) RequestHoverFrame();
    }

    private void RemoveFinishedHoverMarks()
    {
        foreach (var key in _finishedHoverMarks) _hoverMarks.Remove(key);
        _finishedHoverMarks.Clear();
    }

    private sealed class HoverMark
    {
        public double From, Target, Amount;
    }
}
