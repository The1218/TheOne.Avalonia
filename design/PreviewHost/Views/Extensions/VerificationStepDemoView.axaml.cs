using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class VerificationStepDemoView : UserControl
{
    public VerificationStepDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public VerificationStepDemoViewModel Model { get; } = new();
    public VerificationStep Sample => this.FindControl<VerificationStep>("StepSample")!;

    private void ResetClicked(object? sender, RoutedEventArgs args)
    {
        Model.Reset();
    }

    private void ThousandClicked(object? sender, RoutedEventArgs args)
    {
        Model.LoadThousand();
    }

    private async void LastClicked(object? sender, RoutedEventArgs args)
    {
        Model.Status = await Sample.GoToAsync(Model.Steps.Count - 1) ? "已进入末步。" : "不能跳过未验证步骤，请先完成当前步骤。";
    }

}
