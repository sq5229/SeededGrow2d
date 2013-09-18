using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeededBlockFill3d.Block.FF.ConcreteFills
{
    class B_LargeFloodFill_Threshold : B_FloodFillBase
    {
        public void SetThres(byte min, byte max)
        {
            this.min = min;
            this.max = max;
        }
        byte min;
        byte max;
        protected override bool IncludeConditionMeets(Int16Triple t)
        {
            byte v = data[t.X + t.Y * width + t.Z * width * height];
            return v <= max && v >= min;
        }
    }
}
