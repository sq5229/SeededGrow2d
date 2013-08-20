using System;
using System.Collections.Generic;
using System.Text;

namespace SeededBlockFill3d.Layer
{
    class LargeFloodFill_Threshold : SeededGrowBase
    {
        public void SetThres(byte min, byte max)
        {
            this.min = min;
            this.max = max;
        }
        byte min;
        byte max;
        public override bool IncludeConditionMeets(Int16Triple t)
        {
            byte v = data[t.X + t.Y * width + t.Z * width * height];
            return v<=max&&v>=min;
        }
        public List<Int16Triple> GetResult()
        {
            return result;
        }
    }
}
