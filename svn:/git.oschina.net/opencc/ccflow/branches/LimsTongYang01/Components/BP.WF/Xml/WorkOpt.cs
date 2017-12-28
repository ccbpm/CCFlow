using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 工作选项
    /// </summary>
	public class WorkOptXml:XmlEnNoName
    {
        #region 属性.
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public new string CSS
        {
            get
            {
                return this.GetValStringByKey("CSS");
            }
        }

        public string URL
        {
            get
            {
                return this.GetValStringByKey("URL");
            }
        }
        #endregion 属性.

        #region 构造
        /// <summary>
		/// 节点扩展信息
		/// </summary>
		public WorkOptXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new WorkOptXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 工作选项s
	/// </summary>
	public class WorkOptXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 工作选项s
		/// </summary>
        public WorkOptXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new WorkOptXml();
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
				return "WorkOpt";
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
