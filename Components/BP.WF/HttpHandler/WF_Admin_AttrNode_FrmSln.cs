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
    public class WF_Admin_AttrNode_FrmSln : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_AttrNode_FrmSln()
        {
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string RefOneFrmTreeFrms_Init()
        {
            string sql = "";
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            {
                sql += "SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,";
                sql += "A.OrgNo";
                sql += "FROM";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                //sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                sql += "ORDER BY B.IDX,A.IDX";

            }
            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                sql += "SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,";
                sql += "A.OrgNo";
                sql += "FROM";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                sql += "ORDER BY B.IDX,A.IDX";
            }

            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                sql += "SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,";
                sql += "A.OrgNo";
                sql += "FROM";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                sql += "ORDER BY B.IDX,A.IDX";

                sql += " UNION ";

                sql += "SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,A.OrgNo ";
                sql += " FROM ";
                sql += "Sys_MapData A, Sys_FormTree B, WF_FrmOrg C ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.No ";
                sql += " AND B.OrgNo = C.OrgNo ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                sql += "ORDER BY B.IDX,A.IDX";
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            #warning 需要判断不同的数据库类型
            if (SystemConfig.AppCenterDBType == DBType.Oracle || SystemConfig.AppCenterDBType == DBType.PostgreSQL)
            {
                dt.Columns["SORTNAME"].ColumnName = "SortName";
                dt.Columns["NO"].ColumnName = "No";
                dt.Columns["NAME"].ColumnName = "Name";
                dt.Columns["PTABLE"].ColumnName = "PTable";
                dt.Columns["ORGNO"].ColumnName = "OrgNo";
            }

            return BP.Tools.Json.ToJson(dt);
        }

    }
}
