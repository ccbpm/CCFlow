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
using Silverlight;
using BP.DA;
using BP;

namespace BP.Controls
{
    public partial class FlowFrm : ChildWindow
    {
        public FlowFrm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 独立表单ID
        /// </summary>
        public string FlowFormId { get; set; }

        /// <summary>
        /// 子窗体被关闭时执行的事件　
        /// </summary>
        public event EventHandler ClosedHanlder;

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

            string strs = "@EnName=BP.BP.Frm@PKVal=" + pk + "@Name=" + this.TB_Name.Text;
            strs += "@PTable=" + this.TB_PTable.Text + "@FrmType=" + frmType;
            strs += "@FK_Flow=" + string.Empty;
            strs += "@URL=" + this.TB_URL.Text;
            strs += "@DBURL=" + this.DDL_DBUrl.SelectedIndex;
            var client =   Glo.GetDesignerServiceInstance();
            client.DoAsync("SaveFlowFrm", strs, true);
            client.DoCompleted += new EventHandler<global::WF.WS.DoCompletedEventArgs>(client_DoCompleted);
        }

        void client_DoCompleted(object sender, global::WF.WS.DoCompletedEventArgs e)
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
            this.TB_URL.IsEnabled = true;

            if (this.DDL_FrmType.SelectedIndex == 2)
            {
                /*自定义表单*/
                this.TB_PTable.IsEnabled = false;
                this.TB_URL.IsEnabled = true;
            }
            else
            {
                this.TB_PTable.IsEnabled = true;
                this.TB_URL.IsEnabled = false;
            }

        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            if(ClosedHanlder != null)
            {
                ClosedHanlder(sender, e);
            }
        }

        /// <summary>
        /// 如果FlowFormId不为空，说明是编辑页面，需要初始化控件的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if(!string.IsNullOrEmpty(FlowFormId))
            {
                var sql = "select * from Sys_MapData";
                var client = Glo.GetDesignerServiceInstance();
                client.RunSQLReturnTableCompleted += new EventHandler<global::WF.WS.RunSQLReturnTableCompletedEventArgs>(client_RunSQLReturnTableCompleted);
                client.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID, true);
            }
        }
        /// <summary>
        /// 查询结束后，初始化页面控件的值　
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_RunSQLReturnTableCompleted(object sender, global::WF.WS.RunSQLReturnTableCompletedEventArgs e)
        {
            try
            {
                var ds = new DataSet();
                ds.FromXml(e.Result);

                if(ds.Tables.Count >0 && ds.Tables[0].Rows.Count >0)
                {
                    var row = ds.Tables[0].Rows[0];
                    TB_Name.Text = row["Name"].ToString();
                    TB_URL.Text = row["URL"].ToString();
                    TB_PTable.Text = row["PTable"].ToString();
                    if (!string.IsNullOrEmpty(row["FrmType"].ToString()))
                    {
                        DDL_FrmType.SelectedValue = row["FrmType"].ToString();
                    }

                    if(!string.IsNullOrEmpty(row["DBURL"].ToString()))
                    {
                        DDL_DBUrl.SelectedValue = row["DBURL"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("取得表单默认值时遇到了错误，错误信息为：\n" + ex.Message);
            }
        }

        
    }
}

