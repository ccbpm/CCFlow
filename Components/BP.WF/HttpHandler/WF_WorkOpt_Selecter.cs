using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_Selecter : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_Selecter()
        {

        }

        public string ByStation_ShowEmps()
        {
            string staNo = this.GetRequestVal("StaNo");
            string sql = "";
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
                sql = "SELECT A.No, A.Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND B.FK_Station='" + staNo + "'";
            else
                sql = "SELECT A.No, A.Name FROM Port_Emp A, Port_DeptEmpStation B WHERE A.No=B.FK_Emp AND A.OrgNo='" + BP.Web.WebUser.OrgNo + "' AND B.FK_Station='" + staNo + "'";

            DataTable db = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(db);
            //return "方法未完成";
        }

        #region  界面 .
        public string SelectEmpsByTeamStation_Init()
        {
            return "方法未完成";
        }
        #endregion 界面方法.

    }
}
