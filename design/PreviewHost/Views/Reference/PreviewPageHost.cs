using Avalonia;
using Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>Retains visited pages in the tree; only the active frame participates in layout or input.</summary>
public sealed class PreviewPageHost : Panel
{
    private readonly Dictionary<Control, Border> _frames = [];
    private Border? _active;

    internal Control? Content
    {
        get => _active?.Child;
        set
        {
            if (ReferenceEquals(Content, value)) return;
            if (_active is { } previous) previous.IsVisible = false;
            _active = null;
            if (value is not null)
            {
                if (!_frames.TryGetValue(value, out var frame))
                {
                    _frames.Add(value, frame = new Border { Child = value, IsVisible = false });
                    Children.Add(frame);
                }
                _active = frame;
                frame.IsVisible = true;
            }
            InvalidateMeasure();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        _active?.Measure(availableSize);
        return _active?.DesiredSize ?? default;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _active?.Arrange(new Rect(finalSize));
        return finalSize;
    }
}
