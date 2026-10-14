using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridRightFrozenPreview
{
    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var fixture = new DataGridStage1Fixture();
            DataGridFeatures.SetFrozenRightColumnCount(fixture.Grid, 1);
            var window = new Window { Width = 640, Height = 360, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme, Content = fixture.Grid };
            ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush();
            Save("initial");
            fixture.Grid.ScrollIntoView(fixture.Records[0], fixture.Grid.Columns[5]); Flush(); Save("scrolled");
            window.Width = 220; Flush(); Save("narrow");
            window.Close(); Flush();
            void Save(string state)
            {
                var grid = fixture.Grid;
                using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(grid.Bounds.Width), (int)Math.Ceiling(grid.Bounds.Height)));
                bitmap.Render(grid);
                var path = Path.Combine(output, $"datagrid-right-{theme.Key}-{size}-{state}.png");
                bitmap.Save(path, PngBitmapEncoderOptions.Default);
                Console.WriteLine($"Rendered {path}");
            }
        }
    }
}
