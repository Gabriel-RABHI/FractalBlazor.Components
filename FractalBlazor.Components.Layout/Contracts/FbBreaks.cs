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
    
    public enum FbSpacing : sbyte
    {
        [DefaultValue(None)]
        None = -1,
        _0 = 0,
        _1 = 1,
        _2 = 2,
        _3 = 3,
        _4 = 4,
        _5 = 5,
        _6 = 6,
        _7 = 7,
        _8 = 8,
        _10 = 10,
        _12 = 12,
        _14 = 14,
        _16 = 16,
        _20 = 20,
        _24 = 24,
        _28 = 28,
        _32 = 32,
        _40 = 40,
        _48 = 48,
        _56 = 56,
        _64 = 64
    }
}
