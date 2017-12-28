using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.Text;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
using System.Windows.Media.Imaging;
using BP.En;

namespace CCForm.Ctrl
{
    public partial class TempleteFile : UserControl
    {
        public TempleteFile()
        {
            InitializeComponent();
        }
        public void BindText(string t)
        {
            BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/TempleteFile.png", UriKind.Relative));
            this.image1.Source = png;
            this.label1.Content = t;
        }
    }
}
