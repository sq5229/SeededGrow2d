using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Layer
{
    public class Layer
    {
        public int AllWidth;
        public int AllHeight;
        public int AllDepth;
        public int stz;
        public int edz;
        public int subDepth;
        public BitArray Flag;
        public int indexZ;
        public int actualDepth { get { return edz - stz + 1; } }
        public Layer(int allwidth, int allheight, int alldepth,int subDepth,int stz, int edz)
        {
            this.AllWidth = allwidth;
            this.AllHeight = allheight;
            this.AllDepth = alldepth;
            this.stz = stz;
            this.subDepth = subDepth;
            this.edz = edz;
            Flag = null;
            indexZ = -1;
        }
        public Int16Triple ConvertToBlockCoord(Int16Triple globalCoord)
        {
            return new Int16Triple(globalCoord.X, globalCoord.Y, globalCoord.Z - stz);
        }
        public void ConvertCoords(List<Int16Triple> adjSeedList)
        {
            for (int i = 0; i < adjSeedList.Count; i++)
            {
                Int16Triple old = adjSeedList[i];
                old.Z -= stz;
                adjSeedList[i] = old;
            }
        }
        public void InitFlag()
        {
            Flag = new BitArray(AllWidth * AllHeight * actualDepth);
        }
        public override string ToString()
        {
            return string.Format("[{0}*{1},{2}~{3}]", AllWidth, AllHeight, stz, edz);
        }
        public static void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
        {
            adjPoints6[0].X = p.X;
            adjPoints6[0].Y = p.Y + 1;
            adjPoints6[0].Z = p.Z;

            adjPoints6[1].X = p.X;
            adjPoints6[1].Y = p.Y - 1;
            adjPoints6[1].Z = p.Z;

            adjPoints6[2].X = p.X + 1;
            adjPoints6[2].Y = p.Y;
            adjPoints6[2].Z = p.Z;

            adjPoints6[3].X = p.X - 1;
            adjPoints6[3].Y = p.Y;
            adjPoints6[3].Z = p.Z;

            adjPoints6[4].X = p.X;
            adjPoints6[4].Y = p.Y;
            adjPoints6[4].Z = p.Z + 1;

            adjPoints6[5].X = p.X;
            adjPoints6[5].Y = p.Y;
            adjPoints6[5].Z = p.Z - 1;
        }

        public bool HasPoint(Int16Triple seed)
        {
            return seed.X >= 0 && seed.X <= AllWidth-1 && seed.Y >= 0 && seed.Y <= AllHeight-1 && seed.Z >= stz && seed.Z <= edz;
        }
    }
    public class LargeImage
    {
        int width;
        int height;
        int depth;
        int subDepth;
        Layer[] layers;
        int layerCount;
        public int GetLayerCount()
        {
            return layerCount;
        }
        public LargeImage(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        public void CreateLayers(int subDepth)
        {
            if (!(subDepth <= depth))
                throw new Exception();
            this.subDepth = subDepth;
            layerCount = (depth % subDepth == 0) ? (depth / subDepth) : (depth / subDepth + 1);
            layers = new Layer[layerCount];
            for (int k = 0; k < layerCount; k++)
            {
                Layer b = new Layer(width, height, depth, subDepth, k * subDepth,(k + 1) * subDepth - 1);
                layers[k] = b;
                layers[k].indexZ = k;
                if (layers[k].edz > depth - 1)
                   layers[k].edz = depth - 1;
            }
        }
        public Layer GetBlock(int k)
        {
            return layers[k];
        }
    }
}
