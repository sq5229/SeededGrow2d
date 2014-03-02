using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelaunayTriangulation3d
{
    public class PVector
    {
        public float x;
        public float y;
        public float z;
        public PVector(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public PVector()
        {
            this.x = 0;
            this.y = 0;
            this.z = 0;
        }
        public float dot(PVector other)
        {
            return x*other.x+y*other.y+z*other.z;
        }
        public PVector cross(PVector other)
        {
            float u1 = this.x;
            float u2 = this.y;
            float u3 = this.z;
            float v1 = other.x;
            float v2 = other.y;
            float v3 = other.z;
            return new PVector(u2 * v3 - u3 * v2, u3 * v1 - u1 * v3, u1 * v2 - u2 * v1);
        }
        public void normalize()
        {
            float len = (float)Math.Sqrt((x) * (x) + (y) * (y) + (z) * (z));
            this.x /= len;
            this.y /= len;
            this.z /= len;
        }
        public static float dist(PVector p1,PVector p2)
        {
            return (float)Math.Sqrt((p1.x-p2.x)*(p1.x-p2.x)+(p1.y-p2.y)*(p1.y-p2.y)+(p1.z-p2.z)*(p1.z-p2.z));
        }
        public override bool Equals(object obj)
        {
            PVector p = (PVector)obj;
            return p.x == x && p.y == y && p.z == z;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    public class Line
    {
        public PVector start, end;
        public Line(PVector start, PVector end)
        {
            this.start = start;
            this.end = end;
        }

        // 始点と終点をひっくり返す
        public void reverse()
        {
            PVector tmp = this.start;
            this.start = this.end;
            this.end = tmp;
        }

        // 同じかどうか
        public bool equals(Line l)
        {
            if ((this.start == l.start && this.end == l.end)
                    || (this.start == l.end && this.end == l.start))
                return true;
            return false;
        }
    }
    public class Tetrahedron
    {
        // 4頂点を順序づけて格納
        public PVector[] vertices;
        public PVector o;      // 外接円の中心
        public float r;      // 外接円の半径

        public Tetrahedron(PVector[] v)
        {
            this.vertices = v;
            getCenterCircumcircle();
        }

        public Tetrahedron(PVector v1, PVector v2, PVector v3, PVector v4)
        {
            this.vertices = new PVector[4];
            vertices[0] = v1;
            vertices[1] = v2;
            vertices[2] = v3;
            vertices[3] = v4;
            getCenterCircumcircle();
        }

        public bool equals(Tetrahedron t) {
        int count = 0;
        foreach (PVector p1 in this.vertices) {
            foreach (PVector p2 in  t.vertices) {
                if (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z) {
                    count++;
                }
            }
        }
        if (count == 4) return true;
        return false;
    }

        public Line[] getLines()
        {
            PVector v1 = vertices[0];
            PVector v2 = vertices[1];
            PVector v3 = vertices[2];
            PVector v4 = vertices[3];

            Line[] lines = new Line[6];

            lines[0] = new Line(v1, v2);
            lines[1] = new Line(v1, v3);
            lines[2] = new Line(v1, v4);
            lines[3] = new Line(v2, v3);
            lines[4] = new Line(v2, v4);
            lines[5] = new Line(v3, v4);
            return lines;
        }

        // 外接円も求めちゃう
        private void getCenterCircumcircle()
        {
            PVector v1 = vertices[0];
            PVector v2 = vertices[1];
            PVector v3 = vertices[2];
            PVector v4 = vertices[3];

            double[][] A = {
            new double[]{v2.x - v1.x, v2.y-v1.y, v2.z-v1.z},
            new double[]{v3.x - v1.x, v3.y-v1.y, v3.z-v1.z},
            new double[]{v4.x - v1.x, v4.y-v1.y, v4.z-v1.z}
        };
            double[] b = {
            0.5 * (v2.x*v2.x - v1.x*v1.x + v2.y*v2.y - v1.y*v1.y + v2.z*v2.z - v1.z*v1.z),
            0.5 * (v3.x*v3.x - v1.x*v1.x + v3.y*v3.y - v1.y*v1.y + v3.z*v3.z - v1.z*v1.z),
            0.5 * (v4.x*v4.x - v1.x*v1.x + v4.y*v4.y - v1.y*v1.y + v4.z*v4.z - v1.z*v1.z)
        };
            double[] x = new double[3];
            if (gauss(A, b, x) == 0)
            {
                o = null;
                r = -1;
            }
            else
            {
                o = new PVector((float)x[0], (float)x[1], (float)x[2]);
                r = PVector.dist(o, v1);
            }
        }

        /** LU分解による方程式の解法 **/
        private double lu(double[][] a, int[] ip)
        {
            int n = a.Length;
            double[] weight = new double[n];

            for (int k = 0; k < n; k++)
            {
                ip[k] = k;
                double u = 0;
                for (int j = 0; j < n; j++)
                {
                    double t = Math.Abs(a[k][j]);
                    if (t > u) u = t;
                }
                if (u == 0) return 0;
                weight[k] = 1 / u;
            }
            double det = 1;
            for (int k = 0; k < n; k++)
            {
                double u = -1;
                int m = 0;
                for (int i = k; i < n; i++)
                {
                    int ii = ip[i];
                    double t = Math.Abs(a[ii][k]) * weight[ii];
                    if (t > u) { u = t; m = i; }
                }
                int ik = ip[m];
                if (m != k)
                {
                    ip[m] = ip[k]; ip[k] = ik;
                    det = -det;
                }
                u = a[ik][k]; det *= u;
                if (u == 0) return 0;
                for (int i = k + 1; i < n; i++)
                {
                    int ii = ip[i]; double t = (a[ii][k] /= u);
                    for (int j = k + 1; j < n; j++) a[ii][j] -= t * a[ik][j];
                }
            }
            return det;
        }
        private void solve(double[][] a, double[] b, int[] ip, double[] x)
        {
            int n = a.Length;
            for (int i = 0; i < n; i++)
            {
                int ii = ip[i]; double t = b[ii];
                for (int j = 0; j < i; j++) t -= a[ii][j] * x[j];
                x[i] = t;
            }
            for (int i = n - 1; i >= 0; i--)
            {
                double t = x[i]; int ii = ip[i];
                for (int j = i + 1; j < n; j++) t -= a[ii][j] * x[j];
                x[i] = t / a[ii][i];
            }
        }
        private double gauss(double[][] a, double[] b, double[] x)
        {
            int n = a.Length;
            int[] ip = new int[n];
            double det = lu(a, ip);

            if (det != 0) { solve(a, b, ip, x); }
            return det;
        }
    }
    public class DTriangle
    {
        public PVector v1, v2, v3;
        public DTriangle(PVector v1, PVector v2, PVector v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        // 法線を求める
        // 頂点は左回りの順であるとする
        public PVector getNormal()
        {
            PVector edge1 = new PVector(v2.x - v1.x, v2.y - v1.y, v2.z - v1.z);
            PVector edge2 = new PVector(v3.x - v1.x, v3.y - v1.y, v3.z - v1.z);

            // クロス積
            PVector normal = edge1.cross(edge2);
            normal.normalize();
            return normal;
        }

        // 面を裏返す（頂点の順序を逆に）
        public void turnBack()
        {
            PVector tmp = this.v3;
            this.v3 = this.v1;
            this.v1 = tmp;
        }

        // 線分のリストを得る
        public Line[] getLines()
        {
            Line[] l = {
            new Line(v1, v2),
            new Line(v2, v3),
            new Line(v3, v1)
        };
            return l;
        }

        // 同じかどうか。すげー簡易的なチェック
        public bool equals(DTriangle t)
        {
            Line[] lines1 = this.getLines();
            Line[] lines2 = t.getLines();

            int cnt = 0;
            for (int i = 0; i < lines1.Length; i++)
            {
                for (int j = 0; j < lines2.Length; j++)
                {
                    if (lines1[i].equals(lines2[j]))
                        cnt++;
                }
            }
            if (cnt == 3) return true;
            else return false;

        }
    }

}
