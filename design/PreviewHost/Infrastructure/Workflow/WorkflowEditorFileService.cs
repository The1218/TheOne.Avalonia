using Avalonia.Platform.Storage;

namespace TheOne.DesignPreview;

public interface IWorkflowEditorFileService
{
    Task<bool> SaveAsync(string json);
    Task<string?> OpenAsync();
}

/// <summary>Desktop composition service. The ViewModel only depends on its interface.</summary>
public sealed class WorkflowEditorFileService(Func<IStorageProvider?> storageProvider) : IWorkflowEditorFileService
{
    public async Task<bool> SaveAsync(string json)
    {
        var storage = storageProvider();
        if (storage?.CanSave != true) throw new NotSupportedException("当前宿主没有文件保存服务，请使用 JSON 文本。");
        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
            { Title = "保存流程", SuggestedFileName = "workflow.json", DefaultExtension = "json" });
        if (file == null) return false;
        using (file)
        {
            await using var stream = await file.OpenWriteAsync();
            stream.SetLength(0);
            using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);
        }

        return true;
    }

    public async Task<string?> OpenAsync()
    {
        var storage = storageProvider();
        if (storage?.CanOpen != true) throw new NotSupportedException("当前宿主没有文件打开服务，请使用 JSON 文本。");
        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "加载流程", AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("Workflow JSON") { Patterns = ["*.json"] }]
        });
        if (files.Count == 0) return null;
        using var file = files[0];
        await using var stream = await file.OpenReadAsync();
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}