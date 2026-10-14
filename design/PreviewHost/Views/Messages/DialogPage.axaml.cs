using Avalonia.Controls;
namespace TheOne.DesignPreview;
public partial class DialogPage : UserControl { public MessagePageViewModel Model { get; } = new(); public DialogPage() { DataContext = Model; InitializeComponent(); } }
