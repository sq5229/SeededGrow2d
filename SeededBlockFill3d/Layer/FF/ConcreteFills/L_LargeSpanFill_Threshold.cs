using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeededBlockFill3d.Layer.FF.ConcreteFills
{
    class L_LargeSpanFill_Threshold:L_SpanFillBase
    {
        public void SetThres(byte min, byte max)
        {
            this.min = min;
            this.max = max;
        }
        byte min;
        byte max;
        protected override bool IncludeConditionMeets(int x,int y,int z)
        {
            byte v = data.GetPixel(x,y ,z);
            return v <= max && v >= min;
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
