using System.Linq;

namespace FractalBlazor.Components.Layout;

public static class FbLayoutPresets {
    private static string[] _stringMap;

    public static string ToRem(FbSpacing v)
    {
        if (_stringMap == null)
            _stringMap = Enum.GetValues<FbSpacing>().Select(e => (int)e > 0 ? $"{((double)((int)e) / 16d).ToString().Replace(",", ".")}rem" : "0rem").ToArray();
        return _stringMap[((byte)v)+1];
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
