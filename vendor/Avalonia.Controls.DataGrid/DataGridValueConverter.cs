// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

// TheOne extension; see LICENSE.md.

using System;
using System.Globalization;
using Avalonia.Controls.Utils;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia.Controls;

// Built-in scalar conversions use known APIs. Custom domain types supply a column converter.
internal sealed class DataGridValueConverter : IValueConverter
{
    public static readonly DataGridValueConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ConvertValue(value, targetType, culture);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ConvertValue(value, targetType, culture);
    }

    private static object? ConvertValue(object? value, Type targetType, CultureInfo culture)
    {
        if (value == null)
            return targetType.IsValueType && !targetType.IsNullableType() ? BindingOperations.DoNothing : null;
        if (targetType.IsInstanceOfType(value)) return value;
        if (targetType == typeof(string))
            return value is IFormattable formatted ? formatted.ToString(null, culture) : value.ToString();
        if (value is string { Length: 0 } && targetType.IsNullableType()) return null;
        var type = targetType.GetNonNullableType();
        try
        {
            if (value is string text)
            {
                if (type == typeof(Guid)) return Guid.Parse(text);
                if (type == typeof(TimeSpan)) return TimeSpan.Parse(text, culture);
                if (type == typeof(DateOnly)) return DateOnly.Parse(text, culture);
                if (type == typeof(TimeOnly)) return TimeOnly.Parse(text, culture);
                if (type == typeof(DateTimeOffset)) return DateTimeOffset.Parse(text, culture);
            }

            if (value is IConvertible && (type == typeof(bool) || type == typeof(char) || type == typeof(byte) ||
                                          type == typeof(sbyte)
                                          || type == typeof(short) || type == typeof(ushort) || type == typeof(int) ||
                                          type == typeof(uint)
                                          || type == typeof(long) || type == typeof(ulong) || type == typeof(float) ||
                                          type == typeof(double)
                                          || type == typeof(decimal) || type == typeof(DateTime)))
                return System.Convert.ChangeType(value, type, culture);
            return new BindingNotification(
                new InvalidCastException($"Supply a DataGrid column converter for '{type}'."), BindingErrorType.Error);
        }
        catch (Exception error) when (error is FormatException or OverflowException or InvalidCastException)
        {
            return new BindingNotification(error, BindingErrorType.Error);
        }
    }
}