using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow3d.SpanFill
{
    enum ParentDirections
    {
        Y0 = 1, Y2 = 2,Z0=3,Z2=4, Non = 5
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
        public FlagMap3d flagsMap;
        protected Container<Span> container;//以Span为单位的Queue或Stack容器
        protected virtual void ExcuteSpanFill(BitMap3d data, Int16Triple seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap3d(data.width, data.height,data.depth);
            Process(seed);
            flagsMap.SetFlagOn(seed.X, seed.Y,seed.Z, true);
            Span seedspan = new Span();
            seedspan.XLeft = seed.X;
            seedspan.XRight = seed.X;
            seedspan.Y = seed.Y;
            seedspan.Z = seed.Z;
            seedspan.ParentDirection = ParentDirections.Non;
            seedspan.Extended = ExtendTypes.UnRez;
            container.Push(seedspan);

            while (!container.Empty())
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
                        if (span.Z - 1 >=0)//[spx-spy,y,z-1]
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
                    int xl = FindXLeft(span.XLeft, span.Y,span.Z);
                    int xr = FindXRight(span.XRight, span.Y,span.Z);
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
                                CheckRange(xl, span.XLeft, span.Y, span.Z+1, ParentDirections.Z0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y, span.Z+1, ParentDirections.Z0);
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
                                CheckRange(xl, span.XLeft, span.Y, span.Z -1, ParentDirections.Z2);
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
                    int xl = FindXLeft(span.XLeft, span.Y,span.Z);
                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y , span.Z-1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(xl, span.XRight, span.Y , span.Z+1, ParentDirections.Z0);
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
                        if (span.Y - 1 >= 0 )
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
                        if (span.Z + 1 < bmp.depth )
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region RightRequired
                if (span.Extended == ExtendTypes.RightRequired)
                {
                    int xr = FindXRight(span.XRight, span.Y,span.Z);

                    if (span.ParentDirection == ParentDirections.Y2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1,span.Z, ParentDirections.Y2);
                        if (span.Y + 1 < bmp.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1,span.Z, ParentDirections.Y0);
                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z-1, ParentDirections.Z2);
                        if (span.Z + 1 < bmp.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z+1, ParentDirections.Z0); 
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
        protected void CheckRange(int xleft, int xright, int y, int z,ParentDirections ptype)
        {
            for (int i = xleft; i <= xright; )
            {
                if ((!flagsMap.GetFlagOn(i, y,z)) && IncludePredicate(i, y,z))
                {
                    int lb = i;
                    int rb = i + 1;
                    while (rb <= xright && (!flagsMap.GetFlagOn(rb, y,z)) && IncludePredicate(rb, y,z))
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
                        flagsMap.SetFlagOn(j, y,z, true);
                        Process(new Int16Triple(j, y,z));
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
        protected int FindXRight(int x, int y,int z)
        {
            int xright = x + 1;
            while (true)
            {
                if (xright == bmp.width || flagsMap.GetFlagOn(xright, y,z))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xright, y,z))
                    {
                        Int16Triple t = new Int16Triple(xright, y,z);
                        flagsMap.SetFlagOn(xright, y,z ,true);
                        Process(t);
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
        protected int FindXLeft(int x, int y,int z)
        {
            int xleft = x - 1;
            while (true)
            {
                if (xleft == -1 || flagsMap.GetFlagOn(xleft, y, z))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(xleft, y,z))
                    {
                        Int16Triple t = new Int16Triple(xleft, y,z);
                        flagsMap.SetFlagOn(xleft, y,z, true);
                        Process(t);
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
        public byte min;
        public byte max;
        protected bool IncludePredicate(int x, int y, int z)
        {
            byte v = bmp.GetPixel(x, y, z);
            return v > min && v < max;
        }
        protected void Process(Int16Triple p)
        {
            count++;
        }
    }
    class SpanFill3d_T : SpanFill3d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public void ExcuteSpanFill_Queue(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Queue<Span>();
            base.ExcuteSpanFill(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = container.action_pop_count;
            report.container_push_count = container.action_push_count;
            report.container_max_size = container.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
        public void ExcuteSpanFill_Stack(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Stack<Span>();
            base.ExcuteSpanFill(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = container.action_pop_count;
            report.container_push_count = container.action_push_count;
            report.container_max_size = container.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
    }
}
