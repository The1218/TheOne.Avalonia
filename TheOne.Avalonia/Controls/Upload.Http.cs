using System.Net;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public partial class Upload
{
    public static readonly StyledProperty<string?> UploadApiProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(UploadApi), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<UploadRequestType> RequestTypeProperty =
        AvaloniaProperty.Register<Upload, UploadRequestType>(nameof(RequestType),
            validate: value => Enum.IsDefined(value));

    public static readonly StyledProperty<string?> SuccessFileUrlProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(SuccessFileUrl), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<HttpClient?> HttpClientProperty =
        AvaloniaProperty.Register<Upload, HttpClient?>(nameof(HttpClient));

    public static readonly DirectProperty<Upload, bool> IsUploadingProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(nameof(IsUploading), owner => owner.IsUploading);

    private static readonly HttpClient SharedUploadClient = new(new HttpClientHandler
        { AutomaticDecompression = DecompressionMethods.All, AllowAutoRedirect = false });

    private CancellationTokenSource? _uploadCancellation;
    private string? _uploadError;
    private int _uploadVersion;
    private bool _uploading;

    /// <summary>Absolute HTTP(S) endpoint. Binds two-way by default.</summary>
    public string? UploadApi
    {
        get => GetValue(UploadApiProperty);
        set => SetValue(UploadApiProperty, value);
    }

    public UploadRequestType RequestType
    {
        get => GetValue(RequestTypeProperty);
        set => SetValue(RequestTypeProperty, value);
    }

    /// <summary>First valid file URL extracted from the successful response. Binds two-way by default.</summary>
    public string? SuccessFileUrl
    {
        get => GetValue(SuccessFileUrlProperty);
        set => SetValue(SuccessFileUrlProperty, value);
    }

    /// <summary>Optional consumer-owned client for authentication, headers and custom handlers.</summary>
    public HttpClient? HttpClient
    {
        get => GetValue(HttpClientProperty);
        set => SetValue(HttpClientProperty, value);
    }

    public bool IsUploading => _uploading;

    private static bool IsUploadEndpoint(string? endpoint)
    {
        return Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
               && uri.Scheme is "http" or "https" && uri.IsWellFormedOriginalString() &&
               !string.IsNullOrEmpty(uri.Host) && Uri.CheckHostName(uri.Host) != UriHostNameType.Unknown;
    }

    private void SetUploading(bool value)
    {
        if (value) CancelPicker();
        SetAndRaise(IsUploadingProperty, ref _uploading, value);
        PseudoClasses.Set(":busy", IsBusy || value);
        RefreshCommands();
    }

    private void SetUploadError(string? error)
    {
        _uploadError = error;
        UpdateDisplayedError();
    }

    private void InvalidateHttpUpload(bool clearResult)
    {
        _uploadVersion++;
        var cancellation = _uploadCancellation;
        _uploadCancellation = null;
        cancellation?.Cancel(); // The operation owns disposal after its continuations finish.
        if (IsUploading)
        {
            SetUploading(false);
            SetCurrentValue(StatusTextProperty, "上传已取消。");
        }

        if (clearResult)
        {
            SetCurrentValue(SuccessFileUrlProperty, null);
            SetUploadError(null);
            if (UploadCommand is null) SetCurrentValue(StatusTextProperty, null);
        }

        RefreshCommands();
    }

    /// <summary>
    ///     Uploads explicitly. A supplied UploadCommand takes precedence over built-in HTTP.
    ///     Errors are exposed through ErrorMessage; cancellation and stale responses never publish a URL.
    /// </summary>
    public async Task UploadAsync(CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        if (!CanUpload() || SelectedFile is not { } file) return;
        if (UploadCommand is { } command)
        {
            command.Execute(file);
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();
        var endpoint = UploadApi!;
        var method = RequestType;
        var extensions = AllowedExtensions;
        var client = HttpClient ?? SharedUploadClient;
        var version = ++_uploadVersion;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _uploadCancellation = cancellation;
        SetCurrentValue(SuccessFileUrlProperty, null);
        SetUploadError(null);
        SetCurrentValue(ProgressProperty, 0d);
        SetCurrentValue(StatusTextProperty, "正在上传…");
        SetUploading(true);
        var progress = new UploadProgress(value => Dispatcher.UIThread.Post(() =>
        {
            if (version == _uploadVersion && IsUploading && double.IsFinite(value))
                SetCurrentValue(ProgressProperty, Math.Clamp(value, 0, 100));
        }));
        try
        {
            await using var stream = await file.OpenReadAsync(cancellation.Token);
            var model = new UploadModel(file.Name, stream, file.ContentType);
            var result = await UploadHttp.SendAsync(client, endpoint, method, model, extensions, progress,
                cancellation.Token);
            if (version != _uploadVersion || cancellation.IsCancellationRequested) return;
            if (result.SuccessFileUrl is null) throw new InvalidDataException("上传接口响应中未找到合法的 HTTP(S) 文件地址。");
            SetCurrentValue(SuccessFileUrlProperty, result.SuccessFileUrl);
            SetCurrentValue(ProgressProperty, 100d);
            SetCurrentValue(StatusTextProperty, "上传成功。");
        }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
        {
            if (version == _uploadVersion) SetCurrentValue(StatusTextProperty, "上传已取消。");
        }
        catch (Exception error)
        {
            if (version == _uploadVersion)
            {
                SetCurrentValue(SuccessFileUrlProperty, null);
                SetUploadError("上传失败：" + error.Message);
                SetCurrentValue(StatusTextProperty, "上传失败，请重试。");
            }
        }
        finally
        {
            if (version == _uploadVersion)
            {
                _uploadCancellation = null;
                SetUploading(false);
            }
        }
    }

    private sealed class UploadProgress(Action<double> report) : IProgress<double>
    {
        public void Report(double value)
        {
            report(value);
        }
    }
}