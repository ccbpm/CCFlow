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
using System.Windows.Browser;
using Silverlight;
using BP.En;
using BP.Sys;

namespace CCForm
{
    public partial class FrmHiddenField : ChildWindow
    {
        public FrmHiddenField()
        {
            InitializeComponent();
        }

        protected override void OnOpened()
        {
            this.BindIt();
            base.OnOpened();
        }
        public void BindIt()
        {
           // string sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND UIVISIBLE=0 AND EditType=0";
            string sql = "SELECT * FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND UIVISIBLE=0 AND EditType=0";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
        }

        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet("s");
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];
            this.listBox1.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ListBoxItem li = new ListBoxItem();
                li.Content = dr["KeyOfEn"] + " - " + dr["Name"];
                li.Tag = dr["KeyOfEn"] + "#" + dr["MyDataType"] + "#" + dr["LGType"];
                this.listBox1.Items.Add(li);
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            string tKey = "&K=" + DateTime.Now.ToString("yyyyMMddHHmmss");

            string url = "";
            Button btn = sender as Button;
            if (btn.Name == "Btn_New")
            {
                MessageBox.Show("在您选择字段类型后，注意设置字段隐藏属性。",
                    "提示", MessageBoxButton.OK);

                url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Do.htm?DoType=AddF&MyPK=" + Glo.FK_MapData + tKey;
                if (Glo.Platform == Platform.JFlow)
                    url = url.Replace(".aspx", ".jsp");

                HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:600px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                this.DialogResult = false;
                return;
            }

            ListBoxItem li = this.listBox1.SelectedItem as ListBoxItem;
            if (li == null)
                return;

            string[] strs = li.Tag.ToString().Split('#');
            string key = strs[0];
            string fType = strs[1];
            int lgType = int.Parse(strs[2]);
          //  BP.DA.LGType myLGType = (LGType)lgType;
            switch (btn.Name)
            {
                case "Btn_Edit":
                    switch (lgType)
                    {
                        case 0:
                            url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/EditF.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + key + "&FType=" + fType + tKey;
                            break;
                        case 1:
                            url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/EditEnum.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + key + "&FType=" + fType + tKey;
                            break;
                        case 2:
                            url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/EditTable.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + key + "&FType=" + fType + tKey;
                            break;
                        default:
                            break;
                    }
                    break;
                case "Btn_Del":
                    if (MessageBox.Show("您确定要执行删除吗？", "删除提示", MessageBoxButton.OK) == MessageBoxResult.OK)
                    {
                        string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + Glo.FK_MapData + "' AND KeyOfEn='" + key + "'";
                        FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                        da.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
                        da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(da_RunSQLsCompleted);
                        return;
                    }
                    break;
                default:
                    break;
            }

            if (Glo.Platform == Platform.JFlow)
                url = url.Replace(".aspx", ".jsp");


            HtmlPage.Window.Eval("window.showModalDialog('" + url + "',window,'dialogHeight:500px;dialogWidth:500px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
            this.DialogResult = true;
        }
        void da_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.BindIt();
        }

        private void Btn_Visible_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("未完成.");
        }
    }
}

