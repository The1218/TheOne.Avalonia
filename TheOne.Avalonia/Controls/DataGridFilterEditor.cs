using System.Globalization;
using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Documents;
using global::Avalonia.Input;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions;
using global::Avalonia.Media;
using global::Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

internal sealed class DataGridFilterEditor
{
    private readonly DataGridFilterButton _owner;
    private readonly DataGrid _grid;
    private readonly DataGridColumn _column;
    private readonly Action _close;
    private readonly DataGridFilterKind _kind;
    private readonly CultureInfo _culture;
    private readonly TextBox _text = Input("DataGridFilterText", "关键词");
    private readonly TextBox _exact = Input("DataGridFilterExact", "精确值");
    private readonly TextBox _minimum = Input("DataGridFilterMinimum", "下限（包含）");
    private readonly TextBox _maximum = Input("DataGridFilterMaximum", "上限（包含）");
    private readonly ComboBox _mode = new() { Name = "DataGridFilterMode", ItemsSource = new[] { "精确匹配", "范围筛选" }, SelectedIndex = 0 };
    private readonly TextBlock _error = new() { Name = "DataGridFilterError", TextWrapping = TextWrapping.Wrap, IsVisible = false };
    private readonly StackPanel _panel = new() { Spacing = 12 };
    private readonly StackPanel _body = new() { Spacing = 10 };
    private readonly ScrollViewer _scroll = new() { HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
    public Control Content => _panel;
    public DataGridFilterEditor(DataGridFilterButton owner, DataGrid grid, DataGridColumn column, Action close)
    {
        _owner = owner; _grid = grid; _column = column; _close = close;
        _kind = DataGridColumnOptions.EffectiveKind(column); _culture = grid.CollectionView?.Culture ?? CultureInfo.CurrentCulture;
        _panel.Bind(TextElement.ForegroundProperty, new DynamicResourceExtension("One.Text"));
        _panel.SetValue(TextElement.FontWeightProperty, FontWeight.Normal);
        _error.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Danger"));
        var title = new TextBlock { Text = column.Header is string name ? "筛选 · " + name : "筛选当前列", FontWeight = FontWeight.SemiBold };
        _panel.Children.Add(title);
        if (_kind == DataGridFilterKind.Text)
        {
            _text.Text = (DataGridColumnOptions.GetFilterCondition(column) as TextFilterCondition)?.Text;
            _body.Children.Add(new TextBlock { Text = "包含关键词", FontWeight = FontWeight.Medium }); _body.Children.Add(_text);
            var hint = new TextBlock { Text = "匹配包含关键词的文本，忽略大小写。", TextWrapping = TextWrapping.Wrap };
            hint.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Muted")); _body.Children.Add(hint);
        }
        else
        {
            var draft = DataGridColumnOptions.GetFilterCondition(column)?.Draft(_culture) ?? new(NumericFilterMode.Exact, "", "", "");
            _mode.SelectedIndex = draft.Mode == NumericFilterMode.Range ? 1 : 0;
            _exact.Text = draft.Value; _minimum.Text = draft.Minimum; _maximum.Text = draft.Maximum;
            AutomationProperties.SetName(_mode, "数值筛选方式");
            var exactPanel = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "等于" }, _exact } };
            _mode.HorizontalAlignment = HorizontalAlignment.Stretch;
            _body.Children.Add(new TextBlock { Text = "筛选方式", FontWeight = FontWeight.Medium });
            _body.Children.Add(_mode); _body.Children.Add(exactPanel);
            var range = new StackPanel { Spacing = 10 };
            var lower = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "下限（含）" }, _minimum } };
            var upper = new StackPanel { Spacing = 4, Children = { new TextBlock { Text = "上限（含）" }, _maximum } };
            var hint = new TextBlock { Text = "边界值包含在结果中；留空表示不限。", TextWrapping = TextWrapping.Wrap };
            hint.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Muted"));
            range.Children.Add(lower); range.Children.Add(upper); range.Children.Add(hint); _body.Children.Add(range);
            void ModeChanged() { exactPanel.IsVisible = _mode.SelectedIndex == 0; range.IsVisible = _mode.SelectedIndex == 1; }
            _mode.SelectionChanged += (_, _) => ModeChanged(); ModeChanged();
        }
        _body.Children.Add(_error); _scroll.Content = _body; _panel.Children.Add(_scroll);
        var divider = new Border { Height = 1 };
        divider.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Line")); _panel.Children.Add(divider);
        Button ActionButton(string name, string label, Action action)
        {
            var button = new Button { Name = name, Content = label, HorizontalAlignment = HorizontalAlignment.Stretch };
            button.Click += (_, _) => action(); return button;
        }
        var actions = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 8 };
        var cancel = ActionButton("DataGridFilterCancel", "取消", _close);
        var apply = ActionButton("DataGridFilterApply", "应用筛选", Apply); apply.Classes.Add("primary");
        Grid.SetColumn(apply, 1); actions.Children.Add(cancel); actions.Children.Add(apply); _panel.Children.Add(actions);
        var clears = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 8 };
        var clear = ActionButton("DataGridFilterClear", "清除此列", () => Finish(DataGridFeatures.ClearFilter(_grid, _column))); clear.Classes.Add("link");
        var clearAll = ActionButton("DataGridFilterClearAll", "清除全部", () => Finish(DataGridFeatures.ClearAllFilters(_grid))); clearAll.Classes.Add("link");
        ToolTip.SetTip(clearAll, "清除全部列条件，保留业务筛选。");
        Grid.SetColumn(clearAll, 1); clears.Children.Add(clear); clears.Children.Add(clearAll); _panel.Children.Add(clears);
        _panel.KeyDown += (_, args) =>
        {
            if (args.Key == Key.Escape) { _close(); args.Handled = true; }
            else if (args.Key == Key.Enter && args.Source is not Button && args.Source is not ComboBox) { Apply(); args.Handled = true; }
        };
        UpdateSize();
    }
    private static TextBox Input(string name, string placeholder)
    {
        var box = new TextBox { Name = name, PlaceholderText = placeholder };
        AutomationProperties.SetName(box, placeholder); return box;
    }
    public void UpdateSize()
    {
        var size = ControlSizing.GetSize(_owner); ControlSizing.SetSize(_panel, size);
        _panel.SetValue(TextElement.FontSizeProperty, size switch { ControlSize.Small => 12d, ControlSize.Large => 16d, _ => 14d });
        var wanted = size switch { ControlSize.Small => 260, ControlSize.Large => 340, _ => 300 };
        var reserved = size switch { ControlSize.Small => 180, ControlSize.Large => 220, _ => 200 };
        _scroll.MaxHeight = Math.Max(80, (TopLevel.GetTopLevel(_owner)?.ClientSize.Height ?? 600) - reserved);
        _panel.Width = Math.Min(wanted, Math.Max(180, (TopLevel.GetTopLevel(_owner)?.ClientSize.Width ?? wanted + 32) - 32));
    }
    public void FocusInput() => Dispatcher.UIThread.Post(() => (_kind == DataGridFilterKind.Text ? _text : _mode.SelectedIndex == 0 ? _exact : _minimum).Focus(), DispatcherPriority.Input);
    private void Apply()
    {
        try
        {
            var condition = _kind == DataGridFilterKind.Text ? new TextFilterCondition(_text.Text) :
                DataGridFilterParsing.Number(DataGridAutoFields.Resolve(_column)!.ValueType, _mode.SelectedIndex == 1 ? NumericFilterMode.Range : NumericFilterMode.Exact,
                    _exact.Text ?? "", _minimum.Text ?? "", _maximum.Text ?? "", _culture);
            Finish(DataGridFeatures.ApplyFilter(_grid, _column, condition));
        }
        catch (ArgumentException exception) { Finish(new(false, exception.Message)); }
    }
    private void Finish(DataGridFilterApplyResult result)
    {
        if (result.Succeeded) _close();
        else { _error.Text = result.Error; _error.IsVisible = true; }
    }
}
