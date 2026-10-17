using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>Single-file selection and preview, with optional HTTP upload or a host command.</summary>
[PseudoClasses(":has-file", ":loading", ":error", ":busy")]
public partial class Upload : TemplatedControl
{
    public static readonly StyledProperty<UploadFile?> SelectedFileProperty =
        AvaloniaProperty.Register<Upload, UploadFile?>(nameof(SelectedFile), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand?> UploadCommandProperty =
        AvaloniaProperty.Register<Upload, ICommand?>(nameof(UploadCommand));

    public static readonly StyledProperty<IUploadFilePicker?> FilePickerProperty =
        AvaloniaProperty.Register<Upload, IUploadFilePicker?>(nameof(FilePicker));

    public static readonly StyledProperty<bool>
        IsBusyProperty = AvaloniaProperty.Register<Upload, bool>(nameof(IsBusy));

    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<Upload, double>(nameof(Progress),
            validate: value => double.IsFinite(value) && value is >= 0 and <= 100);

    public static readonly StyledProperty<string?> StatusTextProperty =
        AvaloniaProperty.Register<Upload, string?>(nameof(StatusText));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(Title), "上传图片");

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(Description), "PNG、JPG、WebP 等图片 · 单文件");

    public static readonly StyledProperty<string> SelectTextProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(SelectText), "选择图片");

    public static readonly StyledProperty<string> ReplaceTextProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(ReplaceText), "替换");

    public static readonly StyledProperty<string> RemoveTextProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(RemoveText), "移除");

    public static readonly StyledProperty<string> UploadTextProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(UploadText), "上传文件");

    public static readonly StyledProperty<string> AllowedExtensionsProperty =
        AvaloniaProperty.Register<Upload, string>(nameof(AllowedExtensions), ".png,.jpg,.jpeg,.webp,.bmp,.gif");

    public static readonly StyledProperty<long> MaxFileSizeProperty =
        AvaloniaProperty.Register<Upload, long>(nameof(MaxFileSize), 10 * 1024 * 1024, validate: value => value >= 0);

    public static readonly StyledProperty<IDataTemplate?> EmptyTemplateProperty =
        AvaloniaProperty.Register<Upload, IDataTemplate?>(nameof(EmptyTemplate));

    public static readonly StyledProperty<IDataTemplate?> FileTemplateProperty =
        AvaloniaProperty.Register<Upload, IDataTemplate?>(nameof(FileTemplate));

    public static readonly StyledProperty<IDataTemplate?> ActionsTemplateProperty =
        AvaloniaProperty.Register<Upload, IDataTemplate?>(nameof(ActionsTemplate));

    public static readonly DirectProperty<Upload, IImage?> PreviewImageProperty =
        AvaloniaProperty.RegisterDirect<Upload, IImage?>(nameof(PreviewImage), owner => owner.PreviewImage);

    public static readonly DirectProperty<Upload, bool> HasFileProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(nameof(HasFile), owner => owner.HasFile);

    public static readonly DirectProperty<Upload, bool> HasPreviewProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(nameof(HasPreview), owner => owner.HasPreview);

    public static readonly DirectProperty<Upload, bool> IsPreviewLoadingProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(nameof(IsPreviewLoading), owner => owner.IsPreviewLoading);

    public static readonly DirectProperty<Upload, bool> IsSelectingProperty =
        AvaloniaProperty.RegisterDirect<Upload, bool>(nameof(IsSelecting), owner => owner.IsSelecting);

    public static readonly DirectProperty<Upload, string?> ErrorMessageProperty =
        AvaloniaProperty.RegisterDirect<Upload, string?>(nameof(ErrorMessage), owner => owner.ErrorMessage);

    private string? _error;
    private bool _hasFile, _hasPreview, _loading, _selecting, _attached;
    private ICommand? _observedUpload;
    private IImage? _preview;
    private CancellationTokenSource? _previewCancellation, _pickCancellation;
    private string? _previewError;
    private int _version, _pickVersion;

    public Upload()
    {
        SelectCommand = new UploadAction(() => _ = SelectAsync(),
            () => IsEffectivelyEnabled && !IsBusy && !IsUploading && !IsSelecting);
        RemoveCommand = new UploadAction(() => SetCurrentValue(SelectedFileProperty, null),
            () => IsEffectivelyEnabled && !IsBusy && !IsUploading && HasFile);
        RequestUploadCommand = new UploadAction(() => _ = UploadAsync(), CanUpload);
    }

    public UploadFile? SelectedFile
    {
        get => GetValue(SelectedFileProperty);
        set => SetValue(SelectedFileProperty, value);
    }

    public ICommand? UploadCommand
    {
        get => GetValue(UploadCommandProperty);
        set => SetValue(UploadCommandProperty, value);
    }

    public IUploadFilePicker? FilePicker
    {
        get => GetValue(FilePickerProperty);
        set => SetValue(FilePickerProperty, value);
    }

    public bool IsBusy
    {
        get => GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public string? StatusText
    {
        get => GetValue(StatusTextProperty);
        set => SetValue(StatusTextProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string SelectText
    {
        get => GetValue(SelectTextProperty);
        set => SetValue(SelectTextProperty, value);
    }

    public string ReplaceText
    {
        get => GetValue(ReplaceTextProperty);
        set => SetValue(ReplaceTextProperty, value);
    }

    public string RemoveText
    {
        get => GetValue(RemoveTextProperty);
        set => SetValue(RemoveTextProperty, value);
    }

    public string UploadText
    {
        get => GetValue(UploadTextProperty);
        set => SetValue(UploadTextProperty, value);
    }

    public string AllowedExtensions
    {
        get => GetValue(AllowedExtensionsProperty);
        set => SetValue(AllowedExtensionsProperty, value);
    }

    public long MaxFileSize
    {
        get => GetValue(MaxFileSizeProperty);
        set => SetValue(MaxFileSizeProperty, value);
    }

    public IDataTemplate? EmptyTemplate
    {
        get => GetValue(EmptyTemplateProperty);
        set => SetValue(EmptyTemplateProperty, value);
    }

    public IDataTemplate? FileTemplate
    {
        get => GetValue(FileTemplateProperty);
        set => SetValue(FileTemplateProperty, value);
    }

    public IDataTemplate? ActionsTemplate
    {
        get => GetValue(ActionsTemplateProperty);
        set => SetValue(ActionsTemplateProperty, value);
    }

    public IImage? PreviewImage => _preview;
    public bool HasFile => _hasFile;
    public bool HasPreview => _hasPreview;
    public bool IsPreviewLoading => _loading;
    public bool IsSelecting => _selecting;
    public string? ErrorMessage => _error;
    public ICommand SelectCommand { get; }
    public ICommand RemoveCommand { get; }
    public ICommand RequestUploadCommand { get; }
    protected override Type StyleKeyOverride => typeof(Upload);

    private bool CanUpload()
    {
        return IsEffectivelyEnabled && !IsBusy && !IsUploading && !IsPreviewLoading && _previewError is null &&
               SelectedFile is { } file && ValidateFile(file) is null
               && (UploadCommand is { } command ? command.CanExecute(file) : IsUploadEndpoint(UploadApi));
    }

    private void RefreshCommands()
    {
        ((UploadAction)SelectCommand).Notify();
        ((UploadAction)RemoveCommand).Notify();
        ((UploadAction)RequestUploadCommand).Notify();
    }

    private void ObserveCommand()
    {
        var command = _attached ? UploadCommand : null;
        if (ReferenceEquals(command, _observedUpload)) return;
        if (_observedUpload != null) _observedUpload.CanExecuteChanged -= UploadCanExecuteChanged;
        _observedUpload = command;
        if (_observedUpload != null) _observedUpload.CanExecuteChanged += UploadCanExecuteChanged;
        RefreshCommands();
    }

    private void UploadCanExecuteChanged(object? sender, EventArgs e)
    {
        if (Dispatcher.UIThread.CheckAccess()) RefreshCommands();
        else Dispatcher.UIThread.Post(RefreshCommands);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        ObserveCommand();
        ReloadPreview();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        InvalidateHttpUpload(false);
        CancelPicker();
        CancelPreview();
        SetPreview(null);
        SetLoading(false);
        ObserveCommand();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedFileProperty)
        {
            InvalidateHttpUpload(true);
            CancelPicker();
            SetAndRaise(HasFileProperty, ref _hasFile, SelectedFile != null);
            PseudoClasses.Set(":has-file", HasFile);
            ReloadPreview();
        }

        if (change.Property == UploadCommandProperty)
        {
            InvalidateHttpUpload(true);
            ObserveCommand();
        }

        if (change.Property == UploadApiProperty || change.Property == RequestTypeProperty ||
            change.Property == HttpClientProperty) InvalidateHttpUpload(true);
        if (change.Property == FilePickerProperty) CancelPicker();
        if (change.Property == MaxFileSizeProperty || change.Property == AllowedExtensionsProperty)
        {
            InvalidateHttpUpload(true);
            ReloadPreview();
        }

        if (change.Property == IsEffectivelyEnabledProperty || change.Property == IsBusyProperty)
        {
            if (!IsEffectivelyEnabled || IsBusy) CancelPicker();
            if (!IsEffectivelyEnabled) InvalidateHttpUpload(false);
            PseudoClasses.Set(":busy", IsBusy || IsUploading);
            RefreshCommands();
        }
    }

    private void SetError(string? error)
    {
        _previewError = error;
        UpdateDisplayedError();
    }

    private void UpdateDisplayedError()
    {
        SetAndRaise(ErrorMessageProperty, ref _error, _previewError ?? _uploadError);
        PseudoClasses.Set(":error", _error != null);
        RefreshCommands();
    }

    private void SetLoading(bool loading)
    {
        SetAndRaise(IsPreviewLoadingProperty, ref _loading, loading);
        PseudoClasses.Set(":loading", loading);
        RefreshCommands();
    }

    private void SetSelecting(bool selecting)
    {
        SetAndRaise(IsSelectingProperty, ref _selecting, selecting);
        RefreshCommands();
    }

    private void SetPreview(Bitmap? bitmap)
    {
        var old = _preview as Bitmap;
        SetAndRaise(PreviewImageProperty, ref _preview, bitmap);
        SetAndRaise(HasPreviewProperty, ref _hasPreview, bitmap != null);
        old?.Dispose();
    }

    private void CancelPreview()
    {
        _version++;
        _previewCancellation?.Cancel();
        _previewCancellation?.Dispose();
        _previewCancellation = null;
    }

    private void CancelPicker()
    {
        _pickVersion++;
        _pickCancellation?.Cancel();
        _pickCancellation?.Dispose();
        _pickCancellation = null;
        SetSelecting(false);
    }

    private string? ValidateFile(UploadFile file)
    {
        var extensions = Extensions();
        if (extensions.Length > 0 &&
            !extensions.Contains(Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase))
            return "不支持此文件类型，请选择图片。";
        if (MaxFileSize > 0 && file.Size is { } size && size > MaxFileSize)
            return $"文件超过大小限制（{MaxFileSize / 1024d / 1024:0.#} MB）。";
        return null;
    }

    private string[] Extensions()
    {
        return (AllowedExtensions ?? "")
            .Split([',', ';', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.StartsWith('.') ? value : "." + value).ToArray();
    }

    /// <summary>Selects one file through the host service or native storage provider. Cancellation preserves the current file.</summary>
    public async Task SelectAsync(CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        if (!SelectCommand.CanExecute(null)) return;
        if (TopLevel.GetTopLevel(this) is not { } topLevel)
        {
            SetError("控件尚未连接到窗口。");
            return;
        }

        CancelPicker();
        var version = _pickVersion;
        _pickCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _pickCancellation.Token;
        SetSelecting(true);
        UploadFile? file = null;
        try
        {
            if (FilePicker != null)
            {
                file = await FilePicker.PickAsync(topLevel, token);
            }
            else
            {
                if (!topLevel.StorageProvider.CanOpen)
                {
                    SetError("当前平台不支持选择文件。");
                    return;
                }

                var patterns = Extensions().Select(extension => "*" + extension).ToArray();
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = Title, AllowMultiple = false,
                    FileTypeFilter = patterns.Length > 0 ? [new FilePickerFileType("图片") { Patterns = patterns }] : null
                });
                for (var index = 1; index < files.Count; index++) files[index].Dispose();
                file = files.Count > 0 ? await UploadFile.FromStorageFileAsync(files[0], token, true) : null;
            }

            if (version != _pickVersion || token.IsCancellationRequested || !_attached || !IsEffectivelyEnabled ||
                IsBusy || IsUploading || file == null) return;
            if (ValidateFile(file) is { } error)
            {
                SetError(error);
                return;
            }

            if (ReferenceEquals(SelectedFile, file)) ReloadPreview();
            else SetCurrentValue(SelectedFileProperty, file);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception error)
        {
            if (version == _pickVersion && _attached) SetError("选择失败：" + error.Message);
        }
        finally
        {
            if (file != null && !ReferenceEquals(file, SelectedFile)) file.Dispose();
            if (version == _pickVersion) CancelPicker();
        }
    }

    /// <summary>Reloads the thumbnail from a fresh file stream. The control owns and releases decoded thumbnails.</summary>
    public void ReloadPreview()
    {
        VerifyAccess();
        CancelPreview();
        SetPreview(null);
        SetError(null);
        SetLoading(false);
        if (!_attached || SelectedFile is not { } file) return;
        if (ValidateFile(file) is { } error)
        {
            SetError(error);
            return;
        }

        if (!IsImageFile(file)) return;
        _previewCancellation = new CancellationTokenSource();
        SetLoading(true);
        _ = LoadPreviewAsync(file, _version, _previewCancellation.Token);
    }

    private static bool IsImageFile(UploadFile file)
    {
        return file.ContentType?.StartsWith("image/", StringComparison.OrdinalIgnoreCase) == true ||
               new[] { ".png", ".jpg", ".jpeg", ".webp", ".bmp", ".gif", ".ico", ".tif", ".tiff" }.Contains(
                   Path.GetExtension(file.Name), StringComparer.OrdinalIgnoreCase);
    }

    private async Task LoadPreviewAsync(UploadFile file, int version, CancellationToken token)
    {
        Bitmap? bitmap = null;
        string? error = null;
        try
        {
            var maximum = MaxFileSize > 0 ? Math.Min(MaxFileSize, 64 * 1024 * 1024) : 64 * 1024 * 1024;
            bitmap = await Task.Run(async () =>
            {
                await using var stream = await file.OpenReadAsync(token).ConfigureAwait(false);
                using var buffer = new MemoryStream();
                var bytes = new byte[81920];
                int count;
                while ((count = await stream.ReadAsync(bytes, token).ConfigureAwait(false)) > 0)
                {
                    if (buffer.Length + count > maximum) throw new IOException("图片超过预览大小限制。");
                    buffer.Write(bytes, 0, count);
                }

                token.ThrowIfCancellationRequested();
                buffer.Position = 0;
                return Bitmap.DecodeToWidth(buffer, 640, BitmapInterpolationMode.MediumQuality);
            }, token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception exception)
        {
            error = "无法预览此图片：" + exception.Message;
        }

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (version != _version || token.IsCancellationRequested || !_attached)
            {
                bitmap?.Dispose();
                return;
            }

            SetPreview(bitmap);
            SetError(error);
            SetLoading(false);
        });
    }

    private sealed class UploadAction(Action execute, Func<bool> canExecute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return canExecute();
        }

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter)) execute();
        }

        public void Notify()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}