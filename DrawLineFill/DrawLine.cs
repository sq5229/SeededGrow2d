using System;
using System.Collections.Generic;
using System.Text;

namespace DrawLineFill
{
    
    class FillShape
    {
        private BitMap3d bmpMap;

        public FillShape(int width, int height, int depth)
        {
            bmpMap = new BitMap3d(width, height, depth, BitMap3d.BLACK);
        }

        public void DrawTriangle3d(int x0, int y0, int z0, int x1, int y1, int z1, int x2, int y2, int z2)
        {
            double e0 = Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1) + (z0 - z1) * (z0 - z1));
            double e1 = Math.Sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1) + (z2 - z1) * (z2 - z1));
            double e2 = Math.Sqrt((x0 - x2) * (x0 - x2) + (y0 - y2) * (y0 - y2) + (z0 - z2) * (z0 - z2)); 
            List<Int16Triple> list;

            //if (e0 <= e1 && e0 <= e2)
            {
                list = Get3DLineList(x0, y0, z0, x1, y1, z1);
                for (int i = 0; i < list.Count; i++)
                {
                    bmpMap.SetPixel(list[i].X,list[i].Y  ,list[i].Z ,BitMap3d.WHITE);
                    Draw3DLine(x2, y2, z2, list[i].X, list[i].Y, list[i].Z);
                }
              //  return;
            }
           // if (e1 <= e0 && e1 <= e2)
            {
                list = Get3DLineList(x1, y1, z1, x2, y2, z2);
                for (int i = 0; i < list.Count; i++)
                {
                    bmpMap.SetPixel(list[i].X , list[i].Y, list[i].Z, BitMap3d.WHITE);
                    Draw3DLine(x0, y0, z0, list[i].X, list[i].Y, list[i].Z);
                }
               // return;
            }
           // if (e2 <= e0 && e2 <= e1)
            {
                list = Get3DLineList(x2, y2, z2, x0, y0, z0);
                for (int i = 0; i < list.Count; i++)
                {
                    bmpMap.SetPixel(list[i].X , list[i].Y, list[i].Z , BitMap3d.WHITE);
                    Draw3DLine(x1, y1, z1, list[i].X, list[i].Y, list[i].Z);
                }
              //  return;
            }
        }

        private List<Int16Triple> Get3DLineList(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            List<Int16Triple> list = new List<Int16Triple>();
            int dx = x1 - x0;
            int dy = y1 - y0;
            int dz = z1 - z0;
            if (Math.Abs(dx) >= Math.Abs(dy) && Math.Abs(dx) >= Math.Abs(dz))
            { // x distance is largest
                float my = (float)dy / (float)dx;      // compute y slope
                float mz = (float)dz / (float)dx;      // compute z slope
                float by = y0 - my * x0;
                float bz = z0 - mz * x0;
                dx = (dx < 0) ? -1 : 1;
                while (x0 != x1)
                {
                    x0 += dx;
                    int px = x0;
                    int py = (int)Math.Round(my * x0 + by);
                    int pz = (int)Math.Round(mz * x0 + bz);
                    list.Add(new Int16Triple(px, py, pz));
                }
                return list;
            }
            if (Math.Abs(dy) >= Math.Abs(dx) && Math.Abs(dy) >= Math.Abs(dz))
            { // y distance is largest
                float mx = (float)dx / (float)dy;      // compute y slope
                float mz = (float)dz / (float)dy;      // compute z slope
                float bx = x0 - mx * y0;
                float bz = z0 - mz * y0;
                dy = (dy < 0) ? -1 : 1;
                while (y0 != y1)
                {
                    y0 += dy;
                    int px = (int)Math.Round(mx * y0 + bx);
                    int py = y0;
                    int pz = (int)Math.Round(mz * y0 + bz);
                    list.Add(new Int16Triple(px, py, pz));
                }
                return list;
            }
            if (Math.Abs(dz) >= Math.Abs(dx) && Math.Abs(dz) >= Math.Abs(dy))
            { // z distance is largest
                float mx = (float)dx / (float)dz;      // compute y slope
                float my = (float)dy / (float)dz;      // compute z slope
                float bx = x0 - mx * z0;
                float by = y0 - my * z0;
                dz = (dz < 0) ? -1 : 1;
                while (z0 != z1)
                {
                    z0 += dz;
                    int px = (int)Math.Round(mx * z0 + bx);
                    int py = (int)Math.Round(my * z0 + by);
                    int pz = z0;
                    list.Add(new Int16Triple(px, py, pz));
                }
                return list;
            }
            throw new Exception();
        }

        public void Draw3DLine(int x0, int y0, int z0, int x1, int y1, int z1)
        {
            int dx = x1 - x0;
            int dy = y1 - y0;
            int dz = z1 - z0;
            if (Math.Abs(dx) > Math.Abs(dy) && Math.Abs(dx) > Math.Abs(dz))
            { // x distance is largest
                float my = (float)dy / (float)dx;      // compute y slope
                float mz = (float)dz / (float)dx;      // compute z slope
                float by = y0 - my * x0;
                float bz = z0 - mz * x0;
                dx = (dx < 0) ? -1 : 1;
                while (x0 != x1)
                {
                    x0 += dx;
                    int px = x0;
                    int py = (int)Math.Round(my * x0 + by);
                    int pz = (int)Math.Round(mz * x0 + bz);
                    bmpMap.SetPixel(px , py , pz , BitMap3d.WHITE);
                }
                return;
            }
            if (Math.Abs(dy) > Math.Abs(dx) && Math.Abs(dy) > Math.Abs(dz))
            { // y distance is largest
                float mx = (float)dx / (float)dy;      // compute y slope
                float mz = (float)dz / (float)dy;      // compute z slope
                float bx = x0 - mx * y0;
                float bz = z0 - mz * y0;
                dy = (dy < 0) ? -1 : 1;
                while (y0 != y1)
                {
                    y0 += dy;
                    int px = (int)Math.Round(mx * y0 + bx);
                    int py = y0;
                    int pz = (int)Math.Round(mz * y0 + bz);
                    bmpMap.SetPixel(px , py , pz , BitMap3d.WHITE);
                }
                return;
            }
            if (Math.Abs(dz) > Math.Abs(dx) && Math.Abs(dz) > Math.Abs(dy))
            { // z distance is largest
                float mx = (float)dx / (float)dz;      // compute y slope
                float my = (float)dy / (float)dz;      // compute z slope
                float bx = x0 - mx * z0;
                float by = y0 - my * z0;
                dz = (dz < 0) ? -1 : 1;
                while (z0 != z1)
                {
                    z0 += dz;
                    int px = (int)Math.Round(mx * z0 + bx);
                    int py = (int)Math.Round(my * z0 + by);
                    int pz = z0;
                    bmpMap.SetPixel(px , py , pz , BitMap3d.WHITE);
                }
                return;
            }
        }

        public BitMap3d GetMap()
        {
            return bmpMap;
        }


    }
}
