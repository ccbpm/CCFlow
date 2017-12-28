using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndustrialAutomationWorkflow
{
    public partial class FrmInputParams : Form
    {
        public FrmInputParams()
        {
            InitializeComponent();
        }

        public string Params { get; private set; }

        private void FrmInputText_Shown(object sender, EventArgs e)
        {
          //  txtParams.Text = string.Empty;
            Params = string.Empty;
            txtParams.Focus();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //检查格式
            bool isDone = true;
            string[] ps = txtParams.Text.Trim().Split(new[] {'@'}, StringSplitOptions.RemoveEmptyEntries);

            foreach(string p in ps)
            {
                if(p.Split('=').Length != 2)
                {
                    isDone = false;
                    break;
                }
            }

            if(!isDone)
            {
                MessageBox.Show("请输入正确的参数格式！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtParams.Focus();
                return;
            }

            Params = txtParams.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void FrmInputParams_Load(object sender, EventArgs e)
        {

        }
    }
}
