using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class ButtonPage : BasicControlPage
{
    public ButtonPage() : base(new ButtonPageViewModel())
    {
        InitializeComponent();
        foreach (var color in new[] { "primary", "danger", "success", "warning", "info" })
        {
            var row = new WrapPanel();
            foreach (var variant in new[] { "solid", "ghost", "link", "outline", "soft", "gradient" })
            {
                var classes = color + " " + variant;
                var button = new Button { Content = variant, Margin = new Thickness(0, 0, 8, 8) };
                button.Classes.Add(color);
                button.Classes.Add(variant);
                ToolTip.SetTip(button, "Classes=\"" + classes + "\"");
                row.Children.Add(button);
            }
            CombinationsPanel.Children.Add(new TextBlock { Text = color, FontWeight = global::Avalonia.Media.FontWeight.SemiBold });
            CombinationsPanel.Children.Add(row);
        }
        var disabled = new WrapPanel();
        foreach (var color in new[] { "primary", "danger", "success", "warning", "info" })
        {
            var button = new Button { Content = color + " outline", IsEnabled = false, Margin = new Thickness(0, 0, 8, 8) };
            button.Classes.Add(color);
            button.Classes.Add("outline");
            disabled.Children.Add(button);
        }
        CombinationsPanel.Children.Add(new TextBlock { Text = "禁用状态 · 所有组合统一配色" });
        CombinationsPanel.Children.Add(disabled);
    }
}
