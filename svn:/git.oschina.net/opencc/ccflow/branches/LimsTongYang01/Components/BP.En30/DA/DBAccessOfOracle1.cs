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
    /// Oracle 的访问
    /// </summary>
    public class DBAccessOfOracle1
    {
        #region 关于运行存储过程

        #region 执行存储过程返回影响个数
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <returns>返回影响的行数</returns>
        public static int RunSP(string spName)
        {
            return DBProcedure.RunSP(spName, DBAccessOfOracle1.GetSingleConn);
        }
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <param name="paras">参数</param>
        /// <returns>返回影响的行数</returns>
        public static int RunSP(string spName, Paras paras)
        {
            return DBProcedure.RunSP(spName, paras, DBAccessOfOracle1.GetSingleConn);
        }
        public static int RunSP(string spName, string para, string paraVal)
        {
            Paras paras = new Paras();
            Para p = new Para(para, DbType.String, paraVal);
            paras.Add(p);
            return DBProcedure.RunSP(spName, paras, DBAccessOfOracle1.GetSingleConn);
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
            return DBProcedure.RunSPReturnDataTable(spName, DBAccessOfOracle1.GetSingleConn);
        }
        /// <summary>
        /// 运行存储过程
        /// </summary>
        /// <param name="spName">名称</param>
        /// <param name="paras">参数</param>
        /// <returns>DataTable</returns>
        public static DataTable RunSPReTable(string spName, Paras paras)
        {
            return DBProcedure.RunSPReturnDataTable(spName, paras, DBAccessOfOracle1.GetSingleConn);
        }
        #endregion

        #endregion

        #region 检查是否存在.
        /// <summary>
        /// 检查是存在
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns>是否存在</returns>
        public static bool IsExitsObject(string obj)
        {
            //有的同事写的表名包含dbo.导致创建失败.
            obj = obj.Replace("dbo.", "");
            if (obj.IndexOf(".") != -1)
                obj = obj.Split('.')[1];

            string sql = "select tname from tab WHERE  tname = upper('" + obj + "') ";
            return IsExits(sql);
        }
        /// <summary>
        /// 检查是不是存在
        /// </summary>
        /// <param name="selectSQL"></param>
        /// <returns>检查是不是存在</returns>
        public static bool IsExits(string sql)
        {
            if (RunSQLReturnVal(sql) == null)
                return false;
            return true;
        }
        #endregion 检查是否存在.

        #region 取得连接对象 ，CS、BS共用属性【关键属性】
        public static OracleConnection GetSingleConn
        {
            get
            {
                if (SystemConfig.IsBSsystem_Test)
                {
                    OracleConnection conn = HttpContext.Current.Session["DBAccessOfOracle1"] as OracleConnection;
                    if (conn == null)
                    {
                        conn = new OracleConnection(SystemConfig.AppSettings["DBAccessOfOracle1"]);
                        conn.Open();
                        HttpContext.Current.Session["DBAccessOfOracle1"] = conn;
                    }
                    return conn;
                }
                else
                {
                    OracleConnection conn = SystemConfig.CS_DBConnctionDic["DBAccessOfOracle1"] as OracleConnection;
                    if (conn == null)
                    {
                        conn = new OracleConnection(SystemConfig.AppSettings["DBAccessOfOracle1"]);
                        SystemConfig.CS_DBConnctionDic["DBAccessOfOracle1"] = conn;
                        conn.Open();
                    }
                    else
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();
                    }

                    return conn;
                }
            }
        }
        #endregion 取得连接对象 ，CS、BS共用属性

        #region 重载 RunSQLReturnTable
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns>返回的结果集合</returns>
        public static DataTable RunSQLReturnTable(string sql)
        {
            return DBAccess.RunSQLReturnTable(sql, GetSingleConn, CommandType.Text, SystemConfig.AppSettings["DBAccessOfOracle1"]);
        }
        /// <summary>
        /// 运行sql返回table.
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">para</param>
        /// <returns>返回的结果集合</returns>
        public static DataTable RunSQLReturnTable(string sql, CommandType sqlType, params object[] pars)
        {
            return DBAccess.RunSQLReturnTable(sql, GetSingleConn, sqlType, SystemConfig.AppSettings["DBAccessOfOracle1"]);
        }
        #endregion

        #region 重载 RunSQL
        public static int RunSQLTRUNCATETable(string table)
        {
            return DBAccess.RunSQL("  TRUNCATE TABLE " + table);    
        }
        /// <summary>
        /// 运行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int RunSQL(string sql)
        {
            return DBAccess.RunSQL(sql, GetSingleConn, CommandType.Text, SystemConfig.AppSettings["DBAccessOfOracle1"]);
        }
        /// <summary>
        /// 运行 SQL
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">参数</param>
        /// <returns>结果集</returns>
        public static int RunSQL(string sql, CommandType sqlType, params object[] pars)
        {
            return DBAccess.RunSQL(sql, GetSingleConn, sqlType, SystemConfig.AppSettings["DBAccessOfOracle1"]);
        }
        #endregion

        #region 执行SQL ，返回首行首列
        /// <summary>
        /// 运行sql 返回一个object .
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>object</returns>
        public static object RunSQLReturnVal(string sql)
        {
            DataTable dt = RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
                return null;
            return dt.Rows[0][0];
        }
        /// <summary>
        /// run sql return object values
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int RunSQLReturnIntVal(string sql)
        {
            string str = DBAccessOfOracle1.RunSQLReturnVal(sql).ToString();
            try
            {
                return int.Parse(str);
            }
            catch (Exception ex)
            {
                throw new Exception("RunSQLReturnIntVal 9i =" + ex.Message + " str = " + str);
            }
        }
        /// <summary>
        /// run sql return float values.
        /// </summary>
        /// <param name="sql">will run sql</param>
        /// <returns>values</returns>
        public static float RunSQLReturnFloatVal(string sql)
        {

            object obj = DBAccessOfOracle1.RunSQLReturnVal(sql);

            if (obj.ToString() == "System.DBNull")
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnFloatVal(sql)错误,运行的结果是null.请检查sql=" + sql);

            try
            {
                return float.Parse(obj.ToString());
            }
            catch
            {
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnFloatVal(sql)错误,运行的结果是[" + obj.ToString() + "].不能向float 转换,请检查sql=" + sql);
            }
        }
        /// <summary>
        /// 运行sql 返回float
        /// </summary>
        /// <param name="sql">要运行的sql</param>
        /// <param name="isNullAsVal">如果是Null, 返回的信息.</param>
        /// <returns></returns>
        public static float RunSQLReturnFloatVal(string sql, float isNullAsVal)
        {
            object obj = DBAccessOfOracle1.RunSQLReturnVal(sql);
            try
            {
                System.DBNull dbnull = (System.DBNull)obj;
                return isNullAsVal;
            }
            catch
            {
            }

            try
            {
                return float.Parse(obj.ToString());
            }
            catch
            {
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnFloatVal(sql,isNullAsVal)错误,运行的结果是[" + obj + "].不能向float 转换,请检查sql=" + sql);
            }
        }
        /// <summary>
        /// run sql return decimal val
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static decimal RunSQLReturnDecimalVal(string sql)
        {
            object obj = DBAccessOfOracle1.RunSQLReturnVal(sql);
            if (obj == null)
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnDecimalVal()错误,运行的结果是null.请检查sql=" + sql);
            try
            {
                return decimal.Parse(obj.ToString());
            }
            catch
            {
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnDecimalVal()错误,运行的结果是[" + obj + "].不能向decimal 转换,请检查sql=" + sql);
            }
        }
        /// <summary>
        /// run sql return decimal.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isNullAsVal"></param>
        /// <returns></returns>
        public static decimal RunSQLReturnDecimalVal(string sql, decimal isNullAsVal)
        {
            object obj = DBAccessOfOracle1.RunSQLReturnVal(sql);
            try
            {
                System.DBNull dbnull = (System.DBNull)obj;
                return isNullAsVal;
            }
            catch
            {
            }

            try
            {
                return decimal.Parse(obj.ToString());
            }
            catch
            {
                throw new Exception("@执行方法DBAccessOfOracle1.RunSQLReturnDecimalVal(sql,isNullAsVal)错误,运行的结果是[" + obj + "].不能向float 转换,请检查sql=" + sql);
            }
        }
        #endregion 执行SQL ，返回首行首列
    }
}
