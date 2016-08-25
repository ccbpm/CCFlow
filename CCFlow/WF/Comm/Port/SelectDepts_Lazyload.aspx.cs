using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Port;
using BP.Tools;

namespace CCFlow.WF.Comm.Port
{
    public partial class SelectDepts_Lazyload : System.Web.UI.Page
    {
        public string ParentNo
        {
            get
            {
                return Request.QueryString["pno"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request.QueryString["method"];
            string re = string.Empty;

            switch(method)
            {
                case "getdepts":
                    if(string.IsNullOrWhiteSpace(ParentNo))
                    {
                        re = "[]";
                    }
                    else
                    {
                        string sql = string.Format("SELECT NO,NAME FROM Port_Dept WHERE ParentNo = '{0}' ORDER BY Idx ASC", ParentNo);
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        re = FormatToJson.ToJson(dt, "data");
                    }
                    break;
                default:
                    return;
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(re);
            Response.End();
        }
    }
}