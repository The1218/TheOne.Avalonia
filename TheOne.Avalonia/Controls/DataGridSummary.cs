using System.Globalization;
using global::Avalonia.Interactivity;

namespace TheOne.Avalonia.Controls;

public enum DataGridSummaryKind { Sum, Average, Min, Max, Count }
public enum DataGridSummaryScope { FilteredItems, SourceItems }
public sealed record DataGridSummaryResult(object? Value, long ValidCount, string? Error = null);
public sealed class DataGridSummaryUpdatedEventArgs : RoutedEventArgs
{
    public DataGridSummaryUpdatedEventArgs() : base(DataGridFeatures.SummaryUpdatedEvent) { }
}

internal sealed class DataGridSummaryAccumulator(Type type, DataGridSummaryKind kind)
{
    private readonly Type _type = Nullable.GetUnderlyingType(type) ?? type;
    private readonly bool _floating = (Nullable.GetUnderlyingType(type) ?? type) == typeof(float) || (Nullable.GetUnderlyingType(type) ?? type) == typeof(double);
    private long _count;
    private decimal _decimalSum;
    private double _doubleSum;
    private object? _minimum, _maximum;
    private string? _error;
    public void Fail(string error) => _error ??= error;
    public void Add(object? value)
    {
        if (value is null) return;
        if (value.GetType() != _type) { Fail("字段值与声明的数值类型不匹配。"); return; }
        if (value is double d && !double.IsFinite(d) || value is float f && !float.IsFinite(f)) return;
        try
        {
            _count = checked(_count + 1);
            if (kind == DataGridSummaryKind.Count) return;
            if (kind == DataGridSummaryKind.Min)
            {
                if (_minimum is null || ((IComparable)value).CompareTo(_minimum) < 0) _minimum = value;
            }
            else if (kind == DataGridSummaryKind.Max)
            {
                if (_maximum is null || ((IComparable)value).CompareTo(_maximum) > 0) _maximum = value;
            }
            else if (_error is null)
            {
                if (_floating)
                {
                    _doubleSum += Convert.ToDouble(value, CultureInfo.InvariantCulture);
                    if (!double.IsFinite(_doubleSum)) Fail("统计累加超出有限数值范围。");
                }
                else _decimalSum = checked(_decimalSum + Convert.ToDecimal(value, CultureInfo.InvariantCulture));
            }
        }
        catch (OverflowException) { Fail("统计累加溢出。"); }
        catch (ArgumentException exception) { Fail(exception.Message); }
    }
    public DataGridSummaryResult Result()
    {
        if (_error is not null) return new(null, _count, _error);
        if (kind == DataGridSummaryKind.Count) return new(_count, _count);
        if (_count == 0) return new(null, 0);
        object? value = kind switch
        {
            DataGridSummaryKind.Min => _minimum,
            DataGridSummaryKind.Max => _maximum,
            DataGridSummaryKind.Average => _floating ? (object)(_doubleSum / _count) : _decimalSum / _count,
            _ => _floating ? (object)_doubleSum : _decimalSum
        };
        return new(value, _count);
    }
}
