using Avalonia.Controls;
using Avalonia.Controls.Notifications;

namespace TheOne.DesignPreview;

public partial class OneNotificationPage : UserControl
{
    private readonly MessageDemoConfiguration _configuration;

    public OneNotificationPage()
    {
        Model.Position = NotificationPosition.TopRight;
        DataContext = Model;
        InitializeComponent();
        _configuration = new MessageDemoConfiguration(this, Model, false);
        AttachedToVisualTree += (_, _) => _configuration.Attach();
        DetachedFromVisualTree += (_, _) => _configuration.Detach();
    }

    public MessagePageViewModel Model { get; } = new();
}