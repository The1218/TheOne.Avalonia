using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace TheOne.DesignPreview;

public partial class ImagePreviewDemoView : UserControl
{
    public ImagePreviewDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public ImagePreviewDemoViewModel Model { get; } = new();

    private void OnBitmap(object? sender, RoutedEventArgs e)
    {
        Model.UseBitmap();
    }

    private void OnNetwork(object? sender, RoutedEventArgs e)
    {
        Model.UseNetwork();
    }

    private void OnLocal(object? sender, RoutedEventArgs e)
    {
        Model.UseLocal();
    }

    private void OnAddCurrent(object? sender, RoutedEventArgs e)
    {
        Model.AddCurrent();
    }

    private async void OnPickFile(object? sender, RoutedEventArgs e)
    {
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { CanOpen: true } storage) return;
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择预览图片", AllowMultiple = false, FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });
        if (files.Count > 0 && files[0].TryGetLocalPath() is { } path)
        {
            Model.LocalPath = path;
            Model.UseLocal();
        }
    }
}