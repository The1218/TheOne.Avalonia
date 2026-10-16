using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

internal sealed class MenuConsumerApp : Application
{
    public override void Initialize() => Styles.Add(new OneTheme());
}

internal static class MenuOnlyProgram
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"Runtime dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");
        if (args.Contains("--require-aot") && RuntimeFeature.IsDynamicCodeSupported)
            throw new InvalidOperationException("Use the native executable for this check.");
        AppBuilder.Configure<MenuConsumerApp>().WithInterFont().UseSkia()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        var count = 0;
        void Check(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException(message);
            count++;
            Console.WriteLine("PASS: " + message);
        }
        var output = args.FirstOrDefault(arg => !arg.StartsWith("--", StringComparison.Ordinal));
        MenuScrollingPreview.Verify(Check, output);
        MenuPanelTransitionPreview.Verify(Check, output);
        Console.WriteLine($"PASS: {count} focused menu consumer checks.");
    }
}
