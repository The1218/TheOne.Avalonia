using System.Collections;
using System.Runtime.CompilerServices;
using Avalonia;
using ReactiveUI.Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using TheOne.Avalonia.Themes;

namespace TheOne.NativeAotSmoke;

public partial class AuditView : UserControl
{
    public static readonly DirectProperty<AuditView, ChartBase> CurrentChartProperty = AvaloniaProperty.RegisterDirect<AuditView, ChartBase>(nameof(CurrentChart), view => view.CurrentChart);
    private ChartBase _currentChart = new LineChart();
    private AuditModel? _chartModel;
    public ChartBase CurrentChart => _currentChart;
    public AuditView()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is not AuditModel model) return;
            _chartModel = model; model.PropertyChanged += ChartModelChanged; ReplaceChart();
        };
        DetachedFromVisualTree += (_, _) => { if (_chartModel is { } model) model.PropertyChanged -= ChartModelChanged; _chartModel = null; };
    }
    private void ChartModelChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(AuditModel.ChartKind)) ReplaceChart();
    }
    private void ReplaceChart()
    {
        if (_chartModel is not { } model) return;
        var next = TheOne.DesignPreview.ChartPreview.CreateControl(model.ChartKind); next.Classes.Add("consumer-chart");
        SetAndRaise(CurrentChartProperty, ref _currentChart, next);
    }
}
public sealed class AuditApp : Application
{
    public override void Initialize() => Styles.Add(new OneTheme());
}
internal static class Program
{
    private static int _checks;
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"Runtime dynamic code supported: {RuntimeFeature.IsDynamicCodeSupported}");
        if (args.Contains("--require-aot") && RuntimeFeature.IsDynamicCodeSupported) throw new InvalidOperationException("This verification requires the native executable.");
        var lifetimeCommand = Array.FindIndex(args, argument => argument is "--verify-login-lifetime" or "--verify-login-lifetime-close");
        if (lifetimeCommand >= 0)
        {
            if (lifetimeCommand + 1 >= args.Length || !Enum.TryParse<ShutdownMode>(args[lifetimeCommand + 1], out var mode) || !Enum.IsDefined(mode))
                throw new ArgumentException("Use --verify-login-lifetime <OnMainWindowClose|OnLastWindowClose|OnExplicitShutdown>.");
            TheOne.DesignPreview.LoginWindowChecks.StartLifetimeVerification(mode, args[lifetimeCommand] == "--verify-login-lifetime-close");
            return;
        }
        _ = DataGridAot.PreserveRowType<AuditRow>();
        AppBuilder.Configure<AuditApp>().WithInterFont().UseReactiveUI(static _ => { }).UseSkia().UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false }).SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime { ShutdownMode = ShutdownMode.OnExplicitShutdown });
        if (args.Contains("--verify-loginwindow"))
        {
            TheOne.DesignPreview.LoginWindowChecks.Verify(Check);
            Console.WriteLine($"PASS: {_checks} LoginWindow compiled binding / theme / lifetime checks.");
            return;
        }
        if (args.Contains("--verify-datagrid-binding"))
        {
            TheOne.DesignPreview.DataGridBindingPreview.Verify(Check);
            TheOne.DesignPreview.DataGridFilterPreview.Verify(Check);
            TheOne.DesignPreview.DataGridRightFrozenPreview.Verify(Check);
            TheOne.DesignPreview.DataGridSummaryPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} DataGrid inferred binding / sort buttons / density / virtualization / native layout checks.");
            return;
        }
        if (args.Contains("--verify-datagrid-summary"))
        {
            TheOne.DesignPreview.DataGridSummaryPreview.Verify(Check);
            TheOne.DesignPreview.DataGridFilterPreview.Verify(Check);
            TheOne.DesignPreview.DataGridRightFrozenPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} DataGrid summary / automatic fields / filtering / native frozen layout checks.");
            return;
        }
        if (args.Contains("--verify-datagrid-right"))
        {
            TheOne.DesignPreview.DataGridRightFrozenPreview.Verify(Check);
            TheOne.DesignPreview.DataGridFilterPreview.Verify(Check);
            TheOne.DesignPreview.DataGridStage1Preview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} DataGrid native right freezing / filtering / left-only checks.");
            return;
        }
        if (args.Contains("--verify-datagrid-filters"))
        {
            TheOne.DesignPreview.DataGridFilterPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} DataGrid filtering / popup / compiled binding / virtualization checks.");
            return;
        }
        if (args.Contains("--verify-datagrid-stage1"))
        {
            TheOne.DesignPreview.DataGridStage1Preview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} DataGrid stage-one feasibility / compiled binding / native virtualization checks.");
            return;
        }
        if (args.Contains("--verify-charts"))
        {
            TheOne.DesignPreview.ChartPreview.Verify(Check);
            foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
            foreach (var size in Enum.GetValues<ControlSize>()) Verify(theme, size, args.FirstOrDefault(arg => !arg.StartsWith("--")));
            Console.WriteLine($"PASS: {_checks} Chart and compiled consumer binding / theme / interaction checks; {ThemeManifest.Assets.Length} dictionaries, {ThemeManifest.Themes.Length} control theme keys.");
            return;
        }
        if (args.Contains("--verify-navmenu-filter"))
        {
            TheOne.DesignPreview.NavMenuFilterPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} NavMenu integrated filter / compiled binding / virtualization checks."); return;
        }
        if (args.Contains("--verify-navmenu-motion"))
        {
            TheOne.DesignPreview.NavMenuMotionPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} NavMenu motion / compiled binding / virtualization checks."); return;
        }
        if (args.Contains("--verify-navmenu"))
        {
            TheOne.DesignPreview.NavMenuPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} NavMenu compiled binding / theme / virtualization checks.");
            return;
        }
        if (args.Contains("--verify-tag-step"))
        {
            TheOne.DesignPreview.TagStepPreview.Verify(Check);
            Console.WriteLine($"PASS: Tag / Step compiled validation, interaction and virtualization ({_checks} checks)."); return;
        }
        if (args.Contains("--verify-infocard"))
        {
            TheOne.DesignPreview.InfoCardPreview.Verify(Check);
            Console.WriteLine($"PASS: {_checks} InfoCard compiled binding / theme / interaction checks.");
            return;
        }
        TheOne.DesignPreview.DateTimePickerPreview.Verify(Check);
        TheOne.DesignPreview.OneWindowChecks.Verify(Check);
        TheOne.DesignPreview.LoginWindowChecks.Verify(Check);
        TheOne.DesignPreview.ChartPreview.Verify(Check);
        TheOne.DesignPreview.InfoCardPreview.Verify(Check);
        TheOne.DesignPreview.TagStepPreview.Verify(Check);
        TheOne.DesignPreview.NavMenuPreview.Verify(Check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>()) Verify(theme, size, args.FirstOrDefault(arg => !arg.StartsWith("--")));
        Console.WriteLine($"PASS: {_checks} compiled binding / theme / interaction checks; {ThemeManifest.Assets.Length} dictionaries, {ThemeManifest.Themes.Length} control theme keys.");
    }
    private static void Verify(ThemeVariant theme, ControlSize size, string? output)
    {
        var model = new AuditModel(); var view = new AuditView { DataContext = model };
        var window = new Window { Width = 1100, Height = 1500, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = view };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
        Check(view.FindControl<DataGrid>("Table")!.Columns.OfType<DataGridBoundColumn>()
            .All(column => column.Binding is global::Avalonia.Data.CompiledBinding),
            "DataGrid Binding shorthand compiles column paths inferred from typed Rows");
        foreach (var (key, target) in ThemeManifest.Themes)
            Check(window.TryFindResource(key, theme, out var value) && value is ControlTheme controlTheme && controlTheme.TargetType == target, $"{theme.Key}/{size}: theme {key}");
        var editor = view.FindControl<TextBox>("Editor")!;
        Check(editor.Text == model.Draft, "compiled TextBox initial value"); model.Draft = "Updated by model"; Flush(); Check(editor.Text == model.Draft, "INotifyPropertyChanged updates compiled binding");
        editor.SetCurrentValue(TextBox.TextProperty, "Updated by editor"); Flush(); Check(model.Draft == editor.Text, "compiled two-way TextBox writes source");
        var action = view.FindControl<Button>("Action")!;
        Check(ReferenceEquals(action.Command, model.ApplyCommand), "compiled ICommand resolves the consumer command");
        Click(action); Check(model.Applied == 1, $"compiled ICommand executes (actual {model.Applied}; bounds {action.Bounds}; enabled {action.IsEffectivelyEnabled})");
        model.Selected = true; model.Number = 42; model.Range = 65; model.Time = new TimeSpan(14, 25, 0); model.Expanded = true; Flush();
        Check(view.FindControl<CheckBox>("Check")!.IsChecked == true && view.FindControl<NumericUpDown>("Number")!.Value == 42 && view.FindControl<Slider>("Range")!.Value == 65, "compiled selection and numeric bindings update");
        Check(view.FindControl<TimePicker>("TimeParts")!.SelectedTime == model.Time && view.FindControl<SplitView>("Pane")!.IsPaneOpen && view.FindControl<Expander>("Reveal")!.IsExpanded, "compiled time and layout bindings update");
        var chart = view.CurrentChart;
        Check(ReferenceEquals(chart.SeriesSource, model.ChartSeries) && chart.Kind == ChartKind.Line && chart.GetDataPointPosition(0, 999) is not null && !chart.GetVisualDescendants().Any(), "compiled Chart collection binding renders a thousand points without item controls");
        Check(chart.ShowCrosshair && chart.TooltipMode == ChartTooltipMode.Shared && chart.IsLegendInteractive, "compiled Chart interaction preferences enable crosshair, shared tooltip and legend input");
        var hover = chart.GetDataPointPosition(0, 500)!.Value; window.MouseMove(chart.TranslatePoint(hover, window)!.Value); Flush();
        Check(chart.CrosshairPosition is not null && chart.HoveredPoints.Count == 2 && chart.HoveredPoints.Any(point => point.Point == model.ChartSeries[0].ItemsSource![500]) && view.FindControl<TextBlock>("ConsumerHoverCount")!.Text == "Hover models: 2", "compiled readonly shared-hover diagnostics retain original consumer models");
        model.ChartCrosshair = false; model.ChartSharedTooltip = false; Flush();
        Check(!chart.ShowCrosshair && chart.TooltipMode == ChartTooltipMode.Single, "compiled interaction preference changes update crosshair and tooltip mode");
        ClickAt(chart, chart.GetLegendItemBounds(1)!.Value.Center); Check(!model.ChartSeries[1].IsVisible, "native legend input updates the consumer's observable series visibility");
        model.ChartLegendInteractive = false; Flush(); ClickAt(chart, chart.GetLegendItemBounds(1)!.Value.Center); Check(!model.ChartSeries[1].IsVisible, "compiled legend-interaction toggle prevents further input changes");
        model.ChartLegendInteractive = true; Flush(); ClickAt(chart, chart.GetLegendItemBounds(1)!.Value.Center); Check(model.ChartSeries[1].IsVisible, "compiled legend preferences can restore the hidden consumer series");
        model.ChartCrosshair = true; model.ChartSharedTooltip = true; Flush();
        model.ChartPointIndex = 999; Flush(); Check(chart.Selection?.Point == model.ChartSeries[0].ItemsSource![999], "compiled Chart selection follows the consumer model");
        chart.SetCurrentValue(ChartBase.SelectedIndexProperty, 500); Flush(); Check(model.ChartPointIndex == 500 && chart.Selection?.PointIndex == 500, "compiled Chart two-way selection writes the consumer model");
        var formerChart = chart; model.ChartKind = ChartKind.Bar; Flush(); chart = view.CurrentChart; Check(chart is BarChart && !ReferenceEquals(chart, formerChart) && formerChart.GetVisualParent() is null, "consumer changes to an independent BarChart and detaches the previous LineChart"); Check(chart.Kind == ChartKind.Bar && chart.GetDataPointPosition(0, 500) is not null, "compiled Chart kind binding changes rendered geometry");
        model.ChartSeries[0].ItemsSource![500].Y = 70; Flush(); Check(chart.Selection?.Point.Y == 70, "observable chart points remain live in the native consumer");
        model.ChartViewport = new Rect(0.2, 0, 0.5, 1); model.ChartAxes = ChartNavigationAxes.Horizontal; Flush();
        Check(chart.Viewport == model.ChartViewport && chart.NavigationAxes == ChartNavigationAxes.Horizontal, "compiled Chart viewport and direction follow the consumer model");
        chart.Zoom(2); Flush(); Check(model.ChartViewport == chart.Viewport && Math.Abs(model.ChartViewport.Width - 0.25) < 0.0001 && model.ChartViewport.Height == 1, "Chart zoom writes the compiled two-way viewport model");
        var oldViewport = model.ChartViewport; chart.Pan(0.2, 0); Flush(); Check(model.ChartViewport.X > oldViewport.X && model.ChartViewport == chart.Viewport, "Chart pan writes the compiled viewport model");
        chart.ResetViewport(); model.ChartAxes = ChartNavigationAxes.Both;
        var axisChooser = view.FindControl<ComboBox>("ConsumerSeriesAxis")!; axisChooser.SelectedItem = ChartYAxis.Secondary; Flush();
        Check(model.ChartSeries[1].YAxis == ChartYAxis.Secondary && chart.SecondaryYAxisTitle == model.ChartSecondaryTitle, "compiled per-series axis selection updates the observable source and secondary title");
        model.ChartSeries[1].YAxis = ChartYAxis.Primary; Flush(); Check(Equals(axisChooser.SelectedItem, ChartYAxis.Primary), "compiled axis binding follows programmatic series updates");
        foreach (var kind in new[] { ChartKind.StackedBar, ChartKind.StackedHorizontalBar, ChartKind.StackedArea, ChartKind.PercentStackedBar, ChartKind.PercentStackedHorizontalBar, ChartKind.PercentStackedArea })
        {
            model.ChartKind = kind; Flush(); chart = view.CurrentChart; var stackPosition = chart.GetDataPointPosition(1, 500);
            Check(chart.Kind == kind && stackPosition is { } position && chart.HitTestDataPoint(position)?.SeriesIndex == 1 && !chart.GetVisualDescendants().Any(), $"compiled native {kind} geometry maps cumulative data to its original series");
        }
        Check(model.ChartViewport == new Rect(0, 0, 1, 1), "Chart reset writes the full viewport to the compiled consumer model");
        model.ChartSeries[1].IsVisible = false; model.ChartKind = ChartKind.Line; model.ChartPointIndex = -1;
        chart = view.CurrentChart;
        model.ChartSeries[0].ItemsSource = new System.Collections.ObjectModel.ObservableCollection<ChartPoint>(Enumerable.Range(0, 100000).Select(index => new ChartPoint(index, index == 99777 ? 90 : 10)));
        model.ChartSamplingMode = ChartSamplingMode.MinMax; model.ChartSamplingBudget = 512; Flush();
        Check(chart.SamplingMode == model.ChartSamplingMode && chart.SamplingPointBudget == 512 && chart.SamplingActive && chart.RenderedPointCount is > 0 and <= 528 && !chart.GetVisualDescendants().Any(), "compiled sampling preferences reduce 100,000 native-consumer originals to a bounded vertex count");
        model.ChartPointIndex = 99777; Flush(); Check(chart.Selection?.Point == model.ChartSeries[0].ItemsSource![99777] && chart.GetDataPointPosition(0, 99777) is not null, "compiled native dense selection retains the original distant peak model");
        hover = chart.GetDataPointPosition(0, 99777)!.Value; window.MouseMove(chart.TranslatePoint(hover, window)!.Value); Flush();
        Check(chart.HoveredPoints.Count == 1 && chart.HoveredPoints[0].Point == model.ChartSeries[0].ItemsSource![99777] && chart.CrosshairPosition is not null, "native shared hover on sampled data retains the original distant model");
        model.ChartSamplingBudget = 256; Flush(); Check(chart.SamplingPointBudget == 256 && chart.RenderedPointCount <= 272, "compiled sampling budget changes rebuild native prepared vertices");
        model.ChartSmooth = true; model.ChartAnimationDuration = TimeSpan.FromSeconds(2); chart.YMinimum = 0; chart.YMaximum = 100; Flush();
        Check(chart.IsSmooth && chart.IsAnimationEnabled && chart.AnimationDuration == model.ChartAnimationDuration, "compiled smoothing and update-animation preferences reach the native chart");
        Motion.SetEnabled(window, true); model.ChartSeries[0].ItemsSource![99777].Y = 50; Flush(); Advance(100);
        Check(chart.IsAnimating && chart.AnimationProgress is > 0 and < 1 && chart.RenderedPointCount <= 272, "native compiled consumer animates a bounded sampled update through real UI frames");
        using (var exported = new MemoryStream())
        {
            chart.ExportPng(exported, new Size(250, 160), 144); exported.Position = 0; using var image = new Bitmap(exported);
            Check(image.PixelSize == new PixelSize(375, 240) && chart.IsAnimating, "native PNG export uses the requested DPI and target snapshot without stopping live animation");
        }
        Motion.SetEnabled(window, false); Flush(); Check(!chart.IsAnimating, "native global motion disable settles the latest data frame");
        model.ChartAnimateUpdates = false; model.ChartGaugeNeedle = false; model.ChartGaugeTicks = false; Flush();
        Check(!chart.IsAnimationEnabled && !view.FindControl<GaugeChart>("ConsumerGauge")!.ShowGaugeNeedle && !view.FindControl<GaugeChart>("ConsumerGauge")!.ShowGaugeTicks, "compiled animation and gauge feature toggles update the consumer controls");
        Check(view.FindControl<GaugeChart>("ConsumerGauge")!.Value == model.Range, "compiled Gauge value follows the consumer model");
        var list = view.FindControl<ListBox>("Records")!; list.ScrollIntoView(999); list.SetCurrentValue(ListBox.SelectedItemProperty, model.Rows[999]); Flush();
        Check(ReferenceEquals(model.Choice, model.Rows[999]) && list.GetRealizedContainers().Count() < 80 && list.ContainerFromIndex(999) is not null, "compiled item template, far selection, and thousand-row recycling");
        model.Rows[999].Caption = "Compiled far record"; Flush(); Check(list.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Compiled far record"), "compiled item template follows recycled model updates");
        var tree = view.FindControl<TreeView>("Tree")!; ((TreeViewItem)tree.ContainerFromIndex(0)!).SetCurrentValue(TreeViewItem.IsExpandedProperty, true); Flush();
        Check(model.Roots[0].Expanded && tree.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Child"), "compiled TreeDataTemplate and two-way expanded container style");
        var tabs = view.FindControl<TabControl>("Tabs")!; tabs.SelectedIndex = 999; Flush();
        Check(tabs.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Compiled far record"), "compiled card tab content and virtual headers");
        var strip = tabs.GetVisualDescendants().OfType<CardTabStrip>().Single(); Check(strip.HasOverflow && tabs.GetRealizedContainers().Count() < 80, "compiled card overflow bindings preserve header virtualization");
        var carousel = view.FindControl<Carousel>("Carousel")!; carousel.SelectedIndex = 999; Flush(); Check(carousel.GetRealizedContainers().Count() < 8 && carousel.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Compiled far record"), "compiled Carousel template and virtual page realization");
        var table = view.FindControl<DataGrid>("Table")!; table.ScrollIntoView(model.Rows[999], table.Columns[0]); Flush();
        Check(table.GetVisualDescendants().OfType<DataGridRow>().Count() < 80 && table.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Compiled far record"), "compiled DataGrid column binding and virtual rows");
        table.SelectedItem = model.Rows[999]; table.CurrentColumn = table.Columns[0]; Check(table.BeginEdit(), "DataGrid enters edit with compiled column"); Flush();
        var cellEditor = table.GetVisualDescendants().OfType<TextBox>().First(t => t.Text == "Compiled far record"); cellEditor.Text = "Compiled edited row"; table.CommitEdit(); Flush();
        Check(model.Rows[999].Caption == "Compiled edited row", "compiled DataGrid edit writes model without reflection");
        table.Columns[0].SortMemberPath = nameof(AuditRow.Caption);
        Click(table.GetVisualDescendants().OfType<DataGridColumnHeader>().First(h => Equals(h.Content, "Caption"))); Flush();
        Check(table.CollectionView.Cast<AuditRow>().First().Caption == "Compiled edited row", "native DataGrid ascending sort retains row property metadata");
        var grouped = (DataGridCollectionView)table.CollectionView;
        grouped.GroupDescriptions.Add(new DataGridPathGroupDescription(nameof(AuditRow.Expanded))); Flush();
        table.ScrollIntoView(grouped.Groups![0], null); Flush();
        var group = table.GetVisualDescendants().OfType<DataGridRowGroupHeader>().First();
        Check(group.DataContext is DataGridCollectionViewGroup { Key: false }, "native DataGrid path grouping reads the row property and renders the compiled group template");
        var groupToggle = group.GetVisualDescendants().OfType<ToggleButton>().First(b => b.Name == "PART_ExpanderButton"); var expanded = groupToggle.IsChecked; Click(groupToggle);
        Check(groupToggle.IsChecked != expanded && table.GetVisualDescendants().OfType<DataGridRow>().Count() < 80, "native grouped row toggle keeps virtual row bounds");
        model.Page = new AuditRow("Changed content"); Flush(); Check(view.FindControl<TransitioningContentControl>("Transition")!.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Changed content"), "compiled transitioning content template");
        VerifyPopups(window, view, model);
        var refresh = view.FindControl<RefreshContainer>("Refresh")!; var requests = 0; refresh.RefreshRequested += (_, _) => requests++; refresh.RequestRefresh(); Flush(); Check(requests == 1, "native Refresh template request and visualizer");
        var manager = new WindowNotificationManager(window) { MaxItems = 4 }; Flush(); manager.Show(new Notification("AOT notification", "Native and compiled", NotificationType.Success, TimeSpan.Zero)); Flush();
        var card = manager.GetVisualDescendants().OfType<NotificationCard>().Single(); Check(card.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "AOT notification") && card.FontSize == (size == ControlSize.Small ? 12 : size == ControlSize.Large ? 16 : 14), "compiled notification template and inherited density");
        var closed = 0; card.NotificationClosed += (_, _) => closed++; manager.CloseAll(); Flush(); Check(closed == 1 && !manager.GetVisualDescendants().OfType<NotificationCard>().Any(), "no-motion native notification close and removal");
        Motion.SetEnabled(window, true); model.Page = new AuditRow("Animated content"); Flush(); Advance(260); Check(view.FindControl<TransitioningContentControl>("Transition")!.GetVisualDescendants().OfType<TextBlock>().Any(t => t.Text == "Animated content"), "NativeAOT motion adapter finishes content transition");
        manager.Show(new Notification("Animated notification", "Native motion", NotificationType.Warning, TimeSpan.Zero)); Flush(); Advance(240); manager.CloseAll(); Advance(220); Check(!manager.GetVisualDescendants().OfType<NotificationCard>().Any(), "NativeAOT notification enter/exit animations finish");
        Motion.SetEnabled(window, false);
        if (output is not null) { Directory.CreateDirectory(output); using var bitmap = window.CaptureRenderedFrame()!; bitmap.Save(Path.Combine(output, $"aot-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default); }
        window.Close(); Flush();
    }
    private static void VerifyPopups(Window window, AuditView view, AuditModel model)
    {
        var combo = view.FindControl<ComboBox>("Choices")!; combo.IsDropDownOpen = true; Flush(); Check(combo.GetVisualDescendants().OfType<Popup>().Any(p => p.IsOpen && p.Child?.Bounds.Height > 0), "ComboBox popup template opens"); combo.IsDropDownOpen = false;
        var date = view.FindControl<CalendarDatePicker>("Date")!; date.IsDropDownOpen = true; Flush();
        var popupChild = date.GetVisualDescendants().OfType<Popup>().Single(p => p.IsOpen).Child!;
        var calendar = popupChild.GetVisualDescendants().Prepend(popupChild).OfType<Calendar>().Single();
        var month = calendar.DisplayDate; Click(calendar.GetVisualDescendants().OfType<Button>().Single(b => b.Name == "PART_NextButton")); Check(calendar.DisplayDate.Month != month.Month, "calendar internal compiled template navigates month");
        Check(calendar.GetVisualDescendants().OfType<TimePicker>().Single().SelectedTime == model.Time, "calendar time attached property binds through template"); date.IsDropDownOpen = false;
        foreach (var control in new Control[] { view.FindControl<DatePicker>("DateParts")!, view.FindControl<TimePicker>("TimeParts")! })
        { Click(control.GetVisualDescendants().OfType<Button>().First(b => b.Name == "PART_FlyoutButton")); var popup = control.GetVisualDescendants().OfType<Popup>().Single(); Check(popup.IsOpen && popup.Child!.Bounds.Height > 0, "native date/time popup and presenter"); popup.IsOpen = false; }
        var button = view.FindControl<Button>("FlyoutButton")!; button.Flyout!.ShowAt(button); Flush(); Check(button.Flyout.IsOpen, "compiled Flyout content and presenter"); button.Flyout.Hide();
        var menu = view.FindControl<Menu>("Commands")!; var parent = (MenuItem)menu.Items[0]!; parent.IsSubMenuOpen = true; Flush();
        Click((MenuItem)parent.Items[0]!); Check(model.Applied >= 2, "compiled nested menu command"); parent.IsSubMenuOpen = false;
        var list = view.FindControl<ListBox>("Records")!; list.ContextMenu!.Open(list); Flush(); Check(list.ContextMenu.IsOpen, "context-menu theme opens under NativeAOT"); list.ContextMenu.Close();
        var selectable = view.FindControl<SelectableTextBlock>("Selectable")!; selectable.SelectionStart = 0; selectable.SelectionEnd = 3; Check(selectable.SelectedText?.Length == 3, "native selectable text selection");
        ToolTip.SetTip(button, "AOT tooltip"); ToolTip.SetIsOpen(button, true); Flush(); Check(ToolTip.GetIsOpen(button), "native tooltip theme"); ToolTip.SetIsOpen(button, false);
        Check(window.TryFindResource("One.Text", window.ActualThemeVariant, out var textBrush) && textBrush is ISolidColorBrush solid && editorForeground(view) == solid.Color, "dynamic semantic resource matches selected theme");
    }
    private static Color editorForeground(AuditView view) => ((ISolidColorBrush)view.FindControl<TextBox>("Editor")!.Foreground!).Color;
    private static void Click(Control control)
    {
        var top = (TopLevel)TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), top)!.Value;
        top.MouseMove(point); top.MouseDown(point, MouseButton.Left); top.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void ClickAt(Control control, Point local)
    {
        var top = (TopLevel)TopLevel.GetTopLevel(control)!; var point = control.TranslatePoint(local, top)!.Value;
        top.MouseMove(point); top.MouseDown(point, MouseButton.Left); top.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void Advance(int milliseconds)
    {
        var frame = new DispatcherFrame(); var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(milliseconds) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; };
        timer.Start(); Dispatcher.UIThread.PushFrame(frame); Flush();
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Check(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); _checks++; }
}
