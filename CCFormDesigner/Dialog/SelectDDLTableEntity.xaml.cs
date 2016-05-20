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
    public partial class SelectDDLTableEntity : ChildWindow
    {
        public SelectDDLTableEntity()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.DoTypeAsync("SaveSFTable", this.TB_CHName.Text, this.TB_EnName.Text, null, null, null);
            ff.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(ff_DoTypeCompleted);
        }

        void ff_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                this.DialogResult = true;
                return;
            }
            MessageBox.Show(e.Result);
        }
        void ff_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            /*开始执行保存数据.*/
            MessageBox.Show("表或视图数据保存功能未实现，您可以打开数据库直接修改表或视图 " + this.TB_EnName.Text + "。");
            this.DialogResult = true;
        }

        void fc_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result);
                return;
            }

            this.TB_EnName.IsEnabled = false;
            this.OKButton.Content = "保存";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

