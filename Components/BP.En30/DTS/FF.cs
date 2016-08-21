using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls ; 

namespace BP.DTS
{ 
	/// <summary>
	/// 属性
	/// </summary>
	public class FF
	{
		/// <summary>
		/// 从字段
		/// </summary>
		public string FromField=null;
		/// <summary>
		/// 到字段
		/// </summary>
		public string ToField=null;
		/// <summary>
		/// 数据源类型
		/// </summary>
		public int DataType=1;//DataType.AppString;		
		/// <summary>
		/// 是否是主键
		/// </summary>
		public bool IsPK=false;
		public FF()
		{
		}
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="from">从</param>
		/// <param name="to">到</param>
		/// <param name="datatype">数据类型</param>
		/// <param name="isPk">是否PK</param>
		public FF(string from,string to,int datatype, bool isPk)
		{
			this.FromField=from;
			this.ToField=to;
			this.DataType=datatype;
			this.IsPK=isPk;
		}
	}
	/// <summary>
	/// 属性集合
	/// </summary>
	[Serializable]
	public class FFs: CollectionBase
	{
        public int PKCount
        {
            get
            {
                int i = 0;
                foreach (FF ff in this)
                {
                    if (ff.IsPK)
                        i++;
                }
                if (i == 0)
                    throw new Exception("没有设置PK. 请检查map 错误.");
                return i;
            }
        }
		/// <summary>
		/// 属性集合
		/// </summary>
		public FFs(){}
		/// <summary>
		/// 加入一个属性
		/// </summary>
		/// <param name="attr"></param>
		public void Add(FF ff)
		{
			this.InnerList.Add(ff);
		}
		/// <summary>
		/// 增加一个数据影射
		/// </summary>
		/// <param name="fromF"></param>
		/// <param name="toF"></param>
		/// <param name="dataType"></param>
		/// <param name="isPk"></param>
		public void Add(string fromF,string toF, int dataType, bool isPk)
		{
			this.Add( new FF( fromF , toF , dataType ,isPk ) );
		}
		/// <summary>
		/// 根据索引访问集合内的元素Attr。
		/// </summary>
		public FF this[int index]
		{			
			get
			{	
				return (FF)this.InnerList[index];
			}
		}
	}	
}
