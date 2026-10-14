using System.Collections.ObjectModel;
using System.Windows.Input;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class TabControlPageViewModel : BasicControlViewModel
{
    private int _nextId = 1001;

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

    public ObservableCollection<string> Tabs { get; } =
        new(Enumerable.Range(1, 1000).Select(index => $"工作区 {index:0000}"));

    public ICommand AddTabCommand { get; }
    public ICommand LastTabCommand { get; }
}