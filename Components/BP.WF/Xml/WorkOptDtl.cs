using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 工作明细选项
    /// </summary>
	public class WorkOptDtlXml:XmlEnNoName
	{
        /// <summary>
        /// 名称
        /// </summary>
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        /// <summary>
        /// 超链接
        /// </summary>
        public string URL
        {
            get
            {
                return this.GetValStringByKey("URL");
            }
        }

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public WorkOptDtlXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new WorkOptDtlXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 工作明细选项s
	/// </summary>
	public class WorkOptDtlXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 工作明细选项s
		/// </summary>
        public WorkOptDtlXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new WorkOptDtlXml();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\WF\\Style\\Tools.xml";
			}
		}
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "WorkOptDtl";
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
