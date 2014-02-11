using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QEMHeap
{
    class Program
    {
        static void Main(string[] args)
        {
            float[] keys = new float[10] { 8, 9, 19, 23, 1, 2, 44, 15, 8, 9 };
            HeapNode[] nodes = new HeapNode[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                nodes[i] = new HeapNode(keys[i]);
            }
            RAHeap heap = new RAHeap(10);
            for (int i = 0; i < keys.Length; i++)
            {
                heap.Push(nodes[i]);
            }
            Console.WriteLine(heap.ToString());
            heap.Update(nodes[5], 55);
            Console.WriteLine(heap.ToString());
        }
    }
}
