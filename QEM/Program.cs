using System;
using System.Collections.Generic;
using System.Text;

namespace QEM
{
    class TestClass
    {
        public static void TestHeap()
        {
            float[] keys = new float[10] {8,9,19,23,1,2,44,15,8,9};
            HeapNode[] nodes=new HeapNode[keys.Length];
            for(int i=0;i<keys.Length;i++)
            {
                nodes[i] = new HeapNode(keys[i]);
            }
            RAHeap heap = new RAHeap(10);
            for (int i = 0; i < keys.Length; i++)
            {
                heap.Push(nodes[i]);
            }
            Console.WriteLine(heap.ToString());
            heap.Update(nodes[5],55);
            Console.WriteLine(heap.ToString());
        }
    }
    struct EdgeAndWeight:IComparable<EdgeAndWeight>
    {
        public int weight;
        public int edgeID;
        public int CompareTo(EdgeAndWeight other)
        {
            return this.weight.CompareTo(other.weight);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mesh m = Sample.GetSampleMesh2(100);
            PlyManager.Output(m, "D://VTKproj//sample.ply");
            ECMesh ecm = ECMesh.GetECMesh(m);
            List<EdgeAndWeight> edgeWeights = new List<EdgeAndWeight>(ecm.Edges.Count);
            Random r=new Random();
            for (int i = 0; i < ecm.Edges.Count; i++)
            {
                EdgeAndWeight ew = new EdgeAndWeight();
                ew.edgeID = i;
                ew.weight = r.Next(0, 10000);
                edgeWeights.Add(ew);
            }
            edgeWeights.Sort();
            int deciCount = ecm.Edges.Count/10;
            int index=0;
            while (deciCount != 0)
            {
                if (index == edgeWeights.Count)
                    break;
                EdgeAndWeight ew = edgeWeights[index];
                ECEdge edge = ecm.Edges[ew.edgeID];
                if (edge.IsValid() && edge.OrgV().Type == VertexType.INTERIOR && edge.DestV().Type == VertexType.INTERIOR)
                {
                    ECVertex v0 = edge.OrgV();
                    ECVertex v1 = edge.DestV();
                    Point3d mid = new Point3d(v0.X / 2 + v1.X / 2, v0.Y / 2 + v1.Y / 2, v0.Z / 2 + v1.Z / 2);
                    ecm.Contract(v0, v1, mid);
                    deciCount--;
                    index++;
                }
                else
                {
                    index++;
                    continue;
                }
            }
            m = ecm.GetMesh();
            PlyManager.Output(m, "test2.ply");
            Console.WriteLine("\nCMP!");
            Console.Read();
        }
    }
}
