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

namespace BP.Controls
{
	public partial class Toolbar : UserControl
	{
		public Toolbar()
		{
			InitializeComponent();	
		}

		public UIElementCollection Children
		{
			get
			{
				return ButtonStack.Children;
			}
		}
        public void AddBtn(ToolbarButton btn)
        {
            this.Children.Add(btn);
        }
        public void AddBtn(string id, string text, int width)
        {
            ToolbarButton btn = new ToolbarButton();
            btn.Name = id;
            btn.Content = text;
            btn.Width = width;
            this.Children.Add(btn);
        }
	}
}
