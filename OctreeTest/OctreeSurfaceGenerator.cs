using System;
using System.Collections.Generic;
using System.Text;

namespace OctreeTest
{
    public class NodeParms
    {
        public byte Config;
        public byte NormalTypeId;
        public int D;
        public override string ToString()
        {
            if (NormalTypeId < OctreeTable.NormalTypeIdToNormal.Length)
                return "\"" + NormalTypeId + "," + D + "\"";
            else
                return "not simple";
        }
    }
    public static class OctreeSurfaceGenerator
    {
        #region consts
        public static byte VULF = 1 << 0;
        public static byte VULB = 1 << 1;
        public static byte VLLB = 1 << 2;
        public static byte VLLF = 1 << 3;
        public static byte VURF = 1 << 4;
        public static byte VURB = 1 << 5;
        public static byte VLRB = 1 << 6;
        public static byte VLRF = 1 << 7;
        //以上为体素为实点的位标记
        public static Int16Triple[] PointIndexToPointDelta = new Int16Triple[8]
        {
            new Int16Triple(0, 1, 1 ),
            new Int16Triple(0, 1, 0 ),
            new Int16Triple(0, 0, 0 ),
            new Int16Triple(0, 0, 1 ),
            new Int16Triple(1, 1, 1 ),
            new Int16Triple(1, 1, 0 ),
            new Int16Triple(1, 0, 0 ),
            new Int16Triple(1, 0, 1 )
        };//体元内每个体素相对基准体素坐标的偏移
        public static byte[] PointIndexToFlag = new byte[8]
        {
            VULF,
            VULB,
            VLLB,
            VLLF,
            VURF,
            VURB,
            VLRB,
            VLRF
        };//每个体素对应的位标记
        public static int[] VertexVoxelIndex = new int[8]
        {
            2,6,1,5,3,7,0,4
        };
        public static byte BLACK = 0;
        public static byte WHITE = 255;
        public static int[] MidVoxelIndex = new int[8]
        {
            4,0,7,3,5,1,6,2
        };
        #endregion
        public static Mesh GenerateSurface(BitMap3d bmp)
        {
            int width = bmp.width;
            int height = bmp.height;
            int depth = bmp.depth;
            Int16Triple[] tempArray = new Int16Triple[8];
            #region CreateTree
            RegionOctree<NodeParms> otree = new RegionOctree<NodeParms>(width, height, depth);
            Queue<OctreeNode<NodeParms>> nodequeue = new Queue<OctreeNode<NodeParms>>();
            for (int k = 0; k < depth - 1; k++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    for (int i = 0; i < width - 1; i++)
                    {
                        byte value = 0;
                        for (int pi = 0; pi < 8; pi++)
                        {
                            tempArray[pi].X = i + PointIndexToPointDelta[pi].X;
                            tempArray[pi].Y = j + PointIndexToPointDelta[pi].Y;
                            tempArray[pi].Z = k + PointIndexToPointDelta[pi].Z;
                            if (InRange(bmp, tempArray[pi].X, tempArray[pi].Y, tempArray[pi].Z) &&
                                IsWhite(bmp, tempArray[pi].X, tempArray[pi].Y, tempArray[pi].Z))
                            {
                                value |= PointIndexToFlag[pi];
                            }
                        }
                        if (value != 0 && value != 255)
                        {
                            OctreeNode<NodeParms> leafnode = otree.CreateToLeafNode(i, j, k);
                            leafnode.Parms = new NodeParms();
                            leafnode.Parms.Config = value;
                            leafnode.Parms.NormalTypeId = OctreeTable.ConfigToNormalTypeId[value];
                            leafnode.Parms.D = CaculateDFromNormalAndCoord(i, j, k, value);
                            nodequeue.Enqueue(leafnode.Parent);
                        }
                    }
                }
            }
            #endregion
            #region Shrink
            while (nodequeue.Count != 0)
            {
                OctreeNode<NodeParms> node = nodequeue.Dequeue();
                byte normalType = OctreeTable.NormalNotSimple;
                int D = int.MinValue;
                if (CanMergeNode(node, ref normalType, ref D))
                {
                    node.Parms = new NodeParms();
                    //node.Parms.Config = GetConfigFromChildren(node.Children);
                    node.Parms.NormalTypeId = normalType;
                    node.Parms.D = D;
                    nodequeue.Enqueue(node.Parent);
                }
            }
            #endregion
            #region ExtractTriangles
            MeshBuilder_IntegerVertex mb = new MeshBuilder_IntegerVertex(width + 1, height + 1, depth + 1);
            nodequeue.Enqueue(otree.Root);
            while (nodequeue.Count != 0)
            {
                OctreeNode<NodeParms> node = nodequeue.Dequeue();
                if (node.Parms == null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (node.Children[i] != null)
                            nodequeue.Enqueue(node.Children[i]);
                    }
                }
                else
                {
                    if (node.Parms.NormalTypeId != OctreeTable.NormalNotSimple)
                    {
                        if (node.IsLeaf())
                        {
                            GenerateFaceLeaf(node, mb, ref tempArray, bmp);
                        }
                        else
                        {
                            GenerateFace(node, mb, ref tempArray, bmp);
                        }
                    }
                    else
                    {
                        if (node.IsLeaf())
                        {
                            GenerateFaceLeaf(node, mb, ref tempArray, bmp);
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                if (node.Children[i] != null)
                                    nodequeue.Enqueue(node.Children[i]);
                            }
                        }
                    }
                }
            }//采用层次遍历寻找需要抽取三角片的节点
            #endregion
            return mb.GetMesh();
        }

        private static bool InRange(BitMap3d bmp, int x, int y, int z)
        {
            return x > 0 && x < bmp.width && y > 0 && y < bmp.height && z > 0 && z < bmp.depth;
        }

        private static bool IsWhite(BitMap3d bmp, int x, int y, int z)
        {
            return bmp.GetPixel(x, y, z) == BitMap3d.WHITE;
        }

        private static void GenerateFace(OctreeNode<NodeParms> node, MeshBuilder_IntegerVertex mb, ref Int16Triple[] tempArray, BitMap3d bmp)
        {
            InitVoxelPositionForNodeRange(node.XMin, node.XMax, node.YMin, node.YMax, node.ZMin, node.ZMax, ref tempArray);
            //需要找到该节点的端点位置
            byte cubeConfig = 0;
            for (int pi = 0; pi < 8; pi++)
            {
                if (InRange(bmp, tempArray[pi].X, tempArray[pi].Y, tempArray[pi].Z)
                    && IsWhite(bmp, tempArray[pi].X, tempArray[pi].Y, tempArray[pi].Z))
                {
                    cubeConfig |= PointIndexToFlag[pi];
                }
            }
            int index = 0;
            while (MCTable.TriTable[cubeConfig, index] != -1)
            {
                int ei1 = MCTable.TriTable[cubeConfig, index];
                int ei2 = MCTable.TriTable[cubeConfig, index + 1];
                int ei3 = MCTable.TriTable[cubeConfig, index + 2];

                Int16Triple p1 = GetIntersetedPointAtEdge(node, ei1, OctreeTable.NormalTypeIdToNormal[node.Parms.NormalTypeId], node.Parms.D);
                Int16Triple p2 = GetIntersetedPointAtEdge(node, ei2, OctreeTable.NormalTypeIdToNormal[node.Parms.NormalTypeId], node.Parms.D);
                Int16Triple p3 = GetIntersetedPointAtEdge(node, ei3, OctreeTable.NormalTypeIdToNormal[node.Parms.NormalTypeId], node.Parms.D);

                mb.AddTriangle(p1, p2, p3);
                index += 3;
            }
        }//对非叶子节点的超体元的抽取需要参考MCTable求取被截断边的信息

        private static void GenerateFaceLeaf(OctreeNode<NodeParms> node, MeshBuilder_IntegerVertex mb, ref Int16Triple[] tempArray, BitMap3d bmp)
        {
            for (int k = 0; k < 8; k++)
            {
                tempArray[k].X = node.XMin + PointIndexToPointDelta[k].X;
                tempArray[k].Y = node.YMin + PointIndexToPointDelta[k].Y;
                tempArray[k].Z = node.ZMin + PointIndexToPointDelta[k].Z;
            }
            byte value = node.Parms.Config;
            int index = 0;
            while (SMCTable.TableFat[value, index] != -1)
            {
                Int16Triple t0 = tempArray[SMCTable.TableFat[value, index]];
                Int16Triple t1 = tempArray[SMCTable.TableFat[value, index + 1]];
                Int16Triple t2 = tempArray[SMCTable.TableFat[value, index + 2]];
                mb.AddTriangle(t0, t1, t2);
                index += 3;
            }
        }//对叶子节点的单位体元的抽取和SMC算法中的抽取一致

        private static bool CanMergeNode(OctreeNode<NodeParms> node, ref byte normalType, ref int D)
        {
            for (int i = 0; i < 8; i++)
            {
                if (node.Children[i] != null)
                {
                    if (node.Children[i].Parms == null)
                    {
                        return false;//说明其下存在不能合并的节点 合并失败
                    }
                    else
                    {
                        if (node.Children[i].Parms.NormalTypeId != OctreeTable.NormalNotSimple)
                        {
                            normalType = node.Children[i].Parms.NormalTypeId;
                            D = node.Children[i].Parms.D;//记录其中的共面配置信息
                        }
                        else
                        {
                            return false;//遇到了非共面配置 合并失败
                        }
                    }
                }
            }
            for (int i = 0; i < 8; i++)
            {
                if (node.Children[i] != null)
                {
                    if (node.Children[i].Parms.NormalTypeId != normalType || node.Children[i].Parms.D != D)
                    {
                        return false;//体元配置均为共面类型但不是同一种的话也不中
                    }
                }
            }
            return true;
        }

        private static Int16Triple GetIntersetedPointAtEdge(OctreeNode<NodeParms> node, int edgeIndex, Int16Triple normal, int d)
        {
            int x = 0, y = 0, z = 0;
            switch (edgeIndex)
            {
                case 0: { x = node.XMin; y = node.YMax + 1; return new Int16Triple(x, y, (d - normal.X * x - normal.Y * y) / normal.Z); }
                case 2: { x = node.XMin; y = node.YMin; return new Int16Triple(x, y, (d - normal.X * x - normal.Y * y) / normal.Z); }
                case 4: { x = node.XMax + 1; y = node.YMax + 1; return new Int16Triple(x, y, (d - normal.X * x - normal.Y * y) / normal.Z); }
                case 6: { x = node.XMax + 1; y = node.YMin; return new Int16Triple(x, y, (d - normal.X * x - normal.Y * y) / normal.Z); }

                case 8: { y = node.YMax + 1; z = node.ZMax + 1; return new Int16Triple((d - normal.Y * y - normal.Z * z) / normal.X, y, z); }
                case 9: { y = node.YMax + 1; z = node.ZMin; return new Int16Triple((d - normal.Y * y - normal.Z * z) / normal.X, y, z); }
                case 10: { y = node.YMin; z = node.ZMin; return new Int16Triple((d - normal.Y * y - normal.Z * z) / normal.X, y, z); }
                case 11: { y = node.YMin; z = node.ZMax + 1; return new Int16Triple((d - normal.Y * y - normal.Z * z) / normal.X, y, z); }

                case 1: { x = node.XMin; z = node.ZMin; return new Int16Triple(x, (d - normal.X * x - normal.Z * z) / normal.Y, z); }
                case 3: { x = node.XMin; z = node.ZMax + 1; return new Int16Triple(x, (d - normal.X * x - normal.Z * z) / normal.Y, z); }
                case 5: { x = node.XMax + 1; z = node.ZMin; return new Int16Triple(x, (d - normal.X * x - normal.Z * z) / normal.Y, z); }
                case 7: { x = node.XMax + 1; z = node.ZMax + 1; return new Int16Triple(x, (d - normal.X * x - normal.Z * z) / normal.Y, z); }

                default: throw new Exception();
            }
        }

        private static int CaculateDFromNormalAndCoord(int cx, int cy, int cz, byte config)
        {
            byte index = OctreeTable.ConfigToEqType[config];
            if (index >= OctreeTable.EqTypeToEqQuad.Length)
                return int.MinValue;
            Int32Quad eq = OctreeTable.EqTypeToEqQuad[index];
            return eq.D + eq.A * cx + eq.B * cy + eq.C * cz;
        }

        private static void InitVoxelPositionForNodeRange(int xmin, int xmax, int ymin, int ymax, int zmin, int zmax, ref Int16Triple[] temp)
        {
            temp[0].X = xmin;  //(0, 1, 1, VULF);
            temp[0].Y = ymax + 1;
            temp[0].Z = zmax + 1;

            temp[1].X = xmin;//(0, 1, 0, VULB);
            temp[1].Y = ymax + 1;
            temp[1].Z = zmin;

            temp[2].X = xmin;//(0, 0, 0, VLLB);
            temp[2].Y = ymin;
            temp[2].Z = zmin;

            temp[3].X = xmin;//(0, 0, 1, VLLF);
            temp[3].Y = ymin;
            temp[3].Z = zmax + 1;

            temp[4].X = xmax + 1;//(1, 1, 1, VURF);
            temp[4].Y = ymax + 1;
            temp[4].Z = zmax + 1;

            temp[5].X = xmax + 1; //(1, 1, 0, VURB);
            temp[5].Y = ymax + 1;
            temp[5].Z = zmin;

            temp[6].X = xmax + 1;//(1, 0, 0, VLRB);
            temp[6].Y = ymin;
            temp[6].Z = zmin;

            temp[7].X = xmax + 1;  //(1, 0, 1, VLRF);
            temp[7].Y = ymin;
            temp[7].Z = zmax + 1;
        }

        //private static byte GetConfigFromChildren(OctreeNode<NodeParms>[] children)
        //{
        //    byte firstc=0;
        //    int index = -1;
        //    for (int i = 0; i < 8; i++)
        //    {
        //        if (children[i] != null&&children[i].Parms!=null)
        //        {
        //            firstc = children[i].Parms.Config;
        //            index = i;
        //            break;
        //        }
        //    }
        //    byte midPointValue= (((firstc)&(PointIndexToFlag[index]))==0)?BLACK:WHITE;
        //    byte ret = 0;
        //    for (int i = 0; i < 8; i++)
        //    {
        //        byte config = 0;
        //        if (children[i] != null && children[i].Parms != null)
        //        {
        //            config = children[i].Parms.Config;
        //        }
        //        else
        //        {
        //            config = midPointValue;
        //        }
        //        ret |= ((config)&(PointIndexToFlag[VertexVoxelIndex[i]]));
        //    }
        //    return ret;
        //}

    }
}
