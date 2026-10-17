using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Navigation shell with retained, closable card pages and a collapsible sidebar.</summary>
[PseudoClasses(":collapsed", ":custom-header")]
public class NavPage : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<NavPage, IEnumerable?>(nameof(ItemsSource));
    public static readonly StyledProperty<NavPageModel?> DefaultHomeProperty = AvaloniaProperty.Register<NavPage, NavPageModel?>(nameof(DefaultHome));
    public static readonly StyledProperty<BindingBase?> ViewModelBindingProperty = AvaloniaProperty.Register<NavPage, BindingBase?>(nameof(ViewModelBinding));
    public static readonly StyledProperty<BindingBase?> TitleBindingProperty = AvaloniaProperty.Register<NavPage, BindingBase?>(nameof(TitleBinding));
    public static readonly StyledProperty<BindingBase?> IconBindingProperty = AvaloniaProperty.Register<NavPage, BindingBase?>(nameof(IconBinding));
    public static readonly StyledProperty<BindingBase?> ChildrenBindingProperty = AvaloniaProperty.Register<NavPage, BindingBase?>(nameof(ChildrenBinding));
    public static readonly StyledProperty<IDataTemplate?> PageTemplateProperty = AvaloniaProperty.Register<NavPage, IDataTemplate?>(nameof(PageTemplate));
    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty = AvaloniaProperty.Register<NavPage, IDataTemplate?>(nameof(HeaderTemplate));
    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty = AvaloniaProperty.Register<NavPage, IDataTemplate?>(nameof(FooterTemplate));
    public static readonly StyledProperty<object?> RightHeaderProperty = AvaloniaProperty.Register<NavPage, object?>(nameof(RightHeader));
    public static readonly StyledProperty<IDataTemplate?> RightHeaderTemplateProperty = AvaloniaProperty.Register<NavPage, IDataTemplate?>(nameof(RightHeaderTemplate));
    public static readonly StyledProperty<string> HeaderTitleProperty = AvaloniaProperty.Register<NavPage, string>(nameof(HeaderTitle), "THE ONE");
    public static readonly StyledProperty<object?> HeaderIconProperty = AvaloniaProperty.Register<NavPage, object?>(nameof(HeaderIcon));
    public static readonly StyledProperty<bool> IsCollapsedProperty = AvaloniaProperty.Register<NavPage, bool>(nameof(IsCollapsed), defaultBindingMode: BindingMode.TwoWay);
    public static readonly StyledProperty<bool> ShowItemIconProperty = AvaloniaProperty.Register<NavPage, bool>(nameof(ShowItemIcon), true);
    public static readonly StyledProperty<double> ExpandedWidthProperty = AvaloniaProperty.Register<NavPage, double>(nameof(ExpandedWidth), 260, validate: v => v > 0 && double.IsFinite(v));
    public static readonly StyledProperty<double> CollapsedWidthProperty = AvaloniaProperty.Register<NavPage, double>(nameof(CollapsedWidth), 64, validate: v => v > 0 && double.IsFinite(v));
    public static readonly StyledProperty<TimeSpan> TransitionDurationProperty = AvaloniaProperty.Register<NavPage, TimeSpan>(nameof(TransitionDuration), TimeSpan.FromMilliseconds(200), validate: v => v >= TimeSpan.Zero);
    public static readonly DirectProperty<NavPage, object?> SelectedPageProperty = AvaloniaProperty.RegisterDirect<NavPage, object?>(nameof(SelectedPage), p => p.SelectedPage);
    private readonly List<Page> _pages = [];
    private NavMenu? _menu;
    private TabControl? _tabs;
    private Button? _toggle;
    private ContentPresenter? _header;
    private ContentPresenter? _footer;
    private object? _selectedPage;
    private bool _syncing;
    private bool _headerScrollQueued;
    private sealed record Page(object Source, TabItem Tab);

    public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }
    public NavPageModel? DefaultHome { get => GetValue(DefaultHomeProperty); set => SetValue(DefaultHomeProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? ViewModelBinding { get => GetValue(ViewModelBindingProperty); set => SetValue(ViewModelBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? TitleBinding { get => GetValue(TitleBindingProperty); set => SetValue(TitleBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? IconBinding { get => GetValue(IconBindingProperty); set => SetValue(IconBindingProperty, value); }
    [AssignBinding, InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? ChildrenBinding { get => GetValue(ChildrenBindingProperty); set => SetValue(ChildrenBindingProperty, value); }
    public IDataTemplate? PageTemplate { get => GetValue(PageTemplateProperty); set => SetValue(PageTemplateProperty, value); }
    public IDataTemplate? HeaderTemplate { get => GetValue(HeaderTemplateProperty); set => SetValue(HeaderTemplateProperty, value); }
    public IDataTemplate? FooterTemplate { get => GetValue(FooterTemplateProperty); set => SetValue(FooterTemplateProperty, value); }
    public object? RightHeader { get => GetValue(RightHeaderProperty); set => SetValue(RightHeaderProperty, value); }
    public IDataTemplate? RightHeaderTemplate { get => GetValue(RightHeaderTemplateProperty); set => SetValue(RightHeaderTemplateProperty, value); }
    public string HeaderTitle { get => GetValue(HeaderTitleProperty); set => SetValue(HeaderTitleProperty, value); }
    public object? HeaderIcon { get => GetValue(HeaderIconProperty); set => SetValue(HeaderIconProperty, value); }
    public bool IsCollapsed { get => GetValue(IsCollapsedProperty); set => SetValue(IsCollapsedProperty, value); }
    public bool ShowItemIcon { get => GetValue(ShowItemIconProperty); set => SetValue(ShowItemIconProperty, value); }
    public double ExpandedWidth { get => GetValue(ExpandedWidthProperty); set => SetValue(ExpandedWidthProperty, value); }
    public double CollapsedWidth { get => GetValue(CollapsedWidthProperty); set => SetValue(CollapsedWidthProperty, value); }
    public TimeSpan TransitionDuration { get => GetValue(TransitionDurationProperty); set => SetValue(TransitionDurationProperty, value); }
    public object? SelectedPage => _selectedPage;

    private static BindingBase DefaultBinding<T>(System.Linq.Expressions.Expression<Func<NavPageModel, T>> getter,
        BindingMode mode = BindingMode.Default) => CompiledBinding.Create(getter, mode: mode);
    private BindingBase Titles => TitleBinding ?? DefaultBinding(p => p.Title);
    private BindingBase Icons => IconBinding ?? DefaultBinding(p => p.Icon);
    private BindingBase Children => ChildrenBinding ?? DefaultBinding(p => p.Children);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var selected = SelectedPage;
        if (_menu is not null)
        {
            _menu.ItemInvoked -= Invoked;
            _menu.PropertyChanged -= MenuGeometryChanged;
        }
        if (_tabs is not null)
        {
            _tabs.SelectionChanged -= Selected;
            _tabs.Items.CollectionChanged -= TabsChanged;
            foreach (var page in _pages) _tabs.Items.Remove(page.Tab);
        }
        if (_toggle is not null) _toggle.Click -= Toggle;
        base.OnApplyTemplate(e);
        _menu = e.NameScope.Find<NavMenu>("PART_Menu");
        if (_menu is not null)
        {
            // Shell configuration must override standalone-menu density triggers.
            _menu.Bind(NavMenu.CollapsedWidthProperty, CompiledBinding.Create<NavPage, double>(p => p.CollapsedWidth, this));
            _menu.Bind(NavMenu.ExpandedWidthProperty, CompiledBinding.Create<NavPage, double>(p => p.ExpandedWidth, this));
            _menu.Bind(NavMenu.TransitionDurationProperty, CompiledBinding.Create<NavPage, TimeSpan>(p => p.TransitionDuration, this));
        }
        _tabs = e.NameScope.Find<TabControl>("PART_Tabs");
        _toggle = e.NameScope.Find<Button>("PART_Toggle");
        _header = e.NameScope.Find<ContentPresenter>("PART_Header");
        _footer = e.NameScope.Find<ContentPresenter>("PART_Footer");
        if (e.NameScope.Find<ContentControl>("PART_HeaderIcon") is { } icon)
        {
            icon.ContentTemplate = TabFeatures.DefaultIconTemplate;
            if (HeaderIcon is null) SetCurrentValue(HeaderIconProperty, global::Avalonia.Media.Geometry.Parse("M2,4 H20 V6 H2 Z M2,10 H20 V12 H2 Z M2,16 H20 V18 H2 Z"));
        }
        if (_menu is not null)
        {
            _menu.ItemInvoked += Invoked;
            _menu.PropertyChanged += MenuGeometryChanged;
        }
        if (_toggle is not null) _toggle.Click += Toggle;
        if (_tabs is not null)
        {
            TabFeatures.SetCanClose(_tabs, true);
            TabFeatures.SetCanReorder(_tabs, true);
            foreach (var page in _pages) _tabs.Items.Add(page.Tab);
            _tabs.SelectionChanged += Selected;
            _tabs.Items.CollectionChanged += TabsChanged;
        }
        ConfigureMenu();
        if (selected is not null) OpenPage(selected);
        else if (DefaultHome is { } home) OpenPage(home);
    }

    private void Toggle(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e) => SetCurrentValue(IsCollapsedProperty, !IsCollapsed);
    private void MenuGeometryChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_menu is { } menu && e.Property == NavMenu.DisplayWidthProperty &&
            Math.Abs(menu.DisplayWidth - (menu.IsCollapsed ? menu.CollapsedWidth : menu.ExpandedWidth)) < .01)
            QueueSelectedHeader();
    }

    private void QueueSelectedHeader()
    {
        if (_headerScrollQueued) return;
        _headerScrollQueued = true;
        global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            _headerScrollQueued = false;
            if (_tabs is not { } tabs || !tabs.IsAttachedToVisualTree()) return;
            tabs.UpdateLayout();
            var strip = tabs.GetVisualDescendants().OfType<CardTabStrip>()
                .FirstOrDefault(s => s.GetVisualAncestors().OfType<TabControl>().FirstOrDefault() == tabs);
            strip?.RevealSelected();
        }, global::Avalonia.Threading.DispatcherPriority.Background);
    }
    private void Invoked(object? sender, NavMenuItemInvokedEventArgs e) => OpenPage(e.Item);
    private void Selected(object? sender, SelectionChangedEventArgs e)
    {
        if (!ReferenceEquals(e.Source, _tabs)) return;
        var source = _pages.FirstOrDefault(p => p.Tab == _tabs?.SelectedItem)?.Source;
        SetAndRaise(SelectedPageProperty, ref _selectedPage, source);
        if (_menu is not null && source is not null) _menu.SetCurrentValue(SelectingItemsControl.SelectedItemProperty, source);
    }
    private void TabsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Reordering removes then reinserts synchronously; prune only after the transaction settles.
        global::Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (_tabs is null) return;
            _pages.RemoveAll(p => !_tabs.Items.Contains(p.Tab));
        });
    }

    private void ConfigureMenu()
    {
        ConfigureHeader();
        if (_menu is null) return;
        _menu.ItemsSource = ItemsSource;
        _menu.ChildrenBinding = Children;
        _menu.ContentTemplate = new FuncDataTemplate<object>((item, _) =>
        {
            var text = new TextBlock { DataContext = item };
            text.Bind(TextBlock.TextProperty, Titles);
            return text;
        });
        _menu.IconTemplate = new FuncDataTemplate<object>((item, _) =>
        {
            var icon = new ContentControl { DataContext = item, ContentTemplate = TabFeatures.DefaultIconTemplate };
            icon.Bind(ContentControl.ContentProperty, Icons);
            return icon;
        });
        if (_tabs is not null) TabFeatures.SetShowItemIcon(_tabs, ShowItemIcon);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty) QueueSelectedHeader();
        if (change.Property == IsCollapsedProperty || change.Property == HeaderTemplateProperty || change.Property == FooterTemplateProperty)
            ConfigureHeader();
        if (change.Property == ItemsSourceProperty || change.Property == TitleBindingProperty ||
            change.Property == IconBindingProperty || change.Property == ChildrenBindingProperty) ConfigureMenu();
        if (change.Property == ShowItemIconProperty && _tabs is not null) TabFeatures.SetShowItemIcon(_tabs, ShowItemIcon);
        if (change.Property == PageTemplateProperty)
            foreach (var page in _pages)
                if (page.Tab.Content is ContentControl body) body.SetCurrentValue(ContentControl.ContentTemplateProperty, PageTemplate);
        if (change.Property == DefaultHomeProperty && DefaultHome is { } home && _tabs is not null) OpenPage(home);
    }

    private void ConfigureHeader()
    {
        PseudoClasses.Set(":collapsed", IsCollapsed);
        PseudoClasses.Set(":custom-header", HeaderTemplate is not null);
        SetTemplate(_header, HeaderTemplate);
        SetTemplate(_footer, FooterTemplate);
    }

    private void SetTemplate(ContentPresenter? presenter, IDataTemplate? template)
    {
        if (presenter is null || (template is null && presenter.Content is null && presenter.ContentTemplate is null) ||
            (presenter.ContentTemplate is ShellTemplate current && ReferenceEquals(current.Template, template))) return;
        // Install a null-safe template before using the shell as its data. Clear the
        // content before removing that template to avoid the native Control-content path.
        presenter.Content = null;
        presenter.ContentTemplate = template is null ? null : new ShellTemplate(this, template);
        if (template is not null) presenter.Content = this;
    }

    private sealed class ShellTemplate(NavPage owner, IDataTemplate template) : IDataTemplate
    {
        public IDataTemplate Template => template;
        public bool Match(object? data) => true;
        public Control? Build(object? data) => template.Build(owner);
    }

    /// <summary>Opens once per model identity and activates the retained page. Null view models are ignored.</summary>
    public bool OpenPage(object source)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (_tabs is null || _syncing) return false;
        var existing = _pages.FirstOrDefault(p => ReferenceEquals(p.Source, source));
        if (existing is not null && !_tabs.Items.Contains(existing.Tab))
        {
            _pages.Remove(existing);
            existing = null;
        }
        if (existing is not null)
        {
            _tabs.SelectedItem = existing.Tab;
            return true;
        }
        var vmBinding = source is NavPageModel ? DefaultBinding(p => p.ViewModel) : ViewModelBinding;
        if (vmBinding is null) return false;
        var body = new ContentControl { ContentTemplate = PageTemplate, DataContext = source };
        var contentBinding = body.Bind(ContentControl.ContentProperty, vmBinding);
        if (body.Content is null)
        {
            contentBinding.Dispose();
            return false;
        }
        var tab = new TabItem { DataContext = source };
        tab.Bind(HeaderedContentControl.HeaderProperty, source is NavPageModel ? DefaultBinding(p => p.Title) : Titles);
        tab.Bind(TabFeatures.IconProperty, source is NavPageModel ? DefaultBinding(p => p.Icon) : Icons);
        tab.Content = body;
        if (source is NavPageModel)
        {
            tab.Bind(TabFeatures.IsClosableProperty, DefaultBinding(p => p.IsClosable));
            tab.Bind(TabFeatures.IsPinnedProperty, DefaultBinding(p => p.IsPinned, BindingMode.TwoWay));
        }
        _pages.Add(new Page(source, tab));
        _syncing = true;
        try
        {
            var pinned = TabFeatures.GetIsPinned(tab);
            var position = pinned ? _tabs.Items.Cast<TabItem>().Count(TabFeatures.GetIsPinned) : _tabs.Items.Count;
            _tabs.Items.Insert(position, tab);
            _tabs.SelectedItem = tab;
        }
        finally { _syncing = false; }
        return true;
    }

    public bool ClosePage(object source) => _pages.FirstOrDefault(p => ReferenceEquals(p.Source, source)) is { } page && TabFeatures.Close(page.Tab);
    public int CloseAllPages() => _tabs is null ? 0 : TabFeatures.CloseAll(_tabs);

}
