using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class ImagePreviewDemoViewModel : ReactiveObject
{
    public Bitmap Sample { get; } = LoadSample();
    public WriteableBitmap TransparentSample { get; } = CreateTransparentSample();
    public ObservableCollection<object> Images { get; } = [];
    public object? SingleSource { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public int SelectedIndex { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public string NetworkAddress { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public string LocalPath { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public string SourceDescription { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "Bitmap · 内置示例";
    public ImagePreviewDemoViewModel() { SingleSource = Sample; Images.Add(Sample); Images.Add(TransparentSample); }
    public void UseBitmap() { SingleSource = Sample; SourceDescription = "Bitmap · 内置示例"; }
    public void UseNetwork() { SingleSource = string.IsNullOrWhiteSpace(NetworkAddress) ? null : NetworkAddress.Trim(); SourceDescription = "网络地址 · HTTP(S)"; }
    public void UseLocal() { SingleSource = string.IsNullOrWhiteSpace(LocalPath) ? null : LocalPath.Trim(); SourceDescription = "本地路径"; }
    public void AddCurrent() { if (SingleSource is { } source) { Images.Add(source); SelectedIndex = Images.Count - 1; } }
    private static Bitmap LoadSample()
    {
        using var stream = AssetLoader.Open(new Uri($"avares://{typeof(ImagePreviewDemoViewModel).Assembly.GetName().Name}/Assets/preview-lake.png"));
        return new Bitmap(stream);
    }
    public static WriteableBitmap CreateTransparentSample()
    {
        var bitmap = new WriteableBitmap(new PixelSize(640, 400), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Unpremul);
        using var buffer = bitmap.Lock();
        var bytes = new byte[buffer.RowBytes * 400];
        for (var y = 40; y < 360; y++)
        for (var x = 60; x < 580; x++)
        {
            var p = y * buffer.RowBytes + x * 4;
            bytes[p] = 36; bytes[p + 1] = 176; bytes[p + 2] = 180;
            bytes[p + 3] = (byte)(x < 320 ? 255 : 100);
        }
        Marshal.Copy(bytes, 0, buffer.Address, bytes.Length); return bitmap;
    }
}
