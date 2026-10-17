using Avalonia.Controls;
using Avalonia.Media;

namespace TheOne.Avalonia.Styling;

/// <summary>A compact, hue-ordered default palette for the native color editor.</summary>
internal sealed class ColorPickerPalette : IColorPalette
{
    private static readonly uint[] Hues =
        [0xE5484D, 0xEF8B28, 0xE5B72E, 0x8DB636, 0x32A574, 0x006061, 0x4285D4, 0x7464C9, 0xB05BB8];

    private static readonly byte[] Neutrals = [255, 218, 163, 100, 30];
    private static readonly double[] Shades = [.75, .4, 0, -.22, -.45];
    public int ColorCount => 10;
    public int ShadeCount => 5;

    public Color GetColor(int colorIndex, int shadeIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(colorIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(colorIndex, ColorCount);
        ArgumentOutOfRangeException.ThrowIfNegative(shadeIndex);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(shadeIndex, ShadeCount);
        if (colorIndex == 0) return Color.FromRgb(Neutrals[shadeIndex], Neutrals[shadeIndex], Neutrals[shadeIndex]);
        var hue = Hues[colorIndex - 1];
        var amount = Shades[shadeIndex];

        byte Shade(uint channel)
        {
            return (byte)Math.Round(amount >= 0 ? channel + (255 - channel) * amount : channel * (1 + amount));
        }

        return Color.FromRgb(Shade((hue >> 16) & 255), Shade((hue >> 8) & 255), Shade(hue & 255));
    }
}