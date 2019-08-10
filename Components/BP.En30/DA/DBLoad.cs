using System;
using System.IO;
using System.Data;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using BP.NetPlatformImpl;

namespace BP.DA
{
    /// <summary>
    /// DBLoad 的摘要说明。
    /// </summary>
    public class DBLoad
    {
        /// <summary>
        /// 装载
        /// </summary>
		public DBLoad()
        {
        }
        public static int ImportTableInto(DataTable impTb, string intoTb, string select, int clear)
        {
            int count = 0;
            DataTable target = null;

            //导入前是否先清空
            if (clear == 1)
                DBAccess.RunSQL("delete from " + intoTb);

            try
            {
                target = DBAccess.RunSQLReturnTable(select);
            }
            catch (Exception ex) //select查询出错，可能是缺少列
            {
                throw new Exception("源表格式有误，请核对！" + ex.Message + " :" + select);
            }

            object conn = DBAccess.GetAppCenterDBConn;

            SqlDataAdapter sqlada = null;
            OracleDataAdapter oraada = null;
            DBType dbt = DBAccess.AppCenterDBType;
            if (dbt == DBType.MSSQL)
            {
                sqlada = new SqlDataAdapter(select, (SqlConnection)DBAccess.GetAppCenterDBConn);
                SqlCommandBuilder bl = new SqlCommandBuilder(sqlada);
                sqlada.InsertCommand = bl.GetInsertCommand();

                count = ImportTable(impTb, target, sqlada);
            }
            else if (dbt == DBType.Oracle)
            {
                oraada = new OracleDataAdapter(select, (OracleConnection)DBAccess.GetAppCenterDBConn);
                OracleCommandBuilder bl = new OracleCommandBuilder(oraada);
                oraada.InsertCommand = bl.GetInsertCommand();

                count = ImportTable(impTb, target, oraada);
            }
            else
                throw new Exception("未获取数据库连接！ ");

            target.Dispose();
            return count;
        }
        private static int ImportTable(DataTable source, DataTable target, SqlDataAdapter sqlada)
        {
            int count = 0;
            try
            {
                if (sqlada.InsertCommand.Connection.State != ConnectionState.Open)
                    sqlada.InsertCommand.Connection.Open();
                sqlada.InsertCommand.Transaction = sqlada.InsertCommand.Connection.BeginTransaction();
                source.Columns.Add("错误提示", typeof(string));
                source.Columns["错误提示"].MaxLength = 1000;

                int i = 0;
                while (i < source.Rows.Count)   //for( int i=0;i<;i++)
                {
                    for (int c = 0; c < target.Columns.Count; c++)
                    {
                        sqlada.InsertCommand.Parameters[c].Value = source.Rows[i][c];
                    }
                    try//个别记录失败，跳过
                    {
                        sqlada.InsertCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        source.Rows[i]["错误提示"] = ex.Message;
                        i++;
                        continue;
                    }
                    count++; //已导入的记录数
                    source.Rows.RemoveAt(i);
                }
                sqlada.InsertCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (sqlada.InsertCommand.Transaction != null)
                    sqlada.InsertCommand.Transaction.Rollback();
                sqlada.InsertCommand.Connection.Close();
                throw new Exception("导入数据失败！" + ex.Message);
            }
            return count;
        }
        private static int ImportTable(DataTable source, DataTable target, OracleDataAdapter oraada)
        {
            int count = 0;
            try
            {
                if (oraada.InsertCommand.Connection.State != ConnectionState.Open)
                    oraada.InsertCommand.Connection.Open();
                oraada.InsertCommand.Transaction = oraada.InsertCommand.Connection.BeginTransaction();
                int i = 0;
                while (i < source.Rows.Count)   //for( int i=0;i<;i++)
                {
                    for (int c = 0; c < target.Columns.Count; c++)
                    {
                        oraada.InsertCommand.Parameters[c].Value = source.Rows[i][c];
                    }
                    //					if( i>6 )
                    //						throw new Exception( "Test！" );
                    try//个别记录失败，跳过
                    {
                        oraada.InsertCommand.ExecuteNonQuery();
                    }
                    catch
                    {
                        i++;
                        continue;
                    }
                    count++; //已导入的记录数
                    source.Rows.RemoveAt(i);
                }
                oraada.InsertCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                if (oraada.InsertCommand.Transaction != null)
                    oraada.InsertCommand.Transaction.Rollback();
                oraada.InsertCommand.Connection.Close();
                throw new Exception("导入数据失败！" + ex.Message);
            }
            return count;
        }

        public static string GenerFirstTableName(string fileName)
        {
            return DA_DbLoad.GenerTableNameByIndex(fileName, 0);
        }
        public static string GenerTableNameByIndex(string fileName, int index)
        {
            return DA_DbLoad.GenerTableNameByIndex(fileName, index);
        }
        public static string[] GenerTableNames(string fileName)
        {
            return DA_DbLoad.GenerTableNames(fileName);
        }
        public static DataTable ReadExcelFileToDataTable(string fileFullName, int sheetIdx = 0)
        {
            string tableName = GenerTableNameByIndex(fileFullName, sheetIdx);
            return ReadExcelFileToDataTableBySQL(fileFullName, tableName);
        }
        public static DataTable ReadExcelFileToDataTable(string fileFullName, string tableName)
        {
            return ReadExcelFileToDataTableBySQL(fileFullName, tableName);
        }
        /// <summary>
        /// 通过文件，sql ,取出Table.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ReadExcelFileToDataTableBySQL(string filePath, string tableName)
        {
            return DA_DbLoad.ReadExcelFileToDataTableBySQL(filePath, tableName);
        }
    }
}
