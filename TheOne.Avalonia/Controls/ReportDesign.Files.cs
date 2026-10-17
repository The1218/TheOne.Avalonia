using System.ComponentModel;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

public partial class ReportDesign
{
    public static readonly DirectProperty<ReportDesign, int> ImageFitIndexProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, int>(nameof(ImageFitIndex), c => c.ImageFitIndex,
            (c, v) => c.ImageFitIndex = v);

    public static readonly DirectProperty<ReportDesign, int> SumIndexProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, int>(nameof(SumIndex), c => c.SumIndex, (c, v) => c.SumIndex = v);

    public static readonly DirectProperty<ReportDesign, int> ConditionIndexProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, int>(nameof(ConditionIndex), c => c.ConditionIndex,
            (c, v) => c.ConditionIndex = v);

    public static readonly DirectProperty<ReportDesign, string> FontFamilyNameProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, string>(nameof(FontFamilyName), c => c.FontFamilyName,
            (c, v) => c.FontFamilyName = v);

    public static readonly DirectProperty<ReportDesign, double> FontSizeValueProperty =
        AvaloniaProperty.RegisterDirect<ReportDesign, double>(nameof(FontSizeValue), c => c.FontSizeValue,
            (c, v) => c.FontSizeValue = v);

    private static readonly FilePickerFileType TemplateFile = new("ReportDesign 模板")
        { Patterns = ["*.reportdesign.json", "*.json"] };

    public int ConditionIndex
    {
        get => (int)(SelectedColumn?.Condition ?? ReportCondition.GreaterThan);
        set
        {
            if (!_syncing && SelectedColumn is not null && value is >= 0 and <= 2)
                SelectedColumn.Condition = (ReportCondition)value;
        }
    }

    public double FontSizeValue
    {
        get => SelectedElement is { } element ? element.FontSize * .75 : 12;
        set
        {
            if (!_syncing && SelectedElement is { } element && double.IsFinite(value) && value is >= 6 and <= 150)
                element.FontSize = value / .75;
        }
    }

    public string FontFamilyName
    {
        get
        {
            var family = SelectedElement?.FontFamily;
            return State.EnsureFontFamily(string.IsNullOrWhiteSpace(family)
                ? FontManager.Current.DefaultFontFamily.Name
                : family);
        }
        set
        {
            if (!_syncing && SelectedElement is { } element && !string.IsNullOrWhiteSpace(value))
                element.FontFamily = value;
        }
    }

    public int ImageFitIndex
    {
        get => (int)(SelectedElement?.ImageFit ?? ReportImageFit.Contain);
        set
        {
            if (!_syncing && SelectedElement is { Kind: ReportElementKind.Image } element && value is >= 0 and <= 2)
                element.ImageFit = (ReportImageFit)value;
        }
    }

    public int SumIndex
    {
        get => SelectedColumn?.ShowSum == true ? 1 : 0;
        set
        {
            if (!_syncing && SelectedColumn is not null && value is 0 or 1) SelectedColumn.ShowSum = value == 1;
        }
    }

    private void OnOpenFieldPicker(object? sender, RoutedEventArgs e)
    {
        BindingPicker.Focus();
        BindingPicker.IsDropDownOpen = true;
    }

    private void OnSelectTool(object? sender, RoutedEventArgs e)
    {
        ShowDesign();
        PageSurface.Focus();
    }

    private void OnTextAlignment(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is { } element && sender is Control { Tag: string value } &&
            Enum.TryParse<ReportTextAlignment>(value, out var alignment)) element.TextAlignment = alignment;
    }

    private void OnToggleBorder(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is { } element) element.ShowBorder = !element.ShowBorder;
    }

    private void OnTextList(object? sender, RoutedEventArgs e)
    {
        if (SelectedElement is { Kind: ReportElementKind.Text or ReportElementKind.Table } element &&
            sender is Control { Tag: string value } &&
            Enum.TryParse<ReportTextListStyle>(value, out var style))
            element.ListStyle = element.ListStyle == style ? ReportTextListStyle.None : style;
    }

    private void OnFillSettings(object? sender, RoutedEventArgs e)
    {
        State.PropertyTab = 0;
    }

    private void OnZoomIn(object? sender, RoutedEventArgs e)
    {
        Zoom += .1;
    }

    private void OnZoomOut(object? sender, RoutedEventArgs e)
    {
        Zoom -= .1;
    }

    /// <summary>Assigns a validated, embedded raster image as one undoable edit.</summary>
    public void SetImage(ReportElement element, ReadOnlySpan<byte> bytes, string name = "图片")
    {
        if (Document?.Elements.Contains(element) != true || element.Kind != ReportElementKind.Image)
            throw new ArgumentException("请选择当前报表中的图片组件。", nameof(element));
        if (bytes.Length > ReportImageData.MaximumBytes) throw new ArgumentException("单张图片不能超过 8 MiB。");
        var data = Convert.ToBase64String(bytes);
        var decoded = ReportImageData.Decode(data);
        using var bitmap = new Bitmap(new MemoryStream(decoded, false));
        var previous = _batch;
        _batch = true;
        try
        {
            element.ImageUrl = "";
            element.ResolvedImageUrl = "";
            element.ResolvedImageData = "";
            element.ImageData = data;
            element.ImageName = name;
        }
        finally
        {
            _batch = previous;
        }

        DefinitionChanged(this, new PropertyChangedEventArgs(nameof(ReportElement.ImageData)));
    }

    /// <summary>Serializes only the report definition, bindings and embedded images.</summary>
    public string SaveTemplate()
    {
        return ReportTemplate.Save(Document ?? throw new InvalidOperationException("没有可保存的报表。"));
    }

    /// <summary>Validates a template, replaces the document and automatically restores its optional API data.</summary>
    public void LoadTemplate(string json)
    {
        var document = ReportTemplate.Load(json);
        ShowDesign();
        SetCurrentValue(DataSourceProperty, null);
        SetCurrentValue(DocumentProperty, document);
        SetCurrentValue(SelectedElementProperty, document.Elements.FirstOrDefault());
        State.Status = document.DataSourceUrl.Length > 0 ? "模板已加载，正在恢复数据源…" : "模板已加载；可继续设计或连接 API 数据源";
        State.SaveStatus = "已加载";
    }

    /// <summary>Exports every preview page as a vector PDF. Must be called on the UI thread.</summary>
    public Task ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default)
    {
        return ReportPdfExporter.ExportAsync(Document ?? throw new InvalidOperationException("没有可导出的报表。"), DataSource,
            destination, cancellationToken: cancellationToken);
    }

    private async void OnSaveTemplate(object? sender, RoutedEventArgs e)
    {
        try
        {
            var json = SaveTemplate();
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage?.CanSave != true)
            {
                State.Status = "当前平台不支持保存文件对话框。";
                return;
            }

            var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "保存报表模板", SuggestedFileName = SafeFileName() + ".reportdesign.json",
                DefaultExtension = "reportdesign.json", FileTypeChoices = [TemplateFile]
            });
            if (file is null) return;
            using (file)
            {
                await using var stream = await file.OpenWriteAsync();
                if (stream.CanSeek) stream.SetLength(0);
                await stream.WriteAsync(Encoding.UTF8.GetBytes(json));
            }

            State.Status = "模板已保存";
            State.SaveStatus = "已保存";
        }
        catch (Exception exception)
        {
            State.Status = "保存失败：" + exception.Message;
        }
    }

    private async void OnLoadTemplate(object? sender, RoutedEventArgs e)
    {
        try
        {
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage?.CanOpen != true)
            {
                State.Status = "当前平台不支持打开文件对话框。";
                return;
            }

            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
                { Title = "打开报表模板", AllowMultiple = false, FileTypeFilter = [TemplateFile] });
            if (files.Count == 0) return;
            using var file = files[0];
            await using var stream = await file.OpenReadAsync();
            using var buffer = new MemoryStream();
            await ReadLimited(stream, buffer, ReportTemplate.MaximumBytes);
            await LoadTemplateAsync(Encoding.UTF8.GetString(buffer.ToArray()));
        }
        catch (Exception exception)
        {
            State.Status = "打开或恢复数据失败：" + exception.Message;
        }
    }

    private async void OnChooseImage(object? sender, RoutedEventArgs e)
    {
        var element = SelectedElement;
        if (element?.Kind != ReportElementKind.Image) return;
        try
        {
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage?.CanOpen != true)
            {
                State.Status = "当前平台不支持打开图片对话框。";
                return;
            }

            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择图片", AllowMultiple = false,
                FileTypeFilter =
                    [new FilePickerFileType("图片") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp"] }]
            });
            if (files.Count == 0) return;
            using var file = files[0];
            await using var stream = await file.OpenReadAsync();
            using var buffer = new MemoryStream();
            await ReadLimited(stream, buffer, ReportImageData.MaximumBytes);
            SetImage(element, buffer.ToArray(), file.Name);
            State.Status = "图片已嵌入报表";
        }
        catch (Exception exception)
        {
            State.Status = "图片加载失败：" + exception.Message;
        }
    }

    private async void OnExportPdf(object? sender, RoutedEventArgs e)
    {
        if (State.IsExporting) return;
        State.IsExporting = true;
        try
        {
            using var buffer = new MemoryStream();
            await ExportPdfAsync(buffer);
            var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
            if (storage?.CanSave != true)
            {
                State.Status = "当前平台不支持保存 PDF 对话框。";
                return;
            }

            var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "导出 PDF", SuggestedFileName = SafeFileName() + ".pdf", DefaultExtension = "pdf",
                FileTypeChoices = [new FilePickerFileType("PDF 文档") { Patterns = ["*.pdf"] }]
            });
            if (file is null) return;
            using (file)
            {
                await using var stream = await file.OpenWriteAsync();
                if (stream.CanSeek) stream.SetLength(0);
                buffer.Position = 0;
                await buffer.CopyToAsync(stream);
            }

            State.Status = $"已导出 {Layout?.Pages.Count ?? 1} 页 PDF";
        }
        catch (Exception exception)
        {
            State.Status = "导出失败：" + exception.Message;
        }
        finally
        {
            State.IsExporting = false;
        }
    }

    private string SafeFileName()
    {
        return string.Concat((Document?.Title ?? "报表").Select(c =>
            Path.GetInvalidFileNameChars().Contains(c) || c is '/' or '\\' ? '_' : c));
    }

    private static async Task ReadLimited(Stream source, Stream destination, int maximum)
    {
        var bytes = new byte[81920];
        var total = 0;
        while (await source.ReadAsync(bytes) is var count && count > 0)
        {
            total += count;
            if (total > maximum) throw new ArgumentException($"文件不能超过 {maximum / 1024 / 1024} MiB。");
            await destination.WriteAsync(bytes.AsMemory(0, count));
        }
    }
}