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
using BP.Web;
using BP.Port;
using BP.En;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_DoPort : System.Web.UI.Page
    {

        #region 属性
        public string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string UserNo
        {
            get
            {
                return this.Request.QueryString["UserNo"];
            }
        }
        public string UserPass
        {
            get
            {
                return this.Request.QueryString["UserPass"];
            }
        }
        public string SID
        {
            get
            {
                return this.Request.QueryString["SID"];
            }
        }
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "CheckPower":
                    this.CheckPower();
                    break;
                case "GetNewMsg":
                    this.GetNewMsg();
                    break;
                case "OpenWork":
                    Emp emp = new Emp(this.UserNo);
                    emp.No = this.UserNo;
                    emp.RetrieveFromDBSources();

                    BP.Web.WebUser.SignInOfGenerLang(emp, this.Lang);
                    this.Response.Redirect("../MyFlow.aspx?OID=" + this.Request.QueryString["WorkID"] + "&FK_Flow=" + this.Request.QueryString["FK_Flow"], true);
                    return;
                default:
                    throw new Exception("错误的标记:" + this.DoType);
            }
        }
        public void CheckPower()
        {
            Emp emp = new Emp();
            emp.No = this.UserNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("");
                return;
            }

            if (emp.Pass == this.UserPass)
                this.Response.Write("1");
        }
        public void GetNewMsg()
        {
            string sql = "SELECT WorkID, FK_Node , FK_Emp, '001' as FK_Flow,  RDT FROM WF_GenerWorkerlist  WHERE RDT='" + BP.DA.DataType.CurrentData + "' AND ISENABLE=1 AND FK_EMP='" + this.UserNo + "'";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                this.Response.Write("");
                return;
            }

            string html = "<html>";

            html += "<link href='../Comm/Style.css' rel='stylesheet' type='text/css' />";
            html += "<link href='../Comm/Table.css' rel='stylesheet' type='text/css' />";

            html += "<body>";
            html += "<table class=Table>";
            html += "<TR>";
            html += "<TD class=Title>工作名称</TD>";
            html += "<TD class=Title>标题</TD>";
            html += "<TD class=Title>发送人</TD>";
            html += "<TD class=Title>发送时间</TD>";
            html += "</TR>";
            foreach (DataRow dr in dt.Rows)
            {
                html += "<TR>";
                html += "<TD class=TD><a href='DoPort.aspx?DoType=OpenWork&UserNo=" + this.UserNo + "&FK_Flow=" + dr["FK_Flow"] + "&SID=" + this.Session.SessionID + "' traget=_blank>" + dr["WorkID"] + "</a></TD>";
                html += "<TD class=TD>" + dr["WorkID"] + "</TD>";
                html += "<TD class=TD>" + dr["FK_Emp"] + "</TD>";
                html += "<TD class=TD>" + dr["RDT"] + "</TD>";
                html += "</TR>";
            }
            html += "</Table>";
            html += "</body>";
            html += "</html>";

            this.Response.Write(html);
        }


    }

}