using System.ComponentModel;

namespace  FractalBlazor.Components.Layout
{
    public enum FbBreaks : byte
    {
        XXS_480px,
        XS_600px,
        S_960px,
        M_1280px,
        L_1600px,
        XL_1960px
    }
    public enum FbResponsiveBreakpoint : byte
    {
        None,
        XS_360px,
        S_640px,
        M_768px,
        L_1024px,
        XL_1280px,
        XXL_1536px
    }

    public enum FbPadding : sbyte
    {
        [DefaultValue(None)]
        None = -1,
        Padding_0 = 0,
        Padding_1 = 1,
        Padding_2 = 2,
        Padding_3 = 3,
        Padding_4 = 4,
        Padding_8 = 8,
        Padding_12 = 12,
        Padding_16 = 16,
        Padding_24 = 24,
        Padding_32 = 32,
        Padding_40 = 40,
        Padding_48 = 48
    }

    public enum FbMargin : sbyte
    {
        [DefaultValue(None)]
        None = -1,
        Margin_0 = 0,
        Margin_1 = 1,
        Margin_2 = 2,
        Margin_3 = 3,
        Margin_4 = 4,
        Margin_8 = 8,
        Margin_12 = 12,
        Margin_16 = 16,
        Margin_24 = 24,
        Margin_32 = 32,
        Margin_40 = 40,
        Margin_48 = 48
    }
}
