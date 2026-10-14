using System.Globalization;
using System.Numerics;

namespace TheOne.Avalonia.Controls;

internal static class DataGridFilterParsing
{
    public static DataGridFilterKind Kind(IDataGridValueAccessor? accessor, DataGridFilterKind configured)
    {
        if (accessor is null || configured == DataGridFilterKind.None) return DataGridFilterKind.None;
        var type = Nullable.GetUnderlyingType(accessor.ValueType) ?? accessor.ValueType;
        var actual = type == typeof(string) ? DataGridFilterKind.Text : IsNumber(type) ? DataGridFilterKind.Number : DataGridFilterKind.None;
        return configured == DataGridFilterKind.Auto || configured == actual ? actual : DataGridFilterKind.None;
    }
    private static bool IsNumber(Type type) => !type.IsEnum && Type.GetTypeCode(type) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or TypeCode.UInt32 or
        TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;

    public static DataGridFilterCondition? Number(Type valueType, NumericFilterMode mode, string value, string minimum, string maximum, CultureInfo culture)
    {
        var type = Nullable.GetUnderlyingType(valueType) ?? valueType;
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Byte => Parse<byte>(mode, value, minimum, maximum, culture),
            TypeCode.SByte => Parse<sbyte>(mode, value, minimum, maximum, culture),
            TypeCode.Int16 => Parse<short>(mode, value, minimum, maximum, culture),
            TypeCode.UInt16 => Parse<ushort>(mode, value, minimum, maximum, culture),
            TypeCode.Int32 => Parse<int>(mode, value, minimum, maximum, culture),
            TypeCode.UInt32 => Parse<uint>(mode, value, minimum, maximum, culture),
            TypeCode.Int64 => Parse<long>(mode, value, minimum, maximum, culture),
            TypeCode.UInt64 => Parse<ulong>(mode, value, minimum, maximum, culture),
            TypeCode.Single => Parse<float>(mode, value, minimum, maximum, culture),
            TypeCode.Double => Parse<double>(mode, value, minimum, maximum, culture),
            TypeCode.Decimal => Parse<decimal>(mode, value, minimum, maximum, culture),
            _ => throw new ArgumentException("该字段不是受支持的数值类型。")
        };
    }
    private static DataGridFilterCondition? Parse<T>(NumericFilterMode mode, string exact, string minimum, string maximum, CultureInfo culture) where T : struct, INumber<T>
    {
        T? Read(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            if (!T.TryParse(text, culture, out var result) || !T.IsFinite(result)) throw new ArgumentException("请输入有效的有限数值。");
            return result;
        }
        var condition = mode == NumericFilterMode.Exact ? new NumericFilterCondition<T>(mode, value: Read(exact)) :
            new NumericFilterCondition<T>(mode, minimum: Read(minimum), maximum: Read(maximum));
        return condition.IsEmpty ? null : condition;
    }
}
