using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d
{
    public struct SeededGrowResult
    {
        public List<Int16Triple> resultPointSet;
        public List<Int16Triple>[] boundaryRequestPoints;
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
            boundaryRequestPoints = new List<Int16Triple>[2];
            for (int i = 0; i < 2; i++)
            {
                boundaryRequestPoints[i] = new List<Int16Triple>();
            }
            resultPointSet = new List<Int16Triple>();
        }
        public void Clear()
        {
            resultPointSet = null;
            boundaryRequestPoints[0] = null;
            boundaryRequestPoints[1] = null;
        }
    }
    public struct SeededGrowInput
    {
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public BitArray flag;
        public List<Int16Triple> seedlist;
        public bool recordUpper;
        public bool recordLower;
        public bool IsFirst;
        public SeededGrowInput(byte[] data, int width, int height, int depth, BitArray flag, List<Int16Triple> seedList, bool recordUpper, bool recordLower,bool isfirst)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.flag = flag;
            if (flag.Length != width * height * depth)
                throw new Exception();
            this.seedlist = seedList;
            for (int i = 0; i < seedlist.Count; i++)
            {
                if (!(seedlist[i].X >= 0 && seedlist[i].X < width && seedlist[i].Y >= 0 && seedlist[i].Y < height && seedlist[i].Z >= 0 && seedlist[i].Z < depth))
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
            seedlist = null;
        }
    }
    public class SeededGrowBase
    {
        protected int width;
        protected int height;
        protected int depth;
        protected byte[] data;
        protected BitArray flagsMap;
        protected Container_Stack<Int16Triple> queue;
        protected List<Int16Triple> result = new List<Int16Triple>();
        public SeededGrowBase()
        {
            queue = new Container_Stack<Int16Triple>();
        }
        public SeededGrowResult ExecuteSeededGrow(SeededGrowInput input)
        {
            queue.Clear();
            result.Clear();
            this.width = input.width;
            this.height = input.height;
            this.depth = input.depth;
            this.flagsMap = input.flag;
            this.data = input.data;
            List<Int16Triple> seeds = input.seedlist;
            SeededGrowResult ret = new SeededGrowResult();
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            if (!input.IsFirst)
            {
                for (int i = 0; i < seeds.Count; i++)
                {
                    int index = seeds[i].X + seeds[i].Y * width + seeds[i].Z * width * height;
                    if (!flagsMap[index] && IncludeConditionMeets(seeds[i]))
                    {
                        flagsMap.Set(index, true);
                        OnRegionPointFind(seeds[i]);
                        InitAdj6(adjPoints6, seeds[i]);
                        for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                        {
                            Int16Triple t = adjPoints6[adjIndex];
                            if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                            {
                                int indext = t.X + width * t.Y + width * height * t.Z;
                                if (!flagsMap[indext] && IncludeConditionMeets(t))
                                {
                                    flagsMap.Set(indext, true);
                                    queue.Push(t);
                                    OnRegionPointFind(t);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (seeds.Count != 1) { throw new Exception(); }
                for (int i = 0; i < seeds.Count; i++)
                {
                    int indext = seeds[i].X + seeds[i].Y * width + seeds[i].Z * width * height;
                    if (!flagsMap[indext] && IncludeConditionMeets(seeds[i]))
                    {
                        flagsMap.Set(indext, true);
                        queue.Push(seeds[i]);
                        OnRegionPointFind(seeds[i]);
                    }
                }
            }
            while (!queue.Empty())
            {
                Int16Triple p = queue.Pop();
                InitAdj6(adjPoints6, p);
                for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                {
                    Int16Triple t = adjPoints6[adjIndex];
                    if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                    {
                        int indext = t.X + width * t.Y + width * height * t.Z;
                        if (!flagsMap[indext] && IncludeConditionMeets(t))
                        {
                            flagsMap.Set(indext, true);
                            queue.Push(t);
                            OnRegionPointFind(t);
                        }
                    }
                    else
                    {
                        if (input.recordLower && t.Z < 0)
                        {
                            if (t.Z != -1) { throw new Exception(); }
                            ret.boundaryRequestPoints[0].Add(t);
                            continue;
                        }
                        if (input.recordUpper && t.Z >= depth)
                        {
                            if (t.Z > depth) { throw new Exception(); }
                            ret.boundaryRequestPoints[1].Add(t);
                            continue;
                        }
                    }
                }
            }
            ret.resultPointSet = this.result;
            return ret;
        }
        public virtual void OnRegionPointFind(Int16Triple t)
        {
            result.Add(t);
        }
        public virtual bool IncludeConditionMeets(Int16Triple t)
        {
            throw new Exception();
        }
        protected void InitAdj6(Int16Triple[] adjPoints6, Int16Triple p)
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
