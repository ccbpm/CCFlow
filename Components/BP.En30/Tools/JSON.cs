using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using BP.DA;
using Newtonsoft.Json;

namespace BP.Tools
{
     
    public class Json
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static string Hastable2Json_del(Hashtable ht)
        {
            return ToJsonEntityModel(ht);
        }
        /// <summary>
        /// 把一个json转化一个datatable
        /// </summary>
        /// <param name="json">一个json字符串</param>
        /// <returns>序列化的datatable</returns>
        public static DataTable ToDataTable(string strJson)
        {
            if (strJson.Trim().IndexOf('[') != 0)
            {
                strJson = "[" + strJson + "]";
            }
            DataTable dtt = (DataTable)JsonConvert.DeserializeObject<DataTable>(strJson);
            return dtt;
        }
        /// <summary>
        /// 把一个json转化一个datatable 杨玉慧
        /// </summary>
        /// <param name="json">一个json字符串</param>
        /// <returns>序列化的datatable</returns>
        public static DataTable ToDataTableOneRow(string strJson)
        {
            ////杨玉慧  写  用这个写
            if (strJson.Trim().IndexOf('[') != 0)
            {
                strJson = "[" + strJson + "]";
            }
            DataTable dtt = (DataTable)JsonConvert.DeserializeObject<DataTable>(strJson);
            return dtt;
        }
        /// <summary>
        /// 把dataset转成json 不区分大小写.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static string DataSetToJson(DataSet dataSet, bool isUpperColumn = true)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                if (isUpperColumn == true)
                    jsonString += "\"" + table.TableName + "\":" + DataTableToJson(table, true) + ",";
                else
                    jsonString += "\"" + table.TableName + "\":" + DataTableToJson(table, false) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string DataTableToJson(DataTable dt, bool isUpperColumn=true)
        {
            StringBuilder jsonString = new StringBuilder();
            if (dt.Rows.Count == 0)
            {
                jsonString.Append("[]");
                return jsonString.ToString();
            }

            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = null;
                    if (isUpperColumn == true)
                    {
                        strKey = dt.Columns[j].ColumnName;
                    }
                    else
                    {
                        strKey = dt.Columns[j].ColumnName;
                    }

                    Type type = dt.Columns[j].DataType;

                    string strValue="";
                    if (type == typeof(Single))
                    {
                        object v = drc[i][j];

                        if (v == null || v == DBNull.Value)
                        {
                            strValue = "0";
                        }
                        else
                        {
                            double f =(double)((float)v);
                            strValue = Convert.ToString(f);
                        }

                        //strValue = v == null ? "0" : v;
                        //strValue = drc[i][j] == null ? "" : ((float)(drc[i][j])).ToString("0.00");

                    }
                    else
                    {
                        strValue = drc[i][j] == null ? "" : drc[i][j].ToString();
                    }

                   
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        /// <summary> 
        /// 格式化字符型、日期型、布尔型 
        /// </summary> 
        /// <param name="str"></param> 
        /// <param name="type"></param> 
        /// <returns></returns> 
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = String2Json(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + Convert.ToDateTime(str).ToShortDateString() + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }

            if (str.Length == 0)
                str = "\"\"";

            return str;
        }
        /// <summary> 
        /// 过滤特殊字符 
        /// </summary> 
        /// <param name="s"></param> 
        /// <returns></returns> 
        private static string String2Json(String s)
        {
            System.Text.StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 把一个json转化一个datatable
        /// </summary>
        /// <param name="json">一个json字符串</param>
        /// <returns>序列化的datatable</returns>
        public static DataSet ToDataSet(string json)
        {
            ////杨玉慧  写  用这个写
            DataSet ds = JsonConvert.DeserializeObject<DataSet>(json);
            return ds;
        }
        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="jsonObject">对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(object jsonObject)
        {
            string json = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
            return json;
        }
        /// <summary>
        /// 对象集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(IEnumerable array)
        {
            string jsonStr = JsonConvert.SerializeObject(array);
            return jsonStr;
        }
        /// <summary>
        /// 普通集合转换Json
        /// </summary>
        /// <param name="array">集合对象</param>
        /// <returns>Json字符串</returns>
        public static string ToArrayString(IEnumerable array)
        {
            string jsonStr = JsonConvert.SerializeObject(array);
            return jsonStr;
             
        }
        /// <summary>
        /// 删除结尾字符
        /// </summary>
        /// <param name="str">需要删除的字符</param>
        /// <returns>完成后的字符串</returns>
        private static string DeleteLast(string str)
        {

            if (str.Length > 1)
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        }
        /// <summary>
        /// 把Ht转换成Entity模式.
        /// </summary>
        /// <param name="ht"></param>
        /// <returns></returns>
        public static string ToJsonEntityModel(Hashtable ht)
        {
            string strs = "{";
            foreach (string key in ht.Keys)
            {
                var val = ht[key];
                if (val == null)
                {
                    strs += "\"" + key + "\":\"\",";
                    continue;
                }

                if (val.GetType()==typeof(int)
                    || val.GetType() == typeof(decimal)
                    || val.GetType() == typeof(float))
                strs += "\"" + key + "\":" + ht[key] + ",";
                else
                    strs += "\"" + key + "\":\"" + ht[key] + "\",";

            }
            strs += "\"EndJSON\":\"0\"";
            strs += "}";
            strs = TranJsonStr(strs);
            return strs;
        }
        public static string ToJsonEntitiesNoNameMode(Hashtable ht)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");

            foreach (string key in ht.Keys)
            {
                DataRow dr = dt.NewRow();
                dr["No"] = key;
                dr["Name"] = ht[key];
                dt.Rows.Add(dr);

            }

            return BP.Tools.Json.DataTableToJson(dt, false);
        }
        /// <summary>
        /// 转化成Json.
        /// </summary>
        /// <param name="ht">Hashtable</param>
        /// <param name="isNoNameFormat">是否编号名称格式</param>
        /// <returns></returns>
        public static string ToJson(Hashtable ht)
        {
            return ToJsonEntityModel(ht);
        }
        /// <summary>
        /// Datatable转换为Json
        /// </summary>
        /// <param name="table">Datatable对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataTable table)
        {
            // 旧版本...
           return JsonConvert.SerializeObject(table);
        }
        /// <summary>
        /// DataSet转换为Json
        /// </summary>
        /// <param name="dataSet">DataSet对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(DataSet dataSet)
        {
            return  JsonConvert.SerializeObject(dataSet);
        }
        /// <summary>
        /// String转换为Json
        /// </summary>
        /// <param name="value">String对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(string value)
        {
            if (DataType.IsNullOrEmpty(value))
                return string.Empty;

            string temstr;
            temstr = value;
            temstr = temstr.Replace("{", "｛").Replace("}", "｝").Replace(":", "：").Replace(",", "，").Replace("[", "【").Replace("]", "】").Replace(";", "；").Replace("\n", "<br/>").Replace("\r", "");
            temstr = temstr.Replace("\t", "   ");
            temstr = temstr.Replace("'", "\'");
            temstr = temstr.Replace(@"\", @"\\");
            temstr = temstr.Replace("\"", "\"\"");
            return temstr;
        }
        /// <summary>
        /// JSON字符串的转义
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        private static string TranJsonStr(string jsonStr) {
            string strs = jsonStr;
            strs = strs.Replace("\\", "\\\\");
            strs = strs.Replace("\n", "\\n");
            strs = strs.Replace("\b", "\\b");
            strs = strs.Replace("\t", "\\t");
            strs = strs.Replace("\f", "\\f");
            strs = strs.Replace("\r", "\\r");
            strs = strs.Replace("/", "\\/");
            return strs;
        }
    }
}
