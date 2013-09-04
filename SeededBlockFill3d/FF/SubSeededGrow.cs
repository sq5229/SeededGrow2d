using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d
{
    public class SeededGrowResult
    {
        public SeededGrowResult()
        {
            boundaryRequestPoints = new List<Int16Triple>[6];
            for (int i = 0; i < 6; i++)
            {
                boundaryRequestPoints[i] = new List<Int16Triple>();
            }
        }
        public List<Int16Triple>[] boundaryRequestPoints;
    }
    public class SeededGrowInput
    {
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public BitArray flag;
        public List<Int16Triple> seedlist;
    }
    public class SubSeededGrow
    {
        int width;
        int height;
        int depth;
        byte[] data;
        BitArray flagsMap;
        Container_Stack<Int16Triple> queue;
        public SubSeededGrow()
        {
            queue = new Container_Stack<Int16Triple>();
        }
        public SeededGrowResult ExecuteSeededGrow(SeededGrowInput input)
        {
            this.width = input.width;
            this.height = input.height;
            this.depth = input.depth;
            this.flagsMap = input.flag;
            this.data = input.data;
            List<Int16Triple> seeds = input.seedlist;
            SeededGrowResult ret = new SeededGrowResult();
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            #region Process Seeds
            for (int i = 0; i < seeds.Count; i++)
            {
                if (IncludeConditionMeets(seeds[i]))
                {
                    int indext = seeds[i].X + seeds[i].Y * width + seeds[i].Z * width * height;
                    flagsMap.Set(indext, true);
                    OnRegionPointFind(seeds[i]);
                }
            }
            #endregion
            #region Process SeedAdj
            for (int i = 0; i < seeds.Count; i++)
            {
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
            #endregion
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
                        if (t.X < 0 && t.X + block.stx >= 0)
                        {
                            ret.boundaryRequestPoints[3].Add(t);
                            continue;
                        }
                        if (t.X >= width && t.X + block.stx < block.AllWidth)
                        {
                            ret.boundaryRequestPoints[2].Add(t);
                            continue;
                        }
                        if (t.Y < 0 && t.Y + block.sty >= 0)
                        {
                            ret.boundaryRequestPoints[1].Add(t);
                            continue;
                        }
                        if (t.Y >= height && t.Y + block.sty < block.AllHeight)
                        {
                            ret.boundaryRequestPoints[0].Add(t);
                            continue;
                        }
                        if (t.Z < 0 && t.Z + block.stz >= 0)
                        {
                            ret.boundaryRequestPoints[5].Add(t);
                            continue;
                        }
                        if (t.Z >= depth && t.Z + block.stz < block.AllDepth)
                        {
                            ret.boundaryRequestPoints[4].Add(t);
                            continue;
                        }
                    }
                }
            }

            return null;
        }
        public virtual void OnRegionPointFind(Int16Triple t)
        {

        }
        public virtual bool IncludeConditionMeets(Int16Triple t)
        {

            return true;
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
