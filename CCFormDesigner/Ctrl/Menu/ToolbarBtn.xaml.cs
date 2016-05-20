using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Toolbar
{
	public partial class ToolbarBtn
	{
		public ToolbarBtn()
		{
			InitializeComponent();
            
			VerticalAlignment = VerticalAlignment.Top;

			Storyboard.SetTarget(Activate, this);
			Storyboard.SetTarget(Deactivate, this);
			
			MouseEnter += new MouseEventHandler(ToolbarBtn_MouseEnter);
			MouseLeave += new MouseEventHandler(ToolbarBtn_MouseLeave);
		}

		void SetToolbarZIndex(int nValue)
		{
			StackPanel panel = this.Parent as StackPanel;
            Bar toolbar = panel.Parent as Bar;
			toolbar.SetValue(Canvas.ZIndexProperty, nValue);
		}

		void ToolbarBtn_MouseLeave(object sender, MouseEventArgs e)
		{
			Deactivate.Begin();
			SetToolbarZIndex(0);
		}

		void ToolbarBtn_MouseEnter(object sender, MouseEventArgs e)
		{
			Activate.Begin();
			SetToolbarZIndex(3);
		}
	}
}
