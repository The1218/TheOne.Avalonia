using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;
using Path = Avalonia.Controls.Shapes.Path;

namespace TheOne.Avalonia.Controls;

/// <summary>Prototype tree text column. Editing uses the original native TextBox and binding.</summary>
public sealed class DataGridTreeTextColumn : DataGridTextColumn
{
    public static readonly StyledProperty<double> IndentProperty =
        AvaloniaProperty.Register<DataGridTreeTextColumn, double>(nameof(Indent), 16,
            validate: value => double.IsFinite(value) && value >= 0);

    public double Indent
    {
        get => GetValue(IndentProperty);
        set => SetValue(IndentProperty, value);
    }

    protected override Control GenerateElement(DataGridCell cell, object dataItem)
    {
        // A tree column needs a shared origin; centering each node moves it with the label width.
        cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        var content = base.GenerateElement(cell, dataItem);
        if (content is TextBlock text) text.TextAlignment = TextAlignment.Left;
        return new DataGridTreeCellPresenter(content, Indent);
    }

    protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
    {
        var editor = base.GenerateEditingElementDirect(cell, dataItem);
        cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        if (editor is TextBox textBox) textBox.TextAlignment = TextAlignment.Left;
        var tree =
            cell.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault()
                ?.CollectionView as IDataGridTreeCollectionView;
        if (tree is not null && tree.TryGetNodeState(dataItem, out var state))
            editor.Margin = new Thickness(state.Depth * Indent + 20, 0, 0, 0);
        return editor;
    }

    protected override void RefreshCellContent(Control element, string propertyName)
    {
        base.RefreshCellContent(element is DataGridTreeCellPresenter presenter ? presenter.NativeContent : element,
            propertyName);
    }
}

/// <summary>Only realized cells subscribe to tree presentation changes; model DataContext is inherited.</summary>
internal sealed class DataGridTreeCellPresenter : Grid
{
    private static readonly Geometry Collapsed = StreamGeometry.Parse("M2,0 L7,5 L2,10 L1,9 L5,5 L1,1 Z");
    private static readonly Geometry Expanded = StreamGeometry.Parse("M0,2 L5,7 L10,2 L9,1 L5,5 L1,1 Z");
    private readonly Button _expander;
    private readonly Path _icon = new() { Width = 10, Height = 10, Stretch = Stretch.Uniform };
    private readonly double _indent;
    private ExpandCollapseState _automationState = ExpandCollapseState.LeafNode;
    private DataGrid? _grid;
    private IDataGridTreeCollectionView? _tree;

    public DataGridTreeCellPresenter(Control content, double indent)
    {
        NativeContent = content;
        _indent = indent;
        ColumnDefinitions = new ColumnDefinitions("20,*");
        _expander = new Button
        {
            Name = "TreeExpander", Content = _icon, Width = 20, MinWidth = 0, MinHeight = 0,
            Height = 20, Padding = default, BorderThickness = default, Focusable = false,
            VerticalAlignment = VerticalAlignment.Center
        };
        _expander.Classes.Add("ghost");
        _icon.Bind(Shape.FillProperty, _expander.GetObservable(Button.ForegroundProperty));
        _expander.AddHandler(PointerPressedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        _expander.AddHandler(PointerReleasedEvent, (_, args) => args.Handled = true, RoutingStrategies.Bubble, true);
        _expander.Click += (_, args) =>
        {
            args.Handled = true;
            if (DataContext is { } item && _tree is not null && _tree.TryGetNodeState(item, out var state))
                _tree.TrySetExpanded(item, !state.IsExpanded);
        };
        SetColumn(content, 1);
        Children.Add(_expander);
        Children.Add(content);
        DataContextChanged += (_, _) => Update();
    }

    public Control NativeContent { get; }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _grid = this.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
        if (_grid is not null) _grid.PropertyChanged += GridChanged;
        Connect();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        if (_grid is not null) _grid.PropertyChanged -= GridChanged;
        if (_tree is not null) _tree.TreeChanged -= Changed;
        _tree = null;
        _grid = null;
        base.OnDetachedFromVisualTree(args);
    }

    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGrid.CollectionViewProperty) Connect();
    }

    private void Connect()
    {
        if (_tree is not null) _tree.TreeChanged -= Changed;
        _tree = _grid?.CollectionView as IDataGridTreeCollectionView;
        if (_tree is not null) _tree.TreeChanged += Changed;
        Update();
    }

    private void Changed(object? sender, EventArgs args)
    {
        Update();
    }

    private void Update()
    {
        var state = default(DataGridTreeNodeState);
        var valid = DataContext is { } item && _tree is not null && _tree.TryGetNodeState(item, out state);
        Margin = new Thickness(valid ? state.Depth * _indent : 0, 0, 0, 0);
        _expander.IsVisible = valid && state.HasChildren;
        _expander.IsEnabled = !state.IsForcedExpanded;
        _icon.Data = state.IsExpanded ? Expanded : Collapsed;
        AutomationProperties.SetName(_expander, state.IsExpanded ? "Collapse" : "Expand");
        var automationState = !valid || !state.HasChildren ? ExpandCollapseState.LeafNode :
            state.IsExpanded ? ExpandCollapseState.Expanded : ExpandCollapseState.Collapsed;
        var previousState = _automationState;
        _automationState = automationState;
        if (previousState != automationState && ControlAutomationPeer.FromElement(this) is TreeCellAutomationPeer peer)
            peer.StateChanged(previousState, automationState);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new TreeCellAutomationPeer(this);
    }

    private sealed class TreeCellAutomationPeer(DataGridTreeCellPresenter owner)
        : ControlAutomationPeer(owner), IExpandCollapseProvider
    {
        public ExpandCollapseState ExpandCollapseState => owner._automationState;
        public bool ShowsMenu => false;

        public void Expand()
        {
            Change(true);
        }

        public void Collapse()
        {
            Change(false);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.TreeItem;
        }

        protected override bool IsControlElementCore()
        {
            return true;
        }

        protected override bool IsContentElementCore()
        {
            return true;
        }

        protected override string? GetNameCore()
        {
            return !string.IsNullOrEmpty(base.GetNameCore())
                ? base.GetNameCore()
                : (owner.NativeContent as TextBlock)?.Text;
        }

        protected override string? GetHelpTextCore()
        {
            return owner.DataContext is { } item && owner._tree is { } tree && tree.TryGetNodeState(item, out var state)
                ? $"Level {state.Depth + 1}"
                : base.GetHelpTextCore();
        }

        private void Change(bool expanded)
        {
            if (!owner.IsEffectivelyEnabled || owner.DataContext is not { } item || owner._tree is not { } tree ||
                !tree.TrySetExpanded(item, expanded))
                throw new InvalidOperationException(
                    "The tree node cannot change expansion while filtering or editing prevents it.");
        }

        public void StateChanged(ExpandCollapseState previous, ExpandCollapseState current)
        {
            RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, previous, current);
        }
    }
}