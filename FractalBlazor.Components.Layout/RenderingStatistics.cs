using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  FractalBlazor.Components.Layout
{
    public static class RenderingStatistics
    {
        private static int _componentBaseStyleComputationsCount = 0;
        private static int _flexBoxBaseStyleComputationsCount = 0;

        public static int ComponentBaseStyleComputationsCount => _componentBaseStyleComputationsCount;

        public static int FlexBoxBaseStyleComputationsCount => _flexBoxBaseStyleComputationsCount;

        public static void AddComponentBaseStyleComputation()
            => Interlocked.Increment(ref _componentBaseStyleComputationsCount);

        public static void AddFlexBoxBaseStyleComputation()
            => Interlocked.Increment(ref _flexBoxBaseStyleComputationsCount);
    }
}
