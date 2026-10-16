using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;

namespace TheOne.DesignPreview;

internal static class PreviewSection
{
    internal static Border Create(string title, Control content)
    {
        var heading = new Border { Padding = new Thickness(16), BorderThickness = new Thickness(0, 0, 0, 1),
            Child = new TextBlock { Text = title, FontWeight = FontWeight.SemiBold, TextWrapping = TextWrapping.Wrap } };
        heading.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        var result = new Border { BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8),
            Child = new StackPanel { Children = { heading, new Border { Padding = new Thickness(16), Child = content } } } };
        result.Bind(Border.BorderBrushProperty, new DynamicResourceExtension("One.Line"));
        result.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        return result;
    }
}
