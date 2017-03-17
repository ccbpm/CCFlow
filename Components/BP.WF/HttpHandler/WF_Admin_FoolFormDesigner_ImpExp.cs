using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_FoolFormDesigner_ImpExp : BP.WF.HttpHandler.WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_FoolFormDesigner_ImpExp(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 导入
        /// <summary>
        /// 初始化 导入的界面 .
        /// </summary>
        /// <returns></returns>
        public string Imp_Init()
        {
            DataSet ds = new DataSet();

            string sql = "";
            System.Data.DataTable dt;

            if (this.FK_Flow != null)
            {
                //加入节点表单. 如果没有流程参数.
                sql = "SELECT NODEID, NAME  FROM WF_Node WHERE FK_Flow='" + this.FK_Flow + "' ORDER BY NODEID ";
                dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                dt.TableName = "WF_Node";
                ds.Tables.Add(dt);
            }

            //加入表单库目录.
            sql = "SELECT 'ND'+NO,NAME,PARENTNO FROM Sys_FormTree ORDER BY  PARENTNO IDX ";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_FormTree";
            ds.Tables.Add(dt);

            //加入表单
            sql = "SELECT A.No, A.Name, A.FK_FormTree  FROM Sys_MapData A, Sys_FormTree B WHERE A.FK_FormTree=B.No";
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Sys_MapData";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion

    }
}
