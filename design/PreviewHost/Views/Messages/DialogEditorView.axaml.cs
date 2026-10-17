using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class DialogEditorView : UserControl
{
    public DialogEditorView()
    {
        AvaloniaXamlLoader.Load(this);
    }
}