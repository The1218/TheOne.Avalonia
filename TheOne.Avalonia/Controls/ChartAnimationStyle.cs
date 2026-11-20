namespace TheOne.Avalonia.Controls;

/// <summary>Controls the presentation of chart data changes without changing target data or hit testing.</summary>
public enum ChartAnimationStyle
{
    /// <summary>Animate entry by chart type. Interpolate compatible updates or fade between data states.</summary>
    Auto,
    /// <summary>Present the complete target frame immediately.</summary>
    None,
    /// <summary>Fade in the prepared target plot while keeping its geometry cached.</summary>
    Fade,
    /// <summary>Reveal the prepared target plot from left to right.</summary>
    Reveal
}
