using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class ImagePreviewChecks
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        var directory = Path.GetFullPath(output ?? ".preview/image-preview/checks");
        Directory.CreateDirectory(directory);
        using var bitmap = ImagePreviewDemoViewModel.CreateTransparentSample();
        var local = Path.Combine(directory, "local-image.png");
        bitmap.Save(local, PngBitmapEncoderOptions.Default);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyPresentation(bitmap, theme, size, directory, check);
        VerifySources(bitmap, local, directory, check);
        VerifyBindingAndFullScreen(directory, check);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Until(Func<bool> ready)
    {
        var timer = Stopwatch.StartNew();
        while (!ready() && timer.Elapsed < TimeSpan.FromSeconds(12))
        {
            Flush();
            Thread.Sleep(5);
        }

        Flush();
        if (!ready()) throw new InvalidOperationException("ImagePreview async operation timed out.");
    }

    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal,
        double width = 800)
    {
        var window = new Window
        {
            Width = width, Height = 560, Content = content, RequestedThemeVariant = theme ?? ThemeVariant.Dark,
            WindowDecorations = WindowDecorations.None
        };
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static T Part<T>(ImagePreview preview, string name) where T : Control
    {
        return preview.GetVisualDescendants().OfType<T>()
            .Single(part => part.Name == name && part.TemplatedParent == preview);
    }

    private static Point Center(Control control, Window window)
    {
        return control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
    }

    private static void Click(Window window, Control control)
    {
        var point = Center(control, window);
        window.MouseMove(point);
        window.MouseDown(point, MouseButton.Left);
        window.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static void Save(Window window, string directory, string name)
    {
        using var frame = window.CaptureRenderedFrame()!;
        frame.Save(Path.Combine(directory, name + ".png"), PngBitmapEncoderOptions.Default);
    }

    private static void VerifyPresentation(Bitmap bitmap, ThemeVariant theme, ControlSize size, string directory,
        Action<bool, string> check)
    {
        var preview = new ImagePreview { Source = bitmap };
        var window = Open(preview, theme, size);
        var label = $"ImagePreview/{theme.Key}/{size}";
        check(
            preview.HasImage && ReferenceEquals(preview.CurrentImage, bitmap) && preview.ImageCount == 1 &&
            !preview.HasError, label + ": caller Bitmap renders directly");
        check(
            !Part<Button>(preview, "PART_Previous").IsVisible && !Part<Button>(preview, "PART_Next").IsVisible &&
            !Part<TextBlock>(preview, "PART_Page").IsVisible,
            label + ": single image hides navigation and count");
        check(preview.IsFitToWindow && Part<ImagePreviewSurface>(preview, "PART_Viewport").Bounds.Width > 700,
            label + ": image gets the available viewport");
        check(preview.ChromeHeight == (size == ControlSize.Small ? 36 : size == ControlSize.Large ? 52 : 44),
            label + ": chrome inherits density");
        Save(window, directory, $"single-{theme.Key}-{size}");
        // Opaque and transparent fixture regions must both appear in real pixels.
        using (var frame = window.CaptureRenderedFrame()!)
        {
            var viewport = Part<ImagePreviewSurface>(preview, "PART_Viewport");
            var center = Center(viewport, window);
            var pixel = ReadPixel(frame, center + new Vector(-80, 0));
            check(pixel.G > 140 && pixel.B > 140 && pixel.R < 70, label + ": actual frame contains the source bitmap");
        }

        Click(window, Part<Button>(preview, "PART_Actual"));
        check(!preview.IsFitToWindow && preview.ZoomFactor == 1, label + ": 1:1 uses native button input");
        var surface = Part<ImagePreviewSurface>(preview, "PART_Viewport");
        var p = Center(surface, window);
        var old = preview.ZoomFactor;
        window.MouseWheel(p, new Vector(0, 2));
        Flush();
        check(preview.ZoomFactor > old && !preview.IsFitToWindow, label + ": wheel zoom is handled by the viewport");
        preview.ZoomFactor = 4;
        Flush();
        var beforePan = CapturePixel(window, Center(surface, window) + new Vector(80, 0));
        window.MouseDown(p, MouseButton.Left);
        window.MouseMove(p + new Vector(150, 40));
        window.MouseUp(p + new Vector(150, 40), MouseButton.Left);
        Flush();
        var afterPan = CapturePixel(window, Center(surface, window) + new Vector(80, 0));
        check(beforePan != afterPan, label + ": pointer pan changes the rendered image position");
        preview.FitToWindow();
        Flush();
        check(preview.IsFitToWindow, label + ": fit restores centered layout");
        Click(window, surface);
        Click(window, surface);
        check(!preview.IsFitToWindow && preview.ZoomFactor == 1,
            label + ": double-click toggles fit to original pixels");
        preview.ZoomFactor = 3;
        preview.IsEnabled = false;
        Flush();
        window.MouseWheel(p, new Vector(0, 2));
        Flush();
        check(preview.ZoomFactor == 3, label + ": disabled viewer ignores zoom input");
        preview.IsEnabled = true;
        preview.FitToWindow();
        preview.Source = null;
        Flush();
        check(!preview.HasImage && !preview.IsLoading && Part<StackPanel>(preview, "PART_Empty").IsVisible,
            label + ": empty source has an explicit empty state");
        preview.Source = bitmap;
        Flush();
        window.Width = 320;
        Flush();
        check(!Part<Button>(preview, "PART_Actual").IsVisible && Part<Button>(preview, "PART_Fit").Bounds.Width > 0 &&
              Part<Button>(preview, "PART_FullScreen").Bounds.Right <= preview.Bounds.Width,
            label + ": narrow view keeps essential tools without clipping");
        Save(window, directory, $"narrow-{theme.Key}-{size}");
        window.Close();
        using var retained = new MemoryStream();
        bitmap.Save(retained, PngBitmapEncoderOptions.Default);
        check(retained.Length > 0, label + ": detaching does not dispose the consumer Bitmap");
    }

    private static Color CapturePixel(Window window, Point point)
    {
        using var frame = window.CaptureRenderedFrame()!;
        return ReadPixel(frame, point);
    }

    private static Color ReadPixel(Bitmap bitmap, Point point)
    {
        var bytes = new byte[4];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            var x = Math.Clamp((int)(point.X * bitmap.Dpi.X / 96), 0, bitmap.PixelSize.Width - 1);
            var y = Math.Clamp((int)(point.Y * bitmap.Dpi.Y / 96), 0, bitmap.PixelSize.Height - 1);
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), handle.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            handle.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static void VerifySources(Bitmap bitmap, string local, string directory, Action<bool, string> check)
    {
        using var server = new ImageServer(File.ReadAllBytes(local));
        var items = new ObservableCollection<object> { bitmap, local, server.Url + "ok.png" };
        var preview = new ImagePreview { ItemsSource = items, SelectedIndex = 1 };
        var window = Open(preview);
        Until(() => !preview.IsLoading);
        check(
            preview.HasImage && preview.ImageCount == 3 && preview.SelectedIndex == 1 &&
            preview.DisplayTitle == "local-image.png",
            "ImagePreview/local: initial index and file load survive binding order");
        check(preview.CanPrevious && preview.CanNext && preview.PageText == "2 / 3",
            "ImagePreview/multiple: navigation state reflects the current index");
        Click(window, Part<Button>(preview, "PART_Next"));
        Until(() => !preview.IsLoading);
        check(preview.HasImage && !preview.HasError && !preview.CanNext && preview.PageText == "3 / 3",
            "ImagePreview/http: real HTTP request decodes the remote image and disables last-page navigation");
        Save(window, directory, "multiple-http");
        preview.Next();
        check(preview.SelectedIndex == 2, "ImagePreview/multiple: next does not wrap");
        preview.ZoomIn();
        preview.Previous();
        Until(() => !preview.IsLoading);
        check(preview.IsFitToWindow, "ImagePreview/multiple: switching resets zoom");
        items.Insert(0, new object());
        Flush();
        check(preview.SelectedIndex == 2 && preview.HasImage,
            "ImagePreview/collection: inserting preserves the selected source by reference");
        items.Remove(local);
        Until(() => !preview.IsLoading);
        check(preview.SelectedIndex == 2 && preview.DisplayTitle == "ok.png",
            "ImagePreview/collection: removing selection selects the next valid position");
        items.Clear();
        Flush();
        check(preview.SelectedIndex == -1 && preview.ImageCount == 0 && !preview.HasImage,
            "ImagePreview/collection: clearing resets selection and image");
        items.Add(bitmap);
        Flush();
        check(preview.SelectedIndex == 0 && preview.HasImage && !preview.IsMultiple,
            "ImagePreview/collection: adding to empty restores single image mode");
        preview.ItemsSource = null;
        preview.Source = new Uri(local);
        Until(() => !preview.IsLoading);
        check(preview.HasImage && !preview.HasError, "ImagePreview/file-uri: local file URI is supported");
        preview.Source = server.Url + "retry.png";
        Until(() => !preview.IsLoading);
        check(preview.HasError && !preview.HasImage && Part<StackPanel>(preview, "PART_Error").IsVisible,
            "ImagePreview/http-error: failure presents retry without stale pixels");
        server.AllowRetry = true;
        Click(window, Part<Button>(preview, "PART_Retry"));
        Until(() => !preview.IsLoading);
        check(preview.HasImage && !preview.HasError, "ImagePreview/retry: native retry button successfully reloads");
        preview.Source = server.Url + "slow.png";
        Flush();
        check(preview.IsLoading && Part<StackPanel>(preview, "PART_Loading").IsVisible,
            "ImagePreview/loading: async source has a visible loading state");
        Until(() => server.SlowStarted);
        preview.Source = bitmap;
        Flush();
        Until(() => server.SlowCompleted);
        check(ReferenceEquals(preview.CurrentImage, bitmap) && !preview.IsLoading && !preview.HasError,
            "ImagePreview/race: a canceled HTTP result cannot replace a newer Bitmap");
        preview.Source = Path.Combine(directory, "does-not-exist.png");
        Until(() => !preview.IsLoading);
        check(preview.HasError, "ImagePreview/local-error: missing files fail without throwing on the UI thread");
        preview.Source = server.Url + "invalid.png";
        Until(() => !preview.IsLoading);
        check(preview.HasError && !preview.HasImage,
            "ImagePreview/decode-error: corrupt images expose a failure state");
        preview.Source = server.Url + "slow.png";
        Flush();
        window.Close();
        Flush();
        check(!preview.IsLoading && !preview.HasImage,
            "ImagePreview/detach: closing cancels loading and releases owned images");
        window.Content = null;
        window = Open(preview);
        Until(() => !preview.IsLoading);
        check(preview.HasImage && !preview.HasError, "ImagePreview/reattach: detached controls can load again");
        window.Close();
    }

    private static void VerifyBindingAndFullScreen(string directory, Action<bool, string> check)
    {
        var view = new ImagePreviewDemoView();
        var window = Open(new ScrollViewer { Content = view });
        var preview = view.FindControl<ImagePreview>("MultipleImagePreview")!;
        view.Model.SelectedIndex = 1;
        Flush();
        check(preview.SelectedIndex == 1 && ReferenceEquals(preview.CurrentImage, view.Model.TransparentSample),
            "ImagePreview/binding: typed compiled SelectedIndex binding drives selection");
        preview.Previous();
        Flush();
        check(view.Model.SelectedIndex == 0, "ImagePreview/binding: default TwoWay writes selection back to VM");
        view.Model.SelectedIndex = 999;
        Flush();
        check(preview.SelectedIndex == 1 && view.Model.SelectedIndex == 1,
            "ImagePreview/binding: out-of-range index clamps and writes back");
        preview.Focus();
        window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
        window.KeyRelease(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
        Flush();
        check(preview.SelectedIndex == 0 && view.Model.SelectedIndex == 0,
            "ImagePreview/keyboard: focused viewer supports arrow navigation");
        preview.ToggleFullScreen();
        Flush();
        var lifetime = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        var full = lifetime.Windows.Single(w => w.Content is ImagePreview { IsFullScreen: true });
        var fullPreview = (ImagePreview)full.Content!;
        check(preview.IsFullScreen && fullPreview.HasImage && preview.Parent is not null,
            "ImagePreview/fullscreen: separate owned window leaves embedded viewer attached");
        fullPreview.Next();
        Flush();
        check(preview.SelectedIndex == 1 && view.Model.SelectedIndex == 1,
            "ImagePreview/fullscreen: selection stays synchronized with the original VM binding");
        fullPreview.Focus();
        full.KeyPress(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
        window.KeyRelease(Key.Escape, RawInputModifiers.None, PhysicalKey.Escape, null);
        Flush();
        check(!preview.IsFullScreen && !lifetime.Windows.Contains(full),
            "ImagePreview/fullscreen: Escape closes only the preview window");
        view.Model.SelectedIndex = 0;
        Flush();
        check(preview.SelectedIndex == 0, "ImagePreview/fullscreen: original binding remains intact after closing");
        preview.ShowCloseButton = true;
        var close = 0;
        preview.CloseRequested += (_, _) => close++;
        Part<Button>(preview, "PART_Close").RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Flush();
        check(close == 1 && window.IsVisible, "ImagePreview/close: embedded close requests are delegated to the host");
        window.Close();
        var photo = new ImagePreview { Source = view.Model.Sample, Title = "alpine-lake.png" };
        var photoWindow = Open(photo, width: 960);
        Save(photoWindow, directory, "photo-single");
        photoWindow.Close();
        var gallery = new ImagePreview { ItemsSource = view.Model.Images, Title = "山湖 · 多图预览" };
        var galleryWindow = Open(gallery, width: 960);
        Save(galleryWindow, directory, "photo-multiple");
        galleryWindow.Close();
        view.Model.Sample.Dispose();
        view.Model.TransparentSample.Dispose();
    }

    private sealed class ImageServer : IDisposable
    {
        private readonly byte[] _image;
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource _stop = new();
        public volatile bool AllowRetry, SlowStarted, SlowCompleted;

        public ImageServer(byte[] image)
        {
            _image = image;
            _listener.Start();
            Url = $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/";
            _ = Run();
        }

        public string Url { get; }

        public void Dispose()
        {
            _stop.Cancel();
            _listener.Stop();
        }

        private async Task Run()
        {
            try
            {
                while (!_stop.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync(_stop.Token);
                    _ = Respond(client);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async Task Respond(TcpClient client)
        {
            using (client)
            {
                try
                {
                    await using var stream = client.GetStream();
                    using var reader = new StreamReader(stream, Encoding.ASCII, false, 1024, true);
                    var request = await reader.ReadLineAsync(_stop.Token) ?? "";
                    while (!string.IsNullOrEmpty(await reader.ReadLineAsync(_stop.Token)))
                    {
                    }

                    if (request.Contains("slow.png"))
                    {
                        SlowStarted = true;
                        await Task.Delay(300, _stop.Token);
                        SlowCompleted = true;
                    }

                    var failed = request.Contains("retry.png") && !AllowRetry;
                    var data = request.Contains("invalid.png") ? Encoding.UTF8.GetBytes("not an image") : _image;
                    var header = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 {(failed ? "503 Unavailable" : "200 OK")}\r\nContent-Length: {data.Length}\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header, _stop.Token);
                    await stream.WriteAsync(data, _stop.Token);
                }
                catch (Exception e) when (e is IOException or OperationCanceledException or ObjectDisposedException)
                {
                }
            }
        }
    }
}