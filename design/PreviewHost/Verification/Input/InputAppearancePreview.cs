using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class InputAppearancePreview
{
    public static void VerifyTreeViewSpacing(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        {
            var grandchild = new TreeViewItem { Header = "孙节点" };
            var branch = new TreeViewItem { Header = "子分组", IsExpanded = true, Items = { grandchild } };
            var leaf = new TreeViewItem { Header = "子节点" };
            var parent = new TreeViewItem { Header = "工作区 0001", IsExpanded = true, Items = { branch, leaf } };
            var sibling = new TreeViewItem { Header = "工作区 0002" };
            var nodes = new[] { parent, branch, grandchild, leaf, sibling };
            var tree = new TreeView
                { Items = { parent, sibling }, SelectedItem = parent, BorderThickness = new Thickness(1) };
            tree.Bind(TreeView.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
            var window = new Window
            {
                Width = width, Height = 400, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(20), Child = tree }
            };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show();
            Flush();
            var label = $"TreeView/spacing/{theme.Key}/{size}/{width}";

            T Part<T>(TreeViewItem node, string name) where T : Control
            {
                return node.GetVisualDescendants().OfType<T>()
                    .First(c => c.Name == name && ReferenceEquals(c.TemplatedParent, node));
            }

            void Click(Control control)
            {
                var hit =
                    control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!
                        .Value;
                window.MouseMove(hit);
                window.MouseDown(hit, MouseButton.Left);
                window.MouseUp(hit, MouseButton.Left);
                Flush();
            }

            try
            {
                var rows = nodes.Select(node => Part<Border>(node, "PART_LayoutRoot")).ToArray();
                var viewport = tree.GetVisualDescendants().OfType<ScrollContentPresenter>().Single();
                check(rows.All(row =>
                {
                    var point = row.TranslatePoint(default, viewport)!.Value;
                    return point.X >= 4 && viewport.Bounds.Width - point.X - row.Bounds.Width >= 4;
                }), label + ": every depth has the same horizontal inset without accumulating margins");
                check(
                    rows.Zip(rows.Skip(1)).All(pair =>
                        pair.Second.TranslatePoint(default, tree)!.Value.Y -
                        pair.First.TranslatePoint(new Point(0, pair.First.Bounds.Height), tree)!.Value.Y >= 4),
                    label + ": parent, child, grandchild and sibling surfaces have four-pixel gaps");

                double HeaderX(TreeViewItem node)
                {
                    return Part<ContentPresenter>(node, "PART_HeaderPresenter").TranslatePoint(default, tree)!.Value.X;
                }

                tree.TryFindResource("One.TreeViewItemIndent", theme, out var indent);
                check(
                    HeaderX(branch) - HeaderX(parent) == (double)indent! &&
                    HeaderX(grandchild) - HeaderX(branch) == (double)indent,
                    label + ": native level indentation remains unchanged");
                using var bitmap = Render(tree);
                var first = rows[0].TranslatePoint(default, tree)!.Value;
                var y = (int)(first.Y + rows[0].Bounds.Height / 2);
                tree.TryFindResource("One.Surface", theme, out var surface);
                tree.TryFindResource("One.AccentSoft", theme, out var accent);
                check(Pixel(bitmap, (int)first.X - 2, y) == ((ISolidColorBrush)surface!).Color
                      && Pixel(bitmap, (int)(first.X + rows[0].Bounds.Width + 2), y) ==
                      ((ISolidColorBrush)surface).Color
                      && Pixel(bitmap, (int)first.X + 2, y) == ((ISolidColorBrush)accent!).Color,
                    label + ": actual selected pixels leave clear left and right gutters");
                if (output is not null)
                {
                    Directory.CreateDirectory(output);
                    bitmap.Save(Path.Combine(output, $"treeview-{theme.Key}-{size}-{width}.png"),
                        PngBitmapEncoderOptions.Default);
                }

                Click(Part<ContentPresenter>(leaf, "PART_HeaderPresenter"));
                check(ReferenceEquals(tree.SelectedItem, leaf), label + ": native pointer selects a nested node");
                Click(Part<ToggleButton>(parent, "PART_ExpandCollapseChevron"));
                check(!parent.IsExpanded, label + ": native chevron collapses the branch");
                parent.Focus();
                window.KeyPress(Key.Right, RawInputModifiers.None, PhysicalKey.None, null);
                window.KeyRelease(Key.Right, RawInputModifiers.None, PhysicalKey.None, null);
                Flush();
                check(parent.IsExpanded && rows[2].IsEffectivelyVisible,
                    label + ": native Right key restores the expanded descendants");
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    public static void VerifyListBoxSpacing(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var custom in new[] { false, true })
        {
            var list = new ListBox
            {
                ItemsSource = Enumerable.Range(1, 1000).Select(index => $"工作区 {index:D4}").ToArray(),
                SelectionMode = SelectionMode.Multiple | SelectionMode.Toggle, BorderThickness = new Thickness(1)
            };
            if (custom)
                list.ItemTemplate = new FuncDataTemplate<string>((item, _) => new StackPanel
                {
                    Children = { new TextBlock { Text = item }, new TextBlock { Text = "项目成员 · 可多选", FontSize = 11 } }
                });
            var window = new Window
            {
                Width = width, Height = 300, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(20), Child = list }
            };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show();
            Flush();
            list.Selection.Select(0);
            list.Selection.Select(1);
            Flush();
            var label = $"ListBox/spacing/{theme.Key}/{size}/{width}/{(custom ? "custom" : "default")}";
            try
            {
                var items = list.GetRealizedContainers().OfType<ListBoxItem>().ToArray();
                check(items.Length is > 2 and < 60 && items[0].IsSelected && items[1].IsSelected,
                    label + ": native virtualized rows retain multiple selection");
                var viewport = list.GetVisualDescendants().OfType<ScrollContentPresenter>().Single();
                check(items.All(item =>
                {
                    var point = item.TranslatePoint(default, viewport)!.Value;
                    return point.X >= 4 && viewport.Bounds.Width - point.X - item.Bounds.Width >= 4;
                }), label + ": item surfaces leave four pixels on both sides");
                check(items.Zip(items.Skip(1)).All(pair => pair.Second.Bounds.Top - pair.First.Bounds.Bottom >= 4),
                    label + ": vertical gaps remain intact");
                using var bitmap = Render(list);
                var first = items[0].TranslatePoint(default, list)!.Value;
                var y = (int)(first.Y + items[0].Bounds.Height / 2);
                list.TryFindResource("One.Surface", theme, out var surface);
                list.TryFindResource("One.AccentSoft", theme, out var accent);
                check(Pixel(bitmap, (int)first.X - 2, y) == ((ISolidColorBrush)surface!).Color
                      && Pixel(bitmap, (int)(first.X + items[0].Bounds.Width + 2), y) ==
                      ((ISolidColorBrush)surface).Color
                      && Pixel(bitmap, (int)first.X + 2, y) == ((ISolidColorBrush)accent!).Color,
                    label + ": actual selected background pixels are inset from both edges");
                if (output is not null)
                {
                    Directory.CreateDirectory(output);
                    bitmap.Save(
                        Path.Combine(output,
                            $"listbox-{theme.Key}-{size}-{width}-{(custom ? "custom" : "default")}.png"),
                        PngBitmapEncoderOptions.Default);
                }

                var hit = items[2].TranslatePoint(new Point(items[2].Bounds.Width / 2, items[2].Bounds.Height / 2),
                    window)!.Value;
                window.MouseMove(hit);
                window.MouseDown(hit, MouseButton.Left);
                window.MouseUp(hit, MouseButton.Left);
                Flush();
                check(list.Selection.IsSelected(2) && list.Selection.Count == 3,
                    label + ": native pointer toggling retains the previous selections");
                list.ScrollIntoView(999);
                Flush();
                check(
                    list.ContainerFromIndex(999) is not null && list.ContainerFromIndex(0) is null &&
                    list.GetRealizedContainers().Count() < 60,
                    label + ": scrolling still recycles distant rows");
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    public static void VerifyComboBoxSpacing(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        {
            var combo = new ComboBox
            {
                ItemsSource = new[] { "工作区 0001", "工作区 0002", "工作区 0003" }, SelectedIndex = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            var window = new Window
            {
                Width = width, Height = 260, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
                Content = new Border { Padding = new Thickness(20), Child = new StackPanel { Children = { combo } } }
            };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show();
            Flush();
            var label = $"ComboBox/spacing/{theme.Key}/{size}/{width}";
            try
            {
                combo.Focus();
                window.KeyPress(Key.F4, RawInputModifiers.None, PhysicalKey.None, null);
                window.KeyRelease(Key.F4, RawInputModifiers.None, PhysicalKey.None, null);
                Flush();
                var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
                var border = (Border)popup.Child!;
                var items = border.GetVisualDescendants().OfType<ComboBoxItem>().ToArray();
                check(combo.IsDropDownOpen && items.Length == 3, label + ": native keyboard opens the dropdown");
                check(items.All(item =>
                {
                    var point = item.TranslatePoint(default, border)!.Value;
                    return point.X >= 5 && border.Bounds.Width - point.X - item.Bounds.Width >= 5;
                }), label + ": item surfaces leave four pixels inside both popup edges");
                check(items.Zip(items.Skip(1)).All(pair => pair.Second.Bounds.Top - pair.First.Bounds.Bottom >= 4),
                    label + ": vertical item gaps remain intact");
                using var bitmap = Render(border);
                var first = items[0].TranslatePoint(default, border)!.Value;
                var y = (int)(first.Y + items[0].Bounds.Height / 2);
                border.TryFindResource("One.Surface", theme, out var surface);
                border.TryFindResource("One.AccentSoft", theme, out var accent);
                check(Pixel(bitmap, 2, y) == ((ISolidColorBrush)surface!).Color &&
                      Pixel(bitmap, bitmap.PixelSize.Width - 3, y) == ((ISolidColorBrush)surface).Color
                      && Pixel(bitmap, (int)first.X + 2, y) == ((ISolidColorBrush)accent!).Color,
                    label + ": actual pixels separate the selected background from both popup borders");
                if (output is not null)
                {
                    Directory.CreateDirectory(output);
                    bitmap.Save(Path.Combine(output, $"combobox-{theme.Key}-{size}-{width}.png"),
                        PngBitmapEncoderOptions.Default);
                }

                var root = TopLevel.GetTopLevel(items[1])!;
                var hit = items[1]
                    .TranslatePoint(new Point(items[1].Bounds.Width / 2, items[1].Bounds.Height / 2), root)!.Value;
                root.MouseMove(hit);
                root.MouseDown(hit, MouseButton.Left);
                root.MouseUp(hit, MouseButton.Left);
                Flush();
                check(combo.SelectedIndex == 1 && !combo.IsDropDownOpen,
                    label + ": native pointer selection still selects and closes");
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    public static void Verify(Action<bool, string> check, string? output)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var background in new[] { "One.Surface", "One.Subtle" })
            Verify(theme, size, width, background, check, output);
    }

    private static void Verify(ThemeVariant theme, ControlSize size, int width, string background,
        Action<bool, string> check, string? output)
    {
        var text = new TextBox { Text = "输入文本 / Text" };
        var combo = new ComboBox { ItemsSource = new[] { "工作区 0001", "工作区 0002", "工作区 0003" }, SelectedIndex = 0 };
        var inputs = new Control[]
        {
            text, combo, new AutoCompleteBox { Text = "Search" }, new NumericUpDown { Value = 28 },
            new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 16) }, new DatePicker(), new TimePicker()
        };
        var list = new ListBox
        {
            ItemsSource = new[] { "工作区 0001", "工作区 0002", "工作区 0003" }, SelectionMode = SelectionMode.Multiple,
            Height = 170
        };
        list.SelectedItems!.Add("工作区 0001");
        list.SelectedItems.Add("工作区 0002");
        var stack = new StackPanel { Spacing = 8 };
        foreach (var input in inputs) stack.Children.Add(input);
        stack.Children.Add(list);
        var host = new Border { Padding = new Thickness(16), Child = stack };
        host.Bind(Border.BackgroundProperty, new DynamicResourceExtension(background));
        var window = new Window
        {
            Width = width, Height = 800, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None,
            Content = host
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        var label = $"InputAppearance/{theme.Key}/{size}/{width}/{background}";
        try
        {
            host.TryFindResource("One.InputLine", theme, out var line);
            var lineColor = ((ISolidColorBrush)line!).Color;
            foreach (var input in inputs)
            {
                var templated = (TemplatedControl)input;
                check(
                    templated.BorderBrush is ISolidColorBrush brush && brush.Color == lineColor &&
                    templated.BorderThickness.Top == 1,
                    label + "/" + input.GetType().Name + ": the resting input has a semantic one-pixel border");
                using var bitmap = Render(input);
                check(Pixel(bitmap, bitmap.PixelSize.Width / 2, 0) == lineColor,
                    label + "/" + input.GetType().Name + ": the top border is actually painted on the host background");
            }

            var rows = list.GetRealizedContainers().OfType<ListBoxItem>().ToArray();
            check(
                rows.Length == 3 && rows.Zip(rows.Skip(1))
                    .All(pair => pair.Second.Bounds.Top - pair.First.Bounds.Bottom >= 4),
                label + ": adjacent selected list items have four pixels of space");
            var second = rows[1];
            var point = second.TranslatePoint(new Point(30, second.Bounds.Height / 2), window)!.Value;
            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(list.SelectedItems.Count == 1, label + ": spacing preserves native row selection");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, $"inputs-{theme.Key}-{size}-{width}-{background}.png"),
                    PngBitmapEncoderOptions.Default);
            }

            text.Focus();
            window.KeyPress(Key.A, RawInputModifiers.Control, PhysicalKey.None, null);
            window.KeyRelease(Key.A, RawInputModifiers.Control, PhysicalKey.None, null);
            text.SelectAll();
            window.KeyTextInput("Edited");
            Flush();
            check(text.Text == "Edited", label + ": native text input remains editable");
            combo.Focus();
            window.KeyPress(Key.F4, RawInputModifiers.None, PhysicalKey.None, null);
            window.KeyRelease(Key.F4, RawInputModifiers.None, PhysicalKey.None, null);
            Flush();
            check(combo.IsDropDownOpen, label + ": native keyboard opens the dropdown");
            var popup = combo.GetVisualDescendants().OfType<Popup>().Single();
            var items = popup.Child!.GetVisualDescendants().OfType<ComboBoxItem>().ToArray();
            check(
                items.Length == 3 && items.Zip(items.Skip(1))
                    .All(pair => pair.Second.Bounds.Top - pair.First.Bounds.Bottom >= 4),
                label + ": dropdown items keep a gap");
            using var listBitmap = Render(list);
            var gap = rows[0].TranslatePoint(new Point(rows[0].Bounds.Width / 2, rows[0].Bounds.Height + 2), list)!
                .Value;
            list.TryFindResource("One.Surface", theme, out var surface);
            check(Pixel(listBitmap, (int)gap.X, (int)gap.Y) == ((ISolidColorBrush)surface!).Color,
                label + ": the item gap paints the list surface");
            combo.IsDropDownOpen = false;
            Flush();
            window.MouseMove(text.TranslatePoint(new Point(text.Bounds.Width / 2, 1), window)!.Value);
            Flush();
            var border = text.GetVisualDescendants().OfType<Border>().Single(b => b.Name == "PART_BorderElement");
            check(border.BorderBrush is ISolidColorBrush hover && hover.Color.A > 0,
                label + ": pointer feedback retains a visible text border");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static RenderTargetBitmap Render(Control control)
    {
        var bitmap =
            new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)),
                new Vector(96, 96));
        bitmap.Render(control);
        return bitmap;
    }

    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var pixels = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), handle.AddrOfPinnedObject(), pixels.Length,
                bitmap.PixelSize.Width * 4);
        }
        finally
        {
            handle.Free();
        }

        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Bgra8888
            ? Color.FromArgb(pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset])
            : Color.FromArgb(pixels[offset + 3], pixels[offset], pixels[offset + 1], pixels[offset + 2]);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}