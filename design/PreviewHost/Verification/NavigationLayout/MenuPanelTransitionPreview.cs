using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class MenuPanelTransitionPreview
{
    internal static void Verify(Action<bool, string> check, string? output)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var kind in new[] { "ContextMenu", "TopMenuItem", "NestedMenuItem", "MenuFlyoutPresenter", "HorizontalMenu" })
        foreach (var models in new[] { false, true })
            VerifyCase(kind, models, check, output);
    }

    private static void VerifyCase(string kind, bool models, Action<bool, string> check, string? output)
    {
        var label = $"MenuPanelTransition/{kind}/{(models ? "model" : "own")}";
        Fixture? fixture = null;
        try
        {
            var command = new RecordCommand();
            var source = Rows(models, "initial", 32, command);
            fixture = Open(kind, models, source, command);
            VerifyCurrent(fixture, source, label + "/32", check);
            source.Add(Row(models, "added-33", command));
            VerifyCurrent(fixture, source, label + "/add-33", check);
            if (kind != "HorizontalMenu")
            {
                fixture.Owner.ScrollIntoView(32); Flush(fixture.Owner);
                check(fixture.Owner.ContainerFromIndex(32) is { } last && Mounted(fixture.Owner, last) &&
                    ReferenceEquals(fixture.Owner.ItemFromContainer(last), source[32]), label + ": the virtualized final row is actually mounted after crossing the threshold");
                fixture.Owner.ScrollIntoView(0); Flush(fixture.Owner);
            }
            source.RemoveAt(32);
            VerifyCurrent(fixture, source, label + "/remove-32", check);
            var old = source[1];
            ((SelectingItemsControl)fixture.Owner).SelectedItem = old; Flush(fixture.Owner);
            source[1] = Row(models, "replacement", command);
            VerifyCurrent(fixture, source, label + "/replace-32", check);
            VerifyRetired(fixture.Owner, [old], label + "/replace", check);
            var retired = source.ToArray();
            source.Reset(Rows(models, "reset", 2, command));
            VerifyCurrent(fixture, source, label + "/reset-2", check);
            VerifyRetired(fixture.Owner, retired, label + "/reset", check);
            var next = Rows(models, "source", 33, command);
            fixture.SetSource(next);
            VerifyCurrent(fixture, next, label + "/source-33", check);
            VerifyRetired(fixture.Owner, source, label + "/source", check);
            source.Reset(Rows(models, "retired-source", 2, command));
            VerifyCurrent(fixture, next, label + "/old-source-ignored", check);
            retired = next.ToArray();
            var final = Rows(models, "final", 2, command);
            fixture.SetSource(final);
            VerifyCurrent(fixture, final, label + "/source-2", check);
            VerifyRetired(fixture.Owner, retired, label + "/final-source", check);
            if (output is not null && kind == "ContextMenu")
            {
                using var frame = new RenderTargetBitmap(new PixelSize(Math.Max(1, (int)Math.Ceiling(fixture.Owner.Bounds.Width)),
                    Math.Max(1, (int)Math.Ceiling(fixture.Owner.Bounds.Height))), new Vector(96, 96));
                frame.Render(fixture.Owner);
                frame.Save(Path.Combine(output, label.Replace('/', '-') + ".png"), PngBitmapEncoderOptions.Default);
            }
            fixture.UpdateCommands();
            Click((MenuItem)fixture.Owner.ContainerFromIndex(0)!);
            check(command.Count == 1 && ReferenceEquals(command.Last, final[0]), label + ": real pointer activation executes only the current first item");
            fixture.Reopen(); Flush(fixture.Owner); fixture.UpdateCommands();
            var first = (MenuItem)fixture.Owner.ContainerFromIndex(0)!;
            ((SelectingItemsControl)fixture.Owner).SelectedItem = final[0]; first.Focus(); Flush(fixture.Owner);
            var root = TopLevel.GetTopLevel(first)!;
            Press(root, kind == "HorizontalMenu" ? Key.Right : Key.Down);
            check(ReferenceEquals(((SelectingItemsControl)fixture.Owner).SelectedItem, final[1]) &&
                fixture.Owner.ContainerFromIndex(1) is MenuItem { IsSelected: true }, label + ": native keyboard navigation selects the current second item");
            Press(root, Key.Enter);
            check(command.Count == 2 && ReferenceEquals(command.Last, final[1]), label + ": native Enter executes the current command without stale model or container state");
        }
        catch (Exception error) { check(false, label + ": unexpected " + error.GetType().Name + ": " + error.Message); }
        finally { fixture?.Dispose(); Flush(); }
    }

    private static void VerifyCurrent(Fixture fixture, IReadOnlyList<object> expected, string label, Action<bool, string> check)
    {
        var owner = fixture.Owner;
        Flush(owner); owner.ScrollIntoView(0); Flush(owner); fixture.UpdateCommands();
        check(owner.ItemCount == expected.Count && Enumerable.Range(0, expected.Count).All(index => ReferenceEquals(owner.Items[index], expected[index])),
            label + ": collection identity matches the current source");
        check(expected.Count <= 32 ? owner.ItemsPanelRoot is StackPanel : owner.ItemsPanelRoot is VirtualizingStackPanel,
            label + ": short menus fully measure and long menus retain virtualization");
        var visible = Enumerable.Range(0, Math.Min(3, expected.Count)).Select(owner.ContainerFromIndex).ToArray();
        check(visible.All(item => item is not null && Mounted(owner, item)), label + ": leading containers are mounted in the actual current item panel");
        check(visible.Select((item, index) => item is MenuItem menu && ReferenceEquals(owner.ItemFromContainer(menu), expected[index]) &&
                (expected[index] is not MenuItem own || ReferenceEquals(menu, own)) &&
                menu.GetVisualDescendants().OfType<TextBlock>().Any(text => text.IsEffectivelyVisible && text.Text == Title(expected[index]))).All(value => value),
            label + ": own-container identity and generated visible text match the current source");
        var selected = ((SelectingItemsControl)owner).SelectedItem;
        check((selected is null || expected.Any(item => ReferenceEquals(item, selected))) &&
            owner.ItemsPanelRoot?.Children.OfType<MenuItem>().Where(item => item.IsEffectivelyVisible).All(item =>
                item.IsSelected == ReferenceEquals(owner.ItemFromContainer(item), selected)) == true,
            label + ": selected data and selected visuals refer only to the current item identity");
        ((SelectingItemsControl)owner).SelectedItem = expected[0]; Flush(owner);
        check(owner.ContainerFromIndex(0) is MenuItem { IsSelected: true } current && ReferenceEquals(owner.ItemFromContainer(current), expected[0]),
            label + ": selecting the current first item still updates its visible container after panel recreation");
    }

    private static void VerifyRetired(ItemsControl owner, IEnumerable<object> retired, string label, Action<bool, string> check)
    {
        var old = retired.Where(item => !owner.Items.Any(current => ReferenceEquals(current, item))).ToArray();
        var retained = old.OfType<MenuItem>().Where(item => item.GetVisualParent() is not null || item.Parent is not null || item.IsAttachedToVisualTree()).ToArray();
        check(retained.Length == 0,
            label + ": removed caller-owned items leave both visual and logical owners" + (retained.Length == 0 ? "" : " [" + string.Join("; ", retained.Select(item =>
                $"{Title(item)} logical={item.Parent?.GetType().Name ?? "null"}, visual={item.GetVisualParent()?.GetType().Name ?? "null"}, attached={item.IsAttachedToVisualTree()}, currentIndex={owner.IndexFromContainer(item)}, inCurrentPanel={owner.ItemsPanelRoot?.Children.Contains(item)}")) + "]"));
        var titles = old.Select(Title).ToHashSet(StringComparer.Ordinal);
        check(owner.ItemsPanelRoot?.GetVisualDescendants().OfType<TextBlock>().Any(text => text.IsAttachedToVisualTree() && text.IsEffectivelyVisible &&
                text.Bounds.Width > 0 && text.Bounds.Height > 0 && text.Text is { } value && titles.Contains(value)) != true,
            label + ": old item text is absent from the current visible panel");
    }

    private static Fixture Open(string kind, bool models, ResetRows source, RecordCommand command)
    {
        var template = models ? new FuncDataTemplate<ModelRow>((row, _) => new TextBlock { Text = row?.Title ?? "" }) : null;
        var anchor = new Button { Content = "菜单", HorizontalAlignment = HorizontalAlignment.Left };
        var bar = new Menu { VerticalAlignment = VerticalAlignment.Top };
        var content = new StackPanel(); content.Children.Add(kind is "TopMenuItem" or "NestedMenuItem" or "HorizontalMenu" ? bar : anchor);
        var window = new Window { Width = 760, Height = 420, Content = new Border { Padding = new Thickness(16), Child = content } };
        Motion.SetEnabled(window, false);
        ItemsControl owner; Action<ResetRows> setSource; Action reopen; Action close;
        if (kind == "ContextMenu")
        {
            var context = new ContextMenu { ItemsSource = source, ItemTemplate = template, MaxHeight = 260 };
            anchor.ContextMenu = context; owner = context; setSource = rows => context.ItemsSource = rows;
            reopen = () => context.Open(anchor); close = context.Close;
        }
        else if (kind == "MenuFlyoutPresenter")
        {
            var flyout = new MenuFlyout { ItemsSource = source, ItemTemplate = template };
            window.Show(); Flush(); flyout.ShowAt(anchor); Flush();
            owner = (MenuFlyoutPresenter)((PopupFlyoutBase)flyout).Popup.Child!;
            owner.MaxHeight = 260; setSource = rows => flyout.ItemsSource = rows;
            reopen = () => flyout.ShowAt(anchor); close = flyout.Hide;
            return new Fixture(window, owner, setSource, reopen, close, command);
        }
        else if (kind == "HorizontalMenu")
        {
            owner = bar; bar.ItemsSource = source; bar.ItemTemplate = template;
            setSource = rows => bar.ItemsSource = rows; reopen = () => { }; close = bar.Close;
        }
        else
        {
            var target = new MenuItem { Header = "命令", ItemsSource = source, ItemTemplate = template };
            MenuItem? outer = kind == "NestedMenuItem" ? new MenuItem { Header = "文件", Items = { target } } : null;
            bar.Items.Add(outer ?? target); owner = target; setSource = rows => target.ItemsSource = rows;
            reopen = () => { if (outer is not null) outer.IsSubMenuOpen = true; target.IsSubMenuOpen = true; };
            close = () => { target.IsSubMenuOpen = false; if (outer is not null) outer.IsSubMenuOpen = false; bar.Close(); };
        }
        window.Show(); Flush(); reopen(); Flush();
        return new Fixture(window, owner, setSource, reopen, close, command);
    }

    private static ResetRows Rows(bool models, string prefix, int count, RecordCommand command) =>
        new(Enumerable.Range(0, count).Select(index => Row(models, prefix + "-" + index, command)));
    private static object Row(bool models, string title, RecordCommand command)
    {
        if (models) return new ModelRow(title);
        var item = new MenuItem { Header = title, Command = command }; item.CommandParameter = item; return item;
    }
    private static string Title(object item) => item is ModelRow row ? row.Title : ((MenuItem)item).Header?.ToString() ?? "";
    private static bool Mounted(ItemsControl owner, Control item) => item.IsAttachedToVisualTree() && item.IsEffectivelyVisible &&
        item.Bounds.Width > 0 && item.Bounds.Height > 0 && ReferenceEquals(item.Parent, owner) && ReferenceEquals(item.GetVisualParent(), owner.ItemsPanelRoot) &&
        owner.ItemsPanelRoot?.Children.Contains(item) == true;
    private static void Click(MenuItem item)
    {
        var root = TopLevel.GetTopLevel(item)!;
        var point = item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), root)!.Value;
        root.MouseMove(point); root.MouseDown(point, MouseButton.Left); root.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Press(TopLevel root, Key key)
    { root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null); root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null); Flush(); }
    private static void Flush(ItemsControl? owner = null)
    { Dispatcher.UIThread.RunJobs(); owner?.UpdateLayout(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }

    private sealed class Fixture : IDisposable
    {
        private readonly Window _window;
        private readonly Action _close;
        private readonly RecordCommand _command;
        public Fixture(Window window, ItemsControl owner, Action<ResetRows> setSource, Action reopen, Action close, RecordCommand command)
        {
            _window = window; Owner = owner; SetSource = setSource; Reopen = reopen; _close = close; _command = command;
            owner.ContainerPrepared += Prepared; UpdateCommands();
        }
        public ItemsControl Owner { get; }
        public Action<ResetRows> SetSource { get; }
        public Action Reopen { get; }
        private void Prepared(object? sender, ContainerPreparedEventArgs args)
        { if (args.Container is MenuItem item && args.Index >= 0 && args.Index < Owner.ItemCount) SetCommand(item, Owner.Items[args.Index]); }
        private void SetCommand(MenuItem item, object? value)
        { item.Command = _command; item.CommandParameter = value; }
        public void UpdateCommands()
        {
            if (Owner.ItemsPanelRoot is { } panel)
                foreach (var item in panel.Children.OfType<MenuItem>()) SetCommand(item, Owner.ItemFromContainer(item));
        }
        public void Dispose() { Owner.ContainerPrepared -= Prepared; _close(); _window.Close(); Flush(); }
    }
    private sealed record ModelRow(string Title);
    private sealed class ResetRows(IEnumerable<object> values) : ObservableCollection<object>(values)
    {
        public void Reset(IEnumerable<object> values)
        {
            var next = values.ToArray(); Items.Clear(); foreach (var value in next) Items.Add(value);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count))); OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
    private sealed class RecordCommand : ICommand
    {
        public int Count { get; private set; }
        public object? Last { get; private set; }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) { Count++; Last = parameter; }
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
