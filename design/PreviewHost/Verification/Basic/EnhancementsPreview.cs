using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class EnhancementsPreview
{
    public static void Verify(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        window.NavigateToCategory(8);
        ControlPreview.Flush();
        Check(gallery.FindControl<Border>("RefinementsCard")!.IsVisible,
            "Refinement category is reachable from sidebar");
        var text = gallery.FindControl<TextBox>("NormalText")!;
        var border = Part<Border>(text, "PART_BorderElement");
        text.Focus(NavigationMethod.Tab);
        ControlPreview.Flush();
        Check(border.BorderThickness == new Thickness(1) && Colour(border.BorderBrush) == Color.Parse("#129190"),
            "TextBox focus retains an accent border on all four sides");
        gallery.FindControl<Button>("ScopedButton")!.Focus();
        ControlPreview.Flush();
        Check(border.BorderThickness == new Thickness(1) && Colour(border.BorderBrush)?.A == 0,
            "TextBox blur restores normal border");
        foreach (var (prefix, height) in new[] { ("Small", 30), ("Normal", 36), ("Large", 44) })
        {
            var editor = gallery.FindControl<TextBox>(prefix + "Text")!;
            var number = gallery.FindControl<NumericUpDown>(prefix + "Number")!;
            Check(Math.Abs(editor.Bounds.Height - height) < 1 && Math.Abs(number.Bounds.Height - height) < 1,
                $"{prefix} TextBox and NumericUpDown have the expected rendered height");
            var inner = Part<TextBox>(number, "PART_TextBox");
            var innerBorder = Part<Border>(inner, "PART_BorderElement");
            inner.Focus();
            ControlPreview.Flush();
            Check(
                inner.Margin == new Thickness(0) && innerBorder.BorderThickness == new Thickness(0) &&
                Colour(innerBorder.Background)?.A == 0, $"{prefix} spinner editor has no duplicate or offset border");
            var spinner = Part<ButtonSpinner>(number, "PART_Spinner");
            var up = Part<RepeatButton>(spinner, "PART_IncreaseButton");
            var point = inner.TranslatePoint(default, number)!.Value;
            Check(point.X >= 0 && point.Y >= 0 && point.Y + inner.Bounds.Height <= number.Bounds.Height + 0.5,
                $"{prefix} spinner editor stays inside its outer frame");
            var before = number.Value;
            Click(window, up);
            ControlPreview.Flush();
            Check(number.Value == before + 1, $"{prefix} spinner increment works by mouse");
        }

        var scope = gallery.FindControl<StackPanel>("SizeScope")!;
        var scoped = gallery.FindControl<Button>("ScopedButton")!;
        Check(scoped.MinHeight == 30 && gallery.FindControl<ComboBox>("ScopedCombo")!.MinHeight == 30,
            "Container density inherits into buttons and dropdowns");
        scope.Classes.Remove("small");
        scope.Classes.Add("large");
        ControlPreview.Flush();
        Check(scoped.MinHeight == 44 && scoped.Bounds.Height >= 44,
            "Live container density change updates descendants");
        scoped.Classes.Add("normal");
        ControlPreview.Flush();
        Check(scoped.MinHeight == 36, "Child can override inherited density");
        scoped.Classes.Remove("normal");
        scope.Classes.Remove("large");
        scope.Classes.Add("small");
        ControlPreview.Flush();
        var slider = gallery.FindControl<Slider>("RefinedSlider")!;
        var container = Part<Grid>(slider, "SliderContainer");
        var thumb = slider.GetVisualDescendants().OfType<Thumb>().Single();
        var filled = Part<RepeatButton>(slider, "PART_DecreaseButton");
        var thumbPoint = Centre(thumb, window);
        window.MouseMove(thumbPoint);
        ControlPreview.Flush();
        Check(Colour(container.Background)?.A == 0 && Colour(filled.Background) == Color.Parse("#006A6B"),
            "Slider hover keeps canvas transparent and filled track accented");
        window.MouseDown(thumbPoint, MouseButton.Left);
        ControlPreview.Flush();
        Check(Colour(container.Background)?.A == 0, "Slider press does not paint a rectangular background");
        window.MouseUp(thumbPoint, MouseButton.Left);
        window.MouseMove(new Point(1, 1));
        ControlPreview.Flush();

        foreach (var (index, height) in new[] { (0, 30), (2, 44), (1, 36) })
        {
            window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = index;
            ControlPreview.Flush();
            var header = (TabItem)gallery.FindControl<TabControl>("CardTabs")!.ContainerFromIndex(1)!;
            Check(Math.Abs(header.Bounds.Height - height) < 1,
                $"Card headers follow the global density picker ({height} px)");
        }

        VerifyTabs(window, gallery);
        CardTabPreview.Verify(window, gallery);
        VerifyDate(window, gallery);
        VerifyMotion(window, gallery);
        VerifySizingMatrix();
        VerifyTabCollections();
        window.NavigateToCategory(0);
        gallery.ResetScroll();
        ControlPreview.Flush();
    }

    private static void VerifyTabs(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        var tabs = gallery.FindControl<TabControl>("CardTabs")!;
        var collection = window.Model.DemoTabs;
        var initial = collection[0];
        var first = (TabItem)tabs.ContainerFromIndex(0)!;
        var second = (TabItem)tabs.ContainerFromIndex(1)!;
        Check(
            !Part<Button>(first, "PART_CloseButton").IsVisible && Part<Button>(second, "PART_CloseButton").IsVisible &&
            !Part<Button>((TabItem)tabs.ContainerFromIndex(2)!, "PART_CloseButton").IsVisible,
            "Only the selected closable card shows its close affordance");

        var third = (TabItem)tabs.ContainerFromIndex(2)!;
        var start = first.TranslatePoint(new Point(20, first.Bounds.Height / 2), window)!.Value;
        var target = third.TranslatePoint(new Point(20, third.Bounds.Height / 2), window)!.Value;
        window.MouseMove(start);
        window.MouseDown(start, MouseButton.Left);
        window.MouseMove(target, RawInputModifiers.LeftMouseButton);
        ControlPreview.Flush();
        Check(first.Classes.Contains("dragging") && third.Classes.Contains("drop-target"),
            "Real pointer drag paints source and insertion target");
        window.MouseUp(target, MouseButton.Left);
        ControlPreview.Flush();
        Check(ReferenceEquals(collection[2], initial) && ReferenceEquals(tabs.SelectedItem, initial),
            "Pointer drop reorders bound collection and preserves dragged selection");
        Check(!first.Classes.Contains("dragging"), "Drop clears temporary drag visuals");
        var closeItem = (TabItem)tabs.ContainerFromIndex(0)!;
        tabs.SelectedIndex = 0;
        ControlPreview.Flush();
        var closingCount = 0;
        EventHandler<TabClosingEventArgs> cancel = (_, args) =>
        {
            closingCount++;
            args.Cancel = true;
        };
        tabs.AddHandler(TabFeatures.TabClosingEvent, cancel);
        Click(window, Part<Button>(closeItem, "PART_CloseButton"));
        Check(collection.Count == 3 && closingCount == 1, "Close event can cancel a real close-button click");
        tabs.RemoveHandler(TabFeatures.TabClosingEvent, cancel);
        Click(window, Part<Button>(closeItem, "PART_CloseButton"));
        ControlPreview.Flush();
        Check(collection.Count == 2 && collection[1] == initial, "Close-button click removes only its bound data item");
        tabs.SelectedIndex = 0;
        ((TabItem)tabs.ContainerFromIndex(0)!).Focus();
        PressKey(window, Key.W, PhysicalKey.W, RawInputModifiers.Control);
        ControlPreview.Flush();
        Check(collection.Count == 1 && collection[0] == initial, "Ctrl+W closes selected card tab");
        Check(!TabFeatures.Close((TabItem)tabs.ContainerFromIndex(0)!),
            "Pinned tab also rejects keyboard and programmatic close");
        window.Model.ResetTabsCommand.Execute(null);
        ControlPreview.Flush();
        tabs.SelectedIndex = 1;
        ((TabItem)tabs.ContainerFromIndex(1)!).Focus();
        var current = tabs.SelectedItem;
        PressKey(window, Key.Right, PhysicalKey.ArrowRight, RawInputModifiers.Control | RawInputModifiers.Shift);
        Check(tabs.SelectedIndex == 2 && ReferenceEquals(tabs.SelectedItem, current),
            "Keyboard reorder preserves selected data item");
        window.Model.ResetTabsCommand.Execute(null);
        tabs.SelectedIndex = 1;
        ControlPreview.Flush();
    }

    private static void VerifyDate(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        var picker = gallery.FindControl<CalendarDatePicker>("EditableDate")!;
        var text = Part<TextBox>(picker, "PART_TextBox");
        Click(window, text);
        ControlPreview.Flush();
        Check(picker.IsDropDownOpen, "Click in date text editor opens the real calendar popup");
        var popup = Part<Popup>(picker, "PART_Popup");
        var calendar = (Calendar)popup.Child!;
        Check(
            calendar.Bounds.Width > 250 && calendar.Bounds.Height > 250 &&
            Colour(calendar.Background) == Color.Parse("#FFFFFF"), "Calendar popup renders the complete theme");
        PressKey(TopLevel.GetTopLevel(calendar)!, Key.Escape, PhysicalKey.Escape);
        ControlPreview.Flush();
        Check(!picker.IsDropDownOpen && text.IsFocused, "Escape from the calendar returns focus to manual date input");
        picker.IsDropDownOpen = true;
        ControlPreview.Flush();
        var item = Part<CalendarItem>(calendar, "PART_CalendarItem");
        var next = Part<Button>(item, "PART_NextButton");
        var before = calendar.DisplayDate;
        Click(TopLevel.GetTopLevel(next)!, next);
        ControlPreview.Flush();
        Check(calendar.DisplayDate.Month != before.Month, "Calendar next-month navigation works");
        var header = Part<Button>(item, "PART_HeaderButton");
        Click(TopLevel.GetTopLevel(header)!, header);
        ControlPreview.Flush();
        Check(calendar.DisplayMode == CalendarMode.Year, "Calendar header switches to month selection");
        calendar.DisplayMode = CalendarMode.Month;
        picker.IsDropDownOpen = false;
        ControlPreview.Flush();
        text.Focus(NavigationMethod.Tab);
        text.SelectAll();
        window.KeyTextInput("2026-11-23");
        PressKey(window, Key.Enter, PhysicalKey.Enter);
        ControlPreview.Flush();
        Check(picker.SelectedDate?.Date == new DateTime(2026, 11, 23),
            "Manual ISO date input commits using native parser");
        var error = false;
        picker.DateValidationError += (_, _) => error = true;
        text.SelectAll();
        window.KeyTextInput("invalid date");
        PressKey(window, Key.Enter, PhysicalKey.Enter);
        ControlPreview.Flush();
        Check(error && picker.SelectedDate?.Date == new DateTime(2026, 11, 23),
            "Invalid manual input raises validation and preserves selected date");
        text.SelectAll();
        window.KeyTextInput("2026-10-12");
        PressKey(window, Key.Enter, PhysicalKey.Enter);
        picker.IsDropDownOpen = true;
        ControlPreview.Flush();
        window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 2;
        ControlPreview.Flush();
        Check(Colour(calendar.Background) == Color.Parse("#0F0F0F"), "Open calendar follows a live dark-theme switch");
        window.FindControl<ComboBox>("ThemePicker")!.SelectedIndex = 1;
        picker.IsDropDownOpen = false;
        ControlPreview.Flush();
    }

    private static void VerifyMotion(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        var expander = gallery.FindControl<Expander>("MotionExpander")!;
        var reveal = Part<ExpanderRevealHost>(expander, "ExpanderContent");
        var expandedHeight = reveal.Bounds.Height;
        Motion.SetEnabled(window, true);
        var slider = gallery.FindControl<Slider>("RefinedSlider")!;
        var thumb = slider.GetVisualDescendants().OfType<Thumb>().Single();
        var beforeHover = Colour(thumb.Background);
        window.MouseMove(Centre(thumb, window));
        ControlPreview.Flush();
        AdvanceUntil(() => Colour(thumb.Background) != beforeHover, 1500);
        Check(Colour(thumb.Background) != beforeHover && Colour(thumb.Background) != Color.Parse("#006A6B"),
            "Hover brush transition renders a real intermediate colour");
        AdvanceUntil(() => Colour(thumb.Background) == Color.Parse("#006A6B"), 1500);
        Check(Colour(thumb.Background) == Color.Parse("#006A6B"),
            "Hover transition settles on the semantic hover colour");
        window.MouseMove(new Point(1, 1));
        expander.IsExpanded = false;
        ControlPreview.Flush();
        Check(reveal.IsVisible && !reveal.IsHitTestVisible,
            "Collapse keeps content visible for exit motion and blocks input");
        Advance(70);
        Check(
            reveal.Bounds.Height > 0 && reveal.Bounds.Height < expandedHeight && reveal.Opacity == 1 &&
            reveal.RenderTransform == null, "Collapse reduces the viewport without moving or fading borders");
        AdvanceUntil(() => !reveal.IsVisible, 1500);
        Check(!reveal.IsVisible, "Collapse removes content after exit animation");
        expander.IsExpanded = true;
        ControlPreview.Flush();
        Advance(50);
        Check(reveal.IsVisible && reveal.Bounds.Height > 0 && reveal.Bounds.Height < expandedHeight,
            "Expansion grows the clipped viewport through intermediate sizes");
        expander.IsExpanded = false;
        expander.IsExpanded = true;
        Advance(260);
        AdvanceUntil(() => reveal.IsVisible && Math.Abs(reveal.Bounds.Height - expandedHeight) < 0.5, 1500);
        Check(reveal.IsVisible && Math.Abs(reveal.Bounds.Height - expandedHeight) < 0.5 && reveal.IsHitTestVisible,
            "Rapid toggles cancel stale animation and finish in the latest state");
        var tree = gallery.FindControl<TreeView>("MotionTree")!;
        var root = (TreeViewItem)tree.Items[0]!;
        var branch = Part<RevealHost>(root, "TreeContent");
        root.IsExpanded = false;
        ControlPreview.Flush();
        Check(branch.IsVisible && !branch.IsHitTestVisible,
            "TreeView clips shrinking content and blocks input during collapse");
        AdvanceUntil(() => !branch.IsVisible, 1500);
        Check(!branch.IsVisible, "Tree branch is hidden after collapse completes");
        Motion.SetEnabled(window, false);
        root.IsExpanded = true;
        expander.IsExpanded = false;
        ControlPreview.Flush();
        Check(branch.IsVisible && !reveal.IsVisible && reveal.Transitions == null,
            "Reduced motion disables both animated content and state transitions");
        expander.IsExpanded = true;
        ControlPreview.Flush();
        var directionPicker = gallery.FindControl<ComboBox>("MotionDirectionPicker")!;
        for (var index = 0; index < 4; index++)
        {
            directionPicker.SelectedIndex = index;
            ControlPreview.Flush();
            Check(expander.ExpandDirection == (ExpandDirection)index,
                $"Preview direction picker switches to {(ExpandDirection)index}");
        }

        directionPicker.SelectedIndex = 0;
        ControlPreview.Flush();
        VerifyExpanderMotion();
        TreeMotionPreview.Verify(window, gallery);
    }

    internal static void VerifyExpanderMotion()
    {
        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var direction in Enum.GetValues<ExpandDirection>())
        {
            var probe = new MeasureProbe { Width = 180, Height = 120 };
            var expander = new Expander
            {
                Header = "Continuous frame", Content = probe, ExpandDirection = direction, IsExpanded = true,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var panel = new StackPanel { Spacing = 8, Margin = new Thickness(20) };
            panel.Children.Add(expander);
            var sibling = new Border { Height = 12, Background = Brushes.Gray };
            panel.Children.Add(sibling);
            var window = new Window
            {
                Width = 540, Height = 400, Content = panel, RequestedThemeVariant = variant,
                WindowDecorations = WindowDecorations.None
            };
            Motion.SetEnabled(window, false);
            window.Show();
            ControlPreview.Flush();
            var viewport = Part<ExpanderRevealHost>(expander, "ExpanderContent");
            Check(viewport.Duration == TimeSpan.FromMilliseconds(200),
                $"{variant.Key}/{direction}: default reveal duration remains 200 ms");
            // Give four real layout samples and keyboard dispatch sufficient room on
            // loaded renderers without changing the production duration.
            viewport.Duration = TimeSpan.FromMilliseconds(400);
            var frame = Part<Border>(expander, "ExpanderFrame");
            var header = Part<ToggleButton>(expander, "ExpanderHeader");
            var joinedRadius = header.CornerRadius;
            var horizontal = direction is ExpandDirection.Left or ExpandDirection.Right;

            double Extent()
            {
                return horizontal ? viewport.Bounds.Width : viewport.Bounds.Height;
            }

            var fullExtent = Extent();
            var label = $"{variant.Key}/{direction}";
            Motion.SetEnabled(window, true);
            ControlPreview.Flush();
            Check(Math.Abs(Extent() - fullExtent) < 0.5, $"{label}: enabling motion does not replay settled content");
            var measureCount = probe.MeasureCount;
            var siblingStart = sibling.Bounds.Top;
            expander.IsExpanded = false;
            // Flush advances the real animation clock; after a slow dispatcher turn the
            // first observed layout can already be partway through the 140 ms collapse.
            Check(viewport.IsVisible && !viewport.IsHitTestVisible && Math.Abs(Extent() - fullExtent) < 0.5,
                $"{label}: closing retains the current viewport before the animation clock advances");
            ControlPreview.Flush();
            Check(header.CornerRadius == joinedRadius && header.Classes.Contains("joined"),
                $"{label}: closing retains joined header corners");
            var sizes = new List<double>();
            for (var i = 0; i < 4; i++)
            {
                Advance(20);
                sizes.Add(Extent());
            }

            Check(
                sizes.Distinct().Count() >= 3 && sizes[0] < fullExtent && sizes[^1] > 0 &&
                sizes.Zip(sizes.Skip(1)).All(pair => pair.First >= pair.Second),
                $"{label}: collapse emits multiple monotonically shrinking layout frames");
            Check(
                frame.Opacity == 1 && frame.RenderTransform == null && frame.BorderThickness == new Thickness(1) &&
                frame.CornerRadius == expander.CornerRadius && frame.ClipToBounds,
                $"{label}: a single opaque rounded frame owns all outside edges");
            Check(probe.MeasureCount - measureCount <= 1,
                $"{label}: animation reuses the child measure instead of reflowing it per frame");
            if (!horizontal)
                Check(
                    sibling.Bounds.Top < siblingStart &&
                    sibling.Bounds.Top > expander.Bounds.Top + header.Bounds.Height,
                    $"{label}: following content moves during collapse instead of jumping at completion");
            var reversedAt = Extent();
            var progressProperty = AvaloniaPropertyRegistry.Instance.FindRegistered(viewport, "Progress")!;
            var progressAtReversal = (double)viewport.GetValue(progressProperty)!;
            double? firstAnimatedValue = null;

            void ObserveReversal(object? sender, AvaloniaPropertyChangedEventArgs change)
            {
                if (change.Property == progressProperty && change.Priority == BindingPriority.Animation)
                    firstAnimatedValue ??= (double)change.NewValue!;
            }

            viewport.PropertyChanged += ObserveReversal;
            expander.IsExpanded = true;
            ControlPreview.Flush();
            viewport.PropertyChanged -= ObserveReversal;
            // Compare the first actual animation value, not its temporary underlying
            // target or the last value after a render tick with variable elapsed time.
            Check(firstAnimatedValue.HasValue && Math.Abs(firstAnimatedValue.Value - progressAtReversal) < 0.000001,
                $"{label}: reversal retains the exact current value in its first animation frame (before {progressAtReversal}, first {firstAnimatedValue})");
            Check(Extent() >= reversedAt - 0.5 && Extent() <= fullExtent + 0.5,
                $"{label}: the next reversal frame progresses toward the expanded extent without moving backward");
            AdvanceUntil(() => Math.Abs(Extent() - fullExtent) < 0.5, 1500);
            Check(viewport.IsVisible && viewport.IsHitTestVisible && Math.Abs(Extent() - fullExtent) < 0.5,
                $"{label}: reverse animation finishes fully expanded");
            expander.IsExpanded = false;
            ControlPreview.Flush();
            Advance(20);
            Motion.SetEnabled(window, false);
            ControlPreview.Flush();
            Check(
                !viewport.IsVisible && !header.Classes.Contains("joined") &&
                header.CornerRadius == expander.CornerRadius,
                $"{label}: disabling motion mid-flight restores collapsed geometry immediately");
            Motion.SetEnabled(window, true);
            expander.IsExpanded = true;
            ControlPreview.Flush();
            AdvanceUntil(() => Math.Abs(Extent() - fullExtent) < 0.5, 1500);
            expander.IsExpanded = false;
            ControlPreview.Flush();
            AdvanceUntil(() => !viewport.IsVisible, 1500);
            Check(!viewport.IsVisible && header.CornerRadius == expander.CornerRadius,
                $"{label}: natural collapse ends with complete header corners");
            Motion.SetEnabled(window, false);
            expander.Width = 420;
            expander.Height = 240;
            expander.IsExpanded = true;
            ControlPreview.Flush();
            var fixedExtent = Extent();
            Motion.SetEnabled(window, true);
            expander.IsExpanded = false;
            ControlPreview.Flush();
            Advance(50);
            Check(
                Extent() > 0 && Extent() < fixedExtent && expander.Bounds.Width == 420 && expander.Bounds.Height == 240,
                $"{label}: explicit outer dimensions still permit clipped content motion");
            Motion.SetEnabled(window, false);
            expander.BorderThickness = new Thickness(2);
            expander.Width = double.NaN;
            expander.Height = double.NaN;
            ControlPreview.Flush();
            Check(expander.Bounds.Height == expander.MinHeight,
                $"{label}: custom frame thickness preserves collapsed density");
            window.Close();
        }
    }

    private static void VerifySizingMatrix()
    {
        Control[] controls =
        [
            new Button { Content = "Button" }, new ToggleButton { Content = "ToggleButton" },
            new RepeatButton { Content = "RepeatButton" },
            new TextBox { Text = "TextBox" },
            new TextBox { Text = "Clear button", Classes = { "clearButton" } },
            new TextBox { Text = "Password", PasswordChar = '●', Classes = { "revealPasswordButton" } },
            new ComboBox { ItemsSource = new[] { "ComboBox" }, SelectedIndex = 0 },
            new AutoCompleteBox { Text = "AutoCompleteBox" }, new NumericUpDown { Value = 12 },
            new CheckBox { Content = "CheckBox" }, new RadioButton { Content = "RadioButton" }, new ToggleSwitch(),
            new Slider(),
            new CalendarDatePicker(), new DatePicker(), new TimePicker(), new TabItem { Header = "TabItem" },
            new ListBoxItem { Content = "ListBoxItem" }, new TreeViewItem { Header = "TreeViewItem" },
            new MenuItem { Header = "MenuItem" },
            new Expander { Header = "Expander", Content = "Content" }
        ];
        var panel = new StackPanel { Spacing = 4 };
        foreach (var control in controls) panel.Children.Add(control);
        var window = new Window
            { Width = 360, Height = 1100, Content = panel, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        foreach (var (size, height) in new[]
                     { (ControlSize.Small, 30), (ControlSize.Large, 44), (ControlSize.Normal, 36) })
        {
            ControlSizing.SetSize(panel, size);
            ControlPreview.Flush();
            foreach (var control in controls)
            {
                if (control is TextBox editor && editor.Classes.Contains("clearButton"))
                {
                    editor.Focus(NavigationMethod.Tab);
                    ControlPreview.Flush();
                }

                if (control is TreeViewItem node)
                    Check(Math.Abs(Part<Panel>(node, "PART_ExpandCollapseChevronContainer").Bounds.Width - height) < 1,
                        $"Tree leaves keep their chevron column aligned in {size} density");
                Check(control is TemplatedControl themed && themed.MinHeight == height
                                                         && Math.Abs(control.Bounds.Height - height) <= 1,
                    $"{control.GetType().Name} supports inherited {size} density without layout overflow (actual {control.Bounds.Height})");
            }
        }

        window.Close();
    }

    private static void VerifyTabCollections()
    {
        var tabs = new TabControl { Classes = { "card" }, ItemsSource = new[] { "Fixed A", "Fixed B" } };
        var window = new Window
            { Width = 400, Height = 240, Content = tabs, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        var first = (TabItem)tabs.ContainerFromIndex(0)!;
        Check(
            !Part<Button>(first, "PART_CloseButton").IsEffectivelyEnabled && !TabFeatures.Move(tabs, 0, 1) &&
            !TabFeatures.Close(first), "Fixed-size ItemsSource disables close and rejects collection mutations");
        tabs.ItemsSource = null;
        var a = new TabItem { Header = "A", Content = "First" };
        var b = new TabItem { Header = "B", Content = "Second" };
        tabs.Items.Add(a);
        tabs.Items.Add(b);
        ControlPreview.Flush();
        Check(TabFeatures.Move(tabs, 0, 1) && ReferenceEquals(tabs.Items[1], a),
            "Manually declared TabItems support the same reorder behaviour");
        Check(TabFeatures.Close(a) && tabs.Items.Count == 1, "Manual TabItem close removes only the requested item");
        Check(TabFeatures.Close(b) && tabs.Items.Count == 0 && tabs.SelectedIndex == -1,
            "Last tab can close cleanly into an empty state");
        window.Close();
    }

    public static void Render(LegacyPreviewWindow window, LegacyControlGallery gallery, string output, bool dark)
    {
        window.NavigateToCategory(8);
        ControlPreview.Flush();
        Save(window, Path.Combine(output, $"refinements-{(dark ? "dark" : "light")}.png"));
        var picker = gallery.FindControl<CalendarDatePicker>("EditableDate")!;
        picker.IsDropDownOpen = true;
        ControlPreview.Flush();
        Save(Part<Popup>(picker, "PART_Popup").Child!,
            Path.Combine(output, $"calendar-popup-{(dark ? "dark" : "light")}.png"));
        picker.IsDropDownOpen = false;
        ControlPreview.Flush();
        RenderExpanderFrames(output, dark);
        TreeMotionPreview.Render(output, dark);
        CardTabPreview.Render(output, dark);
    }

    private static void RenderExpanderFrames(string output, bool dark)
    {
        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*"), RowDefinitions = new RowDefinitions("*,*"),
            Margin = new Thickness(24)
        };
        var expanders = new List<Expander>();
        foreach (var direction in Enum.GetValues<ExpandDirection>())
        {
            var body = new StackPanel { Spacing = 8, Width = 160 };
            body.Children.Add(new TextBlock { Text = "TheOne · OneTheme", FontWeight = FontWeight.SemiBold });
            body.Children.Add(new TextBlock { Text = "完整内容保持布局，边框连续连接。", TextWrapping = TextWrapping.Wrap });
            body.Children.Add(new Button { Content = "主题控件", HorizontalAlignment = HorizontalAlignment.Left });
            var expander = new Expander
            {
                Header = direction.ToString(), Content = body, ExpandDirection = direction,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            var cell = new StackPanel { Spacing = 12, Margin = new Thickness(12) };
            cell.Children.Add(new TextBlock { Text = $"{direction} · continuous frame", FontSize = 14 });
            cell.Children.Add(expander);
            Grid.SetRow(cell, (int)direction / 2);
            Grid.SetColumn(cell, (int)direction % 2);
            grid.Children.Add(cell);
            expanders.Add(expander);
        }

        var window = new Window
        {
            Width = 880, Height = 500, Content = grid,
            RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light,
            WindowDecorations = WindowDecorations.None
        };
        Motion.SetEnabled(window, false);
        window.Show();
        ControlPreview.Flush();
        var theme = dark ? "dark" : "light";
        Motion.SetEnabled(window, true);
        foreach (var expander in expanders) expander.IsExpanded = true;
        Advance(50);
        Save(window, Path.Combine(output, $"expander-{theme}-opening.png"));
        Advance(210);
        Save(window, Path.Combine(output, $"expander-{theme}-open.png"));
        foreach (var expander in expanders) expander.IsExpanded = false;
        Advance(50);
        Save(window, Path.Combine(output, $"expander-{theme}-closing.png"));
        Advance(160);
        Save(window, Path.Combine(output, $"expander-{theme}-closed.png"));
        window.Close();
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>().Single(control =>
            control.Name == name && ReferenceEquals(control.TemplatedParent, owner));
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Point Centre(Control control, TopLevel window)
    {
        return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
    }

    private static void Click(TopLevel window, Control control)
    {
        var point = Centre(control, window);
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static void PressKey(TopLevel window, Key key, PhysicalKey physical,
        RawInputModifiers modifiers = RawInputModifiers.None)
    {
        window.KeyPress(key, modifiers, physical, null);
        window.KeyRelease(key, modifiers, physical, null);
        ControlPreview.Flush();
    }

    internal static void Advance(int milliseconds)
    {
        // Transitions use the UI clock; pump the dispatcher instead of blocking it.
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            frame.Continue = false;
        };
        timer.Start();
        Dispatcher.UIThread.PushFrame(frame);
        ControlPreview.Flush();
    }

    internal static void AdvanceUntil(Func<bool> predicate, int timeout)
    {
        var timer = Stopwatch.StartNew();
        while (!predicate() && timer.ElapsedMilliseconds < timeout) Advance(10);
    }

    internal static void Save(Control control, string path)
    {
        using var bitmap =
            new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)),
                new Vector(96, 96));
        bitmap.Render(control);
        bitmap.Save(path, PngBitmapEncoderOptions.Default);
        Console.WriteLine($"Rendered {path}");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException($"Failed: {message}");
        Console.WriteLine($"PASS: {message}");
    }

    private sealed class MeasureProbe : Control
    {
        public int MeasureCount { get; private set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            MeasureCount++;
            return new Size(180, 120);
        }
    }
}