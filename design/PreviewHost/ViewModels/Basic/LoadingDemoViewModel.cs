using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public class LoadingDemoViewModel : BasicControlViewModel
{
    public bool IsLoading { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool MotionEnabled { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public string LoadingText { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "正在加载内容…";
    public int OperationCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public ICommand LoadCommand { get; }
    public LoadingDemoViewModel()
    {
        LoadCommand = ReactiveCommand.CreateFromTask<object?>(async _ =>
        {
            if (IsLoading) return;
            IsLoading = true; OperationCount++; Status = $"第 {OperationCount} 次加载：可在加载过程中切换动效。";
            try { await Task.Delay(3000); }
            finally { IsLoading = false; Status = $"第 {OperationCount} 次加载完成。"; }
        });
    }
}
