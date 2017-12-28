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
    public partial class FrmLab : ChildWindow
    {
        public FrmLab()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public void BindIt(BPLabel link)
        {
            this.TB_Text.Text = link.Content.ToString().Replace("\n", "@");

            Glo.BindComboBoxFontSize(this.DDL_FrontSize, link.FontSize);
            this.TB_Text.TextWrapping = TextWrapping.Wrap;
            this.Show();
        }
    }
}

