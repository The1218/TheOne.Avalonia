using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class NewComponentsPreview
{
    public static void VerifyPage(int id, string title, Action<bool, string> check)
    {
        var descriptor = PreviewPageCatalog.Find(id);
        check(descriptor.Title == title && PreviewPageCatalog.Pages.Count(p => p.Id == id) == 1,
            $"PreviewHost/{title}: unique searchable navigation entry");
        var window = new MainWindow { Width = 1200, Height = 900, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        try
        {
            window.NavigateToCategory(id);
            Flush();
            var gallery = window.FindControl<ControlGallery>("Gallery")!;
            var view = gallery.GetControlView(id);
            check(view is not null && view.Bounds.Width > 400 && view.Bounds.Height > 0 && view.DataContext != null,
                $"PreviewHost/{title}: page arranges a concrete command-driven example");
            window.NavigateToCategory(1);
            window.Model.Search = title;
            Flush();
            window.Model.SelectedSearchResult = window.Model.SearchResults.Single(p => p.Id == id);
            Flush();
            check(ReferenceEquals(view, gallery.CurrentView),
                $"PreviewHost/{title}: search restores the cached consumer view");
        }
        finally
        {
            window.Close();
            Flush();
        }
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }
}