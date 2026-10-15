using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class UploadDemoView : UserControl
{
    public UploadDemoViewModel Model { get; } = new();
    public UploadDemoView() { AvaloniaXamlLoader.Load(this); DataContext = Model; }
}
