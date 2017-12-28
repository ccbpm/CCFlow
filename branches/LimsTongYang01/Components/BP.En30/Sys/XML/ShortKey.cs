using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys.XML;


namespace BP.Sys.XML
{
	/// <summary>
	/// 属性
	/// </summary>
	public class ShortKeyAttr
	{
		/// <summary>
		/// 过错行为
		/// </summary>
		public const string No="No";
		/// <summary>
		/// Name
		/// </summary>
		public const string Name="Name";
		/// <summary>
		/// 表达式
		/// </summary>
		public const string URL="URL";
		/// <summary>
		/// 描述
		/// </summary>
		public const string DFor="DFor";
		/// <summary>
		/// 图片
		/// </summary>
		public const string Img="Img";
        /// <summary>
        /// Target
        /// </summary>
        public const string Target = "Target";
	}
	/// <summary>
	/// ShortKey 的摘要说明。
	/// 考核过错行为的数据元素
	/// 1，它是 ShortKey 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
	public class ShortKey:XmlEn
	{
		#region 属性
		public string No
		{
			get
			{
				return this.GetValStringByKey(ShortKeyAttr.No);
			}
		}
		/// <summary>
		/// 数据
		/// </summary>
		public string DFor
		{
			get
			{
				return this.GetValStringByKey(ShortKeyAttr.DFor);
			}
		}
		public string Name
		{
			get
			{
                return this.GetValStringByKey(BP.Web.WebUser.SysLang );
			}
		}
		/// <summary>
		/// URL
		/// </summary>
		public string URL
		{
			get
			{
				return this.GetValStringByKey(ShortKeyAttr.URL);
			}
		}
		/// <summary>
		/// 图片
		/// </summary>
		public string Img
		{
			get
			{
				return this.GetValStringByKey(ShortKeyAttr.Img);
			}
		}
        public string Target
        {
            get
            {
                return this.GetValStringByKey(ShortKeyAttr.Target);
            }
        }
		#endregion

		#region 构造
		public ShortKey()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new ShortKeys();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class ShortKeys:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public ShortKeys(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new ShortKey();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Menu.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "ShortKey";
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
