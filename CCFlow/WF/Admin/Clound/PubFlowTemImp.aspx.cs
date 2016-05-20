using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.Comm;
using BP.DA;
using System.Data;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class PubFlowTemImp : WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //测试连接是否可用
            try
            {
                BP.WF.CloudWS.WSSoapClient ccflowCloud = BP.WF.Cloud.Glo.GetSoap();
                ccflowCloud.GetFlowTemplateByGuid(this.Request.QueryString["GUID"]);
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            string sql = "SELECT No,Name,ParentNo FROM WF_FlowSort";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            BP.Web.Controls.DDL.MakeTree(dt, "ParentNo", "0", "No", "Name", this.DropDownList1, -1);
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string guid = this.Request.QueryString["GUID"];
                BP.WF.CloudWS.WSSoapClient ccflowCloud = BP.WF.Cloud.Glo.GetSoap();
                DataTable dt = ccflowCloud.GetFlowTemplateByGuid(guid);


                byte[] bytes = ccflowCloud.GetFlowXML(true, guid);

                string path = BP.Sys.SystemConfig.PathOfDataUser + "CloundFlow\\Public";
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);

                string xmlStr = System.Text.Encoding.UTF8.GetString(bytes);
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(xmlStr);

                string fileName = dt.Rows[0]["NAME"].ToString() +
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";

                fileName = fileName.Replace("/", "、");
                xml.Save(path + "\\" + fileName);

                path = BP.Sys.SystemConfig.PathOfDataUser + "CloundFlow\\Public\\" + fileName;

                int SpecifiedNumber = 0;
                BP.WF.ImpFlowTempleteModel model = BP.WF.ImpFlowTempleteModel.AsNewFlow;
                //作为新流程导入(由ccbpm自动生成新的流程编号)
                if (this.RB_Import_1.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.AsNewFlow;
                }
                //作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会提示错误)
                if (this.RB_Import_2.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.AsTempleteFlowNo;
                }
                //作为新流程导入(使用流程模版里面的流程编号，如果该编号已经存在系统则会覆盖此流程)
                if (this.RB_Import_3.Checked)
                {
                    model = BP.WF.ImpFlowTempleteModel.OvrewaiteCurrFlowNo;
                }

                //导入并覆盖当前的流程
                if (this.RB_Import_4.Checked)
                {
                    String StrSpecifiedNumber = this.SpecifiedNumber.Text;
                    if (StrSpecifiedNumber == null)
                    {
                        this.Alert("请输入指定流程编号。");
                        return;
                    }

                    SpecifiedNumber = Convert.ToInt32(StrSpecifiedNumber);
                    model = BP.WF.ImpFlowTempleteModel.AsSpecFlowNo;
                }

                //执行导入
                BP.WF.Flow flow = BP.WF.Flow.DoLoadFlowTemplate(this.DropDownList1.SelectedValue, path, model, SpecifiedNumber);

                if (flow.No != "")
                {
                    //调用客户端脚本, 是否在设计器中打开流程
                    ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>openFlow('" + flow.DType + "','" +
                        flow.Name + "','" + flow.No + "','" + WebUser.No + "','" + WebUser.SID + "');</script>");

                    //导入成功禁用导入按钮
                    this.Button1.Enabled = false;
                }
                else
                {
                    this.Alert("导入失败");
                }
            }
            catch(Exception ex)
            {
                this.Response.Write("导入失败："+ex.Message);
            }
        }
    }
}