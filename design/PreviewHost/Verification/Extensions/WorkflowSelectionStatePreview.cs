using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Workflow;

namespace TheOne.DesignPreview;

internal static class WorkflowSelectionStatePreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 900, 320 })
        {
            var model = new WorkflowEditorDemoViewModel();
            var document = model.Document;
            var node = document.Nodes[0];
            var next = document.Nodes[1];
            var edge = document.Connections[0];
            var editor = new WorkflowEditor
            {
                Document = document, NodeDefinitions = model.NodeDefinitions, ShowNodeLibrary = false,
                ShowProperties = false
            };
            var window = new Window { Width = width, Height = 600, RequestedThemeVariant = theme, Content = editor };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var label = $"Workflow/selection-state/{theme}/{size}/{width}";
            try
            {
                editor.SelectedConnection = edge;
                editor.SelectedNode = node;
                Flush();
                check(
                    editor.SelectedConnection is null && editor.SelectedNodes.SequenceEqual(new[] { node }) &&
                    editor.SelectedNodeEditor?.Node == node,
                    $"{label}: externally selecting a node clears the previous edge and synchronizes the inspector");
                editor.SelectedConnection = edge;
                Flush();
                check(
                    editor.SelectedNode is null && editor.SelectedNodes.Count == 0 && editor.SelectedNodeEditor is null,
                    $"{label}: externally selecting an edge clears node selection and the old inspector");
                CheckNodeChrome(editor, check, label);
                editor.SelectNode(node);
                editor.SelectNode(next, true);
                Flush();
                check(editor.SelectedNodes.Count == 2 && editor.SelectedConnection is null,
                    $"{label}: native additive node selection remains available");
                editor.SelectedConnection = edge;
                Flush();
                check(
                    editor.SelectedNodes.Count == 0 && editor.SelectedNode is null && editor.SelectedNodeEditor is null,
                    $"{label}: edge selection clears every node in the previous multi-selection");
                editor.SelectedNode = new WorkflowNode { Id = node.Id, Title = "Foreign node" };
                Flush();
                check(
                    editor.SelectedNode is null && editor.SelectedNodes.Count == 0 && editor.SelectedNodeEditor is null,
                    $"{label}: a foreign node with a matching ID cannot reuse the old document's selection or inspector");
                editor.SelectedConnection = new WorkflowConnection { Id = edge.Id };
                Flush();
                check(editor.SelectedConnection is null,
                    $"{label}: a foreign connection cannot retain selected-edge state");
                var border = editor.GetVisualDescendants().OfType<Border>().First(b => ReferenceEquals(b.Tag, node));
                var point = border.TranslatePoint(new Point(30, 25), window)!.Value;
                window.MouseMove(point);
                window.MouseDown(point, MouseButton.Left);
                window.MouseUp(point, MouseButton.Left);
                Flush();
                check(
                    editor.SelectedNode == node && editor.SelectedConnection is null && editor.SelectedNodes.Count == 1,
                    $"{label}: real node pointer selection follows the same exclusive selection rules");
                CheckNodeChrome(editor, check, label);
                if (output != null)
                {
                    Directory.CreateDirectory(output);
                    using var frame = window.CaptureRenderedFrame()!;
                    frame.Save(Path.Combine(output, $"workflow-selection-{theme}-{size}-{width}.png"),
                        PngBitmapEncoderOptions.Default);
                }

                editor.Document = new WorkflowDocument();
                Flush();
                check(
                    editor.SelectedNode is null && editor.SelectedConnection is null &&
                    editor.SelectedNodes.Count == 0 && editor.SelectedNodeEditor is null,
                    $"{label}: replacing the document leaves no selected model or stale inspector");
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    private static void CheckNodeChrome(WorkflowEditor editor, Action<bool, string> check, string label)
    {
        foreach (var border in editor.GetVisualDescendants().OfType<Border>().Where(b => b.Tag is WorkflowNode))
        {
            var node = (WorkflowNode)border.Tag!;
            editor.TryFindResource(editor.SelectedNodes.Contains(node) ? "One.Accent" : "One.Line",
                editor.ActualThemeVariant, out var brush);
            check((border.BorderBrush as ISolidColorBrush)?.Color == (brush as ISolidColorBrush)?.Color &&
                  border.BoxShadow.Count > 0 == editor.SelectedNodes.Contains(node),
                $"{label}/{node.Title}: node chrome follows its current selection without stale glow");
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}