using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeededBlockFill3d.Block.SP
{
    public class LargeSeededGrowResult
    {
        public int Count = 0;
        public List<Int16Triple> resultSet = new List<Int16Triple>();

    }
    public class BlockIR_SP
    {
        public Int16Triple singleSeed;
        public List<Int16Triple>[] boundarySeedXsInside;
        public List<Range>[] boundaryRangesYZInside;
        public BlockIR_SP()
        {
            boundarySeedXsInside = new List<Int16Triple>[2];
            boundaryRangesYZInside = new List<Range>[4];
            for (int i = 0; i < 2; i++)
            {
                boundarySeedXsInside[i] = new List<Int16Triple>();
            }
            for (int i = 0; i < 4; i++)
            {
                boundaryRangesYZInside[i] = new List<Range>();
            }
            singleSeed = new Int16Triple(-1, -1, -1);
        }
        public bool GetIsFirst()
        {
            return singleSeed.X != -1;
        }
        public override string  ToString()
        {
            return string.Format("[{0},{1},{2},{3}][{4},{5}][{6}]", boundaryRangesYZInside[0].Count, boundaryRangesYZInside[1].Count, boundaryRangesYZInside[2].Count, boundaryRangesYZInside[3].Count, boundarySeedXsInside[0].Count, boundarySeedXsInside[1].Count, singleSeed);
        }
        public void Clear()
        {
            for (int i = 0; i < 2; i++)
            {
                boundarySeedXsInside[i].Clear();
                boundarySeedXsInside[i] = null;
            }
            for (int i = 0; i < 4; i++)
            {
                boundaryRangesYZInside[i].Clear();
                boundaryRangesYZInside[i] = null;
            }
        }
    }
    public class B_LargeSeededGrowManager_SpanFill
    {
        #region Static
        static int[] OppositePos_X = new int[2] { 1, 0};
        static int[] OppositePos_YZ = new int[4] {1, 0, 3, 2};
        public static Int16Triple[] AdX = new Int16Triple[2] 
        {
            new Int16Triple(-1,0,0),
            new Int16Triple(1,0,0),
        };
        public static Int16Triple[] AdYZ = new Int16Triple[4] 
        {
            new Int16Triple(0,-1,0),
            new Int16Triple(0,1,0),
            new Int16Triple(0,0,-1),
            new Int16Triple(0,0,1),
        };
        #endregion
        B_LargeImage image;
        Container_Stack<Block> queue;
        byte[] buffer;
        B_SpanFillBase seedGrowExecutor;
        DataFiller dataProvider;
        LargeSeededGrowResult resultSum;
        BlockIR_SP[, ,] blockIRMap;
        public void SetScale(int width, int height, int depth, int subwidth, int subheight, int subdepth)
        {
            this.image = new B_LargeImage(width, height, depth);
            SetBlockSize(subwidth, subheight, subdepth);
            blockIRMap = new BlockIR_SP[image.GetBlocksCount().X, image.GetBlocksCount().Y, image.GetBlocksCount().Z];
        }

        private void SetBlockSize(int subwidth, int subheight, int subdepth)
        {
            image.CreateBlocks(subwidth, subheight, subdepth);
            buffer = new byte[subwidth * subheight * subdepth];
        }

        public void SetExecutor(B_SpanFillBase grower)
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
            SpanFillInput input = new SpanFillInput();
            SpanFillResult ret = new SpanFillResult();
            resultSum = new LargeSeededGrowResult();
            queue = new Container_Stack<Block>();
            Block firstB = GetFirstBlock(firstseed);
            BlockIR_SP firstBir = GetBlockIR(firstB.indexX, firstB.indexY, firstB.indexZ);
            firstBir.singleSeed = ConvertGlobalCoodToBlockCoord(firstB, firstseed);
            queue.Push(firstB);
            while (!queue.Empty())
            {
                Block block = queue.Pop();
                block.VisitedCount++;
                BlockIR_SP bir = GetBlockIR(block.indexX, block.indexY, block.indexZ);
                FillBlockData(block);

                MarshalInputAndOutput(input, ret, block, bir);
                seedGrowExecutor.ExecuteSeededGrow(input, ret);

                if (input.GetIsFirst())
                    bir.singleSeed = new Int16Triple(-1, -1, -1);

                ConvertBlockCoordsToGlobalCoords(block, ret.resultSet);
                MergeResult(resultSum, ret);


                for (int i = 0; i < 2;i++ )
                {
                    if (ret.GetNeedsToSeekX(i))
                    {
                        Block t = image.GetBlock(block.indexX + AdX[i].X, block.indexY, block.indexZ);
                        if (t.VisitedCount < 1)
                        {
                            ConvertThisBlockCoordsToOtherBlockCoords(block, t, ret.boundaryPoints_X[i]);
                            BlockIR_SP tbir = GetBlockIR(t.indexX, t.indexY, t.indexZ);
                            List<Int16Triple> oppInput = tbir.boundarySeedXsInside[OppositePos_X[i]];
                            if (oppInput != null && oppInput.Count != 0)
                            {
                                oppInput.AddRange(ret.boundaryPoints_X[i]);
                                ret.boundaryPoints_X[i].Clear();
                                tbir.boundarySeedXsInside[OppositePos_X[i]] = oppInput;
                            }
                            else
                            {
                                oppInput = ret.boundaryPoints_X[i];
                                ret.boundaryPoints_X[i] = new List<Int16Triple>();
                                tbir.boundarySeedXsInside[OppositePos_X[i]] = oppInput;
                            }
                            queue.Push(t);
                        }
                        else
                        {
                            ret.boundaryPoints_X[i].Clear();
                        }
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (ret.GetNeedsToSeekYZ(i))
                    {
                        Block t = image.GetBlock(block.indexX, block.indexY + AdYZ[i].Y, block.indexZ + AdYZ[i].Z);
                        if (t.VisitedCount < 1000)
                        {
                            ConvertThisBlockRangesToOtherBlockRanges(block, t, ret.boundaryRanges_YZ[i]);
                            BlockIR_SP tbir = GetBlockIR(t.indexX, t.indexY, t.indexZ);
                            List<Range> oppInput = tbir.boundaryRangesYZInside[OppositePos_YZ[i]];
                            if (oppInput != null && oppInput.Count != 0)
                            {
                                oppInput.AddRange(ret.boundaryRanges_YZ[i]);
                                ret.boundaryRanges_YZ[i].Clear();
                                tbir.boundaryRangesYZInside[OppositePos_YZ[i]] = oppInput;
                            }
                            else
                            {
                                oppInput = ret.boundaryRanges_YZ[i];
                                ret.boundaryRanges_YZ[i] = new List<Range>();
                                tbir.boundaryRangesYZInside[OppositePos_YZ[i]] = oppInput;
                            }
                            queue.Push(t);
                        }
                        else
                        {
                            ret.boundaryRanges_YZ[i].Clear();
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

        private void MarshalInputAndOutput(SpanFillInput input, SpanFillResult ret, Block block, BlockIR_SP bir)
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
                input.overstepPointList = null;
                input.overstepRangeList = null;
                ret.resultCount=0;
                ret.resultSet.Clear();
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
                input.overstepPointList = bir.boundarySeedXsInside;
                input.overstepRangeList = bir.boundaryRangesYZInside;
                ret.resultCount = 0;
                ret.resultSet.Clear();
            }
        }

        public void MergeResult(LargeSeededGrowResult lsg, SpanFillResult ret)
        {
            lsg.resultSet.AddRange(ret.resultSet);
            lsg.Count += ret.resultCount;
        }

        private BlockIR_SP GetBlockIR(int x, int y, int z)
        {
            if (blockIRMap[x, y, z] == null)
            {
                blockIRMap[x, y, z] = new BlockIR_SP();
            }
            return blockIRMap[x, y, z];
        }

        private Block GetFirstBlock(Int16Triple seed)
        {
            if (seed.X < 0 || seed.X > image.GetWidth() || seed.Y < 0 || seed.Y > image.GetHeight() || seed.Z < 0 || seed.Z > image.GetDepth())
                throw new Exception();
            Int16Triple index = image.GetBlockIndex(seed);
            return image.GetBlock(index.X, index.Y, index.Z);
        }

        private void FillBlockData(Block b)
        {
            dataProvider.LoadBlockData(buffer, b.stx, b.sty, b.stz, b.edx, b.edy, b.edz);
        }

        private Int16Triple ConvertGlobalCoodToBlockCoord(Block thisblock, Int16Triple globalCoord)
        {
            return new Int16Triple(globalCoord.X - thisblock.stx, globalCoord.Y - thisblock.sty, globalCoord.Z - thisblock.stz);
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

        private void ConvertThisBlockRangesToOtherBlockRanges(Block thisblock, Block other, List<Range> pList)
        {
            for (int i = 0; i < pList.Count; i++)
            {
                Range old = pList[i];
                old.Y += (thisblock.sty - other.sty);
                old.Z += (thisblock.stz - other.stz);
                pList[i] = old;
            }
        }

        public LargeSeededGrowResult GetResult()
        {
            return resultSum;
        }



    }
}
