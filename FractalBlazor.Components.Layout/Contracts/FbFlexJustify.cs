using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  FractalBlazor.Components.Layout
{
    // https://the-echoplex.net/flexyboxes/
    /*
    bool JustifyStart

    FbFlexJustify Justify 

    */
    public enum FbFlexJustify : byte
    {
        Start,
        End,
        Center,
        Stretch,
        SpaceBetween,
        SpaceAround,
        Evenly
    }
}
