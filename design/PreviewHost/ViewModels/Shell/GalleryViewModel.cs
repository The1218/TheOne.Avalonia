using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Styling;
using ReactiveUI;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

public sealed class GalleryViewModel : ReactiveObject
{
    public GalleryViewModel()
    {
        ToggleNavigationCommand = ReactiveCommand.Create<object?>(_ => NavigationCollapsed = !NavigationCollapsed);
        NavigateCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            if (parameter is PreviewPageDescriptor page)
                Select(page.Id);
        });
        SelectedNavigation = NavigationRoots[0];
    }

    public ObservableCollection<NavEntry> NavigationRoots { get; } = PreviewNavigation.CreatePages();
    public ObservableCollection<PreviewPageDescriptor> SearchResults { get; } = [];

    public NavEntry? SelectedNavigation
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(CurrentPage));
            this.RaisePropertyChanged(nameof(NavigationTitle));
        }
    }

    public PreviewPageDescriptor? CurrentPage =>
        SelectedNavigation?.Category is { } id ? PreviewPageCatalog.Find(id) : null;

    public string NavigationTitle => CurrentPage is { } page ? $"{page.Group} / {page.Title}" : "控件目录";

    public bool NavigationCollapsed
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(EffectiveNavigationCollapsed));
        }
    }

    public bool CompactViewport
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(EffectiveNavigationCollapsed));
        }
    }

    public bool EffectiveNavigationCollapsed => NavigationCollapsed || CompactViewport;

    public bool MotionEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public ControlSize Size
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = ControlSize.Normal;

    public ThemeVariant Theme
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = ThemeVariant.Default;

    public IReadOnlyList<ControlSize> Sizes { get; } = Enum.GetValues<ControlSize>();
    public IReadOnlyList<ThemeVariant> Themes { get; } = [ThemeVariant.Default, ThemeVariant.Light, ThemeVariant.Dark];

    public PreviewPageDescriptor? SelectedSearchResult
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            if (value is not null)
                Select(value.Id);
        }
    }

    public string Search
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            SelectedSearchResult = null;
            SearchResults.Clear();
            foreach (var page in PreviewPageCatalog.SearchEntries.Where(page =>
                         page.Title.Contains(value, StringComparison.OrdinalIgnoreCase) ||
                         page.Group.Contains(value, StringComparison.OrdinalIgnoreCase)))
                SearchResults.Add(page);
        }
    } = "";

    public ICommand ToggleNavigationCommand { get; }
    public ICommand NavigateCommand { get; }

    public event EventHandler<int>? ControlRequested;

    public void Select(int id)
    {
        var destination = PreviewPageCatalog.Find(id);
        if (destination.Id == PreviewPageCatalog.Home.Id)
        {
            SelectedNavigation = NavigationRoots[0];
            Search = "";
            return;
        }

        foreach (var group in NavigationRoots)
        foreach (var page in group.Entries)
            if (page.Category == destination.Id)
            {
                SelectedNavigation = page;
                Search = "";
                if (destination.ControlIds?.Contains(id) == true)
                    ControlRequested?.Invoke(this, id);
                return;
            }

        throw new ArgumentOutOfRangeException(nameof(id));
    }
}