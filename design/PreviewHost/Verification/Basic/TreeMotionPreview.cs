using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.EnhancementsPreview;

namespace TheOne.DesignPreview;

internal static class TreeMotionPreview
{
    public static void Verify(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        gallery.FindControl<TreeView>("MotionTree")!.BringIntoView(); ControlPreview.Flush();
        var root = gallery.FindControl<TreeViewItem>("MotionTreeRoot")!;
        var viewport = Part<RevealHost>(root, "TreeContent");
        var fullHeight = viewport.Bounds.Height;
        Motion.SetEnabled(window, true);
        Click(window, gallery.FindControl<Button>("TreeToggleButton")!);
        Check(!root.IsExpanded && viewport.IsVisible && !viewport.IsHitTestVisible, "Tree preview button starts a clipped, non-interactive collapse");
        AdvanceUntil(() => !viewport.IsVisible, 1500);
        root.Focus(NavigationMethod.Tab); PressKey(window, Key.Right, PhysicalKey.ArrowRight);
        Check(root.IsExpanded, "Right arrow expands the animated tree using native keyboard behaviour");
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, 1500);
        PressKey(window, Key.Left, PhysicalKey.ArrowLeft);
        Check(!root.IsExpanded, "Left arrow collapses the animated tree using native keyboard behaviour");
        Click(window, gallery.FindControl<Button>("TreeReverseButton")!);
        Advance(20);
        Click(window, gallery.FindControl<Button>("TreeReverseButton")!);
        AdvanceUntil(() => root.IsExpanded && Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, 1500);
        Check(root.IsExpanded && viewport.IsVisible && viewport.IsHitTestVisible && Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, "Repeated quick-reversal preview clicks cancel the old sequence and finish expanded");
        Click(window, Part<ToggleButton>(root, "PART_ExpandCollapseChevron"));
        Check(!root.IsExpanded && viewport.IsVisible && !viewport.IsHitTestVisible, "Native tree chevron starts the same clipped collapse by mouse");
        AdvanceUntil(() => !viewport.IsVisible, 1500);
        Click(window, Part<ToggleButton>(root, "PART_ExpandCollapseChevron"));
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, 1500);
        Check(root.IsExpanded && Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, "Native tree chevron restores the full branch by mouse");
        var toggle = gallery.FindControl<CheckBox>("MotionToggle")!;
        root.IsExpanded = false; ControlPreview.Flush(); Advance(20);
        Click(window, toggle);
        Check(!Motion.GetEnabled(window) && !viewport.IsVisible, "Preview motion switch immediately completes an in-flight tree collapse");
        root.IsExpanded = true; ControlPreview.Flush();
        Click(window, toggle);
        Check(Motion.GetEnabled(window) && Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, "Re-enabling preview motion does not replay expanded tree branches");
        Motion.SetEnabled(window, false);

        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyTree(variant, size);
    }

    private static void VerifyTree(ThemeVariant variant, ControlSize size)
    {
        var probe = new HeaderProbe { Width = 180, Child = new TextBlock { Text = "完整布局保持不变，收起区域逐步裁切，下方节点连续移动。", TextWrapping = TextWrapping.Wrap } };
        var leaf = new TreeViewItem { Header = probe };
        var nested = new TreeViewItem { Header = "主题颜色", IsExpanded = true };
        nested.Items.Add(new TreeViewItem { Header = "浅色主题" });
        nested.Items.Add(new TreeViewItem { Header = "深色主题" });
        var root = new TreeViewItem { Header = "资源与控件", IsExpanded = true };
        root.Items.Add(nested); root.Items.Add(leaf);
        var sibling = new TreeViewItem { Header = "下方节点 · 连续移动" };
        var tree = new TreeView(); tree.Items.Add(root); tree.Items.Add(sibling);
        var window = new Window { Width = 420, Height = 460, Content = tree, RequestedThemeVariant = variant, WindowDecorations = WindowDecorations.None };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        var viewport = Part<RevealHost>(root, "TreeContent");
        var nestedViewport = Part<RevealHost>(nested, "TreeContent");
        var header = Part<Border>(root, "PART_LayoutRoot");
        var fullHeight = viewport.Bounds.Height;
        var nestedHeight = nestedViewport.Bounds.Height;
        double SiblingTop() => sibling.TranslatePoint(default, tree)!.Value.Y;
        var siblingStart = SiblingTop();
        var label = $"Tree/{variant.Key}/{size}";
        tree.SelectedItem = leaf; Motion.SetEnabled(window, true); ControlPreview.Flush();
        Check(Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5 && Math.Abs(nestedViewport.Bounds.Height - nestedHeight) < 0.5, $"{label}: enabling motion does not replay either tree level");
        var measureCount = probe.MeasureCount;
        root.IsExpanded = false; ControlPreview.Flush();
        Check(viewport.IsVisible && !viewport.IsHitTestVisible && viewport.Bounds.Height > fullHeight * 0.85, $"{label}: collapse starts at the current extent and blocks child pointer input");
        var frames = new List<(double Height, double SiblingTop)>();
        for (var i = 0; i < 4; i++) { Advance(20); frames.Add((viewport.Bounds.Height, SiblingTop())); }
        Check(frames.Select(frame => frame.Height).Distinct().Count() >= 3 && frames[0].Height < fullHeight && frames[^1].Height > 0 && frames.Zip(frames.Skip(1)).All(pair => pair.First.Height >= pair.Second.Height), $"{label}: collapse produces multiple monotonically shrinking frames");
        Check(frames[0].SiblingTop < siblingStart && frames[^1].SiblingTop > header.Bounds.Height && frames.Zip(frames.Skip(1)).All(pair => pair.First.SiblingTop >= pair.Second.SiblingTop), $"{label}: following root node moves throughout collapse");
        Check(probe.MeasureCount - measureCount <= 1 && viewport.Opacity == 1 && viewport.RenderTransform == null && Math.Abs(probe.Bounds.Height - probe.DesiredSize.Height) < 0.5, $"{label}: full child layout is reused without fading, translating or reflowing text");
        var reversedAt = viewport.Bounds.Height;
        root.IsExpanded = true; ControlPreview.Flush();
        Check(Math.Abs(viewport.Bounds.Height - reversedAt) < fullHeight * 0.15, $"{label}: reverse animation begins at the current visible extent");
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, 1500);
        Check(viewport.IsVisible && viewport.IsHitTestVisible && Math.Abs(SiblingTop() - siblingStart) < 0.5 && ReferenceEquals(tree.SelectedItem, leaf), $"{label}: reversal restores layout and preserves the selected descendant");

        root.IsExpanded = false; ControlPreview.Flush(); Advance(35);
        root.IsExpanded = true; ControlPreview.Flush(); Advance(25);
        root.IsExpanded = false; ControlPreview.Flush(); Advance(20);
        root.IsExpanded = true; ControlPreview.Flush();
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, 1500);
        Check(root.IsExpanded && viewport.IsVisible && viewport.IsHitTestVisible && Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5, $"{label}: rapid toggles settle on the latest expanded state");
        nested.IsExpanded = false; root.IsExpanded = false; ControlPreview.Flush(); Advance(30);
        root.IsExpanded = true; nested.IsExpanded = true; ControlPreview.Flush();
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5 && Math.Abs(nestedViewport.Bounds.Height - nestedHeight) < 0.5, 1500);
        Check(Math.Abs(viewport.Bounds.Height - fullHeight) < 0.5 && Math.Abs(nestedViewport.Bounds.Height - nestedHeight) < 0.5, $"{label}: simultaneous parent and child reversals restore both levels");
        nested.IsExpanded = false; root.IsExpanded = false; ControlPreview.Flush();
        AdvanceUntil(() => !viewport.IsVisible && !nestedViewport.IsVisible, 1500);
        Check(!viewport.IsVisible && !nestedViewport.IsVisible && Math.Abs(SiblingTop() - header.Bounds.Height) < 0.5, $"{label}: completed collapse has no leftover child layout space");
        root.IsExpanded = true; ControlPreview.Flush();
        var withoutNestedHeight = fullHeight - nestedHeight;
        AdvanceUntil(() => Math.Abs(viewport.Bounds.Height - withoutNestedHeight) < 0.5, 1500);
        Check(!nested.IsExpanded && !nestedViewport.IsVisible && Math.Abs(viewport.Bounds.Height - withoutNestedHeight) < 0.5, $"{label}: reopening a parent retains its child's collapsed state");
        var beforeInsert = SiblingTop();
        root.Items.Add(new TreeViewItem { Header = "动态新增节点" }); ControlPreview.Flush();
        Check(SiblingTop() > beforeInsert && root.ContainerFromIndex(root.Items.Count - 1) is TreeViewItem, $"{label}: a settled branch still responds to live item changes");
        nested.IsExpanded = true; root.IsExpanded = false; ControlPreview.Flush(); Advance(20);
        Motion.SetEnabled(window, false); ControlPreview.Flush();
        Check(!viewport.IsVisible && nestedViewport.IsVisible, $"{label}: disabling motion mid-flight snaps each level to its latest state");
        root.IsExpanded = true; ControlPreview.Flush(); var snappedHeight = viewport.Bounds.Height;
        Motion.SetEnabled(window, true); ControlPreview.Flush();
        Check(Math.Abs(viewport.Bounds.Height - snappedHeight) < 0.5, $"{label}: re-enabling motion keeps the settled layout unchanged");
        root.IsExpanded = false; ControlPreview.Flush(); Advance(20);
        viewport.Duration = TimeSpan.Zero; ControlPreview.Flush();
        Check(!viewport.IsVisible, $"{label}: a zero duration immediately finishes the current collapse");
        window.Close();
    }

    public static void Render(string output, bool dark)
    {
        var root = new TreeViewItem { Header = "资源与控件", IsExpanded = true };
        var nested = new TreeViewItem { Header = "主题颜色", IsExpanded = true };
        nested.Items.Add(new TreeViewItem { Header = "浅色主题" }); nested.Items.Add(new TreeViewItem { Header = "深色主题" });
        root.Items.Add(nested); root.Items.Add(new TreeViewItem { Header = "控件模板" });
        var tree = new TreeView(); tree.Items.Add(root); tree.Items.Add(new TreeViewItem { Header = "下方节点 · 连续移动" });
        var panel = new StackPanel { Spacing = 16, Margin = new Thickness(24) };
        panel.Children.Add(new TextBlock { Text = "TreeView · 平滑展开与收起", FontSize = 18, FontWeight = FontWeight.SemiBold });
        panel.Children.Add(new TextBlock { Text = "完整布局保持不变，快速反向从当前进度继续。", TextWrapping = TextWrapping.Wrap }); panel.Children.Add(tree);
        var window = new Window { Width = 440, Height = 340, Content = panel, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        var theme = dark ? "dark" : "light";
        Save(window, Path.Combine(output, $"tree-{theme}-open.png"));
        Motion.SetEnabled(window, true); root.IsExpanded = false;
        Advance(45); Save(window, Path.Combine(output, $"tree-{theme}-closing.png"));
        root.IsExpanded = true;
        Advance(35); Save(window, Path.Combine(output, $"tree-{theme}-reversing.png"));
        Advance(210); root.IsExpanded = false;
        Advance(180); Save(window, Path.Combine(output, $"tree-{theme}-closed.png"));
        root.IsExpanded = true;
        Advance(50); Save(window, Path.Combine(output, $"tree-{theme}-opening.png"));
        window.Close();
    }

    private sealed class HeaderProbe : Decorator
    {
        public int MeasureCount { get; private set; }
        protected override Size MeasureOverride(Size availableSize) { MeasureCount++; return base.MeasureOverride(availableSize); }
    }

    private static T Part<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name && ReferenceEquals(control.TemplatedParent, owner));
    private static void Click(TopLevel window, Control control)
    {
        control.BringIntoView(); ControlPreview.Flush();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
    }
    private static void PressKey(TopLevel window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null); window.KeyRelease(key, RawInputModifiers.None, physical, null); ControlPreview.Flush();
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException($"Failed: {message}");
        Console.WriteLine($"PASS: {message}");
    }
}
