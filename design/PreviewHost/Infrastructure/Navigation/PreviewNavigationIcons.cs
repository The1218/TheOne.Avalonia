using Avalonia.Media;
using Material.Icons;

namespace TheOne.DesignPreview;

/// <summary>Semantic Material silhouettes cached as geometry; navigation models remain framework-neutral.</summary>
internal static class PreviewNavigationIcons
{
    private static readonly Dictionary<MaterialIconKind, Geometry> Cache = new();

    public static Geometry Group(string name)
    {
        return Get(name switch
        {
            "开始使用" => MaterialIconKind.HomeOutline,
            "基础控件" => MaterialIconKind.ViewGridOutline,
            "数据展示" => MaterialIconKind.TableLarge,
            "导航与布局" => MaterialIconKind.DockLeft,
            "图表控件" => MaterialIconKind.ChartLine,
            "扩展控件" => MaterialIconKind.PuzzleOutline,
            "消息与对话框" => MaterialIconKind.MessageTextOutline,
            _ => MaterialIconKind.ViewListOutline
        });
    }

    public static Geometry Page(int id)
    {
        return Get(id switch
        {
            0 => MaterialIconKind.HomeOutline,
            400 or 1 or 305 or 101 or 16 or 12 or 130 => MaterialIconKind.GestureTapButton,
            401 or 2 or 110 or 111 => MaterialIconKind.FormTextbox,
            402 or 3 or 15 or 106 => MaterialIconKind.ToggleSwitchOutline,
            403 or 4 or 108 or 109 => MaterialIconKind.TuneVariant,
            404 or 5 or 113 or 114 or 115 => MaterialIconKind.CalendarMonthOutline,
            405 or 118 or 9 or 7 => MaterialIconKind.ViewListOutline,
            406 or 6 or 17 or 20 => MaterialIconKind.TabUnselected,
            407 or 122 or 123 or 128 or 14 => MaterialIconKind.MenuOpen,
            408 or 125 or 126 or 127 or 132 => MaterialIconKind.ArrowSplitVertical,
            409 or 23 or 201 or 204 => MaterialIconKind.ChartLine,
            410 or 202 or 203 => MaterialIconKind.ChartBar,
            411 or 208 or 209 => MaterialIconKind.ChartBarStacked,
            412 or 210 or 211 or 212 or 213 => MaterialIconKind.ChartAreaspline,
            413 or 205 or 206 => MaterialIconKind.ChartPie,
            414 or 207 => MaterialIconKind.Gauge,
            8 => MaterialIconKind.UnfoldMoreHorizontal,
            11 or 328 => MaterialIconKind.ViewColumnOutline,
            10 or 313 => MaterialIconKind.TableHeadersEye,
            18 => MaterialIconKind.ViewCarouselOutline,
            310 => MaterialIconKind.ImageOutline,
            315 => MaterialIconKind.FormatListNumbered,
            317 => MaterialIconKind.PaletteOutline,
            318 => MaterialIconKind.FileDocumentEditOutline,
            319 => MaterialIconKind.FileDocumentOutline,
            320 => MaterialIconKind.MessageQuestionOutline,
            321 => MaterialIconKind.CloudUploadOutline,
            323 => MaterialIconKind.FormatListNumbered,
            311 => MaterialIconKind.AccountCircleOutline,
            19 => MaterialIconKind.Refresh,
            24 => MaterialIconKind.CardTextOutline,
            25 => MaterialIconKind.NavigationVariantOutline,
            26 => MaterialIconKind.TagOutline,
            415 or 27 or 307 or 322 => MaterialIconKind.FormatListNumbered,
            312 => MaterialIconKind.BookOpenPageVariantOutline,
            300 => MaterialIconKind.MessageBadgeOutline,
            301 => MaterialIconKind.WindowRestore,
            302 => MaterialIconKind.MessageAlertOutline,
            306 => MaterialIconKind.ProgressDownload,
            _ => MaterialIconKind.ViewGridOutline
        });
    }

    private static Geometry Get(MaterialIconKind kind)
    {
        if (!Cache.TryGetValue(kind, out var geometry))
            Cache.Add(kind, geometry = Geometry.Parse(MaterialIconDataProvider.GetData(kind)));
        return geometry;
    }
}
