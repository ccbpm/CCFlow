using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.Serialization.Json;
using BP.DA;
using System.Web;
using Newtonsoft.Json;

namespace BP.Tools
{
    /// <summary>
    /// Summary description for FormatToJson
    /// </summary>
    public class FormatToJson
    {
        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// 将JSON解析成DataSet只限标准的JSON数据
        /// 例如：Json＝{t1:[{name:'数据name',type:'数据type'}]} 或 Json＝{t1:[{name:'数据name',type:'数据type'}],t2:[{id:'数据id',gx:'数据gx',val:'数据val'}]}
        /// </summary>
        /// <param name="Json">Json字符串</param>
        /// <returns>DataSet</returns>
        public static DataSet JsonToDataSet(string Json)
        {
            try
            {
                DataSet ds = new DataSet();
                //JavaScriptSerializer Serializer = new JavaScriptSerializer();
                //object objs = Serializer.Deserialize(Json);
                //2019/7/24 zyt改造JavaScriptSerializer To JsonConvert,Core And Fram4编译正常
                object obj = JsonConvert.DeserializeObject(Json);
                Dictionary<string, object> datajson = (Dictionary<string, object>)obj;

                foreach (var item in datajson)
                {
                    DataTable dt = new DataTable(item.Key);
                    object[] rows = (object[])item.Value;
                    foreach (var row in rows)
                    {
                        Dictionary<string, object> val = (Dictionary<string, object>)row;
                        DataRow dr = dt.NewRow();
                        foreach (KeyValuePair<string, object> sss in val)
                        {
                            if (!dt.Columns.Contains(sss.Key))
                            {
                                dt.Columns.Add(sss.Key);
                                dr[sss.Key] = sss.Value;
                            }
                            else
                                dr[sss.Key] = sss.Value;
                        }
                        dt.Rows.Add(dr);
                    }
                    ds.Tables.Add(dt);
                }
                return ds;
            }
            catch
            {
                return null;
            }
        }

        //反实例化json
        public static T ParseFromJson<T>(string szJson)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(szJson)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        //反实例化json
        public static List<T> ParseListFromJson<T>(string szJson)
        {
            //JavaScriptSerializer Serializer = new JavaScriptSerializer();
            //List<T> objs = Serializer.Deserialize<List<T>>(szJson);
            //2019/7/24 zyt改造JavaScriptSerializer To JsonConvert,Core And Fram4编译正常
            List<T> objs =JsonConvert.DeserializeObject<List<T>>(szJson);
            return objs;
        }

        /// <summary>  
        /// Json 字符串 转换为 DataTable数据集合  
        /// </summary>  
        /// <param name="json"></param>  
        /// <returns></returns>  
        public static DataTable ToDataTable(string json)
        {
            DataTable dataTable = new DataTable();  //实例化  
            try
            {
                //JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                //javascriptserializer.maxjsonlength = int32.maxvalue; //取得最大数值  
                //ArrayList arrayList = javaScriptSerializer.Deserialize<ArrayList>(json);
                //2019/7/24 zyt改造JavaScriptSerializer To JsonConvert,Core And Fram4编译正常
                ArrayList arrayList = JsonConvert.DeserializeObject<ArrayList>(json);

                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count<string>() == 0)
                        {
                            return dataTable;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                if (dictionary[current] == null)
                                {
                                    dataTable.Columns.Add(current, typeof(string));
                                    continue;
                                }
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            if (dictionary[current] == null)
                            {
                                dataTable.Columns[current].AllowDBNull = true;
                                dataRow[current] = DBNull.Value;
                                continue;
                            }
                            dataRow[current] = dictionary[current];
                        }
                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中  
                    }
                }
            }
            catch
            {
            }
            return dataTable;
        }
        #endregion

        public FormatToJson()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary> 
        /// List转成json 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="jsonName"></param> 
        /// <param name="list"></param> 
        /// <returns></returns> 
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (DataType.IsNullOrEmpty(jsonName))
                jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    PropertyInfo[] pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (int j = 0; j < pi.Length; j++)
                    {
                        Type type = pi[j].GetValue(list[i], null).GetType();
                        Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));
                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        /// <summary> 
        /// List转成json 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="list"></param> 
        /// <returns></returns> 
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson<T>(list, obj.GetType().Name);
        }

        /// <summary>
        /// 将实体类转为Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ParseJsonFromEntity<T>(T obj)
        {
            //将对象序列化json  
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                //将json字符串写入内存流中  
                serializer.WriteObject(ms, obj);
                string szJson = Encoding.UTF8.GetString(ms.ToArray());
                return szJson;
            }
        }

        /// <summary> 
        /// 对象转换为Json字符串 
        /// </summary> 
        /// <param name="jsonObject">对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(object jsonObject)
        {
            string jsonString = "{";
            PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                string value = string.Empty;
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value = "'" + objectValue.ToString() + "'";
                }
                else if (objectValue is string)
                {
                    value = "'" + ToJson(objectValue.ToString()) + "'";
                }
                else if (objectValue is IEnumerable)
                {
                    value = ToJson((IEnumerable)objectValue);
                }
                else
                {
                    if (objectValue == null)
                        value = "";
                    else
                        value = ToJson(objectValue.ToString());
                }
                jsonString += "\"" + ToJson(propertyInfo[i].Name) + "\":" + value + ",";
            }
            jsonString = jsonString.Substring(0, jsonString.Length - 1);
            return jsonString + "}";
        }
        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson(item) + ",";
            }
            jsonString = jsonString.Substring(0, jsonString.Length - 1);
            return jsonString + "]";
        }
        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson_FromDictionary(IDictionary<string, object> array)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("{");
            foreach (string key in array.Keys)
            {
                object objectValue = array[key];
                if (objectValue is string)
                {
                    jsonString.AppendFormat("\"{0}\":\"{1}\",", key, objectValue.ToString());
                }
                else if (objectValue is List<string>)
                {
                    List<string> list = objectValue as List<string>;
                    jsonString.Append("\"" + key + "\":[");
                    foreach (string item in list)
                    {
                        jsonString.Append(item + ",");
                    }
                    if (list != null && list.Count > 0)
                        jsonString.Remove(jsonString.Length - 1, 1);
                    jsonString.Append("],");
                }
                else
                {
                    jsonString.AppendFormat("\"{0}\":\"{1}\",", key, objectValue.ToString());
                }
            }
            if (jsonString.Length > 1)
                jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("}");
            return jsonString.ToString();
        }
        /// <summary> 
        /// 普通集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString = ToJson(item.ToString()) + ",";
            }
            jsonString.Remove(jsonString.Length - 1, jsonString.Length);
            return jsonString + "]";
        }
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataTable dt, bool isUpper = true)
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

                    if (isUpper == true)
                        strKey = dt.Columns[j].ColumnName.ToUpper();
                    else
                        strKey = dt.Columns[j].ColumnName;


                    string strValue = drc[i][j] == null ? "" : drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
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
        /// DataTable转成Json 
        /// </summary> 
        /// <param name="jsonName"></param> 
        /// <param name="dt"></param> 
        /// <returns></returns> 
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (DataType.IsNullOrEmpty(jsonName))
                jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        string column = dt.Columns[j].ColumnName.ToString();
                        string val = dt.Rows[i][j] == null ? "" : dt.Rows[i][j].ToString();
                        val = StringFormat(val, type);
                        Json.Append("\"" + column + "\":" + val);
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        /// <summary> 
        /// DataReader转换为Json 
        /// </summary> 
        /// <param name="dataReader">DataReader对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DbDataReader dataReader)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Type type = dataReader.GetFieldType(i);
                    string strKey = dataReader.GetName(i);
                    string strValue = dataReader[i] == null ? "" : dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
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
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        /// <summary> 
        /// DataSet转换为Json  都是大写的列.
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName.ToUpper() + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }

        /// <summary>
        /// 把dataset转成json 列名大写.
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns>json字串.</returns>
        public static string DataSetToJsonUpper(DataSet dataSet)
        {
            return ToJson(dataSet);
        }
        /// <summary> 
        /// 过滤特殊字符 
        /// </summary> 
        /// <param name="s"></param> 
        /// <returns></returns> 
        public static string String2Json(String s)
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
            else if (type == typeof(System.Byte[]))
            {
                //数字字段需转string后进行拼接 
                str = "\"" + str + "\"";
            }
            if (str.Length == 0)
                str = "\"\"";

            return str;
        }
    }
}