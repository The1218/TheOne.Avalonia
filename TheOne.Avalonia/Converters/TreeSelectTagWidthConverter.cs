using System.Globalization;
using Avalonia.Data.Converters;

namespace TheOne.Avalonia.Converters;

/// <summary>Reserve the tag margin inside the bounded selected-items viewport.</summary>
internal sealed class TreeSelectTagWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is double width && double.IsFinite(width) ? Math.Max(0, width - 4) : 0d;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
