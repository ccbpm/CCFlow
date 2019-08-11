using System;
using System.IO;
using System.Data;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;

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
		   
        public static string GenerFirstTableName(string fileName)
        {
            return GenerTableNameByIndex(fileName, 0);
        }
        public static string GenerTableNameByIndex(string fileName,int index)
        {
            String[] excelSheets = GenerTableNames(fileName);
            if (excelSheets != null )
                return excelSheets[index];

            if (excelSheets.Length < index)
                throw new Exception("err@table的索引号错误" + index + "最大索引号为:" + excelSheets.Length);

            return null;
        }
        public static string[] GenerTableNames(string fileName)
        {
            string strConn = "Provider=Microsoft.Jet.Oledb.4.0;Data Source=" + fileName + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
            try
            {
                if (fileName.ToLower().Contains(".xlsx"))
                {
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\"";
                }

                OleDbConnection con = new OleDbConnection(strConn);
                con.Open();
                //计算出有多少个工作表sheet   
                DataTable dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dt == null)
                    return null;
                               
                String[] excelSheets = new String[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    excelSheets[i] = dt.Rows[i]["TABLE_NAME"].ToString();
                }

                con.Close();
                con.Dispose();
                return excelSheets;
            }
            catch (Exception ex)
            {
                throw new Exception("@获取table出错误：" + ex.Message + strConn);
            }
        }
        public static DataTable ReadExcelFileToDataTable(string fileFullName,int sheetIdx = 0)
        {
            string tableName = GenerTableNameByIndex(fileFullName, sheetIdx);
            string sql = "SELECT * FROM [" + tableName + "]";
            return ReadExcelFileToDataTableBySQL(fileFullName, sql);
        }
        public static DataTable ReadExcelFileToDataTable(string fileFullName, string tableName)
        {
            string sql = "SELECT * FROM [" + tableName + "]";
            return ReadExcelFileToDataTableBySQL(fileFullName, sql);
        }
		/// <summary>
		/// 通过文件，sql ,取出Table.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
        public static DataTable ReadExcelFileToDataTableBySQL(string filePath, string sql)
        {
            DataTable dt = new DataTable("dt");

            string typ = System.IO.Path.GetExtension(filePath).ToLower();
            string strConn;
            switch (typ.ToLower() )
            {
                case ".xls":
                    if (sql == null)
                    {
                        sql = "SELECT * FROM [" + GenerFirstTableName(filePath) + "]";
                    }

                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source =" + filePath + ";Extended Properties=Excel 8.0";
                    System.Data.OleDb.OleDbConnection conn = new OleDbConnection(strConn);
                    OleDbDataAdapter ada = new OleDbDataAdapter(sql, conn);
                    try
                    {
                        conn.Open();
                        ada.Fill(dt);
                        dt.TableName = Path.GetFileNameWithoutExtension(filePath);
                    }
                    catch (System.Exception ex)
                    {
                        conn.Close();
                        throw ex;//(ex.Message);
                    }
                    conn.Close();
                    return dt;
                case ".xlsx":
                    if (sql == null)
                        sql = "SELECT * FROM [" + GenerFirstTableName(filePath)+"]";
                    try
                    {
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES\"";
                        System.Data.OleDb.OleDbConnection conn121 = new OleDbConnection(strConn);
                        OleDbDataAdapter ada91 = new OleDbDataAdapter(sql, conn121);
                        conn121.Open();
                        ada91.Fill(dt);
                        dt.TableName = Path.GetFileNameWithoutExtension(filePath);
                        conn121.Close();
                        ada91.Dispose();
                    }
                    catch (System.Exception ex1)
                    {
                        try
                        {
                            strConn = "Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\"";
                            System.Data.OleDb.OleDbConnection conn1215 = new OleDbConnection(strConn);
                            OleDbDataAdapter ada919 = new OleDbDataAdapter(sql, conn1215);
                            ada919.Fill(dt);
                            dt.TableName = Path.GetFileNameWithoutExtension(filePath);
                            ada919.Dispose();
                            conn1215.Close();
                        }
                        catch
                        {

                        }
                        throw ex1;//(ex.Message);
                    }
                    return dt;
                case ".dbf":
                    strConn = "Driver={Microsoft dBASE Driver (*.DBF)};DBQ=" + System.IO.Path.GetDirectoryName(filePath) + "\\"; //+FilePath;//
                    OdbcConnection conn1 = new OdbcConnection(strConn);
                    OdbcDataAdapter ada1 = new OdbcDataAdapter(sql, conn1);
                    conn1.Open();
                    try
                    {
                        ada1.Fill(dt);
                    }
                    catch//(System.Exception ex)
                    {
                        try
                        {
                            int sel = ada1.SelectCommand.CommandText.ToLower().IndexOf("select") + 6;
                            int from = ada1.SelectCommand.CommandText.ToLower().IndexOf("from");
                            ada1.SelectCommand.CommandText = ada1.SelectCommand.CommandText.Remove(sel, from - sel);
                            ada1.SelectCommand.CommandText = ada1.SelectCommand.CommandText.Insert(sel, " top 10 * ");
                            ada1.Fill(dt);
                            dt.TableName = "error";
                        }
                        catch (System.Exception ex)
                        {
                            conn1.Close();
                            throw new Exception("读取DBF数据失败！" + ex.Message + " SQL:" + sql);
                        }
                    }
                    conn1.Close();
                    return dt;
                default:
                    break;
            }
            return dt;
        }
	}
}
