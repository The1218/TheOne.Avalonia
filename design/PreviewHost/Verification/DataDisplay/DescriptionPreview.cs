using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
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

internal static class DescriptionPreview
{
    internal static void Verify(Action<bool, string> check, string? output = null)
    {
        if (output is not null) Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 900, 360 })
            VerifyLayout(theme, size, width, check, output);
        VerifyInheritanceAndCollection(check);
        VerifyTemplates(check);
    }

    private static Description CreateSample()
    {
        return new Description
        {
            VerticalAlignment = VerticalAlignment.Top,
            Items =
            {
                new DescriptionItem { Header = "Classes / 样式", Content = "primary outline：主色描边，保留原生焦点、命令和键盘行为。" },
                new DescriptionItem
                {
                    Header = "公开属性 Public properties",
                    Content = "LabelWidth: double = 180. 标签和说明按行对齐；窄视口自动上下排列，长说明自然增高。"
                },
                new DescriptionItem
                {
                    Header = "API / Model",
                    Content =
                        "Header and Content accept text, controls or typed data templates. Long API descriptions wrap to the available width without clipping the following rows. 模型字段的类型、默认值与作用均可完整显示。"
                }
            }
        };
    }

    private static Window Open(Description description, double width = 900)
    {
        var window = new Window
        {
            Width = width, Height = 720, WindowDecorations = WindowDecorations.None,
            Content = new Border
            {
                Padding = new Thickness(20),
                Child = new ScrollViewer
                {
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    Content = description
                }
            }
        };
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    private static void VerifyLayout(ThemeVariant theme, ControlSize size, int width, Action<bool, string> check,
        string? output)
    {
        var description = CreateSample();
        var window = Open(description, width);
        try
        {
            window.RequestedThemeVariant = theme;
            ControlSizing.SetSize(window, size);
            Flush();
            var label = $"Description/{theme.Key}/{size}/{width}";
            var rows = description.Items.Cast<DescriptionItem>().ToArray();
            var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
            var padding = size switch
            {
                ControlSize.Small => new Thickness(10, 7), ControlSize.Large => new Thickness(14, 13),
                _ => new Thickness(12, 10)
            };
            var surface = Part<Border>(description, "PART_Surface");
            check(description.FontSize == font && rows.All(row => row.FontSize == font && row.Padding == padding),
                label + ": inherited density controls typography and cell padding");
            check(Colour(surface.Background) == Resource(description, "One.Surface") &&
                  Colour(surface.BorderBrush) == Resource(description, "One.Line") &&
                  Colour(description.Foreground) == Resource(description, "One.Text") && rows.All(row =>
                      Colour(Part<Border>(row, "PART_Label").Background) == Resource(description, "One.Subtle")),
                label + ": labels, values and border use the active semantic palette");
            check(double.IsNaN(description.Height) && double.IsPositiveInfinity(description.MaxHeight) &&
                  Math.Abs(description.Bounds.Height - description.DesiredSize.Height) < 1 &&
                  !description.GetVisualDescendants().OfType<ScrollViewer>().Any(),
                label + ": rows determine natural height without an internal scroll viewport");
            check(
                rows.All(row =>
                    row.Classes.Contains(":stacked") == width < 520 &&
                    Grid.GetRow(Part<Border>(row, "PART_Value")) == (width < 520 ? 1 : 0)),
                label + ": narrow rows stack labels while wide rows retain two columns");
            var longText = Part<ContentPresenter>(rows[2], "PART_ContentPresenter").GetVisualDescendants()
                .OfType<TextBlock>().Single();
            check(
                longText.TextLayout.TextLines.Count > 1 &&
                Math.Abs(longText.Bounds.Height - longText.DesiredSize.Height) < 1 && longText.FontSize == font,
                label + ": long bilingual API text wraps and retains its full measured height");
            check(rows.All(row =>
                      Fits(Part<Border>(row, "PART_Label"), row) && Fits(Part<Border>(row, "PART_Value"), row)) &&
                  rows.Zip(rows.Skip(1)).All(pair => pair.First.Bounds.Bottom <= pair.Second.Bounds.Top + 0.1),
                label + ": every cell fits its row and subsequent rows remain below previous content");
            using var bitmap = new RenderTargetBitmap(
                new PixelSize((int)Math.Ceiling(description.Bounds.Width),
                    (int)Math.Ceiling(description.Bounds.Height)), new Vector(96, 96));
            bitmap.Render(description);
            var labelPoint = Part<Border>(rows[0], "PART_Label").TranslatePoint(new Point(10, 4), description)!.Value;
            check(
                PreviewRenderedPixels.UniqueColors(bitmap) > 24 &&
                Pixel(bitmap, (int)labelPoint.X, (int)labelPoint.Y) == Resource(description, "One.Subtle"),
                label + ": real text and themed label pixels are drawn");
            if (output is not null)
                bitmap.Save(Path.Combine(output, $"description-{theme.Key}-{size}-{width}.png"),
                    PngBitmapEncoderOptions.Default);
            description.Classes.Add("borderless");
            Flush();
            check(description.BorderThickness == default && surface.BorderThickness == default &&
                  description.CornerRadius == default &&
                  Part<Border>(rows[1], "PART_Row").BorderThickness.Top == 1,
                label + ": borderless removes outer chrome while retaining row separators");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyInheritanceAndCollection(Action<bool, string> check)
    {
        var description = CreateSample();
        var window = Open(description);
        try
        {
            var first = (DescriptionItem)description.Items[0]!;
            var second = (DescriptionItem)description.Items[1]!;
            description.LabelWidth = 144;
            description.StackWidth = 400;
            Flush();
            check(
                first.LabelWidth == 144 && second.LabelWidth == 144 && first.StackWidth == 400 &&
                Part<Border>(first, "PART_Label").Bounds.Width == 144,
                "Description/inheritance: parent label width and threshold reach existing rows");
            first.LabelWidth = 210;
            first.StackWidth = 1000;
            Flush();
            check(
                first.LabelWidth == 210 && first.Classes.Contains(":stacked") && second.LabelWidth == 144 &&
                !second.Classes.Contains(":stacked"),
                "Description/inheritance: one row overrides inherited layout without changing siblings");
            first.ClearValue(DescriptionItem.LabelWidthProperty);
            first.ClearValue(DescriptionItem.StackWidthProperty);
            Flush();
            check(first.LabelWidth == 144 && !first.Classes.Contains(":stacked"),
                "Description/inheritance: clearing a row override restores inherited values");
            description.StackWidth = 1000;
            Flush();
            check(description.Items.Cast<DescriptionItem>().All(row => row.Classes.Contains(":stacked")),
                "Description/threshold: raising threshold updates all measured rows");
            description.StackWidth = 0;
            window.Width = 360;
            Flush();
            check(description.Items.Cast<DescriptionItem>().All(row => !row.Classes.Contains(":stacked")),
                "Description/threshold: zero explicitly keeps two-column layout at narrow width");
            description.Items.RemoveAt(0);
            Flush();
            check(second.Classes.Contains(":first") && Part<Border>(second, "PART_Row").BorderThickness.Top == 0,
                "Description/collection: removing the first row removes the new first row's leading separator");
            description.Items.Insert(0, first);
            Flush();
            check(
                first.Classes.Contains(":first") && !second.Classes.Contains(":first") &&
                Part<Border>(second, "PART_Row").BorderThickness.Top == 1,
                "Description/collection: reinserting a leading row updates existing row separators");
            var rejected = false;
            try
            {
                description.LabelWidth = double.NaN;
            }
            catch (ArgumentException)
            {
                rejected = true;
            }

            check(rejected && description.LabelWidth == 144,
                "Description/validation: invalid widths are rejected without replacing the current value");
        }
        finally
        {
            window.Close();
        }
    }

    private static void VerifyTemplates(Action<bool, string> check)
    {
        var model = new DescriptionNote("类型 Type", "内容由明确类型的数据模板展示。Typed templates retain native DataContext.");
        var row = new DescriptionItem { Header = model, Content = model };
        row.HeaderTemplate = new FuncDataTemplate<DescriptionNote>((_, _) =>
        {
            var text = new TextBlock { Name = "DescriptionTypedHeader", TextWrapping = TextWrapping.Wrap };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<DescriptionNote, string>(value => value.Name));
            return text;
        });
        row.ContentTemplate = new FuncDataTemplate<DescriptionNote>((_, _) =>
        {
            var text = new TextBlock { Name = "DescriptionTypedContent", TextWrapping = TextWrapping.Wrap };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<DescriptionNote, string>(value => value.Details));
            return text;
        });
        var description = new Description { Items = { row } };
        var window = Open(description, 360);
        try
        {
            var header = row.GetVisualDescendants().OfType<TextBlock>()
                .Single(control => control.Name == "DescriptionTypedHeader");
            var content = row.GetVisualDescendants().OfType<TextBlock>()
                .Single(control => control.Name == "DescriptionTypedContent");
            check(
                header.Text == model.Name && content.Text == model.Details &&
                ReferenceEquals(header.DataContext, model) && ReferenceEquals(content.DataContext, model),
                "Description/templates: both native presenters supply the typed model to compiled bindings");
            var replacement = new DescriptionNote("替换的名称", "Replacement content");
            row.Header = replacement;
            row.Content = replacement;
            Flush();
            header = Part<ContentPresenter>(row, "PART_HeaderPresenter").GetVisualDescendants().OfType<TextBlock>()
                .Single(control => control.Name == "DescriptionTypedHeader");
            content = Part<ContentPresenter>(row, "PART_ContentPresenter").GetVisualDescendants().OfType<TextBlock>()
                .Single(control => control.Name == "DescriptionTypedContent");
            check(
                header.Text == replacement.Name && content.Text == replacement.Details &&
                ReferenceEquals(header.DataContext, replacement) && ReferenceEquals(content.DataContext, replacement),
                "Description/templates: replacing models displays both new values and typed contexts through native template materialization");
            var editor = new TextBox { Text = "caller-owned editor" };
            row.HeaderTemplate = null;
            row.ContentTemplate = null;
            row.Header = "编辑器";
            row.Content = editor;
            Flush();
            check(
                ReferenceEquals(Part<ContentPresenter>(row, "PART_ContentPresenter").Content, editor) &&
                editor.GetVisualAncestors().Contains(row),
                "Description/content: caller-owned controls remain in the row's native content presenter");
        }
        finally
        {
            window.Close();
        }
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>().Single(control =>
            control.Name == name && ReferenceEquals(control.TemplatedParent, owner));
    }

    private static bool Fits(Control child, Control parent)
    {
        var origin = child.TranslatePoint(default, parent)!.Value;
        return origin.X >= -0.1 && origin.Y >= -0.1 && origin.X + child.Bounds.Width <= parent.Bounds.Width + 0.1 &&
               origin.Y + child.Bounds.Height <= parent.Bounds.Height + 0.1;
    }

    private static Color? Colour(IBrush? brush)
    {
        return (brush as ISolidColorBrush)?.Color;
    }

    private static Color? Resource(Control control, string key)
    {
        return control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    }

    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[bitmap.PixelSize.Width * bitmap.PixelSize.Height * 4];
        var memory = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try
        {
            bitmap.CopyPixels(new PixelRect(bitmap.PixelSize), memory.AddrOfPinnedObject(), bytes.Length,
                bitmap.PixelSize.Width * 4);
        }
        finally
        {
            memory.Free();
        }

        var offset = (y * bitmap.PixelSize.Width + x) * 4;
        return bitmap.Format == PixelFormat.Bgra8888
            ? Color.FromArgb(bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset])
            : Color.FromArgb(bytes[offset + 3], bytes[offset], bytes[offset + 1], bytes[offset + 2]);
    }

    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed record DescriptionNote(string Name, string Details);
}