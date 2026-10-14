using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal sealed record ButtonComparisonSample(string Theme, ControlSize Size, SelectionVisualState State, bool Selected, bool Primary, Button Control);

public partial class ButtonControlsDemoView : UserControl
{
    private readonly List<ButtonComparisonSample> _samples = [];
    internal IReadOnlyList<ButtonComparisonSample> Samples => _samples;
    public int ActionCount { get; private set; }
    public int RepeatCount { get; private set; }
    public ButtonControlsDemoView()
    {
        InitializeComponent(); AddTheme(LightButtonMatrix, "Light"); AddTheme(DarkButtonMatrix, "Dark"); UpdateStatus();
    }
    private void AddTheme(StackPanel parent, string theme)
    {
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var name = size switch { ControlSize.Small => "小尺寸 / small", ControlSize.Large => "大尺寸 / large", _ => "正常 / normal" };
            var section = new StackPanel { Spacing = 10 };
            section.Children.Add(new TextBlock { Text = $"{name} · {height} px", Classes = { "matrix-text" }, FontWeight = global::Avalonia.Media.FontWeight.SemiBold });
            var matrix = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("48,*,*,*,*"), RowSpacing = 10, ColumnSpacing = 8, IsHitTestVisible = false };
            matrix.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            foreach (var (title, column) in new[] { ("状态", 0), ("Button", 1), ("Toggle 未选", 2), ("Toggle 已选", 3), ("RepeatButton", 4) })
                AddCell(matrix, new TextBlock { Text = title, Classes = { "note" } }, 0, column);
            var row = 0;
            foreach (var state in Enum.GetValues<SelectionVisualState>())
            {
                row++; matrix.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                AddCell(matrix, new TextBlock { Text = StateText(state), Classes = { "note" }, VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center }, row, 0);
                Button[] controls =
                [
                    new StateButton(state) { Content = "操作" },
                    new StateToggleButton(state) { Content = "未选", IsChecked = false },
                    new StateToggleButton(state) { Content = "已选", IsChecked = true },
                    new StateRepeatButton(state) { Content = "重复" }
                ];
                for (var column = 0; column < controls.Length; column++)
                {
                    var control = controls[column]; Prepare(control, state); AddCell(matrix, control, row, column + 1);
                    _samples.Add(new(theme, size, state, column == 2, false, control));
                }
            }
            section.Children.Add(matrix);
            section.Children.Add(new TextBlock { Text = "主按钮 / primary", Classes = { "note" } });
            var primary = new Grid { ColumnDefinitions = ColumnDefinitions.Parse("*,*,*,*"), ColumnSpacing = 8, IsHitTestVisible = false };
            var index = 0;
            foreach (var state in Enum.GetValues<SelectionVisualState>())
            {
                var control = new StateButton(state) { Content = StateText(state), Classes = { "primary" } };
                Prepare(control, state); AddCell(primary, control, 0, index++); _samples.Add(new(theme, size, state, false, true, control));
            }
            section.Children.Add(primary);
            var frame = new Border { Classes = { "size-region" }, Child = section };
            ControlSizing.SetSize(frame, size); Motion.SetEnabled(frame, false); parent.Children.Add(frame);
        }
    }
    private static string StateText(SelectionVisualState state) => state switch { SelectionVisualState.Hover => "悬停", SelectionVisualState.Pressed => "按下", SelectionVisualState.Disabled => "禁用", _ => "正常" };
    private static void Prepare(Button control, SelectionVisualState state)
    {
        control.IsEnabled = state != SelectionVisualState.Disabled; control.Focusable = false; KeyboardNavigation.SetIsTabStop(control, false);
    }
    private static void AddCell(Grid grid, Control control, int row, int column) { Grid.SetRow(control, row); Grid.SetColumn(control, column); grid.Children.Add(control); }
    private void OnMatrixSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        var stacked = args.NewSize.Width < 900;
        if ((Grid.GetRow(DarkButtonScope) == 1) == stacked) return;
        ButtonThemeColumns.ColumnDefinitions = ColumnDefinitions.Parse(stacked ? "*" : "*,*");
        Grid.SetColumn(DarkButtonScope, stacked ? 0 : 1); Grid.SetRow(DarkButtonScope, stacked ? 1 : 0);
    }
    private void OnEnabledChanged(object? sender, RoutedEventArgs args)
    {
        if (LiveButtonControls is null) return;
        LiveButtonControls.IsEnabled = ButtonInteractionEnabled.IsChecked == true; UpdateStatus();
    }
    private void OnAction(object? sender, RoutedEventArgs args) { ActionCount++; UpdateStatus(); }
    private void OnRepeat(object? sender, RoutedEventArgs args) { RepeatCount++; UpdateStatus(); }
    private void OnToggleChanged(object? sender, RoutedEventArgs args) => UpdateStatus();
    private void UpdateStatus()
    {
        if (ButtonStatus is null || LiveToggle is null) return;
        ButtonStatus.Text = $"Button：{ActionCount} 次 · ToggleButton：{(LiveToggle.IsChecked == true ? "已选中" : "未选中")} · RepeatButton：{RepeatCount} 次 · {(ButtonInteractionEnabled.IsChecked == true ? "交互已启用" : "交互已禁用")}";
    }
    private void OnReset(object? sender, RoutedEventArgs args)
    {
        ButtonInteractionEnabled.IsChecked = true; ActionCount = RepeatCount = 0; LiveToggle.IsChecked = false; UpdateStatus();
    }
}

internal sealed class StateButton : Button
{
    protected override Type StyleKeyOverride => typeof(Button);
    public StateButton(SelectionVisualState state) => Loaded += (_, _) => { PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed); PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed); };
}
internal sealed class StateToggleButton : ToggleButton
{
    protected override Type StyleKeyOverride => typeof(ToggleButton);
    public StateToggleButton(SelectionVisualState state) => Loaded += (_, _) => { PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed); PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed); };
}
internal sealed class StateRepeatButton : RepeatButton
{
    protected override Type StyleKeyOverride => typeof(RepeatButton);
    public StateRepeatButton(SelectionVisualState state) => Loaded += (_, _) => { PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed); PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed); };
}
