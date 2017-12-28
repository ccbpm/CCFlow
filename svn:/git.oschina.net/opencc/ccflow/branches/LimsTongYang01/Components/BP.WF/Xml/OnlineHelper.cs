using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 在线帮助
    /// </summary>
	public class OnlineHelper:XmlEnNoName
	{
		#region 构造
		/// <summary>
        /// 在线帮助
		/// </summary>
		public OnlineHelper()
		{
		}
		/// <summary>
        /// 在线帮助
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new OnlineHelpers();
			}
		}
		#endregion
	}
	/// <summary>
    /// 在线帮助s
	/// </summary>
	public class OnlineHelpers:XmlEns
	{
		#region 构造
		/// <summary>
        /// 在线帮助s
		/// </summary>
        public OnlineHelpers() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new OnlineHelper();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfWebApp + "\\WF\\OnlineHepler\\";
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
