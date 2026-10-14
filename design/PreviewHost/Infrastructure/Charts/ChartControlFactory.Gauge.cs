using Avalonia;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static partial class ChartControlFactory
{
    internal static string GaugeStyleTitle(GaugeStyle style) => style switch
    {
        GaugeStyle.Semicircle => "半圆进度 · 兼容默认", GaugeStyle.Basic => "基础仪表盘", GaugeStyle.Speed => "速度仪表盘",
        GaugeStyle.Progress => "进度仪表盘", GaugeStyle.StageSpeed => "分段速度仪表盘", GaugeStyle.Grade => "等级仪表盘",
        GaugeStyle.Temperature => "温度仪表盘", GaugeStyle.Barometer => "双刻度气压表", _ => style.ToString()
    };
    internal static void ConfigureGauge(GaugeChart chart, GaugeStyle style)
    {
        chart.GaugeStyle = style;
        chart.Value = 70; chart.ValueFormat = "0.##";
        if (style == GaugeStyle.Semicircle) chart.Unit = "%";
        if (style is GaugeStyle.Speed or GaugeStyle.StageSpeed) chart.Unit = "km/h";
        if (style == GaugeStyle.Basic) { chart.Value = 50; chart.GaugeCaption = "SCORE"; }
        if (style == GaugeStyle.Speed) { chart.Maximum = 240; chart.Value = 100; }
        if (style == GaugeStyle.Temperature)
        {
            chart.Maximum = 60; chart.Value = 35.78; chart.Unit = "°C";
            chart.AccentBrush = global::Avalonia.Media.Brush.Parse("#FFAB91");
            chart.SecondaryBrush = global::Avalonia.Media.Brush.Parse("#FD7347");
        }
        if (style is GaugeStyle.StageSpeed or GaugeStyle.Grade)
        {
            chart.GaugeSegments = style == GaugeStyle.Grade
                ? new[] { Band(0.25, "#FF6E76", "Grade D"), Band(0.5, "#FDDD60", "Grade C"), Band(0.75, "#58D9F9", "Grade B"), Band(1, "#7CFFB2", "Grade A") }
                : new[] { Band(0.3, "#67E0E3"), Band(0.7, "#37A2DA"), Band(1, "#FD666D") };
            if (style == GaugeStyle.Grade) chart.GaugeCaption = "Grade Rating";
        }
        if (style == GaugeStyle.Barometer)
        {
            chart.Value = 58.46; chart.GaugeCaption = "PLP";
            chart.GaugeTrackBrush = chart.GaugeTickBrush = global::Avalonia.Media.Brushes.Red;
            chart.Bind(GaugeChart.GaugeNeedleBrushProperty, chart.GetObservable(ChartBase.ForegroundProperty));
            chart.GaugeNeedleLength = 0.95;
        }
        static GaugeSegment Band(double fraction, string color, string? label = null) => new()
            { Fraction = fraction, Brush = global::Avalonia.Media.Brush.Parse(color), Label = label };
    }

}
