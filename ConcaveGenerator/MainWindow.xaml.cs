using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConcaveGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.BTN_Cal.Click += new RoutedEventHandler(BTN_Cal_Click);
            this.BTN_StateCheck.Click += new RoutedEventHandler(BTN_StateCheck_Click);
            this.BTN_Clear.Click += new RoutedEventHandler(BTN_Clear_Click);
            this.BTN_ClearR.Click += new RoutedEventHandler(BTN_ClearR_Click);
            this.CA_Main.MouseDown += new MouseButtonEventHandler(CA_Main_MouseDown);
            this.slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged);
            this.CHB_ShowAssis.Checked += new RoutedEventHandler(CHB_ShowAssis_Checked);
            this.CHB_ShowAssis.Unchecked += new RoutedEventHandler(CHB_ShowAssis_Checked);
            slider.Maximum = 400;
        }
        void CHB_ShowAssis_Checked(object sender, RoutedEventArgs e)
        {
            SetVisiibilityOfAlg();
        }
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SetVisibilitiesForTriUsingEdgelength(e.NewValue);
        }
        private void SetVisibilitiesForTriUsingDiameter(double p)
        {
            for (int i = 0; i < retMesh.Faces.Count; i++)
            {
                ConcaveGenerator.DelaunayConcave.Triangle t=retMesh.Faces[i];
                Point2d p0=retMesh.Points[t.P0Index];
                Point2d p1=retMesh.Points[t.P1Index];
                Point2d p2=retMesh.Points[t.P2Index];
                if (ConcaveGenerator.DelaunayConcave.DelaunayTriangulation.Diameter(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y)>=p)
                {
                    for (int j = 0; j < CA_Main.Children.Count; j++)
                    {
                        Line edge = CA_Main.Children[j] as Line;
                        if (edge != null && (int)edge.Tag == i)
                        {
                            edge.Visibility = Visibility.Hidden;
                        }
                    }
                 }
                else
                {
                    for (int j = 0; j < CA_Main.Children.Count; j++)
                    {
                        Line edge = CA_Main.Children[j] as Line;
                        if (edge != null && (int)edge.Tag == i)
                        {
                            edge.Visibility = Visibility.Visible;
                        }
                    }
                }
             }
        }
        private void SetVisibilitiesForTriUsingEdgelength(double p)
        {
            for (int j = 0; j < CA_Main.Children.Count; j++)
            {
                Line edge = CA_Main.Children[j] as Line;
                if (edge != null)
                {
                    double len = GetLineLength(edge);
                    if(len>=p)
                        edge.Visibility = Visibility.Hidden;
                    else
                        edge.Visibility = Visibility.Visible;
                }
            }   
        }
        private void BTN_ClearR_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < CA_Main.Children.Count;i++ )
            {
                if (CA_Main.Children[i] is Polyline)
                {
                    CA_Main.Children.RemoveAt(i);
                }
            }
        }
        private void BTN_Clear_Click(object sender, RoutedEventArgs e)
        {
            OutPutPoints();
            this.pointList.Clear();
            this.CA_Main.Children.Clear();
        }
        private void BTN_StateCheck_Click(object sender, RoutedEventArgs e)
        {
            BallConcave cc = new BallConcave(this.pointList);
            ShowMessage(GetRNeighboursMessage(cc));
            this.slider.Value = cc.GetRecomandedR() + 1;
        }
        private void CA_Main_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AddPoint(e.GetPosition(CA_Main));
        }
        private double R
        {
            get { return this.line.X2; }
        }
        private double GetLineLength(Line line)
        {
            return Math.Sqrt((line.X1 - line.X2) * (line.X1 - line.X2) + (line.Y1 - line.Y2) * (line.Y1 - line.Y2));
        }
        List<Point2d> pointList = new List<Point2d>();
        ConcaveGenerator.DelaunayConcave.DelaunayMesh2d retMesh=null;
        private void BTN_Cal_Click(object sender, RoutedEventArgs e)
        {
          
            if (CB_List.SelectedIndex == 0)
            {
                this.retMesh = DelaunayConcave.GetMesh(pointList);
                for (int i = 0; i < retMesh.Faces.Count; i++)
                {
                    ConcaveGenerator.DelaunayConcave.Triangle t = retMesh.Faces[i];
                    DrawLine_Delauney(retMesh.Points[t.P0Index], retMesh.Points[t.P1Index],"A0");
                    DrawLine_Delauney(retMesh.Points[t.P0Index], retMesh.Points[t.P2Index],"A0");
                    DrawLine_Delauney(retMesh.Points[t.P1Index], retMesh.Points[t.P2Index],"A0");
                }
                retMesh.InitEdgesInfo();
                retMesh.ExecuteEdgeDecimation(R);
                List<ConcaveGenerator.DelaunayConcave.EdgeInfo> edges = retMesh.GetBoundaryEdges();
                for (int i = 0; i < edges.Count; i++)
                {
                    DrawLine(retMesh.Points[edges[i].P0Index], retMesh.Points[edges[i].P1Index], null);
                }
            }
            if (CB_List.SelectedIndex == 1)
            {
                BallConcave cc = new BallConcave(this.pointList);
                if (cc.GetRecomandedR() > R)
                {
                    ShowMessage("R值不合法,可点检查参数！");
                    return;
                }
                cc.main = this;
                List<Point2d> ret = cc.GetConcave_Ball(R);
                DrawPolyline(ret);
            }
            if (CB_List.SelectedIndex == 2)
            {
                BallConcave cc = new BallConcave(this.pointList);
                if (cc.GetRecomandedR() > R)
                {
                    ShowMessage("R值不合法,可点检查参数！");
                    return;
                }
                cc.main = this;
                List<Point2d> ret = cc.GetConcave_Edge(R);
                DrawPolyline(ret);
            }
        }
        public string GetRNeighboursMessage(BallConcave cc)
        {
            List<int>[] nlist = cc.GetInRNeighbourList(this.R);
            StringBuilder sb=new StringBuilder();
            sb.Append("半径领域列表如下:\n");
            for (int i = 0; i < nlist.Length; i++)
            {
                sb.Append("P"+pointList[i].Index+"的R近邻点有"+nlist[i].Count+"个: ");
                for(int j=0;j<nlist[i].Count;j++)
                {
                    sb.Append(string.Format("P{0},", pointList[nlist[i][j]].Index));
                }
                sb.Append("\n");
            }
            sb.Append("执行算法的R值应大于: "+cc.GetRecomandedR());
            return sb.ToString();
        }
        public void SetVisiibilityOfAlg()
        {
            if (CB_List.SelectedIndex == 0)
            {
                if (this.CHB_ShowAssis.IsChecked==true)
                {
                    for (int i = 0; i < CA_Main.Children.Count; i++)
                    {
                        Line liner = CA_Main.Children[i] as Line;
                        if (liner != null &&liner.Tag!=null&& liner.Tag.ToString() == "A0")
                        {
                            liner.Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < CA_Main.Children.Count; i++)
                    {
                        Line liner = CA_Main.Children[i] as Line;
                        if (liner != null && liner.Tag != null && liner.Tag.ToString() == "A0")
                        {
                            liner.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            if (CB_List.SelectedIndex == 1)
            {
                if (this.CHB_ShowAssis.IsChecked==true)
                {
                    for (int i = 0; i < CA_Main.Children.Count; i++)
                    {
                        FrameworkElement fe=(CA_Main.Children[i] as FrameworkElement);
                        if (fe!=null&&fe.Tag!=null&&fe.Tag.ToString() == "A1")
                        {
                            CA_Main.Children[i].Visibility = Visibility.Visible;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < CA_Main.Children.Count; i++)
                    {
                        FrameworkElement fe = (CA_Main.Children[i] as FrameworkElement);
                        if (fe != null &&fe.Tag!=null &&fe.Tag.ToString() == "A1")
                        {
                            CA_Main.Children[i].Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            if (CB_List.SelectedIndex == 2)
            {
                if (this.CHB_ShowAssis.IsChecked == true)
                {

                }
                else
                {

                }
            }
        }
        public void AddPoint(Point p)
        {
            this.pointList.Add(new Point2d((int)p.X,(int)p.Y,pointList.Count));
            DrawPoint((int)p.X, (int)p.Y);
        }
        public void DrawPoint(double x, double y)
        {
            Ellipse ep = new Ellipse();
            ep.Tag = pointList.Count - 1;
            ep.Style = this.FindResource("STY_ELP") as Style;
            this.CA_Main.Children.Add(ep);
            Canvas.SetLeft(ep, x - ep.Width / 2);
            Canvas.SetTop(ep, y - ep.Height / 2);
            TextBlock tb = new TextBlock();
            tb.Style = this.FindResource("STY_TB") as Style;
            this.CA_Main.Children.Add(tb);
            Canvas.SetLeft(tb, x + ep.Width / 2);
            Canvas.SetTop(tb, y + ep.Height / 2);
            //tb.Text = string.Format("P{0}({1},{2})", pointList.Count - 1, (int)x, (int)y);
            tb.Text = string.Format("P{0}", pointList.Count - 1);
        }
        public void DrawLine(Point2d p1, Point2d p2, object tag)
        {
            Line line1 = new Line();
            line1.Tag = tag;
            line1.Width = CA_Main.Width;
            line1.Height = CA_Main.Height;
            line1.X1 = p1.X;
            line1.X2 = p2.X;
            line1.Y1 = p1.Y;
            line1.Y2 = p2.Y;
            line1.Stroke = new SolidColorBrush(Colors.Black);
            line1.StrokeThickness = 2;
            CA_Main.Children.Add(line1);
        }
        public void DrawLine_Delauney(Point2d p1, Point2d p2, object tag)
        {
            Line line1 = new Line();
            line1.Tag = tag;
            line1.Width = CA_Main.Width;
            line1.Height = CA_Main.Height;
            line1.X1 = p1.X;
            line1.X2 = p2.X;
            line1.Y1 = p1.Y;
            line1.Y2 = p2.Y;
            line1.Stroke = new SolidColorBrush(Colors.Blue);
            line1.StrokeThickness = 1;
            line1.Visibility = Visibility.Hidden;
            CA_Main.Children.Add(line1);
        }
        public void DrawR_Ball(Point2d p1, Point2d p2)
        {
            Line line1 = new Line();
            line1.Tag = "A1";
            line1.Width = CA_Main.Width;
            line1.Height = CA_Main.Height;
            line1.X1 = p1.X;
            line1.X2 = p2.X;
            line1.Y1 = p1.Y;
            line1.Y2 = p2.Y;
            line1.StrokeDashArray = new DoubleCollection(new double[] { 3, 2 });
            line1.Stroke = new SolidColorBrush(Colors.Green);
            line1.StrokeThickness = 1;
            line1.Visibility = Visibility.Hidden;
            CA_Main.Children.Add(line1);
        }
        public void DrawCircle_Ball(Point2d center,double radius)
        {
            Ellipse p = new Ellipse();
            p.Tag = "A1";
            p.Width = 2*radius+2;
            p.Height = 2*radius+2;
            p.Fill = null;
            p.Stroke = new SolidColorBrush(Colors.Red);
            p.StrokeThickness =1;
            CA_Main.Children.Add(p);
            p.Visibility = Visibility.Hidden;
            Canvas.SetLeft(p, center.X - radius-1);
            Canvas.SetTop(p, center.Y - radius-1);
        }
        public void DrawCircleWithXian(Point2d center,Point2d p1,Point2d p2, double radius)
        {
            DrawCircle_Ball(center, radius);
            DrawR_Ball(center, p1);
            DrawR_Ball(center, p2);
        }
        public void DrawPolyline(List<Point2d> list)
        {
            Polyline line = new Polyline();
            line.Width = CA_Main.Width;
            line.Height = CA_Main.Height;
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 2;
            for(int i=0;i<list.Count;i++)
            {
                line.Points.Add(new Point(list[i].X, list[i].Y));
            }
           // line.Points.Add(new Point(list[0].X, list[0].Y));
            CA_Main.Children.Add(line);
        }
        public void ShowMessage(string message)
        {
            TB_MEG.Text = message+Environment.NewLine+Environment.NewLine + TB_MEG.Text;
        }
        int c = -1;
        private void OutPutPoints()
        {
            c++;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pointList.Count; i++)
            {
                sb.Append("line"+c+".AddPoint2d("+pointList[i].X+","+pointList[i].Y+");"+Environment.NewLine);
            }
            System.IO.File.WriteAllText("points"+c+".txt",sb.ToString());
        }
    }
}
