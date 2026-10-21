using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class QRCodeDemoView : UserControl
{
    internal QRCodeDemoView()
    {
        var qr = new QRCode { Name = "QRCodeSample", Text = "TheOne 二维码 · https://example.com", Width = 192, Height = 192,
            HorizontalAlignment = HorizontalAlignment.Left };
        var text = new TextBox { Text = qr.Text, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 64 };
        var level = new ComboBox { ItemsSource = Enum.GetValues<QRCodeErrorCorrection>(), SelectedItem = QRCodeErrorCorrection.Medium };
        var quiet = new NumericUpDown { Minimum = 4, Maximum = 32, Increment = 1, Value = 4 };
        var error = new TextBlock { Text = qr.ValidationError ?? "编码有效；默认黑白配色不会跟随主题反色。", TextWrapping = TextWrapping.Wrap };
        text.PropertyChanged += (_, e) => { if (e.Property == TextBox.TextProperty) qr.Text = text.Text; };
        level.SelectionChanged += (_, _) => { if (level.SelectedItem is QRCodeErrorCorrection value) qr.ErrorCorrection = value; };
        quiet.ValueChanged += (_, _) => { if (quiet.Value is { } value) qr.QuietZone = (int)value; };
        qr.PropertyChanged += (_, e) => { if (e.Property == QRCode.ValidationErrorProperty) error.Text = qr.ValidationError ?? "编码有效；默认黑白配色不会跟随主题反色。"; };
        var properties = new Description
        {
            Items =
            {
                new DescriptionItem { Header = "Text", Content = text },
                new DescriptionItem { Header = "ErrorCorrection", Content = level },
                new DescriptionItem { Header = "QuietZone · 4–32 模块", Content = quiet },
                new DescriptionItem { Header = "属性示例", Content = new PreviewCodeBlock
                {
                    Text = "<one:QRCode Text=\"{CompiledBinding QrText}\"\n    ErrorCorrection=\"Medium\" QuietZone=\"4\"\n    Width=\"192\" Height=\"192\" />"
                } }
            }
        };
        Content = new StackPanel
        {
            Spacing = 20, Children =
            {
                PreviewSection.Create("QRCode · 文本、纠错与静区", new StackPanel { Spacing = 16, Children = { qr, error, properties } }),
                PreviewSection.Create("ReportDesign · 静态文本与字段绑定", new Description
                {
                    Items =
                    {
                        new DescriptionItem { Header = "静态二维码", Content = new PreviewCodeBlock
                        {
                            Text = "var qr = designer.AddElement(ReportElementKind.QRCode, 150, 16);\nqr.Text = \"https://example.com\";\nqr.Width = qr.Height = 32;"
                        } },
                        new DescriptionItem { Header = "字段二维码", Content = new PreviewCodeBlock
                        {
                            Text = "designer.SelectedElement = qr;\ndesigner.BindField(\"OrderNo\"); // 使用数据源第一条记录\nqr.QRCodeErrorCorrection = QRCodeErrorCorrection.High;"
                        } },
                        new DescriptionItem { Header = "查看演示", Content = "打开 ReportDesign：页眉同时展示静态链接和 OrderNo 字段二维码，可修改内容、预览、保存和导出 PDF。" }
                    }
                })
            }
        };
    }
}
