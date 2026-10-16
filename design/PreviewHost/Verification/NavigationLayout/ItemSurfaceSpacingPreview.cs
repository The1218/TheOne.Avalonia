using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ItemSurfaceSpacingPreview
{
    public static void VerifyTabs(Action<bool, string> check, string? output, bool strip)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 })
        foreach (var placement in strip ? new[] { Dock.Top, Dock.Left } : new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right })
        {
            var vertical = placement is Dock.Left or Dock.Right;
            SelectingItemsControl control;
            if (strip)
            {
                var tabs = new TabStrip { ItemsSource = new[] { "概览", "成员", "停用" }, SelectedIndex = 0, VerticalAlignment = VerticalAlignment.Top };
                if (vertical)
                {
                    tabs.Height = 200;
                    tabs.ItemsPanel = new FuncTemplate<Panel?>(() => new VirtualizingStackPanel { Orientation = Orientation.Vertical });
                    ScrollViewer.SetHorizontalScrollBarVisibility(tabs, ScrollBarVisibility.Disabled);
                    ScrollViewer.SetVerticalScrollBarVisibility(tabs, ScrollBarVisibility.Auto);
                }
                control = tabs;
            }
            else control = new TabControl { Height = 220, VerticalAlignment = VerticalAlignment.Top, TabStripPlacement = placement, SelectedIndex = 0,
                Items = { new TabItem { Header = "概览", Content = "内容一" }, new TabItem { Header = "成员", Content = "内容二" }, new TabItem { Header = "停用", IsEnabled = false } } };
            var window = Open(control, theme, size, width);
            var label = $"{(strip ? "TabStrip" : "TabItem")}/spacing/{theme.Key}/{size}/{width}/{placement}";
            try
            {
                var items = control.GetRealizedContainers().OfType<TemplatedControl>().ToArray();
                if (strip) items[2].IsEnabled = false;
                var scroll = OwnPart<ScrollViewer>(control, strip ? "PART_ScrollViewer" : "PART_StandardHeaderScroll");
                var rows = items.Select(item => OwnPart<Border>(item, "PART_LayoutRoot")).ToArray();
                check(rows.Zip(rows.Skip(1)).All(pair => Gap(pair.First, pair.Second, scroll, vertical) >= 4), label + ": adjacent header surfaces have four-pixel gaps");
                check(rows.All(row => Inset(row, scroll, vertical, 4)), label + ": selected and hover surfaces are inset from the header edges");
                var origin = rows[0].TranslatePoint(default, scroll)!.Value;
                using var bitmap = Render(scroll);
                var x = (int)origin.X + 2; var y = (int)(origin.Y + rows[0].Bounds.Height / 2);
                check(Pixel(bitmap, x, y) == Resource(control, "One.AccentSoft"), label + ": actual selected pixels use the accent surface");
                if (strip)
                {
                    using var frame = Render(control); Save(frame, output, label);
                }
                else Save(bitmap, output, label);
                Click(rows[1]); check(control.SelectedIndex == 1, label + ": native pointer selects the second header");
                Click(rows[2]); check(control.SelectedIndex == 1, label + ": disabled headers remain unselectable");
                items[1].Focus(); Press(window, vertical ? Key.Up : Key.Left);
                check(control.SelectedIndex == 0, label + ": native arrow navigation remains available");
                var secondOrigin = rows[1].TranslatePoint(default, scroll)!.Value;
                var gapPoint = vertical ? new Point(secondOrigin.X + rows[1].Bounds.Width / 2, secondOrigin.Y - 2)
                    : new Point(secondOrigin.X - 2, secondOrigin.Y + rows[1].Bounds.Height / 2);
                using var gapBitmap = Render(strip ? control : scroll);
                if (strip) gapPoint = scroll.TranslatePoint(gapPoint, control)!.Value;
                check(Pixel(gapBitmap, (int)gapPoint.X, (int)gapPoint.Y) == Resource(control, strip ? "One.Surface" : "One.CardTabStrip"), label + ": actual gap pixels retain the header background");
            }
            finally { window.Close(); Flush(); }
        }
    }
    private static Window Open(Control content, ThemeVariant theme, ControlSize size, int width)
    {
        var host = new Border { Padding = new Thickness(16), Child = content };
        host.Bind(Border.BackgroundProperty, new global::Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("One.Surface"));
        var window = new Window { Width = width, Height = 430, RequestedThemeVariant = theme, WindowDecorations = WindowDecorations.None, Content = host };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static T OwnPart<T>(TemplatedControl owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().First(c => c.Name == name && ReferenceEquals(c.TemplatedParent, owner));
    private static double Gap(Control first, Control second, Visual host, bool vertical)
    {
        var end = first.TranslatePoint(new Point(first.Bounds.Width, first.Bounds.Height), host)!.Value;
        var start = second.TranslatePoint(default, host)!.Value; return vertical ? start.Y - end.Y : start.X - end.X;
    }
    private static bool Inset(Control row, Control host, bool vertical, double minimum)
    {
        var point = row.TranslatePoint(default, host)!.Value;
        return point.X >= minimum && point.Y >= minimum && (vertical ? host.Bounds.Width - point.X - row.Bounds.Width >= minimum : host.Bounds.Height - point.Y - row.Bounds.Height >= minimum);
    }
    private static Color Resource(Control control, string key)
    { control.TryFindResource(key, control.ActualThemeVariant, out var value); return ((ISolidColorBrush)value!).Color; }
    private static void Click(Control control)
    {
        var root = TopLevel.GetTopLevel(control)!; var hit = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), root)!.Value;
        root.MouseMove(hit); root.MouseDown(hit, MouseButton.Left); root.MouseUp(hit, MouseButton.Left); Flush();
    }
    private static void Press(TopLevel root, Key key)
    { root.KeyPress(key, RawInputModifiers.None, PhysicalKey.None, null); root.KeyRelease(key, RawInputModifiers.None, PhysicalKey.None, null); Flush(); }
    private static RenderTargetBitmap Render(Control control)
    {
        var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)), new Vector(96, 96));
        bitmap.Render(control); return bitmap;
    }
    private static void Save(Bitmap bitmap, string? output, string label)
    { if (output is null) return; Directory.CreateDirectory(output); bitmap.Save(Path.Combine(output, label.Replace('/', '-') + ".png"), PngBitmapEncoderOptions.Default); }
    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var pixels = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4]; var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), handle.AddrOfPinnedObject(), pixels.Length, bitmap.PixelSize.Width * 4); }
        finally { handle.Free(); }
        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Bgra8888 ? Color.FromArgb(pixels[offset + 3], pixels[offset + 2], pixels[offset + 1], pixels[offset]) : Color.FromArgb(pixels[offset + 3], pixels[offset], pixels[offset + 1], pixels[offset + 2]);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
