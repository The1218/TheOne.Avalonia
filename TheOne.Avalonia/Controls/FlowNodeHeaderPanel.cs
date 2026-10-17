using Avalonia;
using Avalonia.Controls;

namespace TheOne.Avalonia.Controls;

/// <summary>Aligns a title and state to opposite edges, moving the state below when it cannot fit.</summary>
public class FlowNodeHeaderPanel : Panel
{
    private bool _stacked;

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count != 2) return base.MeasureOverride(availableSize);
        var title = Children[0];
        var state = Children[1];
        title.Measure(availableSize);
        state.Measure(availableSize);
        _stacked = title.DesiredSize.Width + state.DesiredSize.Width > availableSize.Width;
        return _stacked
            ? new Size(Math.Max(title.DesiredSize.Width, state.DesiredSize.Width),
                title.DesiredSize.Height + state.DesiredSize.Height + 6)
            : new Size(title.DesiredSize.Width + state.DesiredSize.Width,
                Math.Max(title.DesiredSize.Height, state.DesiredSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count != 2) return base.ArrangeOverride(finalSize);
        var title = Children[0];
        var state = Children[1];
        title.Arrange(new Rect(0, 0,
            _stacked ? finalSize.Width : Math.Max(0, finalSize.Width - state.DesiredSize.Width),
            title.DesiredSize.Height));
        state.Arrange(new Rect(_stacked ? 0 : Math.Max(0, finalSize.Width - state.DesiredSize.Width),
            _stacked ? title.DesiredSize.Height + 6 : 0, state.DesiredSize.Width, state.DesiredSize.Height));
        return finalSize;
    }
}