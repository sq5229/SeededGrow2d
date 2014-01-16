using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GrowingStrategies.Methods;
using System.Drawing.Imaging;

namespace GrowingStrategies
{
    public partial class Form1 : Form
    {
        private BitMap2d bmp;
        private Int16Double seed1;
        public Form1()
        {
            InitializeComponent();
            bmp = new BitMap2d(PB_OR.Width,PB_OR.Height,0);
            bmp.ReadBitmap(@"F:\workspacecsharp\RecentProject\SeededGrow2d\GrowingStrategies\Image\228_274.bmp");
            this.PB_OR.MouseMove += new MouseEventHandler(TMouseMove);
            this.PB_TE.MouseMove += new MouseEventHandler(TMouseMove);
            this.PB_T.MouseMove += new MouseEventHandler(TMouseMove);
            this.PB_CF.MouseMove += new MouseEventHandler(TMouseMove);
            this.PB_O.MouseMove += new MouseEventHandler(TMouseMove);

            PB_T.MouseClick += new MouseEventHandler(PB_MouseClick);
            PB_TE.MouseClick += new MouseEventHandler(PB_MouseClick);
            PB_CF.MouseClick += new MouseEventHandler(PB_MouseClick);
            PB_O.MouseClick += new MouseEventHandler(PB_MouseClick);

            this.BTN_1.Click += new EventHandler(BTN_EX_Click);
            this.BTN_2.Click += new EventHandler(BTN_2_Click);
            this.BTN_3.Click += new EventHandler(BTN_3_Click);
            this.BTN_4.Click += new EventHandler(BTN_4_Click);
        }

        
        void PB_MouseClick(object sender, MouseEventArgs e)
        {
            seed1.X = e.X;
            seed1.Y = e.Y;
            byte v = bmp.GetPixel(e.X, e.Y);
            label1.Text = "seed 1: " + e.X + " " + e.Y + " value: " + v;
        }
        
        void TMouseMove(object sender, MouseEventArgs e)
        {
            ShowMsg("Position: "+e.X+" "+e.Y+" value: "+bmp.GetPixel(e.X,e.Y));
        }

        public void ShowMsg(string mes)
        {
            RTB_Message.Text = mes;
        }

        void BTN_EX_Click(object sender, EventArgs e)
        {
            string p1 = TB_T.Text;

            if (!string.IsNullOrEmpty(p1))
            {
                string[] ps = p1.Split(' ');
                byte min = Convert.ToByte(ps[0]);
                byte max = Convert.ToByte(ps[1]);
                Threshold thres = new Threshold(bmp, seed1, min, max);
                thres.ExcuteFloodFill();
                ShowResult(thres.results, PB_T, label2);
            }   
           
        }
        void BTN_2_Click(object sender, EventArgs e)
        {
            string p2 = TB_TE.Text;
            if (!string.IsNullOrEmpty(p2))
            {
                string[] ps = p2.Split(' ');
                byte min = Convert.ToByte(ps[0]);
                byte max = Convert.ToByte(ps[1]);
                int r = Convert.ToByte(ps[2]);
                Threshold_Ero thres = new Threshold_Ero(bmp, seed1, min, max, r);
                thres.ExcuteFloodFill();
                ShowResult(thres.results, PB_TE, label3);
            }
        }
        void BTN_3_Click(object sender, EventArgs e)
        {
            string p3 = TB_CF.Text;
            if (!string.IsNullOrEmpty(p3))
            {
                string[] ps = p3.Split(' ');
                int iter = Convert.ToInt32(ps[0]);
                double factor = Convert.ToDouble(ps[1]);
                int r = Convert.ToByte(ps[2]);
                Confidence thres = new Confidence(bmp, seed1, iter, factor, r);
                thres.ExcuteFloodFill();
                ShowResult(thres.results, PB_CF, label4);
            }
        }
        void BTN_4_Click(object sender, EventArgs e)
        {
            string p4 = TB_O.Text;
            if (!string.IsNullOrEmpty(p4))
            {
                byte r = Convert.ToByte(p4);
                GradientGrow thres = new GradientGrow(bmp, seed1, r);
                thres.ExcuteFloodFill();
                ShowResult(thres.results, PB_O, label5);
            }
        }
       
        

        void ShowResult(List<Int16Double> ret, PictureBox pb,Label l)
        {
            Graphics g = pb.CreateGraphics();
            BitMap2d bmpnew = new BitMap2d(this.bmp.width,this.bmp.height,0);
            Array.Copy(bmp.data, bmpnew.data, bmpnew.height * bmpnew.width);
            for (int i = 0; i < ret.Count; i++)
            {
                bmpnew.SetPixel(ret[i].X, ret[i].Y, 0);
                g.DrawRectangle(new Pen(Color.Red), ret[i].X, ret[i].Y, 1, 1);
            }
            l.Text = "result count: " + ret.Count;
        }
    }
}
