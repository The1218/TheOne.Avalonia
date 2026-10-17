using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
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

/// <summary>Isolated checks for closing native TabStrip headers, including recycled containers.</summary>
internal static class TabStripClosePreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyPointerAndKeyboard(check);
        VerifyCommandsAndSources(check);
        VerifyValueTypeItems(check);
        VerifyDisabledAndPinned(check);
        VerifyRecycling(check);
        VerifyTabControlCompatibility(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyChrome(check, output, theme, size);
    }

    private static void VerifyPointerAndKeyboard(Action<bool, string> check)
    {
        var rows = Rows(4);
        var (window, strip) = Open(rows);
        try
        {
            var initial = Item(strip, 0);
            check(strip.GetType() == typeof(TabStrip) && initial.GetType() == typeof(TabStripItem)
                                                      && !TabFeatures.GetCanClose(strip) &&
                                                      !CloseButton(initial).IsVisible && !TabFeatures.Close(initial),
                "TabStrip/close: native headers keep closing opt-in and have no close hit target by default");
            TabFeatures.SetCanClose(strip, true);
            Flush();
            check(TabFeatures.GetShowCloseButton(initial) && CloseButton(initial).IsVisible,
                "TabStrip/close: enabling CanClose updates an already realized native header");

            var attempts = 0;
            object? requested = null;
            EventHandler<TabClosingEventArgs> cancel = (_, args) =>
            {
                attempts++;
                requested = args.Item;
                args.Cancel = true;
            };
            strip.AddHandler(TabFeatures.TabClosingEvent, cancel);
            Click(window, CloseButton(initial));
            check(rows.Count == 4 && attempts == 1 && ReferenceEquals(requested, rows[0]),
                "TabStrip/close: routed TabClosing cancels a real close-button click with the bound data item");
            strip.RemoveHandler(TabFeatures.TabClosingEvent, cancel);

            var selected = rows[2];
            strip.SelectedItem = selected;
            Flush();
            initial = Item(strip, 0);
            window.MouseMove(Centre(initial, window));
            Flush();
            Click(window, CloseButton(initial));
            check(
                rows.Count == 3 && ReferenceEquals(strip.SelectedItem, selected) &&
                !rows.Contains((TabValue)requested!),
                "TabStrip/close: clicking an unselected header's close button preserves the current data selection");

            var next = rows[2];
            Item(strip, strip.SelectedIndex).Focus(NavigationMethod.Tab);
            Flush();
            Press(window, RawInputModifiers.Control);
            check(rows.Count == 2 && ReferenceEquals(strip.SelectedItem, next),
                "TabStrip/close: Ctrl+W closes the selected data item and selects its next neighbor");
            check(ReferenceEquals(window.FocusManager?.GetFocusedElement(), Item(strip, strip.SelectedIndex)),
                "TabStrip/close: keyboard close restores focus to the surviving selected native header");

            Item(strip, strip.SelectedIndex).Focus();
            Flush();
            Press(window, RawInputModifiers.Meta);
            check(rows.Count == 1 && ReferenceEquals(strip.SelectedItem, rows[0]),
                "TabStrip/close: Meta+W closes the last-position header and selects its previous neighbor");
            Click(window, CloseButton(Item(strip, 0)));
            check(rows.Count == 0 && strip.SelectedIndex == -1 && strip.SelectedItem == null,
                "TabStrip/close: closing the final header leaves a safe empty selection");
            rows.Add(new TabValue(10));
            Flush();
            check(strip.SelectedIndex == 0 && CloseButton(Item(strip, 0)).IsEffectivelyVisible
                                           && TabFeatures.CloseTabCommand.CanExecute(Item(strip, 0)),
                "TabStrip/close: adding a document after the final close restores native selection and closing behavior");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyCommandsAndSources(Action<bool, string> check)
    {
        var rows = Rows(2);
        var (window, strip) = Open(rows);
        try
        {
            TabFeatures.SetCanClose(strip, true);
            var command = new CloseProbe(parameter => rows.Remove((TabValue)parameter!));
            TabFeatures.SetCloseCommand(strip, command);
            Flush();
            var first = Item(strip, 0);
            var data = rows[0];
            check(!TabFeatures.CloseTabCommand.CanExecute(first) && !CloseButton(first).IsEffectivelyEnabled
                                                                 && !TabFeatures.Close(first) &&
                                                                 command.Executions == 0 && rows.Count == 2,
                "TabStrip/close: consumer CanExecute rejects both the native close button and programmatic close");
            command.Allowed = true;
            Flush();
            check(CloseButton(first).IsEffectivelyEnabled && TabFeatures.CloseTabCommand.CanExecute(first),
                "TabStrip/close: consumer CanExecuteChanged refreshes the realized close button");
            Click(window, CloseButton(first));
            check(command.Executions == 1 && ReferenceEquals(command.Parameter, data) && rows.Count == 1,
                "TabStrip/close: close command receives the bound data model once and owns collection mutation");

            TabFeatures.SetCloseCommand(strip, null);
            Flush();
            check(TabFeatures.Close(Item(strip, 0)) && rows.Count == 0,
                "TabStrip/close: clearing CloseCommand restores default mutable-source removal");
        }
        finally
        {
            window.Close();
        }

        foreach (var fixedSize in new[] { false, true })
        {
            var values = Rows(2).ToArray();
            IList source = fixedSize ? values : Array.AsReadOnly(values);
            (window, strip) = Open(source);
            try
            {
                TabFeatures.SetCanClose(strip, true);
                Flush();
                var first = Item(strip, 0);
                check(!TabFeatures.Close(first) && !TabFeatures.CloseTabCommand.CanExecute(first) && source.Count == 2,
                    $"TabStrip/close: {(fixedSize ? "fixed-size" : "read-only")} sources safely reject default removal");
                var probe = new CloseProbe { Allowed = true };
                TabFeatures.SetCloseCommand(strip, probe);
                Flush();
                check(
                    TabFeatures.Close(first) && probe.Executions == 1 && ReferenceEquals(probe.Parameter, values[0]) &&
                    source.Count == 2,
                    $"TabStrip/close: {(fixedSize ? "fixed-size" : "read-only")} sources can delegate closing to a consumer command");
            }
            finally
            {
                window.Close();
            }
        }

        var explicitStrip = new TabStrip();
        var explicitItem = new TabStripItem { Content = "Explicit document" };
        explicitStrip.Items.Add(explicitItem);
        TabFeatures.SetCanClose(explicitStrip, true);
        window = Show(explicitStrip, ThemeVariant.Light, ControlSize.Normal, 560);
        try
        {
            var probe = new CloseProbe { Allowed = true };
            TabFeatures.SetCloseCommand(explicitStrip, probe);
            Flush();
            Click(window, CloseButton(explicitItem));
            check(probe.Executions == 1 && ReferenceEquals(probe.Parameter, explicitItem),
                "TabStrip/close: explicit native TabStripItem collections pass their actual item to CloseCommand");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyDisabledAndPinned(Action<bool, string> check)
    {
        var rows = Rows(3);
        var (window, strip) = Open(rows);
        try
        {
            TabFeatures.SetCanClose(strip, true);
            Flush();
            var first = Item(strip, 0);
            TabFeatures.SetIsClosable(first, false);
            Flush();
            check(!TabFeatures.GetShowCloseButton(first) && !CloseButton(first).IsVisible && !TabFeatures.Close(first),
                "TabStrip/close: a pinned header hides the close button and rejects removal");
            first.Focus();
            Press(window, RawInputModifiers.Control);
            check(rows.Count == 3 && strip.SelectedIndex == 0,
                "TabStrip/close: keyboard close respects IsClosable");
            TabFeatures.SetIsClosable(first, true);
            Flush();
            first.IsEnabled = false;
            Flush();
            check(!TabFeatures.Close(first) && !TabFeatures.CloseTabCommand.CanExecute(first) && rows.Count == 3,
                "TabStrip/close: a disabled native header rejects close");
            first.IsEnabled = true;
            strip.IsEnabled = false;
            Flush();
            check(!TabFeatures.Close(first) && !TabFeatures.CloseTabCommand.CanExecute(first) && rows.Count == 3,
                "TabStrip/close: a disabled owner rejects close");
            strip.IsEnabled = true;
            TabFeatures.SetCanClose(strip, false);
            Flush();
            check(!CloseButton(first).IsVisible && !TabFeatures.Close(first),
                "TabStrip/close: disabling CanClose removes the affordance from existing headers");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyValueTypeItems(Action<bool, string> check)
    {
        var rows = new ObservableCollection<int> { 11, 22, 33 };
        var strip = new TabStrip
        {
            ItemsSource = rows,
            ItemTemplate = new FuncDataTemplate<int>((value, _) => new TextBlock { Text = value.ToString() })
        };
        TabFeatures.SetCanClose(strip, true);
        var window = Show(strip, ThemeVariant.Light, ControlSize.Normal, 620);
        try
        {
            strip.SelectedIndex = 1;
            Flush();
            object? requested = null;
            strip.AddHandler(TabFeatures.TabClosingEvent, (_, args) => requested = args.Item);
            var closed = TabFeatures.Close(Item(strip, 1));
            Flush();
            check(closed && rows.SequenceEqual(new[] { 11, 33 }) && requested is 22 && Equals(strip.SelectedItem, 33),
                "TabStrip/close: boxed value-type items remain closable across repeated data lookup and select the correct neighbor");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyRecycling(Action<bool, string> check)
    {
        var rows = Rows(10_000);
        var (window, strip) = Open(rows, width: 620);
        try
        {
            TabFeatures.SetCanClose(strip, true);
            Flush();
            var initial = strip.GetRealizedContainers().ToArray();
            var target = rows[8999];
            var neighbor = rows[9000];
            strip.SelectedIndex = 8999;
            Flush();
            var distant = Item(strip, 8999);
            check(
                strip.ItemsPanelRoot is VirtualizingStackPanel && strip.GetRealizedContainers().Count() is > 0 and < 80
                                                               && initial.Intersect(strip.GetRealizedContainers())
                                                                   .Any(),
                "TabStrip/close: 10,000 headers retain bounded native realization and recycle existing containers");
            object? requested = null;
            strip.AddHandler(TabFeatures.TabClosingEvent, (_, args) => requested = args.Item);
            Click(window, CloseButton(distant));
            check(rows.Count == 9999 && !rows.Contains(target) && ReferenceEquals(requested, target)
                  && ReferenceEquals(strip.SelectedItem, neighbor),
                "TabStrip/close: a recycled close button removes its current data item and selects the correct neighbor");
            strip.SelectedIndex = 0;
            Flush();
            var first = rows[0];
            Click(window, CloseButton(Item(strip, 0)));
            check(rows.Count == 9998 && !rows.Contains(first) && ReferenceEquals(requested, first)
                  && strip.GetRealizedContainers().Count() < 80,
                "TabStrip/close: returning recycled headers to the first viewport does not retain a stale close parameter");

            strip.Focusable = true;
            var offscreenShortcuts = true;
            foreach (var modifiers in new[] { RawInputModifiers.Control, RawInputModifiers.Meta })
            {
                strip.SelectedIndex = 0;
                Flush();
                var offscreen = rows[0];
                var count = rows.Count;
                strip.Focus(NavigationMethod.Tab);
                Flush();
                strip.ScrollIntoView(rows.Count - 1);
                Flush();
                var selectedWasRecycled = strip.ContainerFromIndex(0) == null && strip.IsKeyboardFocusWithin &&
                                          strip.SelectedIndex == 0;
                requested = null;
                Press(window, modifiers);
                offscreenShortcuts &= selectedWasRecycled && rows.Count == count - 1 && !rows.Contains(offscreen)
                                      && ReferenceEquals(requested, offscreen);
            }

            check(offscreenShortcuts,
                "TabStrip/close: Ctrl+W and Meta+W close the selected data item after its header scrolls away and is recycled");

            strip.SelectedIndex = 0;
            Flush();
            var oldItem = rows[0];
            var oldCount = rows.Count;
            var replacement = new ObservableCollection<TabValue> { new(oldItem.Index), new(oldItem.Index + 1) };
            var command = new CloseProbe { Allowed = true };
            TabFeatures.SetCloseCommand(strip, command);
            Flush();
            var recycled = Item(strip, 0);
            EventHandler<TabClosingEventArgs> replaceSource = (_, _) =>
            {
                strip.ItemsSource = replacement;
                strip.SelectedIndex = 0;
                Flush();
            };
            strip.AddHandler(TabFeatures.TabClosingEvent, replaceSource);
            var closed = TabFeatures.Close(recycled);
            Flush();
            strip.RemoveHandler(TabFeatures.TabClosingEvent, replaceSource);
            check(!closed && command.Executions == 0 && rows.Count == oldCount && replacement.Count == 2
                  && ReferenceEquals(strip.SelectedItem, replacement[0]),
                "TabStrip/close: replacing ItemsSource during TabClosing cannot invoke a stale command after container recycling, even for equal-valued new models");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyChrome(Action<bool, string> check, string? output, ThemeVariant theme, ControlSize size)
    {
        var rows = Rows(3);
        var (window, strip) = Open(rows, theme, size, 360);
        try
        {
            TabFeatures.SetCanClose(strip, true);
            Flush();
            var first = Item(strip, 0);
            first.Width = 190;
            Flush();
            var close = CloseButton(first);
            var presenter = Part<ContentPresenter>(first, "PART_ContentPresenter");
            var closeOrigin = close.TranslatePoint(default, first)!.Value;
            var presenterOrigin = presenter.TranslatePoint(default, first)!.Value;
            var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
            var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            var label = $"TabStrip/close/{theme.Key}/{size}";
            check(first.MinHeight == height && first.FontSize == font && close.IsEffectivelyVisible
                  && close.Bounds.Width >= 16 && close.Bounds.Height >= 16 && closeOrigin.Y >= 0
                  && closeOrigin.Y + close.Bounds.Height <= first.Bounds.Height + .5,
                $"{label}: native density keeps an accessible close target inside the header");
            check(presenterOrigin.X >= 0 && presenterOrigin.X + presenter.Bounds.Width <= closeOrigin.X + .5
                                         && closeOrigin.X + close.Bounds.Width <= first.Bounds.Width + .5 &&
                                         strip.Bounds.Width <= 328,
                $"{label}: compact headers reserve separate title and close columns inside the horizontal viewport");
            if (output != null)
            {
                Directory.CreateDirectory(output);
                using var bitmap =
                    new RenderTargetBitmap(PixelSize.FromSize(window.Bounds.Size, 1), new Vector(96, 96));
                bitmap.Render(window);
                bitmap.Save(
                    Path.Combine(output,
                        $"tabstrip-close-{theme.Key!.ToString()!.ToLowerInvariant()}-{size.ToString().ToLowerInvariant()}.png"),
                    PngBitmapEncoderOptions.Default);
            }

            Click(window, close);
            check(rows.Count == 2 && strip.SelectedIndex == 0,
                $"{label}: native close remains operable in the compact theme and density layout");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyTabControlCompatibility(Action<bool, string> check)
    {
        var rows = Rows(3);
        var tabs = new TabControl
        {
            ItemsSource = rows,
            ItemTemplate = new FuncDataTemplate<TabValue>((value, _) => new TextBlock { Text = value?.ToString() })
        };
        TabFeatures.SetCanClose(tabs, true);
        var window = Show(tabs, ThemeVariant.Light, ControlSize.Normal, 620);
        try
        {
            var first = (TabItem)tabs.ContainerFromIndex(0)!;
            var data = rows[0];
            var attempts = 0;
            EventHandler<TabClosingEventArgs> cancel = (_, args) =>
            {
                attempts++;
                args.Cancel = true;
            };
            tabs.AddHandler(TabFeatures.TabClosingEvent, cancel);
            check(!TabFeatures.Close(first) && attempts == 1 && rows.Count == 3,
                "TabFeatures/compat: native TabControl still raises and respects cancelable TabClosing");
            tabs.RemoveHandler(TabFeatures.TabClosingEvent, cancel);
            var command = new CloseProbe(parameter => rows.Remove((TabValue)parameter!));
            TabFeatures.SetCloseCommand(tabs, command);
            Flush();
            check(!TabFeatures.CloseTabCommand.CanExecute(first) && !TabFeatures.Close(first) && rows.Count == 3,
                "TabFeatures/compat: native TabItem close still respects the consumer command's CanExecute");
            command.Allowed = true;
            Flush();
            check(
                TabFeatures.Close(first) && command.Executions == 1 && ReferenceEquals(command.Parameter, data) &&
                rows.Count == 2,
                "TabFeatures/compat: native TabControl still delegates closing with its actual data model");
            TabFeatures.SetCloseCommand(tabs, null);
            Flush();
            tabs.SelectedIndex = 1;
            Flush();
            var selected = rows[1];
            check(
                TabFeatures.Close((TabItem)tabs.ContainerFromIndex(0)!) && rows.Count == 1 &&
                ReferenceEquals(tabs.SelectedItem, selected),
                "TabFeatures/compat: native TabControl default removal still preserves a surviving selection");
            TabFeatures.SetCanClose(tabs, false);
            Flush();
            check(!TabFeatures.Close((TabItem)tabs.ContainerFromIndex(0)!) && rows.Count == 1,
                "TabFeatures/compat: native TabControl still honors runtime opt-out from closing");
        }
        finally
        {
            window.Close();
        }
    }

    private static ObservableCollection<TabValue> Rows(int count)
    {
        return new ObservableCollection<TabValue>(Enumerable.Range(1, count).Select(index => new TabValue(index)));
    }

    private static (Window Window, TabStrip Strip) Open(IEnumerable rows, ThemeVariant? theme = null,
        ControlSize size = ControlSize.Normal, int width = 620)
    {
        var strip = new TabStrip
        {
            ItemsSource = rows,
            ItemTemplate = new FuncDataTemplate<TabValue>((value, _) => new TextBlock
            {
                Text = value?.ToString(), TextTrimming = TextTrimming.CharacterEllipsis
            })
        };
        return (Show(strip, theme ?? ThemeVariant.Light, size, width), strip);
    }

    private static Window Show(Control strip, ThemeVariant theme, ControlSize size, int width)
    {
        var window = new Window
        {
            Width = width, Height = 160, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme,
            Content = new Border
            {
                Padding = new Thickness(16), Child = new StackPanel
                {
                    Spacing = 16, Children =
                    {
                        new TextBlock { Text = "Closable native document tabs" }, strip
                    }
                }
            }
        };
        Motion.SetEnabled(window, false);
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        return window;
    }

    private static TabStripItem Item(TabStrip strip, int index)
    {
        return (TabStripItem)(strip.ContainerFromIndex(index)
                              ?? throw new InvalidOperationException(
                                  $"Native TabStrip did not realize index {index}."));
    }

    private static Button CloseButton(TabStripItem item)
    {
        return Part<Button>(item, "PART_CloseButton");
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>()
            .Single(control => control.Name == name && ReferenceEquals(control.TemplatedParent, owner));
    }

    private static Point Centre(Control control, TopLevel window)
    {
        return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
    }

    private static void Click(TopLevel window, Control control)
    {
        var point = Centre(control, window);
        window.MouseMove(point);
        Flush();
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Press(TopLevel window, RawInputModifiers modifiers)
    {
        window.KeyPress(Key.W, modifiers, PhysicalKey.W, null);
        window.KeyRelease(Key.W, modifiers, PhysicalKey.W, null);
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed record TabValue(int Index)
    {
        public override string ToString()
        {
            return $"Report {Index:00000}";
        }
    }

    private sealed class CloseProbe(Action<object?>? execute = null) : ICommand
    {
        private bool _allowed;
        internal int Executions { get; private set; }
        internal object? Parameter { get; private set; }

        internal bool Allowed
        {
            get => _allowed;
            set
            {
                _allowed = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return Allowed;
        }

        public void Execute(object? parameter)
        {
            Executions++;
            Parameter = parameter;
            execute?.Invoke(parameter);
        }
    }
}