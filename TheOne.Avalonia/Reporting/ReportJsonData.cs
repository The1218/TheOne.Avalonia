using System.Text.Json;

namespace TheOne.Avalonia.Reporting;

/// <summary>JSON Pointer identifies the body unambiguously, even when property names contain dots or slashes.</summary>
public sealed record ReportJsonBody(string Pointer, string Label);

public static class ReportJsonData
{
    public const string RootFieldPrefix = "$root";
    public static bool IsRootField(string key) => key == RootFieldPrefix || key.StartsWith(RootFieldPrefix + "/", StringComparison.Ordinal);

    public static object? ReadRootValue(string json, string pointer)
    {
        using var document = JsonDocument.Parse(json);
        var value = Get(document.RootElement, pointer);
        if (value.ValueKind is JsonValueKind.Object or JsonValueKind.Array) throw new ArgumentException("此节点不是单值。", nameof(pointer));
        return ReadValue(value);
    }

    /// <summary>Builds a bounded response tree. Truncating display nodes never truncates the source rows.</summary>
    public static IReadOnlyList<ReportDataNode> CreateTree(string json)
    {
        using var document = JsonDocument.Parse(json);
        var remaining = 4000;
        ReportDataNode Build(JsonElement value, string name, string pointer, int depth)
        {
            remaining--;
            var kind = value.ValueKind == JsonValueKind.Array ? ReportDataNodeKind.Collection
                : value.ValueKind == JsonValueKind.Object ? ReportDataNodeKind.Object : ReportDataNodeKind.Scalar;
            var children = new List<ReportDataNode>();
            if (depth < 16 && remaining > 0)
            {
                if (value.ValueKind == JsonValueKind.Object)
                    foreach (var property in value.EnumerateObject())
                    {
                        if (remaining <= 0) { children.Add(new("其余属性未展开", "", ReportDataNodeKind.Object, "已达到树节点显示上限", [])); break; }
                        children.Add(Build(property.Value, property.Name, pointer + "/" + Escape(property.Name), depth + 1));
                    }
                else if (value.ValueKind == JsonValueKind.Array)
                {
                    var count = Math.Min(200, value.GetArrayLength());
                    for (var index = 0; index < count && remaining > 0; index++)
                        children.Add(Build(value[index], "[" + index + "]", pointer + "/" + index, depth + 1));
                    if (children.Count < value.GetArrayLength())
                        children.Add(new("其余 " + (value.GetArrayLength() - children.Count) + " 项", "", ReportDataNodeKind.Object, "拖放集合仍使用全部数据", []));
                }
            }
            var preview = kind == ReportDataNodeKind.Collection ? value.GetArrayLength() + " 项"
                : kind == ReportDataNodeKind.Object ? value.EnumerateObject().Count() + " 个属性"
                : Convert.ToString(ReadValue(value), System.Globalization.CultureInfo.InvariantCulture) ?? "null";
            if (preview.Length > 120) preview = preview[..120] + "…";
            return new(name, pointer, kind, preview, children.AsReadOnly(), kind == ReportDataNodeKind.Scalar ? RootFieldPrefix + pointer : "", expanded: depth == 0 && kind == ReportDataNodeKind.Object);
        }
        return [Build(document.RootElement, "$", "", 0)];
    }
    public static IReadOnlyList<ReportJsonBody> FindBodies(string json)
    {
        using var document = JsonDocument.Parse(json);
        var bodies = new List<ReportJsonBody>();
        void Visit(JsonElement element, string pointer, string label, int depth)
        {
            if (depth > 16) return;
            if (element.ValueKind is JsonValueKind.Array or JsonValueKind.Object) bodies.Add(new(pointer, label + (element.ValueKind == JsonValueKind.Array ? $"  [{element.GetArrayLength()} 条]" : "  对象")));
            if (element.ValueKind == JsonValueKind.Object)
                foreach (var property in element.EnumerateObject()) Visit(property.Value, pointer + "/" + Escape(property.Name), label + "." + property.Name, depth + 1);
        }
        Visit(document.RootElement, "", "$", 0);
        if (bodies.Count == 0) bodies.Add(new("", "$  值"));
        return bodies.AsReadOnly();
    }

    public static ReportDataSource CreateSource(string json, string bodyPointer) => CreateSource(json, bodyPointer, null);

    public static ReportDataSource CreateSource(string json, string bodyPointer, IEnumerable<string>? rootFieldKeys)
    {
        using var document = JsonDocument.Parse(json);
        var body = Get(document.RootElement, bodyPointer);
        if (body.ValueKind == JsonValueKind.Undefined) throw new ArgumentException("所选数据体不存在。", nameof(bodyPointer));
        var rows = body.ValueKind == JsonValueKind.Array ? body.EnumerateArray().Select(e => e.Clone()).ToArray() : [body.Clone()];
        var fields = new Dictionary<string, (string Label, ReportValueKind Kind, bool HasValue)>(StringComparer.Ordinal);
        void Discover(JsonElement element, string pointer, string label, int depth)
        {
            if (element.ValueKind == JsonValueKind.Object && depth < 16)
                foreach (var property in element.EnumerateObject()) Discover(property.Value, pointer + "/" + Escape(property.Name), label.Length == 0 ? property.Name : label + "." + property.Name, depth + 1);
            else if (element.ValueKind != JsonValueKind.Array)
            {
                var kind = element.ValueKind == JsonValueKind.Number ? ReportValueKind.Number : ReportValueKind.Text;
                var hasValue = element.ValueKind != JsonValueKind.Null;
                if (!fields.TryGetValue(pointer, out var prior))
                {
                    if (fields.Count >= 4096) throw new ArgumentException("集合的不同字段超过 4096 项，无法配置为报表列。");
                    fields[pointer] = (label.Length == 0 ? "Value" : label, kind, hasValue);
                }
                else if (hasValue) fields[pointer] = (prior.Label, !prior.HasValue || prior.Kind == kind ? kind : ReportValueKind.Text, true);
            }
        }
        foreach (var row in rows) Discover(row, "", "", 0);
        var root = document.RootElement.Clone();
        var rootFields = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        void DiscoverRoot(JsonElement value, string pointer, int depth)
        {
            if (depth > 16 || rootFields.Count >= 4096) return;
            if (value.ValueKind == JsonValueKind.Object)
                foreach (var property in value.EnumerateObject()) DiscoverRoot(property.Value, pointer + "/" + Escape(property.Name), depth + 1);
            else if (value.ValueKind != JsonValueKind.Array) rootFields[RootFieldPrefix + pointer] = value;
        }
        DiscoverRoot(root, "", 0);
        foreach (var key in rootFieldKeys ?? [])
            if (IsRootField(key) && Get(root, key[RootFieldPrefix.Length..]) is var value && value.ValueKind is not (JsonValueKind.Undefined or JsonValueKind.Object or JsonValueKind.Array)) rootFields[key] = value;
        var rowFields = fields.Select(pair => new ReportField<JsonElement>(pair.Key.Length == 0 ? "$" : pair.Key, pair.Value.Label,
            row => Read(row, pair.Key), pair.Value.Kind));
        var globalFields = rootFields.Select(pair => new ReportFieldInfo(pair.Key, "$" + pair.Key[RootFieldPrefix.Length..],
            pair.Value.ValueKind == JsonValueKind.Number ? ReportValueKind.Number : ReportValueKind.Text)).ToArray();
        // The display label is separate from the key: /customer/name and /customer.name remain distinct.
        return ReportDataSource.Create("API", rows, rowFields.ToArray()).WithRootValues(globalFields, rootFields.ToDictionary(pair => pair.Key, pair => ReadValue(pair.Value), StringComparer.Ordinal));
    }
    private static object? Read(JsonElement row, string pointer)
    {
        var value = Get(row, pointer);
        return ReadValue(value);
    }
    private static object? ReadValue(JsonElement value) => value.ValueKind switch
        {
            JsonValueKind.Undefined or JsonValueKind.Null => null,
            JsonValueKind.Number => value.TryGetDecimal(out var number) ? number : value.GetRawText(),
            JsonValueKind.String => value.GetString(), JsonValueKind.True => "是", JsonValueKind.False => "否", _ => value.GetRawText()
        };
    private static JsonElement Get(JsonElement element, string pointer)
    {
        if (pointer.Length == 0) return element;
        if (!pointer.StartsWith('/')) return default;
        foreach (var part in pointer.Split('/').Skip(1))
        {
            var name = part.Replace("~1", "/").Replace("~0", "~");
            if (element.ValueKind == JsonValueKind.Array && int.TryParse(name, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var index)
                && index >= 0 && index < element.GetArrayLength()) element = element[index];
            else if (element.ValueKind != JsonValueKind.Object || !element.TryGetProperty(name, out element)) return default;
        }
        return element;
    }
    private static string Escape(string value) => value.Replace("~", "~0").Replace("/", "~1");
}
