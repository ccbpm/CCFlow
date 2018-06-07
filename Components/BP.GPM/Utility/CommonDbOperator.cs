using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security;
using BP.DA;



namespace BP.GPM.Utility
{
    /// <summary>
    ///CommonDbOperator 的摘要说明
    /// </summary>
    public class CommonDbOperator
    {
        #region"转JSON方法"

        /// <summary>
        /// 暂时用于datatable序列化为json的土办法
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetJsonFromTable(DataTable dt)
        {
            if (dt != null)
            {
                StringBuilder strAppend = new StringBuilder();
                strAppend.Append("{rows:[");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) strAppend.Append(",");
                    DataRowToJson(dt.Rows[i], strAppend);
                }
                strAppend.Append("],total:" + dt.Rows.Count + "}");
                return strAppend.ToString();
            }
            return "[]";
        }

        public static string GetListJsonFromTable(DataTable dt)
        {
            if (dt != null)
            {
                StringBuilder strAppend = new StringBuilder();
                strAppend.Append("[");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) strAppend.Append(",");
                    DataRowToJson(dt.Rows[i], strAppend);
                }
                strAppend.Append("]");
                return strAppend.ToString();
            }
            return "[]";
        }

        /// <summary>
        /// 获取树形结构
        /// </summary>
        /// <param name="dt">数据集合</param>
        /// <param name="columnName">查询列明</param>
        /// <param name="id">根据值查询</param>
        /// <param name="appendChild">是否拼接子节点</param>
        /// <returns></returns>
        public static string GetGridTreeDataString(DataTable dt, string pColumnName, string cColumnName, string id, bool appendChild)
        {
            StringBuilder stringbuilder = new StringBuilder();
            DataRow[] CRow = dt.Select(pColumnName + "='" + id + "'");
            if (CRow.Length > 0)
            {
                stringbuilder.Append("[");
                for (int i = 0; i < CRow.Length; i++)
                {
                    string chidstring = GetGridTreeDataString(dt, pColumnName, cColumnName, CRow[i][cColumnName].ToString(), appendChild);
                    if (!DataType.IsNullOrEmpty(chidstring) && appendChild == true)
                    {
                        DataRowToJson(CRow[i], stringbuilder);
                        stringbuilder.Replace('}', ',', stringbuilder.Length - 1, 1);
                        stringbuilder.Append("children:");
                        stringbuilder.Append(chidstring);
                    }
                    else
                    {
                        DataRowToJson(CRow[i], stringbuilder);
                        stringbuilder.Append(",");
                    }
                }
                stringbuilder.Replace(',', ' ', stringbuilder.Length - 1, 1);
                stringbuilder.Append("]},");
            }
            return stringbuilder.ToString();
        }
        static void DataRowToJson(DataRow dr, StringBuilder strAppend)
        {
            if (dr != null && dr.Table.Columns.Count > 0)
            {
                strAppend.Append("{");
                foreach (DataColumn col in dr.Table.Columns)
                {
                    if (col.Ordinal > 0) strAppend.Append(",");
                    //sw.Write("\"");
                    strAppend.Append(col.ColumnName.ToUpper());
                    if (col.DataType.Equals(typeof(int)) || col.DataType.Equals(typeof(decimal)))
                    {
                        strAppend.Append(":");
                        strAppend.Append(dr[col]);
                        if (dr[col] == null || dr[col] == DBNull.Value)
                        {
                            strAppend.Append("\"\"");
                        }
                    }
                    else if (col.DataType.Equals(typeof(bool)))
                    {
                        strAppend.Append(":");
                        strAppend.Append(dr[col].ToString().ToLower());
                    }
                    else
                    {
                        strAppend.Append(":\"");
                        strAppend.Append(ChangeJsonValue(dr[col]));
                        strAppend.Append("\"");
                    }
                }
                if (dr.Table.Columns.Contains("MenuType"))
                {
                    strAppend.Append(",iconCls:\"");
                    strAppend.Append("icon-" + dr["MenuType"].ToString());
                    strAppend.Append("\"");
                }
                if (dr.Table.Columns.Contains("ICON"))
                {
                    strAppend.Append(string.Format(",iconCls:\"{0}\"", dr["ICON"].ToString()));
                }
                //strAppend.Append(",state:\"");
                //strAppend.Append("closed");
                //strAppend.Append("\"");
                strAppend.Append("}");
            }
        }

        public static string GetJsonString<T>(List<T> list)
        {
            if (list != null)
            {
                StringBuilder jsonString = new StringBuilder();
                jsonString.Append("[");
                foreach (T obj in list)
                {
                    if (jsonString.Length > 1) jsonString.Append(",");
                    jsonString.Append(GetJsonString<T>(obj));
                }
                jsonString.Append("]");
                return jsonString.ToString();
            }
            return null;
        }

        public static string GetJsonString<T>(T mapObject)
        {
            if (mapObject != null)
            {
                StringBuilder jsonString = new StringBuilder();
                PropertyInfo[] pInfo = mapObject.GetType().GetProperties();
                jsonString.Append("{");
                foreach (PropertyInfo property in pInfo)
                {
                    if (jsonString.Length > 1) jsonString.Append(",");
                    jsonString.Append("\"");
                    jsonString.Append(property.Name);
                    if (property.PropertyType.Equals(typeof(int)) || property.PropertyType.Equals(typeof(decimal)))
                    {
                        jsonString.Append("\":");
                        object temp = property.GetValue(mapObject, null);
                        jsonString.Append(temp);
                        if (temp == null || DataType.IsNullOrEmpty(temp.ToString()))
                        {
                            jsonString.Append("\"\"");
                        }
                    }
                    else if (property.PropertyType.Equals(typeof(bool)))
                    {
                        jsonString.Append("\":");
                        jsonString.Append(property.GetValue(mapObject, null).ToString().ToLower());
                    }
                    else
                    {
                        jsonString.Append("\":\"");
                        jsonString.Append(ChangeJsonValue(property.GetValue(mapObject, null)));
                        jsonString.Append("\"");
                    }
                }
                jsonString.Append("}");
                return jsonString.ToString();
            }
            return null;
        }

        public static Stream GetStream(string str)
        {
            if (!DataType.IsNullOrEmpty(str))
            {
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(str);
                sw.Flush();
                ms.Position = 0;
                return ms;
            }
            return null;
        }

        /// <summary>
        /// 处理Json的特殊字符
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string ChangeJsonValue(object value)
        {
            string tempStr = null;
            string tempTag = "[TEMP-$1]";
            //这里由于多个转义字符互相冲突，所以先转为临时标记

            if (value != null && !DataType.IsNullOrEmpty(value.ToString()))
            {
                tempStr = value.ToString();
                if (!tempStr.Contains("0001-1-1"))
                {
                    tempStr = tempStr.Replace("\r\n", "<br>");
                    tempStr = tempStr.Replace("\n", "<br>");
                    tempStr = tempStr.Replace("\f", tempTag + "f");
                    tempStr = tempStr.Replace("\r", tempTag + "r");
                    tempStr = tempStr.Replace("\t", tempTag + "t");
                    tempStr = tempStr.Replace("\"", tempTag + "\"");
                    tempStr = tempStr.Replace("/", tempTag + "/");
                    tempStr = tempStr.Replace("\\", "\\\\");
                    tempStr = tempStr.Replace(tempTag, "\\");
                }
                else
                    tempStr = null;
            }
            return tempStr;
        }
        #endregion
    }

    /// <summary>
    /// EasyUI Tree节点
    /// </summary>
    [Serializable]
    public class TreeNode
    {
        public TreeNode()
        {
            _state = "open";
        }
        #region Model
        private string _id;
        private string _text;
        private string _state;
        private string _iconCls;
        private object _children;
        private object _attributes;
        /// <summary>
        /// ID
        /// </summary>
        public string id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string text
        {
            set { _text = value; }
            get { return _text; }
        }
        /// <summary>
        /// 'open' or 'closed', default is 'open'.
        /// </summary>
        public string state
        {
            set { _state = value; }
            get { return _state; }
        }

        /// <summary>
        /// 图标
        /// </summary>
        public string iconCls
        {
            set { _iconCls = value; }
            get { return _iconCls; }
        }

        /// <summary>
        /// 子栏目
        /// </summary>
        public object children
        {
            set { _children = value; }
            get { return _children; }
        }

        /// <summary>
        /// 其他属性
        /// </summary>
        public object attributes
        {
            set { _attributes = value; }
            get { return _attributes; }
        }

        #endregion
    }
}
