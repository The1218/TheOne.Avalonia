using Avalonia.Controls;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class ThemeControlDemoView : UserControl
{
    internal ThemeControlDemoView(bool button)
    {
        var sample = button ? (Control)new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = "双态 · Light / Dark" },
                new ThemeButton { Scope = ThemeButtonScope.Application },
                new TextBlock { Text = "三态 · Default / Dark / Light（Default 跟随系统）" },
                new ThemeButton { Scope = ThemeButtonScope.Application, IsThreeState = true }
            }
        } : new ThemeColorPicker { MinWidth = 200 };
        Content = new StackPanel
        {
            Spacing = 16, Children =
            {
                new TextBlock { Text = button ? "ThemeButton · 明暗主题" : "ThemeColorPicker · 配色资源", FontSize = 24 },
                new TextBlock
                {
                    Text = button
                        ? "双态切换 Light / Dark；IsThreeState=True 时按 Default → Dark → Light 循环。此处与标题栏均使用 Application 作用域，Default 自动跟随系统。Window 作用域只修改当前窗口，Default 恢复继承应用主题。"
                        : "选择 ThemeColor 枚举切换配色；SelectedThemeColor 支持双向绑定和配置持久化。",
                    TextWrapping = TextWrapping.Wrap
                },
                new WrapPanel { Children = { sample } },
                new PreviewCodeBlock
                {
                    Text = button
                        ? "<one:ThemeButton Scope=\"Application\" />\n<one:ThemeButton Scope=\"Application\" IsThreeState=\"True\" />\n<!-- OneWindow 内置按钮：ThemeButtonIsThreeState=\"True\" -->"
                        : "<one:ThemeColorPicker\n  SelectedThemeColor=\"{Binding ThemeColor, Mode=TwoWay}\" />\n// 启动时恢复保存的 ThemeColor 枚举。"
                }
            }
        };
    }
}
