using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public class LoadingDemoViewModel : BasicControlViewModel
{
    public LoadingDemoViewModel()
    {
        LoadCommand = ReactiveCommand.CreateFromTask<object?>(async _ =>
        {
            if (IsLoading) return;
            IsLoading = true;
            OperationCount++;
            try
            {
                await Task.Delay(3000);
            }
            finally
            {
                IsLoading = false;
            }
        });
    }

    public bool IsLoading
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool MotionEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public string LoadingText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "正在加载内容…";

    public int OperationCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ICommand LoadCommand { get; }
}