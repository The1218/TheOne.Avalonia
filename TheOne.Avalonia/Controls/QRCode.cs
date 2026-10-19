using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>The four QR Code Model 2 error correction levels.</summary>
public enum QRCodeErrorCorrection { Low, Medium, Quartile, High }

/// <summary>A vector QR code with a square symbol and a protected quiet zone.</summary>
public sealed class QRCode : Control
{
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<QRCode, string?>(nameof(Text), "");
    public static readonly StyledProperty<QRCodeErrorCorrection> ErrorCorrectionProperty =
        AvaloniaProperty.Register<QRCode, QRCodeErrorCorrection>(nameof(ErrorCorrection), QRCodeErrorCorrection.Medium,
            validate: value => Enum.IsDefined(value));
    public static readonly StyledProperty<int> QuietZoneProperty =
        AvaloniaProperty.Register<QRCode, int>(nameof(QuietZone), 4, validate: value => value is >= 4 and <= 32);
    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<QRCode, IBrush?>(nameof(Foreground), Brushes.Black);
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<QRCode, IBrush?>(nameof(Background), Brushes.White);
    public static readonly DirectProperty<QRCode, string?> ValidationErrorProperty =
        AvaloniaProperty.RegisterDirect<QRCode, string?>(nameof(ValidationError), control => control.ValidationError);

    private QRCodeSymbol? _symbol;
    private string? _encodingError = "二维码内容不能为空。";
    private string? _validationError = "二维码内容不能为空。";

    static QRCode() => AffectsRender<QRCode>(ForegroundProperty, BackgroundProperty, QuietZoneProperty);

    /// <summary>Payload, preserved exactly, encoded as UTF-8 with an ECI designator.</summary>
    public string? Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    /// <summary>Defaults to Medium. Changes regenerate the cached symbol.</summary>
    public QRCodeErrorCorrection ErrorCorrection { get => GetValue(ErrorCorrectionProperty); set => SetValue(ErrorCorrectionProperty, value); }
    /// <summary>Clear border on every side, in modules. Range 4–32; default 4.</summary>
    public int QuietZone { get => GetValue(QuietZoneProperty); set => SetValue(QuietZoneProperty, value); }
    /// <summary>Dark modules. Defaults to black independently of the application theme.</summary>
    public IBrush? Foreground { get => GetValue(ForegroundProperty); set => SetValue(ForegroundProperty, value); }
    /// <summary>Light modules and quiet zone. Defaults to white independently of the application theme.</summary>
    public IBrush? Background { get => GetValue(BackgroundProperty); set => SetValue(BackgroundProperty, value); }
    /// <summary>Read-only encoding or minimum-module-size error; null when the current symbol is valid.</summary>
    public string? ValidationError => _validationError;

    protected override Size MeasureOverride(Size availableSize) => new(160, 160);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || change.Property == ErrorCorrectionProperty)
        {
            _symbol = null;
            try { _symbol = QRCodeDrawing.Encode(Text, ErrorCorrection); _encodingError = null; }
            catch (ArgumentException exception) { _encodingError = exception.Message; }
            UpdateValidation();
            InvalidateVisual();
        }
        else if (change.Property == BoundsProperty || change.Property == QuietZoneProperty) UpdateValidation();
    }

    private void UpdateValidation()
    {
        var error = _encodingError;
        if (error is null && _symbol is { } symbol && Bounds.Width > 0 && Bounds.Height > 0 &&
            !QRCodeDrawing.Fits(symbol, Bounds.Size, QuietZone))
            error = "二维码尺寸不足，每个模块至少需要 1 DIP（含静区）。";
        SetAndRaise(ValidationErrorProperty, ref _validationError, error);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (_symbol is { } symbol && ValidationError is null)
            QRCodeDrawing.Draw(context, symbol, new Rect(Bounds.Size), QuietZone, Foreground, Background);
        else context.DrawRectangle(Background, null, new Rect(Bounds.Size));
    }
}
