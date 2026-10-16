using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class CarouselMutationPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        VerifyCase(true, check, output);
        VerifyCase(false, check, output);
        VerifyCase(true, check, output, .65);
        VerifyCase(false, check, output, .65);
    }

    private static void VerifyCase(bool ownControls, Action<bool, string> check, string? output, double viewportFraction = 1)
    {
        var mode = (ownControls ? "own" : "models") + (viewportFraction < 1 ? "-fraction" : "");
        var a0 = Item("Mutation-A0"); var a1 = Item("Mutation-A1");
        var b0 = Item("Mutation-B0"); var b1 = Item("Mutation-B1");
        var sourceA = new ResetCollection { a0, a1 };
        var sourceB = new ResetCollection { b0, b1 };
        var carousel = new Carousel { ItemsSource = sourceA, Height = 220, SelectedIndex = 0, IsSwipeEnabled = false, ViewportFraction = viewportFraction, WrapSelection = false };
        if (!ownControls) carousel.ItemTemplate = new FuncDataTemplate<PageModel>((page, _) => page is null ? null : Page(page));
        var window = new Window { Width = 620, Height = 320, Content = new Border { Padding = new Thickness(24), Child = carousel } };
        Motion.SetEnabled(window, false);
        try
        {
            window.Show(); ControlPreview.Flush();
            SelectAndCheck("initial", sourceA, a0);
            if (viewportFraction < 1)
                check(carousel.ContainerFromIndex(1) is { Bounds.Width: > 0 } neighbor && carousel.ItemsPanelRoot is { } panel && neighbor.Bounds.Width < panel.Bounds.Width,
                    Label("fraction") + ": native fraction layout realizes a neighboring page narrower than the viewport");

            var replacement = Item("Mutation-Replacement");
            sourceA[0] = replacement; ControlPreview.Flush();
            CheckCurrent("replace", sourceA, a0);
            SelectAndCheck("replace", sourceA, replacement);

            var reset0 = Item("Mutation-Reset0"); var reset1 = Item("Mutation-Reset1");
            sourceA.ResetTo(reset0, reset1); ControlPreview.Flush();
            CheckCurrent("reset", sourceA, replacement, a1);
            SelectAndCheck("reset", sourceA, reset0);

            carousel.ItemsSource = sourceB; ControlPreview.Flush();
            CheckCurrent("source-B", sourceB, reset0, reset1);
            SelectAndCheck("source-B", sourceB, b0);
            sourceA[1] = Item("Mutation-InactiveA1"); ControlPreview.Flush();
            CheckCurrent("inactive-A-replaced", sourceB, reset0, reset1);
            check(ReferenceEquals(carousel.SelectedItem, b0), Label("inactive-A-replaced") + ": changes in the old source do not alter current selection");

            carousel.ItemsSource = sourceA; ControlPreview.Flush();
            CheckCurrent("source-A-return", sourceA, b0, b1);
            SelectAndCheck("source-A-return", sourceA, reset0);
            Save();
        }
        finally { window.Close(); ControlPreview.Flush(); }

        string Label(string phase) => "CarouselMutation/" + mode + "/" + phase;
        object Item(string title)
        {
            var model = new PageModel(title);
            return ownControls ? Page(model) : model;
        }
        void CheckCurrent(string phase, ResetCollection current, params object[] retired)
        {
            check(carousel.SelectedItem is null || current.Any(item => ReferenceEquals(item, carousel.SelectedItem)),
                Label(phase) + ": selection is null or belongs to the current source, never a retired item");
            var allowed = current.Select(Title).ToHashSet(StringComparer.Ordinal);
            check(VisibleTitles().All(allowed.Contains), Label(phase) + ": no visible page content comes from the old source");
            foreach (var old in retired.OfType<Control>())
            {
                // Native Carousel retains hidden own children as a cache. That alone
                // is not stale display: retired controls must not map to current data.
                check((!old.IsAttachedToVisualTree() || !old.IsEffectivelyVisible) && carousel.IndexFromContainer(old) < 0 && carousel.ItemFromContainer(old) is null,
                    Label(phase) + ": a retired own Control is neither visible nor a valid current item/container");
                check(old.GetVisualParent() is null && old.Parent is null,
                    Label(phase) + ": a retired own Control releases its former visual and logical parent");
                Console.WriteLine(Label(phase) + ": retired-own cache parent=" + (old.GetVisualParent()?.GetType().Name ?? "null") + ", logical=" + (old.Parent?.GetType().Name ?? "null"));
            }
        }
        void SelectAndCheck(string phase, ResetCollection current, object expected)
        {
            carousel.SelectedIndex = current.IndexOf(expected); ControlPreview.Flush();
            var container = carousel.ContainerFromIndex(carousel.SelectedIndex);
            check(ReferenceEquals(carousel.SelectedItem, expected) && container is not null && ReferenceEquals(carousel.ItemFromContainer(container), expected),
                Label(phase) + ": selected data and the realized container refer to the current item");
            var visible = VisibleTitles();
            var expectedVisible = viewportFraction < 1
                ? visible.Contains(Title(expected)) && visible.All(current.Select(Title).Contains)
                : visible.SequenceEqual(new[] { Title(expected) });
            check(container is not null && container.IsAttachedToVisualTree() && container.IsEffectivelyVisible &&
                ReferenceEquals(TopLevel.GetTopLevel(container), window) && container.Bounds.Width > 0 && container.Bounds.Height > 0 &&
                expectedVisible,
                Label(phase) + ": expected page and only current-source content are visibly arranged in the displayed window");
            check(ownControls ? ReferenceEquals(container, expected) : container is not null && !ReferenceEquals(container, expected),
                Label(phase) + (ownControls ? ": original own-container identity is preserved" : ": model data keeps a generated container"));
        }
        string[] VisibleTitles() => carousel.GetVisualDescendants().OfType<TextBlock>()
            .Where(text => text.IsEffectivelyVisible && text.Bounds.Width > 0 && text.Bounds.Height > 0 && text.Text?.StartsWith("Mutation-", StringComparison.Ordinal) == true)
            .Select(text => text.Text!).ToArray();
        void Save()
        {
            if (output is null) return;
            using var frame = window.CaptureRenderedFrame();
            check(frame is not null && PreviewRenderedPixels.UniqueColors(frame) > 16, Label("render") + ": the current page produces a real rendered frame");
            frame?.Save(Path.Combine(output, "carousel-mutations-" + mode + ".png"), PngBitmapEncoderOptions.Default);
        }
    }

    private static string Title(object item) => item is PageModel page ? page.Title : ((TextBlock)((Border)item).Child!).Text!;
    private static Border Page(PageModel page) => new()
    {
        Background = new SolidColorBrush(Color.Parse("#155E75")),
        Child = new TextBlock { Text = page.Title, Foreground = Brushes.White, FontSize = 24, Margin = new Thickness(24) }
    };
    private sealed record PageModel(string Title);
    private sealed class ResetCollection : ObservableCollection<object>
    {
        public void ResetTo(params object[] items)
        {
            Items.Clear();
            foreach (var item in items) Items.Add(item);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
