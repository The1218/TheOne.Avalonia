using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>A hierarchy item. Label names the item; Y supplies a positive leaf weight.</summary>
public sealed class ChartHierarchyNode : ChartPoint
{
    public static readonly StyledProperty<string?> KeyProperty =
        AvaloniaProperty.Register<ChartHierarchyNode, string?>(nameof(Key));
    public static readonly StyledProperty<IList<ChartHierarchyNode>?> ChildrenProperty =
        AvaloniaProperty.Register<ChartHierarchyNode, IList<ChartHierarchyNode>?>(nameof(Children));
    public static readonly StyledProperty<IBrush?> BrushProperty =
        AvaloniaProperty.Register<ChartHierarchyNode, IBrush?>(nameof(Brush));

    public ChartHierarchyNode() => Children = new ObservableCollection<ChartHierarchyNode>();

    public ChartHierarchyNode(string? label, double value, string? key = null) : this()
    {
        Label = label;
        Y = value;
        Key = key;
    }

    /// <summary>Optional application identity. Unique keys can retain selection across source replacement.</summary>
    public string? Key { get => GetValue(KeyProperty); set => SetValue(KeyProperty, value); }
    /// <summary>Child weights replace the parent's Y value when this collection is nonempty.</summary>
    public IList<ChartHierarchyNode>? Children { get => GetValue(ChildrenProperty); set => SetValue(ChildrenProperty, value); }
    /// <summary>Optional branch color; descendants inherit the nearest explicitly assigned brush.</summary>
    public IBrush? Brush { get => GetValue(BrushProperty); set => SetValue(BrushProperty, value); }
}
