using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeededBlockFill3d.Block.FF
{
    public class FloodFillResult
    {
        public FloodFillResult()
        {
            resultSet = new List<Int16Triple>();
            resultCount = 0;
            boundaryRequestPoints = new List<Int16Triple>[6];
            for (int i = 0; i < 6; i++)
            {
                boundaryRequestPoints[i] = new List<Int16Triple>();
            }
        }
        public List<Int16Triple> resultSet;
        public int resultCount;
        public List<Int16Triple>[] boundaryRequestPoints;
        public bool GetNeedsSeekAdj(int index)
        {
            return boundaryRequestPoints[index].Count > 0;
        }
        public void AddPoint(Int16Triple p)
        {
            resultSet.Add(p);
            resultCount++;
        }
        public void ClearResult()
        {
            resultCount = 0;
            resultSet.Clear();
        }
    }
    public class FloodFillInput
    {
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public List<Int16Triple>[] overstepList;
        public Int16Triple seed;
        public BoundaryRecordMask mask;
        public FlagMap3d flagsMap;
        public bool GetIsFirst()
        {
            return seed.X != -1;
        }
        public FloodFillInput()
        {
            this.data = null;
            this.flagsMap = null;
            this.width = -1;
            this.height = -1;
            this.depth = -1;
            this.overstepList = null;
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
            flagsMap = null;
            if (overstepList != null)
            {
                for (int i = 0; i < 6; i++)
                {
                    overstepList[i].Clear();
                }
            }
            overstepList = null;
        }
        public bool GetCanSeekAdj(int adjindex)
        {
            return mask.GetAdjNeedsSeek(adjindex);
        }
    }
    public class B_FloodFillBase
    {
        protected struct Int16TripleWithDirection
        {
            public Int16Triple Poistion;
            public int PDirection;
            public Int16TripleWithDirection(Int16Triple p, int pDirection)
            {
                Poistion = p;
                PDirection = pDirection;
            }
        }
        static int[] OppositePos = new int[7] {1,0,3,2,5,4,6};
        protected int width;
        protected int height;
        protected int depth;
        protected byte[] data;
        protected FlagMap3d flagsMap;
        protected Container_Queue<Int16TripleWithDirection> queue;
        protected FloodFillResult result;
        public B_FloodFillBase()
        {
            queue = new Container_Queue<Int16TripleWithDirection>();
        }
        public void ExecuteSeededGrow(FloodFillInput input,FloodFillResult ret)
        {
            queue.Clear();
            result = ret;
            this.width = input.width;
            this.height = input.height;
            this.depth = input.depth;
            this.data = input.data;
            if(input.flagsMap!=null)
                this.flagsMap = input.flagsMap;
            else
                this.flagsMap = new FlagMap3d(input.width, input.height, input.depth);

            Int16Triple[] adjPoints6 = new Int16Triple[6];
            if (!input.GetIsFirst())
            {
                List<Int16Triple>[] oversteps = input.overstepList;
                for (int j = 0; j < 6; j++)
                {
                    for (int i = 0; i < oversteps[j].Count; i++)
                    {
                        Int16Triple p = oversteps[j][i];
                        if (!flagsMap.GetFlagOn(p.X, p.Y, p.Z) && IncludeConditionMeets(p))
                        {
                            flagsMap.SetFlagOn(p.X, p.Y, p.Z, true);
                            Process(p);
                            queue.Push(new Int16TripleWithDirection(p, j));
                        }
                    }
                }
            }
            else
            {
                Int16Triple seed = input.seed;
                flagsMap.SetFlagOn(seed.X, seed.Y, seed.Z, true);
                queue.Push(new Int16TripleWithDirection(seed,6));
                Process(seed);
            }
            while (!queue.Empty())
            {
                Int16TripleWithDirection p = queue.Pop();
                InitAdj6(adjPoints6, p.Poistion);
                for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                {
                    if (adjIndex == p.PDirection)
                        continue;
                    Int16Triple t = adjPoints6[adjIndex];
                    if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                    {
                        int indext = t.X + width * t.Y + width * height * t.Z;
                        if (!flagsMap.GetFlagOn(t.X, t.Y, t.Z) && IncludeConditionMeets(t))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y, t.Z, true);
                            queue.Push(new Int16TripleWithDirection(t,OppositePos[adjIndex]));
                            Process(t);
                        }
                    }
                    else
                    {
                        if(input.GetCanSeekAdj(adjIndex))
                            result.boundaryRequestPoints[adjIndex].Add(t);
                    }
                }
            }
        }
        protected virtual void Process(Int16Triple t)
        {
            result.AddPoint(t);
        }
        protected virtual bool IncludeConditionMeets(Int16Triple t)
        {
            throw new Exception();
        }
        private void InitAdj6(Int16Triple[] adjPoints6, Int16Triple p)
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
    }
}
