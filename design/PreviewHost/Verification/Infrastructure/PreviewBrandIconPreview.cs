using System.Buffers.Binary;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class PreviewBrandIconPreview
{
    private static readonly int[] IconSizes = [16, 20, 24, 32, 40, 48, 64, 128, 256];

    public static void Export(string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        using (var bitmap = Render(512))
        {
            bitmap.Save(Path.Combine(outputDirectory, "theone-app.png"), PngBitmapEncoderOptions.Default);
        }

        var frames = IconSizes.Select(size => (Size: size, Bytes: Png(size))).ToArray();
        using (var stream = File.Create(Path.Combine(outputDirectory, "theone-app.ico")))
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write((ushort)0);
            writer.Write((ushort)1);
            writer.Write((ushort)frames.Length);
            var offset = 6 + 16 * frames.Length;
            foreach (var frame in frames)
            {
                writer.Write((byte)(frame.Size == 256 ? 0 : frame.Size));
                writer.Write((byte)(frame.Size == 256 ? 0 : frame.Size));
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((ushort)1);
                writer.Write((ushort)32);
                writer.Write(frame.Bytes.Length);
                writer.Write(offset);
                offset += frame.Bytes.Length;
            }

            foreach (var frame in frames) writer.Write(frame.Bytes);
        }

        var macFrames = new[]
            {
                ("icp4", 16), ("icp5", 32), ("icp6", 64), ("ic07", 128), ("ic08", 256), ("ic09", 512), ("ic10", 1024)
            }
            .Select(frame => (Type: frame.Item1, Bytes: Png(frame.Item2))).ToArray();
        using (var stream = File.Create(Path.Combine(outputDirectory, "theone-app.icns")))
        {
            stream.Write("icns"u8);
            WriteBigEndian(stream, 8 + macFrames.Sum(frame => 8 + frame.Bytes.Length));
            foreach (var frame in macFrames)
            {
                stream.Write(Encoding.ASCII.GetBytes(frame.Type));
                WriteBigEndian(stream, 8 + frame.Bytes.Length);
                stream.Write(frame.Bytes);
            }
        }

        Console.WriteLine(
            $"Exported SVG-matched PNG, {frames.Length}-size ICO and ICNS to {Path.GetFullPath(outputDirectory)}");
    }

    public static void Verify(Action<bool, string> check, string? output = null)
    {
        foreach (var size in new[] { 16, 32, 64, 512 })
        {
            using var bitmap = Render(size);
            var background = Sample(bitmap, .5, .12);
            var ring = Sample(bitmap, .26, .5);
            var one = Sample(bitmap, .54, .5);
            var corner = Sample(bitmap, 0, 0);
            // A 16px rounded corner has fractional anti-alias coverage in its first pixel.
            var readable = corner.A < 32 && background.G is > 85 and < 110 && background.R < 12
                           && ring.R > 230 && ring.G > 230 && one.G > 200 && one.R is > 170 and < 210;
            if (!readable)
                Console.WriteLine(
                    $"Brand/{size}: format={bitmap.Format}, corner={corner}, background={background}, ring={ring}, one={one}");
            check(readable,
                $"PreviewHost/brand/{size}: transparent rounded corners and distinct O / 1 remain readable");
            if (output is not null)
            {
                Directory.CreateDirectory(output);
                bitmap.Save(Path.Combine(output, $"theone-app-{size}.png"), PngBitmapEncoderOptions.Default);
            }
        }

        using (var stream = AssetLoader.Open(new Uri("avares://PreviewHost/Assets/Brand/theone-app.ico")))
        using (var reader = new BinaryReader(stream))
        {
            check(reader.ReadUInt16() == 0 && reader.ReadUInt16() == 1 && reader.ReadUInt16() == IconSizes.Length,
                "PreviewHost/brand: embedded ICO contains nine Windows icon sizes");
        }

        check(HasExecutableIcon(), "PreviewHost/brand: managed PE contains Windows icon and icon-group resources");
        foreach (var dark in new[] { false, true })
        {
            var window = new MainWindow
            {
                Width = 1100, Height = 760, WindowDecorations = WindowDecorations.None,
                RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light
            };
            Motion.SetEnabled(window, false);
            window.Show();
            ControlPreview.Flush();
            try
            {
                var mark = window.FindControl<PreviewBrandIcon>("BrandIcon")!;
                check(window.Icon is not null && mark.Bounds.Size == new Size(32, 32),
                    $"PreviewHost/brand/{(dark ? "dark" : "light")}: window icon and vector sidebar brand are installed");
                if (output is not null)
                {
                    using var frame = window.CaptureRenderedFrame();
                    frame?.Save(Path.Combine(output, $"previewhost-brand-{(dark ? "dark" : "light")}.png"),
                        PngBitmapEncoderOptions.Default);
                }

                window.Width = 700;
                ControlPreview.Flush();
                check(mark.IsEffectivelyVisible && !window.FindControl<TextBlock>("BrandWordmark")!.IsVisible,
                    $"PreviewHost/brand/{(dark ? "dark" : "light")}: compact sidebar keeps the mark without clipped lettering");
            }
            finally
            {
                window.Close();
                ControlPreview.Flush();
            }
        }
    }

    private static RenderTargetBitmap Render(int size)
    {
        var visual = new PreviewBrandIcon { Width = size, Height = size };
        visual.Measure(new Size(size, size));
        visual.Arrange(new Rect(0, 0, size, size));
        var bitmap = new RenderTargetBitmap(new PixelSize(size, size), new Vector(96, 96));
        bitmap.Render(visual);
        return bitmap;
    }

    private static byte[] Png(int size)
    {
        using var bitmap = Render(size);
        using var stream = new MemoryStream();
        bitmap.Save(stream, PngBitmapEncoderOptions.Default);
        return stream.ToArray();
    }

    private static Color Sample(Bitmap bitmap, double x, double y)
    {
        var bytes = new byte[4];
        var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(
                new PixelRect((int)(x * bitmap.PixelSize.Width), (int)(y * bitmap.PixelSize.Height), 1, 1),
                pin.AddrOfPinnedObject(), 4, 4);
        }
        finally
        {
            pin.Free();
        }

        return bitmap.Format == PixelFormat.Rgba8888
            ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2])
            : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }

    private static void WriteBigEndian(Stream stream, int value)
    {
        Span<byte> bytes = stackalloc byte[4];
        BinaryPrimitives.WriteInt32BigEndian(bytes, value);
        stream.Write(bytes);
    }

    private static bool HasExecutableIcon()
    {
        using var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "PreviewHost.dll"));
        using var reader = new PEReader(stream);
        var resource = reader.PEHeaders.PEHeader!.ResourceTableDirectory;
        if (resource.Size < 16) return false;
        var directory = reader.GetSectionData(resource.RelativeVirtualAddress).GetContent().ToArray();
        var count = BinaryPrimitives.ReadUInt16LittleEndian(directory.AsSpan(12))
                    + BinaryPrimitives.ReadUInt16LittleEndian(directory.AsSpan(14));
        var icon = false;
        var group = false;
        for (var index = 0; index < count; index++)
        {
            var id = BinaryPrimitives.ReadUInt32LittleEndian(directory.AsSpan(16 + index * 8));
            icon |= id == 3;
            group |= id == 14;
        }

        return icon && group;
    }
}