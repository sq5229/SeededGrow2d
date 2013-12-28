using System;
using System.Collections.Generic;
using System.Text;

namespace ContourLineRecon
{
    
    class ContourStitcher
    {
        struct QuadUnit
        {
            public int UpIndex0;
            public int UpIndex1;
            public int DownIndex0;
            public int DownIndex1;
            public double DiaU0D1Len;
            public double DiaU1D0Len;
            public List<FloatDouble> lineUp;
            public List<FloatDouble> lineDown;
            public void Init(int upIndex, int downIndex)
            {
                UpIndex0 = upIndex;
                DownIndex0 = downIndex;
                UpIndex1 = (upIndex + 1);//%lineUp.Count;
                DownIndex1 = (downIndex + 1);//%lineDown.Count;
                DiaU0D1Len = GetDLen(UpIndex0, DownIndex1);
                DiaU1D0Len = GetDLen(UpIndex1, DownIndex0);
            }
            public void InitLast()
            {
                UpIndex0 = lineUp.Count - 1;
                DownIndex0 = lineDown.Count - 1;
                UpIndex1 = 0;//%lineUp.Count;
                DownIndex1 = 0;//%lineDown.Count;
                DiaU0D1Len = GetDLen(UpIndex0, DownIndex1);
                DiaU1D0Len = GetDLen(UpIndex1, DownIndex0);
            }
            public double GetDLen(int index1, int index2)
            {
                float x0 = lineUp[index1].X;
                float y0 = lineUp[index1].Y;
                float x1 = lineDown[index2].X;
                float y1 = lineDown[index2].Y;
                return Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
            }
        }
        ContourLine lineUp;
        ContourLine lineDown;
        public List<FloatDouble> lineUpProcessed;
        public List<FloatDouble> lineDownProcessed;
        Box3Float boxUp;
        Box3Float boxDown;
        public ContourStitcher(ContourLine line1, ContourLine line2)
        {
            if (line1.GetZ() > line2.GetZ())
            {
                lineUp = line1;
                lineDown = line2;
            }
            else
            {
                lineUp = line2;
                lineDown = line1;
            }
            lineUpProcessed = new List<FloatDouble>(lineUp.GetLinePointCount());
            lineDownProcessed = new List<FloatDouble>(lineDown.GetLinePointCount());
            CopyArray(lineUp.GetPointList(), lineUpProcessed);
            CopyArray(lineDown.GetPointList(), lineDownProcessed);
            boxUp = lineUp.GetBox();
            boxDown = lineDown.GetBox();
            Point3d cU = boxUp.GetCenter();
            Point3d cD = boxDown.GetCenter();
            ContourLineSurfaceGenerator.Transform(lineDownProcessed, -cD.X, -cD.Y);
            ContourLineSurfaceGenerator.Transform(lineUpProcessed, -cU.X, -cU.Y);
            int indexDown = GetNearIndex();
            AdjustDownArray(indexDown);
        }
        public Mesh DoStitching()
        {
            Mesh m = new Mesh();
            int[] upMap = new int[lineUpProcessed.Count];
            float dx1 = boxUp.GetCenter().X;
            float dy1 = boxUp.GetCenter().Y;
            float dx2 = boxDown.GetCenter().X;
            float dy2 = boxDown.GetCenter().Y;
            int[] downMap = new int[lineDownProcessed.Count];
            for (int i = 0; i < lineDownProcessed.Count; i++)
            {
                Point3d p = new Point3d(lineDownProcessed[i].X + dx2, lineDownProcessed[i].Y + dy2, lineDown.GetZ());
                downMap[i] = m.AddVertex(p);
            }
            for (int i = 0; i < lineUpProcessed.Count; i++)
            {
                Point3d p = new Point3d(lineUpProcessed[i].X + dx1, lineUpProcessed[i].Y + dy1, lineUp.GetZ());
                upMap[i] = m.AddVertex(p);
            }

            int upIndex = 0;
            int downIndex = 0;
            QuadUnit quad = new QuadUnit();
            quad.lineDown = lineDownProcessed;
            quad.lineUp = lineUpProcessed;
            while (true)
            {
                if (upIndex == lineUpProcessed.Count - 1 || downIndex == lineDownProcessed.Count - 1)
                    break;
                quad.Init(upIndex, downIndex);
                if (quad.DiaU0D1Len < quad.DiaU1D0Len)
                {
                    Triangle t = new Triangle(upMap[quad.UpIndex0], downMap[quad.DownIndex0], downMap[quad.DownIndex1]);
                    m.AddFace(t);
                    downIndex++;
                }
                else
                {
                    Triangle t = new Triangle(upMap[quad.UpIndex0], downMap[quad.DownIndex0], upMap[quad.UpIndex1]);
                    m.AddFace(t);
                    upIndex++;
                }
            }
            if (upIndex == lineUpProcessed.Count - 1 || downIndex == lineDownProcessed.Count - 1)
            {
                if (downIndex == lineDownProcessed.Count - 1)
                {
                    int last = lineDownProcessed.Count - 1;
                    while (upIndex != lineUpProcessed.Count - 1)
                    {
                        Triangle t = new Triangle(downMap[last], upMap[upIndex + 1], upMap[upIndex]);
                        m.AddFace(t);
                        upIndex++;
                    }
                }
                else
                {
                    int last = lineUpProcessed.Count - 1;
                    while (downIndex != lineDownProcessed.Count - 1)
                    {
                        Triangle t = new Triangle(upMap[last], downMap[downIndex], downMap[downIndex + 1]);
                        m.AddFace(t);
                        downIndex++;
                    }
                }
            }
            quad.InitLast();
            if (quad.DiaU0D1Len < quad.DiaU1D0Len)
            {
                Triangle t = new Triangle(upMap[quad.UpIndex0], downMap[quad.DownIndex0], downMap[quad.DownIndex1]);
                Triangle t2 = new Triangle(upMap[quad.UpIndex0], downMap[quad.DownIndex1], upMap[quad.UpIndex1]);
                m.AddFace(t);
                m.AddFace(t2);
            }
            else
            {
                Triangle t = new Triangle(upMap[quad.UpIndex0], downMap[quad.DownIndex0], upMap[quad.UpIndex1]);
                Triangle t2 = new Triangle(upMap[quad.UpIndex1], downMap[quad.DownIndex0], downMap[quad.DownIndex1]);
                m.AddFace(t);
                m.AddFace(t2);
            }
            return m;
        }
        private void AdjustDownArray(int indexDown)
        {
            List<FloatDouble> list = new List<FloatDouble>();
            for (int i = 0; i < lineDownProcessed.Count; i++)
            {
                list.Add(lineDownProcessed[(indexDown + i) % lineDownProcessed.Count]);
            }
            lineDownProcessed = list;
        }
        private int GetNearIndex()
        {
            int index = -1;
            double distense = double.MaxValue;
            FloatDouble p = lineUpProcessed[0];
            float x0 = p.X;
            float y0 = p.Y;
            for (int i = 0; i < lineDownProcessed.Count; i++)
            {
                float x1 = lineDownProcessed[i].X;
                float y1 = lineDownProcessed[i].Y;
                double dis = Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1));
                if (dis < distense)
                {
                    distense = dis;
                    index = i;
                }
            }
            return index;
        }      
        private void CopyArray(List<FloatDouble> olist, List<FloatDouble> tarlist)
        {
            for (int i = 0; i < olist.Count; i++)
            {
                tarlist.Add(new FloatDouble(olist[i].X, olist[i].Y));
            }
        }
    }
}
