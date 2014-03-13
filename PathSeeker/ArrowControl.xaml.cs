using System;
using System.Collections.Generic;
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
using Microsoft.Expression.Shapes;

namespace PathSeeker
{
	/// <summary>
	/// Interaction logic for ArrowControl.xaml
	/// </summary>
	public partial class ArrowControl : UserControl
	{
		public ArrowControl()
		{
			this.InitializeComponent();
            this.arrows = new BlockArrow[8] {AR_Left,AR_Right,AR_Up,AR_Down,AR_LeftUp,AR_RightUp,AR_LeftDown,AR_RightDown };
            for (int i = 0; i < arrows.Length; i++)
            {
                arrows[i].Visibility = Visibility.Hidden;
            }
		}
        private BlockArrow[] arrows;
        private int direction;
        public void SetDirection(int index)
        {
            if (index >= 0 && index < 8)
            {
                this.direction = index;
                for (int i = 0; i < arrows.Length; i++)
                {
                    arrows[i].Visibility = Visibility.Hidden;
                }
                this.arrows[index].Visibility = Visibility.Visible;
            }
            else
                throw new Exception();
        }

        public void SetEvaDistance(int dis)
        {
            this.TBK_1.Text = dis.ToString();
        }
        public void SetSumDistance(int dis)
        {
            this.TBK_3.Text = dis.ToString();
        }
        public void SetDistance(int dis)
        {
            this.TBK_2.Text = dis.ToString();
        }
	}
}