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
        /// 获得下拉框的值.
        /// </summary>
        /// <returns></returns>
        public string BatchEditSln_InitDDLData()
        {
            DataSet ds = new DataSet();

            SysEnums ses = new SysEnums("FrmSln");
            ds.Tables.Add(ses.ToDataTableField("FrmSln"));

            SysEnums se1s = new SysEnums("FWCSta");
            ds.Tables.Add(ses.ToDataTableField("FWCSta"));

            DataTable dt = DBAccess.RunSQLReturnTable(Glo.SQLOfCheckField);
            dt.TableName = "CheckFields";
            ds.Tables.Add(dt);
            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string RefOneFrmTreeFrms_Init()
        {
            string sql = "";
            //单机模式下
            if (Glo.CCBPMRunModel == CCBPMRunModel.Single)
            {
                sql += "SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,";
                sql += "A.OrgNo ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                //sql += " AND B.OrgNo = '" + WebUser.OrgNo + "'";
                sql += "ORDER BY B.IDX,A.IDX";

            }

            // 云服务器环境下
            if (Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
            {
                sql += "SELECT  b.NAME AS SortName, a.No, A.Name, ";
                sql += "A.PTable, ";
                sql += "A.OrgNo ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "' ";
                sql += "ORDER BY B.IDX,A.IDX";
            }

            //集团模式下
            if (Glo.CCBPMRunModel == CCBPMRunModel.GroupInc)
            {
                sql += "select * from (SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,";
                sql += "A.OrgNo ,b.idx as idx1,a.idx as idx2 ";
                sql += "FROM ";
                sql += "Sys_MapData A, ";
                sql += "Sys_FormTree B ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.NO ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "') t1 ";

                sql += " UNION ";

                sql += "select * from (SELECT  b.NAME AS SortName, a.No, A.Name,";
                sql += "A.PTable,A.OrgNo ,b.idx as idx1,a.idx as idx2 ";
                sql += " FROM ";
                sql += "Sys_MapData A, Sys_FormTree B, WF_FrmOrg C ";
                sql += " WHERE ";
                sql += " A.FK_FormTree = B.No ";
                sql += " AND B.OrgNo = C.OrgNo ";
                sql += " AND B.OrgNo = '" + WebUser.OrgNo + "' ) t2 ";
                sql += "ORDER BY idx1,idx2";
                
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
