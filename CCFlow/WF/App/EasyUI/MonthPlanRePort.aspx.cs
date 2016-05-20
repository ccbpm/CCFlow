using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.IO;
using System.Web.UI.WebControls;
using System.Data;
using BP;
using BP.En;
using BP.DA;
using BP.Web;
using BP.WF;
using BP.WF.Data;

namespace CCFlow.AppDemoLigerUI
{
    public partial class MonthPlanRePort : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
            "<link href='/WF/Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");

            string sql = "SELECT a.* ,b.FK_Flow,b.FK_Node,b.FlowName,b.NodeName,b.IsRead,b.Starter,b.ADT,b.SDT,b.WorkID FROM ND22Rpt"
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState not in (7)"
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            int gIdx = 0;
            string thStyle = "style='font-size: 12px;border: 1px solid #C2D5E3;text-align: center;height: 25px;line-height: 25px;color: #336699;white-space: nowrap;padding: 0 2px;'";
            string tdStyle = "style='border: 1px solid #D6DDE6;padding: 4px;text-align: left;background-color: #FFFFFF; color: #333333;'";
            this.monthReport.AddTable("style='width:100%;border:1px solid #CCCCCC;padding: inherit;margin: 0;margin-bottom: 0px;border-collapse:collapse;align=center'");
            //添加标题
            this.monthReport.Add("<thead>");
            this.monthReport.AddTR("style='width:100%;border:1px solid #C2D5E3;padding: inherit;margin: 0;margin-bottom: 0px;border-collapse:collapse;align=center'");
            this.monthReport.AddTDTitle(thStyle+" width='5%'", "序号");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "计划项目类别");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "工作项目");
            this.monthReport.AddTDTitle(thStyle, "工作内容及要求");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "完成时间");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "责任部门");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "配合部门");
            this.monthReport.AddTDTitle(thStyle + " width='10%'", "检查部门");
            this.monthReport.AddTREnd();
            this.monthReport.Add("</thead>");
            Flow flow = new Flow("022");
            GERpt rpt = flow.HisGERpt;

            foreach (DataRow row in dt.Rows)
            {
                rpt.OID = int.Parse(row["WorkID"].ToString());
                rpt.Retrieve();

                if (rpt != null)
                {
                    gIdx++;
                    this.monthReport.AddTR("style='height:80px;text-align:center;border:1px solid #D6DDE6;padding: inherit;margin: 0;margin-bottom: 0px;border-collapse:collapse;'");
                    this.monthReport.AddTD("style='text-align:center;border: 1px solid #D6DDE6;padding: 4px;background-color: #FFFFFF; color: #333333;'", gIdx);
                    this.monthReport.AddTD(tdStyle, rpt.GetValByKey("SF_PlanItemText").ToString());
                    this.monthReport.AddTD(tdStyle, row["GongZuoXiangMu"].ToString());
                    this.monthReport.AddTD(tdStyle, "<p>" + row["GZNRJYQ"].ToString() + "</p>");
                    this.monthReport.AddTD(tdStyle, row["WanChengShiJian"].ToString());
                    this.monthReport.AddTD(tdStyle, rpt.GetValByKey("SF_ZRBMText").ToString());
                    this.monthReport.AddTD(tdStyle, row["PeiHeBuMen"].ToString());
                    this.monthReport.AddTD(tdStyle, rpt.GetValByKey("SF_JCBMText").ToString());
                    this.monthReport.AddTREnd();
                }
            }
            this.monthReport.AddTableEnd();

            string request = Request.QueryString["exporttype"];
            if (!string.IsNullOrEmpty(request))
            {
                if (request == "xls")
                {
                    exportexcel();
                }
                if (request == "doc")
                {
                    exportword();
                }
            }
        }
        /// <summary>
        /// 导出Excel
        /// </summary>
        void exportexcel()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "utf-8";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("月计划汇总.xls", Encoding.UTF8).ToString());
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");

            Response.ContentType = "application/ms-excel";
            this.EnableViewState = false;
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            oHtmlTextWriter.WriteLine(RedContro());
            Response.Write(oStringWriter.ToString());
            Response.End();
        }
        /// <summary>
        /// 导出Word
        /// </summary>
        void exportword()
        {
            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "utf-8";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode("月计划汇总.doc", Encoding.UTF8).ToString());
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");

            Response.ContentType = "application/ms-word";
            this.EnableViewState = false;
            System.IO.StringWriter oStringWriter = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter oHtmlTextWriter = new System.Web.UI.HtmlTextWriter(oStringWriter);
            oHtmlTextWriter.WriteLine(RedContro());
            Response.Write(oStringWriter.ToString());
            Response.End();
        }

        private string RedContro()
        {
            string result = "";
            StringBuilder build = new StringBuilder();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(new StringWriter(build));
            try
            {
                this.monthReport.RenderControl(htmlWriter);
            }
            catch { }
            finally
            {
                htmlWriter.Flush();
                result = build.ToString();
            }
            return result;//返回控件的HTML代码 
        }
    }
}