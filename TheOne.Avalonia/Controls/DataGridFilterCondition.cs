using System.Globalization;
using System.Numerics;

namespace TheOne.Avalonia.Controls;

public enum DataGridFilterKind
{
    Auto,
    Text,
    Number,
    None
}

public enum NumericFilterMode
{
    Exact,
    Range
}

public interface IDataGridValueAccessor
{
    Type ValueType { get; }
    object? GetValue(object item);
}

public static class DataGridValueAccessor
{
    public static IDataGridValueAccessor Create<TItem, TValue>(Func<TItem, TValue> getter)
    {
        ArgumentNullException.ThrowIfNull(getter);
        return new Accessor<TItem, TValue>(getter);
    }

    private sealed class Accessor<TItem, TValue>(Func<TItem, TValue> getter) : IDataGridValueAccessor
    {
        public Type ValueType => typeof(TValue);

        public object? GetValue(object item)
        {
            return getter(item is TItem row ? row : throw new ArgumentException("数据项与字段 accessor 的行类型不匹配。"));
        }
    }
}

public abstract record DataGridFilterCondition
{
    public abstract bool IsEmpty { get; }
    public abstract bool Matches(object? value);
    internal abstract bool Supports(Type valueType);

    internal virtual DataGridFilterDraft Draft(CultureInfo culture)
    {
        return new DataGridFilterDraft(NumericFilterMode.Exact, "", "", "");
    }
}

public sealed record TextFilterCondition : DataGridFilterCondition
{
    public TextFilterCondition(string? text, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        if (!Enum.IsDefined(comparison)) throw new ArgumentOutOfRangeException(nameof(comparison));
        Text = text?.Trim() ?? "";
        Comparison = comparison;
    }

    public string Text { get; }
    public StringComparison Comparison { get; }
    public override bool IsEmpty => Text.Length == 0;

    public override bool Matches(object? value)
    {
        return IsEmpty || value is string text && text.Contains(Text, Comparison);
    }

    internal override bool Supports(Type valueType)
    {
        return valueType == typeof(string);
    }
}

public sealed record NumericFilterCondition<TNumber> : DataGridFilterCondition where TNumber : struct, INumber<TNumber>
{
    public NumericFilterCondition(NumericFilterMode mode, TNumber? value = null, TNumber? minimum = null,
        TNumber? maximum = null)
    {
        if (!Enum.IsDefined(mode)) throw new ArgumentOutOfRangeException(nameof(mode));
        if ((value is { } v && !TNumber.IsFinite(v)) || (minimum is { } lo && !TNumber.IsFinite(lo)) ||
            (maximum is { } hi && !TNumber.IsFinite(hi)))
            throw new ArgumentException("筛选值必须是有限数值。");
        if (mode == NumericFilterMode.Range && minimum is { } lower && maximum is { } upper && lower > upper)
            throw new ArgumentException("下限不能大于上限。");
        Mode = mode;
        Value = value;
        Minimum = minimum;
        Maximum = maximum;
    }

    public NumericFilterMode Mode { get; }
    public TNumber? Value { get; }
    public TNumber? Minimum { get; }
    public TNumber? Maximum { get; }

    public override bool IsEmpty =>
        Mode == NumericFilterMode.Exact ? Value is null : Minimum is null && Maximum is null;

    public override bool Matches(object? value)
    {
        if (IsEmpty) return true;
        if (value is not TNumber number || !TNumber.IsFinite(number)) return false;
        return Mode == NumericFilterMode.Exact
            ? number == Value!.Value
            : (Minimum is not { } lower || number >= lower) && (Maximum is not { } upper || number <= upper);
    }

    internal override bool Supports(Type valueType)
    {
        return (Nullable.GetUnderlyingType(valueType) ?? valueType) == typeof(TNumber);
    }

    internal override DataGridFilterDraft Draft(CultureInfo culture)
    {
        return new DataGridFilterDraft(Mode,
            Value?.ToString(null, culture) ?? "", Minimum?.ToString(null, culture) ?? "",
            Maximum?.ToString(null, culture) ?? "");
    }
}

internal sealed record DataGridFilterDraft(NumericFilterMode Mode, string Value, string Minimum, string Maximum);