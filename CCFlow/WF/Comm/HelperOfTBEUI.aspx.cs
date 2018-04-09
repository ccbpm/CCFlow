using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using System.Text;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using System.IO;

namespace CCFlow.WF.Comm
{
    public partial class HelperOfTBEUI : System.Web.UI.Page
    {
        #region
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        public int WordsSort
        {
            get
            {
                try
                {
                    return int.Parse(this.getUTF8ToString("WordsSort"));
                }
                catch
                {
                    return 0;
                }
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        /// <summary>
        /// 节点字段
        /// </summary>
        public string NodeAttrKey
        {
            get
            {
                if (this.Request.QueryString["attrKey"] == "undefined")
                    return "null";
                return this.Request.QueryString["attrKey"];
            }
        }
        public string operateFolder
        {
            get
            {
                string path = BP.Sys.SystemConfig.PathOfDataUser + "Fastenter\\" + FK_MapData + "\\" + NodeAttrKey;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);

                }
                string[] folderArray = Directory.GetFiles(path);

                string liStr = "[";
                string fileName;
                string[] strArray;
                if (folderArray.Length == 0)
                {
                    return "[]";
                }
                else
                {
                    foreach (string folder in folderArray)
                    {
                        strArray = folder.Split('\\');
                        fileName = strArray[strArray.Length - 1].Replace("\"", "").Replace("'", "");
                        liStr += string.Format("{{id:\"{0}\",value:\"{1}\"}},", GetFilteredStrForJSON(fileName, true), GetFilteredStrForJSON(File.ReadAllText(folder, System.Text.Encoding.Default), false));
                        //liStr += "<li onclick='fun(this)' id='" + File.ReadAllText(folder, System.Text.Encoding.Default).Replace("\"", "").Replace("'", "") + "'>" + fileName + "</li>";
                    }
                    liStr = liStr.TrimEnd(',') + "]";
                }
                return liStr;
            }
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (DataType.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getData":
                    s_responsetext = getData();
                    break;
                case "addData":
                    s_responsetext = addData();
                    break;
                case "editData":
                    s_responsetext = editData();
                    break;
                case "deleteData":
                    s_responsetext = deleteData();
                    break;
                case "saveHistoryData":
                    s_responsetext = saveHistoryData();
                    break;
            }
            if (DataType.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        /// <summary>
        /// 保存历史数据
        /// </summary>
        /// <returns></returns>
        private string saveHistoryData()
        {
            string lb = getUTF8ToString("lb");
            if (lb == "readWords" || lb == "hisWords")
                return "true";

            string enName = getUTF8ToString("FK_MapData");
            string AttrKey = getUTF8ToString("AttrKey");
            string str = getUTF8ToString("str");


            string sql = "select * from Sys_UserRegedit where LB='2' and FK_Emp='" + WebUser.No
                        + "' and FK_MapData='" + enName + "' and AttrKey='" + AttrKey + "' and CurValue='" + str + "'";

            if (DBAccess.RunSQLReturnCOUNT(sql) != 0)//禁止添加重复数据
                return "true";

            sql = "select * from Sys_UserRegedit where LB='2' and FK_Emp='" + WebUser.No
                        + "' and FK_MapData='" + enName + "' and AttrKey='" + AttrKey + "'";

            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            DefVal dv = new DefVal();
            if (dt.Rows.Count == 50)//动态更新数据，限制50条
            {
                try
                {
                    int minOid = int.Parse(dt.Rows[0]["OID"].ToString());

                    foreach (DataRow dr in dt.Rows)
                    {
                        int drOid = int.Parse(dr["OID"].ToString());
                        if (minOid > drOid)
                            minOid = drOid;
                    }

                    dv = new DefVal();
                    dv.RetrieveByAttr(DefValAttr.MyPK, minOid);

                    dv.Delete();
                }
                catch (Exception)
                {
                    if (dt.Rows.Count != 0)
                        return "false";
                }
            }

            dv = new DefVal();
            dv.FK_MapData = enName;
            dv.AttrKey = AttrKey;
            dv.LB = "2";
            dv.FK_Emp = WebUser.No;
            dv.CurValue = str;

            dv.Insert();

            return "true";
        }
        /// <summary>
        /// 根据oid删除
        /// </summary>
        /// <returns></returns>
        private string deleteData()
        {
            string oids = getUTF8ToString("oids");
            if (DataType.IsNullOrEmpty(oids))
                return "false";

            string lb = getUTF8ToString("lb");
            if (lb == "readWords" || lb == "hisWords")
                return "false";

            try
            {
                string[] oidsArray = oids.Split(',');

                foreach (string oid in oidsArray)
                {
                    if (DataType.IsNullOrEmpty(oid))
                        continue;

                    DefVal dv = new DefVal();
                    dv.RetrieveByAttr(DefValAttr.MyPK, oid);

                    dv.Delete();
                }

                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        private string editData()
        {
            string lb = getUTF8ToString("lb");
            if (lb == "readWords" || lb == "hisWords")//文件,历史词汇
                return "false";

            string oid = getUTF8ToString("oid");
            string text = getUTF8ToString("text");
            try
            {
                DefVal dv = new DefVal();
                dv.RetrieveByAttr(DefValAttr.MyPK, oid);
                dv.CurValue = text;
                dv.Update();

                return "true";
            }
            catch (Exception)
            {
                return "false";
            }
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <returns></returns>
        private string addData()
        {
            string lb = getUTF8ToString("lb");
            if (lb == "readWords" || lb == "hisWords")//文件,历史词汇
                return "false";

            string text = getUTF8ToString("text");
            text = DataTableConvertJson.GetFilteredStrForJSON(text);
            if (DataType.IsNullOrEmpty(text))
            {
                return "false";
            }

            string enName = getUTF8ToString("FK_MapData");
            string AttrKey = getUTF8ToString("AttrKey");


            string lbStr = "";
            string fk_emp = "";
            if (lb == "myWords")//我的词汇
            {
                lbStr = "1";
                fk_emp = WebUser.No;
            }
            if (lb == "sysWords")//系统词汇
            {
                lbStr = "3";
                fk_emp = "";
            }

            string addQue = " and FK_MapData='" + enName + "' and AttrKey='" + AttrKey + "' and CurValue='" + text + "'";
            string sql = "select * from Sys_UserRegedit where LB='" + lbStr + "' and FK_Emp='" + fk_emp + "'" + addQue;
            if (DBAccess.RunSQLReturnCOUNT(sql) != 0)
                return "false";

            try
            {
                DefVal dv = new DefVal();
                dv.FK_MapData = enName;
                dv.AttrKey = AttrKey;
                dv.LB = lbStr;
                dv.FK_Emp = fk_emp;
                dv.CurValue = text;
                dv.Insert();
            }
            catch
            {
                DefVal dv = new DefVal();
                dv.RunSQL("drop table Sys_UserRegedit");
                dv.CheckPhysicsTable();
            }

            return "true";
        }
        //获取数据
        private string getData()
        {
            DefVal dv = new DefVal();
            dv.CheckPhysicsTable();

            string enName = getUTF8ToString("FK_MapData");
            string AttrKey = getUTF8ToString("AttrKey");
            string lb = getUTF8ToString("lb");

            if (lb == "readWords")//读取txt文件
            {
                return readTxt();
            }
            try
            {
                DataTable dt = new DataTable();
                string sql = "";

                string addQue = "";//公用sql查询条件
                addQue = " and FK_MapData='" + enName + "' and AttrKey='" + AttrKey + "'";

                if (lb == "myWords")//我的词汇
                    sql = "select * from Sys_UserRegedit where LB='1' and FK_Emp='" + WebUser.No + "'" + addQue;

                if (lb == "hisWords")//历史词汇
                    sql = "select * from Sys_UserRegedit where LB='2' and FK_Emp='" + WebUser.No + "'" + addQue;

                if (lb == "sysWords")//系统词汇
                    switch (DBAccess.AppCenterDBType)
                    {
                        case DBType.Oracle:
                            sql = "select * from Sys_UserRegedit where LB='3' and FK_Emp is null" + addQue;
                            break;
                            case DBType.MSSQL:
                            sql = "select * from Sys_UserRegedit where LB='3' and FK_Emp=''" + addQue;
                            break;

                    }
                    


                string pageNumber = getUTF8ToString("pageNumber");
                int iPageNumber = DataType.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                //每页多少行
                string pageSize = getUTF8ToString("pageSize");
                int iPageSize = DataType.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);


                switch (DBAccess.AppCenterDBType)
                {
                    case DBType.Oracle:
                    case DBType.MSSQL:
                        return DBPaging("(" + sql + ")sqlStr", iPageNumber, iPageSize, "MyPK", "MyPK");
                    case DBType.MySQL:
                        return DBPaging("(" + sql + " order by MyPK DESC )sqlStr", iPageNumber, iPageSize, "MyPK", "");
                    default:
                        throw new Exception("暂不支持您的数据库类型.");
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 注意特殊字符的处理
        /// </summary>
        /// <returns></returns>
        private string readTxt()
        {
            try
            {
                string path = BP.Sys.SystemConfig.PathOfDataUser + "Fastenter\\" + FK_MapData + "\\" + NodeAttrKey;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string[] folderArray = Directory.GetFiles(path);
                if (folderArray.Length == 0)
                    return "";

                string fileName;
                string[] strArray;

                string pageNumber = getUTF8ToString("pageNumber");
                int iPageNumber = DataType.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                string pageSize = getUTF8ToString("pageSize");
                int iPageSize = DataType.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);


                DataTable dt = new DataTable();
                dt.Columns.Add("OID", typeof(string));
                dt.Columns.Add("TxtStr", typeof(string));
                dt.Columns.Add("CurValue", typeof(string));

                string liStr = "";
                int count = 0;
                int index = iPageSize * (iPageNumber - 1);
                foreach (string folder in folderArray)
                {
                    dt.Rows.Add("", "", "");
                    if (count >= index && count < iPageSize * iPageNumber)
                    {
                        dt.Rows[count]["OID"] = BP.DA.DBAccess.GenerGUID();

                        strArray = folder.Split('\\');
                        fileName = strArray[strArray.Length - 1].Replace("\"", "").Replace("'", "");
                        liStr += string.Format("{{id:\"{0}\",value:\"{1}\"}},", GetFilteredStrForJSON(fileName, true),
                            GetFilteredStrForJSON(File.ReadAllText(folder, System.Text.Encoding.Default), false));

                        dt.Rows[count]["CurValue"] = GetFilteredStrForJSON(fileName, true);
                        dt.Rows[count]["TxtStr"] = GetFilteredStrForJSON(File.ReadAllText(folder, System.Text.Encoding.Default), false);
                    }
                    count += 1;
                }

                return DataTable2Json(dt, folderArray.Length);
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string DataTable2Json(DataTable dt, int totalRows)
        {
            StringBuilder jsonBuilder = new StringBuilder();

            jsonBuilder.Append("{rows:[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (DataType.IsNullOrEmpty(dt.Rows[i]["OID"].ToString()))
                    continue;

                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append(":");
                    jsonBuilder.Append("'" + DataTableConvertJson.GetFilteredStrForJSON(dt.Rows[i][j].ToString()) + "'");
                    jsonBuilder.Append(",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            //不存在数据时
            if (jsonBuilder.Length > 7)
            {
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            //jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            if (totalRows == 0)
            {
                jsonBuilder.Append("],total:0");
            }
            else
            {
                jsonBuilder.Append("],total:" + totalRows);
            }
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 以下算法只包含 oracle mysql sqlserver 三种类型的数据库 qin
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pageNumber">当前页</param>
        /// <param name="pageSize">当前页数据条数</param>
        /// <param name="key">计算总行数需要</param>
        /// <param name="orderKey">排序字段(可以为空)</param>
        /// <returns></returns>
        private string DBPaging(string tableName, int pageNumber, int pageSize, string key, string orderKey)
        {
            string sql = "";
            string orderByStr = "";

            if (!DataType.IsNullOrEmpty(orderKey))
                orderByStr = " ORDER BY " + orderKey + " desc";

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    int beginIndex = (pageNumber - 1) * pageSize + 1;
                    int endIndex = pageNumber * pageSize;
                    sql = "SELECT * FROM ( SELECT A.*, ROWNUM RN " +
                        "FROM (SELECT * FROM  " + tableName + orderByStr + ") A WHERE ROWNUM <= " + endIndex + " ) WHERE RN >=" + beginIndex;
                    break;
                case DBType.MSSQL:
                    sql = "SELECT TOP " + pageSize + " * FROM " + tableName + " WHERE " + key + " NOT IN  ("
                    + "SELECT TOP (" + pageSize + "*(" + pageNumber + "-1)) " + key + " FROM " + tableName + orderByStr + ")" + orderByStr;
                    break;
                case DBType.MySQL:
                    int startIndex = (pageNumber - 1) * pageSize;
                    sql = "select * from  " + tableName + orderByStr + " limit " + startIndex + "," + pageSize;
                    break;
                default:
                    throw new Exception("暂不支持您的数据库类型.");
            }

            DataTable DTable = DBAccess.RunSQLReturnTable(sql);

            int totalCount = DBAccess.RunSQLReturnCOUNT("select " + key + " from " + tableName);

           // string re = BP.Tools.Json.DataTableToJson(dt);

            return DataTableConvertJson.DataTable2Json(DTable, totalCount);
        }
        /// <summary>
        /// 读取文件时必要的替换字符操作---全？？？
        /// </summary>
        /// <param name="getText"></param>
        /// <returns></returns>
        private string GetFilteredStrForJSON(string getText, bool checkId)
        {
            if (checkId)
            {
                //getText = getText.Replace("\r", "");
                getText = getText.Replace("\n", "");
            }
            else
            {
                getText = getText.Replace("\n", "ccflow_lover");
            }
            getText = getText.Replace("\r", "");
            getText = getText.Replace("{", "｛");
            getText = getText.Replace("}", "｝");
            getText = getText.Replace("[", "【");
            getText = getText.Replace("]", "】");
            getText = getText.Replace("\"", "”");
            getText = getText.Replace("\'", "‘");
            getText = getText.Replace("<", "《");
            getText = getText.Replace(">", "》");
            getText = getText.Replace("(", "（");
            getText = getText.Replace(")", "）");
            return getText;
        }
    }
}