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
using BP.Controls;
using BP;
using WF;
using WF.WS;
namespace BP.Frm
{
    public class FlowForm
    {
        public string No { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string PTable { get; set; }
        public string DataBaseName { get; set; }
        public string Type { get; set; }
        public string IsReadOnly { get; set; }
        public string IsPrintable { get; set; }
    }
    public partial class FrmLib : ChildWindow
    {
        List<FlowForm> list = new List<FlowForm>();
        public FrmLib()
        {
            this.MouseRightButtonDown += (sender, e) =>
                {
                    e.Handled = true;
                };

            InitializeComponent();
            var client = Glo.GetDesignerServiceInstance();
            var sql = "SELECT * FROM Sys_MapData WHERE AppType=0";
            client.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(client_RunSQLReturnTableCompleted);
            client.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID, true);

            this.Grid1.LoadingRow += Grid_LoadingRow;
            this.Grid1.UnloadingRow += Grid_UnloadingRow;

            this.RB_0.Checked += new RoutedEventHandler(RB_Checked);
            this.RB_1.Checked += new RoutedEventHandler(RB_Checked);
        }

        void RB_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            string id = rb.Name.Replace("RB_", "");
            var client = Glo.GetDesignerServiceInstance();
            var sql = "SELECT * FROM Sys_MapData WHERE AppType=" + id;
            client.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(client_RunSQLReturnTableCompleted);
            client.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID, true);
        }

        private void Grid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp += Row_MouseLeftButtonUp;
        }
        private void Grid_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseLeftButtonUp -= Row_MouseLeftButtonUp;
        }
        private void Row_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //TimeSpan t = DateTime.Now.TimeOfDay;
            //DataGridRow dgr = sender as DataGridRow;
            //if (dgr.Tag != null)
            //{
            //    TimeSpan oldT = (TimeSpan)dgr.Tag;
            //    if ((t - oldT) < TimeSpan.FromMilliseconds(300))
            //    {
            //        MessageBox.Show("xxx");
            //    }
            //}
            //dgr.Tag = t;
        }
        private void Row_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TimeSpan t = DateTime.Now.TimeOfDay;
            DataGridRow dgr = sender as DataGridRow;
            if (dgr.Tag != null)
            {
                TimeSpan oldT = (TimeSpan)dgr.Tag;
                if ((t - oldT) < TimeSpan.FromMilliseconds(300))
                {
                    FlowForm ff= this.Grid1.SelectedItem as FlowForm;
                    if (ff == null)
                        return;

                    Frm frm = new Frm();
                    //frm.BindFrm(ff.No);
                    frm.Show();
                }
            }
            dgr.Tag = t;
        }
        void client_RunSQLReturnTableCompleted(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            var ds = new DataSet();
            ds.FromXml(e.Result);
            foreach (DataRow dataRow in ds.Tables[0].Rows)
            {
                var flowForm = new FlowForm
                {
                    DataBaseName = string.Empty,
                    No = dataRow["No"].ToString(),
                    Name = dataRow["Name"].ToString(),
                    PTable = dataRow["PTable"].ToString(),
                    Type = formatFormType(dataRow["FrmType"]),
                    URL = dataRow["URL"]
                };
                list.Add(flowForm);
            }

            this.Grid1.ItemsSource = null;
            this.Grid1.ItemsSource = list;
            this.Grid1.SelectedIndex = 0;

            //由于数据源变动,必须首先调用UpdateLayout
            this.Grid1.UpdateLayout();
            //  this.Grid1.ScrollIntoView(this.Grid1.SelectedItems[0], null);
            //  this.Grid1.ItemsSource;
            //this.Grid1.ItemsSource = list;
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
            Button btn = sender as Button;
            switch (btn.Name)
            {
                case "Btn_New":
                    Frm frm = new Frm();
                    frm.BindNew();
                    break;
                case "Btn_Edit":
                    break;
                case "Btn_Delete":
                    break;
                case "Btn_Fields":
                    break;
                case "Btn_Preview":
                    break;
                default:
                    break;
            }
        }
    }
}

