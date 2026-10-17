using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class UploadPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        using var bitmap = new WriteableBitmap(new PixelSize(64, 48), new Vector(96, 96), PixelFormat.Rgba8888,
            AlphaFormat.Premul);
        using var memory = new MemoryStream();
        bitmap.Save(memory, PngBitmapEncoderOptions.Default);
        var bytes = memory.ToArray();
        var file = File(bytes, "cover.png");
        VerifyCommands(check, file, bytes);
        VerifyRaces(check, file, bytes);
        UploadApiPreview.Verify(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var upload = new Upload();
            var window = Open(upload, theme, size);
            check(
                upload.EmptyTemplate != null && upload.FileTemplate != null && upload.ActionsTemplate != null &&
                upload.Bounds.Width > 0, $"Upload/{theme.Key}/{size}: default templates render with inherited density");
            check(upload.FontSize == (size == ControlSize.Small ? 12 : size == ControlSize.Large ? 16 : 14),
                $"Upload/{theme.Key}/{size}: font follows control sizing");
            upload.SelectedFile = file;
            Wait(() => !upload.IsPreviewLoading);
            check(upload.HasFile && upload.PreviewImage != null && upload.ErrorMessage == null,
                $"Upload/{theme.Key}/{size}: selected stream is decoded into thumbnail");
            check(
                upload.GetVisualDescendants().OfType<Image>()
                    .Any(image => ReferenceEquals(image.Source, upload.PreviewImage)),
                "Upload: native Image consumes the owned thumbnail");
            upload.FileTemplate =
                new FuncDataTemplate<Upload>((owner, _) => new TextBlock { Text = "自定义：" + owner.SelectedFile?.Name });
            Flush();
            check(upload.GetVisualDescendants().OfType<TextBlock>().Any(text => text.Text == "自定义：cover.png"),
                "Upload: custom FileTemplate receives Upload and retains selection");
            ((Border)window.Content!).Child = null;
            window.Close();
            Flush();
            check(upload.PreviewImage == null && !upload.IsPreviewLoading, "Upload: detach releases preview state");
            window = Open(upload, theme, size);
            Wait(() => !upload.IsPreviewLoading);
            check(upload.PreviewImage != null, "Upload: reattachment reloads the preserved file");
            window.Close();
            Flush();
            if (output != null)
            {
                var demo = new UploadDemoView();
                var demoWindow = Open(demo, theme, size, 920, 1020);
                demo.Model.ExampleCommand.Execute(null);
                demo.Model.ApiExampleCommand.Execute(null);
                Wait(() => demo.GetVisualDescendants().OfType<Upload>().All(control => !control.IsPreviewLoading));
                check(
                    demo.GetVisualDescendants().OfType<Upload>().First(control => control.Name == "UploadSample")
                        .SelectedFile == demo.Model.File, "Upload: demo uses a compiled two-way selection binding");
                Save(demoWindow, output, $"upload-{theme.Key}-{size}.png");
                SaveApi(demoWindow, demo, output, $"upload-api-{theme.Key}-{size}.png");
                demoWindow.Close();
                Flush();
            }
        }

        if (output != null)
        {
            var demo = new UploadDemoView();
            var window = Open(demo, ThemeVariant.Dark, ControlSize.Large, 390, 880);
            demo.Model.ExampleCommand.Execute(null);
            demo.Model.ApiExampleCommand.Execute(null);
            Wait(() => demo.GetVisualDescendants().OfType<Upload>().All(control => !control.IsPreviewLoading));
            var sample = demo.GetVisualDescendants().OfType<Upload>().Single(control => control.Name == "UploadSample");
            var actions = sample.GetVisualDescendants().OfType<Button>()
                .Where(button => button.Name is "PART_Replace" or "PART_Remove" or "PART_Upload").ToArray();
            check(
                actions.Length == 3 && actions.All(button =>
                    button.TranslatePoint(default, sample) is { } origin && origin.X >= 0 && origin.Y >= 0 &&
                    origin.X + button.Bounds.Width <= sample.Bounds.Width &&
                    origin.Y + button.Bounds.Height <= sample.Bounds.Height),
                "Upload: all native action bounds remain inside the narrow Large card");
            Save(window, output, "upload-Dark-Large-narrow.png");
            SaveApi(window, demo, output, "upload-api-Dark-Large-narrow.png");
            window.Close();
            Flush();
        }
    }

    private static void VerifyCommands(Action<bool, string> check, UploadFile file, byte[] bytes)
    {
        var picker = new Picker { Next = Task.FromResult<UploadFile?>(file) };
        var command = new ProbeCommand();
        var upload = new Upload { FilePicker = picker, UploadCommand = command };
        var window = Open(upload);
        check(
            !upload.HasFile && !upload.RemoveCommand.CanExecute(null) && !upload.RequestUploadCommand.CanExecute(null),
            "Upload: empty selection gates removal and upload");
        var select = upload.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "PART_Select");
        select.Focus();
        window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        Wait(() => !upload.IsPreviewLoading && !upload.IsSelecting && upload.HasFile);
        check(ReferenceEquals(upload.SelectedFile, file) && picker.Calls == 1,
            "Upload: native Enter invokes the single-file picker command");
        var submit = upload.GetVisualDescendants().OfType<Button>().Single(button => button.Name == "PART_Upload");
        submit.Focus();
        window.KeyPress(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        window.KeyRelease(Key.Enter, RawInputModifiers.None, PhysicalKey.Enter, null);
        Flush();
        check(ReferenceEquals(command.Parameter, file) && command.Calls == 1,
            "Upload: native upload button passes the stable UploadFile to host ICommand");
        command.Enabled = false;
        command.Notify();
        Flush();
        check(!submit.IsEffectivelyEnabled && !upload.RequestUploadCommand.CanExecute(null),
            "Upload: host CanExecuteChanged disables the action");
        command.Enabled = true;
        command.Notify();
        upload.IsBusy = true;
        Flush();
        check(
            !upload.SelectCommand.CanExecute(null) && !upload.RemoveCommand.CanExecute(null) &&
            !upload.RequestUploadCommand.CanExecute(null),
            "Upload: busy state prevents replacement, removal and duplicate submission");
        upload.IsBusy = false;
        upload.IsEnabled = false;
        Flush();
        check(!upload.SelectCommand.CanExecute(null) && !upload.RequestUploadCommand.CanExecute(null),
            "Upload: disabled state gates commands as well as native buttons");
        upload.IsEnabled = true;
        picker.Next = Task.FromResult<UploadFile?>(null);
        var cancelled = upload.SelectAsync();
        Wait(() => cancelled.IsCompleted);
        check(ReferenceEquals(upload.SelectedFile, file) && upload.ErrorMessage == null,
            "Upload: picker cancellation preserves selection and thumbnail");
        picker.Next = Task.FromResult<UploadFile?>(File(bytes, "bad.exe"));
        var invalid = upload.SelectAsync();
        Wait(() => invalid.IsCompleted);
        check(ReferenceEquals(upload.SelectedFile, file) && upload.ErrorMessage != null,
            "Upload: unsupported extension preserves the previous file and reports validation");
        upload.ReloadPreview();
        Wait(() => !upload.IsPreviewLoading);
        picker.Next = Task.FromResult<UploadFile?>(new UploadFile("big.png",
            _ => Task.FromResult<Stream>(new MemoryStream(bytes)), 11 * 1024 * 1024));
        var large = upload.SelectAsync();
        Wait(() => large.IsCompleted);
        check(ReferenceEquals(upload.SelectedFile, file) && upload.ErrorMessage != null,
            "Upload: size limit rejects oversized selection before opening a stream");
        upload.SelectedFile = File([1, 2, 3], "broken.png");
        Wait(() => !upload.IsPreviewLoading);
        check(
            upload.HasFile && upload.PreviewImage == null && upload.ErrorMessage != null &&
            !upload.RequestUploadCommand.CanExecute(null),
            "Upload: malformed image produces an error and prevents submission");
        upload.RemoveCommand.Execute(null);
        Flush();
        check(
            upload.SelectedFile == null && upload.PreviewImage == null && upload.ErrorMessage == null &&
            !upload.HasFile, "Upload: removal clears selection, owned preview and errors");
        upload.AllowedExtensions = ".png,.pdf";
        upload.SelectedFile = new UploadFile("document.pdf", _ => Task.FromResult<Stream>(new MemoryStream([1, 2, 3])),
            3, "application/pdf");
        Flush();
        check(
            upload.HasFile && !upload.HasPreview && upload.ErrorMessage == null &&
            upload.RequestUploadCommand.CanExecute(null),
            "Upload: explicitly allowed non-image files use fallback and remain uploadable");
        window.Close();
        Flush();
    }

    private static void VerifyRaces(Action<bool, string> check, UploadFile file, byte[] bytes)
    {
        var delayed = new TaskCompletionSource<Stream>(TaskCreationOptions.RunContinuationsAsynchronously);
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var slow = new UploadFile("slow.png", _ =>
        {
            started.TrySetResult();
            return delayed.Task;
        });
        var upload = new Upload { SelectedFile = slow };
        var window = Open(upload);
        Wait(() => started.Task.IsCompleted);
        upload.SelectedFile = file;
        Wait(() => !upload.IsPreviewLoading);
        var current = upload.PreviewImage;
        var stream = new TrackedStream(bytes);
        delayed.SetResult(stream);
        Wait(() => stream.Disposed);
        check(
            ReferenceEquals(upload.SelectedFile, file) && ReferenceEquals(upload.PreviewImage, current) &&
            upload.ErrorMessage == null,
            "Upload: stale preview completion cannot overwrite a newer selection and closes its stream");
        var pickerCompletion =
            new TaskCompletionSource<UploadFile?>(TaskCreationOptions.RunContinuationsAsynchronously);
        upload.FilePicker = new Picker { Next = pickerCompletion.Task };
        var pending = upload.SelectAsync();
        check(upload.IsSelecting && !upload.SelectCommand.CanExecute(null),
            "Upload: picker cannot be opened twice while pending");
        upload.SelectedFile = File(bytes, "external.png");
        Wait(() => !upload.IsPreviewLoading);
        var rejected = File(bytes, "late.png");
        pickerCompletion.SetResult(rejected);
        Wait(() => pending.IsCompleted);
        check(upload.SelectedFile!.Name == "external.png" && !upload.IsSelecting,
            "Upload: a pending picker cannot overwrite a newer bound selection");
        var rejectedDisposed = false;
        try
        {
            rejected.OpenReadAsync().GetAwaiter().GetResult().Dispose();
        }
        catch (ObjectDisposedException)
        {
            rejectedDisposed = true;
        }

        check(rejectedDisposed, "Upload: an unaccepted picker result is disposed");
        var detachedCompletion =
            new TaskCompletionSource<UploadFile?>(TaskCreationOptions.RunContinuationsAsynchronously);
        upload.FilePicker = new Picker { Next = detachedCompletion.Task };
        var detached = upload.SelectAsync();
        window.Close();
        Flush();
        detachedCompletion.SetResult(File(bytes, "detached.png"));
        Wait(() => detached.IsCompleted);
        check(upload.SelectedFile.Name == "external.png" && upload.PreviewImage == null && !upload.IsSelecting,
            "Upload: detached picker completion does not alter the selection or leak preview state");
        var read = file.OpenReadAsync();
        Wait(() => read.IsCompleted);
        using var readStream = read.GetAwaiter().GetResult();
        check(readStream.CanRead, "Upload: host can open its own fresh stream after preview disposal");
        using var leaseFile = File(bytes, "leased.png");
        var leaseTask = leaseFile.OpenReadAsync();
        Wait(() => leaseTask.IsCompleted);
        using var lease = leaseTask.GetAwaiter().GetResult();
        leaseFile.Dispose();
        check(lease.ReadByte() >= 0, "Upload: disposing a file leaves an already opened read stream usable");
        var disposed = false;
        try
        {
            leaseFile.OpenReadAsync().GetAwaiter().GetResult().Dispose();
        }
        catch (ObjectDisposedException)
        {
            disposed = true;
        }

        check(disposed, "Upload: disposed file rejects new stream leases");
    }

    private static UploadFile File(byte[] bytes, string name)
    {
        return new UploadFile(name, _ => Task.FromResult<Stream>(new MemoryStream(bytes, false)), bytes.Length,
            "image/png");
    }

    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal,
        double width = 480, double height = 460)
    {
        var window = new Window
        {
            Width = width, Height = height, RequestedThemeVariant = theme ?? ThemeVariant.Light,
            Content = new Border { Padding = new Thickness(16), Child = content }
        };
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        return window;
    }

    private static void Wait(Func<bool> done)
    {
        var watch = Stopwatch.StartNew();
        while (!done() && watch.Elapsed < TimeSpan.FromSeconds(8))
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                frame.Continue = false;
            };
            timer.Start();
            Dispatcher.UIThread.PushFrame(frame);
            Flush();
        }

        if (!done()) throw new TimeoutException("Upload operation did not finish.");
        Flush();
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void SaveApi(Window window, UploadDemoView demo, string output, string name)
    {
        var api = demo.GetVisualDescendants().OfType<Upload>().Single(control => control.Name == "ApiUploadSample");
        var card = api.GetVisualAncestors().OfType<Border>().First();
        var scroll = demo.GetVisualDescendants().OfType<ScrollViewer>().First();
        if (card.TranslatePoint(default, scroll) is { } position)
            scroll.Offset = new Vector(scroll.Offset.X, scroll.Offset.Y + position.Y);
        Flush();
        Save(window, output, name);
    }

    private static void Save(Window window, string output, string name)
    {
        Directory.CreateDirectory(output);
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(output, name), PngBitmapEncoderOptions.Default);
    }

    private sealed class TrackedStream(byte[] bytes) : MemoryStream(bytes)
    {
        public bool Disposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class Picker : IUploadFilePicker
    {
        public int Calls { get; private set; }
        public required Task<UploadFile?> Next { get; set; }

        public Task<UploadFile?> PickAsync(TopLevel owner, CancellationToken cancellationToken)
        {
            Calls++;
            return Next;
        }
    }

    private sealed class ProbeCommand : ICommand
    {
        public bool Enabled { get; set; } = true;
        public int Calls { get; private set; }
        public object? Parameter { get; private set; }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return Enabled;
        }

        public void Execute(object? parameter)
        {
            Parameter = parameter;
            Calls++;
        }

        public void Notify()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}