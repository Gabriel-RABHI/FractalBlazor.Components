namespace FractalBlazor.Components.Layout;

public static class FbPresets {
    public static FbMargin S_Margin { get; set; } = FbMargin._4;

    public static string S_Padding_String => $"margin-top:{((double)((int)S_Padding) / 16d).ToString().Replace(",", ".")}rem;";

    public static FbPadding S_Padding { get; set; } = FbPadding._4;

    public static FbGutter S_Gutter { get; set; } = FbGutter._4;

    public static string S_Spacing => $"margin-top:{((double)((int)S_Padding) / 16d).ToString().Replace(",", ".")}rem;";

    public static FbRadius S_Radius { get; set; } = FbRadius._3;

    public static FbMargin M_Margin { get; set; } = FbMargin._8;

    public static FbPadding M_Padding { get; set; } = FbPadding._8;

    public static FbGutter M_Gutter { get; set; } = FbGutter._8;

    public static FbRadius M_Radius { get; set; } = FbRadius._5;

    public static FbMargin L_Margin { get; set; } = FbMargin._14;

    public static FbPadding L_Padding { get; set; } = FbPadding._14;

    public static FbGutter L_Gutter { get; set; } = FbGutter._14;

    public static FbRadius L_Radius { get; set; } = FbRadius._10;

    public static FbMargin X_Margin { get; set; } = FbMargin._28;

    public static FbPadding X_Padding { get; set; } = FbPadding._28;

    public static FbGutter X_Gutter { get; set; } = FbGutter._28;

    public static FbRadius X_Radius { get; set; } = FbRadius._20;

    public static FbMargin S {
        set {
            S_Padding = (FbPadding)(byte)value;
            S_Gutter = (FbGutter)(byte)value;
            S_Margin = value;
        }
    }

    public static FbMargin M {
        set {
            M_Padding = (FbPadding)(byte)value;
            M_Gutter = (FbGutter)(byte)value;
            M_Margin = value;
        }
    }

    public static FbMargin L {
        set {
            L_Padding = (FbPadding)(byte)value;
            L_Gutter = (FbGutter)(byte)value;
            L_Margin = value;
        }
    }

    public static FbMargin X {
        set {
            X_Padding = (FbPadding)(byte)value;
            X_Gutter = (FbGutter)(byte)value;
            X_Margin = value;
        }
    }

    public static int S_Flex { get; set; } = 1;

    public static int M_Flex { get; set; } = 2;

    public static int L_Flex { get; set; } = 4;

    public static int X_Flex { get; set; } = 8;

    public static int XX_Flex { get; set; } = 12;
}
