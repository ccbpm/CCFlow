using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;

namespace CCFlow
{
    public partial class ProjFlowView : WebPage
    {
        #region 流程编号
        /// <summary>
        /// 流程编号
        /// </summary>
        private string FK_Flow
        {
            get
            {
                return Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        private string WorkID
        {
            get
            {
                return Request.QueryString["OID"];
            }
        }
        /// <summary>
        /// 节点表单
        /// </summary>
        private string FK_Node
        {
            get
            {
                return Request.QueryString["FK_Node"];
            }
        }
        /// <summary>
        /// 项目编号
        /// </summary>
        private string ProjNo
        {
            get
            {
                return Request.QueryString["ProjNo"];
            }
        }
        /// <summary>
        /// 展示方式
        /// </summary>
        private string FrmView
        {
            get
            {
                return Request.QueryString["FrmView"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

            string strsql = @"select * from WF_GenerWorkFlow where WorkID not in (
select distinct WorkID from WF_GenerWorkerlist) and WFState<> 3 and WFState<>7 and RDT > '2014-10-01 00:00' order by RDT desc ";
            DataTable dt = DBAccess.RunSQLReturnTable(strsql);
            foreach (DataRow row in dt.Rows)
            {
                string empwors = row["TodoEmps"].ToString();
                if (!string.IsNullOrEmpty(empwors))
                {
                    string[] emps = empwors.Split(';');
                    if (emps != null && emps.Length > 0)
                    {
                        foreach (string str in emps)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                string[] emp = str.Split(',');
                                BP.WF.GenerWorkerList list = new GenerWorkerList();
                                list.WorkID = Convert.ToInt64(row["WorkID"].ToString());
                                list.FK_Emp = emp[0];
                                list.FK_EmpText=emp[1];
                                list.FK_Node = Convert.ToInt32(row["FK_Node"].ToString());
                                list.FK_NodeText = row["NodeName"].ToString();
                                list.FID = Convert.ToInt64(row["FID"].ToString());
                                list.FK_Flow = row["FK_Flow"].ToString();
                                list.FK_Dept= row["FK_Dept"].ToString();
                                list.SDT = row["SDTOfNode"].ToString();
                                list.DTOfWarning = row["SDTOfFlow"].ToString();
                                list.WarningHour = 0;
                                list.RDT = row["RDT"].ToString();
                                list.IsEnable = true;
                                list.IsPass = false;
                                list.WhoExeIt = 0;
                                list.Sender = row["Starter"].ToString();
                                list.PRI =1;
                                list.IsRead = false;
                                list.PressTimes = 0;
                                list.HungUpTimes = 0;
                                list.CDT = row["RDT"].ToString();
                                list.Insert();
                            }
                        }
                    }
                }
            }

            try
            {
                string url = "../WF/WFRpt.aspx?DoType=View&WorkID={0}&FK_Flow={1}&FK_Node={2}";
                string sql = "SELECT top 1 OID FROM RMS.dbo.V_FlowData WHERE ProjNo in (SELECT ProjNo FROM ND137Rpt WHERE OID='{0}') AND FK_Flow='{1}' ORDER BY OID DESC";
                //列表展示
                if (this.FrmView == "1")
                {
                    sql = "SELECT ProjNo FROM ND137Rpt WHERE OID='{0}'";
                    sql = string.Format(sql, this.WorkID);
                    string projNo = DBAccess.RunSQLReturnStringIsNull(sql, "0");
                    url = "ProjectInfoView.aspx?FK_Flow=" + this.FK_Flow + "&id=" + projNo;
                    Response.Redirect(url);
                }
                else
                {
                    //如果传入项目编号，直接使用
                    if (string.IsNullOrEmpty(this.ProjNo))
                    {
                        sql = string.Format(sql, this.WorkID, this.FK_Flow);
                    }
                    else
                    {
                        sql = "SELECT top 1 OID FROM RMS.dbo.V_FlowData WHERE ProjNo ='" + this.ProjNo + "' AND FK_Flow='" + this.FK_Flow + "' ORDER BY OID DESC";
                    }
                    //查到信息进行跳转，查不到进行提示
                    string OID = BP.DA.DBAccess.RunSQLReturnString(sql);
                    if (!string.IsNullOrEmpty(OID))
                    {
                        url = string.Format(url, OID, this.FK_Flow, this.FK_Node);
                        Response.Redirect(url);
                    }
                }
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);

            }
            Response.Write("所选信息不存在！");

        }
    }
}