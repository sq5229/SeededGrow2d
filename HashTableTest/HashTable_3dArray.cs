using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace HashTableTest
{
class HashTable_3dArray<T>:IHashTable<T>
{
    T[,,] array3d;
    int width;
    int height;
    int depth;
    T defaultValue;
    public HashTable_3dArray(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        array3d = new T[width,height,depth];
    }

    public void SetHashValue(int x, int y, int z,T value)
    {
        array3d[x ,y , z] = value;
    }

    public bool GetHashValue(int x, int y, int z,ref T value)
    {
        value = array3d[x ,y , z];
        return true;
    }
    public void Clear()
    {
        return;
    }

    public void SetDefaultValue(T value)
    {
        defaultValue = value;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < depth; k++)
                {
                    array3d[i, j, k] = value;
                }
            }
        }
    }
}
}
