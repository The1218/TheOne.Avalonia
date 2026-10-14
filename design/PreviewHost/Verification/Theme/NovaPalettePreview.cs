using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace TheOne.DesignPreview;

// Public Nova palette at Eremex/controlthemes commit 5f6cb3ad8b904cead6bd432f62596648cc9a1039.
internal static class NovaPalettePreview
{
    private static readonly (string Key, string Light, string Dark)[] Reference =
    [
        ("One.Canvas", "#FFFFFF", "#0F0F0F"),
        ("One.Surface", "#FFFFFF", "#0F0F0F"),
        ("One.Subtle", "#EDEDED", "#282929"),
        ("One.Text", "#212222", "#F2F2F2"),
        ("One.Muted", "#474D4D", "#CACECE"),
        ("One.Line", "#D2D5D5", "#3D4242"),
        ("One.InputLine", "#D2D5D5", "#3D4242"),
        ("One.Accent", "#006061", "#63D9DE"),
        ("One.AccentHover", "#006A6B", "#5DD2D5"),
        ("One.AccentPressed", "#007475", "#55CBCE"),
        ("One.AccentSoft", "#C1DCDC", "#284343"),
        ("One.OnAccent", "#F7F8F8", "#181B1B"),
        ("One.Success", "#007A06", "#57D15D"),
        ("One.Info", "#395DC6", "#9EB4FF"),
        ("One.Warning", "#855D00", "#DDB35F"),
        ("One.Danger", "#99000D", "#FFBDC2"),
        ("One.DangerHover", "#A80B18", "#FFB3B9"),
        ("One.DangerPressed", "#B71523", "#FFA8B0"),
        ("One.DisabledText", "#AAB1B1", "#6C7575"),
        ("One.DisabledSurface", "#F2F2F2", "#212222"),
        ("One.CardTabStrip", "#EDEDED", "#282929"),
        ("One.CardTabBorder", "#CACECE", "#474D4D"),
        ("One.CardTabHover", "#D6D7D7", "#3F4141"),
        ("One.Focus", "#129190", "#36AFB0"),
        ("One.NeutralHover", "#D6D7D7", "#3F4141"),
        ("One.NeutralPressed", "#CECFCF", "#494B4B"),
        ("One.SurfaceHover", "#F2F2F2", "#212222"),
        ("One.SurfacePressed", "#EDEDED", "#282929"),
        ("One.AccentSoftHover", "#B9D4D4", "#324E4E"),
        ("One.AccentSoftPressed", "#B1CDCD", "#3C5858"),
        ("One.RowSelected", "#DFF1F1", "#162C2C"),
        ("One.RowSelectedHover", "#D6EBEB", "#1F3838"),
        ("One.RowSelectedPressed", "#CDE4E5", "#264040"),
        ("One.TextSelection", "#6AE2E7", "#005557"),
        ("One.OutlineMedium", "#CACECE", "#474D4D"),
        ("One.OutlineStrong", "#C2C7C7", "#515758"),
        ("One.DisabledSelection", "#DDDEDE", "#3A3B3B"),
        ("One.DisabledIcon", "#B2B8B8", "#626A6A"),
        ("One.IconOnAccent", "#FFFFFF", "#0F0F0F"),
        ("One.PrimaryDisabled", "#F7F7F7", "#191A1A"),
        ("One.SuccessFill", "#85DB89", "#09620D"),
        ("One.InfoFill", "#88D5DD", "#095C62"),
        ("One.WarningFill", "#E1C689", "#684F0D"),
        ("One.DangerFill", "#FFB3B3", "#8A3838"),
        ("One.Chart.Background", "#FFFFFF", "#000000"),
        ("One.Chart.Text", "#385252", "#DBE6E6"),
        ("One.Chart.Grid", "#CADADA", "#707575"),
        ("One.Placeholder", "#626A6A", "#B2B8B8"),
    ];

    public static void Verify(Control owner, ThemeVariant theme, string label, Action<bool, string> check)
    {
        foreach (var (key, light, dark) in Reference)
            check(owner.TryFindResource(key, theme, out var value) && value is ISolidColorBrush brush &&
                  brush.Color == Color.Parse(theme == ThemeVariant.Dark ? dark : light),
                  $"{label}: {key} matches the original Eremex Nova resource");
    }
}
