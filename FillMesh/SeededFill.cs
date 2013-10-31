using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    enum ParentDirections
    {
        Y0 = 1, Y2 = 2, Z0 = 3, Z2 = 4, Non = 5
    }
    enum ExtendTypes
    {
        LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
    }
    struct Span
    {
        public int XLeft;
        public int XRight;
        public int Y;
        public int Z;
        public ExtendTypes Extended;
        public ParentDirections ParentDirection;
    }
    class SpanFill3d
    {
        protected int count = 0;
        protected BitMap3d bmp;
        protected Stack<Span> container;//以Span为单位的Queue或Stack容器
        public virtual void ExcuteSpanFill(BitMap3d data, Int16Triple seed)
        {
            container = new Stack<Span>();
            this.bmp = data;
            bmp.SetPixel(seed.X, seed.Y, seed.Z,BitMap3d.WHITE);
            Span seedspan = new Span();
            seedspan.XLeft = seed.X;
            seedspan.XRight = seed.X;
            seedspan.Y = seed.Y;
            seedspan.Z = seed.Z;
            seedspan.ParentDirection = ParentDirections.Non;
            seedspan.Extended = ExtendTypes.UnRez;
            container.Push(seedspan);

            while (container.Count!=0)
            {
                Span span = container.Pop();
                #region AllRez
                if (span.Extended == ExtendTypes.AllRez)
                {
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region UnRez
                if (span.Extended == ExtendTypes.UnRez)
                {
                    int xl = FindXLeft(span.XLeft, span.Y, span.Z);
                    int xr = FindXRight(span.XRight, span.Y, span.Z);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        }
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        }
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        }
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Non)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region LeftRequired
                if (span.Extended == ExtendTypes.LeftRequired)
                {
                    int xl = FindXLeft(span.XLeft, span.Y, span.Z);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region RightRequired
                if (span.Extended == ExtendTypes.RightRequired)
                {
                    int xr = FindXRight(span.XRight, span.Y, span.Z);

                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Y - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
        }
        protected void CheckRange(int xleft, int xright, int y, int z, ParentDirections ptype)
        {
            for (int i = xleft; i <= xright; )
            {
                if (IncludePredicate(i, y, z))
                {
                    int lb = i;
                    int rb = i + 1;
                    while (rb <= xright && (IncludePredicate(rb, y, z)))
                    {
                        rb++;
                    }
                    rb--;

                    Span span = new Span();
                    span.XLeft = lb;
                    span.XRight = rb;
                    span.Y = y;
                    span.Z = z;
                    if (lb == xleft && rb == xright)
                    {
                        span.Extended = ExtendTypes.UnRez;
                    }
                    else if (rb == xright)
                    {
                        span.Extended = ExtendTypes.RightRequired;
                    }
                    else if (lb == xleft)
                    {
                        span.Extended = ExtendTypes.LeftRequired;
                    }
                    else
                    {
                        span.Extended = ExtendTypes.AllRez;
                    }
                    span.ParentDirection = ptype;
                    for (int j = lb; j <= rb; j++)
                    {
                        bmp.SetPixel(j, y, z,BitMap3d.WHITE);
                    }
                    container.Push(span);

                    i = rb + 1;
                }
                else
                {
                    i++;
                }
            }
        }//区段法的CheckRange 注意与扫描线的CheckRange的不同
        protected int FindXRight(int x, int y, int z)
        {
            int xright = x + 1;
            while (true)
            {
                if (xright == bmp.width)// || flagsMap.GetFlagOn(xright, y, z))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xright, y, z))
                    {
                        Int16Triple t = new Int16Triple(xright, y, z);
                        bmp.SetPixel(xright, y, z, BitMap3d.WHITE);
                        //Process(t);
                        xright++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return xright - 1;
        }
        protected int FindXLeft(int x, int y, int z)
        {
            int xleft = x - 1;
            while (true)
            {
                if (xleft == -1)// || flagsMap.GetFlagOn(xleft, y, z))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xleft, y, z))
                    {
                        Int16Triple t = new Int16Triple(xleft, y, z);
                        bmp.SetPixel(xleft, y, z, BitMap3d.WHITE);
                        xleft--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return xleft + 1;
        }
        protected bool IncludePredicate(int x, int y, int z)
        {
            return bmp.GetPixel(x, y, z) != BitMap3d.WHITE;
        }
    }
    class FloodFill3d
    {
        protected void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
        {
            adjPoints6[0].X = p.X - 1;
            adjPoints6[0].Y = p.Y;
            adjPoints6[0].Z = p.Z;

            adjPoints6[1].X = p.X + 1;
            adjPoints6[1].Y = p.Y;
            adjPoints6[1].Z = p.Z;

            adjPoints6[2].X = p.X;
            adjPoints6[2].Y = p.Y - 1;
            adjPoints6[2].Z = p.Z;

            adjPoints6[3].X = p.X;
            adjPoints6[3].Y = p.Y + 1;
            adjPoints6[3].Z = p.Z;


            adjPoints6[4].X = p.X;
            adjPoints6[4].Y = p.Y;
            adjPoints6[4].Z = p.Z - 1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z + 1;
        }
        public virtual void ExcuteFloodFill(BitMap3d data, Int16Triple seed)
        {
            BitMap3d bmp = data;
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            Queue<Int16Triple> container = new Queue<Int16Triple>();
            bmp.SetPixel(seed.X, seed.Y, seed.Z, BitMap3d.WHITE);
            container.Enqueue(seed);
            while (container.Count!=0)
            {
                Int16Triple p = container.Dequeue();
                InitAdj6(ref adjPoints6, ref p);
                for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                {
                    Int16Triple t = adjPoints6[adjIndex];
                    if (t.X < data.width && t.X >= 0 && t.Y < data.height && t.Y >= 0 && t.Z < data.depth && t.Z >= 0)
                    {
                        if (bmp.GetPixel(t.X,t.Y,t.Z)!=BitMap3d.WHITE)
                        {
                            bmp.SetPixel(t.X, t.Y, t.Z, BitMap3d.WHITE);
                            container.Enqueue(t);
                        }
                    }
                }
            }
            return;
        }
    }
}
