using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public partial class ImagePreview
{
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private CancellationTokenSource? _loadCancellation;
    private int _loadVersion;
    private Bitmap? _ownedImage;

    private void CancelLoad()
    {
        _loadVersion++;
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = null;
    }

    private void ReleaseImage()
    {
        CurrentImage = null;
        _ownedImage?.Dispose();
        _ownedImage = null;
        HasImage = false;
        _surface?.InvalidateVisual();
    }

    private void StartLoad(object? source)
    {
        CancelLoad();
        ReleaseImage();
        ErrorMessage = null;
        IsLoading = false;
        _surface?.ResetPan();
        SetCurrentValue(IsFitToWindowProperty, true);
        if (source is null)
        {
            UpdateView();
            return;
        }

        if (source is IImage image)
        {
            // Never dispose images supplied by the consumer.
            CurrentImage = image;
            UpdateView();
            _surface?.UpdateFit();
            return;
        }

        _loadCancellation = new CancellationTokenSource();
        var token = _loadCancellation.Token;
        var version = _loadVersion;
        IsLoading = true;
        UpdateView();
        _ = LoadAsync(source, version, token);
    }

    private async Task LoadAsync(object source, int version, CancellationToken token)
    {
        Bitmap? bitmap = null;
        string? error = null;
        try
        {
            bitmap = await Task.Run(async () =>
            {
                var path = source switch
                {
                    string text => text, Uri uri => uri.ToString(),
                    _ => throw new NotSupportedException("Source must be a Bitmap, path or HTTP(S) address.")
                };
                if (Uri.TryCreate(path, UriKind.Absolute, out var address))
                {
                    if (address.Scheme is "http" or "https")
                    {
                        using var response = await Client
                            .GetAsync(address, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        await using var remote = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
                        using var buffer = new MemoryStream();
                        await remote.CopyToAsync(buffer, token).ConfigureAwait(false);
                        buffer.Position = 0;
                        token.ThrowIfCancellationRequested();
                        return new Bitmap(buffer);
                    }

                    if (address.IsFile) path = address.LocalPath;
                    else throw new NotSupportedException("Only HTTP(S) and local files are supported.");
                }

                token.ThrowIfCancellationRequested();
                using var file = File.OpenRead(path);
                return new Bitmap(file);
            }, token).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            if (!token.IsCancellationRequested) error = exception.Message;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (!_attached || token.IsCancellationRequested || version != _loadVersion)
            {
                bitmap?.Dispose();
                return;
            }

            CurrentImage = _ownedImage = bitmap;
            ErrorMessage = error;
            IsLoading = false;
            UpdateView();
            _surface?.UpdateFit();
        });
    }
}