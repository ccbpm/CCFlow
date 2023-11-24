using System;
using System.Data;
using System.Collections;
using BP.DA;
using Newtonsoft.Json;

namespace BP.En
{
    /// <summary>
    /// Row 的摘要说明。
    /// 用来处理一条记录
    /// </summary>
    public class Row : Hashtable
    {
        public override object Clone()
        {
            // @wanglu
            // deep clone 
            string serialized = JsonConvert.SerializeObject(this);
            Row newRow = JsonConvert.DeserializeObject<Row>(serialized);
            return newRow;
            /** 
            Row row = new Row();
            foreach (object key in this.Keys)
            {
                row.Add(key, this[key]);
            }

            return row; */
        }
        public Row() : base(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true))
        {
        }
        public static int attrSta = 0;
        /// <summary>
        /// 初始化数据.
        /// </summary>
        /// <param name="attrs"></param>
        public void LoadAttrs(Attrs attrs)
        {
            this.Clear();
            foreach (Attr attr in attrs)
            {
                switch (attr.MyDataType)
                {
                    case DataType.AppInt:
                        if (attr.ItIsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, int.Parse(attr.DefaultVal.ToString()));
                        break;
                    case DataType.AppFloat:
                        if (attr.ItIsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, float.Parse(attr.DefaultVal.ToString()));
                        break;
                    case DataType.AppMoney:
                        if (attr.ItIsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, decimal.Parse(attr.DefaultVal.ToString()));
                        break;
                    case DataType.AppDouble:
                        if (attr.ItIsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, double.Parse(attr.DefaultVal.ToString()));
                        break;
                    default:
                        this.Add(attr.Key, attr.DefaultVal);
                        break;
                }
            }
        }
        /// <summary>
        /// 装载属性
        /// </summary>
        /// <param name="attrs"></param>
        public void LoadDataTable(DataTable dt, DataRow dr)
        {
            this.Clear();
            foreach (DataColumn dc in dt.Columns)
            {
                this.Add(dc.ColumnName, dr[dc.ColumnName]);
            }
        }

        /// <summary>
        /// 设置一个值by key . 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public void SetValByKey(string key, object val)
        {


            if (key == null)
                return;

            // warning 需要商榷，不增加就会导致赋值错误.
            if (this.ContainsKey(key) == false)
            {
                this.Add(key, val);
                return;
            }

            if (val == null)
            {
                this[key] = val;
                return;
            }

            if (val.GetType() == typeof(TypeCode))
                this[key] = (int)val;
            else
                this[key] = val;


        }

        public string GetValStrByKey(string key)
        {
            return this[key] as string;
        }

        public object GetValByKey(string key)
        {
            return this[key];
        }
    }
}
