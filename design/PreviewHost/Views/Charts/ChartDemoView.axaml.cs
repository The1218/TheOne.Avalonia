using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;



public partial class ChartDemoView : UserControl
{
    public static readonly DirectProperty<ChartDemoView, ChartBase> ChartSampleProperty = AvaloniaProperty.RegisterDirect<ChartDemoView, ChartBase>(nameof(ChartSample), view => view.ChartSample);
    private ChartBase _chartSample = new LineChart();
    public ChartBase ChartSample => _chartSample;
    private DispatcherTimer? _rapidUpdate;
    public bool IsRapidUpdating => _rapidUpdate is not null;
    public ChartDemoViewModel Model { get; } = new();
    public ChartKind FixedKind { get => Model.Kind; set { Model.IsFixedKind = false; Model.KindIndex = (int)value; Model.IsFixedKind = true; } }
    public ChartDemoView()
    {
        InitializeComponent(); DataContext = Model;
        AttachedToVisualTree += (_, _) => { Model.StartObservingSeries(); Model.PropertyChanged += ModelChanged; ReplaceChart(); };
        DetachedFromVisualTree += (_, _) => { Model.PropertyChanged -= ModelChanged; Model.StopObservingSeries(); StopRapidUpdate(); };
    }
    private void ModelChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ChartDemoViewModel.Kind)) ReplaceChart();
    }
    private void ReplaceChart()
    {
        _chartSample.SelectionChanged -= OnSelectionChanged;
        var next = ChartControlFactory.CreateControl(Model.Kind); next.Classes.Add("demo-chart"); next.SelectionChanged += OnSelectionChanged;
        SetAndRaise(ChartSampleProperty, ref _chartSample, next);
    }
    private void OnSelectionChanged(object? sender, ChartSelectionChangedEventArgs args)
    {
        Model.UpdateSelectionDetail(args.Selection);
        if (sender is ChartBase chart && args.Selection is { } selection && chart.GetStackPercentage(selection.SeriesIndex, selection.PointIndex) is { } share)
            Model.SelectedDetail += $" · 占比 {share:P1}";
    }
    private void OnUpdateData(object? sender, RoutedEventArgs args) { Model.UpdateData(); OnSelectionChanged(ChartSample, new ChartSelectionChangedEventArgs(ChartSample.Selection)); }
    private void OnThousandPoints(object? sender, RoutedEventArgs args) => Model.LoadThousandPoints();
    private void OnHundredThousandPoints(object? sender, RoutedEventArgs args) => Model.LoadHundredThousandPoints();
    private void OnGapData(object? sender, RoutedEventArgs args) => Model.LoadGapData();
    private void OnStackData(object? sender, RoutedEventArgs args) => Model.LoadStackData();
    private void OnSecondaryData(object? sender, RoutedEventArgs args) => Model.LoadSecondaryData();
    private void OnThousandSeries(object? sender, RoutedEventArgs args) => Model.LoadThousandSeries();
    private void OnReset(object? sender, RoutedEventArgs args) { StopRapidUpdate(); Model.Reset(); }
    public void StopRapidUpdate() { _rapidUpdate?.Stop(); _rapidUpdate = null; }
    private void OnRapidUpdate(object? sender, RoutedEventArgs args)
    {
        StopRapidUpdate(); var count = 0; var timer = _rapidUpdate = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(120) };
        timer.Tick += (_, _) => { OnUpdateData(this, args); if (++count == 5) StopRapidUpdate(); }; timer.Start();
    }
    private async void OnExportPng(object? sender, RoutedEventArgs args)
    {
        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { CanSave: true } storage) { Model.ExportStatus = "当前宿主不支持保存文件。"; return; }
        try
        {
            var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions { Title = "导出图表 PNG", SuggestedFileName = "chart.png", DefaultExtension = "png", FileTypeChoices = new[] { new FilePickerFileType("PNG 图像") { Patterns = new[] { "*.png" }, MimeTypes = new[] { "image/png" } } } });
            if (file is null) { Model.ExportStatus = "已取消导出。"; return; }
            await using var output = await file.OpenWriteAsync(); ChartSample.ExportPng(output); Model.ExportStatus = $"已保存 {file.Name}";
        }
        catch (Exception error) { Model.ExportStatus = $"导出失败：{error.Message}"; }
    }
    private void OnZoomIn(object? sender, RoutedEventArgs args) => ChartSample.Zoom(1.5);
    private void OnZoomOut(object? sender, RoutedEventArgs args) => ChartSample.Zoom(1 / 1.5);
    private void OnPanLeft(object? sender, RoutedEventArgs args) => ChartSample.Pan(-0.2, 0);
    private void OnPanRight(object? sender, RoutedEventArgs args) => ChartSample.Pan(0.2, 0);
    private void OnResetViewport(object? sender, RoutedEventArgs args) => ChartSample.ResetViewport();
    private void OnLastPoint(object? sender, RoutedEventArgs args)
    {
        Model.SelectedSeries = 0; Model.SelectedPoint = (Model.Series.FirstOrDefault()?.ItemsSource?.Count ?? 0) - 1; ChartSample.BringDataPointIntoView(Model.SelectedSeries, Model.SelectedPoint); ChartSample.Focus();
    }
}
