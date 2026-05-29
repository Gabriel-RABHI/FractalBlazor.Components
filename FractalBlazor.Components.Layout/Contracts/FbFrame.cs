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
        Radius_0 = 0,
        Radius_1 = 1,
        Radius_2 = 2,
        Radius_3 = 3,
        Radius_4 = 4,
        Radius_5 = 5,
        Radius_6 = 6,
        Radius_7 = 7,
        Radius_8 = 8,
        Radius_9 = 9,
        Radius_10 = 10,
        Radius_12 = 12,
        Radius_14 = 14,
        Radius_16 = 16,
        Radius_20 = 20,
        Radius_24 = 24,
        Radius_28 = 28,
        Radius_32 = 32,
        Radius_40 = 40,
        Radius_48 = 48,
        Radius_56 = 56,
        Radius_64 = 64
    }
}
