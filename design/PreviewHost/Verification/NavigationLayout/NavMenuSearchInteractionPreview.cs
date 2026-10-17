using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavMenuSearchInteractionPreview
{
    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var demo = new NavMenuDemoView();
            var menu = demo.Menu;
            var model = demo.Model;
            model.ShowFilter = false;
            // Retain coverage of the public external-input API independently of the integrated header.
            var input = new TextBox { Width = 240 };
            input.Bind(TextBox.TextProperty,
                CompiledBinding.Create<NavMenuDemoViewModel, string>(vm => vm.Search, model, mode: BindingMode.TwoWay));
            NavMenu.SetSearchOwner(input, menu);
            var fixture = new DockPanel();
            DockPanel.SetDock(input, Dock.Top);
            fixture.Children.Add(input);
            fixture.Children.Add(demo);
            var window = new Window { Width = 1120, Height = 1060, Content = fixture, RequestedThemeVariant = theme };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show();
            Flush();
            var label = $"NavMenu/search-input/{theme.Key}/{size}";

            void Key(Key key)
            {
                window.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null);
                window.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null);
                Flush();
            }

            menu.SelectPath(999);
            Flush();
            var original = model.Selected;
            model.Roots[0].IsExpanded = false;
            var leaves = model.Roots[0].Entries[0].Entries;
            leaves[1].IsEnabled = false;
            input.Focus();
            window.KeyTextInput("三级页面");
            Flush();
            check(menu.SearchMatchCount == 1000 && input.IsFocused,
                label + ": typed query reaches a thousand original leaf models");
            Key(global::Avalonia.Input.Key.Down);
            check(ReferenceEquals(model.Selected, leaves[0]) && input.IsFocused,
                label + ": Down selects the first result and retains search focus");
            Key(global::Avalonia.Input.Key.Down);
            check(ReferenceEquals(model.Selected, leaves[2]), label + ": navigation skips disabled matches");
            Key(global::Avalonia.Input.Key.Up);
            check(ReferenceEquals(model.Selected, leaves[0]), label + ": Up returns to the previous enabled match");
            var highlight = menu.GetVisualDescendants().OfType<NavMenuSearchText>()
                .First(text => text.SourceText == leaves[0].Title);
            var runs = highlight.Inlines!.OfType<Run>().ToArray();
            check(
                string.Concat(runs.Select(run => run.Text)) == leaves[0].Title && runs.Any(run =>
                    run.Text == "三级页面" && run.FontWeight == FontWeight.SemiBold && run.TextDecorations?.Count > 0),
                label + ": matching words are highlighted without losing text or theme foreground");
            var template = menu.ContentTemplate;
            menu.ContentTemplate = null;
            Flush();
            check(
                menu.GetVisualDescendants().OfType<NavMenuSearchText>().Any(text =>
                    text.SourceText == leaves[0].Title &&
                    text.Inlines!.OfType<Run>().Any(run => run.FontWeight == FontWeight.SemiBold)),
                label + ": default content highlights the compiled search field without a custom template");
            menu.ContentTemplate = template;
            Flush();
            window.KeyPress(global::Avalonia.Input.Key.Down, RawInputModifiers.Control, PhysicalKey.ArrowDown, null);
            window.KeyRelease(global::Avalonia.Input.Key.Down, RawInputModifiers.Control, PhysicalKey.ArrowDown, null);
            Flush();
            check(ReferenceEquals(model.Selected, leaves[0]),
                label + ": modified arrows keep native input behavior without switching results");
            var caret = input.CaretIndex;
            for (var i = 0; i < 998; i++)
            {
                window.KeyPress(global::Avalonia.Input.Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
                window.KeyRelease(global::Avalonia.Input.Key.Down, RawInputModifiers.None, PhysicalKey.ArrowDown, null);
            }

            Flush();
            var parent = (NavMenuItem)menu.ContainerFromIndex(0)!;
            var child = (NavMenuItem)parent.ContainerFromIndex(0)!;
            check(
                ReferenceEquals(model.Selected, leaves[999]) && input.CaretIndex == caret &&
                child.ContainerFromIndex(999) is not null && child.ContainerFromIndex(0) is null &&
                child.GetRealizedContainers().Count() < 80,
                label + ": repeated Down reaches result 1000 and recycles real containers");
            Key(global::Avalonia.Input.Key.Down);
            check(ReferenceEquals(model.Selected, leaves[999]), label + ": navigation clamps at the last result");
            var events = 0;
            object? invoked = null;
            menu.ItemInvoked += (_, args) =>
            {
                events++;
                invoked = args.Item;
            };
            Key(global::Avalonia.Input.Key.Enter);
            check(
                events == 1 && model.Invocations == 1 && ReferenceEquals(invoked, leaves[999]) && input.IsFocused &&
                menu.IsSearching, label + ": Enter invokes the original model exactly once without clearing the query");
            Key(global::Avalonia.Input.Key.Escape);
            check(
                input.Text == "" && model.Search == "" && !menu.IsSearching &&
                ReferenceEquals(model.Selected, original) && !model.Roots[0].IsExpanded && input.IsFocused,
                label + ": Esc restores original selection and expansion through two-way bindings");
            model.Search = "三级页面";
            Flush();
            Key(global::Avalonia.Input.Key.Up);
            check(ReferenceEquals(model.Selected, leaves[999]),
                label + ": Up starts at the last match when no result is selected");
            model.Search = "missing";
            Flush();
            Key(global::Avalonia.Input.Key.Down);
            Key(global::Avalonia.Input.Key.Enter);
            check(model.Invocations == 1 && input.IsFocused && menu.ItemCount == 0,
                label + ": empty results do not invoke or move focus");
            Key(global::Avalonia.Input.Key.Escape);
            model.IsCollapsed = true;
            model.Search = "标签页面 1000";
            Flush();
            Key(global::Avalonia.Input.Key.Down);
            check(
                ReferenceEquals(model.Selected, model.Roots[0].Entries[1].Entries[999]) &&
                menu.Submenu?.Submenu?.ItemCount == 1 && input.IsFocused,
                label + ": compact keyboard selection opens a transient cascade without stealing input focus");
            var popupText = menu.Submenu!.Submenu!.GetVisualDescendants().OfType<NavMenuSearchText>()
                .First(text => text.SourceText?.Contains("1000") == true);
            check(popupText.Inlines!.OfType<Run>().Count(run => run.FontWeight == FontWeight.SemiBold) == 2,
                label + ": compact multi-token results inherit search highlighting");
            menu.OpenSubmenu(0);
            menu.Submenu!.OpenSubmenu(0);
            Flush();
            input.Focus();
            Key(global::Avalonia.Input.Key.Enter);
            check(events == 2 && model.Invocations == 2 && !menu.IsSubmenuOpen && input.IsFocused,
                label +
                ": compact Enter forwards one invocation, closes an explicitly opened chain and retains input focus");
            Key(global::Avalonia.Input.Key.Escape);
            check(ReferenceEquals(model.Selected, original) && input.Text == "" && !menu.IsSearching,
                label + ": compact Esc restores the pre-search snapshot");
            var textProbe = new NavMenuSearchText { SourceText = "Banana still STILL" };
            NavMenuSearchText.SetHighlightQuery(textProbe, "ana still");
            check(
                string.Concat(textProbe.Inlines!.OfType<Run>().Select(run => run.Text)) == textProbe.SourceText &&
                textProbe.Inlines!.OfType<Run>().Count(run => run.FontWeight == FontWeight.SemiBold) == 3,
                label + ": overlapping and case-insensitive matches preserve every character");
            NavMenuSearchText.SetHighlightQuery(textProbe, "");
            check(
                textProbe.Inlines!.OfType<Run>()
                    .All(run => run.TextDecorations is null || run.TextDecorations.Count == 0),
                label + ": clearing removes all highlight segments");
            window.Close();
            Flush();
        }
    }
}