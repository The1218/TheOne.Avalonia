namespace TheOne.Avalonia.Reporting;

// Code 128 symbol widths (ISO/IEC 15417); also cross-checked against ZXing's Code128Reader.
internal static class ReportBarcode
{
    private static readonly string[] Patterns = (
        "212222 222122 222221 121223 121322 131222 122213 122312 132212 221213 " +
        "221312 231212 112232 122132 122231 113222 123122 123221 223211 221132 " +
        "221231 213212 223112 312131 311222 321122 321221 312212 322112 322211 " +
        "212123 212321 232121 111323 131123 131321 112313 132113 132311 211313 " +
        "231113 231311 112133 112331 132131 113123 113321 133121 313121 211331 " +
        "231131 213113 213311 213131 311123 311321 331121 312113 312311 332111 " +
        "314111 221411 431111 111224 111422 121124 121421 141122 141221 112214 " +
        "112412 122114 122411 142112 142211 241211 221114 413111 241112 134111 " +
        "111242 121142 121241 114212 124112 124211 411212 421112 421211 212141 " +
        "214121 412121 111143 111341 131141 114113 114311 411113 411311 113141 " +
        "114131 311141 411131 211412 211214 211232 2331112").Split(' ');

    internal static string Encode(string text)
    {
        if (text.Length is < 1 or > 128 || text.Any(character => character is < ' ' or > '~'))
            throw new ArgumentException("Code 128 条码需要 1–128 个可打印 ASCII 字符（英文、数字或符号）。");
        var checksum = 104;
        var result = new System.Text.StringBuilder(Patterns[104]);
        for (var i = 0; i < text.Length; i++)
        {
            var value = text[i] - 32; checksum += value * (i + 1); result.Append(Patterns[value]);
        }
        return result.Append(Patterns[checksum % 103]).Append(Patterns[106]).ToString();
    }

    internal static bool CheckedValue(string value, bool fallback) => value.Trim().ToLowerInvariant() switch
    {
        "true" or "1" or "yes" or "是" => true,
        "false" or "0" or "no" or "否" or "" => false,
        _ => fallback
    };

    internal static bool IsCheckValue(string value) => value.Trim().ToLowerInvariant() is "true" or "false" or "1" or "0" or "yes" or "no" or "是" or "否" or "";
}
