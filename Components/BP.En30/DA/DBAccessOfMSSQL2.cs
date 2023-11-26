
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
//using System.EnterpriseServices;
using System.Data.OleDb;
using System.Web;
using System.Data.Odbc;
using System.IO;
using BP.Sys;
using BP.Web;

namespace BP.DA
{
    /// <summary>
    /// 关于DBAccessOfSQLServer2000 的连接
    /// </summary>
    public class DBAccessOfMSSQL2
    {
        #region 关于运行存储过程
        /// <summary>
        /// 运行存储过程.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static object RunSPReObj(string spName, Paras paras)
        {
            SqlConnection conn = DBAccessOfMSSQL2.GetSingleConn;
            SqlCommand cmd = new SqlCommand(spName, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (conn.State == System.Data.ConnectionState.Closed)
                conn.Open();

            foreach (Para para in paras)
            {
                SqlParameter myParameter = new SqlParameter(para.ParaName, para.val);
                myParameter.Direction = ParameterDirection.Input;
                myParameter.Size = para.Size;
                myParameter.Value = para.val;
                cmd.Parameters.Add(myParameter);
            }

            return cmd.ExecuteScalar();
        }
        /// <summary>
        /// 运行存储过程返回一个decimal.
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="paras"></param>
        /// <param name="isNullReVal"></param>
        /// <returns></returns>
        public static decimal RunSPReDecimal(string spName, Paras paras, decimal isNullReVal)
        {
            object obj = RunSPReObj(spName, paras);
            if (obj == null || obj == DBNull.Value)
                return isNullReVal;

            try
            {
                return decimal.Parse(obj.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("RunSPReDecimal error=" + ex.Message + " Obj=" + obj);
            }
        }

        #endregion

        private static bool lock_SQL = false;
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
        /// <summary>
        /// 取出当前的 连接。
        /// </summary>
        public static SqlConnection GetSingleConn
        {
            get
            {
                if (SystemConfig.IsBSsystem_Test)
                {
                    SqlConnection conn = HttpContextHelper.SessionGet("DBAccessOfMSSQL2") as SqlConnection;
                    if (conn == null)
                    {
                        conn = new SqlConnection(SystemConfig.AppSettings["DBAccessOfMSSQL2"]);
                        HttpContextHelper.SessionSet("DBAccessOfMSSQL2", conn);
                    }
                    return conn;
                }
                else
                {
                    SqlConnection conn = SystemConfig.CS_DBConnctionDic["DBAccessOfMSSQL2"] as SqlConnection;
                    if (conn == null)
                    {
                        conn = new SqlConnection(SystemConfig.AppSettings["DBAccessOfMSSQL2"]);
                        SystemConfig.CS_DBConnctionDic["DBAccessOfMSSQL2"] = conn;
                    }
                    return conn;
                }
            }
        }
        #endregion 取得连接对象 ，CS、BS共用属性

        #region 重载 RunSQLReturnTable
        public static DataTable RunSQLReturnTable(string sql)
        {
            return DBAccess.RunSQLReturnTable(sql, GetSingleConn, SystemConfig.AppSettings["DBAccessOfMSSQL2"], CommandType.Text, null);
        }
        public static DataTable RunSQLReturnTable(string sql, params object[] pars)
        {
            return RunSQLReturnTable(sql, CommandType.Text, pars);
        }
        public static DataTable RunSQLReturnTable(string sql, CommandType sqlType, Paras pars)
        {
            return DBAccess.RunSQLReturnTable(sql, GetSingleConn, SystemConfig.AppSettings["DBAccessOfMSSQL2"], sqlType, pars);
        }
        public static int RunSQLReturnCOUNT(string sql)
        {
            return RunSQLReturnTable(sql).Rows.Count;
            //return RunSQLReturnVal( sql ,sql, sql );
        }
        public static bool IsExitsObject(string obj)
        {
            return IsExits("SELECT name  FROM sysobjects  WHERE  name = '" + obj + "'");
        }
        /// <summary>
        /// 运行SQL , 返回影响的行数.
        /// </summary>
        /// <param name="sql">msSQL</param>
        /// <param name="conn">SqlConnection</param>
        /// <param name="sqlType">CommandType</param>
        /// <param name="pars">params</param>
        /// <returns>返回运行结果</returns>
        public static int RunSQL(string sql)
        {
            SqlConnection conn = DBAccessOfMSSQL2.GetSingleConn;
            //string step="step=1" ;
            //如果是锁定状态，就等待.
            while (lock_SQL)
            {
                lock_SQL = true; //锁定
            }
            try
            {

                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;

                int i = 0;
                try
                {
                    i = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                cmd.Dispose();
                lock_SQL = false;
                return i;
            }
            catch (System.Exception ex)
            {
                lock_SQL = false;
                throw new Exception(ex.Message + "    SQL = " + sql);
            }
            finally
            {
                lock_SQL = false;
                conn.Close();
            }
        }
        #endregion

        #region 执行SQL ，返回首行首列
        public static object RunSQLReturnVal(string sql)
        {
            return DBAccess.RunSQLReturnVal(sql, GetSingleConn, CommandType.Text, SystemConfig.AppSettings["DBAccessOfMSSQL2"]);
        }
        public static object RunSQLReturnVal(string sql, CommandType sqlType, params object[] pars)
        {
            return DBAccess.RunSQLReturnVal(sql, GetSingleConn, sqlType, SystemConfig.AppSettings["DBAccessOfMSSQL2"], pars);
        }
        #endregion 执行SQL ，返回首行首列

    }
}
