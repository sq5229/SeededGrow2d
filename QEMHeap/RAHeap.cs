using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QEMHeap
{
    public class HeapNode
    {
        float Key;
        int indexInHeap;
        public HeapNode(float key)
        {
            this.Key = key;
            this.indexInHeap = -1;
        }
        public void SetIndexInHeap(int index)
        {
            this.indexInHeap = index;
        }
        public int GetIndexInHeap()
        {
            return indexInHeap;
        }
        public float GetKey()
        {
            return Key;
        }
        public void SetKey(float key)
        {
            this.Key = key;
        }
        public override string ToString()
        {
            return string.Format("[key:{0},index:{1}]", Key, indexInHeap);
        }
    }
    public class RAHeap
    {
        HeapNode[] array;
        int size;

        public RAHeap(int maxCapacity)
        {
            size = 0;
            array = new HeapNode[maxCapacity];
        }
        public void Push(HeapNode node)
        {
            if (size == array.Length)
            {
                throw new Exception();
            }
            size++;
            int i = size - 1;
            array[i] = node;
            array[i].SetKey(node.GetKey());
            array[i].SetIndexInHeap(i);
            ShiftUp(i);
        }
        public void Update(HeapNode node, float newKeyValue)
        {
            int pos = node.GetIndexInHeap();
            if (pos >= size || pos < 0)
                throw new Exception();
            float oldkey = array[pos].GetKey();
            array[pos].SetKey(newKeyValue);
            if (newKeyValue < oldkey)
                ShiftDown(pos);
            else
                ShiftUp(pos);
        }
        public void Delete(HeapNode node)
        {
            int i = node.GetIndexInHeap();
            if (i < 0 || i > size)
                throw new Exception();
            Swap(i, size - 1);
            size--;
            array[size].SetIndexInHeap(-1);
            if (array[i].GetKey() < array[size].GetKey())
                ShiftDown(i);
            else
                ShiftUp(i);
        }
        public HeapNode Pop()
        {
            if (size == 0)
                throw new Exception();
            Swap(0, size - 1);
            size--;
            ShiftDown(0);
            array[size].SetIndexInHeap(-1);
            return array[size];
        }
        public int GetSize()
        {
            return size;
        }


        private void Swap(int i, int j)
        {
            HeapNode temp = array[i];
            array[i] = array[j];
            array[j] = temp;
            array[i].SetIndexInHeap(i);
            array[j].SetIndexInHeap(j);
        }
        private int GetParent(int i)
        {
            return (i - 1) / 2;
        }
        private int GetLeftChild(int i)
        {
            return 2 * i + 1;
        }
        private int GetRightChild(int i)
        {
            return 2 * i + 2;
        }
        private void ShiftUp(int i)
        {
            if (i == 0)
                return;
            else
            {
                int parent = GetParent(i);
                if (array[i].GetKey() > array[parent].GetKey())
                {
                    Swap(i, parent);
                    ShiftUp(parent);
                }
            }
        }
        private void ShiftDown(int i)
        {
            if (i >= size) return;
            int large = i;
            int lc = GetLeftChild(i);
            int rc = GetRightChild(i);
            if (lc < size && array[lc].GetKey() > array[large].GetKey())
                large = lc;
            if (rc < size && array[rc].GetKey() > array[large].GetKey())
                large = rc;
            if (large != i)
            {
                Swap(i, large);
                ShiftDown(large);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < size; i++)
            {
                sb.Append(array[i].ToString());
            }
            return sb.ToString();
        }
    }
}
