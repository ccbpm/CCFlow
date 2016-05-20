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
    public partial class FrmEle : ChildWindow
    {
        public int NodeID = 0;
        public Boolean IsNew = false;
        public BPEle HisEle
        {
            get
            {
                ComboBoxItem item = (ComboBoxItem)this.DDL_EleType.SelectedItem ;
                string eleType = item.Tag.ToString();

                BPEle ele = new BPEle();
                ele.Name = Glo.FK_MapData + "_" + eleType + "_" + this.TB_EleID.Text;
                ele.EleID = this.TB_EleID.Text;
                ele.EleType = eleType;
                ele.EleName = this.TB_EleName.Text;

                return ele;
            }
            set
            {
                return;
            }
        }
        public FrmEle()
        {
            InitializeComponent();
        }
        private DataTable _dtConfig=null;
        public DataTable dtConfig
        {
            get
            {
                if (_dtConfig == null)
                {
                    _dtConfig = new DataTable();
                    _dtConfig.Columns.Add(new DataColumn("DFor", typeof(string)));
                    _dtConfig.Columns.Add(new DataColumn("Tag1", typeof(string)));
                    _dtConfig.Columns.Add(new DataColumn("Tag2", typeof(string)));
                    _dtConfig.Columns.Add(new DataColumn("Tag3", typeof(string)));
                    _dtConfig.Columns.Add(new DataColumn("Tag4", typeof(string)));

                    DataRow dr = _dtConfig.NewRow();
                    dr["DFor"] = "HandSiganture";
                    dr["Tag1"] = "@Label=存储路径@FType=String";
                    dr["Tag2"] = "@Label=窗口打开宽度@DefVal=300@FType=Int";
                    dr["Tag3"] = "@Label=窗口打开高度@DefVal=200@FType=Int";
                    dr["Tag4"] = "不需填写";
                    _dtConfig.Rows.Add(dr);

                    dr = _dtConfig.NewRow();
                    dr["DFor"] = "iFrame";
                    dr["Tag1"] = "@Label=Url@DefVal=http://ccflow.org@FType=String";
                    dr["Tag2"] = "不需填写";
                    dr["Tag3"] = "不需填写";
                    dr["Tag4"] = "不需填写";
                    _dtConfig.Rows.Add(dr);

                    dr = _dtConfig.NewRow();
                    dr["DFor"] = "Fieldset";
                    dr["Tag1"] = "@Label=标题@FType=String";
                    dr["Tag2"] = "不需填写";
                    dr["Tag3"] = "不需填写";
                    dr["Tag4"] = "不需填写";
                    _dtConfig.Rows.Add(dr);
                }
                return _dtConfig;
            }
        }
        public void SetBlank()
        {
            this.TB_EleID.Text = "";
            this.TB_EleName.Text = "";
            this.DDL_EleType.SelectedIndex = 0;
            this.CB_IsReadonly.IsChecked = true;

            this.TB_Tag1.Text = "";
            this.TB_Tag2.Text = "";
            this.TB_Tag3.Text = "";
            this.TB_Tag4.Text = "";

            this.IsNew = true;
            this.TB_EleID.IsEnabled = true;
            this.DDL_EleType.IsEnabled = true;

            this.ReSetTagDesc();
        }
        protected override void OnOpened()
        {
            base.OnOpened();
            this.ReSetTagDesc();
        }
        /// <summary>
        /// 绑定这个控件.
        /// </summary>
        /// <param name="mypk"></param>
        public void BindData(string mypk)
        {
            this.Name = mypk;
            string sql = "SELECT * FROM Sys_FrmEle WHERE MyPK='" + mypk + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }
        /// <summary>
        /// 绑定这个控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {

            DataSet ds = new DataSet();
            ds.FromXml(e.Result);

            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                this.SetBlank();
            }
            else
            {
                this.TB_EleID.IsEnabled = false;
                this.DDL_EleType.IsEnabled = true;
                this.IsNew = false;

                this.TB_EleID.Text = dt.Rows[0]["EleID"].ToString();
                this.TB_EleName.Text = dt.Rows[0]["EleName"].ToString();

                foreach (var item in this.DDL_EleType.Items)
                {
                    ComboBoxItem myite = item as ComboBoxItem;
                    if (myite.Tag.ToString() == dt.Rows[0]["EleType"].ToString())
                    {
                        this.DDL_EleType.SelectedItem = myite;
                        myite.IsSelected = true;
                        break;
                    }
                }

                this.DDL_EleType.IsEnabled = false;


                //为每个tag 赋值。
                this.TB_Tag1.Text = dt.Rows[0]["Tag1"].ToString();
                this.TB_Tag2.Text = dt.Rows[0]["Tag2"].ToString();
                this.TB_Tag3.Text = dt.Rows[0]["Tag3"].ToString();
                this.TB_Tag4.Text = dt.Rows[0]["Tag4"].ToString();
            }

            this.ReSetTagDesc();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)this.DDL_EleType.SelectedItem;
            string eleType = item.Tag.ToString();

            string mypk = Glo.FK_MapData + "_" + eleType + "_" + this.TB_EleID.Text.Trim();
            string strs = "@EnName=BP.Sys.FrmEle@PKVal=" + mypk;
            strs += "@EleType=" + eleType;
            strs += "@EleName=" + this.TB_EleName.Text.Trim();
            strs += "@EleID=" + this.TB_EleID.Text.Trim();
            strs += "@FK_MapData=" + Glo.FK_MapData;

            if (this.CB_IsReadonly.IsChecked == true)
                strs += "@IsEnable=1";
            else
                strs += "@IsEnable=0";

            strs += "@Tag1=" + this.TB_Tag1.Text.Trim();
            if (this.Lab_Tag2.Content.ToString() != "不需填写")
                strs += "@Tag2=" + this.TB_Tag2.Text.Trim();

            if (this.Lab_Tag3.Content.ToString() != "不需填写")
                strs += "@Tag3=" + this.TB_Tag3.Text.Trim();

            if (this.Lab_Tag4.Content.ToString() != "不需填写")
                strs += "@Tag4=" + this.TB_Tag4.Text.Trim();

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveEnAsync(strs);
            da.SaveEnCompleted += new EventHandler<FF.SaveEnCompletedEventArgs>(da_SaveEnCompleted);
        }
        void da_SaveEnCompleted(object sender, FF.SaveEnCompletedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void DDL_EleType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // MessageBox.Show("初始化注释。");
            this.ReSetTagDesc();
           // MessageBox.Show("初始化注释。 over");
        }
        /// <summary>
        /// 重设置描述
        /// </summary>
        public void ReSetTagDesc()
        {
            if (this.DDL_EleType == null)
                return;

            if (this.DDL_EleType.SelectedItem == null)
                return;

            ComboBoxItem  item = this.DDL_EleType.SelectedItem as ComboBoxItem;
            string eleType = item.Tag.ToString();

            foreach (DataRow dr in dtConfig.Rows)
            {
                if (dr["DFor"].ToString() != eleType)
                    continue;

                for (int i = 1; i < 5; i++)
                {
                    string str = dr["Tag" + i].ToString();
                    if (string.IsNullOrEmpty(str))
                        continue;

                    string[] kvs = str.Split('@');
                    foreach (string k in kvs)
                    {
                        string[] kk = k.Split('=');
                        switch (kk[0])
                        {
                            case "Label":
                                Label lab = this.FindName("Lab_Tag" + i) as Label;
                                lab.Content = kk[1];
                                break;
                            case "DefVal":
                                TextBox tb = this.FindName("TB_Tag" + i) as TextBox;
                                if (tb.Text.Trim() == "")
                                    tb.Text = kk[1];
                                break;
                            case "FType":
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        private void TB_EleName_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        private void TB_EleName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.TB_EleID.IsEnabled == false)
                return;
            Glo.GetKeyOfEn(this.TB_EleName.Text, true, this.TB_EleID);
        }

     
    }
}

