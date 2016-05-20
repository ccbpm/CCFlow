using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;

namespace BP.WF.XML
{
    /// <summary>
    /// 皮肤
    /// </summary>
	public class CondTypeXml:XmlEnNoName
	{
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
        public CondTypeXml()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
                return new CondTypeXmls();
			}
		}
		#endregion
	}
	/// <summary>
    /// 皮肤s
	/// </summary>
	public class CondTypeXmls:XmlEns
	{
		#region 构造
		/// <summary>
        /// 皮肤s
		/// </summary>
        public CondTypeXmls() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new CondTypeXml();
			}
		}
        public override string File
        {
            get
            {
                return SystemConfig.PathOfData + "\\Xml\\WFAdmin.xml";
            }
        }
		/// <summary>
		/// 物理表名
		/// </summary>
		public override string TableName
		{
			get
			{
				return "CondType";
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
