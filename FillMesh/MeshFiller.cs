using System;
using System.Collections.Generic;
using System.Text;

namespace FillMesh
{
    public class MeshVoxelizer
    {
        Mesh mesh;
        Box3Float range;
        public MeshVoxelizer(Mesh m)
        {
            mesh = m;
            range = new Box3Float();
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                range.UpdataRange(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z);
            }
        }

        private ByteMatrix FillMesh()
        {
            int stx = (int)(range.Min3[0]-2);
            int sty = (int)(range.Min3[1]-2);
            int stz = (int)(range.Min3[2]-2);
            int width = (int)(range.GetXLength()+4);
            int height = (int)(range.GetYLength() + 4);
            int depth = (int)(range.GetZLength() + 4);
            ByteMatrix bmp = new ByteMatrix(stx,sty,stz,width, height, depth,0);
            FillTriangles(bmp,255);
            BitMap3d cmp = new BitMap3d(bmp.data, bmp.width, bmp.height, bmp.depth);
            //cmp.SaveRaw("ret1T " + bmp.width + "_" + bmp.height + "_" + bmp.depth + ".raw");
            SpanFill3d rg = new SpanFill3d();
            rg.ExcuteSpanFill(bmp, new Int16Triple(0, 0, 0), 0, 128);
            ReverseValue(bmp);
            return bmp;
        }

        public List<Int16Triple> GetVoxels()
        {
            List<Int16Triple> list = new List<Int16Triple>();
            ByteMatrix mat = FillMesh();
            for (int k = 0; k < mat.depth; k++)
            {
                for (int j = 0; j < mat.height; j++)
                {
                    for (int i = 0; i < mat.width; i++)
                    {
                        if (mat.GetValue(i, j, k)==255)
                        {
                            Int16Triple t = new Int16Triple(i+mat.stx,j+mat.sty,k+mat.stz);
                            list.Add(t);
                        }
                    }
                }
            }
            return list;
        }

        private void FillTriangles(ByteMatrix bmp,byte color)
        {
            FillTriangle fs = new FillTriangle(bmp, color);
            for (int i = 0; i < mesh.Faces.Count; i++)
            {
                Triangle t = mesh.Faces[i];
                Point3d p0 = mesh.Vertices[t.P0Index];
                Point3d p1 = mesh.Vertices[t.P1Index];
                Point3d p2 = mesh.Vertices[t.P2Index];
                int x0 = (int)p0.X; if (x0 - p0.X > 0.5f) throw new Exception();
                int y0 = (int)p0.Y; if (y0 - p0.Y > 0.5f) throw new Exception();
                int z0 = (int)p0.Z; if (z0 - p0.Z > 0.5f) throw new Exception();
                int x1 = (int)p1.X; if (x1 - p1.X > 0.5f) throw new Exception();
                int y1 = (int)p1.Y; if (y1 - p1.Y > 0.5f) throw new Exception();
                int z1 = (int)p1.Z; if (z1 - p1.Z > 0.5f) throw new Exception();
                int x2 = (int)p2.X; if (x2 - p2.X > 0.5f) throw new Exception();
                int y2 = (int)p2.Y; if (y2 - p2.Y > 0.5f) throw new Exception();
                int z2 = (int)p2.Z; if (z2 - p2.Z > 0.5f) throw new Exception();
                fs.DrawTriangle3d(x0, y0, z0, x1, y1, z1, x2, y2, z2);
            }


        }

        private void ReverseValue(ByteMatrix bmp)
        {
            for (int i = 0; i < bmp.data.Length; i++)
            {
                if (bmp.data[i] == 255||bmp.data[i]==0)
                    bmp.data[i] = 255;
                else
                    bmp.data[i] = 0;
            }
        }
    }
}
