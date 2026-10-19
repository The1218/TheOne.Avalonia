using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TheOne.Avalonia.Controls;

public sealed partial class TabFeatures
{
    private static readonly AttachedProperty<bool> ContentRefreshQueuedProperty =
        AvaloniaProperty.RegisterAttached<TabFeatures, TabControl, bool>("ContentRefreshQueued");

    private static void ReleaseTemplateContent(TabControl tabs, AvaloniaPropertyChangedEventArgs args)
    {
        // Native TabControl registers the new presenters with SelectedContent. Release the
        // old visual child first, without changing the selected item or its content/model.
        if (args.Property == TemplatedControl.TemplateProperty)
        {
            var presenters = tabs.GetTemplateDescendants().OfType<ContentPresenter>()
                .Where(p => p.TemplatedParent == tabs &&
                    p.Name is "PART_SelectedContentHost" or "PART_SelectedContentHost2").ToArray();
            foreach (var presenter in presenters)
            {
                presenter.Content = null;
                presenter.UpdateChild();
            }
        }

        // Own TabItems otherwise remain attached to the retired panel and miss
        // the new container theme and inherited density when the panel is replaced.
        if (tabs.ItemsPanelRoot is null || tabs.GetValue(ContentRefreshQueuedProperty)) return;
        tabs.SetValue(ContentRefreshQueuedProperty, true);
        Dispatcher.UIThread.Post(() =>
        {
            tabs.SetValue(ContentRefreshQueuedProperty, false);
            if (!tabs.IsAttachedToVisualTree()) return;
            tabs.ApplyTemplate();
            tabs.UpdateLayout();
            if (tabs.ItemsPanelRoot is VirtualizingItemsPanel panel) panel.RefreshTabContainers(tabs);
            tabs.UpdateLayout();
        }, DispatcherPriority.Loaded);
    }
}
