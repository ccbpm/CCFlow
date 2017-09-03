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
    public class WF_Comm_Sys : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm_Sys(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.


        #region 数据源管理
        public string SFDBSrcNewGuide_GetList()
        {
            //SysEnums enums = new SysEnums(SFDBSrcAttr.DBSrcType);
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();

            return srcs.ToJson();
        }

        public string SFDBSrcNewGuide_LoadSrc()
        {
            DataSet ds = new DataSet();

            SFDBSrc src = new SFDBSrc();
            if (!string.IsNullOrWhiteSpace(this.GetRequestVal("No")))
                src = new SFDBSrc(No);
            ds.Tables.Add(src.ToDataTableField("SFDBSrc"));

            SysEnums enums = new SysEnums();
            enums.Retrieve(SysEnumAttr.EnumKey, SFDBSrcAttr.DBSrcType, SysEnumAttr.IntKey);
            ds.Tables.Add(enums.ToDataTableField("DBSrcType"));

            return BP.Tools.Json.ToJson(ds);
        }

        public string SFDBSrcNewGuide_SaveSrc()
        {
            SFDBSrc src = new SFDBSrc();
            src.No = this.GetRequestVal("TB_No");
            if (src.RetrieveFromDBSources() > 0 && this.GetRequestVal("NewOrEdit") == "New")
            {
                return ("已经存在数据源编号为“" + src.No + "”的数据源，编号不能重复！");
            }
            src.Name = this.GetRequestVal("TB_Name");
            src.DBSrcType = (DBSrcType)this.GetRequestValInt("DDL_DBSrcType");
            switch (src.DBSrcType)
            {
                case DBSrcType.SQLServer:
                case DBSrcType.Oracle:
                case DBSrcType.MySQL:
                case DBSrcType.Informix:
                    if (src.DBSrcType != DBSrcType.Oracle)
                        src.DBName = this.GetRequestVal("TB_DBName");
                    else
                        src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = this.GetRequestVal("TB_UserID");
                    src.Password = this.GetRequestVal("TB_Password");
                    break;
                case DBSrcType.WebServices:
                    src.DBName = string.Empty;
                    src.IP = this.GetRequestVal("TB_IP");
                    src.UserID = string.Empty;
                    src.Password = string.Empty;
                    break;
                default:
                    break;
            }
            //测试是否连接成功，如果连接不成功，则不允许保存。
            string testResult = src.DoConn();

            if (testResult.IndexOf("连接配置成功") == -1)
            {
                return (testResult + ".保存失败！");
            }

            src.Save();

            return "保存成功..";
        }

        public string SFDBSrcNewGuide_DelSrc()
        {
            string no = this.GetRequestVal("No");

            //检验要删除的数据源是否有引用
            SFTables sfs = new SFTables();
            sfs.Retrieve(SFTableAttr.FK_SFDBSrc, no);

            if (sfs.Count > 0)
            {
                //Alert("当前数据源已经使用，不能删除！");
                return "当前数据源已经使用，不能删除！";
                //return;
            }
            SFDBSrc src = new SFDBSrc(no);
            src.Delete();
            return "删除成功..";
        }
        #endregion


    }
}
