using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

internal sealed class ReportColorConverter : IValueConverter
{
    public static ReportColorConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Color.TryParse(value as string, out var color) ? color : Colors.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is Color color
            ? color.A == 255 ? $"#{color.R:X2}{color.G:X2}{color.B:X2}" :
            color.A == 0 ? "Transparent" : color.ToString()
            : "Transparent";
    }
}