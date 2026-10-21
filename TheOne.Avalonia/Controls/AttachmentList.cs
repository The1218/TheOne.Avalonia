using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace TheOne.Avalonia.Controls;

/// <summary>A multi-file picker and attachment presenter. The consumer owns accepted selections and business commands.</summary>
public sealed class AttachmentList : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<AttachmentList, string>(nameof(Title), "附件上传");
    public static readonly StyledProperty<string?> DescriptionProperty = AvaloniaProperty.Register<AttachmentList, string?>(nameof(Description));
    public static readonly StyledProperty<string> UploadButtonTextProperty = AvaloniaProperty.Register<AttachmentList, string>(nameof(UploadButtonText), "上传附件");
    public static readonly StyledProperty<string> EmptyTextProperty = AvaloniaProperty.Register<AttachmentList, string>(nameof(EmptyText), "暂无附件");
    public static readonly StyledProperty<string?> HintTextProperty = AvaloniaProperty.Register<AttachmentList, string?>(nameof(HintText), "可一次选择多个文件。");
    public static readonly StyledProperty<bool> IsUploadVisibleProperty = AvaloniaProperty.Register<AttachmentList, bool>(nameof(IsUploadVisible), true);
    public static readonly StyledProperty<bool> IsRemoveVisibleProperty = AvaloniaProperty.Register<AttachmentList, bool>(nameof(IsRemoveVisible), true);
    public static readonly StyledProperty<bool> IsBusyProperty = AvaloniaProperty.Register<AttachmentList, bool>(nameof(IsBusy));
    public static readonly StyledProperty<IEnumerable<IAttachmentItem>?> FilesProperty = AvaloniaProperty.Register<AttachmentList, IEnumerable<IAttachmentItem>?>(nameof(Files));
    public static readonly StyledProperty<ICommand?> UploadCommandProperty = AvaloniaProperty.Register<AttachmentList, ICommand?>(nameof(UploadCommand));
    public static readonly StyledProperty<ICommand?> RemoveCommandProperty = AvaloniaProperty.Register<AttachmentList, ICommand?>(nameof(RemoveCommand));
    public static readonly StyledProperty<IAttachmentFilePicker?> FilePickerProperty = AvaloniaProperty.Register<AttachmentList, IAttachmentFilePicker?>(nameof(FilePicker));
    public static readonly DirectProperty<AttachmentList, bool> HasFilesProperty = AvaloniaProperty.RegisterDirect<AttachmentList, bool>(nameof(HasFiles), owner => owner.HasFiles);
    public static readonly DirectProperty<AttachmentList, bool> IsSelectingProperty = AvaloniaProperty.RegisterDirect<AttachmentList, bool>(nameof(IsSelecting), owner => owner.IsSelecting);
    public static readonly DirectProperty<AttachmentList, string?> ErrorMessageProperty = AvaloniaProperty.RegisterDirect<AttachmentList, string?>(nameof(ErrorMessage), owner => owner.ErrorMessage);
    public static readonly RoutedEvent<AttachmentPreviewRequestedEventArgs> PreviewRequestedEvent = RoutedEvent.Register<AttachmentList, AttachmentPreviewRequestedEventArgs>(nameof(PreviewRequested), RoutingStrategies.Bubble);

    private bool _attached, _hasFiles, _selecting;
    private string? _error;
    private int _selectionVersion;
    private CancellationTokenSource? _selectionCancellation;
    private INotifyCollectionChanged? _observedFiles;
    private readonly HashSet<IAttachmentItem> _observedItems = new(ReferenceEqualityComparer.Instance);
    private ICommand? _observedUpload, _observedRemove;

    public AttachmentList()
    {
        SelectCommand = new AttachmentAction(_ => _ = SelectFilesAsync(), _ => CanSelect());
        RequestRemoveCommand = new AttachmentAction(parameter =>
        {
            if (parameter is IAttachmentItem item) RemoveCommand?.Execute(item);
        }, parameter => parameter is IAttachmentItem item && CanRemoveItem(item));
        RequestPreviewCommand = new AttachmentAction(parameter =>
        {
            if (parameter is IAttachmentItem item) RaiseEvent(new AttachmentPreviewRequestedEventArgs(item));
        }, parameter => parameter is IAttachmentItem item && CanInteract(item) && item.CanOpen && !item.IsUploading);
    }

    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string? Description { get => GetValue(DescriptionProperty); set => SetValue(DescriptionProperty, value); }
    public string UploadButtonText { get => GetValue(UploadButtonTextProperty); set => SetValue(UploadButtonTextProperty, value); }
    public string EmptyText { get => GetValue(EmptyTextProperty); set => SetValue(EmptyTextProperty, value); }
    public string? HintText { get => GetValue(HintTextProperty); set => SetValue(HintTextProperty, value); }
    public bool IsUploadVisible { get => GetValue(IsUploadVisibleProperty); set => SetValue(IsUploadVisibleProperty, value); }
    public bool IsRemoveVisible { get => GetValue(IsRemoveVisibleProperty); set => SetValue(IsRemoveVisibleProperty, value); }
    public bool IsBusy { get => GetValue(IsBusyProperty); set => SetValue(IsBusyProperty, value); }
    public IEnumerable<IAttachmentItem>? Files { get => GetValue(FilesProperty); set => SetValue(FilesProperty, value); }
    /// <summary>Receives one IReadOnlyList&lt;UploadFile&gt; batch. Accepted files must be disposed by the command owner.</summary>
    public ICommand? UploadCommand { get => GetValue(UploadCommandProperty); set => SetValue(UploadCommandProperty, value); }
    /// <summary>Receives the original item. The control never mutates Files or deletes a server file.</summary>
    public ICommand? RemoveCommand { get => GetValue(RemoveCommandProperty); set => SetValue(RemoveCommandProperty, value); }
    public IAttachmentFilePicker? FilePicker { get => GetValue(FilePickerProperty); set => SetValue(FilePickerProperty, value); }
    public bool HasFiles => _hasFiles;
    public bool IsSelecting => _selecting;
    public string? ErrorMessage => _error;
    public ICommand SelectCommand { get; }
    public ICommand RequestRemoveCommand { get; }
    public ICommand RequestPreviewCommand { get; }
    public event EventHandler<AttachmentPreviewRequestedEventArgs>? PreviewRequested
    {
        add => AddHandler(PreviewRequestedEvent, value);
        remove => RemoveHandler(PreviewRequestedEvent, value);
    }

    private bool CanSelect() => _attached && IsEffectivelyEnabled && IsUploadVisible && !IsBusy && !IsSelecting && UploadCommand != null;
    private bool CanInteract(IAttachmentItem item) => _attached && IsEffectivelyEnabled && !IsBusy && Files?.Any(candidate => ReferenceEquals(candidate, item)) == true;
    private bool CanRemoveItem(IAttachmentItem item) => CanInteract(item) && IsRemoveVisible && item.CanRemove && !item.IsUploading && RemoveCommand?.CanExecute(item) == true;

    /// <summary>Selects a true multi-file batch. Cancellation preserves Files. Only accepted commands receive ownership.</summary>
    public async Task SelectFilesAsync(CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        if (!CanSelect()) return;
        if (TopLevel.GetTopLevel(this) is not { } owner) return;
        CancelSelection();
        var version = _selectionVersion;
        var command = UploadCommand!;
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _selectionCancellation = cancellation;
        var token = cancellation.Token;
        SetSelecting(true);
        SetAndRaise(ErrorMessageProperty, ref _error, null);
        IReadOnlyList<UploadFile> files = [];
        var accepted = false;
        try
        {
            files = FilePicker is { } picker ? await picker.PickAsync(owner, token) : await PickNativeAsync(owner, Title, token);
            token.ThrowIfCancellationRequested();
            if (version != _selectionVersion || !_attached || !IsEffectivelyEnabled || IsBusy || !IsUploadVisible ||
                !ReferenceEquals(command, UploadCommand) || files.Count == 0) return;
            // Snapshot the batch before invoking business code; the picker no longer owns these selections.
            files = files.ToArray();
            if (!command.CanExecute(files)) return;
            command.Execute(files);
            accepted = true;
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception error)
        {
            if (version == _selectionVersion && _attached) SetAndRaise(ErrorMessageProperty, ref _error, "选择文件失败：" + error.Message);
        }
        finally
        {
            if (!accepted) foreach (var file in files) file?.Dispose();
            if (ReferenceEquals(_selectionCancellation, cancellation))
            {
                _selectionCancellation = null;
                SetSelecting(false);
            }
        }
    }

    private static async Task<IReadOnlyList<UploadFile>> PickNativeAsync(TopLevel owner, string title, CancellationToken token)
    {
        if (!owner.StorageProvider.CanOpen) throw new NotSupportedException("当前平台不支持选择文件。");
        var sources = await owner.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = true, Title = title });
        var files = new List<UploadFile>(sources.Count);
        var next = 0;
        try
        {
            token.ThrowIfCancellationRequested();
            while (next < sources.Count)
            {
                var source = sources[next++];
                files.Add(await UploadFile.FromStorageFileAsync(source, token, ownsStorageFile: true));
            }
            return files;
        }
        catch
        {
            foreach (var file in files) file.Dispose();
            throw;
        }
        finally
        {
            while (next < sources.Count) sources[next++].Dispose();
        }
    }

    private void CancelSelection()
    {
        ++_selectionVersion;
        var cancellation = _selectionCancellation;
        _selectionCancellation = null;
        cancellation?.Cancel();
        SetSelecting(false);
    }

    private void SetSelecting(bool value)
    {
        SetAndRaise(IsSelectingProperty, ref _selecting, value);
        RefreshCommands();
    }

    private void RefreshCommands()
    {
        ((AttachmentAction)SelectCommand).Notify();
        ((AttachmentAction)RequestRemoveCommand).Notify();
        ((AttachmentAction)RequestPreviewCommand).Notify();
    }

    private void ObserveFiles()
    {
        var collection = _attached ? Files as INotifyCollectionChanged : null;
        if (!ReferenceEquals(collection, _observedFiles))
        {
            if (_observedFiles != null) _observedFiles.CollectionChanged -= FilesChanged;
            _observedFiles = collection;
            if (_observedFiles != null) _observedFiles.CollectionChanged += FilesChanged;
        }
        foreach (var item in _observedItems) item.PropertyChanged -= ItemChanged;
        _observedItems.Clear();
        if (_attached && Files is { } files)
            foreach (var item in files)
                if (_observedItems.Add(item)) item.PropertyChanged += ItemChanged;
        SetAndRaise(HasFilesProperty, ref _hasFiles, Files?.Any() == true);
        RefreshCommands();
    }

    private void FilesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CancelSelection();
        ObserveFiles();
    }
    private void ItemChanged(object? sender, PropertyChangedEventArgs e) => CommandChanged(sender, EventArgs.Empty);
    private void CommandChanged(object? sender, EventArgs e)
    {
        if (Dispatcher.UIThread.CheckAccess()) RefreshCommands();
        else Dispatcher.UIThread.Post(RefreshCommands);
    }

    private void ObserveCommands()
    {
        if (_observedUpload != null) _observedUpload.CanExecuteChanged -= CommandChanged;
        if (_observedRemove != null) _observedRemove.CanExecuteChanged -= CommandChanged;
        _observedUpload = _attached ? UploadCommand : null;
        _observedRemove = _attached ? RemoveCommand : null;
        if (_observedUpload != null) _observedUpload.CanExecuteChanged += CommandChanged;
        if (_observedRemove != null) _observedRemove.CanExecuteChanged += CommandChanged;
        RefreshCommands();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _attached = true;
        ObserveFiles();
        ObserveCommands();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _attached = false;
        CancelSelection();
        ObserveFiles();
        ObserveCommands();
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FilesProperty)
        {
            CancelSelection();
            ObserveFiles();
        }
        if (change.Property == UploadCommandProperty || change.Property == RemoveCommandProperty)
        {
            CancelSelection();
            ObserveCommands();
        }
        if (change.Property == FilePickerProperty) CancelSelection();
        if (change.Property == DataContextProperty) CancelSelection();
        if (change.Property == IsEffectivelyEnabledProperty || change.Property == IsBusyProperty || change.Property == IsUploadVisibleProperty)
        {
            if (!IsEffectivelyEnabled || IsBusy || !IsUploadVisible) CancelSelection();
            RefreshCommands();
        }
        if (change.Property == IsRemoveVisibleProperty) RefreshCommands();
    }

    private sealed class AttachmentAction(Action<object?> execute, Func<object?, bool> canExecute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => canExecute(parameter);
        public void Execute(object? parameter) { if (CanExecute(parameter)) execute(parameter); }
        public void Notify() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>Provides a multi-file selection. Ownership of returned files transfers to AttachmentList.</summary>
public interface IAttachmentFilePicker
{
    Task<IReadOnlyList<UploadFile>> PickAsync(TopLevel owner, CancellationToken cancellationToken);
}

/// <summary>Forwards the original attachment for host-controlled preview or download.</summary>
public sealed class AttachmentPreviewRequestedEventArgs(IAttachmentItem item) : RoutedEventArgs(AttachmentList.PreviewRequestedEvent)
{
    public IAttachmentItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
}

internal sealed class AttachmentStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        AttachmentStatus.Uploading => TagStatus.Information,
        AttachmentStatus.Success => TagStatus.Success,
        AttachmentStatus.Error => TagStatus.Error,
        _ => TagStatus.Default
    };
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}

internal sealed class AttachmentUnknownProgressConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not double progress || progress <= 0;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
