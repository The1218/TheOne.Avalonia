using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Controls.Utils;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>A native AutoCompleteBox used by NavMenu's optional filter header.</summary>
public sealed class NavMenuFilterBox : AutoCompleteBox
{
    private ISelectionAdapter? _commitAdapter;
    private NavMenuFilterResult? _pendingCommit;
    private ListBox? _suggestions;
    public event EventHandler<NavMenuFilterResult>? ResultCommitted;
    protected override Type StyleKeyOverride => typeof(AutoCompleteBox);
    public NavMenuFilterBox()
    {
        MinimumPrefixLength = 1; IsTextCompletionEnabled = false;
        FilterMode = AutoCompleteFilterMode.Custom;
        ItemFilter = (query, item) => item is NavMenuFilterResult result && (query ?? "").Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).All(token => result.SearchPath.Contains(token, StringComparison.OrdinalIgnoreCase));
        // Keyboard highlighting keeps the user's query rather than replacing it with a label.
        ItemSelector = (query, _) => query ?? "";
        AddHandler(KeyDownEvent, (_, args) =>
        {
            if (args.Key != Key.Escape || args.KeyModifiers != KeyModifiers.None) return;
            SetCurrentValue(IsDropDownOpenProperty, false);
            SetCurrentValue(SelectedItemProperty, null);
            SetCurrentValue(TextProperty, ""); args.Handled = true;
        }, RoutingStrategies.Tunnel);
    }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs args)
    {
        if (_commitAdapter is not null) { _commitAdapter.Commit -= CaptureResult; _commitAdapter.Commit -= CommitResult; }
        if (_suggestions is not null) _suggestions.ContainerPrepared -= PrepareSuggestion;
        base.OnApplyTemplate(args);
        _suggestions = args.NameScope.Find<ListBox>("PART_SelectingItemsControl");
        if (_suggestions is not null)
        {
            _suggestions.SetCurrentValue(FocusableProperty, false);
            _suggestions.ContainerPrepared += PrepareSuggestion;
            foreach (var container in _suggestions.GetRealizedContainers()) container.SetCurrentValue(FocusableProperty, false);
        }
        _commitAdapter = SelectionAdapter;
        if (_commitAdapter is not null) _commitAdapter.Commit += CommitResult;
        if (args.NameScope.Find<Popup>("PART_Popup") is { } popup)
        {
            popup.ShouldUseOverlayLayer = true;
            popup.Opened += (_, _) =>
            {
                var available = Math.Max(1, (TopLevel.GetTopLevel(this)?.Bounds.Width ?? 320) - 16);
                popup.MaxWidth = available;
                popup.MinWidth = Math.Min(available, Math.Max(280, Bounds.Width));
            };
        }
    }
    private static void PrepareSuggestion(object? sender, ContainerPreparedEventArgs args) => args.Container.SetCurrentValue(FocusableProperty, false);
    protected override ISelectionAdapter? GetSelectionAdapterPart(INameScope scope)
    {
        var adapter = base.GetSelectionAdapterPart(scope);
        if (adapter is not null) adapter.Commit += CaptureResult;
        return adapter;
    }
    private void CaptureResult(object? sender, RoutedEventArgs args) => _pendingCommit = SelectedItem as NavMenuFilterResult;
    private void CommitResult(object? sender, RoutedEventArgs args)
    {
        var result = _pendingCommit; _pendingCommit = null;
        if (result is not null) ResultCommitted?.Invoke(this, result);
    }
}

/// <summary>A lightweight suggestion containing an original menu model and its ancestor path.</summary>
public sealed class NavMenuFilterResult
{
    internal NavMenuFilterResult(object item, string label, string path, int[] indices) { Item = item; Label = label; Path = path; Indices = indices; }
    public object Item { get; }
    public string Label { get; }
    public string Path { get; }
    public string SearchPath => Path.Length == 0 ? Label : Path + " › " + Label;
    internal int[] Indices { get; }
    public override string ToString() => Label;
}
