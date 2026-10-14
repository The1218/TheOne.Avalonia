using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public class BasicControlPage : UserControl
{
    public BasicControlViewModel Model { get; }
    protected BasicControlPage(BasicControlViewModel model)
    {
        Model = model; DataContext = model;
        model.ScrollRequested += (_, index) =>
        {
            foreach (var control in this.GetVisualDescendants().OfType<ItemsControl>().Where(control => control.Name is "ControlSample" or "DetailedSample"))
            {
                if (control is TreeView tree)
                {
                    if (Virtualization.BringIntoView(tree, index) is { } row)
                        tree.SetCurrentValue(TreeView.SelectedItemProperty, tree.ItemFromContainer(row));
                }
                else control.ScrollIntoView(index);
            }
        };
    }
}
