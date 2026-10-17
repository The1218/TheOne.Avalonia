using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

// Uses the native dispatcher/render loop: no headless input, forced render ticks or explicit layout.
internal sealed class MenuFirstOpenPreview
{
    private readonly Button _anchor;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly IClassicDesktopStyleApplicationLifetime _desktop;
    private readonly MenuItem[] _items;
    private readonly ContextMenu _menu;
    private readonly bool _mixed;
    private readonly string _output;
    private readonly DispatcherTimer _poll = new() { Interval = TimeSpan.FromMilliseconds(25) };
    private readonly HashSet<string> _samples = new(StringComparer.Ordinal);
    private readonly StreamWriter _trace;
    private readonly Window _window;
    private int _failures;
    private Size? _firstPopupSize;
    private int _hover;
    private string? _lastState;
    private ScrollViewer? _observed;
    private int _opening;

    private MenuFirstOpenPreview(IClassicDesktopStyleApplicationLifetime desktop, string output, bool manual,
        ThemeVariant theme, ControlSize size, int width, bool mixed)
    {
        _desktop = desktop;
        _output = output;
        _mixed = mixed;
        Directory.CreateDirectory(output);
        _trace = new StreamWriter(Path.Combine(output, "native-menu-trace.log")) { AutoFlush = true };
        _anchor = new Button { Content = "右键查看菜单", HorizontalAlignment = HorizontalAlignment.Left };
        _items = mixed
            ?
            [
                new MenuItem { Header = "复制", Height = 28 }, new MenuItem { Header = "查看详情", Height = 68 },
                new MenuItem { Header = "打开完整详情以及所有关联操作面板" }
            ]
            : [new MenuItem { Header = "复制" }, new MenuItem { Header = "查看详情" }];
        _menu = new ContextMenu();
        foreach (var item in _items)
        {
            _menu.Items.Add(item);
            item.PointerEntered += (_, _) =>
            {
                var phase = $"open-{_opening}-hover-{++_hover}";
                Write(phase + ": pointer entered " + item.Header);
                Later(80, () => Sample(phase));
            };
            item.PropertyChanged += (_, change) =>
            {
                if (change.Property == Visual.BoundsProperty) TraceState("item-bounds");
            };
        }

        if (mixed) _menu.Items.Insert(2, new Separator());
        _anchor.ContextMenu = _menu;
        _anchor.Click += (_, _) => _menu.Open(_anchor);
        _window = new Window
        {
            Title = "Menu first open · native verification", Width = width, Height = 450,
            RequestedThemeVariant = theme,
            Content = new Border
            {
                Padding = new Thickness(28), Child = new StackPanel
                {
                    Spacing = 18, Children =
                    {
                        new TextBlock { Text = "ContextMenu 首次打开专项", FontSize = 22 },
                        new TextBlock { Text = "右键或点击按钮打开菜单；首次、重新打开及真实鼠标悬停均记录布局。", TextWrapping = TextWrapping.Wrap },
                        _anchor
                    }
                }
            }
        };
        ControlSizing.SetSize(_window, size);
        _menu.Opened += (_, _) =>
        {
            _opening++;
            _lastState = null;
            TraceState($"opened-{_opening}");
            var opening = _opening;
            Later(200, () =>
            {
                if (_menu.IsOpen && opening == _opening) Sample($"open-{opening}-200ms");
            });
            Later(700, () =>
            {
                if (_menu.IsOpen && opening == _opening) Sample($"open-{opening}-700ms");
            });
        };
        _menu.Closed += (_, _) => Write($"closed-{_opening}");
        _menu.TemplateApplied += (_, _) => TraceState("template-applied");
        _poll.Tick += (_, _) =>
        {
            if (_menu.IsOpen) TraceState("layout");
        };
        _window.Opened += (_, _) =>
        {
            Write(
                $"native startup manual={manual} theme={theme.Key} size={size} width={width} mixed={mixed} output={output}");
            _poll.Start();
            Later(200, () => _menu.Open(_anchor));
            if (manual) return;
            Later(1100, _menu.Close);
            Later(1400, () => _menu.Open(_anchor));
            Later(2400, () =>
            {
                Check(new[] { "open-1-200ms", "open-1-700ms", "open-2-200ms", "open-2-700ms" }.All(_samples.Contains),
                    "first opening and reopening completed all four native layout samples");
                Write($"finished failures={_failures}");
                Environment.ExitCode = _failures == 0 ? 0 : 1;
                _desktop.Shutdown(Environment.ExitCode);
            });
        };
        _window.Closed += (_, _) =>
        {
            _poll.Stop();
            _trace.Dispose();
        };
    }

    internal static Window CreateWindow(IClassicDesktopStyleApplicationLifetime desktop, string[] args)
    {
        var output = args.Skip(1).FirstOrDefault(arg => !arg.StartsWith("--", StringComparison.Ordinal)) ??
                     ".preview/menu-first-open/native";
        var theme = args.Contains("--dark") ? ThemeVariant.Dark : ThemeVariant.Light;
        var size = args.Contains("--large") ? ControlSize.Large :
            args.Contains("--small") ? ControlSize.Small : ControlSize.Normal;
        return new MenuFirstOpenPreview(desktop, Path.GetFullPath(output), args.Contains("--manual"),
            theme, size, args.Contains("--narrow") ? 320 : 760, args.Contains("--mixed"))._window;
    }

    private void Sample(string phase)
    {
        if (!_menu.IsOpen)
        {
            Write(phase + ": menu closed before sample");
            return;
        }

        _samples.Add(phase);
        TraceState(phase, true);
        var scroll = Scroller();
        var viewport = scroll?.GetVisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();
        var arrows = scroll?.GetVisualDescendants().OfType<RepeatButton>()
            .Where(button => ReferenceEquals(button.TemplatedParent, scroll)).ToArray() ?? [];
        Check(arrows.Length == 2 && arrows.All(button => !button.IsVisible),
            phase + ": natural short menu has no scrolling buttons");
        Check(viewport is not null && _items.All(item => FullyInside(item, viewport)),
            phase + ": all command items are completely inside the viewport");
        if (_mixed)
            Check(
                Math.Abs(_items[1].Bounds.Height - 68) < .5 &&
                _menu.Items.OfType<Separator>().Single().Bounds.Height > 0,
                phase + ": mixed-height command and separator retain their measured heights");
        if (_firstPopupSize is not { } first)
            _firstPopupSize = _menu.Bounds.Size;
        else
            Check(Math.Abs(first.Height - _menu.Bounds.Height) < .5 && Math.Abs(first.Width - _menu.Bounds.Width) < .5,
                phase + $": popup size matches the first opening ({first} / {_menu.Bounds.Size})");
        if (_menu.Bounds.Width <= 0 || _menu.Bounds.Height <= 0) return;
        using var bitmap =
            new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(_menu.Bounds.Width), (int)Math.Ceiling(_menu.Bounds.Height)),
                new Vector(96, 96));
        bitmap.Render(_menu);
        bitmap.Save(Path.Combine(_output, phase + ".png"), PngBitmapEncoderOptions.Default);
    }

    private ScrollViewer? Scroller()
    {
        return _menu.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
    }

    private void TraceState(string reason, bool always = false)
    {
        var scroll = Scroller();
        if (scroll is not null && !ReferenceEquals(scroll, _observed))
        {
            _observed = scroll;
            scroll.PropertyChanged += (_, change) =>
            {
                if (change.Property == ScrollViewer.ExtentProperty ||
                    change.Property == ScrollViewer.ViewportProperty ||
                    change.Property == ScrollViewer.OffsetProperty || change.Property == Visual.BoundsProperty)
                    TraceState("scroll-" + change.Property.Name);
            };
        }

        var buttons = scroll?.GetVisualDescendants().OfType<RepeatButton>()
            .Where(button => ReferenceEquals(button.TemplatedParent, scroll)).ToArray() ?? [];
        var viewport = scroll?.GetVisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();
        var state =
            $"menu={_menu.Bounds} desired={_menu.DesiredSize}; scroll={scroll?.Bounds} extent={scroll?.Extent} viewport={scroll?.Viewport} offset={scroll?.Offset}; presenter={viewport?.Bounds}; " +
            string.Join("; ",
                buttons.Select(button =>
                    $"{DockPanel.GetDock(button)} visible={button.IsVisible} bounds={button.Bounds}")) + "; " +
            string.Join("; ",
                _items.Select(item =>
                    $"item={item.Header} bounds={item.Bounds} selected={item.IsSelected} pointer={item.IsPointerOver} screen={ScreenCenter(item)}"));
        if (!always && state == _lastState) return;
        _lastState = state;
        Write($"{reason} open={_opening} {state}");
    }

    private static bool FullyInside(Control item, Control viewport)
    {
        var point = item.TranslatePoint(default, viewport);
        return point is { } p && item.Bounds.Height > 0 && p.Y >= -.5 &&
               p.Y + item.Bounds.Height <= viewport.Bounds.Height + .5 &&
               p.X >= -.5 && p.X + item.Bounds.Width <= viewport.Bounds.Width + .5;
    }

    private static string ScreenCenter(Control item)
    {
        return TopLevel.GetTopLevel(item) is null
            ? "unattached"
            : item.PointToScreen(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2)).ToString();
    }

    private void Check(bool condition, string message)
    {
        if (!condition) _failures++;
        Write((condition ? "PASS: " : "FAIL: ") + message);
    }

    private void Write(string message)
    {
        var line = $"{_clock.Elapsed.TotalMilliseconds:F1}ms {message}";
        _trace.WriteLine(line);
        Console.WriteLine(line);
    }

    private static void Later(int milliseconds, Action action)
    {
        DispatcherTimer.RunOnce(action, TimeSpan.FromMilliseconds(milliseconds));
    }
}