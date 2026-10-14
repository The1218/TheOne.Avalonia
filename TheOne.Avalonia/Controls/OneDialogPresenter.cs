using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Input;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public class OneDialogPresenter : HeaderedContentControl
{
    public static readonly StyledProperty<OneDialogButtons> ButtonsProperty = AvaloniaProperty.Register<OneDialogPresenter, OneDialogButtons>(nameof(Buttons));
    public OneDialogButtons Buttons { get => GetValue(ButtonsProperty); set => SetValue(ButtonsProperty, value); }
    public static readonly StyledProperty<bool> ShowTitleProperty = AvaloniaProperty.Register<OneDialogPresenter, bool>(nameof(ShowTitle), true);
    public bool ShowTitle { get => GetValue(ShowTitleProperty); set => SetValue(ShowTitleProperty, value); }
    public static readonly StyledProperty<bool> ShowCloseButtonProperty = AvaloniaProperty.Register<OneDialogPresenter, bool>(nameof(ShowCloseButton), true);
    public bool ShowCloseButton { get => GetValue(ShowCloseButtonProperty); set => SetValue(ShowCloseButtonProperty, value); }
    public static readonly StyledProperty<bool> ShowButtonsProperty = AvaloniaProperty.Register<OneDialogPresenter, bool>(nameof(ShowButtons), true);
    public bool ShowButtons { get => GetValue(ShowButtonsProperty); set => SetValue(ShowButtonsProperty, value); }
    public static readonly StyledProperty<bool> CloseOnEscapeProperty = AvaloniaProperty.Register<OneDialogPresenter, bool>(nameof(CloseOnEscape), true);
    public bool CloseOnEscape { get => GetValue(CloseOnEscapeProperty); set => SetValue(CloseOnEscapeProperty, value); }
    internal Action<OneDialogResult>? Complete { get; set; }
    private readonly List<(Button Button, EventHandler<global::Avalonia.Interactivity.RoutedEventArgs> Handler)> _handlers = [];
    protected override Type StyleKeyOverride => typeof(OneDialogPresenter);
    public OneDialogPresenter()
    {
        Loaded += (_, _) => FocusPrimary();
        KeyDown += (_, args) => { if (args.Key == Key.Escape && CloseOnEscape) { Complete?.Invoke(OneDialogResult.Cancel); args.Handled = true; } };
        PseudoClasses.Set(":header", true);
        KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ShowTitleProperty || change.Property == ShowCloseButtonProperty) PseudoClasses.Set(":header", ShowTitle || ShowCloseButton);
        if (change.Property == ButtonsProperty)
        {
            PseudoClasses.Set(":yesno", Buttons is OneDialogButtons.YesNo or OneDialogButtons.YesNoCancel);
            PseudoClasses.Set(":cancel", Buttons is OneDialogButtons.OkCancel or OneDialogButtons.YesNoCancel);
        }
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        foreach (var (button, handler) in _handlers) button.Click -= handler;
        _handlers.Clear(); base.OnApplyTemplate(args);
        Wire("PART_PrimaryButton", () => Buttons is OneDialogButtons.YesNo or OneDialogButtons.YesNoCancel ? OneDialogResult.Yes : OneDialogResult.Ok);
        Wire("PART_CloseButton", () => OneDialogResult.Closed);
        Wire("PART_NoButton", () => OneDialogResult.No); Wire("PART_CancelButton", () => OneDialogResult.Cancel);
        void Wire(string name, Func<OneDialogResult> result)
        {
            if (args.NameScope.Find<Button>(name) is not { } button) return;
            EventHandler<global::Avalonia.Interactivity.RoutedEventArgs> handler = (_, _) => Complete?.Invoke(result());
            button.Click += handler; _handlers.Add((button, handler));
        }
    }
    internal void FocusPrimary()
    {
        var target = ShowButtons ? _handlers.FirstOrDefault().Button : this.GetVisualDescendants().OfType<Control>().FirstOrDefault(control => control.Focusable && control.IsEffectivelyVisible && control.IsEffectivelyEnabled && !Equals(control.Name, "PART_CloseButton"));
        if (target is null || !target.Focus(NavigationMethod.Tab)) Focus(NavigationMethod.Tab);
    }
}
