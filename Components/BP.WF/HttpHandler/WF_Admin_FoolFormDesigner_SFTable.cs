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
    public class WF_Admin_FoolFormDesigner_SFTable : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_FoolFormDesigner_SFTable(HttpContext mycontext)
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

        #region xxx 界面 .
        /// <summary>
        ///  初始化sf0.
        /// </summary>
        /// <returns></returns>
        public string SF0_Init()
        {
            return "";
        }
        public string SF0_Save()
        {
            return "保存成功.";
        }
        #endregion xxx 界面方法.


        #region 本地表 .
        /// <summary>
        ///  初始化sf0.
        /// </summary>
        /// <returns></returns>
        public string SF1_Init()
        {
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            return srcs.ToJson();
        }
        public string SF1_Save()
        {
            SFTable sf = new SFTable();
            sf.No = this.GetValFromFrmByKey("No");
            if (sf.IsExits == true)
                return "err@标记:"+sf.No+"已经存在.";

            sf.Name = this.GetRequestVal("Name");
            sf.FK_SFDBSrc = this.GetValFromFrmByKey("FK_DBSrc");

            sf.SrcType = SrcType.CreateTable;
            sf.CodeStruct = (CodeStruct)this.GetValIntFromFrmByKey("CodeStruct");
            sf.Insert();
            return "保存成功.";
        }
        #endregion xxx 界面方法.


        #region 表或者视图 .
        /// <summary>
        ///  初始化sf2.
        /// </summary>
        /// <returns></returns>
        public string SF2_Init()
        {
            SFDBSrcs srcs = new SFDBSrcs();
            srcs.RetrieveAll();
            return srcs.ToJson();
        }

        public string SF2_GetTVs()
        {
            string src = context.Request.QueryString["src"];

            SFDBSrc sr = new SFDBSrc(src);
            DataTable dt = sr.GetTables();

            return BP.Tools.Json.ToJson(dt);
        }

        public string SF2_GetCols()
        {
            string src = this.GetRequestVal("src");
            string table = context.Request.QueryString["table"];

            if (string.IsNullOrWhiteSpace(src))
                throw new Exception("err@参数不正确");


            if (string.IsNullOrWhiteSpace(table))
            {
                return "[]";
            }

            SFDBSrc sr = new SFDBSrc(src);
            DataTable dt = sr.GetColumns(table);

            foreach (DataRow r in dt.Rows)
            {
                r["Name"] = r["No"] + (r["Name"] == null || r["Name"] == DBNull.Value || string.IsNullOrWhiteSpace(r["Name"].ToString()) ? "" : string.Format("[{0}]", r["Name"]));
            }

            return BP.Tools.Json.ToJson(dt);
        }

        public string SF2_Save()
        {
            SFTable sf = new SFTable();
            sf.No = this.GetValFromFrmByKey("No");
            if (sf.IsExits)
                return "err@标记:" + sf.No + "已经存在.";

            sf.Name = this.GetValFromFrmByKey("Name");
            sf.FK_SFDBSrc = this.GetValFromFrmByKey("FK_DBSrc");
            sf.SrcTable = this.GetValFromFrmByKey("SrcTable");
            sf.CodeStruct = (CodeStruct) this.GetValIntFromFrmByKey("CodeStruct");
            sf.ColumnValue = this.GetValFromFrmByKey("ColumnValue");
            sf.ColumnText = this.GetValFromFrmByKey("ColumnText");
            if (sf.CodeStruct == CodeStruct.Tree)
            {
                sf.ParentValue = this.GetValFromFrmByKey("ParentValue");
                sf.DefVal = this.GetValFromFrmByKey("RootValue");
            }
            sf.SelectStatement = this.GetValFromFrmByKey("Selectstatement");
            sf.SrcType = SrcType.TableOrView;
            sf.FK_Val = "FK_" + sf.No;
            sf.Save();

            return "保存成功！";
        }

        #endregion xxx 界面方法.
    }
}
