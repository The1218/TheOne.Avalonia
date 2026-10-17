using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class PopupScrollPreview
{
    private sealed record Sample(string Name, Control Anchor, Func<Control> Open, Action Close);
    private static void Flush()
    {
        for (var i = 0; i < 3; i++)
        {
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static T Part<T>(Control owner, string name) where T : Control =>
        owner.GetVisualDescendants().OfType<T>().First(p => p.Name == name);
    private static Control Root(Popup popup) { Flush(); return popup.Child!; }
    private static void Click(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseDown(point, MouseButton.Left);
        host.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static IEnumerable<Sample> Samples()
    {
        foreach (var count in new[] { 2, 200 })
        {
            var auto = new AutoCompleteBox
            {
                ItemsSource = Enumerable.Range(0, count).Select(i => $"Option {i}").ToArray(),
                MinimumPrefixLength = 0, FilterMode = AutoCompleteFilterMode.None, MaxDropDownHeight = 160
            };
            yield return new($"AutoCompleteBox/{count}", auto,
                () => { auto.IsDropDownOpen = true; return Root(Part<Popup>(auto, "PART_Popup")); },
                () => auto.IsDropDownOpen = false);
            var multi = new MultiSelectBox { ItemsSource = auto.ItemsSource, MaxDropDownHeight = 160 };
            yield return new($"MultiSelectBox/{count}", multi,
                () => { multi.OpenDropDown(); return Root(Part<Popup>(multi, "PART_Popup")); }, multi.CloseDropDown);
        }
        foreach (var count in new[] { 2, 200 })
        {
            var combo = new ComboBox { ItemsSource = Enumerable.Range(0, count).Select(i => $"Option {i}").ToArray(), SelectedIndex = 0, MaxDropDownHeight = 160 };
            yield return new($"ComboBox/{count}", combo, () => { combo.IsDropDownOpen = true; return Root(Part<Popup>(combo, "PART_Popup")); }, () => combo.IsDropDownOpen = false);
        }
        var colors = new ThemeColorPicker { MaxDropDownHeight = 160 };
        yield return new("ThemeColorPicker", colors, () => { colors.IsDropDownOpen = true; return Root(Part<Popup>(colors, "PART_Popup")); }, () => colors.IsDropDownOpen = false);
        var calendar = new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 17) };
        yield return new("CalendarDatePicker", calendar, () => { calendar.IsDropDownOpen = true; return Root(Part<Popup>(calendar, "PART_Popup")); }, () => calendar.IsDropDownOpen = false);
        foreach (var date in new[] { true, false })
        {
            Control picker = date ? new DatePicker { SelectedDate = DateTimeOffset.Now } : new TimePicker { SelectedTime = new TimeSpan(12, 30, 0) };
            yield return new(date ? "DatePicker" : "TimePicker", picker,
                () => { Click(Part<Button>(picker, "PART_FlyoutButton")); return Root(Part<Popup>(picker, "PART_Popup")); },
                () => Part<Popup>(picker, "PART_Popup").IsOpen = false);
        }
        foreach (var count in new[] { 2, 200 })
        {
            var anchor = new Button { Content = "Open flyout" };
            var list = new ListBox { ItemsSource = Enumerable.Range(0, count).Select(i => $"Option {i}").ToArray(), MaxHeight = 160 };
            var flyout = new Flyout { Content = list };
            yield return new($"Flyout/{count}", anchor, () => { flyout.ShowAt(anchor); return Root(flyout.Popup); }, flyout.Hide);
            var menuAnchor = new Button { Content = "Open menu flyout" };
            var menu = new MenuFlyout();
            foreach (var i in Enumerable.Range(0, count)) menu.Items.Add(new MenuItem { Header = $"Action {i}" });
            menu.Popup.MaxHeight = 180;
            yield return new($"MenuFlyout/{count}", menuAnchor, () => { menu.ShowAt(menuAnchor); return Root(menu.Popup); }, menu.Hide);
            var contextAnchor = new Button { Content = "Open context menu" };
            var context = new ContextMenu { MaxHeight = 180 };
            foreach (var i in Enumerable.Range(0, count)) context.Items.Add(new MenuItem { Header = $"Action {i}" });
            contextAnchor.ContextMenu = context;
            yield return new($"ContextMenu/{count}", contextAnchor, () => { context.Open(contextAnchor); Flush(); return context; }, context.Close);
            var parent = new MenuItem { Header = "Submenu" };
            foreach (var i in Enumerable.Range(0, count)) parent.Items.Add(new MenuItem { Header = $"Action {i}" });
            var top = new Menu { Items = { parent } };
            yield return new($"MenuItem/{count}", top, () => { parent.IsSubMenuOpen = true; return Root(Part<Popup>(parent, "PART_Popup")); }, () => parent.IsSubMenuOpen = false);
        }
        var color = new ColorPicker();
        yield return new("ColorPicker", color, () =>
        {
            var button = color.GetVisualDescendants().OfType<DropDownButton>().Single();
            Click(button);
            return Root(((Flyout)button.Flyout!).Popup);
        }, () => color.GetVisualDescendants().OfType<DropDownButton>().Single().Flyout!.Hide());
        foreach (var filter in new[] { false, true })
        {
            var parent = new NavPageModel { Title = "Group" };
            parent.Children.Add(new NavPageModel { Title = "Child" });
            var menu = new NavMenu
            {
                IsCollapsed = true, ShowFilter = true, ItemsSource = new[] { parent }, Height = 200,
                ChildrenBinding = CompiledBinding.Create<NavPageModel, System.Collections.ObjectModel.ObservableCollection<NavPageModel>>(p => p.Children),
                ContentTemplate = new FuncDataTemplate<NavPageModel>((p, _) => new TextBlock { Text = p?.Title })
            };
            yield return new(filter ? "NavMenu/filter" : "NavMenu/submenu", menu, () =>
            {
                if (filter) { menu.OpenFilter(); return Root(Part<Popup>(menu, "PART_FilterPopup")); }
                if (!menu.OpenSubmenu(0)) throw new InvalidOperationException("Native submenu did not open");
                Flush();
                return menu.Submenu!.GetVisualAncestors().OfType<FlyoutPresenter>().First();
            }, () => { menu.CloseSubmenu(); menu.CloseFilter(); });
        }
        var fixture = new DataGridStage1Fixture();
        fixture.Grid.Height = 220;
        fixture.Grid.ItemsSource = new DataGridCollectionView(fixture.Records);
        DataGridFeatures.SetShowColumnFilters(fixture.Grid, true);
        yield return new("DataGrid/filter", fixture.Grid, () =>
        {
            var button = fixture.Grid.GetVisualDescendants().OfType<DataGridFilterButton>().First(b => b.IsEffectivelyVisible && b.IsEnabled);
            Click(button);
            return Root(((Flyout)button.Flyout!).Popup);
        }, () => fixture.Grid.GetVisualDescendants().OfType<DataGridFilterButton>()
            .FirstOrDefault(b => b.Flyout?.IsOpen == true)?.Flyout?.Hide());
    }

    internal static void Verify(Action<bool, string> check, bool audit, string? filter = null)
    {
        foreach (var sample in Samples())
        {
            if (filter is not null && !sample.Name.StartsWith(filter, StringComparison.Ordinal)) continue;
            if (!audit && filter is null && sample.Name != "TimePicker" && sample.Name != "NavMenu/filter" &&
                !sample.Name.StartsWith("MenuItem/", StringComparison.Ordinal)) continue;
            var outside = new Border { Height = 40, Background = Brushes.Transparent };
            var outer = new ScrollViewer
            {
                Content = new StackPanel { Children = { outside, new Border { Height = 160 }, sample.Anchor, new Border { Height = 1400 } } }
            };
            sample.Anchor.MaxWidth = 340;
            var window = new Window { Width = 800, Height = 600, Content = outer };
            Motion.SetEnabled(window, false);
            window.Show(); Flush();
            outer.Offset = new Vector(0, 100); Flush();
            try
            {
                var root = sample.Open();
                Flush();
                if (audit && sample.Name.StartsWith("MenuItem", StringComparison.Ordinal))
                    foreach (var popup in sample.Anchor.GetVisualDescendants().OfType<Popup>().Where(p => p.IsOpen))
                        Console.WriteLine($"AUDIT POPUP: {sample.Name}: owner={popup.TemplatedParent?.GetType().Name}; child-owner={popup.Child?.TemplatedParent?.GetType().Name}; same-root={ReferenceEquals(popup.Child, root)}");
                var offset = outer.Offset;
                var received = 0;
                void Observe(object? sender, PointerWheelEventArgs e) => received++;
                root.AddHandler(InputElement.PointerWheelChangedEvent, Observe, handledEventsToo: true);
                var leaked = false;
                void Wheel(Point point, Vector delta)
                {
                    var host = TopLevel.GetTopLevel(root)!;
                    host.MouseWheel(root.TranslatePoint(point, host)!.Value, delta); Flush();
                    leaked |= outer.Offset != offset;
                }
                if (!audit && sample.Anchor is TimePicker)
                {
                    var column = Part<DateTimePickerPanel>(root, "PART_MinuteSelector");
                    var columnOffset = column.Offset;
                    var host = TopLevel.GetTopLevel(column)!;
                    host.MouseWheel(column.TranslatePoint(new Point(column.Bounds.Width / 2, column.Bounds.Height / 2), host)!.Value, new Vector(0, -1));
                    Flush();
                    check(column.Offset != columnOffset && outer.Offset == offset, "Popup scroll/TimePicker: native minute wheel remains active");
                }
                var scrollers = root.GetVisualDescendants().Prepend(root).OfType<ScrollViewer>().ToArray();
                var previousOffsets = scrollers.Select(s => s.Offset).ToArray();
                Wheel(new Point(root.Bounds.Width / 2, root.Bounds.Height / 2), new Vector(0, -3));
                if (!audit && sample.Name == "MenuItem/200")
                    check(scrollers.Where((s, i) => s.Offset != previousOffsets[i]).Any(), "Popup scroll/MenuItem/200: native internal scrolling remains active");
                foreach (var scroll in scrollers) scroll.Offset = new Vector(Math.Max(0, scroll.Extent.Width - scroll.Viewport.Width), Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height));
                Flush();
                Wheel(new Point(root.Bounds.Width / 2, root.Bounds.Height / 2), new Vector(0, -3));
                foreach (var scroll in scrollers) scroll.Offset = default;
                Flush();
                Wheel(new Point(root.Bounds.Width / 2, root.Bounds.Height / 2), new Vector(0, 3));
                Wheel(new Point(root.Bounds.Width / 2, 2), new Vector(0, -3));
                root.RemoveHandler(InputElement.PointerWheelChangedEvent, Observe);
                if (audit) Console.WriteLine($"AUDIT: {sample.Name}: leaked={leaked}; received={received}");
                else
                {
                    check(received > 0, $"Popup scroll/{sample.Name}: native wheel reaches popup");
                    check(!leaked, $"Popup scroll/{sample.Name}: short list, boundaries and chrome stay isolated");
                }
                sample.Close(); Flush();
                if (!audit)
                {
                    root = sample.Open(); Flush();
                    offset = outer.Offset;
                    leaked = false;
                    Wheel(new Point(root.Bounds.Width / 2, 2), new Vector(0, -3));
                    check(!leaked, $"Popup scroll/{sample.Name}: reopened popup remains isolated");
                    sample.Close(); Flush();
                }
                outside.BringIntoView(); Flush();
                var before = outer.Offset;
                var at = outside.TranslatePoint(new Point(10, 10), window)!.Value;
                window.MouseWheel(at, new Vector(0, -3)); Flush();
                if (!audit) check(outer.Offset.Y > before.Y, $"Popup scroll/{sample.Name}: outer scrolling remains available");
            }
            catch (Exception e) when (audit) { Console.WriteLine($"AUDIT ERROR: {sample.Name}: {e.Message}"); }
            finally { window.Close(); Flush(); }
        }
    }
}
