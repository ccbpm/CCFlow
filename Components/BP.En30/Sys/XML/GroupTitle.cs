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
    public class GroupTitleAttr
    {
        public const string For = "For";
        public const string Key = "Key";
        public const string Title = "Title";
    }
	/// <summary>
	/// GroupTitle 的摘要说明。
	/// 考核过错行为的数据元素
	/// 1，它是 GroupTitle 的一个明细。
	/// 2，它表示一个数据元素。
	/// </summary>
    public class GroupTitle : XmlEn
    {
        #region 属性
        /// <summary>
        /// 选择这个属性时间需要的条件
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStringByKey(GroupTitleAttr.Title);
            }
        }
        public string For
        {
            get
            {
                return this.GetValStringByKey(GroupTitleAttr.For);
            }
        }
        public string Key
        {
            get
            {
                return this.GetValStringByKey(GroupTitleAttr.Key);
            }
        }
        #endregion

        #region 构造
        public GroupTitle()
        {
        }
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new GroupTitles();
            }
        }
        #endregion
    }
	/// <summary>
	/// 
	/// </summary>
	public class GroupTitles:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public GroupTitles(){}
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new GroupTitle();
			}
		}
		public override string File
		{
			get
			{
				return SystemConfig.PathOfXML+"\\Ens\\GroupTitle.xml";
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
