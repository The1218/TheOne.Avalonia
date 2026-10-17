using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Search-aware text for NavMenu content templates. SourceText remains bindable when inlines are rebuilt.</summary>
public sealed class NavMenuSearchText : TextBlock
{
    public static readonly StyledProperty<string?> SourceTextProperty =
        AvaloniaProperty.Register<NavMenuSearchText, string?>(nameof(SourceText));

    public static readonly AttachedProperty<string?> HighlightQueryProperty =
        AvaloniaProperty.RegisterAttached<NavMenuSearchText, Control, string?>("HighlightQuery", inherits: true);

    public string? SourceText
    {
        get => GetValue(SourceTextProperty);
        set => SetValue(SourceTextProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(TextBlock);

    public static string? GetHighlightQuery(Control control)
    {
        return control.GetValue(HighlightQueryProperty);
    }

    public static void SetHighlightQuery(Control control, string? value)
    {
        control.SetValue(HighlightQueryProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != SourceTextProperty && change.Property != HighlightQueryProperty) return;
        var text = SourceText ?? "";
        var highlighted = new bool[text.Length];
        foreach (var token in (GetHighlightQuery(this) ?? "").Split((char[]?)null,
                     StringSplitOptions.RemoveEmptyEntries))
            for (var start = 0; start <= text.Length - token.Length;)
            {
                var index = text.IndexOf(token, start, StringComparison.OrdinalIgnoreCase);
                if (index < 0) break;
                Array.Fill(highlighted, true, index, token.Length);
                start = index + 1;
            }

        if (!highlighted.Contains(true))
        {
            Inlines?.Clear();
            SetCurrentValue(TextProperty, text);
            return;
        }

        var inlines = new InlineCollection();
        for (var start = 0; start < text.Length;)
        {
            var end = start + 1;
            while (end < text.Length && highlighted[end] == highlighted[start]) end++;
            var run = new Run(text[start..end]);
            if (highlighted[start])
            {
                run.FontWeight = FontWeight.SemiBold;
                run.TextDecorations = global::Avalonia.Media.TextDecorations.Underline;
            }

            inlines.Add(run);
            start = end;
        }

        Inlines = inlines;
    }
}