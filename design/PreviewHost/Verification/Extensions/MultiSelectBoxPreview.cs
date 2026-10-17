using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class MultiSelectBoxPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifySelection(check);
        VerifyRefreshIsolation(check);
        VerifyKeyboard(check);
        VerifyLifecycle(check);
        VerifyVirtualization(check);
        VerifyPopupScroll(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
            VerifyAppearance(theme, size, width, check, output);
    }

    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal,
        int width = 760)
    {
        var border = new Border { Padding = new Thickness(20), Child = content };
        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Surface"));
        var window = new Window
        {
            Width = width, Height = 850, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme ?? ThemeVariant.Light, Content = border
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static T Part<T>(Control root, string name) where T : Control
    {
        return root.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    }

    static partial void VerifyPopupScroll(Action<bool, string> check);

    private static Popup Popup(MultiSelectBox box)
    {
        return Part<Popup>(box, "PART_Popup");
    }

    private static T PopupPart<T>(MultiSelectBox box, string name) where T : Control
    {
        return Part<T>(Popup(box).Child!, name);
    }

    private static ListBox Results(MultiSelectBox box)
    {
        return PopupPart<ListBox>(box, "PART_Results");
    }

    private static void VerifySelection(Action<bool, string> check)
    {
        var view = new MultiSelectBoxDemoView();
        var box = view.Sample;
        var model = view.Model;
        var window = Open(view);
        try
        {
            check(
                ReferenceEquals(box.SelectedItems, model.Selected) && box.SelectedCount == 4 &&
                box.DisplayTags.Count == 3 && box.HiddenSelectedCount == 1,
                "MultiSelectBox/binding: compiled selection binding retains the host collection and bounds visible tags");
            var added = new List<object>();
            var removed = new List<object>();
            var events = 0;
            box.SelectionChanged += (_, args) =>
            {
                events++;
                added.AddRange(args.AddedItems.Cast<object>());
                removed.AddRange(args.RemovedItems.Cast<object>());
            };
            Click(Part<Button>(box, "PART_OpenButton"));
            check(box.IsDropDownOpen && PopupPart<TextBox>(box, "PART_SearchBox").IsFocused,
                "MultiSelectBox/pointer: native opener opens the popup and focuses search");
            var list = Results(box);
            Click((Control)list.ContainerFromIndex(4)!);
            check(
                model.Selected.Contains(model.Choices[4]) && box.SelectedCount == 5 && events == 1 &&
                ReferenceEquals(added.Single(), model.Choices[4]),
                "MultiSelectBox/pointer: clicking a native row adds the original model once");
            Click((Control)list.ContainerFromIndex(4)!);
            check(
                !model.Selected.Contains(model.Choices[4]) && box.SelectedCount == 4 && events == 2 &&
                ReferenceEquals(removed.Single(), model.Choices[4]),
                "MultiSelectBox/pointer: clicking a selected row removes it without duplicate notifications");
            check(!list.ContainerFromIndex(0)!.IsEffectivelyEnabled && !box.ToggleItem(model.Choices[0]),
                "MultiSelectBox/disabled: disabled results and the public toggle cannot change a locked item");
            box.SearchText = "00009";
            Flush();
            check(list.ItemCount == 1 && box.SelectedCount == 4 && list.SelectedItems!.Count == 0,
                "MultiSelectBox/search: filtering projects results without losing selections outside the query");
            box.ToggleItem(model.Choices[9]);
            Flush();
            box.SearchText = "missing";
            Flush();
            check(
                box.HasNoResults && box.SelectedCount == 5 &&
                PopupPart<TextBlock>(box, "PART_Empty").IsEffectivelyVisible,
                "MultiSelectBox/search: empty results show a message and preserve all selected models");
            box.SearchText = "选项";
            Flush();
            model.Choices[1].Available = false;
            Flush();
            check(
                box.ClearSelection() && model.Selected.Count == 2 && model.Selected.Contains(model.Choices[0]) &&
                model.Selected.Contains(model.Choices[1]) && !box.CanClear,
                "MultiSelectBox/clear: clearing retains selected disabled items, including newly disabled models");
            box.CloseDropDown();
            Flush();
            check(
                Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<Button>()
                    .All(button => !button.IsEffectivelyEnabled),
                "MultiSelectBox/tags: disabled selected models have disabled native remove buttons");
            model.Choices[1].Available = true;
            Flush();
            var remove = Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<Button>()
                .Single(button => button.IsEffectivelyEnabled);
            Click(remove);
            check(model.Selected.Count == 1 && ReferenceEquals(model.Selected[0], model.Choices[0]) && box.IsFocused,
                "MultiSelectBox/tags: a native remove button removes only its model and restores field focus");
            model.Selected = new ObservableCollection<MultiSelectChoice> { model.Choices[2], model.Choices[3] };
            Flush();
            check(ReferenceEquals(box.SelectedItems, model.Selected) && box.SelectedCount == 2,
                "MultiSelectBox/binding: replacing an observable selection source keeps two-way binding active");
            model.Selected.Add(model.Choices[4]);
            Flush();
            model.Choices[4].Name = "Renamed choice";
            Flush();
            check(box.DisplayTags.Any(entry => entry.Text == "Renamed choice"),
                "MultiSelectBox/models: item notifications refresh text in selected tags");
            model.Choices.Remove(model.Choices[4]);
            Flush();
            check(box.SelectedCount == 3 && box.DisplayTags.Any(entry => entry.Text == "Renamed choice"),
                "MultiSelectBox/source: removing an option does not silently rewrite externally owned selection");
            box.IsReadOnly = true;
            Flush();
            check(!box.ToggleItem(model.Choices[5]) && !box.ClearSelection() && !box.CanClear,
                "MultiSelectBox/readonly: explicit readonly state blocks all user mutation APIs");
            box.IsReadOnly = false;
            box.ItemTemplate =
                new FuncDataTemplate<MultiSelectChoice>((choice, _) =>
                    new TextBlock { Text = "Result: " + choice!.Name });
            box.SelectedItemTemplate =
                new FuncDataTemplate<MultiSelectChoice>((choice, _) => new TextBlock { Text = "Tag: " + choice!.Name });
            box.SearchText = "00002";
            box.OpenDropDown();
            Flush();
            check(Results(box).GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "Result: 选项 00002")
                  && Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<TextBlock>()
                      .Any(text => text.Text == "Tag: 选项 00002"),
                "MultiSelectBox/templates: result and selected templates receive the original typed model");
        }
        finally
        {
            window.Close();
            Flush();
        }

        box = new MultiSelectBox
        {
            ItemsSource = new object?[] { "one", null, "two", "one" },
            SelectedItems = new ArrayList { "one", "one", null }
        };
        window = Open(box);
        try
        {
            check(
                box.VisibleItems.Count == 2 && box.SelectedCount == 1 && box.ClearSelection() && box.SelectedCount == 0,
                "MultiSelectBox/identity: null entries are ignored and equal duplicates represent one selection");
            box.SelectedItems = Array.AsReadOnly(new[] { "one" });
            Flush();
            check(box.SelectedCount == 1 && !box.ToggleItem("two") && !box.ClearSelection(),
                "MultiSelectBox/collections: readonly selections remain displayable without mutation exceptions");
            box.SelectedItems = new[] { "one" };
            Flush();
            check(!box.ToggleItem("two"), "MultiSelectBox/collections: fixed-size selections are not mutated");
            box.SelectedItems = null;
            Flush();
            check(!box.CanClear && !box.ToggleItem("one"),
                "MultiSelectBox/collections: null selection is readonly and never replaces the host binding");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyKeyboard(Action<bool, string> check)
    {
        var box = new MultiSelectBox { ItemsSource = new[] { "Alpha", "Beta", "Gamma", "Delta" } };
        var after = new Button { Content = "After", Margin = new Thickness(0, 380, 0, 0) };
        var window = Open(new StackPanel { Spacing = 12, Children = { box, after } });
        try
        {
            box.Focus();
            Press(window, Key.Enter);
            var search = PopupPart<TextBox>(box, "PART_SearchBox");
            var popupHost = TopLevel.GetTopLevel(search)!;
            check(box.IsDropDownOpen && search.IsFocused,
                "MultiSelectBox/keyboard: Enter opens and focuses native search");
            var presenter = search.GetVisualDescendants().OfType<TextPresenter>().Single();
            presenter.SetCurrentValue(TextPresenter.PreeditTextProperty, "拼音");
            Press(popupHost, Key.Enter);
            check(box.SelectedCount == 0,
                "MultiSelectBox/input: composition Enter remains owned by the native text input");
            presenter.ClearValue(TextPresenter.PreeditTextProperty);
            popupHost.KeyTextInput("a");
            Flush();
            check(box.SearchText == "a" && box.VisibleItems.Count == 4,
                "MultiSelectBox/search: native text input updates the compiled query binding");
            Press(popupHost, Key.Down);
            var list = Results(box);
            check(list.ContainerFromIndex(0)!.IsFocused && box.SelectedCount == 0,
                "MultiSelectBox/keyboard: Down moves result focus without changing selection");
            Press(popupHost, Key.Space);
            Press(popupHost, Key.Down);
            Press(popupHost, Key.Enter);
            check(box.SelectedCount == 2 && box.SelectedItems!.Contains("Alpha") && box.SelectedItems.Contains("Beta"),
                "MultiSelectBox/keyboard: Space and Enter toggle focused rows while arrows preserve other selections");
            Press(popupHost, Key.End);
            check(list.ContainerFromIndex(3)!.IsFocused, "MultiSelectBox/keyboard: End focuses the last result");
            Press(popupHost, Key.Home);
            check(list.ContainerFromIndex(0)!.IsFocused, "MultiSelectBox/keyboard: Home focuses the first result");
            Press(popupHost, Key.Escape);
            check(!box.IsDropDownOpen && box.IsFocused,
                "MultiSelectBox/keyboard: Escape closes and restores field focus");
            Press(window, Key.Back);
            check(box.SelectedCount == 1 && box.SelectedItems!.Contains("Alpha"),
                "MultiSelectBox/keyboard: Backspace removes the last editable selection");
            box.OpenDropDown();
            Flush();
            search = PopupPart<TextBox>(box, "PART_SearchBox");
            popupHost = TopLevel.GetTopLevel(search)!;
            search.Text = "Beta";
            Flush();
            Press(popupHost, Key.Down);
            Press(popupHost, Key.A, RawInputModifiers.Control);
            check(box.SelectedCount == 2 && box.SelectedItems!.Contains("Alpha") && box.SelectedItems.Contains("Beta"),
                "MultiSelectBox/keyboard: Ctrl+A in results selects only enabled query matches and retains hidden selection");
            search.Text = "Delta";
            search.Focus();
            Flush();
            Press(popupHost, Key.Down);
            Press(popupHost, Key.A, RawInputModifiers.Meta);
            check(box.SelectedCount == 3 && box.SelectedItems!.Contains("Delta") && box.SelectedItems.Contains("Alpha"),
                "MultiSelectBox/keyboard: Cmd+A adds only the current query while preserving earlier selections");
            search.Text = "Beta";
            Flush();
            Press(popupHost, Key.Escape);
            box.OpenDropDown();
            Flush();
            search.Focus();
            search.SelectAll();
            Press(popupHost, Key.A, RawInputModifiers.Control);
            check(search.IsFocused && search.SelectedText == "Beta",
                "MultiSelectBox/keyboard: Ctrl+A in search retains native text selection");
            Press(popupHost, Key.Tab);
            check(!box.IsDropDownOpen && !search.IsFocused,
                "MultiSelectBox/keyboard: Tab exits popup search without leaving hidden focus");
            box.OpenDropDown();
            Flush();
            Click(after);
            check(!box.IsDropDownOpen && after.IsFocused,
                $"MultiSelectBox/light-dismiss: an outside click closes without stealing the new focus (open={box.IsDropDownOpen}, focus={window.FocusManager!.GetFocusedElement()})");
            var peer = ControlAutomationPeer.CreatePeerForElement(box);
            check(
                peer is IExpandCollapseProvider provider &&
                provider.ExpandCollapseState == ExpandCollapseState.Collapsed &&
                peer.GetAutomationControlType() == AutomationControlType.ComboBox,
                "MultiSelectBox/accessibility: the field exposes combo-box role and expand/collapse state");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyLifecycle(Action<bool, string> check)
    {
        var choices = new ObservableCollection<string> { "One", "Two", "Three" };
        var selected = new ObservableCollection<string> { "One" };
        var box = new MultiSelectBox { ItemsSource = choices, SelectedItems = selected };
        var host = new ContentControl { Content = box };
        var window = Open(host);
        try
        {
            choices.Add("Four");
            selected.Add("Two");
            Flush();
            check(box.VisibleItems.Count == 4 && box.SelectedCount == 2,
                "MultiSelectBox/lifecycle: observable options and selections update independently");
            box.OpenDropDown();
            Flush();
            host.Content = null;
            Flush();
            selected.Add("Three");
            choices.Add("Five");
            check(!box.IsDropDownOpen && !Popup(box).IsOpen && box.SelectedCount == 2,
                "MultiSelectBox/lifecycle: detach closes the popup and releases collection subscriptions");
            host.Content = box;
            Flush();
            check(box.SelectedCount == 3 && box.VisibleItems.Count == 5,
                "MultiSelectBox/lifecycle: remount rebuilds current collection state");
            var template = box.Template;
            box.OpenDropDown();
            Flush();
            box.Template = new FuncControlTemplate<MultiSelectBox>((_, _) => new Border());
            Flush();
            box.Template = template;
            Flush();
            var events = 0;
            box.SelectionChanged += (_, _) => events++;
            Click(Part<Button>(box, "PART_ClearButton"));
            check(selected.Count == 0 && events == 1,
                "MultiSelectBox/template: replacing and restoring the template does not duplicate clear handlers");
            box.OpenDropDown();
            Flush();
            host.IsEnabled = false;
            Flush();
            check(!box.IsDropDownOpen && !box.ToggleItem("One"),
                "MultiSelectBox/disabled: disabling an ancestor closes the popup and blocks mutation");
            host.IsEnabled = true;
            Flush();
            selected.Add("Four");
            Flush();
            check(box.CanClear && box.SelectedCount == 1,
                "MultiSelectBox/disabled: re-enabling restores interaction without replacing selection");
            var peer = (IExpandCollapseProvider)ControlAutomationPeer.CreatePeerForElement(box)!;
            peer.Expand();
            Flush();
            check(box.IsDropDownOpen, "MultiSelectBox/accessibility: automation can open the choices");
            peer.Collapse();
            Flush();
            check(!box.IsDropDownOpen, "MultiSelectBox/accessibility: automation can close the choices");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyVirtualization(Action<bool, string> check)
    {
        var view = new MultiSelectBoxDemoView();
        var box = view.Sample;
        var window = Open(view);
        try
        {
            box.OpenDropDown();
            Flush();
            var list = Results(box);
            var search = PopupPart<TextBox>(box, "PART_SearchBox");
            Press(TopLevel.GetTopLevel(search)!, Key.Down);
            check(list.ContainerFromIndex(1)!.IsFocused && box.SelectedCount == 4,
                "MultiSelectBox/keyboard: result navigation skips the first disabled option without changing selection");
            check(
                list.ItemCount == 10000 && list.ItemsPanelRoot is VirtualizingStackPanel &&
                list.GetRealizedContainers().Count() is > 0 and < 60,
                "MultiSelectBox/virtualization: ten thousand models create only a viewport of native row containers");
            list.ScrollIntoView(9999);
            Flush();
            check(
                list.ContainerFromIndex(9999) is not null && list.ContainerFromIndex(0) is null &&
                list.GetRealizedContainers().Count() < 60,
                "MultiSelectBox/virtualization: distant scrolling recycles the first rows");
            Click((Control)list.ContainerFromIndex(9999)!);
            check(view.Model.Selected.Contains(view.Model.Choices[9999]),
                "MultiSelectBox/virtualization: a recycled distant row selects its original model");
            box.SearchText = "00007";
            Flush();
            list = Results(box);
            check(list.ItemCount == 1 && !list.ContainerFromIndex(0)!.IsEffectivelyEnabled,
                "MultiSelectBox/virtualization: recycled rows receive the current disabled state");
            var count = box.SelectedCount;
            var host = TopLevel.GetTopLevel(PopupPart<TextBox>(box, "PART_SearchBox"))!;
            Press(host, Key.Enter);
            check(box.SelectedCount == count, "MultiSelectBox/disabled: Enter cannot select a disabled-only result");
            view.Model.Selected = new ObservableCollection<MultiSelectChoice>(view.Model.Choices);
            Flush();
            check(
                box.SelectedCount == 10000 && box.DisplayTags.Count == 3 && Part<ItemsControl>(box, "PART_Tags")
                    .GetVisualDescendants().OfType<Tag>().Count() == 3,
                "MultiSelectBox/virtualization: ten thousand selected models do not create ten thousand tag controls");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void VerifyAppearance(ThemeVariant theme, ControlSize size, int width, Action<bool, string> check,
        string? output)
    {
        var view = new MultiSelectBoxDemoView();
        var box = view.Sample;
        var window = Open(view, theme, size, width);
        var label = $"MultiSelectBox/{theme.Key}/{size}/{width}";
        try
        {
            var input = Part<Border>(box, "PART_Input");
            check(
                box.ItemHeight == size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 } &&
                input.Bounds.Height >= box.ItemHeight && box.Bounds.Width <= width - 40,
                label + ": the field inherits density and stays within the viewport");
            var tags = Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<Tag>().ToArray();
            check(tags.All(tag => tag.Bounds.Height <= 26),
                label +
                $": compact tags fit inside native input density ({string.Join(",", tags.Select(tag => tag.Bounds.Height))})");
            if (width > 400)
                check(input.Bounds.Height == box.ItemHeight,
                    label + $": a single line matches native input height ({input.Bounds.Height}/{box.ItemHeight})");
            box.TryFindResource("One.AccentSoft", box.ActualThemeVariant, out var accentSoft);
            using (var tagBitmap =
                   new RenderTargetBitmap(new PixelSize((int)tags[0].Bounds.Width, (int)tags[0].Bounds.Height),
                       new Vector(96, 96)))
            {
                tagBitmap.Render(tags[0]);
                check(Pixel(tagBitmap, 5, 3) == ((ISolidColorBrush)accentSoft!).Color,
                    label + ": selected tag pixels use the theme accent palette");
            }

            var actions = Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<Button>()
                .Concat(new[] { Part<Button>(box, "PART_ClearButton"), Part<Button>(box, "PART_OpenButton") })
                .ToArray();
            check(
                actions.All(button => button.GetVisualDescendants().OfType<ContentPresenter>().Any(presenter =>
                    presenter.Name == "PART_ContentPresenter" && presenter.Bounds.Width >= 10 &&
                    presenter.Bounds.Height >= 10)),
                label + ": remove, clear and open glyphs have actual visible content space");
            check(actions.All(button => button.GetVisualDescendants().OfType<PathIcon>().Any(HasInk)),
                label + ": each remove, clear and open icon produces nontransparent pixels");
            check(
                !Part<ItemsControl>(box, "PART_Tags").GetVisualDescendants().OfType<ScrollBar>()
                    .Any(bar => bar.IsEffectivelyVisible), label + ": selected tags wrap without an inner scrollbar");
            box.OpenDropDown();
            Flush();
            var popup = Popup(box);
            var search = PopupPart<TextBox>(box, "PART_SearchBox");
            check(
                search.ActualThemeVariant == theme && ControlSizing.GetSize(search) == size &&
                search.MinHeight == box.ItemHeight && search.Bounds.Height >= box.ItemHeight,
                label +
                $": popup input inherits the actual theme and native input density (theme={search.ActualThemeVariant}, size={ControlSizing.GetSize(search)}, height={search.Bounds.Height}, expected={box.ItemHeight})");
            check(
                popup.Width <= window.ClientSize.Width - 16 && Results(box).Bounds.Height > 0 &&
                Results(box).GetRealizedContainers().Count() < 60,
                label + ": popup width and native result viewport are bounded");
            var rows = Results(box).GetRealizedContainers().OfType<ListBoxItem>().ToArray();
            check(rows.All(row => row.Bounds.Height == box.ItemHeight),
                label +
                $": result rows match native density ({string.Join(",", rows.Select(row => row.Bounds.Height))})");
            check(rows.Zip(rows.Skip(1)).All(pair => pair.Second.Bounds.Top - pair.First.Bounds.Bottom >= 4),
                label + ": selected and disabled rows retain a visible gap");
            var border = (Border)popup.Child!;
            box.TryFindResource("One.Surface", box.ActualThemeVariant, out var surface);
            using var bitmap =
                new RenderTargetBitmap(new PixelSize((int)border.Bounds.Width, (int)border.Bounds.Height),
                    new Vector(96, 96));
            bitmap.Render(border);
            check(Pixel(bitmap, 3, Math.Min(20, (int)border.Bounds.Height - 2)) == ((ISolidColorBrush)surface!).Color,
                label + ": actual popup surface pixels match semantic resources");
            var opposite = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
            window.RequestedThemeVariant = opposite;
            Flush();
            check(search.ActualThemeVariant == opposite, label + ": changing theme updates an already open popup");
            window.RequestedThemeVariant = theme;
            Flush();
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, $"multiselect-{theme.Key}-{size}-{width}.png"),
                    PngBitmapEncoderOptions.Default);
                bitmap.Save(Path.Combine(output, $"multiselect-popup-{theme.Key}-{size}-{width}.png"),
                    PngBitmapEncoderOptions.Default);
            }
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Press(TopLevel root, Key key, RawInputModifiers modifiers = RawInputModifiers.None)
    {
        root.KeyPress(key, modifiers, PhysicalKey.None, null);
        root.KeyRelease(key, modifiers, PhysicalKey.None, null);
        Flush();
    }

    private static void Click(Control control)
    {
        control.BringIntoView();
        Flush();
        var root = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value;
        root.MouseMove(point);
        root.MouseDown(point, MouseButton.Left);
        root.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            pin.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static bool HasInk(PathIcon icon)
    {
        if (icon.Bounds.Width < 1 || icon.Bounds.Height < 1) return false;
        using var bitmap = new RenderTargetBitmap(new PixelSize((int)icon.Bounds.Width, (int)icon.Bounds.Height),
            new Vector(96, 96));
        bitmap.Render(icon);
        for (var y = 0; y < (int)icon.Bounds.Height; y++)
        for (var x = 0; x < (int)icon.Bounds.Width; x++)
            if (Pixel(bitmap, x, y).A > 50)
                return true;
        return false;
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}
