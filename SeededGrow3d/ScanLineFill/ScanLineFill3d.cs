using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow3d.ScanLineFill
{
    class ScanlineFill3d
    {
        protected int count = 0;
        protected Container<Int16Triple> container;//这个容器可以是Queue和Stack中任意一种，这里抽象成一个Container
        protected BitMap3d bmp;
        public FlagMap3d flagsMap;
        protected virtual void ExcuteScanlineFill(BitMap3d data, Int16Triple seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap3d(data.width, data.height, data.depth);
            flagsMap.SetFlagOn(seed.X, seed.Y, seed.Z, true);
            container.Push(seed);
            Process(seed);
            while (!container.Empty())
            {
                Int16Triple p = container.Pop();
                int xleft = FindXLeft(p);
                int xright = FindXRight(p);
                if (p.Y - 1 >= 0)
                    CheckRange(xleft, xright, p.Y - 1, p.Z);
                if (p.Y + 1 < data.height)
                    CheckRange(xleft, xright, p.Y + 1, p.Z);
                if (p.Z - 1 >= 0)
                    CheckRange(xleft, xright, p.Y, p.Z - 1);
                if (p.Z + 1 < data.depth)
                    CheckRange(xleft, xright, p.Y, p.Z + 1);
            }
        }//该函数为扫描线法主体
        protected void CheckRange(int xleft, int xright, int y, int z)
        {
            for (int i = xleft; i <= xright; )
            {
                if ((!flagsMap.GetFlagOn(i, y, z)) && IncludePredicate(i, y, z))
                {
                    int rb = i + 1;
                    while (rb <= xright && (!flagsMap.GetFlagOn(rb, y, z)) && IncludePredicate(rb, y, z))
                    {
                        rb++;
                    }
                    rb--;
                    Int16Triple t = new Int16Triple(rb, y, z);
                    flagsMap.SetFlagOn(rb, y, z, true);
                    container.Push(t);
                    Process(t);
                    i = rb + 1;
                }
                else
                {
                    i++;
                }
            }
        }//CheckRange操作
        protected int FindXLeft(Int16Triple p)
        {
            int xleft = p.X - 1;
            while (true)
            {
                if (xleft == -1 || flagsMap.GetFlagOn(xleft, p.Y, p.Z))
                {
                    break;
                }
                else
                {
                    byte value = bmp.GetPixel(xleft, p.Y, p.Z);
                    if (IncludePredicate(xleft, p.Y, p.Z))
                    {
                        Int16Triple t = new Int16Triple(xleft, p.Y, p.Z);
                        flagsMap.SetFlagOn(xleft, p.Y, p.Z, true);
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
        }//FindXLeft操作
        protected int FindXRight(Int16Triple p)
        {
            int xright = p.X + 1;
            while (true)
            {
                if (xright == bmp.width || flagsMap.GetFlagOn(xright, p.Y, p.Z))
                {
                    break;
                }
                else
                {
                    byte value = bmp.GetPixel(xright, p.Y, p.Z);
                    if (IncludePredicate(xright, p.Y, p.Z))
                    {
                        Int16Triple t = new Int16Triple(xright, p.Y, p.Z);
                        flagsMap.SetFlagOn(xright, p.Y, p.Z, true);
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
        }//FindXRight操作
        public byte min;
        public byte max;
        protected bool IncludePredicate(int x,int y,int z)
        {
            byte v = bmp.GetPixel(x, y,z);
            return v > min && v < max;
        }
        protected void Process(Int16Triple p)
        {
            count++;
        }
    }
    class ScanlineFill3d_T : ScanlineFill3d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public void ExcuteScanlineFill_Queue(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Queue<Int16Triple>();
            base.ExcuteScanlineFill(data, seed);
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
        public void ExcuteScanlineFill_Stack(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Stack<Int16Triple>();
            base.ExcuteScanlineFill(data, seed);
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
