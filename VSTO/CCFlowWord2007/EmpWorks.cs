using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BP.WF;
using BP.Web;
using BP.Port;

namespace CCFlowWord2007
{
    public partial class EmpWorks : Form
    {
        public EmpWorks()
        {
            InitializeComponent();
            DataTable dt = BP.WF.Dev2Interface.GetDesignerServiceInstance().DataTable_DB_GenerEmpWorksOfDataTable(BP.Web.WebUser.No);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("您没有拟定公文的权限.", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int type = (int)BP.WF.FlowAppType.DocFlow;
            string sql = "SELECT No,Name FROM WF_Flow WHERE FlowAppType=" + type;
            DataTable dtFlow = BP.WF.Dev2Interface.GetDesignerServiceInstance().RunSQLReturnTable2DataTable(sql);

            string flows = "";
            foreach (DataRow dr in dtFlow.Rows)
            {
                flows += "@" + dr["No"].ToString();
            }

            this.treeView1.Nodes.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                TreeNode tn = new TreeNode();
                string fk_flow = dr["FK_Flow"].ToString();
                if (flows.Contains("@" + fk_flow) == false)
                    continue; /*过滤到非doc 流程的待办。*/

                tn.Tag = "@" + dr["FK_Flow"] + "@" + dr["FK_Node"] + "@" + dr["WorkID"] + "@" + dr["FID"];
                tn.Text = dr["Title"].ToString() + " 发送人:" + dr["StarterName"].ToString();
                this.treeView1.Nodes.Add(tn);
            }

            if (treeView1.Nodes.Count == 0)
            {
                MessageBox.Show("您目前没有待办", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }
        }
        private void EmpWorks_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TreeNode tn = this.treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("请选择一个待办工作", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //设置参数后，就关闭.
            this.Tag = tn.Tag.ToString();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            return; 
        }
    }
}
