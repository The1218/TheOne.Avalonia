using System.Runtime.CompilerServices;
using global::Avalonia;
using global::Avalonia.Collections;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Interactivity;
using global::Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

/// <summary>Coordinates the prototype tree projection with native editing and selection.</summary>
public sealed class DataGridTreeFeatures : AvaloniaObject
{
    public static readonly AttachedProperty<bool> IsEnabledProperty =
        AvaloniaProperty.RegisterAttached<DataGridTreeFeatures, DataGrid, bool>("IsEnabled");
    private static readonly ConditionalWeakTable<DataGrid, State> States = new();
    private static readonly ConditionalWeakTable<IDataGridTreeCollectionView, WeakReference<State>> Owners = new();
    static DataGridTreeFeatures() => IsEnabledProperty.Changed.AddClassHandler<DataGrid>((grid, _) =>
        States.GetValue(grid, owner => new State(owner)).Synchronize());
    public static bool GetIsEnabled(DataGrid grid) => grid.GetValue(IsEnabledProperty);
    public static void SetIsEnabled(DataGrid grid, bool value) => grid.SetValue(IsEnabledProperty, value);

    private sealed class State
    {
        private readonly DataGrid _grid;
        private IDataGridTreeCollectionView? _tree;
        private object[] _selected = [];
        private object? _current;
        private DataGridColumn? _column;
        public State(DataGrid grid)
        {
            _grid = grid;
            grid.AttachedToVisualTree += (_, _) => Synchronize();
            grid.DetachedFromVisualTree += (_, _) => Disconnect();
            grid.PropertyChanged += (_, args) => { if (args.Property == DataGrid.CollectionViewProperty) Synchronize(); };
            grid.AddHandler(InputElement.KeyDownEvent, KeyDown, RoutingStrategies.Tunnel);
        }
        public void Synchronize()
        {
            var tree = GetIsEnabled(_grid) && TopLevel.GetTopLevel(_grid) is not null ? _grid.CollectionView as IDataGridTreeCollectionView : null;
            if (ReferenceEquals(tree, _tree)) return;
            Disconnect();
            if (tree is null) return;
            if (Owners.TryGetValue(tree, out var owner) && owner.TryGetTarget(out _)) throw new InvalidOperationException("Create a separate tree view for each attached DataGrid.");
            Owners.Remove(tree); Owners.Add(tree, new(this));
            _tree = tree;
            tree.TreeChanging += Changing;
            tree.TreeChanged += Changed;
        }
        private void Disconnect()
        {
            if (_tree is not null)
            {
                _tree.TreeChanging -= Changing; _tree.TreeChanged -= Changed;
                Owners.Remove(_tree);
            }
            _tree = null; _selected = []; _current = null; _column = null;
        }
        private void Changing(object? sender, DataGridTreeChangingEventArgs args)
        {
            if (!_grid.CommitEdit(DataGridEditingUnit.Row, true)) { args.Cancel = true; return; }
            _selected = _grid.SelectedItems.Cast<object>().ToArray();
            _current = _grid.CollectionView.CurrentItem;
            _column = _grid.CurrentColumn;
            if (_current is not null && args.Expanded == false && args.Item is { } parent)
            {
                for (var item = _current; _tree!.TryGetNodeState(item, out var state) && state.Parent is { } ancestor; item = ancestor)
                    if (ReferenceEquals(ancestor, parent)) { _current = parent; break; }
            }
        }
        private void Changed(object? sender, EventArgs args)
        {
            var retained = _selected.Where(item => _grid.CollectionView.Contains(item)).ToArray();
            if (_current is not null && _grid.CollectionView.Contains(_current))
            {
                _grid.CollectionView.MoveCurrentTo(_current);
                if (_column is not null && _grid.Columns.Contains(_column)) _grid.CurrentColumn = _column;
            }
            if (_grid.SelectionMode == DataGridSelectionMode.Single)
                _grid.SelectedItem = retained.FirstOrDefault() ??
                    (_selected.Length > 0 && _current is not null && _grid.CollectionView.Contains(_current) ? _current : null);
            else
            {
                foreach (var item in _grid.SelectedItems.Cast<object>().ToArray())
                    if (!retained.Any(value => ReferenceEquals(value, item))) _grid.SelectedItems.Remove(item);
                foreach (var item in retained)
                    if (!_grid.SelectedItems.Contains(item)) _grid.SelectedItems.Add(item);
            }
            _selected = []; _current = null; _column = null;
        }
        private void KeyDown(object? sender, KeyEventArgs args)
        {
            if (_tree is null || args.Handled || args.KeyModifiers != KeyModifiers.None ||
                _grid.CurrentColumn is not DataGridTreeTextColumn || _grid.CollectionView is DataGridCollectionView { IsEditingItem: true } ||
                _grid.SelectedItem is not { } item || !_tree.TryGetNodeState(item, out var state)) return;
            if (args.Key == Key.Left)
            {
                if (state.IsExpanded) args.Handled = _tree.TrySetExpanded(item, false);
                else if (state.Parent is { } parent) { _grid.SelectedItem = parent; _grid.ScrollIntoView(parent, _grid.CurrentColumn); args.Handled = true; }
            }
            else if (args.Key == Key.Right && state.HasChildren)
            {
                if (!state.IsExpanded) args.Handled = _tree.TrySetExpanded(item, true);
                else
                {
                    var next = _grid.CollectionView.Cast<object>().SkipWhile(value => !ReferenceEquals(value, item)).Skip(1).FirstOrDefault();
                    if (next is not null) { _grid.SelectedItem = next; _grid.ScrollIntoView(next, _grid.CurrentColumn); args.Handled = true; }
                }
            }
        }
    }
}
