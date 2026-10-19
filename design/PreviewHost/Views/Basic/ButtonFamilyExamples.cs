using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static class ButtonFamilyExamples
{
    public static void Populate(StackPanel host, string type, Func<Button> create, ICommand command)
    {
        host.Children.Add(new TextBlock { Text = type + " · 语义色 × 外观组合", FontWeight = FontWeight.SemiBold });
        var description = new Description { LabelWidth = 110 };
        description.Items.Add(new DescriptionItem { Header = "语义 / 外观", Content = "primary（accent）、danger、success、warning、info × solid、ghost、link、outline、soft、gradient。单独 Classes 保持原有启用效果。" });
        description.Items.Add(new DescriptionItem { Header = "优先级", Content = "颜色：info > warning > success > danger > primary/accent；外观：gradient > soft > outline > link > ghost > solid。与 Classes 书写顺序无关；禁用覆盖语义配色。" });
        description.Items.Add(new DescriptionItem { Header = "组合示例", Content = new TextBlock { Text = "<" + (type == "IconButton" ? "one:" : "") + type + " Classes=\"danger outline\" Content=\"删除\" />\n<" + (type == "IconButton" ? "one:" : "") + type + " Classes=\"success soft\" Content=\"完成\" />", TextWrapping = TextWrapping.Wrap } });
        if (type == "ToggleButton")
            description.Items.Add(new DescriptionItem { Header = "选中 / 三态", Content = "点击切换；组合样式的选中态使用对应语义色，不确定态仍显示短横线。禁用时保留选中底色，三态仍由原生 IsChecked 驱动。" });
        host.Children.Add(description);
        foreach (var color in new[] { "primary", "danger", "success", "warning", "info" })
        {
            host.Children.Add(new TextBlock { Text = color, FontWeight = FontWeight.SemiBold });
            var row = new WrapPanel();
            foreach (var appearance in new[] { "solid", "ghost", "link", "outline", "soft", "gradient" })
            {
                var button = create();
                button.Content = appearance;
                button.Command = command;
                button.Classes.Add(color);
                button.Classes.Add(appearance);
                button.Margin = new Thickness(0, 0, 8, 8);
                ToolTip.SetTip(button, "Classes=\"" + color + " " + appearance + "\"");
                row.Children.Add(button);
            }
            host.Children.Add(row);
        }
        var disabled = new WrapPanel();
        foreach (var color in new[] { "primary", "danger", "success", "warning", "info" })
        {
            var button = create();
            button.Content = color + " outline";
            button.Classes.Add(color);
            button.Classes.Add("outline");
            button.IsEnabled = false;
            button.Margin = new Thickness(0, 0, 8, 8);
            if (button is ToggleButton toggle) toggle.IsChecked = true;
            disabled.Children.Add(button);
        }
        host.Children.Add(new TextBlock { Text = "禁用状态" });
        host.Children.Add(disabled);
    }
}
