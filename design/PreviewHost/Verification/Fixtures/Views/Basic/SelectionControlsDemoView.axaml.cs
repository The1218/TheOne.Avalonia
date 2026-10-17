using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal enum SelectionVisualState
{
    Normal,
    Hover,
    Pressed,
    Disabled
}

internal sealed record SelectionComparisonSample(
    string Theme,
    ControlSize Size,
    SelectionVisualState State,
    bool? Selected,
    Control Control);

public partial class SelectionControlsDemoView : UserControl
{
    private readonly string _groupPrefix = Guid.NewGuid().ToString("N");
    private readonly List<SelectionComparisonSample> _samples = [];

    public SelectionControlsDemoView()
    {
        InitializeComponent();
        LiveRadioFirst.GroupName = LiveRadioSecond.GroupName = $"{_groupPrefix}-live";
        AddTheme(LightMatrix, "Light");
        AddTheme(DarkMatrix, "Dark");
        UpdateStatus();
    }

    internal IReadOnlyList<SelectionComparisonSample> Samples => _samples;

    private void AddTheme(StackPanel parent, string theme)
    {
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var name = size switch
            {
                ControlSize.Small => "小尺寸 / small", ControlSize.Large => "大尺寸 / large", _ => "正常 / normal"
            };
            var section = new StackPanel { Spacing = 10 };
            section.Children.Add(new TextBlock
                { Text = $"{name} · {height} px", Classes = { "matrix-text" }, FontWeight = FontWeight.SemiBold });
            var matrix = new Grid
            {
                ColumnDefinitions = ColumnDefinitions.Parse("88,*,*,*"), RowSpacing = 8, ColumnSpacing = 8,
                IsHitTestVisible = false
            };
            matrix.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            foreach (var (title, column) in new[]
                         { ("状态", 0), ("CheckBox", 1), ("RadioButton", 2), ("ToggleSwitch", 3) })
                AddCell(matrix, new TextBlock { Text = title, Classes = { "note" } }, 0, column);
            var row = 0;
            foreach (var state in Enum.GetValues<SelectionVisualState>())
            foreach (var selected in new[] { true, false })
            {
                row++;
                matrix.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                var title = state switch
                {
                    SelectionVisualState.Hover => "悬停", SelectionVisualState.Pressed => "按下",
                    SelectionVisualState.Disabled => "禁用", _ => "默认"
                };
                AddCell(matrix,
                    new TextBlock
                    {
                        Text = $"{title} · {(selected ? "选中" : "未选")}", Classes = { "note" },
                        VerticalAlignment = VerticalAlignment.Center
                    }, row, 0);
                var text = selected ? "选中" : "未选";
                Control[] controls =
                [
                    new StateCheckBox(state) { Content = text, IsChecked = selected },
                    new StateRadioButton(state)
                        { Content = text, IsChecked = selected, GroupName = $"{_groupPrefix}-{theme}-{size}-{row}" },
                    new StateToggleSwitch(state) { OnContent = "开启", OffContent = "关闭", IsChecked = selected }
                ];
                for (var index = 0; index < controls.Length; index++)
                {
                    var control = controls[index];
                    control.IsEnabled = state != SelectionVisualState.Disabled;
                    control.Focusable = false;
                    KeyboardNavigation.SetIsTabStop(control, false);
                    AddCell(matrix, control, row, index + 1);
                    _samples.Add(new SelectionComparisonSample(theme, size, state, selected, control));
                }
            }

            row++;
            matrix.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            AddCell(matrix,
                new TextBlock { Text = "部分选择", Classes = { "note" }, VerticalAlignment = VerticalAlignment.Center },
                row, 0);
            var mixed = new StateCheckBox(SelectionVisualState.Normal)
                { Content = "部分", IsThreeState = true, IsChecked = null, Focusable = false };
            KeyboardNavigation.SetIsTabStop(mixed, false);
            AddCell(matrix, mixed, row, 1);
            _samples.Add(new SelectionComparisonSample(theme, size, SelectionVisualState.Normal, null, mixed));
            for (var column = 2; column <= 3; column++)
                AddCell(matrix,
                    new TextBlock { Text = "—", Classes = { "note" }, VerticalAlignment = VerticalAlignment.Center },
                    row, column);
            section.Children.Add(matrix);
            var frame = new Border { Classes = { "size-region" }, Child = section };
            ControlSizing.SetSize(frame, size);
            Motion.SetEnabled(frame, false);
            parent.Children.Add(frame);
        }
    }

    private static void AddCell(Grid grid, Control control, int row, int column)
    {
        Grid.SetRow(control, row);
        Grid.SetColumn(control, column);
        grid.Children.Add(control);
    }

    private void OnMatrixSizeChanged(object? sender, SizeChangedEventArgs args)
    {
        var stacked = args.NewSize.Width < 900;
        if (Grid.GetRow(DarkMatrixScope) == 1 == stacked) return;
        ThemeColumns.ColumnDefinitions = ColumnDefinitions.Parse(stacked ? "*" : "*,*");
        Grid.SetColumn(DarkMatrixScope, stacked ? 0 : 1);
        Grid.SetRow(DarkMatrixScope, stacked ? 1 : 0);
    }

    private void OnEnabledChanged(object? sender, RoutedEventArgs args)
    {
        if (LiveSelectionControls is null) return;
        LiveSelectionControls.IsEnabled = SelectionInteractionEnabled.IsChecked == true;
        UpdateStatus();
    }

    private void OnSelectionChanged(object? sender, RoutedEventArgs args)
    {
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (SelectionStatus is null || LiveCheck is null || LiveRadioFirst is null || LiveSwitch is null) return;
        var check = LiveCheck.IsChecked switch { true => "选中", false => "未选中", _ => "部分选择" };
        SelectionStatus.Text =
            $"CheckBox：{check} · RadioButton：{(LiveRadioFirst.IsChecked == true ? "标准" : "紧凑")} · ToggleSwitch：{(LiveSwitch.IsChecked == true ? "开启" : "关闭")} · {(SelectionInteractionEnabled.IsChecked == true ? "交互已启用" : "交互已禁用")}";
    }

    private void OnReset(object? sender, RoutedEventArgs args)
    {
        SelectionInteractionEnabled.IsChecked = true;
        LiveCheck.IsChecked = LiveRadioFirst.IsChecked = LiveSwitch.IsChecked = true;
        LiveRadioSecond.IsChecked = false;
        UpdateStatus();
    }
}

// Fixed state samples reuse the native StyleKey and semantic templates. These
// preview-only subclasses never enter the public control library or handle input.
internal sealed class StateCheckBox : CheckBox
{
    public StateCheckBox(SelectionVisualState state)
    {
        Loaded += (_, _) =>
        {
            PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed);
            PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed);
        };
    }

    protected override Type StyleKeyOverride => typeof(CheckBox);
}

internal sealed class StateRadioButton : RadioButton
{
    public StateRadioButton(SelectionVisualState state)
    {
        Loaded += (_, _) =>
        {
            PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed);
            PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed);
        };
    }

    protected override Type StyleKeyOverride => typeof(RadioButton);
}

internal sealed class StateToggleSwitch : ToggleSwitch
{
    public StateToggleSwitch(SelectionVisualState state)
    {
        Loaded += (_, _) =>
        {
            PseudoClasses.Set(":pointerover", state is SelectionVisualState.Hover or SelectionVisualState.Pressed);
            PseudoClasses.Set(":pressed", state == SelectionVisualState.Pressed);
        };
    }

    protected override Type StyleKeyOverride => typeof(ToggleSwitch);
}