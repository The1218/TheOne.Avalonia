using System.Globalization;
using global::Avalonia;
using global::Avalonia.Data.Converters;

namespace TheOne.Avalonia.Controls;

/// <summary>Keeps the collapsed height inclusive of the surrounding frame.</summary>
public sealed class ExpanderHeaderHeightConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        => values.Count == 2 && values[0] is double height && values[1] is Thickness border
            ? Math.Max(0, height - border.Top - border.Bottom)
            : AvaloniaProperty.UnsetValue;
}
