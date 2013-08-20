
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace SeededBlockFill3d
{
    public class DataFiller
    {
        byte[] allData;
        int width;
        int height;
        int depth;
        public void Initialize(TestParms test)
        {
            allData = test.image.data;
            width = test.image.width;
            height = test.image.height;
            depth = test.image.depth;
        }
        public void LoadBlockData(byte[] target, int stx, int sty, int stz, int edx, int edy, int edz)
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
        public void LoadLayerData(byte[] target,int stz, int edz)
        {
            int st=stz*width*height;
            int ed=edz*width*height+width*height-1;
            for (int i = st; i <= ed; i++)
            {
                target[i - st] = allData[i];
            }
        }
    }
}
