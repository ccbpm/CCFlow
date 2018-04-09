<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web;
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

namespace CCFlow.WF.MapDef
{
    public class HanderMapExt : IHttpHandler
    {
        #region 属性.
        string no;
        string name;
        string fk_dept;
        string oid;
        string kvs;
        #endregion 属性.

        public string DealSQL(string sql, string key)
        {
            sql = sql.Replace("@Key", key);
            sql = sql.Replace("@key", key);
            sql = sql.Replace("@Val", key);
            sql = sql.Replace("@val", key);

            //sql = sql.Replace("@WebUser.No", no);
            //sql = sql.Replace("@WebUser.Name", name);
            //sql = sql.Replace("@WebUser.FK_Dept", fk_dept);

            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            if (oid != null)
                sql = sql.Replace("@OID", oid);

            if (DataType.IsNullOrEmpty(kvs) == false && sql.Contains("@") == true)
            {
                string[] strs = kvs.Split('~');
                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s)
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
            string fk_mapExt = context.Request.QueryString["FK_MapExt"].ToString();

            MapExt ext = new MapExt();
            ext.MyPK = fk_mapExt;
            ext.RetrieveFromDBSources();
            
            DataTable dt=ext.ToDataTableField("Table");
            return BP.Tools.Json.ToJson(dt);
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
            
            string fk_mapExt = context.Request.QueryString["FK_MapExt"].ToString();
            if (context.Request.QueryString["Key"] == null)
                return;
            
            no = context.Request.QueryString["WebUserNo"];
            name = context.Request.QueryString["WebUserName"];
            fk_dept = context.Request.QueryString["WebUserFK_Dept"];
            oid = context.Request.QueryString["OID"];
            kvs = context.Request.QueryString["KVs"];

            BP.Sys.MapExt me = new BP.Sys.MapExt(fk_mapExt);
            DataTable dt = null;
            string sql = "";
            string key = context.Request.QueryString["Key"];
            key = System.Web.HttpUtility.UrlDecode(key,
                System.Text.Encoding.GetEncoding("GB2312"));
            key = key.Trim();
            // key = "周";
            switch (me.ExtType)
            {
                case BP.Sys.MapExtXmlList.DDLFullCtrl: // 级连ddl.
                    sql = this.DealSQL(me.DocOfSQLDeal, key);
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    context.Response.Write(JSONTODT(dt));
                    return;
                case BP.Sys.MapExtXmlList.ActiveDDL: // 动态填充ddl。
                    sql = this.DealSQL(me.DocOfSQLDeal, key);
                    dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    context.Response.Write(JSONTODT(dt));
                    return;
                case BP.Sys.MapExtXmlList.TBFullCtrl: // 自动完成。
                    switch (context.Request.QueryString["DoType"])
                    {
                        case "ReqCtrl":
                            // 获取填充 ctrl 值的信息.
                            sql = this.DealSQL(me.DocOfSQLDeal, key);
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            context.Response.Write(JSONTODT(dt));
                            break;
                        case "ReqM2MFullList":
                            /* 获取填充的M2m集合. */
                            DataTable dtM2M = new DataTable("Head");
                            dtM2M.Columns.Add("Dtl", typeof(string));
                            string[] strsM2M = me.Tag2.Split('$');
                            foreach (string str in strsM2M)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                string noOfObj = ss[0];
                                string mysql = ss[1];
                                mysql = DealSQL(mysql, key);

                                DataTable dtFull = DBAccess.RunSQLReturnTable(mysql);
                                M2M m2mData = new M2M();
                                m2mData.FK_MapData = me.FK_MapData;
                                m2mData.EnOID = int.Parse(oid);
                                m2mData.M2MNo = noOfObj;
                                string mystr = ",";
                                string mystrT = "";
                                foreach (DataRow dr in dtFull.Rows)
                                {
                                    string myno = dr["No"].ToString();
                                    string myname = dr["Name"].ToString();
                                    mystr += myno + ",";
                                    mystrT += "@" + myno + "," + myname;
                                }
                                m2mData.Vals = mystr;
                                m2mData.ValsName = mystrT;
                                m2mData.InitMyPK();
                                m2mData.NumSelected = dtFull.Rows.Count;
                                m2mData.Save();

                                DataRow mydr = dtM2M.NewRow();
                                mydr[0] = ss[0];
                                dtM2M.Rows.Add(mydr);
                            }
                            context.Response.Write(JSONTODT(dtM2M));
                            break;
                        case "ReqDtlFullList":
                            /* 获取填充的从表集合. */
                            DataTable dtDtl = new DataTable("Head");
                            dtDtl.Columns.Add("Dtl", typeof(string));
                            string[] strsDtl = me.Tag1.Split('$');
                            foreach (string str in strsDtl)
                            {
                                if (DataType.IsNullOrEmpty(str))
                                    continue;

                                string[] ss = str.Split(':');
                                string fk_dtl = ss[0];
                                string mysql = DealSQL(ss[1], key);

                                GEDtls dtls = new GEDtls(fk_dtl);
                                MapDtl dtl = new MapDtl(fk_dtl);

                                DataTable dtDtlFull = DBAccess.RunSQLReturnTable(mysql);
                                BP.DA.DBAccess.RunSQL("DELETE FROM " + dtl.PTable + " WHERE RefPK=" + oid);
                                foreach (DataRow dr in dtDtlFull.Rows)
                                {
                                    BP.Sys.GEDtl mydtl = new GEDtl(fk_dtl);
                                    //  mydtl.OID = dtls.Count + 1;
                                    dtls.AddEntity(mydtl);
                                    foreach (DataColumn dc in dtDtlFull.Columns)
                                    {
                                        mydtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName].ToString());
                                    }
                                    mydtl.RefPKInt = int.Parse(oid);
                                    if (mydtl.OID > 100)
                                    {
                                        mydtl.InsertAsOID(mydtl.OID);
                                    }
                                    else
                                    {
                                        mydtl.OID = 0;
                                        mydtl.Insert();
                                    }

                                }
                                DataRow drRe = dtDtl.NewRow();
                                drRe[0] = fk_dtl;
                                dtDtl.Rows.Add(drRe);
                            }
                            context.Response.Write(JSONTODT(dtDtl));
                            break;
                        case "ReqDDLFullList":
                            /* 获取要个性化填充的下拉框. */
                            DataTable dt1 = new DataTable("Head");
                            dt1.Columns.Add("DDL", typeof(string));
                            //    dt1.Columns.Add("SQL", typeof(string));
                            string[] strs = me.Tag.Split('$');
                            foreach (string str in strs)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                DataRow dr = dt1.NewRow();
                                dr[0] = ss[0];
                                // dr[1] = ss[1];
                                dt1.Rows.Add(dr);
                            }
                            context.Response.Write(JSONTODT(dt1));
                            break;
                        case "ReqDDLFullListDB":
                            /* 获取要个性化填充的下拉框的值. 根据已经传递过来的 ddl id. */
                            string myDDL = context.Request.QueryString["MyDDL"];
                            sql = "";
                            string[] strs1 = me.Tag.Split('$');
                            foreach (string str in strs1)
                            {
                                if (str == "" || str == null)
                                    continue;

                                string[] ss = str.Split(':');
                                if (ss[0] == myDDL)
                                {
                                    sql = ss[1];
                                    sql = this.DealSQL(sql, key);
                                    break;
                                }
                            }
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            context.Response.Write(JSONTODT(dt));
                            break;
                        default:
                            sql = this.DealSQL(me.DocOfSQLDeal, key);
                            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            context.Response.Write(JSONTODT(dt));
                            break;
                    }
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
        public string JSONTODT(DataTable dt)
        {
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower() == "no")
                    dc.ColumnName = "No";
                if (dc.ColumnName.ToLower() == "name")
                    dc.ColumnName = "Name";
            }

            StringBuilder JsonString = new StringBuilder();
            if (dt != null && dt.Rows.Count > 0)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"Head\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            JsonString.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + dt.Rows[i][j].ToString() + "\"");
                        }
                    }
                    /**/
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]}");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }
    }


}