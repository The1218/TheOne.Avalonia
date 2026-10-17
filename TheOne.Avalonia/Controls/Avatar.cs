using System.Globalization;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

public enum AvatarShape
{
    Circle,
    Rounded,
    Square
}

/// <summary>An asynchronously loaded avatar with text or person-icon fallback.</summary>
public class Avatar : TemplatedControl
{
    public static readonly StyledProperty<object?> SourceProperty =
        AvaloniaProperty.Register<Avatar, object?>(nameof(Source));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<Avatar, string?>(nameof(Text));

    public static readonly StyledProperty<AvatarShape> ShapeProperty =
        AvaloniaProperty.Register<Avatar, AvatarShape>(nameof(Shape), validate: Enum.IsDefined);

    public static readonly StyledProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<Avatar, Stretch>(nameof(Stretch), Stretch.UniformToFill, validate: Enum.IsDefined);

    public static readonly DirectProperty<Avatar, IImage?> CurrentImageProperty =
        AvaloniaProperty.RegisterDirect<Avatar, IImage?>(nameof(CurrentImage), owner => owner.CurrentImage);

    public static readonly DirectProperty<Avatar, bool> IsLoadingProperty =
        AvaloniaProperty.RegisterDirect<Avatar, bool>(nameof(IsLoading), owner => owner.IsLoading);

    public static readonly DirectProperty<Avatar, bool> HasImageProperty =
        AvaloniaProperty.RegisterDirect<Avatar, bool>(nameof(HasImage), owner => owner.HasImage);

    public static readonly DirectProperty<Avatar, string?> ErrorMessageProperty =
        AvaloniaProperty.RegisterDirect<Avatar, string?>(nameof(ErrorMessage), owner => owner.ErrorMessage);

    public static readonly DirectProperty<Avatar, string> DisplayTextProperty =
        AvaloniaProperty.RegisterDirect<Avatar, string>(nameof(DisplayText), owner => owner.DisplayText);

    public static readonly DirectProperty<Avatar, CornerRadius> EffectiveCornerRadiusProperty =
        AvaloniaProperty.RegisterDirect<Avatar, CornerRadius>(nameof(EffectiveCornerRadius),
            owner => owner.EffectiveCornerRadius);

    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private CancellationTokenSource? _cancellation;
    private string _displayText = "";
    private string? _error;
    private IImage? _image;
    private bool _loading, _hasImage, _attached;
    private Bitmap? _ownedImage;
    private CornerRadius _radius = new(999999);
    private int _version;

    public Avatar()
    {
        UpdateAppearance();
    }

    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public AvatarShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

    public Stretch Stretch
    {
        get => GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public IImage? CurrentImage => _image;
    public bool IsLoading => _loading;
    public bool HasImage => _hasImage;
    public string? ErrorMessage => _error;
    public string DisplayText => _displayText;
    public CornerRadius EffectiveCornerRadius => _radius;
    protected override Type StyleKeyOverride => typeof(Avatar);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty && _attached) Reload();
        if (change.Property == TextProperty || change.Property == ShapeProperty ||
            change.Property == CornerRadiusProperty) UpdateAppearance();
    }

    private void UpdateAppearance()
    {
        var text = (Text ?? "").Trim();
        var elements = StringInfo.ParseCombiningCharacters(text);
        SetAndRaise(DisplayTextProperty, ref _displayText, elements.Length > 2 ? text[..elements[2]] : text);
        SetAndRaise(EffectiveCornerRadiusProperty, ref _radius, Shape switch
        {
            AvatarShape.Circle => new CornerRadius(999999),
            AvatarShape.Rounded => CornerRadius,
            _ => default
        });
        PseudoClasses.Set(":has-text", _displayText.Length > 0);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _attached = true;
        Reload();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        _attached = false;
        CancelLoad();
        SetImage(null, null);
        SetLoading(false);
        base.OnDetachedFromVisualTree(args);
    }

    /// <summary>Reloads the current source while attached, for example after a failed request.</summary>
    public void Reload()
    {
        VerifyAccess();
        if (!_attached) return;
        CancelLoad();
        SetImage(null, null);
        SetError(null);
        SetLoading(false);
        if (Source is null || (Source is string text && string.IsNullOrWhiteSpace(text))) return;
        if (Source is IImage image)
        {
            SetImage(image, null);
            return;
        }

        _cancellation = new CancellationTokenSource();
        SetLoading(true);
        _ = LoadAsync(Source, _version, _cancellation.Token);
    }

    private void CancelLoad()
    {
        _version++;
        _cancellation?.Cancel();
        _cancellation?.Dispose();
        _cancellation = null;
    }

    private void SetImage(IImage? image, Bitmap? owned)
    {
        var previous = _ownedImage;
        _ownedImage = owned;
        SetAndRaise(CurrentImageProperty, ref _image, image);
        SetAndRaise(HasImageProperty, ref _hasImage, image is not null);
        PseudoClasses.Set(":has-image", image is not null);
        previous?.Dispose();
    }

    private void SetLoading(bool value)
    {
        SetAndRaise(IsLoadingProperty, ref _loading, value);
        PseudoClasses.Set(":loading", value);
    }

    private void SetError(string? value)
    {
        SetAndRaise(ErrorMessageProperty, ref _error, value);
        PseudoClasses.Set(":error", value is not null);
    }

    private async Task LoadAsync(object source, int version, CancellationToken token)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(token);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        var loadToken = timeout.Token;
        Bitmap? bitmap = null;
        string? error = null;
        try
        {
            bitmap = await Task.Run(async () =>
            {
                var path = source switch
                {
                    string value => value, Uri uri => uri.ToString(),
                    _ => throw new NotSupportedException("Source must be a local path, URI or IImage.")
                };
                if (Uri.TryCreate(path, UriKind.Absolute, out var address))
                {
                    if (address.Scheme is "http" or "https")
                    {
                        using var response = await Client
                            .GetAsync(address, HttpCompletionOption.ResponseHeadersRead, loadToken)
                            .ConfigureAwait(false);
                        response.EnsureSuccessStatusCode();
                        await using var remote =
                            await response.Content.ReadAsStreamAsync(loadToken).ConfigureAwait(false);
                        using var buffer = new MemoryStream();
                        var bytes = new byte[81920];
                        int count;
                        while ((count = await remote.ReadAsync(bytes, loadToken).ConfigureAwait(false)) > 0)
                        {
                            if (buffer.Length + count > 16 * 1024 * 1024)
                                throw new InvalidDataException("Avatar download exceeds 16 MiB.");
                            buffer.Write(bytes, 0, count);
                        }

                        buffer.Position = 0;
                        loadToken.ThrowIfCancellationRequested();
                        return new Bitmap(buffer);
                    }

                    if (address.Scheme == "avares")
                    {
                        using var asset = AssetLoader.Open(address);
                        loadToken.ThrowIfCancellationRequested();
                        return new Bitmap(asset);
                    }

                    if (address.IsFile) path = address.LocalPath;
                    else
                        throw new NotSupportedException(
                            "Only local files, avares and HTTP(S) addresses are supported.");
                }

                loadToken.ThrowIfCancellationRequested();
                using var file = File.OpenRead(path);
                return new Bitmap(file);
            }, loadToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            if (!token.IsCancellationRequested)
                error = exception is OperationCanceledException ? "Avatar loading timed out." : exception.Message;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (!_attached || token.IsCancellationRequested || version != _version)
            {
                bitmap?.Dispose();
                return;
            }

            _cancellation?.Dispose();
            _cancellation = null;
            SetImage(bitmap, bitmap);
            SetError(error);
            SetLoading(false);
        });
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new AvatarAutomationPeer(this);
    }

    private sealed class AvatarAutomationPeer(Avatar owner) : ControlAutomationPeer(owner)
    {
        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Image;
        }

        protected override string? GetNameCore()
        {
            return !string.IsNullOrEmpty(base.GetNameCore()) ? base.GetNameCore() :
                !string.IsNullOrWhiteSpace(owner.Text) ? owner.Text : "Avatar";
        }
    }
}