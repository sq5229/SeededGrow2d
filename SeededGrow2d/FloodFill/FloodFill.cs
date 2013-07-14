using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow2d
{
    class FloodFill2d
    {
        protected BitMap2d bmp;
        protected FlagMap2d flagsMap;
        protected Container<Int16Double> container;
        protected int count = 0;
        protected bool IncludePredicate(Int16Double p)
        {
            return bmp.GetPixel(p.X, p.Y) == BitMap2d.WHITE;
        }
        protected void Process(Int16Double p)
        {
            count++;
            return;
        }
        protected virtual void ExcuteFloodFill(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            Int16Double[] adjPoints4 = new Int16Double[4];
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            container.Push(seed);
            Process(seed);
            while (!container.Empty())
            {
                Int16Double p = container.Pop();
                InitAdj4(ref adjPoints4, ref p);
                for (int adjIndex = 0; adjIndex < 4; adjIndex++)
                {
                    Int16Double t = adjPoints4[adjIndex];
                    if (t.X < data.width && t.X >= 0 && t.Y < data.height && t.Y >= 0)
                    {
                        if (!flagsMap.GetFlagOn(t.X, t.Y) && IncludePredicate(t))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y, true);
                            container.Push(t);
                            Process(t);
                        }
                    }
                }
            }
            return;
        }
        protected void InitAdj4(ref Int16Double[] adjPoints4, ref Int16Double p)
        {
            adjPoints4[0].X = p.X - 1;
            adjPoints4[0].Y = p.Y;

            adjPoints4[1].X = p.X + 1;
            adjPoints4[1].Y = p.Y;

            adjPoints4[2].X = p.X;
            adjPoints4[2].Y = p.Y - 1;

            adjPoints4[3].X = p.X;
            adjPoints4[3].Y = p.Y + 1;
        }
    }
    class FloodFill2d_T : FloodFill2d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public  void ExcuteFloodFill_Queue(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            container = new Container_Queue<Int16Double>();
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
        public  void ExcuteFloodFill_Stack(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            container = new Container_Stack<Int16Double>();
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