
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.IO;

namespace SeededBlockFill3d
{
    public abstract class DataFiller
    {
        protected int width;
        protected int height;
        protected int depth;
        public abstract void Initialize(TestParms test);
        public abstract void LoadBlockData(byte[] target, int stx, int sty, int stz, int edx, int edy, int edz);
        public abstract void LoadLayerData(byte[] target, int stz, int edz);
    }
    public class DataFiller_Simulation:DataFiller
    {
        protected byte[] allData;
        public override void Initialize(TestParms test)
        {
            test.LoadData();
            allData = test.image.data;
            width = test.image.width;
            height = test.image.height;
            depth = test.image.depth;
        }

        public override void LoadBlockData(byte[] target, int stx, int sty, int stz, int edx, int edy, int edz)
        {
            int subwidth = edx - stx + 1;
            int subheight = edy - sty + 1;
            int subdepth = edz - stz + 1;
            for (int k = stz; k <= edz; k++)
            {
                for (int j = sty; j <= edy; j++)
                {
                    for (int i = stx; i <= edx; i++)
                    {
                        target[(i - stx) + subwidth * (j - sty) + subwidth * subheight * (k - stz)] = allData[i + width * j + width * height * k];
                    }
                }
            }
        }
        public override void LoadLayerData(byte[] target, int stz, int edz)
        {
            int st=stz*width*height;
            int ed=edz*width*height+width*height-1;
            for (int i = st; i <= ed; i++)
            {
                target[i - st] = allData[i];
            }
        }
    }
    public class DataFiller_MMF:DataFiller
    {
        MemoryMappedFile mmf ;
        MemoryMappedViewAccessor accessor; 
        public override void Initialize(TestParms test)
        {
            this.width=test.image.width;
            this.height = test.image.height;
            this.depth = test.image.depth;
            mmf = MemoryMappedFile.CreateFromFile(test.path, FileMode.OpenOrCreate, "chnhide", 120 * 1024 * 1024);
            accessor= mmf.CreateViewAccessor();
        }
        public override void LoadLayerData(byte[] target, int stz, int edz)
        {
            int st = stz * width * height;
            int ed = edz * width * height + width * height - 1;
            accessor.ReadArray<byte>(st, target,0, ed - st + 1);
        }

        public override void LoadBlockData(byte[] target, int stx, int sty, int stz, int edx, int edy, int edz)
        {
            throw new NotImplementedException();
        }
        public void Close()
        {
            mmf.Dispose();
        }
    }
}
