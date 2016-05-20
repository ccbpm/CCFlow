using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 数据源类型
    /// </summary>
    public class DBSrcAttr
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
        /// 数据源类型
        /// </summary>
        public const string SrcType = "SrcType";
        /// <summary>
        /// 数据源url
        /// </summary>
        public const string Url = "Url";
    }
    /// <summary>
    /// 数据源类型
    /// </summary>
	public class DBSrc:XmlEnNoName
    {
        #region 属性
        /// <summary>
        /// 数据源类型
        /// </summary>
        public string SrcType
        {
            get
            {
                return this.GetValStringByKey(DBSrcAttr.SrcType);
            }
        }
        /// <summary>
        /// 数据源类型URL
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStringByKey(DBSrcAttr.Url);
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 数据源类型
		/// </summary>
		public DBSrc()
		{
		}
		/// <summary>
		/// 数据源类型s
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new DBSrcs();
			}
		}
		#endregion
	}
	/// <summary>
    /// 数据源类型s
	/// </summary>
	public class DBSrcs:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public DBSrcs() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new DBSrc();
			}
		}
        /// <summary>
        /// XML文件位置.
        /// </summary>
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\DataUser\\XML\\DBSrc.xml";
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
