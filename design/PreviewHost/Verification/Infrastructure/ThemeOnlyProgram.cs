using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

internal sealed class ThemeApiApp : Application
{
    public override void Initialize()
    {
        Styles.Add(new OneTheme());
    }
}

internal static class ThemeOnlyProgram
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"Runtime dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");
        if (args.Contains("--require-aot") && RuntimeFeature.IsDynamicCodeSupported)
            throw new InvalidOperationException("Use the native executable for this check.");
        AppBuilder.Configure<ThemeApiApp>().WithInterFont().UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime
                { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        ThemeColorApiPreview.Verify((condition, message) =>
        {
            if (!condition) throw new InvalidOperationException(message);
            Console.WriteLine("PASS: " + message);
        });
    }
}