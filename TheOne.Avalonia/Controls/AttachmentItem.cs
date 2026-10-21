using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TheOne.Avalonia.Controls;

/// <summary>Presentation state of an attachment. Business persistence remains owned by the consumer.</summary>
public enum AttachmentStatus
{
    Default,
    Uploading,
    Success,
    Error
}

/// <summary>Read-only presentation contract. Lists forward the original item to consumer commands.</summary>
public interface IAttachmentItem : INotifyPropertyChanged
{
    string FileName { get; }
    string? FileUrl { get; }
    long FileSize { get; }
    string SizeText { get; }
    string StatusText { get; }
    double UploadProgress { get; }
    bool IsUploading { get; }
    bool CanOpen { get; }
    bool CanRemove { get; }
    AttachmentStatus Status { get; }
}

/// <summary>A theme-independent attachment presentation model, without HTTP or business relationship logic.</summary>
public sealed class AttachmentItem : IAttachmentItem
{
    private string _fileName = "";
    private string? _fileUrl;
    private long _fileSize;
    private string? _sizeText;
    private string _statusText = "待上传";
    private double _progress;
    private bool _uploading;
    private bool _canRemove = true;
    private AttachmentStatus _status;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string FileName { get => _fileName; set => Set(ref _fileName, value ?? ""); }
    public string? FileUrl
    {
        get => _fileUrl;
        set { if (Set(ref _fileUrl, value)) Notify(nameof(CanOpen)); }
    }
    public long FileSize
    {
        get => _fileSize;
        set { if (Set(ref _fileSize, Math.Max(0, value))) Notify(nameof(SizeText)); }
    }
    public string SizeText { get => _sizeText ?? FormatSize(FileSize); set => Set(ref _sizeText, value); }
    public string StatusText { get => _statusText; set => Set(ref _statusText, value ?? ""); }
    public double UploadProgress
    {
        get => _progress;
        set
        {
            if (!double.IsFinite(value) || value is < 0 or > 100) throw new ArgumentOutOfRangeException(nameof(value));
            Set(ref _progress, value);
        }
    }
    public bool IsUploading
    {
        get => _uploading;
        set { if (Set(ref _uploading, value)) Notify(nameof(CanOpen)); }
    }
    public bool CanOpen => !IsUploading && !string.IsNullOrWhiteSpace(FileUrl);
    public bool CanRemove { get => _canRemove; set => Set(ref _canRemove, value); }
    public AttachmentStatus Status
    {
        get => _status;
        set
        {
            if (!Enum.IsDefined(value)) throw new ArgumentOutOfRangeException(nameof(value));
            Set(ref _status, value);
        }
    }

    /// <summary>Starts an upload with unknown progress; no transfer percentage is fabricated.</summary>
    public void MarkUploading()
    {
        Status = AttachmentStatus.Uploading;
        StatusText = "上传中";
        UploadProgress = 0;
        IsUploading = true;
        CanRemove = false;
    }

    /// <summary>Records the consumer-provided server result without rewriting its URL.</summary>
    public void MarkUploaded(string fileUrl, long fileSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileUrl);
        FileUrl = fileUrl;
        FileSize = fileSize;
        SizeText = FormatSize(FileSize);
        Status = AttachmentStatus.Success;
        StatusText = "上传成功";
        UploadProgress = 100;
        IsUploading = false;
        CanRemove = true;
    }

    /// <summary>Marks failure and permits removing the failed row.</summary>
    public void MarkFailed(string message)
    {
        Status = AttachmentStatus.Error;
        StatusText = message;
        UploadProgress = 0;
        IsUploading = false;
        CanRemove = true;
    }

    public static AttachmentItem CreateUploaded(string fileName, string? fileUrl, long fileSize)
    {
        var item = new AttachmentItem { FileName = fileName, FileUrl = fileUrl, FileSize = fileSize };
        item.SizeText = FormatSize(item.FileSize);
        item.Status = AttachmentStatus.Success;
        item.StatusText = "上传成功";
        item.UploadProgress = 100;
        return item;
    }

    public static string FormatSize(long size) => size >= 1024 * 1024 ? $"{size / 1024d / 1024d:N2} MB" :
        size >= 1024 ? $"{size / 1024d:N0} KB" : size > 0 ? $"{size} B" : "大小未知";

    private bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        Notify(name);
        return true;
    }

    private void Notify(string? name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
