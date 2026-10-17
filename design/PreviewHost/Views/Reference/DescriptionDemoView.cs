using Avalonia.Controls;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class DescriptionDemoView : UserControl
{
    internal DescriptionDemoView()
    {
        var bordered = Sample();
        bordered.Name = "DescriptionSample";
        var borderless = Sample();
        borderless.Classes.Add("borderless");
        Content = new StackPanel
        {
            Spacing = 24, Children =
            {
                PreviewSection.Create("Description · 默认 / Classes=\"\"", bordered),
                PreviewSection.Create("Description · Classes=\"borderless\"", borderless),
                PreviewSection.Create("Description · 自定义内容与局部列宽", new Description
                {
                    LabelWidth = 220,
                    Items =
                    {
                        new DescriptionItem { Header = "版本", Content = "1.0.0-preview.3" },
                        new DescriptionItem
                            { Header = "发布状态", Content = new Tag { Content = "已完成", Status = TagStatus.Success } },
                        new DescriptionItem { Header = "操作", Content = new Button { Content = "可交互的自定义内容" } }
                    }
                }),
                new PreviewCodeBlock
                {
                    Text =
                        "<one:Description LabelWidth=\"180\" StackWidth=\"520\">\n  <one:DescriptionItem Header=\"名称\" Content=\"示例项目\" />\n  <one:DescriptionItem Header=\"说明\" Content=\"内容换行，行高自然增长\" />\n</one:Description>"
                }
            }
        };
    }

    private static Description Sample()
    {
        return new Description
        {
            Items =
            {
                new DescriptionItem { Header = "Style", Content = "默认有边框；borderless 去掉外框。" },
                new DescriptionItem { Header = "LabelWidth", Content = "本库新增：名称列宽，默认 180 DIP；可继承到行，也可单行覆盖。" },
                new DescriptionItem
                    { Header = "长内容", Content = "控件不固定行高，较长说明会自动换行。窄于 StackWidth 时，标签在上、内容在下，适合属性、API 和模型文档。" },
                new DescriptionItem
                {
                    Header = "StackWidth",
                    Content = new TextBlock
                        { Text = "本库新增：窄于此宽度时上下排布，默认 520 DIP；设置为 0 保持双列。", TextWrapping = TextWrapping.Wrap }
                }
            }
        };
    }
}