using Avalonia.Controls;
namespace TheOne.DesignPreview;
public partial class LoadingContainerPage : UserControl
{
    public LoadingContainerPageViewModel Model { get; } = new();
    public LoadingContainerPage() { DataContext = Model; InitializeComponent(); }
}
