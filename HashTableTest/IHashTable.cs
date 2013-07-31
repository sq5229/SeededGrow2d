using System;
using System.Collections.Generic;
using System.Text;

namespace HashTableTest
{
    interface IHashTable<T>
    {
        void SetHashValue(int x, int y, int z,T value);
        bool GetHashValue(int x, int y, int z,ref T value);
        void SetDefaultValue(T value);
        void Clear();
    }
}
