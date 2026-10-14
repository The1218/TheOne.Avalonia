using Avalonia.Controls;
namespace TheOne.DesignPreview;
public partial class ToastPage : UserControl
{
    public MessagePageViewModel Model { get; } = new();
    private readonly MessageDemoConfiguration _configuration;
    public ToastPage()
    {
        DataContext = Model; InitializeComponent(); _configuration = new(this, Model, true);
        AttachedToVisualTree += (_, _) => _configuration.Attach();
        DetachedFromVisualTree += (_, _) => _configuration.Detach();
    }
}
