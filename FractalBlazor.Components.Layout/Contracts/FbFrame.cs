using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  FractalBlazor.Components.Layout
{
    public enum FbFrame
    {
        None,
        Small,
        Medium,
        Large
    }

    public enum FbRadius
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
