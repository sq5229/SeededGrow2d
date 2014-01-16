using System;
using System.Collections.Generic;
using System.Text;

namespace GrowingStrategies.Methods
{
public class Confidence
{
    public Confidence(BitMap2d bmp,Int16Double seed,int iterTime,double factor,int radius)
    {
        this.bmp = bmp;
        this.seed = seed;
        this.factor = factor;
        this.radius = radius;
        this.iterTime = iterTime;
        container = new Container_Queue<Int16Double>();
        container2 = new Container_Queue<Int16Double>();
        flagsMap = new FlagMap2d(bmp.width, bmp.height);
    }
    protected double sum;
    protected double sum2;
    protected double mean;
    protected double deviation;
    protected double cmin;
    protected double cmax;
    protected int iterTime;
    protected double factor;
    protected int radius;
    protected Int16Double seed;
    protected BitMap2d bmp;
    protected FlagMap2d flagsMap;
    protected Container<Int16Double> container;
    protected Container<Int16Double> container2;
    public List<Int16Double> results = new List<Int16Double>();
    protected int count = 0;

    protected virtual bool IncludePredicate(Int16Double p,ref double value)
    {
        byte v = bmp.GetPixel(p.X, p.Y);
        value = v;
        return value >= cmin && value <= cmax;
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
        flagsMap.SetFlagOn(seed.X, seed.Y, true);
        container.Push(seed);
        Process(seed);
        FirstCaculate();
        for (int l = 0; l < iterTime; l++)
        {
            FloodFill();
            ReCaluete();
            bool haschanged = ReEnqueue();
            if (!haschanged)
                break;
        }
    }

    protected void FirstCaculate()
    {
        int Radius = radius;
        double sum = 0;
        double sum2 = 0;
        Int16Double[,] radiusRange = new Int16Double[2 * Radius + 1, 2 * Radius + 1];
        for (int i = 0; i < 2 * Radius + 1; i++)
        {
            for (int j = 0; j < 2 * Radius + 1; j++)
            {
                radiusRange[i, j].X = seed.X - Radius + i;
                radiusRange[i, j].Y = seed.Y - Radius + j;
            }
        }
        int c = 0;
        for (int i = 0; i < 2 * Radius + 1; i++)
        {
            for (int j = 0; j < 2 * Radius + 1; j++)
            {
                int d2 = (i - Radius) * (i - Radius) + (j - Radius) * (j - Radius);
                if (d2 <= Radius * Radius)
                {
                    Int16Double t = radiusRange[i, j];
                    if (!InRange(t.X, t.Y))
                        continue;
                    float v = bmp.GetPixel(t.X, t.Y);
                    sum += v;
                    sum2 += v * v;
                    c++;
                }
            }
        }
        this.mean = sum / c;
        this.deviation = Math.Sqrt(sum2 / c - this.mean * this.mean);
        this.cmin = mean - factor * deviation;
        this.cmax = mean + factor * deviation;
    }

    protected void ReCaluete()
    {
        this.mean = sum / count;
        this.deviation = Math.Sqrt(sum2 / count - mean * mean);
        this.cmin = mean - factor * deviation;
        this.cmax = mean + factor * deviation;
    }

    protected void FloodFill()
    {
        double v=0;
        Int16Double[] adjPoints4 = new Int16Double[4];
        while (!container.Empty())
        {
            Int16Double p = container.Pop();
            InitAdj4(ref adjPoints4, ref p);
            for (int adjIndex = 0; adjIndex < 4; adjIndex++)
            {
                Int16Double t = adjPoints4[adjIndex];
                if (InRange(t.X, t.Y))
                {
                    if (!flagsMap.GetFlagOn(t.X, t.Y) && IncludePredicate(t, ref v))
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

    protected bool ReEnqueue()
    {
        bool haschanged = false;
        while (!container2.Empty())
        {
            Int16Double t = container2.Pop();
            double v=0;
            if (IncludePredicate(t, ref v))
            {
                haschanged = true;
                flagsMap.SetFlagOn(t.X,t.Y,true);
                Process(t);
                sum += v;
                sum2 += v * v;
                container.Push(t);
            }
        }
        return haschanged;
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
