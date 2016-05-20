using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys.XML;
using BP.Sys;


namespace BP.WF.XML
{
    /// <summary>
    /// sdk
    /// </summary>
	public class SDK:XmlEn
	{
		#region 属性
        public string No
        {
            get
            {
                return this.GetValStringByKey("DoWhat");
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey("Url");
            }
        }
		#endregion

		#region 构造
		/// <summary>
		/// 节点扩展信息
		/// </summary>
		public SDK()
		{
		}
		/// <summary>
		/// 获取一个实例
		/// </summary>
		public override XmlEns GetNewEntities
		{
			get
			{
				return new SDKs();
			}
		}
		#endregion
	}
	/// <summary>
	/// 
	/// </summary>
	public class SDKs:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核率的数据元素
		/// </summary>
        public SDKs() { }
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new SDK();
			}
		}
		public override string File
		{
			get
			{
                return SystemConfig.PathOfData + "\\XML\\SDK.xml";
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
				return null; //new BP.ZF1.AdminTools();
			}
		}
		#endregion
		 
	}
}
