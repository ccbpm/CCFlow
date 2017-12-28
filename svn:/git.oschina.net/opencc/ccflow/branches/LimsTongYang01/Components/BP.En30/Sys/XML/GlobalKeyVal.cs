using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys.XML;



namespace BP.Sys.XML
{
    public class GlobalKeyValList
    {
        /// <summary>
        /// 系列属性s
        /// </summary>
        public const string Subjection = "Subjection";
    }
	/// <summary>
	/// 属性
	/// </summary>
	public class GlobalKeyValAttr
	{
		/// <summary>
		/// 高度
		/// </summary>
        public const string Key = "Key";
        /// <summary>
        /// 是否接受文件数据
        /// </summary>
        public const string Val = "Val";
	}
	/// <summary>
	/// GlobalKeyVal 的摘要说明。
	/// 考核过错行为的数据元素
	/// 1，它是 GlobalKeyVal 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
	public class GlobalKeyVal:XmlEn
	{
		#region 属性
        public string Key
		{
			get
			{
                return this.GetValStringByKey(GlobalKeyValAttr.Key);
			}
		}
        public string Val
		{
			get
			{
                return this.GetValStringByKey(GlobalKeyValAttr.Val);
			}
		}
		#endregion

		#region 构造
		public GlobalKeyVal()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new GlobalKeyVals();
			}
		}
		#endregion
	}
	/// <summary>
	/// 全局的Key val 类型的变量设置
	/// </summary>
	public class GlobalKeyVals:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public GlobalKeyVals()
		{
		}
	 
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new GlobalKeyVal();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Ens\\GlobalKeyVal.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Item";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null;
			}
		}
		#endregion
	}
}
