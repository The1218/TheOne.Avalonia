using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal sealed class MessageDemoConfiguration(UserControl view, MessagePageViewModel model, bool toast)
{
    public void Attach()
    {
        model.ConfigurationRequested += Requested;
        model.PropertyChanged += Changed;
    }

    public void Detach()
    {
        model.ConfigurationRequested -= Requested;
        model.PropertyChanged -= Changed;
    }

    private void Changed(object? sender, PropertyChangedEventArgs args)
    {
        Apply();
    }

    private void Requested(object? sender, EventArgs args)
    {
        Apply();
    }

    private void Apply()
    {
        if (TopLevel.GetTopLevel(view) is not OneWindow window) return;
        var host = window.GetVisualDescendants().OfType<WindowNotificationManager>()
            .FirstOrDefault(control => control.Name == (toast ? "PART_Toasts" : "PART_Notifications"));
        if (host is null) return;
        host.Position = model.Position;
        host.MaxItems = (int)Math.Clamp(model.Maximum ?? 4, 1, 10);
        ControlSizing.SetSize(host, model.Size);
        Motion.SetEnabled(host, model.MotionEnabled);
    }
}