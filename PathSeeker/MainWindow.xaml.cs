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
using PathSeeker.PathSeekMethod;
using System.Reflection;
using Microsoft.Expression.Media;
using Microsoft.Expression.Shapes;
using System.IO;

namespace PathSeeker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class ButtonProperty
        {
            public int X;
            public int Y;
            public int Type;
            public ButtonProperty(int x, int y, int type)
            {
                this.X = x;
                this.Y = y;
                this.Type = type;
                this.Parent = -1;
            }
            public int Parent;
            public int Distance;
            public int EvaDistance;
          
        }
        public MainWindow()
        {
            InitializeComponent();
            if (ReadFileMap("map.txt"))
            {

            }
            else
            {
                this.SL_Size.Value = 10;
                this.TBK_Size.Text = "Size:" + 10;
                InitMap(15);
            }
            this.SL_Size.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SL_Size_ValueChanged);
            this.BTN_Confirm.Click += new RoutedEventHandler(BTN_Confirm_Click);
            BTN_Clear.Click += new RoutedEventHandler(BTN_Clear_Click);
        }
        int blockWidth = 40;
        bool notClear = false;
        void BTN_Clear_Click(object sender, RoutedEventArgs e)
        {
            InitMap(size);
        }
        void BTN_Confirm_Click(object sender, RoutedEventArgs e)
        {
            ClearResult();
            if (CB_Algs.SelectedIndex==0)
            {
                GetMap();
                BFS_Manhattan bfs = new BFS_Manhattan(map, size);
                ResultCollection ret = bfs.GetResult();
                ShowResult(ret);
            }
            if (CB_Algs.SelectedIndex == 1)
            {
                GetMap();
                DijkstraProcessor dp = new DijkstraProcessor(map, size);
                ResultCollection ret = dp.GetResult();
                ShowResult(ret);
            }
            if (CB_Algs.SelectedIndex == 2)
            {
                GetMap();
                AStarProcessor dp = new AStarProcessor(map, size);
                ResultCollection ret = dp.GetResult();
                ShowResult(ret);
            }
        }
        void SL_Size_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.TBK_Size.Text = "Size:" + (int)SL_Size.Value;
            if (this.size != (int)SL_Size.Value)
            {
                this.size = (int)SL_Size.Value;
                InitMap(size);
            }
        }
        void b_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Button b = (Button)sender;
            ButtonProperty bp = b.Tag as ButtonProperty;
            if (bp != null)
            {
                StringBuilder sb = new StringBuilder();
                FieldInfo[] fields = typeof(ButtonProperty).GetFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    string name = fields[i].Name;
                    string v = fields[i].GetValue(bp).ToString();
                    sb.Append(name + "=" + v + Environment.NewLine);
                }
                ShowMessage(sb.ToString());
            }
        }
        void b_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            ButtonProperty bp = b.Tag as ButtonProperty;
            if (bp.Type == 0)
            {
                bp.Type = 1;
                b.Tag = bp;
                b.Style = this.FindResource("ObstacleButtonStyle") as Style;
            }
        }
        void b_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button b = (Button)sender;
            ButtonProperty bp = b.Tag as ButtonProperty;
            if (bp.Type != 0)
                return;
            setCount++;
            if (setCount % 2 == 1)
            {
                if (st == null)
                {
                    st = bp;
                    if (bp.Type != 2)
                    {
                        bp.Type = 2;
                        b.Tag = bp;
                        b.Style = this.FindResource("StartButtonStyle") as Style;
                    }
                }
                else
                {
                    Button pre = buttonMap[st.X, st.Y];
                    ButtonProperty prebp = pre.Tag as ButtonProperty;
                    prebp.Type = 0;
                    pre.Tag = prebp;
                    pre.Style = this.FindResource("CommonButtonStyle") as Style;

                    st = bp;
                    bp.Type = 2;
                    b.Tag = bp;
                    b.Style = this.FindResource("StartButtonStyle") as Style;
                }

            }
            else
            {
                if (ed == null)
                {
                    ed = bp;
                    if (bp.Type != 3)
                    {
                        bp.Type = 3;
                        b.Tag = bp;
                        b.Style = this.FindResource("EndButtonStyle") as Style;
                    }
                }
                else
                {
                    Button pre = buttonMap[ed.X, ed.Y];

                    ButtonProperty prebp = pre.Tag as ButtonProperty;
                    prebp.Type = 0;
                    pre.Tag = prebp;
                    pre.Style = this.FindResource("CommonButtonStyle") as Style;

                    ed = bp;
                    bp.Type = 3;
                    b.Tag = bp;
                    b.Style = this.FindResource("EndButtonStyle") as Style;
                }
            }

        }
        
        int[,] map;
        Button[,] buttonMap;
        int size;
        int setCount;
        ButtonProperty st;
        ButtonProperty ed;
        private bool ReadFileMap(string path)
        {
            string content;
            try
            {
                content = File.ReadAllText(path);
                content = content.Replace("\r\n", "@");
            }
            catch (System.Exception ex)
            {
                return false;
            }
            string[] lines = content.Split('@');
            if (lines.Length < 3 || lines[0].Length != lines.Length)
            {
                return false;
            }
            this.size = lines.Length;
            InitMap(size);
            for (int j = 0; j < lines.Length; j++)
            {
                for (int i = 0; i < lines[j].Length;i++)
                {
                    if (lines[j][i] == '0')
                    {
                        map[i,j]= 0;
                    }
                    if (lines[j][i] == '1')
                    {
                        map[i, j] = 1;
                        Button b = buttonMap[i, j];
                        ButtonProperty bp = b.Tag as ButtonProperty;
                        if (bp.Type == 0)
                        {
                            bp.Type = 1;
                            b.Tag = bp;
                            b.Style = this.FindResource("ObstacleButtonStyle") as Style;
                        }
                    }
                    if (lines[j][i] == '2')
                    {
                        map[i, j] = 2;
                        Button b = buttonMap[i, j];
                        ButtonProperty bp = b.Tag as ButtonProperty;
                        st = bp;
                        if (bp.Type != 2)
                        {
                            bp.Type = 2;
                            b.Tag = bp;
                            b.Style = this.FindResource("StartButtonStyle") as Style;
                        }
                    }
                    if (lines[j][i] == '3')
                    {
                        map[i, j] = 3;
                        Button b = buttonMap[i, j];
                        ButtonProperty bp = b.Tag as ButtonProperty;
                        ed = bp;
                        if (bp.Type != 3)
                        {
                            bp.Type = 3;
                            b.Tag = bp;
                            b.Style = this.FindResource("EndButtonStyle") as Style;
                        }
                    }
                }
            }
            return true;
        }
        private void InitMap(int size)
        {
            this.st = null;
            this.ed = null;
            this.size = size;
            this.map = new int[size, size];
            this.buttonMap = new Button[size, size];
            int count = 0;
            for (int i = 0; i < CA_Main.Children.Count; i++)
            {
                Button b = CA_Main.Children[i] as Button;
                if (b != null)
                {
                    count++;
                    b.Style = this.FindResource("CommonButtonStyle") as Style;
                    b.Visibility = Visibility.Collapsed;
                    if (b.Content != null)
                    {
                        ArrowControl ar = b.Content as ArrowControl;
                        if(ar!=null)
                            ar.Visibility = Visibility.Hidden;
                    }
                }
            }
            if (count < size * size)
            {
                for (int i = 0; i < size * size - count; i++)
                {
                    Button b = new Button();
                    b.Style = this.FindResource("CommonButtonStyle") as Style;
                    b.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(b_PreviewMouseLeftButtonDown);
                    b.PreviewMouseRightButtonDown += new MouseButtonEventHandler(b_PreviewMouseRightButtonDown);
                    b.PreviewMouseMove += new MouseEventHandler(b_PreviewMouseMove);
                    CA_Main.Children.Add(b);
                    ArrowControl arrow = new ArrowControl();
                    arrow.Width = blockWidth - 1;
                    arrow.Height = blockWidth - 1;
                    arrow.HorizontalAlignment = HorizontalAlignment.Center;
                    arrow.VerticalAlignment = VerticalAlignment.Center;
                    b.Content = arrow;
                    arrow.Visibility = Visibility.Hidden;
                }
            }
            int index = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ButtonProperty bp = new ButtonProperty(i, j, 0);
                    Button b = (CA_Main.Children[index] as Button);
                    buttonMap[i, j] = b;
                    b.Tag = bp;
                    index++;
                    Canvas.SetLeft(b, bp.X * blockWidth);
                    Canvas.SetTop(b, bp.Y * blockWidth);
                    b.Visibility = Visibility.Visible;
                }
            }
        }
        private void ClearResult()
        {
            for (int i = 0; i < CA_Main.Children.Count; i++)
            {
                Button b = CA_Main.Children[i] as Button;
                if (b != null)
                {
                    ButtonProperty bp = b.Tag as ButtonProperty;
                    if (bp.Type == 4 || bp.Type == 5)
                    {
                        bp.Type = 0;
                        bp.Distance = -1;
                        bp.EvaDistance = -1;
                        bp.Parent = -1;
                        b.Style = this.FindResource("CommonButtonStyle") as Style;
                    }
                    if (b.Content != null)
                    {
                        ArrowControl ar = b.Content as ArrowControl;
                        ar.SetDistance(0);
                        ar.SetEvaDistance(0);
                        ar.SetSumDistance(0);
                        if (ar != null)
                            ar.Visibility = Visibility.Hidden;
                    }
                }
            }
        }
        private int[,] GetMap()
        {
            if (buttonMap.GetLength(0) != map.GetLength(0))
                throw new Exception();
            this.size = map.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Button b = buttonMap[i, j];
                    ButtonProperty bp = b.Tag as ButtonProperty;
                    this.map[i, j] = bp.Type;
                }
            }
            return this.map;
        }
        private void ShowResult(ResultCollection ret)
        {
            if (ret.parentMap != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Button b = buttonMap[i, j];
                        ArrowControl arrow = b.Content as ArrowControl;
                        ButtonProperty bp = buttonMap[i, j].Tag as ButtonProperty;
                        if (ret.parentMap[i, j] != -1)
                        {
                            arrow.Visibility = Visibility.Visible;
                            bp.Parent = ret.parentMap[i, j];
                            arrow.SetDirection(ret.parentMap[i, j]);
                        }
                    }
                }
            }
            if (ret.resultMap != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Button b = buttonMap[i, j];
                        ButtonProperty bp = buttonMap[i, j].Tag as ButtonProperty;
                        int type = ret.resultMap[i, j];
                        if (type == 4)
                        {
                            b.Style = FindResource("PathButtonStyle") as Style;
                            bp.Type = type;
                            b.Tag = bp;
                        }
                        if (type == 5)
                        {
                            b.Style = FindResource("TraveledButtonStyle") as Style;
                            bp.Type = type;
                            b.Tag = bp;
                        }
                    }
                }
            }
            if (ret.distanceMap != null)
            {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        Button b = buttonMap[i, j];
                        ArrowControl arrow = b.Content as ArrowControl;
                        ButtonProperty bp = buttonMap[i, j].Tag as ButtonProperty;
                        if (ret.distanceMap[i, j] != int.MaxValue)
                        {
                            arrow.Visibility = Visibility.Visible;
                            bp.Parent = ret.parentMap[i, j];
                            bp.Distance = ret.distanceMap[i, j];
                            if (ret.evaDistanceMap != null)
                                bp.EvaDistance = ret.evaDistanceMap[i, j];
                            arrow.SetDistance(ret.distanceMap[i, j]);
                            if (ret.evaDistanceMap != null)
                                arrow.SetEvaDistance(ret.evaDistanceMap[i, j]);
                            else 
                                arrow.SetEvaDistance(0);
                            if (ret.evaDistanceMap != null)
                                arrow.SetSumDistance(ret.distanceMap[i, j] + ret.evaDistanceMap[i, j]);
                            else
                                arrow.SetEvaDistance(0);
                        }
                    }
                }
            }
        }
        private void ShowMessage(string mes)
        {
            this.TBK_Message.Text = mes;
        }
    }
}
