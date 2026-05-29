namespace FractalBlazor.Components.Layout;

public static class FbSpacings {
    public static FbMargin S_Margin { get; set; } = FbMargin.Margin_4;

    public static FbPadding S_Padding { get; set; } = FbPadding.Padding_4;

    public static FbGutter S_Gutter { get; set; } = FbGutter.Gutter_4;

    public static FbRadius S_Radius { get; set; } = FbRadius.Radius_3;

    public static FbMargin M_Margin { get; set; } = FbMargin.Margin_8;

    public static FbPadding M_Padding { get; set; } = FbPadding.Padding_8;

    public static FbGutter M_Gutter { get; set; } = FbGutter.Gutter_8;

    public static FbRadius M_Radius { get; set; } = FbRadius.Radius_5;

    public static FbMargin L_Margin { get; set; } = FbMargin.Margin_14;

    public static FbPadding L_Padding { get; set; } = FbPadding.Padding_14;

    public static FbGutter L_Gutter { get; set; } = FbGutter.Gutter_14;

    public static FbRadius L_Radius { get; set; } = FbRadius.Radius_9;

    public static FbMargin X_Margin { get; set; } = FbMargin.Margin_28;

    public static FbPadding X_Padding { get; set; } = FbPadding.Padding_28;

    public static FbGutter X_Gutter { get; set; } = FbGutter.Gutter_28;

    public static FbRadius X_Radius { get; set; } = FbRadius.Radius_20;

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
}
