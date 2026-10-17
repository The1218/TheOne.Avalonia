using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class UploadDemoView : UserControl
{
    public UploadDemoView()
    {
        AvaloniaXamlLoader.Load(this);
        DataContext = Model;
    }

    public UploadDemoViewModel Model { get; } = new();
}