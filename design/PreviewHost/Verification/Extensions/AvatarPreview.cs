using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class AvatarPreview
{
    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static void Wait(Func<bool> done)
    {
        var watch = Stopwatch.StartNew();
        while (!done() && watch.Elapsed < TimeSpan.FromSeconds(5))
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

        if (!done()) throw new TimeoutException("Avatar load did not complete.");
        Flush();
    }

    private static Window Open(Control content, ThemeVariant theme, ControlSize size, double width = 720,
        double height = 700)
    {
        var window = new Window { Width = width, Height = height, RequestedThemeVariant = theme, Content = content };
        ControlSizing.SetSize(window, size);
        window.Show();
        Flush();
        return window;
    }

    public static void Verify(Action<bool, string> check, string? output = null)
    {
        var directory = Path.GetFullPath(output ?? ".preview/avatar");
        Directory.CreateDirectory(directory);
        using var image = new WriteableBitmap(new PixelSize(64, 64), new Vector(96, 96), PixelFormat.Rgba8888,
            AlphaFormat.Premul);
        using (var pixels = image.Lock())
        {
            var row = Enumerable.Range(0, 64).SelectMany(_ => new byte[] { 228, 110, 86, 255 }).ToArray();
            for (var y = 0; y < 64; y++) Marshal.Copy(row, 0, pixels.Address + y * pixels.RowBytes, row.Length);
        }

        var file = Path.Combine(directory, "avatar fixture.png");
        image.Save(file, PngBitmapEncoderOptions.Default);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var avatar = new Avatar { Text = "林默的头像" };
            var window = Open(new StackPanel { Children = { avatar } }, theme, size);
            var diameter = size == ControlSize.Small ? 32 : size == ControlSize.Large ? 56 : 40;
            check(avatar.Bounds.Size == new Size(diameter, diameter) && avatar.DisplayText == "林默",
                $"Avatar/{theme.Key}/{size}: inherited density and Unicode fallback text");
            check(avatar.GetVisualDescendants().OfType<TextBlock>().Single().IsVisible,
                "Avatar: text fallback is visible without an image");
            avatar.Text = "👩🏽‍💻张三";
            check(avatar.DisplayText == "👩🏽‍💻张", "Avatar: fallback truncates graphemes without splitting emoji");
            avatar.Text = " ";
            check(ControlAutomationPeer.CreatePeerForElement(avatar)!.GetName() == "Avatar",
                "Avatar: blank text retains an accessible image name");
            avatar.Text = null;
            Flush();
            check(avatar.GetVisualDescendants().OfType<PathIcon>().Single().IsVisible,
                "Avatar: empty text shows the person icon");
            avatar.Source = image;
            Flush();
            check(
                ReferenceEquals(avatar.CurrentImage, image) && avatar.HasImage && !avatar.IsLoading &&
                avatar.GetVisualDescendants().OfType<Image>().Single().IsVisible,
                "Avatar: caller-owned IImage renders through the native Image");
            foreach (var shape in Enum.GetValues<AvatarShape>())
            {
                avatar.Shape = shape;
                Flush();
                check(
                    avatar.EffectiveCornerRadius == (shape == AvatarShape.Circle ? new CornerRadius(999999) :
                        shape == AvatarShape.Rounded ? new CornerRadius(8) : default),
                    $"Avatar: {shape} updates template clipping");
            }

            AutomationProperties.SetName(avatar, "林默的头像");
            check(ControlAutomationPeer.CreatePeerForElement(avatar)!.GetName() == "林默的头像",
                "Avatar: consumer accessibility label is preserved");
            window.Close();
            Flush();
            check(avatar.CurrentImage is null && !avatar.IsLoading,
                "Avatar: detach releases the displayed image and pending load");
            using (image.Lock())
            {
            } // Caller retains ownership after detach.

            if (output is not null)
            {
                var demo = new AvatarDemoView();
                var gallery = Open(demo, theme, size, 720, 750);
                demo.ImageSource = image;
                Flush();
                check(ReferenceEquals(demo.Sample.CurrentImage, image),
                    "Avatar: compiled Source binding updates the demo consumer");
                demo.ImageSource = null;
                Wait(() => demo.GetVisualDescendants().OfType<Avatar>().All(item => !item.IsLoading));
                using var frame = gallery.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(directory, $"avatar-{theme.Key}-{size}.png"), PngBitmapEncoderOptions.Default);
                gallery.Close();
                Flush();
            }
        }

        VerifyPaths(check, file, image);
        VerifyPixels(check, image, directory);
        if (output is not null)
        {
            var narrow = Open(new AvatarDemoView(), ThemeVariant.Dark, ControlSize.Large, 360, 750);
            Wait(() => narrow.GetVisualDescendants().OfType<Avatar>().All(item => !item.IsLoading));
            using var frame = narrow.CaptureRenderedFrame()!;
            frame.Save(Path.Combine(directory, "avatar-Dark-Large-narrow.png"), PngBitmapEncoderOptions.Default);
            narrow.Close();
            Flush();
        }
    }

    private static void VerifyPaths(Action<bool, string> check, string file, WriteableBitmap callerImage)
    {
        var avatar = new Avatar { Text = "LM" };
        var window = Open(avatar, ThemeVariant.Light, ControlSize.Normal);
        foreach (var path in new object[]
                     { file, Path.GetRelativePath(Environment.CurrentDirectory, file), new Uri(file) })
        {
            avatar.Source = path;
            Wait(() => !avatar.IsLoading);
            check(avatar.HasImage && avatar.CurrentImage!.Size == new Size(64, 64) && avatar.ErrorMessage is null,
                "Avatar: absolute / relative / file URI paths load, including spaces");
        }

        avatar.Source = $"avares://{typeof(AvatarPreview).Assembly.GetName().Name}/Assets/preview-lake.png";
        Wait(() => !avatar.IsLoading);
        check(avatar.HasImage && avatar.ErrorMessage is null,
            "Avatar: packaged avares image resolves in the consumer assembly");
        avatar.Source = file + ".missing";
        Wait(() => !avatar.IsLoading);
        check(!avatar.HasImage && avatar.ErrorMessage is not null && avatar.DisplayText == "LM",
            "Avatar: missing local file keeps a usable fallback and reports the error");
        using var server = new ImageServer(File.ReadAllBytes(file));
        avatar.Source = server.Address + "ok";
        Wait(() => !avatar.IsLoading);
        check(avatar.HasImage && avatar.ErrorMessage is null, "Avatar: HTTP response is decoded into an image");
        foreach (var path in new[] { "missing", "bad" })
        {
            avatar.Source = server.Address + path;
            Wait(() => !avatar.IsLoading);
            check(!avatar.HasImage && avatar.ErrorMessage is not null,
                $"Avatar: HTTP {path} returns to fallback without throwing on the UI thread");
        }

        avatar.Source = server.Address + "slow";
        Wait(() => server.SlowStarted.Task.IsCompleted);
        check(avatar.IsLoading && !avatar.HasImage && avatar.DisplayText == "LM",
            "Avatar: fallback remains while loading");
        avatar.Source = callerImage;
        Wait(() => server.SlowFinished.Task.IsCompleted);
        check(ReferenceEquals(avatar.CurrentImage, callerImage) && avatar.ErrorMessage is null && !avatar.IsLoading,
            "Avatar: delayed response cannot overwrite a newer source");
        avatar.Source = file;
        Wait(() => !avatar.IsLoading);
        avatar.Reload();
        Wait(() => !avatar.IsLoading);
        check(avatar.HasImage, "Avatar: explicit reload replaces the owned image");
        avatar.Source = "ftp://example.test/avatar.png";
        Wait(() => !avatar.IsLoading);
        check(avatar.ErrorMessage is not null, "Avatar: unsupported URI scheme reports an error");
        avatar.Source = null;
        Flush();
        check(!avatar.HasImage && avatar.ErrorMessage is null, "Avatar: clearing the source clears image and errors");
        avatar.Source = server.Address + "slow-detach";
        window.Content = null;
        window.Close();
        Flush();
        check(!avatar.IsLoading && !avatar.HasImage, "Avatar: removing a pending request resets the visual state");
        window = Open(avatar, ThemeVariant.Dark, ControlSize.Normal);
        Wait(() => !avatar.IsLoading);
        check(avatar.HasImage, "Avatar: reattachment reloads the current source");
        window.Close();
        Flush();
    }

    private static void VerifyPixels(Action<bool, string> check, WriteableBitmap image, string output)
    {
        var avatar = new Avatar
        {
            Source = image, Width = 64, Height = 64, BorderThickness = default, Margin = new Thickness(20),
            HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top
        };
        var window = Open(new Border { Background = Brushes.White, Child = avatar }, ThemeVariant.Light,
            ControlSize.Normal, 120, 120);
        foreach (var shape in Enum.GetValues<AvatarShape>())
        {
            avatar.Shape = shape;
            Flush();
            using var frame = window.CaptureRenderedFrame()!;
            var point = avatar.TranslatePoint(default, window)!.Value;
            var corner = Pixel(frame, (int)((point.X + 1) * window.RenderScaling),
                (int)((point.Y + 1) * window.RenderScaling));
            var center = Pixel(frame, (int)((point.X + 32) * window.RenderScaling),
                (int)((point.Y + 32) * window.RenderScaling));
            check(shape == AvatarShape.Square ? corner.SequenceEqual(center) : !corner.SequenceEqual(center),
                $"Avatar: actual {shape} pixels clip the image corners");
            frame.Save(Path.Combine(output, $"avatar-pixels-{shape}.png"), PngBitmapEncoderOptions.Default);
        }

        window.Close();
        Flush();
    }

    private static byte[] Pixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[4];
        var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(x, y, 1, 1), handle.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            handle.Free();
        }

        return bytes;
    }

    private sealed class ImageServer : IDisposable
    {
        private readonly TcpListener _listener = new(IPAddress.Loopback, 0);
        private readonly Task _loop;
        private readonly byte[] _png;
        private readonly CancellationTokenSource _stop = new();

        public ImageServer(byte[] png)
        {
            _png = png;
            _listener.Start();
            Address = $"http://127.0.0.1:{((IPEndPoint)_listener.LocalEndpoint).Port}/";
            _loop = AcceptAsync();
        }

        public TaskCompletionSource SlowStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource SlowFinished { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public string Address { get; }

        public void Dispose()
        {
            _stop.Cancel();
            _listener.Stop();
            _loop.GetAwaiter().GetResult();
        }

        private async Task AcceptAsync()
        {
            try
            {
                while (!_stop.IsCancellationRequested)
                {
                    var client = await _listener.AcceptTcpClientAsync(_stop.Token).ConfigureAwait(false);
                    _ = ReplyAsync(client);
                }
            }
            catch (Exception error) when (error is OperationCanceledException or SocketException)
            {
            }
        }

        private async Task ReplyAsync(TcpClient client)
        {
            var slow = false;
            try
            {
                using (client)
                {
                    await using var stream = client.GetStream();
                    using var reader = new StreamReader(stream, Encoding.ASCII, leaveOpen: true);
                    var line = await reader.ReadLineAsync(_stop.Token) ?? "";
                    while (!string.IsNullOrEmpty(await reader.ReadLineAsync(_stop.Token)))
                    {
                    }

                    slow = line.Contains("slow");
                    if (slow)
                    {
                        SlowStarted.TrySetResult();
                        await Task.Delay(180, _stop.Token);
                    }

                    var body = line.Contains("bad") ? Encoding.UTF8.GetBytes("invalid image") : _png;
                    var header = Encoding.ASCII.GetBytes(
                        $"HTTP/1.1 {(line.Contains("missing") ? "404 Not Found" : "200 OK")}\r\nContent-Length: {body.Length}\r\nContent-Type: image/png\r\nConnection: close\r\n\r\n");
                    await stream.WriteAsync(header, _stop.Token);
                    await stream.WriteAsync(body, _stop.Token);
                }
            }
            catch (Exception error) when (error is IOException or OperationCanceledException or SocketException)
            {
            }
            finally
            {
                if (slow) SlowFinished.TrySetResult();
            }
        }
    }
}