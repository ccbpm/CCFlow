using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Silverlight;
using BP.En;

namespace CCForm
{
    public partial class FrmEleMicHot : ChildWindow
    {
        public bool IsNewElement = false;
        public BPMicrophonehot HisEle = null;

        public FrmEleMicHot()
        {
            InitializeComponent();
        }
        //新增
        public void InitForm()
        {
            this.IsNewElement = true;
            this.RB_0.IsChecked = true;
            this.TB_MicHot_EnName.IsEnabled = true;
            this.TB_MicHot_EnName.Text = "";
            this.TB_MicHot_Path.Text = "/DataUser/";
            this.TB_MicHot_CName.Text = "";
        }
        //修改
        public void BindIt(BPMicrophonehot micHot)
        {
            this.HisEle = micHot;
            this.IsNewElement = false;
            this.RB_0.IsChecked = true;
            this.TB_MicHot_EnName.IsEnabled = false;
            this.TB_MicHot_CName.Text = this.HisEle.KeyName;
            this.TB_MicHot_EnName.Text = this.HisEle.Name;

            string sql = "SELECT * FROM Sys_MapAttr WHERE MyPK='" + this.HisEle.MyPK + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }

        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(dt.Rows[0]["UIIsEnable"]) && dt.Rows[0]["UIIsEnable"].ToString() == "0")
                {
                    this.RB_1.IsChecked = true;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string cnName = this.TB_MicHot_CName.Text;
            if (string.IsNullOrEmpty(cnName))
            {
                MessageBox.Show("中文名不能为空。");
                return;
            }
            string enName = this.TB_MicHot_EnName.Text;
            if (string.IsNullOrEmpty(enName))
            {
                MessageBox.Show("英文名不能为空。");
                return;
            }
            //判断英文名称不能包含汉字
            if (BP.SL.Glo.ContainsChinese(this.TB_MicHot_EnName.Text))
            {
                MessageBox.Show("英文名称 不能使用汉字！");
                return;
            }
            if (string.IsNullOrEmpty(this.TB_MicHot_Path.Text.Trim()))
            {
                this.TB_MicHot_Path.Text = "/DataUser/";
            }

            this.HisEle.Name = this.TB_MicHot_EnName.Text;
            this.HisEle.KeyName = TB_MicHot_CName.Text;
            this.HisEle.MyPK = Glo.FK_MapData + "_" + this.TB_MicHot_EnName.Text;

            if (IsNewElement == true)
            {
                this.DialogResult = true;
            }
            else
            {
                string UIIsEnable = "1";
                if (this.RB_1.IsChecked == true)
                {
                    UIIsEnable = "0";
                }
                string sql = "UPDATE Sys_MapAttr SET UIIsEnable=" + UIIsEnable + " WHERE MyPK='" + this.HisEle.MyPK + "'";
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
                da.RunSQLCompleted += new EventHandler<FF.RunSQLCompletedEventArgs>(da_RunSQLCompleted);
            }
        }

        void da_RunSQLCompleted(object sender, FF.RunSQLCompletedEventArgs e)
        {
            this.DialogResult = true;   
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void TB_MicHot_CName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.TB_MicHot_EnName.IsEnabled == false)
                return;
            Glo.GetKeyOfEn(this.TB_MicHot_CName.Text, true, this.TB_MicHot_EnName);
        }
    }
}

