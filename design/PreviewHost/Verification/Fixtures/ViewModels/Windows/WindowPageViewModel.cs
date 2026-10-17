using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class WindowPageViewModel : ReactiveObject
{
    public WindowPageViewModel()
    {
        OpenCommand = ReactiveCommand.Create<object?>(_ => OpenRequested?.Invoke(this, EventArgs.Empty));
    }

    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "工作区窗口";

    public bool ShowTitle
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowTheme
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public ICommand OpenCommand { get; }

    public event EventHandler? OpenRequested;
}