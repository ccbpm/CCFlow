using System;
using System.Data;
using System.Collections;
using BP.DA;

namespace BP.En
{
	/// <summary>
	/// Row 的摘要说明。
	/// 用来处理一条记录
	/// </summary>
	public class Row : Hashtable
	{
        public Row():base(System.StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true))
        {
        }
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
                    case BP.DA.DataType.AppInt:
                        if (attr.IsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, int.Parse(attr.DefaultVal.ToString()));
                        break;
                    case BP.DA.DataType.AppFloat:
                        if (attr.IsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, float.Parse(attr.DefaultVal.ToString()));
                        break;
                    case BP.DA.DataType.AppMoney:
                        if (attr.IsNull)
                            this.Add(attr.Key, DBNull.Value);
                        else
                            this.Add(attr.Key, decimal.Parse(attr.DefaultVal.ToString()));
                        break;
                    case BP.DA.DataType.AppDouble:
                        if (attr.IsNull)
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
        /// LoadAttrs
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

            if (val == null )
            {
                this[key] = val;
                return;
            }

            if (val.GetType() == typeof(TypeCode))
                this[key] = (int)val;
            else
                this[key] = val;
        }


        public bool GetBoolenByKey(string key)
        {
            object obj = this[key];
            if (obj == null || DataType.IsNullOrEmpty(obj.ToString())==true || obj.ToString()=="0")
                return false;
            return true;
        }
        public object GetValByKey(string key)
        {
            return this[key];
            /*
            if (SystemConfig.IsDebug)
            {
                try
                {
                    return this[key];
                }
                catch(Exception ex)
                {
                    throw new Exception("@GetValByKey没有找到key="+key+"的属性Vale , 请确认Map 里面是否有此属性."+ex.Message);
                }
            }
            else
            {
                return this[key];
            }
            */
        }

	}
	/// <summary>
	/// row 集合
	/// </summary>
	public class Rows : System.Collections.CollectionBase
	{
		public Rows()
		{
		}
		public Row this[int index]
		{
			get 
			{
				return (Row)this.InnerList[index];
			}
		}	 
		/// <summary>
		/// 增加一个Row .
		/// </summary>
		/// <param name="r">row</param>
		public void Add(Row r)
		{
			this.InnerList.Add(r);
		}
	}
}
