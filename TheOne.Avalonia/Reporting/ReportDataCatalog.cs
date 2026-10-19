using System.Globalization;
using System.Text.Json;

namespace TheOne.Avalonia.Reporting;

/// <summary>Designer projection: scalar properties and whole arrays, never one item per record.</summary>
internal static class ReportDataCatalog
{
    internal static IReadOnlyList<ReportDataNode> Items(string json)
    {
        using var document = JsonDocument.Parse(json);
        var items = new List<ReportDataNode>();
        Visit(document.RootElement, "$", "", 0);
        return items;

        void Visit(JsonElement value, string name, string pointer, int depth)
        {
            if (depth > 16) throw new ArgumentException("数据项层级超过 16 层。");
            if (value.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in value.EnumerateObject())
                    Visit(property.Value, property.Name, pointer + "/" + Escape(property.Name), depth + 1);
                return;
            }
            if (items.Count >= 4096) throw new ArgumentException("可选数据项超过 4096 项。");
            if (value.ValueKind != JsonValueKind.Array)
            {
                items.Add(Scalar(value, name, pointer));
                return;
            }
            var fields = new Dictionary<string, ReportDataNode>(StringComparer.Ordinal);
            var index = 0;
            foreach (var row in value.EnumerateArray()) Discover(row, "", "", pointer + "/" + index++, 0);
            items.Add(new ReportDataNode(name, pointer, ReportDataNodeKind.Collection,
                $"{value.GetArrayLength()} 行 · {fields.Count} 字段", fields.Values.ToArray()));

            void Discover(JsonElement row, string path, string label, string absolute, int level)
            {
                if (row.ValueKind == JsonValueKind.Object && level < 16)
                {
                    foreach (var property in row.EnumerateObject())
                        Discover(property.Value, path + "/" + Escape(property.Name),
                            label.Length == 0 ? property.Name : label + "." + property.Name,
                            absolute + "/" + Escape(property.Name), level + 1);
                }
                else if (row.ValueKind is not (JsonValueKind.Array or JsonValueKind.Object) && !fields.ContainsKey(path))
                {
                    if (fields.Count >= 4096) throw new ArgumentException("集合字段超过 4096 项。");
                    fields.Add(path, Scalar(row, label.Length == 0 ? "Value" : label, absolute));
                }
            }
        }
    }

    internal static IReadOnlyList<ReportDataNode> Tree(string json, ReportApiRequest? request)
    {
        var selected = request?.SelectedDataPointers?.ToHashSet(StringComparer.Ordinal);
        var items = Items(json).Where(item => selected is null || selected.Contains(item.Pointer)).ToArray();
        var scalars = items.Where(item => item.Kind == ReportDataNodeKind.Scalar).ToArray();
        var collections = items.Where(item => item.Kind == ReportDataNodeKind.Collection).ToArray();
        var groups = new List<ReportDataNode>();
        if (scalars.Length > 0) groups.Add(Group("单值字段", scalars));
        if (collections.Length > 0) groups.Add(Group("数据集", collections));
        return [new ReportDataNode(request?.SourceName ?? "接口数据", "", ReportDataNodeKind.Object,
            $"{scalars.Length} 个单值 · {collections.Length} 个数据集", groups, expanded: true, typeLabel: "API")];
    }

    private static ReportDataNode Group(string name, ReportDataNode[] items) =>
        new(name, "", ReportDataNodeKind.Object, items.Length + " 项", items, expanded: true, typeLabel: "分组");

    private static string Escape(string name) => name.Replace("~", "~0").Replace("/", "~1");

    private static ReportDataNode Scalar(JsonElement value, string name, string pointer)
    {
        var text = value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.GetRawText();
        var type = value.ValueKind switch
        {
            JsonValueKind.Number => "数值", JsonValueKind.True or JsonValueKind.False => "布尔",
            JsonValueKind.Null => "空值",
            JsonValueKind.String when DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _) => "日期",
            _ => "文本"
        };
        return new ReportDataNode(name, pointer, ReportDataNodeKind.Scalar,
            text.Length > 120 ? text[..120] + "…" : text, [], ReportJsonData.RootFieldPrefix + pointer,
            typeLabel: type);
    }
}
