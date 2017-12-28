using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.WF.XML
{
    /// <summary>
    /// 公文左边谓词
    /// </summary>
    public class GovWordLeftAttr
    {
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
    }
    /// <summary>
    /// 公文左边谓词
    /// </summary>
	public class GovWordLeft:XmlEnNoName
    {

        #region 构造
        /// <summary>
        /// 公文左边谓词
		/// </summary>
		public GovWordLeft()
		{
		}
		/// <summary>
		/// 公文左边谓词s
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new GovWordLefts();
			}
		}
		#endregion
	}
	/// <summary>
    /// 公文左边谓词s
	/// </summary>
	public class GovWordLefts:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public GovWordLefts() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new GovWordLeft();
			}
		}
        /// <summary>
        /// XML文件位置.
        /// </summary>
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\WF\\Data\\XML\\XmlDB.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
                return "GovWordLeft";
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
