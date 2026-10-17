using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Platform;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>Demonstrates both host commands and the built-in HTTP upload contract.</summary>
public sealed class UploadDemoViewModel : INotifyPropertyChanged
{
    private UploadFile? _apiFile;
    private bool _busy;
    private UploadFile? _file;
    private double _progress;
    private UploadRequestType _requestType = UploadRequestType.Post;
    private string _status = "选择一张图片后，使用宿主命令读取其文件流。";
    private string? _successFileUrl;
    private string _uploadApi = "";

    public UploadDemoViewModel()
    {
        ReadCommand = new UploadDemoCommand(ReadAsync);
        ExampleCommand = new UploadDemoCommand(_ =>
        {
            File = ExampleFile;
            return Task.CompletedTask;
        });
        ApiExampleCommand = new UploadDemoCommand(_ =>
        {
            ApiFile = ApiExampleFile;
            return Task.CompletedTask;
        });
    }

    public UploadFile? File
    {
        get => _file;
        set
        {
            if (ReferenceEquals(_file, value)) return;
            var old = _file;
            _file = value;
            if (!ReferenceEquals(old, ExampleFile)) old?.Dispose();
            Notify();
            Status = value == null ? "尚未选择图片。" : "图片已选择，尚未提交。";
        }
    }

    public UploadFile? ApiFile
    {
        get => _apiFile;
        set
        {
            if (ReferenceEquals(_apiFile, value)) return;
            var old = _apiFile;
            _apiFile = value;
            if (!ReferenceEquals(old, ApiExampleFile)) old?.Dispose();
            Notify();
        }
    }

    public string UploadApi
    {
        get => _uploadApi;
        set
        {
            if (_uploadApi == value) return;
            _uploadApi = value;
            Notify();
        }
    }

    public UploadRequestType RequestType
    {
        get => _requestType;
        set
        {
            if (_requestType == value) return;
            _requestType = value;
            Notify();
        }
    }

    public string? SuccessFileUrl
    {
        get => _successFileUrl;
        set
        {
            if (_successFileUrl == value) return;
            _successFileUrl = value;
            Notify();
        }
    }

    public UploadRequestType[] RequestTypes { get; } =
        [UploadRequestType.Post, UploadRequestType.Put, UploadRequestType.Patch];

    public bool IsBusy
    {
        get => _busy;
        private set
        {
            _busy = value;
            Notify();
        }
    }

    public double Progress
    {
        get => _progress;
        private set
        {
            _progress = value;
            Notify();
        }
    }

    public string Status
    {
        get => _status;
        private set
        {
            _status = value;
            Notify();
        }
    }

    public UploadFile ExampleFile { get; } = new("preview-lake.png",
        _ => Task.FromResult(AssetLoader.Open(
            new Uri($"avares://{typeof(UploadDemoViewModel).Assembly.GetName().Name}/Assets/preview-lake.png"))),
        contentType: "image/png");

    public UploadFile ApiExampleFile { get; } = new("preview-lake.png",
        _ => Task.FromResult(AssetLoader.Open(
            new Uri($"avares://{typeof(UploadDemoViewModel).Assembly.GetName().Name}/Assets/preview-lake.png"))),
        contentType: "image/png");

    public ICommand ReadCommand { get; }
    public ICommand ExampleCommand { get; }
    public ICommand ApiExampleCommand { get; }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void Notify([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    private async Task ReadAsync(object? parameter)
    {
        if (parameter is not UploadFile file || IsBusy) return;
        IsBusy = true;
        Progress = 0;
        Status = "宿主正在读取选中文件…";
        try
        {
            await using var stream = await file.OpenReadAsync();
            var buffer = new byte[81920];
            long total = 0;
            int count;
            while ((count = await stream.ReadAsync(buffer)) > 0)
            {
                total += count;
                Progress = file.Size is > 0 ? Math.Min(100, total * 100d / file.Size.Value) : 0;
            }

            Progress = 100;
            Status = $"宿主已读取 {file.Name}（{total:N0} 字节）。此示例未发送网络请求。";
        }
        catch (Exception error)
        {
            Status = "读取失败：" + error.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private sealed class UploadDemoCommand(Func<object?, Task> execute) : ICommand
    {
        private bool _executing;
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return !_executing;
        }

        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            _executing = true;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            try
            {
                await execute(parameter);
            }
            finally
            {
                _executing = false;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}