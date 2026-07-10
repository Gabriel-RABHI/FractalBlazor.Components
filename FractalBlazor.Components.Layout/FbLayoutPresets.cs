using System.Linq;

namespace FractalBlazor.Components.Layout;

public static class FbLayoutPresets {
    private static string[] _stringMap;

    public static string ToRem(FbSpacing v)
    {
        var index = (byte)v;
        if (index < 0)
            return "0rem";
        if (_stringMap == null)
        {
            var strs = new List<string>();
            for (var x = 0; x < Enum.GetValues<FbSpacing>().Select(e => (byte)e).Max(); x++)
                strs.Add($"{((double)(x / 16d)).ToString().Replace(",", ".")}rem");
            _stringMap = strs.ToArray();
        }
        return _stringMap[((byte)v)+1];
    }

    public static string ToSpacingCss(FbSpacing v)
    {
        if (v == S)
            return "var(--fb-space-s)";
        if (v == M)
            return "var(--fb-space-m)";
        if (v == L)
            return "var(--fb-space-l)";
        if (v == X)
            return "var(--fb-space-x)";

        return ToRem(v);
    }

    public static string ToRadiusCss(FbSpacing v)
    {
        if (v == RS)
            return "var(--fb-radius-s)";
        if (v == RM)
            return "var(--fb-radius-m)";
        if (v == RL)
            return "var(--fb-radius-l)";
        if (v == RX)
            return "var(--fb-radius-x)";

        return ToRem(v);
    }

    public static FbSpacing RS { get; set; } = FbSpacing._2;

    public static FbSpacing RM { get; set; } = FbSpacing._4;

    public static FbSpacing RL { get; set; } = FbSpacing._7;

    public static FbSpacing RX { get; set; } = FbSpacing._16;


    public static FbSpacing S { get; set; } = FbSpacing._4;

    public static FbSpacing M { get; set; } = FbSpacing._8;

    public static FbSpacing L { get; set; } = FbSpacing._14;

    public static FbSpacing X { get; set; } = FbSpacing._28;


    public static int S_Flex { get; set; } = 1;

    public static int M_Flex { get; set; } = 2;

    public static int L_Flex { get; set; } = 4;

    public static int X_Flex { get; set; } = 8;

    public static int XX_Flex { get; set; } = 12;
}
