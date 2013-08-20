using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow3d.FloodFill
{
    public class FloodFill3d
    {
        
        protected BitMap3d bmp;
        protected FlagMap3d flagsMap;
        protected Container<Int16Triple> container;
        protected int count = 0;
        public byte min;
        public byte max;
        protected bool IncludePredicate(Int16Triple p)
        {
            byte v = bmp.GetPixel(p.X, p.Y, p.Z);
            return v > min && v < max;
        }
        protected virtual void Process(Int16Triple p)
        {
            count++;
            return;
        }
        protected virtual void ExcuteFloodFill(BitMap3d data, Int16Triple seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap3d(data.width, data.height,data.depth);
            Int16Triple[] adjPoints6 = new Int16Triple[6];
            flagsMap.SetFlagOn(seed.X, seed.Y, seed.Z,true);
            container.Push(seed);
            Process(seed);
            while (!container.Empty())
            {
                Int16Triple p = container.Pop();
                InitAdj6(ref adjPoints6, ref p);
                for (int adjIndex = 0; adjIndex < 6; adjIndex++)
                {
                    Int16Triple t = adjPoints6[adjIndex];
                    if (t.X < data.width && t.X >= 0 && t.Y < data.height && t.Y >= 0&&t.Z<data.depth&&t.Z>=0)
                    {
                        if (!flagsMap.GetFlagOn(t.X, t.Y,t.Z) && IncludePredicate(t))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y,t.Z, true);
                            container.Push(t);
                            Process(t);
                        }
                    }
                }
            }
            return;
        }
        protected void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
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
            adjPoints6[4].Z = p.Z-1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z+1;
        }
    }
    public class FloodFill3d_T : FloodFill3d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public void ExcuteFloodFill_Queue(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Queue<Int16Triple>();
            base.ExcuteFloodFill(data, seed);
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
        public void ExcuteFloodFill_Stack(BitMap3d data, Int16Triple seed)
        {
            watch.Start();
            container = new Container_Stack<Int16Triple>();
            base.ExcuteFloodFill(data, seed);
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
