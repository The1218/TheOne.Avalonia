using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

// Existing PreviewHost/NativeAotSmoke projects can verify reports without compiling unrelated demos.
internal static class ReportOnlyProgram
{
    internal static bool IsHeadless { get; private set; }
    [STAThread]
    public static void Main(string[] args)
    {
        IsHeadless = args.Any(argument => argument.StartsWith("--verify-", StringComparison.Ordinal));
        if (args.Contains("--require-aot") && RuntimeFeature.IsDynamicCodeSupported) throw new InvalidOperationException("NativeAOT is required.");
        var builder = AppBuilder.Configure<ReportOnlyApp>().WithInterFont().UseSkia();
        if (!IsHeadless) { builder.UsePlatformDetect().StartWithClassicDesktopLifetime(args); return; }
        builder.UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        Console.WriteLine("Runtime dynamic code supported: " + RuntimeFeature.IsDynamicCodeSupported);
        var count = 0;
        void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); ++count; Console.WriteLine("PASS: " + message); }
        var output = args.FirstOrDefault(argument => !argument.StartsWith("--", StringComparison.Ordinal));
        if (args.Contains("--verify-report-reload")) ReportReloadPreview.Verify(Check, output);
        else if (args.Contains("--verify-report-sidebar")) { ReportDataTreeScrollPreview.Verify(Check, output); ReportStructurePreview.Verify(Check, output); }
        else if (args.Contains("--verify-report-tree-scroll")) ReportDataTreeScrollPreview.Verify(Check, output);
        else if (args.Contains("--verify-report-authentication")) ReportAuthenticationPreview.Verify(Check, output);
        else if (args.Contains("--verify-report-lifecycle")) ReportLifecyclePreview.Verify(Check, output);
        else if (args.Contains("--verify-report-navigation")) { ReportNavigationPreview.Verify(Check, output); ReportFontsPreview.Verify(Check); }
        else if (args.Contains("--verify-report-api-components")) ReportApiComponentsPreview.Verify(Check, output);
        else if (args.Contains("--verify-reportdesign")) ReportDesignPreview.Verify(Check, output);
        else throw new ArgumentException("Use --verify-reportdesign, --verify-report-api-components, --verify-report-navigation, --verify-report-authentication, --verify-report-lifecycle, --verify-report-tree-scroll, --verify-report-sidebar or --verify-report-reload.");
        Console.WriteLine($"SUMMARY: {count} report checks passed.");
    }
}

internal sealed class ReportOnlyApp : Application
{
    public override void Initialize() => Styles.Add(new OneTheme());
    public override void OnFrameworkInitializationCompleted()
    {
        if (!ReportOnlyProgram.IsHeadless && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new Window { Title = "ReportDesign", Width = 1586, Height = 992, Content = new ReportDesign { Document = new ReportDocument() } };
        base.OnFrameworkInitializationCompleted();
    }
}
