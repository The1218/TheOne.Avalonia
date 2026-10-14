using ReactiveUI;
using System.Windows.Input;
using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class DialogEditorViewModel : DialogContext
{
    public string Name { get; set => SetProperty(ref field, value); } = "我的工作区";
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public DialogEditorViewModel()
    {
        SaveCommand = ReactiveCommand.Create<object?>(_ => RequestClose(OneDialogResult.Ok));
        CancelCommand = ReactiveCommand.Create<object?>(_ => RequestClose(OneDialogResult.Cancel));
    }
}
