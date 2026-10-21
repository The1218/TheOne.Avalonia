using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal sealed class NavPageDemoView : UserControl
{
    public NavPageDemoView()
    {
        var icon = Geometry.Parse("M0,8 L8,0 L16,8 V16 H0 Z");
        var home = new NavPageModel { Title = "Home 首页", Icon = icon, ViewModel = "欢迎使用 NavPage", IsClosable = false };
        var editor = new NavPageModel { Title = "文档编辑", Icon = Geometry.Parse("M2,0 H12 L16,4 V18 H2 Z"), ViewModel = "编辑内容在切换页签后保留。" };
        var fixedPage = new NavPageModel { Title = "固定页面", Icon = icon, ViewModel = "此页默认固定，不能关闭或拖动；右键可以取消固定。", IsPinned = true };
        var group = new NavPageModel { Title = "工作区", Icon = icon };
        group.Children.Add(editor);
        group.Children.Add(fixedPage);
        var shell = new NavPage
        {
            Name = "NavPageSample", HeaderTitle = "THE ONE", HeaderIcon = icon,
            DefaultHome = home, ItemsSource = new[] { home, group }, MinHeight = 440,
            RightHeader = new Border
            {
                Padding = new Thickness(16, 10), BorderThickness = new Thickness(0, 0, 0, 1),
                Child = new TextBlock { Text = "工作区 · 自定义 RightHeader", FontWeight = FontWeight.SemiBold }
            },
            PageTemplate = new FuncDataTemplate<string>((value, _) => new StackPanel
            {
                Spacing = 12, Margin = new Thickness(20), Children =
                {
                    new TextBlock { Text = value, TextWrapping = TextWrapping.Wrap },
                    new TextBox { PlaceholderText = "输入草稿，切换页面后回来查看" }
                }
            })
        };
        var description = new Description
        {
            Items =
            {
                Row("DefaultHome / ItemsSource", "首页自动打开；导航模型按实例复用页签。分组节点只展开。", "new NavPage { DefaultHome = home, ItemsSource = pages }"),
                Row("HeaderTitle / HeaderIcon", "默认品牌栏；点击图标折叠或展开。HeaderTemplate、FooterTemplate 的数据为 NavPage。", "<one:NavPage HeaderTitle=\"THE ONE\" HeaderIcon=\"{StaticResource AppIcon}\" />"),
                Row("ViewModelBinding", "自定义模型通过编译绑定指定 ViewModel，TitleBinding、IconBinding、ChildrenBinding 分别映射标题、图标和子级。", "ViewModelBinding = CompiledBinding.Create<Route, object?>(x => x.Screen)"),
                Row("PageTemplate", "ViewModel 通过此模板或宿主 DataTemplates 呈现；页签切换保留已打开视图。", "<one:NavPage.PageTemplate><DataTemplate x:DataType=\"vm:EditorVm\"><views:EditorView /></DataTemplate></one:NavPage.PageTemplate>"),
                Row("RightHeader / RightHeaderTemplate", "右侧顶部可放工具栏或模板化数据；RightHeader 为 null 时，内容占满右侧，切换不会重建页面。", "<one:NavPage.RightHeader><StackPanel Orientation=\"Horizontal\"><TextBlock Text=\"工作区\" /><Button Content=\"保存\" Command=\"{CompiledBinding Save}\" /></StackPanel></one:NavPage.RightHeader>"),
                Row("ShowItemIcon / IsCollapsed", "控制页签图标、侧栏状态；展开和收缩支持过渡。", "<one:NavPage ShowItemIcon=\"True\" IsCollapsed=\"{Binding Compact, Mode=TwoWay}\" />"),
                Row("ExpandedWidth / CollapsedWidth / TransitionDuration", "侧栏宽度默认 260 / 64 DIP；使用 One.Motion.Expand 时长，零时长或关闭 Motion 立即切换。", "<one:NavPage ExpandedWidth=\"280\" CollapsedWidth=\"64\" TransitionDuration=\"0:0:0.2\" />"),
                Row("OpenPage / ClosePage / CloseAllPages", "打开或激活页面；关闭遵循可取消事件、可关闭和固定状态。", "shell.OpenPage(route);\nshell.ClosePage(route);\nint closed = shell.CloseAllPages();"),
                Row("NavPageModel", "Title、Icon、ViewModel、Children、IsClosable、IsPinned。固定页显示固钉，不能拖动；普通页不能拖过固定区。关闭所有页保留固定及不可关闭页。", "new NavPageModel { Title = \"首页\", ViewModel = homeVm, IsClosable = false }")
            }
        };
        var open = new Button { Content = "打开编辑页" };
        open.Click += (_, _) => shell.OpenPage(editor);
        var pinned = new Button { Content = "打开固定页" };
        pinned.Click += (_, _) => shell.OpenPage(fixedPage);
        var close = new Button { Content = "关闭可关闭页" };
        close.Click += (_, _) => shell.CloseAllPages();
        var toolbar = shell.RightHeader;
        var toggleHeader = new Button { Content = "显示／隐藏 RightHeader" };
        toggleHeader.Click += (_, _) => shell.RightHeader = shell.RightHeader is null ? toolbar : null;
        Content = new StackPanel
        {
            Spacing = 18,
            Children =
            {
                new TextBlock { Text = "NavPage 导航页面", FontSize = 22, FontWeight = FontWeight.SemiBold },
                new WrapPanel { Children = { open, pinned, close, toggleHeader } },
                PreviewSection.Create("导航、保留页面、固定与右键关闭", shell),
                PreviewSection.Create("附加属性、业务 API 与内置模型", description)
            }
        };
    }

    private static DescriptionItem Row(string name, string text, string code) => new()
    {
        Header = name,
        Content = new StackPanel
        {
            Spacing = 6, Children =
            {
                new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap },
                new PreviewCodeBlock { Text = code }
            }
        }
    };
}
