using System.Collections;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class CarouselReinsertPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var mode in new[] { "own-items", "own-source", "models" }) VerifyCase(mode, check, output);
    }

    private static void VerifyCase(string mode, Action<bool, string> check, string? output)
    {
        var pages = new[]
        {
            new PageModel("需求页面", Color.Parse("#155E75")),
            new PageModel("开发页面", Color.Parse("#166534")),
            new PageModel("验收页面", Color.Parse("#92400E"))
        };
        var models = mode == "models";
        var original = pages.Select(page => models ? (object)page : CreatePage(page)).ToArray();
        var source = mode == "own-items" ? null : new ObservableCollection<object>(original);
        var carousel = new Carousel { Height = 220, SelectedIndex = 0, IsSwipeEnabled = false };
        if (models)
            carousel.ItemTemplate =
                new FuncDataTemplate<PageModel>((page, _) => page is null ? null : CreatePage(page));
        if (source is null)
            foreach (var item in original)
                carousel.Items.Add(item);
        else carousel.ItemsSource = source;
        IList items = source is null ? carousel.Items : source;
        var window = new Window
            { Width = 620, Height = 320, Content = new Border { Padding = new Thickness(24), Child = carousel } };
        Motion.SetEnabled(window, false);
        try
        {
            window.Show();
            ControlPreview.Flush();
            check(carousel.ItemsPanelRoot is VirtualizingCarouselPanel,
                "CarouselReinsert/" + mode + ": native VirtualizingCarouselPanel remains active");
            SelectAndCheck("initial", original[0]);
            Save("before");

            carousel.SelectedIndex = 0;
            ControlPreview.Flush();
            items.RemoveAt(0);
            ControlPreview.Flush();
            SelectAndCheck("after-remove", original[1]);
            items.Add(original[0]);
            ControlPreview.Flush();
            SelectAndCheck("reinsert-at-end", original[0]);

            if (source is not null)
            {
                source.Move(2, 0);
            }
            else
            {
                items.RemoveAt(2);
                items.Insert(0, original[0]);
            }

            ControlPreview.Flush();
            check(
                ReferenceEquals(items[0], original[0]) && ReferenceEquals(items[1], original[1]) &&
                ReferenceEquals(items[2], original[2]),
                "CarouselReinsert/" + mode + ": reorder retains each original item identity");
            SelectAndCheck("after-reorder", original[0]);
            Save("after");
        }
        finally
        {
            window.Close();
            ControlPreview.Flush();
        }

        void SelectAndCheck(string phase, object item)
        {
            var index = items.IndexOf(item);
            carousel.SelectedIndex = index;
            ControlPreview.Flush();
            var container = carousel.ContainerFromIndex(index);
            var title = item is PageModel page ? page.Title : ((TextBlock)((Border)item).Child!).Text;
            var label = "CarouselReinsert/" + mode + "/" + phase + "/" + index;
            check(ReferenceEquals(carousel.SelectedItem, item) && container is not null,
                label + ": original item is selected and has a realized container");
            check(container is not null && container.IsAttachedToVisualTree() && container.IsEffectivelyVisible &&
                  ReferenceEquals(TopLevel.GetTopLevel(container), window) &&
                  container.GetVisualAncestors().Contains(carousel) &&
                  container.Bounds.Width > 0 && container.Bounds.Height > 0 &&
                  container.GetVisualDescendants().OfType<TextBlock>()
                      .Any(text => text.Text == title && text.IsEffectivelyVisible),
                label + ": the selected page remains mounted, visible and arranged in the displayed window");
            check(
                models ? container is not null && !ReferenceEquals(container, item) : ReferenceEquals(container, item),
                label + (models
                    ? ": a data model uses its generated container"
                    : ": the same direct Control remains its own container"));
        }

        void Save(string phase)
        {
            if (output is null) return;
            using var frame = window.CaptureRenderedFrame();
            check(frame is not null && PreviewRenderedPixels.UniqueColors(frame) > 16,
                "CarouselReinsert/" + mode + "/" + phase + ": the selected page produces a real rendered frame");
            frame?.Save(Path.Combine(output, "carousel-" + mode + "-" + phase + ".png"),
                PngBitmapEncoderOptions.Default);
        }
    }

    private static Border CreatePage(PageModel page)
    {
        return new Border
        {
            Background = new SolidColorBrush(page.Background),
            Child = new TextBlock
                { Text = page.Title, Foreground = Brushes.White, FontSize = 24, Margin = new Thickness(24) }
        };
    }

    private sealed record PageModel(string Title, Color Background);
}