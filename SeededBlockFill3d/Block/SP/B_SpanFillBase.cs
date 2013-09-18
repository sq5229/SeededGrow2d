//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace SeededBlockFill3d.Block.FF
//{
//    enum ParentDirections
//    {
//        Y0 = 1, Y2 = 2, Z0 = 3, Z2 = 4, Non = 5
//    }
//    enum ExtendTypes
//    {
//        LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
//    }
//    struct Span
//    {
//        public int XLeft;
//        public int XRight;
//        public int Y;
//        public int Z;
//        public ExtendTypes Extended;
//        public ParentDirections ParentDirection;
//    }
//    struct Range
//    {
//        public int XLeft;
//        public int XRight;
//        public int Y;
//        public int Z;
//        public Range(int xleft, int xright, int y, int z)
//        {
//            this.XLeft = xleft;
//            this.XRight = xright;
//            this.Y = y;
//            this.Z = z;
//        }
//    }
//    struct SpanFillResult
//    {
//        public List<Int16Triple> resultSet;
//        public int resultCount;
//        public List<Range>[] boundaryRanges_YZ;
//        public List<Int16Triple>[] boundaryPoints_X;
//        public void AddPoint(Int16Triple p)
//        {
//            resultSet.Add(p);
//            resultCount++;
//        }
//        public bool GetNeedsSeekY0()
//        {
//            return boundaryRanges_YZ[0].Count > 0;
//        }
//        public bool GetNeedsSeekY2()
//        {
//            return boundaryRanges_YZ[1].Count > 0;
//        }
//        public bool GetNeedsSeekZ0()
//        {
//            return boundaryRanges_YZ[2].Count > 0;
//        }
//        public bool GetNeedsSeekZ2()
//        {
//            return boundaryRanges_YZ[3].Count > 0;
//        }
//        public bool GetNeedsSeekX0()
//        {
//            return boundaryPoints_X[0].Count > 0;
//        }
//        public bool GetNeedsSeekX2()
//        {
//            return boundaryPoints_X[1].Count > 0;
//        }
//        public void Init()
//        {
//            boundaryRanges_YZ = new List<Range>[2];
//            for (int i = 0; i < 2; i++)
//            {
//                boundaryRanges_YZ[i] = new List<Range>();
//            }
//            resultSet = new List<Int16Triple>();
//        }
//        public void RemovePointer()
//        {
//            for (int i = 0; i < 4; i++)
//            {
//                boundaryRanges_YZ[i] = null;
//                boundaryRanges_YZ[i] = new List<Range>();
//            }
//            if (resultSet == null)
//                resultSet = new List<Int16Triple>();
//            else
//                resultSet.Clear();
//        }
//        public void Clear()
//        {
//            resultSet = null;
//            boundaryRanges_YZ[0] = null;
//            boundaryRanges_YZ[1] = null;
//            boundaryRanges_YZ[2] = null;
//            boundaryRanges_YZ[3] = null;
//            boundaryPoints_X[0] = null;
//            boundaryPoints_X[1] = null;

//        }
//    }
//    struct SpanFillInput
//    {
//        public byte[] data;
//        public int width;
//        public int height;
//        public int depth;
//        public List<Int16Triple> overstepPointList;
//        public List<Range> overstepRangeList;
//        public Int16Triple seed;
//        public BoundaryRecordMask mask;
//        public bool GetIsFirst()
//        {
//            return seed.X == -1;
//        }
//        public SpanFillInput(byte[] data, int width, int height, int depth, List<Int16Triple> osPoints,List<Range> osRanges,BoundaryRecordMask mask)
//        {
//            this.data = data;
//            this.width = width;
//            this.height = height;
//            this.depth = depth;
//            this.overstepPointList = osPoints;
//            this.overstepRangeList = osRanges;
//            this.seed = new Int16Triple(-1,-1,-1);
//            this.mask = mask;
//            for (int i = 0; i < overstepRangeList.Count; i++)
//            {
//                if (!(overstepRangeList[i].Y >= 0 && overstepRangeList[i].Y < height && overstepRangeList[i].Z >= 0 && overstepRangeList[i].Z < depth))
//                {
//                    throw new Exception();
//                }
//            }
//            for (int i = 0; i < overstepPointList.Count; i++)
//            {
//                if (!(overstepPointList[i].Y >= 0 && overstepPointList[i].Y < height && overstepPointList[i].Z >= 0 && overstepPointList[i].Z < depth && overstepPointList[i].X >= 0 && overstepPointList[i].X<width))
//                {
//                    throw new Exception();
//                }
//            }
            
//        }
//        public SpanFillInput(byte[] data, int width, int height, int depth, Int16Triple seed, BoundaryRecordMask mask)
//        {
//            this.data = data;
//            this.width = width;
//            this.height = height;
//            this.depth = depth;
//            this.seed = seed;
//            this.mask = mask;
//            this.overstepPointList = null;
//            this.overstepRangeList = null;
//        }
//    }
//    class B_SpanFillBase
//    {
//        public B_SpanFillBase()
//        {
//            container = new Container_Stack<Span>();
//            result.Init();
//        }
//        public void LoadFlagsMap(FlagMap3d flags)
//        {
//            if (flags != null)
//                flagsMap = flags;
//            else if (flagsMap == null)
//                this.flagsMap = new FlagMap3d(this.data.width, this.data.height, this.data.depth);
//            else
//                flagsMap.ClearMap();
//        }
//        protected BitMap3d data;
//        protected FlagMap3d flagsMap;
//        protected Container<Span> container;
//        protected SpanFillResult result;

//        public virtual SpanFillResult ExecuteSeededGrow(SpanFillInput input)
//        {
//            this.data = new BitMap3d(input.data, input.width, input.height, input.depth);
//            result.RemovePointer();
//            if (input.GetIsFirst())
//            {
//                Int16Triple seed = input.seed;
//                Process(seed);
//                flagsMap.SetFlagOn(seed.X, seed.Y, seed.Z, true);
//                Span seedspan = new Span();
//                seedspan.XLeft = seed.X;
//                seedspan.XRight = seed.X;
//                seedspan.Y = seed.Y;
//                seedspan.Z = seed.Z;
//                seedspan.ParentDirection = ParentDirections.Non;
//                seedspan.Extended = ExtendTypes.UnRez;
//                container.Push(seedspan);
//            }
//            else
//            {
//                for (int i = 0; i < input.overstepPointList.Count; i++)
//                {
//                    Int16Triple p=input.overstepPointList[i];
//                    if (!flagsMap.GetFlagOn(p.X, p.Y, p.Z) && IncludeConditionMeets(p.X, p.Y, p.Z))
//                    {
//                        Process(p);
//                        flagsMap.SetFlagOn(p.X, p.Y, p.Z, true);
//                        Span span = new Span();
//                        span.XLeft = p.X;
//                        span.XRight = p.X;
//                        span.Y = p.Y;
//                        span.Z = p.Z;
//                        span.ParentDirection = ParentDirections.Non;
//                        if(p.X==0)
//                        {
//                            span.Extended = ExtendTypes.RightRequired;
//                        }
//                        else if(p.X==data.width-1)
//                        {
//                            span.Extended = ExtendTypes.LeftRequired;
//                        }
//                        else throw new Exception();
//                        container.Push(span);
//                    }
//                }
//                for (int i = 0; i < input.overstepRangeList.Count; i++)
//                {
//                    Range r = input.overstepRangeList[i];
//                }
//                //ParentDirections p = (input.spanlist[0].Z == 0) ? ParentDirections.Z0 : ParentDirections.Z2;
//                //for (int i = 0; i < input.spanlist.Count; i++)
//                //{
//                //    Range r = input.spanlist[i];
//                //    CheckRange(r.XLeft, r.XRight, r.Y, r.Z, p);
//                //}
//            }

//            while (!container.Empty())
//            {
//                Span span = container.Pop();
//                #region AllRez
//                if (span.Extended == ExtendTypes.AllRez)
//                {
//                    if (span.ParentDirection == ParentDirections.Y2)
//                    {
//                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
//                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Y0)
//                    {
//                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
//                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z2)
//                    {
//                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
//                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
//                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z0)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    throw new Exception();
//                }
//                #endregion
//                #region UnRez
//                if (span.Extended == ExtendTypes.UnRez)
//                {
//                    int xl = FindXLeft(span.XLeft, span.Y, span.Z);
//                    int xr = FindXRight(span.XRight, span.Y, span.Z);
//                    if (span.ParentDirection == ParentDirections.Y2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
//                        if (span.Y + 1 < data.height)
//                        {
//                            if (xl != span.XLeft)
//                                CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
//                            if (span.XRight != xr)
//                                CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
//                        }
//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Y0)
//                    {
//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Y - 1 >= 0)
//                        {
//                            if (xl != span.XLeft)
//                                CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);
//                            if (span.XRight != xr)
//                                CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);
//                        }

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                        {
//                            if (xl != span.XLeft)
//                                CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
//                            if (span.XRight != xr)
//                                CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        }
//                        else
//                        {
//                            if (xl != span.XLeft)
//                                OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                            if (span.XRight != xr)
//                                OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        }
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z0)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                        {
//                            if (xl != span.XLeft)
//                                CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
//                            if (span.XRight != xr)
//                                CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        }
//                        else
//                        {
//                            if (xl != span.XLeft)
//                                OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
//                            if (span.XRight != xr)
//                                OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);
//                        }

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);

//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Non)
//                    {
//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    throw new Exception();
//                }
//                #endregion
//                #region LeftRequired
//                if (span.Extended == ExtendTypes.LeftRequired)
//                {
//                    int xl = FindXLeft(span.XLeft, span.Y, span.Z);
//                    if (span.ParentDirection == ParentDirections.Y2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height && xl != span.XLeft)
//                            CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Y0)
//                    {
//                        if (span.Y - 1 >= 0 && xl != span.XLeft)
//                            CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth && xl != span.XLeft)
//                            CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else if (xl != span.XLeft) OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z0)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0 && xl != span.XLeft)
//                            CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else if (xl != span.XLeft) OnOverstepSpanFound(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    throw new Exception();
//                }
//                #endregion
//                #region RightRequired
//                if (span.Extended == ExtendTypes.RightRequired)
//                {
//                    int xr = FindXRight(span.XRight, span.Y, span.Z);

//                    if (span.ParentDirection == ParentDirections.Y2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height && span.XRight != xr)
//                            CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }

//                    if (span.ParentDirection == ParentDirections.Y0)
//                    {
//                        if (span.Y + 1 < data.height)
//                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Y - 1 >= 0 && span.XRight != xr)
//                            CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z2)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth && span.XRight != xr)
//                            CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else if (span.XRight != xr) OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    if (span.ParentDirection == ParentDirections.Z0)
//                    {
//                        if (span.Y - 1 >= 0)
//                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);

//                        if (span.Y + 1 < data.height)
//                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);

//                        if (span.Z - 1 >= 0 && span.XRight != xr)
//                            CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
//                        else if (span.XRight != xr) OnOverstepSpanFound(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2, ref ret);

//                        if (span.Z + 1 < data.depth)
//                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
//                        else OnOverstepSpanFound(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0, ref ret);
//                        continue;
//                    }
//                    throw new Exception();
//                }
//                #endregion
//            }
//            return ret;
//        }
//        protected void CheckRange(int xleft, int xright, int y, int z, ParentDirections ptype)
//        {
//            for (int i = xleft; i <= xright; )
//            {
//                if ((!flagsMap.GetFlagOn(i, y, z)) && IncludeConditionMeets(i, y, z))
//                {
//                    int lb = i;
//                    int rb = i + 1;
//                    while (rb <= xright && (!flagsMap.GetFlagOn(rb, y, z)) && IncludeConditionMeets(rb, y, z))
//                    {
//                        rb++;
//                    }
//                    rb--;

//                    Span span = new Span();
//                    span.XLeft = lb;
//                    span.XRight = rb;
//                    span.Y = y;
//                    span.Z = z;
//                    if (lb == xleft && rb == xright)
//                    {
//                        span.Extended = ExtendTypes.UnRez;
//                    }
//                    else if (rb == xright)
//                    {
//                        span.Extended = ExtendTypes.RightRequired;
//                    }
//                    else if (lb == xleft)
//                    {
//                        span.Extended = ExtendTypes.LeftRequired;
//                    }
//                    else
//                    {
//                        span.Extended = ExtendTypes.AllRez;
//                    }
//                    span.ParentDirection = ptype;
//                    for (int j = lb; j <= rb; j++)
//                    {
//                        flagsMap.SetFlagOn(j, y, z, true);
//                        Process(new Int16Triple(j, y, z));
//                    }
//                    container.Push(span);

//                    i = rb + 1;
//                }
//                else
//                {
//                    i++;
//                }
//            }
//        }//区段法的CheckRange 注意与扫描线的CheckRange的不同
//        protected int FindXRight(int x, int y, int z)
//        {
//            int xright = x + 1;
//            while (true)
//            {
//                if (xright == data.width || flagsMap.GetFlagOn(xright, y, z))
//                {
//                    if (xright == data.width)
//                    {
//                        OnOverstepX2PointFound(xright, y, z);
//                    }
//                    break;
//                }
//                else
//                {
//                    if (IncludeConditionMeets(xright, y, z))
//                    {
//                        Int16Triple t = new Int16Triple(xright, y, z);
//                        flagsMap.SetFlagOn(xright, y, z, true);
//                        Process(t);
//                        xright++;
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }
//            }
//            return xright - 1;
//        }
//        protected int FindXLeft(int x, int y, int z)
//        {
//            int xleft = x - 1;
//            while (true)
//            {
//                if (xleft == -1 || flagsMap.GetFlagOn(xleft, y, z))
//                {
//                    if (xleft == -1)
//                        OnOverstepX0PointFound(x, y, z);
//                    break;
//                }
//                else
//                {
//                    if (IncludeConditionMeets(xleft, y, z))
//                    {
//                        Int16Triple t = new Int16Triple(xleft, y, z);
//                        flagsMap.SetFlagOn(xleft, y, z, true);
//                        Process(t);
//                        xleft--;
//                    }
//                    else
//                    {
//                        break;
//                    }
//                }
//            }
//            return xleft + 1;
//        }
//        protected virtual bool IncludeConditionMeets(int x, int y, int z)
//        {
//            throw new Exception();
//        }
//        protected virtual void Process(Int16Triple p)
//        {
//            result.AddPoint(p);
//        }

//        private void OnOverstepY0RangeFound(int xleft, int xright, int y, int z)
//        {
//            Range r = new Range(xleft, xright, y, z);
//            result.boundaryRanges_YZ[0].Add(r);
//        }
//        private void OnOverstepY2RangeFound(int xleft, int xright, int y, int z)
//        {
//            Range r = new Range(xleft, xright, y, z);
//            result.boundaryRanges_YZ[1].Add(r);
//        }
//        private void OnOverstepZ0RangeFound(int xleft, int xright, int y, int z)
//        {
//            Range r = new Range(xleft, xright, y, z);
//            result.boundaryRanges_YZ[2].Add(r);
//        }
//        private void OnOverstepZ2RangeFound(int xleft, int xright, int y, int z)
//        {
//            Range r = new Range(xleft, xright, y, z);
//            result.boundaryRanges_YZ[3].Add(r);
//        }
//        private void OnOverstepX0PointFound(int x, int y, int z)
//        {
//            result.boundaryPoints_X[0].Add(new Int16Triple(x, y, z));
//        }
//        private void OnOverstepX2PointFound(int x, int y, int z)
//        {
//            result.boundaryPoints_X[1].Add(new Int16Triple(x, y, z));
//        }
//        //protected void OnOverstepSpanFound(int xleft, int xright, int y, int z, ParentDirections ptype, ref SpanFillResult ret)
//        //{
//        //    if (z == -1)
//        //    {
//        //        Range r = new Range(xleft, xright, y, z);
//        //        ret.boundaryRequestPoints[0].Add(r);
//        //    }
//        //    else if (z == data.depth)
//        //    {
//        //        Range r = new Range(xleft, xright, y, z);
//        //        ret.boundaryRequestPoints[1].Add(r);
//        //    }
//        //    else
//        //        throw new Exception();
//        //}
//    }
//}
