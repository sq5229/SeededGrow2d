using System;
using System.Collections.Generic;
using System.Text;

namespace GrowingStrategies.Methods
{
    public class GradientGrow
    {
        public GradientGrow(BitMap2d bmp, Int16Double seed,byte r)
        {
            this.bmp = bmp;
            this.seed = seed;
            this.r = r;
            container = new Container_Queue<Int16Double>();
            flagsMap = new FlagMap2d(bmp.width, bmp.height);
        }
        protected byte r;
        protected Int16Double seed;
        protected BitMap2d bmp;
        protected FlagMap2d flagsMap;
        protected Container<Int16Double> container;
        public List<Int16Double> results = new List<Int16Double>();
        protected int count = 0;
        protected virtual bool IncludePredicate(Int16Double p, Int16Double or)
        {
            byte v = bmp.GetPixel(p.X, p.Y);
            byte vr = bmp.GetPixel(or.X, or.Y);
            return Math.Abs(vr - v) < r;
        }
        protected virtual bool InRange(int x, int y)
        {
            return x < bmp.width && x >= 0 && y < bmp.height && y >= 0;
        }
        protected virtual void Process(Int16Double p)
        {
            results.Add(p);
            count++;
            return;
        }
        public virtual void ExcuteFloodFill()
        {
            Int16Double[] adjPoints4 = new Int16Double[4];
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            container.Push(seed);
            Process(seed);
            while (!container.Empty())
            {
                Int16Double p = container.Pop();
                InitAdj4(ref adjPoints4, ref p);
                for (int adjIndex = 0; adjIndex < 4; adjIndex++)
                {
                    Int16Double t = adjPoints4[adjIndex];
                    if (InRange(t.X, t.Y))
                    {
                        if (!flagsMap.GetFlagOn(t.X, t.Y) && IncludePredicate(t,p))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y, true);
                            container.Push(t);
                            Process(t);
                        }
                    }
                }
            }
            return;
        }
        protected void InitAdj4(ref Int16Double[] adjPoints4, ref Int16Double p)
        {
            adjPoints4[0].X = p.X - 1;
            adjPoints4[0].Y = p.Y;

            adjPoints4[1].X = p.X + 1;
            adjPoints4[1].Y = p.Y;

            adjPoints4[2].X = p.X;
            adjPoints4[2].Y = p.Y - 1;

            adjPoints4[3].X = p.X;
            adjPoints4[3].Y = p.Y + 1;
        }
    }
}
