using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;


namespace BP.Web.Port.Xml
{
    public class ItemAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// HelpFile
        /// </summary>
        public const string HelpFile = "HelpFile";
    }
	/// <summary>
	/// 
	/// </summary>
	public class Item:XmlEn
	{
		#region 属性
        public string HelpFile
        {
            get
            {
                return this.GetValStringByKey(ItemAttr.HelpFile);
            }
        }
		/// <summary>
		/// 编号
		/// </summary>
		public string No
		{
			get
			{
				return this.GetValStringByKey(ItemAttr.No);
			}
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			get
			{
                return this.GetValStringByKey(BP.Web.WebUser.SysLang );
			}
		}
		#endregion

		#region 构造
		public Item()
		{
		}
		/// <summary>
		/// 编号
		/// </summary>
		/// <param name="no"></param>
		public Item(string no)
		{

		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new Items();
			}
		}
		#endregion

		#region  公共方法
		 
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class Items:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
		public Items(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new Item();
			}
		}
		public override string File
		{
			get
			{
				return  SystemConfig.PathOfXML+"\\Menu.xml";
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
				return null; //new BP.ZF1.Items();
			}
		}
		#endregion
		 
	}
}
