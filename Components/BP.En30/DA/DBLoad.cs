using System;
using System.IO;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Data.SqlClient;
using BP.Difference;

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
    
        /// <summary>
        /// 按照顺序获得名字
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GenerTableNameByIndex(string fileName, int index)
        {
            return DA_DbLoad.GenerTableNameByIndex(fileName, index);
        }
        /// <summary>
        /// 获得名字集合
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] GenerTableNames(string fileName)
        {
            return DA_DbLoad.GenerTableNames(fileName);
        }
        /// <summary>
        /// 获得excel文件中的数据，按照指定顺序号的idx.
        /// </summary>
        /// <param name="fileFullName">绝对文件路径</param>
        /// <param name="sheetIdx">顺序号</param>
        /// <returns>返回数据</returns>
        public static DataTable ReadExcelFileToDataTable(string fileFullName, int sheetIdx = 0)
        {
            string tableName = GenerTableNameByIndex(fileFullName, sheetIdx);
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
