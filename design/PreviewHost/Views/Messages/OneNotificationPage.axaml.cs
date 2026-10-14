using Avalonia.Controls;
namespace TheOne.DesignPreview;
public partial class OneNotificationPage : UserControl
{
    public MessagePageViewModel Model { get; } = new();
    private readonly MessageDemoConfiguration _configuration;
    public OneNotificationPage()
    {
        Model.Position = global::Avalonia.Controls.Notifications.NotificationPosition.TopRight; DataContext = Model; InitializeComponent(); _configuration = new(this, Model, false);
        AttachedToVisualTree += (_, _) => _configuration.Attach();
        DetachedFromVisualTree += (_, _) => _configuration.Detach();
    }
}
