using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>Small display-only sources, without constructing or subscribing the interactive editor view model.</summary>
internal static class ChartPreviewData
{
    internal static IList<ChartPoint>[] CreatePoints()
    {
        double[][] values = [[42, 58, 51, 76, 68, 92], [34, 43, 49, 56, 61, 70]];
        return values.Select(series => (IList<ChartPoint>)series.Select((value, index) =>
            new ChartPoint(index, value, $"{index + 1}月")).ToArray()).ToArray();
    }

    internal static IList<ChartSeries> CreateSeries(IList<ChartPoint>[] points, bool single = false) =>
        points.Take(single ? 1 : points.Length).Select((source, index) => new ChartSeries
        {
            Name = index == 0 ? "本期" : "上期", ItemsSource = source
        }).ToArray();
}
