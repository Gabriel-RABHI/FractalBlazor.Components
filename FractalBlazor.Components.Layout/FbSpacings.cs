namespace FractalBlazor.Components.Layout;

public static class FbSpacings {
    public static FbMargin S_Margin { get; set; } = FbMargin.Margin_8;

    public static FbPadding S_Padding { get; set; } = FbPadding.Padding_8;

    public static FbGutter S_Gutter { get; set; } = FbGutter.Gutter_8;

    public static FbMargin M_Margin { get; set; } = FbMargin.Margin_12;

    public static FbPadding M_Padding { get; set; } = FbPadding.Padding_12;

    public static FbGutter M_Gutter { get; set; } = FbGutter.Gutter_12;

    public static FbMargin L_Margin { get; set; } = FbMargin.Margin_24;

    public static FbPadding L_Padding { get; set; } = FbPadding.Padding_24;

    public static FbGutter L_Gutter { get; set; } = FbGutter.Gutter_24;

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
}
