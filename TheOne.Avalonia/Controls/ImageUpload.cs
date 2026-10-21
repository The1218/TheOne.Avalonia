using System.Globalization;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>Determines whether an image can be selected, replaced or removed.</summary>
public enum ImageUploadMode
{
    Upload,
    Preview
}

/// <summary>A single-image card with an accessible hover toolbar and a read-only preview mode.</summary>
[PseudoClasses(":preview")]
public class ImageUpload : Upload
{
    public static readonly StyledProperty<ImageUploadMode> ModeProperty =
        AvaloniaProperty.Register<ImageUpload, ImageUploadMode>(nameof(Mode),
            validate: value => value is ImageUploadMode.Upload or ImageUploadMode.Preview);

    public static readonly StyledProperty<string> ViewTextProperty =
        AvaloniaProperty.Register<ImageUpload, string>(nameof(ViewText), "查看");

    public static readonly StyledProperty<string?> HintTextProperty =
        AvaloniaProperty.Register<ImageUpload, string?>(nameof(HintText));

    public static readonly StyledProperty<double> ImageHeightProperty =
        AvaloniaProperty.Register<ImageUpload, double>(nameof(ImageHeight), 240,
            validate: value => double.IsFinite(value) && value > 0);

    public static readonly StyledProperty<Stretch> ImageStretchProperty =
        AvaloniaProperty.Register<ImageUpload, Stretch>(nameof(ImageStretch), Stretch.Uniform);

    public static readonly DirectProperty<ImageUpload, string> DisplayHintProperty =
        AvaloniaProperty.RegisterDirect<ImageUpload, string>(nameof(DisplayHint), owner => owner.DisplayHint);

    private string _displayHint = "上传图片最大为 3 MB";
    private readonly ViewAction _view;
    private Flyout? _previewFlyout;

    static ImageUpload()
    {
        MaxFileSizeProperty.OverrideDefaultValue<ImageUpload>(3 * 1024 * 1024);
        ReplaceTextProperty.OverrideDefaultValue<ImageUpload>("编辑");
        RemoveTextProperty.OverrideDefaultValue<ImageUpload>("删除");
    }

    public ImageUpload()
    {
        _view = new ViewAction(this);
    }

    public ImageUploadMode Mode { get => GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public string ViewText { get => GetValue(ViewTextProperty); set => SetValue(ViewTextProperty, value); }
    public string? HintText { get => GetValue(HintTextProperty); set => SetValue(HintTextProperty, value); }
    public double ImageHeight { get => GetValue(ImageHeightProperty); set => SetValue(ImageHeightProperty, value); }
    public Stretch ImageStretch { get => GetValue(ImageStretchProperty); set => SetValue(ImageStretchProperty, value); }
    public string DisplayHint => _displayHint;
    public ICommand ViewCommand => _view;
    protected override Type StyleKeyOverride => typeof(ImageUpload);
    protected override bool CanEditFile => Mode == ImageUploadMode.Upload;

    protected override string? ValidateFile(UploadFile file) =>
        IsImageFile(file) ? base.ValidateFile(file) : "请选择图片文件。";

    // Keep the original resolution for ImagePreview's zoom and actual-size actions.
    protected override Bitmap DecodePreview(Stream stream) => new(stream);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == SelectedFileProperty || change.Property == PreviewImageProperty ||
            (change.Property == IsEffectivelyEnabledProperty && !IsEffectivelyEnabled)) ClosePreview();
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty)
        {
            PseudoClasses.Set(":preview", Mode == ImageUploadMode.Preview);
            RefreshEditingPolicy();
        }

        if (change.Property == ModeProperty || change.Property == HintTextProperty || change.Property == MaxFileSizeProperty)
        {
            var hint = HintText ?? (Mode == ImageUploadMode.Preview ? "图片预览" : MaxFileSize == 0
                ? "上传图片" : $"上传图片最大为 {(MaxFileSize / 1024d / 1024).ToString("0.##", CultureInfo.CurrentCulture)} MB");
            SetAndRaise(DisplayHintProperty, ref _displayHint, hint);
        }

        if (change.Property == HasPreviewProperty || change.Property == IsEffectivelyEnabledProperty)
            _view?.Notify();
    }

    /// <summary>Opens the image viewer; Escape or the close button dismisses it.</summary>
    public void View()
    {
        VerifyAccess();
        if (!_view.CanExecute(null) || TopLevel.GetTopLevel(this) is not { } topLevel) return;
        ClosePreview();
        var viewer = new ImagePreview
        {
            Source = PreviewImage, Title = SelectedFile?.Name, ShowCloseButton = true,
            Width = Math.Max(120, Math.Min(800, topLevel.Bounds.Width - 64)),
            Height = Math.Max(160, Math.Min(560, topLevel.Bounds.Height - 80))
        };
        // A popup has no desktop window owner for ImagePreview's full-screen action.
        viewer.TemplateApplied += (_, e) =>
        {
            if (e.NameScope.Find<Button>("PART_FullScreen") is { } fullScreen) fullScreen.IsVisible = false;
        };
        ControlSizing.SetSize(viewer, ControlSizing.GetSize(this));
        _previewFlyout = new Flyout
        {
            Content = viewer, Placement = PlacementMode.Center,
            FlyoutPresenterClasses = { "one-image-upload-preview" }
        };
        viewer.CloseRequested += (_, _) => ClosePreview();
        _previewFlyout.ShowAt(this);
    }

    private void ClosePreview()
    {
        var flyout = _previewFlyout;
        _previewFlyout = null;
        flyout?.Hide();
        if (flyout?.Content is ImagePreview viewer) viewer.Source = null;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        ClosePreview();
        base.OnDetachedFromVisualTree(e);
    }

    private sealed class ViewAction(ImageUpload owner) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => owner.IsEffectivelyEnabled && owner.HasPreview;
        public void Execute(object? parameter) { if (CanExecute(parameter)) owner.View(); }
        public void Notify() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
