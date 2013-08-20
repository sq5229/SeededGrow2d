using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SeededBlockFill3d
{
    class IO
    {
        public static void WriteXYZFile(List<Int16Triple> list, string fileName)
        {
            FileStream fs = File.Create(fileName);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.WriteLine(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                sw.WriteLine(list[i].X + " " + list[i].Y + " " + list[i].Z);
            }
            sw.Close();
            fs.Close();
        }
    }
}
