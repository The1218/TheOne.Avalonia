using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static class UploadApiPreview
{
    private static readonly byte[] Payload = Encoding.UTF8.GetBytes("Single-file upload\npreview payload\n");
    private const string Endpoint = "https://upload.example.test/files";
    private const string FileUrl = "https://cdn.example.test/files/report.pdf?token=preview#page=1";

    public static void Verify(Action<bool, string> check)
    {
        VerifyResponseUrls(check);
        VerifyHttpContract(check);
        VerifyLoopback(check);
        VerifyControl(check);
        VerifyCancellation(check);
        VerifyBindings(check);
    }

    private static void VerifyResponseUrls(Action<bool, string> check)
    {
        foreach (var body in new[]
        {
            FileUrl,
            "\"" + FileUrl + "\"",
            "{\"data\":{\"files\":[null,7,{\"path\":\"" + FileUrl + "\"}]}}",
            "{\"data\":\"https:\\/\\/cdn.example.test\\/files\\/report.pdf?token=preview#page=1\"}",
            "{\"data\":\"\\u0068ttps://cdn.example.test/files/report.pdf?token=preview#page=1\"}"
        })
            check(UploadHttp.ExtractFileUrl(body, ".pdf,.png") == FileUrl, "Upload API: plain text, nested JSON, arrays and escaped strings yield the file URL");

        foreach (var body in new[]
        {
            "https://cdn.example.test/files/report", "https://cdn.example.test/files/report?name=report.pdf",
            "https://cdn.example.test/files/report.pdf.exe", "https://cdn.example.test/files/report.png",
            "https://cdn.example.test/files/report.pdf/", "/files/report.pdf", "file:///tmp/report.pdf",
            "ftp://cdn.example.test/report.pdf", "javascript:report.pdf", "https:///report.pdf",
            "https://cdn.example.test/files/report name.pdf", "https://cdn.example.test/files/report.pdf\\tail",
            "{\"url\":\"https://cdn.example.test/files/report.pdf\\\\tail\"}", "{\"path\":null}"
        })
            check(UploadHttp.ExtractFileUrl(body, ".pdf") == null, "Upload API: invalid, non-HTTP, missing-extension and disallowed-extension paths are rejected");

        check(UploadHttp.ExtractFileUrl("[\"https://cdn.example.test/wrong.exe\",{\"url\":\"" + FileUrl + "\"}]", ".pdf") == FileUrl,
            "Upload API: parser skips invalid candidates before finding a valid nested URL");
        check(UploadHttp.ExtractFileUrl("https://cdn.example.test/files/REPORT.PDF", "pdf") != null,
            "Upload API: suffix matching is case-insensitive and accepts extension configuration without a dot");
    }

    private static void VerifyHttpContract(Action<bool, string> check)
    {
        foreach (var type in Enum.GetValues<UploadRequestType>())
        {
            RequestCapture? capture = null;
            using var handler = new Handler(async (request, token) =>
            {
                capture = new(request.Method.Method, request.Content!.Headers.ContentType!.ToString(), await request.Content.ReadAsByteArrayAsync(token));
                return Response(HttpStatusCode.Created, "{\"data\":{\"url\":\"" + FileUrl + "\"}}");
            });
            using var client = new HttpClient(handler);
            using var stream = new MemoryStream(Payload, writable: false);
            var progress = new ProgressCapture();
            var operation = UploadHttp.SendAsync(client, Endpoint, type, new UploadModel("report.pdf", stream, "application/pdf"), ".pdf", progress, default);
            Wait(() => operation.IsCompleted); var result = operation.GetAwaiter().GetResult();
            check(capture?.Method == type.ToString().ToUpperInvariant(), $"Upload API/{type}: configured HTTP method reaches the transport");
            check(capture != null && IsSingleFileMultipart(capture), $"Upload API/{type}: multipart contains exactly filename and filedata, preserving the filename and file bytes");
            check(result.StatusCode == HttpStatusCode.Created && result.ResponseBody.Contains(FileUrl, StringComparison.Ordinal) && result.SuccessFileUrl == FileUrl,
                $"Upload API/{type}: a successful response exposes its body, status and parsed file URL");
            check(progress.Values.Count > 0 && progress.Values[^1] == 100 && progress.Values.All(value => double.IsFinite(value) && value is >= 0 and <= 100),
                $"Upload API/{type}: request content reports bounded progress through completion");
            check(!stream.CanRead, $"Upload API/{type}: request disposal releases its transferred file stream");
        }

        using var failureHandler = new Handler((_, _) => Task.FromResult(Response(HttpStatusCode.BadRequest, "{\"url\":\"" + FileUrl + "\"}")));
        using var failureClient = new HttpClient(failureHandler);
        var failure = UploadHttp.SendAsync(failureClient, Endpoint, UploadRequestType.Post, new UploadModel("report.pdf", new MemoryStream(Payload)), ".pdf", null, default);
        Wait(() => failure.IsCompleted); var rejected = false;
        try { failure.GetAwaiter().GetResult(); } catch (HttpRequestException error) { rejected = error.StatusCode == HttpStatusCode.BadRequest; }
        check(rejected, "Upload API: non-success HTTP responses fail before a URL in their error body can be accepted");

        using var missingHandler = new Handler((_, _) => Task.FromResult(Response(HttpStatusCode.OK, "{\"ok\":true}")));
        using var missingClient = new HttpClient(missingHandler);
        var missing = UploadHttp.SendAsync(missingClient, Endpoint, UploadRequestType.Post, new UploadModel("report.pdf", new MemoryStream(Payload)), ".pdf", null, default);
        Wait(() => missing.IsCompleted);
        check(missing.GetAwaiter().GetResult().SuccessFileUrl == null, "Upload API: successful responses without a valid file URL do not invent a result");

        var unknownLengthRead = false;
        using var unknownHandler = new Handler(async (request, token) =>
        {
            unknownLengthRead = IsSingleFileMultipart(new(request.Method.Method, request.Content!.Headers.ContentType!.ToString(), await request.Content.ReadAsByteArrayAsync(token)));
            return Response(HttpStatusCode.OK, FileUrl);
        });
        using var unknownClient = new HttpClient(unknownHandler); var unknownProgress = new ProgressCapture();
        var unknown = UploadHttp.SendAsync(unknownClient, Endpoint, UploadRequestType.Post,
            new UploadModel("report.pdf", new NonSeekableStream(Payload), "application/pdf"), ".pdf", unknownProgress, default);
        Wait(() => unknown.IsCompleted);
        check(unknown.GetAwaiter().GetResult().SuccessFileUrl == FileUrl && unknownLengthRead && unknownProgress.Values[0] == 0 && unknownProgress.Values[^1] == 100,
            "Upload API: non-seekable files stream without a Length property and still report start and completion");

        using var oversizedHandler = new Handler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            { Content = new StreamContent(new NonSeekableStream(new byte[1024 * 1024 + 1])) }));
        using var oversizedClient = new HttpClient(oversizedHandler);
        var oversized = UploadHttp.SendAsync(oversizedClient, Endpoint, UploadRequestType.Post,
            new UploadModel("report.pdf", new MemoryStream(Payload)), ".pdf", null, default);
        Wait(() => oversized.IsCompleted); var limited = false;
        try { oversized.GetAwaiter().GetResult(); } catch (IOException) { limited = true; }
        check(limited, "Upload API: streaming responses with no Content-Length are bounded to 1 MiB");
    }

    private static void VerifyLoopback(Action<bool, string> check)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(8));
        var listener = new TcpListener(IPAddress.Loopback, 0); listener.Start();
        try
        {
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            var receive = Task.Run(async () =>
            {
                using var socket = await listener.AcceptTcpClientAsync(timeout.Token);
                await using var network = socket.GetStream();
                var headers = await ReadHeaders(network, timeout.Token);
                var body = await ReadBody(network, headers, timeout.Token);
                var response = Encoding.UTF8.GetBytes("{\"url\":\"" + FileUrl + "\"}");
                await network.WriteAsync(Encoding.ASCII.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {response.Length}\r\nConnection: close\r\n\r\n"), timeout.Token);
                await network.WriteAsync(response, timeout.Token);
                var contentType = headers.Split("\r\n").Single(line => line.StartsWith("Content-Type:", StringComparison.OrdinalIgnoreCase))[13..].Trim();
                return new RequestCapture(headers.Split(' ')[0], contentType, body);
            });
            using var client = new HttpClient(new SocketsHttpHandler { UseProxy = false });
            var operation = UploadHttp.SendAsync(client, $"http://127.0.0.1:{port}/upload", UploadRequestType.Post,
                new UploadModel("report.pdf", new MemoryStream(Payload), "application/pdf"), ".pdf", null, timeout.Token);
            Wait(() => operation.IsCompleted && receive.IsCompleted);
            check(operation.GetAwaiter().GetResult().SuccessFileUrl == FileUrl && IsSingleFileMultipart(receive.GetAwaiter().GetResult()),
                "Upload API: real loopback HTTP server receives interoperable multipart filename/filedata parts and returns the parsed URL");
        }
        finally { listener.Stop(); }
    }

    private static void VerifyControl(Action<bool, string> check)
    {
        var calls = 0;
        using var handler = new Handler((_, _) => Task.FromResult(Response(++calls == 1 ? HttpStatusCode.InternalServerError : HttpStatusCode.OK,
            "{\"url\":\"" + FileUrl + "\"}")));
        using var client = new HttpClient(handler);
        var upload = NewUpload(client); var window = Open(upload);
        check(!upload.RequestUploadCommand.CanExecute(null), "Upload API: an endpoint alone cannot submit an empty selection");
        upload.SelectedFile = File(); Flush();
        check(upload.RequestUploadCommand.CanExecute(null), "Upload API: valid endpoint and selected file enable the built-in upload command");
        var first = upload.UploadAsync(); Wait(() => first.IsCompleted);
        check(upload.ErrorMessage != null && upload.SuccessFileUrl == null && !upload.IsUploading && upload.RequestUploadCommand.CanExecute(null),
            "Upload API: failed HTTP upload clears the result, releases busy state and permits retry");
        upload.RequestUploadCommand.Execute(null); Wait(() => calls == 2 && !upload.IsUploading);
        check(calls == 2 && upload.ErrorMessage == null && upload.SuccessFileUrl == FileUrl && !upload.IsUploading,
            "Upload API: retry succeeds through the same native command path without replacing the selection");

        upload.HttpClient = new HttpClient(new Handler((_, _) => Task.FromResult(Response(HttpStatusCode.OK, "{\"ok\":true}"))));
        using var noUrlClient = upload.HttpClient;
        var noUrl = upload.UploadAsync(); Wait(() => noUrl.IsCompleted);
        check(upload.SuccessFileUrl == null && upload.ErrorMessage != null && upload.RequestUploadCommand.CanExecute(null),
            "Upload API: a 2xx response without a file URL clears the previous result and exposes a retryable error");

        upload.HttpClient = client; var command = new HostCommand(); upload.UploadCommand = command;
        var beforeHost = calls; var host = upload.UploadAsync(); Wait(() => host.IsCompleted);
        check(command.Calls == 1 && ReferenceEquals(command.Parameter, upload.SelectedFile) && calls == beforeHost,
            "Upload API: a supplied host command takes precedence over the built-in HTTP endpoint");
        command.Enabled = false; command.Notify(); Flush();
        check(!upload.RequestUploadCommand.CanExecute(null), "Upload API: host CanExecute cannot fall through to built-in HTTP");
        command.Enabled = true; command.Notify(); upload.IsBusy = true;
        check(!upload.RequestUploadCommand.CanExecute(null), "Upload API: host busy state also gates built-in or custom upload");
        upload.IsBusy = false; upload.UploadCommand = null; upload.UploadApi = "not a url";
        check(!upload.RequestUploadCommand.CanExecute(null), "Upload API: malformed upload endpoints cannot start a request");
        window.Close(); Flush();

        foreach (var invalidFile in new[]
        {
            File("blocked.exe"),
            new UploadFile("oversized.pdf", _ => Task.FromResult<Stream>(new MemoryStream(Payload)), 2048, "application/pdf")
        })
        {
            using var rejectedFile = invalidFile;
            var unattached = NewUpload(client); unattached.SelectedFile = rejectedFile; var beforeInvalid = calls;
            var invalidUpload = unattached.UploadAsync(); Wait(() => invalidUpload.IsCompleted);
            check(!unattached.RequestUploadCommand.CanExecute(null) && calls == beforeInvalid && unattached.SuccessFileUrl == null,
                $"Upload API/{invalidFile.Name}: unmounted validation gates requests (CanExecute={unattached.RequestUploadCommand.CanExecute(null)}, calls={calls}/{beforeInvalid}, SuccessFileUrl={unattached.SuccessFileUrl ?? "<null>"})");
        }
    }

    private static void VerifyCancellation(Action<bool, string> check)
    {
        foreach (var change in new[] { "selection", "endpoint", "method", "client", "command", "detach", "caller", "disabled" })
        {
            var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var completion = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            var requestToken = default(CancellationToken); var calls = 0;
            using var handler = new Handler((_, token) => { calls++; requestToken = token; started.TrySetResult(); return completion.Task; });
            using var client = new HttpClient(handler); using var replacementClient = new HttpClient(new Handler((_, _) => Task.FromResult(Response(HttpStatusCode.OK, "{}"))));
            using var cancellation = new CancellationTokenSource();
            var upload = NewUpload(client); upload.SelectedFile = File(); var window = Open(upload);
            upload.SuccessFileUrl = FileUrl;
            var operation = upload.UploadAsync(cancellation.Token); Wait(() => started.Task.IsCompleted);
            var progressBar = upload.GetVisualDescendants().OfType<ProgressBar>().Single(control => control.Name == "PART_Progress");
            check(upload.IsUploading && !upload.IsBusy && upload.SuccessFileUrl == null && !upload.SelectCommand.CanExecute(null) && !upload.RemoveCommand.CanExecute(null) && !upload.RequestUploadCommand.CanExecute(null),
                $"Upload API/{change}: pending upload clears the old URL and blocks duplicate submission and file actions");
            check(progressBar.IsVisible, $"Upload API/{change}: built-in busy state displays the native progress bar");
            var duplicate = upload.UploadAsync(); Wait(() => duplicate.IsCompleted);
            check(calls == 1, $"Upload API/{change}: direct UploadAsync calls also respect the in-flight guard");
            switch (change)
            {
                case "selection": upload.SelectedFile = File("replacement.pdf"); break;
                case "endpoint": upload.UploadApi = "https://upload.example.test/another"; break;
                case "method": upload.RequestType = UploadRequestType.Put; break;
                case "client": upload.HttpClient = replacementClient; break;
                case "command": upload.UploadCommand = new HostCommand(); break;
                case "detach": window.Close(); break;
                case "caller": cancellation.Cancel(); break;
                case "disabled": upload.IsEnabled = false; break;
            }
            check(requestToken.IsCancellationRequested, $"Upload API/{change}: changing request ownership cancels the previous transport");
            completion.SetResult(Response(HttpStatusCode.OK, "{\"url\":\"" + FileUrl + "\"}")); Wait(() => operation.IsCompleted);
            check(!upload.IsUploading && upload.SuccessFileUrl == null && upload.ErrorMessage == null,
                $"Upload API/{change}: late completion cannot publish a stale URL or error after cancellation");
            check(!progressBar.IsVisible, $"Upload API/{change}: cancellation or completion hides the progress bar");
            if (change != "detach") window.Close(); Flush();
        }

        var oldResponse = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously); var count = 0;
        using var raceHandler = new Handler((_, _) => ++count == 1 ? oldResponse.Task : Task.FromResult(Response(HttpStatusCode.OK, "https://cdn.example.test/new.pdf")));
        using var raceClient = new HttpClient(raceHandler); var raceUpload = NewUpload(raceClient); raceUpload.SelectedFile = File(); var raceWindow = Open(raceUpload);
        var oldRequest = raceUpload.UploadAsync(); Wait(() => count == 1);
        raceUpload.UploadApi = "https://upload.example.test/new";
        var newRequest = raceUpload.UploadAsync(); Wait(() => newRequest.IsCompleted);
        oldResponse.SetResult(Response(HttpStatusCode.OK, FileUrl)); Wait(() => oldRequest.IsCompleted);
        check(count == 2 && raceUpload.SuccessFileUrl == "https://cdn.example.test/new.pdf" && raceUpload.ErrorMessage == null,
            "Upload API: a cancelled request arriving after a newer successful request cannot overwrite its URL");
        raceWindow.Close(); Flush();
    }

    private static void VerifyBindings(Action<bool, string> check)
    {
        check(Upload.UploadApiProperty.GetMetadata(typeof(Upload)).DefaultBindingMode == BindingMode.TwoWay
            && Upload.SuccessFileUrlProperty.GetMetadata(typeof(Upload)).DefaultBindingMode == BindingMode.TwoWay,
            "Upload API: endpoint and successful result both default to TwoWay binding");
        var view = new UploadDemoView(); var window = Open(view);
        var upload = view.GetVisualDescendants().OfType<Upload>().Single(control => control.Name == "ApiUploadSample");
        view.Model.UploadApi = Endpoint; Flush();
        check(upload.UploadApi == Endpoint, "Upload API: compiled endpoint binding propagates ViewModel changes");
        upload.SetCurrentValue(Upload.UploadApiProperty, "https://upload.example.test/changed"); Flush();
        check(view.Model.UploadApi == upload.UploadApi, "Upload API: compiled TwoWay endpoint binding propagates control changes to the ViewModel");
        view.Model.SuccessFileUrl = FileUrl; Flush();
        check(upload.SuccessFileUrl == FileUrl, "Upload API: compiled result binding accepts an existing ViewModel URL");
        upload.SetCurrentValue(Upload.SuccessFileUrlProperty, null); Flush();
        check(view.Model.SuccessFileUrl == null, "Upload API: compiled TwoWay result binding propagates clearing to the ViewModel");
        window.Close(); Flush();
    }

    private static Upload NewUpload(HttpClient client) => new() { UploadApi = Endpoint, HttpClient = client, AllowedExtensions = ".pdf", Description = "PDF", MaxFileSize = 1024 };
    private static UploadFile File(string name = "report.pdf") => new(name, _ => Task.FromResult<Stream>(new MemoryStream(Payload, writable: false)), Payload.Length, "application/pdf");
    private static HttpResponseMessage Response(HttpStatusCode code, string body) => new(code) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static bool IsSingleFileMultipart(RequestCapture request)
    {
        var boundaryParameter = request.ContentType.Split(';').Select(part => part.Trim()).SingleOrDefault(part => part.StartsWith("boundary=", StringComparison.OrdinalIgnoreCase));
        if (!request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) || boundaryParameter == null) return false;
        var boundary = boundaryParameter[9..].Trim('"');
        var parts = Encoding.UTF8.GetString(request.Body).Split("--" + boundary, StringSplitOptions.RemoveEmptyEntries)
            .Where(part => part.StartsWith("\r\n", StringComparison.Ordinal)).ToArray();
        if (parts.Length != 2) return false;
        var namePart = parts.SingleOrDefault(part => HasDisposition(part, "filename"));
        var dataPart = parts.SingleOrDefault(part => HasDisposition(part, "filedata"));
        return namePart != null && dataPart != null && namePart[(namePart.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4)..] == "report.pdf\r\n"
            && (dataPart.Contains("filename=report.pdf", StringComparison.Ordinal) || dataPart.Contains("filename=\"report.pdf\"", StringComparison.Ordinal))
            && dataPart[(dataPart.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4)..] == Encoding.UTF8.GetString(Payload) + "\r\n"
            && dataPart.Contains("Content-Type: application/pdf", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasDisposition(string part, string name)
    {
        var headers = part[..part.IndexOf("\r\n\r\n", StringComparison.Ordinal)];
        return headers.Split(["\r\n", ";"], StringSplitOptions.TrimEntries).Any(value => value == "name=" + name || value == "name=\"" + name + "\"");
    }

    private static async Task<string> ReadHeaders(Stream stream, CancellationToken token)
    {
        using var result = new MemoryStream(); var single = new byte[1];
        while (result.Length < 32768)
        {
            if (await stream.ReadAsync(single, token) == 0) throw new EndOfStreamException();
            result.WriteByte(single[0]);
            if (result.Length >= 4 && Encoding.ASCII.GetString(result.GetBuffer(), (int)result.Length - 4, 4) == "\r\n\r\n") return Encoding.ASCII.GetString(result.ToArray());
        }
        throw new IOException("Loopback request headers exceeded the verification limit.");
    }

    private static async Task<byte[]> ReadBody(Stream stream, string headers, CancellationToken token)
    {
        using var result = new MemoryStream();
        if (headers.Contains("Transfer-Encoding: chunked", StringComparison.OrdinalIgnoreCase))
        {
            while (true)
            {
                var line = await ReadLine(stream, token); var length = Convert.ToInt32(line.Split(';')[0], 16);
                if (length == 0) { await ReadLine(stream, token); break; }
                var chunk = new byte[length]; await stream.ReadExactlyAsync(chunk, token); result.Write(chunk); await ReadLine(stream, token);
            }
        }
        else
        {
            var line = headers.Split("\r\n").Single(value => value.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase));
            var body = new byte[int.Parse(line[15..].Trim(), System.Globalization.CultureInfo.InvariantCulture)];
            await stream.ReadExactlyAsync(body, token); result.Write(body);
        }
        return result.ToArray();
    }

    private static async Task<string> ReadLine(Stream stream, CancellationToken token)
    {
        using var bytes = new MemoryStream(); var single = new byte[1];
        while (true)
        {
            if (await stream.ReadAsync(single, token) == 0) throw new EndOfStreamException();
            bytes.WriteByte(single[0]);
            if (single[0] == '\n') return Encoding.ASCII.GetString(bytes.ToArray()).TrimEnd('\r', '\n');
        }
    }

    private static Window Open(Control control)
    {
        var window = new Window { Width = 900, Height = 900, Content = control }; window.Show(); Flush(); return window;
    }

    private static void Wait(Func<bool> done)
    {
        var watch = Stopwatch.StartNew();
        while (!done() && watch.Elapsed < TimeSpan.FromSeconds(10))
        {
            var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; }; timer.Start(); Dispatcher.UIThread.PushFrame(frame); Flush();
        }
        if (!done()) throw new TimeoutException("Upload API verification did not finish."); Flush();
    }

    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private sealed record RequestCapture(string Method, string ContentType, byte[] Body);
    private sealed class Handler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => send(request, cancellationToken);
    }
    private sealed class ProgressCapture : IProgress<double>
    {
        public List<double> Values { get; } = [];
        public void Report(double value) => Values.Add(value);
    }
    private sealed class NonSeekableStream(byte[] data) : Stream
    {
        private readonly MemoryStream _stream = new(data, writable: false);
        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() => throw new NotSupportedException();
        public override int Read(byte[] buffer, int offset, int count) => _stream.Read(buffer, offset, count);
        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => _stream.ReadAsync(buffer, cancellationToken);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        protected override void Dispose(bool disposing) { if (disposing) _stream.Dispose(); base.Dispose(disposing); }
    }
    private sealed class HostCommand : ICommand
    {
        public bool Enabled { get; set; } = true;
        public int Calls { get; private set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => Enabled;
        public void Execute(object? parameter) { Calls++; Parameter = parameter; }
        public void Notify() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
