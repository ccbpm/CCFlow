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
using BP.En;
using BP.Sys;

namespace CCForm
{
    public partial class NodeFrms : ChildWindow
    {
        public int FK_Node = 0;
        public NodeFrms()
        {
            InitializeComponent();
        }
        protected override void OnOpened()
        {
            this.listBox1.Items.Clear();
           // string sqls = "SELECT * FROM Sys_MapData WHERE FK_FrmSort!=''";
            string sqls = "SELECT * FROM Sys_MapData ";
            sqls += "@SELECT * FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node;

            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableSAsync(sqls, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableSCompleted += new EventHandler<FF.RunSQLReturnTableSCompletedEventArgs>(da_RunSQLReturnTableSCompleted);
            base.OnOpened();
        }
        public DataSet ds = new DataSet();
        void da_RunSQLReturnTableSCompleted(object sender, FF.RunSQLReturnTableSCompletedEventArgs e)
        {
            ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dtFrm = ds.Tables[0];
            DataTable dtFrmNode = ds.Tables[1];

            this.listBox1.Items.Clear();
            foreach (DataRow dr in dtFrm.Rows)
            {
                CheckBox cb = new CheckBox();
                cb.Name = dr["No"];
                cb.Content = dr["Name"];

                if (string.IsNullOrEmpty(dr["FK_FrmSort"]))
                    continue;

                this.listBox1.Items.Add(cb);
                cb.Tag = 0;
                foreach (DataRow drFrmNode in dtFrmNode.Rows)
                {
                    string fk_frm = drFrmNode["FK_Frm"];
                    if (fk_frm == cb.Name)
                    {
                        cb.IsChecked = true;
                        cb.Tag = 1;
                    }
                }
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            string sqls = "@DELETE WF_FrmNode WHERE FK_Node='" + this.FK_Node + "'";
            foreach (CheckBox cb in this.listBox1.Items)
            {
                if (cb.IsChecked == true)
                {
                    sqls += "@INSERT INTO WF_FrmNode (MyPK,FK_Flow,FK_Node,FK_Frm)VALUES('" + cb.Name + "_" + this.FK_Node + "','" + Glo.FK_Flow + "','" + this.FK_Node + "','" + cb.Name + "')";
                    continue;
                }
            }
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLsAsync(sqls, Glo.UserNo, Glo.SID);
            da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(da_RunSQLsCompleted);
        }
        void da_RunSQLsCompleted(object sender, FF.RunSQLsCompletedEventArgs e)
        {
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}

