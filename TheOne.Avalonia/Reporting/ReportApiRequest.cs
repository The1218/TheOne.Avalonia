using System.Text;
using System.Text.Json;

namespace TheOne.Avalonia.Reporting;

public enum ReportApiParameterMode
{
    Json,
    Form
}

/// <summary>Portable API configuration. Parameters is a JSON object; GET parameters become query values.</summary>
public sealed record ReportApiRequest(
    string Url,
    string Method = "GET",
    string Parameters = "",
    ReportApiParameterMode ParameterMode = ReportApiParameterMode.Json)
{
    public ReportApiAuthentication? Authentication { get; init; }
    public string SourceName { get; init; } = "接口数据";
    /// <summary>JSON Pointers visible in the designer; null retains all available items.</summary>
    public string[]? SelectedDataPointers { get; init; }

    internal static ReportApiRequest FromDocument(ReportDocument document)
    {
        return new ReportApiRequest(document.DataSourceUrl, document.DataSourceMethod, document.DataSourceParameters,
            document.DataSourceParameterMode)
        {
            Authentication = document.DataSourceAuthentication, SourceName = document.DataSourceName,
            SelectedDataPointers = document.SelectedDataPointers
        };
    }

    internal HttpRequestMessage CreateMessage()
    {
        Authentication?.Validate();
        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https"))
            throw new ArgumentException("请输入有效的 HTTP / HTTPS 接口地址。");
        var method = Method?.Trim().ToUpperInvariant();
        if (method is not ("GET" or "POST" or "PUT" or "PATCH" or "DELETE") || !Enum.IsDefined(ParameterMode))
            throw new ArgumentException("请求方式或参数格式无效。");
        if (Encoding.UTF8.GetByteCount(Parameters) > 64 * 1024) throw new ArgumentException("请求参数不能超过 64 KiB。");
        using var json = JsonDocument.Parse(string.IsNullOrWhiteSpace(Parameters) ? "{}" : Parameters);
        if (json.RootElement.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("请求参数需要是 JSON 对象，例如 {\"page\":1}。");
        var values = json.RootElement.EnumerateObject().Select(property => new KeyValuePair<string, string>(
            property.Name,
            property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString()! :
            property.Value.ValueKind == JsonValueKind.Null ? "" : property.Value.GetRawText())).ToArray();
        if (method == "GET" && values.Length > 0)
        {
            var builder = new UriBuilder(uri);
            var encoded = string.Join("&",
                values.Select(pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value)));
            builder.Query = builder.Query.TrimStart('?') is { Length: > 0 } existing
                ? existing + "&" + encoded
                : encoded;
            uri = builder.Uri;
        }

        var request = new HttpRequestMessage(new HttpMethod(method), uri);
        if (method != "GET")
            request.Content = ParameterMode == ReportApiParameterMode.Form
                ? new FormUrlEncodedContent(values)
                : new StringContent(json.RootElement.GetRawText(), Encoding.UTF8, "application/json");
        return request;
    }
}
