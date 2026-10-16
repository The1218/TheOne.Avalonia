using System.Collections;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

/// <summary>Own-container reinsert coverage for the other native virtualized item surfaces.</summary>
internal static class VirtualizedItemsReinsertPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var name in new[] { "ListBox", "ComboBox", "ItemsControl", "TabStrip" })
        foreach (var storage in new[] { "direct", "source-own", "models" })
            VerifySurface(name, storage, check, output);
        VerifyAutoComplete(check, output);
    }

    private static void VerifySurface(string name, string storage, Action<bool, string> check, string? output)
    {
        ItemsControl owner = name switch
        {
            "ListBox" => new ListBox { Height = 180 },
            "ComboBox" => new ComboBox { MaxDropDownHeight = 180 },
            "ItemsControl" => new ItemsControl { Height = 180 },
            "TabStrip" => new TabStrip { Height = 64 },
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };
        owner.Width = 520;
        var own = storage != "models";
        var original = Enumerable.Range(0, 64).Select(index => own ? (object)OwnItem(name, index) : new ReinsertRow(index, "项目 " + index)).ToArray();
        ObservableCollection<object>? source = null;
        IList rows;
        if (storage == "direct")
        {
            foreach (var item in original) owner.Items.Add(item);
            rows = owner.Items;
        }
        else
        {
            source = new ObservableCollection<object>(original); owner.ItemsSource = source; rows = source;
        }
        if (!own) owner.ItemTemplate = new FuncDataTemplate<ReinsertRow>((row, _) =>
            new TextBlock { Text = row?.Name, Height = 32, MinWidth = name == "TabStrip" ? 100 : 0 });
        var window = new Window
        {
            Width = 640, Height = 380, RequestedThemeVariant = ThemeVariant.Light, WindowDecorations = WindowDecorations.None,
            Content = new Border { Padding = new Thickness(24), Child = new StackPanel { Spacing = 16, Children =
            {
                new TextBlock { Text = name + " · " + storage }, owner
            } } }
        };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        var label = "Virtualized reinsert/" + name + "/" + storage;
        try
        {
            var anchor = rows[1]!;
            SetSelection(owner, anchor);
            if (owner is ComboBox combo)
            {
                Click(window, combo); Flush();
                check(combo.IsDropDownOpen, label + ": a real pointer opens the actual ComboBox popup");
            }
            Show(owner, 0);
            check(owner.ItemsPanelRoot is VirtualizingStackPanel && owner.GetRealizedContainers().Count() is > 0 and < 32 && Mounted(owner, 0, original[0], own),
                label + ": the initial native virtualized surface realizes and visually attaches its first item");
            if (own) SavePanel(owner, output, label, "before");

            var moved = rows[0]!; rows.RemoveAt(0); rows.Insert(2, moved); Flush();
            VerifyState(owner, rows, 2, moved, anchor, own, check, output, label + "/remove-insert");
            if (source is not null)
            {
                source.Move(2, 0); Flush();
                VerifyState(owner, rows, 0, moved, anchor, own, check, output, label + "/observable-move");
            }

            var distantAnchor = rows[57]!;
            SetSelection(owner, distantAnchor); Show(owner, 56);
            check(owner.GetRealizedContainers().Count() is > 0 and < 32 && FirstItemOutsideViewport(owner) && Mounted(owner, 56, rows[56]!, own),
                label + ": distant scrolling leaves the first viewport while allowing native focused-container retention");
            var distant = rows[56]!; rows.RemoveAt(56); rows.Insert(58, distant); Flush();
            VerifyState(owner, rows, 58, distant, distantAnchor, own, check, output, label + "/recycled-remove-insert");
            if (source is not null)
            {
                source.Move(58, 56); Flush();
                VerifyState(owner, rows, 56, distant, distantAnchor, own, check, output, label + "/recycled-observable-move");
            }
            SetSelection(owner, anchor); Show(owner, 0);
            check(Mounted(owner, 0, rows[0]!, own) && Mounted(owner, 1, rows[1]!, own) && CurrentRealized(owner, own),
                label + ": scrolling back remounts current-index items instead of returning detached cached own containers");
            check(SelectionMatches(owner, rows, anchor), label + ": returning to the first viewport retains the unaffected selected item and index");
        }
        finally
        {
            if (owner is ComboBox combo) combo.IsDropDownOpen = false;
            window.Close(); Flush();
        }
    }

    private static Control OwnItem(string name, int index)
    {
        var text = "项目 " + index;
        return name switch
        {
            "ListBox" => new ListBoxItem { Content = text, Height = 32 },
            "ComboBox" => new ComboBoxItem { Content = text, Height = 32 },
            "TabStrip" => new TabStripItem { Content = text, Width = 112, MinHeight = 36 },
            "ItemsControl" => new Border { Height = 32, Child = new TextBlock { Text = text, Margin = new Thickness(8, 4) } },
            _ => throw new ArgumentOutOfRangeException(nameof(name))
        };
    }

    private static void VerifyState(ItemsControl owner, IList rows, int index, object expected, object anchor, bool own,
        Action<bool, string> check, string? output, string label)
    {
        // Insertion can move the target just beyond the viewport while the native
        // selector keeps the unchanged selected neighbour visible. Realize it first.
        Show(owner, index);
        var targetMounted = Mounted(owner, index, expected, own);
        var realizedCurrent = CurrentRealized(owner, own);
        var mounted = targetMounted && realizedCurrent;
        if (!mounted) SavePanel(owner, output, label, "detached");
        var container = owner.ContainerFromIndex(index);
        check(rows.Count == 64 && owner.ItemCount == 64 && ReferenceEquals(rows[index], expected) && ReferenceEquals(owner.Items[index], expected),
            label + ": mutation retains collection identity, count and the expected current item index");
        var diagnostic = mounted ? "" : DescribeRealized(owner, index, expected, own);
        check(mounted, label + $": realized own/model containers have real visual parents and valid current indices (target={container?.GetType().Name ?? "null"}, attached={container?.IsAttachedToVisualTree()}, parent={container?.GetVisualParent()?.GetType().Name ?? "null"}, targetMounted={targetMounted}, realizedCurrent={realizedCurrent})" + diagnostic);
        check(SelectionMatches(owner, rows, anchor), label + ": moving an unselected item preserves the unaffected selected identity and updated index");
    }

    private static bool Mounted(ItemsControl owner, int index, object expected, bool own)
    {
        var container = owner.ContainerFromIndex(index);
        return container is not null && container.IsAttachedToVisualTree() && ReferenceEquals(container.GetVisualParent(), owner.ItemsPanelRoot) &&
            owner.IndexFromContainer(container) == index && ReferenceEquals(owner.ItemFromContainer(container), expected) &&
            (!own || ReferenceEquals(container, expected));
    }

    private static bool CurrentRealized(ItemsControl owner, bool own) => owner.GetRealizedContainers().All(container =>
    {
        var index = owner.IndexFromContainer(container);
        return index >= 0 && index < owner.ItemCount && Mounted(owner, index, owner.Items[index]!, own);
    });

    private static string DescribeRealized(ItemsControl owner, int targetIndex, object expected, bool own)
    {
        var lines = new List<string>
        {
            $"\n  Target[{targetIndex}] expected={Identity(expected)}, resolved={Identity(owner.ContainerFromIndex(targetIndex))}, ownerItem={Identity(owner.Items[targetIndex])}, panel={Identity(owner.ItemsPanelRoot)}"
        };
        foreach (var container in owner.GetRealizedContainers())
        {
            var index = owner.IndexFromContainer(container);
            var row = index >= 0 && index < owner.ItemCount ? owner.Items[index] : null;
            var data = owner.ItemFromContainer(container);
            var actualOwnIndex = ReferenceIndex(owner, container);
            var dataIndex = ReferenceIndex(owner, data);
            var roundTrip = index >= 0 && index < owner.ItemCount && ReferenceEquals(owner.ContainerFromIndex(index), container);
            lines.Add($"  Realized {Identity(container)}: index={index}, actualOwnIndex={actualOwnIndex}, dataIndex={dataIndex}, currentItem={Identity(row)}, itemFromContainer={Identity(data)}, currentDataRef={ReferenceEquals(row, data)}, ownCurrentRef={ReferenceEquals(row, container)}, roundTrip={roundTrip}, mountedAtReportedIndex={index >= 0 && index < owner.ItemCount && Mounted(owner, index, row!, own)}, attached={container.IsAttachedToVisualTree()}, visualParent={Identity(container.GetVisualParent())}, logicalParent={Identity(container.Parent)}, bounds={container.Bounds}, visible={container.IsVisible}, effectivelyVisible={container.IsEffectivelyVisible}, focused={container.IsFocused}, focusWithin={container.IsKeyboardFocusWithin}");
        }
        return string.Join("\n", lines);
    }

    private static int ReferenceIndex(ItemsControl owner, object? value)
    {
        if (value is null) return -1;
        for (var index = 0; index < owner.ItemCount; index++) if (ReferenceEquals(owner.Items[index], value)) return index;
        return -1;
    }

    private static string Identity(object? value)
    {
        if (value is null) return "null";
        var label = value switch
        {
            ReinsertRow row => row.Name,
            ContentControl { Content: string text } => text,
            Border { Child: TextBlock { Text: { } text } } => text,
            _ => ""
        };
        return value.GetType().Name + "#" + System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(value).ToString("x") + (label.Length > 0 ? "(" + label + ")" : "");
    }

    private static void SetSelection(ItemsControl owner, object selected)
    {
        if (owner is SelectingItemsControl selector) selector.SelectedItem = selected;
        Flush();
    }

    private static bool SelectionMatches(ItemsControl owner, IList rows, object selected) => owner is not SelectingItemsControl selector ||
        ReferenceEquals(selector.SelectedItem, selected) && selector.SelectedIndex == rows.IndexOf(selected);

    private static void Show(ItemsControl owner, int index) { owner.ScrollIntoView(index); Flush(); owner.UpdateLayout(); Flush(); }

    private static bool FirstItemOutsideViewport(ItemsControl owner)
    {
        var first = owner.ContainerFromIndex(0);
        if (first is null || !first.IsAttachedToVisualTree()) return true;
        var scroll = owner.ItemsPanelRoot?.GetVisualAncestors().OfType<ScrollViewer>().FirstOrDefault();
        if (scroll is null) return false;
        var start = first.TranslatePoint(default, scroll);
        var end = first.TranslatePoint(new Point(first.Bounds.Width, first.Bounds.Height), scroll);
        if (start is not { } top || end is not { } bottom) return false;
        return owner.ItemsPanelRoot is VirtualizingStackPanel { Orientation: Orientation.Horizontal }
            ? bottom.X <= 0 || top.X >= scroll.Viewport.Width
            : bottom.Y <= 0 || top.Y >= scroll.Viewport.Height;
    }

    private static void VerifyAutoComplete(Action<bool, string> check, string? output)
    {
        var own = new ObservableCollection<ListBoxItem>(Enumerable.Range(0, 64).Select(index => new ListBoxItem { Content = "建议 " + index, Height = 32 }));
        var auto = new AutoCompleteBox
        {
            Width = 520, ItemsSource = own, MinimumPrefixLength = 0, MinimumPopulateDelay = TimeSpan.Zero,
            MaxDropDownHeight = 180, FilterMode = AutoCompleteFilterMode.Custom, ItemFilter = (search, _) => search != "none"
        };
        var window = new Window { Width = 640, Height = 380, Content = new Border { Padding = new Thickness(24), Child = auto } };
        Motion.SetEnabled(window, false); window.Show(); Flush();
        const string label = "Virtualized reinsert/AutoCompleteBox/own-suggestions";
        try
        {
            var editor = auto.GetVisualDescendants().OfType<TextBox>().Single();
            Type("all");
            var popup = auto.GetVisualDescendants().OfType<Popup>().Concat(auto.GetLogicalDescendants().OfType<Popup>()).Distinct().Single(part => part.Name == "PART_Popup");
            var popupChild = popup.Child!;
            var suggestions = popupChild.GetVisualDescendants().Prepend(popupChild).OfType<ListBox>().Single();
            Show(suggestions, 0);
            check(auto.IsDropDownOpen && popup.IsOpen && suggestions.ItemCount == 64 && Mounted(suggestions, 0, own[0], true),
                label + ": real text input opens native filtered own-item suggestions in their actual popup");
            SavePanel(suggestions, output, label, "before");
            Type("none"); Type("all-again"); Show(suggestions, 0);
            var mounted = Mounted(suggestions, 0, own[0], true) && CurrentRealized(suggestions, true);
            if (!mounted) SavePanel(suggestions, output, label, "refilter-detached");
            check(suggestions.ItemCount == 64 && ReferenceEquals(suggestions.Items[0], own[0]) && mounted,
                label + $": clearing and repopulating filtered items remounts own ListBoxItem suggestions (count={suggestions.ItemCount}, attached={suggestions.ContainerFromIndex(0)?.IsAttachedToVisualTree()}, parent={suggestions.ContainerFromIndex(0)?.GetVisualParent()?.GetType().Name ?? "null"}, popup={popup.IsOpen})");
            var first = own[0]; own.RemoveAt(0); own.Insert(2, first); Type("all-reordered"); Show(suggestions, 0);
            mounted = Mounted(suggestions, 2, first, true) && CurrentRealized(suggestions, true);
            if (!mounted) SavePanel(suggestions, output, label, "source-reinsert-detached");
            check(suggestions.ItemCount == 64 && ReferenceEquals(suggestions.Items[2], first) && mounted,
                label + $": source reinsert plus a real search retains the filtered current index and visual attachment (count={suggestions.ItemCount}, attached={suggestions.ContainerFromIndex(2)?.IsAttachedToVisualTree()}, parent={suggestions.ContainerFromIndex(2)?.GetVisualParent()?.GetType().Name ?? "null"}, popup={popup.IsOpen})");

            void Type(string text)
            {
                editor.Focus(); editor.SelectAll(); window.KeyTextInput(text); Flush();
                var frame = new DispatcherFrame(); DispatcherTimer.RunOnce(() => frame.Continue = false, TimeSpan.FromMilliseconds(30));
                Dispatcher.UIThread.PushFrame(frame); Flush();
                if (text != "none") { auto.IsDropDownOpen = true; Flush(); }
            }
        }
        finally { auto.IsDropDownOpen = false; window.Close(); Flush(); }
    }

    private sealed record ReinsertRow(int Id, string Name);

    private static void SavePanel(ItemsControl owner, string? output, string label, string phase)
    {
        if (output is null || owner.ItemsPanelRoot is not { } panel || panel.Bounds.Width <= 0 || panel.Bounds.Height <= 0) return;
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(panel.Bounds.Width), (int)Math.Ceiling(panel.Bounds.Height)), new Vector(96, 96));
        bitmap.Render(panel); bitmap.Save(Path.Combine(output, label.Replace('/', '-') + "-" + phase + ".png"), PngBitmapEncoderOptions.Default);
    }

    private static void Click(Window window, Control control)
    {
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }

    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
