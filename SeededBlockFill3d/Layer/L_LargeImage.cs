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
    public int stz;//z起始偏移
    public int edz;//z结束偏移
    public int subDepth;
    public FlagMap3d Flag;//相应位图标记
    public int indexZ;//自己是第多少个
    public int AllLayerCount;
    public int actualDepth { get { return edz - stz + 1; } }//实际高度，考虑到最后一个层高度可能不够
    public int visitcount;
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
        visitcount = 0;
    }

    public FlagMap3d GetAndInitFlag()
    {
        if (Flag != null)
            return Flag;
        else
        {
            Flag = new FlagMap3d(AllWidth,AllHeight,actualDepth);
            return Flag;
        }
    }//第一次访问时初始化
    public override string ToString()
    {
        return string.Format("[{0}*{1},{2}~{3}]", AllWidth, AllHeight, stz, edz);
    }
    public bool HasPoint(Int16Triple seed)
    {
        return seed.X >= 0 && seed.X <= AllWidth-1 && seed.Y >= 0 && seed.Y <= AllHeight-1 && seed.Z >= stz && seed.Z <= edz;
    }//检测一个点是否在该层
    public bool HasLowerLayer()
    {
        return this.indexZ > 0;
    }//检测自己上面是不是还有一层
    public bool HasUpperLayer()
    {
        return this.indexZ < AllLayerCount - 1;
    }//检测自己下面是不是还有一层
}
public class L_LargeImage
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
    public int GetLayerDepth()
    {
        return subDepth;
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
    public int GetDepth()
    {
        return depth;
    }
    public L_LargeImage(int width, int height, int depth)
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
            layers[k].AllLayerCount = layerCount;
            if (layers[k].edz > depth - 1)
                layers[k].edz = depth - 1;
        }
    }
    public Layer GetLayer(int k)
    {
        if (k < 0 || k >= GetLayerCount())
            return null;
        return layers[k];
    }
    public int GetLayerIndex(Int16Triple innerPoint)
    {
        for (int k = 0; k < layerCount; k++)
        {
            Layer b = this.GetLayer(k);
            if (b.HasPoint(innerPoint))
            {
                return k;
            }
        }
        throw new Exception();
    }
}
}
