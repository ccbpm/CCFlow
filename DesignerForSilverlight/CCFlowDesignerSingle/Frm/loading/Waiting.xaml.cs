using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace BP.Frm
{
    public partial class Waiting : ChildWindow
    {
        public Waiting()
        {
            InitializeComponent();
            this.Storyboard_loading_win8.Begin();
        }
    }
}

