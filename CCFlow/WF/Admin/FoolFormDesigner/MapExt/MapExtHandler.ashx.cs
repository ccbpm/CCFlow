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
        public string InitPopValSetting()
        {
            string fk_mapExt = context.Request.QueryString["MyPK"].ToString();

            MapExt ext = new MapExt();
            ext.MyPK = fk_mapExt;
            int i = ext.RetrieveFromDBSources();
            if (i == 0)
            {
                ext.FK_DBSrc = "local";
                ext.PopValSelectModel = PopValSelectModel.One;
                ext.PopValWorkModel = PopValWorkModel.TableOnlyModel;
            }

            //创建一个ht, 然后把他转化成json返回出去。
            Hashtable ht = new Hashtable();

            

            switch (ext.PopValWorkModel)
            {
                case  PopValWorkModel.SelfUrl:
                    ht.Add("URL", ext.Doc);
                    break;
                case PopValWorkModel.TableOnlyModel:
                    ht.Add("EntitySQL", ext.Tag2);
                    break;
                case PopValWorkModel.TablePageModel:
                    ht.Add("TableIntSQL", ext.Tag1);
                    ht.Add("EntitySQL", ext.Tag2);
                    break;
                case PopValWorkModel.GroupModel:
                    ht.Add("GroupSQL", ext.Tag1);
                    ht.Add("EntitySQL", ext.Tag2);
                    break;
                case PopValWorkModel.TreeModel:
                    ht.Add("EntitySQL", ext.Tag2);
                    break;
                default:
                    break;
            }

            ht.Add(MapExtAttr.W, ext.W);
            ht.Add(MapExtAttr.H, ext.H);

            ht.Add("PopValWorkModel", ext.PopValWorkModel);
            ht.Add("PopValSelectModel", ext.PopValSelectModel);
            ht.Add("PopValFormat", ext.PopValFormat);
            ht.Add("PopValTitle", ext.PopValTitle);

            //转化为Json.
            return BP.Tools.Json.ToJson(ht); 
        }

        public HttpContext context = null;

        public void ProcessRequest(HttpContext mycontext)
        {
            context = mycontext;
            string doType = context.Request.QueryString["DoType"];
            switch (doType)
            {
                case "InitPopValSetting":
                    context.Response.Write(this.InitPopValSetting());
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
