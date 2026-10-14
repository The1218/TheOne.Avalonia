using global::Avalonia;
using global::Avalonia.Media.Imaging;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public partial class ChartBase
{
    private bool _isExportSnapshot;
    private bool _skipExportBackground;
    // Resolve after all partial-class property initializers have completed.
    private static AvaloniaProperty[] ExportProperties =>
    [
        TitleProperty, SeriesSourceProperty, ShowLegendProperty, ShowGridProperty, ShowMarkersProperty, ShowDataLabelsProperty,
        StrokeThicknessProperty, InnerRadiusProperty, YMinimumProperty, YMaximumProperty, MinimumProperty, MaximumProperty, ValueProperty,
        ValueFormatProperty, UnitProperty, EmptyTextProperty, BackgroundProperty, ForegroundProperty, GridBrushProperty, MutedBrushProperty,
        AccentBrushProperty, SecondaryBrushProperty, TertiaryBrushProperty, QuaternaryBrushProperty, FifthBrushProperty, SixthBrushProperty, FontFamilyProperty, FontSizeProperty,
        FlowDirectionProperty, ViewportProperty, SecondaryYMinimumProperty, SecondaryYMaximumProperty, YAxisTitleProperty, SecondaryYAxisTitleProperty,
        SecondaryValueFormatProperty, SecondaryUnitProperty, ShowSecondaryAxisProperty, SamplingModeProperty, SamplingThresholdProperty,
        SamplingPointBudgetProperty, IsSmoothProperty, DefaultChartWidthProperty, DefaultChartHeightProperty, ChartPaddingProperty,
        MarkerRadiusProperty, SelectionThicknessProperty, SliceHoverOffsetProperty, SliceHoverScaleProperty, GaugeBandRatioProperty,
        ShowGaugeNeedleProperty, ShowGaugeTicksProperty, GaugeMajorTickCountProperty, GaugeMinorTickCountProperty
    ];

    /// <summary>Render a stable target-state PNG on the UI thread without altering this chart. Size is logical pixels; DPI scales the output.</summary>
    public void ExportPng(Stream stream, Size? size = null, double dpi = 96, bool transparentBackground = false)
    {
        Dispatcher.UIThread.VerifyAccess();
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite) throw new ArgumentException("The PNG destination must be writable.", nameof(stream));
        var logical = size ?? (Bounds.Width > 0 && Bounds.Height > 0 ? Bounds.Size : new Size(DefaultChartWidth, DefaultChartHeight));
        var pixels = ValidateExportSize(logical, dpi);
        var snapshot = CreateExportSnapshot();
        snapshot._isExportSnapshot = true; snapshot._skipExportBackground = transparentBackground; snapshot._legendPage = _legendPage;
        foreach (var property in ExportProperties) snapshot.SetValue(property, GetValue(property));
        snapshot.IsAnimationEnabled = false; snapshot.IsLegendInteractive = false; snapshot.IsEnabled = IsEffectivelyEnabled;
        snapshot.Width = logical.Width; snapshot.Height = logical.Height;
        snapshot.Measure(logical); snapshot.Arrange(new Rect(logical));
        using var bitmap = new RenderTargetBitmap(pixels, new Vector(dpi, dpi));
        bitmap.Render(snapshot);
        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
    }

    public void ExportPng(string path, Size? size = null, double dpi = 96, bool transparentBackground = false)
    {
        Dispatcher.UIThread.VerifyAccess();
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var logical = size ?? (Bounds.Width > 0 && Bounds.Height > 0 ? Bounds.Size : new Size(DefaultChartWidth, DefaultChartHeight));
        _ = ValidateExportSize(logical, dpi);
        using var stream = File.Create(path);
        ExportPng(stream, logical, dpi, transparentBackground);
    }

    private static PixelSize ValidateExportSize(Size size, double dpi)
    {
        if (!double.IsFinite(dpi) || dpi < 16 || dpi > 600) throw new ArgumentOutOfRangeException(nameof(dpi), "DPI must be between 16 and 600.");
        if (!double.IsFinite(size.Width) || !double.IsFinite(size.Height) || size.Width <= 0 || size.Height <= 0)
            throw new ArgumentOutOfRangeException(nameof(size), "Both logical dimensions must be finite and positive.");
        var width = Math.Max(1, Math.Ceiling(size.Width * dpi / 96)); var height = Math.Max(1, Math.Ceiling(size.Height * dpi / 96));
        if (width > 8192 || height > 8192 || width * height > 16_777_216)
            throw new ArgumentOutOfRangeException(nameof(size), "PNG output is limited to 8192 pixels per side and 16,777,216 pixels in total.");
        return new PixelSize((int)width, (int)height);
    }
}
