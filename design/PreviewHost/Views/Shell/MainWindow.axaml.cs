using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class MainWindow : OneWindow
{
    public MainWindow()
    {
        DataContext = Model;
        InitializeComponent();
        Model.PropertyChanged += ModelChanged;
        SizeChanged += (_, _) => UpdateSidebarLayout();
        if (Application.Current is { } app) app.PropertyChanged += ApplicationThemeChanged;
        Closed += (_, _) =>
        {
            Model.PropertyChanged -= ModelChanged;
            if (Application.Current is { } app) app.PropertyChanged -= ApplicationThemeChanged;
        };
    }

    public GalleryViewModel Model { get; } = new();

    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(GalleryViewModel.EffectiveNavigationCollapsed)) UpdateSidebarLayout();
        if (args.PropertyName == nameof(GalleryViewModel.Theme) && Application.Current is { } app)
            app.RequestedThemeVariant = Model.Theme;
        if (args.PropertyName == nameof(GalleryViewModel.Search)) SearchOverlay.IsVisible = Model.Search.Length > 0;
        if (args.PropertyName == nameof(GalleryViewModel.CurrentPage))
        {
            Model.Search = "";
        }
    }

    private void ApplicationThemeChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == Application.RequestedThemeVariantProperty && Application.Current is { } app)
            Model.Theme = app.RequestedThemeVariant ?? ThemeVariant.Default;
    }

    private void UpdateSidebarLayout()
    {
        Model.CompactViewport = Bounds.Width < 1000;
    }

    public void NavigateToCategory(int category)
    {
        var destination = PreviewPageCatalog.Find(category);
        if (destination.Id == PreviewPageCatalog.Home.Id)
        {
            Navigation.SelectPath(0);
            Model.Select(category);
            Navigation.CloseSubmenu();
            return;
        }

        for (var root = 0; root < Model.NavigationRoots.Count; root++)
        for (var index = 0; index < Model.NavigationRoots[root].Entries.Count; index++)
            if (Model.NavigationRoots[root].Entries[index].Category == destination.Id)
            {
                Navigation.SelectPath(root, index);
                Model.Select(category);
                Navigation.CloseSubmenu();
                return;
            }

        throw new ArgumentOutOfRangeException(nameof(category));
    }
}
