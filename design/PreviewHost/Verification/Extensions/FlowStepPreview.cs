using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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

internal static class FlowStepPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var orientation in Enum.GetValues<Orientation>())
            VerifyLayout(theme, size, orientation, check);
        VerifyChanges(check);
        VerifyTemplates(check);
        VerifyDemo(check);
        if (output is not null) Render(output);
    }
    private static ObservableCollection<FlowNodeItem> Items() => new FlowStepDemoViewModel().Nodes;
    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal, double width = 1080, double height = 760)
    {
        var window = new Window { Width = width, Height = height, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme ?? ThemeVariant.Light,
            Content = new Border { Padding = new Thickness(24), Child = content } };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static FlowNode[] Nodes(FlowStep flow) => flow.GetRealizedContainers().OfType<FlowNode>().ToArray();
    private static T Part<T>(Control control, string name) where T : Control => control.GetVisualDescendants().OfType<T>().Single(child => child.Name == name && child.TemplatedParent == control);
    private static Point Centre(FlowNode node, Visual target)
    {
        var marker = Part<Control>(node, "PART_Indicator");
        return marker.TranslatePoint(new Point(marker.Bounds.Width / 2, marker.Bounds.Height / 2), target)!.Value;
    }
    private static bool Visible(Control control) => control.IsEffectivelyVisible;
    private static void VerifyLayout(ThemeVariant theme, ControlSize size, Orientation orientation, Action<bool, string> check)
    {
        var flow = new FlowStep { ItemsSource = Items(), Orientation = orientation, VerticalAlignment = VerticalAlignment.Top };
        var window = Open(flow, theme, size);
        try
        {
            var label = $"FlowStep/{theme.Key}/{size}/{orientation}";
            var nodes = Nodes(flow);
            check(nodes.Length == 4 && nodes.Select(node => node.Status).SequenceEqual(new[] { FlowNodeStatus.Completed, FlowNodeStatus.Completed, FlowNodeStatus.InProgress, FlowNodeStatus.Pending }), $"{label}: compiled item bindings preserve all business states");
            var diameter = size switch { ControlSize.Small => 24, ControlSize.Large => 36, _ => 30 };
            check(nodes.All(node => node.Orientation == orientation && Part<Control>(node, "PART_Indicator").Bounds.Size == new Size(diameter, diameter)), $"{label}: orientation and density reach every marker");
            var points = nodes.Select(node => Centre(node, flow)).ToArray();
            check(orientation == Orientation.Horizontal ? points.All(point => Math.Abs(point.Y - points[0].Y) < 0.1) && nodes.All(node => Math.Abs(node.Bounds.Width - nodes[0].Bounds.Width) < 0.1) :
                points.All(point => Math.Abs(point.X - points[0].X) < 0.1) && points.Zip(points.Skip(1)).All(pair => pair.Second.Y > pair.First.Y), $"{label}: equal horizontal slots or a continuous vertical rail");
            check(Visible(Part<Control>(nodes[0], "PART_Check")) && !Visible(Part<Control>(nodes[2], "PART_Check")) && Visible(Part<Control>(nodes[2], "PART_Dot")) && !Visible(Part<Control>(nodes[3], "PART_Dot")), $"{label}: states differ in shape as well as colour");
            check(nodes[3].GetVisualDescendants().OfType<TextBlock>().Where(Visible).Count(text => !string.IsNullOrWhiteSpace(text.Text)) == 3, $"{label}: missing time has no placeholder and vertical headings are not duplicated");
            CheckRails(flow, theme, check, label);
        }
        finally { window.Close(); Flush(); }
    }
    private static void CheckRails(FlowStep flow, ThemeVariant theme, Action<bool, string> check, string label)
    {
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(flow.Bounds.Width), (int)Math.Ceiling(flow.Bounds.Height)), new Vector(96, 96));
        bitmap.Render(flow);
        var nodes = Nodes(flow);
        var success = Resource(flow, "One.Success");
        var line = Resource(flow, "One.Line");
        bool Near(Point point, Color expected) => Enumerable.Range(-1, 3).Any(dx => Enumerable.Range(-1, 3).Any(dy =>
            Pixel(bitmap, (int)Math.Round(point.X) + dx, (int)Math.Round(point.Y) + dy) is { } colour && Math.Abs(colour.R - expected.R) + Math.Abs(colour.G - expected.G) + Math.Abs(colour.B - expected.B) < 12));
        var correct = Enumerable.Range(1, nodes.Length - 1).All(index =>
        {
            var a = Centre(nodes[index - 1], flow); var b = Centre(nodes[index], flow);
            var expected = nodes[index - 1].IsCompleted && nodes[index].Status != FlowNodeStatus.Pending ? success.Color : line.Color;
            return Near(new Point((a.X + b.X) / 2, (a.Y + b.Y) / 2), expected);
        });
        check(correct, $"{label}: actual pixels show completed and pending connectors with {theme.Key} resources");
    }
    private static void VerifyChanges(Action<bool, string> check)
    {
        var items = Items(); var flow = new FlowStep { ItemsSource = items, VerticalAlignment = VerticalAlignment.Top };
        var window = Open(flow);
        try
        {
            items[2].Status = FlowNodeStatus.Completed; items[3].Status = FlowNodeStatus.InProgress; Flush();
            check(Nodes(flow)[2].EffectiveStateText == "已完成" && Nodes(flow)[3].IsInProgress, "FlowStep/changes: model status changes update markers and fallback state text");
            CheckRails(flow, ThemeVariant.Light, check, "FlowStep/changes");
            flow.Orientation = Orientation.Vertical; Flush();
            var nodes = Nodes(flow); var oldY = Centre(nodes[1], flow).Y;
            ((FlowNodeContent)items[0].Content!).Text = string.Join("\n", Enumerable.Repeat("内容变高时连接线仍按实际节点位置绘制", 5)); Flush();
            check(Nodes(flow).All(node => node.IsVertical) && Centre(nodes[1], flow).Y > oldY + 40, "FlowStep/changes: runtime orientation and longer content reflow subsequent markers");
            CheckRails(flow, ThemeVariant.Light, check, "FlowStep/long-content");
            ((FlowNodeContent)items[0].Content!).State = "已提交业务系统"; Flush();
            check(nodes[0].EffectiveStateText == "已提交业务系统", "FlowStep/changes: custom state descriptions remain separate from marker status");
            items.Move(3, 0); Flush();
            check(Nodes(flow)[0].IsInProgress, "FlowStep/changes: moving an item updates flow order without rewriting business states");
            items.RemoveAt(0); items.Add(new FlowNodeItem { Content = new FlowNodeContent { Title = "新节点" } }); Flush();
            check(Nodes(flow).Length == 4 && Nodes(flow)[3].Status == FlowNodeStatus.Pending && ((FlowNodeContent)Nodes(flow)[3].Content!).Title == "新节点", "FlowStep/changes: insertion and removal leave no stale content or state bindings");
            flow.Orientation = Orientation.Horizontal; window.Width = 360; Flush();
            var scroll = Part<ScrollViewer>(flow, "PART_ScrollViewer");
            check(scroll.Extent.Width > scroll.Viewport.Width && Nodes(flow).All(node => node.Bounds.Width >= flow.MinNodeWidth - 1), "FlowStep/narrow: horizontal nodes retain their minimum width and scroll");
            flow.Orientation = Orientation.Vertical; Flush();
            check(Part<ScrollViewer>(flow, "PART_ScrollViewer").Extent.Width <= flow.Bounds.Width + 1 && Nodes(flow).All(node => node.Bounds.Width <= flow.Bounds.Width), "FlowStep/narrow: vertical content wraps within the viewport");
            flow.Height = 160; Flush();
            scroll = Part<ScrollViewer>(flow, "PART_ScrollViewer");
            check(scroll.Extent.Height > scroll.Viewport.Height && scroll.VerticalScrollBarVisibility == global::Avalonia.Controls.Primitives.ScrollBarVisibility.Auto, "FlowStep/narrow: a bounded vertical flow scrolls its natural-height content");
            flow.Height = double.NaN; flow.Padding = new Thickness(12); Flush();
            check(Centre(Nodes(flow)[0], flow).X >= 12 + Nodes(flow)[0].IndicatorSize / 2, "FlowStep/layout: native Padding insets markers and content together");
            items.Clear(); Flush(); check(Nodes(flow).Length == 0, "FlowStep/changes: collection reset clears all containers");
            items.Add(new FlowNodeItem { Content = new FlowNodeContent { Title = "唯一节点" } }); Flush();
            check(Nodes(flow).Length == 1 && Nodes(flow)[0].EffectiveContentTemplate is not null, "FlowStep/changes: empty and single-node flows have valid layouts");
        }
        finally { window.Close(); Flush(); }
    }
    private static IDataTemplate Template(string label) => new FuncDataTemplate<FlowNodeContent>((_, _) => new TextBlock { Text = label });
    private static void VerifyTemplates(Action<bool, string> check)
    {
        var items = Items(); var shared = Template("共享模板"); var local = Template("节点模板");
        var flow = new FlowStep { ItemsSource = items, ContentTemplate = shared, Orientation = Orientation.Vertical, VerticalAlignment = VerticalAlignment.Top };
        items[1].ContentTemplate = local;
        var window = Open(flow);
        try
        {
            check(Nodes(flow)[0].EffectiveContentTemplate == shared && Nodes(flow)[1].EffectiveContentTemplate == local, "FlowStep/templates: a node template overrides the shared template");
            flow.ContentTemplate = null; items[1].ContentTemplate = null; Flush();
            check(Nodes(flow).All(node => node.EffectiveContentTemplate == node.DefaultContentTemplate), "FlowStep/templates: clearing overrides restores the default template");
            var ambient = Template("类型模板"); flow.DataTemplates.Add(ambient);
            items[0].Content = new FlowNodeContent { Title = "类型匹配" }; Flush();
            check(Nodes(flow)[0].EffectiveContentTemplate == ambient, "FlowStep/templates: a typed ambient template is resolved before the built-in fallback");
        }
        finally { window.Close(); Flush(); }
        var own = new FlowNode { Content = new FlowNodeContent { Title = "直接声明" } };
        flow = new FlowStep { ContentTemplate = shared, Items = { own }, VerticalAlignment = VerticalAlignment.Top };
        window = Open(flow);
        try
        {
            check(own.EffectiveContentTemplate == shared, "FlowStep/templates: directly declared FlowNode also receives the shared template");
            var input = new TextBox { Text = "编辑内容" };
            own.ContentTemplate = null; flow.ContentTemplate = null; own.Content = input; Flush();
            check(Part<ContentPresenter>(own, "PART_ContentPresenter").Child == input && input.Focus(), "FlowNode/content: native controls retain presenter identity and focus");
            own.Status = FlowNodeStatus.Completed; Flush();
            check(Visible(Part<Control>(own, "PART_Check")), "FlowNode/content: direct node state changes update its marker");
        }
        finally { window.Close(); Flush(); }
    }
    private static void VerifyDemo(Action<bool, string> check)
    {
        var demo = new FlowStepDemoView(); var window = Open(demo, height: 1900);
        try
        {
            var button = demo.GetVisualDescendants().OfType<Button>().Single(value => value.Name == "DetailsButton");
            var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
            check(demo.Model.Approval.DetailsCount == 1 && demo.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == demo.Model.Approval.Details), "FlowStep/demo: custom ContentTemplate invokes a compiled command and displays its result");
            demo.Model.AdvanceCommand.Execute(null); Flush();
            check(Nodes(demo.FindControl<FlowStep>("HorizontalFlow")!)[3].IsInProgress && Nodes(demo.FindControl<FlowStep>("VerticalFlow")!)[3].IsInProgress, "FlowStep/demo: both layouts update from the same observable items");
            window.Width = 380; Flush();
            check(Grid.GetRow(demo.FindControl<StackPanel>("CustomExamples")!) == 1, "FlowStep/demo: examples stack in a narrow viewport");
        }
        finally { window.Close(); Flush(); }
    }
    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1080, 380 })
        {
            var demo = new FlowStepDemoView(); var window = Open(demo, theme, size, width, 1900);
            try
            {
                window.Height = Math.Ceiling(demo.DesiredSize.Height) + 48; Flush();
                using var frame = window.CaptureRenderedFrame()!;
                var path = Path.Combine(output, $"flowstep-{theme.Key}-{size}-{width}.png");
                frame.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
            }
            finally { window.Close(); Flush(); }
        }
    }
    private static ISolidColorBrush Resource(Control control, string key)
    {
        if (!control.TryFindResource(key, control.ActualThemeVariant, out var value) || value is not ISolidColorBrush brush) throw new InvalidOperationException($"Missing resource: {key}");
        return brush;
    }
    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        if (x < 0 || y < 0 || x >= bitmap.PixelSize.Width || y >= bitmap.PixelSize.Height) return default;
        var bytes = new byte[4]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4); }
        finally { pin.Free(); }
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]) : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
