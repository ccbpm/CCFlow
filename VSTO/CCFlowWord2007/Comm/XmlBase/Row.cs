using System;
using System.Data;
using System.Collections;
using BP.En;

namespace BP.En
{
	/// <summary>
	/// Row ��ժҪ˵����
	/// ��������һ����¼�Ĵ�ţ����⡣
	/// </summary>
	public class Row : Hashtable
	{
        /// <summary>
        /// 
        /// </summary>
		public Row()
		{

		}
		/// <summary>
		/// �����ݱ����������һ��(����xmlEn)
		/// </summary>
		/// <param name="dt"></param>
		public Row(DataTable dt, DataRow dr)
		{
			this.Clear();
			foreach( DataColumn dc in dt.Columns ) 
			{
				this.Add(dc.ColumnName, dr[dc.ColumnName] );
			}
		}

		/// <summary>
		/// ����һ��ֵby key . 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="val"></param>
		public void SetValByKey(string key, object val)
		{
			//this.Values[].Add(key,val);
			if (val.GetType()== typeof( TypeCode ) )
				this[key]=(int)val;
			else
				this[key]=val;
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
                    throw new Exception("@GetValByKeyû���ҵ�key="+key+"������Vale , ��ȷ��Map �����Ƿ��д�����."+ex.Message);
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
	/// row ����
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
		/// ����һ��Row .
		/// </summary>
		/// <param name="r">row</param>
		public void Add(Row r)
		{
			this.InnerList.Add(r);
		}
	}
}
