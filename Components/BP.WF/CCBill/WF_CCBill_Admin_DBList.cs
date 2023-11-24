using System;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.WF.HttpHandler;
using BP.Difference;

namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin_DBList : DirectoryPageBase
    {
        #region 属性.
        /// <summary>
        /// 模块编号
        /// </summary>
        public string ModuleNo
        {
            get
            {
                string str = this.GetRequestVal("ModuleNo");
                return str;
            }
        }
        /// <summary>
        /// 菜单ID.
        /// </summary>
        public string MenuNo
        {
            get
            {
                string str = this.GetRequestVal("MenuNo");
                return str;
            }
        }

        public string GroupID
        {
            get
            {
                string str = this.GetRequestVal("GroupID");
                return str;
            }
        }
      
        #endregion 属性.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin_DBList()
        {

        }
    
        /// <summary>
        /// 映射
        /// </summary>
        /// <returns></returns>
        public string FieldsORM_Init()
        {
            try
            {
                DBList db = new DBList(this.FrmID);

                string sql = db.ExpEn;
                if (DataType.IsNullOrEmpty(sql) == true)
                    return "";

                sql = sql.Replace("~~", "\"");
                sql = sql.Replace("~", "'");
               
                DataTable mydt = new DataTable();
                mydt.Columns.Add("No");
                mydt.Columns.Add("DBType");
                //SQL查询
                if (db.DBType == 0)
                {
                    sql = sql.Replace("@Key", "1234");
                    DataTable dt = null;
                    try
                    {
                        //本机数据库查询
                        if (db.DBSrc == "local" || DataType.IsNullOrEmpty(db.DBSrc) == true)
                            dt = DBAccess.RunSQLReturnTable(sql);
                        else
                        {
                            SFDBSrc dbsrc = new SFDBSrc(db.DBSrc);
                            dt = dbsrc.RunSQLReturnTable(sql);
                        }
                    }
                    catch(Exception ex) 
                    {
                        return ex.Message;
                    }

                    foreach (DataColumn dc in dt.Columns)
                    {
                        DataRow dr = mydt.NewRow();
                        dr[0] = dc.ColumnName;
                        dr[1] = dc.DataType.Name;
                        mydt.Rows.Add(dr);
                    }
                    return BP.Tools.Json.ToJson(mydt);
                }


                //根据URL获取数据源
                if (db.DBType == 1)
                {
                    string url = sql;
                    url = url.Replace("@Key", "");
                    if (url.Contains("http") == false)
                    {
                        /*如果没有绝对路径 */
                        if (BP.Difference.SystemConfig.isBSsystem)
                        {
                            /*在cs模式下自动获取*/
                            string host = HttpContextHelper.RequestUrlHost;//BP.Sys.Base.Glo.Request.Url.Host;
                            if (url.Contains("@AppPath"))
                                url = url.Replace("@AppPath", "http://" + host + HttpContextHelper.RequestApplicationPath);//BP.Sys.Base.Glo.Request.ApplicationPath
                            else
                                url = "http://" + HttpContextHelper.RequestUrlAuthority + url;
                        }

                        if (BP.Difference.SystemConfig.isBSsystem == false)
                        {
                            /*在cs模式下它的baseurl 从web.config中获取.*/
                            string cfgBaseUrl =  BP.Difference.SystemConfig.AppSettings["HostURL"];
                            if (DataType.IsNullOrEmpty(cfgBaseUrl))
                            {
                                string err = "调用url失败:没有在web.config中配置BaseUrl,导致url事件不能被执行.";
                                BP.DA.Log.DebugWriteError(err);
                                throw new Exception(err);
                            }
                            url = cfgBaseUrl + url;
                        }
                    }
                    System.Text.Encoding encode = System.Text.Encoding.GetEncoding("UTF-8");
                    string json = DataType.ReadURLContext(url, 8000, encode);
                    if (DataType.IsNullOrEmpty(json) == true)
                        return "err@执行URL没有返回结果值";

                    DataTable dt = BP.Tools.Json.ToDataTable(json);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        DataRow dr = mydt.NewRow();
                        dr[0] = dc.ColumnName;
                        dr[1] = dc.DataType.Name;
                        mydt.Rows.Add(dr);
                    }
                    json = BP.Tools.Json.ToJson(mydt);

                    return json;
                }
                //执行存储过程
                if(db.DBType == 2)
                {
                    if (sql.Trim().ToUpper().StartsWith("SELECT") == false)
                    {
                        switch (BP.Difference.SystemConfig.AppCenterDBType)
                        {
                            case DBType.MSSQL:
                                sql = "EXEC " + sql + " null";
                                break;
                            case DBType.MySQL:
                                sql = "CALL " + sql + "(null)";
                                break;
                            default:
                                throw new Exception("err@其他版本的数据还未解析该功能");
                        }
                    }
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataColumn dc in dt.Columns)
                    {
                        DataRow dr = mydt.NewRow();
                        dr[0] = dc.ColumnName;
                        dr[1] = dc.DataType.Name;
                        mydt.Rows.Add(dr);
                    }
                    return BP.Tools.Json.ToJson(mydt);

                }
                return "err@没有增加的判断" + db.DBType;
            }
            catch (Exception ex)
            {
                return "err@"+ex.Message;
            }
        }
        public string FieldsORM_SaveKeysName()
        {
            MapAttrs mapAttrs = new MapAttrs(this.FrmID + "Bak");
            //改变类型先保存字段集合的名称
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (key == null) 
                    continue;
                if (key.IndexOf("TB_") == -1 || key.Equals("TB_Doc") == true)
                    continue;
                string myKey = key;
                string val = HttpContextHelper.RequestParams(key);
                myKey = myKey.Replace("TB_", "");

                val = HttpUtility.UrlDecode(val, Encoding.UTF8);

                MapAttr attr = mapAttrs.GetEntityByKey(this.FrmID + "Bak_" + myKey) as MapAttr;
                if (attr != null)
                {
                    bool uiVisible = this.GetRequestValBoolen("CB_" + attr.KeyOfEn);
                    attr.setUIVisible(uiVisible);
                    attr.Name = val;
                    attr.DirectUpdate();
                }


            }
            return "保存成功";
        }
        /// <summary>
        /// 执行应用
        /// </summary>
        /// <returns></returns>
        public string FieldsORM_App()
        {

            // 删除当前的字段.
            //系统字段
            string systemKeys = "BillState,RDT,Starter,StarterName,OrgNo,AtPara";
            string sql = "DELETE FROM Sys_MapAttr Where FK_MapData='" + this.FrmID + "' AND KeyOfEn NOT IN('" + systemKeys.Replace(",", "','") + "')";
            DBAccess.RunSQL(sql);
            // 按照顺序从 bak里copy过来.
            systemKeys = systemKeys + ",";
            //保存当前字段
            MapAttrs mapAttrs = new MapAttrs(this.FrmID + "Bak");
            //改变类型先保存字段集合的名称
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (key == null)
                    continue;
                if (key.IndexOf("TB_") == -1 || key.Equals("TB_DBSrc") == true)
                    continue;
                string myKey = key;
                string val = HttpContextHelper.RequestParams(key);
                myKey = myKey.Replace("TB_", "");

                val = HttpUtility.UrlDecode(val, Encoding.UTF8);

                MapAttr attr = mapAttrs.GetEntityByKey(this.FrmID + "Bak_" + myKey) as MapAttr;
                if (attr != null)
                {
                    bool uiVisible = this.GetRequestValBoolen("CB_" + attr.KeyOfEn);
                    attr.Name = val;
                    attr.UIVisible = uiVisible;
                    attr.DirectUpdate();
                    if (systemKeys.IndexOf(attr.KeyOfEn + ",") == -1)
                    {
                        attr.FrmID =this.FrmID;
                        attr.setMyPK(this.FrmID + "_" + attr.KeyOfEn);
                        attr.GroupID = 1;
                        attr.Insert();
                    }

                }


            }
            MapData mapData = new MapData(this.FrmID);
            mapData.ClearCache();
            return "执行成功";
        }

    }
}
