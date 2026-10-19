using System.Text;
using Avalonia;
using Avalonia.Media;
using Net.Codecrete.QrCodeGenerator;

namespace TheOne.Avalonia.Controls;

internal sealed record QRCodeSymbol(Geometry Geometry, int Size);

/// <summary>One encoder and vector drawing path for the control, report canvas and PDF.</summary>
internal static class QRCodeDrawing
{
    internal const int MaximumTextBytes = 7089;
    private static readonly Encoding Utf8 = new UTF8Encoding(false, true);

    internal static QRCodeSymbol Encode(string? text, QRCodeErrorCorrection correction)
    {
        if (string.IsNullOrEmpty(text)) throw new ArgumentException("二维码内容不能为空。");
        if (!Enum.IsDefined(correction)) throw new ArgumentException("二维码纠错等级无效。");
        try
        {
            if (text.Length > MaximumTextBytes || Utf8.GetByteCount(text) > MaximumTextBytes)
                throw new ArgumentException("二维码内容过长，请减少字符。");
            var level = correction switch
            {
                QRCodeErrorCorrection.Low => QrCode.Ecc.Low,
                QRCodeErrorCorrection.Quartile => QrCode.Ecc.Quartile,
                QRCodeErrorCorrection.High => QrCode.Ecc.High,
                _ => QrCode.Ecc.Medium
            };
            var code = QrCode.EncodeTextAdvanced(text, level, boostEcl: false, eci: ECI.UTF8,
                encoding: Utf8, kanjiStrategy: KanjiStrategy.Disabled);
            return new QRCodeSymbol(Geometry.Parse(code.ToGraphicsPath(0)), code.Size);
        }
        catch (EncoderFallbackException exception)
        {
            throw new ArgumentException("二维码内容包含无效的 Unicode 字符。", exception);
        }
        catch (DataTooLongException exception)
        {
            throw new ArgumentException("二维码内容超过当前纠错等级的容量，请减少字符或降低纠错等级。", exception);
        }
    }

    internal static void ValidateQuietZone(int quietZone)
    {
        if (quietZone is < 4 or > 32) throw new ArgumentException("二维码静区应为 4–32 个模块。");
    }

    internal static bool Fits(QRCodeSymbol symbol, Size size, int quietZone) =>
        Math.Min(size.Width, size.Height) / (symbol.Size + quietZone * 2) >= 1;

    internal static void Draw(DrawingContext context, QRCodeSymbol symbol, Rect area, int quietZone,
        IBrush? foreground, IBrush? background)
    {
        var size = Math.Min(area.Width, area.Height);
        if (size <= 0) return;
        var unit = size / (symbol.Size + quietZone * 2);
        var x = area.X + (area.Width - size) / 2 + quietZone * unit;
        var y = area.Y + (area.Height - size) / 2 + quietZone * unit;
        context.DrawRectangle(background, null, area);
        using (context.PushRenderOptions(new RenderOptions { EdgeMode = EdgeMode.Aliased }))
        using (context.PushTransform(new Matrix(unit, 0, 0, unit, x, y)))
            context.DrawGeometry(foreground, null, symbol.Geometry);
    }
}
