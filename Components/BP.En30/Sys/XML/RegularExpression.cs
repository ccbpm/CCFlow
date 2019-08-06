using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;


namespace BP.Sys.XML
{
    /// <summary>
    ///  RegularExpression 正则表达模版
    /// </summary>
	public class RegularExpression:XmlEn
	{
		#region 属性
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        public string Note
        {
            get
            {
                return this.GetValStringByKey("Note");
            }
        }
        public string ForCtrl
        {
            get
            {
                return this.GetValStringByKey("ForCtrl");
            }
        }
        public string Exp
        {
            get
            {
                return this.GetValStringByKey("Exp");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
        public RegularExpression()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new RegularExpressions();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class RegularExpressions:XmlEns
	{
		#region 构造
		/// <summary>
        /// 正则表达模版
		/// </summary>
        public RegularExpressions() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new RegularExpression();
			}
		}
        /// <summary>
        /// 文件路径
        /// </summary>
		public override string File
		{
			get
			{
                return SystemConfig.PathOfData + "\\XML\\RegularExpression.xml";
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
