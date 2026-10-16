using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using TheOne.Avalonia.Controls;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

internal sealed record ExplorerChoice(int Id, string Name, string Group);
internal sealed class ExplorerSample(Control view, AvaloniaObject target, Action? close = null) : IDisposable
{
    public Control View { get; } = view;
    public AvaloniaObject Target { get; } = target;
    public void Dispose() => close?.Invoke();
}

internal static class ExplorerSamples
{
    internal static IReadOnlyList<ExplorerChoice> Choices { get; } = PreviewPageCatalog.Controls
        .Select(c => new ExplorerChoice(c.Id, c.Title, c.Group)).Concat(new[]
        {
            new ExplorerChoice(-1, "ThemeColorPicker", "主题与窗口"), new(-2, "ThemeButton", "主题与窗口"),
            new(-3, "OneWindow", "主题与窗口"), new(-4, "LoginWindow", "主题与窗口"),
            new(-5, "FormItem", "扩展控件"), new(-6, "FlowNode", "扩展控件"),
            new(-7, "LoadingIndicator", "扩展控件"), new(-8, "NotificationCard", "消息与对话框"),
            new(-9, "TextBlock", "基础控件"), new(-10, "ColorView", "基础控件"),
            new(-11, "RefreshVisualizer", "扩展控件"), new(-12, "ButtonSpinner", "基础控件")
        }).ToArray();

    internal static ExplorerSample Create(ExplorerChoice choice, Control owner)
    {
        if (choice.Name == "FlyoutPresenter")
        {
            var presenter = new FlyoutPresenter { Content = new TextBlock { Text = "浮层内容 · 边框与主题样式", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap } };
            return new ExplorerSample(presenter, presenter);
        }
        Control? direct = choice.Id switch
        {
            -1 => new ThemeColorPicker { Width = 200 }, -2 => new ThemeButton(),
            -5 => new FormItem { Label = "名称", MemberName = "Name", IsRequired = true, HelpText = "字段标签、必填提示与帮助", Content = new TextBox { Text = "示例名称" } },
            -6 => new FlowNode { Content = "提交申请 · 模型和节点内容可分别定制" },
            -7 => new LoadingIndicator(),
            -8 => new global::Avalonia.Controls.Notifications.NotificationCard { Content = "通知内容", NotificationType = global::Avalonia.Controls.Notifications.NotificationType.Information },
            -9 => new TextBlock { Text = "文本展示 · 主题和尺寸继承" }, -10 => new ColorView(),
            -11 => new RefreshVisualizer(), -12 => new ButtonSpinner { Content = new TextBlock { Text = "28" } },
            _ => null
        };
        if (direct is not null) return new ExplorerSample(direct, direct);
        if (choice.Id is -3 or -4)
        {
            Window window = choice.Id == -4 ? new LoginWindow() : new OneWindow();
            window.Title = choice.Name + " 示例"; window.Width = 500; window.Height = 320;
            var button = new Button { Content = "打开 " + choice.Name };
            var message = new Button { Content = "在这个 owner 显示 Toast" };
            message.Click += (_, _) => OneToast.Show("来自控件实验室", owner: window);
            window.Content = new StackPanel { Margin = new Thickness(24), Spacing = 12, Children =
            {
                new TextBlock { Text = "仅演示窗口与消息宿主，不替换应用登录生命周期。", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap }, message
            } };
            button.Click += (_, _) => { if (TopLevel.GetTopLevel(owner) is Window parent) window.Show(parent); };
            window.Closed += (_, _) => button.IsEnabled = false;
            return new ExplorerSample(button, window, () => window.Close());
        }
        var view = PreviewPageCatalog.CreateControl(choice.Id);
        if (choice.Name is "Toast" or "Dialog" or "OneNotification" or "ToolTip" or "ContextMenu" or "GridSplitter") return new ExplorerSample(view, view);
        var name = choice.Name == "DataGridTree" ? "DataGrid" : choice.Name;
        var control = view.GetLogicalDescendants().Prepend(view).OfType<Control>().FirstOrDefault(c => c.GetType().Name == name);
        if (control is null) return new ExplorerSample(view, view);
        var dataContext = control.DataContext ?? view.DataContext;
        switch (control.Parent)
        {
            case Panel panel: panel.Children.Remove(control); break;
            case Decorator decorator: decorator.Child = null; break;
            case ContentControl content when ReferenceEquals(content.Content, control): content.Content = null; break;
            default: if (ReferenceEquals(control, view)) break; return new ExplorerSample(view, view);
        }
        control.DataContext = dataContext;
        control.HorizontalAlignment = HorizontalAlignment.Stretch;
        control.Margin = default;
        if (control is ItemsControl or ChartBase or WorkflowEditor or ReportDesign or ReportPreview or ImagePreview or ColorView)
        { control.Height = 300; control.MaxHeight = 360; }
        return new ExplorerSample(control, control);
    }

    internal static AvaloniaObject Resolve(ExplorerSample sample, PropertyContract property)
    {
        if (property.Target.EndsWith("DataGridColumn", StringComparison.Ordinal) && sample.Target is DataGrid grid && grid.Columns.Count > 0) return grid.Columns[0];
        return sample.Target;
    }
}
