using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class DataGridFilterPreview
{
    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var (window, fixture, _) = Open(theme, size); var grid = fixture.Grid;
            DataGridFeatures.ApplyFilter(grid, grid.Columns[2], new TextFilterCondition("上海"));
            DataGridFeatures.ApplyFilter(grid, grid.Columns[3], new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 20)); Flush();
            Save(window, $"datagrid-filters-{theme.Key}-{size}.png");
            foreach (var (column, kind) in new[] { (grid.Columns[2], "text"), (grid.Columns[3], "range") })
            {
                var button = Entry(grid, column); Click(window, button); var popup = Popup(button);
                Save(popup, $"datagrid-filter-{kind}-{theme.Key}-{size}.png");
                button.Flyout!.Hide(); Flush();
            }
            window.Close(); Flush();
        }
        void Save(Control control, string name)
        {
            using var bitmap = new RenderTargetBitmap(new PixelSize((int)Math.Ceiling(control.Bounds.Width), (int)Math.Ceiling(control.Bounds.Height)), new Vector(96, 96));
            bitmap.Render(control); var path = Path.Combine(output, name); bitmap.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
        }
    }
}
