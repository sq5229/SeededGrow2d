using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    public class SpanFill3d
    {
        private enum ParentDirections
        {
            Y0 = 1, Y2 = 2, Z0 = 3, Z2 = 4, Non = 5
        }
        private enum ExtendTypes
        {
            LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
        }
        private struct Span
        {
            public int XLeft;
            public int XRight;
            public int Y;
            public int Z;
            public ExtendTypes Extended;
            public ParentDirections ParentDirection;
        }
        private byte includeColor;
        private byte replaceColor;
        private ByteMatrix bmp;
        private Stack<Span> container;
        public virtual void ExcuteSpanFill(ByteMatrix data, Int16Triple seed, byte includeColor,byte replaceColor)
        {
            this.includeColor = includeColor;
            this.replaceColor = replaceColor;
            this.container = new Stack<Span>();
            this.bmp = data;
            Process(seed.X, seed.Y, seed.Z);
            Span seedspan = new Span();
            seedspan.XLeft = seed.X;
            seedspan.XRight = seed.X;
            seedspan.Y = seed.Y;
            seedspan.Z = seed.Z;
            seedspan.ParentDirection = ParentDirections.Non;
            seedspan.Extended = ExtendTypes.UnRez;
            container.Push(seedspan);

            while (container.Count != 0)
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
        private void CheckRange(int xleft, int xright, int y, int z, ParentDirections ptype)
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
                        Process(j, y, z);
                    }
                    container.Push(span);

                    i = rb + 1;
                }
                else
                {
                    i++;
                }
            }
        }
        private int FindXRight(int x, int y, int z)
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
                        Process(xright, y, z);
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
        private int FindXLeft(int x, int y, int z)
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
                        Process(xleft, y, z);
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
        private bool IncludePredicate(int x, int y, int z)
        {
            return bmp.GetValue(x, y, z) ==includeColor;
        }
        private void Process(int x, int y, int z)
        {
            bmp.SetValue(x, y, z, replaceColor);
        }
    }
}