using System.Buffers;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TheOne.Avalonia.Controls;

/// <summary>The HTTP method used by the built-in multipart upload.</summary>
public enum UploadRequestType
{
    Post,
    Put,
    Patch
}

/// <summary>A single upload payload. Sending it transfers ownership of <see cref="FileData" /> to the request.</summary>
public sealed class UploadModel
{
    public UploadModel(string fileName, Stream fileData, string? contentType = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(fileData);
        if (!fileData.CanRead) throw new ArgumentException("The file stream must be readable.", nameof(fileData));
        if (fileName.Contains('\r') || fileName.Contains('\n'))
            throw new ArgumentException("The file name cannot contain line breaks.", nameof(fileName));
        if (!string.IsNullOrWhiteSpace(contentType) && !MediaTypeHeaderValue.TryParse(contentType, out _))
            throw new ArgumentException("The content type must be a valid media type.", nameof(contentType));
        FileName = fileName;
        FileData = fileData;
        ContentType = string.IsNullOrWhiteSpace(contentType) ? null : contentType;
    }

    public string FileName { get; }
    public Stream FileData { get; }
    public string? ContentType { get; }
}

/// <summary>A successful HTTP response. A null URL means the response contained no matching absolute file URL.</summary>
public sealed record UploadHttpResult(HttpStatusCode StatusCode, string ResponseBody, string? SuccessFileUrl);

/// <summary>Streams one file as multipart/form-data, using the fields filename and filedata.</summary>
public static class UploadHttp
{
    private const int ResponseLimit = 1024 * 1024;

    /// <summary>
    ///     Sends from the stream's current position and closes it when the request finishes. The caller owns the client.
    ///     Progress is the file transfer percentage (0–100); an unknown-length stream reports its start and completion.
    ///     Responses larger than 1 MiB are rejected. Only successful HTTP responses are inspected for file URLs.
    /// </summary>
    public static async Task<UploadHttpResult> SendAsync(HttpClient client, string uploadApi,
        UploadRequestType requestType,
        UploadModel model, string allowedExtensions, IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(model);
        if (!TryHttpUri(uploadApi, out var endpoint))
            throw new ArgumentException("UploadApi must be an absolute HTTP or HTTPS URL.", nameof(uploadApi));
        var method = requestType switch
        {
            UploadRequestType.Post => HttpMethod.Post,
            UploadRequestType.Put => HttpMethod.Put,
            UploadRequestType.Patch => HttpMethod.Patch,
            _ => throw new ArgumentOutOfRangeException(nameof(requestType))
        };
        using var request = new HttpRequestMessage(method, endpoint);
        var multipart = new MultipartFormDataContent();
        request.Content = multipart;
        multipart.Add(new StringContent(model.FileName, Encoding.UTF8), "filename");
        var fileContent = new ProgressFileContent(model.FileData, progress);
        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(model.ContentType ?? "application/octet-stream");
        multipart.Add(fileContent, "filedata", model.FileName);
        using var response = await client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        response.EnsureSuccessStatusCode();
        var body = await ReadResponseAsync(response.Content, cancellationToken).ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
        var url = ExtractFileUrl(body, allowedExtensions);
        cancellationToken.ThrowIfCancellationRequested();
        return new UploadHttpResult(response.StatusCode, body, url);
    }

    /// <summary>Returns the first absolute HTTP(S) file URL matching the extension filter, without contacting that URL.</summary>
    public static string? ExtractFileUrl(string responseBody, string allowedExtensions)
    {
        ArgumentNullException.ThrowIfNull(responseBody);
        var extensions = (allowedExtensions ?? "").Split([',', ';', ' '],
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.StartsWith('.') ? value : "." + value).ToArray();
        try
        {
            using var document = JsonDocument.Parse(responseBody);
            return FindJsonUrl(document.RootElement, extensions);
        }
        catch (JsonException)
        {
            return FindTextUrl(responseBody, extensions);
        }
    }

    private static string? FindJsonUrl(JsonElement element, string[] extensions)
    {
        if (element.ValueKind == JsonValueKind.String) return FindTextUrl(element.GetString() ?? "", extensions);
        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var child in element.EnumerateArray())
                if (FindJsonUrl(child, extensions) is { } url)
                    return url;
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
                if (FindJsonUrl(property.Value, extensions) is { } url)
                    return url;
        }

        return null;
    }

    private static string? FindTextUrl(string text, string[] extensions)
    {
        // Check the complete JSON string/plain URL first, preserving legal punctuation in its path and query.
        var trimmed = text.Trim();
        if (IsFileUrl(trimmed, extensions)) return trimmed;
        for (var start = 0; start < text.Length; start++)
        {
            var remaining = text.AsSpan(start);
            if (!remaining.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !remaining.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) continue;
            var end = start;
            while (end < text.Length && !char.IsWhiteSpace(text[end]) &&
                   text[end] is not ('<' or '>' or '"' or '\'' or '\\' or '{' or '}' or '[' or ']')) end++;
            // A backslash is a broken URL path, not a boundary from which to salvage a valid URL prefix.
            if (end < text.Length && text[end] == '\\')
            {
                start = end;
                continue;
            }

            var candidate = text[start..end].TrimEnd('.', ',', ';', ':', '!', '?', ')');
            if (IsFileUrl(candidate, extensions)) return candidate;
            start = Math.Max(start, end - 1);
        }

        return null;
    }

    private static bool IsFileUrl(string candidate, string[] extensions)
    {
        if (!TryHttpUri(candidate, out var uri)) return false;
        var path = Uri.UnescapeDataString(uri!.AbsolutePath);
        if (path.EndsWith('/') || path.Any(char.IsControl)) return false;
        var name = path[(path.LastIndexOf('/') + 1)..];
        var dot = name.LastIndexOf('.');
        if (dot <= 0 || dot == name.Length - 1) return false;
        var suffix = name.AsSpan(dot + 1);
        if (suffix.Length > 16) return false;
        foreach (var character in suffix)
            if (!char.IsAsciiLetterOrDigit(character))
                return false;
        return extensions.Length == 0 ||
               extensions.Any(extension => name.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryHttpUri(string? value, out Uri? uri)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out uri) && uri.IsWellFormedOriginalString() &&
               (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
               !string.IsNullOrWhiteSpace(uri.Host) && Uri.CheckHostName(uri.Host) != UriHostNameType.Unknown;
    }

    private static async Task<string> ReadResponseAsync(HttpContent content, CancellationToken cancellationToken)
    {
        if (content.Headers.ContentLength > ResponseLimit)
            throw new IOException("The upload response exceeds the 1 MiB limit.");
        await using var source = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var buffer = new MemoryStream();
        var bytes = ArrayPool<byte>.Shared.Rent(8192);
        try
        {
            int read;
            while ((read = await source.ReadAsync(bytes.AsMemory(0, bytes.Length), cancellationToken)
                       .ConfigureAwait(false)) != 0)
            {
                if (buffer.Length + read > ResponseLimit)
                    throw new IOException("The upload response exceeds the 1 MiB limit.");
                buffer.Write(bytes, 0, read);
            }

            var encoding = Encoding.UTF8;
            if (content.Headers.ContentType?.CharSet is { Length: > 0 } charset)
                try
                {
                    encoding = Encoding.GetEncoding(charset.Trim('"'));
                }
                catch (ArgumentException)
                {
                }

            using var reader = new StreamReader(new MemoryStream(buffer.GetBuffer(), 0, (int)buffer.Length, false),
                encoding, true);
            return await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(bytes);
        }
    }

    private sealed class ProgressFileContent(Stream source, IProgress<double>? progress) : HttpContent
    {
        private int _disposed;

        protected override bool TryComputeLength(out long length)
        {
            try
            {
                if (source.CanSeek)
                {
                    length = Math.Max(0, source.Length - source.Position);
                    return true;
                }
            }
            catch (NotSupportedException)
            {
            }

            length = 0;
            return false;
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            return SerializeToStreamAsync(stream, context, CancellationToken.None);
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context,
            CancellationToken cancellationToken)
        {
            var knownLength = TryComputeLength(out var length);
            var buffer = ArrayPool<byte>.Shared.Rent(81920);
            long written = 0;
            try
            {
                progress?.Report(0);
                int read;
                while ((read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)
                           .ConfigureAwait(false)) != 0)
                {
                    await stream.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                    written += read;
                    if (knownLength && length > 0) progress?.Report(Math.Min(100, written * 100d / length));
                }

                progress?.Report(100);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && Interlocked.Exchange(ref _disposed, 1) == 0) source.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}