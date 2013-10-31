using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Block
{    
    public class BoundaryRecordMask
    {
        bool[] mark = new bool[6];
        public BoundaryRecordMask(bool x0,bool x2,bool y0,bool y2,bool z0,bool z2)
        {
            mark[0] = x0;
            mark[1] = x2;
            mark[2] = y0;
            mark[3] = y2;
            mark[4] = z0;
            mark[5] = z2;
        }
        public bool GetAdjNeedsSeek(int index)
        {
            return mark[index];
        }
        public void SetAdjNeedsSeek(int index,bool value)
        {
            mark[index] = value;
        }
    }
    public class Block
    {
        public int AllWidth;
        public int AllHeight;
        public int AllDepth;
        public int stx;
        public int sty;
        public int stz;
        public int edx;
        public int edy;
        public int edz;
        public int subWidth;
        public int subHeight;
        public int subDepth;
        public int indexX;
        public int indexY;
        public int indexZ;
        public int actualWidth { get { return edx - stx + 1; } }
        public int actualHeight { get { return edy - sty + 1; } }
        public int actualDepth { get { return edz - stz + 1; } }
        public Int16Triple AllDemensionSize;
        public BoundaryRecordMask boundaryMask;
        public FlagMap3d flagMap;
        public Block(int allwidth, int allheight, int alldepth, int subWidth, int subHeight, int subDepth, int stx, int sty, int stz, int edx, int edy, int edz)
        {
            this.AllWidth = allwidth;
            this.AllHeight = allheight;
            this.AllDepth = alldepth;
            this.stx = stx;
            this.sty = sty;
            this.stz = stz;
            this.subWidth = subWidth;
            this.subHeight = subHeight;
            this.subDepth = subDepth;
            this.edx = edx;
            this.edy = edy;
            this.edz = edz;
            indexX = -1;
            indexY = -1;
            indexZ = -1;
        }
        public int VisitedCount;
        public override string ToString()
        {
            return string.Format("[{0}~{1},{2}~{3},{4}~{5}]<{6}>", stx, edx, sty, edy, stz, edz,VisitedCount);
        }
        public FlagMap3d GetFlagMap3d()
        {
            if (flagMap == null)
                flagMap = new FlagMap3d(actualWidth, actualHeight, actualDepth);
            return flagMap;
            //return null;
        }
        public bool HasPoint(Int16Triple seed)
        {
            return seed.X >= stx && seed.X <= edx && seed.Y >= sty && seed.Y <= edy && seed.Z >= stz && seed.Z <= edz;
        }
        public bool HasAdjIndex(int adjIndex)
        {
            switch (adjIndex)
            {
                case 0: 
                    {
                        return indexX > 0;
                    }
                case 1:
                    {
                        return indexX < AllDemensionSize.X-1;
                    }
                case 2:
                    {
                        return indexY > 0;
                    }
                case 3:
                    {
                        return indexY > AllDemensionSize.Y-1;
                    }
                case 4:
                    {
                        return indexZ > 0;
                    }
                case 5:
                    {
                        return indexZ > AllDemensionSize.Z-1;
                    }
            }
            throw new Exception();
        }
    }
    public class B_LargeImage
    {
        int width;
        int height;
        int depth;
        int subWidth;
        int subHeight;
        int subDepth;
        Block[, ,] blocksMap;
        int rowCount;
        int columnCount;
        int layerCount;
        public Int16Triple GetBlocksCount()
        {
            return new Int16Triple(rowCount, columnCount, layerCount);
        }
        public B_LargeImage(int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
        }
        public void CreateBlocks(int subWidth, int subHeight, int subDepth)
        {
            this.subWidth = subWidth;
            this.subHeight = subHeight;
            this.subDepth = subDepth;
            rowCount = (width % subWidth == 0) ? (width / subWidth) : (width / subWidth + 1);
            columnCount = (height % subHeight == 0) ? (height / subHeight) : (height / subHeight + 1);
            layerCount = (depth % subDepth == 0) ? (depth / subDepth) : (depth / subDepth + 1);
            blocksMap = new Block[rowCount, columnCount, layerCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    for (int k = 0; k < layerCount; k++)
                    {
                        Block b = new Block(width, height, depth, subWidth, subHeight, subDepth, i * subWidth, j * subHeight, k * subDepth, (i + 1) * subWidth - 1, (j + 1) * subHeight - 1, (k + 1) * subDepth - 1);
                        blocksMap[i, j, k] = b;
                        blocksMap[i, j, k].indexX = i;
                        blocksMap[i, j, k].indexY = j;
                        blocksMap[i, j, k].indexZ = k;
                        b.AllDemensionSize = new Int16Triple(rowCount,columnCount,layerCount);
                        b.boundaryMask = new BoundaryRecordMask(true, true, true, true, true, true);
                        if (i == 0)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(0, false);
                        }
                        if (i == rowCount-1)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(1, false);
                        }
                        if (j == 0)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(2, false);
                        }
                        if (j == columnCount-1)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(3, false);
                        }
                        if (k == 0)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(4, false);
                        }
                        if (k == layerCount-1)
                        {
                            b.boundaryMask.SetAdjNeedsSeek(5, false);
                        }
                    }
                }
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    for (int k = 0; k < layerCount; k++)
                    {
                        Block b = blocksMap[i, j, k];
                        if (b.edx > width - 1)
                            b.edx = width - 1;
                        if (b.edy > height - 1)
                            b.edy = height - 1;
                        if (b.edz > depth - 1)
                            b.edz = depth - 1;
                    }
                }
            }
        }
        public Block GetBlock(int i, int j, int k)
        {
            return blocksMap[i, j, k];
        }
        public Int16Triple GetBlockIndex(Int16Triple seed)
        {
            for(int i=0;i<rowCount;i++)
                for(int j=0;j<columnCount;j++)
                    for (int k = 0; k < layerCount; k++)
                    {
                        if (blocksMap[i, j, k].HasPoint(seed))
                            return new Int16Triple(i, j, k);
                    }
            throw new Exception();
        }
        public Int16Triple GetDemensionCount()
        {
            return new Int16Triple(rowCount,columnCount,layerCount);
        }
        public Int16Triple GetDemensionSize()
        {
            return new Int16Triple(subWidth,subHeight,subDepth);
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
    }
}
