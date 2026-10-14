using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Styling;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class LegacyPreviewWindow : OneWindow
{
    public LegacyGalleryViewModel Model { get; } = new();

    public LegacyPreviewWindow()
    {
        DataContext = Model;
        InitializeComponent();
        Model.SelectedNavigation = Model.NavigationRoots[0];
        if (Application.Current is { } app) app.PropertyChanged += ApplicationThemeChanged;
        Closed += (_, _) => { if (Application.Current is { } app) app.PropertyChanged -= ApplicationThemeChanged; };
        Opened += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            Search.Focus();
            Gallery.ResetScroll();
        }, DispatcherPriority.Background);
        SizeChanged += (_, _) => UpdateSidebarLayout();
        Model.PropertyChanged += (_, args) => { if (args.PropertyName == nameof(LegacyGalleryViewModel.NavigationCollapsed)) UpdateSidebarLayout(); };
        Navigation.PropertyChanged += (_, args) => { if (args.Property == NavMenu.CollapsedWidthProperty) UpdateSidebarLayout(); };

    }

    private void OnToggleNavigation(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args) => Model.NavigationCollapsed = !Model.NavigationCollapsed;
    private void UpdateSidebarLayout()
    {
        if (Sidebar is null || Navigation is null) return;
        var compact = Bounds.Width < 1000;
        Sidebar.IsVisible = !compact;
        Sidebar.Padding = Model.NavigationCollapsed ? new Thickness(8, 24) : new Thickness(16, 24);
        Shell.ColumnDefinitions = new ColumnDefinitions(compact ? "0,*" : Model.NavigationCollapsed ? $"{Navigation.CollapsedWidth + 16},*" : "256,*");
        Search.IsVisible = !compact;
    }

    private void OnDensityChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (Gallery != null && sender is ComboBox picker)
            TheOne.Avalonia.Styling.ControlSizing.SetSize(this, picker.SelectedIndex switch
            {
                0 => TheOne.Avalonia.Styling.ControlSize.Small,
                2 => TheOne.Avalonia.Styling.ControlSize.Large,
                _ => TheOne.Avalonia.Styling.ControlSize.Normal
            });
    }

    private void ApplicationThemeChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == Application.RequestedThemeVariantProperty && Application.Current is { } app)
            ThemePicker.SelectedIndex = app.RequestedThemeVariant == ThemeVariant.Light ? 1 : app.RequestedThemeVariant == ThemeVariant.Dark ? 2 : 0;
    }
    private void OnWindowNotification(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args) => OneNotification.Show("OneWindow", "窗口自动管理通知宿主，无需注册。", owner: this);
    private async void OnWindowDialog(object? sender, global::Avalonia.Interactivity.RoutedEventArgs args)
    {
        var result = await OneDialog.ShowAsync("窗口对话框", "确认后在底部显示 Toast。", OneDialogButtons.OkCancel, this);
        if (IsVisible) OneToast.Show($"对话框结果：{result}", owner: this);
    }

    private void OnThemeChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is ComboBox picker && Application.Current is { } app)
            app.RequestedThemeVariant = picker.SelectedIndex switch
            {
                1 => ThemeVariant.Light,
                2 => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
    }

    private void OnNavigationChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (Gallery is not null && sender is NavMenu { SelectedItem: NavEntry { Category: { } category } })
            Gallery.SelectCategory(category);
    }

    public void NavigateToCategory(int category)
    {
        for (var root = 0; root < Model.NavigationRoots.Count; root++)
        {
            var entry = Model.NavigationRoots[root];
            if (entry.Category == category) { Navigation.SelectPath(root); return; }
            for (var child = 0; child < entry.Entries.Count; child++)
                if (entry.Entries[child].Category == category) { Navigation.SelectPath(root, child); return; }
        }
        throw new ArgumentOutOfRangeException(nameof(category));
    }

    private void OnSearchChanged(object? sender, TextChangedEventArgs args)
    {
        if (Gallery is not null && sender is TextBox search)
            Gallery.Search(search.Text ?? "");
    }
}
