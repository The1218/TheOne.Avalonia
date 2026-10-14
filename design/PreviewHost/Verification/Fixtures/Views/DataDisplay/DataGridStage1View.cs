using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

/// <summary>Interactive stage-one experiment; the normal DataGrid stays unchanged.</summary>
public sealed class DataGridStage1View : UserControl
{
    internal DataGridStage1Fixture Fixture { get; } = new();
    private readonly TextBlock _metrics = new() { TextWrapping = TextWrapping.Wrap };
    public DataGridStage1View()
    {
        var panel = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto"), RowSpacing = 12, Margin = new Thickness(20) };
        var heading = new TextBlock { Text = "DataGrid 阶段 1 · 右侧冻结可行性", FontWeight = FontWeight.SemiBold, FontSize = 18 };
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
        var trial = new Button { Content = "尝试仅移动右列" };
        var reset = new Button { Content = "还原原生布局" };
        var last = new Button { Content = "第 1,000 行" };
        var right = new Button { Content = "原生定位右列" };
        trial.Click += (_, _) => { Fixture.ApplyPositionOnlyTrial(); RefreshMetrics(); };
        reset.Click += (_, _) => { Fixture.ClearTrial(); RefreshMetrics(); };
        last.Click += (_, _) => { Fixture.Grid.SelectedItem = Fixture.Records[^1]; Fixture.Grid.ScrollIntoView(Fixture.Records[^1], null!); };
        right.Click += (_, _) => Fixture.Grid.ScrollIntoView(Fixture.Grid.SelectedItem ?? Fixture.Records[0], Fixture.RightColumn);
        foreach (var button in new[] { trial, reset, last, right }) actions.Children.Add(button);
        Grid.SetRow(actions, 1); Grid.SetRow(Fixture.Grid, 2); Grid.SetRow(_metrics, 3);
        panel.Children.Add(heading); panel.Children.Add(actions); panel.Children.Add(Fixture.Grid); panel.Children.Add(_metrics);
        Content = panel;
        Fixture.Grid.LayoutUpdated += (_, _) => RefreshMetrics();
        AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(RefreshMetrics, DispatcherPriority.Loaded);
    }
    private void RefreshMetrics() => _metrics.Text = $"1,000 行 · 当前行容器 {Fixture.Rows.Length} · 累计创建 {Fixture.CreatedRows} · 右列可见单元格 {Fixture.VisibleRightCells}\n" +
        "实验按钮只移动表头和已存在单元格。原生可见性与键盘滚动仍受官方内部布局控制；此界面用于验证限制，不代表已实现右侧冻结。";
}
