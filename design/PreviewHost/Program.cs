using Avalonia;
using ReactiveUI.Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;

namespace TheOne.DesignPreview;

internal static class Program
{
    internal static bool IsHeadless { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] is "--help" or "-h")
        {
            Console.WriteLine("""
                PreviewHost: no arguments = desktop
                --login [--trace-login] = desktop login demo
                --datagrid-stage1 = desktop DataGrid experiment
                --verify-controls | --verify-core-controls | --verify-preview-pages | --verify-buttons-messages
                --verify-gantt [output] | --verify-relation-graph [output] | --verify-gauge-styles [output] | --verify-chart-axis-labels [output] | --verify-charts | --verify-compact-controls | --verify-text-alignment | --verify-pickers
                --verify-datagrid-binding | --verify-datagrid-stage1 | --verify-datagrid-right
                --verify-datagrid-summary | --verify-datagrid-filters
                --verify-loginwindow | --verify-onewindow | --verify-tag-verification-step | --verify-flow-step [output] | --verify-infocard [output] | --verify-control-variants
                --verify-login-lifetime / --verify-login-lifetime-close <OnMainWindowClose|OnLastWindowClose|OnExplicitShutdown>
                --verify-navmenu | --verify-navmenu-hover | --verify-navmenu-edge
                --verify-navmenu-search | --verify-navmenu-motion | --verify-navmenu-filter
                --verify-expander-motion | --verify-carousel | --verify-refresh | --verify-tab-drag
                --verify-flyout | --verify-notifications | --verify-content-transition
                --render-controls <output> <Control...|--all>
                """);
            return;
        }
        if (args.Length > 0 && args[0] is not ("--login" or "--datagrid-stage1" or "--verify-datagrid-stage1" or "--verify-datagrid-binding" or "--verify-datagrid-summary" or "--verify-datagrid-right" or "--verify-datagrid-filters" or "--verify-loginwindow" or "--verify-login-lifetime" or "--verify-login-lifetime-close" or "--verify-controls" or "--verify-core-controls" or "--verify-compact-controls" or "--verify-charts" or "--verify-chart-axis-labels" or "--verify-gauge-styles" or "--verify-gantt" or "--verify-relation-graph" or "--verify-text-alignment" or "--verify-navigation-chrome" or "--verify-onewindow" or "--verify-expander-motion" or "--render-controls" or "--verify-pickers" or "--verify-tag-verification-step" or "--verify-flow-step" or "--verify-infocard" or "--verify-control-variants" or "--verify-tab-drag" or "--verify-navmenu" or "--verify-navmenu-hover" or "--verify-navmenu-edge" or "--verify-loading" or "--verify-buttons-messages" or "--verify-preview-pages" or "--verify-notifications" or "--verify-content-transition" or "--verify-carousel" or "--verify-refresh" or "--verify-flyout" or "--verify-navmenu-search" or "--verify-navmenu-motion" or "--verify-navmenu-filter"))
            throw new ArgumentException($"Unknown preview command: {args[0]}");
        if (args.Length == 0 || args[0] is "--login" or "--datagrid-stage1")
        {
            BuildAvaloniaApp(false).StartWithClassicDesktopLifetime(args);
            return;
        }

        if (args[0] is "--verify-login-lifetime" or "--verify-login-lifetime-close")
        {
            if (args.Length != 2 || !Enum.TryParse<ShutdownMode>(args[1], out var mode) || !Enum.IsDefined(mode))
                throw new ArgumentException("Use --verify-login-lifetime / --verify-login-lifetime-close <OnMainWindowClose|OnLastWindowClose|OnExplicitShutdown>.");
            LoginWindowChecks.StartLifetimeVerification(mode, args[0] == "--verify-login-lifetime-close");
            return;
        }

        BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        if (args[0] == "--verify-gantt") { GanttPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-relation-graph") { RelationGraphPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-loginwindow")
        {
            LoginWindowChecks.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            return;
        }
        if (args[0] == "--verify-text-alignment")
        {
            TextAlignmentPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            ButtonControlsPreview.Verify(); TextControlsPreview.Verify(); InteractionRepairsPreview.Verify();
            return;
        }
        if (args[0] == "--verify-datagrid-binding") { DataGridBindingPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridPreview.Verify(); return; }
        if (args[0] == "--verify-datagrid-summary") { DataGridSummaryPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridFilterPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridRightFrozenPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridPreview.Verify(); return; }
        if (args[0] == "--verify-datagrid-right") { DataGridRightFrozenPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridFilterPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridStage1Preview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridPreview.Verify(); return; }
        if (args[0] == "--verify-datagrid-filters") { DataGridFilterPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridPreview.Verify(); return; }
        if (args[0] == "--verify-datagrid-stage1") { DataGridStage1Preview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); DataGridPreview.Verify(); return; }
        if (args[0] == "--verify-compact-controls")
        {
            Console.WriteLine($"Runtime dynamic code supported: {System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported}");
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }
            ChartPreview.Verify(Check); LayoutControlsPreview.Verify(); NotificationPreview.Verify();
            ButtonsMessagesPreview.Verify(Check); PreviewPagesPreview.Verify(Check); return;
        }
        if (args[0] == "--verify-gauge-styles") { ChartPreview.VerifyGaugeStyles((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-chart-axis-labels") { ChartPreview.VerifyAxisLabels((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-charts") { ChartPreview.Verify(); ChartDemoPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-notifications") { NotificationPreview.Verify(); return; }
        if (args[0] == "--verify-content-transition") { ContentTransitionPreview.Verify(); return; }
        if (args[0] == "--verify-carousel") { CarouselPreview.Verify(); return; }
        if (args[0] == "--verify-refresh") { RefreshPreview.Verify(); return; }
        if (args[0] == "--verify-flyout") { FlyoutPreview.Verify(); return; }
        if (args[0] == "--verify-loading") { LoadingPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-buttons-messages") { ButtonsMessagesPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-preview-pages")
        {
            PreviewPagesPreview.Verify((condition,message)=>{if(!condition)throw new InvalidOperationException(message);Console.WriteLine("PASS: "+message);});return;
        }
        if (args[0] == "--verify-navmenu-filter")
        {
            NavMenuFilterPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return;
        }
        if (args[0] == "--verify-navmenu-motion")
        {
            NavMenuMotionPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return;
        }
        if (args[0] == "--verify-navmenu-search")
        {
            NavMenuSearchPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            NavMenuSearchInteractionPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return;
        }
        if (args[0] == "--verify-navmenu-edge")
        {
            NavMenuEdgePreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return;
        }
        if (args[0] == "--verify-navmenu-hover")
        {
            NavMenuCascadePreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            return;
        }
        if (args[0] == "--verify-navmenu")
        {
            NavMenuPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            return;
        }
        if (args[0] == "--verify-flow-step") { FlowStepPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-tag-verification-step") { TagVerificationStepPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-tab-drag") { TabDragPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-control-variants") { ControlVariantsPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-infocard")
        {
            InfoCardPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
        if (args[0] == "--verify-navigation-chrome") { NavigationChromePreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-onewindow")
        {
            OneWindowChecks.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            var window = new LegacyPreviewWindow { Width = 1280, Height = 1060, WindowDecorations = WindowDecorations.None };
            TheOne.Avalonia.Styling.Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
            var gallery = window.FindControl<LegacyControlGallery>("Gallery")!; gallery.SelectCategory(8); ControlPreview.Flush();
            foreach (var index in new[] { 0, 2, 1 }) { window.FindControl<ComboBox>("SizePicker")!.SelectedIndex = index; ControlPreview.Flush(); }
            CardTabPreview.Verify(window, gallery); window.Close(); return;
        }
        if (args[0] == "--verify-expander-motion")
        {
            EnhancementsPreview.VerifyExpanderMotion();
            return;
        }
        if (args[0] == "--verify-pickers")
        {
            DateTimePickerPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            return;
        }
        if (args[0] == "--render-controls")
        {
            if (args.Length < 3) throw new ArgumentException("Use --render-controls <output> <Control...> or --all.");
            ControlPreview.Render(args[1], args.Skip(2).ToArray());
            return;
        }
        if (args[0] == "--verify-core-controls") { ControlPreview.VerifyCoreControls(); return; }
        if (args[0] == "--verify-controls")
        {
            ControlPreview.Verify();
            return;
        }
    }

    // The Avalonia remote designer discovers this public, parameterless entry point.
    public static AppBuilder BuildAvaloniaApp() => BuildAvaloniaApp(headless: false);

    private static AppBuilder BuildAvaloniaApp(bool headless)
    {
        IsHeadless = headless;
        var builder = AppBuilder.Configure<App>()
            .WithInterFont()
            .UseReactiveUI(static _ => { });
        builder = headless
            ? builder.UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
            : builder.UsePlatformDetect();
        return builder;
    }
}
