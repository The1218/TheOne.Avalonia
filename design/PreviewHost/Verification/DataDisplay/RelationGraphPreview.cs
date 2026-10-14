using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class RelationGraphPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var density in Enum.GetValues<ControlSize>())
        {
            foreach (var direction in Enum.GetValues<Orientation>()) VerifyLayout(theme, density, direction, check, output);
            VerifySlots(theme, density, check, output);
        }
        VerifyChanges(check);
        VerifyConsumer(check);
        VerifyNativeContent(check);
    }

    private static IDataTemplate Icons() => new FuncDataTemplate<Geometry>((icon, _) => new PathIcon { Data = icon });
    private static Window Open(Control content, ThemeVariant theme = null!, ControlSize density = ControlSize.Normal, double width = 1360, double height = 760)
    {
        var window = new Window { Width = width, Height = height, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme ?? ThemeVariant.Dark, Content = new Border { Padding = new Thickness(24), Child = content } };
        ControlSizing.SetSize(window, density); Motion.SetEnabled(window, false);
        window.Show(); Flush(); return window;
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static T Part<T>(Control node, string name) where T : Control => node.GetVisualDescendants().OfType<T>().Single(part => part.Name == name && part.TemplatedParent == node);
    private static Point CenterIn(Control control, Visual relative) => control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), relative)!.Value;
    private static void Save(Window window, string? output, string name)
    {
        if (output is null) return;
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    private static void VerifyLayout(ThemeVariant theme, ControlSize density, Orientation direction, Action<bool, string> check, string? output)
    {
        var root = RelationGraphDemoViewModel.CreateTree();
        var graph = new RelationGraph { RootNode = root, Orientation = direction, IconTemplate = Icons() };
        var scroll = new ScrollViewer { Content = graph, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        var window = Open(scroll, theme, density);
        var label = $"RelationGraph/{theme.Key}/{density}/{direction}";
        check(graph.NodeCount == 8 && graph.ValidationError is null, $"{label}: single root and all three tree levels are realized");
        var cards = graph.GetVisualDescendants().OfType<RelationGraphNodePresenter>().ToArray();
        check(cards.Length == 8 && cards.All(card => ControlSizing.GetSize(card) == density && card.ActualThemeVariant == theme), $"{label}: theme and inherited density reach every native card");
        var expectedWidth = density == ControlSize.Small ? 220 : density == ControlSize.Large ? 300 : 260;
        check(cards.All(card => Math.Abs(card.Bounds.Width - expectedWidth) < 1), $"{label}: density sets consistent node widths");
        foreach (var card in cards)
        {
            var body = Part<StackPanel>(card, "PART_TextHost");
            check(Math.Abs(CenterIn(body, card).Y - card.Bounds.Height / 2) < 1, $"{label}/{card.Header}: title/content stack is vertically centered");
        }
        check(cards.All(card => cards.Where(other => !ReferenceEquals(card, other)).All(other => !card.Bounds.Intersects(other.Bounds))), $"{label}: node cards never overlap");
        var rootCard = graph.GetNodePresenter(root)!;
        foreach (var child in root.Children!)
        {
            var childCard = graph.GetNodePresenter(child)!;
            check(direction == Orientation.Horizontal ? childCard.Bounds.Left >= rootCard.Bounds.Right + graph.LevelSpacing - 1 : childCard.Bounds.Top >= rootCard.Bounds.Bottom + graph.LevelSpacing - 1,
                $"{label}: descendants advance in the selected direction");
        }
        var brush = (ISolidColorBrush)graph.ConnectorBrush!;
        using (var frame = window.CaptureRenderedFrame()!)
        {
            var childCard = graph.GetNodePresenter(root.Children![1])!;
            var start = direction == Orientation.Horizontal ? new Point(rootCard.Bounds.Right + 12, rootCard.Bounds.Center.Y) : new Point(rootCard.Bounds.Center.X, rootCard.Bounds.Bottom + 12);
            var point = graph.TranslatePoint(start, window)!.Value;
            check(HasInk(frame, point, brush.Color), $"{label}: connector appears in the actual rendered pixels");
        }
        Save(window, output, $"relation-{theme.Key}-{density}-{direction}");
        window.Width = 390; window.Height = 420; Flush();
        check(scroll.Extent.Width > scroll.Viewport.Width && cards.All(card => Math.Abs(card.Bounds.Width - expectedWidth) < 1), $"{label}: narrow viewport scrolls without squeezing or clipping node text");
        scroll.Offset = new Vector(scroll.Extent.Width, scroll.Extent.Height); Flush();
        check(scroll.Offset.X > 0 && scroll.Offset.X <= scroll.Extent.Width - scroll.Viewport.Width + 1, $"{label}: distant nodes remain reachable with native scrolling");
        Save(window, output, $"relation-narrow-{theme.Key}-{density}-{direction}");
        window.Close();
    }

    private static bool HasInk(Bitmap bitmap, Point point, Color expected)
    {
        var stride = bitmap.PixelSize.Width * 4;
        var bytes = new byte[stride * bitmap.PixelSize.Height];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), pin.AddrOfPinnedObject(), bytes.Length, stride); }
        finally { pin.Free(); }
        var scale = bitmap.Dpi.X / 96;
        for (var y = Math.Max(0, (int)(point.Y * scale) - 3); y <= Math.Min(bitmap.PixelSize.Height - 1, (int)(point.Y * scale) + 3); y++)
        for (var x = Math.Max(0, (int)(point.X * scale) - 3); x <= Math.Min(bitmap.PixelSize.Width - 1, (int)(point.X * scale) + 3); x++)
        {
            var i = y * stride + x * 4;
            var rgba = bitmap.Format == PixelFormat.Rgba8888;
            if (Math.Abs(bytes[i + (rgba ? 0 : 2)] - expected.R) + Math.Abs(bytes[i + 1] - expected.G) + Math.Abs(bytes[i + (rgba ? 2 : 0)] - expected.B) < 45) return true;
        }
        return false;
    }

    private static void VerifySlots(ThemeVariant theme, ControlSize density, Action<bool, string> check, string? output)
    {
        var samples = new RelationGraphDemoViewModel().SlotSamples;
        var panel = new WrapPanel();
        foreach (var sample in samples)
        {
            panel.Children.Add(new StackPanel { Spacing = 8, Margin = new Thickness(0, 0, 16, 16), Children =
            {
                new TextBlock { Text = sample.Label },
                new RelationGraphNodePresenter { Width = 280, MinHeight = 110, Header = sample.Node.Title, Content = sample.Node.Content, Icon = sample.Node.Icon, IconTemplate = Icons() }
            } });
        }
        var window = Open(panel, theme, density, width: 1280, height: 470);
        foreach (var card in panel.GetVisualDescendants().OfType<RelationGraphNodePresenter>())
        {
            var icon = Part<Border>(card, "PART_IconHost"); var text = Part<StackPanel>(card, "PART_TextHost");
            var title = Part<ContentPresenter>(card, "PART_HeaderPresenter"); var content = Part<ContentPresenter>(card, "PART_ContentPresenter");
            var label = $"RelationGraph/slots/{theme.Key}/{density}/{card.Header ?? card.Content ?? "icon/empty"}";
            check(icon.IsVisible == (card.Icon is not null) && title.IsVisible == (card.Header is not null) && content.IsVisible == (card.Content is not null), $"{label}: null slots reserve no region");
            if (card.Header is not null || card.Content is not null)
            {
                check(Math.Abs(CenterIn(text, card).Y - card.Bounds.Height / 2) < 1, $"{label}: remaining text uses the whole height and centers vertically");
                if (card.Icon is null) check(Math.Abs(text.TranslatePoint(default, card)!.Value.X - card.Padding.Left - card.BorderThickness.Left) < 1, $"{label}: no icon leaves the entire content width for text");
            }
            else if (card.Icon is not null) check(Math.Abs(CenterIn(icon, card).X - card.Bounds.Width / 2) < 1 && Math.Abs(CenterIn(icon, card).Y - card.Bounds.Height / 2) < 1, $"{label}: icon-only card centers across the entire node");
            check(Part<Border>(card, "PART_TextGap").IsVisible == (card.Header is not null && card.Content is not null), $"{label}: absent title/body removes the internal gap");
        }
        Save(window, output, $"relation-slots-{theme.Key}-{density}"); window.Close();
    }

    private static void VerifyChanges(Action<bool, string> check)
    {
        var root = RelationGraphDemoViewModel.CreateTree();
        var graph = new RelationGraph { RootNode = root, IconTemplate = Icons() };
        var scroll = new ScrollViewer { Content = graph, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto };
        var window = Open(scroll);
        var card = graph.GetNodePresenter(root)!;
        root.Title = "更新标题"; root.Icon = null; Flush();
        check(card.Header as string == "更新标题" && card.Icon is null && ReferenceEquals(card, graph.GetNodePresenter(root)), "RelationGraph/model: default model notifications update the existing native card");
        var compactHeight = card.Bounds.Height;
        root.Content = new string('长', 120); Flush();
        var wrappedHeight = card.Bounds.Height;
        check(wrappedHeight > compactHeight, "RelationGraph/model: changing existing content remeasures the tree height");
        root.Content = new string('长', 240); Flush();
        check(card.Bounds.Height > wrappedHeight, "RelationGraph/model: repeated content changes continue propagating layout invalidation");
        root.Content = null; Flush();
        check(Math.Abs(card.Bounds.Height - compactHeight) < 1, "RelationGraph/model: removing content restores the natural minimum height");
        var extra = new RelationGraphNode { Title = "新增节点", Content = "Long content wraps to multiple lines. " + new string('长', 80) };
        root.Children!.Add(extra); Flush();
        check(graph.NodeCount == 9 && ReferenceEquals(card, graph.GetNodePresenter(root)) && graph.GetNodePresenter(extra)!.Bounds.Height > graph.NodeMinHeight,
            "RelationGraph/model: observable additions reuse cards and allow content-driven height");
        graph.Orientation = Orientation.Vertical; Flush();
        var taller = graph.GetNodePresenter(extra)!;
        check(root.Children.All(child => graph.GetNodePresenter(child)!.Bounds.Top >= card.Bounds.Bottom + graph.LevelSpacing - 1), "RelationGraph/layout: taller sibling content retains the common depth row");
        root.Children.Remove(extra); Flush(); check(graph.NodeCount == 8 && graph.GetNodePresenter(extra) is null, "RelationGraph/model: removed nodes release their presenters");
        root.Children = new ObservableCollection<RelationGraphNode> { extra }; Flush();
        check(graph.NodeCount == 2, "RelationGraph/model: replacing the child collection updates the tree");
        extra.Children!.Add(root); Flush(); check(graph.NodeCount == 0 && graph.ValidationError is not null, "RelationGraph/model: cycles are rejected instead of recursing forever");
        extra.Children.Clear(); Flush(); check(graph.NodeCount == 2 && graph.ValidationError is null, "RelationGraph/model: observable correction recovers from an invalid tree");
        root.Children.Add(extra); Flush(); check(graph.ValidationError is not null, "RelationGraph/model: sharing a node between branches is rejected");
        root.Children.RemoveAt(1); Flush(); check(graph.NodeCount == 2, "RelationGraph/model: correcting repeated nodes recovers");
        root.Children = new List<RelationGraphNode>(); Flush(); root.Children.Add(extra); graph.RefreshData(); Flush();
        check(graph.NodeCount == 2, "RelationGraph/model: RefreshData supports collections without notifications");
        scroll.Content = null; Flush(); root.Children.Add(new() { Title = "detached update" }); scroll.Content = graph; Flush();
        check(graph.NodeCount == 3 && graph.GetNodePresenter(root)!.Header as string == "更新标题", "RelationGraph/lifetime: reattachment renews bindings and collection observers");
        graph.RootNode = null; Flush(); check(graph.NodeCount == 0 && graph.ValidationError is null, "RelationGraph/model: null root is an empty graph"); window.Close();
    }

    private static void VerifyConsumer(Action<bool, string> check)
    {
        var view = new RelationGraphDemoView();
        var window = Open(new ScrollViewer { Content = view, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto });
        var graph = view.FindControl<RelationGraph>("CustomRelationGraph")!;
        var model = view.Model.CustomRoot;
        check(graph.NodeCount == 3 && graph.GetNodePresenter(model)!.Header as string == "业务模型", "RelationGraph/compiled consumer: ChildrenSource selects Branches on the user's model");
        var original = graph.GetNodePresenter(model)!;
        model.Name = "业务模型已更新"; model.Description = null; Flush();
        check(original.Header as string == model.Name && original.Content is null, "RelationGraph/compiled consumer: custom title/content bindings observe changes");
        model.Branches!.Add(new() { Name = "外部新增" }); Flush(); check(graph.NodeCount == 4, "RelationGraph/compiled consumer: custom child collection changes are observed");
        model.Branches = [new() { Name = "替换分支", Description = "替换整个属性" }]; Flush();
        check(graph.NodeCount == 2 && ReferenceEquals(original, graph.GetNodePresenter(model)), "RelationGraph/compiled consumer: replacing the bound children property reuses the root card");
        view.Model.Direction = Orientation.Vertical; Flush(); check(graph.Orientation == Orientation.Vertical && graph.GetNodePresenter(model.Branches[0])!.Bounds.Top > original.Bounds.Bottom, "RelationGraph/compiled consumer: Orientation binding switches to vertical layout");
        graph.ChildrenSource = CompiledBinding.Create<RelationUnit, string?>(unit => unit.Name); Flush();
        check(graph.ValidationError is not null && graph.NodeCount == 0, "RelationGraph/compiled consumer: non-collection ChildrenSource produces a validation error");
        graph.ChildrenSource = CompiledBinding.Create<RelationUnit, ObservableCollection<RelationUnit>?>(unit => unit.Branches); Flush();
        check(graph.ValidationError is null && graph.NodeCount == 2, "RelationGraph/compiled consumer: replacing the binding restores the valid tree");
        window.Close();
    }

    private static void VerifyNativeContent(Action<bool, string> check)
    {
        var clicked = 0;
        var button = new Button { Content = "原生命令按钮" }; button.Click += (_, _) => clicked++;
        var icon = new PathIcon { Data = RelationGraphDemoViewModel.CheckIcon };
        var root = new RelationGraphNode { Title = "包含控件", Content = button, Icon = icon };
        var graph = new RelationGraph { RootNode = root };
        var window = Open(graph);
        var point = CenterIn(button, window);
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
        check(clicked == 1 && button.Focusable && button.Bounds.Height > 0, "RelationGraph/native content: embedded controls retain pointer and keyboard behavior");
        root.Children!.Add(new() { Title = "新节点" }); Flush();
        check(ReferenceEquals(graph.GetNodePresenter(root)!.Content, button) && icon.Parent == graph.GetNodePresenter(root), "RelationGraph/native content: tree changes preserve caller-owned content and logical parents");
        graph.RootNode = new RelationGraphNode { Title = "新根节点" }; Flush();
        check(button.Parent is null && icon.Parent is null, "RelationGraph/native content: replacing the root releases caller-owned controls");
        window.Close();
    }
}
