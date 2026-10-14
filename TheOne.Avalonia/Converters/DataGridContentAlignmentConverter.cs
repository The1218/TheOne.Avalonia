using System.Globalization;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Data.Converters;
using global::Avalonia.Layout;

namespace TheOne.Avalonia.Converters;

/// <summary>Centered text uses the full cell width; controls retain their requested alignment.</summary>
public sealed class DataGridContentAlignmentConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || values[1] is not HorizontalAlignment alignment) return AvaloniaProperty.UnsetValue;
        return values[0] is TextBlock && alignment == HorizontalAlignment.Center ? HorizontalAlignment.Stretch : alignment;
    }
}
