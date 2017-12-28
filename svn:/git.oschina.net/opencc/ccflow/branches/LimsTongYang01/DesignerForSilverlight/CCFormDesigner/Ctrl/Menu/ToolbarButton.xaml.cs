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
	public partial class ToolbarButton
	{
        public ToolbarButton()
        {
            InitializeComponent();

            this.VerticalAlignment = VerticalAlignment.Top;

            Storyboard.SetTarget(this.Activate, this);
            Storyboard.SetTarget(this.Deactivate, this);

            MouseEnter += new MouseEventHandler(ToolbarButton_MouseEnter);
            MouseLeave += new MouseEventHandler(ToolbarButton_MouseLeave);
        }

		void SetToolbarZIndex(int nValue)
		{
			StackPanel panel = this.Parent as StackPanel;
            Bar toolbar = panel.Parent as Bar;
			toolbar.SetValue(Canvas.ZIndexProperty, nValue);
		}

		void ToolbarButton_MouseLeave(object sender, MouseEventArgs e)
		{
			Deactivate.Begin();
			SetToolbarZIndex(0);
		}
		void ToolbarButton_MouseEnter(object sender, MouseEventArgs e)
		{
			Activate.Begin();
			SetToolbarZIndex(3);
		}
	}
}
