namespace TheOne.Avalonia.Reporting;

public sealed record ReportFieldInfo(string Key, string Label, ReportValueKind Kind)
{
    public string TypeLabel => Kind switch { ReportValueKind.Number => "123", ReportValueKind.Date => "日期", _ => "Aa" };
}

/// <summary>Register the actual row accessor at compile time, including any nested property access.</summary>
public sealed record ReportField<T>(
    string Key,
    string Label,
    Func<T, object?> Getter,
    ReportValueKind Kind = ReportValueKind.Text);

/// <summary>A row snapshot with explicitly typed accessors. Replace the source to refresh changed data.</summary>
public sealed class ReportDataSource
{
    private readonly Func<int, string, object?> _getValue;
    private readonly IReadOnlyDictionary<string, object?>? _rootValues;

    private ReportDataSource(string name, int count, IReadOnlyList<ReportFieldInfo> fields,
        Func<int, string, object?> getValue, IReadOnlyDictionary<string, object?>? rootValues = null)
    {
        Name = name;
        RowCount = count;
        Fields = fields;
        _getValue = getValue;
        _rootValues = rootValues;
    }

    public string Name { get; }
    public int RowCount { get; }
    public IReadOnlyList<ReportFieldInfo> Fields { get; }

    public object? GetValue(int rowIndex, string fieldKey)
    {
        if (rowIndex < 0 || (rowIndex >= RowCount && !(rowIndex == 0 && _rootValues?.ContainsKey(fieldKey) == true)))
            throw new ArgumentOutOfRangeException(nameof(rowIndex));
        if (_rootValues?.TryGetValue(fieldKey, out var rootValue) == true) return rootValue;
        return _getValue(rowIndex, fieldKey);
    }

    internal ReportDataSource WithRootValues(IReadOnlyList<ReportFieldInfo> fields,
        IReadOnlyDictionary<string, object?> values)
    {
        return new ReportDataSource(Name, RowCount,
            Array.AsReadOnly(Fields.Where(field => !ReportJsonData.IsRootField(field.Key)).Concat(fields).ToArray()),
            _getValue, values);
    }

    internal ReportDataSource WithRootValues(ReportDataSource source)
    {
        return WithRootValues(
            source.Fields.Where(field => ReportJsonData.IsRootField(field.Key)).ToArray(),
            source._rootValues ?? new Dictionary<string, object?>());
    }

    public static ReportDataSource Create<T>(string name, IEnumerable<T> rows, params ReportField<T>[] fields)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(fields);
        var getters = new Dictionary<string, Func<T, object?>>(StringComparer.Ordinal);
        foreach (var field in fields)
        {
            ArgumentNullException.ThrowIfNull(field);
            ArgumentException.ThrowIfNullOrWhiteSpace(field.Key);
            ArgumentNullException.ThrowIfNull(field.Getter);
            if (!getters.TryAdd(field.Key, field.Getter))
                throw new ArgumentException($"Duplicate report field: {field.Key}", nameof(fields));
        }

        var snapshot = rows.ToArray();
        var descriptors = Array.AsReadOnly(fields.Select(f => new ReportFieldInfo(f.Key, f.Label, f.Kind)).ToArray());
        return new ReportDataSource(name, snapshot.Length, descriptors, (index, key) =>
            getters.TryGetValue(key, out var getter)
                ? getter(snapshot[index])
                : throw new ArgumentException($"Unknown report field: {key}", nameof(key)));
    }
}