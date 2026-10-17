using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class AvatarDemoView : UserControl
{
    public static readonly StyledProperty<object?> ImageSourceProperty =
        AvaloniaProperty.Register<AvatarDemoView, object?>(nameof(ImageSource));

    public AvatarDemoView()
    {
        DataContext = this;
        AvaloniaXamlLoader.Load(this);
        Sample.PropertyChanged += OnAvatarChanged;
    }

    public object? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public string ExampleImageSource =>
        $"avares://{typeof(AvatarDemoView).Assembly.GetName().Name}/Assets/preview-lake.png";

    public Avatar Sample => this.FindControl<Avatar>("AvatarSample")!;
    private TextBox SourceInput => this.FindControl<TextBox>("AvatarSourceInput")!;
    private TextBlock LoadStatus => this.FindControl<TextBlock>("AvatarLoadStatus")!;

    private void OnLoad(object? sender, RoutedEventArgs args)
    {
        ImageSource = SourceInput.Text;
    }

    private void OnReload(object? sender, RoutedEventArgs args)
    {
        Sample.Reload();
    }

    private void OnClear(object? sender, RoutedEventArgs args)
    {
        SourceInput.Text = "";
        ImageSource = null;
    }

    private void OnAvatarChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == Avatar.IsLoadingProperty || args.Property == Avatar.HasImageProperty ||
            args.Property == Avatar.ErrorMessageProperty)
            LoadStatus.Text = Sample.IsLoading ? "正在加载，保持占位显示…" :
                Sample.HasImage ? "图片已加载。" :
                Sample.ErrorMessage is { } error ? $"图片加载失败，已显示占位：{error}" : "未设置图片时显示文字占位。";
    }
}