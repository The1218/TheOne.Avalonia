using Avalonia.Controls;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;

namespace TheOne.DesignPreview;

internal static class ReportDesignDemo
{
    internal static ReportDesign Create(int count = 96)
    {
        var document = new ReportDocument
        {
            Title = "销售明细报表", Subtitle = "SALES DETAIL REPORT", Description = "统计期间：2026.10.01 — 2026.10.15",
            Footer = "销售管理中心"
        };
        document.Columns.Add(new ReportColumn { Header = "日期", FieldKey = "OrderDate", WidthWeight = 1.05 });
        document.Columns.Add(new ReportColumn { Header = "订单编号", FieldKey = "OrderNo", WidthWeight = 1.4 });
        document.Columns.Add(new ReportColumn { Header = "客户", FieldKey = "Customer", WidthWeight = .9 });
        document.Columns.Add(new ReportColumn { Header = "产品名称", FieldKey = "ProductName", WidthWeight = 1.4 });
        document.Columns.Add(new ReportColumn
            { Header = "数量", FieldKey = "Quantity", WidthWeight = .65, DecimalPlaces = 0 });
        document.Columns.Add(new ReportColumn { Header = "单价", FieldKey = "UnitPrice", WidthWeight = .95 });
        document.Columns.Add(new ReportColumn
        {
            Header = "金额", FieldKey = "Amount", WidthWeight = 1.15, Format = ReportValueFormat.Currency, ShowSum = true
        });
        return new ReportDesign { Document = document, DataSource = Source(count) };
    }

    internal static ReportDataSource Source(int count)
    {
        var products = new[] { "笔记本电脑", "无线鼠标", "显示器", "机械键盘", "扩展坞" };
        var prices = new[] { 4999m, 89m, 1599m, 399m, 259m };
        var rows = Enumerable.Range(0, count).Select(i => new Sale(new DateOnly(2026, 10, 1).AddDays(i % 15),
            $"SO2610{i + 1:0000}",
            "客户 " + (char)('A' + i % 6), products[i % 5], i % 5 + 1, prices[i % 5]));
        return ReportDataSource.Create("Sales", rows,
            new ReportField<Sale>("OrderDate", "日期", r => r.OrderDate, ReportValueKind.Date),
            new ReportField<Sale>("OrderNo", "订单编号", r => r.OrderNo),
            new ReportField<Sale>("Customer", "客户", r => r.Customer),
            new ReportField<Sale>("ProductName", "产品名称", r => r.ProductName),
            new ReportField<Sale>("Quantity", "数量", r => r.Quantity, ReportValueKind.Number),
            new ReportField<Sale>("UnitPrice", "单价", r => r.UnitPrice, ReportValueKind.Number),
            new ReportField<Sale>("Amount", "金额", r => r.Amount, ReportValueKind.Number));
    }

    internal static ReportDesign CreateDesignerSample(bool withData = true)
    {
        var document = ReportTemplate.CreateSalesDetail();
        var view = new ReportDesign { Document = document, DataSource = withData ? Source(96) : null };
        view.SelectedElement = document.Elements.Single(e => e.Kind == ReportElementKind.Table);
        return view;
    }

    internal static Window CreateWindow()
    {
        return new Window
        {
            Title = "ReportDesign · 报表设计器", Width = 1600, Height = 1050, MinWidth = 1024, MinHeight = 680,
            Content = new ReportDesign { Document = new ReportDocument() }
        };
    }

    internal static ReportPreview CreatePreviewSample()
    {
        return new ReportPreview
        {
            DataSource = Source(96), DocumentJson = ReportTemplate.CreateSalesDetail().Save()
        };
    }

    internal sealed record Sale(
        DateOnly OrderDate,
        string OrderNo,
        string Customer,
        string ProductName,
        int Quantity,
        decimal UnitPrice)
    {
        public decimal Amount => Quantity * UnitPrice;
    }
}