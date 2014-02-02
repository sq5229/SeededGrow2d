using System;
using System.Collections.Generic;
using System.Text;

namespace QEM
{
    public class Sample
    {
        public static Mesh GetSampleMesh1(int width)
        {
            Mesh m = new Mesh();
            int[,] hash=new int[width,width];
            for(int i=0;i<width;i++)
                for (int j=0;j<width;j++)
                {
                    Point3d p=new Point3d(i,j,0);
                    hash[i,j]=m.AddVertex(p);
                }
              for(int i=0;i<width-1;i++)
                  for (int j = 0; j < width-1; j++)
                  {
                      Triangle t1 = new Triangle(hash[i,j],hash[i+1,j],hash[i,j+1]);
                      Triangle t2 = new Triangle(hash[i, j+1], hash[i + 1, j], hash[i+1, j + 1]);
                      m.AddFace(t1);
                      m.AddFace(t2);
                  }
            return m;
        }
        public static Mesh GetSampleMesh2(int width)
        {
            Mesh m = new Mesh();
            int[,] hash = new int[width, width];
            int r = width/2;
            int a = width / 2;
            for (int i = 0; i < width; i++)
                for (int j = 0; j < width; j++)
                {
                    float h2 = r* r  - (i - a) * (i - a) - (j - a) * (j - a)-900;
                    if (h2 < 0)
                        h2 = 0;
                    float h = (float)Math.Sqrt(h2);
                    Point3d p = new Point3d(i, j, h);
                    hash[i, j] = m.AddVertex(p);
                }
            for (int i = 0; i < width - 1; i++)
                for (int j = 0; j < width - 1; j++)
                {
                    Triangle t1 = new Triangle(hash[i, j], hash[i + 1, j], hash[i, j + 1]);
                    Triangle t2 = new Triangle(hash[i, j + 1], hash[i + 1, j], hash[i + 1, j + 1]);
                    m.AddFace(t1);
                    m.AddFace(t2);
                }
            return m;
        }
    }
}
