using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class ImageUploadDemoView : UserControl
{
    public ImageUploadDemoView()
    {
        AvaloniaXamlLoader.Load(this);
        Model.File = Model.ExampleFile;
        DataContext = Model;
    }

    public UploadDemoViewModel Model { get; } = new();
}
