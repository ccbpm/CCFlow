
/*
简介：负责存取数据的类
创建时间：2002-10
最后修改时间：2004-2-1 ccb

 说明：
  在次文件种处理了4种方式的连接。
  1， sql server .
  2， oracle.
  3， ole.
  4,  odbc.
  
*/
using System;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Data.OracleClient;
using System.EnterpriseServices;
using System.Data.OleDb;
using System.Web;
using System.Data.Odbc;
using System.IO;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using IBM.Data;
using IBM.Data.Informix;
using IBM.Data.Utilities;
using BP.Sys;

namespace BP.DA
{
    /// <summary>
    /// 数据库访问。
    /// 这个类负责处理了 实体信息
    /// </summary>
    public class DBAccess
    {
        #region 文件存储数据库处理.
        /// <summary>
        /// 保存文件到数据库
        /// </summary>
        /// <param name="bytes">数据流</param>
        /// <param name="tableName">表名称</param>
        /// <param name="tablePK">表主键</param>
        /// <param name="pkVal">主键值</param>
        /// <param name="saveFileField">保存到字段</param>
        public static void SaveFileToDB(byte[] bytes, string tableName, string tablePK, string pkVal, string saveToFileField)
        {
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                SqlConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as SqlConnection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                SqlCommand cm = new SqlCommand();
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;
                if (cn.State == 0) cn.Open();
                cm.CommandText = "UPDATE " + tableName + " SET " + saveToFileField + "=@FlowJsonFile WHERE " + tablePK + " =@PKVal";

                SqlParameter spFile = new SqlParameter("@FlowJsonFile", SqlDbType.Image);
                spFile.Value = bytes;
                cm.Parameters.Add(spFile);

                SqlParameter spPK = new SqlParameter("@PKVal", SqlDbType.VarChar);
                spPK.Value = pkVal;
                cm.Parameters.Add(spPK);

                // 执行它.
                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (BP.DA.DBAccess.IsExitsTableCol(tableName, saveToFileField) == false)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        string sql = "ALTER TABLE " + tableName + " ADD  " + saveToFileField + " image ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }
                return;
            }

            //修复for：jlow  oracle 异常： ORA-01745: 无效的主机/绑定变量名 edited by qin 16.7.1
            //错误的引用oracle的关键字file
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                OracleConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as OracleConnection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                OracleCommand cm = new OracleCommand();
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;
                if (cn.State == 0) cn.Open();
                cm.CommandText = "UPDATE " + tableName + " SET " + saveToFileField + "=:FlowJsonFile WHERE " + tablePK + " =:PKVal";

                OracleParameter spFile = new OracleParameter("FlowJsonFile", OracleType.Blob);
                spFile.Value = bytes;
                cm.Parameters.Add(spFile);

                OracleParameter spPK = new OracleParameter("PKVal", OracleType.NVarChar);
                spPK.Value = pkVal;
                cm.Parameters.Add(spPK);

                // 执行它.
                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (BP.DA.DBAccess.IsExitsTableCol(tableName, saveToFileField) == false)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        //修改数据类型   oracle 不存在image类型   edited by qin 16.7.1
                        string sql = "ALTER TABLE " + tableName + " ADD  " + saveToFileField + " blob ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }
                return;
            }

            //added by liuxc,2016-12-7，增加对mysql大数据longblob字段存储逻辑
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                MySqlConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as MySqlConnection;

                if (cn.State != ConnectionState.Open)
                    cn.Open();

                MySqlCommand cm = new MySqlCommand();
                cm.Connection = cn;
                cm.CommandType = CommandType.Text;
                cm.CommandText = "UPDATE " + tableName + " SET " + saveToFileField + "=@FlowJsonFile WHERE " + tablePK + " =@PKVal";

                MySqlParameter spFile = new MySqlParameter("FlowJsonFile", MySqlDbType.Blob);
                spFile.Value = bytes;
                cm.Parameters.Add(spFile);

                MySqlParameter spPK = new MySqlParameter("PKVal", MySqlDbType.VarChar);
                spPK.Value = pkVal;
                cm.Parameters.Add(spPK);

                try
                {
                    cm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    if (BP.DA.DBAccess.IsExitsTableCol(tableName, saveToFileField) == false)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        string sql = "ALTER TABLE " + tableName + " ADD  " + saveToFileField + " BLOB NULL ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }

                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }
                return;
            }
        }
        /// <summary>
        /// 保存文件到数据库
        /// </summary>
        /// <param name="fullFileName">完成的文件路径</param>
        /// <param name="tableName">表名称</param>
        /// <param name="tablePK">表主键</param>
        /// <param name="pkVal">主键值</param>
        /// <param name="saveFileField">保存到字段</param>
        public static void SaveBigTextToDB(string docs, string tableName, string tablePK, string pkVal, string saveToFileField)
        {
            System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
            //byte[] inputBytes = converter.GetBytes(docs);
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(docs);
            //执行保存.
            SaveFileToDB(inputBytes, tableName, tablePK, pkVal, saveToFileField);
        }
        /// <summary>
        /// 保存文件到数据库
        /// </summary>
        /// <param name="fullFileName">完成的文件路径</param>
        /// <param name="tableName">表名称</param>
        /// <param name="tablePK">表主键</param>
        /// <param name="pkVal">主键值</param>
        /// <param name="saveFileField">保存到字段</param>
        public static void SaveFileToDB(string fullFileName, string tableName, string tablePK, string pkVal, string saveToFileField)
        {
            FileInfo fi = new FileInfo(fullFileName);
            FileStream fs = fi.OpenRead();
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

            // bug 的提示者 http://bbs.ccflow.org/showtopic-3958.aspx
            fs.Close();
            fs.Dispose();

            //执行保存.
            SaveFileToDB(bytes, tableName, tablePK, pkVal, saveToFileField);
        }
        public static void GetFileFromDB(string fileFullName,string tableName, string tablePK, string pkVal, string fileSaveField)
        {
            byte[] byteFile = GetByteFromDB(tableName, tablePK, pkVal, fileSaveField);
            FileStream fs;
            //如果文件不为空,就把流数据保存一个文件.
            if (fileFullName != null)
            {
                FileInfo fi = new System.IO.FileInfo(fileFullName);
                fs = fi.OpenWrite();
                fs.Write(byteFile, 0, byteFile.Length);
                fs.Close();
            }
        }
        /// <summary>
        /// 从数据库里获得文本
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tablePK">主键</param>
        /// <param name="pkVal">主键值</param>
        /// <param name="fileSaveField">保存字段</param>
        /// <returns></returns>
        public static string GetBigTextFromDB(string tableName, string tablePK, string pkVal, string fileSaveField)
        {
            byte[] byteFile = GetByteFromDB(tableName, tablePK, pkVal, fileSaveField);
            if (byteFile == null)
                return null;

            //System.Text.UnicodeEncoding converter = new System.Text.UnicodeEncoding();
            //return converter.GetString(byteFile);
            return System.Text.Encoding.UTF8.GetString(byteFile);
        }
        /// <summary>
        /// 从数据库里提取文件
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tablePK">表主键</param>
        /// <param name="pkVal">主键值</param>
        /// <param name="fileSaveField">字段</param>
        public static byte[] GetByteFromDB(string tableName, string tablePK, string pkVal, string fileSaveField)
        {
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
            {
                SqlConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as SqlConnection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                string strSQL = "SELECT [" + fileSaveField + "] FROM " + tableName + " WHERE " + tablePK + "='" + pkVal + "'";

                SqlDataReader dr = null;
                SqlCommand cm = new SqlCommand();
                cm.Connection = cn;
                cm.CommandText = strSQL;
                cm.CommandType = CommandType.Text;

                // 执行它.
                try
                {
                    dr = cm.ExecuteReader();
                }
                catch (Exception ex)
                {
                    if (!BP.DA.DBAccess.IsExitsTableCol(tableName, fileSaveField))
                    {
                        /*如果没有此列，就自动创建此列.*/
                        string sql = "ALTER TABLE " + tableName + " ADD  " + fileSaveField + " image ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }

                byte[] byteFile = null;
                if (dr.Read())
                {
                    if (dr[0] == null || DataType.IsNullOrEmpty(dr[0].ToString()))
                        return null;

                    byteFile = (byte[])dr[0];
                }
                return byteFile;


            }

            //增加对oracle数据库的逻辑 qin
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
            {
                OracleConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as OracleConnection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                string strSQL = "SELECT " + fileSaveField + " FROM " + tableName + " WHERE " + tablePK + "='" + pkVal + "'";

                OracleDataReader dr = null;
                OracleCommand cm = new OracleCommand();
                cm.Connection = cn;
                cm.CommandText = strSQL;
                cm.CommandType = CommandType.Text;


                // 执行它.
                try
                {
                    dr = cm.ExecuteReader();
                }
                catch (Exception ex)
                {
                    if (BP.DA.DBAccess.IsExitsTableCol(tableName, fileSaveField) == false)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        string sql = "ALTER TABLE " + tableName + " ADD  " + fileSaveField + " blob ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }

                byte[] byteFile = null;
                if (dr.Read())
                {
                    if (dr[0] == null || DataType.IsNullOrEmpty(dr[0].ToString()))
                        return null;

                    byteFile = (byte[])dr[0];
                }
                
                return byteFile;
            }

            //added by liuxc,2016-12-7,增加对mysql数据库的逻辑
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MySQL)
            {
                MySqlConnection cn = BP.DA.DBAccess.GetAppCenterDBConn as MySqlConnection;
                if (cn.State != ConnectionState.Open)
                    cn.Open();

                string strSQL = "SELECT " + fileSaveField + " FROM " + tableName + " WHERE " + tablePK + "='" + pkVal + "'";

                MySqlDataReader dr = null;
                MySqlCommand cm = new MySqlCommand();
                cm.Connection = cn;
                cm.CommandText = strSQL;
                cm.CommandType = CommandType.Text;


                // 执行它.
                try
                {
                    dr = cm.ExecuteReader();
                }
                catch (Exception ex)
                {
                    if (BP.DA.DBAccess.IsExitsTableCol(tableName, fileSaveField) == false)
                    {
                        /*如果没有此列，就自动创建此列.*/
                        string sql = "ALTER TABLE " + tableName + " ADD " + fileSaveField + " LONGBLOB NULL ";
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                    throw new Exception("@缺少此字段,有可能系统自动修复." + ex.Message);
                }

                byte[] byteFile = null;
                if (dr.Read())
                {
                    if (dr[0] == null || DataType.IsNullOrEmpty(dr[0].ToString()))
                        return null;

                    byteFile = dr[0] as byte[];
                    //System.Text.Encoding.Default.GetBytes(dr[0].ToString());
                }

                return byteFile;
            }

            //最后仍然没有找到.
            throw new Exception("@没有判断的数据库类型.");
        }
        
        #endregion 文件存储数据库处理.


        #region 事务处理
        /// <summary>
        /// 执行增加一个事务
        /// </summary>
        public static void DoTransactionBegin()
        {
            return;

            //if (SystemConfig.AppCenterDBType != DBType.MSSQL)
            //    return;
            //if (BP.Web.WebUser.No == null)
            //    return;
            //SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            //BP.DA.Cash.SetConn(BP.Web.WebUser.No, conn);
            //DBAccess.RunSQL("BEGIN TRANSACTION");
        }
        /// <summary>
        /// 回滚事务
        /// </summary>
        public static void DoTransactionRollback()
        {
            return;

            //if (SystemConfig.AppCenterDBType != DBType.MSSQL)
            //    return;

            //if (BP.Web.WebUser.No == null)
            //    return;

            //DBAccess.RunSQL("Rollback TRANSACTION");
            //SqlConnection conn = BP.DA.Cash.GetConn(BP.Web.WebUser.No) as SqlConnection;
            //conn.Close();
            //conn.Dispose();
        }
        /// <summary>
        /// 提交事务
        /// </summary>
        public static void DoTransactionCommit()
        {
            return;

            if (SystemConfig.AppCenterDBType != DBType.MSSQL)
                return;

            if (BP.Web.WebUser.No == null)
                return;

            DBAccess.RunSQL("Commit TRANSACTION");
        }
        #endregion 事务处理

        public static Paras DealParasBySQL(string sql, Paras ps)
        {
            Paras myps = new Paras();
            foreach (Para p in ps)
            {
                if (sql.Contains(":" + p.ParaName) == false)
                    continue;
                myps.Add(p);
            }
            return myps;
        }
        /// <summary>
        /// 运行一个sql返回该table的第1列，组成一个查询的where in 字符串.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string GenerWhereInPKsString(string sql)
        {
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return GenerWhereInPKsString(dt);
        }
        /// <summary>
        /// 通过table的第1列，组成一个查询的where in 字符串.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GenerWhereInPKsString(DataTable dt)
        {
            string pks = "";
            foreach (DataRow dr in dt.Rows)
            {
                pks += "'" + dr[0] + "',";
            }
            if (pks == "")
                return "";
            return pks.Substring(0, pks.Length - 1);
        }
        /// <summary>
        /// 检查是否连接成功.
        /// </summary>
        /// <returns></returns>
        public static bool TestIsConnection()
        {
            try
            {
                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 ");
                        break;
                    case DBType.Oracle:
                    case DBType.MySQL:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 FROM DUAL ");
                        break;
                    case DBType.Informix:
                        BP.DA.DBAccess.RunSQLReturnString("SELECT 1+2 FROM DUAL ");
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #region IO
        public static void copyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                //   Sub   directories   
                if (Directory.Exists(Element))
                    copyDirectory(Element, Dst + Path.GetFileName(Element));
                //   Files   in   directory   
                else
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
            }
        }
        #endregion

        #region 读取Xml

        #endregion

        #region 关于运行存储过程

        #region 执行存储过程返回影响个数
        public static int RunSP(string spName, string paraKey, object paraVal)
        {
            Paras pas = new Paras();
            pas.Add(paraKey, paraVal);
            return DBAccess.RunSP(spName, pas);
        }
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <returns>返回影响的行数</returns>
        public static int RunSP(string spName)
        {
            int i = 0;
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.Access:
                    return DBProcedure.RunSP(spName, (SqlConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Oracle:
                    return DBProcedure.RunSP(spName, (OracleConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Informix:
                    return DBProcedure.RunSP(spName, (IfxConnection)DBAccess.GetAppCenterDBConn);
                default:
                    throw new Exception("Error: " + BP.Sys.SystemConfig.AppCenterDBType);
            }
        }
        public static int RunSPReturnInt(string spName)
        {
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return DBProcedure.RunSP(spName, (SqlConnection)DBAccess.GetAppCenterDBConn);
                case DBType.MySQL:
                    return DBProcedure.RunSP(spName, (MySqlConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Informix:
                    return DBProcedure.RunSP(spName, (IfxConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Access:
                case DBType.Oracle:
                    return DBProcedure.RunSP(spName, (OracleConnection)DBAccess.GetAppCenterDBConn);
                default:
                    throw new Exception("Error: " + BP.Sys.SystemConfig.AppCenterDBType);
            }
        }

        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <param name="paras">参数</param>
        /// <returns>返回影响的行数</returns>
        public static int RunSP(string spName, Paras paras)
        {
            int i = 0;
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return DBProcedure.RunSP(spName, paras, (SqlConnection)DBAccess.GetAppCenterDBConn);
                case DBType.MySQL:
                case DBType.Access:
                    // return DBProcedure.RunSP(spName, paras, new MySqlConnection(SystemConfig.AppCenterDSN));
                    throw new Exception("@没有实现...");
                case DBType.Oracle:
                    return DBProcedure.RunSP(spName, paras, (OracleConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Informix:
                    return DBProcedure.RunSP(spName, paras, (IfxConnection)DBAccess.GetAppCenterDBConn);
                default:
                    throw new Exception("Error " + BP.Sys.SystemConfig.AppCenterDBType);
            }
        }
        #endregion

        #region 运行存储过程返回 DataTable
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSPReTable(string spName)
        {
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                case DBType.Access:
                    return DBProcedure.RunSPReturnDataTable(spName, (SqlConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Oracle:
                    return DBProcedure.RunSPReturnDataTable(spName, (OracleConnection)DBAccess.GetAppCenterDBConn);
                case DBType.Informix:
                    return DBProcedure.RunSPReturnDataTable(spName, (IfxConnection)DBAccess.GetAppCenterDBConn);
                default:
                    throw new Exception("Error " + BP.Sys.SystemConfig.AppCenterDBType);

            }
        }
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <param name="paras">参数</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSPReTable(string spName, Paras paras)
        {
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return DBProcedure.RunSPReturnDataTable(spName, paras, new SqlConnection(SystemConfig.AppCenterDSN));
                case DBType.Oracle:
                    return DBProcedure.RunSPReturnDataTable(spName, paras, new OracleConnection(SystemConfig.AppCenterDSN));
                case DBType.Informix:
                    return DBProcedure.RunSPReturnDataTable(spName, paras, new IfxConnection(SystemConfig.AppCenterDSN));
                case DBType.Access:
                default:
                    throw new Exception("Error " + BP.Sys.SystemConfig.AppCenterDBType);
            }
        }
        #endregion

        #endregion

        //构造函数
        static DBAccess()
        {
            CurrentSys_Serial = new Hashtable();
            KeyLockState = new Hashtable();
        }

        #region 运行中定义的变量
        public static readonly Hashtable CurrentSys_Serial;
        private static int readCount = -1;
        private static readonly Hashtable KeyLockState;
        #endregion


        #region 产生序列号码方法
        /// <summary>
        /// 根据标识产生的序列号
        /// </summary>
        /// <param name="type">OID</param>
        /// <returns></returns>
        public static int GenerSequenceNumber(string type)
        {
            if (readCount == -1)  //系统第一次运行时
            {
                DataTable tb = DBAccess.RunSQLReturnTable("SELECT CfgKey, IntVal FROM Sys_Serial ");
                foreach (DataRow row in tb.Rows)
                {
                    string str = row[0].ToString().Trim();
                    int id = Convert.ToInt32(row[1]);
                    try
                    {
                        CurrentSys_Serial.Add(str, id);
                        KeyLockState.Add(row[0].ToString().Trim(), false);
                    }
                    catch
                    {
                    }
                }
                readCount++;
            }
            if (CurrentSys_Serial.ContainsKey(type) == false)
            {
                DBAccess.RunSQL("insert into Sys_Serial values('" + type + "',1 )");
                return 1;
            }

            while (true)
            {
                while (!(bool)KeyLockState[type])
                {
                    KeyLockState[type] = true;
                    int cur = (int)CurrentSys_Serial[type];
                    if (readCount++ % 10 == 0)
                    {
                        readCount = 1;
                        int n = (int)CurrentSys_Serial[type] + 10;

                        Paras ps = new Paras();
                        ps.Add("intVal", n);
                        ps.Add("CfgKey", type);

                        string upd = "update Sys_Serial set intVal=" + SystemConfig.AppCenterDBVarStr + "intVal WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
                        DBAccess.RunSQL(upd, ps);
                    }

                    cur++;
                    CurrentSys_Serial[type] = cur;
                    KeyLockState[type] = false;
                    return cur;
                }
            }
        }
        /// <summary>
        /// 生成 GenerOIDByGUID.
        /// </summary>
        /// <returns></returns>
        public static int GenerOIDByGUID()
        {
            int i = BP.Tools.CRC32Helper.GetCRC32(Guid.NewGuid().ToString());
            if (i <= 0)
                i = -i;
            return i;
        }
        /// <summary>
        /// 生成 GenerOIDByGUID.
        /// </summary>
        /// <returns></returns>
        public static int GenerOIDByGUID(string strs)
        {
            int i = BP.Tools.CRC32Helper.GetCRC32(strs);
            if (i <= 0)
                i = -i;
            return i;
        }
        /// <summary>
        /// 生成 GenerGUID
        /// </summary>
        /// <returns></returns>
        public static string GenerGUID()
        {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 锁定OID
        /// </summary>
        private static bool lock_OID = false;
        /// <summary>
        /// 产生一个OID
        /// </summary>
        /// <returns></returns>
        public static int GenerOID()
        {
            while (lock_OID == true)
            {
            }

            lock_OID = true;
            if (DBAccess.RunSQL("UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey='OID'") == 0)
                DBAccess.RunSQL("INSERT INTO Sys_Serial (CfgKey,IntVal) VALUES ('OID',100)");
            int oid = DBAccess.RunSQLReturnValInt("SELECT  IntVal FROM Sys_Serial WHERE CfgKey='OID'");
            lock_OID = false;
            return oid;
        }
        /// <summary>
        /// 锁
        /// </summary>
        private static bool lock_OID_CfgKey = false;
        /// <summary>
        /// 生成唯一的序列号
        /// </summary>
        /// <param name="cfgKey">配置信息</param>
        /// <returns>唯一的序列号</returns>
        public static Int64 GenerOID_2013(string cfgKey)
        {
            while (lock_OID_CfgKey == true)
            {
            }
            lock_OID_CfgKey = true;

            Paras ps = new Paras();
            ps.Add("CfgKey", cfgKey);
            string sql = "UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQL(sql, ps);
            if (num == 0)
            {
                sql = "INSERT INTO Sys_Serial (CFGKEY,INTVAL) VALUES ('" + cfgKey + "',100)";
                DBAccess.RunSQL(sql);
                lock_OID_CfgKey = false;
                return 100;
            }
            sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            num = DBAccess.RunSQLReturnValInt(sql, ps);
            lock_OID_CfgKey = false;
            return num;
        }
        #region 第二版本的生成 OID。
        /// <summary>
        /// 锁
        /// </summary>
        private static bool lock_HT_CfgKey = false;
        private static Hashtable lock_HT = new Hashtable();
        /// <summary>
        /// 生成唯一的序列号
        /// </summary>
        /// <param name="cfgKey">配置信息</param>
        /// <returns>唯一的序列号</returns>
        public static Int64 GenerOID(string cfgKey)
        {
            //while (lock_HT_CfgKey == true)
            //{
            //}
            lock_HT_CfgKey = true;

            if (lock_HT.ContainsKey(cfgKey) == false)
            {
            }
            else
            {
            }

            Paras ps = new Paras();
            ps.Add("CfgKey", cfgKey);
            //string sql = "UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            //int num = DBAccess.RunSQL(sql, ps);
            string sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQLReturnValInt(sql, ps);
            if (num == 0)
            {
                sql = "INSERT INTO Sys_Serial (CFGKEY,INTVAL) VALUES ('" + cfgKey + "',100)";
                DBAccess.RunSQL(sql);
                lock_HT_CfgKey = false;

                if (lock_HT.ContainsKey(cfgKey) == false)
                    lock_HT.Add(cfgKey, 200);

                return 100;
            }
            else
            {
                sql = "UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
                DBAccess.RunSQL(sql, ps);
            }
            sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            num = DBAccess.RunSQLReturnValInt(sql, ps);
            lock_HT_CfgKey = false;
            return num;
        }
        #endregion 第二版本的生成 OID。

        /// <summary>
        /// 获取一个从OID, 更新到OID.
        /// 用例: 我已经明确知道要用到260个OID, 
        /// 但是为了避免多次取出造成效率浪费，就可以一次性取出 260个OID.
        /// </summary>
        /// <param name="cfgKey"></param>
        /// <param name="getOIDNum">要获取的OID数量.</param>
        /// <returns>从OID</returns>
        public static Int64 GenerOID(string cfgKey, int getOIDNum)
        {
            Paras ps = new Paras();
            ps.Add("CfgKey", cfgKey);
            string sql = "UPDATE Sys_Serial SET IntVal=IntVal+" + getOIDNum + " WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQL(sql, ps);
            if (num == 0)
            {
                getOIDNum = getOIDNum + 100;
                sql = "INSERT INTO Sys_Serial (CFGKEY,INTVAL) VALUES (" + SystemConfig.AppCenterDBVarStr + "CfgKey," + getOIDNum + ")";
                DBAccess.RunSQL(sql, ps);
                return 100;
            }
            sql = "SELECT  IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            return DBAccess.RunSQLReturnValInt(sql, ps) - getOIDNum;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intKey"></param>
        /// <returns></returns>
        public static Int64 GenerOIDByKey64(string intKey)
        {
            Paras ps = new Paras();
            ps.Add("CfgKey", intKey);
            string sql = "";
            sql = "UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQL(sql, ps);
            if (num == 0)
            {
                sql = "INSERT INTO Sys_Serial (CFGKEY,INTVAL) VALUES (" + SystemConfig.AppCenterDBVarStr + "CfgKey,'1')";
                DBAccess.RunSQL(sql, ps);
                return Int64.Parse(intKey + "1");
            }
            sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int val = DBAccess.RunSQLReturnValInt(sql, ps);
            return Int64.Parse(intKey + val.ToString());
        }
        public static Int32 GenerOIDByKey32(string intKey)
        {
            Paras ps = new Paras();
            ps.Add("CfgKey", intKey);

            string sql = "";
            sql = "UPDATE Sys_Serial SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQL(sql, ps);
            if (num == 0)
            {
                sql = "INSERT INTO Sys_Serial (CFGKEY,INTVAL) VALUES (" + SystemConfig.AppCenterDBVarStr + "CfgKey,'100')";
                DBAccess.RunSQL(sql, ps);
                return int.Parse(intKey + "100");
            }
            sql = "SELECT IntVal FROM Sys_Serial WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int val = DBAccess.RunSQLReturnValInt(sql, ps);
            return int.Parse(intKey + val.ToString());
        }
        public static Int64 GenerOID(string table, string intKey)
        {
            Paras ps = new Paras();
            ps.Add("CfgKey", intKey);

            string sql = "";
            sql = "UPDATE " + table + " SET IntVal=IntVal+1 WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int num = DBAccess.RunSQL(sql, ps);
            if (num == 0)
            {
                sql = "INSERT INTO " + table + " (CFGKEY,INTVAL) VALUES (" + SystemConfig.AppCenterDBVarStr + "CfgKey,100)";
                DBAccess.RunSQL(sql, ps);
                return int.Parse(intKey + "100");
            }
            sql = "SELECT  IntVal FROM " + table + " WHERE CfgKey=" + SystemConfig.AppCenterDBVarStr + "CfgKey";
            int val = DBAccess.RunSQLReturnValInt(sql, ps);

            return Int64.Parse(intKey + val.ToString());
        }
        #endregion

        #region 取得连接对象 ，CS、BS共用属性【关键属性】
        /// <summary>
        /// AppCenterDBType
        /// </summary>
        public static DBType AppCenterDBType
        {
            get
            {
                return SystemConfig.AppCenterDBType;
            }
        }
        public static string _connectionUserID = null;
        /// <summary>
        /// 连接用户的ID
        /// </summary>
        public static string ConnectionUserID
        {
            get
            {
                if (_connectionUserID == null)
                {
                    string[] strs = BP.Sys.SystemConfig.AppCenterDSN.Split(';');
                    foreach (string str in strs)
                    {
                        if (str.Contains("user ") == true)
                        {
                            _connectionUserID = str.Split('=')[1];
                            break;
                        }
                    }
                }
                return _connectionUserID;
            }
        }
        public static IDbConnection GetAppCenterDBConn
        {
            get
            {
                string connstr = BP.Sys.SystemConfig.AppCenterDSN;
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        return new SqlConnection(connstr);
                    case DBType.Oracle:
                        return new OracleConnection(connstr);
                    case DBType.MySQL:
                        return new MySqlConnection(connstr);
                    case DBType.Informix:
                        return new IfxConnection(connstr);
                    case DBType.Access:
                    default:
                        throw new Exception("发现未知的数据库连接类型！");
                }
            }
        }

        public static IDbDataAdapter GetAppCenterDBAdapter
        {
            get
            {
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        return new SqlDataAdapter();
                    case DBType.Oracle:
                        return new OracleDataAdapter();
                    case DBType.MySQL:
                        return new MySqlDataAdapter();
                    case DBType.Informix:
                        return new IfxDataAdapter();
                    case DBType.Access:
                    default:
                        throw new Exception("发现未知的数据库连接类型！");
                }
            }
        }
        public static IDbCommand GetAppCenterDBCommand
        {
            get
            {
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        return new SqlCommand();
                    case DBType.Oracle:
                        return new OracleCommand();
                    case DBType.MySQL:
                        return new MySqlCommand();
                    case DBType.Informix:
                        return new IfxCommand();
                    case DBType.Access:
                    default:
                        throw new Exception("发现未知的数据库连接类型！");
                }
            }
        }

        #endregion 取得连接对象 ，CS、BS共用属性

        /// <summary>
        /// 同一个Connetion执行多条sql返回DataSet
        /// edited by qin 16.6.30 oracle数据库执行多条sql语句异常的修复
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public static DataSet RunSQLReturnDataSet(string sqls)
        {
            IDbConnection conn = GetAppCenterDBConn;
            IDbDataAdapter ada = GetAppCenterDBAdapter;
            try
            {
                IDbCommand cmd = GetAppCenterDBCommand;
                cmd.CommandText = sqls;
                cmd.CommandType = CommandType.Text;
                cmd.Connection = conn;
                if (ada != null)
                {
                    ada.SelectCommand = cmd;
                }

                DataSet oratb = new DataSet();

                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        ada.Fill(oratb);
                        break;
                    case DBType.Oracle:
                        string[] sqlArray = sqls.Split(';');
                        DataTable dt = null;
                        for (int i = 0; i < sqlArray.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(sqlArray[i]))
                                continue;

                            dt = DBAccess.RunSQLReturnTable(sqlArray[i]);
                            dt.TableName = "dt_" + i.ToString();
                            oratb.Tables.Add(dt);
                        }
                        break;
                    default:
                        throw new Exception("@发现未知的数据库连接类型！");
                }

                if (conn.State != ConnectionState.Open)
                    conn.Open();

                return oratb;
            }
            catch (Exception ex)
            {
                throw new Exception("SQLs=" + sqls + " Exception=" + ex.Message);
            }
            finally
            {
                (ada as System.Data.Common.DbDataAdapter).Dispose();
                conn.Close();
            }
        }

        #region 运行 SQL

        #region 在指定的Connection上执行 SQL 语句，返回受影响的行数

        #region OleDbConnection
        public static int RunSQLDropTable(string table)
        {
            if (IsExitsObject(new DBUrl(DBUrlType.AppCenterDSN), table))
            {
                switch (AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.MSSQL:
                    case DBType.Informix:
                    case DBType.Access:
                        return RunSQL("DROP TABLE " + table);
                    default:
                        throw new Exception(" Exception ");
                }
            }
            return 0;

            /* return RunSQL("TRUNCATE TABLE " + table);*/

        }

        public static int RunSQL(string sql, OleDbConnection conn, string dsn)
        {
            return RunSQL(sql, conn, CommandType.Text, dsn);
        }
        public static int RunSQL(string sql, OleDbConnection conn, CommandType sqlType, string dsn, params object[] pars)
        {
            try
            {
                if (conn == null)
                    conn = new OleDbConnection(dsn);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = dsn;
                    conn.Open();
                }

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandType = sqlType;
                int i = cmd.ExecuteNonQuery();

                //cmd.ExecuteReader();

                cmd.Dispose();
                conn.Close();

                //lock_SQL_RunSQL = false;
                return i;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }
        #endregion

        #region SqlConnection
        /// <summary>
        /// 运行SQL
        /// </summary>
        // private static bool lock_SQL_RunSQL = false;
        /// <summary>
        /// 运行SQL, 返回影响的行数.
        /// </summary>
        /// <param name="sql">msSQL</param>
        /// <param name="conn">SqlConnection</param>
        /// <returns>返回运行结果。</returns>
        public static int RunSQL(string sql, SqlConnection conn, string dsn)
        {
            return RunSQL(sql, conn, CommandType.Text, dsn);
        }
        /// <summary>
        /// 运行SQL , 返回影响的行数.
        /// </summary>
        /// <param name="sql">msSQL</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">params</param>
        /// <returns>返回运行结果</returns>
        public static int RunSQL(string sql, SqlConnection conn, CommandType sqlType, string dsn)
        {
            conn.Close();
#if DEBUG
            Debug.WriteLine(sql);
#endif
            //如果是锁定状态，就等待
            //while (lock_SQL_RunSQL)
            //    ;
            // 开始执行.
            //lock_SQL_RunSQL = true; //锁定
            string step = "1";
            try
            {

                if (conn == null)
                    conn = new SqlConnection(dsn);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = dsn;
                    conn.Open();
                }

                step = "2";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = sqlType;
                step = "3";

                step = "4";
                int i = 0;
                try
                {
                    i = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    step = "5";
                    //lock_SQL_RunSQL = false;
                    cmd.Dispose();
                    step = "6";
                    throw new Exception("RunSQL step=" + step + ex.Message + " SQL=" + sql);
                }
                step = "7";
                cmd.Dispose();
                // lock_SQL_RunSQL = false;
                return i;
            }
            catch (System.Exception ex)
            {
                step = "8";
                // lock_SQL_RunSQL = false;
                throw new Exception("RunSQL2 step=" + step + ex.Message + " 设置连接时间=" + conn.ConnectionTimeout);
            }
            finally
            {
                step = "9";
                //lock_SQL_RunSQL = false;
                conn.Close();
            }
        }
        #endregion

        #region OracleConnection
        public static int RunSQL(string sql, OracleConnection conn, string dsn)
        {
            return RunSQL(sql, conn, CommandType.Text, dsn);
        }
        public static int RunSQL(string sql, OracleConnection conn, CommandType sqlType, string dsn)
        {
#if DEBUG
            Debug.WriteLine(sql);
#endif
            //如果是锁定状态，就等待
            // while (lock_SQL_RunSQL)
            //  ;
            // 开始执行.
            // lock_SQL_RunSQL = true; //锁定
            string step = "1";
            try
            {
                if (conn == null)
                    conn = new OracleConnection(dsn);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = dsn;
                    conn.Open();
                }

                step = "2";
                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = sqlType;
                step = "3";
                int i = 0;
                try
                {
                    i = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    step = "5";
                    // lock_SQL_RunSQL = false;
                    cmd.Dispose();
                    step = "6";
                    throw new Exception("RunSQL step=" + step + ex.Message + " SQL=" + sql);
                }
                step = "7";
                cmd.Dispose();


                //lock_SQL_RunSQL = false;
                return i;
            }
            catch (System.Exception ex)
            {
                step = "8";
                // lock_SQL_RunSQL = false;
                throw new Exception("RunSQL2 step=" + step + ex.Message);
            }
            finally
            {
                step = "9";
                // lock_SQL_RunSQL = false;
                conn.Close();
            }

            /*
            Debug.WriteLine( sql );
            try
            {
                OracleCommand cmd = new OracleCommand( sql ,conn);
                cmd.CommandType = sqlType;
                foreach(object par in pars)
                {
                    cmd.Parameters.Add( "par",par);
                }
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
				
                int i= cmd.ExecuteNonQuery();				 
                cmd.Dispose();
                return i;				 
            }
            catch(System.Exception ex)
            {
                throw new Exception(ex.Message + sql );
            }
            finally
            {
                conn.Close();

            }
            */
        }
        #endregion

        #endregion

        #region 通过主应用程序在其他库上运行sql

        /// <summary>
        /// 删除表的主键
        /// </summary>
        /// <param name="table">表名称</param>
        public static void DropTablePK(string table)
        {
            string pkName = DBAccess.GetTablePKName(table);
            if (pkName == null)
                return;

            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                case DBType.MSSQL:
                    sql = "ALTER TABLE "+table+" DROP CONSTRAINT "+pkName;
                    break;
                case DBType.MySQL:
                    sql = "ALTER TABLE " + table + " DROP primary key";
                    break;
                default:
                    throw new Exception("@不支持的数据库类型." + SystemConfig.AppCenterDBType);
                    break;
            }
            BP.DA.DBAccess.RunSQL(sql);
        }

        #region pk
        /// <summary>
        /// 建立主键
        /// </summary>
        /// <param name="tab">物理表</param>
        /// <param name="pk">主键</param>
        public static void CreatePK(string tab, string pk, DBType db)
        {
            if (tab == null || tab == "")
                return;
            if (DBAccess.IsExitsTabPK(tab) == true)
                return;
            string sql;
            switch (db)
            {
                case DBType.Informix:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT  PRIMARY KEY(" + pk + ") CONSTRAINT " + tab + "pk ";
                    break;
                case DBType.MySQL:
                    sql = "ALTER TABLE " + tab + " ADD CONSTRAINT  PRIMARY KEY(" + pk + ") CONSTRAINT " + tab + "pk ";
                    break;
                default:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT " + tab + "pk PRIMARY KEY(" + pk.ToUpper() + ")";
                    break;
            }
            DBAccess.RunSQL(sql);
        }
        public static void CreatePK(string tab, string pk1, string pk2, DBType db)
        {
            if (tab == null || tab == "")
                return;

            if (DBAccess.IsExitsTabPK(tab) == true)
                return;

            string sql;
            switch (db)
            {
                case DBType.Informix:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT  PRIMARY KEY(" + pk1.ToUpper() + "," + pk2.ToUpper() + ") CONSTRAINT " + tab + "pk ";
                    break;
                default:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT " + tab + "pk  PRIMARY KEY(" + pk1.ToUpper() + "," + pk2.ToUpper() + ")";
                    break;
            }
            DBAccess.RunSQL(sql);
        }
        public static void CreatePK(string tab, string pk1, string pk2, string pk3, DBType db)
        {
            if (tab == null || tab == "")
                return;

            if (DBAccess.IsExitsTabPK(tab) == true)
                return;

            string sql;
            switch (db)
            {
                case DBType.Informix:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT  PRIMARY KEY(" + pk1.ToUpper() + "," + pk2.ToUpper() + "," + pk3.ToUpper() + ") CONSTRAINT " + tab + "pk ";
                    break;
                default:
                    sql = "ALTER TABLE " + tab.ToUpper() + " ADD CONSTRAINT " + tab + "pk PRIMARY KEY(" + pk1.ToUpper() + "," + pk2.ToUpper() + "," + pk3.ToUpper() + ")";
                    break;
            }
            DBAccess.RunSQL(sql);
        }
        #endregion


        #region index
        public static void CreatIndex(DBUrlType mydburl, string table, string pk)
        {
            string idxName = table + "ID";
            if (BP.DA.DBAccess.IsExitsObject(idxName) == true)
                return;

            //string sql = "";
            //try
            //{
            //    sql = "DROP INDEX " + table + "ID ON " + table;
            //    DBAccess.RunSQL(mydburl, sql);
            //}
            //catch
            //{
            //}

            //try
            //{
            //    sql = "CREATE INDEX " + table + "ID ON " + table + " (" + pk + ")";
            //    DBAccess.RunSQL(mydburl, sql);
            //}
            //catch
            //{
            //}
        }
        public static void CreatIndex(DBUrlType mydburl, string table, string pk1, string pk2)
        {

            //try
            //{
            //    DBAccess.RunSQL(mydburl, "CREATE INDEX " + table + "ID ON " + table + " (" + pk1 + "," + pk2 + ")");
            //}
            //catch
            //{
            //}
        }
        public static void CreatIndex(DBUrlType mydburl, string table, string pk1, string pk2, string pk3)
        {
            //  DBAccess.RunSQL(mydburl, "CREATE INDEX " + table + "ID ON " + table + " (" + pk1 + "," + pk2 + "," + pk3 + ")");
        }
        public static void CreatIndex(DBUrlType mydburl, string table, string pk1, string pk2, string pk3, string pk4)
        {
            //DBAccess.RunSQL(mydburl, "CREATE INDEX " + table + "ID ON " + table + " (" + pk1 + "," + pk2 + "," + pk3 + "," + pk4 + ")");
        }
        #endregion

        public static int CreatTableFromODBC(string selectSQL, string table, string pk)
        {
            DBAccess.RunSQLDropTable(table);
            string sql = "SELECT * INTO " + table + " FROM OPENROWSET('MSDASQL','" + SystemConfig.AppSettings["DBAccessOfODBC"] + "','" + selectSQL + "')";
            int i = DBAccess.RunSQL(sql);
            DBAccess.RunSQL("CREATE INDEX " + table + "ID ON " + table + " (" + pk + ")");
            return i;
        }
        public static int CreatTableFromODBC(string selectSQL, string table, string pk1, string pk2)
        {
            DBAccess.RunSQLDropTable(table);
            //DBAccess.RunSQL("DROP TABLE "+table);
            string sql = "SELECT * INTO " + table + " FROM OPENROWSET('MSDASQL','" + SystemConfig.AppSettings["DBAccessOfODBC"] + "','" + selectSQL + "')";
            int i = DBAccess.RunSQL(sql);
            DBAccess.RunSQL("CREATE INDEX " + table + "ID ON " + table + " (" + pk1 + "," + pk2 + ")");
            return i;
        }
        public static int CreatTableFromODBC(string selectSQL, string table, string pk1, string pk2, string pk3)
        {
            DBAccess.RunSQLDropTable(table);
            string sql = "SELECT * INTO " + table + " FROM OPENROWSET('MSDASQL','" + SystemConfig.AppSettings["DBAccessOfODBC"] + "','" + selectSQL + "')";
            int i = DBAccess.RunSQL(sql);
            DBAccess.RunSQL("CREATE INDEX " + table + "ID ON " + table + " (" + pk1 + "," + pk2 + "," + pk3 + ")");
            return i;
        }
        #endregion

        #region 在当前的Connection执行 SQL 语句，返回受影响的行数
        public static int RunSQL(string sql, CommandType sqlType, string dsn, params object[] pars)
        {
            IDbConnection oconn = GetAppCenterDBConn;
            if (oconn is SqlConnection)
                return RunSQL(sql, (SqlConnection)oconn, sqlType, dsn);
            else if (oconn is OracleConnection)
                return RunSQL(sql, (OracleConnection)oconn, sqlType, dsn);
            else
                throw new Exception("获取数据库连接[GetAppCenterDBConn]失败！");
        }
        public static DataTable ReadProText(string proName)
        {
            string sql = "";
            switch (BP.Sys.SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT text FROM user_source WHERE name=UPPER('" + proName + "') ORDER BY LINE ";
                    break;
                default:
                    sql = "SP_Help  " + proName;
                    break;
            }
            try
            {
                return BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
            catch
            {
                sql = "select * from Port_Emp WHERE 1=2";
                return BP.DA.DBAccess.RunSQLReturnTable(sql);
            }
        }
        public static void RunSQLScript(string sqlOfScriptFilePath)
        {
            string str = BP.DA.DataType.ReadTextFile(sqlOfScriptFilePath);
            string[] strs = str.Split(';');
            foreach (string s in strs)
            {
                if (DataType.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                    continue;

                if (s.Contains("--"))
                    continue;

                if (s.Contains("/*"))
                    continue;

                BP.DA.DBAccess.RunSQL(s);
            }
        }
        /// <summary>
        /// 执行具有Go的sql 文本。
        /// </summary>
        /// <param name="sqlOfScriptFilePath"></param>
        public static void RunSQLScriptGo(string sqlOfScriptFilePath)
        {
            string str = BP.DA.DataType.ReadTextFile(sqlOfScriptFilePath);
            string[] strs = str.Split(new String[] { "--GO--" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in strs)
            {
                if (DataType.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s))
                    continue;

                if (s.Contains("/**"))
                    continue;

                string mysql = s.Replace("--GO--", "");
                if (DataType.IsNullOrEmpty(mysql.Trim()))
                    continue;
                if (s.Contains("--"))
                    continue;

                BP.DA.DBAccess.RunSQL(mysql);
            }
        }
        /// <summary>
        /// 运行SQLs
        /// </summary>
        /// <param name="sql"></param>
        public static void RunSQLs(string sql)
        {
            if (DataType.IsNullOrEmpty(sql))
                return;

            sql = sql.Replace("@GO", "~");
            sql = sql.Replace("@", "~");
            string[] strs = sql.Split('~');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;

                if (str.Contains("--") || str.Contains("/*"))
                    continue;

                RunSQL(str);
            }
        }
        /// <summary>
        /// 运行带有参数的sql
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static int RunSQL(Paras ps)
        {
            return RunSQL(ps.SQL, ps);
        }
        /// <summary>
        /// 运行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int RunSQL(string sql)
        {
            if (sql == null || sql.Trim() == "")
                return 1;
            Paras ps = new Paras();
            ps.SQL = sql;
            return RunSQL(ps);
        }
        public static int RunSQL(DBUrlType dburl, string sql)
        {
            if (sql == null || sql.Trim() == "")
                return 1;
            Paras ps = new Paras();
            ps.SQL = sql;

            switch (dburl)
            {
                case DBUrlType.AppCenterDSN:
                    return RunSQL(ps);
                case DBUrlType.DBAccessOfMSSQL1:
                    return DBAccessOfMSSQL1.RunSQL(sql);
                case DBUrlType.DBAccessOfMSSQL2:
                    return DBAccessOfMSSQL2.RunSQL(sql);
                case DBUrlType.DBAccessOfOracle1:
                    return DBAccessOfOracle1.RunSQL(sql);
                case DBUrlType.DBAccessOfOracle2:
                    return DBAccessOfOracle2.RunSQL(sql);
                default:
                    throw new Exception("@没有判断的类型" + dburl.ToString());
            }
        }
        public static int RunSQL(string sql, string paraKey, object val)
        {
            Paras ens = new Paras();
            ens.Add(paraKey, val);
            return RunSQL(sql, ens);
        }
        public static int RunSQL(string sql, string paraKey1, object val1, string paraKey2, object val2)
        {
            Paras ens = new Paras();
            ens.Add(paraKey1, val1);
            ens.Add(paraKey2, val2);
            return RunSQL(sql, ens);
        }
        public static int RunSQL(string sql, string paraKey1, object val1, string paraKey2, object val2, string k3, object v3)
        {
            Paras ens = new Paras();
            ens.Add(paraKey1, val1);
            ens.Add(paraKey2, val2);
            ens.Add(k3, v3);
            return RunSQL(sql, ens);
        }
        /// <summary>
        /// 
        /// </summary>
        public static bool lockRunSQL = false;
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int RunSQL(string sql, Paras paras)
        {
            if (sql == null || sql.Trim() == "")
                return 1;
                
            while (lockRunSQL == true)
            {
            };
            lockRunSQL = true;

            int result = 0;
            try
            {
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        result = RunSQL_200705_SQL(sql, paras);
                        break;
                    case DBType.Oracle:
                        result = RunSQL_200705_Ora(sql.Replace("]", "").Replace("[", ""), paras);
                        break;
                    case DBType.MySQL:
                        result = RunSQL_200705_MySQL(sql, paras);
                        break;
                    case DBType.Informix:
                        result = RunSQL_201205_Informix(sql, paras);
                        break;
                    case DBType.Access:
                        result = RunSQL_200705_OLE(sql, paras);
                        break;
                    default:
                        throw new Exception("发现未知的数据库连接类型！");
                }
                lockRunSQL = false;
                return result;
            }
            catch (Exception ex)
            {
                lockRunSQL = false;
                string msg = "";
                string mysql = sql.Clone() as string;

                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                foreach (Para p in paras)
                {
                    msg += "@" + p.ParaName + "=" + p.val + "," + p.DAType.ToString();

                    string str = mysql;

                    mysql = mysql.Replace(dbstr + p.ParaName + ",", "'" + p.val + "',");

                    // add by qin 16/3/22  
                    if (str == mysql)//表明类似":OID"的字段没被替换
                        mysql = mysql.Replace(dbstr + p.ParaName, "'" + p.val + "'");

                }
                throw new Exception("执行sql错误:" + ex.Message + " Paras(" + paras.Count + ")=" + msg + "<hr>" + mysql);
            }
        }

        /// <summary>
        /// 运行sql返回结果
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="paras">参数</param>
        /// <returns>执行的结果</returns>
        private static int RunSQL_200705_SQL(string sql, Paras paras)
        {
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = SystemConfig.AppCenterDSN;
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.CommandType = CommandType.Text;

            try
            {
                //if (SystemConfig.IsEnableNull )
                //{
                //    /*如果数值类型的允许为null, 就要特殊的判断. */
                //    foreach (Para para in paras)
                //    {
                //        switch (para.DAType)
                //        {
                //            case DbType.Int32:
                //            case DbType.Decimal: 
                //            case DbType.Double:
                //                if (para.val ==)
                //                {
                //                    SqlParameter oraP1 = new SqlParameter(para.ParaName, DBNull.Value);
                //                    cmd.Parameters.Add(oraP1);
                //                    continue;
                //                }
                //                break;
                //            default:
                //                break;
                //        }
                //        SqlParameter op = new SqlParameter(para.ParaName, para.val);
                //        cmd.Parameters.Add(op);
                //    }
                //}
                //else
                //{
                foreach (Para para in paras)
                {
                    SqlParameter oraP = new SqlParameter(para.ParaName, para.val);
                    cmd.Parameters.Add(oraP);
                }
                // }

                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                return i;
            }
            catch (System.Exception ex)
            {
                cmd.Dispose();
                conn.Close();

                paras.SQL = sql;
                string msg = "";
                if (paras.Count == 0)
                    msg = "SQL=" + sql + ",异常信息:" + ex.Message;
                else
                    msg = "SQL=" + paras.SQLNoPara + ",异常信息:" + ex.Message;

                Log.DefaultLogWriteLineInfo(msg);
                throw new Exception(msg);
            }
            finally
            {
                cmd.Dispose();
                conn.Close();
            }
        }
        /// <summary>
        /// 运行sql
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>执行结果</returns>
        private static int RunSQL_200705_MySQL(string sql)
        {
            return RunSQL_200705_MySQL(sql, new Paras());
        }

        #region 处理mysql conn的缓存.
        private static Hashtable _ConnHTOfMySQL = null;
        public static Hashtable ConnHTOfMySQL
        {
            get
            {
                if (_ConnHTOfMySQL == null || _ConnHTOfMySQL.Count <=0 )
                {
                    _ConnHTOfMySQL = new Hashtable();
                    int numConn = 10;
                    for (int i = 0; i < numConn; i++)
                    {
                        MySqlConnection conn = new MySqlConnection(SystemConfig.AppCenterDSN);
                        conn.Open(); //打开连接.
                        _ConnHTOfMySQL.Add("Conn" + i, conn);
                    }
                }
                return _ConnHTOfMySQL;
            }
        }
        public static MySqlConnection GetOneMySQLConn
        {
            get
            {
                foreach (MySqlConnection conn in _ConnHTOfMySQL)
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                        return conn;
                    }
                }
                return null;
                //foreach (MySqlConnection conn in _ConnHTOfMySQL)
                //{
                //}
            }
        }
        #endregion 处理mysql conn的缓存.

        /// <summary>
        /// RunSQL_200705_MySQL
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        private static int RunSQL_200705_MySQL(string sql, Paras paras)
        {
             
            MySqlConnection connOfMySQL = new MySqlConnection(SystemConfig.AppCenterDSN);
            if (connOfMySQL.State != System.Data.ConnectionState.Open)
            {
                connOfMySQL.ConnectionString = SystemConfig.AppCenterDSN;
                connOfMySQL.Open();
            }

            int i = 0;
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql, connOfMySQL);
                cmd.CommandType = CommandType.Text;
                foreach (Para para in paras)
                {
                    MySqlParameter oraP = new MySqlParameter(para.ParaName, para.val);
                    cmd.Parameters.Add(oraP);
                }
                i = cmd.ExecuteNonQuery();
                cmd.Dispose();

                connOfMySQL.Close();
             //   connOfMySQL.Dispose();
                return i;
            }
            catch (System.Exception ex)
            {
                connOfMySQL.Close();
                connOfMySQL.Dispose();
                throw new Exception(ex.Message + "@SQL:" + sql);
            }
        }
        private static int RunSQL_200705_Ora(string sql, Paras paras)
        {
            OracleConnection conn = new OracleConnection(SystemConfig.AppCenterDSN);
            try
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = SystemConfig.AppCenterDSN;
                    conn.Open();
                }

                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                foreach (Para para in paras)
                {
                    OracleParameter oraP = new OracleParameter(para.ParaName, para.DATypeOfOra);
                    oraP.Size = para.Size;

                    if (para.DATypeOfOra == OracleType.Clob)
                    {
                        if (DataType.IsNullOrEmpty(para.val as string) == true)
                            oraP.Value = DBNull.Value;
                        else
                            oraP.Value = para.val;
                    }
                    else
                        oraP.Value = para.val;

                    cmd.Parameters.Add(oraP);
                }
                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close(); //把它关闭.
                return i;
            }
            catch (System.Exception ex)
            {
                conn.Close(); //把它关闭.


                if (paras != null)
                {
                    foreach (Para item in paras)
                    {
                        if (item.DAType == DbType.String)
                        {
                            if (sql.Contains(":" + item.ParaName + ","))
                                sql = sql.Replace(":" + item.ParaName + ",", "'" + item.val + "',");
                            else
                                sql = sql.Replace(":" + item.ParaName, "'" + item.val + "'");
                        }
                        else
                        {
                            if (sql.Contains(":" + item.ParaName + ","))
                                sql = sql.Replace(":" + item.ParaName + ",", item.val + ",");
                            else
                                sql = sql.Replace(":" + item.ParaName, item.val.ToString());
                        }
                    }
                }

                if (BP.Sys.SystemConfig.IsDebug)
                {
                    string msg = "RunSQL2   SQL=" + sql + ex.Message;
                    //Log.DebugWriteError(msg);

                    throw new Exception("err@" + ex.Message + " SQL=" + sql);
                }
                else
                {
                    //    Log.DebugWriteError(ex.Message);
                    throw new Exception(ex.Message + "@可以执行的SQL:" + sql);
                }
            }
            finally
            {
                conn.Close();
            }
        }
        private static int RunSQL_200705_OLE(string sql, Paras para)
        {
            OleDbConnection conn = new OleDbConnection(SystemConfig.AppCenterDSN); // connofora.Conn;
            try
            {
                if (conn == null)
                    conn = new OleDbConnection(SystemConfig.AppCenterDSN);

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                foreach (Para mypara in para)
                {
                    OleDbParameter oraP = new OleDbParameter(mypara.ParaName, mypara.val);
                    cmd.Parameters.Add(oraP);
                }

                int i = cmd.ExecuteNonQuery();
                conn.Close();
                return i;
            }
            catch (System.Exception ex)
            {
                conn.Close();
                string msg = "RunSQL_200705_OLE   SQL=" + sql + ex.Message;
                Log.DebugWriteError(msg);
                throw new Exception(msg);
            }
            finally
            {
                conn.Close();
            }
        }
        private static int RunSQL_200705_OLE(string sql)
        {
            Paras ps = new Paras();
            return RunSQL_200705_OLE(sql, ps);
        }
        /// <summary>
        /// 运行sql
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>执行结果</returns>
        private static int RunSQL_201205_Informix(string sql)
        {
            return RunSQL_201205_Informix(sql, new Paras());
        }
        private static int RunSQL_201205_Informix(string sql, Paras paras)
        {
            if (paras.Count != 0)
                sql = DealInformixSQL(sql);

            IfxConnection conn = new IfxConnection(SystemConfig.AppCenterDSN);
            try
            {
                if (conn == null)
                    conn = new IfxConnection(SystemConfig.AppCenterDSN);

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = SystemConfig.AppCenterDSN;
                    conn.Open();
                }

                IfxCommand cmd = new IfxCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                foreach (Para para in paras)
                {
                    IfxParameter oraP = new IfxParameter(para.ParaName, para.val);
                    cmd.Parameters.Add(oraP);
                }

                int i = cmd.ExecuteNonQuery();
                cmd.Dispose();
                conn.Close();
                return i;
            }
            catch (System.Exception ex)
            {
                conn.Close();
                string msg = "RunSQL2   SQL=" + sql + "\r\n Message=: " + ex.Message;
                Log.DebugWriteError(msg);
                throw new Exception(msg);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #endregion

        #region 运行SQL 返回 DataTable
        #region 在指定的 Connection 上执行

        #region SqlConnection
        /// <summary>
        /// 锁
        /// </summary>
        private static bool lock_msSQL_ReturnTable = false;
        public static DataTable RunSQLReturnTable(string oraSQL, OracleConnection conn, CommandType sqlType, string dsn)
        {

#if DEBUG
            //Debug.WriteLine( oraSQL );
#endif


            try
            {
                if (conn == null)
                {
                    conn = new OracleConnection(dsn);
                    conn.Open();
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.ConnectionString = dsn;
                    conn.Open();
                }

                OracleDataAdapter oraAda = new OracleDataAdapter(oraSQL, conn);
                oraAda.SelectCommand.CommandType = sqlType;


                DataTable oratb = new DataTable("otb");
                oraAda.Fill(oratb);

                // peng add 07-19
                oraAda.Dispose();

                return oratb;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + " [RunSQLReturnTable on OracleConnection dsn=App ] sql=" + oraSQL + "<br>");
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msSQL"></param>
        /// <param name="sqlconn"></param>
        /// <param name="sqlType"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static DataTable RunSQLReturnTable(string msSQL, SqlConnection conn, string connStr, CommandType sqlType, params object[] pars)
        {
            string msg = "step1";

            if (conn.State == ConnectionState.Closed)
            {
                conn.ConnectionString = connStr;
                conn.Open();
            }

#if DEBUG
            Debug.WriteLine(msSQL);
#endif

            while (lock_msSQL_ReturnTable)
                ;

            SqlDataAdapter msAda = new SqlDataAdapter(msSQL, conn);
            msg = "error 2";
            msAda.SelectCommand.CommandType = sqlType;
            if (pars != null)
            {
                //CommandType.
                foreach (object par in pars)
                {
                    msAda.SelectCommand.Parameters.AddWithValue("par", par);
                }
            }

            DataTable mstb = new DataTable("mstb");
            //如果是锁定状态，就等待
            lock_msSQL_ReturnTable = true; //锁定
            try
            {
                msg = "error 3";
                try
                {
                    msg = "4";
                    msAda.Fill(mstb);
                }
                catch (Exception ex)
                {
                    msg = "5";
                    lock_msSQL_ReturnTable = false;
                    conn.Close();
                    throw new Exception(ex.Message + " msg=" + msg + " Run@DBAccess");
                }
                msg = "10";
                msAda.Dispose();
                msg = "11";
                //				if (SystemConfig.IsBSsystem==false )
                //				{
                //					msg="13";
                //					sqlconn.Close();
                //				}
                msg = "14";
                lock_msSQL_ReturnTable = false;// 返回前一定要开锁
                conn.Close();
            }
            catch (System.Exception ex)
            {
                lock_msSQL_ReturnTable = false;
                conn.Close();
                throw new Exception("[RunSQLReturnTable on SqlConnection 1] step = " + msg + "<BR>" + ex.Message + " sql=" + msSQL);
            }
            return mstb;
        }
        #endregion

        #region OleDbConnection
        /// <summary>
        /// 锁
        /// </summary>
        private static bool lock_oleSQL_ReturnTable = false;
        /// <summary>
        /// 运行sql 返回Table
        /// </summary>
        /// <param name="oleSQL">oleSQL</param>
        /// <param name="oleconn">连接</param>
        /// <param name="sqlType">类型</param>
        /// <param name="pars">参数</param>
        /// <returns>执行SQL返回的DataTable</returns>
        public static DataTable RunSQLReturnTable(string oleSQL, OleDbConnection oleconn, CommandType sqlType, params object[] pars)
        {
#if DEBUG
            Debug.WriteLine(oleSQL);
#endif


            while (lock_oleSQL_ReturnTable)
            {
                ;
            }  //如果是锁定状态，就等待
            lock_oleSQL_ReturnTable = true; //锁定
            string msg = "step1";
            try
            {
                OleDbDataAdapter msAda = new OleDbDataAdapter(oleSQL, oleconn);
                msg += "2";
                msAda.SelectCommand.CommandType = sqlType;
                foreach (object par in pars)
                {
                    msAda.SelectCommand.Parameters.AddWithValue("par", par);
                }
                DataTable mstb = new DataTable("mstb");
                msg += "3";
                msAda.Fill(mstb);
                msg += "4";
                // peng add 2004-07-19 .
                msAda.Dispose();
                msg += "5";
                if (SystemConfig.IsBSsystem_Test == false)
                {
                    msg += "6";
                    oleconn.Close();
                }
                msg += "7";
                lock_oleSQL_ReturnTable = false;//返回前一定要开锁
                return mstb;
            }
            catch (System.Exception ex)
            {
                lock_oleSQL_ReturnTable = false;//返回前一定要开锁
                throw new Exception("[RunSQLReturnTable on OleDbConnection] error  请把错误交给 peng . step = " + msg + "<BR>" + oleSQL + " ex=" + ex.Message);
            }
            finally
            {
                oleconn.Close();
            }
        }
        /// <summary>
        /// 运行sql 返回Table
        /// </summary>
        /// <param name="oleSQL">要运行的sql</param>
        /// <param name="sqlconn">OleDbConnection</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSQLReturnTable(string oleSQL, OleDbConnection sqlconn)
        {
            return RunSQLReturnTable(oleSQL, sqlconn, CommandType.Text);
        }
        #endregion

        #region OracleConnection
        private static DataTable RunSQLReturnTable_200705_Ora(string selectSQL, Paras paras)
        {
            OracleConnection conn = new OracleConnection(SystemConfig.AppCenterDSN);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                OracleDataAdapter ada = new OracleDataAdapter(selectSQL, conn);
                ada.SelectCommand.CommandType = CommandType.Text;

                // 加入参数
                if (paras != null)
                {
                    foreach (Para para in paras)
                    {
                        OracleParameter myParameter = new OracleParameter(para.ParaName, para.DATypeOfOra);
                        myParameter.Size = para.Size;
                        myParameter.Value = para.val;
                        ada.SelectCommand.Parameters.Add(myParameter);
                    }
                }

                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();
                conn.Close();
                return oratb;
            }
            catch (System.Exception ex)
            {
                conn.Close();
                string msg = "@运行查询在(RunSQLReturnTable_200705_Ora with paras)出错 sql=" + selectSQL + " @异常信息：" + ex.Message;
                msg += "@Para Num= " + paras.Count;
                foreach (Para pa in paras)
                {
                    msg += "@" + pa.ParaName + "=" + pa.val;
                }
                Log.DebugWriteError(msg);
                throw new Exception(msg);
            }
            finally
            {
                conn.Close();
            }
        }
        private static DataTable RunSQLReturnTable_200705_SQL(string selectSQL)
        {
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlDataAdapter ada = new SqlDataAdapter(selectSQL, conn);
                ada.SelectCommand.CommandType = CommandType.Text;
                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();
                return oratb;
            }
            catch (System.Exception ex)
            {
                string msgErr = ex.Message;
                string msg = "@运行查询在(RunSQLReturnTable_200705_SQL)出错 sql=" + selectSQL + " @异常信息：" + msgErr;
                Log.DebugWriteError(msg);
                throw new Exception(msg);
            }
        }

        private static DataTable RunSQLReturnTable_200705_SQL(string sql, Paras paras)
        {
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != ConnectionState.Open)
                conn.Open();

            SqlDataAdapter ada = new SqlDataAdapter(sql, conn);
            ada.SelectCommand.CommandType = CommandType.Text;

            // 加入参数
            if (paras != null)//qin 解决为null时的异常
            {
                foreach (Para para in paras)
                {
                    SqlParameter myParameter = new SqlParameter(para.ParaName, para.val);
                    myParameter.Size = para.Size;
                    ada.SelectCommand.Parameters.Add(myParameter);
                }
            }

            try
            {
                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();
                conn.Close();
                return oratb;
            }
            catch (Exception ex)
            {
                ada.Dispose();
                conn.Close();
                throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
            }
        }
        private static string DealInformixSQL(string sql)
        {
            if (sql.Contains("?") == false)
                return sql;

            string mysql = "";
            if (sql.Contains("? ") == true || sql.Contains("?,") == true)
            {
                /*如果有空格,说明已经替换过了。*/
                return sql;
            }
            else
            {
                sql += " ";
                /*说明需要处理的变量.*/
                string[] strs = sql.Split('?');
                mysql = strs[0];
                for (int i = 1; i < strs.Length; i++)
                {
                    string str = strs[i];
                    switch (str.Substring(0, 1))
                    {
                        case " ":
                            mysql += "?" + str;
                            break;
                        case ")":
                            mysql += "?" + str;
                            break;
                        case ",":
                            mysql += "?" + str;
                            break;
                        default:
                            char[] chs = str.ToCharArray();
                            foreach (char c in chs)
                            {
                                if (c == ',')
                                {
                                    int idx1 = str.IndexOf(",");
                                    mysql += "?" + str.Substring(idx1);
                                    break;
                                }

                                if (c == ')')
                                {
                                    int idx1 = str.IndexOf(")");
                                    mysql += "?" + str.Substring(idx1);
                                    break;
                                }

                                if (c == ' ')
                                {
                                    int idx1 = str.IndexOf(" ");
                                    mysql += "?" + str.Substring(idx1);
                                    break;
                                }
                            }

                            //else
                            //{
                            //    mysql += "?" + str;
                            //}

                            //if (str.Contains(")") == true)
                            //    mysql += "?" + str.Substring(str.IndexOf(")"));
                            //else
                            //    mysql += "?" + str;
                            break;
                    }
                }
            }
            return mysql;
        }
        /// <summary>
        /// RunSQLReturnTable_200705_Informix
        /// </summary>
        /// <param name="selectSQL">要执行的sql</param>
        /// <returns>返回table</returns>
        private static DataTable RunSQLReturnTable_201205_Informix(string sql, Paras paras)
        {
            //if (paras.Count != 0 && sql.Contains("?") == false)
            //{
            //    sql = DealInformixSQL(sql);
            //}
            sql = DealInformixSQL(sql);

            IfxConnection conn = new IfxConnection(SystemConfig.AppCenterDSN);
            if (conn.State != ConnectionState.Open)
                conn.Open();

            IfxDataAdapter ada = new IfxDataAdapter(sql, conn);
            ada.SelectCommand.CommandType = CommandType.Text;

            // 加入参数
            foreach (Para para in paras)
            {
                IfxParameter myParameter = new IfxParameter(para.ParaName, para.val);
                myParameter.Size = para.Size;
                ada.SelectCommand.Parameters.Add(myParameter);
            }

            try
            {
                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();
                conn.Close();
                return oratb;
            }
            catch (Exception ex)
            {
                ada.Dispose();
                conn.Close();
                Log.DefaultLogWriteLineError(sql);
                Log.DefaultLogWriteLineError(ex.Message);

                throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
            }
            finally
            {
                ada.Dispose();
                conn.Close();
            }
        }
        /// <summary>
        /// RunSQLReturnTable_200705_SQL
        /// </summary>
        /// <param name="selectSQL">要执行的sql</param>
        /// <returns>返回table</returns>
        private static DataTable RunSQLReturnTable_200705_MySQL(string selectSQL)
        {
            return RunSQLReturnTable_200705_MySQL(selectSQL, new Paras());
        }
        /// <summary>
        /// RunSQLReturnTable_200705_SQL
        /// </summary>
        /// <param name="selectSQL">要执行的sql</param>
        /// <returns>返回table</returns>
        private static DataTable RunSQLReturnTable_200705_MySQL(string sql, Paras paras)
        {
            //  string mcs = "Data Source=127.0.0.1;User ID=root;Password=root;DataBase=wk;Charset=gb2312;";
            //  MySqlConnection conn = new MySqlConnection(SystemConfig.AppCenterDSN);
            //  SqlDataAdapter ad = new SqlDataAdapter("select username,password from person", conn);
            //  DataTable dt = new DataTable();
            //  conn.Open();
            //  ad.Fill(dt);
            //  conn.Close();
            //  return dt;

            using (MySqlConnection conn = new MySqlConnection(SystemConfig.AppCenterDSN))
            {
                using (MySqlDataAdapter ada = new MySqlDataAdapter(sql, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    ada.SelectCommand.CommandType = CommandType.Text;

                    // 加入参数
                    foreach (Para para in paras)
                    {
                        MySqlParameter myParameter = new MySqlParameter(para.ParaName, para.val);
                        myParameter.Size = para.Size;
                        ada.SelectCommand.Parameters.Add(myParameter);
                    }

                    try
                    {
                        DataTable oratb = new DataTable("otb");
                        ada.Fill(oratb);
                        return oratb;
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        throw new Exception("SQL=" + sql + " Exception=" + ex.Message);
                    }
                }
            }
        }
        /// <summary>
        /// RunSQLReturnTable_200705_SQL
        /// </summary>
        /// <param name="selectSQL">要执行的sql</param>
        /// <returns>返回table</returns>
        private static DataTable RunSQLReturnTable_201205_Informix(string selectSQL)
        {
            return RunSQLReturnTable_201205_Informix(selectSQL, new Paras());
        }
        /// <summary>
        /// RunSQLReturnTable_200705_SQL
        /// </summary>
        /// <param name="selectSQL">要执行的sql</param>
        /// <returns>返回table</returns>
        private static DataTable RunSQLReturnTable_200705_OLE(string selectSQL, Paras paras)
        {
            OleDbConnection conn = new OleDbConnection(SystemConfig.AppCenterDSN);
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                OleDbDataAdapter ada = new OleDbDataAdapter(selectSQL, conn);
                ada.SelectCommand.CommandType = CommandType.Text;

                // 加入参数
                foreach (Para para in paras)
                {
                    OleDbParameter myParameter = new OleDbParameter(para.ParaName, para.DATypeOfOra);
                    myParameter.Size = para.Size;
                    myParameter.Value = para.val;
                    ada.SelectCommand.Parameters.Add(myParameter);
                }

                DataTable oratb = new DataTable("otb");
                ada.Fill(oratb);
                ada.Dispose();

                conn.Close();

                return oratb;
            }
            catch (System.Exception ex)
            {
                conn.Close();
                string msg = "@RunSQLReturnTable_200705_OLE with paras) Error sql=" + selectSQL + " @Messages：" + ex.Message;
                msg += "@Para Num= " + paras.Count;
                foreach (Para pa in paras)
                {
                    msg += "@" + pa.ParaName + "=" + pa.val + " type=" + pa.DAType.ToString();
                }
                Log.DebugWriteError(msg);
                throw new Exception(msg);
            }
            finally
            {
                conn.Close();
            }
        }
        #endregion

        #endregion

        #region 在当前Connection上执行
        public static DataTable RunSQLReturnTable(Paras ps)
        {
            return RunSQLReturnTable(ps.SQL, ps);
        }
        public static int RunSQLReturnTableCount = 0;
        /// <summary>
        /// 传递一个select 语句返回一个查询结果集合。
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <returns>查询结果集合DataTable</returns>
        public static DataTable RunSQLReturnTable(string sql)
        {
            Paras ps = new Paras();
            return RunSQLReturnTable(sql, ps);
        }
        public static DataTable RunSQLReturnTable(string sql, string key1, object v1, string key2, object v2)
        {
            Paras ens = new Paras();
            ens.Add(key1, v1);
            ens.Add(key2, v2);
            return RunSQLReturnTable(sql, ens);
        }
        public static DataTable RunSQLReturnTable(string sql, string key, object val)
        {
            Paras ens = new Paras();
            ens.Add(key, val);
            return RunSQLReturnTable(sql, ens);
        }

        /// <summary>
        /// 通用SQL查询分页返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句，不带排序（Order By）语句</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <param name="pageIdx">请求页码</param>
        /// <param name="key">记录主键（不能为空，不能有重复，必须包含在返回字段中）</param>
        /// <param name="orderKey">排序字段（此字段必须包含在返回字段中）</param>
        /// <param name="orderType">排序方式，ASC/DESC</param>
        /// <returns></returns>
        public static DataTable RunSQLReturnTable(string sql, int pageSize, int pageIdx, string key, string orderKey, string orderType)
        {
            switch(DBAccess.AppCenterDBType)
            {
                case DBType.MSSQL:
                    return RunSQLReturnTable_201612_SQL(sql, pageSize, pageIdx, key, orderKey, orderType);
                case DBType.Oracle:
                    return RunSQLReturnTable_201612_Ora(sql, pageSize, pageIdx, orderKey, orderType);
                case DBType.MySQL:
                    return RunSQLReturnTable_201612_MySql(sql, pageSize, pageIdx, key, orderKey, orderType);
                default:
                    throw new Exception("@未涉及的数据库类型！");
            }
        }

        /// <summary>
        /// 通用SqlServer查询分页返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句，不带排序（Order By）语句</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <param name="pageIdx">请求页码</param>
        /// <param name="key">记录主键（不能为空，不能有重复，必须包含在返回字段中）</param>
        /// <param name="orderKey">排序字段（此字段必须包含在返回字段中）</param>
        /// <param name="orderType">排序方式，ASC/DESC</param>
        /// <returns></returns>
        private static DataTable RunSQLReturnTable_201612_SQL(string sql, int pageSize, int pageIdx, string key, string orderKey, string orderType)
        {
            string sqlstr = string.Empty;

            orderType = string.IsNullOrWhiteSpace(orderType) ? "ASC" : orderType.ToUpper();

            if (pageIdx < 1)
                pageIdx = 1;

            if(pageIdx == 1)
            {
                sqlstr = "SELECT TOP " + pageSize + " * FROM (" + sql + ") T1" +
                         (string.IsNullOrWhiteSpace(orderKey)
                              ? string.Empty
                              : string.Format(" ORDER BY T1.{0} {1}", orderKey, orderType));
            }
            else
            {
                sqlstr = "SELECT TOP " + pageSize + " * FROM (" + sql + ") T1"
                         + " WHERE T1." + key + (orderType == "ASC" ? " > " : " < ") + "("
                         + " SELECT " + (orderType == "ASC" ? "MAX(T3." : "MIN(T3.") + key + ") FROM ("
                         + " SELECT TOP ((" + pageIdx + " - 1) * 10) T2." + key + "FROM (" + sql + ") T2"
                         + (string.IsNullOrWhiteSpace(orderKey)
                                ? string.Empty
                                : string.Format(" ORDER BY T2.{0} {1}", orderKey, orderType))
                         + " ) T3)"
                         + (string.IsNullOrWhiteSpace(orderKey)
                                ? string.Empty
                                : string.Format(" ORDER BY T.{0} {1}", orderKey, orderType));
            }

            return RunSQLReturnTable(sqlstr);
        }

        /// <summary>
        /// 通用Oracle查询分页返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句，不带排序（Order By）语句</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <param name="pageIdx">请求页码</param>
        /// <param name="orderKey">排序字段（此字段必须包含在返回字段中）</param>
        /// <param name="orderType">排序方式，ASC/DESC</param>
        /// <returns></returns>
        private static DataTable RunSQLReturnTable_201612_Ora(string sql, int pageSize, int pageIdx, string orderKey, string orderType)
        {
            if (pageIdx < 1)
                pageIdx = 1;

            int start = (pageIdx - 1)*pageSize + 1;
            int end = pageSize*pageIdx;

            orderType = string.IsNullOrWhiteSpace(orderType) ? "ASC" : orderType.ToUpper();
            
            string sqlstr = "SELECT * FROM ( SELECT T1.*, ROWNUM RN "
                            + "FROM (SELECT * FROM  (" + sql + ") T2 "
                            +
                            (string.IsNullOrWhiteSpace(orderType)
                                 ? string.Empty
                                 : string.Format("ORDER BY T2.{0} {1}", orderKey, orderType)) + ") T1 WHERE ROWNUM <= " +
                            end + " ) WHERE RN >=" + start;

            return RunSQLReturnTable(sqlstr);
        }

        /// <summary>
        /// 通用MySql查询分页返回DataTable
        /// </summary>
        /// <param name="sql">SQL语句，不带排序（Order By）语句</param>
        /// <param name="pageSize">每页记录数量</param>
        /// <param name="pageIdx">请求页码</param>
        /// <param name="key">记录主键（不能为空，不能有重复，必须包含在返回字段中）</param>
        /// <param name="orderKey">排序字段（此字段必须包含在返回字段中）</param>
        /// <param name="orderType">排序方式，ASC/DESC</param>
        /// <returns></returns>
        private static DataTable RunSQLReturnTable_201612_MySql(string sql, int pageSize, int pageIdx, string key, string orderKey, string orderType)
        {
            string sqlstr = string.Empty;
            orderType = string.IsNullOrWhiteSpace(orderType) ? "ASC" : orderType.ToUpper();

            if (pageIdx < 1)
                pageIdx = 1;

            sqlstr = "SELECT * FROM (" + sql + ") T1 WHERE T1." + key + (orderType == "ASC" ? " >= " : " <= ")
                     + "(SELECT T2." + key + " FROM (" + sql + ") T2"
                     + (string.IsNullOrWhiteSpace(orderKey)
                            ? string.Empty
                            : string.Format(" ORDER BY T2.{0} {1}", orderKey, orderType))
                     + " LIMIT " + ((pageIdx - 1)*pageSize) + ",1) LIMIT " + pageSize;

            return RunSQLReturnTable(sqlstr);
        }

        private static bool lockRunSQLReTable = false;
        /// <summary>
        /// 运行SQL
        /// </summary>
        /// <param name="sql">带有参数的SQL语句</param>
        /// <param name="paras">参数</param>
        /// <returns>返回执行结果</returns>
        public static DataTable RunSQLReturnTable(string sql, Paras paras)
        {
            if (DataType.IsNullOrEmpty(sql))
                throw new Exception("要执行的 sql = null ");

            try
            {
                DataTable dt = null;
                switch (AppCenterDBType)
                {
                    case DBType.MSSQL:
                        dt = RunSQLReturnTable_200705_SQL(sql, paras);
                        break;
                    case DBType.Oracle:
                        dt = RunSQLReturnTable_200705_Ora(sql, paras);
                        break;
                    case DBType.Informix:
                        dt = RunSQLReturnTable_201205_Informix(sql, paras);
                        break;
                    case DBType.MySQL:
                        dt = RunSQLReturnTable_200705_MySQL(sql, paras);
                        break;
                    case DBType.Access:
                        dt = RunSQLReturnTable_200705_OLE(sql, paras);
                        break;
                    default:
                        throw new Exception("@发现未知的数据库连接类型！");
                }
                return dt;
            }
            catch (Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
                throw ex;
            }
        }
        #endregion 在当前Connection上执行

        public static DataTable  ToUpper(DataTable dt)
        {
            if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Oracle)
                return dt;

            foreach (DataColumn dc in dt.Columns)
                dc.ColumnName = dc.ColumnName.ToUpper();
            return dt;

            return dt;
        }
        #endregion



        #region 查询单个值的方法.

        #region OleDbConnection
        public static float RunSQLReturnValFloat(Paras ps)
        {
            return RunSQLReturnValFloat(ps.SQL, ps, 0);
        }
        public static float RunSQLReturnValFloat(string sql, Paras ps, float val)
        {
            ps.SQL = sql;
            object obj = DA.DBAccess.RunSQLReturnVal(ps);

            try
            {
                if (obj == null || obj.ToString() == "")
                    return val;
                else
                    return float.Parse(obj.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + sql + " @OBJ=" + obj);
            }
        }
        /// <summary>
        /// 运行sql返回float
        /// </summary>
        /// <param name="sql">要执行的sql,返回一行一列.</param>
        /// <param name="isNullAsVal">如果是空值就返回的默认值</param>
        /// <returns>float的返回值</returns>
        public static float RunSQLReturnValFloat(string sql, float isNullAsVal)
        {
            return RunSQLReturnValFloat(sql, new Paras(), isNullAsVal);
        }
        /// <summary>
        /// sdfsd
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static float RunSQLReturnValFloat(string sql)
        {
            try
            {
                return float.Parse(DA.DBAccess.RunSQLReturnVal(sql).ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }
        public static int RunSQLReturnValInt(Paras ps, int IsNullReturnVal)
        {
            return RunSQLReturnValInt(ps.SQL, ps, IsNullReturnVal);
        }
        /// <summary>
        /// sdfsd
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="IsNullReturnVal"></param>
        /// <returns></returns>
        public static int RunSQLReturnValInt(string sql, int IsNullReturnVal)
        {
            object obj = "";
            obj = DA.DBAccess.RunSQLReturnVal(sql);
            if (obj == null || obj.ToString() == "" || obj == DBNull.Value)
                return IsNullReturnVal;
            else
                return Convert.ToInt32(obj);
        }
        public static int RunSQLReturnValInt(string sql, int IsNullReturnVal, Paras paras)
        {
            object obj = "";

            obj = DA.DBAccess.RunSQLReturnVal(sql, paras);
            if (obj == null || obj.ToString() == "")
                return IsNullReturnVal;
            else
                return Convert.ToInt32(obj);
        }
        public static decimal RunSQLReturnValDecimal(string sql, decimal IsNullReturnVal, int blws)
        {
            Paras ps = new Paras();
            ps.SQL = sql;
            return RunSQLReturnValDecimal(ps, IsNullReturnVal, blws);
        }
        public static decimal RunSQLReturnValDecimal(Paras ps, decimal IsNullReturnVal, int blws)
        {
            try
            {
                object obj = DA.DBAccess.RunSQLReturnVal(ps);
                if (obj == null || obj.ToString() == "")
                    return IsNullReturnVal;
                else
                {
                    decimal d = decimal.Parse(obj.ToString());
                    return decimal.Round(d, blws);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + ps.SQL);
            }
        }
        public static int RunSQLReturnValInt(Paras ps)
        {
            string str = DBAccess.RunSQLReturnString(ps.SQL, ps);
            if (str.Contains("."))
                str = str.Substring(0, str.IndexOf("."));
            try
            {
                return Convert.ToInt32(str);
            }
            catch (Exception ex)
            {
                throw new Exception("@" + ps.SQL + "   Val=" + str + ex.Message);
            }
        }
        public static int RunSQLReturnValInt(string sql)
        {
            object obj = DBAccess.RunSQLReturnVal(sql);
            if (obj == null || obj == DBNull.Value)
                throw new Exception("@没有获取您要查询的数据,请检查SQL:" + sql + " @关于查询出来的详细信息已经记录日志文件，请处理。");
            string s = obj.ToString();
            if (s.Contains("."))
                s = s.Substring(0, s.IndexOf("."));
            return Convert.ToInt32(s);
        }
        public static int RunSQLReturnValInt(string sql, Paras paras)
        {
            return Convert.ToInt32(DA.DBAccess.RunSQLReturnVal(sql, paras));
        }
        public static int RunSQLReturnValInt(string sql, Paras paras, int isNullAsVal)
        {
            try
            {
                return Convert.ToInt32(DA.DBAccess.RunSQLReturnVal(sql, paras));
            }
            catch
            {
                return isNullAsVal;
            }
        }
        /// <summary>
        /// 查询单个值的方法
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">OleDbConnection</param>
        /// <returns>object</returns>
        public static object RunSQLReturnVal(string sql, OleDbConnection conn, string dsn)
        {
            return RunSQLReturnVal(sql, conn, CommandType.Text, dsn);
        }

        public static string RunSQLReturnString(string sql, Paras ps)
        {
            if (ps == null)
                ps = new Paras();
            object obj = DBAccess.RunSQLReturnVal(sql, ps);
            if (obj == DBNull.Value || obj == null)
                return null;
            else
                return obj.ToString();
        }
        /// <summary>
        /// 执行查询返回结果,如果为dbNull 返回 null.
        /// </summary>
        /// <param name="sql">will run sql.</param>
        /// <returns>,如果为dbNull 返回 null.</returns>
        public static string RunSQLReturnString(string sql)
        {
            try
            {
                return RunSQLReturnString(sql, new Paras());
            }
            catch (Exception ex)
            {
                throw new Exception("@运行 RunSQLReturnString出现错误：" + ex.Message + sql);
            }
        }
        public static string RunSQLReturnStringIsNull(Paras ps, string isNullAsVal)
        {
            string v = RunSQLReturnString(ps);
            if (v == null)
                return isNullAsVal;
            else
                return v;
        }
        /// <summary>
        /// 运行sql返回一个值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isNullAsVal"></param>
        /// <returns></returns>
        public static string RunSQLReturnStringIsNull(string sql, string isNullAsVal)
        {
            //try{
            string s = RunSQLReturnString(sql, new Paras());
            if (s == null)
                return isNullAsVal;
            return s;
            //}
            //catch (Exception ex)
            //{
            //    Log.DebugWriteInfo("RunSQLReturnStringIsNull@" + ex.Message);
            //    return isNullAsVal;
            //}
        }
        public static string RunSQLReturnString(Paras ps)
        {
            return RunSQLReturnString(ps.SQL, ps);
        }
        /// <summary>
        /// 查询单个值的方法
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">OleDbConnection</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">pars</param>
        /// <returns>object</returns>
        public static object RunSQLReturnVal(string sql, OleDbConnection conn, CommandType sqlType, params object[] pars)
        {

#if DEBUG
            Debug.WriteLine(sql);
#endif
            OleDbConnection tmpconn = new OleDbConnection(conn.ConnectionString);
            OleDbCommand cmd = new OleDbCommand(sql, tmpconn);
            object val = null;
            try
            {
                tmpconn.Open();
                cmd.CommandType = sqlType;
                foreach (object par in pars)
                {
                    cmd.Parameters.AddWithValue("par", par);
                }
                val = cmd.ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                cmd.Cancel();
                tmpconn.Close();
                cmd.Dispose();
                tmpconn.Dispose();
                throw new Exception(ex.Message + " [RunSQLReturnVal on OleDbConnection] " + sql);
            }
            tmpconn.Close();
            return val;
        }
        #endregion

        #region SqlConnection
        /// <summary>
        /// 查询单个值的方法
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">SqlConnection</param>
        /// <returns>object</returns>
        public static object RunSQLReturnVal(string sql, SqlConnection conn, string dsn)
        {
            return RunSQLReturnVal(sql, conn, CommandType.Text, dsn);

        }
        /// <summary>
        /// 查询单个值的方法
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">pars</param>
        /// <returns>object</returns>
        public static object RunSQLReturnVal(string sql, SqlConnection conn, CommandType sqlType, string dsn, params object[] pars)
        {
            //return DBAccess.RunSQLReturnTable(sql,conn,dsn,sqlType,null).Rows[0][0];

#if DEBUG
            Debug.WriteLine(sql);
#endif

            object val = null;
            SqlCommand cmd = null;

            try
            {
                if (conn == null)
                {
                    conn = new SqlConnection(dsn);
                    conn.Open();
                }

                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.ConnectionString = dsn;
                    conn.Open();
                }

                cmd = new SqlCommand(sql, conn);
                cmd.CommandType = sqlType;
                val = cmd.ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                //return DBAccess.re

                cmd.Cancel();
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
                throw new Exception(ex.Message + " [RunSQLReturnVal on SqlConnection] " + sql);
            }
            //conn.Close();
            return val;
        }
        #endregion


        #region 在当前的Connection执行 SQL 语句，返回首行首列
        public static int RunSQLReturnCOUNT(string sql)
        {
            return RunSQLReturnTable(sql).Rows.Count;
            //return RunSQLReturnVal( sql ,sql, sql );
        }
        public static object RunSQLReturnVal(string sql, string pkey, object val)
        {
            Paras ps = new Paras();
            ps.Add(pkey, val);

            return RunSQLReturnVal(sql, ps);
        }

        public static object RunSQLReturnVal(string sql, Paras paras)
        {
            RunSQLReturnTableCount++;
            //  Log.DebugWriteInfo("NUMOF " + RunSQLReturnTableCount + "===RunSQLReturnTable sql=" + sql);
            DataTable dt = null;
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    dt = DBAccess.RunSQLReturnTable_200705_Ora(sql, paras);
                    break;
                case DBType.MSSQL:
                    dt = DBAccess.RunSQLReturnTable_200705_SQL(sql, paras);
                    break;
                case DBType.MySQL:
                    dt = DBAccess.RunSQLReturnTable_200705_MySQL(sql, paras);
                    break;
                case DBType.Informix:
                    dt = DBAccess.RunSQLReturnTable_201205_Informix(sql, paras);
                    break;
                case DBType.Access:
                    dt = DBAccess.RunSQLReturnTable_200705_OLE(sql, paras);
                    break;
                default:
                    throw new Exception("@没有判断的数据库类型");
            }

            if (dt.Rows.Count == 0)
                return null;
            return dt.Rows[0][0];
        }
        public static object RunSQLReturnVal(Paras ps)
        {
            return RunSQLReturnVal(ps.SQL, ps);
        }
        public static object RunSQLReturnVal(string sql)
        {
            RunSQLReturnTableCount++;
            DataTable dt = null;
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    dt = DBAccess.RunSQLReturnTable_200705_Ora(sql, new Paras());
                    break;
                case DBType.MSSQL:
                    dt = DBAccess.RunSQLReturnTable_200705_SQL(sql, new Paras());
                    break;
                case DBType.Informix:
                    dt = DBAccess.RunSQLReturnTable_201205_Informix(sql, new Paras());
                    break;
                case DBType.MySQL:
                    dt = DBAccess.RunSQLReturnTable_200705_MySQL(sql, new Paras());
                    break;
                case DBType.Access:
                    dt = DBAccess.RunSQLReturnTable_200705_OLE(sql, new Paras());
                    break;
                default:
                    throw new Exception("@没有判断的数据库类型");
            }
            if (dt.Rows.Count == 0)
            {

#warning 不应该出现的异常 2011-12-03
                string cols = "";
                foreach (DataColumn dc in dt.Columns)
                    cols += " , " + dc.ColumnName;

               // BP.DA.Log.DebugWriteInfo("@SQL=" + sql + " . 列=" + cols);
                return null;
            }
            return dt.Rows[0][0];
        }
        #endregion

        #endregion

        #region 检查是不是存在
        /// <summary>
        /// 检查是不是存在
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>检查是不是存在</returns>
        public static bool IsExits(string sql)
        {
            if (sql.ToUpper().Contains("SELECT") == false)
                throw new Exception("@非法的查询语句" + sql);

            if (RunSQLReturnVal(sql) == null)
                return false;
            return true;
        }
        public static bool IsExits(string sql, Paras ps)
        {
            if (RunSQLReturnVal(sql, ps) == null)
                return false;
            return true;
        }

        /// <summary>
        /// 获得table的主键
        /// </summary>
        /// <param name="table">表名称</param>
        /// <returns>主键名称、没有返回为空.</returns>
        public static string GetTablePKName(string table)
        {
            BP.DA.Paras ps = new Paras();
            ps.Add("Tab", table);
            string sql = "";
            switch (AppCenterDBType)
            {
                case DBType.Access:
                    return null;
                case DBType.MSSQL:
                    sql = "SELECT CONSTRAINT_NAME,column_name FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE table_name =@Tab ";
                    break;
                case DBType.Oracle:
                    sql = "SELECT constraint_name, constraint_type,search_condition, r_constraint_name  from user_constraints WHERE table_name = upper(:tab) AND constraint_type = 'P'";
                    break;
                case DBType.MySQL:
                    sql = "SELECT CONSTRAINT_NAME , column_name, table_name CONSTRAINT_NAME from INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE table_name =@Tab and table_schema='" + SystemConfig.AppCenterDBDatabase + "' ";
                    break;
                case DBType.Informix:
                    sql = "SELECT * FROM sysconstraints c inner join systables t on c.tabid = t.tabid where t.tabname = lower(?) and constrtype = 'P'";
                    break;
                default:
                    throw new Exception("@没有判断的数据库类型.");
            }

            DataTable dt = DBAccess.RunSQLReturnTable(sql, ps);
            if (dt.Rows.Count == 0)
                return null;
            return dt.Rows[0][0].ToString();
        }
        /// <summary>
        /// 判断是否存在主键pk .
        /// </summary>
        /// <param name="tab">物理表</param>
        /// <returns>是否存在</returns>
        public static bool IsExitsTabPK(string tab)
        {
            if (DBAccess.GetTablePKName(tab) == null)
                return false;
            else
                return true;
        }
        /// <summary>
        /// 是否是view
        /// </summary>
        /// <param name="tabelOrViewName"></param>
        /// <returns></returns>
        public static bool IsView(string tabelOrViewName)
        {
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = "SELECT TABTYPE  FROM TAB WHERE UPPER(TNAME)=:v";
                    DataTable oradt = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName.ToUpper());
                    if (oradt.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");
                    if (oradt.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
                        return true;
                    else
                        return false;
                    //break;
                case DBType.Access:
                    sql = "select   Type   from   msysobjects   WHERE   UCASE(name)='" + tabelOrViewName.ToUpper() + "'";
                    DataTable dtw = DBAccess.RunSQLReturnTable(sql);
                    if (dtw.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");
                    if (dtw.Rows[0][0].ToString().Trim() == "5")
                        return true;
                    else
                        return false;
                case DBType.MSSQL:
                    sql = "select xtype from sysobjects WHERE name =" + SystemConfig.AppCenterDBVarStr + "v";
                    DataTable dt1 = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName);
                    if (dt1.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dt1.Rows[0][0].ToString().ToUpper().Trim() == "V".ToString())
                        return true;
                    else
                        return false;
                case DBType.Informix:
                    sql = "select tabtype from systables where tabname = '" + tabelOrViewName.ToLower() + "'";
                    DataTable dtaa = DBAccess.RunSQLReturnTable(sql);
                    if (dtaa.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dtaa.Rows[0][0].ToString().ToUpper().Trim() == "V")
                        return true;
                    else
                        return false;
                case DBType.MySQL:
                    sql = "SELECT Table_Type FROM information_schema.TABLES WHERE table_name='" + tabelOrViewName + "' and table_schema='" + SystemConfig.AppCenterDBDatabase + "'";
                    DataTable dt2 = DBAccess.RunSQLReturnTable(sql);
                    if (dt2.Rows.Count == 0)
                        throw new Exception("@表不存在[" + tabelOrViewName + "]");

                    if (dt2.Rows[0][0].ToString().ToUpper().Trim() == "VIEW")
                        return true;
                    else
                        return false;
                default:
                    throw new Exception("@没有做的判断。");
            }

            /*DataTable dt = DBAccess.RunSQLReturnTable(sql, "v", tabelOrViewName.ToUpper());
            if (dt.Rows.Count == 0)
                throw new Exception("@表不存在[" + tabelOrViewName + "]");

            if (dt.Rows[0][0].ToString() == "VIEW")
                return true;
            else
                return false;
            return true;*/
        }
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsExitsObject(string obj)
        {
            return IsExitsObject(new DBUrl(DBUrlType.AppCenterDSN), obj);
        }
        /// <summary>
        /// 判断系统中是否存在对象.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static bool IsExitsObject(DBUrl dburl, string obj)
        {
            if (dburl.DBUrlType == DBUrlType.AppCenterDSN)
            {
                //有的同事写的表名包含dbo.导致创建失败.
                obj = obj.Replace("dbo.", "");

                // 增加参数.
                Paras ps = new Paras();
                ps.Add("obj", obj);

                switch (AppCenterDBType)
                {
                    case DBType.Oracle:
                        if (obj.IndexOf(".") != -1)
                            obj = obj.Split('.')[1];
                        return IsExits("select object_name from all_objects WHERE  object_name = upper(:obj) and OWNER='" + DBAccess.ConnectionUserID.ToUpper() + "' ", ps);
                    case DBType.MSSQL:
                        return IsExits("SELECT name FROM sysobjects WHERE name = '" + obj + "'");
                    case DBType.Informix:
                        return IsExits("select tabname from systables where tabname = '" + obj.ToLower() + "'");
                    case DBType.MySQL:

                            /*如果不是检查的PK.*/
                            if (obj.IndexOf(".") != -1)
                                obj = obj.Split('.')[1];

                            // *** 屏蔽到下面的代码, 不需要从那个数据库里取，jflow 发现的bug  edit by :zhoupeng   2016.01.26 for fuzhou.
                            return IsExits("SELECT table_name, table_type FROM information_schema.tables  WHERE table_name = '" + obj + "' AND TABLE_SCHEMA='" + BP.Sys.SystemConfig.AppCenterDBDatabase + "' ");

                    case DBType.Access:
                        //return false ; //IsExits("SELECT * FROM MSysObjects WHERE (((MSysObjects.Name) =  '"+obj+"' ))");
                        return IsExits("SELECT * FROM MSysObjects WHERE Name =  '" + obj + "'");
                    default:
                        throw new Exception("没有识别的数据库编号");
                }
            }

            if (dburl.DBUrlType == DBUrlType.DBAccessOfMSSQL1)
                return DBAccessOfMSSQL1.IsExitsObject(obj);

            //if (dburl.DBUrlType == DBUrlType.DBAccessOfODBC)
            //    return DBAccessOfODBC.IsExitsObject(obj);

            //if (dburl.DBUrlType == DBUrlType.DBAccessOfOLE)
            //    return DBAccessOfOLE.IsExitsObject(obj);

            if (dburl.DBUrlType == DBUrlType.DBAccessOfOracle1)
                return DBAccessOfOracle1.IsExitsObject(obj);

            if (dburl.DBUrlType == DBUrlType.DBAccessOfOracle2)
                return DBAccessOfOracle2.IsExitsObject(obj);

            throw new Exception("@没有判断的数据库类型:" + dburl);
        }
        /// <summary>
        /// 表中是否存在指定的列
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="col">列名</param>
        /// <returns>是否存在</returns>
        public static bool IsExitsTableCol(string table, string col)
        {
            Paras ps = new Paras();
            ps.Add("tab", table);
            ps.Add("col", col);

            int i = 0;
            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Access:
                    return false;
                    break;
                case DBType.MSSQL:
                    i = DBAccess.RunSQLReturnValInt("SELECT  COUNT(*)  FROM information_schema.COLUMNS  WHERE TABLE_NAME='" + table + "' AND COLUMN_NAME='" + col + "'", 0);
                    break;
                case DBType.MySQL:
                    string sql = "select count(*) FROM information_schema.columns WHERE TABLE_SCHEMA='" + BP.Sys.SystemConfig.AppCenterDBDatabase + "' AND table_name ='" + table + "' and column_Name='" + col + "'";
                    i = DBAccess.RunSQLReturnValInt(sql);
                    break;
                case DBType.Oracle:
                    if (table.IndexOf(".") != -1)
                        table = table.Split('.')[1];
                    i = DBAccess.RunSQLReturnValInt("SELECT COUNT(*) from user_tab_columns  WHERE table_name= upper(:tab) AND column_name= upper(:col) ", ps);
                    break;
                case DBType.Informix:
                    i = DBAccess.RunSQLReturnValInt("select count(*) from syscolumns c where tabid in (select tabid	from systables	where tabname = lower('" + table + "')) and c.colname = lower('" + col + "')", 0);
                    break;
                default:
                    throw new Exception("error");
            }

            if (i == 1)
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// 获得表的基础信息，返回如下列:
        /// 1, 字段名称，字段描述，字段类型，字段长度.
        /// </summary>
        /// <param name="tableName">表名</param>
        public static DataTable GetTableSchema(string tableName, bool isUpper = true)
        {
            string sql = "";
            switch (SystemConfig.AppCenterDBType)
            {
                case DBType.MSSQL:
                    sql = "SELECT column_name as FNAME, data_type as FTYPE, CHARACTER_MAXIMUM_LENGTH as FLEN , column_name as FDESC FROM information_schema.columns where table_name='" + tableName + "'";
                    break;
                case DBType.Oracle:
                    sql = "SELECT COLUMN_NAME as FNAME,DATA_TYPE as FTYPE,DATA_LENGTH as FLEN,COLUMN_NAME as FDESC FROM all_tab_columns WHERE table_name = upper('" + tableName + "')";
                    break;
                case DBType.MySQL:
                    sql = "SELECT COLUMN_NAME FNAME,DATA_TYPE FTYPE,CHARACTER_MAXIMUM_LENGTH FLEN,COLUMN_COMMENT FDESC FROM information_schema.columns WHERE table_name='" + tableName + "' and TABLE_SCHEMA='" + SystemConfig.AppCenterDBDatabase + "'";
                    break;
                default:
                    break;
            }

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (isUpper == false)
            {
                dt.Columns["FNAME"].ColumnName = "FName";
                dt.Columns["FTYPE"].ColumnName = "FType";
                dt.Columns["FLEN"].ColumnName = "FLen";
                dt.Columns["FDESC"].ColumnName = "FDesc";
            }
            return dt;
        }

        public static DataTable ToLower(DataTable dt)
        {
            //把列名转成小写.
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToLower();
            }
            return dt;
        }

        #region LoadConfig
        public static void LoadConfig(string cfgFile, string basePath)
        {
            if (!File.Exists(cfgFile))
                throw new Exception("找不到配置文件==>[" + cfgFile + "]1");

            StreamReader read = new StreamReader(cfgFile);
            string firstline = read.ReadLine();
            string cfg = read.ReadToEnd();
            read.Close();

            int start = cfg.ToLower().IndexOf("<appsettings>");
            int end = cfg.ToLower().IndexOf("</appsettings>");

            cfg = cfg.Substring(start, end - start + "</appsettings".Length + 1);

            cfgFile = basePath + "\\__$AppConfig.cfg";
            StreamWriter write = new StreamWriter(cfgFile);
            write.WriteLine(firstline);
            write.Write(cfg);
            write.Flush();
            write.Close();

            DataSet dscfg = new DataSet("cfg");
            try
            {
                dscfg.ReadXml(cfgFile);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //  BP.Sys.SystemConfig.CS_AppSettings = new System.Collections.Specialized.NameValueCollection();
            BP.Sys.SystemConfig.CS_DBConnctionDic.Clear();
            foreach (DataRow row in dscfg.Tables["add"].Rows)
            {
                BP.Sys.SystemConfig.CS_AppSettings.Add(row["key"].ToString().Trim(), row["value"].ToString().Trim());
            }
            dscfg.Dispose();

            BP.Sys.SystemConfig.IsBSsystem = false;
        }


        #endregion
    }

    #region ODBC
    public class DBAccessOfODBC
    {
        /// <summary>
        /// 检查是不是存在
        /// </summary>
        public static bool IsExits(string selectSQL)
        {
            if (RunSQLReturnVal(selectSQL) == null)
                return false;
            return true;
        }

        #region 取得连接对象 ，CS、BS共用属性【关键属性】
        public static OdbcConnection GetSingleConn
        {
            get
            {
                if (SystemConfig.IsBSsystem_Test)
                {
                    OdbcConnection conn = HttpContext.Current.Session["DBAccessOfODBC"] as OdbcConnection;
                    if (conn == null)
                    {
                        conn = new OdbcConnection(SystemConfig.AppSettings["DBAccessOfODBC"]);
                        HttpContext.Current.Session["DBAccessOfODBC"] = conn;
                    }
                    return conn;
                }
                else
                {
                    OdbcConnection conn = SystemConfig.CS_DBConnctionDic["DBAccessOfODBC"] as OdbcConnection;
                    if (conn == null)
                    {
                        conn = new OdbcConnection(SystemConfig.AppSettings["DBAccessOfODBC"]);
                        SystemConfig.CS_DBConnctionDic["DBAccessOfODBC"] = conn;
                    }
                    return conn;
                }
            }
        }
        #endregion 取得连接对象 ，CS、BS共用属性


        #region 重载 RunSQLReturnTable

        #region 使用本地的连接
        public static DataTable RunSQLReturnTable(string sql)
        {
            return RunSQLReturnTable(sql, GetSingleConn, CommandType.Text);
        }
        public static DataTable RunSQLReturnTable(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQLReturnTable(sql, GetSingleConn, sqlType, pars);
        }

        #endregion

        #region 使用指定的连接
        public static DataTable RunSQLReturnTable(string sql, OdbcConnection conn)
        {
            return RunSQLReturnTable(sql, conn, CommandType.Text);
        }
        public static DataTable RunSQLReturnTable(string sql, OdbcConnection conn, CommandType sqlType, params object[] pars)
        {
            try
            {
                OdbcDataAdapter ada = new OdbcDataAdapter(sql, conn);
                ada.SelectCommand.CommandType = sqlType;
                foreach (object par in pars)
                {
                    ada.SelectCommand.Parameters.AddWithValue("par", par);
                }
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DataTable dt = new DataTable("tb");
                ada.Fill(dt);
                // peng add 
                ada.Dispose();
                return dt;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }
        #endregion

        #endregion

        #region 重载 RunSQL

        #region 使用本地的连接
        public static int RunSQLReturnCOUNT(string sql)
        {
            return RunSQLReturnTable(sql).Rows.Count;
        }
        public static int RunSQL(string sql)
        {
            return RunSQL(sql, GetSingleConn, CommandType.Text);
        }
        public static int RunSQL(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQL(sql, GetSingleConn, sqlType, pars);
        }
        #endregion 使用本地的连接

        #region 使用指定的连接
        public static int RunSQL(string sql, OdbcConnection conn)
        {
            return RunSQL(sql, conn, CommandType.Text);
        }
        public static int RunSQL(string sql, OdbcConnection conn, CommandType sqlType, params object[] pars)
        {
            Debug.WriteLine(sql);
            try
            {
                OdbcCommand cmd = new OdbcCommand(sql, conn);
                cmd.CommandType = sqlType;
                foreach (object par in pars)
                {
                    cmd.Parameters.AddWithValue("par", par);
                }
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                return cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }

        #endregion 使用指定的连接

        #endregion

        #region 执行SQL ，返回首行首列

        /// <summary>
        /// 运行select sql, 返回一个值。
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <returns>返回的值object</returns>
        public static float RunSQLReturnFloatVal(string sql)
        {
            return (float)RunSQLReturnVal(sql, GetSingleConn, CommandType.Text);
        }
        public static int RunSQLReturnValInt(string sql)
        {
            return (int)RunSQLReturnVal(sql, GetSingleConn, CommandType.Text);
        }
        /// <summary>
        /// 运行select sql, 返回一个值。
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <returns>返回的值object</returns>
        public static object RunSQLReturnVal(string sql)
        {
            return RunSQLReturnVal(sql, GetSingleConn, CommandType.Text);
        }
        /// <summary>
        /// 运行select sql, 返回一个值。
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">params</param>
        /// <returns>返回的值object</returns>
        public static object RunSQLReturnVal(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQLReturnVal(sql, GetSingleConn, sqlType, pars);
        }
        public static object RunSQLReturnVal(string sql, OdbcConnection conn)
        {
            return RunSQLReturnVal(sql, conn, CommandType.Text);
        }
        public static object RunSQLReturnVal(string sql, OdbcConnection conn, CommandType sqlType, params object[] pars)
        {
            Debug.WriteLine(sql);
            OdbcConnection tmp = new OdbcConnection(conn.ConnectionString);
            OdbcCommand cmd = new OdbcCommand(sql, tmp);
            object val = null;
            try
            {
                cmd.CommandType = sqlType;
                foreach (object par in pars)
                {
                    cmd.Parameters.AddWithValue("par", par);
                }
                tmp.Open();
                val = cmd.ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                tmp.Close();
                throw new Exception(ex.Message + sql);
            }
            tmp.Close();
            return val;
        }
        #endregion 执行SQL ，返回首行首列

    }
    #endregion

    /// <summary>
    /// 关于OLE 的连接
    /// </summary>
    public class DBAccessOfOLE
    {
        /// <summary>
        /// 检查是不是存在
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <returns>检查是不是存在</returns>
        public static bool IsExits(string selectSQL)
        {
            if (RunSQLReturnVal(selectSQL) == null)
                return false;
            return true;
        }

        #region 取得连接对象 ，CS、BS共用属性【关键属性】
        public static OleDbConnection GetSingleConn
        {
            get
            {
                if (SystemConfig.IsBSsystem_Test)
                {
                    OleDbConnection conn = HttpContext.Current.Session["DBAccessOfOLE"] as OleDbConnection;
                    if (conn == null)
                    {
                        conn = new OleDbConnection(SystemConfig.DBAccessOfOLE);
                        HttpContext.Current.Session["DBAccessOfOLE"] = conn;
                    }
                    return conn;
                }
                else
                {
                    OleDbConnection conn = SystemConfig.CS_DBConnctionDic["DBAccessOfOLE"] as OleDbConnection;
                    if (conn == null)
                    {
                        conn = new OleDbConnection(SystemConfig.DBAccessOfOLE);
                        SystemConfig.CS_DBConnctionDic["DBAccessOfOLE"] = conn;
                    }
                    return conn;
                }
            }
        }
        #endregion 取得连接对象 ，CS、BS共用属性

        #region 重载 RunSQLReturnTable

        #region 使用本地的连接
        public static int RunSQLReturnCOUNT(string sql)
        {
            return RunSQLReturnTable(sql).Rows.Count;
            //return RunSQLReturnVal( sql ,sql, sql );
        }
        /// <summary>
        /// 运行查询语句返回Table
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSQLReturnTable(string sql)
        {
            return RunSQLReturnTable(sql, GetSingleConn, CommandType.Text);
        }
        /// <summary>
        /// 运行查询语句返回Table
        /// </summary>
        /// <param name="sql">select sql</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">pareas</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSQLReturnTable(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQLReturnTable(sql, GetSingleConn, sqlType, pars);
        }
        #endregion

        #region 使用指定的连接
        public static DataTable RunSQLReturnTable(string sql, OleDbConnection conn)
        {
            return RunSQLReturnTable(sql, conn, CommandType.Text);
        }
        public static DataTable RunSQLReturnTable(string sql, OleDbConnection conn, CommandType sqlType, params object[] pars)
        {
            try
            {
                OleDbDataAdapter ada = new OleDbDataAdapter(sql, conn);
                ada.SelectCommand.CommandType = sqlType;
                foreach (object par in pars)
                {
                    ada.SelectCommand.Parameters.AddWithValue("par", par);
                }
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                DataTable dt = new DataTable("tb");
                ada.Fill(dt);
                ada.Dispose();
                return dt;
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }
        #endregion

        #endregion

        #region 重载 RunSQL

        #region 使用本地的连接
        public static int RunSQL(string sql)
        {
            return RunSQL(sql, GetSingleConn, CommandType.Text);
        }
        public static int RunSQL(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQL(sql, GetSingleConn, sqlType, pars);
        }
        #endregion 使用本地的连接

        #region 使用指定的连接
        public static int RunSQL(string sql, OleDbConnection conn)
        {
            return RunSQL(sql, conn, CommandType.Text);
        }
        public static int RunSQL(string sql, OleDbConnection conn, CommandType sqlType, params object[] pars)
        {
            Debug.WriteLine(sql);
            try
            {
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.CommandType = sqlType;
                foreach (object par in pars)
                {
                    cmd.Parameters.AddWithValue("par", par);
                }

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {
                throw new Exception(ex.Message + sql);
            }
        }

        #endregion 使用指定的连接

        #endregion

        #region 执行SQL ，返回首行首列
        public static object RunSQLReturnVal(string sql)
        {
            return RunSQLReturnVal(sql, GetSingleConn, CommandType.Text);
        }
        public static object RunSQLReturnVal(string sql, CommandType sqlType, params object[] pars)
        {
            return RunSQLReturnVal(sql, GetSingleConn, sqlType, pars);
        }

        public static object RunSQLReturnVal(string sql, OleDbConnection conn)
        {
            return RunSQLReturnVal(sql, conn, CommandType.Text);
        }
        public static object RunSQLReturnVal(string sql, OleDbConnection conn, CommandType sqlType, params object[] pars)
        {
            Debug.WriteLine(sql);
            OleDbConnection tmpconn = new OleDbConnection(conn.ConnectionString);
            OleDbCommand cmd = new OleDbCommand(sql, tmpconn);
            object val = null;
            try
            {
                cmd.CommandType = sqlType;
                foreach (object par in pars)
                {
                    cmd.Parameters.AddWithValue("par", par);
                }
                tmpconn.Open();
                val = cmd.ExecuteScalar();
            }
            catch (System.Exception ex)
            {
                cmd.Cancel();
                tmpconn.Close();
                throw new Exception(ex.Message + sql);
            }
            tmpconn.Close();
            return val;
        }
        #endregion 执行SQL ，返回首行首列
    }

}
