using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ItemSurfaceSpacingPreview
{
    public static void VerifyMenus(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var kind in new[] { "Menu", "ContextMenu", "MenuFlyout", "HorizontalFlyout" }) VerifyMenu(theme, size, width, kind, check, output);
    }
    private static void VerifyMenu(ThemeVariant theme, ControlSize size, int width, string kind, Action<bool, string> check, string? output)
    {
        var command = new CounterCommand();
        var first = new MenuItem { Header = "新建", Command = command };
        var second = new MenuItem { Header = "保存", Command = command };
        var disabled = new MenuItem { Header = "停用", IsEnabled = false, Command = command };
        var nested = new MenuItem { Header = "文档", Command = command };
        var branch = new MenuItem { Header = "更多", Items = { nested } };
        var horizontal = kind == "HorizontalFlyout";
        var entries = horizontal ? new[] { first, second, disabled } : new[] { first, second, disabled, branch };
        var anchor = new Button { Content = "菜单", HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };
        var menu = new Menu { Items = { new MenuItem { Header = "文件" }, new MenuItem { Header = "编辑" } }, VerticalAlignment = VerticalAlignment.Top };
        var window = Open(kind == "Menu" ? menu : anchor, theme, size, width);
        var top = (MenuItem)menu.Items[0]!;
        ContextMenu? context = null; MenuFlyout? flyout = null; Control host;
        var label = $"{kind}/spacing/{theme.Key}/{size}/{width}";
        try
        {
            if (kind == "Menu")
            {
                foreach (var entry in entries) top.Items.Add(entry);
                var tops = menu.GetRealizedContainers().OfType<MenuItem>().Select(item => OwnPart<Border>(item, "PART_LayoutRoot")).ToArray();
                check(Gap(tops[0], tops[1], menu, false) >= 4 && tops.All(row => Inset(row, menu, false, 4)), label + ": top-level menu surfaces have gutters and four-pixel gaps without clipping");
                Click(top); host = OwnPart<Popup>(top, "PART_Popup").Child!;
            }
            else if (kind == "ContextMenu")
            {
                context = new ContextMenu(); foreach (var entry in entries) context.Items.Add(entry);
                anchor.ContextMenu = context; context.Open(anchor); Flush(); host = context;
            }
            else
            {
                flyout = new MenuFlyout(); foreach (var entry in entries) flyout.Items.Add(entry);
                if (horizontal)
                {
                    anchor.TryFindResource("One.HorizontalMenuFlyoutPresenter", theme, out var presenterTheme);
                    anchor.TryFindResource("One.HorizontalMenuItem", theme, out var itemTheme);
                    flyout.FlyoutPresenterTheme = (ControlTheme)presenterTheme!; flyout.ItemContainerTheme = (ControlTheme)itemTheme!;
                }
                flyout.ShowAt(anchor); Flush(); host = ((PopupFlyoutBase)flyout).Popup.Child!;
            }
            Flush();
            var rows = entries.Select(item => OwnPart<Border>(item, "PART_LayoutRoot")).ToArray();
            if (host is MenuFlyoutPresenter presenter)
            {
                presenter.Padding = new Thickness(10); Flush();
                check(rows.All(row => Inset(row, host, !horizontal, 10)), label + ": explicit presenter padding reaches the actual surface");
                presenter.ClearValue(TemplatedControl.PaddingProperty); Flush();
            }
            check(rows.All(row => row.IsEffectivelyVisible && Inset(row, host, !horizontal, 4)), label + ": menu surfaces stay clear of every popup edge");
            check(rows.Zip(rows.Skip(1)).All(pair => Gap(pair.First, pair.Second, host, !horizontal) >= 4), label + ": enabled and disabled items keep four-pixel gaps");
            var root = TopLevel.GetTopLevel(first)!;
            var hit = rows[0].TranslatePoint(new Point(rows[0].Bounds.Width / 2, rows[0].Bounds.Height / 2), root)!.Value;
            root.MouseMove(hit); Flush();
            using (var bitmap = Render(host))
            {
                var point = rows[0].TranslatePoint(default, host)!.Value;
                var y = (int)(point.Y + rows[0].Bounds.Height / 2);
                var gapPoint = horizontal ? new Point(point.X - 2, y) : new Point(point.X + rows[0].Bounds.Width / 2, point.Y - 2);
                check(Pixel(bitmap, (int)gapPoint.X, (int)gapPoint.Y) == Resource(first, "One.Surface")
                    && rows[0].Background is ISolidColorBrush { Color.A: > 0 } brush && Pixel(bitmap, (int)point.X + 2, y) == brush.Color,
                    label + ": actual hovered pixels remain separate from popup padding and neighboring items");
                Save(bitmap, output, label);
            }
            Click(disabled); check(command.Count == 0, label + ": disabled menu entries do not execute commands");
            first.Focus(); first.IsSelected = true; Press(root, horizontal ? Key.Right : Key.Down);
            check(second.IsSelected, label + ": native arrow navigation selects the next item");
            Press(root, Key.Enter);
            check(command.Count == 1 && (kind == "Menu" ? !top.IsSubMenuOpen : context is not null ? !context.IsOpen : !flyout!.IsOpen), label + ": native Enter executes once and closes the menu");
            if (!horizontal)
            {
                if (kind == "Menu") { menu.Open(); top.IsSubMenuOpen = true; }
                else if (context is not null) context.Open(anchor);
                else flyout!.ShowAt(anchor);
                Flush(); branch.IsSubMenuOpen = true; Flush();
                var childHost = OwnPart<Popup>(branch, "PART_Popup").Child!;
                check(Inset(OwnPart<Border>(nested, "PART_LayoutRoot"), childHost, true, 4), label + ": nested popup entries retain edge gutters");
                Click(nested); check(command.Count == 2, label + ": nested pointer activation retains the native command path");
            }
        }
        finally { context?.Close(); flyout?.Hide(); menu.Close(); window.Close(); Flush(); }
    }
    private sealed class CounterCommand : ICommand
    {
        public int Count { get; private set; }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => Count++;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
