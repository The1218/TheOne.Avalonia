using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace TheOne.Avalonia.Converters;

internal sealed class TagDashRadiusConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) =>
        values.Count >= 2 && values[0] is CornerRadius radius && values[1] is Rect bounds
            ? Math.Min(radius.TopLeft, Math.Min(bounds.Width, bounds.Height) / 2)
            : 0d;
}
