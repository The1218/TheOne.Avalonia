using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
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

internal static class WorkflowViewportPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1100, 480 })
            DefaultViews(theme, size, width, check, output);
        DefaultCacheEviction(check);
        CustomEditorState(check);
        ActiveGestures(check);
        CrossingEdges(check, output);
    }

    private static WorkflowNode Node(string id, double x, double y = 40)
    {
        var node = new WorkflowNode { Id = id, Title = id, X = x, Y = y };
        node.Ports.Add(new("in", "Input", WorkflowPortDirection.Input, "flow", 0));
        node.Ports.Add(new("out", "Output", WorkflowPortDirection.Output, "flow", 0));
        return node;
    }

    private static WorkflowEditor Editor(WorkflowDocument document) => new()
    {
        Document = document, ShowNodeLibrary = false, ShowProperties = false,
        Pan = new Vector(24, 24)
    };

    private static Window Open(WorkflowEditor editor, int width = 1100,
        ThemeVariant? theme = null, ControlSize size = ControlSize.Normal)
    {
        var window = new Window
        {
            Width = width, Height = 620, Content = editor,
            RequestedThemeVariant = theme ?? ThemeVariant.Light
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        return window;
    }

    private static WorkflowCanvas Canvas(WorkflowEditor editor) =>
        editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single();

    private static Border[] Views(WorkflowCanvas canvas) => canvas.GetVisualDescendants().OfType<Border>()
        .Where(b => b.Tag is WorkflowNode).ToArray();

    private static void DefaultViews(ThemeVariant theme, ControlSize size, int width,
        Action<bool, string> check, string? output)
    {
        var document = new WorkflowDocument();
        for (var i = 0; i < 240; i++) document.Nodes.Add(Node($"viewport-{i}", i * 260));
        var editor = Editor(document);
        var window = Open(editor, width, theme, size);
        var canvas = Canvas(editor);
        var label = $"Workflow/viewport/{theme}/{size}/{width}";
        try
        {
            check(Views(canvas).Length is > 0 and < 60 && document.Nodes.Count == 240,
                $"{label}: default node views are bounded by the viewport while the full document stays intact");
            var chosen = document.Nodes[180];
            editor.SelectNode(chosen);
            Flush();
            check(editor.SelectedNodes.SequenceEqual(new[] { chosen }),
                $"{label}: selecting an offscreen model does not depend on an attached view");
            foreach (var zoom in new[] { .25, 1, 2 })
            {
                editor.Zoom = zoom;
                editor.Pan = new Vector(24 - chosen.X * zoom, 24 - chosen.Y * zoom);
                Flush();
                var views = Views(canvas);
                var selected = views.Single(b => ReferenceEquals(b.Tag, chosen));
                var point = selected.TranslatePoint(default, canvas)!.Value;
                check(views.Length < 60 && Math.Abs(point.X - 24) < 1 && Math.Abs(point.Y - 24) < 1 &&
                      selected.BoxShadow.Count > 0,
                    $"{label}/{zoom}: recycled views restore their world position and selected appearance");
            }

            editor.Zoom = 1;
            for (var pass = 0; pass < 3; pass++)
            {
                editor.Pan = new Vector(24, 24);
                Flush();
                check(Views(canvas).Length < 60 && Views(canvas).Any(b => ReferenceEquals(b.Tag, document.Nodes[0])) &&
                      editor.SelectedNode == chosen,
                    $"{label}/pan-{pass}: returning to the start leaves bounded views and retains offscreen selection");
                editor.Pan = new Vector(24 - chosen.X, 24 - chosen.Y);
                Flush();
            }

            var replacement = Node(chosen.Id, chosen.X, chosen.Y);
            document.Nodes[180] = replacement;
            Flush();
            check(editor.SelectedNode == null && Views(canvas).All(b => !ReferenceEquals(b.Tag, chosen)) &&
                  Views(canvas).Any(b => ReferenceEquals(b.Tag, replacement)),
                $"{label}: replacing a recycled model with the same ID drops old views and selection");
            if (output != null) Save(window, output, $"workflow-viewport-{theme}-{size}-{width}.png");

            document.Nodes.Clear();
            document.Nodes.Add(Node("reset", 0));
            editor.Pan = new Vector(24, 24);
            Flush();
            check(Views(canvas).Select(b => ((WorkflowNode)b.Tag!).Id).SequenceEqual(new[] { "reset" }),
                $"{label}: Reset removes both attached and retained old views");
            var next = new WorkflowDocument();
            next.Nodes.Add(Node("replacement-document", 0));
            editor.Document = next;
            Flush();
            check(Views(canvas).Select(b => ((WorkflowNode)b.Tag!).Id)
                    .SequenceEqual(new[] { "replacement-document" }),
                $"{label}: switching Document drops the old viewport and geometry state");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void DefaultCacheEviction(Action<bool, string> check)
    {
        var document = new WorkflowDocument();
        for (var i = 0; i < 90; i++) document.Nodes.Add(Node($"eviction-{i}", i * 500));
        var first = document.Nodes[0];
        var editor = Editor(document);
        var window = Open(editor);
        try
        {
            var canvas = Canvas(editor);
            editor.SelectedNode = first;
            Flush();
            var original = Views(canvas).Single(b => ReferenceEquals(b.Tag, first));
            var originalHeight = original.Bounds.Height;
            var visited = true;
            for (var i = 1; i <= 80; i++)
            {
                var current = document.Nodes[i];
                editor.Pan = new Vector(24 - current.X, 24 - current.Y);
                Flush();
                visited &= Views(canvas).Any(b => ReferenceEquals(b.Tag, current));
            }

            check(visited && TopLevel.GetTopLevel(original) == null && editor.SelectedNode == first,
                "Workflow/viewport/eviction: visiting over 64 distinct default nodes detaches the old view without clearing model selection");
            first.Title = string.Concat(Enumerable.Repeat("Updated offscreen title wraps into multiple measured lines. ", 12));
            first.Width = 340;
            Flush();
            editor.Pan = new Vector(24, 24);
            Flush();
            var returned = Views(canvas).Single(b => ReferenceEquals(b.Tag, first));
            check(!ReferenceEquals(returned, original) && TopLevel.GetTopLevel(original) == null &&
                  editor.SelectedNode == first && returned.BoxShadow.Count > 0,
                "Workflow/viewport/eviction: returning after cold-cache eviction creates a new view for the same selected model");
            check(Math.Abs(returned.Bounds.Width - first.Width) < 1 &&
                  returned.Bounds.Height > originalHeight + 40 &&
                  returned.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == first.Title),
                "Workflow/viewport/eviction: offscreen title and Width changes discard stale dimensions and render the newly measured content");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void CustomEditorState(Action<bool, string> check)
    {
        var document = new WorkflowDocument();
        var node = Node("editable", 0);
        node.Content = new DraftContent();
        document.Nodes.Add(node);
        var editor = Editor(document);
        var nodeBuilds = 0;
        var contentBuilds = 0;
        editor.NodeTemplate = new FuncDataTemplate<WorkflowNode>((item, _) =>
        {
            nodeBuilds++;
            return new TextBlock { Text = item!.Title };
        });
        editor.ContentTemplate = new FuncDataTemplate<DraftContent>((_, _) =>
        {
            contentBuilds++;
            return new TextBox { Text = "local draft", Name = "ViewportDraft" };
        });
        var window = Open(editor);
        try
        {
            var canvas = Canvas(editor);
            var border = Views(canvas).Single();
            var input = canvas.GetVisualDescendants().OfType<TextBox>().Single(t => t.Name == "ViewportDraft");
            input.Focus();
            input.SelectionStart = input.SelectionEnd = input.Text!.Length;
            window.KeyTextInput(" edited");
            Flush();
            input.SelectionStart = 2;
            input.SelectionEnd = 7;
            var originalText = input.Text;
            check(input.CanUndo, "Workflow/viewport/state: native input records undo before viewport changes");
            var originalBuilds = (nodeBuilds, contentBuilds);
            editor.SelectedNode = node;
            editor.Pan = new Vector(-6000, 24);
            Flush();
            check(input.IsFocused && TopLevel.GetTopLevel(input) == window &&
                  ReferenceEquals(Views(canvas).Single(), border),
                "Workflow/viewport/state: focused custom editor remains attached when its node leaves the viewport");
            editor.Pan = new Vector(24, 24);
            Flush();
            check(ReferenceEquals(canvas.GetVisualDescendants().OfType<TextBox>()
                          .Single(t => t.Name == "ViewportDraft"), input) && input.Text == originalText &&
                      input.SelectionStart == 2 && input.SelectionEnd == 7 &&
                      originalBuilds == (nodeBuilds, contentBuilds) && editor.SelectedNode == node,
                "Workflow/viewport/state: custom templates retain the same controls, draft, caret selection and model selection");
            canvas.Focus();
            editor.Pan = new Vector(-6000, 24);
            Flush();
            editor.Pan = new Vector(24, 24);
            Flush();
            check(ReferenceEquals(Views(canvas).Single(), border) && input.Text == originalText &&
                  originalBuilds == (nodeBuilds, contentBuilds),
                "Workflow/viewport/state: an unfocused stateful custom template is retained without rebuilding its content");
            input.Focus();
            var undo = Application.Current!.PlatformSettings!.HotkeyConfiguration.Undo.First();
            var undoModifiers = undo.KeyModifiers.HasFlag(KeyModifiers.Meta)
                ? RawInputModifiers.Meta : RawInputModifiers.Control;
            window.KeyPress(undo.Key, undoModifiers, PhysicalKey.Z, "z");
            window.KeyRelease(undo.Key, undoModifiers, PhysicalKey.Z, "z");
            Flush();
            check(input.Text == "local draft",
                $"Workflow/viewport/state: native text undo history survives an offscreen round trip (text={input.Text})");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void ActiveGestures(Action<bool, string> check)
    {
        var document = new WorkflowDocument();
        var node = Node("gesture", 0);
        document.Nodes.Add(node);
        var editor = Editor(document);
        var window = Open(editor);
        try
        {
            var canvas = Canvas(editor);
            var border = Views(canvas).Single();
            var port = canvas.GetVisualDescendants().OfType<Button>()
                .Single(b => AutomationProperties.GetName(b) == $"{node.Title} Output Output");
            port.Focus();
            editor.Pan = new Vector(-6000, 24);
            Flush();
            check(port.IsFocused && Views(canvas).Any(b => ReferenceEquals(b, border)),
                "Workflow/viewport/focus: a focused native port keeps its default node attached offscreen");
            editor.Pan = new Vector(24, 24);
            Flush();
            window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
            canvas.Focus();
            editor.Pan = new Vector(-6000, 24);
            Flush();
            check(Views(canvas).Any(b => ReferenceEquals(b, border)),
                "Workflow/viewport/connect: a pending keyboard connection keeps its source node attached offscreen");
            window.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
            editor.Pan = new Vector(24, 24);
            Flush();
            border = Views(canvas).Single();
            var point = border.TranslatePoint(new Point(35, 22), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            editor.Pan = new Vector(-6000, 24);
            Flush();
            check(Views(canvas).Any(b => ReferenceEquals(b, border)),
                "Workflow/viewport/gesture: a node participating in an active captured drag stays attached offscreen");
            window.MouseMove(point + new Vector(40, 20));
            window.MouseUp(point + new Vector(40, 20), MouseButton.Left);
            Flush();
            check(node.X == 40 && node.Y == 60 && document.CanUndo,
                "Workflow/viewport/gesture: leaving the viewport does not cancel the captured drag or its undo operation");
            document.Undo();
            editor.Pan = new Vector(24, 24);
            Flush();
            border = Views(canvas).Single();
            point = border.TranslatePoint(new Point(35, 22), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Right);
            window.MouseUp(point, MouseButton.Right);
            Flush();
            var menu = border.ContextMenu!;
            check(menu.IsOpen, "Workflow/viewport/menu: native pointer opens the node context menu");
            editor.Pan = new Vector(-6000, 24);
            Flush();
            check(menu.IsOpen && Views(canvas).Any(b => ReferenceEquals(b, border)),
                "Workflow/viewport/menu: an open context menu keeps its offscreen node attached");
            menu.Close();
            canvas.Focus();
            Flush();
            editor.Pan = new Vector(24, 24);
            Flush();
            check(Views(canvas).Single().Tag == node && editor.SelectedNode == node,
                "Workflow/viewport/menu: closing the menu and returning restores the current model without stale selection");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void CrossingEdges(Action<bool, string> check, string? output)
    {
        var document = new WorkflowDocument();
        var source = Node("offscreen-source", -2000);
        var target = Node("offscreen-target", 2200);
        document.Nodes.Add(source);
        document.Nodes.Add(target);
        var edge = new WorkflowConnection
        {
            SourceNodeId = source.Id, SourcePortId = "out", TargetNodeId = target.Id, TargetPortId = "in",
            Label = "Visible crossing edge"
        };
        document.Connections.Add(edge);
        var editor = Editor(document);
        editor.Pan = new Vector(24 - source.X, 24);
        var window = Open(editor);
        try
        {
            var canvas = Canvas(editor);
            editor.Pan = new Vector(24 - target.X, 24);
            Flush();
            var input = canvas.GetVisualDescendants().OfType<Button>()
                .Single(b => AutomationProperties.GetName(b) == $"{target.Title} Input Input");
            var targetAnchor = canvas.ToWorld(input.TranslatePoint(new Point(input.Bounds.Width / 2,
                input.Bounds.Height / 2), canvas)!.Value);
            editor.Pan = new Vector(24, 24);
            Flush();
            var point = canvas.TranslatePoint(new Point(canvas.Bounds.Width / 2, targetAnchor.Y + editor.Pan.Y), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(editor.SelectedConnection == edge,
                "Workflow/viewport/edges: a visible edge between two offscreen nodes remains selectable");
            var beforeLabelChange = Frame(window);
            edge.Label = "Updated cached edge label";
            Flush();
            check(!beforeLabelChange.SequenceEqual(Frame(window)),
                "Workflow/viewport/cache: changing an offscreen edge label refreshes visible cached text");
            if (output != null) Save(window, output, "workflow-viewport-crossing-edge.png");
            target.Ports[0] = target.Ports[0] with { Id = "replacement-in" };
            Flush();
            editor.SelectedConnection = null;
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(editor.SelectedConnection == null && document.Validate().Any(i => i.Message == "连线端点不存在"),
                "Workflow/viewport/edges: replacing an offscreen port invalidates cached endpoints");
            target.Ports[0] = target.Ports[0] with { Id = "in" };
            source.X = 0;
            target.X = 1200;
            editor.FitToView();
            Flush();
            var views = Views(canvas);
            check(views.Length == 2 && views.All(b =>
                new Rect(canvas.Bounds.Size).Contains(b.TranslatePoint(new Point(b.Bounds.Width / 2, b.Bounds.Height / 2), canvas)!.Value)),
                "Workflow/viewport/bounds: FitToView includes models whose node views were previously offscreen");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Save(Window window, string output, string name)
    {
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name), PngBitmapEncoderOptions.Default);
    }

    private static byte[] Frame(Window window)
    {
        using var frame = window.CaptureRenderedFrame()!;
        using var stream = new MemoryStream();
        frame.Save(stream, PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class DraftContent { }
}
