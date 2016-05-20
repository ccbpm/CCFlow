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
using System.Windows.Navigation;
using Silverlight;
using WF.Controls;
using BP;
using WF;
namespace BP.Frm
{
    public partial class Frms : Page
    {
        private List<CheckBox> checkboxLists = new List<CheckBox>();
        List<FlowForm> list = new List<FlowForm>();
        public Frms()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var client = Glo.GetDesignerServiceInstance();
            var sql = "select * from Sys_MapData";
            client.RunSQLReturnTableCompleted += client_RunSQLReturnTableCompleted;
            client.RunSQLReturnTableAsync(sql, true);
        }
        void client_RunSQLReturnTableCompleted(object sender, WF.WS.RunSQLReturnTableCompletedEventArgs e)
        {
            var ds = new DataSet();
            ds.FromXml(e.Result);
            foreach (DataRow dataRow in ds.Tables[0].Rows)
            {
                var flowForm = new FlowForm
                                   {
                                       DataBaseName = string.Empty,
                                       Id = dataRow["No"].ToString(),
                                       Name = dataRow["Name"].ToString(),
                                       TableName = dataRow["PTable"].ToString(),
                                       //Type = formatFormType(dataRow["FormType"]),
                                       //URL = dataRow["URL"]
                                   };
                list.Add(flowForm);
            }
            this.Grid1.ItemsSource = list;
            this.Grid1.LoadingRow += new EventHandler<DataGridRowEventArgs>(gridFlowFrom_LoadingRow);
        }
        void gridFlowFrom_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var chk = this.Grid1.Columns[0].GetCellContent(e.Row) as CheckBox;
            chk.IsChecked = false;
            checkboxLists.Add(chk);
        }
        private string formatFormType(string intValue)
        {
            string stringValue = string.Empty;
            switch (intValue)
            {
                case "0":
                    stringValue = "傻瓜表单";
                    break;
                case "1":
                    stringValue = "自由表单";
                    break;
                default:
                    stringValue = "自定义表单";
                    break;
            }
            return stringValue;
        }
        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var flowFormDialog = new FlowFrm();
            flowFormDialog.ClosedHanlder += (s, arg) =>
            {
                var frm = s as FlowFrm;
                if (frm != null && frm.DialogResult == true)
                {
                    Page_Loaded(null, null);
                }
            };
            flowFormDialog.Show();
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            var flowFormDialog = new FlowFrm();
            flowFormDialog.ClosedHanlder += (s, arg) =>
            {
                var frm = s as FlowFrm;
                if (frm != null && frm.DialogResult == true)
                {
                    Page_Loaded(null, null);
                }
            };
            flowFormDialog.Show();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btnShowField_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            if(checkboxLists[2].IsChecked == true)
            {
                MessageBox.Show(list[2].Name);
            }
        }
        private void chkAll_Click(object sender, RoutedEventArgs e)
        {
            var chk = sender as CheckBox;
            bool check = chk.IsChecked.Value;
            foreach (CheckBox chkbox in checkboxLists)
                chkbox.IsChecked = check;
        }
        private void chkID_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = getCheckBoxWithParent(this.Grid1, typeof(CheckBox), "chkAll");
            if (cb != null)
                cb.IsChecked = false;
        }
        private CheckBox getCheckBoxWithParent(UIElement parent, Type targetType, string checkBoxName)
        {
            if (parent.GetType() == targetType && ((CheckBox)parent).Name == checkBoxName)
            {
                return (CheckBox)parent;
            }
            CheckBox result = null;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = (UIElement)VisualTreeHelper.GetChild(parent, i);
                if (getCheckBoxWithParent(child, targetType, checkBoxName) != null)
                {
                    result = getCheckBoxWithParent(child, targetType, checkBoxName);
                    break;
                }
            }
            return result;
        }
        private void GetSelected<T>()
        {
        }
    }
    public class FlowForm
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string TableName { get; set; }
        public string DataBaseName { get; set; }
        public string Type { get; set; }
        public string IsReadOnly { get; set; }
        public string IsPrintable { get; set; }
    }
}
