using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Notifications;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Input;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A native Window with a customizable title bar and automatically managed message hosts.</summary>
public class OneWindow : Window
{
    public static readonly StyledProperty<object?> LeftContentProperty = AvaloniaProperty.Register<OneWindow, object?>(nameof(LeftContent));
    public static readonly StyledProperty<object?> RightContentProperty = AvaloniaProperty.Register<OneWindow, object?>(nameof(RightContent));
    public static readonly StyledProperty<IDataTemplate?> LeftContentTemplateProperty = AvaloniaProperty.Register<OneWindow, IDataTemplate?>(nameof(LeftContentTemplate));
    public static readonly StyledProperty<IDataTemplate?> RightContentTemplateProperty = AvaloniaProperty.Register<OneWindow, IDataTemplate?>(nameof(RightContentTemplate));
    public static readonly StyledProperty<bool> ShowThemeButtonProperty = AvaloniaProperty.Register<OneWindow, bool>(nameof(ShowThemeButton), true);
    public static readonly StyledProperty<ThemeButtonScope> ThemeButtonScopeProperty = AvaloniaProperty.Register<OneWindow, ThemeButtonScope>(nameof(ThemeButtonScope));
    public static readonly StyledProperty<bool> ShowTitleBarProperty = AvaloniaProperty.Register<OneWindow, bool>(nameof(ShowTitleBar), true);
    public static readonly StyledProperty<double> TitleBarHeightProperty = AvaloniaProperty.Register<OneWindow, double>(nameof(TitleBarHeight), 48, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly DirectProperty<OneWindow, bool> IsDialogOpenProperty = AvaloniaProperty.RegisterDirect<OneWindow, bool>(nameof(IsDialogOpen), window => window.IsDialogOpen);
    private bool _isDialogOpen;
    private Panel? _dialogLayer;
    private WindowNotificationManager? _notifications, _toasts;
    private readonly List<(Button Button, EventHandler<global::Avalonia.Interactivity.RoutedEventArgs> Handler)> _captionHandlers = [];
    private Control? _titleBar;
    private Panel? _dialogShield;
    internal DialogCoordinator Dialogs { get; }
    public object? LeftContent { get => GetValue(LeftContentProperty); set => SetValue(LeftContentProperty, value); }
    public object? RightContent { get => GetValue(RightContentProperty); set => SetValue(RightContentProperty, value); }
    public IDataTemplate? LeftContentTemplate { get => GetValue(LeftContentTemplateProperty); set => SetValue(LeftContentTemplateProperty, value); }
    public IDataTemplate? RightContentTemplate { get => GetValue(RightContentTemplateProperty); set => SetValue(RightContentTemplateProperty, value); }
    public bool ShowThemeButton { get => GetValue(ShowThemeButtonProperty); set => SetValue(ShowThemeButtonProperty, value); }
    public ThemeButtonScope ThemeButtonScope { get => GetValue(ThemeButtonScopeProperty); set => SetValue(ThemeButtonScopeProperty, value); }
    public bool ShowTitleBar { get => GetValue(ShowTitleBarProperty); set => SetValue(ShowTitleBarProperty, value); }
    public double TitleBarHeight { get => GetValue(TitleBarHeightProperty); set => SetValue(TitleBarHeightProperty, value); }
    public bool IsDialogOpen => _isDialogOpen;
    public static OneWindow? Current => OneWindowRegistry.Current;
    protected override Type StyleKeyOverride => typeof(OneWindow);
    public OneWindow()
    {
        Dialogs = new DialogCoordinator(this, DialogLayer, SetDialogOpen);
        Opened += (_, _) => OneWindowRegistry.Activate(this);
        Activated += (_, _) => OneWindowRegistry.Activate(this);
        Closed += (_, _) => { OneWindowRegistry.Remove(this); Dialogs.CloseWindow(); };
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        foreach (var (button, handler) in _captionHandlers) button.Click -= handler;
        _captionHandlers.Clear();
        if (_titleBar is not null) _titleBar.PointerPressed -= TitleBarPressed;
        if (_dialogShield is not null) _dialogShield.PointerReleased -= OverlayReleased;
        base.OnApplyTemplate(args);
        _titleBar = args.NameScope.Find<Control>("PART_TitleBar");
        if (_titleBar is not null) _titleBar.PointerPressed += TitleBarPressed;
        BindCaption("PART_MinimizeButton", () => SetCurrentValue(WindowStateProperty, WindowState.Minimized));
        BindCaption("PART_MaximizeButton", ToggleMaximize);
        BindCaption("PART_CloseButton", Close);
        _notifications = args.NameScope.Find<WindowNotificationManager>("PART_Notifications");
        _toasts = args.NameScope.Find<WindowNotificationManager>("PART_Toasts");
        _dialogLayer = args.NameScope.Find<Panel>("PART_DialogLayer");
        _dialogShield = args.NameScope.Find<Panel>("PART_DialogShield");
        if (_dialogShield is not null) _dialogShield.PointerReleased += OverlayReleased;
        Dialogs.TemplateChanged(_dialogLayer);
        void BindCaption(string name, Action action)
        {
            if (args.NameScope.Find<Button>(name) is not { } button) return;
            EventHandler<global::Avalonia.Interactivity.RoutedEventArgs> handler = (_, _) => action();
            button.Click += handler; _captionHandlers.Add((button, handler));
        }
    }
    private void OverlayReleased(object? sender, PointerReleasedEventArgs args)
    {
        if (ReferenceEquals(args.Source, _dialogShield) && args.InitialPressMouseButton == MouseButton.Left) Dialogs.OverlayClicked();
    }
    private void ToggleMaximize()
    {
        if (CanMaximize) SetCurrentValue(WindowStateProperty, WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized);
    }
    private void TitleBarPressed(object? sender, PointerPressedEventArgs args)
    {
        if (!args.GetCurrentPoint(this).Properties.IsLeftButtonPressed || args.Source is not Visual source) return;
        if (source.GetVisualAncestors().Prepend(source).TakeWhile(visual => visual != _titleBar).Any(visual => visual is Button || visual is Control { Name: "PART_LeftContent" or "PART_RightContent" })) return;
        if (args.ClickCount >= 2 && args.ClickCount % 2 == 0) ToggleMaximize(); else BeginMoveDrag(args);
        args.Handled = true;
    }
    internal WindowNotificationManager MessageHost(bool toast)
    {
        ApplyTemplate();
        var host = (toast ? _toasts : _notifications) ?? throw new InvalidOperationException("OneWindow template must contain PART_Notifications and PART_Toasts.");
        host.ApplyTemplate(); return host;
    }
    internal Panel DialogLayer()
    {
        ApplyTemplate(); return _dialogLayer ?? throw new InvalidOperationException("OneWindow template must contain PART_DialogLayer.");
    }
    internal void SetDialogOpen(bool value)
    {
        SetAndRaise(IsDialogOpenProperty, ref _isDialogOpen, value); PseudoClasses.Set(":dialog-open", value);
    }
}
