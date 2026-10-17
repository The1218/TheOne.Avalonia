using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class FormDemoView : UserControl
{
    public FormDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public FormDemoViewModel Model { get; } = new();
    public Form Sample => this.FindControl<Form>("FormSample")!;

    private async void SaveClicked(object? sender, RoutedEventArgs args)
    {
        await Sample.SubmitAsync();
    }

    private void ClearClicked(object? sender, RoutedEventArgs args)
    {
        Sample.ClearValidation();
    }
}