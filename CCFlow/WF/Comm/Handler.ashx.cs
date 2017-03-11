using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using BP.En;
using BP.DA;
using BP.Sys;

namespace CCFlow.WF.Comm
{
    /// <summary>
    /// Handler 的摘要说明
    /// 1, 公共处理程序.
    /// 2, 解决bp框架的通用的问题.
    /// </summary>
    public class Handler : IHttpHandler
    {
        #region 执行.
        public HttpContext context = null;
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                string str = context.Request.QueryString["DoType"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = context.Request.QueryString["MyPK"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                string str = context.Request.QueryString["EnumKey"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 实体 EnsName
        /// </summary>
        public string EnsName
        {
            get
            {
                string str = context.Request.QueryString["EnsName"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string SFTable
        {
            get
            {
                string str = context.Request.QueryString["SFTable"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 表单外键
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = context.Request.QueryString["FK_MapData"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        #endregion 执行.

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "SQLList": //获得枚举列表的JSON.
                        msg = this.SQLList(); 
                        break;
                    case "EnumList": //获得枚举列表的JSON.
                        SysEnums ses = new SysEnums(this.EnumKey);
                        msg= ses.ToJson();
                        break;
                    case "EnsData": //获得枚举列表的JSON.
                        Entities ens = ClassFactory.GetEns(this.EnsName);
                        ens.RetrieveAll();
                        msg = ens.ToJson();
                        break;
                    case "SFTable": //获得枚举列表的JSON.
                        SFTable sftable = new SFTable(this.SFTable);
                        DataTable dt= sftable.GenerData();
                        msg= BP.Tools.Json.ToJson(dt);
                        break;
                    default:
                        msg = "err@没有判断的标记:" + this.DoType;
                        break;
                }
            }
            catch (Exception ex)
            {
                msg = "err@" + ex.Message;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(msg);
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
                string[] p=str.Split('=');
                sql = sql.Replace("@" + p[0], p[1]);
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.Tools.Json.ToJson(dt);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}