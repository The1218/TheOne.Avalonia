using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public partial class Breadcrumb
{
    private NavigationKeys? _navigationKeys;
    private WeakReference<NavigationKeys>? _suspendedKeys;
    private BreadcrumbItem? _spaceRow;
    private object? _spaceSource;
    private long _spacePathVersion;
    private long _spaceLifetime;
    private bool _keyUpBlocked;

    private void AttachNavigationKeys()
    {
        if (TopLevel.GetTopLevel(this) is not { } root) return;
        if (_suspendedKeys?.TryGetTarget(out var keys) == true && keys.Root == root && !keys.IsDisposed)
        {
            _navigationKeys = keys;
            keys.Resume();
        }
        else _navigationKeys = new NavigationKeys(this, root);
        _suspendedKeys = null;
    }

    private void DetachNavigationKeys()
    {
        CancelSpaceGesture();
        _keyUpBlocked = false;
        if (_navigationKeys is { } keys)
        {
            keys.Suspend();
            _suspendedKeys = keys.IsDisposed ? null : new WeakReference<NavigationKeys>(keys);
            _navigationKeys = null;
        }
    }

    private BreadcrumbItem? NavigationKeyRow(KeyEventArgs e) => e.Source is Visual source &&
        source.GetSelfAndVisualAncestors().OfType<BreadcrumbItem>().FirstOrDefault() is { } row &&
        ReferenceEquals(row.Parent, this) && row.IsNavigationSource(e.Source) ? row : null;

    private void NavigationKeyDown(KeyEventArgs e, bool repeated)
    {
        _keyUpBlocked = false;
        if (e.Handled || NavigationKeyRow(e) is not { } row) return;
        if (!_isAttached || _isInvoking || repeated || !row.IsNavigationAvailable) { e.Handled = true; return; }
        if (e.Key == Key.Space)
        {
            _spaceRow = row;
            _spaceSource = e.Source;
            _spacePathVersion = _pathVersion;
            _spaceLifetime = _lifetimeVersion;
        }
    }

    private void NavigationKeyUp(KeyEventArgs e)
    {
        _keyUpBlocked = e.Key == Key.Space && NavigationKeyRow(e) is { } row &&
            (!_isAttached || !ReferenceEquals(row, _spaceRow) || !ReferenceEquals(e.Source, _spaceSource) || _spacePathVersion != _pathVersion ||
             _spaceLifetime != _lifetimeVersion || !row.IsNavigationAvailable);
        if (e.Key == Key.Space) CancelSpaceGesture();
        // Let Button finish its native key-up/pressed-state cleanup. InvokeItem suppresses an invalid release.
    }

    private void CancelSpaceGesture() { _spaceRow = null; _spaceSource = null; }

    /// <summary>Observe key release even if focus moves. A suspended watch only weakly references its owner.</summary>
    private sealed class NavigationKeys
    {
        private readonly WeakReference<Breadcrumb> _owner;
        private readonly HashSet<Key> _down = [];
        private bool _suspended;
        internal TopLevel Root { get; }
        internal bool IsDisposed { get; private set; }

        internal NavigationKeys(Breadcrumb owner, TopLevel root)
        {
            _owner = new(owner);
            Root = root;
            root.AddHandler(KeyDownEvent, Down, RoutingStrategies.Tunnel, true);
            root.AddHandler(KeyUpEvent, Up, RoutingStrategies.Tunnel, true);
            root.AddHandler(KeyUpEvent, EndUp, RoutingStrategies.Bubble, true);
            root.AddHandler(PointerPressedEvent, PointerDown, RoutingStrategies.Tunnel, true);
            root.AddHandler(LostFocusEvent, LostNavigationFocus, RoutingStrategies.Bubble, true);
            if (root is WindowBase window) window.Deactivated += Deactivated;
        }

        internal void Resume() => _suspended = false;
        internal void Suspend() { _suspended = true; if (_down.Count == 0) Dispose(); }

        private void Down(object? sender, KeyEventArgs e)
        {
            if (_suspended || e.Key is not (Key.Enter or Key.Space)) return;
            var repeated = !_down.Add(e.Key);
            if (_owner.TryGetTarget(out var owner)) owner.NavigationKeyDown(e, repeated); else Dispose();
        }

        private void Up(object? sender, KeyEventArgs e)
        {
            if (e.Key is not (Key.Enter or Key.Space)) return;
            _down.Remove(e.Key);
            if (_owner.TryGetTarget(out var owner)) { if (!_suspended) owner.NavigationKeyUp(e); }
            else Dispose();
            if (_suspended && _down.Count == 0) Dispose();
        }

        private void EndUp(object? sender, KeyEventArgs e)
        {
            if (!_suspended && _owner.TryGetTarget(out var owner)) owner._keyUpBlocked = false;
        }

        private void PointerDown(object? sender, PointerPressedEventArgs e)
        {
            if (!_suspended && _owner.TryGetTarget(out var owner)) owner._keyUpBlocked = false;
        }

        private void Deactivated(object? sender, EventArgs e)
        {
            _down.Clear();
            if (!_suspended && _owner.TryGetTarget(out var owner)) { owner.CancelSpaceGesture(); owner._keyUpBlocked = false; }
            if (_suspended) Dispose();
        }

        private void LostNavigationFocus(object? sender, FocusChangedEventArgs e)
        {
            if (!_suspended && _owner.TryGetTarget(out var owner) && ReferenceEquals(e.Source, owner._spaceSource)) owner.CancelSpaceGesture();
        }

        private void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            Root.RemoveHandler(KeyDownEvent, Down);
            Root.RemoveHandler(KeyUpEvent, Up);
            Root.RemoveHandler(KeyUpEvent, EndUp);
            Root.RemoveHandler(PointerPressedEvent, PointerDown);
            Root.RemoveHandler(LostFocusEvent, LostNavigationFocus);
            if (Root is WindowBase window) window.Deactivated -= Deactivated;
        }
    }
}
