using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class DialogPage : UserControl
{
    public DialogPage()
    {
        DataContext = Model;
        InitializeComponent();
    }

    public MessagePageViewModel Model { get; } = new();
}