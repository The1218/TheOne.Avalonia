using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace TheOne.DesignPreview;

public sealed class TabControlPageViewModel : BasicControlViewModel
{
    private int _nextId = 1001;
    public ObservableCollection<string> Tabs { get; } = new(Enumerable.Range(1, 1000).Select(index => $"工作区 {index:0000}"));
    public ICommand AddTabCommand { get; }
    public ICommand LastTabCommand { get; }

    public TabControlPageViewModel()
    {
        AddTabCommand = ReactiveCommand.Create<object?>(_ =>
        {
            Tabs.Add($"工作区 {_nextId++:0000}");
            SelectedIndex = Tabs.Count - 1;
            RequestScroll(SelectedIndex);
        });
        LastTabCommand = ReactiveCommand.Create<object?>(_ =>
        {
            SelectedIndex = Tabs.Count - 1;
            RequestScroll(SelectedIndex);
        });
    }
}
