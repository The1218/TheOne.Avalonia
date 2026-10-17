using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Workflow;

namespace TheOne.DesignPreview;

internal static class WorkflowOptimizationPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        IndexLifetime(check);
        GraphSemantics(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1280, 480 })
            LibraryAndReplacement(theme, size, width, check, output);
    }

    private static WorkflowNode Node(string id, string output = "out")
    {
        var node = new WorkflowNode { Id = id, Title = id };
        node.Ports.Add(new("in", "Input", WorkflowPortDirection.Input, "flow", 0));
        node.Ports.Add(new(output, "Output", WorkflowPortDirection.Output, "flow", 0,
            [new("value", "Value", "flow")]));
        return node;
    }

    private static WorkflowConnection Edge(string source, string target) => new()
    {
        SourceNodeId = source, SourcePortId = "out", TargetNodeId = target, TargetPortId = "in"
    };

    private static void IndexLifetime(Action<bool, string> check)
    {
        var document = new WorkflowDocument();
        var first = Node("same");
        var duplicate = Node("same", "other");
        var target = Node("target");
        document.Nodes.Add(first);
        document.Nodes.Add(duplicate);
        document.Nodes.Add(target);
        check(document.CheckConnection("same", "out", "target", "in") == null &&
              document.Validate().Any(i => i.Message == "节点 ID 为空或重复"),
            "Workflow/index: duplicate draft IDs still resolve to the first collection item and remain invalid");
        document.Nodes.Move(1, 0);
        check(document.CheckConnection("same", "out", "target", "in") == "节点或端口不存在",
            "Workflow/index: Move changes duplicate-ID resolution to the current first item");
        document.Nodes[0] = Node("replacement");
        check(document.CheckConnection("same", "out", "target", "in") == null &&
              document.CheckConnection("replacement", "out", "target", "in") == null,
            "Workflow/index: Replace invalidates old IDs without hiding surviving duplicate IDs");
        document.Nodes[1] = Node("same", "new");
        check(document.CheckConnection("same", "out", "target", "in") == "节点或端口不存在" &&
              document.CheckConnection("same", "new", "target", "in") == null,
            "Workflow/index: replacing a model with the same ID does not retain old ports");
        document.Nodes.Clear();
        check(document.CheckConnection("same", "new", "target", "in") == "节点或端口不存在",
            "Workflow/index: Reset discards old node lookup entries");
        document.Nodes.Add(first);
        document.Nodes.Add(target);
        var edge = Edge("same", "target");
        document.Connections.Add(edge);
        check(document.IsReachable("same", "target"), "Workflow/index: added edges update reachability");
        document.Connections[0] = Edge("target", "same");
        check(!document.IsReachable("same", "target") && document.IsReachable("target", "same"),
            "Workflow/index: Replace rebuilds adjacency and endpoint indexes");
        document.Connections.Clear();
        check(!document.IsReachable("target", "same") &&
              document.CheckConnection("same", "out", "target", "in") == null,
            "Workflow/index: connection Reset discards stale reachability and duplicate counts");

        var notifications = 0;
        document.Changed += (_, _) => notifications++;
        document.Nodes.Add(first);
        document.Nodes.Remove(first);
        notifications = 0;
        first.Title = "still observed";
        check(notifications == 1,
            "Workflow/index: removing one duplicate model reference retains exactly one subscription");
        document.Nodes.Remove(first);
        notifications = 0;
        first.Title = "released";
        edge.Label = "released edge";
        duplicate.Title = "replaced node";
        check(notifications == 0, "Workflow/index: removed and replaced models no longer publish document changes");

        var draft = new WorkflowDocument();
        draft.Nodes.Add(Node(null!));
        draft.Nodes.Add(Node(""));
        draft.Nodes.Add(Node("target"));
        check(draft.Validate().Count(i => i.Message == "节点 ID 为空或重复") == 2 &&
              draft.CheckConnection(null!, "out", "target", "in") == null &&
              draft.CheckConnection("", "out", "target", "in") == null,
            "Workflow/index: null and empty draft IDs remain separately resolvable and both fail validation");
    }

    private static void GraphSemantics(Action<bool, string> check)
    {
        var random = new Random(4182);
        for (var sample = 0; sample < 12; sample++)
        {
            var document = new WorkflowDocument();
            for (var i = 0; i < 24; i++) document.Nodes.Add(Node($"node-{i}"));
            for (var i = 0; i < 48; i++)
                document.Connections.Add(Edge($"node-{random.Next(24)}", $"node-{random.Next(24)}"));
            var expected = document.Connections.Where(e => Reachable(document, e.TargetNodeId, e.SourceNodeId))
                .Select(e => e.Id).ToHashSet();
            var actual = document.Validate().Where(i => i.Message == "存在环路")
                .Select(i => i.ConnectionId!).ToHashSet();
            check(expected.SetEquals(actual),
                $"Workflow/index/graph-{sample}: iterative component validation matches directed reachability");
            var target = document.Nodes[sample];
            var property = new WorkflowProperty { DataType = "flow" };
            var expectedSources = document.Nodes.Where(n => n != target && Reachable(document, n.Id, target.Id))
                .Select(n => n.Id);
            check(document.GetReferenceOptions(target, property).Select(o => o.Reference.NodeId)
                    .SequenceEqual(expectedSources) && document.Validate(true).All(i => i.Message != "存在环路"),
                $"Workflow/index/graph-{sample}: upstream options retain collection order and AllowCycles semantics");
        }

        var chain = new WorkflowDocument();
        for (var i = 0; i < 1200; i++) chain.Nodes.Add(Node($"node-{i}"));
        for (var i = 1; i < 1200; i++) chain.Connections.Add(Edge($"node-{i - 1}", $"node-{i}"));
        var timer = Stopwatch.StartNew();
        var valid = chain.Validate().Count == 0;
        timer.Stop();
        check(valid && chain.IsReachable("node-0", "node-1199"),
            $"Workflow/index/chain: 1,200-node graph validates without recursive traversal ({timer.Elapsed.TotalMilliseconds:F2} ms; diagnostic only)");
    }

    private static bool Reachable(WorkflowDocument document, string from, string to)
    {
        HashSet<string> seen = [];
        var pending = new Stack<string>();
        pending.Push(from);
        while (pending.TryPop(out var id))
        {
            if (!seen.Add(id)) continue;
            if (id == to) return true;
            foreach (var edge in document.Connections.Where(e => e.SourceNodeId == id))
                pending.Push(edge.TargetNodeId);
        }

        return false;
    }

    private static void LibraryAndReplacement(ThemeVariant theme, ControlSize size, int width,
        Action<bool, string> check, string? output)
    {
        var model = new WorkflowEditorDemoViewModel();
        var editor = new WorkflowEditor
        {
            Document = model.Document, NodeDefinitions = model.NodeDefinitions, ShowProperties = false
        };
        var window = new Window { Width = width, Height = 600, Content = editor, RequestedThemeVariant = theme };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var label = $"Workflow/optimization/{theme}/{size}/{width}";
        try
        {
            var toggle = editor.GetVisualDescendants().OfType<ToggleButton>()
                .Single(b => b.Name == "PART_NodeLibraryToggle");
            var collapse = editor.GetVisualDescendants().OfType<Button>()
                .Single(b => b.Name == "PART_NodeLibraryCollapse");
            var library = editor.GetVisualDescendants().OfType<Border>()
                .Single(b => b.Name == "PART_NodeLibrary");
            Click(window, collapse);
            check(!editor.ShowNodeLibrary && !library.IsVisible && toggle.IsEffectivelyVisible && toggle.IsChecked == false,
                $"{label}: collapsing the node library retains a visible toolbar entry");
            Click(window, toggle);
            check(editor.ShowNodeLibrary && library.IsVisible && toggle.IsChecked == true,
                $"{label}: real pointer reopens the node library after collapsing it");
            toggle.Focus();
            window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Flush();
            check(!editor.ShowNodeLibrary && toggle.IsChecked == false,
                $"{label}: native keyboard toggle remains available");

            var document = model.Document;
            var oldNode = document.Nodes[0];
            editor.SelectedNode = oldNode;
            var replacement = Node(oldNode.Id);
            document.Nodes[0] = replacement;
            Flush();
            var canvas = editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single();
            var borders = canvas.GetVisualDescendants().OfType<Border>().Where(b => b.Tag is WorkflowNode).ToArray();
            check(editor.SelectedNode == null && borders.Any(b => ReferenceEquals(b.Tag, replacement)) &&
                  borders.All(b => !ReferenceEquals(b.Tag, oldNode)),
                $"{label}: same-ID Replace removes the old view and does not reuse selected state");
            document.Nodes.Clear();
            document.Nodes.Add(replacement);
            Flush();
            check(canvas.GetVisualDescendants().OfType<Border>().Count(b => b.Tag is WorkflowNode) == 1,
                $"{label}: Reset and reinsertion leave exactly the current node views");
            var next = new WorkflowDocument();
            var nextNode = Node(replacement.Id);
            next.Nodes.Add(nextNode);
            editor.Document = next;
            Flush();
            var edits = 0;
            editor.DocumentEdited += (_, _) => edits++;
            replacement.Title = "old document update";
            Flush();
            check(edits == 0 && editor.SelectedNodes.Count == 0 &&
                  canvas.GetVisualDescendants().OfType<Border>().Single(b => b.Tag is WorkflowNode).Tag == nextNode,
                $"{label}: switching Document releases old editor notifications and node views");
            if (output != null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, $"workflow-optimization-{theme}-{size}-{width}.png"),
                    PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}
