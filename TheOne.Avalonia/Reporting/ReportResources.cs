using System.Globalization;
using System.Text;
using System.Text.Json;
using SkiaSharp;

namespace TheOne.Avalonia.Reporting;

/// <summary>Loads public HTTP resources with bounded responses. Hosts can supply an authenticated HttpClient.</summary>
public static class ReportResources
{
    private static async Task<string> DownloadBase64Async(string url, int maximumBytes, HttpClient? client = null,
        CancellationToken cancellationToken = default)
    {
        return await DownloadBase64Async(new ReportApiRequest(url), maximumBytes, client, cancellationToken);
    }

    private static async Task<string> DownloadBase64Async(ReportApiRequest request, int maximumBytes,
        HttpClient? client, CancellationToken cancellationToken, string? authorization = null)
    {
        if (maximumBytes <= 0) throw new ArgumentOutOfRangeException(nameof(maximumBytes));
        using var message = request.CreateMessage();
        if (authorization is not null) message.Headers.Add(request.Authentication!.HeaderName, authorization);
        // ResponseHeadersRead only bounds the headers with HttpClient.Timeout. Bound body reads as well,
        // including requests made through a consumer-owned authenticated client.
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        var token = timeout.Token;
        using var owned = client is null ? new HttpClient() : null;
        try
        {
            using var response =
                await (client ?? owned!).SendAsync(message, HttpCompletionOption.ResponseHeadersRead, token);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentLength > maximumBytes) throw new ArgumentException("远程资源超过大小限制。");
            await using var stream = await response.Content.ReadAsStreamAsync(token);
            using var buffer = new MemoryStream();
            var bytes = new byte[8192];
            int count;
            while ((count = await stream.ReadAsync(bytes, token)) > 0)
            {
                if (buffer.Length + count > maximumBytes) throw new ArgumentException("远程资源超过大小限制。");
                buffer.Write(bytes, 0, count);
            }

            token.ThrowIfCancellationRequested();
            return Convert.ToBase64String(buffer.ToArray());
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException("远程资源加载超时。", exception);
        }
    }

    /// <summary>Fetches one bounded JSON response using the same request path as the independent preview.</summary>
    public static async Task<string> FetchJsonAsync(ReportApiRequest request, HttpClient? client = null,
        CancellationToken cancellationToken = default)
    {
        return await FetchJsonWithAuthenticationAsync(request, client, cancellationToken, null);
    }

    internal static async Task<string> FetchJsonWithAuthenticationAsync(ReportApiRequest request, HttpClient? client,
        CancellationToken cancellationToken, Action<string, string>? authenticated)
    {
        ArgumentNullException.ThrowIfNull(request);
        // Validate both requests before making network calls. The header belongs to this
        // data request only; consumer-owned HttpClient.DefaultRequestHeaders stay untouched.
        using var validation = request.CreateMessage();
        using var owned = client is null ? new HttpClient() : null;
        client ??= owned!;
        string? header = null;
        if (request.Authentication is { } authentication)
        {
            var loginJson = await FetchJsonAsync(authentication.LoginRequest, client, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            header = authentication.ResolveHeader(loginJson);
            authenticated?.Invoke(loginJson, header);
        }

        cancellationToken.ThrowIfCancellationRequested();
        var bytes = await DownloadBase64Async(request, 4 * 1024 * 1024, client, cancellationToken, header);
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(bytes));
        using var parsed = JsonDocument.Parse(json);
        return json;
    }

    public static async Task<ReportDataSource?> LoadDataAsync(ReportDocument document, HttpClient? client = null,
        CancellationToken cancellationToken = default)
    {
        if (document.DataSourceUrl.Length == 0) return null;
        var json = await FetchJsonAsync(ReportApiRequest.FromDocument(document), client, cancellationToken);
        return ReportJsonData.CreateSource(json, document.DataBodyPointer,
            document.Elements.Select(element => element.FieldKey)
                .Concat(document.Columns.Select(column => column.FieldKey)));
    }

    public static Task LoadImagesAsync(ReportDocument document, HttpClient? client = null,
        CancellationToken cancellationToken = default)
    {
        return LoadImagesAsync(document, client, cancellationToken, null);
    }

    public static async Task LoadImagesAsync(ReportDocument document, HttpClient? client,
        CancellationToken cancellationToken, ReportDataSource? source)
    {
        ArgumentNullException.ThrowIfNull(document);
        var images = new Dictionary<string, string>(StringComparer.Ordinal);
        var pending = new List<(ReportElement Element, string OriginalUrl, string Url, string Data)>();
        long totalBytes = 0, totalPixels = 0;
        foreach (var element in document.Elements
                     .Where(e => e.Kind == ReportElementKind.Image && (e.ImageUrl.Length > 0 || e.FieldKey.Length > 0))
                     .ToArray())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var originalUrl = element.ImageUrl;
            var url = source is not null && element.FieldKey.Length > 0
                ? Convert.ToString(source.GetValue(0, element.FieldKey), CultureInfo.InvariantCulture) ?? ""
                : originalUrl;
            if (!images.TryGetValue(url, out var data))
            {
                data = element.ResolvedImageUrl == url && element.ResolvedImageData.Length > 0
                    ? element.ResolvedImageData
                    : await DownloadBase64Async(url, ReportImageData.MaximumBytes, client, cancellationToken);
                var bytes = ReportImageData.Decode(data);
                using var codec = SKCodec.Create(new MemoryStream(bytes, false)) ??
                                  throw new ArgumentException("远程图片无法解码。");
                totalBytes += bytes.Length;
                totalPixels += (long)codec.Info.Width * codec.Info.Height;
                if (totalBytes > 32 * 1024 * 1024 || totalPixels > 32_000_000)
                    throw new ArgumentException("报表远程图片合计不能超过 32 MiB 或 3200 万像素。");
                var info = new SKImageInfo(codec.Info.Width, codec.Info.Height, SKColorType.Bgra8888,
                    SKAlphaType.Premul);
                using var decoded = new SKBitmap(info);
                if (decoded.GetPixels() == IntPtr.Zero ||
                    codec.GetPixels(info, decoded.GetPixels()) != SKCodecResult.Success)
                    throw new ArgumentException("远程图片像素无法完整解码。");
                images.Add(url, data);
            }

            pending.Add((element, originalUrl, url, data));
        }

        // An unavailable image fails the report instead of committing an incomplete set of pictures.
        cancellationToken.ThrowIfCancellationRequested();
        foreach (var (element, originalUrl, url, data) in pending)
            if (element.ImageUrl == originalUrl)
            {
                element.ImageUrl = url;
                element.ResolvedImageUrl = url;
                element.ResolvedImageData = data;
            }
    }
}