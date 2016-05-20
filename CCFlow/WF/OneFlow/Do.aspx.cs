using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.DA;
using BP.En;

namespace CCFlow.WF.OneFlow
{

    public partial class WF_OneFlow_Do : BP.Web.WebPage
    {
        #region 属性.
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            string sql = "";
            Paras ps = new Paras();
            string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
            switch (this.Request.QueryString["DoType"])
            {
                case "DelDraft":
                    BP.Sys.MapData md = new BP.Sys.MapData("ND"+this.FK_Node);
                    sql = "SELECT count(*) FROM " + md.PTable + " WHERE Rec=" + dbstr + "Rec AND OID=" + dbstr + "OID";
                    ps.SQL = sql;
                    ps.Add("Rec", BP.Web.WebUser.No);
                    ps.Add("OID", this.WorkID);
                    if (DBAccess.RunSQLReturnValInt(ps) == 0)
                        throw new Exception("非法的用户删除草稿.");

                    sql = "DELETE  FROM ND" + this.FK_Node + " WHERE Rec=" + dbstr + "Rec AND OID=" + dbstr + "OID";
                    ps.SQL = sql;
                    DBAccess.RunSQL(ps);
                    this.WinClose();
                    break;
                default:
                    break;
            }
        }
    }
}