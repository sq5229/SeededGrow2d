using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Collections;
using SeededGrow3d;

namespace BoundaryLine2d
{
    public class MyFF : SeededGrow3d.FloodFill.FloodFill3d_T
    {
        public List<Int16Triple> result = new List<Int16Triple>();
        protected override void Process(SeededGrow3d.Int16Triple p)
        {
            base.Process(p);
            result.Add(new Int16Triple(p.X,p.Y,p.Z));
        }
    }

}
