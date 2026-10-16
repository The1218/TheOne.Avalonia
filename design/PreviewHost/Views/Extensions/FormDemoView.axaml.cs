using Avalonia.Controls;
using Avalonia.Interactivity;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class FormDemoView : UserControl
{
    public FormDemoViewModel Model { get; } = new();
    public Form Sample => this.FindControl<Form>("FormSample")!;
    public FormDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
    private async void SaveClicked(object? sender, RoutedEventArgs args) => await Sample.SubmitAsync();
    private void ClearClicked(object? sender, RoutedEventArgs args) => Sample.ClearValidation();
}
