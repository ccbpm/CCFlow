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

namespace CCForm
{
    public partial class FrmImgAth : ChildWindow
    {
        public BPImgAth HisImgAth = null;
        public FrmImgAth()
        {
            InitializeComponent();
        }

        public void BindIt(BPImgAth imgAth)
        {
            this.HisImgAth = imgAth;
            RBtn_EditDisplay.IsChecked = true;
            RBtn_Edit.IsChecked = this.HisImgAth.IsEdit;            
            this.Show();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.HisImgAth.IsEdit = (bool)RBtn_Edit.IsChecked;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

