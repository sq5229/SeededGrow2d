using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace SeededGrow2d
{
    public struct Int16Double
    {
        public int X;
        public int Y;
        public Int16Double(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class BitMap2d
    {
        public const byte WHITE = 255;
        public const byte BLACK = 0;
        public byte[] data;
        public int width;
        public int height;
        public int action_set_count;
        public int action_get_count;
        public BitMap2d(int width, int height, byte v)
        {
            this.width = width;
            this.height = height;
            data = new byte[width * height];
            action_set_count = 0;
            action_get_count = 0;
            for (int i = 0; i < width * height; i++)
                data[i] = v;
        }
        public BitMap2d(byte[] data, int width, int height)
        {
            this.data = data;
            this.width = width;
            this.height = height;
        }
        public void SetPixel(int x, int y, byte v)
        {
            action_set_count++;
            data[x + y * width] = v;
        }
        public byte GetPixel(int x, int y)
        {
            action_get_count++;
            return data[x + y * width];
        }
        public void ResetVisitCount()
        {
            action_get_count = 0;
            action_set_count = 0;
        }


        public void ReadBitmap(string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(path);
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    System.Drawing.Color c = bmp.GetPixel(i, j);
                    byte v = (c.R == 255) ? WHITE : BLACK;
                    this.SetPixel(i, j, v);
                }
            }
            bmp.Dispose();
        }
        public void OutputBitMap(string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(this.width, this.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    byte s = this.GetPixel(i, j);
                    if (s > 128)
                        bmp.SetPixel(i, j, System.Drawing.Color.White);
                    else
                        bmp.SetPixel(i, j, System.Drawing.Color.Black);
                }
            }
            bmp.Save(path);
            bmp.Dispose();
        }
    }

    public class FlagMap2d
    {
        public int action_set_count;
        public int action_get_count;
        public int width;
        public int height;
        BitArray flags;
        public FlagMap2d(int width, int height)
        {
            this.width = width;
            this.height = height;
            action_get_count = 0;
            action_set_count = 0;
            flags = new BitArray(width * height, false);
        }
        public void SetFlagOn(int x, int y, bool v)
        {
            flags[x + y * width] = v;
            action_set_count++;
        }
        public bool GetFlagOn(int x, int y)
        {
            action_get_count++;
            return flags[x + y * width];
        }

        public int GetCount()
        {
            int sum = 0;
            for (int i = 0; i < flags.Count; i++)
            {
                if (flags[i])
                    sum++;
            }
            return sum;
        }
    }

    public class Container<T>
    {
        public int action_push_count;
        public int action_pop_count;
        public int max_contain_count;
        public Container()
        {
            max_contain_count = 0;
            action_pop_count = 0;
            action_push_count = 0;
        }
        public virtual void Push(T v)
        {
        }
        public virtual T Pop()
        {
            return default(T);
        }
        public virtual bool Empty()
        {
            return false;
        }
    }

    public class Container_Queue<T> : Container<T>
    {
        Queue<T> queue;
        public Container_Queue()
        {
            queue = new Queue<T>();
        }
        public override void Push(T v)
        {
            action_push_count++;
            if (queue.Count > max_contain_count)
                max_contain_count = queue.Count;
            queue.Enqueue(v);
        }
        public override T Pop()
        {
            action_pop_count++;
            T value = queue.Dequeue();
            return value;
        }
        public override bool Empty()
        {
            return queue.Count == 0;
        }
    }

    public class Container_Stack<T> : Container<T>
    {
        Stack<T> stack;
        public Container_Stack()
        {
            stack = new Stack<T>();
        }
        public override void Push(T v)
        {
            action_push_count++;
            if (stack.Count > max_contain_count)
                max_contain_count = stack.Count;
            stack.Push(v);
        }
        public override T Pop()
        {
            action_pop_count++;
            T value = stack.Pop();
            return value;
        }
        public override bool Empty()
        {
            return stack.Count == 0;
        }
    }

    public struct ResultReport
    {
        public int result_point_count;
        public int flag_set_count;
        public int flag_get_count;
        public int bmp_get_count;
        public int bmp_set_count;
        public int container_push_count;
        public int container_pop_count;
        public int container_max_size;
        public long time;
        public void PrintInfo()
        {
            Console.Write("*******************************************\n");
            Console.Write(string.Format("Time: {0}ms . 结果的点数: {1} .\n", this.time, this.result_point_count));
            Console.Write(string.Format("bitmap的Get次数: {0} , 比例为{1} .\n", this.bmp_get_count, (float)this.bmp_get_count / this.result_point_count));
            Console.Write(string.Format("flag的Set次数: {0}  , 比例为{1} .\n", this.flag_set_count, (float)this.flag_set_count / this.result_point_count));
            Console.Write(string.Format("flag的Get次数: {0}  , 比例为{1} .\n", this.flag_get_count, (float)this.flag_get_count / this.result_point_count));
            Console.Write(string.Format("总存取次数: {0} , 比例为{1}  .\n", this.flag_get_count + this.bmp_get_count + this.flag_set_count, (float)(this.flag_get_count + this.bmp_get_count + this.flag_set_count) / this.result_point_count));
            Console.Write(string.Format("container的Push/Pop次数: {0}  , 比例为{1} .\n", this.container_push_count, (float)this.container_push_count / this.result_point_count));
            Console.Write(string.Format("container的MaxSize数: {0}，比例为{1} .\n", this.container_max_size, (float)(this.container_max_size) / this.result_point_count));
            Console.Write(string.Format("总操作比例: {0} .\n", (float)(this.flag_get_count + this.bmp_get_count + this.flag_set_count + 2 * this.container_push_count) / this.result_point_count));
        }

        public void OutPutMap(FlagMap2d flagsmap, string path)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(flagsmap.width, flagsmap.height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            for (int i = 0; i < flagsmap.width; i++)
            {
                for (int j = 0; j < flagsmap.height; j++)
                {
                    bool s = flagsmap.GetFlagOn(i, j);
                    if (s)
                        bmp.SetPixel(i, j, System.Drawing.Color.White);
                    else
                        bmp.SetPixel(i, j, System.Drawing.Color.Black);
                }
            }
            bmp.Save(path);
        }
    }
}
