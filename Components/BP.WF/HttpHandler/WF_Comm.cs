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
    public class WF_Comm : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm(HttpContext mycontext)
        {
            this.context = mycontext;
        }

         

        #region xxx 界面 .
        #endregion xxx 界面方法.

        public string SFTable()
        {
            SFTable sftable = new SFTable(this.GetRequestVal("SFTable") );
            DataTable dt = sftable.GenerData();
            return BP.Tools.Json.ToJson(dt);
        }
        public string EnsData()
        {
            Entities ens = ClassFactory.GetEns(this.EnsName);
            ens.RetrieveAll();
            return ens.ToJson();
        }

        /// <summary>
        /// 执行一个SQL，然后返回一个列表.
        /// </summary>
        /// <returns></returns>
        public string SQLList()
        {
            string sqlKey = context.Request.QueryString["SQLKey"]; //SQL的key.
            string paras = context.Request.QueryString["Paras"]; //参数. 格式为 @para1=paraVal@para2=val2

            BP.Sys.XML.SQLList sqlXml = new BP.Sys.XML.SQLList(sqlKey);

            //获得SQL
            string sql = sqlXml.SQL;
            string[] strs = paras.Split('@');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                //参数.
                string[] p = str.Split('=');
                sql = sql.Replace("@" + p[0], p[1]);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }
        public string EnumList()
        {
            SysEnums ses = new SysEnums(this.EnumKey);
            return ses.ToJson();
        }

    }
}
