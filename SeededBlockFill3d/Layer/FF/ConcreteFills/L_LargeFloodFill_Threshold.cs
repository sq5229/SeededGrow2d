using System;
using System.Collections.Generic;
using System.Text;

namespace SeededBlockFill3d.Layer.FF.ConcreteFills
{
    class L_LargeFloodFill_Threshold : L_FloodFillBase
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
            return v<=max&&v>=min;
        }
        //public List<Int16Triple> GetResult()
        //{
        //    return result;
        //}
        public int GetResult()
        {
            return resultCount;
        }
    }
}
