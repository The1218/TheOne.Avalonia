using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions;
using global::Avalonia.Media;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

internal sealed class ReportPageSettingsDialog : Window
{
    internal ReportDocument? Result { get; private set; }

    internal ReportPageSettingsDialog(ReportDocument source)
    {
        ArgumentNullException.ThrowIfNull(source);
        ReportDialogChrome.Initialize(this, "页面设置", 800, 660);
        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,*,Auto") };
        root.Children.Add(ReportDialogChrome.Header("PageNumber", "页面设置", "设置纸张、方向，以及每页固定的页眉与页脚区域。"));

        var body = new Grid { ColumnDefinitions = new ColumnDefinitions("*,224"), ColumnSpacing = 24, Margin = new Thickness(24, 20) };
        Grid.SetRow(body, 1); root.Children.Add(body);
        var form = new StackPanel { Spacing = 16 };
        var scroll = new ScrollViewer { Name = "PageSettingsFormScroll", Content = form, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        body.Children.Add(scroll);
        var paper = new ComboBox { Name = "PaperPicker", ItemsSource = new[] { "A4", "A3", "Letter", "自定义" }, SelectedIndex = 3, HorizontalAlignment = HorizontalAlignment.Stretch };
        var orientation = new ComboBox { Name = "PageOrientationPicker", ItemsSource = new[] { "纵向", "横向" }, SelectedIndex = source.PageWidthMm > source.PageHeightMm ? 1 : 0, HorizontalAlignment = HorizontalAlignment.Stretch };
        AutomationProperties.SetName(paper, "纸张");
        AutomationProperties.SetName(orientation, "纸张方向");

        var paperGroup = new StackPanel { Spacing = 10 };
        paperGroup.Children.Add(ReportDialogChrome.Section("纸张与方向", "所有尺寸均以毫米（mm）为单位"));
        var paperRow = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 12 };
        paperRow.Children.Add(ReportDialogChrome.Field("纸张", paper));
        var direction = ReportDialogChrome.Field("方向", orientation); Grid.SetColumn(direction, 1); paperRow.Children.Add(direction);
        paperGroup.Children.Add(paperRow);
        var sizeRow = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 12 };
        NumericUpDown Number(string name, string label, double value) => ReportDialogChrome.Number(name, label, value);
        var width = Number("PaperWidth", "纸张宽度（mm）", source.PageWidthMm);
        var height = Number("PaperHeight", "纸张高度（mm）", source.PageHeightMm);
        sizeRow.Children.Add(ReportDialogChrome.Field("宽度", width));
        var heightField = ReportDialogChrome.Field("高度", height); Grid.SetColumn(heightField, 1); sizeRow.Children.Add(heightField);
        paperGroup.Children.Add(sizeRow); form.Children.Add(paperGroup);

        var margins = new StackPanel { Spacing = 10 };
        margins.Children.Add(ReportDialogChrome.Section("页边距", "纸张边缘到可排版区域的距离"));
        var marginGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), RowDefinitions = new RowDefinitions("Auto,Auto"), ColumnSpacing = 12, RowSpacing = 10 };
        var left = Number("MarginLeft", "左边距（mm）", source.LeftMargin);
        var right = Number("MarginRight", "右边距（mm）", source.RightMargin);
        var top = Number("MarginTop", "上边距（mm）", source.TopMargin);
        var bottom = Number("MarginBottom", "下边距（mm）", source.BottomMargin);
        void Place(Grid grid, Control control, string label, int row, int column)
        {
            var field = ReportDialogChrome.Field(label, control);
            Grid.SetRow(field, row); Grid.SetColumn(field, column); grid.Children.Add(field);
        }
        Place(marginGrid, left, "左", 0, 0); Place(marginGrid, right, "右", 0, 1);
        Place(marginGrid, top, "上", 1, 0); Place(marginGrid, bottom, "下", 1, 1);
        margins.Children.Add(marginGrid); form.Children.Add(margins);

        var bands = new StackPanel { Spacing = 10 };
        bands.Children.Add(ReportDialogChrome.Section("固定区域", "页眉、页脚在每页保留，明细在中间展开"));
        var bandGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,*"), ColumnSpacing = 12 };
        var header = Number("HeaderHeight", "页眉高度（mm）", source.HeaderHeightMm);
        var footer = Number("FooterHeight", "页脚高度（mm）", source.FooterHeightMm);
        Place(bandGrid, header, "页眉高度", 0, 0); Place(bandGrid, footer, "页脚高度", 0, 1);
        bands.Children.Add(bandGrid); form.Children.Add(bands);

        var preview = new StackPanel { Spacing = 12 };
        preview.Children.Add(ReportDialogChrome.Text("纸张预览", 14, weight: FontWeight.SemiBold));
        preview.Children.Add(ReportDialogChrome.Text("按真实宽高比显示", 12, muted: true));
        var paperCanvas = new Canvas { Background = Brushes.White };
        var page = new Border { Name = "PageSettingsPaperPreview", Background = Brushes.White, BorderThickness = new Thickness(1), Child = paperCanvas, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        page.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        var sheetArea = new Grid { Height = 230, Children = { page } }; preview.Children.Add(sheetArea);
        var dimensions = ReportDialogChrome.Text("", 13, weight: FontWeight.Medium); dimensions.Name = "PaperDimensions"; dimensions.HorizontalAlignment = HorizontalAlignment.Center; preview.Children.Add(dimensions);
        var detailSize = ReportDialogChrome.Text("", 12, muted: true); detailSize.Name = "PrintableDimensions"; detailSize.TextWrapping = TextWrapping.Wrap; preview.Children.Add(detailSize);
        var previewFrame = new Border { Padding = new Thickness(16), CornerRadius = new CornerRadius(10), Child = preview };
        previewFrame.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Subtle"));
        Grid.SetColumn(previewFrame, 1); body.Children.Add(previewFrame);

        var error = ReportDialogChrome.Text("设置将应用于整份报表。", 12, muted: true);
        error.Name = "PageSettingsValidation"; error.TextWrapping = TextWrapping.Wrap;
        var cancel = ReportDialogChrome.Button("CancelPageSettings", "取消");
        cancel.IsCancel = true; cancel.Click += (_, _) => Close(null);
        var apply = ReportDialogChrome.Button("ApplyPageSettings", "应用设置", primary: true);
        apply.IsDefault = true;
        var footerBar = ReportDialogChrome.Footer(error, cancel, apply); Grid.SetRow(footerBar, 2); root.Children.Add(footerBar);
        Content = root;

        var syncing = false;
        double Value(NumericUpDown editor) => (double)(editor.Value ?? 0);
        string? ValidateDimensions()
        {
            var w = Value(width); var h = Value(height);
            if (w <= 0 || h <= 0) return "纸张宽度和高度必须大于 0。";
            if (Value(left) + Value(right) >= w) return "左右页边距之和必须小于纸张宽度。";
            if (Value(top) + Value(bottom) + Value(header) + Value(footer) >= h) return "页边距、页眉和页脚占满了纸张，请为明细保留空间。";
            return null;
        }
        void UpdatePreview()
        {
            var w = Math.Max(1, Value(width)); var h = Math.Max(1, Value(height)); var scale = Math.Min(178 / w, 210 / h);
            page.Width = w * scale; page.Height = h * scale;
            paperCanvas.Width = Math.Max(1, w * scale - 2); paperCanvas.Height = Math.Max(1, h * scale - 2); paperCanvas.Children.Clear();
            var contentLeft = Math.Clamp(Value(left) * scale, 0, paperCanvas.Width);
            var contentTop = Math.Clamp(Value(top) * scale, 0, paperCanvas.Height);
            var contentWidth = Math.Max(0, paperCanvas.Width - contentLeft - Value(right) * scale);
            var contentHeight = Math.Max(0, paperCanvas.Height - contentTop - Value(bottom) * scale);
            var headerHeight = Math.Min(contentHeight, Value(header) * scale);
            var footerHeight = Math.Min(Math.Max(0, contentHeight - headerHeight), Value(footer) * scale);
            void Band(string label, double y, double bandHeight, bool fixedBand)
            {
                if (contentWidth <= 0 || bandHeight <= 0) return;
                var block = new Border { Width = contentWidth, Height = bandHeight, BorderThickness = new Thickness(.5), ClipToBounds = true };
                block.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
                if (fixedBand) block.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.AccentSoft"));
                block.Child = new TextBlock { Text = label, FontSize = 9, Foreground = Brushes.DimGray, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                Canvas.SetLeft(block, contentLeft); Canvas.SetTop(block, y); paperCanvas.Children.Add(block);
            }
            Band("页眉", contentTop, headerHeight, true);
            Band("明细", contentTop + headerHeight, Math.Max(0, contentHeight - headerHeight - footerHeight), false);
            Band("页脚", contentTop + contentHeight - footerHeight, footerHeight, true);
            dimensions.Text = $"{Value(width):0.##} × {Value(height):0.##} mm";
            detailSize.Text = $"明细可用区域\n{Math.Max(0, Value(width) - Value(left) - Value(right)):0.##} × {Math.Max(0, Value(height) - Value(top) - Value(bottom) - Value(header) - Value(footer)):0.##} mm";
            var validation = ValidateDimensions();
            apply.IsEnabled = validation is null;
            error.Text = validation ?? "设置将应用于整份报表。";
            error.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension(validation is null ? "One.Muted" : "One.Danger"));
        }
        void SetPaper()
        {
            if (syncing) return;
            syncing = true;
            try
            {
                var size = paper.SelectedIndex switch { 0 => (210m, 297m), 1 => (297m, 420m), 2 => (215.9m, 279.4m), _ => (width.Value ?? 210, height.Value ?? 297) };
                width.Value = orientation.SelectedIndex == 1 ? Math.Max(size.Item1, size.Item2) : Math.Min(size.Item1, size.Item2);
                height.Value = orientation.SelectedIndex == 1 ? Math.Min(size.Item1, size.Item2) : Math.Max(size.Item1, size.Item2);
            }
            finally { syncing = false; }
            UpdatePreview();
        }
        paper.SelectionChanged += (_, _) => SetPaper(); orientation.SelectionChanged += (_, _) => SetPaper();
        foreach (var editor in new[] { width, height, left, right, top, bottom, header, footer })
        {
            editor.PropertyChanged += (_, args) =>
            {
                if (args.Property != NumericUpDown.ValueProperty || syncing) return;
                if (ReferenceEquals(editor, width) || ReferenceEquals(editor, height))
                {
                    syncing = true;
                    paper.SelectedIndex = 3;
                    orientation.SelectedIndex = Value(width) > Value(height) ? 1 : 0;
                    syncing = false;
                }
                UpdatePreview();
            };
        }
        var shortSide = Math.Min(source.PageWidthMm, source.PageHeightMm); var longSide = Math.Max(source.PageWidthMm, source.PageHeightMm);
        paper.SelectedIndex = shortSide == 210 && longSide == 297 ? 0 : shortSide == 297 && longSide == 420 ? 1 : shortSide == 215.9 && longSide == 279.4 ? 2 : 3;
        UpdatePreview();
        apply.Click += (_, _) =>
        {
            try
            {
                var draft = ReportDesign.Copy(source);
                draft.PageWidthMm = Value(width); draft.PageHeightMm = Value(height);
                draft.MarginLeftMm = Value(left); draft.MarginRightMm = Value(right); draft.MarginTopMm = Value(top); draft.MarginBottomMm = Value(bottom);
                draft.HeaderHeightMm = Value(header); draft.FooterHeightMm = Value(footer);
                foreach (var element in draft.Elements.Where(e => e.Kind != ReportElementKind.Table && e.Y >= source.FooterTop)) element.Y += draft.FooterTop - source.FooterTop;
                ReportPaginator.Paginate(draft); Result = draft; Close(draft);
            }
            catch (ArgumentException exception)
            {
                error.Text = exception.Message;
                error.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension("One.Danger"));
            }
        };
    }
}

internal static class ReportDialogChrome
{
    internal static void Initialize(Window window, string title, double width, double height)
    {
        window.Title = title; window.Width = width; window.Height = height;
        window.MinWidth = 680; window.MinHeight = 480; window.CanResize = true;
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        window.Bind(Window.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        window.Bind(Window.ForegroundProperty, new DynamicResourceExtension("One.Text"));
        window.Opened += (_, _) =>
        {
            if (window.Owner is not { } owner) return;
            window.RequestedThemeVariant = owner.ActualThemeVariant;
            ControlSizing.SetSize(window, ControlSizing.GetSize(owner));
        };
    }

    internal static TextBlock Text(string text, double fontSize = 14, bool muted = false, FontWeight? weight = null)
    {
        var result = new TextBlock { Text = text, FontSize = fontSize, FontWeight = weight ?? FontWeight.Normal };
        result.Bind(TextBlock.ForegroundProperty, new DynamicResourceExtension(muted ? "One.Muted" : "One.Text"));
        return result;
    }

    internal static Control Section(string title, string subtitle)
    {
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 12 };
        var heading = Text(title, 14, weight: FontWeight.SemiBold); heading.VerticalAlignment = VerticalAlignment.Center;
        content.Children.Add(heading);
        var description = Text(subtitle, 12, muted: true);
        description.TextWrapping = TextWrapping.Wrap; description.TextAlignment = TextAlignment.Right; description.VerticalAlignment = VerticalAlignment.Center;
        Grid.SetColumn(description, 1); content.Children.Add(description); return content;
    }

    internal static Border Header(string iconKind, string title, string subtitle)
    {
        var icon = new ReportIcon { Kind = iconKind, Width = 22, Height = 22 };
        icon.Bind(ReportIcon.ForegroundProperty, new DynamicResourceExtension("One.Accent"));
        var iconFrame = new Border { Width = 44, Height = 44, CornerRadius = new CornerRadius(10), Child = icon, VerticalAlignment = VerticalAlignment.Top };
        iconFrame.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.AccentSoft"));
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("Auto,*"), ColumnSpacing = 14 };
        grid.Children.Add(iconFrame);
        var text = new StackPanel { Spacing = 4 };
        text.Children.Add(Text(title, 22, weight: FontWeight.SemiBold));
        var description = Text(subtitle, 13, muted: true); description.TextWrapping = TextWrapping.Wrap; text.Children.Add(description);
        Grid.SetColumn(text, 1); grid.Children.Add(text);
        var border = new Border { Padding = new Thickness(24, 22, 24, 18), BorderThickness = new Thickness(0, 0, 0, 1), Child = grid };
        border.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line")); return border;
    }

    internal static Control Field(string label, Control editor)
    {
        var panel = new StackPanel { Spacing = 4 };
        panel.Children.Add(Text(label, 12, muted: true)); panel.Children.Add(editor); return panel;
    }

    internal static NumericUpDown Number(string name, string label, double value)
    {
        var number = new NumericUpDown { Name = name, Value = (decimal)value, Minimum = 0, Maximum = 2000, Increment = 1, FormatString = "0.##", HorizontalAlignment = HorizontalAlignment.Stretch };
        AutomationProperties.SetName(number, label); return number;
    }

    internal static Button Button(string name, string content, bool primary = false)
    {
        var button = new Button { Name = name, Content = content, MinWidth = 92, Padding = new Thickness(18, 7), HorizontalContentAlignment = HorizontalAlignment.Center };
        if (primary) button.Classes.Add("primary"); return button;
    }

    internal static Border Footer(Control status, Button cancel, Button apply)
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto,Auto"), ColumnSpacing = 10 };
        status.VerticalAlignment = VerticalAlignment.Center; status.Margin = new Thickness(0, 0, 16, 0);
        grid.Children.Add(status); Grid.SetColumn(cancel, 1); grid.Children.Add(cancel); Grid.SetColumn(apply, 2); grid.Children.Add(apply);
        var frame = new Border { Padding = new Thickness(24, 16), BorderThickness = new Thickness(0, 1, 0, 0), Child = grid };
        frame.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        return frame;
    }
}
