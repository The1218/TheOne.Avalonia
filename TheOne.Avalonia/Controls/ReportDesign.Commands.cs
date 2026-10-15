using System.Windows.Input;
using global::Avalonia;
using global::Avalonia.Interactivity;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    internal ICommand CloseDocumentCommand { get; private set; } = null!;
    private sealed class CloseReportCommand(ReportDesign owner) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        internal void Refresh() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object? parameter) => parameter is ReportDocument document && ReferenceEquals(document, owner.Document);
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            owner._fetchGeneration++; owner._fetchCancellation?.Cancel(); owner._imageCancellation?.Cancel(); owner.State.IsFetching = false;
            owner._apiJson = null; owner._apiRequest = null; owner.State.SelectedBody = null; owner.State.Bodies.Clear(); owner.State.ResponsePreview = ""; owner.State.DataNodes.Clear();
            owner.ShowDesign(); owner.SetCurrentValue(DataSourceProperty, null); owner.SetCurrentValue(DocumentProperty, null);
        }
    }
    /// <summary>Consumer save command. Its parameter is the current ReportDocument; call document.Save() to obtain JSON.</summary>
    public static readonly StyledProperty<ICommand?> SaveCommandProperty = AvaloniaProperty.Register<ReportDesign, ICommand?>(nameof(SaveCommand));
    public ICommand? SaveCommand { get => GetValue(SaveCommandProperty); set => SetValue(SaveCommandProperty, value); }
    /// <summary>Call after the consumer's server save succeeds; executing SaveCommand alone does not imply success.</summary>
    public void MarkSaved() { State.SaveStatus = "已保存"; State.Status = "报表已保存"; }
    private void OnSaveRequested(object? sender, RoutedEventArgs e) { if (SaveCommand is null) OnSaveTemplate(sender, e); }
    private CancellationTokenSource? _imageCancellation;
    private async void OnLoadRemoteImage(object? sender, RoutedEventArgs e) => await RefreshRemoteImagesAsync();
    private async Task RefreshRemoteImagesAsync()
    {
        if (Document is not { } document) return;
        _imageCancellation?.Cancel(); _imageCancellation?.Dispose();
        var cancellation = _imageCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(25));
        try
        {
            await ReportResources.LoadImagesAsync(document, cancellationToken: cancellation.Token);
            if (!cancellation.IsCancellationRequested && ReferenceEquals(Document, document)) { Redraw(); State.Status = "网络图片已加载"; }
        }
        catch (OperationCanceledException) { }
        catch (Exception exception) { if (!cancellation.IsCancellationRequested && ReferenceEquals(_imageCancellation, cancellation) && ReferenceEquals(Document, document)) State.Status = "图片加载失败：" + exception.Message; }
    }
}
