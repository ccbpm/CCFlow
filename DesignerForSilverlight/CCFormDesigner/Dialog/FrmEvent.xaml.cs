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

namespace CCForm
{
    public partial class FrmEvent : ChildWindow
    {
        public FrmEvent()
        {
            InitializeComponent();
            bindIt();
        }

        protected override void OnOpened()
        {
           
            base.OnOpened();
        }
        public void bindIt()
        {
            this.RB_FrmLoadBefore.IsChecked = true;
            this.RB_Checked(this.RB_FrmLoadBefore, null);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string id = "";
            if ((bool)this.RB_FrmLoadAfter.IsChecked)
                id = this.RB_FrmLoadAfter.Name;

            if ((bool)this.RB_FrmLoadBefore.IsChecked)
                id = this.RB_FrmLoadBefore.Name;

            if ((bool)this.RB_SaveBefore.IsChecked)
                id = this.RB_SaveBefore.Name;

            if ((bool)this.RB_SaveAfter.IsChecked)
                id = this.RB_SaveAfter.Name;

            if (id == "")
            {
                MessageBox.Show("请选择事件类型。", "提示", MessageBoxButton.OK);
                return;
            }
            id = id.Replace("RB_", "");

            string info = "@EnName=BP.Sys.FrmEvent@MyPK=" + Glo.FK_MapData + "_" + id + "@FK_Event=" + id + "@FK_MapData=" + Glo.FK_MapData + "@DoType=" + this.DDL_EventType.SelectedIndex + "@DoDoc=" + this.TB_DoDoc.Text.Replace('@', '^') + "@MsgOK=" + this.TB_MsgOK.Text.Replace('@', '^') + "@MsgError=" + this.TB_MsgErr.Text.Replace('@', '^');
            FF.CCFormSoapClient daInfoSave = Glo.GetCCFormSoapClientServiceInstance();
            daInfoSave.SaveEnAsync(info);
            daInfoSave.SaveEnCompleted += new EventHandler<FF.SaveEnCompletedEventArgs>(daInfoSave_SaveEnCompleted);
        }
        void daInfoSave_SaveEnCompleted(object sender, FF.SaveEnCompletedEventArgs e)
        {
            if (e.Result.Contains("Err"))
            {
                MessageBox.Show(e.Result, "保存错误", MessageBoxButton.OK);
                return;
            }
            MessageBox.Show("单条记录保存成功", "保存提示", MessageBoxButton.OK);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void RB_Checked(object sender, RoutedEventArgs e)
        {
            // 获取id.
            RadioButton rb = sender as RadioButton;
            string id = rb.Name.Replace("RB_", "");

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            string sql = "SELECT * FROM Sys_FrmEvent WHERE FK_MapData='" + Glo.FK_MapData + "' AND FK_Event='" + id + "'";
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                /* 没有数据 */
                this.TB_DoDoc.Text = "";
                this.TB_MsgErr.Text = "";
                this.TB_MsgOK.Text = "";
                this.DDL_EventType.SelectedIndex = 0;
            }
            else
            {
                this.TB_DoDoc.Text = dt.Rows[0]["DoDoc"].Replace("~", "'");
                try
                {
                    this.TB_MsgErr.Text = dt.Rows[0]["MsgError"].Replace("~", "'");
                }
                catch
                {
                }

                //try
                //{
                //    this.TB_MsgErr.Text = dt.Rows[0]["MsgErr"].Replace("~", "'");
                //}
                //catch
                //{
                //}
                this.TB_MsgOK.Text = dt.Rows[0]["MsgOK"].Replace("~", "'");
                this.DDL_EventType.SelectedIndex = int.Parse(dt.Rows[0]["DoType"]);
            }
        }
        private void OKBtnSaveAndClose_Click(object sender, RoutedEventArgs e)
        {
        }
        private void Btn_Help_Click(object sender, RoutedEventArgs e)
        {
            string msg = "帮助";
            msg += "\r\n1, 如果执行多个 sql 可以用 ; 分开。";
            msg += "\r\n2, 内容支持变量约定格式比如:@WebUser.No 当前操作员编号....";
            msg += "\r\n3, 更多详细的使用方法请参考 ccflow表单设计器操作说明书。";
            MessageBox.Show(msg);
        }
    }
}

