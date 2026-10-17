using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class WorkflowEditorDemoView : UserControl
{
    private IDataTemplate? _defaultTemplate;

    public WorkflowEditorDemoView()
    {
        Model = new WorkflowEditorDemoViewModel(true,
            new WorkflowEditorFileService(() => TopLevel.GetTopLevel(this)?.StorageProvider));
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
        AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(Fit);
        Model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Model.Document)) Dispatcher.UIThread.Post(Fit);
        };
        SizeChanged += (_, _) =>
        {
            foreach (var panel in this.GetVisualDescendants().OfType<StackPanel>())
            {
                if (panel.Name == "HeaderBreadcrumb") panel.IsVisible = Bounds.Width >= 1250;
                if (panel.Name == "HeaderSaveState") panel.IsVisible = Bounds.Width >= 1050;
            }
        };
    }

    public WorkflowEditorDemoViewModel Model { get; }
    public WorkflowEditorDemoViewModel AssociationModel { get; } = new();

    private void Fit()
    {
        this.FindControl<WorkflowEditor>("Editor")?.FitToView();
        this.FindControl<WorkflowEditor>("DefaultEditor")?.FitToView();
        this.FindControl<WorkflowEditor>("AssociationEditor")?.FitToView();
    }

    private void ApplyNodeTitle(object? sender, FocusChangedEventArgs e)
    {
        Model.ApplyTitleCommand.Execute(null);
    }

    private void CopyNode(object? sender, RoutedEventArgs e)
    {
        this.FindControl<WorkflowEditor>("Editor")?.CopySelection();
    }

    private void TemplateChanged(object? sender, RoutedEventArgs e)
    {
        var editor = this.FindControl<WorkflowEditor>("Editor");
        if (editor is null) return;
        _defaultTemplate ??= editor.NodeTemplate;
        editor.NodeTemplate = (sender as CheckBox)?.IsChecked == true
            ? (IDataTemplate)Resources["CustomNodeTemplate"]!
            : _defaultTemplate;
    }
}