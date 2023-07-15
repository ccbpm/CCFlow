using System;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.IO;
using System.IO.Compression;
using System.Text;
using BP.En;
using BP.Difference;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.Text.RegularExpressions;
using BP.Port;
using System.Collections.Generic;

namespace BP.Pub
{
    /// <summary>
    /// PageBase 的摘要说明。
    /// </summary>
    public class PubClass
    {
        /// <summary>
        /// 处理字段
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        public static string DealToFieldOrTableNames(string fd)
        {
            string ptable = fd;
            string keys = "~!@#$%^&*()+{}|:<>?`=[];,./～！＠＃￥％……＆×（）——＋｛｝｜：“《》？｀－＝［］；＇，．／";
            char[] cc = keys.ToCharArray();
            foreach (char c in cc)
                fd = fd.Replace(c.ToString(), "");
            if (fd.Length <= 0)
                throw new Exception("存储表PTable为" + ptable + ",不合法");

            char c1c = fd.ToCharArray()[0];
            if (char.IsLetter(c1c) == false)
                fd = "F" + fd;

            return fd;
        }
        public static string KeyFields
        {
            get
            {

                return ",release,declare,key,select,from,delete,update,insert,into,go,to,goto,where,order,by,use,user,function,produce,talbe,colmen,rows,";
            }
        }

        /// <summary>
        /// 产生临时文件名称
        /// </summary>
        /// <param name="hz"></param>
        /// <returns></returns>
        public static string GenerTempFileName(string hz)
        {
            return Web.WebUser.No + DateTime.Now.ToString("MMddhhmmss") + "." + hz;
        }
        /// <summary>
        /// 获取datatable.
        /// </summary>
        /// <param name="uiBindKey"></param>
        /// <returns></returns>
        public static DataTable GetDataTableByUIBineKey(string uiBindKey, Hashtable ht = null)
        {

            DataTable dt = new DataTable();
            if (uiBindKey.Contains("."))
            {
                Entities ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);

                if (ens == null)
                    ens = BP.En.ClassFactory.GetEns(uiBindKey);
                if (ens == null)
                    throw new Exception("类名错误:" + uiBindKey + ",不能转化成ens.");

                ens.RetrieveAll();
                dt = ens.ToDataTableField(uiBindKey);
                return dt;
            }

            SFTable sf = new SFTable();
            sf.No = uiBindKey;
            if (sf.RetrieveFromDBSources() != 0)
            {
                if (sf.SrcType == DictSrcType.Handler || sf.SrcType == DictSrcType.JQuery)
                    return null;
                dt = sf.GenerHisDataTable(ht);
            }

            if (dt == null)
                dt = new DataTable();

            #region 把列名做成标准的.
            foreach (DataColumn col in dt.Columns)
            {
                string colName = col.ColumnName.ToLower();
                switch (colName)
                {
                    case "no":
                        col.ColumnName = "No";
                        break;
                    case "name":
                        col.ColumnName = "Name";
                        break;
                    case "parentno":
                        col.ColumnName = "ParentNo";
                        break;
                    default:
                        break;
                }
            }
            #endregion 把列名做成标准的.

            dt.TableName = uiBindKey;
            return dt;
        }


        #region 系统调度
        public static string DBRpt(DBCheckLevel level)
        {
            // 取出全部的实体
            ArrayList als = ClassFactory.GetObjects("BP.En.Entities");
            string msg = "";
            foreach (object obj in als)
            {
                Entities ens = (Entities)obj;
                try
                {
                    msg += DBRpt1(level, ens);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + ens.ToString() + "体检失败:" + ex.Message;
                }
            }

            MapDatas mds = new MapDatas();
            mds.RetrieveAllFromDBSource();
            foreach (MapData md in mds)
            {
                try
                {
                    md.HisGEEn.CheckPhysicsTable();
                    PubClass.AddComment(md.HisGEEn);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + md.No + "体检失败:" + ex.Message;
                }
            }

            MapDtls dtls = new MapDtls();
            dtls.RetrieveAllFromDBSource();
            foreach (MapDtl dtl in dtls)
            {
                try
                {
                    dtl.HisGEDtl.CheckPhysicsTable();
                    PubClass.AddComment(dtl.HisGEDtl);
                }
                catch (Exception ex)
                {
                    msg += "<hr>" + dtl.No + "体检失败:" + ex.Message;
                }
            }

            #region 检查处理必要的基础数据 Pub_Day .
            string sql = "";
            string sqls = "";
            sql = "SELECT count(*) Num FROM Pub_Day";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 1; i <= 31; i++)
                    {
                        string d = i.ToString().PadLeft(2, '0');
                        sqls += "@INSERT INTO Pub_Day(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {
            }

            sql = "SELECT count(*) Num FROM Pub_YF";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 1; i <= 12; i++)
                    {
                        string d = i.ToString().PadLeft(2, '0');
                        sqls += "@INSERT INTO Pub_YF(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {
            }

            sql = "SELECT count(*) Num FROM Pub_ND";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 2010; i < 2015; i++)
                    {
                        string d = i.ToString();
                        sqls += "@INSERT INTO Pub_ND(No,Name)VALUES('" + d.ToString() + "','" + d.ToString() + "')";
                    }
                }
            }
            catch
            {

            }
            sql = "SELECT count(*) Num FROM Pub_NY";
            try
            {
                if (DBAccess.RunSQLReturnValInt(sql) == 0)
                {
                    for (int i = 2010; i < 2015; i++)
                    {

                        for (int yf = 1; yf <= 12; yf++)
                        {
                            string d = i.ToString() + "-" + yf.ToString().PadLeft(2, '0');
                            sqls += "@INSERT INTO Pub_NY(No,Name)VALUES('" + d + "','" + d + "')";
                        }
                    }
                }
            }
            catch
            {
            }

            DBAccess.RunSQLs(sqls);
            #endregion 检查处理必要的基础数据。
            return msg;
        }
        /// <summary>
        /// 为表增加注释
        /// </summary>
        /// <returns></returns>
        public static string AddComment()
        {
            // 取出全部的实体
            ArrayList als = ClassFactory.GetObjects("BP.En.Entities");
            string msg = "";
            Entity en = null;
            Entities ens = null;
            foreach (object obj in als)
            {
                try
                {

                    ens = (Entities)obj;
                    string className = ens.ToString();
                    if (className == null)
                        continue;
                    switch (className.ToUpper())
                    {
                        case "BP.WF.STARTWORKS":
                        case "BP.WF.WORKS":
                        case "BP.WF.GESTARTWORKS":
                        case "BP.EN.GENONAMES":
                        case "BP.EN.GETREES":
                        case "BP.WF.GERptS":
                        case "BP.WF.GEENTITYS":
                        case "BP.WF.GEWORKS":
                        case "BP.SYS.TSENTITYNONAMES":
                            continue;
                        default:
                            break;
                    }
                    en = ens.GetNewEntity;
                    if (en.EnMap.EnType == EnType.View || en.EnMap.EnType == EnType.ThirdPartApp)
                        continue;
                }
                catch
                {
                    continue;
                }

                msg += AddComment(en);
            }
            return msg;
        }
        public static string AddComment(Entity en)
        {

            if (en == null)
                return "实体错误 en=null ";
            if (en.EnMap == null)
                return "实体错误 en.getEnMap=null ";
            if (en.EnMap.PhysicsTable == null)
                return "实体错误 en.getEnMap.getPhysicsTable=null ";
            if (DBAccess.IsExitsObject(en.EnMap.PhysicsTable) == false)
                return "实体表不存在.";

            try
            {
                switch (en.EnMap.EnDBUrl.DBType)
                {
                    case DBType.Oracle:
                        AddCommentForTable_Ora(en);
                        break;
                    case DBType.MySQL:
                        AddCommentForTable_MySql(en);
                        break;
                    default:
                        AddCommentForTable_MS(en);
                        break;
                }
                return "";
            }
            catch (Exception ex)
            {
                return "<hr>" + en.ToString() + "体检失败:" + ex.Message;
            }
        }
        private static void AddCommentForTable_Ora(Entity en)
        {
            en.RunSQL("comment on table " + en.EnMap.PhysicsTable + " IS '" + en.EnDesc + "'");
            SysEnums ses = new SysEnums();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                if (DBAccess.IsExitsTableCol(en.EnMap.PhysicsTable, attr.Field) == false)
                    continue;

                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + "-主键'");
                        break;
                    case FieldType.Normal:
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + "'");
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ",枚举类型:" + ses.ToDesc() + "'");
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ", 主键:枚举类型:" + ses.ToDesc() + "'");
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ", 外键:对应物理表:" + myen.EnMap.PhysicsTable + ",表描述:" + myen.EnDesc + "'");
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL("comment on column  " + en.EnMap.PhysicsTable + "." + attr.Field + " IS '" + attr.Desc + ", 主外键:对应物理表:" + myen1.EnMap.PhysicsTable + ",表描述:" + myen1.EnDesc + "'");
                        break;
                    default:
                        break;
                }
            }
        }
        private static void AddCommentForTable_MySql(Entity en)
        {
            MySql.Data.MySqlClient.MySqlConnection conn =
                new MySql.Data.MySqlClient.MySqlConnection(BP.Difference.SystemConfig.AppCenterDSN);
            en.RunSQL("alter table " + conn.Database + "." + en.EnMap.PhysicsTable + " comment = '" + en.EnDesc + "'");


            //获取当前实体对应表的所有字段结构信息
            DataTable cols =
                DBAccess.RunSQLReturnTable(
                    "select column_name,column_default,is_nullable,character_set_name,column_type from information_schema.columns where table_schema = '" +
                    conn.Database + "' and table_name='" + en.EnMap.PhysicsTable + "'");
            SysEnums ses = new SysEnums();
            string sql = string.Empty;
            DataRow row = null;

            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                row = cols.Select(string.Format("column_name='{0}'", attr.Field))[0];
                sql = string.Format("ALTER TABLE {0}.{1} CHANGE COLUMN {2} {2} {3}{4}{5}{6} COMMENT ",
                                    conn.Database,
                                    en.EnMap.PhysicsTable,
                                    attr.Field,
                                    row["column_type"].ToString().ToUpper(),
                                    Equals(row["character_set_name"], "utf8") ? " CHARACTER SET 'utf8'" : "",
                                    Equals(row["is_nullable"], "YES") ? " NULL" : " NOT NULL",
                                    Equals(row["column_default"], "NULL")
                                        ? " DEFAULT NULL"
                                        : (Equals(row[""], "") ? "" : " DEFAULT " + row[""]));

                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        en.RunSQL(sql + string.Format("'{0} - 主键'", attr.Desc));
                        break;
                    case FieldType.Normal:
                        en.RunSQL(sql + string.Format("'{0}'", attr.Desc));
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL(sql + string.Format("'{0},枚举类型:{1}'", attr.Desc, ses.ToDesc()));
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        en.RunSQL(sql + string.Format("'{0},主键:枚举类型:{1}'", attr.Desc, ses.ToDesc()));
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL(sql +
                                  string.Format("'{0},外键:对应物理表:{1},表描述:{2}'", attr.Desc, myen.EnMap.PhysicsTable,
                                                myen.EnDesc));
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        en.RunSQL(sql +
                                  string.Format("'{0},主外键:对应物理表:{1},表描述:{2}'", attr.Desc, myen1.EnMap.PhysicsTable,
                                                myen1.EnDesc));
                        break;
                    default:
                        break;
                }
            }
        }
        private static void AddColNote(Entity en, string table, string col, string note)
        {
            try
            {
                string sql = "execute  sp_addextendedproperty 'MS_Description', '" + note + "', 'user', dbo, 'table', '" + table + "', 'column', '" + col + "'";
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {
                string sql = "execute  sp_updateextendedproperty 'MS_Description', '" + note + "', 'user', dbo, 'table', '" + table + "', 'column', '" + col + "'";
                en.RunSQL(sql);
            }

        }
        /// <summary>
        /// 为表增加解释
        /// </summary>
        /// <param name="en"></param>
        public static void AddCommentForTable_MS(Entity en)
        {

            if (en.EnMap.EnType == EnType.View || en.EnMap.EnType == EnType.ThirdPartApp)
                return;

            try
            {
                string sql = "execute  sp_addextendedproperty 'MS_Description', '" + en.EnDesc + "', 'user', dbo, 'table', '" + en.EnMap.PhysicsTable + "'";
                en.RunSQL(sql);
            }
            catch (Exception ex)
            {
                string sql = "execute  sp_updateextendedproperty  'MS_Description', '" + en.EnDesc + "', 'user', dbo, 'table', '" + en.EnMap.PhysicsTable + "'";
                en.RunSQL(sql);
            }

            SysEnums ses = new SysEnums();
            foreach (Attr attr in en.EnMap.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.Key == attr.Desc)
                    continue;
                if (attr.Field == attr.Desc)
                    continue;

                switch (attr.MyFieldType)
                {
                    case FieldType.PK:
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + "(主键)");
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+"'");
                        break;
                    case FieldType.Normal:
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+"'");
                        break;
                    case FieldType.Enum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        //	en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"++"'" );
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ",枚举类型:" + ses.ToDesc());
                        break;
                    case FieldType.PKEnum:
                        ses = new SysEnums(attr.Key, attr.UITag);
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ",主键:枚举类型:" + ses.ToDesc());
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+en.EnDesc+", 主键:枚举类型:"+ses.ToDesc()+"'" );
                        break;
                    case FieldType.FK:
                        Entity myen = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ", 外键:对应物理表:" + myen.EnMap.PhysicsTable + ",表描述:" + myen.EnDesc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS "+  );
                        break;
                    case FieldType.PKFK:
                        Entity myen1 = attr.HisFKEn; // ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                        AddColNote(en, en.EnMap.PhysicsTable, attr.Field, attr.Desc + ", 主外键:对应物理表:" + myen1.EnMap.PhysicsTable + ",表描述:" + myen1.EnDesc);
                        //en.RunSQL("comment on table "+ en.EnMap.PhysicsTable+"."+attr.Field +" IS '"+  );
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 产程系统报表，如果出现问题，就写入日志里面。
        /// </summary>
        /// <returns></returns>
        public static string DBRpt1(DBCheckLevel level, Entities ens)
        {
            Entity en = ens.GetNewEntity;
            if (en.EnMap.EnDBUrl.DBUrlType != DBUrlType.AppCenterDSN)
                return null;

            if (en.EnMap.EnType == EnType.ThirdPartApp)
                return null;

            if (en.EnMap.EnType == EnType.View)
                return null;

            if (en.EnMap.EnType == EnType.Ext)
                return null;

            // 检测物理表的字段。
            en.CheckPhysicsTable();

            PubClass.AddComment(en);
            return "";

            string msg = "";

            string table = en.EnMap.PhysicsTable;
            Attrs fkAttrs = en.EnMap.HisFKAttrs;
            if (fkAttrs.Count == 0)
                return msg;
            int num = 0;
            string sql;
            //string msg="";
            foreach (Attr attr in fkAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                string enMsg = "";
                try
                {
                    #region 更新他们，去掉左右空格，因为外键不能包含左右空格。
                    if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                    {
                        /*如果是高中级别,就去掉左右空格*/
                        if (attr.MyDataType == DataType.AppString)
                        {
                            DBAccess.RunSQL("UPDATE " + en.EnMap.PhysicsTable + " SET " + attr.Field + " = rtrim( ltrim(" + attr.Field + ") )");
                        }
                    }
                    #endregion

                    #region 处理关联表的情况.
                    Entities refEns = attr.HisFKEns; // ClassFactory.GetEns(attr.UIBindKey);
                    Entity refEn = refEns.GetNewEntity;

                    //取出关联的表。
                    string reftable = refEn.EnMap.PhysicsTable;
                    //sql="SELECT COUNT(*) FROM "+en.EnMap.PhysicsTable+" WHERE "+attr.Key+" is null or len("+attr.Key+") < 1 ";
                    // 判断外键表是否存在。

                    sql = "SELECT COUNT(*) FROM  sysobjects  WHERE  name = '" + reftable + "'";
                    //num=DA.DBAccess.RunSQLReturnValInt(sql,0);
                    if (DBAccess.IsExitsObject(new DBUrl(DBUrlType.AppCenterDSN), reftable) == false)
                    {
                        //报告错误信息
                        enMsg += "<br>@检查实体：" + en.EnDesc + ",字段 " + attr.Key + " , 字段描述:" + attr.Desc + " , 外键物理表:" + reftable + "不存在:" + sql;
                    }
                    else
                    {
                        Attr attrRefKey = refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue); // 去掉主键的左右 空格．
                        if (attrRefKey.MyDataType == DataType.AppString)
                        {
                            if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                            {
                                /*如果是高中级别,就去掉左右空格*/
                                DBAccess.RunSQL("UPDATE " + reftable + " SET " + attrRefKey.Field + " = rtrim( ltrim(" + attrRefKey.Field + ") )");
                            }
                        }

                        Attr attrRefText = refEn.EnMap.GetAttrByKey(attr.UIRefKeyText);  // 去掉主键 Text 的左右 空格．

                        if (level == DBCheckLevel.Middle || level == DBCheckLevel.High)
                        {
                            /*如果是高中级别,就去掉左右空格*/
                            DBAccess.RunSQL("UPDATE " + reftable + " SET " + attrRefText.Field + " = rtrim( ltrim(" + attrRefText.Field + ") )");
                        }

                    }
                    #endregion

                    #region 外键的实体是否为空
                    switch (en.EnMap.EnDBUrl.DBType)
                    {
                        case DBType.Oracle:
                            sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " is null or length(" + attr.Field + ") < 1 ";
                            break;
                        default:
                            sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " is null or len(" + attr.Field + ") < 1 ";
                            break;
                    }

                    num = DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    if (num == 0)
                    {
                    }
                    else
                    {
                        enMsg += "<br>@检查实体：" + en.EnDesc + ",物理表:" + en.EnMap.PhysicsTable + "出现" + attr.Key + "," + attr.Desc + "不正确,共有[" + num + "]行记录没有数据。" + sql;
                    }
                    #endregion

                    #region 是否能够对应到外键
                    //是否能够对应到外键。
                    sql = "SELECT COUNT(*) FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( SELECT " + refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue).Field + " FROM " + reftable + "	 ) ";
                    num = DA.DBAccess.RunSQLReturnValInt(sql, 0);
                    if (num == 0)
                    {
                    }
                    else
                    {
                        /*如果是高中级别.*/
                        string delsql = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( SELECT " + refEn.EnMap.GetAttrByKey(attr.UIRefKeyValue).Field + " FROM " + reftable + "	 ) ";
                        //int i =DBAccess.RunSQL(delsql);
                        enMsg += "<br>@" + en.EnDesc + ",物理表:" + en.EnMap.PhysicsTable + "出现" + attr.Key + "," + attr.Desc + "不正确,共有[" + num + "]行记录没有关联到数据，请检查物理表与外键表。" + sql + "如果您想删除这些对应不上的数据请运行如下SQL: " + delsql + " 请慎重执行.";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    enMsg += "<br>@" + ex.Message;
                }

                if (enMsg != "")
                {
                    msg += "<BR><b>-- 检查[" + en.EnDesc + "," + en.EnMap.PhysicsTable + "]出现如下问题,类名称:" + en.ToString() + "</b>";
                    msg += enMsg;
                }
            }
            return msg;
        }
        #endregion

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static DataTable HashtableToDataTable(Hashtable ht)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Hashtable";
            foreach (string key in ht.Keys)
            {
                dt.Columns.Add(key, typeof(string));
            }

            DataRow dr = dt.NewRow();
            foreach (string key in ht.Keys)
            {
                dr[key] = ht[key] as string;
            }
            dt.Rows.Add(dr);
            return dt;
        }

        #region 通用方法.
        public static Hashtable GetMainTableHT()
        {
            Hashtable htMain = new Hashtable();
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(key))
                    continue;
                string mykey = key;
                string val = HttpContextHelper.RequestParams(key);
                mykey = mykey.Replace("TB_", "");
                mykey = mykey.Replace("DDL_", "");
                mykey = mykey.Replace("CB_", "");
                mykey = mykey.Replace("RB_", "");
                val = HttpUtility.UrlDecode(val, Encoding.UTF8);

                if (htMain.ContainsKey(mykey) == true)
                    htMain[mykey] = val;
                else
                    htMain.Add(mykey, val);
            }
            return htMain;
        }
        public static BP.En.Entity CopyFromRequest(BP.En.Entity en)
        {
            //获取传递来的所有的checkbox ids 用于设置该属性为falsse.
            string checkBoxIDs = HttpContextHelper.RequestParams("CheckBoxIDs");
            if (checkBoxIDs != null)
            {
                string[] strs = checkBoxIDs.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    string key = str.Replace("CB_", "");
                    if (en.Row.ContainsKey(key) == false)
                        continue; //判断是否存在?

                    //设置该属性为false.
                    en.Row[key] = 0;
                }
            }

            Attrs attrs = en.EnMap.Attrs;
            /*说明已经找到了这个字段信息。*/
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(key))
                    continue;

                //获得实际的值, 具有特殊标记的，系统才赋值.
                string attrKey = key.Clone() as string;
                if (key.StartsWith("TB_"))
                    attrKey = attrKey.Replace("TB_", "");
                else if (key.StartsWith("CB_"))
                    attrKey = attrKey.Replace("CB_", "");
                else if (key.StartsWith("DDL_"))
                    attrKey = attrKey.Replace("DDL_", "");
                else if (key.StartsWith("RB_"))
                    attrKey = attrKey.Replace("RB_", "");
                else
                    continue;

                if (en.Row.ContainsKey(attrKey) == false)
                    continue; //判断是否存在?

                var val = HttpContextHelper.RequestParams(key);
                Attr attr = attrs.GetAttrByKey(attrKey);
                if (val == null)
                    val = attr.DefaultVal.ToString(); //如果此值为空,就让其设置默认值.

                //如果是数值类型的值.
                if (attr.IsNum && DataType.IsNumStr(val.ToString()) == false)
                    throw new Exception("err@[" + en.ToString() + "," + en.EnDesc + "]输入错误:" + attr.Key + "," + attr.Desc + ",应该是数值类型，但是输入了[" + val.ToString() + "]");

                //设置他的属性.
                en.SetValByKey(attrKey, val);
            }
            return en;
        }
        public static BP.En.Entity CopyFromRequestByPost(BP.En.Entity en)
        {
            //获取传递来的所有的checkbox ids 用于设置该属性为falsse.
            string checkBoxIDs = HttpContextHelper.RequestParams("CheckBoxIDs");
            if (checkBoxIDs != null)
            {
                string[] strs = checkBoxIDs.Split(',');
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str))
                        continue;

                    if (str.Contains("CBPara"))
                    {
                        /*如果是参数字段.*/
                        en.Row[str.Replace("CBPara_", "")] = 0;
                    }
                    else
                    {
                        //设置该属性为false.
                        en.Row[str.Replace("CB_", "")] = 0;
                    }
                }
            }

            //如果不使用clone 就会导致 “集合已修改;可能无法执行枚举操作。”的错误。
            Hashtable ht = en.Row.Clone() as Hashtable;

            /*说明已经找到了这个字段信息。*/
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(key))
                    continue;

                //获得实际的值, 具有特殊标记的，系统才赋值.
                string attrKey = key.Clone() as string;
                if (key.StartsWith("TB_"))
                    attrKey = attrKey.Replace("TB_", "");
                else if (key.StartsWith("CB_"))
                    attrKey = attrKey.Replace("CB_", "");
                else if (key.StartsWith("DDL_"))
                    attrKey = attrKey.Replace("DDL_", "");
                else if (key.StartsWith("RB_"))
                    attrKey = attrKey.Replace("RB_", "");
                else
                {
                    //给参数赋值.
                    if (key.StartsWith("TBPara_"))
                        attrKey = attrKey.Replace("TBPara_", "");
                    else if (key.StartsWith("DDLPara_"))
                        attrKey = attrKey.Replace("DDLPara_", "");
                    else if (key.StartsWith("RBPara_"))
                        attrKey = attrKey.Replace("RBPara_", "");
                    else if (key.StartsWith("CBPara_"))
                        attrKey = attrKey.Replace("CBPara_", "");
                    else
                        continue;
                }

                string val = HttpContextHelper.RequestParams(key); // Form[key]
                if (key.IndexOf("CB_") == 0 || key.IndexOf("CBPara_") == 0)
                {
                    en.Row[attrKey] = 1;
                    continue;
                }

                //其他的属性.
                en.Row[attrKey] = val;
            }
            return en;
        }
        /// <summary>
        /// 明细表传参保存
        /// </summary>
        /// <param name="en"></param>
        /// <param name="pk"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static Entity CopyDtlFromRequests11(Entity en, string pk, Map map)
        {
            string allKeys = ";";
            if (DataType.IsNullOrEmpty(pk))
                pk = "";
            else
                pk = "_" + pk;

            foreach (string myK in HttpContextHelper.RequestParamKeys)
                allKeys += myK + ";";

            Attrs attrs = map.Attrs;
            foreach (Attr attr in attrs)
            {
                string relKey = null;
                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        relKey = "TB_" + attr.Key + pk;
                        break;
                    case UIContralType.CheckBok:
                        relKey = "CB_" + attr.Key + pk;
                        break;
                    case UIContralType.DDL:
                        relKey = "DDL_" + attr.Key + pk;
                        break;
                    case UIContralType.RadioBtn:
                        relKey = "RB_" + attr.Key + pk;
                        break;
                    case UIContralType.MapPin:
                        relKey = "TB_" + attr.Key + pk;
                        break;
                    default:
                        break;
                }

                if (relKey == null)
                    continue;

                if (allKeys.Contains(relKey + ";"))
                {
                    /*说明已经找到了这个字段信息。*/
                    foreach (string myK in HttpContextHelper.RequestParamKeys)
                    {
                        if (DataType.IsNullOrEmpty(myK))
                            continue;

                        if (myK.EndsWith(relKey))
                        {
                            if (attr.UIContralType == UIContralType.CheckBok)
                            {
                                string val = HttpContextHelper.RequestParams(myK);
                                if (val == "on" || val == "1" || val.Contains(",on"))
                                    en.SetValByKey(attr.Key, 1);
                                else
                                    en.SetValByKey(attr.Key, 0);
                            }
                            else
                            {
                                en.SetValByKey(attr.Key, HttpContextHelper.RequestParams(myK));
                            }
                        }
                    }
                    continue;
                }
            }
            return en;
        }
        #endregion

        /// <summary>
        /// 外部参数.
        /// </summary>
        public static string RequestParas
        {
            get
            {
                return BP.Difference.Glo.RequestParas;
            }
        }
    }
}