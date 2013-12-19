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

    class Program
    {
        static void Main(string[] args)
        {
            Mesh m = Sample.GetSampleMesh1(5);
            ECMesh ecm = ECMesh.GetECMesh(m);
            int c = 0;
            for (int i = 0;; i++)
            {
                int r =( i * i * i + i * i) % ecm.Edges.Count;
                r += ecm.Edges.Count;
                r %= ecm.Edges.Count;
                ECEdge randomEdge = ecm.Edges[r];
                if (randomEdge.IsValid()&&randomEdge.OrgV().Type==VertexType.INTERIOR&&randomEdge.DestV().Type==VertexType.INTERIOR)
                {
                    c++;
                    if (c == 5)
                        break;
                    ECVertex v0 = randomEdge.OrgV();
                    ECVertex v1 = randomEdge.DestV();
                    Point3d mid = new Point3d(v0.X / 2 + v1.X / 2, v0.Y / 2 + v1.Y / 2, v0.Z / 2 + v1.Z / 2);
                    ecm.Contract(v0, v1, mid);
                }
                
            }
            m = ecm.GetMesh();
            //Console.WriteLine(ecm);
            PlyManager.Output(m, "test2.ply");
            Console.WriteLine("\nCMP!");
            Console.Read();
        }
    }
}
