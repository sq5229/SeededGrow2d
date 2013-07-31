using System;
using System.Collections.Generic;
using System.Text;

namespace HashTableTest
{
public class HashTable_2dArray<T>:IHashTable<T>
{
    struct DepthAndValue<T1>
    {
        public int K;
        public T1 Value;
        public DepthAndValue(int k, T1 value)
        {
            K = k;
            Value = value;
        }
    }
    List<DepthAndValue<T>>[,] mapHash;
    int width;
    int height;
    int depth;
    public HashTable_2dArray(int width,int height,int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        mapHash = new List<DepthAndValue<T>>[this.width, this.height];
    }
    public void SetHashValue(int x, int y, int z, T value)
    {
        if (mapHash[x, y] == null)
        {
            mapHash[x, y ] = new List<DepthAndValue<T>>();
            mapHash[x, y].Add(new DepthAndValue<T>(z, value));
        }
        else
        {
            mapHash[x,y].Add(new DepthAndValue<T>(z, value));
        }
    }
    static int FindK(List<DepthAndValue<T>> list, int k)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].K == k)
                return i;
        }
        return -1;
    }
    public bool GetHashValue(int x, int y, int z, ref T value)
    {
        if (mapHash[x, y] != null)
        {
            int index = FindK(mapHash[x, y], z);
            if (index == -1)
            {
                return false;
            }
            else
            {
                value = mapHash[x, y][index].Value;
                return true;
            }
        }
        else
            return false;
    }
    public void SetDefaultValue(T value)
    {
        return;
    }
    public void Clear()
    {
        return;
    }
}
}
