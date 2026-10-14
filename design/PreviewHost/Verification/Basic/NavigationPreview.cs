using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;

namespace TheOne.DesignPreview;

internal static class NavigationPreview
{
    public static void Verify(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(7);
        ControlPreview.Flush();
        var tree = gallery.FindControl<TreeView>("TreeSample")!;
        var model = window.Model;
        var root = Item(tree, model.TreeRoots[0]);
        Check(root.IsExpanded && root.Items.Count == 3, "TreeDataTemplate creates child containers");
        model.SelectedTreeNode = model.TreeRoots[0];
        model.TreeRoots[0].IsExpanded = false;
        ControlPreview.Flush();
        root.Focus(NavigationMethod.Tab);
        PressKey(window, Key.Right, PhysicalKey.ArrowRight);
        Check(root.IsExpanded && model.TreeRoots[0].IsExpanded, "Right arrow expands tree and updates binding");
        PressKey(window, Key.Left, PhysicalKey.ArrowLeft);
        Check(!root.IsExpanded, "Left arrow collapses tree");
        PressKey(window, Key.Right, PhysicalKey.ArrowRight);
        PressKey(window, Key.Down, PhysicalKey.ArrowDown);
        Check(ReferenceEquals(model.SelectedTreeNode, model.TreeRoots[0].Children[0]), "Arrow navigation updates tree selection");
        var selected = Item(tree, model.SelectedTreeNode!);
        var selectedBorder = LayoutRoot(selected);
        Check(ColorOf(selectedBorder.Background) == Color.Parse("#C1DCDC"), "Tree selection uses TheOne highlight");
        Check(!Item(tree, model.TreeRoots[0].Children[2]).IsEffectivelyEnabled, "Disabled tree node remains disabled");

        model.CollapseTreeCommand.Execute(null);
        ControlPreview.Flush();
        Check(!root.IsExpanded, "Collapse-all menu command updates real tree containers");
        model.ExpandTreeCommand.Execute(null);
        ControlPreview.Flush();
        Check(root.IsExpanded, "Expand-all command restores hierarchy");

        var file = gallery.FindControl<MenuItem>("FileMenu")!;
        var menu = gallery.FindControl<Menu>("MenuSample")!;
        menu.Open();
        file.IsSubMenuOpen = true;
        ControlPreview.Flush();
        var popup = file.GetVisualDescendants().OfType<Popup>().Single();
        Check(popup.IsOpen && popup.Child!.Bounds.Height > 0, "Top-level menu popup renders");
        Check(ColorOf(((Border)popup.Child!).Background) == Color.Parse("#FFFFFF"), "Menu popup uses One.Surface");
        var newItem = gallery.FindControl<MenuItem>("NewMenuItem")!;
        newItem.Focus(NavigationMethod.Tab);
        PressKey(TopLevel.GetTopLevel(newItem)!, Key.Down, PhysicalKey.ArrowDown);
        Check(((MenuItem)file.Items[1]!).IsSelected, "Down arrow navigates menu items");
        PressKey(TopLevel.GetTopLevel(newItem)!, Key.Enter, PhysicalKey.Enter);
        Check(model.Status.Contains("已保存第") && !file.IsSubMenuOpen, "Enter executes the focused menu command and closes popup");
        menu.Open();
        file.IsSubMenuOpen = true;
        ControlPreview.Flush();
        var recent = gallery.FindControl<MenuItem>("RecentMenuItem")!;
        recent.IsSubMenuOpen = true;
        ControlPreview.Flush();
        Check(recent.GetVisualDescendants().OfType<Popup>().Single().IsOpen, "Nested menu popup opens");
        ClickItem(gallery.FindControl<MenuItem>("RecentLeafItem")!);
        ControlPreview.Flush();
        Check(model.Status == "菜单命令：打开 TheOne.Avalonia", "Nested menu command reaches view model");
        Check(!file.IsSubMenuOpen, "Leaf command closes the menu chain");

        var view = gallery.FindControl<MenuItem>("ViewMenu")!;
        menu.Open();
        view.IsSubMenuOpen = true;
        ControlPreview.Flush();
        ClickItem(gallery.FindControl<MenuItem>("DetailsMenuItem")!);
        ControlPreview.Flush();
        Check(!model.ShowTreeDetails, "Checked menu item updates two-way view model binding");
        model.ShowTreeDetails = true;
        menu.Close();

        var targetNode = model.TreeRoots[0].Children[0].Children[1];
        var target = Item(tree, targetNode);
        target.BringIntoView();
        ControlPreview.Flush();
        RightClick(window, LayoutRoot(target));
        var treeContext = gallery.FindControl<ContextMenu>("TreeContextMenu")!;
        Check(treeContext.IsOpen && ReferenceEquals(model.SelectedTreeNode, targetNode), "Tree right-click selects the target node before opening context menu");
        var oldName = targetNode.Name;
        ClickItem(gallery.FindControl<MenuItem>("TreeRenameItem")!);
        ControlPreview.Flush();
        Check(targetNode.Name != oldName && model.Status.Contains(targetNode.Name), "Tree context command renames only the selected demo node");
        Check(!treeContext.IsOpen, "Tree context menu closes after command");

        var contextTarget = gallery.FindControl<Border>("ContextTarget")!;
        contextTarget.BringIntoView();
        ControlPreview.Flush();
        RightClick(window, contextTarget);
        var context = gallery.FindControl<ContextMenu>("ContextSample")!;
        Check(context.IsOpen, "Native right-click opens ContextMenu");
        Check(ReferenceEquals(context.DataContext, model), "Detached ContextMenu retains view model context");
        var nested = gallery.FindControl<MenuItem>("ContextNestedItem")!;
        nested.IsSubMenuOpen = true;
        ControlPreview.Flush();
        Check(nested.GetVisualDescendants().OfType<Popup>().Single().IsOpen, "Context menu nested popup opens");
        ClickItem(gallery.FindControl<MenuItem>("ContextCollapseItem")!);
        ControlPreview.Flush();
        Check(!model.TreeRoots[0].IsExpanded && !context.IsOpen, "Context submenu executes command and closes");
        model.ExpandTreeCommand.Execute(null);

        context.Open(contextTarget);
        ControlPreview.Flush();
        var disabled = gallery.FindControl<MenuItem>("ContextDisabledItem")!;
        var before = model.Status;
        MouseClick(TopLevel.GetTopLevel(disabled)!, disabled);
        Check(model.Status == before && !disabled.IsEffectivelyEnabled, "Disabled context item does not execute via mouse");
        context.Close();

        menu.Open();
        file.IsSubMenuOpen = true;
        ControlPreview.Flush();
        selectedBorder = LayoutRoot(Item(tree, model.SelectedTreeNode!));
        foreach (var theme in new[] { 2, 1 })
        {
            window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = theme;
            ControlPreview.Flush();
            Check(ColorOf(((Border)popup.Child!).Background) == Color.Parse(theme == 2 ? "#0F0F0F" : "#FFFFFF"),
                "Open menu popup follows live theme switch");
            Check(ColorOf(selectedBorder.Background) == Color.Parse(theme == 2 ? "#284343" : "#C1DCDC"),
                "Existing tree row follows theme switch");
        }
        menu.Close();

        context.Open(contextTarget);
        ControlPreview.Flush();
        foreach (var theme in new[] { 2, 1 })
        {
            window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = theme;
            ControlPreview.Flush();
            Check(ColorOf(context.Background) == Color.Parse(theme == 2 ? "#0F0F0F" : "#FFFFFF"),
                "Open ContextMenu follows live theme switch");
        }
        var first = gallery.FindControl<MenuItem>("ContextInspectItem")!;
        first.Focus(NavigationMethod.Tab);
        PressKey(TopLevel.GetTopLevel(first)!, Key.Escape, PhysicalKey.Escape);
        Check(!context.IsOpen, "Escape dismisses ContextMenu");

        contextTarget.Focus(NavigationMethod.Tab);
        window.KeyPress(Key.N, RawInputModifiers.Control, PhysicalKey.N, "n");
        window.KeyRelease(Key.N, RawInputModifiers.Control, PhysicalKey.N, "n");
        ControlPreview.Flush();
        Check(model.Status == "菜单命令：新建项目", "Window shortcut invokes the same menu command");

        window.NavigateToCategory(0);
        gallery.ResetScroll();
        ControlPreview.Flush();
    }

    public static void Render(LegacyPreviewWindow window, LegacyControlGallery gallery, string output, bool dark)
    {
        window.NavigateToCategory(7);
        ControlPreview.Flush();
        Save(window, Path.Combine(output, $"navigation-{(dark ? "dark" : "light")}.png"));
        var menu = gallery.FindControl<Menu>("MenuSample")!;
        var file = gallery.FindControl<MenuItem>("FileMenu")!;
        menu.Open();
        file.IsSubMenuOpen = true;
        ControlPreview.Flush();
        Save(file.GetVisualDescendants().OfType<Popup>().Single().Child!, Path.Combine(output, $"menu-popup-{(dark ? "dark" : "light")}.png"));
        menu.Close();
        var context = gallery.FindControl<ContextMenu>("ContextSample")!;
        context.Open(gallery.FindControl<Border>("ContextTarget")!);
        ControlPreview.Flush();
        Save(context, Path.Combine(output, $"context-popup-{(dark ? "dark" : "light")}.png"));
        context.Close();
    }

    private static void Save(Control control, string path)
    {
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)), new Vector(96, 96));
        bitmap.Render(control);
        bitmap.Save(path, PngBitmapEncoderOptions.Default);
        Console.WriteLine($"Rendered {path}");
    }

    private static Border LayoutRoot(TreeViewItem item) => item.GetVisualDescendants().OfType<Border>()
        .Single(control => control.Name == "PART_LayoutRoot" && ReferenceEquals(control.TemplatedParent, item));
    private static TreeViewItem Item(TreeView tree, PreviewTreeNode node) =>
        tree.GetVisualDescendants().OfType<TreeViewItem>().Single(item => ReferenceEquals(item.DataContext, node));
    private static Color? ColorOf(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static void PressKey(TopLevel topLevel, Key key, PhysicalKey physical)
    {
        topLevel.KeyPress(key, RawInputModifiers.None, physical, null);
        topLevel.KeyRelease(key, RawInputModifiers.None, physical, null);
        ControlPreview.Flush();
    }
    private static void RightClick(TopLevel topLevel, Control control)
    {
        var point = control.TranslatePoint(new Point(Math.Min(50, control.Bounds.Width / 2), control.Bounds.Height / 2), topLevel)!.Value;
        topLevel.MouseMove(point);
        topLevel.MouseDown(point, MouseButton.Right);
        topLevel.MouseUp(point, MouseButton.Right);
        ControlPreview.Flush();
    }
    private static void ClickItem(MenuItem item) => MouseClick(TopLevel.GetTopLevel(item)!, item);
    private static void MouseClick(TopLevel topLevel, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), topLevel)!.Value;
        topLevel.MouseMove(point);
        topLevel.MouseDown(point, MouseButton.Left);
        topLevel.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException($"Failed: {message}");
        Console.WriteLine($"PASS: {message}");
    }
}
