using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace TheOne.DesignPreview;

/// <summary>Remembers the last editing target without taking focus from navigation UI.</summary>
internal sealed class PreviewPageFocus(InputElement target) : IDisposable
{
    private readonly WeakReference<InputElement> _target = new(target);
    private (int Start, int End, int Caret)? _selection;
    private TextBox? _textBox;

    internal bool Matches(InputElement element) => _target.TryGetTarget(out var target) && ReferenceEquals(target, element);

    internal void Track()
    {
        if (!_target.TryGetTarget(out var target) || target is not TextBox textBox) return;
        _textBox = textBox;
        Capture();
        textBox.PropertyChanged += SelectionChanged;
    }

    private void SelectionChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == TextBox.SelectionStartProperty || args.Property == TextBox.SelectionEndProperty ||
            args.Property == TextBox.CaretIndexProperty) Capture();
    }

    internal void Capture()
    {
        if (_textBox is { IsFocused: true } textBox)
            _selection = (textBox.SelectionStart, textBox.SelectionEnd, textBox.CaretIndex);
    }

    internal void Resume(Control page)
    {
        if (!_target.TryGetTarget(out var target)) return;
        if (!ReferenceEquals(target, page) && !target.GetVisualAncestors().Contains(page))
        {
            Dispose();
            return;
        }
        if (!target.IsEffectivelyVisible ||
            !target.IsEffectivelyEnabled || !target.Focusable) return;
        var selection = _selection;
        if (!target.Focus(NavigationMethod.Unspecified)) return;
        // Native TextBox focus can collapse the selection. Restore only the saved native selection,
        // synchronously; never schedule a callback that could override a later user action.
        if (_textBox is { } textBox && selection is { } range)
            ApplySelection(textBox, range);
    }

    internal void RestoreSelection(Control page)
    {
        if (_textBox is { } textBox && _selection is { } range && textBox.GetVisualAncestors().Contains(page))
            ApplySelection(textBox, range);
    }

    private static void ApplySelection(TextBox textBox, (int Start, int End, int Caret) range)
    {
        var length = textBox.Text?.Length ?? 0;
        textBox.CaretIndex = Math.Clamp(range.Caret, 0, length);
        textBox.SelectionStart = Math.Clamp(range.Start, 0, length);
        textBox.SelectionEnd = Math.Clamp(range.End, 0, length);
    }

    public void Dispose()
    {
        if (_textBox is { } textBox) textBox.PropertyChanged -= SelectionChanged;
        _textBox = null;
    }
}
