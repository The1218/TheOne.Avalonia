using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Data;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.Media;
using global::Avalonia.Threading;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>One-based pagination with bounded page links and native navigation / input controls.</summary>
[PseudoClasses(":compact", ":empty", ":invalid-jump")]
[TemplatePart("PART_PreviousButton", typeof(Button))]
[TemplatePart("PART_NextButton", typeof(Button))]
[TemplatePart("PART_Pages", typeof(ItemsControl))]
[TemplatePart("PART_PageSizes", typeof(ComboBox))]
[TemplatePart("PART_JumpInput", typeof(TextBox))]
[TemplatePart("PART_JumpButton", typeof(Button))]
[TemplatePart("PART_Layout", typeof(PaginationPanel))]
public class Pagination : TemplatedControl
{
    private static readonly IReadOnlyList<int> DefaultSizes = Array.AsReadOnly(new[] { 10, 20, 50, 100 });
    public static readonly StyledProperty<long> TotalCountProperty = AvaloniaProperty.Register<Pagination, long>(nameof(TotalCount), 0, validate: value => value >= 0);
    public static readonly StyledProperty<long> CurrentPageProperty = AvaloniaProperty.Register<Pagination, long>(nameof(CurrentPage), 1, defaultBindingMode: BindingMode.TwoWay, validate: value => value >= 1);
    public static readonly StyledProperty<int> PageSizeProperty = AvaloniaProperty.Register<Pagination, int>(nameof(PageSize), 20, defaultBindingMode: BindingMode.TwoWay, validate: value => value > 0);
    public static readonly StyledProperty<IEnumerable<int>?> PageSizeOptionsProperty = AvaloniaProperty.Register<Pagination, IEnumerable<int>?>(nameof(PageSizeOptions), DefaultSizes);
    public static readonly StyledProperty<PaginationLayout> LayoutProperty = AvaloniaProperty.Register<Pagination, PaginationLayout>(nameof(Layout), validate: Enum.IsDefined);
    public static readonly StyledProperty<double> CompactBreakpointProperty = AvaloniaProperty.Register<Pagination, double>(nameof(CompactBreakpoint), 640, validate: value => double.IsFinite(value) && value >= 0);
    public static readonly StyledProperty<int> SiblingCountProperty = AvaloniaProperty.Register<Pagination, int>(nameof(SiblingCount), 2, validate: value => value is >= 0 and <= 5);
    public static readonly StyledProperty<double> ItemHeightProperty = AvaloniaProperty.Register<Pagination, double>(nameof(ItemHeight), 36, validate: value => double.IsFinite(value) && value > 0);
    public static readonly StyledProperty<bool> ShowTotalProperty = AvaloniaProperty.Register<Pagination, bool>(nameof(ShowTotal), true);
    public static readonly StyledProperty<bool> ShowRangeProperty = AvaloniaProperty.Register<Pagination, bool>(nameof(ShowRange), true);
    public static readonly StyledProperty<bool> ShowPageSizeSelectorProperty = AvaloniaProperty.Register<Pagination, bool>(nameof(ShowPageSizeSelector), true);
    public static readonly StyledProperty<bool> ShowJumpProperty = AvaloniaProperty.Register<Pagination, bool>(nameof(ShowJump), true);
    public static readonly StyledProperty<string> TotalTextFormatProperty = AvaloniaProperty.Register<Pagination, string>(nameof(TotalTextFormat), "共 {0:N0} 条");
    public static readonly StyledProperty<string> RangeTextFormatProperty = AvaloniaProperty.Register<Pagination, string>(nameof(RangeTextFormat), "第 {0:N0}–{1:N0} 条，共 {2:N0} 条");
    public static readonly StyledProperty<string> PageSizeTextFormatProperty = AvaloniaProperty.Register<Pagination, string>(nameof(PageSizeTextFormat), "{0} 条 / 页");
    public static readonly StyledProperty<string> PreviousTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(PreviousText), "上一页");
    public static readonly StyledProperty<string> NextTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(NextText), "下一页");
    public static readonly StyledProperty<string> JumpTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(JumpText), "跳转");
    public static readonly StyledProperty<string> JumpPrefixTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(JumpPrefixText), "跳至");
    public static readonly StyledProperty<string> PageUnitTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(PageUnitText), "页");
    public static readonly StyledProperty<string> EmptyTextProperty = AvaloniaProperty.Register<Pagination, string>(nameof(EmptyText), "暂无记录");
    public static readonly StyledProperty<ICommand?> PageChangedCommandProperty = AvaloniaProperty.Register<Pagination, ICommand?>(nameof(PageChangedCommand));
    public static readonly DirectProperty<Pagination, long> PageCountProperty = AvaloniaProperty.RegisterDirect<Pagination, long>(nameof(PageCount), control => control.PageCount);
    public static readonly DirectProperty<Pagination, long> FirstItemProperty = AvaloniaProperty.RegisterDirect<Pagination, long>(nameof(FirstItem), control => control.FirstItem);
    public static readonly DirectProperty<Pagination, long> LastItemProperty = AvaloniaProperty.RegisterDirect<Pagination, long>(nameof(LastItem), control => control.LastItem);
    public static readonly DirectProperty<Pagination, bool> CanGoPreviousProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(CanGoPrevious), control => control.CanGoPrevious);
    public static readonly DirectProperty<Pagination, bool> CanGoNextProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(CanGoNext), control => control.CanGoNext);
    public static readonly DirectProperty<Pagination, bool> HasPagesProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(HasPages), control => control.HasPages);
    public static readonly DirectProperty<Pagination, bool> IsCompactProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(IsCompact), control => control.IsCompact);
    public static readonly DirectProperty<Pagination, bool> HasSummaryProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(HasSummary), control => control.HasSummary);
    public static readonly DirectProperty<Pagination, bool> HasJumpErrorProperty = AvaloniaProperty.RegisterDirect<Pagination, bool>(nameof(HasJumpError), control => control.HasJumpError);
    public static readonly DirectProperty<Pagination, string> JumpErrorProperty = AvaloniaProperty.RegisterDirect<Pagination, string>(nameof(JumpError), control => control.JumpError);
    public static readonly DirectProperty<Pagination, string> TotalTextProperty = AvaloniaProperty.RegisterDirect<Pagination, string>(nameof(TotalText), control => control.TotalText);
    public static readonly DirectProperty<Pagination, string> RangeTextProperty = AvaloniaProperty.RegisterDirect<Pagination, string>(nameof(RangeText), control => control.RangeText);
    public static readonly DirectProperty<Pagination, string> CurrentPageTextProperty = AvaloniaProperty.RegisterDirect<Pagination, string>(nameof(CurrentPageText), control => control.CurrentPageText);
    public static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageItem>> PageItemsProperty = AvaloniaProperty.RegisterDirect<Pagination, IReadOnlyList<PaginationPageItem>>(nameof(PageItems), control => control.PageItems);
    public static readonly DirectProperty<Pagination, IReadOnlyList<PaginationPageSize>> PageSizesProperty = AvaloniaProperty.RegisterDirect<Pagination, IReadOnlyList<PaginationPageSize>>(nameof(PageSizes), control => control.PageSizes);
    public static readonly RoutedEvent<PaginationPageChangedEventArgs> PageChangedEvent = RoutedEvent.Register<Pagination, PaginationPageChangedEventArgs>(nameof(PageChanged), RoutingStrategies.Bubble);
    private Button? _previous, _next, _jumpButton;
    private ItemsControl? _pages;
    private ComboBox? _sizes;
    private TextBox? _jumpInput;
    private INotifyCollectionChanged? _observedOptions;
    private bool _synchronizing, _hasTemplate;
    private long _reportedPage = 1;
    private int _reportedSize = 20;

    public Pagination() { Synchronize(false); SizeChanged += (_, _) => UpdateLayoutMode(); }
    public long TotalCount { get => GetValue(TotalCountProperty); set => SetValue(TotalCountProperty, value); }
    public long CurrentPage { get => GetValue(CurrentPageProperty); set => SetValue(CurrentPageProperty, value); }
    public int PageSize { get => GetValue(PageSizeProperty); set => SetValue(PageSizeProperty, value); }
    public IEnumerable<int>? PageSizeOptions { get => GetValue(PageSizeOptionsProperty); set => SetValue(PageSizeOptionsProperty, value); }
    public PaginationLayout Layout { get => GetValue(LayoutProperty); set => SetValue(LayoutProperty, value); }
    public double CompactBreakpoint { get => GetValue(CompactBreakpointProperty); set => SetValue(CompactBreakpointProperty, value); }
    public int SiblingCount { get => GetValue(SiblingCountProperty); set => SetValue(SiblingCountProperty, value); }
    public double ItemHeight { get => GetValue(ItemHeightProperty); set => SetValue(ItemHeightProperty, value); }
    public bool ShowTotal { get => GetValue(ShowTotalProperty); set => SetValue(ShowTotalProperty, value); }
    public bool ShowRange { get => GetValue(ShowRangeProperty); set => SetValue(ShowRangeProperty, value); }
    public bool ShowPageSizeSelector { get => GetValue(ShowPageSizeSelectorProperty); set => SetValue(ShowPageSizeSelectorProperty, value); }
    public bool ShowJump { get => GetValue(ShowJumpProperty); set => SetValue(ShowJumpProperty, value); }
    public string TotalTextFormat { get => GetValue(TotalTextFormatProperty); set => SetValue(TotalTextFormatProperty, value); }
    public string RangeTextFormat { get => GetValue(RangeTextFormatProperty); set => SetValue(RangeTextFormatProperty, value); }
    public string PageSizeTextFormat { get => GetValue(PageSizeTextFormatProperty); set => SetValue(PageSizeTextFormatProperty, value); }
    public string PreviousText { get => GetValue(PreviousTextProperty); set => SetValue(PreviousTextProperty, value); }
    public string NextText { get => GetValue(NextTextProperty); set => SetValue(NextTextProperty, value); }
    public string JumpText { get => GetValue(JumpTextProperty); set => SetValue(JumpTextProperty, value); }
    public string JumpPrefixText { get => GetValue(JumpPrefixTextProperty); set => SetValue(JumpPrefixTextProperty, value); }
    public string PageUnitText { get => GetValue(PageUnitTextProperty); set => SetValue(PageUnitTextProperty, value); }
    public string EmptyText { get => GetValue(EmptyTextProperty); set => SetValue(EmptyTextProperty, value); }
    public ICommand? PageChangedCommand { get => GetValue(PageChangedCommandProperty); set => SetValue(PageChangedCommandProperty, value); }
    public long PageCount { get; private set => SetAndRaise(PageCountProperty, ref field, value); }
    public long FirstItem { get; private set => SetAndRaise(FirstItemProperty, ref field, value); }
    public long LastItem { get; private set => SetAndRaise(LastItemProperty, ref field, value); }
    public bool CanGoPrevious { get; private set => SetAndRaise(CanGoPreviousProperty, ref field, value); }
    public bool CanGoNext { get; private set => SetAndRaise(CanGoNextProperty, ref field, value); }
    public bool HasPages { get; private set => SetAndRaise(HasPagesProperty, ref field, value); }
    public bool IsCompact { get; private set => SetAndRaise(IsCompactProperty, ref field, value); }
    public bool HasSummary { get; private set => SetAndRaise(HasSummaryProperty, ref field, value); }
    public bool HasJumpError { get; private set => SetAndRaise(HasJumpErrorProperty, ref field, value); }
    public string JumpError { get; private set => SetAndRaise(JumpErrorProperty, ref field, value); } = "";
    public string TotalText { get; private set => SetAndRaise(TotalTextProperty, ref field, value); } = "";
    public string RangeText { get; private set => SetAndRaise(RangeTextProperty, ref field, value); } = "";
    public string CurrentPageText { get; private set => SetAndRaise(CurrentPageTextProperty, ref field, value); } = "";
    public IReadOnlyList<PaginationPageItem> PageItems { get; private set => SetAndRaise(PageItemsProperty, ref field, value); } = Array.Empty<PaginationPageItem>();
    public IReadOnlyList<PaginationPageSize> PageSizes { get; private set => SetAndRaise(PageSizesProperty, ref field, value); } = Array.Empty<PaginationPageSize>();
    public event EventHandler<PaginationPageChangedEventArgs>? PageChanged { add => AddHandler(PageChangedEvent, value); remove => RemoveHandler(PageChangedEvent, value); }
    protected override Type StyleKeyOverride => typeof(Pagination);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        UnsubscribeParts(); base.OnApplyTemplate(args);
        _previous = args.NameScope.Find<Button>("PART_PreviousButton"); _next = args.NameScope.Find<Button>("PART_NextButton");
        _pages = args.NameScope.Find<ItemsControl>("PART_Pages"); _sizes = args.NameScope.Find<ComboBox>("PART_PageSizes");
        _jumpInput = args.NameScope.Find<TextBox>("PART_JumpInput"); _jumpButton = args.NameScope.Find<Button>("PART_JumpButton");
        if (_previous is not null) _previous.Click += PreviousClicked;
        if (_next is not null) _next.Click += NextClicked;
        if (_pages is not null) _pages.AddHandler(Button.ClickEvent, PageClicked);
        if (_sizes is not null) _sizes.SelectionChanged += SizeSelected;
        if (_jumpButton is not null) _jumpButton.Click += JumpClicked;
        if (_jumpInput is not null) { _jumpInput.KeyDown += JumpKeyDown; _jumpInput.TextChanged += JumpEdited; }
        _hasTemplate = true; Synchronize(false);
    }
    private void UnsubscribeParts()
    {
        if (_previous is not null) _previous.Click -= PreviousClicked;
        if (_next is not null) _next.Click -= NextClicked;
        if (_pages is not null) _pages.RemoveHandler(Button.ClickEvent, PageClicked);
        if (_sizes is not null) _sizes.SelectionChanged -= SizeSelected;
        if (_jumpButton is not null) _jumpButton.Click -= JumpClicked;
        if (_jumpInput is not null) { _jumpInput.KeyDown -= JumpKeyDown; _jumpInput.TextChanged -= JumpEdited; }
    }
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args); ObserveOptions(); Synchronize(false);
    }
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        ObserveOptions(null); base.OnDetachedFromVisualTree(args);
    }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == LayoutProperty || change.Property == CompactBreakpointProperty || change.Property == PaddingProperty || change.Property == BorderThicknessProperty ||
            change.Property == ItemHeightProperty || change.Property == FontSizeProperty || change.Property == FontFamilyProperty || change.Property == FontWeightProperty || change.Property == FontStyleProperty || change.Property == FontStretchProperty) UpdateLayoutMode();
        if (change.Property == PageSizeOptionsProperty) ObserveOptions();
        if (change.Property == TotalCountProperty || change.Property == CurrentPageProperty || change.Property == PageSizeProperty ||
            change.Property == PageSizeOptionsProperty || change.Property == SiblingCountProperty || change.Property == TotalTextFormatProperty ||
            change.Property == RangeTextFormatProperty || change.Property == PageSizeTextFormatProperty || change.Property == EmptyTextProperty ||
            change.Property == ShowTotalProperty || change.Property == ShowPageSizeSelectorProperty)
            Synchronize(change.Property == PageSizeProperty);
    }
    private void ObserveOptions() => ObserveOptions(this.IsAttachedToVisualTree() ? PageSizeOptions as INotifyCollectionChanged : null);
    private void ObserveOptions(INotifyCollectionChanged? options)
    {
        if (_observedOptions is not null) _observedOptions.CollectionChanged -= OptionsChanged;
        _observedOptions = options;
        if (options is not null) options.CollectionChanged += OptionsChanged;
    }
    private void OptionsChanged(object? sender, NotifyCollectionChangedEventArgs args) => Synchronize(false);
    private void UpdateLayoutMode()
    {
        var width = Math.Max(0, Bounds.Width - Padding.Left - Padding.Right - BorderThickness.Left - BorderThickness.Right);
        var compact = Layout == PaginationLayout.Compact;
        if (!compact && Bounds.Width > 0 && CompactBreakpoint > 0)
        {
            var button = _pages?.GetVisualDescendants().OfType<Button>().FirstOrDefault();
            var typeface = new Typeface(button?.FontFamily ?? FontFamily, button?.FontStyle ?? FontStyle, button?.FontWeight ?? FontWeight, button?.FontStretch ?? FontStretch);
            var fontSize = button?.FontSize ?? FontSize;
            var padding = button?.Padding ?? new Thickness(6, 0);
            var needed = ItemHeight * 2 + (PageItems.Count + 1) * 6;
            foreach (var item in PageItems)
            {
                var text = new FormattedText(item.Text, CultureInfo.CurrentCulture, FlowDirection, typeface, fontSize, Foreground ?? Brushes.Black);
                needed += item.IsEllipsis ? text.Width + 12 : Math.Max(ItemHeight, text.Width + padding.Left + padding.Right + 2);
            }
            compact = width < Math.Max(CompactBreakpoint, needed);
        }
        IsCompact = compact; PseudoClasses.Set(":compact", compact);
    }
    private void Synchronize(bool resetPage)
    {
        if (_synchronizing) return;
        _synchronizing = true;
        try
        {
            PageCount = TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
            var page = resetPage ? 1 : PageCount > 0 ? Math.Min(CurrentPage, PageCount) : _hasTemplate ? 1 : CurrentPage;
            SetCurrentValue(CurrentPageProperty, page);
            HasPages = PageCount > 0; CanGoPrevious = HasPages && page > 1; CanGoNext = HasPages && page < PageCount;
            FirstItem = HasPages ? (page - 1) * PageSize + 1 : 0;
            LastItem = HasPages ? FirstItem + Math.Min(TotalCount - FirstItem, PageSize - 1L) : 0;
            TotalText = string.Format(CultureInfo.CurrentCulture, TotalTextFormat, TotalCount);
            RangeText = HasPages ? string.Format(CultureInfo.CurrentCulture, RangeTextFormat, FirstItem, LastItem, TotalCount) : EmptyText;
            CurrentPageText = $"{(HasPages ? page : 0):N0} / {PageCount:N0}";
            var links = BuildPages();
            if (!PageItems.SequenceEqual(links)) PageItems = Array.AsReadOnly(links);
            var sizes = (PageSizeOptions ?? DefaultSizes).ToArray();
            if (sizes.Any(value => value <= 0)) throw new ArgumentException("Page size options must be positive.", nameof(PageSizeOptions));
            var options = sizes.Append(PageSize).Distinct().Select(value => new PaginationPageSize(value, string.Format(CultureInfo.CurrentCulture, PageSizeTextFormat, value))).ToArray();
            if (!PageSizes.SequenceEqual(options)) PageSizes = Array.AsReadOnly(options);
            if (_sizes is not null) _sizes.SetCurrentValue(SelectingItemsControl.SelectedItemProperty, PageSizes.First(option => option.Value == PageSize));
            if (_jumpInput is not null && (_reportedPage != page || _reportedSize != PageSize || _jumpInput.Text is null)) _jumpInput.SetCurrentValue(TextBox.TextProperty, page.ToString(CultureInfo.CurrentCulture));
            HasSummary = ShowTotal || ShowPageSizeSelector;
            PseudoClasses.Set(":empty", !HasPages); ClearJumpError(); UpdateLayoutMode();
        }
        finally { _synchronizing = false; }
        if (_reportedPage == CurrentPage && _reportedSize == PageSize) return;
        var args = new PaginationPageChangedEventArgs(PageChangedEvent, _reportedPage, CurrentPage, _reportedSize, PageSize);
        _reportedPage = CurrentPage; _reportedSize = PageSize;
        RaiseEvent(args);
        if (PageChangedCommand is { } command && command.CanExecute(args)) command.Execute(args);
    }
    private PaginationPageItem[] BuildPages()
    {
        if (PageCount == 0) return [];
        var numbers = new SortedSet<long> { 1, PageCount };
        if (PageCount <= SiblingCount * 2 + 5)
        {
            for (long page = 1; page <= PageCount; page++) numbers.Add(page);
        }
        else
        {
            var start = CurrentPage - Math.Min(CurrentPage - 1, SiblingCount);
            var end = CurrentPage + Math.Min(PageCount - CurrentPage, SiblingCount);
            if (start == 1) end = Math.Min(PageCount, SiblingCount * 2 + 1L);
            if (end == PageCount) start = Math.Max(1, PageCount - SiblingCount * 2L);
            for (var page = start; ; page++) { numbers.Add(page); if (page == end) break; }
        }
        var result = new List<PaginationPageItem>(); long previous = 0;
        foreach (var page in numbers)
        {
            if (previous != 0 && page - previous == 2) result.Add(new(previous + 1, previous + 1 == CurrentPage));
            else if (previous != 0 && page - previous > 2) result.Add(new(0, false));
            result.Add(new(page, page == CurrentPage)); previous = page;
        }
        return result.ToArray();
    }
    public bool GoToPage(long page)
    {
        Dispatcher.UIThread.VerifyAccess();
        if (!IsEffectivelyEnabled || page < 1 || page > PageCount || page == CurrentPage) return false;
        SetCurrentValue(CurrentPageProperty, page); return true;
    }
    public bool GoPrevious() => CanGoPrevious && GoToPage(CurrentPage - 1);
    public bool GoNext() => CanGoNext && GoToPage(CurrentPage + 1);
    public bool GoFirst() => GoToPage(1);
    public bool GoLast() => GoToPage(PageCount);
    private void PreviousClicked(object? sender, RoutedEventArgs args) => GoPrevious();
    private void NextClicked(object? sender, RoutedEventArgs args) => GoNext();
    private void PageClicked(object? sender, RoutedEventArgs args)
    {
        if (args.Source is not Button { DataContext: PaginationPageItem { IsPage: true } item } button) return;
        var focused = button.IsFocused;
        if (!GoToPage(item.Page) || !focused) return;
        Dispatcher.UIThread.Post(() =>
        {
            if (!this.IsAttachedToVisualTree() || _pages is null) return;
            var focus = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
            if (focus is not null && !ReferenceEquals(focus, button)) return;
            _pages.UpdateLayout();
            var index = PageItems.ToList().FindIndex(value => value.Page == CurrentPage);
            _pages.ContainerFromIndex(index)?.GetVisualDescendants().OfType<Button>().FirstOrDefault()?.Focus();
        }, DispatcherPriority.Background);
    }
    private void SizeSelected(object? sender, SelectionChangedEventArgs args)
    {
        if (!_synchronizing && _sizes?.SelectedItem is PaginationPageSize option) SetCurrentValue(PageSizeProperty, option.Value);
    }
    private void JumpClicked(object? sender, RoutedEventArgs args) => CommitJump();
    private void JumpKeyDown(object? sender, KeyEventArgs args)
    {
        if (args.Key == Key.Enter) { CommitJump(); args.Handled = true; }
        else if (args.Key == Key.Escape) { _jumpInput?.SetCurrentValue(TextBox.TextProperty, CurrentPage.ToString(CultureInfo.CurrentCulture)); ClearJumpError(); args.Handled = true; }
    }
    private void JumpEdited(object? sender, TextChangedEventArgs args) => ClearJumpError();
    private void CommitJump()
    {
        if (!IsEffectivelyEnabled || !HasPages) return;
        if (long.TryParse(_jumpInput?.Text?.Trim(), NumberStyles.None, CultureInfo.CurrentCulture, out var page) && page >= 1 && page <= PageCount)
        { GoToPage(page); ClearJumpError(); return; }
        JumpError = $"请输入 1–{PageCount} 之间的页码"; HasJumpError = true; PseudoClasses.Set(":invalid-jump", true);
    }
    private void ClearJumpError() { JumpError = ""; HasJumpError = false; PseudoClasses.Set(":invalid-jump", false); }
}
