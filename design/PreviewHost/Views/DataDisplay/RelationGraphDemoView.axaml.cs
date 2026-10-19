using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TheOne.DesignPreview;

public partial class RelationGraphDemoView : UserControl
{
    public static readonly DirectProperty<RelationGraphDemoView, RelationGraphDemoViewModel?> ViewModelProperty =
        AvaloniaProperty.RegisterDirect<RelationGraphDemoView, RelationGraphDemoViewModel?>(
            nameof(ViewModel), view => view.ViewModel);

    private RelationGraphDemoViewModel? _viewModel;

    public RelationGraphDemoView()
    {
        DataContext = Model;
        UpdateViewModel();
        AvaloniaXamlLoader.Load(this);
        DataContextChanged += (_, _) => UpdateViewModel();
    }

    public RelationGraphDemoViewModel Model { get; } = new();
    public RelationGraphDemoViewModel? ViewModel => _viewModel;

    private void UpdateViewModel() =>
        SetAndRaise(ViewModelProperty, ref _viewModel, DataContext as RelationGraphDemoViewModel);
}
