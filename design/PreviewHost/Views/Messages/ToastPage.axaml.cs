using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ToastPage : UserControl
{
    private readonly MessageDemoConfiguration _configuration;

    public ToastPage()
    {
        DataContext = Model;
        InitializeComponent();
        _configuration = new MessageDemoConfiguration(this, Model, true);
        AttachedToVisualTree += (_, _) => _configuration.Attach();
        DetachedFromVisualTree += (_, _) => _configuration.Detach();
    }

    public MessagePageViewModel Model { get; } = new();
}