using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridSummaryPreview
{
    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, fixture, _, _) = Open(theme, size);
            var grid = fixture.Grid;
            Save("source");
            DataGridFeatures.ApplyFilter(grid, grid.Columns[3],
                new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 10));
            Flush();
            grid.ScrollIntoView(fixture.Records.First(row => row.Quantity == 5), grid.Columns[4]);
            Flush();
            Save("filtered");
            var header = grid.GetVisualDescendants().OfType<DataGridColumnHeader>()
                .Single(h => Equals(h.Content, grid.Columns[7].Header));
            var start = header.TranslatePoint(new Point(30, 14), window)!.Value;
            window.MouseMove(start);
            window.MouseDown(start, MouseButton.Left);
            window.MouseMove(start - new Vector(24, 0));
            Flush();
            Save("drag");
            window.MouseUp(new Point(-10, -10), MouseButton.Left);
            Flush();
            window.Width = 320;
            Flush();
            Save("narrow");
            window.Close();
            Flush();

            void Save(string state)
            {
                using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(grid.Bounds.Width),
                    (int)Math.Ceiling(grid.Bounds.Height)));
                bitmap.Render(grid);
                var path = Path.Combine(output, $"datagrid-summary-{theme.Key}-{size}-{state}.png");
                bitmap.Save(path, PngBitmapEncoderOptions.Default);
                Console.WriteLine($"Rendered {path}");
            }
        }
    }
}