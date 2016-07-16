using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.Text;
using Microsoft.Expression.Interactivity;
using Microsoft.Expression.Interactivity.Layout;
using System.Windows.Media.Imaging;
using Silverlight;
using BP.En;
using BP.Sys;
using CCForm.FF;

namespace CCForm
{
    public partial class SelectDDLTable : ChildWindow
    {
        public SelectDDLTable()
        {
            InitializeComponent();
            this.tableEntity.Closed += new EventHandler(tableEntity_Closed);
        }
        void tableEntity_Closed(object sender, EventArgs e)
        {
            if (this.tableEntity.DialogResult == false)
                return;

            this.BindData();

            foreach (ListBoxItem item in this.listBox1.Items)
                item.IsSelected = false;

            foreach (ListBoxItem item in this.listBox1.Items)
            {
                if (item.Content.ToString().Contains(":" + this.TB_KeyOfName.Text))
                {
                    item.IsSelected = true;
                    break;
                }
            }
        }
        protected override void OnOpened()
        {
            this.BindData();
            base.OnOpened();
        }
        public void BindData()
        {
            this.listBox1.Items.Clear();
            string sql = "SELECT No,Name,TableDesc,FK_Val,SrcType FROM Sys_SFTable ORDER BY SrcType, FK_SFDBSrc";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
            //  this.listBox1.SelectionChanged += new SelectionChangedEventHandler(listBox1_SelectionChanged);
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                ListBoxItem item = new ListBoxItem();

                if (string.IsNullOrEmpty(dr["SrcType"]) || dr["SrcType"].ToString() == "")
                    item.Tag = dr["No"].ToString() + ":" + dr["FK_Val"] + ":0";
                else
                    item.Tag = dr["No"].ToString() + ":" + dr["FK_Val"] + ":" + dr["SrcType"].ToString();

                item.Content = dr["No"] + ":" + dr["Name"];
                this.listBox1.Items.Add(item);
            }

            this.listBox1.UpdateLayout();


        }
        public string SelectEnName = null;
        void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
                return;

            ListBoxItem li = e.AddedItems[0] as ListBoxItem;
            string[] itemName = li.Content.ToString().Split(':');
            string[] noFK_Val = li.Tag.ToString().Split(':');

            this.TB_KeyOfEn.Text = noFK_Val[1];
            this.TB_KeyOfName.Text = itemName[1];

            //选择的entit.
            this.SelectEnName = noFK_Val[0];
             
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            #region 数据检查。
            if (string.IsNullOrEmpty(this.TB_KeyOfName.Text)
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


            //直接保存到数据库里.
            CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.SaveFKFieldAsync(Glo.FK_MapData, this.TB_KeyOfEn.Text.Trim(), this.TB_KeyOfName.Text.Trim(),
                this.SelectEnName, Glo.X, Glo.Y);

            da.SaveFKFieldCompleted += new EventHandler<SaveFKFieldCompletedEventArgs>(da_SaveFKFieldCompleted);
        }
        void da_SaveFKFieldCompleted(object sender, SaveFKFieldCompletedEventArgs e)
        {
            if (e.Result != "OK")
            {
                MessageBox.Show(e.Result);
                return;
            }
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_Del_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请选择要删除的项目。");
                return;
            }

            if (MessageBox.Show("您确定要删除吗？", "删除确认", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            ListBoxItem item = this.listBox1.SelectedItem as ListBoxItem;
            string[] kv = item.Content.ToString().Split(':');

            FF.CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.DoTypeAsync("DelSFTable", kv[0], null, null, null, null, null);
            ff.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>(ff_DoTypeCompleted);
        }

        void ff_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result);
                return;
            }
            MessageBox.Show("删除成功");
            this.BindData();
        }
        public SelectDDLTableEntity tableEntity = new SelectDDLTableEntity();
        private void Btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请选择一个字典表,然后点编辑按钮.", "提示", MessageBoxButton.OK);
                return;
            }

            ListBoxItem item = this.listBox1.SelectedItem as ListBoxItem;
            string[] kv = item.Content.ToString().Split(':');


            string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Do.aspx?DoType=EditSFTable&RefNo=" + kv[0];
            if (Glo.Platform == Platform.JFlow)
                url = url.Replace(".aspx", ".jsp");

            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:450px;dialogWidth:780px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
            this.Close();
        }

        private void Btn_DBSrc_Click(object sender, RoutedEventArgs e)
        {
            string url = Glo.BPMHost + "/WF/Comm/Sys/SFDBSrcNewGuide.aspx";
            if (Glo.Platform == Platform.JFlow)
                url = url.Replace(".aspx", ".jsp");

            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:450px;dialogWidth:980px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
        }

        private void Btn_Create_Click(object sender, RoutedEventArgs e)
        {
            // 被zhoupeng 在 2014-10-24注销.
            ////   this.tableEntity.tabItem2.IsEnabled = false;
            //this.tableEntity.TB_EnName.IsEnabled = true;
            //this.tableEntity.OKButton.Content = "确定";
            //this.tableEntity.TB_CHName.Text = "";
            //this.tableEntity.TB_EnName.Text = "";
            //this.tableEntity.Show();

            string url = Glo.BPMHost + "/WF/Comm/Sys/SFGuide.aspx?DoType=New&MyPK=" + Glo.FK_MapData+"&FromApp=SL";

            if (Glo.Platform == Platform.JFlow)
                url = url.Replace(".aspx", ".jsp");

            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:450px;dialogWidth:680px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
            this.Close();
        }
    }
}

