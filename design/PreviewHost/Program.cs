using Avalonia;
using ReactiveUI.Avalonia;

namespace TheOne.DesignPreview;

internal static class Program
{
    [STAThread]
    public static void Main() => BuildAvaloniaApp().StartWithClassicDesktopLifetime([]);

    // The Avalonia remote designer discovers this public, parameterless entry point.
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .WithInterFont()
        .UseReactiveUI(static _ => { })
        .UsePlatformDetect();
}
