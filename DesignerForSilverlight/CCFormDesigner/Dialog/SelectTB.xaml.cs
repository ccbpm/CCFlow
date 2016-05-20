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

namespace CCForm
{
    public partial class SelectTB : ChildWindow
    {
        public bool IsCheckGroup = false;
        public UIElementCollection eles = null;
        public SelectTB()
        {
            InitializeComponent();
            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
            this.RB_jp.Checked += new RoutedEventHandler(rdbEnName_Checked);
            this.RB_qp.Checked += new RoutedEventHandler(rdbEnName_Checked);
        }

        protected override void OnOpened()
        {
            this.IsCheckGroup = false;
            this.TB_Name.Text = "";
            this.TB_KeyOfEn.Text = string.Empty;
            this.CB_IsHid.IsChecked = false;
            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
            base.OnOpened();
        }

        void HisFrmSelectCheckGroup_Closed(object sender, EventArgs e)
        {
            if (this.HisFrmSelectCheckGroup.DialogResult == false)
                return;

            this.TB_Name.Text = this.HisFrmSelectCheckGroup.TB_GroupName.Text;
            this.TB_KeyOfEn.Text = this.HisFrmSelectCheckGroup.TB_GroupKey.Text;
            this.IsCheckGroup = true;
            this.DialogResult = true;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            #region 数据检查。
            if (string.IsNullOrEmpty(this.TB_Name.Text)
               || string.IsNullOrEmpty(this.TB_KeyOfEn.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "Note", MessageBoxButton.OK);
                return;
            }
            
            if (this.TB_KeyOfEn.Text.Length >= 50)
            {
                MessageBox.Show("英文名称太长,不能多于50个字符，并且必须是下划线或者英文字母。", "Note",
                    MessageBoxButton.OK);
                return;
            }
            #endregion 数据检查。

            if (this.CB_IsHid.IsChecked == true)
            {
                if (MessageBox.Show("隐藏字段只能在工具箱的隐藏区域才能找到。",
                    "您确定吗?", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

                string dtype = "0";
                if (this.RB_Boolen.IsChecked == true)
                    dtype = BP.En.DataType.AppBoolean;

                if (this.RB_Data.IsChecked == true)
                    dtype = BP.En.DataType.AppDate;

                if (this.RB_DataTime.IsChecked == true)
                    dtype = BP.En.DataType.AppDateTime;

                if (this.RB_Float.IsChecked == true)
                    dtype = BP.En.DataType.AppFloat;

                if (this.RB_Int.IsChecked == true)
                    dtype = BP.En.DataType.AppInt;

                if (this.RB_Money.IsChecked == true)
                    dtype = BP.En.DataType.AppMoney;

                if (this.RB_String.IsChecked == true)
                    dtype = BP.En.DataType.AppString;

                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.DoTypeAsync("NewHidF", Glo.FK_MapData, this.TB_KeyOfEn.Text, this.TB_Name.Text, dtype, null);
                da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
            }
            else
            {
                this.DialogResult = true;
            }
        }
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                MessageBox.Show("执行成功.", "提示", MessageBoxButton.OK);
                this.DialogResult = false;
            }
            else
            {
                MessageBox.Show( e.Result, "执行错误", MessageBoxButton.OK);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TB_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            getKeyOfEn();
        }

        private void rdbEnName_Checked(object sender, RoutedEventArgs e)
        {
            getKeyOfEn();
        }
        void getKeyOfEn()
        {
            if (!string.IsNullOrEmpty(this.TB_Name.Text) && this.TB_Name.Text.Length > 0)
            {
                bool flag = (this.RB_qp.IsChecked == true) ? true : false;

                if (this.TB_Name.Text.Length >= 50)
                    flag = false;

                Glo.GetKeyOfEn(this.TB_Name.Text, flag, this.TB_KeyOfEn);
            }
        }


        private void Btn_CreateCheckGroup_Click(object sender, RoutedEventArgs e)
        {
            HisFrmSelectCheckGroup = new SelectCheckGroup();
            HisFrmSelectCheckGroup.TB_GroupKey.Text = "";
            HisFrmSelectCheckGroup.TB_GroupName.Text = "";
            HisFrmSelectCheckGroup.Show();

            this.HisFrmSelectCheckGroup.Closed += new EventHandler(HisFrmSelectCheckGroup_Closed);
        }
        public SelectCheckGroup HisFrmSelectCheckGroup = new SelectCheckGroup();
      

        /// <summary>
        /// 系统内置字段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_InitField_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("施工中....");
            CCForm.Dialog.SelectInitField SF = new Dialog.SelectInitField();
            SF.Show();
        }

       

    }
}

