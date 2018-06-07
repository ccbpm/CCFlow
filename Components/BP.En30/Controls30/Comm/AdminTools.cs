using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;


namespace BP.Web.Port.Xml
{
	public class AdminToolAttr
	{
		/// <summary>
		/// 编号
		/// </summary>
		public const string ICON="ICON";
		/// <summary>
		/// 名称
		/// </summary>
		public const string Name="Name";
		/// <summary>
		/// Url
		/// </summary>
		public const string Url="Url";
		/// <summary>
		/// DESC
		/// </summary>
		public const string DESC="DESC";
		/// <summary>
		/// 
		/// </summary>
		public const string Enable="Enable";
	}
	public class AdminTool:XmlEn
	{
		#region 属性
		public string Enable
		{
			get
			{
				return this.GetValStringByKey(AdminToolAttr.Enable);
			}
		}
		public string Url
		{
			get
			{
				return this.GetValStringByKey(AdminToolAttr.Url);
			}
		}
		public string DESC
		{
			get
			{
				return this.GetValStringByKey(AdminToolAttr.DESC);
			}
		}
		/// <summary>
		/// 编号
		/// </summary>
		public string ICON
		{
			get
			{
				return this.GetValStringByKey(AdminToolAttr.ICON);
			}
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get
			{
				return this.GetValStringByKey(AdminToolAttr.Name);
			}
		}
		#endregion

		#region 构造
		public AdminTool()
		{
		}
		/// <summary>
		/// 编号
		/// </summary>
		/// <param name="no"></param>
		public AdminTool(string no)
		{

		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new AdminTools();
			}
		}
		#endregion

		#region  公共方法
		 
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class AdminTools:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
		public AdminTools(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new AdminTool();
			}
		}
		public override string File
		{
			get
			{
				return  SystemConfig.PathOfXML+"\\AdminTools.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "AdminTool";
			}
		}
		public override Entities RefEns
		{
			get
			{
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
