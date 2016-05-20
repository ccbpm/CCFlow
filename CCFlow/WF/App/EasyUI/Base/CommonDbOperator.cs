using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Security;
using System.IO;
using System.Reflection;

namespace CCFlow.AppDemoLigerUI.Base
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
                strAppend.Append("{Rows:[");

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i > 0) strAppend.Append(",");
                    DataRowToJson(dt.Rows[i], strAppend);
                }
                strAppend.Append("],Total:" + dt.Rows.Count + "}");
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
                        if (temp == null || string.IsNullOrEmpty(temp.ToString()))
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
            if (!string.IsNullOrEmpty(str))
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

            if (value != null && !string.IsNullOrEmpty(value.ToString()))
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

        public static Dictionary<string, string> GetParams(Stream values)
        {
            Dictionary<string, string> Params = new Dictionary<string, string>();
            if (values != null)
            {
                string s = "";
                using (StreamReader sr = new StreamReader(values))
                {
                    s = sr.ReadToEnd();
                    s = s.Replace("&nbsp;", "#nbsp;");
                }
                NameValueCollection qs = HttpUtility.ParseQueryString(s);
                if (qs.Count > 0)
                {
                    string tempValue = null;
                    foreach (string key in qs.AllKeys)
                    {
                        tempValue = qs[key];
                        if (tempValue != null && tempValue.Equals("null", StringComparison.OrdinalIgnoreCase))
                            tempValue = "";
                        Params.Add(key.ToUpper(), tempValue.Replace("#nbsp;", "&nbsp;"));
                    }
                }
            }
            return Params;
        }
    }
}