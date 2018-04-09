using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using BP.DA;

namespace CCFlow.WF.Comm.Port
{
    public class DataTableTree2Json
    {
        private const String LEVEL_COLUMN_NAME = "XXXYYYTREELEVELCOL";
        private System.Data.DataTable _DT;
        private string _IdName;
        private string _ParentIdName;
        private string _ChildNodeName;
        private string _RootParent;
        public string QuoteMark = "\'"; //可改为单引号 

        /// <summary>
        /// 如果为true,则所有字段按字符串处理，两边都会加上QuoteMark.
        /// 如果为false,则会检测字段类型及值类型，如果为数字或布尔型，则两边不加QuoteMark.
        /// </summary>
        public bool SetAllQuote = false;
        /// <summary>
        /// 格式化模式
        /// 0-不格式化
        /// 1-对象行格式化，每个对象一行
        /// 2-属性格式化，每个属性一行
        /// </summary>
        public int FormatMode = 1;
        /// <summary>
        /// 序列化方法 0-人工替换，1-Microsoft.Jscript序列化
        /// </summary>
        public int EscapeMethod = 0;
        /// <summary>
        /// 不加Quote的字段列表，当AllQuote为true时起作用。
        /// </summary>
        public string[] NoQuoteCols = null;
        /// <summary>
        /// 序列化字段
        /// </summary>
        public string[] EscapeCols = null;
        public DataTableTree2Json(System.Data.DataTable dt, string idName, string parentIdName, string childNodeName, string rootParent)
        {
            this._DT = dt;
            this._IdName = idName;
            this._ParentIdName = parentIdName;
            this._ChildNodeName = childNodeName;
            this._RootParent = rootParent;

            this.CheckInit();
            this.AddDegreeColumn();
        }
        private void CheckInit()
        {
            if (this._DT == null || DataType.IsNullOrEmpty(this._IdName) || DataType.IsNullOrEmpty(this._ParentIdName))
            {
                throw new ApplicationException("输入变量不能为空！");
            }
            if (DataType.IsNullOrEmpty(this._ChildNodeName))
                this._ChildNodeName = "Childs";
        }
        private void AddDegreeColumn()
        {
            this._DT.Columns.Add(LEVEL_COLUMN_NAME);
            string parentIdList = this._RootParent;
            for (int i = 0; i < 100; i++)
            {
                string con = null;
                if (DataType.IsNullOrEmpty(parentIdList))
                    con = String.Format("{0} is null", this._ParentIdName);
                else
                    con = String.Format("{0} in ({1})", this._ParentIdName, String.Format("'{0}'", parentIdList.Replace(",", "','")));
                DataRow[] drs = this._DT.Select(con);
                if (drs.Length == 0) break;
                parentIdList = String.Empty;
                foreach (DataRow dr in drs)
                {
                    dr[LEVEL_COLUMN_NAME] = i;
                    if (!DataType.IsNullOrEmpty(parentIdList)) parentIdList += ",";
                    parentIdList += Convert.ToString(dr[_IdName]);
                }
            }
        }
        /// <summary>
        /// 唯一对外接口
        /// </summary>
        /// <param name="parentId">当为Null时，定义RootParent，当为Convert.DBNull时，将执行Null值查询</param>
        /// <returns></returns>
        public string ToJsonStr(string parentId)
        {
            if (DataType.IsNullOrEmpty(parentId))
                parentId = this._RootParent;

            string jsonStr = this.GetJsonFromDataTable(parentId);
            if (this.FormatMode == 0)
                return String.Format("[{0}]", jsonStr);
            else
                return String.Format("[{0}{1}]", jsonStr, "\r\n");
        }
        private string Name(string name)
        {
            return String.Format("{0}{1}{0}", this.QuoteMark, name);
        }
        private string GetJsonFromDataTable(String parentId)
        {
            String jsonStr = String.Empty;
            string con = null;
            if (DataType.IsNullOrEmpty(parentId))
                con = String.Format("{0} is null", this._ParentIdName);
            else
                con = String.Format("{0} = '{1}'", this._ParentIdName, parentId);
            DataRow[] drs = this._DT.Select(con);
            foreach (DataRow dr in drs)
            {
                if (!DataType.IsNullOrEmpty(jsonStr)) jsonStr += ",";
                String drJson = this.GetJsonFromDataRow(dr);
                jsonStr += drJson;
            }
            return jsonStr;
        }
        private string RepeatString(String strToRepeat, int repeatCount)
        {
            string strTemp = String.Empty;
            for (int i = 0; i < repeatCount; i++) strTemp += strToRepeat;
            return strTemp;
        }
        private string GetJsonFromDataRow(DataRow drT)
        {
            string jsonStr = String.Empty;
            if (true)
            {
                for (int i = 0; i < this._DT.Columns.Count; i++)
                {
                    //自已加的标志，过滤掉
                    if (this._DT.Columns[i].ColumnName == LEVEL_COLUMN_NAME) continue;
                    //正常字段
                    if (!DataType.IsNullOrEmpty(jsonStr)) jsonStr += ",";
                    if (this.FormatMode == 2)
                        if (this.FormatMode == 2)
                        {
                            jsonStr += "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME]) + 1) + this.GetJsonFromKeyValue(this._DT.Columns[i].ColumnName, drT[i]);
                        }
                        else
                        {
                            jsonStr += this.GetJsonFromKeyValue(this._DT.Columns[i].ColumnName, drT[i]);
                        }
                }
            }
            if (true)
            {
                DataRow[] drs = this._DT.Select(String.Format("{0}='{1}'", this._ParentIdName, drT[this._IdName]));
                string childJson = String.Empty;
                foreach (DataRow dr in drs)
                {
                    if (!DataType.IsNullOrEmpty(childJson)) childJson += ",";
                    String drJson = this.GetJsonFromDataRow(dr);
                    childJson += drJson;
                }
                if (!DataType.IsNullOrEmpty(childJson))
                {
                    if (this.FormatMode == 0)
                        childJson = this.Name(this._ChildNodeName) + ":[" + childJson + "]";
                    else if (this.FormatMode == 1)
                        childJson = this.Name(this._ChildNodeName) + ":[" + childJson + "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME]) + 1) + "]";
                    else
                        childJson = this.Name(this._ChildNodeName) + ":"
                            + "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME]) + 1) + "["
                            + childJson
                            + "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME]) + 1) + "]";
                }
                else
                {
                    childJson = this.Name(this._ChildNodeName) + ": null";
                }
                if (!DataType.IsNullOrEmpty(jsonStr) && !DataType.IsNullOrEmpty(childJson)) jsonStr += ",";
                //行格式化
                if (this.FormatMode == 2)
                {
                    childJson = "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME]) + 1) + childJson;
                }
                jsonStr += childJson;
            }
            if (this.FormatMode == 0)
                return "{" + jsonStr + "}";
            else if (this.FormatMode == 1)
            {
                return "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME])) + "{" + jsonStr + "}";
            }
            else //if (this.FormatMode == 2)
                return "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME])) + "{"
                    + jsonStr
                    + "\r\n" + this.RepeatString("\t", Convert.ToInt32(drT[LEVEL_COLUMN_NAME])) + "}";
        }
        private string GetJsonFromKeyValue(string key, object value)
        {
            return String.Format("{2}{0}{2}:{1}", key, this.GetJsonFromValue(key, value), this.QuoteMark);
        }
        private string GetJsonFromValue(string key, object value)
        {
            if (value == null || value == Convert.DBNull)
            {
                return "null";
            }
            else
            {
                return String.Format(this.GetQuoteFormat(key, Convert.ToString(value)), this.SerializeJsonValue(key, Convert.ToString(value)));
            }
            //else if (!this.SetAllQuote && (this.isNumber(key, value) || this.IsBool(key, value)))
            //{
            //    return String.Format("{0}", value);
            //}
            //else if (this.SetAllQuote)
            //{
            //    if (this.NoQuoteCols != null)
            //    {
            //        List<String> ls = new List<string>(NoQuoteCols);
            //        if (ls.Contains(key))
            //        {
            //            return this.SerializeJsonValue(key, String.Format("{0}", value));
            //        }
            //        else
            //        {
            //            return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //        }
            //    }
            //    else
            //    {
            //        return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //    }
            //}
            //else
            //{
            //    return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //}
        }
        private string GetQuoteFormat(string key, string value)
        {
            if (!this.SetAllQuote && (this.isNumber(key, value) || this.IsBool(key, value)))
            {
                return "{0}";
            }
            else if (this.SetAllQuote)
            {
                if (this.NoQuoteCols != null)
                {
                    List<String> ls = new List<string>(NoQuoteCols);
                    if (ls.Contains(key))
                    {
                        return "{0}";
                    }
                    else
                    {
                        return this.QuoteMark + "{0}" + this.QuoteMark;
                    }
                }
                else
                {
                    return this.QuoteMark + "{0}" + this.QuoteMark;
                }
            }
            else
            {
                return this.QuoteMark + "{0}" + this.QuoteMark;
            }
        }
        private string SerializeJsonValue(string key, string value)
        {
            if (this.EscapeMethod == 0)
                return this.SerializeJsonValue_Replace(value);
            else
                return this.SerializeJsonValue_MJ(value);
        }
        private string SerializeJsonValue_MJ(string value)
        {
            return Uri.EscapeDataString(value);
        }
        private string SerializeJsonValue_Replace(string value)
        {
            if (DataType.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            string temstr;
            temstr = value;
            temstr = temstr.Replace("{", "｛").Replace("}", "｝")
                .Replace(":", "：")
                .Replace(",", "，")
                .Replace("[", "【")
                .Replace("]", "】")
                .Replace(";", "；")
                .Replace("\n", "<br/>")
                .Replace("\r", "");

            temstr = temstr.Replace("\t", "   ");
            temstr = temstr.Replace("'", "\'");
            temstr = temstr.Replace(@"\", @"\\");
            temstr = temstr.Replace("\"", "\"\"");
            return temstr;
        }
        private bool isNumber(string key, object value)
        {
            //首先要值要是数字(多余)，因为数据类型设置后，若不为数字会出错。若用做它用，可打开
            //double d = 0;
            //bool bln = double.TryParse(Convert.ToString(value), out d);
            //if (!bln) return false;
            //再次数据类型要为数字
            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": break;
                case "DbType.Byte": b = true; break;
                case "DbType.Currency": b = true; break;
                case "DbType.Date": break;
                case "DbType.DateTime": break;
                case "DbType.DateTime2": break;
                case "DbType.DateTimeOffset": break;
                case "DbType.Decimal": b = true; break;
                case "DbType.Double": b = true; break;
                case "DbType.Guid": break;
                case "DbType.Int16": b = true; break;
                case "DbType.Int32": b = true; break;
                case "DbType.Int64": b = true; break;
                case "DbType.Object": break;
                case "DbType.SByte": b = true; break;
                case "DbType.Single": b = true; break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": break;
                case "DbType.UInt16": b = true; break;
                case "DbType.UInt32": b = true; break;
                case "DbType.UInt64": b = true; break;
                case "DbType.VarNumeric": b = true; break;
                case "DbType.Xml": break;
            }
            return b;
        }
        private bool IsBool(string key, object value)
        {
            bool bR = false;
            bool bln = bool.TryParse(Convert.ToString(value), out bR);
            if (!bln) return false;

            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": b = true; break;
                case "DbType.Byte": break;
                case "DbType.Currency": break;
                case "DbType.Date": break;
                case "DbType.DateTime": break;
                case "DbType.DateTime2": break;
                case "DbType.DateTimeOffset": break;
                case "DbType.Decimal": break;
                case "DbType.Double": break;
                case "DbType.Guid": break;
                case "DbType.Int16": break;
                case "DbType.Int32": break;
                case "DbType.Int64": break;
                case "DbType.Object": break;
                case "DbType.SByte": break;
                case "DbType.Single": break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": break;
                case "DbType.UInt16": break;
                case "DbType.UInt32": break;
                case "DbType.UInt64": break;
                case "DbType.VarNumeric": break;
                case "DbType.Xml": break;
            }
            return b;
        }
        private bool IsDateTime(string key, object value)
        {
            DateTime d = DateTime.Now;
            bool bln = DateTime.TryParse(Convert.ToString(value), out d);
            if (!bln) return false;

            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": break;
                case "DbType.Byte": break;
                case "DbType.Currency": break;
                case "DbType.Date": b = true; break;
                case "DbType.DateTime": b = true; break;
                case "DbType.DateTime2": b = true; break;
                case "DbType.DateTimeOffset": b = true; break;
                case "DbType.Decimal": break;
                case "DbType.Double": break;
                case "DbType.Guid": break;
                case "DbType.Int16": break;
                case "DbType.Int32": break;
                case "DbType.Int64": break;
                case "DbType.Object": break;
                case "DbType.SByte": break;
                case "DbType.Single": break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": b = true; break;
                case "DbType.UInt16": break;
                case "DbType.UInt32": break;
                case "DbType.UInt64": break;
                case "DbType.VarNumeric": break;
                case "DbType.Xml": break;
            }
            return b;
        }
    }
    public class DataTableLine2Json
    {
        private System.Data.DataTable _DT;
        public string QuoteMark = "\'"; //可改为单引号 

        /// <summary>
        /// 如果为true,则所有字段按字符串处理，两边都会加上QuoteMark.
        /// 如果为false,则会检测字段类型及值类型，如果为数字或布尔型，则两边不加QuoteMark.
        /// </summary>
        public bool SetAllQuote = false;
        /// <summary>
        /// 格式化模式
        /// 0-不格式化
        /// 1-对象行格式化，每个对象一行
        /// 2-属性格式化，每个属性一行
        /// </summary>
        public int FormatMode = 1;

        /// <summary>
        /// 不加Quote的字段列表，当AllQuote为true时起作用。
        /// </summary>
        public string[] NoQuoteCols = null;
        /// <summary>
        /// 序列化字段
        /// </summary>
        public string[] EscapeCols = null;
        /// <summary>
        /// 序列化方法 0-人工替换，1-Microsoft.Jscript序列化
        /// </summary>
        public int EscapeMethod = 0;
        public DataTableLine2Json(System.Data.DataTable dt)
        {
            this._DT = dt;

            this.CheckInit();
        }
        private void CheckInit()
        {
            if (this._DT == null)
            {
                throw new ApplicationException("输入变量不能为空！");
            }
        }
        /// <summary>
        /// 唯一对外接口
        /// </summary>
        /// <returns></returns>
        public string ToJsonStr(string con)
        {
            string jsonStr = this.GetJsonFromDataTable(con);
            if (this.FormatMode == 0)
                return String.Format("[{0}]", jsonStr);
            else
                return String.Format("[{0}{1}]", jsonStr, "\r\n");
        }
        private string Name(string name)
        {
            return String.Format("{0}{1}{0}", this.QuoteMark, name);
        }
        private string GetJsonFromDataTable(string con)
        {
            String jsonStr = String.Empty;
            DataRow[] drs = this._DT.Select(con);
            foreach (DataRow dr in drs)
            {
                if (!DataType.IsNullOrEmpty(jsonStr)) jsonStr += ",";
                String drJson = this.GetJsonFromDataRow(dr);
                jsonStr += drJson;
            }
            return jsonStr;
        }
        private string RepeatString(String strToRepeat, int repeatCount)
        {
            string strTemp = String.Empty;
            for (int i = 0; i < repeatCount; i++) strTemp += strToRepeat;
            return strTemp;
        }
        private string GetJsonFromDataRow(DataRow drT)
        {
            string jsonStr = String.Empty;
            if (true)
            {
                for (int i = 0; i < this._DT.Columns.Count; i++)
                {
                    //正常字段
                    if (!DataType.IsNullOrEmpty(jsonStr)) jsonStr += ",";
                    if (this.FormatMode == 2)
                        if (this.FormatMode == 2)
                        {
                            jsonStr += "\r\n" + this.RepeatString("\t", 1) + this.GetJsonFromKeyValue(this._DT.Columns[i].ColumnName, drT[i]);
                        }
                        else
                        {
                            jsonStr += this.GetJsonFromKeyValue(this._DT.Columns[i].ColumnName, drT[i]);
                        }
                }
            }
            if (this.FormatMode == 0)
                return "{" + jsonStr + "}";
            else if (this.FormatMode == 1)
            {
                return "\r\n" + this.RepeatString("\t", 1) + "{" + jsonStr + "}";
            }
            else //if (this.FormatMode == 2)
                return "\r\n" + this.RepeatString("\t", 1) + "{"
                    + jsonStr
                    + "\r\n" + this.RepeatString("\t", 1) + "}";
        }
        private string GetJsonFromKeyValue(string key, object value)
        {
            return String.Format("{2}{0}{2}:{1}", key, this.GetJsonFromValue(key, value), this.QuoteMark);
        }
        private string GetJsonFromValue(string key, object value)
        {
            if (value == null || value == Convert.DBNull)
            {
                return "null";
            }
            else
            {
                return String.Format(this.GetQuoteFormat(key, Convert.ToString(value)), this.SerializeJsonValue(key, Convert.ToString(value)));
            }
            //else if (!this.SetAllQuote && (this.isNumber(key, value) || this.IsBool(key, value)))
            //{
            //    return String.Format("{0}", value);
            //}
            //else if (this.SetAllQuote)
            //{
            //    if (this.NoQuoteCols != null)
            //    {
            //        List<String> ls = new List<string>(NoQuoteCols);
            //        if (ls.Contains(key))
            //        {
            //            return this.SerializeJsonValue(key, String.Format("{0}", value));
            //        }
            //        else
            //        {
            //            return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //        }
            //    }
            //    else
            //    {
            //        return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //    }
            //}
            //else
            //{
            //    return this.QuoteMark + this.SerializeJsonValue(key, String.Format("{0}", value)) + this.QuoteMark;
            //}
        }
        private string GetQuoteFormat(string key, string value)
        {
            if (!this.SetAllQuote && (this.isNumber(key, value) || this.IsBool(key, value)))
            {
                return "{0}";
            }
            else if (this.SetAllQuote)
            {
                if (this.NoQuoteCols != null)
                {
                    List<String> ls = new List<string>(NoQuoteCols);
                    if (ls.Contains(key))
                    {
                        return "{0}";
                    }
                    else
                    {
                        return this.QuoteMark + "{0}" + this.QuoteMark;
                    }
                }
                else
                {
                    return this.QuoteMark + "{0}" + this.QuoteMark;
                }
            }
            else
            {
                return this.QuoteMark + "{0}" + this.QuoteMark;
            }
        }
        private string SerializeJsonValue(string key, string value)
        {
            if (this.EscapeMethod == 0)
                return this.SerializeJsonValue_Replace(value);
            else
                return this.SerializeJsonValue_MJ(value);
        }
        private string SerializeJsonValue_MJ(string value)
        {
            return Uri.EscapeDataString(value);
        }
        private string SerializeJsonValue_Replace(string value)
        {
            if (DataType.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            string temstr;
            temstr = value;
            temstr = temstr.Replace("{", "｛").Replace("}", "｝")
                .Replace(":", "：")
                .Replace(",", "，")
                .Replace("[", "【")
                .Replace("]", "】")
                .Replace(";", "；")
                .Replace("\n", "<br/>")
                .Replace("\r", "");

            temstr = temstr.Replace("\t", "   ");
            temstr = temstr.Replace("'", "\'");
            temstr = temstr.Replace(@"\", @"\\");
            temstr = temstr.Replace("\"", "\"\"");
            return temstr;
        }
        private bool isNumber(string key, object value)
        {
            //首先要值要是数字(多余)，因为数据类型设置后，若不为数字会出错。若用做它用，可打开
            //double d = 0;
            //bool bln = double.TryParse(Convert.ToString(value), out d);
            //if (!bln) return false;
            //再次数据类型要为数字
            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": break;
                case "DbType.Byte": b = true; break;
                case "DbType.Currency": b = true; break;
                case "DbType.Date": break;
                case "DbType.DateTime": break;
                case "DbType.DateTime2": break;
                case "DbType.DateTimeOffset": break;
                case "DbType.Decimal": b = true; break;
                case "DbType.Double": b = true; break;
                case "DbType.Guid": break;
                case "DbType.Int16": b = true; break;
                case "DbType.Int32": b = true; break;
                case "DbType.Int64": b = true; break;
                case "DbType.Object": break;
                case "DbType.SByte": b = true; break;
                case "DbType.Single": b = true; break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": break;
                case "DbType.UInt16": b = true; break;
                case "DbType.UInt32": b = true; break;
                case "DbType.UInt64": b = true; break;
                case "DbType.VarNumeric": b = true; break;
                case "DbType.Xml": break;
            }
            return b;
        }
        private bool IsBool(string key, object value)
        {
            bool bR = false;
            bool bln = bool.TryParse(Convert.ToString(value), out bR);
            if (!bln) return false;

            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": b = true; break;
                case "DbType.Byte": break;
                case "DbType.Currency": break;
                case "DbType.Date": break;
                case "DbType.DateTime": break;
                case "DbType.DateTime2": break;
                case "DbType.DateTimeOffset": break;
                case "DbType.Decimal": break;
                case "DbType.Double": break;
                case "DbType.Guid": break;
                case "DbType.Int16": break;
                case "DbType.Int32": break;
                case "DbType.Int64": break;
                case "DbType.Object": break;
                case "DbType.SByte": break;
                case "DbType.Single": break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": break;
                case "DbType.UInt16": break;
                case "DbType.UInt32": break;
                case "DbType.UInt64": break;
                case "DbType.VarNumeric": break;
                case "DbType.Xml": break;
            }
            return b;
        }
        private bool IsDateTime(string key, object value)
        {
            DateTime d = DateTime.Now;
            bool bln = DateTime.TryParse(Convert.ToString(value), out d);
            if (!bln) return false;

            bool b = false;
            switch (this._DT.Columns[key].DataType.FullName)
            {
                case "DbType.AnsiString": break;
                case "DbType.AnsiStringFixedLength": break;
                case "DbType.Binary": break;
                case "DbType.Boolean": break;
                case "DbType.Byte": break;
                case "DbType.Currency": break;
                case "DbType.Date": b = true; break;
                case "DbType.DateTime": b = true; break;
                case "DbType.DateTime2": b = true; break;
                case "DbType.DateTimeOffset": b = true; break;
                case "DbType.Decimal": break;
                case "DbType.Double": break;
                case "DbType.Guid": break;
                case "DbType.Int16": break;
                case "DbType.Int32": break;
                case "DbType.Int64": break;
                case "DbType.Object": break;
                case "DbType.SByte": break;
                case "DbType.Single": break;
                case "DbType.String": break;
                case "DbType.StringFixedLength": break;
                case "DbType.Time": b = true; break;
                case "DbType.UInt16": break;
                case "DbType.UInt32": break;
                case "DbType.UInt64": break;
                case "DbType.VarNumeric": break;
                case "DbType.Xml": break;
            }
            return b;
        }
    }
}