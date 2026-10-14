using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Layout;
using TheOne.Avalonia.Styling;
using Avalonia.Interactivity;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class NavMenuDemoView : UserControl
{
    private bool _ready;
    private readonly DispatcherTimer _metricsTimer = new() { Interval = TimeSpan.FromMilliseconds(250) };
    public NavMenuDemoViewModel Model { get; } = new();
    public NavMenuDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        _ready = true;
        Model.Requested += (_, request) =>
        {
            switch (request.Action)
            {
                case NavMenuDemoAction.Open: Menu.OpenSubmenu(request.Path[0]); break;
                case NavMenuDemoAction.Close: Menu.CloseSubmenu(); break;
                case NavMenuDemoAction.Select: Menu.SelectPath(request.Path); break;
                case NavMenuDemoAction.EdgeWindow: ShowEdgeWindow(); break;
            }
            UpdateMetrics();
        };
        _metricsTimer.Tick += (_, _) => UpdateMetrics();
        Loaded += (_, _) => { UpdateMetrics(); _metricsTimer.Start(); };
        DetachedFromVisualTree += (_, _) => _metricsTimer.Stop();
    }
    public NavMenu Menu => this.FindControl<NavMenu>("DemoMenu")!;
    public void UpdateMetrics()
    {
        if (!_ready) return;
        var realized = Menu.GetRealizedTreeContainers().ToArray();
        var submenuCount = 0; var depth = 0;
        for (var child = Menu.Submenu; child is not null; child = child.Submenu) { submenuCount += child.GetRealizedContainers().Count(); depth++; }
        Model.Metrics = $"级联层数 {depth} · 弹层容器 {submenuCount} · 已创建容器 {realized.Length} / 5,000 · 匹配 {Menu.SearchMatchCount} 项 · 根菜单 {Menu.GetRealizedContainers().Count()} / 1,000 · 各级按视口回收 · {(Menu.IsCompactSubmenu || Menu.Submenu?.IsCompactSubmenu == true ? "窄窗口逐级展示" : "边缘自动翻转")}";
    }
    private void OnSelected(object? sender, SelectionChangedEventArgs args) => UpdateMetrics();
    private void ShowEdgeWindow()
    {
        var demo = new NavMenuDemoView(); var menu = demo.Menu;
        menu.DataContext = demo.Model; ((Border)menu.Parent!).Child = null;
        demo.Model.IsCollapsed = true; menu.Height = double.NaN; menu.MaxHeight = 340;
        menu.HorizontalAlignment = Model.EdgeRight ? HorizontalAlignment.Right : HorizontalAlignment.Left;
        var window = new OneWindow { Width = (double)(Model.EdgeWidth ?? 420), Height = 480, MinWidth = 220, MinHeight = 280, Title = "NavMenu · 边缘与窄窗口", RequestedThemeVariant = ActualThemeVariant, Content = new Border { Padding = new global::Avalonia.Thickness(16), Child = menu } };
        ControlSizing.SetSize(window, ControlSizing.GetSize(this)); Motion.SetEnabled(window, Motion.GetEnabled(this));
        window.Show(); menu.OpenSubmenu(0);
    }
}
