using System.Globalization;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data.Converters;
using global::Avalonia.Layout;

namespace TheOne.Avalonia.Controls;

internal static class MenuItemsPanels
{
    // A pointer popup keeps its initial native window size. A virtualizing panel's
    // first estimate (before it has a viewport) can therefore clip even two items.
    // Measure short menus completely, including separators and custom-height rows;
    // retain native virtualization for larger collections.
    private const int FullMeasureLimit = 32;

    public static IMultiValueConverter Vertical { get; } = new Selector(Orientation.Vertical);
    public static IMultiValueConverter Horizontal { get; } = new Selector(Orientation.Horizontal);

    private sealed class Selector(Orientation orientation) : IMultiValueConverter
    {
        private readonly ITemplate<Panel> _complete = new FuncTemplate<Panel>(() => new StackPanel { Orientation = orientation });
        private readonly ITemplate<Panel> _virtualized = new FuncTemplate<Panel>(() => new VirtualizingItemsPanel { Orientation = orientation });

        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count != 2 || values[0] is not ItemsControl owner || values[1] is not int count)
                return global::Avalonia.AvaloniaProperty.UnsetValue;
            if (count > FullMeasureLimit) return _virtualized;
            // ItemCount changes before the old panel receives the collection Reset.
            // Release its containers while its owner is still available: native
            // Detach clears the owner before clearing only the visual children.
            if (ReferenceEquals(owner.ItemsPanel, _virtualized) && owner.ItemsPanelRoot is VirtualizingItemsPanel panel)
                panel.ReleaseForMenuPanelChange(owner);
            return _complete;
        }
    }
}
