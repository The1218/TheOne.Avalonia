using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class PaginationDemoViewModel : ReactiveObject
{
    public long TotalCount { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 1280;
    public long CurrentPage { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 8;
    public int PageSize { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 20;
    public ObservableCollection<int> PageSizeOptions { get; } = [10, 20, 50, 100];
    public string Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "两个布局绑定同一组分页数据；页码与每页条数支持双向绑定。";
    public ICommand PageChangedCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand EmptyCommand { get; }
    public ICommand ShrinkCommand { get; }
    public int NotificationCount { get; private set; }
    public PaginationDemoViewModel()
    {
        PageChangedCommand = ReactiveCommand.Create<object?>(value =>
        {
            if (value is not PaginationPageChangedEventArgs args) return;
            NotificationCount++;
            Status = $"分页请求 · 第 {args.CurrentPage} 页 · 每页 {args.PageSize} 条 · 通知 {NotificationCount} 次";
        });
        ResetCommand = ReactiveCommand.Create<object?>(_ => { TotalCount = 1280; PageSize = 20; CurrentPage = 8; });
        EmptyCommand = ReactiveCommand.Create<object?>(_ => TotalCount = 0);
        ShrinkCommand = ReactiveCommand.Create<object?>(_ => TotalCount = 45);
    }
}
