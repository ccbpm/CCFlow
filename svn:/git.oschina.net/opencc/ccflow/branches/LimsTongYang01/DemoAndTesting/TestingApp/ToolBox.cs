using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CCFlowServices
{
    public partial class ToolBox : Form
    {
        public ToolBox()
        {
            InitializeComponent();
        }


        private void Btn_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Btn_Imp_Click(object sender, EventArgs e)
        {
            // 把流程运行到最后的节点上去，并且结束流程。
            string file = @"C:\aa\流程已完成.xls";
            string info = BP.WF.Glo.LoadFlowDataWithToSpecEndNode(file);
            BP.DA.Log.DefaultLogWriteLineInfo(info);

            // 流程未完成
            string file1 = @"C:\aa\流程未完成.xls";
            string info1 = BP.WF.Glo.LoadFlowDataWithToSpecNode(file1);
            BP.DA.Log.DefaultLogWriteLineInfo(info1);
            MessageBox.Show("执行成功。");
        }
        private void Btn_DTSNDxxxRpt_Click(object sender, EventArgs e)
        {
            this.Btn_DTSNDxxxRpt.Enabled = false;
            this.statusStrip1.Text = "正在执行，请等待，执行时间:"+System.DateTime.Now.ToString("dd日HH时mm分");
            BP.WF.DTS.ReLoadNDxxxxxxRpt rpt = new BP.WF.DTS.ReLoadNDxxxxxxRpt();
            string msg= rpt.Do() as string;
            this.Btn_DTSNDxxxRpt.Enabled = true;
            MessageBox.Show(msg);
        }

        private void Btn_ChOfNode_Click(object sender, EventArgs e)
        {
            this.Btn_ChOfNode.Enabled = false;
            this.statusStrip1.Text = "正在执行，请等待，执行时间:" + System.DateTime.Now.ToString("dd日HH时mm分");
            ////BP.WF.DTS.ReLoadCHOfNode rpt = new BP.WF.DTS.ReLoadCHOfNode();
            //string msg = rpt.Do() as string;
            //this.Btn_ChOfNode.Enabled = true;
            ///   MessageBox.Show(msg);
            MessageBox.Show("已经取消.");
        }

        private void ToolBox_Load(object sender, EventArgs e)
        {
        }

        private void Btn_XL_Click(object sender, EventArgs e)
        {
            TestingApp.FrmTesting ft = new TestingApp.FrmTesting();
            ft.ShowDialog();
        }
    }
}
