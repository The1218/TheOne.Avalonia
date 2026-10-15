using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace TheOne.DesignPreview;

public partial class ColorPickerPage : UserControl
{
    public ColorPickerPageViewModel Model { get; } = new();
    public ColorPickerPage()
    {
        DataContext = Model; AvaloniaXamlLoader.Load(this);
        SizeChanged += (_, _) =>
        {
            var narrow = Bounds.Width < 700;
            this.FindControl<Grid>("ColorLayout")!.ColumnDefinitions = new(narrow ? "*" : "340,28,*");
            this.FindControl<Border>("InlineFrame")!.Width = Math.Min(340, Bounds.Width);
            var summary = this.FindControl<StackPanel>("ColorSummary")!;
            Grid.SetColumn(summary, narrow ? 0 : 2); Grid.SetRow(summary, narrow ? 1 : 0);
            summary.Margin = narrow ? new Thickness(0,24,0,0) : default;
        };
    }
    private void ResetColor(object? sender, RoutedEventArgs e) => Model.SelectedColor = Color.Parse("#006061");
}
