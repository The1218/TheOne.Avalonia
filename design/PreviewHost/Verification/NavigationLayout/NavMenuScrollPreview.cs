using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuScrollPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        var results = new List<string>();
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var motion in new[] { false, true })
        {
            var menu = new NavMenu { ItemsSource = Enumerable.Range(0, 10000).Select(i => $"菜单 {i:00000}").ToArray() };
            var window = new Window { Width = 320, Height = 440, RequestedThemeVariant = variant, Content = menu };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, motion); window.Show(); ThemeAppearancePreview.Flush();
            var label = $"NavMenu/scroll/{variant}/{size}/motion={motion}";
            try
            {
                menu.SelectedItem = menu.Items[0];
                var selected = menu.SelectedItem;
                var grids = new Dictionary<NavMenuItem, Control>(); var reused = 0; var changes = 0;
                var texts = new Dictionary<NavMenuItem, NavMenuSearchText>(); var textReused = true;
                void Observe()
                {
                    foreach (var row in menu.GetRealizedContainers().OfType<NavMenuItem>())
                    {
                        var grid = row.GetVisualDescendants().OfType<ContentPresenter>().FirstOrDefault(p => p.Name == "NavMenuIcon")?.Parent;
                        if (grid is not Control content) continue;
                        if (grids.TryGetValue(row, out var previous)) { if (ReferenceEquals(grid, previous)) reused++; }
                        else { grids[row] = content; row.ResourcesChanged += (_, _) => changes++; }
                        if (row.GetVisualDescendants().OfType<NavMenuSearchText>().FirstOrDefault() is { } text)
                        {
                            if (texts.TryGetValue(row, out var previousText)) textReused &= ReferenceEquals(text, previousText);
                            else texts[row] = text;
                        }
                    }
                }
                Observe();
                for (var i = 0; i < 12; i++) { window.MouseWheel(new Point(150, 200), new Vector(0, -3), RawInputModifiers.None); ThemeAppearancePreview.Flush(); Observe(); }
                changes = 0;
                var prepared = 0; var cleared = 0;
                menu.ContainerPrepared += (_, _) => prepared++;
                menu.ContainerClearing += (_, _) => cleared++;
                var times = new List<double>(); var allocation = GC.GetAllocatedBytesForCurrentThread();
                for (var i = 0; i < 60; i++)
                {
                    var timer = Stopwatch.StartNew();
                    window.MouseWheel(new Point(150, 200), new Vector(0, i < 40 ? -3 : 3), RawInputModifiers.None);
                    ThemeAppearancePreview.Flush(); times.Add(timer.Elapsed.TotalMilliseconds); Observe();
                }
                check(menu.ItemsPanelRoot is VirtualizingStackPanel && menu.GetRealizedContainers().Count() < 60,
                    label + ": native wheel keeps 10,000 items virtualized");
                check(reused > 20 && grids.Count < 80, label + $": recycled rows retain icon/text controls (reused={reused}, rows={grids.Count})");
                check(textReused && texts.Count > 0 && menu.GetRealizedContainers().OfType<NavMenuItem>().All(row =>
                    row.GetVisualDescendants().OfType<NavMenuSearchText>().Any(text => Equals(text.SourceText, menu.ItemFromContainer(row)))),
                    label + ": default text controls are reused and display their current model");
                check(changes <= prepared + cleared, label + $": resource notifications stay within native attachment changes (resources={changes}, prepared={prepared}, cleared={cleared})");
                check(ReferenceEquals(selected, menu.SelectedItem), label + ": scrolling preserves the selected model");
                check(menu.GetRealizedContainers().OfType<NavMenuItem>().All(row => ToolTip.GetTip(row) is ContentControl tip && Equals(tip.Content, menu.ItemFromContainer(row))),
                    label + ": recycled tooltip content follows its current item");
                var current = menu.GetRealizedContainers().OfType<NavMenuItem>().First(row => row.Bounds.Height > 0);
                menu.SelectedItem = menu.ItemFromContainer(current); current.Focus();
                var before = menu.SelectedItem; ThemeAppearancePreview.Press(window, Key.Down);
                check(!Equals(before, menu.SelectedItem), label + ": native keyboard selection works after rapid wheel recycling");
                times.Sort();
                results.Add($"{label}: 60 native wheel frames median={times[times.Count / 2]:F3}ms p95={times[(int)(times.Count * .95)]:F3}ms max={times[^1]:F3}ms; allocation={GC.GetAllocatedBytesForCurrentThread() - allocation}; rows={grids.Count}");
                menu.ItemsSource = new object?[] { "新页面", null, "最后一页" }; ThemeAppearancePreview.Flush();
                check(menu.ContainerFromIndex(1) is NavMenuItem empty && ToolTip.GetTip(empty) is null &&
                    !empty.GetVisualDescendants().OfType<NavMenuSearchText>().Any(text => !string.IsNullOrEmpty(text.Text)),
                    label + ": a null row never displays a recycled label or tooltip");
            }
            finally { window.Close(); }
        }
        if (output is not null) { Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!); File.WriteAllLines(output, results); }
        foreach (var line in results) Console.WriteLine(line);
    }
}
