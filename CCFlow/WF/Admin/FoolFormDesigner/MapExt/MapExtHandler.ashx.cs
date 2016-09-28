using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using System.Web.SessionState;
using BP.DA;
using BP.Web;
using BP.WF;
using BP.Sys;
using BP.En;

namespace CCFlow.WF.Admin.FoolFormDesigner
{
    /// <summary>
    /// MapExt 的摘要说明
    /// </summary>
    public class MapExtHandler : IHttpHandler
    {
        string no;
        string name;
        string fk_dept;
        string oid;
        string kvs;
        public string DealSQL(string sql, string key)
        {
            sql = sql.Replace("@Key", key);
            sql = sql.Replace("@key", key);
            sql = sql.Replace("@Val", key);
            sql = sql.Replace("@val", key);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            if (oid != null)
                sql = sql.Replace("@OID", oid);

            if (string.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (string.IsNullOrEmpty(s)
                        || s.Contains("=") == false)
                        continue;
                    string[] mykv = s.Split('=');
                    sql = sql.Replace("@" + mykv[0], mykv[1]);

                    if (sql.Contains("@") == false)
                        break;
                }
            }
            return sql;
        }
        public string PopVal_Init()
        {
            string fk_mapExt = context.Request.QueryString["MyPK"].ToString();

            MapExt ext = new MapExt();
            ext.MyPK = fk_mapExt;
            int i = ext.RetrieveFromDBSources();

            if (i == 0)
            {
                ext.FK_DBSrc = "local";
                ext.PopValSelectModel = PopValSelectModel.One;
                ext.PopValWorkModel = PopValWorkModel.TableOnly;
            }

            return ext.PopValToJson();

            //return ext.PopValToJson();
        }
        /// <summary>
        /// 保存设置.
        /// </summary>
        /// <returns></returns>
        public string PopVal_Save()
        {
            try
            {
                MapExt me = new MapExt();
                me.MyPK = context.Request.QueryString["FK_MapExt"];
                me.FK_MapData = context.Request.QueryString["FK_MapData"];
                me.AttrOfOper = context.Request.QueryString["KeyOfEn"];
                me.RetrieveFromDBSources();

                string valWorkModel = this.GetValFromFrmByKey("Model");
                
                switch (valWorkModel)
                {
                    case "SelfUrl": //URL模式.
                        me.PopValWorkModel = PopValWorkModel.SelfUrl;
                        me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "TableOnly": //表格模式.
                        me.PopValWorkModel = PopValWorkModel.TableOnly;

                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_Table_SQL");

                        break;
                    case "TablePage": //分页模式.
                        me.PopValWorkModel = PopValWorkModel.TablePage;
                        
                        me.PopValTablePageSQL = this.GetValFromFrmByKey("TB_TablePage_SQL");
                        me.PopValTablePageSQLCount = this.GetValFromFrmByKey("TB_TablePage_SQLCount");
                        break;
                    case "Group": //分组模式.
                        me.PopValWorkModel = PopValWorkModel.Group;                        
                        me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "Tree": //树模式.
                        me.PopValWorkModel = PopValWorkModel.Tree;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_TreeSQL");
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_TreeParentNo");
                        break;
                    default:
                        break;
                }

                //高级属性.
                me.W = int.Parse(this.GetValFromFrmByKey("TB_Width"));
                me.H = int.Parse(this.GetValFromFrmByKey("TB_Height"));
                me.PopValColNames = this.GetValFromFrmByKey("TB_ColNames"); //中文列名的对应.
                me.PopValTitle = this.GetValFromFrmByKey("TB_Title"); //标题.
                me.PopValSearchTip = this.GetValFromFrmByKey("TB_PopValSearchTip"); //提示.


                //数据返回格式.
                string popValFormat = this.GetValFromFrmByKey("PopValFormat");
                switch (popValFormat)
                {
                    case "OnlyNo":
                        me.PopValFormat = PopValFormat.OnlyNo;
                        break;
                    case "OnlyName":
                        me.PopValFormat = PopValFormat.OnlyName;
                        break;
                    case "NoName":
                        me.PopValFormat = PopValFormat.NoName;
                        break;
                    default:
                        break;
                }

                //选择模式.
                string seleModel = this.GetValFromFrmByKey("PopValSelectModel");
                if (seleModel == "One")
                    me.PopValSelectModel = PopValSelectModel.One;
                else
                    me.PopValSelectModel = PopValSelectModel.More;

                me.Save();
                return "保存成功.";
            }
            catch(Exception ex)
            {
                return "@保存失败:" + ex.Message;
            }
        }
        /// <summary>
        /// 获得Form数据.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>返回值</returns>
        public string GetValFromFrmByKey(string key)
        {
            string val= context.Request.Form[key];
            val = val.Replace("'", "~");
            return val;
        }

        public HttpContext context = null;

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string doType = context.Request.QueryString["DoType"];
            switch (doType)
            {
                case "PopVal_Init":
                    context.Response.Write(this.PopVal_Init());
                    return;
                case "PopVal_Save":
                    context.Response.Write(this.PopVal_Save());
                    return;
                default:
                    break;
            }
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
