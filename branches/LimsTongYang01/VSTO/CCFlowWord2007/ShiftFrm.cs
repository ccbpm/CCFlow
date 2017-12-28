using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BP.WF;
using BP.Port;
using BP.Web;

namespace CCFlowWord2007
{
    public partial class ShiftFrm : Form
    {
        public ShiftFrm()
        {
            InitializeComponent();
        }

        private void Btn_OK_Click(object sender, EventArgs e)
        {
            string toEmp=this.textBox1.Text.Trim();
            if (string.IsNullOrEmpty(toEmp))
            {
                MessageBox.Show("没有选择接受人.");
                return;
            }

            try
            {
                string info = BP.WF.Dev2Interface.GetDesignerServiceInstance().Node_Shift(WebUser.WorkID, toEmp, this.textBox2.Text, 
                    WebUser.No, WebUser.SID);
 
                MessageBox.Show(info, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
