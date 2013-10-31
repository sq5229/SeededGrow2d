using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SeededBlockFill3d.Block.FF;

namespace SeededBlockFill3d.Block.FF
{
    public class LargeSpanFillResult
    {
        public int Count = 0;
        public List<Int16Triple> resultSet = new List<Int16Triple>();

    }
    class BlockIR_FF
    {
        public Int16Triple singleSeed;
        public List<Int16Triple>[] boundarySeedsInside;
        //public List<Int16Triple>[] boundaryOverstepOutside;
        public BlockIR_FF()
        {
            boundarySeedsInside = new List<Int16Triple>[6];
            //boundaryOverstepOutside = new List<Int16Triple>[6];
            for (int i = 0; i < 6; i++)
            {
                boundarySeedsInside[i] = new List<Int16Triple>();
                //boundaryOverstepOutside[i] = new List<Int16Triple>();
            }
            singleSeed = new Int16Triple(-1, -1, -1);
        }
        public bool GetIsFirst()
        {
            return singleSeed.X != -1;
        }
        public override string ToString()
        {
            return string.Format("[{0},{1},{2},{3},{4},{5}][{6}]", boundarySeedsInside[0].Count, boundarySeedsInside[1].Count, boundarySeedsInside[2].Count, boundarySeedsInside[3].Count, boundarySeedsInside[4].Count, boundarySeedsInside[5].Count, singleSeed);
        }
        public void Clear()
        {
            for (int i = 0; i < 6; i++)
            {
                boundarySeedsInside[i].Clear();
                boundarySeedsInside[i] = null;
            }
        }
    }
    public class B_LargeSeededGrowManager_FloodFill
    {
        #region Static
        static int[] OppositePos = new int[7] { 1, 0, 3, 2, 5, 4, 6 };
        public static Int16Triple[] Ad = new Int16Triple[6] 
        {
            new Int16Triple(-1,0,0),
            new Int16Triple(1,0,0),
            new Int16Triple(0,-1,0),
            new Int16Triple(0,1,0),
            new Int16Triple(0,0,-1),
            new Int16Triple(0,0,1),
        };
        #endregion
        B_LargeImage image;
        Container_Stack<Block> queue;
        byte[] buffer;
        B_FloodFillBase seedGrowExecutor;
        DataFiller dataProvider;
        LargeSpanFillResult resultSum;
        BlockIR_FF[, ,] blockIRMap;
        public void SetScale(int width, int height, int depth, int subwidth, int subheight, int subdepth)
        {
            this.image = new B_LargeImage(width, height, depth);
            SetBlockSize(subwidth, subheight, subdepth);
            blockIRMap = new BlockIR_FF[image.GetBlocksCount().X, image.GetBlocksCount().Y, image.GetBlocksCount().Z];
        }

        private void SetBlockSize(int subwidth, int subheight, int subdepth)
        {
            image.CreateBlocks(subwidth, subheight, subdepth);
            buffer = new byte[subwidth * subheight * subdepth];
        }

        public void SetExecutor(B_FloodFillBase grower)
        {
            this.seedGrowExecutor = grower;
        }

        public void SetDataProvider(DataFiller df)
        {
            dataProvider = df;
        }

        public void ExecuteSeededGrow(Int16Triple firstseed)
        {
            if (buffer == null || seedGrowExecutor == null || dataProvider == null)
                throw new Exception();
            FloodFillInput input=new FloodFillInput();
            FloodFillResult ret = new FloodFillResult() ;
            resultSum = new LargeSpanFillResult();
            queue = new Container_Stack<Block>();
            Block firstB = GetFirstBlock(firstseed);
            BlockIR_FF firstBir = GetBlockIR(firstB.indexX, firstB.indexY, firstB.indexZ);
            firstBir.singleSeed =ConvertGlobalCoodToBlockCoord(firstB,firstseed);
            queue.Push(firstB);
            while (!queue.Empty())
            {
                Block block = queue.Pop();
                block.VisitedCount++;
                BlockIR_FF bir = GetBlockIR(block.indexX, block.indexY, block.indexZ);
                FillBlockData(block);

                MarshalInputAndOutput(input, ret,block, bir);
                seedGrowExecutor.ExecuteSeededGrow(input,ret);

                if (input.GetIsFirst())
                    bir.singleSeed = new Int16Triple(-1, -1, -1);

                ConvertBlockCoordsToGlobalCoords(block,ret.resultSet);
                MergeResult(resultSum,ret);

                for (int i = 0; i < 6; i++)
                {
                    if (ret.GetNeedsSeekAdj(i))
                    {
                        Block t = image.GetBlock(block.indexX + Ad[i].X, block.indexY + Ad[i].Y, block.indexZ + Ad[i].Z);
                        if (t.VisitedCount < 1)
                        {
                            ConvertThisBlockCoordsToOtherBlockCoords(block, t, ret.boundaryRequestPoints[i]);
                            BlockIR_FF tbir = GetBlockIR(t.indexX, t.indexY, t.indexZ);
                            List<Int16Triple> oppInput = tbir.boundarySeedsInside[OppositePos[i]];
                            if (oppInput != null && oppInput.Count != 0)
                            {
                                oppInput.AddRange(ret.boundaryRequestPoints[i]);
                                ret.boundaryRequestPoints[i].Clear();
                                tbir.boundarySeedsInside[OppositePos[i]] = oppInput;
                            }
                            else
                            {
                                oppInput = ret.boundaryRequestPoints[i];
                                ret.boundaryRequestPoints[i] = new List<Int16Triple>();
                                tbir.boundarySeedsInside[OppositePos[i]] = oppInput;
                            }
                            queue.Push(t);
                        }
                        else
                        {
                            ret.boundaryRequestPoints[i].Clear();
                        }
                    }
                } 
                input.ClearAll(); 
            }
            ClearTail();
        }
        private void ClearTail()
        {
            for (int i = 0; i < image.GetBlocksCount().X; i++)
            {
                for (int j = 0; j < image.GetBlocksCount().Y; j++)
                {
                    for (int k = 0; k < image.GetBlocksCount().Z; k++)
                    {
                        if (blockIRMap[i, j, k] != null)
                        {
                            blockIRMap[i, j, k].Clear();
                        }
                    }
                }
            }
        }

        private void MarshalInputAndOutput(FloodFillInput input,FloodFillResult ret, Block block, BlockIR_FF bir)
        {
            if (bir.GetIsFirst())
            {
                input.width = block.actualWidth;
                input.height = block.actualHeight;
                input.depth = block.actualDepth;
                input.data = buffer;
                input.seed = bir.singleSeed;
                input.mask = block.boundaryMask;
                input.flagsMap = block.GetFlagMap3d();
                input.overstepList = null;
                ret.ClearResult();
            }
            else 
            {
                input.width = block.actualWidth;
                input.height = block.actualHeight;
                input.depth = block.actualDepth;
                input.data = buffer;
                input.mask = block.boundaryMask;
                input.flagsMap = block.GetFlagMap3d();
                input.seed = new Int16Triple(-1, -1, -1);
                input.overstepList = bir.boundarySeedsInside;
                ret.ClearResult();
            }
        }

        public void MergeResult(LargeSpanFillResult lsg, FloodFillResult ret)
        {
            lsg.resultSet.AddRange(ret.resultSet);
            lsg.Count += ret.resultCount;
        }


        private BlockIR_FF GetBlockIR(int x,int y,int z)
        {
            if (blockIRMap[x, y, z] == null)
            {
                blockIRMap[x, y, z] = new BlockIR_FF();
            }
            return blockIRMap[x, y, z];
        }


        private Block GetFirstBlock(Int16Triple seed)
        {
            if (seed.X < 0 || seed.X > image.GetWidth() || seed.Y < 0 || seed.Y > image.GetHeight() || seed.Z < 0 || seed.Z > image.GetDepth())
                throw new Exception();
            Int16Triple index = image.GetBlockIndex(seed);
            return image.GetBlock(index.X,index.Y,index.Z);
        }

        private void FillBlockData(Block b)
        {
            dataProvider.LoadBlockData(buffer,b.stx,b.sty,b.stz, b.edx, b.edy,b.edz);
        }

        private Int16Triple ConvertGlobalCoodToBlockCoord(Block thisblock, Int16Triple globalCoord)
        {
            return new Int16Triple(globalCoord.X-thisblock.stx, globalCoord.Y-thisblock.sty, globalCoord.Z - thisblock.stz);
        }

        private void ConvertBlockCoordsToGlobalCoords(Block thisblock, List<Int16Triple> pList)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                Int16Triple old = pList[i];
                old.X += thisblock.stx;
                old.Y += thisblock.sty;
                old.Z += thisblock.stz;
                pList[i] = old;
            }
        }

        private void ConvertThisBlockCoordsToOtherBlockCoords(Block thisblock, Block other, List<Int16Triple> pList)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                Int16Triple old = pList[i];
                old.X += (thisblock.stx - other.stx);
                old.Y += (thisblock.sty - other.sty);
                old.Z += (thisblock.stz - other.stz);
                pList[i] = old;
            }
        }

        public LargeSpanFillResult GetResult()
        {
            return resultSum;
        }

     

    }
}
