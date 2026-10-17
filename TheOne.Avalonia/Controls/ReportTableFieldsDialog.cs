using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed record ReportColumnFieldBinding(ReportColumn Column, string FieldKey);

/// <summary>Edits only local field choices. The caller commits the returned mappings as one report edit.</summary>
internal sealed class ReportTableFieldsDialog : Window
{
    internal ReportTableFieldsDialog(ReportDocument document, IReadOnlyList<ReportFieldInfo> fields,
        string collectionName,
        IReadOnlyDictionary<string, string>? fieldSamples = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(fields);
        ReportDialogChrome.Initialize(this, "配置表格字段", 790, 620);
        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*,Auto") };
        root.Children.Add(ReportDialogChrome.Header("Table", "配置表格字段", "为每一列选择集合字段；未选择字段的列保留静态内容。"));

        var descriptors = fields.DistinctBy(field => field.Key, StringComparer.Ordinal).ToArray();
        var choices = new[] { new FieldChoice(null) }.Concat(descriptors.Select(field => new FieldChoice(field)))
            .ToArray();
        var summary = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 16 };
        var collection = ReportDialogChrome.Text(collectionName, weight: FontWeight.SemiBold);
        collection.Name = "TableFieldsCollection";
        collection.TextTrimming = TextTrimming.CharacterEllipsis;
        ToolTip.SetTip(collection, collectionName);
        summary.Children.Add(collection);
        var count = ReportDialogChrome.Text($"{document.Columns.Count} 列 · {descriptors.Length} 个字段", 12, true);
        count.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(count, 1);
        summary.Children.Add(count);
        var summaryFrame = new Border
        {
            Margin = new Thickness(24, 16, 24, 0), Padding = new Thickness(14, 11), CornerRadius = new CornerRadius(8),
            Child = summary
        };
        summaryFrame.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Subtle"));
        Grid.SetRow(summaryFrame, 1);
        root.Children.Add(summaryFrame);

        var mappingArea = new Grid
            { RowDefinitions = new RowDefinitions("Auto,*"), Margin = new Thickness(24, 16, 24, 16) };
        Grid.SetRow(mappingArea, 2);
        root.Children.Add(mappingArea);

        Grid Row()
        {
            return new Grid { ColumnDefinitions = new ColumnDefinitions("28,150,*,164"), ColumnSpacing = 12 };
        }

        void At(Grid row, Control control, int column)
        {
            Grid.SetColumn(control, column);
            row.Children.Add(control);
        }

        var headings = Row();
        headings.Margin = new Thickness(0, 0, 0, 8);
        At(headings, ReportDialogChrome.Text("列", 12, true), 1);
        At(headings, ReportDialogChrome.Text("绑定字段", 12, true), 2);
        At(headings, ReportDialogChrome.Text("数据样例", 12, true), 3);
        mappingArea.Children.Add(headings);
        var rows = new StackPanel();
        var list = new ScrollViewer
        {
            Name = "TableFieldsScroll", Content = rows, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto
        };
        Grid.SetRow(list, 1);
        mappingArea.Children.Add(list);

        var status = ReportDialogChrome.Text("选择需要绑定的列。", 12, true);
        status.Name = "TableFieldsStatus";
        status.TextWrapping = TextWrapping.Wrap;
        var cancel = ReportDialogChrome.Button("CancelTableFields", "取消");
        cancel.IsCancel = true;
        cancel.Click += (_, _) => Close(null);
        var apply = ReportDialogChrome.Button("ApplyTableFields", "应用字段", true);
        apply.IsDefault = true;
        var footer = ReportDialogChrome.Footer(status, cancel, apply);
        Grid.SetRow(footer, 3);
        root.Children.Add(footer);
        var editors = new List<(ReportColumn Column, ComboBox Picker)>();

        void UpdateStatus()
        {
            var selected = editors.Count(entry => entry.Picker.SelectedItem is FieldChoice { Field: not null });
            apply.IsEnabled = descriptors.Length > 0 && editors.Count > 0;
            status.Text = descriptors.Length == 0 ? "当前集合没有可配置字段，请选择包含对象记录的集合。"
                : selected == 0 ? "所有列将使用静态内容，不绑定集合字段。"
                : $"将绑定 {selected} 列，其余列保留静态内容。";
        }

        for (var index = 0; index < document.Columns.Count; index++)
        {
            var column = document.Columns[index];
            var row = Row();
            row.Margin = new Thickness(0, 12);
            var number = ReportDialogChrome.Text((index + 1).ToString(CultureInfo.InvariantCulture), 12, true);
            number.VerticalAlignment = VerticalAlignment.Center;
            At(row, number, 0);
            var title = ReportDialogChrome.Text(column.Header, weight: FontWeight.Medium);
            title.TextTrimming = TextTrimming.CharacterEllipsis;
            title.VerticalAlignment = VerticalAlignment.Center;
            ToolTip.SetTip(title, column.Header);
            At(row, title, 1);
            var picker = new ComboBox
            {
                Name = "TableFieldPicker" + index, ItemsSource = choices,
                SelectedItem = choices.FirstOrDefault(choice => choice.Field?.Key == column.FieldKey) ?? choices[0],
                HorizontalAlignment = HorizontalAlignment.Stretch, IsEnabled = descriptors.Length > 0
            };
            AutomationProperties.SetName(picker, $"第 {index + 1} 列 {column.Header} 绑定字段");
            At(row, picker, 2);
            var sample = ReportDialogChrome.Text("—", 12, true);
            sample.TextTrimming = TextTrimming.CharacterEllipsis;
            sample.VerticalAlignment = VerticalAlignment.Center;
            At(row, sample, 3);

            void UpdateSample()
            {
                var field = (picker.SelectedItem as FieldChoice)?.Field;
                var sampleText = "暂无样例";
                if (field is not null && fieldSamples is not null && fieldSamples.TryGetValue(field.Key, out var value))
                    sampleText = value;
                sample.Text = field is null ? "保留静态内容" : sampleText;
                ToolTip.SetTip(sample, field is null ? "不绑定字段，保留此列的静态内容" : field.Key + "\n" + sample.Text);
                ToolTip.SetTip(picker, field?.Key);
                UpdateStatus();
            }

            editors.Add((column, picker));
            picker.SelectionChanged += (_, _) => UpdateSample();
            UpdateSample();
            var frame = new Border { BorderThickness = new Thickness(0, 0, 0, 1), Child = row };
            frame.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
            rows.Children.Add(frame);
        }

        if (document.Columns.Count == 0)
        {
            var empty = ReportDialogChrome.Text("表格尚未配置列，请先添加列。", 14, true);
            empty.Margin = new Thickness(0, 28);
            rows.Children.Add(empty);
        }

        UpdateStatus();
        apply.Click += (_, _) =>
        {
            Result = Array.AsReadOnly(editors
                .Where(entry => entry.Picker.SelectedItem is FieldChoice { Field: not null })
                .Select(entry =>
                    new ReportColumnFieldBinding(entry.Column, ((FieldChoice)entry.Picker.SelectedItem!).Field!.Key))
                .ToArray());
            Close(Result);
        };
        Content = root;
    }

    internal IReadOnlyList<ReportColumnFieldBinding>? Result { get; private set; }

    private sealed record FieldChoice(ReportFieldInfo? Field)
    {
        public override string ToString()
        {
            return Field is null
                ? "不绑定（静态内容）"
                : Field.Label + " · " + Field.Kind switch
                {
                    ReportValueKind.Number => "数字", ReportValueKind.Date => "日期", _ => "文本"
                };
        }
    }
}