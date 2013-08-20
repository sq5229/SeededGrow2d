using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Block
{
    //public class Block
    //{
    //    public int AllWidth;
    //    public int AllHeight;
    //    public int AllDepth;
    //    public int stx;
    //    public int sty;
    //    public int stz;
    //    public int edx;
    //    public int edy;
    //    public int edz;
    //    public int subWidth;
    //    public int subHeight;
    //    public int subDepth;
    //    public BitArray Flag;
    //    public int indexX;
    //    public int indexY;
    //    public int indexZ;
    //    public int actualWidth { get { return edx - stx + 1; } }
    //    public int actualHeight { get { return edy - sty + 1; } }
    //    public int actualDepth { get { return edz - stz + 1; } }
    //    public Block(int allwidth,int allheight,int alldepth,int subWidth, int subHeight, int subDepth,int stx,int sty,int stz,int edx,int edy,int edz)
    //    {
    //        this.AllWidth = allwidth;
    //        this.AllHeight = allheight;
    //        this.AllDepth = alldepth;
    //        this.stx = stx;
    //        this.sty = sty;
    //        this.stz = stz;
    //        this.subWidth = subWidth;
    //        this.subHeight = subHeight;
    //        this.subDepth = subDepth;
    //        this.edx = edx;
    //        this.edy = edy;
    //        this.edz = edz;
    //        Flag = null;
    //        indexX = -1;
    //        indexY = -1;
    //        indexZ = -1;
    //    }
    //    public Int16Triple ConvertToBlockCoord(Int16Triple globalCoord)
    //    {
    //        return new Int16Triple(globalCoord.X - stx, globalCoord.Y - sty, globalCoord.Z - stz);
    //    }
    //    public void ConvertCoords(List<Int16Triple> adjSeedList)
    //    {
    //        for (int i = 0; i < adjSeedList.Count; i++)
    //        {
    //            Int16Triple old = adjSeedList[i];
    //            old.X -= stx;
    //            old.Y -= sty;
    //            old.Z -= stz;
    //            adjSeedList[i] = old;
    //        }
    //    }
    //    public void InitFlag()
    //    {
    //        Flag = new BitArray(actualWidth * actualHeight * actualDepth);
    //    }
    //    public override string ToString()
    //    {
    //        return string.Format("[{0}~{1},{2}~{3},{4}~{5}]",stx,edx,sty,edy,stz,edz);
    //    }
    //    public static void InitAdj6(ref Int16Triple[] adjPoints6, ref Int16Triple p)
    //    {
    //        adjPoints6[0].X = p.X;
    //        adjPoints6[0].Y = p.Y + 1;
    //        adjPoints6[0].Z = p.Z;

    //        adjPoints6[1].X = p.X;
    //        adjPoints6[1].Y = p.Y - 1;
    //        adjPoints6[1].Z = p.Z;

    //        adjPoints6[2].X = p.X + 1;
    //        adjPoints6[2].Y = p.Y;
    //        adjPoints6[2].Z = p.Z;

    //        adjPoints6[3].X = p.X - 1;
    //        adjPoints6[3].Y = p.Y;
    //        adjPoints6[3].Z = p.Z;

    //        adjPoints6[4].X = p.X;
    //        adjPoints6[4].Y = p.Y;
    //        adjPoints6[4].Z = p.Z + 1;

    //        adjPoints6[5].X = p.X;
    //        adjPoints6[5].Y = p.Y;
    //        adjPoints6[5].Z = p.Z - 1;
    //    }

    //    public bool HasPoint(Int16Triple seed)
    //    {
    //        return seed.X >= stx && seed.X <= edx && seed.Y >= sty && seed.Y <= edy && seed.Z >= stz && seed.Z <= edz;
    //    }
    //}
    //public class LargeImage
    //{
    //    int width;
    //    int height;
    //    int depth;
    //    int subWidth;
    //    int subHeight;
    //    int subDepth;
    //    Block[,,] blocksMap;
    //    int rowCount;
    //    int columnCount;
    //    int layerCount;
    //    public Int16Triple GetBlocksCount()
    //    {
    //        return new Int16Triple(rowCount, columnCount, layerCount);
    //    }
    //    public LargeImage(int width, int height, int depth)
    //    {
    //        this.width = width;
    //        this.height = height;
    //        this.depth = depth;
    //    }
    //    public void CreateBlocks(int subWidth, int subHeight, int subDepth)
    //    {
    //        if(!(subWidth<=width&&subHeight<=height&&subDepth<=depth))
    //            throw new Exception();
    //        this.subWidth = subWidth;
    //        this.subHeight = subHeight;
    //        this.subDepth = subDepth;
    //        rowCount = (width % subWidth == 0) ? (width / subWidth) : (width / subWidth+1);
    //        columnCount = (height % subHeight == 0) ? (height / subHeight) : (height / subHeight + 1);
    //        layerCount = (depth % subDepth == 0) ? (depth / subDepth) : (depth / subDepth + 1);
    //        blocksMap = new Block[rowCount, columnCount, layerCount];
    //        for (int i = 0; i < rowCount; i++)
    //        {
    //            for (int j = 0; j < columnCount; j++)
    //            {
    //                for (int k = 0; k < layerCount; k++)
    //                {
    //                    Block b = new Block(width,height,depth,subWidth, subHeight, subDepth, i * subWidth, j * subHeight, k * subDepth, (i + 1) * subWidth - 1, (j + 1) * subHeight - 1, (k + 1) * subDepth - 1);
    //                    blocksMap[i, j, k] = b;
    //                    blocksMap[i, j, k].indexX = i;
    //                    blocksMap[i, j, k].indexY = j;
    //                    blocksMap[i, j, k].indexZ = k;
    //                }
    //            }
    //        }
    //        for (int i = 0; i < rowCount; i++)
    //        {
    //            for (int j = 0; j < columnCount; j++)
    //            {
    //                for (int k = 0; k < layerCount; k++)
    //                {
    //                    Block b = blocksMap[i, j, k];
    //                    if (b.edx > width-1)
    //                        b.edx = width - 1;
    //                    if (b.edy > height - 1)
    //                        b.edy = height - 1;
    //                    if (b.edz > depth - 1)
    //                        b.edz = depth - 1;
    //                }
    //            }
    //        }
    //    }
    //    public Block GetBlock(int i, int j, int k)
    //    {
    //        return blocksMap[i, j, k];
    //    }
    //}
}
