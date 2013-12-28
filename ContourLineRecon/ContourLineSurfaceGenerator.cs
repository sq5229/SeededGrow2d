using System;
using System.Collections.Generic;
using System.Text;

namespace ContourLineRecon
{
    public class ContourLineSurfaceGenerator
    {
        List<ContourLine> lines;
        public ContourLineSurfaceGenerator(List<ContourLine> lines)
        {
            this.lines = lines;
        }
        public Mesh GenerateSurface()
        {
            if(lines.Count<=1)
                return null;
            lines.Sort();
            for (int i = 0; i < lines.Count; i++)
            {
                List<FloatDouble> linepoints = lines[i].GetPointList();
                if (0.0f > PolyTriangulator.Area(linepoints))
                {
                    ReverseArray(linepoints);
                }
            }

            Mesh m = new Mesh();
            List<int>[] maps = new List<int>[lines.Count];
            for (int i = 0; i < maps.Length; i++)
            {
                maps[i] = new List<int>();
                for (int j = 0; j < lines[i].GetLinePointCount(); j++)
                    maps[i].Add(-1);
            }
            List<FloatDouble> upContourPrcessed = null;
            Box3Float boxU=lines[lines.Count-1].GetBox();
            List<FloatDouble> downContourPrcessed = null;
            Box3Float boxD = lines[0].GetBox();
            for (int i = 0; i < lines.Count - 1; i++)
            {
                ContourStitcher cs = new ContourStitcher(lines[i],lines[i+1]);
                if (i == 0)
                    downContourPrcessed = cs.lineDownProcessed;
                if (i == lines.Count - 2)
                    upContourPrcessed = cs.lineUpProcessed;
                Mesh submesh = cs.DoStitching();
                //PlyManager.Output(submesh, i+"_.ply");
                int Z0Count = lines[i].GetLinePointCount();
                int Z2Count = lines[i+1].GetLinePointCount();
                if (submesh.Vertices.Count != Z0Count + Z2Count)
                    throw new Exception();
                for (int j = 0; j < Z0Count; j++)
                {
                    if (maps[i][j] == -1)
                        maps[i][j] = m.AddVertex(submesh.Vertices[j]);
                }
                for (int j = 0; j < Z2Count; j++)
                {
                    maps[i+1][j] = m.AddVertex(submesh.Vertices[j+Z0Count]);
                }
                for (int j = 0; j < submesh.Faces.Count; j++)
                {
                    Triangle t = submesh.Faces[j];
                    if (t.P0Index < Z0Count)
                        t.P0Index = maps[i][t.P0Index];
                    else
                        t.P0Index = maps[i + 1][t.P0Index - Z0Count];

                    if (t.P1Index < Z0Count)
                        t.P1Index = maps[i][t.P1Index];
                    else
                        t.P1Index = maps[i + 1][t.P1Index - Z0Count];

                    if (t.P2Index < Z0Count)
                        t.P2Index = maps[i][t.P2Index];
                    else
                        t.P2Index = maps[i + 1][t.P2Index - Z0Count];
                    m.AddFace(t);
                }
            }
            FillUpHole(m,maps,upContourPrcessed, boxU);
            FillDownHole(m, maps, downContourPrcessed, boxD);
            return m;
        }
        private void FillDownHole(Mesh m, List<int>[] maps, List<FloatDouble> downContourPrcessed, Box3Float boxD)
        {
            Transform(downContourPrcessed, boxD.GetCenter().X, boxD.GetCenter().Y);
            List<Triangle> down = new List<Triangle>();
            PolyTriangulator.Process(downContourPrcessed, down);
            if (IsNormalZ2(downContourPrcessed, down))
            {
                ReverseNormal(down);
            }
            for (int i = 0; i < down.Count; i++)
            {
                Triangle t = down[i];
                t.P0Index = maps[0][t.P0Index];
                t.P1Index = maps[0][t.P1Index];
                t.P2Index = maps[0][t.P2Index];
                m.AddFace(t);
            }
        }
        private void FillUpHole(Mesh m,List<int>[] maps,List<FloatDouble> upContourPrcessed, Box3Float boxU)
        {
            Transform(upContourPrcessed, boxU.GetCenter().X, boxU.GetCenter().Y);
            List<Triangle> up = new List<Triangle>();
            PolyTriangulator.Process(upContourPrcessed, up);
            if (!IsNormalZ2(upContourPrcessed, up))
            {
                ReverseNormal(up);
            }
            for (int i = 0; i < up.Count; i++)
            {
                Triangle t = up[i];
                t.P0Index = maps[lines.Count - 1][t.P0Index];
                t.P1Index = maps[lines.Count - 1][t.P1Index];
                t.P2Index = maps[lines.Count - 1][t.P2Index];
                m.AddFace(t);
            }
        }
        private void ReverseNormal(List<Triangle> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Triangle t = list[i];
                int temp = t.P0Index;
                t.P0Index = t.P2Index;
                t.P2Index = temp;
                list[i] = t;
            }
        }
        private bool IsNormalZ2(List<FloatDouble> vertices,List<Triangle> faces)
        {
            if (faces.Count == 0)
                return true;
            else
            {
                Triangle t = faces[0];
                Point3d p0 = new Point3d(vertices[t.P0Index].X, vertices[t.P0Index].Y, 0);
                Point3d p1 = new Point3d(vertices[t.P1Index].X, vertices[t.P1Index].Y, 0);
                Point3d p2 = new Point3d(vertices[t.P2Index].X, vertices[t.P2Index].Y, 0);
                
                Vector v = Triangle.CaculateNormal(p0,p1,p2);
                if (v.Z > 0)
                    return true;
                else
                    return false;
            }
        }
        public static void Transform(List<FloatDouble> list, float dx, float dy)
        {
            for (int i = 0; i < list.Count; i++)
            {
                FloatDouble p = list[i];
                p.X += dx;
                p.Y += dy;
                list[i] = p;
            }
        }
        private void ReverseArray(List<FloatDouble> list)
        {
            int count = list.Count;
            for (int i = 0; i < count / 2; ++i)
            {
                FloatDouble temp = list[count - i - 1];
                list[count - i - 1] = list[i];
                list[i] = temp;
            }
        }
    }
}
