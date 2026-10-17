using System.Globalization;
using Avalonia.Media;

namespace TheOne.Avalonia.Reporting;

public sealed record ReportLayoutColumn(string Header, string FieldKey, double Width, bool IsNumeric);

public sealed record ReportLayoutRow(int SourceIndex, IReadOnlyList<string> Cells, double Height)
{
    public IReadOnlyList<string> Foregrounds { get; init; } = [];
}

public sealed record ReportLayoutElement(
    string Name,
    ReportElementKind Kind,
    double X,
    double Y,
    double Width,
    double Height,
    string Text,
    double FontSize,
    bool Bold,
    string Foreground,
    string Fill,
    bool ShowBorder);

public sealed record ReportPage(int Number, IReadOnlyList<ReportLayoutRow> Rows, bool ShowSummary);

/// <summary>Immutable text and geometry snapshot shared by preview and the designer's drawing surface.</summary>
public sealed record ReportLayout(
    string Title,
    string Subtitle,
    string Description,
    string Footer,
    double PageWidth,
    double PageHeight,
    double Margin,
    double FontSize,
    double TableTop,
    double HeaderHeight,
    double DetailBottom,
    double SummaryHeight,
    IReadOnlyList<ReportLayoutColumn> Columns,
    IReadOnlyList<string> Summary,
    IReadOnlyList<ReportPage> Pages)
{
    public double ContentWidth => TableWidth > 0 ? TableWidth : PageWidth - Margin * 2;
    public double TableX { get; init; } = Margin;
    public double TableWidth { get; init; }
    public IReadOnlyList<ReportLayoutElement> Elements { get; init; } = [];
}

/// <summary>Single table pagination: repeated title/header, atomic wrapped rows, last-page totals and page numbers.</summary>
public static class ReportPaginator
{
    public const double DipsPerMillimetre = 96d / 25.4;
    internal static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("zh-CN");

    internal static FormattedText MeasureText(string text, double width, double fontSize, IBrush? brush = null,
        bool bold = false)
    {
        return new FormattedText(text, Culture, FlowDirection.LeftToRight,
                new Typeface(FontFamily.Default, FontStyle.Normal, bold ? FontWeight.SemiBold : FontWeight.Normal),
                fontSize, brush ?? Brushes.Black)
            { MaxTextWidth = Math.Max(1, width) };
    }

    internal static FormattedText MeasureElementText(string text, double width, ReportElement element,
        IBrush? brush = null, bool bold = false, double? fontSize = null)
    {
        if (element.ListStyle != ReportTextListStyle.None && (element.Kind == ReportElementKind.Text ||
                                                              (element.Kind == ReportElementKind.Table && !bold)))
            text = string.Join('\n',
                text.Split('\n')
                    .Select(line => (element.ListStyle == ReportTextListStyle.Bullet ? "•  " : "☐  ") + line));
        var result = new FormattedText(text, Culture, FlowDirection.LeftToRight,
            new Typeface(
                string.IsNullOrWhiteSpace(element.FontFamily) ? FontFamily.Default : new FontFamily(element.FontFamily),
                element.Italic ? FontStyle.Italic : FontStyle.Normal,
                bold || element.Bold ? FontWeight.SemiBold : FontWeight.Normal), fontSize ?? element.FontSize,
            brush ?? Brushes.Black) { MaxTextWidth = Math.Max(1, width) };
        if (element.Underline) result.SetTextDecorations(TextDecorations.Underline);
        if (element.TextAlignment == ReportTextAlignment.Justify) result.TextAlignment = TextAlignment.Justify;
        return result;
    }

    public static ReportLayout Paginate(ReportDocument document, ReportDataSource? source = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        static bool Positive(double value)
        {
            return double.IsFinite(value) && value > 0;
        }

        if (!Positive(document.PageWidthMm) || !Positive(document.PageHeightMm) ||
            !double.IsFinite(document.MarginMm) || document.MarginMm < 0 ||
            !Positive(document.FontSize) || document.FontSize < 8 || document.FontSize > 32 ||
            !Positive(document.MinimumRowHeight))
            throw new ArgumentException("纸张、边距、行高或字号无效（字号范围 8–32）。");
        var width = document.PageWidthMm * DipsPerMillimetre;
        var height = document.PageHeightMm * DipsPerMillimetre;
        if (new[]
            {
                document.LeftMargin, document.TopMargin, document.RightMargin, document.BottomMargin,
                document.HeaderHeightMm, document.FooterHeightMm
            }.Any(v => !double.IsFinite(v) || v < 0)
            || document.HeaderBottom >= document.FooterTop) throw new ArgumentException("页边距或页眉页脚区域无效。");
        var margin = document.LeftMargin * DipsPerMillimetre;
        var table = document.Elements.FirstOrDefault(e => e.Kind == ReportElementKind.Table);
        var tableX = table?.X * DipsPerMillimetre ?? margin;
        var contentWidth = table?.Width * DipsPerMillimetre ??
                           width - (document.LeftMargin + document.RightMargin) * DipsPerMillimetre;
        if (!double.IsFinite(width) || !double.IsFinite(height) || !double.IsFinite(margin))
            throw new ArgumentException("纸张尺寸无效。");
        foreach (var element in document.Elements)
            if (!double.IsFinite(element.X) || !double.IsFinite(element.Y) || element.X < 0 || element.Y < 0 ||
                !Positive(element.Width) || !Positive(element.Height) ||
                element.X + element.Width > document.PageWidthMm + .01 ||
                element.Y + element.Height > document.PageHeightMm + .01 || !Positive(element.FontSize) ||
                !Enum.IsDefined(element.Kind) || !Enum.IsDefined(element.TextAlignment) ||
                !Enum.IsDefined(element.ImageFit) || !Enum.IsDefined(element.ListStyle) || element.FontSize > 200 ||
                !Positive(element.TableHeaderHeightMm) || !Color.TryParse(element.Foreground, out _) ||
                !Color.TryParse(element.Fill, out _))
                throw new ArgumentException($"元素“{element.Name}”的位置、尺寸或样式无效。");
        var elements = Array.AsReadOnly(document.Elements.Select(element =>
        {
            var text = element.Text;
            if (element.FieldKey.Length > 0)
            {
                if (source is not null && !source.Fields.Any(f => f.Key == element.FieldKey))
                    throw new ArgumentException($"元素“{element.Name}”的字段不存在：{element.FieldKey}");
                try
                {
                    text = source is not null && (source.RowCount > 0 || ReportJsonData.IsRootField(element.FieldKey))
                        ? Convert.ToString(source.GetValue(0, element.FieldKey), Culture) ?? ""
                        : "{" + element.FieldKey + "}";
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"元素“{element.Name}”的字段读取失败：{exception.Message}", exception);
                }
            }

            if (element.Kind == ReportElementKind.CheckBox && element.FieldKey.Length > 0 && source is not null &&
                (source.RowCount > 0 || ReportJsonData.IsRootField(element.FieldKey)) &&
                !ReportBarcode.IsCheckValue(text))
                throw new ArgumentException($"复选框“{element.Name}”需要布尔值、1/0 或 是/否 字段。");
            if (element.Kind == ReportElementKind.Barcode && (element.FieldKey.Length == 0 ||
                                                              (source is not null && (source.RowCount > 0 ||
                                                                  ReportJsonData.IsRootField(element.FieldKey)))))
            {
                var runs = ReportBarcode.Encode(text);
                if ((element.Width * DipsPerMillimetre - 4) / (runs.Sum(run => run - '0') + 20) < .75)
                    throw new ArgumentException($"条码“{element.Name}”过窄，请增大宽度或减少字符。");
                if (element.Height * DipsPerMillimetre < (element.BarcodeShowText ? element.FontSize + 6 : 0) + 20)
                    throw new ArgumentException($"条码“{element.Name}”高度不足，请增大高度或隐藏文字。");
            }

            return new ReportLayoutElement(element.Name, element.Kind, element.X * DipsPerMillimetre,
                element.Y * DipsPerMillimetre,
                element.Width * DipsPerMillimetre, element.Height * DipsPerMillimetre, text, element.FontSize,
                element.Bold, element.Foreground, element.Fill, element.ShowBorder);
        }).ToArray());
        if (width - margin * 2 < 80 || height - margin * 2 < 240) throw new ArgumentException("纸张可用区域过小，请调整纸张或边距。");
        if (document.Columns.Count == 0)
            return new ReportLayout(document.Title, document.Subtitle, document.Description, document.Footer, width,
                height, margin, document.FontSize, 0, 0,
                document.FooterTop * DipsPerMillimetre, 0, [], [],
                Array.AsReadOnly(new[] { new ReportPage(1, [], false) })) { Elements = elements };
        if (contentWidth < 80 || height - margin * 2 < 240) throw new ArgumentException("纸张可用区域过小，请调整纸张或边距。");
        var fields = source?.Fields.ToDictionary(f => f.Key, StringComparer.Ordinal) ?? document.Columns
            .Where(c => c.FieldKey.Length > 0).Select(c => c.FieldKey).Distinct().ToDictionary(k => k,
                k => new ReportFieldInfo(k, k, ReportValueKind.Text), StringComparer.Ordinal);
        foreach (var column in document.Columns)
        {
            if (column.FieldKey.Length > 0 && !fields.ContainsKey(column.FieldKey))
                throw new ArgumentException($"列“{column.Header}”尚未绑定有效字段：{column.FieldKey}");
            if (!Positive(column.WidthWeight) || column.DecimalPlaces < 0 || column.DecimalPlaces > 8 ||
                !Enum.IsDefined(column.Format))
                throw new ArgumentException($"列“{column.Header}”的宽度或格式无效。");
            if (!Enum.IsDefined(column.Condition) || !Color.TryParse(column.ConditionColor, out _))
                throw new ArgumentException($"列“{column.Header}”的条件样式无效。");
            if (column.ConditionEnabled && source is not null &&
                (!fields.TryGetValue(column.FieldKey, out var conditionField) ||
                 conditionField.Kind != ReportValueKind.Number))
                throw new ArgumentException($"列“{column.Header}”的条件样式仅支持数字字段。");
            if (column.ShowSum && source is not null && (!fields.TryGetValue(column.FieldKey, out var sumField) ||
                                                         sumField.Kind != ReportValueKind.Number))
                throw new ArgumentException($"列“{column.Header}”仅数字字段可汇总。");
        }

        var weight = document.Columns.Sum(c => c.WidthWeight);
        if (!double.IsFinite(weight)) throw new ArgumentException("列宽总和无效。");
        var columns = document.Columns.Select(c => new ReportLayoutColumn(c.Header, c.FieldKey,
            contentWidth * c.WidthWeight / weight,
            fields.TryGetValue(c.FieldKey, out var field) && field.Kind == ReportValueKind.Number)).ToArray();
        if (columns.Any(c => c.Width < 32)) throw new ArgumentException("表格列过窄，请减少列数或调整列宽。");
        var font = table?.FontSize ?? document.FontSize;
        var tableTop = table is not null
            ? table.Y * DipsPerMillimetre
            : margin + MeasureText(document.Title, contentWidth, 27, bold: true).Height +
              MeasureText(document.Subtitle, contentWidth, 11).Height +
              MeasureText(document.Description, contentWidth, 11).Height + 40;
        var headerHeight = Math.Max(table is null ? 34 : table.TableHeaderHeightMm * DipsPerMillimetre,
            columns.Max(c =>
                (table is null
                    ? MeasureText(c.Header, c.Width - 16, font, bold: true)
                    : MeasureElementText(c.Header, c.Width - 16, table, bold: true)).Height + 16));
        var minimumRowHeight = table is null
            ? document.MinimumRowHeight
            : Math.Max(document.MinimumRowHeight, table.Height * DipsPerMillimetre - headerHeight);
        var footerTop = document.FooterTop * DipsPerMillimetre;
        var bottom = Math.Min(footerTop,
            elements.Where(e => e.Kind != ReportElementKind.Table && e.Y > tableTop + 40).Select(e => e.Y - 12)
                .DefaultIfEmpty(footerTop).Min());
        var summaryHeight = Math.Max(36, font * 2 + 12);
        var capacity = bottom - tableTop - headerHeight;
        if (capacity < summaryHeight) throw new ArgumentException("标题或表头过高，当前页面无法容纳明细和汇总。");
        var pages = new List<ReportPage>();
        var current = new List<ReportLayoutRow>();
        var used = 0d;
        var sums = new decimal[columns.Length];

        void Finish(bool summary)
        {
            pages.Add(new ReportPage(pages.Count + 1, Array.AsReadOnly(current.ToArray()), summary));
            current.Clear();
            used = 0;
        }

        var repeat = source is not null && document.Columns.Any(c => c.FieldKey.Length > 0);
        var rowCount = repeat ? source!.RowCount : 1;
        for (var index = 0; index < rowCount; index++)
        {
            var cells = new string[columns.Length];
            var colors = new string[columns.Length];
            Array.Fill(colors, "");
            for (var c = 0; c < columns.Length; c++)
            {
                object? value;
                try
                {
                    value = source is not null && columns[c].FieldKey.Length > 0
                        ? source.GetValue(index, columns[c].FieldKey)
                        : null;
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"第 {index + 1} 行字段 {columns[c].FieldKey} 读取失败：{exception.Message}",
                        exception);
                }

                cells[c] = columns[c].FieldKey.Length == 0 ? document.Columns[c].Text :
                    source is null ? "{" + columns[c].FieldKey + "}" :
                    FormatValue(value, document.Columns[c], fields[columns[c].FieldKey].Kind);
                var definition = document.Columns[c];
                if (definition.ConditionEnabled && value is not null)
                    try
                    {
                        var number = Convert.ToDecimal(value, Culture);
                        if (definition.Condition switch
                            {
                                ReportCondition.GreaterThan => number > definition.ConditionValue,
                                ReportCondition.LessThan => number < definition.ConditionValue,
                                _ => number == definition.ConditionValue
                            }) colors[c] = definition.ConditionColor;
                    }
                    catch (Exception exception) when (exception is FormatException or InvalidCastException
                                                          or OverflowException)
                    {
                        throw new ArgumentException($"第 {index + 1} 行字段 {definition.FieldKey} 无法进行数字条件比较。", exception);
                    }

                if (document.Columns[c].ShowSum && value is not null)
                    try
                    {
                        sums[c] += Convert.ToDecimal(value, Culture);
                    }
                    catch (Exception exception) when (exception is FormatException or InvalidCastException
                                                          or OverflowException)
                    {
                        throw new ArgumentException($"第 {index + 1} 行字段 {columns[c].FieldKey} 无法进行数字汇总。", exception);
                    }
            }

            var rowHeight = Math.Max(minimumRowHeight,
                columns.Select((c, i) =>
                    (table is null
                        ? MeasureText(cells[i], c.Width - 16, font)
                        : MeasureElementText(cells[i], c.Width - 16, table)).Height + 14).Max());
            if (rowHeight > capacity) throw new ArgumentException($"第 {index + 1} 行内容超过单页高度，请缩小字号或调整列宽。");
            if (current.Count > 0 && used + rowHeight > capacity + .001) Finish(false);
            current.Add(new ReportLayoutRow(index, Array.AsReadOnly(cells), rowHeight)
                { Foregrounds = Array.AsReadOnly(colors) });
            used += rowHeight;
        }

        var hasSummary = document.Columns.Any(c => c.ShowSum);
        if (hasSummary && used + summaryHeight + 8 > capacity + .001) Finish(false);
        Finish(hasSummary);
        var summaryCells = document.Columns
            .Select((c, i) => c.ShowSum ? FormatValue(sums[i], c, ReportValueKind.Number) : "").ToArray();
        return new ReportLayout(document.Title, document.Subtitle, document.Description, document.Footer, width, height,
            margin, font, tableTop,
            headerHeight, bottom, summaryHeight, Array.AsReadOnly(columns), Array.AsReadOnly(summaryCells),
            Array.AsReadOnly(pages.ToArray())) { Elements = elements, TableX = tableX, TableWidth = contentWidth };
    }

    internal static string FormatValue(object? value, ReportColumn column, ReportValueKind kind)
    {
        if (value is null) return "";
        var format = column.Format == ReportValueFormat.Automatic
            ? kind switch
            {
                ReportValueKind.Number => ReportValueFormat.Number, ReportValueKind.Date => ReportValueFormat.Date,
                _ => ReportValueFormat.Text
            }
            : column.Format;
        if (format == ReportValueFormat.Date)
            return value switch
            {
                DateOnly date => date.ToString("yyyy.MM.dd", Culture),
                DateTime date => date.ToString("yyyy.MM.dd", Culture),
                DateTimeOffset date => date.ToString("yyyy.MM.dd", Culture),
                _ => throw new ArgumentException($"列“{column.Header}”需要日期值。")
            };
        if (format is ReportValueFormat.Number or ReportValueFormat.Currency)
        {
            decimal number;
            try
            {
                number = Convert.ToDecimal(value, Culture);
            }
            catch (Exception exception) when (exception is FormatException or InvalidCastException or OverflowException)
            {
                throw new ArgumentException($"列“{column.Header}”需要有效数字。", exception);
            }

            return (format == ReportValueFormat.Currency ? "¥ " : "") +
                   number.ToString((column.UseThousandsSeparator ? "N" : "F") + column.DecimalPlaces, Culture);
        }

        return Convert.ToString(value, Culture) ?? "";
    }
}