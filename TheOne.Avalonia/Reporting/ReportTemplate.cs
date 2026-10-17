using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaSharp;

namespace TheOne.Avalonia.Reporting;

/// <summary>Versioned, portable report templates. Data rows and API responses are never serialized.</summary>
public static class ReportTemplate
{
    public const int MaximumBytes = 32 * 1024 * 1024;

    /// <summary>A source-independent starting template matching the sales report design.</summary>
    public static ReportDocument CreateSalesDetail()
    {
        var document = new ReportDocument
        {
            Title = "销售明细报表", Subtitle = "SALES DETAIL REPORT", Description = "统计期间：2026.10.01 — 2026.10.15",
            Footer = "销售管理中心"
        };
        var headers = new[] { "日期", "订单编号", "客户", "产品名称", "数量", "单价", "金额" };
        var keys = new[] { "OrderDate", "OrderNo", "Customer", "ProductName", "Quantity", "UnitPrice", "Amount" };
        var widths = new[] { 1.05, 1.3, .9, 1.4, .85, .95, 1.15 };
        for (var i = 0; i < headers.Length; i++)
            document.Columns.Add(new ReportColumn
            {
                Header = headers[i], FieldKey = keys[i], WidthWeight = widths[i],
                Format = i == 6 ? ReportValueFormat.Currency : ReportValueFormat.Automatic, ShowSum = i == 6,
                DecimalPlaces = i == 4 ? 0 : 2
            });
        document.Elements.Add(new ReportElement
        {
            Name = "顶部线条", Kind = ReportElementKind.Line, X = 11, Y = 12, Width = 188, Height = 2,
            Foreground = "#006061"
        });
        document.Elements.Add(new ReportElement
        {
            Name = "报表标题", Kind = ReportElementKind.Text, X = 14, Y = 16, Width = 125, Height = 14,
            Text = document.Title, FontSize = 36, Bold = true
        });
        document.Elements.Add(new ReportElement
        {
            Name = "英文副标题", Kind = ReportElementKind.Text, X = 14, Y = 28, Width = 125, Height = 8,
            Text = document.Subtitle, FontSize = 16, Foreground = "#B0B7BC"
        });
        document.Elements.Add(new ReportElement
        {
            Name = "统计期间", Kind = ReportElementKind.Text, X = 14, Y = 35, Width = 150, Height = 8,
            Text = document.Description, FontSize = 13
        });
        document.Elements.Add(new ReportElement
        {
            Name = "公司名称", Kind = ReportElementKind.Text, X = 155, Y = 19, Width = 42, Height = 8, Text = "XX 科技有限公司",
            FontSize = 14, Bold = true, TextAlignment = ReportTextAlignment.Right
        });
        document.Elements.Add(new ReportElement
        {
            Name = "品牌说明", Kind = ReportElementKind.Text, X = 155, Y = 27, Width = 42, Height = 7,
            Text = "专业 · 高效 · 共赢", FontSize = 11, Foreground = "#657171", TextAlignment = ReportTextAlignment.Right
        });
        document.Elements.Add(new ReportElement
        {
            Name = "Table1", Kind = ReportElementKind.Table, X = 9, Y = 50, Width = 192, Height = 22, FontSize = 16,
            Foreground = "#FFFFFF", Fill = "#006061", ShowBorder = true
        });
        document.Elements.Add(new ReportElement
        {
            Name = "页脚线条", Kind = ReportElementKind.Line, X = 11, Y = 270, Width = 188, Height = 2,
            Foreground = "#006061"
        });
        document.Elements.Add(new ReportElement
        {
            Name = "页脚", Kind = ReportElementKind.Text, X = 11, Y = 273, Width = 100, Height = 8,
            Text = document.Footer, FontSize = 10, Foreground = "#657171"
        });
        document.Elements.Add(new ReportElement
        {
            Name = "页码", Kind = ReportElementKind.PageNumber, X = 133, Y = 273, Width = 66, Height = 8, FontSize = 10,
            Foreground = "#657171", TextAlignment = ReportTextAlignment.Right
        });
        return document;
    }

    public static string Save(ReportDocument document)
    {
        Validate(document);
        var json = JsonSerializer.Serialize(new ReportTemplatePayload { Document = document },
            ReportTemplateJsonContext.Default.ReportTemplatePayload);
        if (Encoding.UTF8.GetByteCount(json) > MaximumBytes) throw new ArgumentException("模板超过 32 MiB，请减少图片数量或大小。");
        return json;
    }

    public static ReportDocument Load(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        if (Encoding.UTF8.GetByteCount(json) > MaximumBytes) throw new ArgumentException("模板超过 32 MiB。");
        var template = JsonSerializer.Deserialize(json, ReportTemplateJsonContext.Default.ReportTemplatePayload) ??
                       throw new ArgumentException("模板内容为空。");
        if (template.Version != 1 || template.Document is null) throw new ArgumentException("模板版本不受支持或缺少报表定义。");
        Validate(template.Document);
        return template.Document;
    }

    private static void Validate(ReportDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        if (document.DataSourceUrl.Length > 0)
        {
            using var request = ReportApiRequest.FromDocument(document).CreateMessage();
        }

        if (document.Elements.Count > 1000 || document.Columns.Count > 100 ||
            document.Elements.Count(e => e.Kind == ReportElementKind.Table) > 1)
            throw new ArgumentException("模板最多包含 1000 个元素、100 列和一个明细表格。");
        foreach (var element in document.Elements)
        {
            if (!Enum.IsDefined(element.Kind) || !Enum.IsDefined(element.TextAlignment) ||
                !Enum.IsDefined(element.ImageFit)) throw new ArgumentException("模板中存在未知的元素或样式类型。");
            if (element.ImageData.Length > 0) ReportImageData.Decode(element.ImageData);
        }

        ReportPaginator.Paginate(document);
    }
}

internal sealed class ReportTemplatePayload
{
    [JsonRequired] public int Version { get; set; } = 1;
    [JsonRequired] public ReportDocument? Document { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = true, RespectNullableAnnotations = true,
    UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow)]
[JsonSerializable(typeof(ReportTemplatePayload))]
internal partial class ReportTemplateJsonContext : JsonSerializerContext;

internal static class ReportImageData
{
    internal const int MaximumBytes = 8 * 1024 * 1024;

    internal static byte[] Decode(string data)
    {
        if (data.Length > (MaximumBytes + 2L) / 3 * 4) throw new ArgumentException("单张图片不能超过 8 MiB。");
        byte[] bytes;
        try
        {
            bytes = Convert.FromBase64String(data);
        }
        catch (FormatException exception)
        {
            throw new ArgumentException("图片数据不是有效的 Base64。", exception);
        }

        if (bytes.Length > MaximumBytes) throw new ArgumentException("单张图片不能超过 8 MiB。");
        using var codec = SKCodec.Create(new MemoryStream(bytes, false));
        if (codec is null || codec.Info.Width <= 0 || codec.Info.Height <= 0 ||
            (long)codec.Info.Width * codec.Info.Height > 16_000_000)
            throw new ArgumentException("图片无法解码或超过 1600 万像素。");
        return bytes;
    }
}