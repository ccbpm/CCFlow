using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 语言属性
    /// </summary>
    public class LangAttr
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
    /// 语言
    /// </summary>
	public class Lang:XmlEnNoName
	{
		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public Lang()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new Langs();
			}
		}
		#endregion
	}
	/// <summary>
	/// 语言s
	/// </summary>
	public class Langs:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public Langs() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new Lang();
			}
		}
        /// <summary>
        /// XML文件位置.
        /// </summary>
		public override string File
		{
			get
			{
                return SystemConfig.CCFlowAppPath + "WF\\Style\\Tools.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "Lang";
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
