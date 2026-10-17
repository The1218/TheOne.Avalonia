using System.Text.Json;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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

internal static class WorkflowEditorPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        Contracts(check);
        DefaultTemplates(check);
        FileCommands(check);
        WorkflowOptimizationPreview.Verify(check, output);
        WorkflowViewportPreview.Verify(check, output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            Visuals(theme, size, check, output);
        Interaction(check);
        Consumer(check, output);
    }

    private static void Contracts(Action<bool, string> check)
    {
        var model = new WorkflowEditorDemoViewModel();
        var doc = model.Document;
        check(doc.Validate().Count == 0 && doc.Nodes.Count == 3 && doc.Connections.Count == 2,
            "Workflow/contracts: host factories create a valid connected document");
        var notify = doc.Nodes[2];
        var recipient = notify.Properties[0];
        check(doc.GetReferenceOptions(notify, recipient).Single().Reference == recipient.Reference,
            "Workflow/contracts: reference choices filter by upstream reachability and type");
        var oldRef = recipient.Reference;
        doc.SetProperty(recipient, "literal@example.com", null);
        doc.Undo();
        check(recipient.Reference == oldRef,
            "Workflow/contracts: literal/reference edits undo with identity preserved");
        doc.Redo();
        check(recipient.Reference == null && recipient.Value == "literal@example.com",
            "Workflow/contracts: redo restores literal mode");
        doc.Undo();
        check(!doc.TryConnect("submit_01", "request", "approval_01", "in", out var duplicate) && duplicate != null,
            "Workflow/contracts: duplicate edges rejected");
        check(!doc.TryConnect("approval_01", "approved", "approval_01", "in", out _),
            "Workflow/contracts: self edges rejected");
        check(!doc.TryConnect("submit_01", "missing", "approval_01", "in", out _),
            "Workflow/contracts: missing endpoints rejected");
        var second = model.NodeDefinitions.Single(d => d.TypeId == "approval").CreateNode();
        doc.AddNode(second);
        check(doc.TryConnect("submit_01", "request", second.Id, "in", out _),
            "Workflow/contracts: unlimited output supports additional branches");
        check(!doc.TryConnect("approval_01", "approved", second.Id, "in", out _),
            "Workflow/contracts: input cardinality enforced");
        var outputIndex = second.Ports.IndexOf(second.Ports.Single(p => p.Id == "approved"));
        second.Ports[outputIndex] = second.Ports[outputIndex] with { DataType = "number" };
        var third = model.NodeDefinitions.Single(d => d.TypeId == "approval").CreateNode();
        doc.AddNode(third);
        check(!doc.TryConnect(second.Id, "approved", third.Id, "in", out _),
            "Workflow/contracts: incompatible port types rejected");
        second.Ports[outputIndex] = second.Ports[outputIndex] with { DataType = "flow" };
        check(doc.TryConnect(second.Id, "approved", third.Id, "in", out _),
            "Workflow/contracts: compatible named ports connect");
        second.Ports.Add(new WorkflowPort("back", "回到", WorkflowPortDirection.Input));
        check(!doc.TryConnect(third.Id, "approved", second.Id, "back", out _),
            "Workflow/contracts: cycles rejected by default");
        doc.TryConnect(third.Id, "approved", second.Id, "back", out _, true);
        check(
            doc.Validate().Any(i => i.Message.Contains("环路")) && doc.Validate(true).All(i => !i.Message.Contains("环路")),
            "Workflow/contracts: host can explicitly allow cycles and validation follows policy");
        doc.Undo();
        var ids = doc.Connections.Select(c => c.Id).ToArray();
        doc.Remove([doc.Nodes[0]]);
        check(doc.Validate().Any(i => i.Message.Contains("引用")),
            "Workflow/contracts: deleting a source exposes dangling references without data loss");
        doc.Undo();
        check(doc.Connections.Select(c => c.Id).SequenceEqual(ids),
            "Workflow/contracts: node deletion restores exact connection order and identity");
        var json = WorkflowSerializer.Save(doc);
        var loaded = WorkflowSerializer.Load(json);
        check(
            loaded.Nodes.Select(n => n.Id).SequenceEqual(doc.Nodes.Select(n => n.Id)) &&
            loaded.Connections.Select(c => c.Id).SequenceEqual(ids),
            "Workflow/persistence: IDs, order and connections round trip");
        check(
            loaded.Nodes[2].Properties[0].Reference == oldRef && loaded.Nodes.All(n => n.Content == null) &&
            !loaded.CanUndo, "Workflow/persistence: references persist; presentation and history do not");
        check(WorkflowSerializer.Save(loaded) == json, "Workflow/persistence: stable versioned JSON round trip");
        var invalidVersion = false;
        try
        {
            WorkflowSerializer.Load(json.Replace("\"version\": 1", "\"version\": 2"));
        }
        catch (JsonException)
        {
            invalidVersion = true;
        }

        check(invalidVersion, "Workflow/persistence: unsupported schema versions rejected");
        model.ExportCommand.Execute(null);
        var saved = model.Document;
        model.Json = "{broken";
        model.LoadJson();
        check(model.Document == saved && model.Message.Contains("失败"),
            "Workflow/consumer: failed load preserves current document");
        model.Json = WorkflowSerializer.Save(saved);
        model.LoadJson();
        check(model.Document != saved && model.Document.Nodes.All(n => n.Content is WorkflowBusinessSummary),
            "Workflow/consumer: loading replaces document and host restores typed Content");
        model.ResolveDataCommand.Execute(null);
        check(model.Message.Contains("user@example.com") && model.Message.Contains("6800"),
            "Workflow/consumer: host resolves stable references from actual supplied outputs");
        loaded.Nodes[0].Title = "external";
        check(!loaded.CanUndo, "Workflow/contracts: host changes reset stale history");
    }

    private static Window Open(Control content, double width = 1200, double height = 700, ThemeVariant? theme = null,
        ControlSize size = ControlSize.Normal)
    {
        var window = new Window
        {
            Content = content, Width = width, Height = height, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme ?? ThemeVariant.Light
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void Visuals(ThemeVariant theme, ControlSize size, Action<bool, string> check, string? output)
    {
        var model = new WorkflowEditorDemoViewModel();
        var editor = new WorkflowEditor { Document = model.Document, NodeDefinitions = model.NodeDefinitions };
        var window = Open(editor, 1280, 600, theme, size);
        try
        {
            editor.FitToView();
            Flush();
            var canvas = editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single();
            var nodes = canvas.GetVisualDescendants().OfType<Border>().Where(b => b.Tag is WorkflowNode).ToArray();
            check(nodes.Length == 3 && nodes.All(n => n.Bounds.Width >= 160 && n.Bounds.Height >= 80),
                $"Workflow/{theme.Key}/{size}: default template creates measured node cards");
            check(nodes.All(n =>
                {
                    var p = n.TranslatePoint(default, canvas)!.Value;
                    var modelNode = (WorkflowNode)n.Tag!;
                    return Math.Abs(p.X - editor.Pan.X - modelNode.X * editor.Zoom) < 1 &&
                           Math.Abs(p.Y - editor.Pan.Y - modelNode.Y * editor.Zoom) < 1;
                }),
                $"Workflow/{theme.Key}/{size}: actual arranged positions follow pan and zoom after layout invalidation (zoom={editor.Zoom}, pan={editor.Pan}, positions={string.Join(';', nodes.Select(n => n.TranslatePoint(default, canvas)))})");
            check(
                nodes.All(n =>
                    new Rect(canvas.Bounds.Size).Contains(
                        n.TranslatePoint(new Point(n.Bounds.Width / 2, n.Bounds.Height / 2), canvas)!.Value)),
                $"Workflow/{theme.Key}/{size}: fit uses actual measured node geometry (canvas={canvas.Bounds}; zoom={editor.Zoom}; pan={editor.Pan}; centers={string.Join(';', nodes.Select(n => n.TranslatePoint(new Point(n.Bounds.Width / 2, n.Bounds.Height / 2), canvas)))} )");
            check(editor.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "提交申请"),
                $"Workflow/{theme.Key}/{size}: compiled default title binding renders");
            editor.SelectNode(model.Document.Nodes[1]);
            Flush();
            check(
                editor.SelectedNode == model.Document.Nodes[1] && editor.GetVisualDescendants().OfType<TextBox>()
                    .Any(t => t.Name == "PART_NodeTitle" && t.Text == editor.SelectedNode.Title),
                $"Workflow/{theme.Key}/{size}: default property template binds selected node");
            var chrome = editor.GetVisualDescendants().Where(v => !v.GetVisualAncestors().Contains(canvas)).ToArray();
            check(
                chrome.OfType<Button>().All(b => ControlSizing.GetSize(b) == ControlSize.Small && b.FontSize == 12) &&
                chrome.OfType<TextBox>().All(t => ControlSizing.GetSize(t) == ControlSize.Small && t.FontSize == 12) &&
                chrome.OfType<TextBlock>().All(t => t.FontSize <= 12),
                $"Workflow/{theme.Key}/{size}: chrome uses small controls and text independently of host density");
            check(ControlSizing.GetSize(canvas) == size && nodes.All(n => ControlSizing.GetSize(n) == size),
                $"Workflow/{theme.Key}/{size}: canvas retains host density and node geometry");
            model.Document.Nodes[1].Title = "更新审批";
            Flush();
            check(editor.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "更新审批"),
                $"Workflow/{theme.Key}/{size}: external model notifications update node templates");
            if (output != null) Save(window, output, $"workflow-{theme.Key}-{size}-1280.png");
            window.Width = 640;
            editor.ShowNodeLibrary = editor.ShowProperties = false;
            Flush();
            editor.FitToView();
            Flush();
            check(canvas.Bounds.Width > 600 && editor.Zoom >= .25 && editor.Zoom <= 2,
                $"Workflow/{theme.Key}/{size}: narrow view folds optional panels and keeps usable canvas");
            if (output != null) Save(window, output, $"workflow-{theme.Key}-{size}-640.png");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Interaction(Action<bool, string> check)
    {
        var model = new WorkflowEditorDemoViewModel();
        var editor = new WorkflowEditor
        {
            Document = model.Document, NodeDefinitions = model.NodeDefinitions, ShowNodeLibrary = false,
            ShowProperties = false
        };
        var window = Open(editor, 1100, 650);
        try
        {
            var canvas = editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single();
            var first = model.Document.Nodes[0];
            check(canvas.GetVisualDescendants().OfType<Border>().Any(b => b.Tag == first),
                $"Workflow/input: initial visible node is mounted (node={first.Id}/{first.X}/{first.Y}, pan={editor.Pan}, zoom={editor.Zoom}, canvas={canvas.Bounds}, views={string.Join(',', canvas.GetVisualDescendants().OfType<Border>().Select(b => b.Tag).OfType<WorkflowNode>().Select(n => n.Id))})");
            var border = canvas.GetVisualDescendants().OfType<Border>().Single(b => b.Tag == first);
            var point = border.TranslatePoint(new Point(30, 20), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseMove(point + new Vector(60, 40));
            window.MouseUp(point + new Vector(60, 40), MouseButton.Left);
            Flush();
            check(first.X == 60 && first.Y == 120 && editor.SelectedNode == first,
                "Workflow/input: actual pointer drag changes logical coordinates and selects model");
            model.Document.Undo();
            Flush();
            check(first.X == 0 && first.Y == 80, "Workflow/input: complete pointer gesture is one undo operation");
            editor.SelectNode(first);
            canvas.Focus();
            Press(window, Key.Right, PhysicalKey.ArrowRight);
            Flush();
            check(first.X == 1, "Workflow/input: actual arrow key moves selected node");
            editor.IsReadOnly = true;
            Press(window, Key.Delete, PhysicalKey.Delete);
            Flush();
            check(
                model.Document.Nodes.Count == 3 && editor.AddNode(model.NodeDefinitions[0], default) == null &&
                !editor.Connect("approval_01", "approved", "notify_01", "in"),
                "Workflow/input: read-only blocks keyboard and public editing actions");
            editor.IsReadOnly = false;
            editor.SelectNode(first);
            editor.CopySelection();
            editor.PasteSelection();
            Flush();
            check(model.Document.Nodes.Count == 4 && editor.SelectedNode?.Id != first.Id,
                "Workflow/input: clipboard clones nodes with fresh stable identities");
            Press(window, Key.Delete, PhysicalKey.Delete);
            Flush();
            check(model.Document.Nodes.Count == 3, "Workflow/input: Delete removes selected clone");
            model.Document.Undo();
            Flush();
            check(model.Document.Nodes.Count == 4, "Workflow/input: deletion can be undone");
            var clone = model.Document.Nodes.Last();
            clone.Ports.Add(new WorkflowPort("extra", "额外输入", WorkflowPortDirection.Input));
            Flush();
            // Start a keyboard-accessible connection with native port buttons.
            var output = canvas.GetVisualDescendants().OfType<Button>()
                .Single(b => AutomationProperties.GetName(b) == "主管审批 通过 Output");
            var input = canvas.GetVisualDescendants().OfType<Button>().Single(b =>
                AutomationProperties.GetName(b) == $"{clone.Title} 额外输入 Input" &&
                b.GetVisualAncestors().OfType<Border>().Any(parent => parent.Tag == clone));
            output.Focus();
            Press(window, Key.Enter, PhysicalKey.Enter);
            input.Focus();
            Press(window, Key.Enter, PhysicalKey.Enter);
            Flush();
            check(model.Document.Connections.Any(c => c.TargetNodeId == clone.Id),
                "Workflow/input: native keyboard port activation creates connection");
            var added = editor.AddNode(model.NodeDefinitions.Single(d => d.TypeId == "approval"), new Point(600, 340))!;
            Flush();

            Button Port(WorkflowNode n, string title, string direction)
            {
                return canvas.GetVisualDescendants().OfType<Button>().Single(b =>
                    AutomationProperties.GetName(b) == $"{n.Title} {title} {direction}" &&
                    b.GetVisualAncestors().OfType<Border>().Any(parent => parent.Tag == n));
            }

            Point WindowPoint(Control c)
            {
                return c.TranslatePoint(new Point(c.Bounds.Width / 2, c.Bounds.Height / 2), window)!.Value;
            }

            var from = WindowPoint(Port(first, "申请数据", "Output"));
            var to = WindowPoint(Port(added, "输入", "Input"));
            window.MouseMove(from);
            window.MouseDown(from, MouseButton.Left);
            window.MouseMove(to);
            window.MouseUp(to, MouseButton.Left);
            Flush();
            check(model.Document.Connections.Any(c => c.SourceNodeId == first.Id && c.TargetNodeId == added.Id),
                $"Workflow/input: actual output-to-input pointer drag creates named-port connection (status={editor.Status}, from={from}, to={to})");
            editor.ShowNodeLibrary = true;
            Flush();
            editor.SearchText = "审批";
            Flush();
            check(
                editor.LibraryGroups.SelectMany(g => g.Items)
                    .All(d => d.Title.Contains("审批") || d.Description.Contains("审批")),
                "Workflow/input: library search filters registered node definitions");
            editor.SearchText = "";
            Flush();
            var palette = editor.GetVisualDescendants().OfType<Button>()
                .Single(b => b.Tag is WorkflowNodeDefinition d && d.TypeId == "request");
            var palettePoint = WindowPoint(palette);
            var drop = canvas.TranslatePoint(new Point(200, 250), window)!.Value;
            var beforeAdd = model.Document.Nodes.Count;
            window.MouseMove(palettePoint);
            window.MouseDown(palettePoint, MouseButton.Left);
            window.MouseMove(drop);
            window.MouseUp(drop, MouseButton.Left);
            Flush();
            check(model.Document.Nodes.Count == beforeAdd + 1,
                "Workflow/input: actual palette drag invokes host factory at the canvas location");
            var oldDocument = model.Document;
            var replacement = new WorkflowEditorDemoViewModel().Document;
            editor.Document = replacement;
            Flush();
            var count = canvas.GetVisualDescendants().OfType<Border>().Count(b => b.Tag is WorkflowNode);
            oldDocument.AddNode(model.NodeDefinitions[0].CreateNode());
            Flush();
            check(
                count == 3 && canvas.GetVisualDescendants().OfType<Border>().Count(b => b.Tag is WorkflowNode) == 3 &&
                editor.SelectedNode == null,
                "Workflow/lifecycle: document replacement detaches old model and clears selection");
            editor.IsReadOnly = true;
            var viewCount = canvas.GetVisualDescendants().OfType<Border>().Count(b => b.Tag is WorkflowNode);
            editor.ApplyTemplate();
            Flush();
            check(
                viewCount == 3 && canvas.GetVisualDescendants().OfType<Border>().Count(b => b.Tag is WorkflowNode) == 3,
                "Workflow/lifecycle: template reuse does not duplicate node presenters");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Consumer(Action<bool, string> check, string? output)
    {
        var demo = new WorkflowEditorDemoView();
        var window = Open(demo, 1600, 1100, size: ControlSize.Large);
        try
        {
            var editor = demo.FindControl<WorkflowEditor>("Editor")!;
            editor.FitToView();
            Flush();
            check(
                Math.Abs(editor.Bounds.Height + demo.FindControl<Expander>("ConsumerExamples")!.Bounds.Height + 14 -
                         demo.Bounds.Height) < 1 && double.IsNaN(editor.Height),
                "Workflow/demo: editor fills the finite parent without a fixed height");
            var save = editor.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "SaveWorkflowButton");
            check(
                save.Command == demo.Model.SaveCommand && editor.SaveCommand == demo.Model.SaveCommand &&
                save.CommandParameter == demo.Model.Document,
                "Workflow/demo: save binds the host ViewModel command and current document");
            check(
                ReferenceEquals(editor.Document, demo.Model.Document) && editor.GetVisualDescendants()
                    .OfType<TextBlock>().Any(t => t.Text == "提交采购申请"),
                "Workflow/demo: compiled document and typed Content bindings use consumer data");
            editor.SelectNode(demo.Model.Document.Nodes.Single(n => n.Id == "approval_01"));
            Flush();
            check(
                demo.Model.SelectedNode == editor.SelectedNode &&
                demo.Model.Fields.Any(f => f.Property.Key == "roleId"),
                "Workflow/demo: compiled two-way selection builds consumer property editors");
            var title = demo.FindControl<TextBox>("NodeTitleInput") ??
                        editor.GetVisualDescendants().OfType<TextBox>().First(t => t.Text == "主管审批");
            check(title.FontSize == 12 && title.Bounds.Height == 30,
                "Workflow/demo: inspector input uses small text and height in a large host");
            title.Focus();
            title.SelectAll();
            window.KeyTextInput("用户配置审批");
            Flush();
            demo.Model.ApplyTitleCommand.Execute(null);
            Flush();
            check(editor.SelectedNode!.Title == "用户配置审批",
                "Workflow/demo: real text input updates host configuration through compiled binding");
            var row = demo.Model.Fields.Single(f => f.Property.Key == "amount");
            row.SelectedOption = row.Options.First();
            row.Literal = "1200";
            Flush();
            check(
                editor.SelectedNode.Properties.Single(p => p.Key == "amount").Reference == null &&
                row.Literal == "1200",
                "Workflow/demo: consumer changes reference to literal and writes the same public model");
            demo.Model.Document.Undo();
            demo.Model.Document.Undo();
            Flush();
            check(row.Property.Reference != null && row.SelectedOption?.Reference == row.Property.Reference,
                "Workflow/demo: undo synchronizes reference picker and property model");
            var tabs = editor.GetVisualDescendants().OfType<TabStrip>().Single();
            var appearance = tabs.GetVisualDescendants().OfType<TabStripItem>().Single(t => t.Name == "AppearanceTab");
            var tabPoint =
                appearance.TranslatePoint(new Point(appearance.Bounds.Width / 2, appearance.Bounds.Height / 2), window)!
                    .Value;
            window.MouseDown(tabPoint, MouseButton.Left);
            window.MouseUp(tabPoint, MouseButton.Left);
            Flush();
            var panels = editor.GetVisualDescendants().OfType<StackPanel>()
                .Where(p => p.Name is "BasicPanel" or "RulesPanel" or "AppearancePanel").ToArray();
            check(
                tabs.SelectedIndex == 2 && panels.Single(p => p.Name == "AppearancePanel").IsVisible &&
                panels.Count(p => p.IsVisible) == 1,
                "Workflow/demo: native TabStrip pointer selection switches compiled panel bindings");
            appearance.Focus();
            Press(window, Key.Left, PhysicalKey.ArrowLeft);
            Flush();
            check(tabs.SelectedIndex == 1 && panels.Single(p => p.Name == "RulesPanel").IsVisible,
                "Workflow/demo: native TabStrip keyboard navigation switches property panels");
            tabs.SelectedIndex = 2;
            Flush();
            var oversizedButtons = demo.GetVisualDescendants().OfType<Button>().Where(b =>
                !b.GetVisualAncestors().OfType<WorkflowCanvas>().Any() &&
                (ControlSizing.GetSize(b) != ControlSize.Small || b.FontSize != 12)).ToArray();
            check(
                tabs.FontSize == 12 && ControlSizing.GetSize(tabs) == ControlSize.Small && oversizedButtons.Length == 0,
                $"Workflow/demo: header, inspector tabs, fields and buttons stay small in a large host (tabs={tabs.FontSize}/{ControlSizing.GetSize(tabs)}; buttons={string.Join(';', oversizedButtons.Select(b => $"{b.GetType().Name}:{b.Name}:{b.Content}:{b.FontSize}/{ControlSizing.GetSize(b)}"))})");
            check(!demo.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text?.Contains("组件状态与交互细节") == true),
                "Workflow/demo: obsolete design specification board is removed");
            var toggle = editor.GetVisualDescendants().OfType<CheckBox>().Single(c => c.Name == "CustomTemplateToggle");
            toggle.IsChecked = true;
            Flush();
            check(editor.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "approval"),
                "Workflow/demo: custom node template receives WorkflowNode and renders TypeId");
            toggle.IsChecked = false;
            Flush();
            demo.Model.ExportCommand.Execute(null);
            demo.Model.LoadCommand.Execute(null);
            Flush();
            check(
                editor.Document == demo.Model.Document && editor.Document.Nodes.Single(n => n.Id == "approval_01")
                    .Properties.Single(p => p.Key == "amount").Reference?.NodeId == "submit_01",
                "Workflow/demo: JSON loading replaces bound document and preserves data associations");
            demo.FindControl<Expander>("ConsumerExamples")!.IsExpanded = true;
            Flush();
            var association = demo.FindControl<WorkflowEditor>("AssociationEditor")!;
            check(
                association.Document == demo.AssociationModel.Document && association.GetVisualDescendants()
                    .OfType<TextBlock>().Any(t => t.Text == "业务数据源"),
                "Workflow/demo: independent data sample uses default node template and typed custom ContentTemplate");
            check(
                demo.GetVisualDescendants().OfType<WorkflowCanvas>()
                    .All(c => ControlSizing.GetSize(c) == ControlSize.Large) &&
                demo.FindControl<TextBox>("JsonInput")!.FontSize == 12,
                "Workflow/demo: expanded consumer UI is small while all canvases retain host density");
        }
        finally
        {
            window.Close();
            Flush();
        }

        if (output != null)
        {
            var fresh = new WorkflowEditorDemoView();
            var design = Open(fresh, 1536, 1040);
            try
            {
                Flush();
                Save(design, output, "workflow-consumer-1536.png");
            }
            finally
            {
                design.Close();
                Flush();
            }
        }
    }

    private static void DefaultTemplates(Action<bool, string> check)
    {
        var model = new WorkflowEditorDemoViewModel();
        var command = new SaveProbe();
        var editor = new WorkflowEditor
            { Document = model.Document, NodeDefinitions = model.NodeDefinitions, SaveCommand = command };
        var window = Open(editor, 1100);
        try
        {
            check(
                editor.HeaderTemplate != null && editor.NodeTemplate != null && editor.PropertyTemplate != null &&
                editor.NodeDefinitionTemplate != null,
                "Workflow/defaults: complete templates are available without host templates");
            var save = editor.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_Save");
            var point = save.TranslatePoint(new Point(save.Bounds.Width / 2, save.Bounds.Height / 2), window)!.Value;
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(command.Calls == 1 && command.Parameter == model.Document,
                "Workflow/defaults: native save button executes host command with the document");
            command.Enabled = false;
            command.Raise();
            Flush();
            check(!save.IsEffectivelyEnabled, "Workflow/defaults: save respects ICommand.CanExecute");
            editor.SelectNode(model.Document.Nodes[1]);
            Flush();
            var title = editor.GetVisualDescendants().OfType<TextBox>().Single(t => t.Name == "PART_NodeTitle");
            var old = editor.SelectedNode!.Title;
            title.Focus();
            title.SelectAll();
            window.KeyTextInput("默认模板编辑");
            Flush();
            check(editor.SelectedNode.Title == "默认模板编辑",
                "Workflow/defaults: real input edits the selected node without host event handlers");
            model.Document.Undo();
            Flush();
            check(title.Text == old, "Workflow/defaults: title binding follows document undo");
            var fields = editor.SelectedNodeEditor!.Fields;
            var amount = fields.Single(f => f.Property.Key == "amount");
            var reference = amount.Property.Reference;
            amount.SelectedOption = amount.Options[0];
            amount.Literal = "2500";
            Flush();
            check(amount.Property.Reference == null && amount.Property.Value == "2500",
                "Workflow/defaults: built-in field editors update literals and data associations");
            model.Document.Undo();
            model.Document.Undo();
            Flush();
            check(amount.SelectedOption?.Reference == reference && amount.Property.Reference == reference,
                "Workflow/defaults: data reference picker follows undo");
            editor.IsReadOnly = true;
            Flush();
            check(!title.IsEffectivelyEnabled, "Workflow/defaults: read-only disables built-in property input");
            editor.HeaderTemplate = new FuncDataTemplate<WorkflowEditor>((_, _) => new TextBlock { Text = "自定义标题栏" });
            Flush();
            check(
                editor.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "自定义标题栏") &&
                editor.Document == model.Document,
                "Workflow/defaults: header template can be replaced without replacing data");
            window.Width = 900;
            window.Height = 340;
            Flush();
            check(
                editor.Bounds.Size == window.ClientSize &&
                editor.GetVisualDescendants().OfType<WorkflowCanvas>().Single().Bounds.Height > 0,
                "Workflow/layout: default editor stretches with a resized parent below the former minimum height");
            editor.Document = new WorkflowDocument();
            Flush();
            check(editor.SelectedNodeEditor == null,
                "Workflow/defaults: document replacement clears the old inspector adapter");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void FileCommands(Action<bool, string> check)
    {
        var files = new WorkflowFilesProbe();
        var model = new WorkflowEditorDemoViewModel(true, files);
        var save = (WorkflowFileCommand)model.SaveCommand;
        var open = (WorkflowFileCommand)model.OpenCommand;
        save.ExecuteAsync().GetAwaiter().GetResult();
        check(
            model.SaveStateText == "已保存" &&
            WorkflowSerializer.Load(files.Saved!).Nodes.Count == model.Document.Nodes.Count,
            "Workflow/files: SaveCommand serializes through an injected service and records the saved revision");
        model.Document.Nodes[0].Title = "取消保存";
        files.Result = false;
        var oldJson = model.Json;
        save.ExecuteAsync().GetAwaiter().GetResult();
        check(model.SaveStateText != "已保存" && model.Json == oldJson,
            "Workflow/files: canceled save does not mark edits saved");
        files.Pending = new TaskCompletionSource<bool>();
        var calls = files.Calls;
        var pending = save.ExecuteAsync();
        save.Execute(null);
        check(!save.CanExecute(null) && files.Calls == calls + 1,
            "Workflow/files: asynchronous save disables the command and rejects duplicate execution");
        model.Document.Nodes[0].Title = "保存期间继续编辑";
        files.Pending.SetResult(true);
        Flush();
        check(pending.IsCompletedSuccessfully && save.CanExecute(null) && model.SaveStateText != "已保存",
            "Workflow/files: completing an older snapshot leaves subsequent changes dirty");
        files.Pending = null;
        files.Error = new IOException("模拟写入失败");
        save.ExecuteAsync().GetAwaiter().GetResult();
        check(model.Message.Contains("模拟写入失败") && save.CanExecute(null),
            "Workflow/files: save failures are observed and restore command availability");
        files.Opened = files.Saved;
        var before = model.Document;
        open.ExecuteAsync().GetAwaiter().GetResult();
        check(model.Document != before && model.Document.Nodes.All(n => n.Content != null),
            "Workflow/files: OpenCommand loads data and restores host presentation");
        before = model.Document;
        files.Opened = "{broken";
        open.ExecuteAsync().GetAwaiter().GetResult();
        check(model.Document == before && model.Message.Contains("失败"),
            "Workflow/files: invalid files preserve the current document");
        files.Opened = null;
        open.ExecuteAsync().GetAwaiter().GetResult();
        check(model.Document == before, "Workflow/files: canceled open preserves the current document");
    }

    private static void Press(Window window, Key key, PhysicalKey physical)
    {
        window.KeyPress(key, RawInputModifiers.None, physical, null);
        window.KeyRelease(key, RawInputModifiers.None, physical, null);
        Flush();
    }

    private static void Save(Window window, string output, string name)
    {
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name), PngBitmapEncoderOptions.Default);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class SaveProbe : ICommand
    {
        public bool Enabled { get; set; } = true;
        public int Calls { get; private set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return Enabled;
        }

        public void Execute(object? parameter)
        {
            Calls++;
            Parameter = parameter;
        }

        public void Raise()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class WorkflowFilesProbe : IWorkflowEditorFileService
    {
        public int Calls;
        public Exception? Error;
        public TaskCompletionSource<bool>? Pending;
        public bool Result = true;
        public string? Saved, Opened;

        public Task<bool> SaveAsync(string json)
        {
            Calls++;
            Saved = json;
            return Error != null ? Task.FromException<bool>(Error) : Pending?.Task ?? Task.FromResult(Result);
        }

        public Task<string?> OpenAsync()
        {
            return Task.FromResult(Opened);
        }
    }
}
