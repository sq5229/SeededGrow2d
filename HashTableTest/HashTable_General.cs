using System;
using System.Collections.Generic;
using System.Text;

namespace HashTableTest
{
class HashTable_General<T>:IHashTable<T>
{
    Dictionary<Point3d, T> hashTable = new Dictionary<Point3d, T>();
    public void SetHashValue(int x, int y, int z, T value)
    {
        Point3d p = new Point3d(x, y, z);
        hashTable.Add(p, value);
    }

    public bool GetHashValue(int x, int y, int z, ref T value)
    {
        Point3d p = new Point3d(x, y, z);
        if (hashTable.ContainsKey(p))
        {
            value = hashTable[p];
            return true;
        }
        else
        {
            return false;
        }
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
