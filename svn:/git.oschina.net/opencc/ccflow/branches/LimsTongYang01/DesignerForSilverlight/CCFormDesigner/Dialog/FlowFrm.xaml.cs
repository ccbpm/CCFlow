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
    public partial class FlowFrm : ChildWindow
    {
        public int NodeID = 0;
        public Boolean IsNew = false;
        public FlowFrm()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string pk = this.TB_No.Text;
            if (pk == "系统自动编号...")
                pk = "";
            int frmType = this.DDL_FrmType.SelectedIndex;
            if (frmType == 0 || frmType == 1)
            {
                if (this.TB_PTable.Text.Trim().Length == 0)
                {
                    MessageBox.Show("您需要输入物理表名");
                    return;
                }
            }
            else
            {
                if (this.TB_URL.Text.Trim().Length == 0)
                {
                    MessageBox.Show("您需要输入 URL ");
                    return;
                }
            }

            string strs = "@EnName=BP.WF.Frm@PKVal=" + pk + "@Name=" + this.TB_Name.Text;
            strs += "@PTable=" + this.TB_PTable.Text + "@FrmType=" + frmType;
            strs += "@FK_Flow=" + Glo.FK_Flow;
            strs += "@URL=" + this.TB_URL.Text;
            strs += "@No=" + this.TB_No.Text;
            strs += "@DBURL=" + this.DDL_DBUrl.SelectedIndex;

            string isReadonly = "0";
            if (this.CB_IsReadonly.IsChecked == true)
                isReadonly = "1";

            string isPrint = "0";
            if (this.CB_IsPrint.IsChecked == true)
                isPrint = "1";

            //FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            //da.SaveEnAsync(strs);
            //da.SaveEnCompleted += new EventHandler<FF.SaveEnCompletedEventArgs>(da_SaveEnCompleted);

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.DoTypeAsync("SaveFlowFrm", strs, this.NodeID.ToString(), Glo.FK_Flow,
             isReadonly, isPrint);
            da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(da_DoTypeCompleted);
        }
        //void da_SaveEnCompleted(object sender, FF.SaveEnCompletedEventArgs e)
        //{
        //    if (e.Result == null)
        //    {
        //        this.TB_No.Text = e.Result;
        //        this.DialogResult = true;
        //        return;
        //    }
        //    MessageBox.Show(e.Result);
        //}
        void da_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result.Contains("Error:"))
            {
                MessageBox.Show(e.Result, "Save Error", MessageBoxButton.OK);
            }
            else
            {
                this.TB_No.Text = e.Result;
                this.DialogResult = true;
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void DDL_FrmType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.SetState();
            }
            catch
            {
            }
        }
        public void SetState()
        {
            this.TB_PTable.IsEnabled = true;
            this.DDL_DBUrl.IsEnabled = true;
            this.TB_URL.IsEnabled = true;
            this.CB_IsPrint.IsEnabled = true;
            this.CB_IsReadonly.IsEnabled = true;

            if (this.DDL_FrmType.SelectedIndex == 2)
            {
                /*自定义表单*/
                this.TB_PTable.IsEnabled = false;
                this.DDL_DBUrl.IsEnabled = false;
                this.TB_URL.IsEnabled = true;
                this.CB_IsPrint.IsEnabled = false;
                this.CB_IsReadonly.IsEnabled = false;
            }
            else
            {
                this.TB_PTable.IsEnabled = true;
                this.DDL_DBUrl.IsEnabled = true;
                this.TB_URL.IsEnabled = false;
                this.CB_IsPrint.IsEnabled = true;
                this.CB_IsReadonly.IsEnabled = true;
            }

        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

