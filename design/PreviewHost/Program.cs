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
        if (args.Length > 0 && args[0] is "--verify-report-authentication" or "--verify-report-lifecycle" or "--verify-report-tree-scroll" or "--verify-report-sidebar" or "--verify-report-reload")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }
            var output = args.Length > 1 ? args[1] : null;
            if (args[0] == "--verify-report-reload") ReportReloadPreview.Verify(Check, output);
            else if (args[0] == "--verify-report-sidebar") { ReportDataTreeScrollPreview.Verify(Check, output); ReportStructurePreview.Verify(Check, output); }
            else if (args[0] == "--verify-report-authentication") ReportAuthenticationPreview.Verify(Check, output);
            else if (args[0] == "--verify-report-tree-scroll") ReportDataTreeScrollPreview.Verify(Check, output);
            else ReportLifecyclePreview.Verify(Check, output);
            return;
        }
        if (args.Length > 0 && args[0] == "--verify-report-api-components")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            ReportApiComponentsPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
        if (args.Length > 0 && args[0] is "--verify-reportdesign-interactions" or "--verify-reportdialogs" or "--verify-reportdata-tree")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }
            var output = args.Length > 1 ? args[1] : null;
            if (args[0] == "--verify-reportdesign-interactions") ReportCanvasInteractionPreview.Verify(Check, output);
            if (args[0] != "--verify-reportdata-tree") ReportDialogsPreview.Verify(Check, output);
            if (args[0] != "--verify-reportdialogs") ReportDataTreePreview.Verify(Check, output);
            return;
        }
        if (args.Length > 0 && args[0] == "--verify-tabstrip-close")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            TabStripClosePreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
        if (args.Length > 0 && args[0] is "--verify-messagebox" or "--verify-upload" or "--verify-brand-icon" or "--export-brand-icon")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }
            var output = args.Length > 1 ? args[1] : null;
            if (args[0] == "--verify-messagebox") { MessageBoxPreview.Verify(Check, output); NewComponentsPreview.VerifyPage(320, "MessageBox", Check); }
            else if (args[0] == "--verify-upload") { UploadPreview.Verify(Check, output); NewComponentsPreview.VerifyPage(321, "Upload", Check); }
            else if (args[0] == "--verify-brand-icon") PreviewBrandIconPreview.Verify(Check, output);
            else PreviewBrandIconPreview.Export(output ?? ".preview/icon-messagebox-upload/brand-assets");
            return;
        }
        if (args.Length > 0 && args[0] == "--colorpicker")
        {
            BuildAvaloniaApp(false).StartWithClassicDesktopLifetime(args);
            return;
        }
        if (args.Length > 0 && args[0] == "--verify-colorpicker")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            ColorPickerPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
        if (args.Length > 0 && args[0] == "--verify-reportdesign-page")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            PreviewPagesPreview.VerifyReportDesignPage((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
        if (args.Length > 0 && args[0] == "--verify-workflow")
        {
            BuildAvaloniaApp(true).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
            WorkflowEditorPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            PreviewPagesPreview.VerifyWorkflowPage((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); });
            return;
        }
        if (args.Length > 0 && args[0] is "--help" or "-h")
        {
            Console.WriteLine("""
                PreviewHost: no arguments = desktop
                --verify-brand-icon [output] = application icon assets and PreviewHost branding only
                --export-brand-icon [output] = export vector-based PNG / ICO / ICNS application icons
                --verify-messagebox [output] = simple confirmation, commands, keyboard and themes only
                --verify-upload [output] = single-file selection, preview, commands, HTTP API and themes only
                --verify-tabstrip-close [output] = native TabStrip closing, commands, recycling and TabControl compatibility only
                --colorpicker = desktop native ColorPicker demo
                --verify-colorpicker [output] = ColorPicker / ColorView themes, input, binding and pixels only
                --verify-workflow [output] = WorkflowEditor contracts, binding, editing, persistence and rendering
                --reportdesign = desktop report designer
                --verify-reportdesign [output] = report binding / expansion / pagination checks
                --verify-report-api-components [output] = API request dialog / checkbox / barcode checks
                --verify-reportdesign-interactions [output] = report canvas / data tree / mapping and page dialogs only
                --verify-reportdialogs [output] = page settings and collection field dialog checks only
                --verify-reportdata-tree [output] = JSON data tree, drop binding and standalone preview only
                --verify-reportdesign-page [output] = report designer navigation / viewport / cached state
                --login [--trace-login] = desktop login demo
                --datagrid-stage1 = desktop DataGrid experiment
                --datagrid-tree = desktop tree DataGrid prototype
                --verify-datagrid-tree-page [output] = tree grid navigation / search / cached state
                --verify-datagrid-tree-query [output] = tree sibling sorting / filtering / summaries
                --verify-datagrid-tree [output] = tree projection / editing / selection checks
                --verify-controls | --verify-core-controls | --verify-preview-pages | --verify-buttons-messages
                --verify-image-preview [output]
                --verify-gantt [output] | --verify-relation-graph [output] | --verify-gauge-styles [output] | --verify-chart-axis-labels [output] | --verify-charts | --verify-compact-controls | --verify-text-alignment | --verify-pickers
                --verify-datagrid-binding | --verify-datagrid-stage1 | --verify-datagrid-right
                --verify-datagrid-summary | --verify-datagrid-filters
                --verify-loginwindow | --verify-onewindow | --verify-tag-verification-step | --verify-verification-step | --verify-flow-step [output] | --verify-pagination [output] | --verify-avatar [output] | --verify-infocard [output] | --verify-control-variants
                --verify-login-lifetime / --verify-login-lifetime-close <OnMainWindowClose|OnLastWindowClose|OnExplicitShutdown>
                --verify-navmenu | --verify-navmenu-hover | --verify-navmenu-edge
                --verify-navmenu-search | --verify-navmenu-motion | --verify-navmenu-filter
                --verify-expander-motion | --verify-carousel | --verify-refresh | --verify-tab-drag
                --verify-flyout | --verify-notifications | --verify-content-transition
                --render-controls <output> <Control...|--all>
                """);
            return;
        }
        if (args.Length > 0 && args[0] is not ("--reportdesign" or "--verify-reportdesign" or "--login" or "--datagrid-tree" or "--datagrid-stage1" or "--verify-datagrid-stage1" or "--verify-datagrid-tree-page" or "--verify-datagrid-tree-query" or "--verify-datagrid-tree" or "--verify-datagrid-binding" or "--verify-datagrid-summary" or "--verify-datagrid-right" or "--verify-datagrid-filters" or "--verify-loginwindow" or "--verify-login-lifetime" or "--verify-login-lifetime-close" or "--verify-controls" or "--verify-core-controls" or "--verify-compact-controls" or "--verify-charts" or "--verify-chart-axis-labels" or "--verify-gauge-styles" or "--verify-gantt" or "--verify-relation-graph" or "--verify-text-alignment" or "--verify-navigation-chrome" or "--verify-onewindow" or "--verify-expander-motion" or "--render-controls" or "--verify-pickers" or "--verify-tag-verification-step" or "--verify-verification-step" or "--verify-flow-step" or "--verify-pagination" or "--verify-avatar" or "--verify-image-preview" or "--verify-infocard" or "--verify-control-variants" or "--verify-tab-drag" or "--verify-navmenu" or "--verify-navmenu-hover" or "--verify-navmenu-edge" or "--verify-loading" or "--verify-buttons-messages" or "--verify-preview-pages" or "--verify-notifications" or "--verify-content-transition" or "--verify-carousel" or "--verify-refresh" or "--verify-flyout" or "--verify-navmenu-search" or "--verify-navmenu-motion" or "--verify-navmenu-filter"))
            throw new ArgumentException($"Unknown preview command: {args[0]}");
        if (args.Length == 0 || args[0] is "--reportdesign" or "--login" or "--datagrid-tree" or "--datagrid-stage1")
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
        if (args[0] == "--verify-reportdesign") { ReportDesignPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
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
        if (args[0] == "--verify-datagrid-tree-page") { PreviewPagesPreview.VerifyDataGridTreePage((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-datagrid-tree-query") { DataGridTreeQueryPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-datagrid-tree") { DataGridTreePreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
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
        if (args[0] == "--verify-pagination") { PaginationPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); PreviewPagesPreview.VerifyPaginationPage((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-flow-step") { FlowStepPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-avatar") { AvatarPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null); return; }
        if (args[0] == "--verify-verification-step") { TagVerificationStepPreview.VerifyStep((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-tag-verification-step") { TagVerificationStepPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-tab-drag") { TabDragPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-control-variants") { ControlVariantsPreview.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }); return; }
        if (args[0] == "--verify-image-preview")
        {
            ImagePreviewChecks.Verify((condition, message) => { if (!condition) throw new InvalidOperationException(message); Console.WriteLine("PASS: " + message); }, args.Length > 1 ? args[1] : null);
            return;
        }
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
