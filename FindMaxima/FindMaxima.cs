using System;
using System.Collections.Generic;
using System.Text;

namespace FindMaxima
{
public class MaximunFinder
{
    BitMap2d bmp;
    float torlerance;
    const byte UNPROCESSED = 0;
    const byte VISITED = 1;
    const byte PROCESSED = 2;
    static Int16Double[] Delta = new Int16Double[8]
    {
        new Int16Double(-1,-1),
        new Int16Double(-1,0),
        new Int16Double(-1,1),
        new Int16Double(0,-1),
        new Int16Double(0,1),
        new Int16Double(1,-1),
        new Int16Double(1,0),
        new Int16Double(1,1),
    };

    public MaximunFinder(BitMap2d bmp,float torlerance)
    {
        this.bmp = bmp;
        this.torlerance = torlerance;
    }
    public List<Int16DoubleWithValue> FindMaxima()
    {
        List<Int16DoubleWithValue> list = FindLocalMaxima();
        list.Sort();
        FlagMap2d flag=new FlagMap2d(bmp.width,bmp.height,0);
        List<Int16DoubleWithValue> r=new List<Int16DoubleWithValue>();
        List<Int16Double> temp=new List<Int16Double>();
        for (int i = 0; i < list.Count; i++)
        {
            if (flag.GetFlagOn(list[i].X, list[i].Y) == UNPROCESSED)
            {
                bool ret = FloodFill(list[i].X, list[i].Y,temp,flag);
                if (ret)
                {
                    r.Add(list[i]);
                        
                    MarkAll(temp, PROCESSED, flag);
                }
                else
                {
                    MarkAll(temp, UNPROCESSED, flag);
                    flag.SetFlagOn(list[i].X, list[i].Y, PROCESSED);
                }
                temp.Clear();
            }
        }
        return r;
    }

    private List<Int16DoubleWithValue> FindLocalMaxima()
    {
        List<Int16DoubleWithValue> list = new List<Int16DoubleWithValue>();
        for (int i = 1; i < bmp.width - 1; i++)
        {
            for (int j = 1; j < bmp.height - 1; j++)
            {
                if (IsMaxima(i, j))
                {
                    list.Add(new Int16DoubleWithValue(i, j,bmp.GetPixel(i,j)));
                }
            }
        }
        return list;
    }

    private bool IsMaxima(int i, int j)
    {
        float v = bmp.GetPixel(i, j);
        bool b1 = v > bmp.GetPixel(i - 1, j - 1);
        bool b2 = v > bmp.GetPixel(i, j - 1);
        bool b3 = v > bmp.GetPixel(i +1, j - 1);

        bool b4 = v > bmp.GetPixel(i - 1, j);
        bool b5 = v > bmp.GetPixel(i + 1, j);

        bool b6 = v > bmp.GetPixel(i - 1, j + 1);
        bool b7 = v > bmp.GetPixel(i, j + 1);
        bool b8 = v > bmp.GetPixel(i + 1, j + 1);
        return b1 && b2 && b3 && b4 && b5 && b6 && b7 && b8;
    }

    private bool FloodFill(int x, int y,List<Int16Double> ret,FlagMap2d flag)
    {
        ret.Clear();
        Queue<Int16Double> queue = new Queue<Int16Double>();
        ret.Add(new Int16Double(x, y));
        float pvalue = bmp.GetPixel(x, y);
        flag.SetFlagOn(x, y, VISITED);
        queue.Enqueue(new Int16Double(x, y));
        while (queue.Count != 0)
        {
            Int16Double p = queue.Dequeue();
            for (int i = 0; i < 8; i++)
            {
                int tx = p.X + Delta[i].X;
                int ty = p.Y + Delta[i].Y;
                if(InRange(tx, ty))
                {
                    byte f= flag.GetFlagOn(tx,ty);
                    if(f==PROCESSED)
                        return false;
                    else
                    {
                        bool minum = false;
                        if (IncludePredicate(tx, ty, pvalue,ref minum) && f == UNPROCESSED)
                        {
                            if (minum)
                                return false;
                            Int16Double t = new Int16Double(tx, ty);
                            queue.Enqueue(t);
                            flag.SetFlagOn(tx, ty, VISITED);
                            ret.Add(t);
                        }
                    }

                }
            }
        }
        return true;
    }

    private bool InRange(int tx, int ty)
    {
        return tx >= 0 && tx < bmp.width && ty >= 0 && ty < bmp.height;
    }

    private bool IncludePredicate(int x, int y, float pv, ref bool min)
    {
        float v = bmp.GetPixel(x, y);
        if (pv < v)
            min = true;
        return pv - v <= torlerance;
    }

    private void MarkAll(List<Int16Double> ret, byte v,FlagMap2d flag)
    {
        for (int i = 0; i < ret.Count; i++)
        {
            flag.SetFlagOn(ret[i].X, ret[i].Y, v);
        }
    }

}
}
