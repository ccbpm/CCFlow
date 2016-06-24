using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web.Security;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls.WebParts;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using BP.Port;
namespace CCFlow.WF.WorkOpt
{
    public partial class ChartTrackV2 : System.Web.UI.Page
    {
        #region 属性
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public int OID
        {
            get
            {
                return int.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public string FK_Node
        {
            get { return this.Request.QueryString["FK_Node"]; }
        }
       
        public int StartNodeID
        {
            get
            {
                return int.Parse(this.FK_Flow + "01");
            }
        }
        public string FK_Flow
        {
            get
            {
                string flow = this.Request.QueryString["FK_Flow"];
                if (flow == null)
                {
                    throw new Exception("@没有获取它的流程编号。");
                }
                else
                {
                    return flow;
                }
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["NodeID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var workId = Request.QueryString["WorkID"];
                var fk_flow = Request.QueryString["FK_Flow"];
                var fid = Request.QueryString["FID"];
                string url = string.Empty;

                url = "ChartTrack.htm?FID=" + fid + "&FK_Flow=" + fk_flow + "&WorkID=" + workId;
                //content.Attributes.Add("src", url);
            }
            catch (Exception ee)
            {
                Response.Write("轨迹图加载错误：" + ee.Message);
            }
           
            string isZhidu = this.Request.QueryString["isZhiDu"];
            if (string.IsNullOrEmpty(isZhidu) == false)
            {

                string zhiduNo = this.Request.QueryString["ZhiDuNo"];
                if (!string.IsNullOrEmpty(zhiduNo))
                {
                    try
                    {
                        //BP.MS.ZhiDu zhidu = new BP.MS.ZhiDu();
                        //zhidu.No = zhiduNo;
                        //zhidu.Retrieve();

                        GenerWorkFlow workflow = new GenerWorkFlow();
                        workflow.WorkID = 0;// Int64.Parse(zhidu.OID);
                        workflow.Retrieve();
                        this.Response.Redirect("Ath.aspx?FK_Flow=" + workflow.FK_Flow + "&WorkID=" + workflow.WorkID + "&FID=" + workflow.FID + "&FK_Node=" + workflow.FK_Node, true);

                        return;
                    }
                    catch (Exception)
                    {
                        throw new Exception("该流程未完成或未找到对应的流程数据!");
                    }
                }
            }

            string flowID = int.Parse(this.FK_Flow).ToString();
            string sql = "";
            if (string.IsNullOrEmpty(FK_Node))
                sql = "SELECT * FROM Sys_FrmAttachmentDB WHERE FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  " + BP.WF.Glo.MapDataLikeKey(this.FK_Flow, "FK_MapData") + "  AND IsUpload=1) AND RefPKVal='" + this.OID + "' ORDER BY RDT";
            else
                sql = "SELECT * FROM Sys_FrmAttachmentDB WHERE FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  FK_MapData='ND" + FK_Node + "' ) AND RefPKVal='" + this.OID + "' ORDER BY RDT";

            //string sql = "SELECT * FROM Sys_FrmAttachmentDB WHERE FK_FrmAttachment IN (SELECT MyPK FROM Sys_FrmAttachment WHERE  FK_MapData ='ND"+FK_Node+"'  AND IsUpload=1) AND RefPKVal='" + this.OID + "' ORDER BY RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            if (dt.Rows.Count > 0)
            {
                this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width: 100%'");
                this.Pub1.AddTR();
                this.Pub1.AddTDGroupTitle("style='text-align:center'", "序");
                this.Pub1.AddTDGroupTitle("附件编号");
                this.Pub1.AddTDGroupTitle("节点");
                this.Pub1.AddTDGroupTitle("名称");
                this.Pub1.AddTDGroupTitle("大小(kb)");
                this.Pub1.AddTDGroupTitle("上传人");
                this.Pub1.AddTDGroupTitle("上传日期");
                this.Pub1.AddTREnd();
                int i = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    i++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(i);
                    this.Pub1.AddTD(dr["FK_FrmAttachment"].ToString());
                    string nodeName = "";
                    try
                    {
                        int nodeID = int.Parse(dr["NodeID"].ToString());
                        Node node = new Node(nodeID);
                        nodeName = node.Name;

                    }
                    catch
                    {

                    }

                    this.Pub1.AddTD(nodeName);
                    this.Pub1.AddTD("<a href='/WF/CCForm/AttachmentUpload.aspx?DoType=Down&MyPK=" + dr["MyPK"] + "' target=_sd ><img src='/WF/Img/FileType/" + dr["FileExts"] + ".gif' onerror=\"this.src='/WF/Img/FileType/Undefined.gif'\" border=0/>" + dr["FileName"].ToString() + "</a>");
                    this.Pub1.AddTD(dr["FileSize"].ToString());
                    this.Pub1.AddTD(dr["RecName"].ToString());
                    this.Pub1.AddTD(dr["RDT"].ToString());
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
            }

            Bills bills = new Bills();
            bills.Retrieve(BillAttr.WorkID, this.OID);
            if (bills.Count > 0)
            {
                this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width: 100%'");
                this.Pub1.AddTR();
                this.Pub1.AddTDGroupTitle("style='text-align:center'", "序");
                this.Pub1.AddTDGroupTitle("名称");
                this.Pub1.AddTDGroupTitle("节点");
                this.Pub1.AddTDGroupTitle("打印人");
                this.Pub1.AddTDGroupTitle("日期");
                this.Pub1.AddTDGroupTitle("功能");
                this.Pub1.AddTREnd();
                int idx = 0;
                foreach (Bill bill in bills)
                {
                    idx++;
                    this.Pub1.AddTR();
                    this.Pub1.AddTDIdx(idx);
                    this.Pub1.AddTD(bill.FK_BillTypeT);
                    this.Pub1.AddTD(bill.FK_NodeT);
                    this.Pub1.AddTD(bill.FK_EmpT);
                    this.Pub1.AddTD(bill.RDT);
                    this.Pub1.AddTD("<a class='easyui-linkbutton' data-options=\"iconCls:'icon-print'\" href='" + this.Request.ApplicationPath + "WF/Rpt/Bill.aspx?MyPK=" + bill.MyPK + "&DoType=Print' >打印</a>");
                    this.Pub1.AddTREnd();
                }
                this.Pub1.AddTableEnd();
            }


            int num = bills.Count + dt.Rows.Count;
            if (num == 0)
            {
                Pub1.AddEasyUiPanelInfo("提示", "<h3>当前流程没有数据，或者该流程没有附件或者单据。</h3>");
            }
        }
        //删除
       

        protected void LogOut_Click(object sender, EventArgs e)
        {

            string info = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
            if (info.Equals("删除成功"))
            {
                BP.Sys.PubClass.Alert("删除流程成功");
            }
            else
            {
                BP.Sys.PubClass.Alert("删除流程失败");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {

            try
            {
                string str1 = BP.WF.Dev2Interface.Flow_DoUnSend(FK_Flow, WorkID);
                BP.WF.Glo.ToMsg(str1);
                return;
            }
            catch (Exception ex)
            {
                BP.WF.Glo.ToMsgErr(ex.Message);
                return;
            }
        }
    }
}