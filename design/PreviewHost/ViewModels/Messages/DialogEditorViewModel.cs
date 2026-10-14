using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class DialogEditorViewModel : DialogContext
{
    public DialogEditorViewModel()
    {
        SaveCommand = ReactiveCommand.Create<object?>(_ => RequestClose());
        CancelCommand = ReactiveCommand.Create<object?>(_ => RequestClose(OneDialogResult.Cancel));
    }

    public string Name
    {
        get;
        set => SetProperty(ref field, value);
    } = "我的工作区";

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
}