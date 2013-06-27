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
        protected Container<Int16Double> queue;
        protected int count = 0;
        public bool IncludePredicate(Int16Double p)
        {
            return bmp.GetPixel(p.X, p.Y) == BitMap2d.WHITE;
        }
        public void Process(Int16Double p)
        {
            count++;
            return;
        }
        public virtual void ExcuteFloodFill_Q(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            queue = new Container_Queue<Int16Double>();
            Int16Double[] adjPoints4 = new Int16Double[6];
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            queue.Push(seed);
            Process(seed);
            while (!queue.Empty())
            {
                Int16Double p = queue.Pop();
                InitAdj4(ref adjPoints4, ref p);
                for (int adjIndex = 0; adjIndex < 4; adjIndex++)
                {
                    Int16Double t = adjPoints4[adjIndex];
                    if (t.X < data.width && t.X >= 0 && t.Y < data.height && t.Y >= 0)
                    {
                        if (!flagsMap.GetFlagOn(t.X, t.Y) && IncludePredicate(t))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y, true);
                            queue.Push(t);
                            Process(t);
                        }
                    }
                }
            }
            return;
        }
        public virtual void ExcuteFloodFill_S(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            queue = new Container_Stack<Int16Double>();
            Int16Double[] adjPoints4 = new Int16Double[6];
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            queue.Push(seed);
            Process(seed);
            while (!queue.Empty())
            {
                Int16Double p = queue.Pop();
                InitAdj4(ref adjPoints4, ref p);
                for (int adjIndex = 0; adjIndex < 4; adjIndex++)
                {
                    Int16Double t = adjPoints4[adjIndex];
                    if (t.X < data.width && t.X >= 0 && t.Y < data.height && t.Y >= 0)
                    {
                        if (!flagsMap.GetFlagOn(t.X, t.Y) && IncludePredicate(t))
                        {
                            flagsMap.SetFlagOn(t.X, t.Y, true);
                            queue.Push(t);
                            Process(t);
                        }
                    }
                }
            }
            return;
        }
        private void InitAdj4(ref Int16Double[] adjPoints4, ref Int16Double p)
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
        public override void ExcuteFloodFill_Q(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            base.ExcuteFloodFill_Q(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = queue.action_pop_count;
            report.container_push_count = queue.action_push_count;
            report.container_max_size = queue.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
        public override void ExcuteFloodFill_S(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            base.ExcuteFloodFill_S(data, seed);
            watch.Stop();
            report.time = watch.ElapsedMilliseconds;
            report.result_point_count = count;
            report.bmp_get_count = data.action_get_count;
            report.bmp_set_count = data.action_set_count;
            report.container_pop_count = queue.action_pop_count;
            report.container_push_count = queue.action_push_count;
            report.container_max_size = queue.max_contain_count;
            report.flag_get_count = flagsMap.action_get_count;
            report.flag_set_count = flagsMap.action_set_count;
            return;
        }
    }
}