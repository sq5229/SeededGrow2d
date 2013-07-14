using System;
using System.Collections.Generic;
using System.Text;

namespace SeededBlockFill3d.Block
{
    public class FloodFill_Threshold:SubSeededGrow
    {
        public void SetThres(byte min, byte max)
        {
            this.min = min;
            this.max = max;
        }
        byte min;
        byte max;
        List<Int16Triple> result = new List<Int16Triple>();
        public override void OnRegionPointFind(Int16Triple t)
        {
            result.Add(t);
        }
        public override bool IncludeConditionMeets(Int16Triple t)
        {
            return true;
        }
    }
}
