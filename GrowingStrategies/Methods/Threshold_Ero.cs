using System;
using System.Collections.Generic;
using System.Text;

namespace GrowingStrategies.Methods
{
    public class Threshold_Ero: Threshold
    {
        int radius;
        
        public Threshold_Ero(BitMap2d bmp, Int16Double seed, byte min, byte max, int radius)
            :base(bmp,seed,min,max)
        {
            this.radius = radius;
            InitRadiusRange();
        }
List<Int16Double> radiusWin = new List<Int16Double>();
private void InitRadiusRange()
{
    int Radius = radius;
    for (int i = 0; i < 2 * Radius + 1; i++)
    {
            for (int j = 0; j < 2 * Radius + 1; j++)
            {
                    int d2=(i-Radius)*(i-Radius)+(j-Radius)*(j-Radius);
                    if(d2<=Radius*Radius)
                    {
                            Int16Double t=new Int16Double(i-Radius,j-Radius);
                            radiusWin.Add(t);
                    }
            }
    }
}
protected override bool IncludePredicate(Int16Double p)
{
    bool ret1 = base.IncludePredicate(p);
    if (ret1)
    {
        if (CheckRange(p.X, p.Y))
        {
            return true;
        }
        else
            return false;
    }
    else
    {
        return false;
    }
}
bool CheckRange(int x, int y)
{
    for (int i = 0; i < radiusWin.Count; i++)
    {
        int rx = x + radiusWin[i].X;
        int ry = y + radiusWin[i].Y;
        if (!InRange(rx, ry))
            continue;
        if (!base.IncludePredicate(new Int16Double(rx, ry)))
            return false;
    }
    return true;
}

    }
}
