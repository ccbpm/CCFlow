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
        #region 属性.
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                string str = context.Request.QueryString["KeyOfEn"].ToString();
                if (str == null || str == "")
                    str = context.Request.QueryString["KeyOfEn"].ToString();
                return str;
            }
        }
        /// <summary>
        /// extMap
        /// </summary>
        public string FK_MapExt
        {
            get
            {
                string fk_mapExt = context.Request.QueryString["MyPK"] as string;
                if (fk_mapExt == null || fk_mapExt=="")
                    fk_mapExt = context.Request.QueryString["FK_MapExt"] as string;
                return fk_mapExt;
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = context.Request.QueryString["FK_MapData"] as string;
                if (str == null || str == "")
                    str = context.Request.QueryString["MyPK"] as string;
                return str;
            }
        }
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

        /// <summary>
        /// 返回
        /// </summary>
        /// <returns></returns>
        public string PopVal_Init()
        {
            MapExt ext = new MapExt();
            ext.MyPK = this.FK_MapExt;
            if (ext.RetrieveFromDBSources() == 0)
            {
                ext.FK_DBSrc = "local";
                ext.PopValSelectModel = PopValSelectModel.One;
                ext.PopValWorkModel = PopValWorkModel.TableOnly;
            }
            return ext.PopValToJson();
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
                me.MyPK =this.FK_MapExt;
                me.FK_MapData = this.FK_MapData;
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

                        me.PopValGroupSQL = this.GetValFromFrmByKey("TB_GroupModel_Group");
                         me.PopValEntitySQL = this.GetValFromFrmByKey("TB_GroupModel_Entity");

                        //me.PopValUrl = this.GetValFromFrmByKey("TB_Url");
                        break;
                    case "Tree": //单实体树.
                        me.PopValWorkModel = PopValWorkModel.Tree;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_TreeSQL");
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_TreeParentNo");
                        break;
                    case "TreeDouble": //双实体树.
                        me.PopValWorkModel = PopValWorkModel.TreeDouble;
                        me.PopValTreeSQL = this.GetValFromFrmByKey("TB_DoubleTreeSQL");// 树SQL
                        me.PopValTreeParentNo = this.GetValFromFrmByKey("TB_DoubleTreeParentNo");

                        me.PopValEntitySQL = this.GetValFromFrmByKey("TB_DoubleTreeEntitySQL"); //实体SQL
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
                case "RadioBtns_Init":
                    context.Response.Write(this.RadioBtns_Init());
                    break;
                case "RadioBtns_Save":
                    context.Response.Write(this.RadioBtns_Save());
                    break;
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
        /// <summary>
        /// 返回信息。
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Init()
        {
            DataSet ds = new DataSet();

            //放入表单字段.
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            //属性.
            MapAttr attr = new MapAttr();
            attr.MyPK = this.FK_MapData + "_" + this.KeyOfEn;
            attr.Retrieve();

            //字段值.
            FrmRBs rbs= new FrmRBs();
            rbs.Retrieve(FrmRBAttr.FK_MapData, this.FK_MapData, FrmRBAttr.KeyOfEn, this.KeyOfEn);
            if (rbs.Count == 0)
            {
                /*初始枚举值变化.
                 */
                SysEnums ses = new SysEnums(attr.UIBindKey);
                foreach (SysEnum se in ses)
                {
                    FrmRB rb = new FrmRB();
                    rb.FK_MapData = this.FK_MapData;
                    rb.KeyOfEn = this.KeyOfEn;
                    rb.IntKey = se.IntKey;
                    rb.Lab = se.Lab;
                    rb.EnumKey = attr.UIBindKey;
                    rb.Insert(); //插入数据.
                }

                rbs.Retrieve(FrmRBAttr.FK_MapData, this.FK_MapData, FrmRBAttr.KeyOfEn, this.KeyOfEn);
            }

            //加入单选按钮.
            ds.Tables.Add(attrs.ToDataTableField("Sys_FrmRB"));
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns></returns>
        public string RadioBtns_Save()
        {
            string json = context.Request.Form["data"];
            DataTable dt = BP.Tools.Json.ToDataTable(json);

            foreach (DataRow dr in dt.Rows)
            {
                FrmRB rb = new FrmRB();
                rb.MyPK = dr["MyPK"].ToString();
                rb.Retrieve();

                rb.Script = dr["Script"].ToString();
                rb.FieldsCfg = dr["FieldsCfg"].ToString(); //格式为 @字段名1=1@字段名2=0
                rb.Tip = dr["Tip"].ToString(); //提示信息
                rb.Update();
            }

            return "保存成功.";
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
