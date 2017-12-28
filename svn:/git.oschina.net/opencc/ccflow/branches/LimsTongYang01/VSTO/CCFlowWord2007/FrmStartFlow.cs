using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using BP.WF;
using BP.Web;

namespace CCFlowWord2007
{
    public partial class FrmStartFlow : Form
    {
        public FrmStartFlow()
        {
            InitializeComponent();
        }

        private void FrmStartFlow_Load(object sender, EventArgs e)
        {
            DataTable dt = null;
            try
            {
                dt = BP.WF.Dev2Interface.DataTable_DB_GenerCanStartFlowsOfDataTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                MessageBox.Show(ex.Message);
                return;
            }

            this.treeView1.Nodes.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                FlowAppType appType = (FlowAppType)int.Parse(dr["FlowAppType"].ToString());
                if (appType != FlowAppType.DocFlow)
                    continue;

                TreeNode tn = new TreeNode();
                tn.Text = dr["Name"].ToString();
                tn.Tag = dr["No"].ToString();
                this.treeView1.Nodes.Add(tn);
            }
        }
        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            //FrmStartFlow_Load(null, null);
            this.Close();
        }
        /// <summary>
        /// 新建流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Start_Click(object sender, EventArgs e)
        {
            TreeNode tn = this.treeView1.SelectedNode;
            if (tn == null)
            {
                MessageBox.Show("请选择一个流程，您才能发起它。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //流程编号。
            string fk_flow = tn.Tag.ToString();
            BP.Web.WebUser.FK_Flow = fk_flow;

            //生成新的workid.
            WebUser.WorkID = BP.WF.Dev2Interface.GetDesignerServiceInstance().Node_CreateBlankWork(WebUser.FK_Flow,
                WebUser.No, null);


            #region 首先判断服务器上有没有该流程的草稿文件，如果有就掉出来.
            FtpSupport.FtpConnection conn = Glo.HisFtpConn;
            try
            {
                conn.SetCurrentDirectory("/DocFlow/" + WebUser.FK_Flow + "/" + WebUser.WorkID + "/");
                bool isHave = conn.FileExist(WebUser.WorkID + ".doc");
                if (isHave == false)
                {
                    /*如果不存在就向下走。 */
                }
                else
                {
                    if (MessageBox.Show("我们发现了您曾经保存过草稿，您想打开这个草稿吗？", "提示",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        try
                        {
                            /*下载下来，并打开它.*/
                            string fileTemp = Glo.PathOfTInstall + fk_flow + "@" + DateTime.Now.ToString("MM月dd日hh时mm分ss秒") + ".doc";
                            conn.GetFile(WebUser.WorkID + ".doc", fileTemp, true, FileAttributes.Archive);
                            conn.Close();
                            MessageBox.Show("草稿已经被打开", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Glo.OpenDoc(fileTemp, false);
                            this.DialogResult = System.Windows.Forms.DialogResult.OK;
                            this.Close();
                            return;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "草稿打开失败", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        /*不处理，打开模版.*/
                    }
                }
            }
            catch
            {
                /*说明没有此文件,就向下执行。*/
            }
            #endregion

            // 下载流程模板 
            conn = Glo.HisFtpConn;
            string file = Glo.PathOfTInstall + fk_flow + "@" + DateTime.Now.ToString("MM月dd日hh时mm分ss秒") + ".doc";
            try
            {
                conn.SetCurrentDirectory("/DocFlowTemplete/");
                if (conn.FileExist(fk_flow + ".doc") == false)
                    throw new Exception("@没有为公文启动设置模板。");

                conn.GetFile(fk_flow + ".doc", file, true, FileAttributes.Archive);
                conn.Close();

                Glo.OpenDoc(file, false);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                file = null;
                MessageBox.Show("@异常信息:" + ex.Message + "\t\n@流程编号：" + WebUser.FK_Flow + "\t\n@可能的原因如下：1，设计人员没有正确的设置ftp服务器。 \t\n2，没有该流程的公文模板。");
            }

            WebUser.FK_Flow = fk_flow;
            WebUser.FK_Node = int.Parse(fk_flow + "01");

            //  WebUser.RetrieveWFNode(WebUser.FK_Node);
            //    WebUser.WriterIt(StartFlag.DoNewFlow, fk_flow, int.Parse(fk_flow + "01"), 0);
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
