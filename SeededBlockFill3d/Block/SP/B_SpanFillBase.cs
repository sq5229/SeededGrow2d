using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeededBlockFill3d.Block.SP
{
    public enum ParentDirections
    {
        Y0 = 1, Y2 = 2, Z0 = 3, Z2 = 4, Non = 5
    }
    public enum ExtendTypes
    {
        LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
    }
    public struct Span
    {
        public int XLeft;
        public int XRight;
        public int Y;
        public int Z;
        public ExtendTypes Extended;
        public ParentDirections ParentDirection;
    }
    public struct Range
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
    public class SpanFillResult
    {
        public List<Int16Triple> resultSet;
        public int resultCount;
        public List<Range>[] boundaryRanges_YZ;
        public List<Int16Triple>[] boundaryPoints_X;
        public SpanFillResult()
        {
            boundaryRanges_YZ = new List<Range>[4];
            for (int i = 0; i < 4; i++)
            {
                boundaryRanges_YZ[i] = new List<Range>();
            }
            boundaryPoints_X = new List<Int16Triple>[2];
            for (int i = 0; i < 2; i++)
            {
                boundaryPoints_X[i] = new List<Int16Triple>();
            }
            resultSet = new List<Int16Triple>();
            resultCount = 0;
        }
        public void AddPoint(Int16Triple p)
        {
            resultSet.Add(p);
            resultCount++;
        }
        public bool GetNeedsToSeekYZ(int adjindex)
        {
            return boundaryRanges_YZ[adjindex].Count > 0;
        }
        public bool GetNeedsToSeekX(int adjindex)
        {
            return boundaryPoints_X[adjindex].Count > 0;
        }
        public void RemovePointer()
        {
            for (int i = 0; i < 4; i++)
            {
                boundaryRanges_YZ[i] = null;
                boundaryRanges_YZ[i] = new List<Range>();
            }
            if (resultSet == null)
                resultSet = new List<Int16Triple>();
            else
                resultSet.Clear();
        }
        public void Clear()
        {
            resultSet = null;
            boundaryRanges_YZ[0] = null;
            boundaryRanges_YZ[1] = null;
            boundaryRanges_YZ[2] = null;
            boundaryRanges_YZ[3] = null;
            boundaryPoints_X[0] = null;
            boundaryPoints_X[1] = null;
        }
    }
    public class SpanFillInput
    {
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public List<Int16Triple>[] overstepPointList;
        public List<Range>[] overstepRangeList;
        public Int16Triple seed;
        public BoundaryRecordMask mask;
        public FlagMap3d flagsMap;
        public bool GetIsFirst()
        {
            return seed.X != -1;
        }
        public SpanFillInput()
        {
            this.data = null;
            this.width = -1;
            this.height = -1;
            this.depth = -1;
            this.overstepPointList = null;
            this.overstepRangeList = null;
            this.seed = new Int16Triple(-1, -1, -1);
            this.mask = null;
        }
        public void ClearAll()
        {
            data = null;
            width = -1;
            height = -1;
            depth = -1;
            data = null;
            seed = new Int16Triple(-1, -1, -1);
            mask = null;
            if(overstepPointList!=null)
                for (int i = 0; i < 2; i++)
                {
                    if (overstepPointList[i] != null)
                        overstepPointList[i].Clear();
                }
            if(overstepRangeList!=null)
                for (int i = 0; i < 4; i++)
                {
                    if (overstepRangeList[i] != null)
                        overstepRangeList[i].Clear();
                }
            overstepPointList = null;
            overstepRangeList = null;
        }
        public bool GetCanSeekAdj(int adjindex)
        {
            return mask.GetAdjNeedsSeek(adjindex);
        }
    }
   public class B_SpanFillBase
    {
        static int[] OppositePositionYZ=new int[4]{1,0,3,2};
        static int[] OppositePositionX = new int[2] { 1, 0};
        static ExtendTypes[] ET = new ExtendTypes[2] {ExtendTypes.RightRequired,ExtendTypes.LeftRequired };
        static ParentDirections[] PT = new ParentDirections[4] {ParentDirections.Y2,ParentDirections.Y0,ParentDirections.Z2,ParentDirections.Z0 };
        public B_SpanFillBase()
        {
            container = new Container_Stack<Span>();
        }
        protected BitMap3d data;
        protected FlagMap3d flagsMap;
        protected Container<Span> container;
        protected SpanFillResult result;
        protected BoundaryRecordMask mask;

        public virtual void ExecuteSeededGrow(SpanFillInput input,SpanFillResult ret)
        {
            this.data = new BitMap3d(input.data, input.width, input.height, input.depth);
            if (input.flagsMap == null)
                this.flagsMap = new FlagMap3d(input.width, input.height, input.depth);
            else
                this.flagsMap = input.flagsMap;
            this.result = ret;
            this.mask = input.mask;

            if (input.GetIsFirst())
            {
                ProcessFirstSeed(input);
            }
            else
            {
                ProcessYZSeedRanges(input);
                ProcessXSeeds(input);
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
                        else OnOverstepY0RangeFound(span.XLeft, span.XRight, span.Y - 1, span.Z);

                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, span.XRight, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, span.XRight, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, span.XRight, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, span.XRight, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)//[spx-spy,y,z+1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, span.XRight, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)//[spx-spy,y-1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(span.XLeft, span.XRight, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)//[spx-spy,y+1,z]
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, span.XRight, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)//[spx-spy,y,z-1]
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, span.XRight, span.Y, span.Z - 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(span.XLeft, span.XRight, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, span.XRight, span.Y + 1, span.Z);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, span.XRight, span.Y, span.Z + 1);
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
                        else OnOverstepY0RangeFound(xl, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        }
                        else
                        {
                            if (xl != span.XLeft)
                                OnOverstepY2RangeFound(xl, span.XLeft, span.Y + 1, span.Z);
                            if (span.XRight != xr)
                                OnOverstepY2RangeFound(span.XRight, xr, span.Y + 1, span.Z);
                        }

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, xr, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, xr, span.Y + 1, span.Z);

                        if (span.Y - 1 >= 0)
                        {
                            if (xl != span.XLeft)
                                CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);
                            if (span.XRight != xr)
                                CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        }
                        else
                        {
                            if (xl != span.XLeft)
                                OnOverstepY0RangeFound(xl, span.XLeft, span.Y - 1, span.Z);
                            if (span.XRight != xr)
                                OnOverstepY0RangeFound(span.XRight, xr, span.Y - 1, span.Z);
                        }

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, xr, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(xl, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, xr, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, xr, span.Y, span.Z - 1);

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
                                OnOverstepZ2RangeFound(xl, span.XLeft, span.Y, span.Z + 1);
                            if (span.XRight != xr)
                                OnOverstepZ2RangeFound(span.XRight, xr, span.Y, span.Z + 1);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(xl, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, xr, span.Y + 1, span.Z);

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
                                OnOverstepZ0RangeFound(xl, span.XLeft, span.Y, span.Z - 1);
                            if (span.XRight != xr)
                                OnOverstepZ0RangeFound(span.XRight, xr, span.Y, span.Z - 1);
                        }

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, xr, span.Y, span.Z + 1);

                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Non)
                    {
                        if (span.Y + 1 < data.height)
                            CheckRange(xl, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, xr, span.Y + 1, span.Z);

                        if (span.Y - 1 >= 0)
                            CheckRange(xl, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(xl, xr, span.Y - 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, xr, span.Y, span.Z + 1);
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
                        else OnOverstepY0RangeFound(xl, span.XRight, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y + 1, span.Z, ParentDirections.Y0);
                        else if (xl != span.XLeft) OnOverstepY2RangeFound(xl, span.XLeft, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, span.XRight, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, span.XRight, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y - 1, span.Z, ParentDirections.Y2);
                        else if (xl != span.XLeft) OnOverstepY0RangeFound(xl, span.XLeft, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, span.XRight, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, span.XRight, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, span.XRight, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(xl, span.XRight, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, span.XRight, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(xl, span.XRight, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z + 1, ParentDirections.Z0);
                        else if (xl != span.XLeft) OnOverstepZ2RangeFound(xl, span.XLeft, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(xl, span.XRight, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(xl, span.XRight, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(xl, span.XRight, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(xl, span.XRight, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0 && xl != span.XLeft)
                            CheckRange(xl, span.XLeft, span.Y, span.Z - 1, ParentDirections.Z2);
                        else if (xl != span.XLeft) OnOverstepZ0RangeFound(xl, span.XLeft, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(xl, span.XRight, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(xl, span.XRight, span.Y, span.Z + 1);
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
                        else OnOverstepY0RangeFound(span.XLeft, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else if (span.XRight != xr) OnOverstepY2RangeFound(span.XRight, xr, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, xr, span.Y, span.Z + 1);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirections.Y0)
                    {
                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, xr, span.Y + 1, span.Z);

                        if (span.Y - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else if (span.XRight != xr) OnOverstepY0RangeFound(span.XRight, xr, span.Y - 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, xr, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z2)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(span.XLeft, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, xr, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else OnOverstepZ0RangeFound(span.XLeft, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else if (span.XRight != xr) OnOverstepZ2RangeFound(span.XRight, xr, span.Y, span.Z + 1);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirections.Z0)
                    {
                        if (span.Y - 1 >= 0)
                            CheckRange(span.XLeft, xr, span.Y - 1, span.Z, ParentDirections.Y2);
                        else OnOverstepY0RangeFound(span.XLeft, xr, span.Y - 1, span.Z);

                        if (span.Y + 1 < data.height)
                            CheckRange(span.XLeft, xr, span.Y + 1, span.Z, ParentDirections.Y0);
                        else OnOverstepY2RangeFound(span.XLeft, xr, span.Y + 1, span.Z);

                        if (span.Z - 1 >= 0 && span.XRight != xr)
                            CheckRange(span.XRight, xr, span.Y, span.Z - 1, ParentDirections.Z2);
                        else if (span.XRight != xr) OnOverstepZ0RangeFound(span.XRight, xr, span.Y, span.Z - 1);

                        if (span.Z + 1 < data.depth)
                            CheckRange(span.XLeft, xr, span.Y, span.Z + 1, ParentDirections.Z0);
                        else OnOverstepZ2RangeFound(span.XLeft, xr, span.Y, span.Z + 1);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
        }

        private void ProcessFirstSeed(SpanFillInput input)
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
        private void ProcessYZSeedRanges(SpanFillInput input)
        {
            for (int k = 0; k < 4; k++)
            {
                List<Range> list = input.overstepRangeList[k];
                for (int i = 0; i < list.Count; i++)
                {
                    Range r = list[i];
                    CheckRange(r.XLeft, r.XRight, r.Y, r.Z, PT[k]);
                }
            }
        }
        private void ProcessXSeeds(SpanFillInput input)
        {
            for (int k = 0; k < 2; k++)
            {
                List<Int16Triple> list = input.overstepPointList[k];
                for (int i = 0; i < list.Count; i++)
                {
                    Int16Triple p = list[i];
                    if (!flagsMap.GetFlagOn(p.X, p.Y, p.Z) && IncludeConditionMeets(p.X, p.Y, p.Z))
                    {
                        Process(p);
                        flagsMap.SetFlagOn(p.X, p.Y, p.Z, true);
                        Span span = new Span();
                        span.XLeft = p.X;
                        span.XRight = p.X;
                        span.Y = p.Y;
                        span.Z = p.Z;
                        span.ParentDirection = ParentDirections.Non;
                        span.Extended = ExtendTypes.UnRez;
                        container.Push(span);
                    }
                }
            }
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
                    if (xright == data.width)
                    {
                        OnOverstepX2PointFound(xright, y, z);
                    }
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
                    if (xleft == -1)
                        OnOverstepX0PointFound(xleft, y, z);
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
        }
        protected virtual void Process(Int16Triple p)
        {
            result.AddPoint(p);
        }

        private void OnOverstepY0RangeFound(int xleft, int xright, int y, int z)
        {
            if (mask.GetAdjNeedsSeek(2))
            {
                Range r = new Range(xleft, xright, y, z);
                result.boundaryRanges_YZ[0].Add(r);
            }
        }
        private void OnOverstepY2RangeFound(int xleft, int xright, int y, int z)
        {
            if (mask.GetAdjNeedsSeek(3))
            {
                Range r = new Range(xleft, xright, y, z);
                result.boundaryRanges_YZ[1].Add(r);
            }
        }
        private void OnOverstepZ0RangeFound(int xleft, int xright, int y, int z)
        {
            if (mask.GetAdjNeedsSeek(4))
            {
                Range r = new Range(xleft, xright, y, z);
                result.boundaryRanges_YZ[2].Add(r);
            }
        }
        private void OnOverstepZ2RangeFound(int xleft, int xright, int y, int z)
        {
            if (mask.GetAdjNeedsSeek(5))
            {
                Range r = new Range(xleft, xright, y, z);
                result.boundaryRanges_YZ[3].Add(r);
            }
        }
        private void OnOverstepX0PointFound(int x, int y, int z)
        {
            if(mask.GetAdjNeedsSeek(0))
                result.boundaryPoints_X[0].Add(new Int16Triple(x, y, z));
        }
        private void OnOverstepX2PointFound(int x, int y, int z)
        {
            if(mask.GetAdjNeedsSeek(1))
                result.boundaryPoints_X[1].Add(new Int16Triple(x, y, z));
        }
    }
}
