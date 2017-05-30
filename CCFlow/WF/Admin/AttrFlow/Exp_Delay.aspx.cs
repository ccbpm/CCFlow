using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using System.IO;
using BP.Web;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class Exp : System.Web.UI.Page
    {
        #region    参数
        private BP.WF.CloudWS.WSSoapClient ccflowCloud
        {
            get
            {
                BP.WF.CloudWS.WSSoapClient cloud = BP.WF.Cloud.Glo.GetSoap();
                return cloud;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        private Flow flow
        {
            get
            {
                Flow flow = new Flow(this.Request.QueryString["FK_Flow"]);
                return flow;
            }
        }
        #endregion 参数
        protected void Page_Load(object sender, EventArgs e)
        {

            System.Data.DataTable dt = null;
            try
            {
                dt = ccflowCloud.GetCloudFlowsDDlTreeDt();
            }
            catch (Exception)
            {
                this.BtnPub.Visible = false;
                this.BtnPri.Visible = false;
                return;
            }

            BP.Web.Controls.DDL.MakeTree(dt, "ParentNo", "0", "No", "Name", DropDownList1, -1);

            try
            {
                dt = ccflowCloud.PriFlowDir(BP.WF.Cloud.CCLover.UserNo, BP.WF.Cloud.CCLover.Password, BP.WF.Cloud.CCLover.GUID);

                BP.Web.Controls.DDL.MakeTree(dt, "ParentNo", "0", "MyPK", "Name", DropDownList2, -1);
            }
            catch (Exception)
            {
            }


        }
        protected void BtnPub_Click(object sender, EventArgs e)
        {
            if (this.DropDownList1.SelectedValue == "1")
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert('请选择具体类别!');</script>");
                return;
            }
            //获取此流程的xml模版路径

            Nodes nds = flow.HisNodes;

            string nodeNames = "";
            foreach (Node nd in nds)
            {
                nodeNames = nodeNames + "@" + nd.Name;
            }

            string xmlPath = flow.GenerFlowXmlTemplete();
            //读取xml文件
            StreamReader sr = new StreamReader(xmlPath, System.Text.Encoding.UTF8);
            string xmlStr = sr.ReadToEnd();
            sr.Close();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xmlStr);

            //调用服务保存数据.
            bool isSave = ccflowCloud.SaveToFlowTemplete(this.DropDownList1.SelectedValue, flow.Name, bytes, BP.WF.Cloud.CCLover.UserNo, nodeNames, nds.Count);
            if (isSave)  //成功执行上传
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert('操作成功,我们需要审核后发布,感谢您的分享.');</script>");
                this.BtnPub.Enabled = false;//禁用按钮.
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert('操作失败!');</script>");
            }
        }

        protected void BtnPri_Click(object sender, EventArgs e)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(BP.WF.Cloud.CCLover.UserNo) || string.IsNullOrWhiteSpace(BP.WF.Cloud.CCLover.GUID))
                {
                    this.Response.Redirect("RegUser.aspx");
                    return;
                }
            }
            catch (Exception)
            {
                this.Response.Redirect("RegUser.aspx");
                return;
            }

            //获取此流程的xml模版路径
            Nodes nds = flow.HisNodes;

            string nodeNames = "";
            foreach (Node nd in nds)
            {
                nodeNames = nodeNames + "@" + nd.Name;
            }

            string xmlPath = flow.GenerFlowXmlTemplete();
            //读取xml文件
            StreamReader sr = new StreamReader(xmlPath, System.Text.Encoding.UTF8);
            string xmlStr = sr.ReadToEnd();
            sr.Close();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(xmlStr);

            //调用服务保存数据.
            bool isSave = ccflowCloud.SavePriFlowTemplete(this.DropDownList2.SelectedValue,
                flow.Name, bytes, BP.WF.Cloud.CCLover.UserNo, nodeNames, nds.Count);
            if (isSave)  //成功执行上传
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert('流程已经成功导入您的私有云.');</script>");
                this.BtnPri.Enabled = false;//禁用按钮.
            }
            else
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>alert('操作失败!');</script>");
            }
        }
    }
}