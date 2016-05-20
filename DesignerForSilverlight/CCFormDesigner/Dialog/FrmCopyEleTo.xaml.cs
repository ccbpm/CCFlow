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
    public partial class FrmCopyEleTo : ChildWindow
    {
        public FrmCopyEleTo()
        {
            InitializeComponent();
        }
        protected override void OnOpened()
        {
            string sql = "SELECT NodeID,Name,Step FROM WF_Node WHERE FK_Flow='" + Glo.FK_Flow + "'";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += new EventHandler<FF.RunSQLReturnTableCompletedEventArgs>(da_RunSQLReturnTableCompleted);
            base.OnOpened();
        }
        void da_RunSQLReturnTableCompleted(object sender, FF.RunSQLReturnTableCompletedEventArgs e)
        {
            this.listBox1.Items.Clear();
            this.listBox1.SelectionMode = SelectionMode.Extended;
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["NodeID"].ToString() == Glo.FK_Node.ToString())
                    continue;

                ListBoxItem lb = new ListBoxItem();
                lb.Name = "LB" + dr["NodeID"].ToString();
                lb.Tag = dr["NodeID"].ToString();
                lb.Content = "Node:" + dr["Name"].ToString() + " Step:" + dr["Step"];
                this.listBox1.Items.Add(lb);
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.listBox1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择要批处理的项目，Ctrl+ 鼠标可以实现多选。", "提示", MessageBoxButton.OK);
                return;
            }

            string nodeIDs = "";
            foreach (ListBoxItem lb in this.listBox1.SelectedItems)
            {
                nodeIDs += lb.Tag + ",";
            }

            BPTextBox tb = Glo.currEle as BPTextBox;

            // this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

