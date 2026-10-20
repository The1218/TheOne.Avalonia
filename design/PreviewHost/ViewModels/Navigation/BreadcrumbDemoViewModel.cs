using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class BreadcrumbDemoViewModel : ReactiveObject
{
    public ObservableCollection<BreadcrumbItemModel> Path { get; } = [];
    public ICommand NavigateCommand { get; }
    public string NavigationText { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = "点击祖先项返回；最后一项为当前页。";

    public BreadcrumbDemoViewModel()
    {
        NavigateCommand = ReactiveCommand.Create<object?>(item =>
        {
            if (item is not BreadcrumbItemModel node) return;
            var index = Path.IndexOf(node);
            if (index < 0) return;
            while (Path.Count > index + 1) Path.RemoveAt(Path.Count - 1);
            NavigationText = $"导航到 {node.Header} · Key = {node.Key}";
        });
        Reset();
    }

    public void Reset()
    {
        Path.Clear();
        Path.Add(new() { Header = "首页", Key = "home", Icon = OneIconKind.Home });
        Path.Add(new() { Header = "项目管理", Key = "projects", Icon = OneIconKind.Folder });
        Path.Add(new() { Header = "控件库", Key = "theone" });
        Path.Add(new() { Header = "组件详情", Key = "details" });
        NavigationText = "点击祖先项返回；最后一项为当前页。";
    }
}
