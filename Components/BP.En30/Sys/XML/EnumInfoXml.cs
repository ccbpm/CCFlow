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
    public class EnumInfoXmlAttr
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
        /// 描述
        /// </summary>
        public const string Vals = "Vals";
    }
	/// <summary>
	/// EnumInfoXml 的摘要说明，属性的配置。
	/// </summary>
    public class EnumInfoXml : XmlEn
    {
        #region 属性
        public new string Name
        {
            get
            {
                return this.GetValStringByKey(BP.Web.WebUser.SysLang);
            }
        }
        public string Key
        {
            get
            {
                return this.GetValStringByKey("Key");
            }
        }
        /// <summary>
        /// Vals
        /// </summary>
        public string Vals
        {
            get
            {
                string str = BP.Web.WebUser.SysLang;
                str = "CH";
                return this.GetValStringByKey(str);
            }
        }
        #endregion

        #region 构造
        public EnumInfoXml()
        {
        }
        public EnumInfoXml(string key)
        {
            this.RetrieveByPK("Key", key);
        }
        
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new EnumInfoXmls();
            }
        }
        #endregion
    }
	/// <summary>
	/// 属性集合
	/// </summary>
	public class EnumInfoXmls:XmlEns
	{
		#region 构造
		/// <summary>
		/// 考核过错行为的数据元素
		/// </summary>
		public EnumInfoXmls()
		{
		}
	 
		#endregion

		#region 重写基类属性或方法。
		/// <summary>
		/// 得到它的 Entity 
		/// </summary>
		public override XmlEn GetNewEntity
		{
			get
			{
				return new EnumInfoXml();
			}
		}
        public override string File
        {
            get
            {
                return SystemConfig.PathOfXML + "\\Enum\\";
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
