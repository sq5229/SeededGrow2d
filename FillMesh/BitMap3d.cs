using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FillMesh
{
    public class Box3Int
    {
        public int[] Min3;
        public int[] Max3;
        public Box3Int()
        {
            Min3 = new int[3] { int.MaxValue, int.MaxValue, int.MaxValue };
            Max3 = new int[3] { int.MinValue, int.MinValue, int.MinValue };
        }
        public void UpdataRange(int x, int y, int z)
        {
            if (x < Min3[0])
                Min3[0] = x;
            if (y < Min3[1])
                Min3[1] = y;
            if (z < Min3[2])
                Min3[2] = z;

            if (x > Max3[0])
                Max3[0] = x;
            if (y > Max3[1])
                Max3[1] = y;
            if (z > Max3[2])
                Max3[2] = z;
        }
        public int GetXLength() { return Max3[0] - Min3[0] + 1; }
        public int GetYLength() { return Max3[1] - Min3[1] + 1; }
        public int GetZLength() { return Max3[2] - Min3[2] + 1; }
        public override string ToString()
        {
            return string.Format("[{0},{1}] [{2},{3}] [{4},{5}]", Min3[0], Max3[0], Min3[1], Max3[1], Min3[2], Max3[2]);
        }
    }
    public class BitMap3d
    {
        public const byte WHITE = 255;
        public const byte GRAY = 127;
        public const byte BLACK = 0;
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        public BitMap3d(int width, int height, int depth, byte v)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            data = new byte[width * height * depth];
            for (int i = 0; i < width * height * depth; i++)
                data[i] = v;
        }

        public BitMap3d(byte[] data, int width, int height, int depth)
        {
            this.data = data;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public void SetPixel(int x, int y, int z, byte v)
        {
            data[x + y * width + z * width * height] = v;
        }

        public void Reverse()
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == WHITE)
                    data[i] = BLACK;
                else
                    data[i] = WHITE;
            }
        }

        public void ExecuteDiaEro()
        {
            
        }

        public byte GetPixel(int x, int y, int z)
        {
            return data[x + y * width + z * width * height];
        }
        public void ReadRaw(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Read(data, 0, width * height * depth);
            fs.Close();
        }
        public void SaveRaw(string path)
        {
            FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fs.Write(data, 0, data.Length);
            fs.Close();
        }
        public static BitMap3d CreateSampleTedVolume(int is400_300_200_100)
        {
            BitMap3d image = new BitMap3d(is400_300_200_100, is400_300_200_100, is400_300_200_100, BitMap3d.BLACK);
            image.ReadRaw(string.Format("D://VTKproj//Ted_{0}.raw", is400_300_200_100));
            byte[] data = image.data;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] > 128)
                    data[i] = BitMap3d.WHITE;
                else
                    data[i] = BitMap3d.BLACK;
            }
            return image;
        }

        public static BitMap3d CreateSampleForFan()
        {
            BitMap3d image = new BitMap3d(150, 150, 150, BitMap3d.BLACK);
            image.ReadRaw("D://VTKproj//marschnerlobb15.raw");
            byte[] data = image.data;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 89 && data[i] <= 255)
                    data[i] = BitMap3d.BLACK;
                else
                    data[i] = BitMap3d.WHITE;
            }
            for (int k = 0; k < image.depth; k++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    for (int i = 0; i < image.width; i++)
                    {
                        int index = k * image.width * image.height + j * image.width + i;
                        if (i == 0 || i == image.width - 1 || j == 0 || j == image.height - 1 || k == 0 || k == image.depth - 1)
                        {
                            data[index] = BitMap3d.BLACK;
                        }
                    }
                }
            }
            return image;
        }

        public static BitMap3d CreateSampleEngineVolume(string x2)
        {
            BitMap3d image;
            if (x2 == "")
            {
                image = new BitMap3d(256, 256, 128, BitMap3d.BLACK);
                image.ReadRaw("D://VTKproj//engine.raw");
            }
            else
            {
                image = new BitMap3d(512, 512, 256, BitMap3d.BLACK);
                image.ReadRaw("D://VTKproj//enginex2.raw");
            }
            byte[] data = image.data;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 64 && data[i] <= 255)
                    data[i] = BitMap3d.WHITE;
                else
                    data[i] = BitMap3d.BLACK;
            }
            for (int k = 0; k < image.depth; k++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    for (int i = 0; i < image.width; i++)
                    {
                        int index = k * image.width * image.height + j * image.width + i;
                        if (i == 0 || i == image.width - 1 || j == 0 || j == image.height - 1 || k == 0 || k == image.depth - 1)
                        {
                            data[index] = BitMap3d.BLACK;
                        }
                    }
                }
            }
            return image;
        }

        public static BitMap3d CreateSampleForLobster()
        {
            BitMap3d image = new BitMap3d(301, 324, 56, BitMap3d.BLACK);
            image.ReadRaw("D://VTKproj//lobster.raw");
            byte[] data = image.data;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 37 && data[i] <= 255)
                    data[i] = BitMap3d.WHITE;
                else
                    data[i] = BitMap3d.BLACK;
            }
            for (int k = 0; k < image.depth; k++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    for (int i = 0; i < image.width; i++)
                    {
                        int index = k * image.width * image.height + j * image.width + i;
                        if (i == 0 || i == image.width - 1 || j == 0 || j == image.height - 1 || k == 0 || k == image.depth - 1)
                        {
                            data[index] = BitMap3d.BLACK;
                        }
                    }
                }
            }
            return image;
        }

        public static BitMap3d CreateSampleForLobsterX2()
        {
            BitMap3d image = new BitMap3d(602, 648, 112, BitMap3d.BLACK);
            image.ReadRaw("D://VTKproj//lobsterx2.raw");
            byte[] data = image.data;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] >= 37 && data[i] <= 255)
                    data[i] = BitMap3d.WHITE;
                else
                    data[i] = BitMap3d.BLACK;
            }
            for (int k = 0; k < image.depth; k++)
            {
                for (int j = 0; j < image.height; j++)
                {
                    for (int i = 0; i < image.width; i++)
                    {
                        int index = k * image.width * image.height + j * image.width + i;
                        if (i == 0 || i == image.width - 1 || j == 0 || j == image.height - 1 || k == 0 || k == image.depth - 1)
                        {
                            data[index] = BitMap3d.BLACK;
                        }
                    }
                }
            }
            return image;
        }

        public static BitMap3d CreateFromPointSet(List<Int16Triple> list)
        {
            Box3Int box = new Box3Int();
            for (int i = 0; i < list.Count; i++)
                box.UpdataRange(list[i].X, list[i].Y, list[i].Z);
            BitMap3d bmp = new BitMap3d(box.Max3[0]+1, box.Max3[1]+1, box.Max3[2]+1, 0);
            for (int i = 0; i < list.Count; i++)
            {
                bmp.SetPixel(list[i].X, list[i].Y, list[i].Z, 255);
            }
            return bmp;
        }
    }
    public class ByteMatrix
    {
        public int stx;
        public int sty;
        public int stz;
        public byte[] data;
        public int width;
        public int height;
        public int depth;
        int wh;
        public ByteMatrix(int stx,int sty,int stz,int width, int height, int depth, byte v)
        {
            this.stx = stx;
            this.sty = sty;
            this.stz = stz;
            this.width = width;
            this.height = height;
            this.depth = depth;
            this.wh = width * height;
            data = new byte[width * height * depth];
            for (int i = 0; i < width * height * depth; i++)
                data[i] = v;
        }
        public void SetValue_OFF(int x, int y, int z, byte v)
        {
            x -= stx;
            y -= sty;
            z -= stz;
            data[x + y * width + z * wh] = v;
        }
        public byte GetValue_OFF(int x, int y, int z)
        {
            x -= stx;
            y -= sty;
            z -= stz;
            return data[x + y * width + z * wh];
        }
        public void SetValue(int x, int y, int z, byte v)
        {
            data[x + y * width + z * wh] = v;
        }
        public byte GetValue(int x, int y, int z)
        {
            return data[x + y * width + z * wh];
        }
    }
}
