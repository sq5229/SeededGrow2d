using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Layer.FF
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
    struct Range
    {
        public int XLeft;
        public int XRight;
        public int Y;
        public int Z;
        public Range(int xleft, int xright, int y, int z)
        {
            this.XLeft = xleft;
            this.XRight = xright;
            this.Y = y;
            this.Z = z;
        }
    }
    struct SpanFillResult
    {
        //public List<Int16Triple> resultPointSet;
        public int resultCount;
        public List<Range>[] boundaryRequestPoints;
        public bool GetNeedsSeekLower()
        {
            return boundaryRequestPoints[0].Count > 0;
        }
        public bool GetNeedsSeekUpper()
        {
            return boundaryRequestPoints[1].Count > 0;
        }
        public void Init()
        {
            boundaryRequestPoints = new List<Range>[2];
            for (int i = 0; i < 2; i++)
            {
                boundaryRequestPoints[i] = new List<Range>();
            }
            //resultPointSet = new List<Int16Triple>();
        }
        public void Clear()
        {
            //resultPointSet = null;
            boundaryRequestPoints[0] = null;
            boundaryRequestPoints[1] = null;
        }
    }
    struct SpanFillInput
    {
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public FlagMap3d flag;
        public List<Range> spanlist;
        public Int16Triple seed;
        public bool recordUpper;
        public bool recordLower;
        public bool IsFirst;
        public SpanFillInput(byte[] data, int width, int height, int depth, FlagMap3d flag, List<Range> spanList, Int16Triple seed, bool recordUpper, bool recordLower, bool isfirst)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.flag = flag;
            this.spanlist = spanList;
            this.seed = seed;
            if (spanlist != null)
                for (int i = 0; i < spanlist.Count; i++)
                {
                    if (!(spanlist[i].Y >= 0 && spanlist[i].Y < height && spanlist[i].Z >= 0 && spanlist[i].Z < depth))
                    {
                        throw new Exception();
                    }
                }
            this.recordLower = recordLower;
            this.recordUpper = recordUpper;
            this.IsFirst = isfirst;
        }
        public void Clear()
        {
            data = null;
            flag = null;
            spanlist = null;
        }
    }
    class L_SpanFillBase
    {
        public L_SpanFillBase()
        {
            container = new Container_Stack<Span>();
        }
        protected BitMap3d data;
        protected FlagMap3d flagsMap;
        protected Container<Span> container;//以Span为单位的Queue或Stack容器
        //protected List<Int16Triple> result = new List<Int16Triple>();
        protected int resultCount = 0;
        public virtual SpanFillResult ExecuteSeededGrow(SpanFillInput input)
        {
            this.data = new BitMap3d(input.data, input.width, input.height, input.depth);
            flagsMap = input.flag;
            //result.Clear()
            resultCount = 0;
            container.Clear();
            SpanFillResult ret = new SpanFillResult();
            ret.Init();
            if (input.IsFirst)
            {
                Int16Triple seed = input.seed;
                Process(seed);
                flagsMap.SetFlagOn(seed.X, seed.Y, seed.Z, true);
                Span seedspan = new Span();
                seedspan.XLeft = seed.X;
                seedspan.XRight = seed.X;
                seedspan.Y = seed.Y;
                seedspan.Z = seed.Z;
                seedspan.ParentDirection = ParentDirections.Non;
                seedspan.Extended = ExtendTypes.UnRez;
                container.Push(seedspan);
            }
            else
            {
                ParentDirections p = (input.spanlist[0].Z == 0) ? ParentDirections.Z0 : ParentDirections.Z2;
                for (int i = 0; i < input.spanlist.Count; i++)
                {
                    Range r = input.spanlist[i];
                    CheckRange(r.XLeft, r.XRight, r.Y, r.Z, p);
                }
            }

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
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
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
                        if (span.Y + 1 < data.height)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        }
                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)
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
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        }
                        else
                        {
                            if (xl != span.XLeft)
                                OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                            if (span.XRight != xr)
                                OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        }
                        else
                        {
                            if (xl != span.XLeft)
                                OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
                            if (span.XRight != xr)
                                OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
                        }

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);

                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Non)
                    {
                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
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

                        if (span.Y + 1 < data.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
                        else if (xl != span.XLeft) OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
                        else if (xl != span.XLeft) OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
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

                        if (span.Y + 1 < data.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Y - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else if (span.XRight != xr) OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

                        if (span.Z - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else if (span.XRight != xr) OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
            //ret.resultPointSet = this.result;
            ret.resultCount = this.resultCount;
            return ret;
        }
        protected void CheckRange(int xleft, int xright, int y, int z, ParentDirections ptype)
        {
            for (int i = xleft; i <= xright; )
            {
                if ((!flagsMap.GetFlagOn(i, y, z)) && IncludeConditionMeets(i, y, z))
                {
                    int lb = i;
                    int rb = i + 1;
                    while (rb <= xright && (!flagsMap.GetFlagOn(rb, y, z)) && IncludeConditionMeets(rb, y, z))
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
                        flagsMap.SetFlagOn(j, y, z, true);
                        Process(new Int16Triple(j, y, z));
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
                if (xright == data.width || flagsMap.GetFlagOn(xright, y, z))
                {
                    break;
                }
                else
                {
                    if (IncludeConditionMeets(xright, y, z))
                    {
                        Int16Triple t = new Int16Triple(xright, y, z);
                        flagsMap.SetFlagOn(xright, y, z, true);
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
        protected int FindXLeft(int x, int y, int z)
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
                    if (IncludeConditionMeets(xleft, y, z))
                    {
                        Int16Triple t = new Int16Triple(xleft, y, z);
                        flagsMap.SetFlagOn(xleft, y, z, true);
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
        protected virtual bool IncludeConditionMeets(int x, int y, int z)
        {
            throw new Exception();
            //byte v = bmp.GetPixel(x, y, z);
            //return v > min && v < max;
        }
        protected virtual void Process(Int16Triple p)
        {
            resultCount++;
            // result.Add(p);
        }
        protected void OnOverstepSpanFound(int xleft, int xright, int y, int z, ParentDirections ptype, ref SpanFillResult ret)
        {
            if (z == -1)
            {
                Range r = new Range(xleft, xright, y, z);
                ret.boundaryRequestPoints[0].Add(r);
            }
            else if (z == data.depth)
            {
                Range r = new Range(xleft, xright, y, z);
                ret.boundaryRequestPoints[1].Add(r);
            }
            else
                throw new Exception();
        }
    }
  
}
