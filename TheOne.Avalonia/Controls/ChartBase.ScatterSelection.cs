using System.Collections;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    // An immutable splice keeps the selected original in source order without copying the base sample.
    private sealed class ScatterSelectedSample : IReadOnlyList<ChartRenderPoint>
    {
        public IReadOnlyList<ChartRenderPoint> Baseline { get; }
        public ChartRenderPoint Selected { get; }
        private readonly int _insertion;

        public ScatterSelectedSample(IReadOnlyList<ChartRenderPoint> baseline, ChartRenderPoint selected)
        {
            Baseline = baseline; Selected = selected;
            var low = 0; var high = baseline.Count;
            while (low < high)
            {
                var middle = low + (high - low) / 2;
                if (baseline[middle].Index < selected.Index) low = middle + 1;
                else high = middle;
            }
            _insertion = low;
        }

        public int Count => Baseline.Count + 1;
        public ChartRenderPoint this[int index] => (uint)index >= (uint)Count
            ? throw new ArgumentOutOfRangeException(nameof(index))
            : index == _insertion ? Selected : Baseline[index < _insertion ? index : index - 1];

        public IEnumerator<ChartRenderPoint> GetEnumerator()
        {
            for (var i = 0; i < Count; i++) yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
