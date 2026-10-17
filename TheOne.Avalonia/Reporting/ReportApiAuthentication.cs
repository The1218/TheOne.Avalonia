using System.Text.Json;

namespace TheOne.Avalonia.Reporting;

/// <summary>Login configuration only. Resolved tokens are never stored in the report document.</summary>
public sealed record ReportApiAuthentication
{
    public string Url { get; init; } = "";
    public string Method { get; init; } = "POST";
    public string Parameters { get; init; } = "";
    public ReportApiParameterMode ParameterMode { get; init; }
    public string TokenField { get; init; } = "/data/token";
    public string HeaderName { get; init; } = "Authorization";
    public string HeaderPrefix { get; init; } = "Bearer ";

    internal ReportApiRequest LoginRequest => new(Url, Method, Parameters, ParameterMode);

    internal void Validate()
    {
        using var login = LoginRequest.CreateMessage();
        if (string.IsNullOrWhiteSpace(TokenField)) throw new ArgumentException("请填写登录响应中的 token 字段路径。");
        ValidateHeader(HeaderPrefix + "token");
    }

    internal void ValidateHeader(string value)
    {
        if (string.IsNullOrWhiteSpace(HeaderName) || value.Any(char.IsControl))
            throw new ArgumentException("权限请求头名称或值无效。");
        try
        {
            using var message = new HttpRequestMessage();
            message.Headers.Add(HeaderName, value);
        }
        catch (Exception error) when (error is FormatException or InvalidOperationException)
        {
            throw new ArgumentException("权限请求头名称或值无效。", error);
        }
    }

    internal string ResolveHeader(string json)
    {
        using var document = JsonDocument.Parse(json);
        var value = document.RootElement;
        var parts = TokenField.StartsWith('/')
            ? TokenField[1..].Split('/').Select(part => part.Replace("~1", "/").Replace("~0", "~"))
            : TokenField.Split('.');
        foreach (var part in parts)
            if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty(part, out var property))
                value = property;
            else if (value.ValueKind == JsonValueKind.Array && int.TryParse(part, out var index) && index >= 0 &&
                     index < value.GetArrayLength()) value = value[index];
            else throw new ArgumentException("登录响应中找不到指定的 token 字段。");
        var token = value.ValueKind == JsonValueKind.String ? value.GetString() :
            value.ValueKind == JsonValueKind.Number ? value.GetRawText() : null;
        if (string.IsNullOrWhiteSpace(token)) throw new ArgumentException("登录响应的 token 字段需要是非空字符串或数字。");
        var header = HeaderPrefix + token;
        ValidateHeader(header);
        return header;
    }
}