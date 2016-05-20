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
    public partial class SelectM2M : ChildWindow
    {
        public double X = 0;
        public double Y = 0;

        public int IsM2M = 0;
        public SelectM2M()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

            #region 数据检查。
            if (string.IsNullOrEmpty(this.TB_Name.Text)
               || string.IsNullOrEmpty(this.TB_No.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "提示", MessageBoxButton.OK);
                return;
            }
            if (this.TB_No.Text.Length >= 50)
            {
                MessageBox.Show("英文名称太长,不能多于50个字符，并且必须是下划线或者英文字母。", "Note",
                    MessageBoxButton.OK);
                return;
            }
            #endregion 数据检查。
             

            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.DoTypeAsync("NewM2M", Glo.FK_MapData, this.TB_No.Text, this.IsM2M.ToString(), this.IsM2M.ToString(), this.X.ToString(), this.Y.ToString());
            ff.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(ff_DoTypeCompleted);
        }

        void ff_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result,"提示", MessageBoxButton.OK);
                return;
            }

            Glo.TempVal = this.TB_No.Text;
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            Glo.GetKeyOfEn(this.TB_Name.Text, true, this.TB_No);
        }
      
    }
}

