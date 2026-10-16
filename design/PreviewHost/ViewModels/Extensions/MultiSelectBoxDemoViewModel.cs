using System.Collections.ObjectModel;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class MultiSelectChoice : ReactiveObject
{
    public int Id { get; init; }
    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public bool Available { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
}

public sealed class MultiSelectBoxDemoViewModel : ReactiveObject
{
    public ObservableCollection<MultiSelectChoice> Choices { get; } = new(Enumerable.Range(0, 10000).Select(index =>
        new MultiSelectChoice { Id = index, Name = $"选项 {index:D5}", Available = index % 7 != 0 }));
    public ObservableCollection<MultiSelectChoice> Selected { get; set => this.RaiseAndSetIfChanged(ref field, value); } = [];
    public MultiSelectItemAdapter Adapter { get; } = MultiSelectItemAdapter.Create<MultiSelectChoice>(item => item.Name, item => item.Available);
    public MultiSelectBoxDemoViewModel()
    {
        Selected.Add(Choices[0]); Selected.Add(Choices[1]); Selected.Add(Choices[2]); Selected.Add(Choices[3]);
    }
}
