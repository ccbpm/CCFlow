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
using Silverlight;
namespace CCForm
{
    public partial class FrmShare : ChildWindow
    {
        public FrmShare()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string sql = "UPDATE Sys_MapData SET Name='" + this.TB_FrmName.Text + "', Designer='" + this.TB_Designer.Text + "',DesignerUnit='" + this.TB_DesignerUnit.Text + "',DesignerContext='" + this.TB_DesignerContext.Text + "' WHERE No='" + Glo.FK_MapData + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(da_RunSQLsCompleted);
        }
        void da_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            if (this.DDL_Sort.SelectedIndex < 0)
            {
                MessageBox.Show("请选择表单类型.");
                return;
            }

            FF.CCFormSoapClient daFtp = Glo.GetCCFormSoapClientServiceInstance();
            string isSec = "0";
            if (this.checkBox1.IsChecked == true)
                isSec = "1";

            ListBoxItem lbi = (ListBoxItem)this.DDL_Sort.SelectedItem;
            daFtp.FtpMethodAsync("ShareFrm", Glo.FK_MapData, isSec, lbi.Tag.ToString());
            daFtp.FtpMethodCompleted += new EventHandler<FF.FtpMethodCompletedEventArgs>(daFtp_FtpMethodCompleted);
        }
        void daFtp_FtpMethodCompleted(object sender, FF.FtpMethodCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "共享错误", MessageBoxButton.OK);
                return;
            }
            MessageBox.Show("感谢您为中国开源软件做出一份贡献.", "成功共享", MessageBoxButton.OK);
            this.DialogResult = true;
        }
        protected override void OnOpened()
        {
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            string sql = "SELECT Name,Designer,DesignerUnit,DesignerContext FROM Sys_MapData WHERE No='" + Glo.FK_MapData + "'";
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);

            da.FtpMethodAsync("GetDirs", "/Form.表单模版/", null, null);
            da.FtpMethodCompleted += new EventHandler<FF.FtpMethodCompletedEventArgs>(da_FtpMethodCompleted);
            base.OnOpened();
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("系统错误没有找到 fk_mapdata=" + Glo.FK_MapData + "的数据。", "系统错误", MessageBoxButton.OK);
                return;
            }

            this.TB_FrmName.Text = dt.Rows[0]["Name"];

            if (dt.Rows[0]["Designer"] != null)
                this.TB_Designer.Text = dt.Rows[0]["Designer"];

            if (dt.Rows[0]["DesignerUnit"] != null)
                this.TB_DesignerUnit.Text = dt.Rows[0]["DesignerUnit"];

            if (dt.Rows[0]["DesignerContext"] != null)
                this.TB_DesignerContext.Text = dt.Rows[0]["DesignerContext"];
        }
        void da_FtpMethodCompleted(object sender, FF.FtpMethodCompletedEventArgs e)
        {
            try
            {
                if (e.Result == null || e.Result.Contains("Err") == true)
                {
                    MessageBox.Show(e.Result + "\r 连接到网络错误", "连接到网络错误", MessageBoxButton.OK);
                    this.DialogResult = false;
                    return;
                }

                string[] strs = e.Result.Split('@');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    ListBoxItem lb = new ListBoxItem();
                    lb.Content = s;
                    lb.Tag = s;
                    this.DDL_Sort.Items.Add(lb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "连接到网络错误", MessageBoxButton.OK);
                this.DialogResult = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void textBox4_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
        }

        private void TB_Designer_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void TB_User_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}

