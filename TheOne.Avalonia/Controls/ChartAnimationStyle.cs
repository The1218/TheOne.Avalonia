namespace TheOne.Avalonia.Controls;

/// <summary>Controls the presentation of chart data changes without changing target data or hit testing.</summary>
public enum ChartAnimationStyle
{
    /// <summary>Reveal the first frame; interpolate compatible data changes, otherwise crossfade.</summary>
    Auto,
    /// <summary>Present the complete target frame immediately.</summary>
    None,
    /// <summary>Fade in the prepared target plot while keeping its geometry cached.</summary>
    Fade,
    /// <summary>Reveal the prepared target plot from left to right.</summary>
    Reveal
}
