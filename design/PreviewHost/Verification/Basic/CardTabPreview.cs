using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;
using static TheOne.DesignPreview.EnhancementsPreview;

namespace TheOne.DesignPreview;

internal static class CardTabPreview
{
    private static readonly string[] Titles = ["Kestrel Sport Sedan", "Meridian GT", "Longhaul Estate", "Trailcut 200", "Velocia Coupe", "Regent Executive", "Nimbus Crossover", "Foreman Crew Cab", "Dune Runner", "Cityhop Compact", "Auralis Grand Tourer", "Trailhead Wagon"];

    public static void Verify(LegacyPreviewWindow window, LegacyControlGallery gallery)
    {
        Click(window, gallery.FindControl<Button>("ReferenceTabsButton")!);
        var tabs = gallery.FindControl<TabControl>("CardTabs")!;
        Check(window.Model.DemoTabs.Count == 12 && window.Model.DemoTabs[0].Title == Titles[0], "Long-tab preview button loads the supplied reference header example");
        var scroll = Part<ScrollViewer>(tabs, "PART_HeaderScroll");
        var strip = Part<CardTabStrip>(tabs, "CardStrip");
        Check(strip.HasOverflow && Part<Button>(tabs, "PART_HeaderForward").IsVisible && scroll.HorizontalScrollBarVisibility == ScrollBarVisibility.Hidden, "Overflow uses a trailing header arrow without adding a scrollbar row");
        var index = tabs.SelectedIndex;
        Click(window, Part<Button>(tabs, "PART_HeaderForward"));
        var afterForward = scroll.Offset.X;
        Check(afterForward > 0 && strip.CanScrollBackward && tabs.SelectedIndex == index, "Header forward arrow scrolls while preserving selected content");
        Click(window, Part<Button>(tabs, "PART_HeaderBack"));
        Check(scroll.Offset.X < afterForward, "Header backward arrow returns through overflowing tabs");
        tabs.SelectedIndex = 11; ControlPreview.Flush();
        var last = (TabItem)tabs.ContainerFromIndex(11)!;
        var position = last.TranslatePoint(default, scroll)!.Value;
        Check(position.X >= -0.5 && position.X + last.Bounds.Width <= scroll.Bounds.Width + 0.5, $"Programmatic selection brings the entire last tab and close icon into view (x {position.X}, item {last.Bounds.Width}, view {scroll.Bounds.Width}, offset {scroll.Offset.X})");
        window.Model.ResetTabsCommand.Execute(null); tabs.SelectedIndex = 1; ControlPreview.Flush();

        foreach (var variant in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyAppearance(variant, size);
        VerifyPlacements();
    }

    private static TabControl CreateTabs() => new() { Classes = { "card" }, SelectedIndex = 0 };
    private static void AddTabs(TabControl tabs)
    {
        foreach (var title in Titles) tabs.Items.Add(new TabItem { Header = title, Content = title });
    }
    private static void VerifyAppearance(ThemeVariant variant, ControlSize size)
    {
        var tabs = CreateTabs(); AddTabs(tabs);
        var window = new Window { Width = 780, Height = 300, Content = tabs, RequestedThemeVariant = variant, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); ControlSizing.SetSize(window, size); window.Show(); ControlPreview.Flush();
        var selected = (TabItem)tabs.ContainerFromIndex(0)!;
        var chrome = Part<CardTabChrome>(selected, "PART_LayoutRoot");
        var frame = Part<Border>(tabs, "CardContentFrame");
        var header = Part<Border>(tabs, "CardHeaderBackground");
        var label = $"Card/{variant.Key}/{size}";
        var height = size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 };
        Check(Math.Abs(selected.Bounds.Height - height) < 0.5 && header.Bounds.Height == selected.Bounds.Height, $"{label}: header strip retains its density with no extra inset (item {selected.Bounds.Height}, strip {header.Bounds.Height})");
        Check(chrome.IsSelected && Colour(chrome.Background) == Colour(frame.Background) && Colour(chrome.BorderBrush) == Colour(frame.BorderBrush), $"{label}: selected outline uses the content surface and frame border");
        var others = tabs.GetRealizedContainers().OfType<TabItem>().Skip(1).ToArray();
        Check(others.All(item => item.Margin == default && Colour(Part<CardTabChrome>(item, "PART_LayoutRoot").Background)?.A == 0), $"{label}: inactive headers are flat and contiguous");
        Check(!Part<Border>(selected, "PART_Separator").IsVisible && Part<Border>(others[0], "PART_Separator").IsVisible && Part<Border>(others[^1], "PART_Separator").IsVisible == (tabs.IndexFromContainer(others[^1]) < tabs.ItemCount - 1), $"{label}: separators appear only between ordinary inactive neighbours");
        Check(tabs.GetRealizedContainers().OfType<TabItem>().Count(item => Part<Button>(item, "PART_CloseButton").IsVisible) == 1, $"{label}: only the selected header exposes a close button");
        var seam = frame.TranslatePoint(default, window)!.Value.Y;
        var selectedOrigin = chrome.TranslatePoint(default, window)!.Value;
        Check(Math.Abs(selectedOrigin.Y + chrome.Bounds.Height - seam - 1) < 0.5, $"{label}: selected tab overlaps the content border by one pixel");
        using (var bitmap = new RenderTargetBitmap(new PixelSize(780, 300), new Vector(96, 96)))
        {
            bitmap.Render(window);
            var x = (int)(selectedOrigin.X + chrome.Bounds.Width / 2);
            var y = (int)Math.Round(seam);
            Check(Pixel(bitmap, x, y) == Colour(frame.Background), $"{label}: rendered selected seam has no divider line");
            var surface = Part<Border>(tabs, "HeaderSurface");
            var top = surface.TranslatePoint(default, window)!.Value;
            var line = Colour(frame.BorderBrush);
            Check(Near(Pixel(bitmap, 0, y), line) && Near(Pixel(bitmap, 779, y), line) && Pixel(bitmap, 0, (int)(top.Y + surface.Bounds.Height / 2)) == line && Pixel(bitmap, 779, (int)(top.Y + surface.Bounds.Height / 2)) == line, $"{label}: header and content side borders meet without a missing corner or gap");
            Check(Pixel(bitmap, 400, (int)top.Y) == line, $"{label}: gray header has a complete outer top border");
            var inactive = Part<CardTabChrome>(others[0], "PART_LayoutRoot");
            var inactiveOrigin = inactive.TranslatePoint(default, window)!.Value;
            Check(Pixel(bitmap, (int)(inactiveOrigin.X + inactive.Bounds.Width / 2), y) == Colour(frame.BorderBrush), $"{label}: inactive seam retains the continuous frame line");
        }
        tabs.SelectedIndex = 2; ControlPreview.Flush();
        Check(!Part<Border>((TabItem)tabs.ContainerFromIndex(1)!, "PART_Separator").IsVisible && !Part<Border>((TabItem)tabs.ContainerFromIndex(2)!, "PART_Separator").IsVisible && Part<Border>((TabItem)tabs.ContainerFromIndex(3)!, "PART_Separator").IsVisible, $"{label}: selection updates adjacent separator suppression");
        TabFeatures.Move(tabs, 2, 1); ControlPreview.Flush();
        Check(tabs.SelectedIndex == 1 && !Part<Border>((TabItem)tabs.ContainerFromIndex(0)!, "PART_Separator").IsVisible, $"{label}: reorder updates neighbours around the selected tab");
        var closing = (TabItem)tabs.ContainerFromIndex(1)!;
        TabFeatures.Close(closing); ControlPreview.Flush();
        Check(tabs.SelectedIndex == 1 && tabs.Items.Count == 11 && !Part<Border>((TabItem)tabs.ContainerFromIndex(0)!, "PART_Separator").IsVisible, $"{label}: close restores the adjacent selected outline");
        tabs.Classes.Remove("card"); ControlPreview.Flush();
        Check(tabs.GetRealizedContainers().OfType<TabItem>().All(item => !item.Classes.Contains("card-tab") && !item.Classes.Contains("before-selected") && !item.Classes.Contains("last-tab")), $"{label}: switching back to standard tabs clears card-specific state");
        window.Close();
    }

    private static void VerifyPlacements()
    {
        foreach (var placement in new[] { Dock.Top, Dock.Bottom, Dock.Left, Dock.Right })
        {
            var tabs = CreateTabs(); AddTabs(tabs); tabs.TabStripPlacement = placement;
            var window = new Window { Width = 780, Height = 300, Content = tabs, WindowDecorations = WindowDecorations.None };
            Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
            var vertical = placement is Dock.Left or Dock.Right;
            var scroll = Part<ScrollViewer>(tabs, "PART_HeaderScroll");
            Check(Part<CardTabChrome>((TabItem)tabs.ContainerFromIndex(0)!, "PART_LayoutRoot").Placement == placement && Part<CardTabStrip>(tabs, "CardStrip").HasOverflow, $"Card/{placement}: placement rotates the joined outline and retains overflow");
            Click(window, Part<Button>(tabs, "PART_HeaderForward"));
            Check(vertical ? scroll.Offset.Y > 0 && scroll.Offset.X == 0 : scroll.Offset.X > 0 && scroll.Offset.Y == 0, $"Card/{placement}: arrow scrolls on the correct axis");
            window.Close();
        }
    }

    public static void Render(string output, bool dark)
    {
        var tabs = CreateTabs();
        foreach (var title in Titles)
        {
            var form = new Grid { ColumnDefinitions = new ColumnDefinitions("180,*"), RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto"), RowSpacing = 12, ColumnSpacing = 16 };
            var labels = new[] { "Trademark", "Transmission Type", "Description", "In Stock" };
            Control[] fields = [new TextBox { Text = title }, new TextBox { Text = "Automatic" }, new TextBox { Text = "Selected tab joins the content surface. Drag headers to reorder; close the active tab with ×; use the header arrows for overflow.", TextWrapping = TextWrapping.Wrap }, new CheckBox { IsChecked = true }];
            for (var i = 0; i < fields.Length; i++)
            {
                var caption = new TextBlock { Text = labels[i], VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center };
                Grid.SetRow(caption, i); Grid.SetRow(fields[i], i); Grid.SetColumn(fields[i], 1);
                form.Children.Add(caption); form.Children.Add(fields[i]);
            }
            tabs.Items.Add(new TabItem { Header = title, Content = form });
        }
        var window = new Window { Width = 1280, Height = 400, Content = new Border { Padding = new Thickness(12), Child = tabs }, RequestedThemeVariant = dark ? ThemeVariant.Dark : ThemeVariant.Light, WindowDecorations = WindowDecorations.None };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        var theme = dark ? "dark" : "light";
        Save(window, Path.Combine(output, $"card-tabs-{theme}.png"));
        tabs.SelectedIndex = 2; ControlPreview.Flush(); Save(window, Path.Combine(output, $"card-tabs-{theme}-middle.png"));
        tabs.SelectedIndex = 11; ControlPreview.Flush(); Save(window, Path.Combine(output, $"card-tabs-{theme}-overflow.png"));
        window.Close();
    }

    private static bool Near(Color actual, Color? expected) => expected is { } colour && Math.Abs(actual.R - colour.R) <= 16 && Math.Abs(actual.G - colour.G) <= 16 && Math.Abs(actual.B - colour.B) <= 16;
    private static Color Pixel(RenderTargetBitmap bitmap, int x, int y)
    {
        var bytes = new byte[4]; var pinned = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pinned.AddrOfPinnedObject(), 4, 4); }
        finally { pinned.Free(); }
        if (bitmap.Format == PixelFormat.Rgba8888) return Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]);
        if (bitmap.Format == PixelFormat.Bgra8888) return Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
        throw new NotSupportedException($"Unsupported render format: {bitmap.Format}");
    }
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static T Part<T>(Control owner, string name) where T : Control => owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name && ReferenceEquals(control.TemplatedParent, owner));
    private static void Click(TopLevel window, Control control)
    {
        control.BringIntoView(); ControlPreview.Flush();
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); ControlPreview.Flush();
    }
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException($"Failed: {message}");
        Console.WriteLine($"PASS: {message}");
    }
}
