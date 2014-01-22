using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Layer.FF
{
public struct FloodFillResult
{
    //public List<Int16Triple> resultPointSet;
    public int resultCount;
    public List<Int16Triple>[] boundaryRequestPoints;//存储越界点的列表 上越界点存在第0个，下越界点存第1个
    public bool GetNeedsSeekLower()
    {
        return boundaryRequestPoints[0].Count > 0;
    }//返回是否需要往上层找
    public bool GetNeedsSeekUpper()
    {
        return boundaryRequestPoints[1].Count > 0;
    }//返回是否需要往下层找
    public void Init()
    {
        boundaryRequestPoints = new List<Int16Triple>[2];
        for (int i = 0; i < 2; i++)
        {
            boundaryRequestPoints[i] = new List<Int16Triple>();
        }
        //resultPointSet = new List<Int16Triple>();
        resultCount = 0;
    }
    public void Clear()
    {
        //r//esultPointSet = 0;
        boundaryRequestPoints[0] = null;
        boundaryRequestPoints[1] = null;
    }
}
public struct FloodFillInput
{
    public byte[] data;
    public int width;
    public int height;
    public int depth;
    public FlagMap3d flag;
    public List<Int16Triple> overstepList;
    public bool recordUpper;
    public bool recordLower;
    public bool IsFirst;
    public FloodFillInput(byte[] data, int width, int height, int depth, FlagMap3d flag, List<Int16Triple> overstepList, bool recordUpper, bool recordLower, bool isfirst)
    {
        this.data = data;
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.flag = flag;
        this.overstepList = overstepList;
        for (int i = 0; i < overstepList.Count; i++)
        {
            if (!(overstepList[i].X >= 0 && overstepList[i].X < width && overstepList[i].Y >= 0 && overstepList[i].Y < height && overstepList[i].Z >= 0 && overstepList[i].Z < depth))
            {
                throw new Exception();
            }
        }//确保越界点都确实在这层里
        this.recordLower = recordLower;
        this.recordUpper = recordUpper;
        this.IsFirst = isfirst;
    }
    public void Clear()
    {
        data = null;
        flag = null;
        overstepList = null;
    }
}//存储输入参数
public class L_FloodFillBase
{
    protected int width;
    protected int height;
    protected int depth;
    protected byte[] data;
    protected FlagMap3d flagsMap;
    protected Container_Queue<Int16Triple> queue;
    //protected List<Int16Triple> result = new List<Int16Triple>();
    protected int resultCount = 0;
    public L_FloodFillBase()
    {
        queue = new Container_Queue<Int16Triple>();
    }
    public FloodFillResult ExecuteSeededGrow(FloodFillInput input)
    {
        queue.Clear();
        //result.Clear();
        resultCount = 0;
        this.width = input.width;
        this.height = input.height;
        this.depth = input.depth;
        this.flagsMap = input.flag;
        this.data = input.data;
        List<Int16Triple> oversteps = input.overstepList;
        FloodFillResult ret = new FloodFillResult();
        ret.Init();
        Int16Triple[] adjPoints6 = new Int16Triple[6];
        if (!input.IsFirst)
        {
            for (int i = 0; i < oversteps.Count; i++)
            {
                if (!flagsMap.GetFlagOn(oversteps[i].X, oversteps[i].Y, oversteps[i].Z) && IncludeConditionMeets(oversteps[i]))
                {
                    flagsMap.SetFlagOn(oversteps[i].X, oversteps[i].Y, oversteps[i].Z, true);
                    Process(oversteps[i]);
                    InitAdj6(adjPoints6, oversteps[i]);
                    for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                    {
                        Int16Triple t = adjPoints6[adjIndex];
                        if (t.X < width && t.X >= 0 && t.Y < height && t.Y >= 0 && t.Z < depth && t.Z >= 0)
                        {
                            int indext = t.X + width * t.Y + width * height * t.Z;
                            if (!flagsMap.GetFlagOn(t.X ,t.Y ,t.Z) && IncludeConditionMeets(t))
                            {
                                flagsMap.SetFlagOn(t.X, t.Y, t.Z, true);
                                queue.Push(t);
                                Process(t);
                            }
                        }
                    }
                }//首次生长需要避免让越界种子点又重新回溯到原先的层
            }
        }
        else
        {
            //以下是第一次生长的时候种子点不需要担心回溯
            if (oversteps.Count != 1) { throw new Exception(); }
            for (int i = 0; i < oversteps.Count; i++)
            {
                if (!flagsMap.GetFlagOn(oversteps[i].X,oversteps[i].Y,oversteps[i].Z) && IncludeConditionMeets(oversteps[i]))
                {
                    flagsMap.SetFlagOn(oversteps[i].X, oversteps[i].Y, oversteps[i].Z, true);
                    queue.Push(oversteps[i]);
                    Process(oversteps[i]);
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
                    if (!flagsMap.GetFlagOn(t.X, t.Y, t.Z) && IncludeConditionMeets(t))
                    {
                        flagsMap.SetFlagOn(t.X, t.Y, t.Z, true);
                        queue.Push(t);
                        Process(t);
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
        //ret.resultPointSet = this.result;
        ret.resultCount = this.resultCount;
        return ret;
    }
    protected virtual void Process(Int16Triple t)
    {
        //result.Add(t);
        resultCount++;
    }
    protected virtual bool IncludeConditionMeets(Int16Triple t)
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
