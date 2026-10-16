using System.Collections;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Primitives;
using global::Avalonia.Threading;

namespace TheOne.Avalonia.Styling;

// Avalonia 12.1.4 unwraps the flyout's ItemCollection when creating its presenter.
// A later ItemsSource change otherwise leaves that presenter listening to the old list.
internal static class MenuFlyoutItems
{
    private static readonly ConditionalWeakTable<MenuFlyout, State> States = new();

    static MenuFlyoutItems()
    {
        MenuFlyout.ItemsSourceProperty.Changed.AddClassHandler<MenuFlyout>((flyout, _) =>
        {
            var state = States.GetValue(flyout, static current => new State(current));
            if (flyout.IsOpen) state.Queue(flyout);
        });
    }

    internal static void Initialize() { }

    private sealed class State
    {
        private bool _queued;
        private LiveItems? _items;

        public State(MenuFlyout flyout) => flyout.Opened += Opened;

        public void Queue(MenuFlyout flyout)
        {
            if (_queued) return;
            _queued = true;
            Dispatcher.UIThread.Post(() =>
            {
                _queued = false;
                Synchronize(flyout);
            }, DispatcherPriority.Loaded);
        }

        private void Opened(object? sender, EventArgs args)
        {
            if (sender is MenuFlyout flyout) Synchronize(flyout);
        }

        private void Synchronize(MenuFlyout flyout)
        {
            // Popup is lazy: closed source changes must not create it or a presenter.
            if (!flyout.IsOpen || ((PopupFlyoutBase)flyout).Popup.Child is not MenuFlyoutPresenter presenter) return;
            var source = flyout.ItemsSource ?? (_items ??= new LiveItems(flyout.Items));
            if (!ReferenceEquals(presenter.ItemsSource, source))
                presenter.SetCurrentValue(ItemsControl.ItemsSourceProperty, source);
        }
    }

    // Do not pass ItemCollection itself: ItemsSourceView unwraps and snapshots its
    // current underlying source. This view retains identity and forwards live changes.
    private sealed class LiveItems : IList, INotifyCollectionChanged
    {
        private readonly IList _items;

        public LiveItems(ItemCollection items)
        {
            _items = items;
            items.CollectionChanged += (_, args) => CollectionChanged?.Invoke(this, args);
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public int Count => _items.Count;
        public bool IsReadOnly => true;
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public object? this[int index] { get => _items[index]; set => throw new NotSupportedException(); }
        public bool Contains(object? value) => _items.Contains(value);
        public int IndexOf(object? value) => _items.IndexOf(value);
        public void CopyTo(Array array, int index) => _items.CopyTo(array, index);
        public IEnumerator GetEnumerator() => _items.GetEnumerator();
        public int Add(object? value) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public void Insert(int index, object? value) => throw new NotSupportedException();
        public void Remove(object? value) => throw new NotSupportedException();
        public void RemoveAt(int index) => throw new NotSupportedException();
    }
}
