using System;
using System.Collections.Generic;
using System.Text;

namespace OctreeTest
{
public class OctreeNode<T>
{
    public OctreeNode<T>[] Children;//孩子指针,数组大小为8
    public OctreeNode<T> Parent;//父节点指针
    public T Parms;//携带的参数
    public int XMin;//所代表范围的X轴下界
    public int YMin;//所代表范围的Y轴下界
    public int ZMin;//所代表范围的Z轴下界
    public int XMax;//所代表范围的X轴下界
    public int YMax;//所代表范围的Y轴下界
    public int ZMax;//所代表范围的Z轴下界
    public int IndexInParent;//自己在父节点孩子数组中的索引
    public int LayerIndex;//自己所在的层索引
    public bool IsLeaf()
    {
        return (XMin == XMax)&&(YMin==YMax)&&(ZMin==ZMax);
    }//返回是否是叶子节点
    public OctreeNode()
    {
    }
    public override string ToString()
    {
        if (IsLeaf())
        {
            return string.Format("[{0},{1}][{2},{3}][{4},{5}] {6} Leaf", XMin, XMax, YMin, YMax, ZMin, ZMax,Parms);
        }
        if (Parms == null)
            return string.Format("[{0},{1}][{2},{3}][{4},{5}] {6}", XMin, XMax, YMin, YMax, ZMin, ZMax, "not simple"); ;
        return string.Format("[{0},{1}][{2},{3}][{4},{5}] {6}", XMin, XMax, YMin, YMax, ZMin, ZMax,Parms);
    }
}//BON八叉树节点
public class RegionOctree<T>
{
    private static int GetMax2Power(int xmax,int ymax,int zmax,ref int log)
    {
        int max = xmax;
        if (ymax > max)
            max = ymax;
        if (zmax > max)
            max = zmax;
        if ((max & (max - 1)) == 0)
        {
            double L = Math.Log(max, 2);
            log = (int)L + 1;
            return max;
        }
        else
        {
            double L = Math.Log(max, 2);
            log = (int)L + 2;
            return (int)Math.Pow(2, log - 1);
        }
    }
    private int Width;//树所关联空间范围的X上界
    private int Height;//树所关联空间范围的Y上界
    private int Depth;//树所关联空间范围的Z上界
    public OctreeNode<T> Root;//树根节点
    public  int NodeCount;//所有节点总数
    public  int LeafCount;//叶子节点
    private int Scale;//2的幂包围盒边长
    private int LayerNum;//层次数
    private OctreeNode<T>[] NodeLayers;//指代一条由根通往叶子的路径

    public RegionOctree(int width,int height,int depth)//使用范围构造BON树
    {
        this.Width = width;
        this.Height = height;
        this.Depth = depth;
        Scale = GetMax2Power(Width,Height,Depth,ref LayerNum);
        NodeCount = 0;
        Root = new OctreeNode<T>();
        Root.XMin = 0;
        Root.XMax = Scale-1;
        Root.YMin = 0;
        Root.YMax = Scale-1;
        Root.ZMin = 0;
        Root.ZMax = Scale-1;
        Root.Parent = null;
        Root.IndexInParent = -1;
        Root.LayerIndex = LayerNum - 1;
        Root.Children = new OctreeNode<T>[8];
        NodeLayers = new OctreeNode<T>[LayerNum];
        NodeLayers[0] = Root;
    }

    public OctreeNode<T> CreateToLeafNode(int x,int y,int z)
    {
        LeafCount++;
        for (int i = 1; i <= LayerNum - 1; i++)
        {
            int index = GetIndexOn(x, y, z, LayerNum - i-1);
            if (NodeLayers[i - 1].Children[index] == null)
            {
                OctreeNode<T> node = new OctreeNode<T>();
                NodeCount++;
                node.Parent = NodeLayers[i - 1];
                node.IndexInParent = index;
                node.Children = new OctreeNode<T>[8];
                node.LayerIndex = NodeLayers[i - 1].LayerIndex - 1;
                InitRangeByParentAndIndex(node, NodeLayers[i - 1], index);
                NodeLayers[i - 1].Children[index] = node;
            }
            NodeLayers[i]=NodeLayers[i-1].Children[index];
        }
        return NodeLayers[NodeLayers.Length - 1];
    }//将关联着坐标（x，y，z）处元素一路插入到底层为叶子节点

    private int GetIndexOn(int x, int y, int z, int bitindex)
    {
        int ret = 0;
        if ((x & (1 << bitindex)) != 0)
        {
            ret |= 1;
        }
        if ((y & (1 << bitindex)) != 0)
        {
            ret |= 2;
        }
        if ((z & (1 << bitindex)) != 0)
        {
            ret |= 4;
        }
        return ret;
    }

    private void InitRangeByParentAndIndex(OctreeNode<T> node,OctreeNode<T> pnode, int index)
    {
        int deltaX = (pnode.XMax - pnode.XMin + 1) / 2;
        int deltaY = (pnode.YMax - pnode.YMin + 1) / 2;
        int deltaZ = (pnode.ZMax -  pnode.ZMin + 1) / 2;
        if ((index & 1) == 0)
        {
            node.XMin = pnode.XMin;
            node.XMax = pnode.XMin + deltaX - 1;
        }
        else
        {
            node.XMin = pnode.XMin + deltaX;
            node.XMax = pnode.XMax;
        }
        if ((index & 2) == 0)
        {
            node.YMin = pnode.YMin;
            node.YMax = pnode.YMin + deltaY - 1;
        }
        else
        {
            node.YMin = pnode.YMin + deltaY;
            node.YMax = pnode.YMax;
        }
        if ((index & 4) == 0)
        {
            node.ZMin = pnode.ZMin;
            node.ZMax = pnode.ZMin + deltaZ - 1;
        }
        else
        {
            node.ZMin = pnode.ZMin + deltaZ;
            node.ZMax = pnode.ZMax;
        }
    }//使用父节点的信息初始化子节点的范围

    public static void Test()
    {
        RegionOctree<byte> tree = new RegionOctree<byte>(2047, 480, 1250);
        //tree.AddLeafNode(0, 0, 0,0);
        tree.CreateToLeafNode(0, 0, 0);
        tree.CreateToLeafNode(0, 0, 1);
        tree.CreateToLeafNode(0, 0, 2);
        tree.CreateToLeafNode(0, 0, 3);
        tree.CreateToLeafNode(0, 0, 4);
        tree.CreateToLeafNode(0, 0, 5);
        tree.CreateToLeafNode(0, 0, 6);
        tree.CreateToLeafNode(0, 0, 7);
        tree.CreateToLeafNode(0, 0, 8);
        tree.CreateToLeafNode(0, 0, 9);
        tree.CreateToLeafNode(0, 0, 10);
        tree.CreateToLeafNode(0, 0, 11);
           
        Console.WriteLine(tree);
    }

}
}
