using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d.Block
{

    public class LargeSeededGrow
    {
        public static Int16Triple[] Adj6IndexToAdjDelta = new Int16Triple[6];
        static LargeSeededGrow()
        {
            Adj6IndexToAdjDelta[0] = new Int16Triple(0, 1, 0);
            Adj6IndexToAdjDelta[1] = new Int16Triple(0, -1, 0);
            Adj6IndexToAdjDelta[2] = new Int16Triple(1, 0, 0);
            Adj6IndexToAdjDelta[3] = new Int16Triple(-1, 0, 0);
            Adj6IndexToAdjDelta[4] = new Int16Triple(0, 0, 1);
            Adj6IndexToAdjDelta[5] = new Int16Triple(0, 0, -1);
        }
        LargeImage image;
        Int16Triple blockMapSize;
        Container_Stack<SeededGrowInput> queue;
        byte[] buffer;
        SubSeededGrow seedGrowExecutor;
        public LargeSeededGrow(int width,int height,int depth)
        {
            this.image = new LargeImage(width,height,depth);
        }
        public void SetBlockSize(int subwidth, int subheight, int subdepth)
        {
            image.CreateBlocks(subwidth, subheight, subdepth);
            blockMapSize = image.GetBlocksCount();
            buffer = new byte[subwidth*subheight*subdepth];
        }
        public void SetExecutor(SubSeededGrow grower)
        {
            this.seedGrowExecutor = grower;
        }
        public void ExecuteSeededGrow(Int16Triple firstseed)
        {
            if (buffer == null)
                throw new Exception();
            if (seedGrowExecutor == null)
                throw new Exception();
            queue=new Container_Stack<SeededGrowInput>();
            Block firstBlock = GetFirstBlock(firstseed);
            queue.Push(new SeededGrowInput(firstseed,firstBlock));
            while (!queue.Empty())
            {
                SeededGrowInput sgi=queue.Pop();
                Block block=sgi.block;
                FillBlockData(block, buffer);
                SeededGrowResult ret1 = seedGrowExecutor.ExecuteSeededGrow(buffer,sgi);
                for (int i = 0; i < 6; i++)
                {
                    if (ret1.boundaryRequestPoints[i].Count != 0)
                    {
                        int x = Adj6IndexToAdjDelta[i].X + block.indexX;
                        int y = Adj6IndexToAdjDelta[i].Y + block.indexY;
                        int z = Adj6IndexToAdjDelta[i].Z + block.indexZ;
                        if (x >= 0 && x < blockMapSize.X && y >= 0 && y < blockMapSize.Y && z >= 0 && z < blockMapSize.Z)
                        {
                            SeededGrowInput input = new SeededGrowInput(ret1.boundaryRequestPoints[i],image.GetBlock(x,y,z));
                            queue.Push(input);
                        }
                    }
                }
            }

        }
        private Block GetFirstBlock(Int16Triple seed)
        {
            for(int i=0;i<blockMapSize.X;i++)
                for (int j = 0; j < blockMapSize.Y; j++)
                {
                    for (int k = 0; k < blockMapSize.Z; k++)
                    {
                        Block b = image.GetBlock(i, j, k);
                        if (b.HasPoint(seed))
                        {
                            return b;
                        }
                    }
                }
            throw new Exception();
        }
        private void FillBlockData(Block b, byte[] data)
        {
            DataFiller.LoadBlockData(buffer, b.stx, b.sty, b.stz, b.edx, b.edy, b.edz);
        }
    }
}
