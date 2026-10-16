using Avalonia;
using Avalonia.Controls;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class ThemeControlDemoView : UserControl
{
    internal ThemeControlDemoView(bool button)
    {
        var sample = button ? (Control)new ThemeButton() : new ThemeColorPicker { MinWidth = 200 };
        Content = new StackPanel { Spacing = 16, Children =
        {
            new TextBlock { Text = button ? "ThemeButton · 明暗主题" : "ThemeColorPicker · 配色资源", FontSize = 24 },
            new TextBlock { Text = button ? "点击图标在 Light / Dark 间切换。Scope 可选择窗口或应用范围。" : "选择 ThemeColor 枚举切换配色；SelectedThemeColor 支持双向绑定和配置持久化。", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
            new WrapPanel { Children = { sample } },
            new PreviewCodeBlock { Text = button ? "<one:ThemeButton Scope=\"Application\" />" : "<one:ThemeColorPicker\n  SelectedThemeColor=\"{Binding ThemeColor, Mode=TwoWay}\" />\n// 启动时恢复保存的 ThemeColor 枚举。" }
        } };
    }
}
