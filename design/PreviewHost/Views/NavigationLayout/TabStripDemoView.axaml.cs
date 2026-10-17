using Avalonia.Controls;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public partial class TabStripDemoView : UserControl
{
    public TabStripDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        LayoutUpdated += (_, _) => UpdateMetrics();
        Model.ScrollRequested += (_, index) =>
        {
            HugeStrip.ScrollIntoView(index);
            Dispatcher.UIThread.Post(() =>
            {
                HugeStrip.ContainerFromIndex(index)?.Focus();
                UpdateMetrics();
            }, DispatcherPriority.Loaded);
        };
    }

    public TabStripDemoViewModel Model { get; } = new();
    public IReadOnlyList<string> Rows => Model.Rows;

    public void UpdateMetrics()
    {
        Model.Metrics =
            $"已创建 {HugeStrip.GetRealizedContainers().Count()} / {Rows.Count:N0} · 当前第 {HugeStrip.SelectedIndex + 1:N0} 项";
    }
}