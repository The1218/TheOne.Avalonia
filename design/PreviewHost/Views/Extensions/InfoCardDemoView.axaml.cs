using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace TheOne.DesignPreview;

public sealed class InfoCardTemplateVisibilityConverter : IValueConverter
{
    public static InfoCardTemplateVisibilityConverter Instance { get; } = new();
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value is true ? parameter as IDataTemplate : null;
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => BindingOperations.DoNothing;
}

public partial class InfoCardDemoView : UserControl
{
    private bool _initialized;
    public InfoCardDemoViewModel Model { get; } = new();

    public InfoCardDemoView()
    {
        DataContext = Model;
        global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
        _initialized = true;
        Loaded += (_, _) => UpdateListMetrics();
    }

    private void OnJumpLast(object? sender, RoutedEventArgs args)
    {
        var list = this.FindControl<ListBox>("InfoCardMembers")!;
        Model.SelectedIndex = Model.Members.Count - 1;
        list.ScrollIntoView(list.SelectedIndex);
        Dispatcher.UIThread.Post(() =>
        {
            list.ContainerFromIndex(list.SelectedIndex)?.Focus();
            UpdateListMetrics();
        }, DispatcherPriority.Background);
    }

    private void OnMemberChanged(object? sender, SelectionChangedEventArgs args) => UpdateListMetrics();

    public void UpdateListMetrics()
    {
        if (!_initialized) return;
        var list = this.FindControl<ListBox>("InfoCardMembers");
        var metrics = this.FindControl<TextBlock>("InfoCardListMetrics");
        if (list is null || metrics is null) return;
        metrics.Text = $"已创建 {list.GetRealizedContainers().Count()} / {Model.Members.Count:N0} · {(list.SelectedItem ?? "尚未选择成员")}";
    }
}
