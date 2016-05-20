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
    public partial class FrmLink : ChildWindow
    {
        public FrmLink()
        {
            InitializeComponent();
        }
        protected override void OnOpened()
        {
            base.OnOpened();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public BPLink HisLink = null;
        /// <summary>
        /// 绑定link.
        /// </summary>
        /// <param name="link"></param>
        public void BindIt(BPLink link)
        {
            HisLink = link;
            this.TB_Text.Text = link.Content.ToString().Replace("\n", "@");
            string url = link.URL as string;
            if (string.IsNullOrEmpty(url))
                url = "http://ccflow.org";

            this.TB_URL.Text = url;
            Glo.BindComboBoxFontSize(this.DDL_FrontSize, link.FontSize);
            Glo.BindComboBoxWinOpenTarget(this.DDL_WinName, link.WinTarget);
            Glo.BindComboBoxFrontName(this.DDL_FrontName, link.FontFamily.Source);
            SolidColorBrush d = (SolidColorBrush)link.Foreground;
            this.Show();
        }
        private void DDL_WinName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem it = (ComboBoxItem)this.DDL_WinName.SelectedItem;
            if (it == null)
                return;

            this.TB_WinName.Text = it.Tag.ToString();
            if (this.TB_WinName.Text=="def" )
                this.TB_WinName.Text="";
        }
    }
}

