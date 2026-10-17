using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class LoadingContainerPage : UserControl
{
    public LoadingContainerPage()
    {
        DataContext = Model;
        InitializeComponent();
    }

    public LoadingContainerPageViewModel Model { get; } = new();
}