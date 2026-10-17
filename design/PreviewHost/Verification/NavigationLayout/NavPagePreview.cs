using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NavPagePreview
{
    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static T Part<T>(Control owner, string name) where T : Control =>
        owner.GetVisualDescendants().OfType<T>().First(p => p.Name == name);

    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 1000, 480 })
        {
            var home = new NavPageModel { Title = "首页", Icon = Geometry.Parse("M0,8 L8,0 L16,8 V16 H0 Z"), ViewModel = "Home", IsClosable = false };
            var doc = new NavPageModel { Title = "编辑文档", ViewModel = "Draft", Icon = home.Icon };
            var keep = new NavPageModel { Title = "保留页面", ViewModel = "Keep", IsClosable = false };
            var group = new NavPageModel { Title = "工作区" };
            group.Children.Add(doc);
            group.Children.Add(keep);
            var shell = new NavPage
            {
                ItemsSource = new[] { home, group }, DefaultHome = home, HeaderTitle = "THE ONE",
                HeaderIcon = home.Icon,
                PageTemplate = new FuncDataTemplate<string>((value, _) => new TextBox { Text = value, Margin = new Thickness(20) })
            };
            var window = new Window { Width = width, Height = 600, Content = shell, RequestedThemeVariant = theme };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var tabs = Part<TabControl>(shell, "PART_Tabs");
            var menu = Part<NavMenu>(shell, "PART_Menu");
            var label = $"NavPage/{theme.Key}/{size}/{width}";
            check(tabs.Items.Count == 1 && ReferenceEquals(shell.SelectedPage, home), label + "/default-home");
            check(!shell.OpenPage(group) && tabs.Items.Count == 1, label + "/group-not-page");
            check(shell.OpenPage(doc) && shell.OpenPage(doc) && tabs.Items.Count == 2, label + "/open-once");
            Flush();
            var docTab = (TabItem)tabs.SelectedItem!;
            check(TabFeatures.GetIsIconVisible(docTab) && Part<ContentPresenter>(docTab, "PART_ItemIcon").IsVisible, label + "/icon");
            shell.ShowItemIcon = false;
            Flush();
            check(!Part<ContentPresenter>(docTab, "PART_ItemIcon").IsVisible, label + "/hide-icon");
            shell.ShowItemIcon = true;
            var iconHost = Part<Viewbox>(docTab, "PART_ItemIconHost");
            var iconChild = Part<ContentPresenter>(docTab, "PART_ItemIcon").Child!;
            var iconStart = iconChild.TranslatePoint(default, iconHost)!.Value;
            var iconEnd = iconChild.TranslatePoint(new Point(iconChild.Bounds.Width, iconChild.Bounds.Height), iconHost)!.Value;
            check(iconHost.Bounds.Width == 14 && iconEnd.X - iconStart.X <= 14.01 &&
                  iconEnd.Y - iconStart.Y <= 14.01, label + "/compact-icon-actual-render-size");
            var body = (ContentControl)docTab.Content!;
            var editor = body.GetVisualDescendants().OfType<TextBox>().Single();
            editor.Focus();
            editor.Text = "未保存的草稿";
            editor.CaretIndex = 3;
            shell.OpenPage(keep);
            shell.OpenPage(doc);
            Flush();
            check(ReferenceEquals(editor, body.GetVisualDescendants().OfType<TextBox>().Single()) && editor.Text == "未保存的草稿" && editor.CaretIndex == 3,
                label + "/retained-editor");
            check(TabFeatures.Pin(docTab) && ReferenceEquals(tabs.Items[0], docTab) && ReferenceEquals(tabs.SelectedItem, docTab), label + "/pin-left-selection");
            Flush();
            check(Part<PathIcon>(docTab, "PART_PinIcon").IsVisible, label + "/pin-glyph");
            check(doc.IsPinned && !shell.ClosePage(doc) && !TabFeatures.GetShowCloseButton(docTab), label + "/pin-close-guard");
            shell.OpenPage(keep);
            check(shell.CloseAllPages() == 0 && tabs.Items.Count == 3, label + "/close-all-protected");
            TabFeatures.Pin(docTab, false);
            check(!doc.IsPinned && shell.CloseAllPages() == 1 && tabs.Items.Count == 2, label + "/close-all-editable-only");
            Flush();
            check(shell.OpenPage(doc) && tabs.Items.Count == 3, label + "/reopen-closed");
            Flush();
            docTab = (TabItem)tabs.SelectedItem!;
            var rightHeader = Part<ContentPresenter>(shell, "PART_RightHeader");
            var rightBody = Part<Grid>(shell, "PART_RightBody");
            var fullHeight = rightBody.Bounds.Height;
            check(!rightHeader.IsVisible && rightBody.Bounds.Y == 0, label + "/null-right-header");
            shell.RightHeader = new Border { Height = 48, Child = new TextBlock { Text = "Workspace toolbar" } };
            Flush();
            check(rightHeader.IsVisible && rightHeader.Bounds.Height == 48 && rightBody.Bounds.Y == 48 &&
                  Math.Abs(fullHeight - rightBody.Bounds.Height - 48) < 1, label + "/right-header-layout");
            shell.RightHeader = "Header model";
            shell.RightHeaderTemplate = new FuncDataTemplate<string>((value, _) => new TextBlock { Text = value, Height = 40 });
            Flush();
            check(rightHeader.Child is TextBlock { Text: "Header model" } && rightBody.Bounds.Y == 40,
                label + "/right-header-template");
            var reopenedBody = (ContentControl)docTab.Content!;
            var reopenedEditor = reopenedBody.GetVisualDescendants().OfType<TextBox>().Single();
            reopenedEditor.Text = "Retained toolbar draft";
            reopenedEditor.Focus();
            reopenedEditor.CaretIndex = 4;
            shell.RightHeader = null;
            Flush();
            check(!rightHeader.IsVisible && rightBody.Bounds.Y == 0 &&
                  Math.Abs(rightBody.Bounds.Height - fullHeight) < 1, label + "/clear-right-header-fills-body");
            check(ReferenceEquals(reopenedEditor, reopenedBody.GetVisualDescendants().OfType<TextBox>().Single()) &&
                  reopenedEditor.Text == "Retained toolbar draft" && reopenedEditor.CaretIndex == 4 && reopenedEditor.IsFocused,
                label + "/right-header-retains-editor-focus");
            check(docTab.ContextMenu?.Items.Count == 4, label + "/context-menu");
            var point = docTab.TranslatePoint(new Point(12, 12), window)!.Value;
            window.MouseDown(point, MouseButton.Right);
            window.MouseUp(point, MouseButton.Right);
            Flush();
            check(docTab.ContextMenu!.IsOpen, label + "/native-context-open");
            docTab.ContextMenu.Close();
            var button = Part<Button>(shell, "PART_Toggle");
            point = button.TranslatePoint(new Point(12, 12), window)!.Value;
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Flush();
            check(shell.IsCollapsed && menu.IsCollapsed && Math.Abs(menu.Bounds.Width - shell.CollapsedWidth) < 1,
                label + $"/header-pointer-collapse(shell={shell.IsCollapsed},menu={menu.IsCollapsed},width={menu.Bounds.Width:0.##})");
            check(!Part<TextBlock>(shell, "PART_HeaderTitle").IsVisible && tabs.Bounds.Width > width - 100, label + "/collapsed-layout");
            shell.IsCollapsed = false;
            Flush();
            check(Math.Abs(menu.Bounds.Width - shell.ExpandedWidth) < 1, label + "/expand-width");
            var scroll = Part<ScrollViewer>(tabs, "PART_HeaderScroll");
            var headerPosition = docTab.TranslatePoint(default, scroll)!.Value;
            check(headerPosition.X >= -1 && headerPosition.X + docTab.Bounds.Width <= scroll.Bounds.Width + 1,
                label + $"/selected-header-visible-after-expand(x={headerPosition.X:0.##},w={docTab.Bounds.Width:0.##},scroll={scroll.Bounds.Width:0.##},viewport={scroll.Viewport.Width:0.##},offset={scroll.Offset.X:0.##},extent={scroll.Extent.Width:0.##})");
            if (output is not null)
            {
                using var image = window.CaptureRenderedFrame();
                image?.Save(Path.Combine(output, $"navpage-{theme.Key}-{size}-{width}.png"), new PngBitmapEncoderOptions());
                check(image is not null, label + "/rendered");
            }
            shell.HeaderTemplate = new FuncDataTemplate<NavPage>((_, _) =>
            {
                var text = new TextBlock { Margin = new Thickness(12) };
                text.Bind(TextBlock.TextProperty, CompiledBinding.Create<NavPage, string>(p => p.HeaderTitle));
                return text;
            });
            shell.FooterTemplate = new FuncDataTemplate<NavPage>((_, _) => new TextBlock { Text = "Footer" });
            Flush();
            check(!Part<Grid>(shell, "PART_DefaultHeader").IsVisible && Part<ContentPresenter>(shell, "PART_Header").Child is TextBlock &&
                  Part<ContentPresenter>(shell, "PART_Footer").Child is TextBlock, label + "/header-footer-templates");
            check(Part<ContentPresenter>(shell, "PART_Header").Child is TextBlock title && title.Text == shell.HeaderTitle &&
                  ReferenceEquals(title.DataContext, shell), label + "/header-compiled-data-context");
            shell.HeaderTemplate = null;
            shell.FooterTemplate = null;
            Flush();
            check(Part<Grid>(shell, "PART_DefaultHeader").IsVisible && Part<ContentPresenter>(shell, "PART_Header").Child is null,
                label + "/clear-template");
            window.Close();
        }
        VerifyCustom(check);
        VerifyTabs(check);
        VerifyPinnedDrag(check);
        VerifyWidthMotion(check);
    }

    internal static void VerifyPinLayout(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var card in new[] { false, true })
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 800, 360 })
        {
            var pinned = new TabItem { Header = "固定页面", Content = "Pinned page" };
            var ordinary = new TabItem { Header = "普通页面", Content = "Ordinary page" };
            TabFeatures.SetIcon(pinned, Geometry.Parse("M0,0 H16 V16 H0 Z"));
            TabFeatures.SetIsPinned(pinned, true);
            var tabs = new TabControl { Items = { pinned, ordinary } };
            if (card) tabs.Classes.Add("card");
            TabFeatures.SetCanClose(tabs, true);
            var window = new Window { Width = width, Height = 180, Content = tabs, RequestedThemeVariant = theme };
            Motion.SetEnabled(window, false);
            ControlSizing.SetSize(window, size);
            window.Show(); Flush();
            var label = $"TabControl/pin-layout/{card}/{theme.Key}/{size}/{width}";
            var pin = Part<PathIcon>(pinned, "PART_PinIcon");
            var title = Part<ContentPresenter>(pinned, "PART_ContentPresenter");
            var titleRight = title.TranslatePoint(new Point(title.Bounds.Width, 0), pinned)!.Value.X;
            var pinLeft = pin.TranslatePoint(default, pinned)!.Value.X;
            check(pin.IsVisible && pinLeft >= titleRight + 5.9 &&
                  pinLeft + pin.Bounds.Width <= pinned.Bounds.Width, label + "/pin-right-of-title-no-overflow");
            check(!Part<PathIcon>(ordinary, "PART_PinIcon").IsVisible &&
                  !TabFeatures.GetShowCloseButton(pinned), label + "/pin-and-close-visibility");
            if (output is not null)
            {
                using var image = window.CaptureRenderedFrame();
                image?.Save(Path.Combine(output, $"tab-pin-{card}-{theme.Key}-{size}-{width}.png"), new PngBitmapEncoderOptions());
                check(image is not null, label + "/rendered");
            }
            window.Close();
        }
    }

    private static void VerifyTabs(Action<bool, string> check)
    {
        var tabs = new TabControl();
        var first = new TabItem { Header = "First" };
        var last = new TabItem { Header = "Last" };
        tabs.Items.Add(first);
        tabs.Items.Add(last);
        var window = new Window { Width = 520, Height = 280, Content = tabs };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        check(first.ContextMenu is not null, "TabFeatures/native/default-menu");
        TabFeatures.SetCanClose(tabs, true);
        TabFeatures.SetCanReorder(tabs, true);
        TabFeatures.SetIcon(first, Geometry.Parse("M0,0 H16 V16 H0 Z"));
        Flush();
        check(Part<ContentPresenter>(first, "PART_ItemIcon").IsVisible, "TabFeatures/native/icon-after-attach");
        TabFeatures.SetIsPinned(last, true);
        Flush();
        check(ReferenceEquals(tabs.Items[0], last), "TabFeatures/native/attached-pin-left");
        check(!TabFeatures.Move(tabs, 0, 1) && !TabFeatures.Move(tabs, 1, 0), "TabFeatures/native/reorder-pin-boundary");
        tabs.SelectedItem = last;
        last.Focus();
        window.KeyPress(Key.W, RawInputModifiers.Control, PhysicalKey.W, null);
        Flush();
        check(tabs.Items.Count == 2, "TabFeatures/native/keyboard-pin-close-guard");
        var custom = new ContextMenu { Items = { new MenuItem { Header = "Custom" } } };
        first.ContextMenu = custom;
        TabFeatures.SetShowItemIcon(tabs, false);
        Flush();
        check(ReferenceEquals(first.ContextMenu, custom), "TabFeatures/native/preserve-consumer-menu");
        window.Close();

        var models = new System.Collections.ObjectModel.ObservableCollection<string>(Enumerable.Range(0, 80).Select(i => $"Page {i}"));
        tabs = new TabControl { ItemsSource = models };
        tabs.Classes.Add("card");
        TabFeatures.SetCanClose(tabs, true);
        TabFeatures.SetCanReorder(tabs, true);
        window = new Window { Width = 320, Height = 240, Content = tabs };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        var pinned = (TabItem)tabs.ContainerFromIndex(0)!;
        TabFeatures.Pin(pinned);
        tabs.ScrollIntoView(models[^1]);
        tabs.UpdateLayout();
        Flush();
        check(tabs.GetRealizedContainers().OfType<TabItem>().All(t => tabs.IndexFromContainer(t) == 0 || !TabFeatures.GetIsPinned(t)),
            "TabFeatures/virtualized/no-pin-state-leak");
        tabs.ScrollIntoView(models[0]);
        tabs.UpdateLayout();
        Flush();
        check(TabFeatures.GetIsPinned((TabItem)tabs.ContainerFromIndex(0)!), "TabFeatures/virtualized/restore-pin");
        check(TabFeatures.CloseAll(tabs) == 79 && models.Count == 1, "TabFeatures/virtualized/close-all-offscreen");
        window.Close();
    }

    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        var pulse = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
        pulse.Tick += (_, _) => AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; };
        pulse.Start(); timer.Start();
        try { Dispatcher.UIThread.PushFrame(frame); }
        finally { pulse.Stop(); timer.Stop(); }
        Flush();
    }

    private static void VerifyPinnedDrag(Action<bool, string> check)
    {
        foreach (var card in new[] { false, true })
        foreach (var placement in new[] { Dock.Top, Dock.Left })
        {
            var tabs = new TabControl { TabStripPlacement = placement };
            if (card) tabs.Classes.Add("card");
            TabFeatures.SetCanReorder(tabs, true);
            TabFeatures.SetCanClose(tabs, true);
            var items = Enumerable.Range(0, 5).Select(i => new TabItem { Header = $"Page {i}" }).ToArray();
            foreach (var item in items) tabs.Items.Add(item);
            TabFeatures.SetIsPinned(items[0], true);
            TabFeatures.SetIsPinned(items[1], true);
            var window = new Window { Width = 760, Height = 480, Content = tabs };
            Motion.SetEnabled(window, false);
            window.Show(); Flush();
            var label = $"TabFeatures/pinned-drag/{card}/{placement}";
            check(!TabFeatures.Move(tabs, 0, 1) && !TabFeatures.Move(tabs, 1, 0) &&
                  ReferenceEquals(tabs.Items[0], items[0]), label + "/two-pins-immutable-api");
            tabs.SelectedItem = items[0];
            items[0].Focus();
            window.KeyPress(Key.Right, RawInputModifiers.Control | RawInputModifiers.Shift, PhysicalKey.ArrowRight, null);
            Flush();
            check(ReferenceEquals(tabs.Items[0], items[0]), label + "/pin-keyboard-guard");
            Point Center(TabItem item) => item.TranslatePoint(new Point(item.Bounds.Width / 2, item.Bounds.Height / 2), window)!.Value;
            var pinnedPoint = Center(items[0]);
            var lastPoint = Center(items[^1]);
            window.MouseDown(pinnedPoint, MouseButton.Left);
            window.MouseMove(lastPoint, RawInputModifiers.LeftMouseButton);
            Flush();
            check(!window.GetVisualDescendants().OfType<Border>().Any(p => p.Name == "PART_TabDragPreview"),
                label + "/pin-pointer-no-preview");
            window.MouseUp(lastPoint, MouseButton.Left); Flush();
            tabs.SelectedItem = items[^1]; Flush();
            lastPoint = Center(items[^1]);
            pinnedPoint = Center(items[0]);
            var boundary = items[1].TranslatePoint(new Point(items[1].Bounds.Width, items[1].Bounds.Height), window)!.Value;
            window.MouseDown(lastPoint, MouseButton.Left);
            window.MouseMove(pinnedPoint, RawInputModifiers.LeftMouseButton);
            Flush();
            var preview = window.GetVisualDescendants().OfType<Border>().FirstOrDefault(p => p.Name == "PART_TabDragPreview");
            check(preview is not null, label + "/ordinary-pointer-preview");
            if (preview is not null)
            {
                var position = preview.TranslatePoint(default, window)!.Value;
                check(placement == Dock.Top ? position.X >= boundary.X - .1 : position.Y >= boundary.Y - .1,
                    label + "/preview-stops-at-pin-boundary");
            }
            check(Center(items[0]) == pinnedPoint && !items[0].Classes.Contains("drop-target") &&
                  !items[1].Classes.Contains("drop-target"), label + $"/pins-never-shift({pinnedPoint}->{Center(items[0])})");
            window.MouseUp(pinnedPoint, MouseButton.Left); Flush();
            check(ReferenceEquals(tabs.Items[0], items[0]) && ReferenceEquals(tabs.Items[1], items[1]) &&
                  ReferenceEquals(tabs.Items[2], items[^1]) && tabs.Items.Count == 5,
                label + "/drop-clamped-after-pins");
            check(TabFeatures.Move(tabs, 2, 4) && ReferenceEquals(tabs.Items[4], items[^1]),
                label + "/ordinary-reorder-still-works");
            check(Part<PathIcon>(items[0], "PART_PinIcon").IsVisible &&
                  !Part<PathIcon>(items[2], "PART_PinIcon").IsVisible, label + "/pin-glyph-only-pinned");
            window.Close();
        }
    }

    private static void VerifyWidthMotion(Action<bool, string> check)
    {
        var home = new NavPageModel { Title = "Home", ViewModel = "Home" };
        var shell = new NavPage { ItemsSource = new[] { home }, DefaultHome = home, TransitionDuration = TimeSpan.FromMilliseconds(240) };
        var window = new Window { Width = 800, Height = 500, Content = shell };
        Motion.SetEnabled(window, true);
        window.Show(); Flush();
        var menu = Part<NavMenu>(shell, "PART_Menu");
        shell.IsCollapsed = true;
        Advance(70);
        check(menu.Bounds.Width > shell.CollapsedWidth && menu.Bounds.Width < shell.ExpandedWidth, "NavPage/motion/collapse-intermediate-width");
        shell.IsCollapsed = false;
        Advance(300);
        check(Math.Abs(menu.Bounds.Width - shell.ExpandedWidth) < 1 && ReferenceEquals(shell.SelectedPage, home), "NavPage/motion/rapid-reverse-selection");
        shell.IsCollapsed = true;
        Advance(50);
        Motion.SetEnabled(window, false);
        Flush();
        check(Math.Abs(menu.Bounds.Width - shell.CollapsedWidth) < 1, "NavPage/motion/disabled-snap");
        Motion.SetEnabled(window, true);
        shell.TransitionDuration = TimeSpan.Zero;
        shell.IsCollapsed = false;
        Flush();
        check(Math.Abs(menu.Bounds.Width - shell.ExpandedWidth) < 1, "NavPage/motion/zero-duration");
        window.Close();
    }
    private static void VerifyCustom(Action<bool, string> check)
    {
        var fixture = new NavPageBindingFixture();
        var model = fixture.Model.Routes[0];
        var shell = fixture.Shell;
        var window = new Window { Width = 800, Height = 500, Content = fixture };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        check(shell.OpenPage(model), "NavPage/custom/compiled-viewmodel-binding");
        Flush();
        var tabs = Part<TabControl>(shell, "PART_Tabs");
        var tab = (TabItem)tabs.SelectedItem!;
        check(Equals(tab.Header, model.Name) && Equals(((ContentControl)tab.Content!).Content, model.Screen), "NavPage/custom/title-and-viewmodel");
        check(Part<ContentPresenter>(shell, "PART_RightHeader").Child is TextBlock { Text: "Compiled toolbar" },
            "NavPage/custom/right-header-compiled-binding");
        var menu = Part<NavMenu>(shell, "PART_Menu");
        var row = (NavMenuItem)menu.ContainerFromIndex(0)!;
        var point = row.TranslatePoint(new Point(80, row.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
        check(ReferenceEquals(shell.SelectedPage, model) && tabs.Items.Count == 1, "NavPage/custom/native-navigation-invocation");
        var cancel = true;
        tabs.AddHandler(TabFeatures.TabClosingEvent, (_, e) => e.Cancel = cancel);
        check(shell.CloseAllPages() == 0 && tabs.Items.Count == 1, "NavPage/custom/close-all-cancellation");
        cancel = false;
        check(shell.ClosePage(model) && tabs.Items.Count == 0, "NavPage/custom/close-page");
        Flush();
        check(shell.SelectedPage is null, "NavPage/custom/empty-selection");
        window.Close();
    }
}
