using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BP.WF;
using BP.Port;
using BP.Web;

namespace CCFlowWord2007
{
    public partial class ReturnFrm : Form
    {
        public ReturnFrm()
        {
            InitializeComponent();
        }

        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Btn_Return_Click(object sender, EventArgs e)
        {
            try
            {
                int toNode = int.Parse(this.comboBox1.SelectedValue.ToString());
                bool isback = false;
                string info = BP.WF.Dev2Interface.GetDesignerServiceInstance().Node_ReturnWork(WebUser.FK_Flow, WebUser.WorkID,
                    WebUser.FID,
                    WebUser.FK_Node, toNode, this.textBox1.Text, isback, WebUser.No, WebUser.SID);

                MessageBox.Show(info, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ReturnFrm_Load(object sender, EventArgs e)
        {
            DataTable dt = BP.WF.Dev2Interface.GetDesignerServiceInstance().DataTable_DB_GenerWillReturnNodes(BP.Web.WebUser.FK_Node,
                WebUser.WorkID, WebUser.FID, WebUser.No);

            foreach (DataRow dr in dt.Rows)
            {
                dr["Name"] = dr["RecName"].ToString() + dr["Name"].ToString();
            }

            this.comboBox1.Items.Clear();
            this.comboBox1.DataSource = dt;
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "No";
            this.comboBox1.SelectedIndex = 0;
        }
    }
}
