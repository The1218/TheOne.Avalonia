using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class TabSelectionStatePreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 800, 320 })
        foreach (var card in new[] { false, true })
        {
            var rows = new ObservableCollection<object?> { new Page("One"), new Page("Two"), new Page("Three") };
            var tabs = new TabControl
            {
                ItemsSource = rows,
                ItemTemplate = new FuncDataTemplate<Page>((page, _) => new TextBlock { Text = page?.Title }),
                ContentTemplate = new FuncDataTemplate<Page>((page, _) => new TextBlock { Text = page?.Title })
            };
            if (card) tabs.Classes.Add("card");
            TabFeatures.SetCanClose(tabs, true);
            var window = new Window
            {
                Width = width, Height = 260, RequestedThemeVariant = theme,
                Content = new Border { Padding = new Thickness(16), Child = tabs }
            };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            Flush();
            var label = $"TabControl/selection-state/{theme}/{size}/{width}/{(card ? "card" : "standard")}";
            try
            {
                foreach (var scenario in new[]
                         {
                             "pin", "disable-owner", "disable-item", "close-off", "command-denied", "source-replaced",
                             "container-replaced", "reentrant-close"
                         })
                {
                    tabs.ItemsSource = rows;
                    tabs.IsEnabled = true;
                    TabFeatures.SetCanClose(tabs, true);
                    TabFeatures.SetCloseCommand(tabs, null);
                    while (rows.Count < 3) rows.Add(new Page("Extra"));
                    tabs.SelectedIndex = 0;
                    Flush();
                    var item = (TabItem)tabs.ContainerFromIndex(0)!;
                    item.IsEnabled = true;
                    TabFeatures.SetIsClosable(item, true);
                    Flush();
                    var original = rows[0];
                    var replacement = new ObservableCollection<object?> { new Page("Replacement"), original, rows[2] };
                    var probe = new CloseProbe();
                    if (scenario == "command-denied") TabFeatures.SetCloseCommand(tabs, probe);
                    var attempts = 0;
                    EventHandler<TabClosingEventArgs>? handler = null;
                    handler = (_, args) =>
                    {
                        attempts++;
                        switch (scenario)
                        {
                            case "pin": TabFeatures.SetIsClosable(item, false); break;
                            case "disable-owner": tabs.IsEnabled = false; break;
                            case "disable-item": item.IsEnabled = false; break;
                            case "close-off": TabFeatures.SetCanClose(tabs, false); break;
                            case "command-denied": probe.Allowed = false; break;
                            case "source-replaced": tabs.ItemsSource = Array.AsReadOnly(rows.ToArray()); break;
                            case "container-replaced":
                                tabs.ItemsSource = replacement;
                                tabs.SelectedIndex = 2;
                                Flush();
                                break;
                            case "reentrant-close":
                                tabs.RemoveHandler(TabFeatures.TabClosingEvent, handler!);
                                TabFeatures.Close(item);
                                break;
                        }
                    };
                    tabs.AddHandler(TabFeatures.TabClosingEvent, handler);
                    try
                    {
                        var result = TabFeatures.Close(item);
                        Flush();
                        check(!result && attempts == 1 && probe.Executions == 0,
                            $"{label}/{scenario}: aborts the stale close after callback state changes");
                        if (scenario == "reentrant-close")
                            check(rows.Count == 2 && ReferenceEquals(tabs.SelectedItem, rows[0]),
                                $"{label}/{scenario}: nested close removes once and selects a surviving page");
                        else
                            check(rows.Count == 3 && (scenario == "container-replaced"
                                    ? replacement.Count == 3 && ReferenceEquals(tabs.SelectedItem, replacement[2])
                                    : ReferenceEquals(tabs.SelectedItem, original)),
                                $"{label}/{scenario}: preserves the callback's source and current selected page");
                    }
                    finally
                    {
                        tabs.RemoveHandler(TabFeatures.TabClosingEvent, handler);
                    }
                }

                tabs.IsEnabled = true;
                TabFeatures.SetCanClose(tabs, true);
                TabFeatures.SetCloseCommand(tabs, null);
                while (rows.Count < 3) rows.Add(new Page("Survivor"));
                tabs.ItemsSource = rows;
                tabs.SelectedIndex = 0;
                Flush();
                foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>())
                {
                    item.IsEnabled = true;
                    TabFeatures.SetIsClosable(item, true);
                }

                var selected = rows[1];
                tabs.SelectedItem = selected;
                Flush();
                check(
                    TabFeatures.Close((TabItem)tabs.ContainerFromIndex(0)!) &&
                    ReferenceEquals(tabs.SelectedItem, selected),
                    $"{label}: closing an unrelated tab keeps the selected model");
                var current = (TabItem)tabs.ContainerFromIndex(tabs.SelectedIndex)!;
                current.Focus();
                Flush();
                window.KeyPress(Key.W, RawInputModifiers.Control, PhysicalKey.W, null);
                window.KeyRelease(Key.W, RawInputModifiers.Control, PhysicalKey.W, null);
                Flush();
                check(rows.Count == 1 && ReferenceEquals(tabs.SelectedItem, rows[0]),
                    $"{label}: real Ctrl+W selects the surviving native tab (count={rows.Count}, index={tabs.SelectedIndex}, focus={current.IsFocused}, closable={TabFeatures.GetIsClosable(current)}, enabled={current.IsEffectivelyEnabled})");
                var first = (TabItem)tabs.ContainerFromIndex(0)!;
                if (card)
                {
                    var button = first.GetVisualDescendants().OfType<Button>().First(b => b.Name == "PART_CloseButton");
                    var point = button.TranslatePoint(new Point(button.Bounds.Width / 2, button.Bounds.Height / 2),
                        window)!.Value;
                    window.MouseMove(point);
                    window.MouseDown(point, MouseButton.Left);
                    window.MouseUp(point, MouseButton.Left);
                    Flush();
                }
                else
                {
                    first.Focus();
                    Flush();
                    window.KeyPress(Key.W, RawInputModifiers.Control, PhysicalKey.W, null);
                    window.KeyRelease(Key.W, RawInputModifiers.Control, PhysicalKey.W, null);
                    Flush();
                }

                check(rows.Count == 0 && tabs.SelectedIndex == -1 && tabs.SelectedItem is null,
                    $"{label}: {(card ? "pointer" : "keyboard")} close of final tab clears selection");
                rows.Add(null);
                rows.Add(null);
                rows.Add(new Page("After empty"));
                Flush();
                tabs.SelectedIndex = 0;
                Flush();
                check(
                    tabs.GetRealizedContainers().OfType<TabItem>().All(item =>
                        item.IsSelected == (tabs.IndexFromContainer(item) == tabs.SelectedIndex)),
                    $"{label}: null data items do not share native selected state (index={tabs.SelectedIndex}, states={string.Join(',', tabs.GetRealizedContainers().OfType<TabItem>().Select(item => $"{tabs.IndexFromContainer(item)}:{item.IsSelected}"))})");
                check(!TabFeatures.Close((TabItem)tabs.ContainerFromIndex(0)!),
                    $"{label}: null data cannot resolve a close target");
                tabs.SelectedIndex = 2;
                Flush();
                check(
                    tabs.GetRealizedContainers().OfType<TabItem>()
                        .All(item => item.IsSelected == (tabs.IndexFromContainer(item) == 2)),
                    $"{label}: selection moves away from null entries without stale highlight");
                foreach (var item in tabs.GetRealizedContainers().OfType<TabItem>())
                foreach (var chrome in item.GetVisualDescendants().OfType<CardTabChrome>())
                    check(chrome.IsSelected == item.IsSelected,
                        $"{label}: recycled card chrome matches its own native selection");
                if (output != null)
                {
                    Directory.CreateDirectory(output);
                    using var frame = window.CaptureRenderedFrame()!;
                    frame.Save(
                        Path.Combine(output,
                            $"tab-selection-{theme}-{size}-{width}-{(card ? "card" : "standard")}.png"),
                        PngBitmapEncoderOptions.Default);
                }
            }
            finally
            {
                window.Close();
                Flush();
            }
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed record Page(string Title);

    private sealed class CloseProbe : ICommand
    {
        public bool Allowed = true;
        public int Executions;

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return Allowed;
        }

        public void Execute(object? parameter)
        {
            Executions++;
        }
    }
}