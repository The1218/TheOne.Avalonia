using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed partial class ReportDataSourceDialog
{
    private TextBox _sourceName = null!, _search = null!;
    private ListBox _selectionList = null!;
    private readonly List<DataChoice> _choices = [];

    private sealed class DataChoice(ReportDataNode node, bool selected)
    {
        internal ReportDataNode Node { get; } = node;
        internal bool IsSelected { get; set; } = selected;
    }

    private Control CreateSelection()
    {
        var form = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,Auto,*"), RowSpacing = 12 };
        var title = new StackPanel { Spacing = 4 };
        title.Children.Add(ReportDialogChrome.Text("勾选需要的数据项", 17, weight: FontWeight.SemiBold));
        title.Children.Add(ReportDialogChrome.Text("单个属性为一项，整个数据集为一项。仅筛选展示项，不裁剪原始数据。", 12, true));
        form.Children.Add(title);
        _sourceName = new TextBox { Name = "ApiDataSourceName", Text = _initial.SourceName };
        var name = Field("SourceName", "数据源名称", _sourceName);
        Grid.SetRow(name, 1); form.Children.Add(name);
        _search = new TextBox { Name = "ApiDataItemSearch", PlaceholderText = "搜索数据项名称或路径" };
        Grid.SetRow(_search, 2); form.Children.Add(_search);
        _selectionList = new ListBox
        {
            Name = "ApiDataItemChoices", MinHeight = 140, MaxHeight = 320,
            ItemTemplate = new FuncDataTemplate<DataChoice>((choice, _) =>
            {
                if (choice is null) return new Border();
                if (choice.Node.Kind == ReportDataNodeKind.Object)
                    return ReportDialogChrome.Text(choice.Node.Name, 13, true, FontWeight.SemiBold);
                var check = new CheckBox { IsChecked = choice.IsSelected, VerticalAlignment = VerticalAlignment.Top,
                    Name = "ApiDataItemToggle", Margin = new Thickness(0, 2, 10, 0) };
                AutomationProperties.SetName(check, choice.Node.Name + " " + choice.Node.Pointer);
                check.IsCheckedChanged += (_, _) => { choice.IsSelected = check.IsChecked == true; SelectionChanged(); };
                var row = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*"), ColumnSpacing = 12, Margin = new Thickness(2, 3) };
                row.Children.Add(check);
                var text = new StackPanel { Spacing = 3 };
                text.Children.Add(ReportDialogChrome.Text(choice.Node.Name));
                text.Children.Add(ReportDialogChrome.Text(choice.Node.Pointer.Length == 0 ? "/" : choice.Node.Pointer, 12, true));
                if (choice.Node.Kind == ReportDataNodeKind.Collection && choice.Node.Children.Count > 0)
                {
                    ToolTip.SetTip(row, "字段：" + string.Join(" · ", choice.Node.Children.Take(8).Select(node => node.Name)) +
                        (choice.Node.Children.Count > 8 ? " …" : ""));
                }
                Add(row, text, 1);
                Add(row, ReportDialogChrome.Text(choice.Node.TypeLabel, 12, true), 2);
                var preview = ReportDialogChrome.Text(choice.Node.Preview, 12, true);
                preview.TextTrimming = TextTrimming.CharacterEllipsis;
                preview.HorizontalAlignment = HorizontalAlignment.Right;
                ToolTip.SetTip(preview, choice.Node.Preview);
                Add(row, preview, 3);
                return row;
            })
        };
        ScrollViewer.SetIsScrollChainingEnabled(_selectionList, false);
        ScrollViewer.SetHorizontalScrollBarVisibility(_selectionList, ScrollBarVisibility.Disabled);
        var selection = new StepField { MemberName = "Selection", Content = _selectionList };
        Grid.SetRow(selection, 3); form.Children.Add(selection);
        ObserveText(_search, FilterSelection);
        ObserveText(_sourceName, SelectionChanged);
        var viewport = Page(form);
        viewport.SizeChanged += (_, _) => _selectionList.MaxHeight = Math.Max(140,
            viewport.Bounds.Height - title.Bounds.Height - name.Bounds.Height - _search.Bounds.Height - 36);
        return viewport;
    }

    private void PopulateSelection(IReadOnlyList<ReportDataNode> items)
    {
        var initialRequest = _initial with { SourceName = "接口数据", SelectedDataPointers = null };
        var selected = _restoreInitialSelection && ReadRequest() == initialRequest
            ? _initial.SelectedDataPointers?.ToHashSet(StringComparer.Ordinal) : null;
        _restoreInitialSelection = false;
        _choices.Clear();
        foreach (var node in items.OrderBy(item => item.Kind == ReportDataNodeKind.Collection))
            _choices.Add(new DataChoice(node, selected is null || selected.Contains(node.Pointer)));
        FilterSelection();
        SelectionChanged();
    }

    private void FilterSelection()
    {
        var search = _search.Text?.Trim() ?? "";
        var visible = _choices.Where(choice => search.Length == 0 ||
            choice.Node.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
            choice.Node.Pointer.Contains(search, StringComparison.OrdinalIgnoreCase)).ToArray();
        var rows = new List<DataChoice>();
        foreach (var kind in new[] { ReportDataNodeKind.Scalar, ReportDataNodeKind.Collection })
        {
            var group = visible.Where(choice => choice.Node.Kind == kind).ToArray();
            if (group.Length == 0) continue;
            rows.Add(new DataChoice(new ReportDataNode((kind == ReportDataNodeKind.Scalar ? "单值字段" : "数据集") +
                " · " + group.Length + " 项", "", ReportDataNodeKind.Object, "", []), false));
            rows.AddRange(group);
        }
        _selectionList.ItemsSource = rows;
    }

    private void SelectionChanged()
    {
        var count = _choices.Count(choice => choice.IsSelected);
        var collections = _choices.Count(choice => choice.IsSelected && choice.Node.Kind == ReportDataNodeKind.Collection);
        _status.Text = string.IsNullOrWhiteSpace(_sourceName.Text) ? "请输入数据源名称。" :
            $"已选 {count} 项：{count - collections} 个单值、{collections} 个数据集。";
        _complete.Changed();
    }
}
