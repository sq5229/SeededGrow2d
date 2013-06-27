using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace SeededGrow2d
{
    public enum ParentDirectionsY
    {
        X0 = 1, X2 = 2, Non = 5
    }
    public enum ExtendTypesY
    {
        LeftRequired = 1, RightRequired = 2, AllRez = 3, UnRez = 4
    }
    public struct SpanY
    {
        public int YLeft;
        public int YRight;
        public int X;
        public ExtendTypesY Extended;
        public ParentDirectionsY ParentDirection;
    }
    public class SpanYFill2d
    {
        protected int count = 0;
        protected BitMap2d bmp;
        public FlagMap2d flagsMap;
        protected Container<SpanY> queue;
        public virtual void ExcuteSpanFill_S(BitMap2d data, Int16Double seed)
        {
            this.bmp = data;
            data.ResetVisitCount();
            flagsMap = new FlagMap2d(data.width, data.height);
            queue = new Container_Stack<SpanY>();
            Process(seed);
            flagsMap.SetFlagOn(seed.X, seed.Y, true);
            SpanY seedspan = new SpanY();
            seedspan.YLeft = seed.Y;
            seedspan.YRight = seed.Y;
            seedspan.X = seed.X;
            seedspan.ParentDirection = ParentDirectionsY.Non;
            seedspan.Extended = ExtendTypesY.UnRez;
            queue.Push(seedspan);

            while (!queue.Empty())
            {
                SpanY span = queue.Pop();
                #region AllRez
                if (span.Extended == ExtendTypesY.AllRez)
                {
                    if (span.ParentDirection == ParentDirectionsY.X2)
                    {
                        if (span.X - 1 >= 0)
                            CheckRange(span.YLeft, span.YRight, span.X - 1, ParentDirectionsY.X2);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirectionsY.X0)
                    {
                        if (span.X + 1 < bmp.width)
                            CheckRange(span.YLeft, span.YRight, span.X + 1, ParentDirectionsY.X0);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region UnRez
                if (span.Extended == ExtendTypesY.UnRez)
                {
                    int yl = FindYLeft(span.YLeft, span.X);
                    int yr = FindYRight(span.YRight, span.X);
                    if (span.ParentDirection == ParentDirectionsY.X2)
                    {
                        if (span.X - 1 >= 0)
                            CheckRange(yl, yr, span.X - 1, ParentDirectionsY.X2);
                        if (span.X + 1 < bmp.width)
                        {
                            if (yl != span.YLeft)
                                CheckRange(yl, span.YLeft, span.X + 1, ParentDirectionsY.X0);
                            if (span.YRight != yr)
                                CheckRange(span.YRight, yr, span.X + 1, ParentDirectionsY.X0);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirectionsY.X0)
                    {
                        if (span.X + 1 < bmp.width)
                            CheckRange(yl, yr, span.X + 1, ParentDirectionsY.X0);
                        if (span.X - 1 >= 0)
                        {
                            if (yl != span.YLeft)
                                CheckRange(yl, span.YLeft, span.X - 1, ParentDirectionsY.X2);
                            if (span.YRight != yr)
                                CheckRange(span.YRight, yr, span.X - 1, ParentDirectionsY.X2);
                        }
                        continue;
                    }
                    if (span.ParentDirection == ParentDirectionsY.Non)
                    {
                        if (span.X + 1 < bmp.width)
                            CheckRange(yl, yr, span.X + 1, ParentDirectionsY.X0);
                        if (span.X - 1 >= 0)
                            CheckRange(yl, yr, span.X - 1, ParentDirectionsY.X2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region LeftRequired
                if (span.Extended == ExtendTypesY.LeftRequired)
                {
                    int yl = FindYLeft(span.YLeft, span.X);
                    if (span.ParentDirection == ParentDirectionsY.X2)
                    {
                        if (span.X - 1 >= 0)
                            CheckRange(yl, span.YRight, span.X - 1, ParentDirectionsY.X2);
                        if (span.X + 1 < bmp.width && yl != span.YLeft)
                            CheckRange(yl, span.YLeft, span.X + 1, ParentDirectionsY.X0);
                        continue;
                    }
                    if (span.ParentDirection == ParentDirectionsY.X0)
                    {
                        if (span.X + 1 < bmp.width)
                            CheckRange(yl, span.YRight, span.X + 1, ParentDirectionsY.X0);
                        if (span.X - 1 >= 0 && yl != span.YLeft)
                            CheckRange(yl, span.YLeft, span.X - 1, ParentDirectionsY.X2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
                #region RightRequired
                if (span.Extended == ExtendTypesY.RightRequired)
                {
                    int yr = FindYRight(span.YRight, span.X);

                    if (span.ParentDirection == ParentDirectionsY.X2)
                    {
                        if (span.X - 1 >= 0)
                            CheckRange(span.YLeft, yr, span.X - 1, ParentDirectionsY.X2);
                        if (span.X + 1 < bmp.width && span.YRight != yr)
                            CheckRange(span.YRight, yr, span.X + 1, ParentDirectionsY.X0);
                        continue;
                    }

                    if (span.ParentDirection == ParentDirectionsY.X0)
                    {
                        if (span.X + 1 < bmp.width)
                            CheckRange(span.YLeft, yr, span.X + 1, ParentDirectionsY.X0);
                        if (span.X - 1 >= 0 && span.YRight != yr)
                            CheckRange(span.YRight, yr, span.X - 1, ParentDirectionsY.X2);
                        continue;
                    }
                    throw new Exception();
                }
                #endregion
            }
        }
        private void CheckRange(int yleft, int yright, int x, ParentDirectionsY ptype)
        {
            for (int i = yleft; i <= yright; )
            {
                if ((!flagsMap.GetFlagOn(x, i)) && IncludePredicate(x, i))
                {
                    int lb = i;
                    int rb = i + 1;
                    while (rb <= yright && (!flagsMap.GetFlagOn(x, rb)) && IncludePredicate(x, rb))
                    {
                        rb++;
                    }
                    rb--;

                    SpanY span = new SpanY();
                    span.YLeft = lb;
                    span.YRight = rb;
                    span.X = x;
                    if (lb == yleft && rb == yright)
                    {
                        span.Extended = ExtendTypesY.UnRez;
                    }
                    else if (rb == yright)
                    {
                        span.Extended = ExtendTypesY.RightRequired;
                    }
                    else if (lb == yleft)
                    {
                        span.Extended = ExtendTypesY.LeftRequired;
                    }
                    else
                    {
                        span.Extended = ExtendTypesY.AllRez;
                    }
                    span.ParentDirection = ptype;
                    for (int j = lb; j <= rb; j++)
                    {
                        flagsMap.SetFlagOn(x, j, true);
                        Process(new Int16Double(x, j));
                    }
                    queue.Push(span);

                    i = rb + 1;
                }
                else
                {
                    i++;
                }
            }
        }
        private int FindYRight(int y, int x)
        {
            int yright = y + 1;
            while (true)
            {
                if (yright == bmp.height || flagsMap.GetFlagOn(x, yright))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(x, yright))
                    {
                        Int16Double t = new Int16Double(x, yright);
                        flagsMap.SetFlagOn(x, yright, true);
                        Process(t);
                        yright++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return yright - 1;
        }
        private int FindYLeft(int y, int x)
        {
            int yleft = y - 1;
            while (true)
            {
                if (yleft == -1 || flagsMap.GetFlagOn(x, yleft))
                {
                    break;
                }
                else
                {
                    if (IncludePredicate(x, yleft))
                    {
                        Int16Double t = new Int16Double(x, yleft);
                        flagsMap.SetFlagOn(x, yleft, true);
                        Process(t);
                        yleft--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return yleft + 1;
        }
        bool IncludePredicate(int x, int y)
        {
            byte value = bmp.GetPixel(x, y);
            return value == 255;
        }
        public void Process(Int16Double p)
        {
            count++;
        }
    }
    class SpanYFill2d_T : SpanYFill2d
    {
        public Stopwatch watch = new Stopwatch();
        public ResultReport report;
        public override void ExcuteSpanFill_S(BitMap2d data, Int16Double seed)
        {
            watch.Start();
            base.ExcuteSpanFill_S(data, seed);
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
